#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using RoadArchitect;
#endregion


namespace RoadArchitect
{
    [ExecuteInEditMode]
    public class Road : MonoBehaviour
    {
        public enum RoadMaterialDropdownEnum
        {
            Asphalt,
            Dirt,
            Brick,
            Cobblestone
        };


        #region "Vars"
        public GameObject MainMeshes;
        public GameObject MeshRoad;
        public GameObject MeshShoR;
        public GameObject MeshShoL;
        public GameObject MeshiLanes;
        public GameObject MeshiLanes0;
        public GameObject MeshiLanes1;
        public GameObject MeshiLanes2;
        public GameObject MeshiLanes3;
        public GameObject MeshiMainPlates;
        public GameObject MeshiMarkerPlates;

        [System.NonSerialized]
        public string editorTitleString = "";

        public SplineC spline;

        public int MostRecentNodeCount = -1;
        public GameObject splineObject;
        public RoadSystem roadSystem;
        public SplineC[] PiggyBacks = null;
        public bool isEditorProgressBar = false;
        //Unique ID
        public string UID;
        [SerializeField]
        public List<TerrainHistoryMaker> TerrainHistory;
        public string TerrainHistoryByteSize = "";
        [System.NonSerialized]
        public bool isUpdatingSpline = false;

        //Road editor options:
        public float laneWidth = 5f;
        public bool isShouldersEnabled = true;
        public float shoulderWidth = 3f;
        public int laneAmount = 2;
        public float roadDefinition = 5f;
        public bool roadCornerDefinition = false;
        public bool isRoadCutsEnabled = true;
        public bool isShoulderCutsEnabled = true;
        public bool isDynamicCutsEnabled = false;
        public bool isMaxGradeEnabled = true;
        public float maxGrade = 0.08f;
        public bool isUsingDefaultMaterials = true;
        public bool isAutoUpdatingInEditor = true;

        public float matchTerrainSubtraction = 0.1f;
        public bool isRaisingRoad = false;

        /// <summary> Defines the width of height modification in meters </summary>
        public float matchHeightsDistance = 50f;
        public float clearDetailsDistance = 30f;
        public float clearDetailsDistanceHeight = 5f;
        public float clearTreesDistance = 30f;
        public float clearTreesDistanceHeight = 50f;

        public bool isHeightModificationEnabled = true;
        public bool isDetailModificationEnabled = true;
        public bool isTreeModificationEnabled = true;

        public bool isSavingTerrainHistoryOnDisk = true;
        public float magnitudeThreshold = 300f;
        public bool isGizmosEnabled = true;
        public bool isUsingMultithreading = true;
        public bool isSavingMeshes = false;
        public bool isUsingMeshColliders = true;
        public bool isStatic = false;
        public bool isLightmapped = false;
        public float desiredRampHeight = 0.35f;


        public RoadMaterialDropdownEnum roadMaterialDropdown = RoadMaterialDropdownEnum.Asphalt;
        public RoadMaterialDropdownEnum tRoadMaterialDropdownOLD = RoadMaterialDropdownEnum.Asphalt;


        public Material RoadMaterial1;
        public Material RoadMaterial2;
        public Material RoadMaterial3;
        public Material RoadMaterial4;
        public Material RoadMaterialMarker1;
        public Material RoadMaterialMarker2;
        public Material RoadMaterialMarker3;
        public Material RoadMaterialMarker4;
        public Material ShoulderMaterial1;
        public Material ShoulderMaterial2;
        public Material ShoulderMaterial3;
        public Material ShoulderMaterial4;
        public Material ShoulderMaterialMarker1;
        public Material ShoulderMaterialMarker2;
        public Material ShoulderMaterialMarker3;
        public Material ShoulderMaterialMarker4;

        public PhysicMaterial RoadPhysicMaterial;
        public PhysicMaterial ShoulderPhysicMaterial;
        #endregion


        #region "Road Construction"
        #region "Vars"
        [System.NonSerialized]
        public Threading.TerrainCalcs TerrainCalcsJob;
        [System.NonSerialized]
        public Threading.RoadCalcs1 RoadCalcsJob1;
        [System.NonSerialized]
        public Threading.RoadCalcs2 RoadCalcsJob2;
        [System.NonSerialized]
        public RoadConstructorBufferMaker RCS;

        public string roadName = "";
        public bool isProfiling = false;
        public bool isSkippingStore = true;
        [System.NonSerialized]
        public float EditorConstructionStartTime = 0f;
        public bool isEditorError = false;
        public System.Exception exceptionError = null;


        private int editorTimer = 0;
        private int editorTimerMax = 0;
        private int editorTimerSpline = 0;
        private const int editorTimerSplineMax = 2;
        [System.NonSerialized]
        public int editorProgress = 0;
        private const int gizmoNodeTimerMax = 2;
        public bool isUpdateRequired = false;
        public bool isTriggeringGC = false;
        private bool isTriggeredGCExecuting;
        private float triggerGCEnd = 0f;

        [System.NonSerialized]
        public bool isEditorCameraMoving = false;
        [System.NonSerialized]
        public float EditorCameraPos = 0f;
        private const float EditorCameraTimeUpdateInterval = 0.015f;
        private float EditorCameraNextMove = 0f;
        private bool isEditorCameraSetup = false;
        private float EditorCameraStartPos = 0f;
        private float EditorCameraEndPos = 1f;
        private float EditorCameraIncrementDistance = 0f;
        private float EditorCameraIncrementDistance_Full = 0f;
        public float EditorCameraMetersPerSecond = 60f;
        public bool isEditorCameraRotated = false;
        private Vector3 EditorCameraV1 = default(Vector3);
        private Vector3 EditorCameraV2 = default(Vector3);
        [System.NonSerialized]
        public Vector3 editorCameraOffset = new Vector3(0f, 5f, 0f);
        [System.NonSerialized]
        public Camera editorPlayCamera = null;
        private Vector3 editorCameraBadVec = default(Vector3);

