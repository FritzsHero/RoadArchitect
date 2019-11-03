#region "Imports"
using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion


namespace GSD.Roads
{
    public static class GSDRoadUtilityEditor
    {
        private static readonly string[] validFolders =
        {
            "Assets/RoadArchitect",
            "Assets/RoadArchitect-master",
            "Assets/Resources/RoadArchitect",
            "Assets/Resources/RoadArchitect-master"
        };


        public static string GetBasePath()
        {
            // TODO this might break in future versions of Unity
#if UNITY_EDITOR
            foreach (string folder in validFolders)
            {
                if (Directory.Exists(Environment.CurrentDirectory + "/" + folder))
                {
                    return folder;
                }
            }
            throw new System.Exception("RoadArchitect must be placed in one of the valid folders, read the top of this script");
#else
            return "";
#endif
        }


        // Refactor with above function is in work
        public static string GetRoadArchitectApplicationPath()
        {
            if (Directory.Exists(Application.dataPath + "/RoadArchitect"))
            {
                return Application.dataPath + "/RoadArchitect";
            }
            else if (Directory.Exists(Application.dataPath + "/RoadArchitect-master"))
            {
                return Application.dataPath + "/RoadArchitect-master";
            }
            else if (Directory.Exists(Application.dataPath + "/Resources/RoadArchitect"))
            {
                return Application.dataPath + "/Resources/RoadArchitect";
            }
            else if (Directory.Exists(Application.dataPath + "/Resources/RoadArchitect-master"))
            {
                return Application.dataPath + "/Resources/RoadArchitect-master";
            }
            else if (Directory.Exists(Application.dataPath + "/Tools/RoadArchitect"))
            {
                return Application.dataPath + "/Tools/RoadArchitect";
            }
            else if (Directory.Exists(Application.dataPath + "/Tools/RoadArchitect-master"))
            {
                return Application.dataPath + "/Tools/RoadArchitect-master";
            }
            else
            {
                throw new System.Exception("RoadArchitect must be placed in one of the valid folders, read the top of this script");
            }
        }


#if UNITY_EDITOR
        public static void SetRoadMaterial(string _assetPath, MeshRenderer _MR, string _assetPath2 = "")
        {
            Material material2;

            Material[] tMats;
            Material material = (Material) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(Material));
            if (_assetPath2.Length > 0)
            {
                material2 = (Material) AssetDatabase.LoadAssetAtPath(_assetPath2, typeof(Material));
                tMats = new Material[2];
                tMats[0] = material;
                tMats[1] = material2;
            }
            else
            {
                tMats = new Material[1];
                tMats[0] = material;
            }

            _MR.sharedMaterials = tMats;
        }


        public static Material LoadMaterial(string _assetPath)
        {
            return (Material) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(Material));
        }


        public static PhysicMaterial LoadPhysicsMaterial(string _assetPath)
        {
            return (PhysicMaterial) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(PhysicMaterial));
        }
#endif
    }
}