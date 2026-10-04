#region "Imports"
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion


namespace RoadArchitect
{
    public static class Storage
    {
        public const char UnityPathSeparator = '/';

        private const string ManualFileName = "RoadArchitectManual.htm";
        private static string baseDirectory = "";
        private static string assemblyFilePath = "";


        /// <summary> Returns the relative path of the RoadArchitect folder. For Example: Assets/RoadArchitect </summary>
        public static string GetRoadArchitectDirectory()
        {
            #if UNITY_EDITOR
            if (baseDirectory != "" && assemblyFilePath != "")
            {
                if (File.Exists(assemblyFilePath))
                {
                    return baseDirectory;
                }
            }

            string currentDirectory = Environment.CurrentDirectory;
            #if UNITY_2019_3_OR_NEWER
            string[] assemblyDefinitions = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");
            for (int index = 0; index < assemblyDefinitions.Length; index++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assemblyDefinitions[index]);
                if (Path.GetFileName(path) == "RoadArchitect.asmdef")
                {
                    assemblyFilePath = path;
                    baseDirectory = Path.GetRelativePath(currentDirectory, Path.GetDirectoryName(path));
                    return baseDirectory;
                }
            }
            #else
            string[] assemblyDefinitionPaths = Directory.GetFiles(
                dataPath,
                "RoadArchitect.asmdef",
                SearchOption.AllDirectories);
            if (assemblyDefinitionPaths.Length > 0)
            {
                assemblyFilePath = assemblyDefinitionPaths[0];
                baseDirectory = Path.GetRelativePath(currentDirectory, Path.GetDirectoryName(assemblyDefinitionPaths[0]));
                return baseDirectory;
            }
            #endif

            throw new Exception("Could not locate RoadArchitect.asmdef under the project's Assets folder.");
            #else
            return "";
            #endif
        }


        /// <summary> Returns the relative base of the RoadArchitect folder with OS compatible directory separator </summary>
        public static string GetRoadArchitectDirectoryCompatibleWithOS()
        {
            string baseDirectory = GetRoadArchitectDirectory();
            if('/' != Path.DirectorySeparatorChar && '/' != Path.AltDirectorySeparatorChar)
            {
                return baseDirectory.Replace('/', Path.DirectorySeparatorChar);
            }
            return baseDirectory;
        }


        /// <summary> Returns the absolute path of the RoadArchitect folder with OS compatible directory separator </summary>
        public static string GetAbsoluteRoadArchitectDirectory()
        {
            string relativeDirectory = GetRoadArchitectDirectoryCompatibleWithOS();
            return Path.Combine(Environment.CurrentDirectory, relativeDirectory);
        }


        #region Terrain History
        /// <summary> Returns the path where Terrain History is saved. This is the RoadArchitect folder outside the Assets. </summary>
        public static string GetTerrainHistoryDirectory()
        {
            string path = Path.Combine(Environment.CurrentDirectory, "RoadArchitect", "TerrainHistory");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary> Returns the path where Terrain History of this Road is saved. This is the RoadArchitect folder outside the Assets. </summary>
        private static string GetTerrainHistoryDirectory(Road _road)
        {
            return Path.Combine(GetTerrainHistoryDirectory(), GetRoadTerrainHistoryFileName(ref _road));
        }


        // http://forum.unity3d.com/threads/32647-C-Sharp-Binary-Serialization
        // http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html
        /// <summary> This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile. Do not change </summary> 
        public sealed class VersionDeserializationBinder : SerializationBinder
        {
            public override System.Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
                {
                    System.Type typeToDeserialize = null;
                    assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                    // The following line of code returns the type.
                    typeToDeserialize = System.Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
                    return typeToDeserialize;
                }
                return null;
            }
        }


        /// <summary> Saves the Terrain History to disk </summary>
        public static bool SaveTerrainHistory(List<TerrainHistoryMaker> _obj, Road _road)
        {
            string path = GetTerrainHistoryDirectory(_road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return false;
            }
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = CreateFormatter();
                bformatter.Serialize(stream, _obj);
                stream.Flush();
                _road.TerrainHistoryByteSize = (stream.Length * 0.001f).ToString("n0") + " kb";
            }
            return true;
        }


        /// <summary> Deletes the Terrain History from disk </summary>
        public static void DeleteTerrainHistory(Road _road)
        {
            string path = GetTerrainHistoryDirectory(_road);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        /// <summary> Loads the Terrain History from disk </summary>
        public static List<TerrainHistoryMaker> LoadTerrainHistory(Road _road)
        {
            string path = GetTerrainHistoryDirectory(_road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return null;
            }
            if (!File.Exists(path))
            {
                return null;
            }
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bFormatter = CreateFormatter();
                return (List<TerrainHistoryMaker>)bFormatter.Deserialize(stream);
            }
        }


        private static BinaryFormatter CreateFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Binder = new VersionDeserializationBinder();
            return formatter;
        }


        /// <summary> Generates the Terrain History file name </summary>
        private static string GetRoadTerrainHistoryFileName(ref Road _road)
        {
            string sceneName;

            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");
            string roadName = _road.roadSystem.transform.name.Replace("RoadArchitectSystem", "RAS") + "-" + _road.transform.name;
            return sceneName + "-" + roadName + ".th";
        }
        #endregion


        public static string GetManualPath()
        {
            return Path.Combine(GetAbsoluteRoadArchitectDirectory(), ManualFileName);
        }


        public static string GetExtrudedSplineObjectLibraryFile(string _name)
        {
            return Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library", "ESO" + _name + ".rao");
        }


        public static string GetEdgeObjectLibraryFile(string _name)
        {
            return Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library", "EOM" + _name + ".rao");
        }


        /// <summary> Returns relative Assets/RoadArchitect/Editor/Library with OS compatible directory separator </summary>
        public static string GetLibraryDirectory()
        {
            string path = Path.Combine(GetRoadArchitectDirectoryCompatibleWithOS(), "Editor", "Library");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
