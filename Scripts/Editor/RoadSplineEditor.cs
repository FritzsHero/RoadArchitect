#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(RoadSpline))]
    [MovedFrom(true, sourceClassName: "SplineCEditor")]
    public class RoadSplineEditor : Editor
    {
        private RoadSpline spline;
        private int browseNode = 0;


        private void OnEnable()
        {
            spline = (RoadSpline)target;
        }


        public override void OnInspectorGUI()
        {
            #region NodeBrowser
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Browse to node:", EditorStyles.boldLabel);
            browseNode = EditorGUILayout.IntField(browseNode);
            if (GUILayout.Button("Browse"))
            {
                if (browseNode < spline.nodes.Count)
                {
                    Selection.objects = new Object[1] { spline.nodes[browseNode] };
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }
    }
}
#endif
