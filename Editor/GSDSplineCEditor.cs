#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDSplineC))]
public class GSDSplineCEditor : Editor
{
    protected GSDSplineC spline { get { return (GSDSplineC) target; } }
    private int browseNode = 0;


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
#endif