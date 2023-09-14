#region "Imports"
using UnityEngine;
#endregion


namespace RoadArchitect
{
    public class RoadSystem : MonoBehaviour
    {
        #region "Vars"
        public bool isMultithreaded = true;
        public bool isSavingMeshes = false;
        public bool isAllowingRoadUpdates = true;

        public Camera editorPlayCamera = null;
        #endregion


        /// <summary> Adds a new road to this RoadSystem </summary>
        public GameObject AddRoad(bool _isForceSelected = false)
        {
            Road[] roads = GetComponentsInChildren<Road>();
            int newRoadNumber = (roads.Length + 1);

            //Road:
            GameObject roadObj = new GameObject("Road" + newRoadNumber.ToString());

            EngineIntegration.RegisterUndo(roadObj, "Created road");

            roadObj.transform.parent = transform;
            Road road = roadObj.AddComponent<Road>();

            //Spline:
            GameObject splineObj = new GameObject("Spline");
            splineObj.transform.parent = road.transform;
            road.spline = splineObj.AddComponent<SplineC>();
            road.spline.splineRoot = splineObj;
            road.spline.road = road;
            road.splineObject = splineObj;
            road.roadSystem = this;
            RoadArchitect.RootUtils.SetupUniqueIdentifier(ref road.UID);

            road.ResetTerrainHistory();

            EngineIntegration.SetActiveGameObject(roadObj, _isForceSelected);

            return roadObj;
        }


        /// <summary> Sets the editorPlayCamera to the first camera, if it is the only camera in this scene </summary>
        public void EditorCameraSetSingle()
        {
            if (editorPlayCamera == null)
            {
                Camera[] editorCams = GameObject.FindObjectsOfType<Camera>();
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
        /// <summary> Writes isMultithreaded into roads of this system </summary>
        public void UpdateAllRoadsMultiThreadedOption(bool _isMultithreaded)
        {
            Road[] roads = GetComponentsInChildren<Road>();
            int roadsCount = roads.Length;
            Road road = null;
            for (int i = 0; i < roadsCount; i++)
            {
                road = roads[i];
                if (road != null)
                {
                    road.isUsingMultithreading = _isMultithreaded;
                }
            }
        }


        //Workaround for submission rules:
        /// <summary> Writes isSavingMeshes into roads of this system </summary>
        public void UpdateAllRoadsSavingMeshesOption(bool _isSavingMeshes)
        {
            Road[] roads = GetComponentsInChildren<Road>();
            int roadsCount = roads.Length;
            Road road = null;
            for (int i = 0; i < roadsCount; i++)
            {
                road = roads[i];
                if (road != null)
                {
                    road.isSavingMeshes = _isSavingMeshes;
                }
            }
        }
    }
}