        public List<Terraforming.TempTerrainData> EditorTTDList;

        public bool isEditorConstructing = false;
        public int editorConstructionID = 0;
        public bool isEditorSelected = false;
        public bool isEditorMouseHittingTerrain = false;
        public Vector3 editorMousePos = new Vector3(0f, 0f, 0f);
        public Color defaultNodeColor = new Color(0f, 1f, 1f, 0.75f);
        /// <summary> Connection node color </summary>
        public readonly Color Color_NodeConnColor = new Color(0f, 1f, 0f, 0.75f);
        /// <summary> The color of the nodes when they are selected </summary>
        public Color selectedColor = Color.yellow;
        /// <summary> Color of the node preview when adding a new node </summary>
        public Color newNodePreviewColor = Color.red;
        #endregion


        /// <summary> Make sure unused items are not using memory space in runtime </summary>
        private void CleanRunTime()
        {
            TerrainHistory = null;
            RCS = null;
        }


        private void OnEnable()
        {
            #if UNITY_EDITOR
            isEditorConstructing = false;
            UnityEditor.EditorApplication.update += delegate
            {
                EditorUpdate();
            };
            #if UNITY_2018_1_OR_NEWER
            UnityEditor.EditorApplication.hierarchyChanged += delegate
            {
                HierarchyWindowChanged();
            };
            #else
            UnityEditor.EditorApplication.hierarchyWindowChanged += delegate { HierarchyWindowChanged(); };
            #endif
            if (spline == null || spline.nodes == null)
            {
                MostRecentNodeCount = 0;
            }
            else
            {
                MostRecentNodeCount = spline.GetNodeCount();
            }
            tRoadMaterialDropdownOLD = roadMaterialDropdown;
            CheckMats();
            #endif
        }


        public void Awake()
        {
            if (spline == null || spline.nodes == null)
            {
                MostRecentNodeCount = 0;
            }
            else
            {
                MostRecentNodeCount = spline.GetNodeCount();
            }
        }


        private void EditorUpdate()
        {
            #if UNITY_EDITOR
            if (this == null)
            {
                UnityEditor.EditorApplication.update -= delegate
                {
                    EditorUpdate();
                };
                isEditorConstructing = false;
                EngineIntegration.ClearProgressBar();
                return;
            }
            #endif

            //Custom garbage collection demands for editor:
            if (isTriggeringGC)
            {
                isTriggeringGC = false;
                triggerGCEnd = Time.realtimeSinceStartup + 1f;
                isTriggeredGCExecuting = true;
            }
            if (isTriggeredGCExecuting)
            {
                if (Time.realtimeSinceStartup > triggerGCEnd)
                {
                    isTriggeredGCExecuting = false;
                    RootUtils.ForceCollection();
                    triggerGCEnd = 200000f;
                }
            }

            if (isEditorConstructing)
            {
                if (isUsingMultithreading)
                {
                    editorTimer += 1;
                    if (editorTimer > editorTimerMax)
                    {
                        if ((Time.realtimeSinceStartup - EditorConstructionStartTime) > 180f)
                        {
                            isEditorConstructing = false;
                            EngineIntegration.ClearProgressBar();
                            Debug.Log("Update shouldn't take longer than 180 seconds. Aborting update.");
                        }
                        editorTimer = 0;
                        if (isEditorError)
                        {
                            isEditorConstructing = false;
                            EngineIntegration.ClearProgressBar();

                            isEditorError = false;
                            if (exceptionError != null)
                            {
                                Debug.LogError(exceptionError.StackTrace);
                                throw exceptionError;
                            }
                        }

                        if (TerrainCalcsJob != null && TerrainCalcsJob.Update())
                        {
                            ConstructRoad2();
                        }
                        else if (RoadCalcsJob1 != null && RoadCalcsJob1.Update())
                        {
                            ConstructRoad3();
                        }
                        else if (RoadCalcsJob2 != null && RoadCalcsJob2.Update())
                        {
                            ConstructRoad4();
                        }
                    }
                }
            }
            else
            {
                if (isUpdateRequired)
                {
                    isUpdateRequired = false;
                    spline.TriggerSetup();
                }
            }

            if (isEditorConstructing || isEditorProgressBar)
            {
                RoadUpdateProgressBar();
            }


            #if UNITY_EDITOR
            if (!Application.isPlaying && isUpdatingSpline)
            {
                editorTimerSpline += 1;
                if (editorTimerSpline > editorTimerSplineMax)
                {
                    editorTimerSpline = 0;
                    isUpdatingSpline = false;
                    spline.TriggerSetup();
                    MostRecentNodeCount = spline.nodes.Count;
                }
            }


            if (isEditorCameraMoving && EditorCameraNextMove < UnityEditor.EditorApplication.timeSinceStartup)
            {
                EditorCameraNextMove = (float)UnityEditor.EditorApplication.timeSinceStartup + EditorCameraTimeUpdateInterval;
                DoEditorCameraLoop();
            }
            #endif
        }


