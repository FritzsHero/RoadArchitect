#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(RoadTerrain))]
    public class TerrainEditor : Editor
    {
        #region "Vars"
        private RoadTerrain terrain;

        //Serialized properties:
        SerializedProperty splatImageWidth;
        SerializedProperty splatImageHeight;
        SerializedProperty splatBackgroundColor;
        SerializedProperty splatForegroundColor;
        SerializedProperty splatWidth;
        SerializedProperty isSkippingBridges;
        SerializedProperty isSkippingTunnels;
        SerializedProperty isSplatSingleRoad;
        SerializedProperty splatSingleChoiceIndex;
        SerializedProperty roadSingleChoiceUID;

        //Editor only variables:
        private bool isInitialized;
        private string[] roads = null;
        private string[] roadsString = null;
        private Texture refreshButtonText = null;
        private GUIStyle imageButton = null;
        private Texture2D loadButtonBG = null;
        private Texture2D loadButtonBGGlow = null;
        private GUIStyle loadButton = null;
        SplatImageResoMatchingEnum splatReso = SplatImageResoMatchingEnum.None;
        #endregion


        public enum SplatImageResoMatchingEnum
        {
            None,
            Match512x512,
            Match1024x1024,
            Match2048x2048,
            Match4096x4096,
            MatchHeightmapResolution,
            MatchDetailResolution,
            MatchTerrainSize
        };


        private static string[] TheSplatResoOptions = new string[]{
        "Select option to match resolution",
        "512 x 512",
        "1024 x 1024",
        "2048 x 2048",
        "4096 x 4096",
        "Match heightmap resolution",
        "Match detail resolution",
        "Match terrain size"
    };


        private void OnEnable()
        {
            terrain = (RoadTerrain)target;

            splatImageWidth = serializedObject.FindProperty("splatResoWidth");
            splatImageHeight = serializedObject.FindProperty("splatResoHeight");
            splatBackgroundColor = serializedObject.FindProperty("splatBackground");
            splatForegroundColor = serializedObject.FindProperty("splatForeground");
            splatWidth = serializedObject.FindProperty("splatWidth");
            isSkippingBridges = serializedObject.FindProperty("isSplatSkipBridges");
            isSkippingTunnels = serializedObject.FindProperty("isSplatSkipTunnels");
            isSplatSingleRoad = serializedObject.FindProperty("isSplatSingleRoad");
            splatSingleChoiceIndex = serializedObject.FindProperty("splatSingleChoiceIndex");
            roadSingleChoiceUID = serializedObject.FindProperty("roadSingleChoiceUID");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if(!isInitialized)
            {
                isInitialized = true;
                InitNullChecks();
            }

            RoadArchitect.EditorUtilities.DrawLine();
            EditorGUILayout.BeginHorizontal();
            //Main label:
            EditorGUILayout.LabelField("Splat map generation:", EditorStyles.boldLabel);
            //Online manual button:
            if (GUILayout.Button("Online manual", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                Application.OpenURL("https://github.com/MicroGSD/RoadArchitect/wiki");
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(6f);

            //Splat Resolution input:
            splatImageWidth.intValue = terrain.splatResoWidth;
            splatImageHeight.intValue = terrain.splatResoHeight;
            EditorGUILayout.BeginHorizontal();
            splatReso = (SplatImageResoMatchingEnum)EditorGUILayout.Popup("Match resolutions:", (int)splatReso, TheSplatResoOptions);
            if (GUILayout.Button(refreshButtonText, imageButton, GUILayout.Width(16f)))
            {
                splatImageWidth.intValue = 1024;
                splatImageHeight.intValue = 1024;
            }
            EditorGUILayout.EndHorizontal();

            if (splatReso != SplatImageResoMatchingEnum.None)
            {
                if (splatReso == SplatImageResoMatchingEnum.MatchHeightmapResolution)
                {
                    splatImageWidth.intValue = terrain.terrain.terrainData.heightmapResolution;
                    splatImageHeight.intValue = terrain.terrain.terrainData.heightmapResolution;
                }
                else if (splatReso == SplatImageResoMatchingEnum.MatchDetailResolution)
                {
                    splatImageWidth.intValue = terrain.terrain.terrainData.detailResolution;
                    splatImageHeight.intValue = terrain.terrain.terrainData.detailResolution;
                }
                else if (splatReso == SplatImageResoMatchingEnum.MatchTerrainSize)
                {
                    splatImageWidth.intValue = (int)terrain.terrain.terrainData.size.x;
                    splatImageHeight.intValue = (int)terrain.terrain.terrainData.size.z;
                }
                else if (splatReso == SplatImageResoMatchingEnum.Match512x512)
                {
                    splatImageWidth.intValue = 512;
                    splatImageHeight.intValue = 512;
                }
                else if (splatReso == SplatImageResoMatchingEnum.Match1024x1024)
                {
                    splatImageWidth.intValue = 1024;
                    splatImageHeight.intValue = 1024;
                }
                else if (splatReso == SplatImageResoMatchingEnum.Match2048x2048)
                {
                    splatImageWidth.intValue = 2048;
                    splatImageHeight.intValue = 2048;
                }
                else if (splatReso == SplatImageResoMatchingEnum.Match4096x4096)
                {
                    splatImageWidth.intValue = 4096;
                    splatImageHeight.intValue = 4096;
                }
                splatReso = SplatImageResoMatchingEnum.None;
            }

            //Splat image width input:
            splatImageWidth.intValue = EditorGUILayout.IntField("Splat image width:", splatImageWidth.intValue);
            //Splat image height input:
            splatImageHeight.intValue = EditorGUILayout.IntField("Splat image height:", splatImageHeight.intValue);


            //Splat background color input:
            EditorGUILayout.BeginHorizontal();
            splatBackgroundColor.colorValue = EditorGUILayout.ColorField("Splat background:", terrain.splatBackground);
            //Default button:
            if (GUILayout.Button(refreshButtonText, imageButton, GUILayout.Width(16f)))
            {
                splatBackgroundColor.colorValue = new Color(0f, 0f, 0f, 1f);
            }
            EditorGUILayout.EndHorizontal();

            //Splat foreground color input:
            EditorGUILayout.BeginHorizontal();
            splatForegroundColor.colorValue = EditorGUILayout.ColorField("Splat foreground:", terrain.splatForeground);
            //Default button:
            if (GUILayout.Button(refreshButtonText, imageButton, GUILayout.Width(16f)))
            {
                splatForegroundColor.colorValue = new Color(1f, 1f, 1f, 1f);
            }
            EditorGUILayout.EndHorizontal();

            //Splat width (meters) input:
            EditorGUILayout.BeginHorizontal();
            splatWidth.floatValue = EditorGUILayout.Slider("Splat width (meters):", terrain.splatWidth, 0.02f, 256f);
            //Default button:
            if (GUILayout.Button(refreshButtonText, imageButton, GUILayout.Width(16f)))
            {
                splatWidth.floatValue = 30f;
            }
            EditorGUILayout.EndHorizontal();

            //Skip bridges:
            isSkippingBridges.boolValue = EditorGUILayout.Toggle("Skip bridges: ", terrain.isSplatSkipBridges);

            //Skip tunnels:
            isSkippingTunnels.boolValue = EditorGUILayout.Toggle("Skip tunnels: ", terrain.isSplatSkipTunnels);

            //Splat single road bool input:
            EditorGUILayout.BeginHorizontal();
            isSplatSingleRoad.boolValue = EditorGUILayout.Toggle("Splat a single road: ", terrain.isSplatSingleRoad);

            //Splat single road , road input:
            if (terrain.isSplatSingleRoad)
            {
                LoadSplatSingleChoice();
                splatSingleChoiceIndex.intValue = EditorGUILayout.Popup(terrain.splatSingleChoiceIndex, roadsString, GUILayout.Width(150f));
                roadSingleChoiceUID.stringValue = roads[splatSingleChoiceIndex.intValue];
            }

            EditorGUILayout.EndHorizontal();

            //Generate splatmap button:
            GUILayout.Space(8f);
            if (GUILayout.Button("Generate splatmap for this terrain"))
            {
                GenerateSplatMap();
            }
            GUILayout.Space(10f);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                //Necessary?
                //EditorUtility.SetDirty(target);
            }
        }


        private void InitNullChecks()
        {
            string basePath = RoadEditorUtility.GetBasePath();

            RoadArchitect.EditorUtilities.LoadTexture(ref refreshButtonText, basePath + "/Editor/Icons/refresh2.png");
            RoadArchitect.EditorUtilities.LoadTexture(ref loadButtonBG, basePath + "/Editor/Icons/FlexBG.png");
            RoadArchitect.EditorUtilities.LoadTexture(ref loadButtonBGGlow, basePath + "/Editor/Icons/FlexBG.png");

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
                loadButton.padding = new RectOffset(0, 0, 0, 0);
            }
        }


        private void LoadSplatSingleChoice()
        {
            roads = null;
            roadsString = null;
            Object[] allRoads = GameObject.FindObjectsOfType<Road>();
            int roadsCount = allRoads.Length;
            roads = new string[roadsCount];
            roadsString = new string[roadsCount];
            int counter = 0;
            foreach (Road road in allRoads)
            {
                roads[counter] = road.UID;
                roadsString[counter] = road.transform.name;
                counter += 1;
            }
        }


        private void GenerateSplatMap()
        {
            byte[] bytes = null;
            if (terrain.isSplatSingleRoad && terrain.roadSingleChoiceUID != "")
            {
                bytes = RoadUtility.MakeSplatMap(terrain.terrain, terrain.splatBackground, terrain.splatForeground, terrain.splatResoWidth, terrain.splatResoHeight, terrain.splatWidth, terrain.isSplatSkipBridges, terrain.isSplatSkipTunnels, terrain.roadSingleChoiceUID);
            }
            else
            {
                bytes = RoadUtility.MakeSplatMap(terrain.terrain, terrain.splatBackground, terrain.splatForeground, terrain.splatResoWidth, terrain.splatResoHeight, terrain.splatWidth, terrain.isSplatSkipBridges, terrain.isSplatSkipTunnels);
            }

            if (bytes != null && bytes.Length > 3)
            {
                string path = UnityEditor.EditorUtility.SaveFilePanel("Save splat map", Application.dataPath, "Splat", "png");
                if (path != null && path.Length > 3)
                {
                    System.IO.File.WriteAllBytes(path, bytes);
                }
                bytes = null;
            }
        }
    }
}
#endif
