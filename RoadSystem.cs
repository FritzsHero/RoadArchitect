#region "Imports"
using UnityEngine;
#endregion


namespace RoadArchitect
{
    public class RoadSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        #region "Vars"
        [UnityEngine.Serialization.FormerlySerializedAs("opt_bMultithreading")]
        public bool isMultithreaded = true;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_bSaveMeshes")]
        public bool isSavingMeshes = false;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_bAllowRoadUpdates")]
        public bool isAllowingRoadUpdates = true;

        public Camera editorPlayCamera = null;
        #endregion


        /// <summary> Adds a new road to this RoadSystem </summary>
        public GameObject AddRoad(bool _isForceSelected = false)
        {
            Road[] allRoadObj = GetComponentsInChildren<Road>();
            int newRoadNumber = (allRoadObj.Length + 1);

            //Road:
            GameObject roadObj = new GameObject("Road" + newRoadNumber.ToString());
            UnityEditor.Undo.RegisterCreatedObjectUndo(roadObj, "Created road");
            roadObj.transform.parent = transform;
            Road road = roadObj.AddComponent<Road>();

            //Spline:
            GameObject splineObj = new GameObject("Spline");
            splineObj.transform.parent = road.transform;
            road.spline = splineObj.AddComponent<SplineC>();
            road.spline.splineRoot = splineObj;
            road.spline.road = road;
            road.GSDSplineObj = splineObj;
            road.GSDRS = this;
            RoadArchitect.RootUtils.SetupUniqueIdentifier(ref road.UID);

            road.ConstructRoad_ResetTerrainHistory();

            if (_isForceSelected)
            {
                UnityEditor.Selection.activeGameObject = roadObj;
            }

            return roadObj;
        }


        /// <summary> Sets the editorPlayCamera to the first camera, if it is the only camera in this scene </summary>
        public void EditorCameraSetSingle()
        {
            if (editorPlayCamera == null)
            {
                Camera[] editorCams = (Camera[])GameObject.FindObjectsOfType(typeof(Camera));
                if (editorCams != null && editorCams.Length == 1)
                {
                    editorPlayCamera = editorCams[0];
                }
            }
        }


        /// <summary> Updates all roads of this RoadSystem </summary>
        public void UpdateAllRoads()
        {
            Road[] allRoadObjs = GetComponentsInChildren<Road>();
            int roadCount = allRoadObjs.Length;
            SplineC[] piggys = null;
            if (roadCount > 1)
            {
                piggys = new SplineC[roadCount];
                for (int i = 0; i < roadCount; i++)
                {
                    piggys[i] = allRoadObjs[i].spline;
                }
            }

            Road road = allRoadObjs[0];
            if (piggys != null && piggys.Length > 0)
            {
                road.PiggyBacks = piggys;
            }
            road.UpdateRoad();
        }


        //Workaround for submission rules:
        public void UpdateAllRoadsMultiThreadedOption()
        {
            Road[] allRoadObjs = (Road[])GetComponentsInChildren<Road>();
            int roadCount = allRoadObjs.Length;
            Road road = null;
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
            Road[] allRoadObjs = (Road[])GetComponentsInChildren<Road>();
            int roadsCount = allRoadObjs.Length;
            Road road = null;
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
}