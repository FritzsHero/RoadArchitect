#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
#endregion


//=====================================================
//==   NOTE THAT CUSTOM SERIALIZATION IS USED HERE   ==
//==     SOLELY TO COMPLY WITH UNDO REQUIREMENTS 	 ==
//=====================================================


namespace RoadArchitect
{
    [CustomEditor(typeof(SplineN))]
    public class SplineNEditor : Editor
    {
        #region "Vars"
        private SplineN node;
        private int count = -1;
        private int currentCount = 0;
        public bool isSplinatedObjectHelp = false;
        public bool isEdgeObjectHelp = false;
        private bool isMouseDragProcessed = true;
        private bool isRemovingAll = false;
        private float horizRoadMax = 0;

        private SplineN node1 = null;
        private SplineN node2 = null;
        private bool isCreatingIntersection = false;


        #region "Button icons"
        private Texture deleteButtonTexture = null;
        private Texture copyButtonTexture = null;
        private Texture saveButtonTexture = null;
        private Texture loadButtonTexture = null;
        private Texture extrudeButtonTexture = null;
        private Texture edgeButtonTexture = null;
        private Texture helpButtonTexture = null;
        private Texture refreshButtonTexture = null;
        private Texture defaultButtonTexture = null;
        private Texture2D loadButtonBG = null;
        private Texture2D textAreaBG = null;
        private Texture2D loadButtonBGGlow = null;
        private Texture2D manualBG = null;
        #endregion


        public bool isLoadingEOS = false;
        public int loadingEOSIndex = 0;
        public List<string> loadingEOSNames = null;
        public List<string> loadingEOSPaths = null;

        public bool isLoadingEOM = false;
        public int loadingEOMIndex = 0;
        public List<string> loadingEOMNames = null;
        public List<string> loadingEOMPaths = null;

        //Checkers:
        private Splination.SplinatedMeshMaker SMM = null;
        private EndObjectsDefaultsEnum endObjectAdd = EndObjectsDefaultsEnum.None;
        private SMMDefaultsEnum SMMQuickAdd = SMMDefaultsEnum.None;
        private BridgeTopBaseDefaultsEnum BridgeTopBaseQuickAdd = BridgeTopBaseDefaultsEnum.None;
        private BridgeBottomBaseDefaultsEnum BridgeBottomBaseQuickAdd = BridgeBottomBaseDefaultsEnum.None;
        private HorizMatchingDefaultsEnum horizMatching = HorizMatchingDefaultsEnum.None;

        //BridgeWizardDefaultsEnum bridgeWizardQuickAdd = BridgeWizardDefaultsEnum.None;
        //RoadArchitect.Splination.CollisionTypeEnum collisionType = RoadArchitect.Splination.CollisionTypeEnum.SimpleMeshTriangle;
        //RoadArchitect.Splination.RepeatUVTypeEnum repeatUVType = RoadArchitect.Splination.RepeatUVTypeEnum.None;
        private EdgeObjects.EdgeObjectMaker EOM = null;
        private GUIStyle imageButton = null;
        private GUIStyle loadButton = null;
        private GUIStyle manualButton = null;
        private GUIStyle guiButton = null;

        private bool isSceneRectSet = false;
        private Rect sceneRect = default(Rect);

        private bool isInitialized = false;
        private bool isGizmosEnabled = false;

        // Bridge
        private bool isBridgeStart = false;
        private bool isBridgeEnd = false;
        private bool isRoadCut = false;

        private string[] HorizMatchSubTypeDescriptions;
        #endregion


        #region "Enums"
        public enum EndObjectsDefaultsEnum
        {
            None,
            WarningSign1_Static,
            WarningSign2_Static,
            Atten_Static,
            Barrel1_Static,
            Barrel1_Rigid,
            Barrel3_Static,
            Barrel3_Rigid,
            Barrel7_Static,
            Barrel7_Rigid
        };


        private static string[] EndObjectsDefaultsEnumDesc = new string[]{
        "Quick add",
        "WarningSign1",
        "WarningSign2",
        "Attenuator",
        "1 Sand barrel Static",
        "1 Sand barrel Rigid",
        "3 Sand barrels Static",
        "3 Sand barrels Rigid",
        "7 Sand barrels Static",
        "7 Sand barrels Rigid"
    };


        public enum SMMDefaultsEnum
        {
            None, Custom,
            KRail,
            WBeamR,
            WBeamL,
            Railing1,
            Railing2,
            Railing3,
            Railing4,
            RailingBase05m,
            RailingBase1m
        };


        public enum BridgeTopBaseDefaultsEnum
        {
            None,
            BaseExact,
            Base1MOver,
            Base2MOver,
            Base3MDeep,
        };


        public enum BridgeBottomBaseDefaultsEnum
        {
            None,
            BridgeBase6,
            BridgeBase7,
            BridgeBase8,
            BridgeBaseGrid,
            BridgeSteel,
            BridgeBase2,
            BridgeBase3,
            BridgeBase4,
            BridgeBase5,
        };


        public enum BridgeWizardDefaultsEnum
        {
            None,
            ArchBridge12m,
            ArchBridge24m,
            ArchBridge48m,
            SuspensionBridgeSmall,
            SuspensionBridgeLarge,
            CausewayBridge1,
            CausewayBridge2,
            CausewayBridge3,
            CausewayBridge4,
            ArchBridge1,
            ArchBridge2,
            ArchBridge3,
            GridBridge,
            SteelBeamBridge
        };


        public enum HorizMatchingDefaultsEnum
        {
            None,
            MatchCenter,
            MatchRoadLeft,
            MatchShoulderLeft,
            MatchRoadRight,
            MatchShoulderRight
        };


        public enum EOMDefaultsEnum { None, Custom, StreetLightSingle, StreetLightDouble };


        private static string[] TheAxisDescriptionsSpline = new string[]{
        "X axis",
        "Z axis"
    };


        private static string[] RepeatUVTypeDescriptionsSpline = new string[]{
        "None",
        "X axis",
        "Y axis"
    };


        private static string[] TheCollisionTypeEnumDescSpline = new string[]{
        "None",
        "Simple triangle",
        "Simple trapezoid",
        "Meshfilter collision mesh",
        "Straight line box collider"
    };
        #endregion


        private void OnEnable()
        {
            node = (SplineN)target;
        }


        private void Init()
        {
            isInitialized = true;
            EditorStyles.label.wordWrap = true;
            EditorStyles.miniLabel.wordWrap = true;
            string basePath = RoadEditorUtility.GetBasePath();

            EditorUtilities.LoadTexture(ref deleteButtonTexture, basePath + "/Editor/Icons/delete.png");
            EditorUtilities.LoadTexture(ref copyButtonTexture, basePath + "/Editor/Icons/copy.png");
            EditorUtilities.LoadTexture(ref loadButtonTexture, basePath + "/Editor/Icons/load.png");
            EditorUtilities.LoadTexture(ref saveButtonTexture, basePath + "/Editor/Icons/save.png");
            EditorUtilities.LoadTexture(ref extrudeButtonTexture, basePath + "/Editor/Icons/extrude.png");
            EditorUtilities.LoadTexture(ref edgeButtonTexture, basePath + "/Editor/Icons/edge.png");
            EditorUtilities.LoadTexture(ref helpButtonTexture, basePath + "/Editor/Icons/help.png");
            EditorUtilities.LoadTexture(ref refreshButtonTexture, basePath + "/Editor/Icons/refresh.png");
            EditorUtilities.LoadTexture(ref defaultButtonTexture, basePath + "/Editor/Icons/refresh2.png");
            EditorUtilities.LoadTexture(ref textAreaBG, basePath + "/Editor/Icons/popupbg.png");
            EditorUtilities.LoadTexture(ref loadButtonBG, basePath + "/Editor/Icons/loadbg.png");
            EditorUtilities.LoadTexture(ref loadButtonBGGlow, basePath + "/Editor/Icons/loadbgglow.png");
            EditorUtilities.LoadTexture(ref manualBG, basePath + "/Editor/Icons/manualbg.png");

            if (imageButton == null)
            {
                imageButton = new GUIStyle(GUI.skin.button);
                imageButton.contentOffset = new Vector2(0f, -2f);
                imageButton.border = new RectOffset(0, 0, 0, 0);
                imageButton.fixedHeight = 16f;
                imageButton.padding = new RectOffset(0, 0, 0, 0);
                imageButton.normal.background = null;
            }

            if (loadButton == null)
            {
                loadButton = new GUIStyle(GUI.skin.button);
                loadButton.contentOffset = new Vector2(0f, 1f);
                loadButton.normal.textColor = new Color(1f, 1f, 1f, 1f);
                loadButton.normal.background = loadButtonBG;
                loadButton.active.background = loadButtonBGGlow;
                loadButton.focused.background = loadButtonBGGlow;
                loadButton.hover.background = loadButtonBGGlow;
                loadButton.fixedHeight = 16f;
                loadButton.fixedWidth = 128f;
                loadButton.padding = new RectOffset(0, 35, 0, 0);
            }

            if (manualButton == null)
            {
                manualButton = new GUIStyle(GUI.skin.button);
                manualButton.contentOffset = new Vector2(0f, 1f);
                manualButton.normal.textColor = new Color(1f, 1f, 1f, 1f);
                manualButton.normal.background = manualBG;
                manualButton.fixedHeight = 16f;
                manualButton.fixedWidth = 128f;
            }

            if (guiButton == null)
            {
                guiButton = new GUIStyle(GUI.skin.button);
                guiButton.normal.textColor = new Color(0.5f, 1f, 0.5f, 1f);
            }

            float halfRoadWidth = node.spline.road.RoadWidth() * 0.5f;
            HorizMatchSubTypeDescriptions = new string[6];
            HorizMatchSubTypeDescriptions[0] = "Select preset";
            HorizMatchSubTypeDescriptions[1] = "Match center: 0 meters";
            HorizMatchSubTypeDescriptions[2] = "Match road left edge: -" + halfRoadWidth.ToString("F1") + " meters";
            HorizMatchSubTypeDescriptions[4] = "Match road right edge: " + halfRoadWidth.ToString("F1") + " meters";

            if (node.spline.road.isShouldersEnabled)
            {
                HorizMatchSubTypeDescriptions[3] = "Match shoulder left edge: -" + (halfRoadWidth + node.spline.road.shoulderWidth).ToString("F1") + " meters";
                HorizMatchSubTypeDescriptions[5] = "Match shoulder right edge: " + (halfRoadWidth + node.spline.road.shoulderWidth).ToString("F1") + " meters";
            }
            else
            {
                HorizMatchSubTypeDescriptions[2] = "Match shoulder left edge: -" + halfRoadWidth.ToString("F1") + " meters";
                HorizMatchSubTypeDescriptions[4] = "Match shoulder right edge: " + halfRoadWidth.ToString("F1") + " meters";
            }

            horizRoadMax = node.spline.road.RoadWidth() * 20;
        }


