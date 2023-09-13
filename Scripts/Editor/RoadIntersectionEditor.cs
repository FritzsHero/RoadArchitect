#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(RoadIntersection))]
    public class RoadIntersectionEditor : Editor
    {
        #region "Vars"
        private RoadIntersection intersection;

        private SerializedProperty isAutoUpdatingIntersection;
        private SerializedProperty isDrawingGizmo;
        private SerializedProperty isLeftTurnYieldOnGreen;
        private SerializedProperty fixedTimeRegularLightLength;
        private SerializedProperty fixedTimeLeftTurnLightLength;
        private SerializedProperty fixedTimeAllRedLightLength;
        private SerializedProperty fixedTimeYellowLightLength;
        private SerializedProperty isTrafficPoleStreetLight;
        private SerializedProperty isTrafficLightGray;
        private SerializedProperty isRegularPoleAlignment;
        private SerializedProperty isLightsEnabled;
        private SerializedProperty streetLightRange;
        private SerializedProperty streetLightIntensity;
        private SerializedProperty streetLightColor;
        private SerializedProperty scalingSense;
        private SerializedProperty gradeMod;
        private SerializedProperty isUsingDefaultMaterials;
        private SerializedProperty markerCenter1;
        private SerializedProperty markerCenter2;
        private SerializedProperty markerCenter3;
        private SerializedProperty markerExtStretch1;
        private SerializedProperty markerExtStretch2;
        private SerializedProperty markerExtStretch3;
        private SerializedProperty markerExtTiled1;
        private SerializedProperty markerExtTiled2;
        private SerializedProperty markerExtTiled3;
        private SerializedProperty lane0Mat1;
        private SerializedProperty lane0Mat2;
        private SerializedProperty lane1Mat1;
        private SerializedProperty lane1Mat2;
        private SerializedProperty lane2Mat1;
        private SerializedProperty lane2Mat2;
        private SerializedProperty lane3Mat1;
        private SerializedProperty lane3Mat2;
        private SerializedProperty roadType;
        private SerializedProperty intersectionStopType;
        private SerializedProperty lightType;


        #region "Editor only variables"
        //Editor only variables
        private const bool isDebug = false;
        private bool isShowingMarkerStretch = false;
        private bool isShowingMarkerTiled = false;
        private bool isShowingMarkerCenter = false;
        private bool isShowingTLSense = false;
        private bool isShowingTLPole = false;
        private bool isShowingLightHelp = false;
        private bool isShowingLanes = false;
        private bool isGradeCorrect = false;
        private bool isShowingDefaultMatHelp = false;
        private bool isShowingHelpLeftTurnGreen = false;
        private string status = "Show help";

        private GUIStyle imageButton = null;
        private Texture refreshButtonTexture = null;
        private Texture deleteButtonTexture = null;

        private const string helpText1 = "Each material added is rendered on top of the previous. Combine with transparent shaders which accept shadows to allow for easy marking.";

        //Checkers:
        private Texture2D loadButtonBG = null;
        private Texture2D loadButtonBGGlow = null;

        private GUIStyle loadButton = null;
        private bool isInitialized = false;
        #endregion
        #endregion


        private static string[] rTypeDescriptions = new string[]{
        "No turn lanes",
        "Left turn lane only",
        "Both left and right turn lanes"
    };


        private static string[] rTypeDescriptions_3Way = new string[]{
        "No turn lanes",
        "Left turn lane only"
    };


        private static string[] iStopTypeEnumDescriptions = new string[]{
        "Stop signs",
        "Traffic lights",
        "None"
//		"Traffic lights #2"
    };


        private static string[] iTrafficLightSequenceTypeDesc = new string[]{
        "Fixed time",
        "Other"
    };


        private void OnEnable()
        {
            intersection = (RoadIntersection)target;

            isAutoUpdatingIntersection = serializedObject.FindProperty("isAutoUpdatingIntersection");
            isDrawingGizmo = serializedObject.FindProperty("isDrawingGizmo");
            isLeftTurnYieldOnGreen = serializedObject.FindProperty("isLeftTurnYieldOnGreen");
            fixedTimeRegularLightLength = serializedObject.FindProperty("fixedTimeRegularLightLength");
            fixedTimeLeftTurnLightLength = serializedObject.FindProperty("fixedTimeLeftTurnLightLength");
            fixedTimeAllRedLightLength = serializedObject.FindProperty("fixedTimeAllRedLightLength");
            fixedTimeYellowLightLength = serializedObject.FindProperty("fixedTimeYellowLightLength");
            isTrafficPoleStreetLight = serializedObject.FindProperty("isTrafficPoleStreetLight");
            isTrafficLightGray = serializedObject.FindProperty("isTrafficLightGray");
            isRegularPoleAlignment = serializedObject.FindProperty("isRegularPoleAlignment");
            isLightsEnabled = serializedObject.FindProperty("isLightsEnabled");
            streetLightRange = serializedObject.FindProperty("streetLightRange");
            streetLightIntensity = serializedObject.FindProperty("streetLightIntensity");
            streetLightColor = serializedObject.FindProperty("streetLightColor");
            scalingSense = serializedObject.FindProperty("scalingSense");
            gradeMod = serializedObject.FindProperty("gradeMod");
            isUsingDefaultMaterials = serializedObject.FindProperty("isUsingDefaultMaterials");
            markerCenter1 = serializedObject.FindProperty("markerCenter1");
            markerCenter2 = serializedObject.FindProperty("markerCenter2");
            markerCenter3 = serializedObject.FindProperty("markerCenter3");
            markerExtStretch1 = serializedObject.FindProperty("markerExtStretch1");
            markerExtStretch2 = serializedObject.FindProperty("markerExtStretch2");
            markerExtStretch3 = serializedObject.FindProperty("markerExtStretch3");
            markerExtTiled1 = serializedObject.FindProperty("markerExtTiled1");
            markerExtTiled2 = serializedObject.FindProperty("markerExtTiled2");
            markerExtTiled3 = serializedObject.FindProperty("markerExtTiled3");
            lane0Mat1 = serializedObject.FindProperty("lane0Mat1");
            lane0Mat2 = serializedObject.FindProperty("lane0Mat2");
            lane1Mat1 = serializedObject.FindProperty("lane1Mat1");
            lane1Mat2 = serializedObject.FindProperty("lane1Mat2");
            lane2Mat1 = serializedObject.FindProperty("lane2Mat1");
            lane2Mat2 = serializedObject.FindProperty("lane2Mat2");
            lane3Mat1 = serializedObject.FindProperty("lane3Mat1");
            lane3Mat2 = serializedObject.FindProperty("lane3Mat2");
            roadType = serializedObject.FindProperty("roadType");
            intersectionStopType = serializedObject.FindProperty("intersectionStopType");
            lightType = serializedObject.FindProperty("lightType");
        }


        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.ValidateCommand)
            {
                switch (Event.current.commandName)
                {
                    case "UndoRedoPerformed":
                        TriggerRoadUpdate(true);
                        break;
                }
            }

            serializedObject.Update();

            //Graphic null checks:
            if (!isInitialized)
            {
                Init();
            }

            EditorUtilities.DrawLine();
            EditorGUILayout.BeginHorizontal();
            {
                Road road1 = intersection.node1.spline.transform.parent.GetComponent<Road>();
                Road road2 = intersection.node2.spline.transform.parent.GetComponent<Road>();
                EditorGUILayout.LabelField("First road: " + road1.name + " node: " + intersection.node1.name);
                EditorGUILayout.LabelField("Second road: " + road2.name + " node: " + intersection.node2.name);
            }
            EditorGUILayout.EndHorizontal();
            EditorUtilities.DrawLine();
            EditorGUILayout.LabelField("Intersection options", EditorStyles.boldLabel);
            if (GUILayout.Button("Update intersection", loadButton))
            {
                TriggerRoadUpdate(true);
            }

            //Option: Auto update:
            isAutoUpdatingIntersection.boolValue = EditorGUILayout.Toggle("Auto-update:", intersection.isAutoUpdatingIntersection);

            //Option: Gizmo:
            isDrawingGizmo.boolValue = EditorGUILayout.Toggle("Gizmo:", intersection.isDrawingGizmo);

            //UI:
            if (intersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
            {
                EditorGUILayout.LabelField("Intersection type: 3 way");
            }
            else
            {
                EditorGUILayout.LabelField("Intersection type: 4 way");
            }
            if (GUILayout.Button("Online manual", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");
            }
            if (GUILayout.Button("Offline manual", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                RoadArchitect.EditorUtilities.OpenOfflineManual();
            }
            EditorUtilities.DrawLine();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Access objects on first road node"))
            {
                Selection.objects = new Object[1] { intersection.node1 };
            }
            if (GUILayout.Button("Access objects on second road node"))
            {
                Selection.objects = new Object[1] { intersection.node2 };
            }
            EditorGUILayout.EndVertical();
            //Option: Intersection turn lane options
            EditorUtilities.DrawLine();
            EditorGUILayout.LabelField("Intersection turn lane options:");
            if (intersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
            {
                roadType.enumValueIndex = (int)EditorGUILayout.Popup((int)intersection.roadType, rTypeDescriptions_3Way);
            }
            else
            {
                roadType.enumValueIndex = (int)EditorGUILayout.Popup((int)intersection.roadType, rTypeDescriptions);
            }

            //Option: Left yield on green:
            if (intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                isLeftTurnYieldOnGreen.boolValue = EditorGUILayout.Toggle("Left yield on green: ", intersection.isLeftTurnYieldOnGreen);

                isShowingHelpLeftTurnGreen = EditorGUILayout.Foldout(isShowingHelpLeftTurnGreen, status);
                if (isShowingHelpLeftTurnGreen)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Left yield on green: If checked, replaces the standard 3-light left turn light with a five-light yield on green left turn signal structure and sign.");
                    EditorGUILayout.EndVertical();
                }
            }
            EditorUtilities.DrawLine();


            //Option: Intersection stop type:
            intersectionStopType.enumValueIndex = (int)EditorGUILayout.Popup("Intersection stop type:", (int)intersection.intersectionStopType, iStopTypeEnumDescriptions);


            if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
            {
                //Option: Traffic light timing type:
                lightType.enumValueIndex = (int)EditorGUILayout.Popup("Traffic light timing:", (int)intersection.lightType, iTrafficLightSequenceTypeDesc);

                //Options: Traffic fixed light timings:
                if (intersection.lightType == RoadIntersection.LightTypeEnum.Timed)
                {
                    EditorGUILayout.LabelField("Traffic light fixed time lengths (in seconds):");
                    EditorGUILayout.BeginHorizontal();
                    fixedTimeRegularLightLength.floatValue = EditorGUILayout.Slider("Green length: ", intersection.fixedTimeRegularLightLength, 0.1f, 180f);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        fixedTimeRegularLightLength.floatValue = 30f;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        EditorGUILayout.BeginHorizontal();
                        fixedTimeLeftTurnLightLength.floatValue = EditorGUILayout.Slider("Left turn only length: ", intersection.fixedTimeLeftTurnLightLength, 0.1f, 180f);
                        if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                        {
                            fixedTimeLeftTurnLightLength.floatValue = 10f;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    fixedTimeAllRedLightLength.floatValue = EditorGUILayout.Slider("All red length: ", intersection.fixedTimeAllRedLightLength, 0.1f, 180f);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        fixedTimeAllRedLightLength.floatValue = 1f;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    fixedTimeYellowLightLength.floatValue = EditorGUILayout.Slider("Yellow light length: ", intersection.fixedTimeYellowLightLength, 0.1f, 180f);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        fixedTimeYellowLightLength.floatValue = 3f;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
            {
                //Option: Traffic light poles:
                EditorUtilities.DrawLine();
                EditorGUILayout.LabelField("Traffic light poles:");
                isTrafficPoleStreetLight.boolValue = EditorGUILayout.Toggle("Street lights: ", intersection.isTrafficPoleStreetLight);

                //Option: Traffic light pole gray color:
                isTrafficLightGray.boolValue = EditorGUILayout.Toggle("Gray color: ", intersection.isTrafficLightGray);

                //Option: Normal pole alignment:
                isRegularPoleAlignment.boolValue = EditorGUILayout.Toggle("Normal pole alignment: ", intersection.isRegularPoleAlignment);
                isShowingTLPole = EditorGUILayout.Foldout(isShowingTLPole, status);
                if (isShowingTLPole)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Street lights: If checked, attaches street lights to each intersection pole. Point lights optional and can be manipulated in the next option segment.");
                    GUILayout.Space(4f);
                    EditorGUILayout.LabelField("Gray color: If checked, sets the traffic light pole base materials to gray galvanized steel. If unchecked the material used is black metal paint.");
                    GUILayout.Space(4f);
                    EditorGUILayout.LabelField("Normal pole alignment: Recommended to keep this option on unless abnormal twisting of light objects is occurring. Turn this option off if the roads immediately surrounding your intersection are curved at extreme angles and cause irregular twisting of the traffic light objects. Turning this option off will attempt to align the poles perpendicular to the adjacent relevant road without any part existing over the main intersection bounds.");
                    EditorGUILayout.EndVertical();
                }

                //Option: Point lights enabled:
                EditorUtilities.DrawLine();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Point lights:");
                EditorGUILayout.EndHorizontal();
                isLightsEnabled.boolValue = EditorGUILayout.Toggle("  Point lights enabled: ", intersection.isLightsEnabled);

                //Options: Street point light options:
                if (intersection.isTrafficPoleStreetLight)
                {
                    //Option: Street light range:
                    EditorGUILayout.BeginHorizontal();
                    streetLightRange.floatValue = EditorGUILayout.Slider("  Street light range: ", intersection.streetLightRange, 1f, 128f);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        streetLightRange.floatValue = 30f;
                    }
                    EditorGUILayout.EndHorizontal();

                    //Option: Street light intensity:
                    EditorGUILayout.BeginHorizontal();
                    streetLightIntensity.floatValue = EditorGUILayout.Slider("  Street light intensity: ", intersection.streetLightIntensity, 0f, 8f);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        streetLightIntensity.floatValue = 1f;
                    }
                    EditorGUILayout.EndHorizontal();

                    //Option: Street light color:
                    EditorGUILayout.BeginHorizontal();
                    streetLightColor.colorValue = EditorGUILayout.ColorField("  Street light color: ", intersection.streetLightColor);
                    if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        streetLightColor.colorValue = new Color(1f, 0.7451f, 0.27451f, 1f);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                isShowingLightHelp = EditorGUILayout.Foldout(isShowingLightHelp, status);
                if (isShowingLightHelp)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Point lights: Enabled means that point lights for the traffic lights (and street lights, if enabled) will be turned on. This is accessible via script \"bLightsEnabled\"");

                    GUILayout.Space(4f);
                    EditorGUILayout.LabelField("If street pole lights enabled: Street light range, intensity and color: These settings directly correlate to the standard point light settings.");

                    GUILayout.Space(4f);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f));
                    EditorGUILayout.LabelField(" = Resets settings to default.");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                EditorUtilities.DrawLine();

                //Option: Traffic light scaling sensitivity:
                EditorGUILayout.LabelField("Traffic light scaling sensitivity: *Does not auto-update");
                EditorGUILayout.BeginHorizontal();
                scalingSense.floatValue = EditorGUILayout.Slider(intersection.scalingSense, 0f, 200f);
                if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    scalingSense.floatValue = 170f;
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4f);
                isShowingTLSense = EditorGUILayout.Foldout(isShowingTLSense, status);
                if (isShowingTLSense)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Increasing this value will increase the scaling sensitivity relative to the size of the intersection. Higher scaling value = bigger traffic lights at further distances. Default value is 170.");
                    GUILayout.Space(4f);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f));
                    EditorGUILayout.LabelField(" = Resets settings to default.");
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
                GUILayout.Space(4f);
            }

            //Option: Grade correction modifier:
            EditorUtilities.DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grade correction factor: *Does not auto-update");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            gradeMod.floatValue = EditorGUILayout.Slider(intersection.gradeMod, 0.01f, 2f);
            if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                gradeMod.floatValue = 0.375f;
            }
            EditorGUILayout.EndHorizontal();
            isGradeCorrect = EditorGUILayout.Foldout(isGradeCorrect, status);
            if (isGradeCorrect)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("If using extreme road grades immediately surrounding the intersection, terrain height matching errors may occur at the point of road expansion leading to the intersection.");
                EditorGUILayout.LabelField("Raise this value if you see terrain poking through the road at the road expansion areas.");
                EditorGUILayout.LabelField("Lower this value if you are not using road mesh colliders and notice dips at the expansion points.");
                EditorGUILayout.LabelField("Default value is 0.375 meters, which is the maximum value for a linear range of 0% to 20% grade.");
                EditorGUILayout.LabelField("Recommended to keep grades and angles small leading up to intersections.");
                GUILayout.Space(4f);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f));
                EditorGUILayout.LabelField(" = Resets settings to default.");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            //Option: Use default materials:
            EditorUtilities.DrawLine();
            isUsingDefaultMaterials.boolValue = EditorGUILayout.Toggle("Use default materials:", intersection.isUsingDefaultMaterials);
            isShowingDefaultMatHelp = EditorGUILayout.Foldout(isShowingDefaultMatHelp, status);
            if (isShowingDefaultMatHelp)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Checking this option will reset all materials for this intersection and use the default intersection materials that come with this addon.");
                EditorGUILayout.EndVertical();
            }


            EditorUtilities.DrawLine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Center marker material(s):");
            if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.ResetCenterMaterials();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4f);
            //Options: Center materials:
            //		EditorGUILayout.BeginHorizontal();
            //		EditorGUILayout.PropertyField (t_MarkerCenter1, new GUIContent ("Center material #1:"));
            //		if(tInter.MarkerCenter1 != null && GUILayout.Button(btnDeleteText,GSDImageButton,GUILayout.Width(16f))){ t_MarkerCenter1 = null; }
            //		EditorGUILayout.EndHorizontal();
            //		if(tInter.MarkerCenter1 != null){
            //			EditorGUILayout.BeginHorizontal();
            //			EditorGUILayout.PropertyField (t_MarkerCenter2, new GUIContent ("Center material #2:"));
            //			if(tInter.MarkerCenter2 != null && GUILayout.Button(btnDeleteText,GSDImageButton,GUILayout.Width(16f))){ t_MarkerCenter2 = null; }
            //			EditorGUILayout.EndHorizontal();
            //			if(tInter.MarkerCenter2 != null){
            //				EditorGUILayout.BeginHorizontal();
            //				EditorGUILayout.PropertyField (t_MarkerCenter3, new GUIContent ("Center material #3:"));
            //				if(tInter.MarkerCenter3 != null && GUILayout.Button(btnDeleteText,GSDImageButton,GUILayout.Width(16f))){ t_MarkerCenter3 = null; }
            //				EditorGUILayout.EndHorizontal();
            //			}
            //		}

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(markerCenter1, new GUIContent("  Mat #1: "));
            if (intersection.markerCenter1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.markerCenter1 = null;
            }
            EditorGUILayout.EndHorizontal();

            if (intersection.markerCenter1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerCenter2, new GUIContent("  Mat #2: "));
                if (intersection.markerCenter2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerCenter2 = null;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (intersection.markerCenter2 != null && intersection.markerCenter1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerCenter3, new GUIContent("  Mat #3: "));
                if (intersection.markerCenter3 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerCenter3 = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            isShowingMarkerCenter = EditorGUILayout.Foldout(isShowingMarkerCenter, status);
            if (isShowingMarkerCenter)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Center marker materials require transparent shaders. Covers the center area of the intersection. Displayed in order #1 on bottom to #4 on top. Combine with transparent shaders which accept shadows to allow for easy marking.");
                DoDefaultHelpMat();
                DoDeleteHelpMat();
                EditorGUILayout.EndVertical();
            }
            EditorUtilities.DrawLine();

            //Options: Marker ext stretched materials:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Exterior fitted marker material(s):");
            if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.ResetExtStrechtedMaterials();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(markerExtStretch1, new GUIContent("  Mat #1: "));
            if (intersection.markerExtStretch1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.markerExtStretch1 = null;
            }
            EditorGUILayout.EndHorizontal();

            if (intersection.markerExtStretch1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerExtStretch2, new GUIContent("  Mat #2: "));
                if (intersection.markerExtStretch2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerExtStretch2 = null;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (intersection.markerExtStretch2 != null && intersection.markerExtStretch1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerExtStretch3, new GUIContent("  Mat #3: "));
                if (intersection.markerExtStretch3 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerExtStretch3 = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            isShowingMarkerStretch = EditorGUILayout.Foldout(isShowingMarkerStretch, status);
            if (isShowingMarkerStretch)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Fitted marker materials require transparent shaders. Covers the exterior area of the intersection with the UV's stretched to match at a 1:1 ratio. Should be use for intersection markings and any visual effects like dirt." + helpText1);
                DoDefaultHelpMat();
                DoDeleteHelpMat();
                EditorGUILayout.EndVertical();
            }
            EditorUtilities.DrawLine();

            //Options: Marker ext tiled materials:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Exterior tiled marker material(s):");
            if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.ResetExtTiledMaterials();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(markerExtTiled1, new GUIContent("  Mat #1: "));
            if (intersection.markerExtTiled1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.markerExtTiled1 = null;
            }
            EditorGUILayout.EndHorizontal();

            if (intersection.markerExtTiled1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerExtTiled2, new GUIContent("  Mat #2: "));
                if (intersection.markerExtTiled2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerExtTiled2 = null;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (intersection.markerExtTiled2 != null && intersection.markerExtTiled1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(markerExtTiled3, new GUIContent("  Mat #3: "));
                if (intersection.markerExtTiled3 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.markerExtTiled3 = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            isShowingMarkerTiled = EditorGUILayout.Foldout(isShowingMarkerTiled, status);
            if (isShowingMarkerTiled)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Covers the exterior area of the intersection with the UV's tiled matching world coordinates. Tiled and used for road pavement textures. UV coordinates will match up seamlessly with road pavement." + helpText1);
                DoDefaultHelpMat();
                DoDeleteHelpMat();
                EditorGUILayout.EndVertical();
            }
            EditorUtilities.DrawLine();

            //Option: Lane section 0:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Lanes marker materials:");
            if (GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.ResetLanesMaterials();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(lane0Mat1, new GUIContent("Lane section 0 mat #1:"));
            if (intersection.lane0Mat1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.lane0Mat1 = null;
            }
            EditorGUILayout.EndHorizontal();
            if (intersection.lane0Mat1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lane0Mat2, new GUIContent("Lane section 0 mat #2:"));
                if (intersection.lane0Mat2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.lane0Mat2 = null;
                }
                EditorGUILayout.EndHorizontal();
            }


            //Option: Lane section 1:
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(lane1Mat1, new GUIContent("Lane section 1 mat #1:"));
            if (intersection.lane1Mat1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
            {
                intersection.lane1Mat1 = null;
            }
            EditorGUILayout.EndHorizontal();
            if (intersection.lane1Mat1 != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lane1Mat2, new GUIContent("Lane section 1 mat #2:"));
                if (intersection.lane1Mat2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.lane1Mat2 = null;
                }
                EditorGUILayout.EndHorizontal();
            }

            //Option: Lane section 2:
            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes || intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lane2Mat1, new GUIContent("Lane section 2 mat #1:"));
                if (intersection.lane2Mat1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.lane2Mat1 = null;
                }
                EditorGUILayout.EndHorizontal();
                if (intersection.lane2Mat1 != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(lane2Mat2, new GUIContent("Lane section 2 mat #2:"));
                    if (intersection.lane2Mat2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        intersection.lane2Mat2 = null;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(4f);

            //Option: Lane section 3:
            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(lane3Mat1, new GUIContent("Lane section 3 mat #1:"));
                if (intersection.lane3Mat1 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                {
                    intersection.lane3Mat1 = null;
                }
                EditorGUILayout.EndHorizontal();
                if (intersection.lane3Mat1 != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(lane3Mat2, new GUIContent("Lane section 3 mat #2:"));
                    if (intersection.lane3Mat2 != null && GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f)))
                    {
                        intersection.lane3Mat2 = null;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            isShowingLanes = EditorGUILayout.Foldout(isShowingLanes, status);
            if (isShowingLanes)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Covers individual lane sections in the intersection. Used for high-definition line marking.");
                EditorGUILayout.LabelField("Lane section 0 = Left side of the intersection where oncoming traffic travels. Use lane texture which matches the number of lanes used on the road system, with a white left line and a yellow right line.");

                if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    EditorGUILayout.LabelField("Lane section 1 = Left turn lane. Use a single lane texture with a yellow left line and a white right line.");
                    EditorGUILayout.LabelField("Lane section 2 = Right outgoing traffic lane. Use lane texture which matches the number of lanes used on the road system with a white right line.");
                    EditorGUILayout.LabelField("Lane section 3 = Right turn lane. Use a single lane texture with a white right line.");
                }
                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    EditorGUILayout.LabelField("Lane section 1 = Left turn lane. Use a single lane texture with a yellow left line and a white right line.");
                    EditorGUILayout.LabelField("Lane section 2 = Right outgoing traffic lane. Use lane texture which matches the number of lanes used on the road system with a white right line.");
                }
                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    EditorGUILayout.LabelField("Lane section 1 = Right outgoing traffic lane. Use lane texture which matches the number of lanes used on the road system with a white right line.");
                }

                EditorGUILayout.LabelField(helpText1);
                DoDefaultHelpMat();
                DoDeleteHelpMat();
                EditorGUILayout.EndVertical();
            }
            EditorUtilities.DrawLine();

            //if(isDebug)
            //{
            //	Line();
            //	EditorGUILayout.LabelField("Debug");
            //	if(tInter.Node1 != null)
            //  {
            //      EditorGUILayout.LabelField("  Node1: " + tInter.Node1.transform.name);
            //  }
            //  else
            //  {
            //      EditorGUILayout.LabelField("  Node1: null");
            //  }
            //	if(tInter.Node2 != null)
            //  {
            //      EditorGUILayout.LabelField("  Node2: " + tInter.Node2.transform.name);
            //  }
            //  else
            //  {
            //      EditorGUILayout.LabelField("  Node2: null");
            //  }
            //	if(tInter.Node1 != null)
            //  {
            //      EditorGUILayout.LabelField("  UID1: " + tInter.Node1.UID);
            //  }
            //  else
            //  {
            //      EditorGUILayout.LabelField("  UID1: null");
            //  }
            //	if(tInter.Node2 != null)
            //  {
            //      EditorGUILayout.LabelField("  UID2: " + tInter.Node2.UID);
            //  }
            //  else
            //  {
            //      EditorGUILayout.LabelField("  UID2: null");
            //  }
            //	EditorGUILayout.LabelField("  Same spline: " + tInter.bSameSpline);
            //	EditorGUILayout.LabelField("  bFlipped: " + tInter.bFlipped);
            //	EditorGUILayout.LabelField("  IgnoreSide: " + tInter.IgnoreSide);
            //	EditorGUILayout.LabelField("  IgnoreCorner: " + tInter.IgnoreCorner);
            //			
            //	if(tInter.iStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || tInter.iStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
            //  {
            //		if(tInter.LightsRR != null)
            //      {
            //          EditorGUILayout.LabelField("  LightsRR: " + tInter.LightsRR);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsRR: null");
            //      }
            //		if(tInter.LightsRR != null || tInter.LightsRR.MR_Main != null)
            //      {
            //          EditorGUILayout.LabelField("   MR_Main: " + tInter.LightsRR.MR_Main);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsRR.MR_Main: null");
            //      }
            //	    if(tInter.LightsRR != null || tInter.LightsRR.MR_Left != null)
            //      {
            //          EditorGUILayout.LabelField("   MR_Left: " + tInter.LightsRR.MR_Left);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsRR.MR_Left: null");
            //      }
            //	    if(tInter.LightsRR != null || tInter.LightsRR.MR_Right != null)
            //      {
            //          EditorGUILayout.LabelField("   MR_Right: " + tInter.LightsRR.MR_Right);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsRR.MR_Right: null");
            //      }
            //	    if(tInter.LightsRL != null)
            //      {
            //          EditorGUILayout.LabelField("  LightsRL: " + tInter.LightsRL);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsRL: null");
            //      }
            //		if(tInter.LightsLL != null)
            //      {
            //          EditorGUILayout.LabelField("  LightsLL: " + tInter.LightsLL);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsLL: null");
            //      }
            //		if(tInter.LightsLR != null)
            //      {
            //          EditorGUILayout.LabelField("  LightsLR: " + tInter.LightsLR);
            //      }
            //      else
            //      {
            //          EditorGUILayout.LabelField("  LightsLR: null");
            //      }
            //	}
            //  Line();
            //}


            //Set change bools:
            bool isTrafficLightPoleColorToggled = false;
            bool isPointLightsToggled = false;
            bool isGizmoChanged = false;
            bool isMaterialChanged = false;
            bool isRoadUpdate = false;

            if (GUI.changed)
            {
                //Option: Gizmo:
                if (isDrawingGizmo.boolValue != intersection.isDrawingGizmo)
                {
                    isGizmoChanged = true;
                }

                //Option: Intersection turn lane options
                if ((int)intersection.roadType != roadType.enumValueIndex)
                {
                    isMaterialChanged = true;
                    isRoadUpdate = true;
                }

                //Option: Left yield on green:
                if (isLeftTurnYieldOnGreen.boolValue != intersection.isLeftTurnYieldOnGreen)
                {
                    isRoadUpdate = true;
                }

                //Option: Intersection stop type:
                if (intersectionStopType.enumValueIndex != (int)intersection.intersectionStopType)
                {
                    isRoadUpdate = true;
                }

                //Option: Traffic light poles:
                if (isTrafficPoleStreetLight.boolValue != intersection.isTrafficPoleStreetLight)
                {
                    isRoadUpdate = true;
                }

                //Option: Traffic light pole gray color:
                if (isTrafficLightGray.boolValue != intersection.isTrafficLightGray)
                {
                    isTrafficLightPoleColorToggled = true;
                }

                //Option: Normal pole alignment:
                if (isRegularPoleAlignment.boolValue != intersection.isRegularPoleAlignment)
                {
                    isRoadUpdate = true;
                }

                //Option: Point lights enabled:
                if (isLightsEnabled.boolValue != intersection.isLightsEnabled)
                {
                    isPointLightsToggled = true;
                }

                //Option: Street light range:
                if (!RootUtils.IsApproximately(streetLightRange.floatValue, intersection.streetLightRange, 0.01f))
                {
                    isPointLightsToggled = true;
                }

                //Option: Street light intensity:
                if (!RootUtils.IsApproximately(streetLightIntensity.floatValue, intersection.streetLightIntensity, 0.01f))
                {
                    isPointLightsToggled = true;
                }

                //Option: Street light color:
                if (streetLightColor.colorValue != intersection.streetLightColor)
                {
                    isPointLightsToggled = true;
                }

                //Option: Use default materials:
                if (isUsingDefaultMaterials.boolValue != intersection.isUsingDefaultMaterials)
                {
                    isMaterialChanged = true;
                }

                if (markerCenter1 == null || markerCenter1.objectReferenceValue == null)
                {
                    intersection.markerCenter1 = null;
                }
                if (markerCenter2 == null || markerCenter2.objectReferenceValue == null)
                {
                    intersection.markerCenter2 = null;
                }
                if (markerCenter3 == null || markerCenter3.objectReferenceValue == null)
                {
                    intersection.markerCenter3 = null;
                }
                if (markerExtStretch1 == null || markerExtStretch1.objectReferenceValue == null)
                {
                    intersection.markerExtStretch1 = null;
                }
                if (markerExtStretch2 == null || markerExtStretch2.objectReferenceValue == null)
                {
                    intersection.markerExtStretch2 = null;
                }
                if (markerExtStretch3 == null || markerExtStretch3.objectReferenceValue == null)
                {
                    intersection.markerExtStretch3 = null;
                }
                if (markerExtTiled1 == null || markerExtTiled1.objectReferenceValue == null)
                {
                    intersection.markerExtTiled1 = null;
                }
                if (markerExtTiled2 == null || markerExtTiled2.objectReferenceValue == null)
                {
                    intersection.markerExtTiled2 = null;
                }
                if (markerExtTiled3 == null || markerExtTiled3.objectReferenceValue == null)
                {
                    intersection.markerExtTiled3 = null;
                }
                if (lane0Mat1 == null || lane0Mat1.objectReferenceValue == null)
                {
                    intersection.lane0Mat1 = null;
                }
                if (lane0Mat2 == null || lane0Mat2.objectReferenceValue == null)
                {
                    intersection.lane0Mat2 = null;
                }
                if (lane1Mat1 == null || lane1Mat1.objectReferenceValue == null)
                {
                    intersection.lane1Mat1 = null;
                }
                if (lane1Mat2 == null || lane1Mat2.objectReferenceValue == null)
                {
                    intersection.lane1Mat2 = null;
                }
                if (lane2Mat1 == null || lane2Mat1.objectReferenceValue == null)
                {
                    intersection.lane2Mat1 = null;
                }
                if (lane2Mat2 == null || lane2Mat2.objectReferenceValue == null)
                {
                    intersection.lane2Mat2 = null;
                }
                if (lane3Mat1 == null || lane3Mat1.objectReferenceValue == null)
                {
                    intersection.lane3Mat1 = null;
                }
                if (lane3Mat2 == null || lane3Mat2.objectReferenceValue == null)
                {
                    intersection.lane3Mat2 = null;
                }

                if (intersection.markerCenter1 != null && markerCenter1.objectReferenceValue != intersection.markerCenter1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerCenter2 != null && markerCenter2.objectReferenceValue != intersection.markerCenter2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerCenter3 != null && markerCenter3.objectReferenceValue != intersection.markerCenter3)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtStretch1 != null && markerExtStretch1.objectReferenceValue != intersection.markerExtStretch1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtStretch2 != null && markerExtStretch2.objectReferenceValue != intersection.markerExtStretch2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtStretch3 != null && markerExtStretch3.objectReferenceValue != intersection.markerExtStretch3)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtTiled1 != null && markerExtTiled1.objectReferenceValue != intersection.markerExtTiled1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtTiled2 != null && markerExtTiled2.objectReferenceValue != intersection.markerExtTiled2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.markerExtTiled3 != null && markerExtTiled3.objectReferenceValue != intersection.markerExtTiled3)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane0Mat1 != null && lane0Mat1.objectReferenceValue != intersection.lane0Mat1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane0Mat2 != null && lane0Mat2.objectReferenceValue != intersection.lane0Mat2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane1Mat1 != null && lane1Mat1.objectReferenceValue != intersection.lane1Mat1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane1Mat2 != null && lane1Mat2.objectReferenceValue != intersection.lane1Mat2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane2Mat1 != null && lane2Mat1.objectReferenceValue != intersection.lane2Mat1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane2Mat2 != null && lane2Mat2.objectReferenceValue != intersection.lane2Mat2)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane3Mat1 != null && lane3Mat1.objectReferenceValue != intersection.lane3Mat1)
                {
                    isMaterialChanged = true;
                }
                if (intersection.lane3Mat2 != null && lane3Mat2.objectReferenceValue != intersection.lane3Mat2)
                {
                    isMaterialChanged = true;
                }


                //Apply changes:
                serializedObject.ApplyModifiedProperties();


                //Apply secondary effects:
                if (isGizmoChanged)
                {
                    MeshRenderer[] meshRenderers = intersection.transform.GetComponentsInChildren<MeshRenderer>();
                    int count = meshRenderers.Length;
                    for (int i = 0; i < count; i++)
                    {
                        EngineIntegration.SetSelectedRenderState(meshRenderers[i], intersection.isDrawingGizmo);
                    }
                    SceneView.RepaintAll();
                }

                if (isPointLightsToggled)
                {
                    intersection.TogglePointLights(intersection.isLightsEnabled);
                }

                if (isTrafficLightPoleColorToggled)
                {
                    intersection.ToggleTrafficLightPoleColor();
                }

                if (isMaterialChanged)
                {
                    intersection.UpdateMaterials();
                    if (intersection.isUsingDefaultMaterials)
                    {
                        intersection.ResetMaterialsAll();
                    }
                }

                if (isRoadUpdate)
                {
                    TriggerRoadUpdate();
                }

                EditorUtility.SetDirty(intersection);
            }
        }


        public void OnSceneGUI()
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

            //		if (Event.current.type == EventType.MouseDrag && Event.current.button == 0){
            //			//Update relevant splines:
            //			tInter.Node1.spline.Setup();
            //			if(!tInter.bSameSpline){
            //				tInter.Node2.spline.Setup();
            //			}
            //			bMouseDragHasProcessed = false;
            //		}
            //		
            //		if(Event.current.type == EventType.MouseUp && Event.current.button == 0){
            //			if(!bMouseDragHasProcessed){
            //				tInter.Node1.transform.position = tInter.transform.position;
            //				tInter.Node2.transform.position = tInter.transform.position;
            //				tInter.StartCoroutine(TriggerRoadUpdate());
            //			} 
            //			bMouseDragHasProcessed = true;
            //		}

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (intersection.transform.position != intersection.node1.transform.position)
                {
                    intersection.node1.transform.position = intersection.transform.position;
                    intersection.node2.transform.position = intersection.transform.position;
                    intersection.height = intersection.transform.position.y;
                    TriggerRoadUpdate();
                }
            }



            if (current.type == EventType.ValidateCommand)
            {
                switch (current.commandName)
                {
                    case "UndoRedoPerformed":
                        TriggerRoadUpdate(true);
                        break;
                }
            }

            if (current.type == EventType.MouseDown)
            {
                return;
            }

            if (Selection.activeGameObject == intersection.transform.gameObject)
            {
                if (current.keyCode == KeyCode.F5)
                {
                    if (intersection != null && intersection.node1 != null && intersection.node2 != null)
                    {
                        TriggerRoadUpdate();
                    }
                }
            }

            //		switch(current.type){
            //			case EventType.layout:
            ////			if(current.type != EventType.MouseDown){
            //		        HandleUtility.AddDefaultControl(controlID);
            ////			}
            //		    break;
            //				
            //		}

            if (controlID == 1)
            {
                //Do nothing	
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(intersection);
            }
        }


        private void TriggerRoadUpdate(bool _isForced = false)
        {
            if (!_isForced && !intersection.isAutoUpdatingIntersection)
            {
                return;
            }

            if (intersection != null && intersection.node1 != null && intersection.node2 != null)
            {
                if (!intersection.node1.spline.road.isEditorConstructing && !intersection.node2.spline.road.isEditorConstructing)
                {
                    if (!intersection.isSameSpline)
                    {
                        intersection.node1.spline.road.PiggyBacks = new SplineC[1];
                        intersection.node1.spline.road.PiggyBacks[0] = intersection.node2.spline;
                    }
                    intersection.node1.spline.road.isUpdateRequired = true;
                }
            }
        }


        private void Init()
        {
            EditorStyles.label.wordWrap = true;
            isInitialized = true;
            string basePath = RoadEditorUtility.GetBasePath();

            EditorUtilities.LoadTexture(ref refreshButtonTexture, basePath + "/Editor/Icons/refresh2.png");
            EditorUtilities.LoadTexture(ref deleteButtonTexture, basePath + "/Editor/Icons/delete.png");
            EditorUtilities.LoadTexture(ref loadButtonBG, basePath + "/Editor/Icons/otherbg.png");
            EditorUtilities.LoadTexture(ref loadButtonBGGlow, basePath + "/Editor/Icons/otherbg2.png");

            if (imageButton == null)
            {
                imageButton = new GUIStyle(GUI.skin.button);
                imageButton.contentOffset = new Vector2(0f, 0f);
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
            }
        }


        private void DoDefaultHelpMat()
        {
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button(refreshButtonTexture, imageButton, GUILayout.Width(16f));
            EditorGUILayout.LabelField(" = Resets material(s) to default materials.");
            EditorGUILayout.EndHorizontal();
        }


        private void DoDeleteHelpMat()
        {
            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Button(deleteButtonTexture, imageButton, GUILayout.Width(16f));
            EditorGUILayout.LabelField(" = Removes material(s) from intersection.");
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