        public void DoEditorCameraLoop()
        {
            if (!isEditorCameraSetup)
            {
                isEditorCameraSetup = true;
                if (spline.isSpecialEndControlNode)
                {
                    //If control node, start after the control node:
                    EditorCameraEndPos = spline.nodes[spline.GetNodeCount() - 2].time;
                }
                if (spline.isSpecialStartControlNode)
                {
                    //If ends in control node, end construction before the control node:
                    EditorCameraStartPos = spline.nodes[1].time;
                }
                ChangeEditorCameraMetersPerSec();
            }

            #if UNITY_EDITOR
            if (!UnityEditor.Selection.Contains(this.transform.gameObject))
            {
                QuitEditorCamera();
                return;
            }
            #endif

            //EditorCameraPos_Full+=EditorCameraIncrementDistance_Full;
            //if(EditorCameraPos_Full > spline.distance)
            //{
            //  EditorCameraPos = EditorCameraStartPos;
            //  isEditorCameraMoving = false;
            //  EditorCameraPos_Full = 0f;
            //  return;
            //  }
            //EditorCameraPos = spline.TranslateDistBasedToParam(EditorCameraPos_Full);

            EditorCameraPos += EditorCameraIncrementDistance;
            if (EditorCameraPos > EditorCameraEndPos)
            {
                QuitEditorCamera();
                return;
            }
            if (EditorCameraPos < EditorCameraStartPos)
            {
                EditorCameraPos = EditorCameraStartPos;
            }

            spline.GetSplineValueBoth(EditorCameraPos, out EditorCameraV1, out EditorCameraV2);


            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (editorPlayCamera != null)
                {
                    editorPlayCamera.transform.position = EditorCameraV1;
                    if (isEditorCameraRotated)
                    {
                        editorPlayCamera.transform.position += editorCameraOffset;
                        if (EditorCameraV2 != editorCameraBadVec)
                        {
                            editorPlayCamera.transform.rotation = Quaternion.LookRotation(EditorCameraV2);
                        }
                    }
                }
            }
            else
            {
                UnityEditor.SceneView.lastActiveSceneView.pivot = EditorCameraV1;
                if (isEditorCameraRotated)
                {
                    UnityEditor.SceneView.lastActiveSceneView.pivot += editorCameraOffset;
                    if (EditorCameraV2 != editorCameraBadVec)
                    {
                        UnityEditor.SceneView.lastActiveSceneView.rotation = Quaternion.LookRotation(EditorCameraV2);
                    }
                }
                UnityEditor.SceneView.lastActiveSceneView.Repaint();
            }
            #endif
        }


        public void EditorCameraSetSingle()
        {
            if (editorPlayCamera == null)
            {
                Camera[] editorCameras = GameObject.FindObjectsOfType<Camera>();
                if (editorCameras != null && editorCameras.Length == 1)
                {
                    editorPlayCamera = editorCameras[0];
                }
            }
        }


        public void QuitEditorCamera()
        {
            EditorCameraPos = EditorCameraStartPos;
            isEditorCameraMoving = false;
            isEditorCameraSetup = false;
        }


        public void ChangeEditorCameraMetersPerSec()
        {
            EditorCameraIncrementDistance_Full = (EditorCameraMetersPerSecond / 60);
            EditorCameraIncrementDistance = (EditorCameraIncrementDistance_Full / spline.distance);
        }


