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

        private const string UnifiedPackagePath = "Assets/RoadArchitect/";
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
            string[] assemblyDefinitionPaths = Directory.GetFiles(Application.dataPath, "RoadArchitect.asmdef", SearchOption.AllDirectories);
            if (assemblyDefinitionPaths.Length > 0)
            {
                assemblyFilePath = assemblyDefinitionPaths[0];
                #if UNITY_2017_1_OR_NEWER
                baseDirectory = Path.GetRelativePath(currentDirectory, Path.GetDirectoryName(assemblyDefinitionPaths[0]));
                #else
                var uri1 = new Uri(Path.GetFullPath(currentDirectory));
                var uri2 = new Uri(Path.GetFullPath(Path.GetDirectoryName(assemblyDefinitionPaths[0])));
                baseDirectory = uri1.MakeRelativeUri(uri2).ToString();
                #endif
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
            #if UNITY_6000_0_OR_NEWER
            string path = Path.Combine(Environment.CurrentDirectory, "RoadArchitect", "TerrainHistory");
            #else
            string path = Path.Combine(Path.Combine(Environment.CurrentDirectory, "RoadArchitect"), "TerrainHistory");
            #endif
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
            #if UNITY_6000_0_OR_NEWER
            return Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library", "ESO" + _name + ".rao");
            #else
            return Path.Combine(Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library"), "ESO" + _name + ".rao");
            #endif
        }


        public static string GetEdgeObjectLibraryFile(string _name)
        {
            #if UNITY_6000_0_OR_NEWER
            return Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library", "EOM" + _name + ".rao");
            #else
            return Path.Combine(Path.Combine(GetAbsoluteRoadArchitectDirectory(), "Library"), "EOM" + _name + ".rao");
            #endif
        }


        /// <summary> Returns relative Assets/RoadArchitect/Editor/Library with OS compatible directory separator </summary>
        public static string GetLibraryDirectory()
        {
            #if UNITY_6000_0_OR_NEWER
            string path = Path.Combine(GetRoadArchitectDirectoryCompatibleWithOS(), "Editor", "Library");
            #else
            string path = Path.Combine(Path.Combine(GetRoadArchitectDirectoryCompatibleWithOS(), "Editor"), "Library");
            #endif
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary> Stores package assets relative to the RoadArchitect folder and leaves external asset paths unchanged. </summary>
        public static string MakeAssetReference(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
            {
                return "";
            }

            string packagePath = GetRoadArchitectDirectory().Replace('\\', UnityPathSeparator);
            string assetPath = _assetPath.Replace('\\', UnityPathSeparator);
            if (assetPath.StartsWith(UnifiedPackagePath, StringComparison.OrdinalIgnoreCase))
            {
                return assetPath;
            }
            else if (assetPath.StartsWith(packagePath, StringComparison.OrdinalIgnoreCase))
            {
                return UnifiedPackagePath + assetPath.Replace(packagePath, "").TrimStart(UnityPathSeparator);
            }

            return _assetPath;
        }


        /// <summary> Resolves versioned package-relative references and preserves legacy Unity asset paths. </summary>
        public static string ResolveAssetReference(string _assetReference)
        {
            if (string.IsNullOrEmpty(_assetReference))
            {
                return _assetReference;
            }

            if (!_assetReference.StartsWith(UnifiedPackagePath, StringComparison.OrdinalIgnoreCase))
            {
                return _assetReference;
            }

            string packagePath = GetRoadArchitectDirectory().Replace('\\', UnityPathSeparator).TrimEnd(UnityPathSeparator);
            return packagePath + UnityPathSeparator + _assetReference.Replace(UnifiedPackagePath, "");
        }
    }
}
