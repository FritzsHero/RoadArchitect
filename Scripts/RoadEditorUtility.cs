#region "Imports"
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion


namespace RoadArchitect
{
    public static class RoadEditorUtility
    {
        private static string basePath = "";
        private static string assemblyFilePath = "";

        /// <summary> Returns the relative path of the RoadArchitect folder. For Example: Assets/RoadArchitect </summary>
        public static string GetBasePath()
        {
            #if UNITY_EDITOR
            if (basePath != "" && assemblyFilePath != "")
            {
                if (File.Exists(assemblyFilePath))
                {
                    return basePath;
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
                    basePath = Path.GetRelativePath(currentDirectory, Path.GetDirectoryName(path));
                    return basePath;
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
                basePath = Path.GetRelativePath(currentDirectory, Path.GetDirectoryName(assemblyDefinitionPaths[0]));
                return basePath;
            }
            #endif

            throw new Exception("Could not locate RoadArchitect.asmdef under the project's Assets folder.");
            #else
            return "";
            #endif
        }


        /// <summary> Returns the relative base of the RoadArchitect folder with OS compatible directory separator </summary>
        public static string GetBasePathForIO()
        {
            string basePath = GetBasePath();
            if('/' != Path.DirectorySeparatorChar && '/' != Path.AltDirectorySeparatorChar)
            {
                return basePath.Replace('/', Path.DirectorySeparatorChar);
            }
            return basePath;
        }


        /// <summary> Loads _assetPath materials and applies them to _MR.sharedMaterials </summary>
        public static void SetRoadMaterial(string _assetPath, MeshRenderer _MR, string _assetPath2 = "")
        {
            Material material;
            Material material2;
            Material[] tMats;

            material = LoadMaterial(_assetPath);
            
            if (_assetPath2.Length > 0)
            {
                material2 = LoadMaterial(_assetPath2);

                tMats = new Material[2];
                tMats[1] = material2;
            }
            else
            {
                tMats = new Material[1];
            }

            tMats[0] = material;

            _MR.sharedMaterials = tMats;
        }


        /// <summary> Returns the Material from _assetPath </summary>
        public static Material LoadMaterial(string _assetPath)
        {
            return EngineIntegration.LoadAssetFromPath<Material>(_assetPath);
        }


        /// <summary> Returns the PhysicsMaterial from _assetPath </summary>
        #if UNITY_6000_0_OR_NEWER
        public static PhysicsMaterial LoadPhysicsMaterial(string _assetPath)
        {
            return EngineIntegration.LoadAssetFromPath<PhysicsMaterial>(_assetPath);
        }
        #else
        public static PhysicMaterial LoadPhysicsMaterial(string _assetPath)
        {
            return EngineIntegration.LoadAssetFromPath<PhysicMaterial>(_assetPath);
        }
        #endif
    }
}
