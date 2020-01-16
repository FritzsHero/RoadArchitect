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
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");
            }
            EditorGUILayout.LabelField("https://github.com/MicroGSD/RoadArchitect/wiki");

            if (GUILayout.Button("Click here to open offline manual", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                EditorUtilities.OpenOfflineManual();
            }


            GUILayout.Space(12f);


            EditorGUILayout.LabelField("Please visit us or reach out to us on Github (links below) with any questions or comments.");
            EditorGUILayout.LabelField("You can also check out the following Sites, for an Beta of RoadArchitect:");
            EditorGUILayout.LabelField("If you encounter Bugs or have a Feature Suggestion, you can submit them on one of the following sites:");


            GUILayout.Space(12f);


            EditorGUILayout.LabelField("RoadArchitect 1.7 and above", EditorStyles.boldLabel);

            GUILayout.Space(4f);

            if (GUILayout.Button("FritzsHero's RoadArchitect Repository", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/FritzsHero/RoadArchitect/tree/master");
            }
            EditorGUILayout.LabelField("https://github.com/FritzsHero/RoadArchitect/tree/master");

            GUILayout.Space(4f);

            if (GUILayout.Button("FritzsHero's RoadArchitect Issues", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/FritzsHero/RoadArchitect/issues");
            }
            EditorGUILayout.LabelField("https://github.com/FritzsHero/RoadArchitect/issues");


            GUILayout.Space(12f);


            EditorGUILayout.LabelField("RoadArchitect 1.6 and older", EditorStyles.boldLabel);

            GUILayout.Space(4f);

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("MicroGSD's RoadArchitect Repository", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/tree/master");
            }
            EditorGUILayout.LabelField("https://github.com/MicroGSD/RoadArchitect/tree/master");

            EditorGUILayout.EndVertical();

            GUILayout.Space(4f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("MicroGSD's RoadArchitect Issues", EditorStyles.toolbarButton, GUILayout.Width(310f)))
            {
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/issues");
            }
            EditorGUILayout.LabelField("https://github.com/MicroGSD/RoadArchitect/issues");

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }


        public void Initialize()
        {
            Rect rect = new Rect(340, 170, 500, 500);
            position = rect;
            Show();
            titleContent.text = "Help Info";
        }
    }
}
#endif