#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using RoadArchitect;
#endregion


[ExecuteInEditMode]
public class SplineRepair : MonoBehaviour
{
    public void RemoveLastNode()
    {
        SplineC spline = GetComponent<SplineC>();
        spline.nodes.RemoveAt(spline.nodes.Count - 1);
        spline.road.UpdateRoad();
    }
}


[CustomEditor(typeof(SplineRepair))]
public class SplineRepairEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineRepair repair = (SplineRepair) target;
        if (GUILayout.Button("Remove last node"))
        {
            repair.RemoveLastNode();
        }
    }
}
#endif