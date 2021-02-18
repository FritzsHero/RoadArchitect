#region "Imports"
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
#endregion


namespace RoadArchitect
{
    public static class TerrainHistoryUtility
    {
        //http://forum.unity3d.com/threads/32647-C-Sharp-Binary-Serialization
        //http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html

        // === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
        // Do not change this
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
        public static void SaveTerrainHistory(List<TerrainHistoryMaker> _obj, Road _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return;
            }
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Binder = new VersionDeserializationBinder();
            bformatter.Serialize(stream, _obj);
            _road.TerrainHistoryByteSize = (stream.Length * 0.001f).ToString("n0") + " kb";
            stream.Close();
        }


        /// <summary> Deletes the Terrain History from disk </summary>
        public static void DeleteTerrainHistory(Road _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        /// <summary> Loads the Terrain History from disk </summary>
        public static List<TerrainHistoryMaker> LoadTerrainHistory(Road _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return null;
            }
            if (!File.Exists(path))
            {
                return null;
            }
            List<TerrainHistoryMaker> result;
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Binder = new VersionDeserializationBinder();

            result = (List<TerrainHistoryMaker>)bFormatter.Deserialize(stream);

            stream.Close();
            return result;
        }


        /// <summary> Generates the Terrain History file name </summary>
        private static string GetRoadTHFilename(ref Road _road)
        {
            string sceneName;
            
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");
            string roadName = _road.roadSystem.transform.name.Replace("RoadArchitectSystem", "RAS") + "-" + _road.transform.name;
            return sceneName + "-" + roadName + ".th";
        }


        /// <summary> Returns the path to the RoadArchitect folder where Terrain History is saved </summary>
        public static string GetDirBase()
        {
            return UnityEngine.Application.dataPath.Replace("/Assets", "/RoadArchitect/");
        }


        /// <summary> Returns the path where Terrain History is saved </summary>
        public static string GetTHDir()
        {
            string path = GetDirBase() + "TerrainHistory/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary> Checks if RoadArchitect folder exists </summary>
        public static string CheckRoadArchitectDirectory()
        {
            string path = GetDirBase();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (Directory.Exists(path))
            {
                return path + "/";
            }
            else
            {
                return "";
            }
        }


        /// <summary> Returns RoadArchitect/TerrainHistory path or empty </summary>
        public static string CheckNonAssetDirTH()
        {
            CheckRoadArchitectDirectory();

            string path = GetTHDir();
            if (Directory.Exists(path))
            {
                return path;
            }
            else
            {
                return "";
            }
        }
    }
}
