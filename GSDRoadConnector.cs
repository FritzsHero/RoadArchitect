#region "Imports"
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using RoadArchitect;
#endif
#endregion


[ExecuteInEditMode]
public class GSDRoadConnector : MonoBehaviour
{
    public SplineN connectedNode;
    [HideInInspector]
    public OffRoadObject offRoadObject { get { return transform.parent.GetComponent<OffRoadObject>(); } }


#if UNITY_EDITOR
    #region "Gizmos"
    private void OnDrawGizmos()
    {
        Gizmos.color = OffRoadObject.offRoadNodeColor;
        Gizmos.DrawCube(transform.position + new Vector3(0f, 6f, 0f), new Vector3(2f, 11f, 2f));
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = OffRoadObject.offRoadSelectedNodeColor;
        Gizmos.DrawCube(transform.position + new Vector3(0f, 6.25f, 0f), new Vector3(3.5f, 12.5f, 3.5f));
    }
    #endregion


    public void ConnectToNode(SplineN _node)
    {
        Debug.Log("Would connect to " + _node);
        connectedNode = _node;
        connectedNode.transform.position = transform.position;
        connectedNode.spline.road.UpdateRoad();
    }


    // Update is called once per frame
    private void Update()
    {
        if (connectedNode != null)
        {
            if (offRoadObject == null)
            {
                Debug.LogError("Parent should have OffRoadObject component attached");
            }
            if (connectedNode.transform.position != transform.position)
            {
                connectedNode.transform.position = transform.position;
                connectedNode.spline.road.UpdateRoad();
            }
        }
    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(GSDRoadConnector))]
public class GSDRoadConnectorEditor : Editor
{
    public GSDRoadConnector connector { get { return (GSDRoadConnector) target; } }


    public override void OnInspectorGUI()
    {
        if (connector.connectedNode != null)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Off-road connection:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(connector.connectedNode.spline.road.name + " to " + connector.offRoadObject.name);
            if (GUILayout.Button("Break connection"))
            {
                connector.connectedNode = null;
            }
            EditorGUILayout.EndVertical();
        }
    }
}
#endif