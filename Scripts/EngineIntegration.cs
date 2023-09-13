using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace RoadArchitect
{
    public static class EngineIntegration
    {
        /// <summary>
        /// Unity works with forward slash so we convert
        /// If you want to implement your own Asset creation and saving you should just use finalName
        /// or what applies to the engine you use
        /// </summary>
        public static string GetUnityFilePath(string _filePath)
        {
            string path;
            path = _filePath.Replace(Path.DirectorySeparatorChar, '/');
            path = path.Replace(Path.AltDirectorySeparatorChar, '/');
            return path;
        }


        public static void DisplayDialog(string _title, string _message, string _ok)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog(_title, _message, _ok);
            #endif
        }


        public static void RepaintAllSceneView()
        {
            #if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
            #endif
        }


        public static void SetActiveGameObject(GameObject _object, bool _isSelected = true)
        {
            #if UNITY_EDITOR
            if (_isSelected)
            {
                UnityEditor.Selection.activeGameObject = _object;
            }
            #endif
        }


        public static GameObject InstantiatePrefab(GameObject _gameObject)
        {
            #if UNITY_EDITOR
            // Instantiate prefab instead of object
            return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(_gameObject);
            #else
            // Line to instantiate the object instead of an prefab
            return GameObject.Instantiate(_gameObject);
            #endif
        }


        public static GameObject GetObjectFromSource(GameObject _object)
        {
            #if UNITY_EDITOR
            #if UNITY_2018_2_OR_NEWER
            return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(_object);
            #else
            return UnityEditor.PrefabUtility.GetPrefabParent(_object);
            #endif
            #else
            //TODO: Check if this is correct to do
            //Return your object
            return null;
            #endif
        }


        public static void SetSelectedRenderState(Renderer _renderer, bool _isDrawingWireframe)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetSelectedRenderState(_renderer, _isDrawingWireframe ? EditorSelectedRenderState.Wireframe : EditorSelectedRenderState.Hidden);
            #endif
        }


        public static void CreateAsset(Object _object, string _path)
        {
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(_object, _path);
            #endif
        }


        public static string GetAssetPath<T>(T _asset) where T : UnityEngine.Object
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(_asset);
            #else
            //TODO: Check if this is correct to do
            return "";
            #endif
        }


        public static void GenerateSecondaryUVSet(Mesh _mesh)
        {
            #if UNITY_EDITOR
            UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            #endif
        }


        public static void SaveAssets()
        {
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }


        public static void SetStaticEditorFlags(GameObject _object)
        {
            #if UNITY_EDITOR
            #if UNITY_2019_2_OR_NEWER
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(_object, UnityEditor.StaticEditorFlags.ContributeGI);
            #else
            UnityEditor.GameObjectUtility.SetStaticEditorFlags(_object, UnityEditor.StaticEditorFlags.LightmapStatic);
            #endif
            #endif
        }


        public static void ClearProgressBar()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
            #endif
        }


        public static T LoadAssetFromPath<T>(string _path) where T: UnityEngine.Object
        {
            //RoadEditorUtility.GetBasePath() + "/Prefabs/Signs/StopSignAllway.prefab"
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path);
            #else
            // Get your prefab in your runtime build
            return null;
            #endif
        }


        public static void RegisterUndo(Object _objectToUndo, string _name)
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(_objectToUndo, _name);
            #endif
        }
    }
}