        public override void OnInspectorGUI()
        {
            #region "Event"
            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        UpdateSplineObjectsOnUndo();
                        break;
                }
            }

            if (Event.current.type != EventType.Layout && isCreatingIntersection)
            {
                isCreatingIntersection = false;
                Selection.activeGameObject = Intersections.CreateIntersection(node1, node2);
                return;
            }


            if (Event.current.type != EventType.Layout && node.isQuitGUI)
            {
                node.isQuitGUI = false;
                return;
            }
            #endregion


            //Graphic null checks:
            if (!isInitialized)
            {
                Init();
            }

            EditorUtilities.DrawLine();

            #region "Manuals on Top of SplineN Scripts"
            EditorGUILayout.LabelField(node.editorDisplayString, EditorStyles.boldLabel);

            if (GUILayout.Button("Online manual", EditorStyles.miniButton, GUILayout.Width(128f)))
            {
                // formerly, which redirect to the master github http://microgsd.com/Support/RoadArchitectManual.aspx
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");
            }

            if (GUILayout.Button("Offline manual", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                RoadArchitect.EditorUtilities.OpenOfflineManual();
            }
            #endregion


            #region Option: Gizmo options, Convoluted due to submission compliance for undo rules:
            if (node.spline.road.isGizmosEnabled != node.isGizmosEnabled)
            {
                node.spline.road.isGizmosEnabled = node.isGizmosEnabled;
                node.spline.road.UpdateGizmoOptions();
                node.spline.road.WireframesToggle();
            }
            isGizmosEnabled = EditorGUILayout.Toggle("Gizmos: ", node.spline.road.isGizmosEnabled);
            #endregion


            #region "Option: Manual road cut"
            if (node.idOnSpline > 0 && node.idOnSpline < (node.spline.GetNodeCount() - 1) && !node.isIntersection && !node.isSpecialEndNode)
            {
                // && !cNode.bIsBridge_PreNode && !cNode.bIsBridge_PostNode){
                if (node.spline.road.isDynamicCutsEnabled)
                {
                    EditorUtilities.DrawLine();
                    isRoadCut = EditorGUILayout.Toggle("Cut road at this node: ", node.isRoadCut);
                }
                EditorUtilities.DrawLine();
            }
            #endregion


            #region "Option: Bridge options"
            bool isBridgeDisplayed = false;
            if (!node.isEndPoint)
            {
                //Bridge start:
                if (!node.isBridgeEnd && node.CanBridgeStart())
                {
                    isBridgeStart = EditorGUILayout.Toggle(" Bridge start", node.isBridgeStart);
                    isBridgeDisplayed = true;
                }
                //Bridge end:
                if (!node.isBridgeStart && node.CanBridgeEnd())
                {
                    isBridgeEnd = EditorGUILayout.Toggle(" Bridge end", node.isBridgeEnd);
                    isBridgeDisplayed = true;
                }

                if (isBridgeDisplayed)
                {
                    RoadArchitect.EditorUtilities.DrawLine();
                }
            }
            #endregion


            if ((Selection.objects.Length == 1 && Selection.objects[0] is SplineN) || (node.specialNodeCounterpart == null && !node.isSpecialRoadConnPrimary))
            {
                //Do extrusion and edge objects overview:
                DoExtAndEdgeOverview();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Road objects"))
                {
                    Selection.objects = new Object[1] { node.specialNodeCounterpart };
                }
                EditorGUILayout.EndHorizontal();
            }

            if (node.isSpecialRoadConnPrimary)
            {
                EditorGUILayout.LabelField("Road connection:", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Update road connection"))
                {
                    SplineN node1 = node.originalConnectionNodes[0];
                    SplineN node2 = node.originalConnectionNodes[1];
                    node.specialNodeCounterpart.BreakConnection();
                    node.spline.road.UpdateRoad();
                    node1.spline.ActivateEndNodeConnection(node1, node2);
                }
                if (GUILayout.Button("Break road connection"))
                {
                    node.specialNodeCounterpart.BreakConnection();
                }
                if (GUILayout.Button("Access objects on other node"))
                {
                    Selection.objects = new Object[] { node.specialNodeCounterpart };
                }
                EditorGUILayout.EndVertical();
                if (node.specialNodeCounterpart != null)
                {
                    EditorGUILayout.LabelField(node.specialNodeCounterpart.spline.road.transform.name + " to " + node.specialNodeCounterpart.specialNodeCounterpart.spline.road.transform.name);
                }
                EditorGUILayout.LabelField("To break this road connection, click the \"Break road connection\" button.");
                EditorUtilities.DrawLine();
            }

            //Statistics:
            DoStats();
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                //Set snapshot for undo:

                Undo.RecordObject(node, "Modify node");

                //Option: Gizmo options, Convoluted due to submission compliance for undo rules:
                if (isGizmosEnabled != node.spline.road.isGizmosEnabled)
                {
                    node.spline.road.isGizmosEnabled = isGizmosEnabled;
                    node.spline.road.UpdateGizmoOptions();
                    node.spline.road.WireframesToggle();
                    SceneView.RepaintAll();
                }

                //Option: Manual cut:
                if (node.idOnSpline > 0 && node.idOnSpline < (node.spline.GetNodeCount() - 1) && !node.isIntersection && !node.isSpecialEndNode)
                {
                    // && !cNode.bIsBridge_PreNode && !cNode.bIsBridge_PostNode){
                    if (node.spline.road.isDynamicCutsEnabled)
                    {
                        if (isRoadCut != node.isRoadCut)
                        {
                            node.isRoadCut = isRoadCut;
                        }
                    }
                }

                #region "Option: Bridge options"
                //Bridge start:
                if (!node.isEndPoint)
                {
                    if (!node.isBridgeEnd && node.CanBridgeStart())
                    {
                        if (isBridgeStart != node.isBridgeStart)
                        {
                            node.isBridgeStart = isBridgeStart;
                            node.BridgeToggleStart();
                        }
                    }
                }
                //Bridge end:
                if (!node.isEndPoint)
                {
                    if (!node.isBridgeStart && node.CanBridgeEnd())
                    {
                        if (isBridgeEnd != node.isBridgeEnd)
                        {
                            node.isBridgeEnd = isBridgeEnd;
                            node.BridgeToggleEnd();
                        }
                    }
                }
                #endregion

                UpdateSplineObjects();
                UpdateEdgeObjects();

                EditorUtility.SetDirty(target);
            }
        }


        private void OnSelectionChanged()
        {
            Repaint();
        }


        private void DoExtAndEdgeOverview()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extrusion & edge objects", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (isEdgeObjectHelp)
            {
                isEdgeObjectHelp = EditorGUILayout.Foldout(isEdgeObjectHelp, "Hide quick help");
            }
            else
            {
                isEdgeObjectHelp = EditorGUILayout.Foldout(isEdgeObjectHelp, "Show quick help");
            }
            EditorGUILayout.LabelField("");