        /// <summary> This is called when the hierarchy is changed in the editor </summary>
        private void HierarchyWindowChanged()
        {
            #if UNITY_EDITOR
            #if UNITY_2018_1_OR_NEWER
            UnityEditor.EditorApplication.hierarchyChanged -= delegate { HierarchyWindowChanged(); };
            #else
            UnityEditor.EditorApplication.hierarchyWindowChanged -= delegate { HierarchyWindowChanged(); };
            #endif
            #endif

            if (Application.isPlaying || !Application.isEditor)
            {
                return;
            }
 

            #if UNITY_EDITOR
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if(UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
            #endif
      

            int count = 0;
            if (spline != null && spline.nodes != null)
            {
                count = spline.GetNodeCountNonNull();
            }
            if (count != MostRecentNodeCount)
            {
                isUpdatingSpline = true;
            }
        }


        /// <summary> Display the progress of the road update </summary>
        private void RoadUpdateProgressBar()
        {
            #if UNITY_EDITOR
            if (isEditorConstructing)
            {
                UnityEditor.EditorUtility.DisplayProgressBar(
                    "RoadArchitect: Road Update",
                    editorTitleString,
                    ((float)editorProgress / 100f));
            }
            else if (isEditorProgressBar)
            {
                isEditorProgressBar = false;
                EngineIntegration.ClearProgressBar();
            }
            #endif
        }


        /// <summary> Starts the road update </summary>
        public void UpdateRoad(RoadUpdateTypeEnum _updateType = RoadUpdateTypeEnum.Full)
        {
            if (!roadSystem.isAllowingRoadUpdates)
            {
                spline.Setup();
                isEditorConstructing = false;
                return;
            }

            if (isEditorConstructing)
            {
                return;
            }

            RootUtils.SetupUniqueIdentifier(ref UID);

            RootUtils.StartProfiling(this, "UpdateRoadPrelim");

            roadDefinition = Mathf.Clamp(roadDefinition, 1f, 50f);
            laneWidth = Mathf.Clamp(laneWidth, 0.2f, 500f);

            EditorConstructionStartTime = Time.realtimeSinceStartup;
            editorTitleString = "Updating " + transform.name + "...";
            System.GC.Collect();

            if (isSavingTerrainHistoryOnDisk)
            {
                LoadTerrainHistory();
            }

            CheckMats();
            EngineIntegration.ClearProgressBar();

            isProfiling = true;
            if (isUsingMultithreading)
            {
                isProfiling = false;
            }

            //Set all terrains to height 0:
            Terraforming.CheckAllTerrainsHeight0();

            editorProgress = 20;
            isEditorProgressBar = true;
            if (isEditorConstructing)
            {
                AbortJobs();

                isEditorConstructing = false;
            }

            //In here for intersection patching purposes:
            int nodeCount = spline.GetNodeCount();
            SplineN node = null;
            SplineN node1 = null;
            SplineN node2 = null;


            if (spline.CheckInvalidNodeCount())
            {
                spline.Setup();
                nodeCount = spline.GetNodeCount();
            }


            if (nodeCount > 1)
            {
                // Iterate over every node
                for (int i = 0; i < nodeCount; i++)
                {
                    //try
                    //{
                    node = spline.nodes[i];
                    //}
                    //catch
                    //{
                    //  Editor_bIsConstructing = false;
                    //	EditorUpdateMe = true;
                    //	return;	
                    //}

                    //If node is intersection with an invalid RoadIntersection, mark it as non-intersection. Just-in-case.
                    if (node.isIntersection && node.intersection == null)
                    {
                        node.isIntersection = false;
                        node.intersectionOtherNodeID = -1;
                        node.intersectionOtherNode = null;
                    }


                    //If node is intersection, re-setup:
                    if (node.isIntersection && node.intersection != null)
                    {
                        node1 = node.intersection.node1;
                        node2 = node.intersection.node2;
                        node.intersection.Setup(node1, node2);
                        node.intersection.DeleteRelevantChildren(node, node.spline.road.transform.name);

                        //If primary node on intersection, do more re-setup:
                        if (node.intersection.node1 == node)
                        {
                            node.intersection.lanesAmount = laneAmount;
                            node.intersection.name = node.intersection.transform.name;
                        }
                        //Setup construction objects:
                        node.intersection.node1.intersectionConstruction = new iConstructionMaker();
                        node.intersection.node2.intersectionConstruction = new iConstructionMaker();
                    }


                    //Store materials and physical materials for road and or shoulder cuts on each node, if necessary:
                    node.StoreCuts();
                }
            }


            name = transform.name;

            spline.RoadWidth = RoadWidth();
            //RootUtils.StartProfiling(this, "SplineSetup");
            spline.Setup();
            //RootUtils.EndProfiling(this);
            nodeCount = spline.GetNodeCount();

            if (spline == null || spline.nodes == null)
            {
                MostRecentNodeCount = 0;
            }
            else
            {
                MostRecentNodeCount = spline.GetNodeCount();
            }


            if (isUsingDefaultMaterials)
            {
                SetDefaultMats();

                if (DetectInvalidDefaultMatsForUndo())
                {
                    SetAllCutsToCurrentMaterials();
                }
            }

            //Hiding in hierarchy:
            for (int i = 0; i < nodeCount; i++)
            {
                node = spline.nodes[i];
                if (node != null)
                {
                    if (node.isIntersection || node.isSpecialEndNode)
                    {
                        node.ToggleHideFlags(true);
                    }
                    else
                    {
                        node.ToggleHideFlags(false);
                    }
                }
            }


            // Delete mainMeshes of this road
            int childCount = transform.childCount;
            GameObject mainMeshes = null;
            for (int i = 0; i < childCount; i++)
            {
                if (transform.GetChild(i).transform.name.ToLower().Contains("mainmeshes"))
                {
                    mainMeshes = transform.GetChild(i).transform.gameObject;
                    Object.DestroyImmediate(mainMeshes);
                }
            }


            if (nodeCount < 2)
            {
                //Delete old objs and return:
                Object.DestroyImmediate(MainMeshes);
                Object.DestroyImmediate(MeshRoad);
                Object.DestroyImmediate(MeshShoR);
                Object.DestroyImmediate(MeshShoL);
                Object.DestroyImmediate(MeshiLanes);
                Object.DestroyImmediate(MeshiLanes0);
                Object.DestroyImmediate(MeshiLanes1);
                Object.DestroyImmediate(MeshiLanes2);
                Object.DestroyImmediate(MeshiLanes3);
                Object.DestroyImmediate(MeshiMainPlates);
                Object.DestroyImmediate(MeshiMarkerPlates);
                RootUtils.EndProfiling(this);
                return;
            }


            spline.HeightHistory = new List<KeyValuePair<float, float>>();
            if (roadSystem == null)
            {
                roadSystem = transform.parent.GetComponent<RoadSystem>();
            }
            //Compatibility update.
            if (isUsingMultithreading)
            {
                isEditorConstructing = true;
            }
            else
            {
                isEditorConstructing = false;
            }
            editorConstructionID = 0;


            //Check if road takes place on only 1 terrain:
            Terrain terrain = RoadUtility.GetTerrain(spline.nodes[0].pos);
            bool isSameTerrain = true;
            for (int i = 1; i < nodeCount; i++)
            {
                if (terrain != RoadUtility.GetTerrain(spline.nodes[0].pos))
                {
                    isSameTerrain = false;
                    break;
                }
            }

            RCS = new RoadConstructorBufferMaker(this, _updateType);

            if (isSameTerrain)
            {
                RCS.tTerrain = terrain;
            }
            else
            {
                RCS.tTerrain = null;
            }
            terrain = null;

            RootUtils.EndProfiling(this);
            if (isUsingMultithreading)
            {
                if (RCS.isTerrainOn || TerrainHistory == null)
                {
                    Terraforming.ProcessRoadTerrainHook1(spline, this);
                }
                else
                {
                    ConstructRoad2();
                }
            }
            else
            {
                UpdateRoadNoMultiThreading();
            }
        }


        #region "Terrain history"
        public void StoreTerrainHistory(bool _isDiskOnly = false)
        {
            if (!_isDiskOnly)
            {
                Road road = this;
                RoadUtility.ConstructRoadStoreTerrainHistory(ref road);
            }

            if (isSavingTerrainHistoryOnDisk && TerrainHistory != null && TerrainHistory.Count > 0)
            {
                RootUtils.StartProfiling(this, "TerrainHistory_Save");
                TerrainHistoryUtility.SaveTerrainHistory(TerrainHistory, this);
                RootUtils.EndProfiling(this);
                TerrainHistory.Clear();
                TerrainHistory = null;
            }
            else
            {
                if (TerrainHistory != null && TerrainHistory.Count > 0)
                {
                    int terrainSize = 0;
                    for (int i = 0; i < TerrainHistory.Count; i++)
                    {
                        terrainSize += TerrainHistory[i].GetSize();
                    }
                    TerrainHistoryByteSize = (terrainSize * 0.001f).ToString("n0") + " kb";
                }
                else
                {
                    TerrainHistoryByteSize = "0 bytes";
                }
            }
        }


        public void ResetTerrainHistory()
        {
            Road tRoad = this;
            if (isSavingTerrainHistoryOnDisk && TerrainHistory != null)
            {
                TerrainHistoryUtility.DeleteTerrainHistory(this);
            }
            else
            {
                RoadUtility.ResetTerrainHistory(ref tRoad);
            }
        }


        /// <summary> Loads terrain history from disk </summary>
        public void LoadTerrainHistory(bool _isForced = false)
        {
            if (isSavingTerrainHistoryOnDisk || _isForced)
            {
                if (TerrainHistory != null)
                {
                    TerrainHistory.Clear();
                    TerrainHistory = null;
                }
                TerrainHistory = TerrainHistoryUtility.LoadTerrainHistory(this);
            }
            if (_isForced)
            {
                TerrainHistoryUtility.DeleteTerrainHistory(this);
            }
        }
        #endregion


        #region "Construction process"
        #region "No multithread"
        private void UpdateRoadNoMultiThreading()
        {
            if (isHeightModificationEnabled || isDetailModificationEnabled || isTreeModificationEnabled)
            {
                RootUtils.StartProfiling(this, "RoadCon_Terrain");
                if (RCS.isTerrainOn || TerrainHistory == null)
                {
                    Terraforming.ProcessRoadTerrainHook1(spline, this, false);
                    Terraforming.ProcessRoadTerrainHook2(spline, ref EditorTTDList);
                    //Store history.
                    StoreTerrainHistory();
                    int editorTTDListCount = EditorTTDList.Count;
                    for (int i = 0; i < editorTTDListCount; i++)
                    {
                        EditorTTDList[i] = null;
                    }
                    EditorTTDList = null;
                    System.GC.Collect();
                }
                RootUtils.EndProfiling(this);
            }

            editorProgress = 50;
            Road road = this;
            RootUtils.StartProfiling(this, "RoadCon_RoadPrelim");

            editorProgress = 80;
            Threading.RoadCreationT.RoadJobPrelim(ref road);
            RootUtils.EndStartProfiling(this, "RoadCon_Road1");
            editorProgress = 90;
            Threading.RoadCreationT.RoadJob1(ref RCS);
            RootUtils.EndStartProfiling(this, "MeshSetup1");
            editorProgress = 92;
            RCS.MeshSetup1();
            RootUtils.EndStartProfiling(this, "RoadCon_Road2");
            editorProgress = 94;
            Threading.RoadCreationT.RoadJob2(ref RCS);
            RootUtils.EndStartProfiling(this, "MeshSetup2");
            editorProgress = 96;
            RCS.MeshSetup2();
            RootUtils.EndProfiling(this);
            ConstructionCleanup();
        }
        #endregion


        /// <summary> Stores terrain history and nulls TerrainCalcsJob</summary>
        private void ConstructRoad2()
        {
            editorProgress = 40;
            if (RCS.isTerrainOn)
            {
                //Store history:
                Terraforming.ProcessRoadTerrainHook2(spline, ref EditorTTDList);
                StoreTerrainHistory();

                EditorTTDList.Clear();
                EditorTTDList = null;
                System.GC.Collect();
            }
            editorProgress = 60;

            if (TerrainCalcsJob != null)
            {
                TerrainCalcsJob.Abort();
                TerrainCalcsJob = null;
            }
            Road road = this;
            editorProgress = 72;
            RoadCalcsJob1 = new Threading.RoadCalcs1();
            RoadCalcsJob1.Setup(ref RCS, ref road);
            RoadCalcsJob1.Start();
        }


        private void ConstructRoad3()
        {
            editorProgress = 84;
            RCS.MeshSetup1();
            editorProgress = 96;
            if (RoadCalcsJob1 != null)
            {
                RoadCalcsJob1.Abort();
                RoadCalcsJob1 = null;
            }
            RoadCalcsJob2 = new Threading.RoadCalcs2();
            RoadCalcsJob2.Setup(ref RCS);
            RoadCalcsJob2.Start();
            editorProgress = 98;
        }


        private void ConstructRoad4()
        {
            RCS.MeshSetup2();
            ConstructionCleanup();
        }
        #endregion


        private void AbortJobs()
        {
            if (TerrainCalcsJob != null)
            {
                TerrainCalcsJob.Abort();
                TerrainCalcsJob = null;
            }
            if (RoadCalcsJob1 != null)
            {
                RoadCalcsJob1.Abort();
                RoadCalcsJob1 = null;
            }
            if (RoadCalcsJob2 != null)
            {
                RoadCalcsJob2.Abort();
                RoadCalcsJob2 = null;
            }
        }


        private void ConstructionCleanup()
        {
            FixDisplay();
            AbortJobs();

            isEditorConstructing = false;
            int nodeCount = spline.GetNodeCount();
            SplineN node;
            for (int i = 0; i < nodeCount; i++)
            {
                node = spline.nodes[i];
                if (node.isIntersection)
                {
                    if (node.intersectionConstruction != null)
                    {
                        node.intersectionConstruction.Nullify();
                        node.intersectionConstruction = null;
                    }
                }
                node.SetupSplinationLimits();
                node.SetupEdgeObjects(false);
                node.SetupSplinatedMeshes(false);
            }
            if (spline.HeightHistory != null)
            {
                spline.HeightHistory.Clear();
                spline.HeightHistory = null;
            }
            if (RCS != null)
            {
                RCS.Nullify();
                RCS = null;
            }

            if (isSavingMeshes)
            {
                EngineIntegration.SaveAssets();
            }

            isEditorProgressBar = false;
            EngineIntegration.ClearProgressBar();

            //Make sure terrain history out of memory if necessary (redudant but keep):
            if (isSavingTerrainHistoryOnDisk && TerrainHistory != null)
            {
                TerrainHistory.Clear();
                TerrainHistory = null;
            }

            //Collect:
            isTriggeringGC = true;

            if (tRoadMaterialDropdownOLD != roadMaterialDropdown)
            {
                tRoadMaterialDropdownOLD = roadMaterialDropdown;
                SetAllCutsToCurrentMaterials();
            }

            if (PiggyBacks != null && PiggyBacks.Length > 0)
            {
                for (int i = 0; i < PiggyBacks.Length; i++)
                {
                    if (PiggyBacks[i] == null)
                    {
                        PiggyBacks = null;
                        break;
                    }
                }

                if (PiggyBacks != null)
                {
                    SplineC tPiggy = PiggyBacks[0];
                    SplineC[] NewPiggys = null;

                    PiggyBacks[0] = null;
                    if (PiggyBacks.Length > 1)
                    {
                        NewPiggys = new SplineC[PiggyBacks.Length - 1];
                        for (int i = 1; i < PiggyBacks.Length; i++)
                        {
                            NewPiggys[i - 1] = PiggyBacks[i];
                        }
                    }

                    if (NewPiggys != null)
                    {
                        tPiggy.road.PiggyBacks = NewPiggys;
                    }
                    NewPiggys = null;
                    tPiggy.TriggerSetup();
                }
            }
        }


        public void SetEditorTerrainCalcs(ref List<Terraforming.TempTerrainData> _tddList)
        {
            EditorTTDList = _tddList;
        }
        #endregion


        #region "Gizmos"
        private void OnDrawGizmosSelected()
        {
            if (isEditorMouseHittingTerrain)
            {
                Gizmos.color = newNodePreviewColor;
                Gizmos.DrawCube(editorMousePos, new Vector3(10f, 4f, 10f));
            }
        }
        #endregion


        /// <summary> Returns width of road </summary>
        public float RoadWidth()
        {
            return (laneWidth * (float)laneAmount);
        }


        #if UNITY_EDITOR
        public float EditorCameraTimer = 0f;


        private void Update()
        {
            if (Application.isEditor && isEditorCameraMoving)
            {
                EditorCameraTimer += Time.deltaTime;
                if (EditorCameraTimer > EditorCameraTimeUpdateInterval)
                {
                    EditorCameraTimer = 0f;
                    DoEditorCameraLoop();
                }
            }
        }
        #endif


        #region "Default materials retrieval"
        public bool DetectInvalidDefaultMatsForUndo()
        {
            string lowerName = "";
            int counter = 0;
            if (!MeshRoad)
            {
                return false;
            }

            string basePath = RoadEditorUtility.GetBasePath();

            MeshRenderer[] MRs = MeshRoad.GetComponentsInChildren<MeshRenderer>();
            Material tMat2Lanes = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble.mat");
            Material tMat4Lanes = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-4L.mat");
            Material tMat6Lanes = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-6L.mat");
            foreach (MeshRenderer MR in MRs)
            {
                lowerName = MR.transform.name.ToLower();
                if (lowerName.Contains("marker"))
                {
                    if (laneAmount == 2)
                    {
                        if (MR.sharedMaterials[0] == tMat4Lanes)
                        {
                            counter += 1;
                        }
                        else if (MR.sharedMaterials[0] == tMat6Lanes)
                        {
                            counter += 1;
                        }
                    }
                    else if (laneAmount == 4)
                    {
                        if (MR.sharedMaterials[0] == tMat2Lanes)
                        {
                            counter += 1;
                        }
                        else if (MR.sharedMaterials[0] == tMat6Lanes)
                        {
                            counter += 1;
                        }
                    }
                    else if (laneAmount == 6)
                    {
                        if (MR.sharedMaterials[0] == tMat2Lanes)
                        {
                            counter += 1;
                        }
                        else if (MR.sharedMaterials[0] == tMat4Lanes)
                        {
                            counter += 1;
                        }
                    }
                }
                if (counter > 1)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary> Assigns materials on all mesh renderers </summary>
        public void SetAllCutsToCurrentMaterials()
        {
            if (!MeshRoad)
            {
                return;
            }

            MeshRenderer[] MRs = MeshRoad.GetComponentsInChildren<MeshRenderer>();
            Material[] roadWorldMats = GetRoadWorldMaterials();
            Material[] roadMarkerMats = GetRoadMarkerMaterials();
            SetCutMaterials(MRs, roadWorldMats, roadMarkerMats);

            if (isShouldersEnabled)
            {
                roadWorldMats = GetShoulderWorldMaterials();
                roadMarkerMats = GetShoulderMarkerMaterials();
                if (MeshShoL != null)
                {
                    MRs = MeshShoL.GetComponentsInChildren<MeshRenderer>();
                    SetCutMaterials(MRs, roadWorldMats, roadMarkerMats);
                }
                if (MeshShoR != null)
                {
                    MRs = MeshShoR.GetComponentsInChildren<MeshRenderer>();
                    SetCutMaterials(MRs, roadWorldMats, roadMarkerMats);
                }
            }
        }


        /// <summary> Sets materials of each MR in _MRs </summary>
        private void SetCutMaterials(MeshRenderer[] _MRs, Material[] _roadWorldMats, Material[] _roadMarkerMats)
        {
            string lowerName;
            foreach (MeshRenderer MR in _MRs)
            {
                lowerName = MR.transform.name.ToLower();
                if (lowerName.Contains("marker"))
                {
                    if (_roadMarkerMats != null)
                    {
                        MR.sharedMaterials = _roadMarkerMats;
                    }
                }
                else if (lowerName.Contains("cut"))
                {
                    if (_roadWorldMats != null)
                    {
                        MR.sharedMaterials = _roadWorldMats;
                    }
                }
            }
        }


        public Material[] GetRoadWorldMaterials()
        {
            List<Material> roadMaterials = new List<Material>();
            if (RoadMaterial1 != null)
            {
                roadMaterials.Add(RoadMaterial1);
                if (RoadMaterial2 != null)
                {
                    roadMaterials.Add(RoadMaterial2);
                    if (RoadMaterial3 != null)
                    {
                        roadMaterials.Add(RoadMaterial3);
                        if (RoadMaterial4 != null)
                        {
                            roadMaterials.Add(RoadMaterial4);
                        }
                    }
                }
            }

            return roadMaterials.ToArray();
        }


        public Material[] GetRoadMarkerMaterials()
        {
            List<Material> roadMarkerMaterials = new List<Material>();
            if (RoadMaterialMarker1 != null)
            {
                roadMarkerMaterials.Add(RoadMaterialMarker1);
                if (RoadMaterialMarker2 != null)
                {
                    roadMarkerMaterials.Add(RoadMaterialMarker2);
                    if (RoadMaterialMarker3 != null)
                    {
                        roadMarkerMaterials.Add(RoadMaterialMarker3);
                        if (RoadMaterialMarker4 != null)
                        {
                            roadMarkerMaterials.Add(RoadMaterialMarker4);
                        }
                    }
                }
            }

            return roadMarkerMaterials.ToArray();
        }


        public Material[] GetShoulderWorldMaterials()
        {
            if (!isShouldersEnabled)
            {
                return null;
            }

            List<Material> shoulderMaterials = new List<Material>();
            if (ShoulderMaterial1 != null)
            {
                shoulderMaterials.Add(ShoulderMaterial1);
                if (ShoulderMaterial2 != null)
                {
                    shoulderMaterials.Add(ShoulderMaterial2);
                    if (ShoulderMaterial3 != null)
                    {
                        shoulderMaterials.Add(ShoulderMaterial3);
                        if (ShoulderMaterial4 != null)
                        {
                            shoulderMaterials.Add(ShoulderMaterial4);
                        }
                    }
                }
            }

            return shoulderMaterials.ToArray();
        }


        public Material[] GetShoulderMarkerMaterials()
        {
            if (!isShouldersEnabled)
            {
                return null;
            }

            List<Material> shoulderMarkerMaterials = new List<Material>();
            if (ShoulderMaterialMarker1 != null)
            {
                shoulderMarkerMaterials.Add(ShoulderMaterialMarker1);
                if (ShoulderMaterialMarker2 != null)
                {
                    shoulderMarkerMaterials.Add(ShoulderMaterialMarker2);
                    if (ShoulderMaterialMarker3 != null)
                    {
                        shoulderMarkerMaterials.Add(ShoulderMaterialMarker3);
                        if (ShoulderMaterialMarker4 != null)
                        {
                            shoulderMarkerMaterials.Add(ShoulderMaterialMarker4);
                        }
                    }
                }
            }

            return shoulderMarkerMaterials.ToArray();
        }
        #endregion


        #region "Materials"
        /// <summary> Loads the standard materials if the road uses default materials </summary>
        private void CheckMats()
        {
            if (!isUsingDefaultMaterials)
            {
                return;
            }

            string basePath = RoadEditorUtility.GetBasePath();

            if (!RoadMaterial1)
            {
                RoadMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                RoadMaterial2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
            }

            if (!RoadMaterialMarker1)
            {
                if (laneAmount == 2)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks.mat");
                }
                else if (laneAmount == 4)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-4L.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks-4L.mat");
                }
                else if (laneAmount == 6)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-6L.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks-6L.mat");
                }
                else
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks.mat");
                }
            }

