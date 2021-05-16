#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


namespace RoadArchitect
{
    public class HelpWindow : EditorWindow
    {
        private void OnGUI()
        {
            EditorStyles.label.wordWrap = true;
            EditorStyles.miniLabel.wordWrap = true;

            EditorGUILayout.LabelField("Road Architect Help", EditorStyles.boldLabel);
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Please visit the online manual for help.");
            GUILayout.Space(4f);


            if (GUILayout.Button("Click here to open online manual", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/FritzsHero/RoadArchitect/wiki");
            }
            EditorGUILayout.LabelField("https://github.com/FritzsHero/RoadArchitect/wiki");

            if (GUILayout.Button("Click here to open offline manual", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                EditorUtilities.OpenOfflineManual();
            }


            GUILayout.Space(12f);


            EditorGUILayout.LabelField("Please visit us or reach out to us on Github (links below) with any questions or comments.");
            EditorGUILayout.LabelField("If you encounter Bugs or have a Feature Suggestion, you can submit them on the following sites:");


            GUILayout.Space(12f);

            if (GUILayout.Button("RoadArchitect Repository", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/FritzsHero/RoadArchitect");
            }
            EditorGUILayout.LabelField("https://github.com/FritzsHero/RoadArchitect");

            GUILayout.Space(4f);

            if (GUILayout.Button("RoadArchitect Issues", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/FritzsHero/RoadArchitect/issues");
            }
            EditorGUILayout.LabelField("https://github.com/FritzsHero/RoadArchitect/issues");
            GUILayout.Space(12f);
        }


        public void Initialize()
        {
            Rect rect = new Rect(340, 170, 400, 400);
            position = rect;
            Show();
            titleContent.text = "Help Info";
        }
    }
}
#endif
