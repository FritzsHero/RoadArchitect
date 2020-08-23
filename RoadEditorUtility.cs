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
            "Assets/RoadArchitect-master",
            "Assets/Resources/RoadArchitect",
            "Assets/Resources/RoadArchitect-master"
        };


        public static string GetBasePath()
        {
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


        public static Material LoadMaterial(string _assetPath)
        {
            #if UNITY_EDITOR
            return (Material) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(Material));
            #else
            // Here you can return your material loaded at runtime
            return null;
            #endif
        }


        public static PhysicMaterial LoadPhysicsMaterial(string _assetPath)
        {
            #if UNITY_EDITOR
            return (PhysicMaterial) AssetDatabase.LoadAssetAtPath(_assetPath, typeof(PhysicMaterial));
            #else
            // Here you can return your physics material loaded at runtime
            return null;
            #endif
        }
    }
}