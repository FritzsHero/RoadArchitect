#region "Imports"
using UnityEngine;
#endregion


public class GSDRoadSystem : MonoBehaviour
{
#if UNITY_EDITOR
    #region "Vars"
    public static string assetBasePath = GSD.Roads.GSDRoadUtilityEditor.GetBasePath() + "";

    [UnityEngine.Serialization.FormerlySerializedAs("opt_bMultithreading")]
    public bool isMultithreaded = true;
    [UnityEngine.Serialization.FormerlySerializedAs("opt_bSaveMeshes")]
    public bool isSavingMeshes = false;
    [UnityEngine.Serialization.FormerlySerializedAs("opt_bAllowRoadUpdates")]
    public bool isAllowingRoadUpdates = true;

    public Camera editorPlayCamera = null;
    #endregion


    public GameObject AddRoad(bool _isForceSelected = false)
    {
        GSDRoad[] allRoadObj = GetComponentsInChildren<GSDRoad>();
        int newRoadNumber = (allRoadObj.Length + 1);

        //Road:
        GameObject roadObj = new GameObject("Road" + newRoadNumber.ToString());
        UnityEditor.Undo.RegisterCreatedObjectUndo(roadObj, "Created road");
        roadObj.transform.parent = transform;
        GSDRoad road = roadObj.AddComponent<GSDRoad>();

        //Spline:
        GameObject splineObj = new GameObject("Spline");
        splineObj.transform.parent = road.transform;
        road.spline = splineObj.AddComponent<GSDSplineC>();
        road.spline.splineRoot = splineObj;
        road.spline.road = road;
        road.GSDSplineObj = splineObj;
        road.GSDRS = this;
        road.SetupUniqueIdentifier();

        road.ConstructRoad_ResetTerrainHistory();

        if (_isForceSelected)
        {
            UnityEditor.Selection.activeGameObject = roadObj;
        }

        return roadObj;
    }


    public void EditorCameraSetSingle()
    {
        if (editorPlayCamera == null)
        {
            Camera[] editorCams = (Camera[]) GameObject.FindObjectsOfType(typeof(Camera));
            if (editorCams != null && editorCams.Length == 1)
            {
                editorPlayCamera = editorCams[0];
            }
        }
    }


    public void UpdateAllRoads()
    {
        GSDRoad[] allRoadObjs = GetComponentsInChildren<GSDRoad>();
        int roadCount = allRoadObjs.Length;
        GSDRoad road = null;
        GSDSplineC[] piggys = null;
        if (roadCount > 1)
        {
            piggys = new GSDSplineC[roadCount];
            for (int i = 0; i < roadCount; i++)
            {
                road = allRoadObjs[i];
                piggys[i] = road.spline;
            }
        }

        road = allRoadObjs[0];
        if (piggys != null && piggys.Length > 0)
        {
            road.PiggyBacks = piggys;
        }
        road.UpdateRoad();
    }


    //Workaround for submission rules:
    public void UpdateAllRoadsMultiThreadedOption()
    {
        GSDRoad[] allRoadObjs = (GSDRoad[]) GetComponentsInChildren<GSDRoad>();
        int roadCount = allRoadObjs.Length;
        GSDRoad road = null;
        for (int i = 0; i < roadCount; i++)
        {
            road = allRoadObjs[i];
            if (road != null)
            {
                road.isUsingMultithreading = isMultithreaded;
            }
        }
    }


    //Workaround for submission rules:
    public void UpdateAllRoadsSavingMeshesOption()
    {
        GSDRoad[] allRoadObjs = (GSDRoad[]) GetComponentsInChildren<GSDRoad>();
        int roadsCount = allRoadObjs.Length;
        GSDRoad road = null;
        for (int i = 0; i < roadsCount; i++)
        {
            road = allRoadObjs[i];
            if (road != null)
            {
                road.isSavingMeshes = isSavingMeshes;
            }
        }
    }
#endif
}