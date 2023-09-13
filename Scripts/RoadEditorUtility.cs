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

        private static readonly string[] validFolders =
        {
            "Assets/RoadArchitect",
            "Assets/Tools/RoadArchitect",
            "Assets/Plugins/RoadArchitect",
            "Assets/Resources/RoadArchitect"
        };


        /// <summary> Returns the relative base of the RoadArchitect folder </summary>
        public static string GetBasePath()
        {
            if (basePath != "")
            {
                return basePath;
            }

            #if UNITY_EDITOR
            string path = AssetDatabase.GUIDToAssetPath("e9c7aa1199abeb64c82a4831b3c7286f");
            if (path != "" && path.Contains("RoadArchitect.asmdef"))
            {
                basePath = Path.GetDirectoryName(path);
                return basePath;
            }

            if ('/' != Path.DirectorySeparatorChar && '/' != Path.AltDirectorySeparatorChar)
            {
                foreach (string folder in validFolders)
                {
                    if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, folder.Replace('/', Path.DirectorySeparatorChar))))
                    {
                        basePath = folder;
                        return folder;
                    }
                }
            }
            else
            {
                foreach (string folder in validFolders)
                {
                    if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, folder)))
                    {
                        basePath = folder;
                        return folder;
                    }
                }
            }


            throw new Exception("GUID of RoadArchitect.asmdef was changed. " +
                "Alternatively RoadArchitect can be placed in one of the valid folders. " +
                "You can change these suppoted folders by modifiying validFolders on top of this script");
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
        public static PhysicMaterial LoadPhysicsMaterial(string _assetPath)
        {
            return EngineIntegration.LoadAssetFromPath<PhysicMaterial>(_assetPath);
        }
    }
}
