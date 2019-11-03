#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDTerrain))]
public class GSDTerrainEditor : Editor
{
    protected GSDTerrain GSDT { get { return (GSDTerrain) target; } }

    //Serialized properties:
    SerializedProperty tSplatImageWidth;
    SerializedProperty tSplatImageHeight;
    SerializedProperty tSplatBackgroundColor;
    SerializedProperty tSplatForegroundColor;
    SerializedProperty tSplatWidth;
    SerializedProperty tSkipBridges;
    SerializedProperty tSkipTunnels;
    SerializedProperty tSplatSingleRoad;
    SerializedProperty tSplatSingleChoiceIndex;
    SerializedProperty tRoadSingleChoiceUID;

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
    SplatImageResoMatchingEnum tSplatReso = SplatImageResoMatchingEnum.None;

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

    //Editor only variables:
    private string[] tRoads = null;
    private string[] tRoadsString = null;
    private Texture btnRefreshText = null;
    private GUIStyle GSDImageButton = null;
    private Texture2D LoadBtnBG = null;
    private Texture2D LoadBtnBGGlow = null;
    private GUIStyle GSDLoadButton = null;


    private void OnEnable()
    {
        tSplatImageWidth = serializedObject.FindProperty("SplatResoWidth");
        tSplatImageHeight = serializedObject.FindProperty("SplatResoHeight");
        tSplatBackgroundColor = serializedObject.FindProperty("SplatBackground");
        tSplatForegroundColor = serializedObject.FindProperty("SplatForeground");
        tSplatWidth = serializedObject.FindProperty("SplatWidth");
        tSkipBridges = serializedObject.FindProperty("SplatSkipBridges");
        tSkipTunnels = serializedObject.FindProperty("SplatSkipTunnels");
        tSplatSingleRoad = serializedObject.FindProperty("SplatSingleRoad");
        tSplatSingleChoiceIndex = serializedObject.FindProperty("SplatSingleChoiceIndex");
        tRoadSingleChoiceUID = serializedObject.FindProperty("RoadSingleChoiceUID");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        InitNullChecks();

        RAEditorUtilitys.DrawLine();
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
        tSplatImageWidth.intValue = GSDT.splatResoWidth;
        tSplatImageHeight.intValue = GSDT.splatResoHeight;
        EditorGUILayout.BeginHorizontal();
        tSplatReso = (SplatImageResoMatchingEnum) EditorGUILayout.Popup("Match resolutions:", (int) tSplatReso, TheSplatResoOptions);
        if (GUILayout.Button(btnRefreshText, GSDImageButton, GUILayout.Width(16f)))
        {
            tSplatImageWidth.intValue = 1024;
            tSplatImageHeight.intValue = 1024;
        }
        EditorGUILayout.EndHorizontal();

        if (tSplatReso != SplatImageResoMatchingEnum.None)
        {
            if (tSplatReso == SplatImageResoMatchingEnum.MatchHeightmapResolution)
            {
                tSplatImageWidth.intValue = GSDT.terrain.terrainData.heightmapResolution;
                tSplatImageHeight.intValue = GSDT.terrain.terrainData.heightmapResolution;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.MatchDetailResolution)
            {
                tSplatImageWidth.intValue = GSDT.terrain.terrainData.detailResolution;
                tSplatImageHeight.intValue = GSDT.terrain.terrainData.detailResolution;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.MatchTerrainSize)
            {
                tSplatImageWidth.intValue = (int) GSDT.terrain.terrainData.size.x;
                tSplatImageHeight.intValue = (int) GSDT.terrain.terrainData.size.z;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.Match512x512)
            {
                tSplatImageWidth.intValue = 512;
                tSplatImageHeight.intValue = 512;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.Match1024x1024)
            {
                tSplatImageWidth.intValue = 1024;
                tSplatImageHeight.intValue = 1024;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.Match2048x2048)
            {
                tSplatImageWidth.intValue = 2048;
                tSplatImageHeight.intValue = 2048;
            }
            else if (tSplatReso == SplatImageResoMatchingEnum.Match4096x4096)
            {
                tSplatImageWidth.intValue = 4096;
                tSplatImageHeight.intValue = 4096;
            }
            tSplatReso = SplatImageResoMatchingEnum.None;
        }

        //Splat image width input:
        tSplatImageWidth.intValue = EditorGUILayout.IntField("Splat image width:", tSplatImageWidth.intValue);
        //Splat image height input:
        tSplatImageHeight.intValue = EditorGUILayout.IntField("Splat image height:", tSplatImageHeight.intValue);


        //Splat background color input:
        EditorGUILayout.BeginHorizontal();
        tSplatBackgroundColor.colorValue = EditorGUILayout.ColorField("Splat background:", GSDT.splatBackground);
        //Default button:
        if (GUILayout.Button(btnRefreshText, GSDImageButton, GUILayout.Width(16f)))
        {
            tSplatBackgroundColor.colorValue = new Color(0f, 0f, 0f, 1f);
        }
        EditorGUILayout.EndHorizontal();

        //Splat foreground color input:
        EditorGUILayout.BeginHorizontal();
        tSplatForegroundColor.colorValue = EditorGUILayout.ColorField("Splat foreground:", GSDT.splatForeground);
        //Default button:
        if (GUILayout.Button(btnRefreshText, GSDImageButton, GUILayout.Width(16f)))
        {
            tSplatForegroundColor.colorValue = new Color(1f, 1f, 1f, 1f);
        }
        EditorGUILayout.EndHorizontal();

        //Splat width (meters) input:
        EditorGUILayout.BeginHorizontal();
        tSplatWidth.floatValue = EditorGUILayout.Slider("Splat width (meters):", GSDT.splatWidth, 0.02f, 256f);
        //Default button:
        if (GUILayout.Button(btnRefreshText, GSDImageButton, GUILayout.Width(16f)))
        {
            tSplatWidth.floatValue = 30f;
        }
        EditorGUILayout.EndHorizontal();

        //Skip bridges:
        tSkipBridges.boolValue = EditorGUILayout.Toggle("Skip bridges: ", GSDT.isSplatSkipBridges);

        //Skip tunnels:
        tSkipTunnels.boolValue = EditorGUILayout.Toggle("Skip tunnels: ", GSDT.isSplatSkipTunnels);

        //Splat single road bool input:
        EditorGUILayout.BeginHorizontal();
        tSplatSingleRoad.boolValue = EditorGUILayout.Toggle("Splat a single road: ", GSDT.isSplatSingleRoad);

        //Splat single road , road input:
        if (GSDT.isSplatSingleRoad)
        {
            LoadSplatSingleChoice();
            tSplatSingleChoiceIndex.intValue = EditorGUILayout.Popup(GSDT.splatSingleChoiceIndex, tRoadsString, GUILayout.Width(150f));
            tRoadSingleChoiceUID.stringValue = tRoads[tSplatSingleChoiceIndex.intValue];
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
            //			EditorUtility.SetDirty(target); //Necessary?
        }
    }