            // Can be simplified
            if (isShouldersEnabled && !ShoulderMaterial1)
            {
                ShoulderMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Shoulder1.mat");
                ShoulderMaterial2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
            }

            if (isShouldersEnabled && !RoadPhysicMaterial)
            {
                RoadPhysicMaterial = RoadEditorUtility.LoadPhysicsMaterial(basePath + "/Physics/Pavement.physicMaterial");
            }
            if (isShouldersEnabled && !ShoulderPhysicMaterial)
            {
                ShoulderPhysicMaterial = RoadEditorUtility.LoadPhysicsMaterial(basePath + "/Physics/Dirt.physicMaterial");
            }
        }


        public void SetDefaultMats()
        {
            string basePath = RoadEditorUtility.GetBasePath();

            // Reset materials
            RoadMaterial1 = null;
            RoadMaterial2 = null;
            RoadMaterial3 = null;
            RoadMaterial4 = null;
            RoadMaterialMarker2 = null;
            RoadMaterialMarker3 = null;
            RoadMaterialMarker4 = null;


            if (roadMaterialDropdown == RoadMaterialDropdownEnum.Asphalt)
            {
                RoadMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                RoadMaterial2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");

                if (laneAmount == 2)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks.mat");
                }
                else if (laneAmount == 4)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-4L.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks-4L.mat");
                }
                else if (laneAmount == 6)
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble-6L.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks-6L.mat");
                }
                else
                {
                    RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/WhiteYellowDouble.mat");
                    RoadMaterialMarker2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/TireMarks.mat");
                }


                ShoulderMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Shoulder1.mat");
                ShoulderMaterial2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");

                RoadPhysicMaterial = RoadEditorUtility.LoadPhysicsMaterial(basePath + "/Physics/Pavement.physicMaterial");
                ShoulderPhysicMaterial = RoadEditorUtility.LoadPhysicsMaterial(basePath + "/Physics/Dirt.physicMaterial");
            }
            else if (roadMaterialDropdown == RoadMaterialDropdownEnum.Dirt)
            {
                RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/DirtRoad.mat");
                RoadMaterial1 = RoadMaterialMarker1;
            }
            else if (roadMaterialDropdown == RoadMaterialDropdownEnum.Brick)
            {
                RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/BrickRoad.mat");
                RoadMaterial1 = RoadMaterialMarker1;
            }
            else if (roadMaterialDropdown == RoadMaterialDropdownEnum.Cobblestone)
            {
                RoadMaterialMarker1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/CobblestoneRoad.mat");
                RoadMaterial1 = RoadMaterialMarker1;
            }


            if (roadMaterialDropdown == RoadMaterialDropdownEnum.Brick
            || roadMaterialDropdown == RoadMaterialDropdownEnum.Cobblestone
            || roadMaterialDropdown == RoadMaterialDropdownEnum.Dirt)
            {
                if (laneAmount > 2)
                {
                    RoadMaterialMarker1 = new Material(RoadMaterialMarker1);
                    RoadMaterialMarker1.mainTextureScale = Vector2.Scale(RoadMaterialMarker1.mainTextureScale, new Vector2(laneAmount / 2, 1f));
                }
            }


            int nodeCount = spline.GetNodeCount();
            for (int i = 0; i < nodeCount; i++)
            {
                if (spline.nodes[i] && spline.nodes[i].isIntersection && spline.nodes[i].intersection != null && spline.nodes[i].intersection.isUsingDefaultMaterials)
                {
                    spline.nodes[i].intersection.ResetMaterialsAll();
                }
            }
        }
        #endregion


        /// <summary> Toggles render state of every mesh of this road </summary>
        public void ToggleWireframes()
        {
            ToggleWireframesHelper(transform);

            if (spline != null)
            {
                ToggleWireframesHelper(spline.transform);
            }
        }


        /// <summary>  Toggles render state of each mesh in _MRs through isGizmosEnabled </summary>
        private void ToggleWireframesHelper(Transform _transform)
        {
            MeshRenderer[] _MRs = _transform.GetComponentsInChildren<MeshRenderer>();
            int meshRenderersCount = _MRs.Length;
            for (int i = 0; i < meshRenderersCount; i++)
            {
                //UnityEditor.EditorUtility.SetSelectedWireframeHidden(_MRs[i], !isGizmosEnabled);
                EngineIntegration.SetSelectedRenderState(_MRs[i], isGizmosEnabled);
            }
        }


        /// <summary> Sets localPosition for meshes up to prevent z fighting </summary>
        private void FixDisplay()
        {
            FixDisplayMobile();
        }


        /// <summary> Sets localPosition for meshes up to prevent z fighting </summary>
        private void FixDisplayMobile()
        {
            //This road:
            Object[] markerObjs = transform.GetComponentsInChildren<MeshRenderer>();
            Vector3 vector;
            foreach (MeshRenderer MR in markerObjs)
            {
                if (MR.transform.name.Contains("Marker"))
                {
                    vector = new Vector3(0f, 0.02f, 0f);
                    MR.transform.localPosition = vector;
                }
                else if (MR.transform.name.Contains("SCut") || MR.transform.name.Contains("RoadCut")
                    || MR.transform.name.Contains("ShoulderR")
                    || MR.transform.name.Contains("ShoulderL"))
                {
                    vector = MR.transform.position;
                    vector.y += 0.01f;
                    MR.transform.position = vector;
                }
                else if (MR.transform.name.Contains("RoadMesh"))
                {
                    vector = MR.transform.position;
                    vector.y += 0.02f;
                    MR.transform.position = vector;
                }
                else if (MR.transform.name.Contains("Pavement"))
                {
                    vector = MR.transform.position;
                    vector.y -= 0.01f;
                    MR.transform.position = vector;
                }
            }


            //Intersections (all):
            markerObjs = roadSystem.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer MR in markerObjs)
            {
                if (MR.transform.name.Contains("CenterMarkers"))
                {
                    vector = new Vector3(0f, 0.02f, 0f);
                    MR.transform.localPosition = vector;
                }
                else if (MR.transform.name.Contains("-Inter") && MR.transform.name.Contains("-Lane"))
                {
                    vector = new Vector3(0f, 0.02f, 0f);
                    MR.transform.localPosition = vector;
                }
                else if (MR.transform.name.Contains("-Inter") && MR.transform.name.Contains("-Stretch"))
                {
                    vector = new Vector3(0f, 0.03f, 0f);
                    MR.transform.localPosition = vector;
                }
                else if (MR.transform.name.Contains("-Inter") && MR.transform.name.Contains("-Tiled"))
                {
                    vector = new Vector3(0f, 0.01f, 0f);
                    MR.transform.localPosition = vector;
                }
            }
        }


        /// <summary> Sets localPosition up by 0.01 </summary>
        private void FixDisplayWin()
        {
            //This road:
            Object[] markerObjs = transform.GetComponentsInChildren<MeshRenderer>();
            Vector3 vector;
            foreach (MeshRenderer MR in markerObjs)
            {
                if (MR.transform.name.Contains("Marker"))
                {
                    vector = new Vector3(0f, 0.01f, 0f);
                    MR.transform.localPosition = vector;
                }
            }


            //Intersections (all):
            markerObjs = FindObjectsOfType<MeshRenderer>();
            foreach (MeshRenderer MR in markerObjs)
            {
                if (MR.transform.name.Contains("-Inter") && MR.transform.name.Contains("-Lane"))
                {
                    vector = new Vector3(0f, 0.01f, 0f);
                    MR.transform.localPosition = vector;
                }
                else if (MR.transform.name.Contains("-Inter") && MR.transform.name.Contains("-Stretch"))
                {
                    vector = new Vector3(0f, 0.01f, 0f);
                    MR.transform.localPosition = vector;
                }
            }
        }
    }
}