            if (GUILayout.Button("Save group", EditorStyles.miniButton, GUILayout.Width(108f)) || GUILayout.Button(saveButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                SaveWindow saveWindow = EditorWindow.GetWindow<SaveWindow>();
                if (node.isBridge)
                {
                    saveWindow.Initialize(ref sceneRect, SaveWindow.WindowTypeEnum.BridgeWizard, node);
                }
                else
                {
                    saveWindow.Initialize(ref sceneRect, SaveWindow.WindowTypeEnum.BridgeWizard, node);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4f);


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("Open Wizard", loadButton, GUILayout.Width(128f)))
            {
                // || GUILayout.Button(btnLoadText,GSDImageButton,GUILayout.Width(16f))){
                Wizard wizard = EditorWindow.GetWindow<Wizard>();
                if (sceneRect.x < 0)
                {
                    sceneRect.x = 0f;
                }
                if (sceneRect.y < 0)
                {
                    sceneRect.y = 0f;
                }
                wizard.rect = sceneRect;
                if (node.isBridgeStart)
                {
                    wizard.Initialize(Wizard.WindowTypeEnum.BridgeComplete, node);
                }
                else
                {
                    wizard.Initialize(Wizard.WindowTypeEnum.Extrusion, node);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            
            GUILayout.Space(4f);

            if (isEdgeObjectHelp)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(extrudeButtonTexture, imageButton, GUILayout.Width(32f)))
                { }
                EditorGUILayout.LabelField("= Extrusion objects", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Railings, bridge pieces, center dividers and other connected objects.", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(edgeButtonTexture, imageButton, GUILayout.Width(32f)))
                { }
                EditorGUILayout.LabelField("= Edge objects", EditorStyles.miniLabel);
                EditorGUILayout.LabelField("");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Signs, street lights, bridge pillars and other unconnected road objects.", EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(saveButtonTexture, imageButton, GUILayout.Width(16f)))
                { }
                EditorGUILayout.LabelField("= Saves object config to library for use on other nodes.", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(copyButtonTexture, imageButton, GUILayout.Width(16f)))
                { }
                EditorGUILayout.LabelField("= Duplicates object onto current node.", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                { }
                EditorGUILayout.LabelField("= Deletes object.", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                { }
                EditorGUILayout.LabelField("= Refreshes object.", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                { }
                EditorGUILayout.LabelField("= Resets setting(s) to default.", EditorStyles.miniLabel);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                EditorUtilities.DrawLine();
            }
            currentCount = 0;

            GUILayout.Space(2f);


            //Splinated objects:
            DoSplineObjects();

            //Edge Objects:
            DoEdgeObjects();

            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("Add custom extrusion object", EditorStyles.miniButton))
            {
                node.AddSplinatedObject();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            if (GUILayout.Button("Add custom edge object", EditorStyles.miniButton))
            {
                node.AddEdgeObject();
            }
            EditorGUILayout.EndHorizontal();

            if (node.SplinatedObjects.Count > 20 || node.EdgeObjects.Count > 20)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("");
                isRemovingAll = EditorGUILayout.Toggle(isRemovingAll, GUILayout.Width(20f));
                if (GUILayout.Button("Remove all", EditorStyles.miniButton, GUILayout.Width(100f)))
                {
                    if (isRemovingAll)
                    {
                        node.RemoveAllSplinatedObjects();
                        node.RemoveAllEdgeObjects();
                        isRemovingAll = false;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorUtilities.DrawLine();
        }


        private void DoStats()
        {
            EditorGUILayout.LabelField("Statistics:");
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Grade to next node: " + node.gradeToNext);
            EditorGUILayout.LabelField("Grade to prev node: " + node.gradeToPrev);
            EditorGUILayout.LabelField("Distance from start: " + node.dist.ToString("F3") + " meters");
            EditorGUILayout.LabelField("% of spline: " + ((node.dist / node.spline.distance) * 100f).ToString("F2") + "%");
            EditorGUILayout.LabelField("Parameter: " + node.time);
            EditorGUILayout.LabelField("Tangent: " + node.tangent);
            EditorGUILayout.LabelField("POS: " + node.pos);
            EditorGUILayout.LabelField("ID on spline: " + node.idOnSpline);
            EditorGUILayout.LabelField("Is intersection node: " + node.isIntersection);
            EditorGUILayout.LabelField("Is end node: " + node.isEndPoint);
            EditorGUILayout.LabelField("Is bridge start: " + node.isBridgeStart);
            EditorGUILayout.LabelField("Is bridge end: " + node.isBridgeEnd);
            EditorGUILayout.LabelField("Road: " + node.spline.road.transform.name);
            EditorGUILayout.LabelField("System: " + node.spline.road.roadSystem.transform.name);
            EditorGUILayout.SelectableLabel("UID: " + node.uID);
        }


        public void DoSplineObjects()
        {
            if (!node.CanSplinate())
            {
                return;
            }
            if (node.SplinatedObjects == null)
            {
                node.SplinatedObjects = new List<Splination.SplinatedMeshMaker>();
            }
            count = node.SplinatedObjects.Count;

            SMM = null;
            count = node.SplinatedObjects.Count;
            if (count == 0)
            {

            }

            for (int index = 0; index < node.SplinatedObjects.Count; index++)
            {
                currentCount += 1;
                SMM = node.SplinatedObjects[index];
                if (SMM.splinatedMeshEditorMaker == null)
                {
                    SMM.splinatedMeshEditorMaker = new Splination.SplinatedMeshMaker.SplinatedMeshEditorMaker();
                }
                SMM.splinatedMeshEditorMaker.Setup(SMM);

                //RoadArchitect.Splination.AxisTypeEnum tAxisTypeSpline = RoadArchitect.Splination.AxisTypeEnum.Z;

                EditorGUILayout.BeginVertical("TextArea");

                if (SMM.isRequiringUpdate)
                {
                    SMM.Setup(true);
                }


                EditorGUILayout.BeginHorizontal();

                SMM.isToggled = EditorGUILayout.Foldout(SMM.isToggled, "#" + currentCount.ToString() + ": " + SMM.objectName);

                if (GUILayout.Button(extrudeButtonTexture, imageButton, GUILayout.Width(32f)))
                {

                }

                if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.Setup();
                }
                if (GUILayout.Button(saveButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SaveWindow saveDialog = EditorWindow.GetWindow<SaveWindow>();
                    saveDialog.Initialize(ref sceneRect, SaveWindow.WindowTypeEnum.Extrusion, node, SMM);
                }
                if (GUILayout.Button(copyButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    Undo.RecordObject(node, "Copy");
                    node.CopySplinatedObject(ref SMM);
                    EditorUtility.SetDirty(node);
                }
                if (GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    Undo.RecordObject(node, "Delete");
                    node.RemoveSplinatedObject(index);
                    EditorUtility.SetDirty(node);
                }
                EditorGUILayout.EndHorizontal();
                if (!SMM.isToggled)
                {
                    EditorGUILayout.EndVertical();
                    continue;
                }

                GUILayout.Space(8f);
                EditorGUILayout.LabelField("General options:");

                EditorGUILayout.BeginVertical("box");
                //Name:
                SMM.splinatedMeshEditorMaker.objectName = EditorGUILayout.TextField("Name:", SMM.objectName);

                //Game object (prefab):
                SMM.splinatedMeshEditorMaker.CurrentSplination = (GameObject)EditorGUILayout.ObjectField("Prefab:", SMM.currentSplination, typeof(GameObject), false);

                //Game object (prefab start cap):
                SMM.splinatedMeshEditorMaker.CurrentSplinationCap1 = (GameObject)EditorGUILayout.ObjectField("Prefab start cap:", SMM.currentSplinationCap1, typeof(GameObject), false);
                //Prefab start cap height offset:
                if (SMM.currentSplinationCap1 != null)
                {
                    SMM.splinatedMeshEditorMaker.CapHeightOffset1 = EditorGUILayout.FloatField("  Height offset:", SMM.capHeightOffset1);
                }

                //Game object (prefab end cap):
                SMM.splinatedMeshEditorMaker.CurrentSplinationCap2 = (GameObject)EditorGUILayout.ObjectField("Prefab end cap:", SMM.currentSplinationCap2, typeof(GameObject), false);
                //Prefab end cap height offset:
                if (SMM.currentSplinationCap2 != null)
                {
                    SMM.splinatedMeshEditorMaker.CapHeightOffset2 = EditorGUILayout.FloatField("  Height offset:", SMM.capHeightOffset2);
                }

                //Material overrides:
                SMM.splinatedMeshEditorMaker.isMaterialOverriden = EditorGUILayout.Toggle("Material override: ", SMM.isMaterialOverriden);
                if (SMM.isMaterialOverriden)
                {
                    SMM.splinatedMeshEditorMaker.SplinatedMaterial1 = (Material)EditorGUILayout.ObjectField("Override mat #1: ", SMM.SplinatedMaterial1, typeof(Material), false);
                    SMM.splinatedMeshEditorMaker.SplinatedMaterial2 = (Material)EditorGUILayout.ObjectField("Override mat #2: ", SMM.SplinatedMaterial2, typeof(Material), false);
                }

                //Axis:
                SMM.splinatedMeshEditorMaker.Axis = (Splination.AxisTypeEnum)EditorGUILayout.Popup("Extrusion axis: ", (int)SMM.Axis, TheAxisDescriptionsSpline, GUILayout.Width(250f));

                //Start time:
                if (SMM.StartTime < node.minSplination)
                {
                    SMM.StartTime = node.minSplination;
                }
                if (SMM.EndTime > node.maxSplination)
                {
                    SMM.EndTime = node.maxSplination;
                }
                EditorGUILayout.BeginHorizontal();

                SMM.splinatedMeshEditorMaker.StartTime = EditorGUILayout.Slider("Start param: ", SMM.StartTime, node.minSplination, node.maxSplination - 0.01f);
                if (GUILayout.Button("match node", EditorStyles.miniButton, GUILayout.Width(80f)))
                {
                    SMM.splinatedMeshEditorMaker.StartTime = node.time;
                }
                if (SMM.splinatedMeshEditorMaker.StartTime >= SMM.splinatedMeshEditorMaker.EndTime)
                {
                    SMM.splinatedMeshEditorMaker.EndTime = (SMM.splinatedMeshEditorMaker.StartTime + 0.01f);
                }
                EditorGUILayout.EndHorizontal();


                //End time:
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.EndTime = EditorGUILayout.Slider("End param: ", SMM.EndTime, SMM.StartTime, node.maxSplination);
                if (GUILayout.Button("match next", EditorStyles.miniButton, GUILayout.Width(80f)))
                {
                    SMM.splinatedMeshEditorMaker.EndTime = node.nextTime;
                }
                if (SMM.splinatedMeshEditorMaker.StartTime >= SMM.splinatedMeshEditorMaker.EndTime)
                {
                    SMM.splinatedMeshEditorMaker.EndTime = (SMM.splinatedMeshEditorMaker.StartTime + 0.01f);
                }
                EditorGUILayout.EndHorizontal();


                //Straight line options:
                if (node.IsStraight())
                {
                    if (!SMM.isStretched)
                    {
                        SMM.splinatedMeshEditorMaker.isStretched = EditorGUILayout.Toggle("Straight line stretch:", SMM.isStretched);
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical("box");
                        SMM.splinatedMeshEditorMaker.isStretched = EditorGUILayout.Toggle("Straight line stretch:", SMM.isStretched);

                        //Stretch_UVThreshold:
                        SMM.splinatedMeshEditorMaker.stretchedUVThreshold = EditorGUILayout.Slider("UV stretch threshold:", SMM.stretchUVThreshold, 0.01f, 0.5f);

                        //UV repeats:
                        SMM.splinatedMeshEditorMaker.repeatUVType = (Splination.RepeatUVTypeEnum)EditorGUILayout.Popup("UV stretch axis: ", (int)SMM.RepeatUVType, RepeatUVTypeDescriptionsSpline, GUILayout.Width(250f));
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    SMM.splinatedMeshEditorMaker.isStretched = false;
                }



                SMM.splinatedMeshEditorMaker.isTrimStart = EditorGUILayout.Toggle("Trim start:", SMM.isTrimStart);
                SMM.splinatedMeshEditorMaker.isTrimEnd = EditorGUILayout.Toggle("Trim end:", SMM.isTrimEnd);



                //Static option:
                SMM.splinatedMeshEditorMaker.isStatic = EditorGUILayout.Toggle("Static: ", SMM.isStatic);


                //Splination method
                //			SMM.EM.bMatchRoadIncrements = EditorGUILayout.Toggle("Match road increments: ",SMM.bMatchRoadIncrements); 
                SMM.splinatedMeshEditorMaker.isMatchingTerrain = EditorGUILayout.Toggle("Match ground: ", SMM.isMatchingTerrain);

                //Vector min/max threshold: 
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.MinMaxMod = EditorGUILayout.Slider("Vertex min/max threshold: ", SMM.minMaxMod, 0.01f, 0.2f);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.splinatedMeshEditorMaker.MinMaxMod = 0.05f;
                }
                EditorGUILayout.EndHorizontal();

                //Vertex matching precision:
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.VertexMatchingPrecision = EditorGUILayout.Slider("Vertex matching precision: ", SMM.vertexMatchingPrecision, 0f, 0.01f);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.splinatedMeshEditorMaker.VertexMatchingPrecision = 0.005f;
                }
                EditorGUILayout.EndHorizontal();

                //UV repeats:
                if (!SMM.isStretched)
                {
                    SMM.splinatedMeshEditorMaker.repeatUVType = (Splination.RepeatUVTypeEnum)EditorGUILayout.Popup("UV repeat axis: ", (int)SMM.RepeatUVType, RepeatUVTypeDescriptionsSpline, GUILayout.Width(250f));
                }

                if (SMM.isMatchingRoadDefinition)
                {
                    EditorGUILayout.BeginVertical("TextArea");
                    EditorGUILayout.BeginHorizontal();
                    SMM.splinatedMeshEditorMaker.isMatchingRoadDefinition = EditorGUILayout.Toggle("Match road definition: ", SMM.isMatchingRoadDefinition);
                    if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        SMM.splinatedMeshEditorMaker.isMatchingRoadDefinition = false;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (SMM.isMatchingRoadDefinition)
                    {
                        EditorGUILayout.LabelField("  Only use this option if object length doesn't match the road definition.", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField("  Matching road definition requires a UV repeat type.", EditorStyles.miniLabel);
                        EditorGUILayout.LabelField("  If the material fails to scale properly, try flipping the Y rotation.", EditorStyles.miniLabel);
                    }
                    //Flip rotation option:
                    SMM.splinatedMeshEditorMaker.isFlippedRotation = EditorGUILayout.Toggle("  Flip Y rotation: ", SMM.isFlippingRotation);
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    SMM.splinatedMeshEditorMaker.isMatchingRoadDefinition = EditorGUILayout.Toggle("Match road definition: ", SMM.isMatchingRoadDefinition);
                    if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        SMM.splinatedMeshEditorMaker.isMatchingRoadDefinition = false;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();


                #region "Vertical offset"
                EditorGUILayout.LabelField("Vertical options:");
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.VerticalRaise = EditorGUILayout.Slider("Vertical raise magnitude:", SMM.verticalRaise, -512f, 512f);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.splinatedMeshEditorMaker.VerticalRaise = 0f;
                }
                EditorGUILayout.EndHorizontal();
                #endregion


                #region "Vertical curve"
                if (SMM.verticalCurve == null || SMM.verticalCurve.keys.Length < 2)
                {
                    EnforceCurve(ref SMM.verticalCurve);
                }
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.VerticalCurve = EditorGUILayout.CurveField("Curve: ", SMM.verticalCurve);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    ResetCurve(ref SMM.splinatedMeshEditorMaker.VerticalCurve);
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                EditorGUILayout.EndVertical();


                #region "Horizontal offsets"
                SMM.splinatedMeshEditorMaker.HorizontalSep = SMM.horizontalSep;
                EditorGUILayout.LabelField("Horizontal offset options:");
                EditorGUILayout.BeginVertical("box");
                horizMatching = HorizMatchingDefaultsEnum.None;
                horizMatching = (HorizMatchingDefaultsEnum)EditorGUILayout.Popup((int)horizMatching, HorizMatchSubTypeDescriptions, GUILayout.Width(100f));
                if (horizMatching != HorizMatchingDefaultsEnum.None)
                {
                    if (horizMatching == HorizMatchingDefaultsEnum.MatchCenter)
                    {
                        SMM.splinatedMeshEditorMaker.HorizontalSep = 0f;
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchRoadLeft)
                    {
                        SMM.splinatedMeshEditorMaker.HorizontalSep = (node.spline.road.RoadWidth() / 2) * -1;
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchShoulderLeft)
                    {
                        SMM.splinatedMeshEditorMaker.HorizontalSep = ((node.spline.road.RoadWidth() / 2) + node.spline.road.shoulderWidth) * -1;
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchRoadRight)
                    {
                        SMM.splinatedMeshEditorMaker.HorizontalSep = (node.spline.road.RoadWidth() / 2);
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchShoulderRight)
                    {
                        SMM.splinatedMeshEditorMaker.HorizontalSep = (node.spline.road.RoadWidth() / 2) + node.spline.road.shoulderWidth;
                    }
                    horizMatching = HorizMatchingDefaultsEnum.None;
                }
                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.HorizontalSep = EditorGUILayout.Slider("Horiz offset magnitude:", SMM.splinatedMeshEditorMaker.HorizontalSep, (-1f * horizRoadMax), horizRoadMax);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.splinatedMeshEditorMaker.HorizontalSep = 0f;
                }
                EditorGUILayout.EndHorizontal();
                #endregion


                //Horizontal curve:
                if (SMM.horizontalCurve == null || SMM.horizontalCurve.keys.Length < 2)
                {
                    EnforceCurve(ref SMM.horizontalCurve);
                }

                EditorGUILayout.BeginHorizontal();
                SMM.splinatedMeshEditorMaker.HorizontalCurve = EditorGUILayout.CurveField("Curve: ", SMM.horizontalCurve);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    ResetCurve(ref SMM.splinatedMeshEditorMaker.HorizontalCurve);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                //Vertical cutoff:
                EditorGUILayout.LabelField("Vertical cutoff:");
                EditorGUILayout.BeginVertical("box");
                SMM.splinatedMeshEditorMaker.isVerticalCutoff = EditorGUILayout.Toggle("Height cutoff enabled:", SMM.isVerticalCutoff);
                if (SMM.isVerticalCutoff)
                {
                    SMM.splinatedMeshEditorMaker.isVerticalCutoffMatchingZero = EditorGUILayout.Toggle("Match spline height:", SMM.isVerticalCutoffMatchingZero);
                    SMM.splinatedMeshEditorMaker.isVerticalCutoffDownwards = EditorGUILayout.Toggle("Cut direction toggle:", SMM.isVerticalCutoffDownwards);
                    SMM.splinatedMeshEditorMaker.VerticalCutoff = EditorGUILayout.Slider("Height cut offset: ", SMM.VerticalCutoff, -50f, 50f);
                    SMM.splinatedMeshEditorMaker.isVerticalMeshCutoffOppositeDir = EditorGUILayout.Toggle("Opposite dir mesh cut:", SMM.isVerticalMeshCutoffOppositeDir);
                    SMM.splinatedMeshEditorMaker.VerticalMeshCutoffOffset = EditorGUILayout.Slider("Mesh cut offset: ", SMM.VerticalMeshCutoffOffset, -5f, 5f);
                }
                EditorGUILayout.EndVertical();

                //End type:
                EditorGUILayout.LabelField("Extrusion ending options:");
                EditorGUILayout.BeginVertical("box");
                SMM.splinatedMeshEditorMaker.isStartDown = EditorGUILayout.Toggle("Push start down:", SMM.isStartDown);
                SMM.splinatedMeshEditorMaker.isEndDown = EditorGUILayout.Toggle("Push end down:", SMM.isEndDown);
                if (SMM.isStartDown)
                {
                    SMM.splinatedMeshEditorMaker.isStartTypeDownOverriden = EditorGUILayout.Toggle("Override start down value: ", SMM.isStartTypeDownOverriden);
                    if (SMM.isStartTypeDownOverriden)
                    {
                        SMM.splinatedMeshEditorMaker.startTypeDownOverriden = EditorGUILayout.Slider("Downward movement: ", SMM.startTypeDownOverride, -10f, 10f);
                    }
                }
                if (SMM.isEndDown)
                {
                    SMM.splinatedMeshEditorMaker.isEndTypeDownOverriden = EditorGUILayout.Toggle("Override end down value: ", SMM.isEndTypeDownOverriden);
                    if (SMM.isEndTypeDownOverriden)
                    {
                        SMM.splinatedMeshEditorMaker.endTypeDownOverriden = EditorGUILayout.Slider("Downward movement: ", SMM.endTypeDownOverride, -10f, 10f);
                    }
                }
                EditorGUILayout.EndVertical();

                //Start and end objects:
                EditorGUILayout.LabelField("Start & end objects:");
                EditorGUILayout.BeginVertical("box");
                //End cap custom match start:
                SMM.splinatedMeshEditorMaker.isEndCapCustomMatchingStart = EditorGUILayout.Toggle("Match objects to ends:", SMM.isEndCapCustomMatchStart);

                //End objects match ground:
                SMM.splinatedMeshEditorMaker.isEndObjectsMatchingGround = EditorGUILayout.Toggle("Force origins to ground:", SMM.isEndObjectsMatchingGround);

                //Start cap:
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Start object:");
                endObjectAdd = (EndObjectsDefaultsEnum)EditorGUILayout.Popup((int)endObjectAdd, EndObjectsDefaultsEnumDesc);
                if (endObjectAdd != EndObjectsDefaultsEnum.None)
                {
                    SMM.splinatedMeshEditorMaker.EndCapStart = GetEndObjectQuickAdd();
                    endObjectAdd = EndObjectsDefaultsEnum.None;
                }
                EditorGUILayout.EndHorizontal();


                SMM.splinatedMeshEditorMaker.EndCapStart = (GameObject)EditorGUILayout.ObjectField("Prefab:", SMM.EndCapStart, typeof(GameObject), false);
                if (SMM.EndCapStart != null)
                {
                    SMM.splinatedMeshEditorMaker.EndCapCustomOffsetStart = EditorGUILayout.Vector3Field("Position offset:", SMM.EndCapCustomOffsetStart);
                    SMM.splinatedMeshEditorMaker.EndCapCustomRotOffsetStart = EditorGUILayout.Vector3Field("Rotation offset:", SMM.EndCapCustomRotOffsetStart);
                }
                EditorGUILayout.EndVertical();

                //End cap:
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("End object:");
                endObjectAdd = (EndObjectsDefaultsEnum)EditorGUILayout.Popup((int)endObjectAdd, EndObjectsDefaultsEnumDesc);
                if (endObjectAdd != EndObjectsDefaultsEnum.None)
                {
                    SMM.splinatedMeshEditorMaker.EndCapEnd = GetEndObjectQuickAdd();
                    SMM.splinatedMeshEditorMaker.EndCapCustomRotOffsetEnd = new Vector3(0f, 180f, 0f);
                    endObjectAdd = EndObjectsDefaultsEnum.None;
                }
                EditorGUILayout.EndHorizontal();


                SMM.splinatedMeshEditorMaker.EndCapEnd = (GameObject)EditorGUILayout.ObjectField("Prefab:", SMM.EndCapEnd, typeof(GameObject), false);
                if (SMM.EndCapEnd != null)
                {
                    SMM.splinatedMeshEditorMaker.EndCapCustomOffsetEnd = EditorGUILayout.Vector3Field("Position offset:", SMM.EndCapCustomOffsetEnd);
                    SMM.splinatedMeshEditorMaker.EndCapCustomRotOffsetEnd = EditorGUILayout.Vector3Field("Rotation offset:", SMM.EndCapCustomRotOffsetEnd);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndVertical();

                //Collision:
                EditorGUILayout.LabelField("Collision options:");
                EditorGUILayout.BeginVertical("box");
                SMM.splinatedMeshEditorMaker.collisionType = (Splination.CollisionTypeEnum)EditorGUILayout.Popup("Collision type: ", (int)SMM.CollisionType, TheCollisionTypeEnumDescSpline, GUILayout.Width(320f));
                //Mesh collison convex option
                if (SMM.CollisionType != Splination.CollisionTypeEnum.None && SMM.CollisionType != Splination.CollisionTypeEnum.BoxCollision)
                {
                    SMM.splinatedMeshEditorMaker.isCollisionConvex = EditorGUILayout.Toggle(" Convex: ", SMM.isCollisionConvex);
                    SMM.splinatedMeshEditorMaker.isCollisionTriggered = EditorGUILayout.Toggle(" Trigger: ", SMM.isCollisionTrigger);
                }

                if (SMM.CollisionType == Splination.CollisionTypeEnum.SimpleMeshTriangle || SMM.CollisionType == Splination.CollisionTypeEnum.SimpleMeshTrapezoid)
                {
                    SMM.splinatedMeshEditorMaker.isSimpleCollisionAutomatic = EditorGUILayout.Toggle(" Automatic simple collision: ", SMM.isSimpleCollisionAutomatic);
                }
                //If not automatic simple collisions:
                if (!SMM.isSimpleCollisionAutomatic)
                {
                    if (SMM.CollisionType == Splination.CollisionTypeEnum.SimpleMeshTriangle)
                    {
                        SMM.splinatedMeshEditorMaker.CollisionTriBL = SMM.CollisionTriBL;
                        SMM.splinatedMeshEditorMaker.CollisionTriBR = SMM.CollisionTriBR;
                        SMM.splinatedMeshEditorMaker.CollisionTriT = SMM.CollisionTriT;

                        EditorGUILayout.LabelField("Bottom left:");
                        SMM.splinatedMeshEditorMaker.CollisionTriBL.x = EditorGUILayout.Slider(" x-axis: ", SMM.CollisionTriBL.x, SMM.mMinX - 5f, SMM.mMaxX + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriBL.y = EditorGUILayout.Slider(" y-axis: ", SMM.CollisionTriBL.y, SMM.mMinY - 5f, SMM.mMaxY + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriBL.z = EditorGUILayout.Slider(" z-axis: ", SMM.CollisionTriBL.z, SMM.mMinZ - 5f, SMM.mMaxZ + 5f);

                        EditorGUILayout.LabelField("Bottom right:");
                        SMM.splinatedMeshEditorMaker.CollisionTriBR.x = EditorGUILayout.Slider(" x-axis: ", SMM.CollisionTriBR.x, SMM.mMinX - 5f, SMM.mMaxX + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriBR.y = EditorGUILayout.Slider(" y-axis: ", SMM.CollisionTriBR.y, SMM.mMinY - 5f, SMM.mMaxY + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriBR.z = EditorGUILayout.Slider(" z-axis: ", SMM.CollisionTriBR.z, SMM.mMinZ - 5f, SMM.mMaxZ + 5f);

                        EditorGUILayout.LabelField("Top:");
                        SMM.splinatedMeshEditorMaker.CollisionTriT.x = EditorGUILayout.Slider(" x-axis: ", SMM.CollisionTriT.x, SMM.mMinX - 5f, SMM.mMaxX + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriT.y = EditorGUILayout.Slider(" y-axis: ", SMM.CollisionTriT.y, SMM.mMinY - 5f, SMM.mMaxY + 5f);
                        SMM.splinatedMeshEditorMaker.CollisionTriT.z = EditorGUILayout.Slider(" z-axis: ", SMM.CollisionTriT.z, SMM.mMinZ - 5f, SMM.mMaxZ + 5f);

                    }
                    else if (SMM.CollisionType == Splination.CollisionTypeEnum.SimpleMeshTrapezoid)
                    {
                        SMM.splinatedMeshEditorMaker.CollisionBoxBL = EditorGUILayout.Vector3Field(" Bottom left:", SMM.CollisionBoxBL);
                        SMM.splinatedMeshEditorMaker.CollisionBoxBR = EditorGUILayout.Vector3Field(" Bottom right:", SMM.CollisionBoxBR);
                        SMM.splinatedMeshEditorMaker.CollisionBoxTL = EditorGUILayout.Vector3Field(" Top left:", SMM.CollisionBoxTL);
                        SMM.splinatedMeshEditorMaker.CollisionBoxTR = EditorGUILayout.Vector3Field(" Top right:", SMM.CollisionBoxTR);
                    }
                }

                if (SMM.CollisionType == Splination.CollisionTypeEnum.BoxCollision)
                {
                    SMM.splinatedMeshEditorMaker.stretchedBCLocOffset = EditorGUILayout.Vector3Field("Box collider center offset:", SMM.boxColliderOffset);
                    SMM.splinatedMeshEditorMaker.isBCFlippedX = EditorGUILayout.Toggle("Flip center X:", SMM.isBoxColliderFlippedOnX);
                    SMM.splinatedMeshEditorMaker.isBCFlippedZ = EditorGUILayout.Toggle("Flip center Z:", SMM.isBoxColliderFlippedOnZ);


                    SMM.splinatedMeshEditorMaker.isStretchedSize = EditorGUILayout.Toggle("Box collider size edit:", SMM.isStretchedSize);
                    if (SMM.isStretchedSize)
                    {
                        SMM.splinatedMeshEditorMaker.stretchedBCSize = EditorGUILayout.Vector3Field("Size:", SMM.boxColliderSize);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Size:", SMM.boxColliderSize.ToString());
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.LabelField("Rotation options:");
                EditorGUILayout.BeginVertical("box");

                //Custom rotation:
                SMM.splinatedMeshEditorMaker.CustomRotation = SMM.CustomRotation;
                //EOM.CustomRotation = EditorGUILayout.Vector3Field("Custom rotation: ",EOM.CustomRotation);
                EditorGUILayout.BeginHorizontal();
                //Flip rotation option:
                if (SMM.splinatedMeshEditorMaker.isFlippedRotation != SMM.isFlippingRotation)
                {
                    SMM.splinatedMeshEditorMaker.isFlippedRotation = EditorGUILayout.Toggle("Flip Y rotation: ", SMM.splinatedMeshEditorMaker.isFlippedRotation);
                }
                else
                {
                    SMM.splinatedMeshEditorMaker.isFlippedRotation = EditorGUILayout.Toggle("Flip Y rotation: ", SMM.isFlippingRotation);
                }


                //			if(GUILayout.Button("Reset custom rotation",EditorStyles.miniButton,GUILayout.Width(160f))){
                //				SMM.CustomRotation = new Vector3(0f,0f,0f);
                //			}
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SMM.splinatedMeshEditorMaker.CustomRotation = new Vector3(0f, 0f, 0f);
                }
                EditorGUILayout.EndHorizontal();
                //			SMM.EM.CustomRotation = EditorGUILayout.Vector3Field("",SMM.CustomRotation);
                //			SMM.EM.CustomRotation.x = EditorGUILayout.Slider("x-axis: ",SMM.CustomRotation.x,-360f,360f);
                //			SMM.EM.CustomRotation.y = EditorGUILayout.Slider("y-axis: ",SMM.CustomRotation.y,-360f,360f);
                //			SMM.EM.CustomRotation.z = EditorGUILayout.Slider("z-axis: ",SMM.CustomRotation.z,-360f,360f);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();


                EditorGUILayout.LabelField("Deprecated options:");
                EditorGUILayout.BeginVertical("box");
                SMM.splinatedMeshEditorMaker.isExactSplination = EditorGUILayout.Toggle("Directional extrusion: ", SMM.isExactSplination);

                EditorGUILayout.EndVertical();
                // Big Lines
                EditorUtilities.DrawLine(4f, 4f);
                EditorUtilities.DrawLine(4f, 4f);
            }
        }


        public void UpdateSplineObjects()
        {
            if (!node.CanSplinate())
            {
                return;
            }
            if (node.SplinatedObjects == null)
            {
                node.SplinatedObjects = new List<Splination.SplinatedMeshMaker>();
            }
            count = node.SplinatedObjects.Count;
            for (int index = 0; index < count; index++)
            {
                SMM = node.SplinatedObjects[index];
                if (SMM.splinatedMeshEditorMaker != null)
                {
                    if (!SMM.splinatedMeshEditorMaker.IsEqualToSMM(SMM))
                    {
                        SMM.splinatedMeshEditorMaker.LoadToSMM(SMM);

                        SMM.UpdatePositions();
                        if (SMM.splinatedMeshEditorMaker.isStretched != SMM.isStretched)
                        {
                            if (SMM.isStretched)
                            {
                                SMM.CollisionType = Splination.CollisionTypeEnum.BoxCollision;
                                SMM.isMatchingRoadDefinition = false;
                                SMM.isMatchingTerrain = false;
                                SMM.isCollisionConvex = false;
                                SMM.isStartDown = false;
                                SMM.isEndDown = false;
                                SMM.isVerticalCutoff = false;
                                SMM.isExactSplination = false;
                                SMM.isEndTypeDownOverriden = false;
                            }
                        }

                        SMM.Setup(true);
                        //Debug.Log ("Setup SMM");
                    }
                }
            }
        }


        public void UpdateSplineObjectsOnUndo()
        {
            if (!node.CanSplinate())
            {
                return;
            }
            if (node.SplinatedObjects == null)
            {
                node.SplinatedObjects = new List<Splination.SplinatedMeshMaker>();
            }

            //Destroy all children:
            for (int index = node.transform.childCount - 1; index >= 0; index--)
            {
                Object.DestroyImmediate(node.transform.GetChild(index).gameObject);
            }

            //Re-setup the SMM:
            count = node.SplinatedObjects.Count;
            for (int index = 0; index < count; index++)
            {
                SMM = node.SplinatedObjects[index];
                SMM.UpdatePositions();
                //if(SMM.bIsStretch != SMM.bIsStretch){ 
                if (SMM.isStretched)
                {
                    SMM.CollisionType = Splination.CollisionTypeEnum.BoxCollision;
                    SMM.isMatchingRoadDefinition = false;
                    SMM.isMatchingTerrain = false;
                    SMM.isCollisionConvex = false;
                    SMM.isStartDown = false;
                    SMM.isEndDown = false;
                    SMM.isVerticalCutoff = false;
                    SMM.isExactSplination = false;
                    SMM.isEndTypeDownOverriden = false;
                }
                //}				
                SMM.Setup(true);
            }

            UpdateEdgeObjectsOnUndo();
        }


        /// <summary> Renders the EdgeObject items in the Editor </summary>
        public void DoEdgeObjects()
        {
            if (!node.CanSplinate())
            {
                return;
            }

            if (node.EdgeObjects == null)
            {
                node.EdgeObjects = new List<EdgeObjects.EdgeObjectMaker>();
            }
            count = node.EdgeObjects.Count;

            EOM = null;

            for (int index = 0; index < node.EdgeObjects.Count; index++)
            {
                EOM = node.EdgeObjects[index];
                if (EOM.edgeMaker == null)
                {
                    EOM.edgeMaker = new EdgeObjects.EdgeObjectMaker.EdgeObjectEditorMaker();
                }
                EOM.edgeMaker.Setup(EOM);

                currentCount += 1;
                EditorGUILayout.BeginVertical("TextArea");


                if (EOM.isRequiringUpdate)
                {
                    EOM.Setup();
                }
                EOM.isRequiringUpdate = false;

                EditorGUILayout.BeginHorizontal();

                EOM.isToggled = EditorGUILayout.Foldout(EOM.isToggled, "#" + currentCount.ToString() + ": " + EOM.objectName);

                if (GUILayout.Button(edgeButtonTexture, imageButton, GUILayout.Width(32f)))
                {

                }
                if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    EOM.Setup();
                }
                if (GUILayout.Button(saveButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    SaveWindow saveWindow = EditorWindow.GetWindow<SaveWindow>();
                    saveWindow.Initialize(ref sceneRect, SaveWindow.WindowTypeEnum.Edge, node, null, EOM);
                }

                if (GUILayout.Button(copyButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    Undo.RecordObject(node, "Copy");
                    node.CopyEdgeObject(index);
                    EditorUtility.SetDirty(node);
                }
                if (GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    Undo.RecordObject(node, "Delete");
                    node.RemoveEdgeObject(index);
                    EditorUtility.SetDirty(node);
                }
                EditorGUILayout.EndHorizontal();

                if (!EOM.isToggled)
                {
                    EditorGUILayout.EndVertical();
                    continue;
                }

                GUILayout.Space(8f);
                EditorGUILayout.LabelField("General options:");

                EditorGUILayout.BeginVertical("box");
                //Name:
                EOM.edgeMaker.objectName = EditorGUILayout.TextField("Name: ", EOM.objectName);

                //Edge object:
                EOM.edgeMaker.edgeObject = (GameObject)EditorGUILayout.ObjectField("Edge object: ", EOM.edgeObject, typeof(GameObject), false);
                if (EOM.edgeMaker.edgeObject != EOM.edgeObject)
                {
                    EOM.isEdgeSignLabelInit = false;
                    EOM.isEdgeSignLabel = false;
                }


                #region "Material override"
                EOM.edgeMaker.isMaterialOverriden = EditorGUILayout.Toggle("Material override: ", EOM.isMaterialOverriden);
                if (!EOM.isMaterialOverriden)
                {
                    EOM.edgeMaker.edgeMaterial1 = null;
                    EOM.edgeMaker.edgeMaterial2 = null;
                }

                if (!EOM.isEdgeSignLabelInit && EOM.edgeMaker.edgeObject != null)
                {
                    EOM.isEdgeSignLabel = false;
                    if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "SignDiamond") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-diamond";
                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "SignSquare-Small") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-Square";
                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "SignSquare") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-Square";

                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign988-Small") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-988";
                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign988") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-988";

                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign861-Small") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-861";
                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign861") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-861";

                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign617-Small") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-617";
                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign617") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-617";

                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign396") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-396";

                    }
                    else if (string.CompareOrdinal(EOM.edgeMaker.edgeObject.name, "Sign330") == 0)
                    {
                        EOM.isEdgeSignLabel = true;
                        EOM.edgeSignLabel = "FedSign-330";
                    }
                }

                if (EOM.isMaterialOverriden)
                {
                    if (EOM.isEdgeSignLabel)
                    {
                        EditorGUILayout.TextField("Material search term: ", EOM.edgeSignLabel);
                    }

                    EOM.edgeMaker.edgeMaterial1 = (Material)EditorGUILayout.ObjectField("Override mat #1: ", EOM.edgeMaterial1, typeof(Material), false);
                    EOM.edgeMaker.edgeMaterial2 = (Material)EditorGUILayout.ObjectField("Override mat #2: ", EOM.edgeMaterial2, typeof(Material), false);
                }
                #endregion


                #region "Combine Mesh / MeshCollider"
                if (EOM.isSingle)
                {
                    EOM.edgeMaker.isCombinedMesh = false;
                }
                else
                {
                    EOM.edgeMaker.isCombinedMesh = EditorGUILayout.Toggle("Combine meshes: ", EOM.isCombinedMesh);

                    if (EOM.isCombinedMesh)
                    {
                        EOM.edgeMaker.isCombinedMeshCollider = EditorGUILayout.Toggle("Combined mesh collider: ", EOM.isCombinedMeshCollider);
                    }
                }
                #endregion


                #region "SingleObject"
                EOM.edgeMaker.isSingle = EditorGUILayout.Toggle("Single object only: ", EOM.isSingle);
                if (EOM.edgeMaker.isSingle != EOM.isSingle)
                {
                    EOM.edgeMaker.endTime = node.nextTime;
                    //EOM.edgeMaker.EndPos = node.spline.GetSplineValue(EOM.edgeMaker.endTime, false);
                    EOM.edgeMaker.singlePosition = node.time + 0.025f;
                    if (EOM.edgeMaker.isSingle)
                    {
                        EOM.edgeMaker.isCombinedMesh = false;
                    }
                }

                if (EOM.isSingle)
                {
                    EOM.edgeMaker.singlePosition = EditorGUILayout.Slider("Single location: ", EOM.singlePosition, node.time, 1f);

                    if (node.isBridgeStart && node.isBridgeMatched)
                    {
                        EOM.edgeMaker.singleOnlyBridgePercent = EditorGUILayout.Slider("Bridge %: ", EOM.singleOnlyBridgePercent, 0f, 1f);
                        if (!RootUtils.IsApproximately(EOM.singleOnlyBridgePercent, EOM.edgeMaker.singleOnlyBridgePercent, 0.001f))
                        {
                            EOM.edgeMaker.singleOnlyBridgePercent = Mathf.Clamp(EOM.edgeMaker.singleOnlyBridgePercent, 0f, 1f);
                            float dist = (EOM.edgeMaker.singleOnlyBridgePercent * (node.bridgeCounterpartNode.dist - node.dist) + node.dist);
                            EOM.edgeMaker.singlePosition = node.spline.TranslateDistBasedToParam(dist);
                        }
                    }
                }
                #endregion


                EOM.edgeMaker.isStatic = EditorGUILayout.Toggle("Static: ", EOM.isStatic);


                EOM.edgeMaker.isMatchingTerrain = EditorGUILayout.Toggle("Match ground height: ", EOM.isMatchingTerrain);

                if (!EOM.isSingle)
                {
                    EOM.edgeMaker.meterSep = EditorGUILayout.Slider("Dist between objects: ", EOM.meterSep, 1f, 256f);
                }


                #region "Match Road"
                EOM.edgeMaker.isStartMatchingRoadDefinition = EditorGUILayout.Toggle("Match road definition: ", EOM.isStartMatchRoadDefinition);
                if (EOM.isStartMatchRoadDefinition)
                {
                    EOM.edgeMaker.startMatchRoadDef = EditorGUILayout.Slider("Position fine tuning: ", EOM.startMatchRoadDef, 0f, 1f);
                    if (!RootUtils.IsApproximately(EOM.edgeMaker.startMatchRoadDef, EOM.startMatchRoadDef, 0.001f))
                    {
                        EOM.edgeMaker.startMatchRoadDef = Mathf.Clamp(EOM.edgeMaker.startMatchRoadDef, 0f, 1f);
                    }
                }
                #endregion


                // Multi placed edge objects
                if (!EOM.isSingle)
                {
                    if (EOM.edgeMaker.startTime < node.minSplination)
                    {
                        EOM.edgeMaker.startTime = node.minSplination;
                    }
                    if (EOM.edgeMaker.endTime > node.maxSplination)
                    {
                        EOM.edgeMaker.endTime = node.maxSplination;
                    }


                    #region "Start param"
                    EditorGUILayout.BeginHorizontal();
                    EOM.edgeMaker.startTime = EditorGUILayout.Slider("Start param: ", EOM.startTime, node.minSplination, EOM.endTime);

                    if (EOM.edgeMaker.endTime < EOM.edgeMaker.startTime)
                    {
                        EOM.edgeMaker.endTime = Mathf.Clamp(EOM.startTime + 0.01f, 0f, 1f);
                    }


                    if (GUILayout.Button("match node", EditorStyles.miniButton, GUILayout.Width(80f)))
                    {
                        EOM.edgeMaker.startTime = node.time;
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion


                    #region "End param"
                    EditorGUILayout.BeginHorizontal();
                    EOM.edgeMaker.endTime = EditorGUILayout.Slider("End param: ", EOM.endTime, EOM.startTime, node.maxSplination);
                    //Mathf.Clamp(EditorGUILayout.Slider( "End param: ", EOM.EndTime, 0f/*EOM.StartTime*/, 1f/*tNode.MaxSplination*/ ), 0f, 1f);
                    // FH EXPERIMENTAL fix for EdgeObjects???

                    if (EOM.edgeMaker.startTime > EOM.edgeMaker.endTime)
                    {
                        EOM.edgeMaker.startTime = Mathf.Clamp(EOM.endTime - 0.01f, 0f, 1f);
                    }

                    if (GUILayout.Button("match next", EditorStyles.miniButton, GUILayout.Width(80f)))
                    {
                        EOM.edgeMaker.endTime = node.nextTime;
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion
                }


                EditorGUILayout.EndVertical();


                #region "Vertical offset"
                EditorGUILayout.LabelField("Vertical options:");
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                EOM.edgeMaker.verticalRaise = EditorGUILayout.Slider("Vertical raise magnitude:", EOM.verticalRaise, -512f, 512f);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    EOM.edgeMaker.verticalRaise = 0f;
                }
                EditorGUILayout.EndHorizontal();

                if (EOM.verticalCurve == null || EOM.verticalCurve.keys.Length < 2)
                {
                    EnforceCurve(ref EOM.verticalCurve);
                }
                EditorGUILayout.BeginHorizontal();
                EOM.edgeMaker.verticalCurve = EditorGUILayout.CurveField("Curve: ", EOM.verticalCurve);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    ResetCurve(ref EOM.edgeMaker.verticalCurve);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                #endregion


                #region "Horizontal offsets"
                EditorGUILayout.LabelField("Horizontal offset options:");
                EditorGUILayout.BeginVertical("box");
                horizMatching = HorizMatchingDefaultsEnum.None;
                horizMatching = (HorizMatchingDefaultsEnum)EditorGUILayout.Popup((int)horizMatching, HorizMatchSubTypeDescriptions, GUILayout.Width(100f));
                if (horizMatching != HorizMatchingDefaultsEnum.None)
                {
                    if (horizMatching == HorizMatchingDefaultsEnum.MatchCenter)
                    {
                        EOM.edgeMaker.horizontalSep = 0f;
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchRoadLeft)
                    {
                        EOM.edgeMaker.horizontalSep = (node.spline.road.RoadWidth() * 0.5f) * -1;
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchShoulderLeft)
                    {
                        if (node.spline.road.isShouldersEnabled)
                        {
                            EOM.edgeMaker.horizontalSep = ((node.spline.road.RoadWidth() * 0.5f) + node.spline.road.shoulderWidth) * -1;
                        }
                        else
                        {
                            EOM.edgeMaker.horizontalSep = ((node.spline.road.RoadWidth() * 0.5f)) * -1;
                        }
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchRoadRight)
                    {
                        EOM.edgeMaker.horizontalSep = (node.spline.road.RoadWidth() * 0.5f);
                    }
                    else if (horizMatching == HorizMatchingDefaultsEnum.MatchShoulderRight)
                    {
                        if (node.spline.road.isShouldersEnabled)
                        {
                            EOM.edgeMaker.horizontalSep = (node.spline.road.RoadWidth() * 0.5f) + node.spline.road.shoulderWidth;
                        }
                        else
                        {
                            EOM.edgeMaker.horizontalSep = (node.spline.road.RoadWidth() * 0.5f);
                        }
                    }
                    horizMatching = HorizMatchingDefaultsEnum.None;
                }
                if (!RootUtils.IsApproximately(EOM.edgeMaker.horizontalSep, EOM.horizontalSep))
                {
                    EOM.edgeMaker.horizontalSep = Mathf.Clamp(EOM.edgeMaker.horizontalSep, (-1f * horizRoadMax), horizRoadMax);
                }


                EditorGUILayout.BeginHorizontal();
                EOM.edgeMaker.horizontalSep = EditorGUILayout.Slider("Horiz offset magnitude:", EOM.edgeMaker.horizontalSep, (-1f * horizRoadMax), horizRoadMax);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    EOM.edgeMaker.horizontalSep = 0f;
                }
                if (!RootUtils.IsApproximately(EOM.edgeMaker.horizontalSep, EOM.horizontalSep))
                {
                    EOM.edgeMaker.horizontalSep = Mathf.Clamp(EOM.edgeMaker.horizontalSep, (-1f * horizRoadMax), horizRoadMax);
                }
                EditorGUILayout.EndHorizontal();
                if (EOM.horizontalCurve == null || EOM.horizontalCurve.keys.Length < 2)
                { EnforceCurve(ref EOM.horizontalCurve); }
                EditorGUILayout.BeginHorizontal();
                EOM.edgeMaker.horizontalCurve = EditorGUILayout.CurveField("Curve: ", EOM.horizontalCurve);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    ResetCurve(ref EOM.edgeMaker.horizontalCurve);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                #endregion


                #region "Rotation/scale options"
                EditorGUILayout.LabelField("Rotation/scale options:");
                EditorGUILayout.BeginVertical("box");
                if (EOM.horizontalSep < 0f)
                {
                    EOM.edgeMaker.isOncomingRotation = EditorGUILayout.Toggle("Auto rotate oncoming objects: ", EOM.isOncomingRotation);
                    EOM.edgeMaker.subType = SignPlacementSubTypeEnum.Left;
                }
                else
                {
                    EOM.edgeMaker.subType = SignPlacementSubTypeEnum.Right;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Custom rotation: ");
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    EOM.edgeMaker.customRotation = new Vector3(0f, 0f, 0f);
                }
                EditorGUILayout.EndHorizontal();

                EOM.edgeMaker.customRotation.x = EditorGUILayout.Slider("x-axis: ", EOM.customRotation.x, -360f, 360f);
                EOM.edgeMaker.customRotation.y = EditorGUILayout.Slider("y-axis: ", EOM.customRotation.y, -360f, 360f);
                EOM.edgeMaker.customRotation.z = EditorGUILayout.Slider("z-axis: ", EOM.customRotation.z, -360f, 360f);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Lock align rotation: ");
                EOM.edgeMaker.isRotationAligning = EditorGUILayout.Toggle("Align objects to road rotation: ", EOM.isRotationAligning);
                if(!EOM.edgeMaker.isRotationAligning)
                {
                    EOM.edgeMaker.isXRotationLocked = EditorGUILayout.Toggle("Lock x axis: ", EOM.isXRotationLocked);
                    EOM.edgeMaker.isYRotationLocked = EditorGUILayout.Toggle("Lock y axis: ", EOM.isYRotationLocked);
                    EOM.edgeMaker.isZRotationLocked = EditorGUILayout.Toggle("Lock z axis: ", EOM.isZRotationLocked);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                float scale = EditorGUILayout.Slider("Custom scale: ", EOM.customScale.x, 1f, 10f);
                EOM.edgeMaker.customScale = new Vector3(scale, scale, scale);
                if (GUILayout.Button(defaultButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    EOM.edgeMaker.customScale = new Vector3(1f, 1f, 1f);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                #endregion
            }
        }


        public void UpdateEdgeObjects()
        {
            if (!node.CanSplinate())
            {
                return;
            }
            count = node.EdgeObjects.Count;
            for (int index = 0; index < node.EdgeObjects.Count; index++)
            {
                EOM = node.EdgeObjects[index];
                if (EOM.edgeMaker != null)
                {
                    if (!EOM.edgeMaker.IsEqual(EOM))
                    {
                        EOM.edgeMaker.LoadTo(EOM);
                        EOM.UpdatePositions();
                        EOM.Setup();
                        //Debug.Log ("Setup EOM"); 
                    }
                }
            }
        }


        public void UpdateEdgeObjectsOnUndo()
        {
            if (!node.CanSplinate())
            {
                return;
            }
            count = node.EdgeObjects.Count;
            for (int index = 0; index < node.EdgeObjects.Count; index++)
            {
                EOM = node.EdgeObjects[index];
                EOM.Setup();
            }
        }


        #region "Quick adds"
        private void BridgeAddTopBase(float _horizSep = 0f, float _vertRaise = -0.01f, string _mat = "", bool _isOverridenPrefab = false, string _overridePrefab = "")
        {
            string basePath = RoadEditorUtility.GetBasePath();

            if (_mat == "")
            {
                _mat = basePath + "/Materials/Concrete2.mat";
            }
            SMM = node.AddSplinatedObject();
            string tBridgeTopBaseToAdd = "";
            string tName = "";
            if (node.spline.road.laneAmount == 2)
            {
                if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base1MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-19w-5l-1d.fbx";
                    tName = "BridgeTop1M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base2MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-20w-5l-1d.fbx";
                    tName = "BridgeTop2M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base3MDeep)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-18w-5l-3d.fbx";
                    tName = "BridgeTop0M-3M";
                }
                else
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-18w-5l-1d.fbx";
                    tName = "BridgeTop0M-1M";
                }
            }
            else if (node.spline.road.laneAmount == 4)
            {
                if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base1MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-29w-5l-1d.fbx";
                    tName = "BridgeTop1M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base2MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-30w-5l-1d.fbx";
                    tName = "BridgeTop2M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base3MDeep)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-28w-5l-3d.fbx";
                    tName = "BridgeTop0M-3M";
                }
                else
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-28w-5l-1d.fbx";
                    tName = "BridgeTop0M-1M";
                }
            }
            else
            {
                if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base1MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-39w-5l-1d.fbx";
                    tName = "BridgeTop1M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base2MOver)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-40w-5l-1d.fbx";
                    tName = "BridgeTop2M-1M";
                }
                else if (BridgeTopBaseQuickAdd == BridgeTopBaseDefaultsEnum.Base3MDeep)
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-38w-5l-3d.fbx";
                    tName = "BridgeTop0M-3M";
                }
                else
                {
                    tBridgeTopBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase-38w-5l-1d.fbx";
                    tName = "BridgeTop0M-1M";
                }
            }

            if (_isOverridenPrefab)
            {
                tBridgeTopBaseToAdd = _overridePrefab;
            }

            SMM.objectName = tName;
            SMM.currentSplination = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(tBridgeTopBaseToAdd);
            SMM.horizontalSep = _horizSep;
            SMM.verticalRaise = _vertRaise;
            SMM.isMaterialOverriden = true;
            SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            SMM.Axis = Splination.AxisTypeEnum.Z;

            BridgeTopBaseQuickAdd = BridgeTopBaseDefaultsEnum.None;
            if (SMM.StartTime < node.minSplination)
            {
                SMM.StartTime = node.minSplination;
            }
            if (SMM.EndTime > node.maxSplination)
            {
                SMM.EndTime = node.maxSplination;
            }
        }


        private void BridgeAddBottomBase(float _horizSep = 0f, float _vertRaise = -1.01f, string _mat = "", bool _isOverridenPrefab = false, string _overridePrefab = "")
        {
            string basePath = RoadEditorUtility.GetBasePath();

            if (_mat == "")
            {
                _mat = basePath + "/Materials/Concrete2.mat";
            }
            SMM = node.AddSplinatedObject();
            string tBridgeBottomBaseToAdd = "";
            string tName = "";
            if (node.spline.road.laneAmount == 2)
            {
                if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase2)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase2-18w-5l-3d.fbx";
                    tName = "BridgeBase2";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase3)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase3-18w-5l-5d.fbx";
                    tName = "BridgeBase3";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase4)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase4-18w-5l-5d.fbx";
                    tName = "BridgeBase4";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase5)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase5-18w-5l-5d.fbx";
                    tName = "BridgeBase5";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase6)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase6On2Lanes.fbx";
                    tName = "BridgeArchBeam80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase7)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase7On2Lanes.fbx";
                    tName = "BridgeArchSolid80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase8)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase8On2Lanes.fbx";
                    tName = "BridgeArchSolid180";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBaseGrid)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseGrid-18w-5l-5d.fbx";
                    tName = "BridgeGrid";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeSteel)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseSteelBeam-18w-20l-3d.fbx";
                    tName = "BridgeSteelBeams";
                }
            }
            else if (node.spline.road.laneAmount == 4)
            {
                if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase2)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase2-28w-5l-3d.fbx";
                    tName = "BridgeBase2";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase3)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase3-28w-5l-5d.fbx";
                    tName = "BridgeBase3";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase4)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase4-28w-5l-5d.fbx";
                    tName = "BridgeBase4";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase5)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase5-28w-5l-5d.fbx";
                    tName = "BridgeBase5";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase6)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase6On4Lanes.fbx";
                    tName = "BridgeArchBeam80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase7)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase7On4Lanes.fbx";
                    tName = "BridgeArchSolid80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase8)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase8On4Lanes.fbx";
                    tName = "BridgeArchSolid180";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBaseGrid)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseGrid-28w-5l-5d.fbx";
                    tName = "BridgeGrid";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeSteel)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseSteelBeam-28w-20l-3d.fbx";
                    tName = "BridgeSteelBeams";
                }
            }
            else
            {
                if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase2)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase2-38w-5l-3d.fbx";
                    tName = "BridgeBase2";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase3)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase3-38w-5l-5d.fbx";
                    tName = "BridgeBase3";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase4)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase4-38w-5l-5d.fbx";
                    tName = "BridgeBase4";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase5)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase5-38w-5l-5d.fbx";
                    tName = "BridgeBase5";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase6)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase6On6Lanes.fbx";
                    tName = "BridgeArchBeam80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase7)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase7On6Lanes.fbx";
                    tName = "BridgeArchSolid80";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase8)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBase8On6Lanes.fbx";
                    tName = "BridgeArchSolid180";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBaseGrid)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseGrid-38w-5l-5d.fbx";
                    tName = "BridgeGrid";
                }
                else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeSteel)
                {
                    tBridgeBottomBaseToAdd = basePath + "/Mesh/Bridges/BridgeBaseSteelBeam-38w-20l-3d.fbx";
                    tName = "BridgeBeams";
                }
            }

            if (_isOverridenPrefab)
            {
                tBridgeBottomBaseToAdd = _overridePrefab;
            }

            SMM.currentSplination = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(tBridgeBottomBaseToAdd);
            SMM.horizontalSep = _horizSep;
            SMM.verticalRaise = _vertRaise;
            SMM.isMaterialOverriden = true;
            SMM.objectName = tName;

            if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase2)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase3)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase4)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase5)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase6)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase7)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBase8)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(_mat);
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeBaseGrid)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Steel7.mat");
            }
            else if (BridgeBottomBaseQuickAdd == BridgeBottomBaseDefaultsEnum.BridgeSteel)
            {
                SMM.SplinatedMaterial1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Steel7.mat");
            }

            SMM.Axis = Splination.AxisTypeEnum.Z;

            BridgeTopBaseQuickAdd = BridgeTopBaseDefaultsEnum.None;
            if (SMM.StartTime < node.minSplination)
            {
                SMM.StartTime = node.minSplination;
            }
            if (SMM.EndTime > node.maxSplination)
            {
                SMM.EndTime = node.maxSplination;
            }
        }


        private void ExtrusionQuickAdd(bool _isHorizOverriden = false, float _horizSep = 0f, bool _isVertOverriden = false, float _vertRaise = 0f)
        {
            try
            {
                ExtrusionQuickAddDo();
            }
            catch (System.Exception exception)
            {
                SMMQuickAdd = SMMDefaultsEnum.None;
                throw exception;
            }
        }


        private void ExtrusionQuickAddDo()
        {
            if (SMMQuickAdd == SMMDefaultsEnum.KRail)
            {
                node.SplinatedObjectQuickAdd("KRail");
            }
        }


        private void ExtrudeHelper(string _path, string _name, float DefaultHoriz, Splination.AxisTypeEnum _axisType = Splination.AxisTypeEnum.Z, bool _isHorizOverriden = false, float _horizSep = 0f, bool _isVertOverriden = false, float _vertRaise = 0f, bool _isFlippingRot = false)
        {
            SMM = node.AddSplinatedObject();
            SMM.currentSplination = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(_path);

            if (_isHorizOverriden)
            {
                SMM.horizontalSep = _horizSep;
            }
            else
            {
                SMM.horizontalSep = ((node.spline.road.RoadWidth() / 2) + node.spline.road.shoulderWidth) * -1f;
            }

            if (_isVertOverriden)
            {
                SMM.verticalRaise = _vertRaise;
            }
            else
            {
                if (node.isBridgeStart)
                {
                    SMM.verticalRaise = -0.01f;
                }
            }

            SMM.isFlippingRotation = _isFlippingRot;
            SMM.Axis = _axisType;
            if (SMM.StartTime < node.minSplination)
            {
                SMM.StartTime = node.minSplination;
            }
            if (SMM.EndTime > node.maxSplination)
            {
                SMM.EndTime = node.maxSplination;
            }
            SMM.objectName = _name;
        }
        #endregion


        public void OnSceneGUI()
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            bool isUsed = false;

            if (!isSceneRectSet)
            {
                try
                {
                    sceneRect = EditorWindow.GetWindow<SceneView>().position;
                }
                catch
                {
                    sceneRect = EditorWindow.GetWindow<EditorWindow>().position;
                }
                isSceneRectSet = true;
            }

            if (!node.isEditorSelected)
            {
                node.isEditorSelected = true;
            }

            if (current.type == EventType.ValidateCommand)
            {
                switch (current.commandName)
                {
                    case "UndoRedoPerformed":
                        UpdateSplineObjectsOnUndo();
                        break;
                }
            }

            if (controlID != node.GetHashCode())
            {
                node.isEditorSelected = false;
            }

            //Drag with left click:
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                isMouseDragProcessed = false;
                node.isDrawingIntersectionHighlightGizmos = true;
            }
            //Drag with left click release:
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                Object[] nodeObjects = GameObject.FindObjectsOfType<SplineN>();
                foreach (SplineN xNode in nodeObjects)
                {
                    if (Vector3.Distance(xNode.transform.position, node.transform.position) < 2f)
                    {
                        if (xNode == node)
                        {
                            continue;
                        }
                        if (node.isSpecialEndNode || xNode.isSpecialEndNode)
                        {
                            continue;
                        }
                        if (xNode.isEndPoint && node.isEndPoint)
                        {
                            //End point connection.
                            node.transform.position = xNode.transform.position;
                            //Activate special end node for tnode
                            TriggerRoadConnection(node, xNode);
                            isUsed = true;
                            break;
                        }
                        if (xNode.isIntersection)
                        {
                            continue;
                        }
                        if (xNode.isNeverIntersect)
                        {
                            continue;
                        }
                        if (node.isEndPoint && xNode.isEndPoint)
                        {
                            continue;
                        }
                        if (xNode.spline == node.spline)
                        {
                            //Don't let intersection be created on consecutive nodes:
                            if ((node.idOnSpline + 1) == xNode.idOnSpline || (node.idOnSpline - 1) == xNode.idOnSpline)
                            {
                                continue;
                            }
                        }
                        node.transform.position = xNode.transform.position;
                        TriggerIntersection(node, xNode);
                        isUsed = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!isMouseDragProcessed)
                {
                    //Enforce maximum road grade:
                    if (node.IsLegitimate() && node.spline.road.isMaxGradeEnabled)
                    {
                        node.EnsureGradeValidity();
                    }
                    TriggerRoadUpdate();
                    isUsed = true;
                }
                isMouseDragProcessed = true;
                node.isDrawingIntersectionHighlightGizmos = false;
                isUsed = true;
            }

            //Enforce maximum road grade:
            if (isMouseDragProcessed)
            {

                Vector3 vChangeChecker = node.transform.position;
                if (VectorDiff(vChangeChecker, node.pos))
                {
                    node.pos = vChangeChecker;
                    if (node.IsLegitimate() && node.spline.road.isMaxGradeEnabled)
                    {
                        node.EnsureGradeValidity();
                    }
                    TriggerRoadUpdate();
                }
                isUsed = true;
            }

            if (Selection.activeGameObject == node.transform.gameObject)
            {
                if (current.keyCode == KeyCode.F5)
                {
                    TriggerRoadUpdate();
                }
            }

            if (isUsed)
            {
                //			switch(current.type){
                //				case EventType.layout:
                //			        HandleUtility.AddDefaultControl(controlID);
                //			    break;
                //			}
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(node);
            }
        }


        private bool VectorDiff(Vector3 _vect1, Vector3 _vect2)
        {
            if (!RootUtils.IsApproximately(_vect1.x, _vect2.x, 0.0001f))
            {
                return true;
            }
            if (!RootUtils.IsApproximately(_vect1.y, _vect2.y, 0.0001f))
            {
                return true;
            }
            if (!RootUtils.IsApproximately(_vect1.z, _vect2.z, 0.0001f))
            {
                return true;
            }
            return false;
        }


        private bool VectorEquals(ref Vector3 _v1, ref Vector3 _v2)
        {
            if (!RootUtils.IsApproximately(_v1.x, _v2.x, 0.001f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.y, _v2.y, 0.001f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.z, _v2.z, 0.001f))
            {
                return false;
            }
            return true;
        }


        #region "Triggers Intersections; Connections; Update"
        private void TriggerRoadConnection(SplineN _node1, SplineN _node2)
        {
            node.spline.ActivateEndNodeConnection(_node1, _node2);
        }


        private void TriggerIntersection(SplineN _node1, SplineN _node2)
        {
            isCreatingIntersection = true;
            node1 = _node1;
            node2 = _node2;
            Selection.activeGameObject = Intersections.CreateIntersection(_node1, _node2);
        }


        private void TriggerRoadUpdate()
        {
            if (node != null)
            {
                node.spline.road.isUpdateRequired = true;
            }
        }
        #endregion


        private void ResetCurve(ref AnimationCurve _curve)
        {
            _curve = null;
            _curve = new AnimationCurve();
            EnforceCurve(ref _curve);
        }


        private void EnforceCurve(ref AnimationCurve _curve)
        {
            if (_curve == null)
            {
                return;
            }
            if (_curve.keys.Length == 0)
            {
                _curve.AddKey(0f, 1f);
                _curve.AddKey(1f, 1f);
            }
            else if (_curve.keys.Length == 1)
            {
                _curve.keys[0].time = 0f;
                _curve.AddKey(1f, 1f);
            }
        }


        private GameObject GetEndObjectQuickAdd()
        {
            string basePath = RoadEditorUtility.GetBasePath();

            string path = "";
            if (endObjectAdd == EndObjectsDefaultsEnum.WarningSign1_Static)
            {
                path = basePath + "/Prefabs/Interactive/WarningSignStatic.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.WarningSign2_Static)
            {
                path = basePath + "/Prefabs/Interactive/WarningSignStatic2.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Atten_Static)
            {
                path = basePath + "/Prefabs/Interactive/AttenStatic.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel1_Static)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelStatic.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel1_Rigid)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelRigid.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel3_Static)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelStatic3.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel3_Rigid)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelRigid3.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel7_Static)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelStatic7.prefab";
            }
            else if (endObjectAdd == EndObjectsDefaultsEnum.Barrel7_Rigid)
            {
                path = basePath + "/Prefabs/Interactive/RoadBarrelRigid7.prefab";
            }
            else
            {
                return null;
            }

            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
    }
}
#endif
