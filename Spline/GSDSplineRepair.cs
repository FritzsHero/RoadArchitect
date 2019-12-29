#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using RoadArchitect;
#endregion


[ExecuteInEditMode]
public class GSDSplineRepair : MonoBehaviour
{
    public void RemoveLastNode()
    {
        GSDSplineC spline = GetComponent<GSDSplineC>();
        spline.nodes.RemoveAt(spline.nodes.Count - 1);
        spline.road.UpdateRoad();
    }
}


[CustomEditor(typeof(GSDSplineRepair))]
public class GSDSplineRepairEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GSDSplineRepair repair = (GSDSplineRepair) target;
        if (GUILayout.Button("Remove last node"))
        {
            repair.RemoveLastNode();
        }
    }
}
#endif