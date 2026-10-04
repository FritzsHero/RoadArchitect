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
        [Obsolete("Use new Storage class. Storage.GetRoadArchitectDirectory()")]
        public static string GetBasePath()
        {
            return Storage.GetRoadArchitectDirectory();
        }


        [Obsolete("Use new Storage class. Storage.GetRoadArchitectDirectoryCompatibleWithOS()")]
        public static string GetBasePathForIO()
        {
            return Storage.GetRoadArchitectDirectoryCompatibleWithOS();
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
