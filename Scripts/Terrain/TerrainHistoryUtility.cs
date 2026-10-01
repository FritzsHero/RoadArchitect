#region "Imports"
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
#endregion


namespace RoadArchitect
{
    public static class TerrainHistoryUtility
    {
        //http://forum.unity3d.com/threads/32647-C-Sharp-Binary-Serialization
        //http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html

        // === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
        // Do not change this
        [Obsolete("Use new Storage class. Storage.VersionDeserializationBinder")]
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
        [Obsolete("Use new Storage class. Storage.SaveTerrainHistory()")]
        public static bool SaveTerrainHistory(List<TerrainHistoryMaker> _obj, Road _road)
        {
            return Storage.SaveTerrainHistory(_obj, _road);
        }


        /// <summary> Deletes the Terrain History from disk </summary>
        [Obsolete("Use new Storage class. Storage.DeleteTerrainHistory()")]
        public static void DeleteTerrainHistory(Road _road)
        {
            Storage.DeleteTerrainHistory(_road);
        }


        /// <summary> Loads the Terrain History from disk </summary>
        [Obsolete("Use new Storage class. Storage.LoadTerrainHistory()")]
        public static List<TerrainHistoryMaker> LoadTerrainHistory(Road _road)
        {
            return Storage.LoadTerrainHistory(_road);
        }


        /// <summary> Returns the path to the RoadArchitect folder where Terrain History is saved </summary>
        [Obsolete("Use new Storage class. Storage.GetTerrainHistoryDirectory()")]
        public static string GetDirBase()
        {
            return Storage.GetTerrainHistoryDirectory();
        }


        /// <summary> Returns the path where Terrain History is saved </summary>
        [Obsolete("Use new Storage class. Storage.GetTerrainHistoryDirectory()")]
        public static string GetTHDir()
        {
            return Storage.GetTerrainHistoryDirectory();
        }


        /// <summary> Checks if RoadArchitect folder exists </summary>
        [Obsolete("Use new Storage class. Storage.CheckRoadArchitectDirectory()")]
        public static string CheckRoadArchitectDirectory()
        {
            return Storage.CheckRoadArchitectDirectory();
        }


        /// <summary> Returns RoadArchitect/TerrainHistory path or empty </summary>
        [Obsolete("Use new Storage class. Storage.CheckNonAssetDirTH()")]
        public static string CheckNonAssetDirTH()
        {
            return Storage.CheckNonAssetDirTH();
        }
    }
}
