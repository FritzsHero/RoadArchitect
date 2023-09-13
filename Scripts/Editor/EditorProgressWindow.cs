#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
using UnityEngine;
#endregion


namespace RoadArchitect
{
    /// <summary> Used for progress information for other areas of RA. </summary>
    public class EditorProgressWindow : EditorWindow
    {
        private float seconds = 10.0f;
        private float startValue = 0f;
        private float progress = 0f;


        private static void Init()
        {
            EditorProgressWindow window = (EditorProgressWindow)EditorWindow.GetWindow(typeof(EditorProgressWindow));
            window.Show();
        }


        private void OnGUI()
        {
            seconds = EditorGUILayout.FloatField("Time to wait:", seconds);
            if (GUILayout.Button("Display bar"))
            {
                if (seconds < 1)
                {
                    Debug.LogError("Seconds should be bigger than 1");
                    return;
                }
                startValue = (float)EditorApplication.timeSinceStartup;
            }

            if (progress < seconds)
            {
                EditorUtility.DisplayProgressBar(
                "Simple Progress Bar",
                "Shows a progress bar for the given seconds",
                progress / seconds);
            }
            else
            {
                EngineIntegration.ClearProgressBar();
            }

            progress = (float)(EditorApplication.timeSinceStartup - startValue);
        }


        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
#endif
