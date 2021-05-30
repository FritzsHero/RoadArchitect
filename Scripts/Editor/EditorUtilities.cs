#if UNITY_EDITOR
using UnityEngine;


namespace RoadArchitect
{
    public class EditorUtilities
    {
        /// <summary> Opens the loacally stored manual </summary>
        public static void OpenOfflineManual()
        {
            Application.OpenURL(System.Environment.CurrentDirectory.Replace(@"\", "/") + "/" + RoadEditorUtility.GetBasePath() + "/RoadArchitectManual.htm");
        }


        /// <summary> Loads the _texture from _path if necessary </summary>
        public static void LoadTexture<T>(ref T _texture, string _path) where T : Texture
        {
            _texture = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(_path);
        }


        public static void DrawLine(float _spacing = 4f, float _size = 1f)
        {
            //Horizontal bar
            GUILayout.Space(_spacing);
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(_size));
            GUILayout.Space(_spacing);
        }
    }
}
#endif