    private void InitNullChecks()
    {
        if (btnRefreshText == null)
        {
            btnRefreshText = (Texture) AssetDatabase.LoadAssetAtPath(GSD.Roads.GSDRoadUtilityEditor.GetBasePath() + "/Editor/Icons/refresh2.png", typeof(Texture)) as Texture;
        }
        if (GSDImageButton == null)
        {
            GSDImageButton = new GUIStyle(GUI.skin.button);
            GSDImageButton.contentOffset = new Vector2(0f, 0f);
            GSDImageButton.border = new RectOffset(0, 0, 0, 0);
            GSDImageButton.fixedHeight = 16f;
            GSDImageButton.padding = new RectOffset(0, 0, 0, 0);
            GSDImageButton.normal.background = null;
        }
        if (LoadBtnBG == null)
        {
            LoadBtnBG = (Texture2D) AssetDatabase.LoadAssetAtPath(GSD.Roads.GSDRoadUtilityEditor.GetBasePath() + "/Editor/Icons/FlexBG.png", typeof(Texture2D)) as Texture2D;
        }
        if (LoadBtnBGGlow == null)
        {
            LoadBtnBGGlow = (Texture2D) AssetDatabase.LoadAssetAtPath(GSD.Roads.GSDRoadUtilityEditor.GetBasePath() + "/Editor/Icons/FlexBG.png", typeof(Texture2D)) as Texture2D;
        }
        if (GSDLoadButton == null)
        {
            GSDLoadButton = new GUIStyle(GUI.skin.button);
            GSDLoadButton.contentOffset = new Vector2(0f, 1f);
            GSDLoadButton.normal.textColor = new Color(1f, 1f, 1f, 1f);
            GSDLoadButton.normal.background = LoadBtnBG;
            GSDLoadButton.active.background = LoadBtnBGGlow;
            GSDLoadButton.focused.background = LoadBtnBGGlow;
            GSDLoadButton.hover.background = LoadBtnBGGlow;
            GSDLoadButton.fixedHeight = 16f;
            GSDLoadButton.padding = new RectOffset(0, 0, 0, 0);
        }
    }


    private void LoadSplatSingleChoice()
    {
        tRoads = null;
        tRoadsString = null;
        Object[] xRoads = GameObject.FindObjectsOfType(typeof(GSDRoad));
        int xRoadsCount = xRoads.Length;
        tRoads = new string[xRoadsCount];
        tRoadsString = new string[xRoadsCount];
        int xCounter = 0;
        foreach (GSDRoad tRoad in xRoads)
        {
            tRoads[xCounter] = tRoad.UID;
            tRoadsString[xCounter] = tRoad.transform.name;
            xCounter += 1;
        }
    }


    private void GenerateSplatMap()
    {
        byte[] tBytes = null;
        if (GSDT.isSplatSingleRoad && GSDT.roadSingleChoiceUID != "")
        {
            tBytes = GSD.Roads.GSDRoadUtil.MakeSplatMap(GSDT.terrain, GSDT.splatBackground, GSDT.splatForeground, GSDT.splatResoWidth, GSDT.splatResoHeight, GSDT.splatWidth, GSDT.isSplatSkipBridges, GSDT.isSplatSkipTunnels, GSDT.roadSingleChoiceUID);
        }
        else
        {
            tBytes = GSD.Roads.GSDRoadUtil.MakeSplatMap(GSDT.terrain, GSDT.splatBackground, GSDT.splatForeground, GSDT.splatResoWidth, GSDT.splatResoHeight, GSDT.splatWidth, GSDT.isSplatSkipBridges, GSDT.isSplatSkipTunnels);
        }

        if (tBytes != null && tBytes.Length > 3)
        {
            string tPath = UnityEditor.EditorUtility.SaveFilePanel("Save splat map", Application.dataPath, "Splat", "png");
            if (tPath != null && tPath.Length > 3)
            {
                System.IO.File.WriteAllBytes(tPath, tBytes);
            }
            tBytes = null;
        }
    }
}
#endif