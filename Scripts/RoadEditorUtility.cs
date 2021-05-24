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
            #if UNITY_EDITOR
            if ('/' != Path.DirectorySeparatorChar && '/' != Path.AltDirectorySeparatorChar)
            {
                foreach (string folder in validFolders)
                {
                    if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, folder.Replace('/', Path.DirectorySeparatorChar))))
                    {
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
                        return folder;
                    }
                }
            }

            throw new Exception("RoadArchitect must be placed in one of the valid folders. You can change these suppoted folders by modifiying validFolders on top of this script");
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
                basePath.Replace('/', Path.DirectorySeparatorChar);
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
            #if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<Material>(_assetPath);
            #else
            // Here you can return your material loaded at runtime
            return null;
            #endif
        }


        /// <summary> Returns the PhysicsMaterial from _assetPath </summary>
        public static PhysicMaterial LoadPhysicsMaterial(string _assetPath)
        {
            #if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<PhysicMaterial>(_assetPath);
            #else
            // Here you can return your physics material loaded at runtime
            return null;
            #endif
        }
    }
}
