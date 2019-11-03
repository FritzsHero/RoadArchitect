#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using GSD.Roads.Splination;
using GSD.Roads.EdgeObjects;
using GSD;
using System.IO;
using GSD.Roads;
#endregion


public class GSDWizard : EditorWindow
{
    public enum WindowTypeEnum
    {
        Extrusion,
        Edge,
        BridgeComplete,
        Groups
    };


    public enum WindowTypeEnumShort
    {
        Extrusion,
        Edge,
        Groups
    };


    private static string[] WindowTypeDescBridge = new string[]{
        "Extrusion items",
        "Edge objects",
        "Complete bridges",
        "Other groups"
    };


    private static string[] WindowTypeDesc = new string[]{
        "Extrusion items",
        "Edge objects",
        "Other groups"
    };


    private WindowTypeEnum tWindowType = WindowTypeEnum.Extrusion;
    private WindowTypeEnum xWindowType = WindowTypeEnum.Extrusion;
    private WindowTypeEnumShort StWindowType = WindowTypeEnumShort.Extrusion;
    private WindowTypeEnumShort SxWindowType = WindowTypeEnumShort.Extrusion;
    private static string xPath = "";

    private GUIStyle ThumbStyle;
    private Vector2 scrollPos = new Vector2(0f, 25f);
    [UnityEngine.Serialization.FormerlySerializedAs("tNode")]
    private GSDSplineN thisNode = null;
    private List<GSDRoadUtil.WizardObject> oList = null;
    [UnityEngine.Serialization.FormerlySerializedAs("bNoGUI")]
    private bool useNoGUI = false;


    private void OnGUI()
    {
        DoGUI();
    }


    private void DoGUI()
    {
        if (useNoGUI)
        {
            return;
        }
        if (oList == null)
        {
            Close();
            return;
        }

        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal();

        if (thisNode.bIsBridgeStart)
        {
            xWindowType = (WindowTypeEnum) EditorGUILayout.Popup("Category: ", (int) tWindowType, WindowTypeDescBridge, GUILayout.Width(312f));
        }
        else
        {

            if (xWindowType == WindowTypeEnum.Edge)
            {
                SxWindowType = WindowTypeEnumShort.Edge;
            }
            else if (xWindowType == WindowTypeEnum.Extrusion)
            {
                SxWindowType = WindowTypeEnumShort.Extrusion;
            }
            else
            {
                SxWindowType = WindowTypeEnumShort.Groups;
            }

            SxWindowType = (WindowTypeEnumShort) EditorGUILayout.Popup("Category: ", (int) StWindowType, WindowTypeDesc, GUILayout.Width(312f));

            if (SxWindowType == WindowTypeEnumShort.Extrusion)
            {
                xWindowType = WindowTypeEnum.Extrusion;
            }
            else if (SxWindowType == WindowTypeEnumShort.Edge)
            {
                xWindowType = WindowTypeEnum.Edge;
            }
            else
            {
                xWindowType = WindowTypeEnum.Groups;
            }
            StWindowType = SxWindowType;
        }

        if (xWindowType != tWindowType)
        {
            Initialize(xWindowType, thisNode);
            EditorGUILayout.EndHorizontal();
            return;
        }



        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Single-click items to load", EditorStyles.boldLabel, GUILayout.Width(200f));



        EditorGUILayout.EndHorizontal();
        if (oList.Count == 0)
        {
            return;
        }
        int oCount = oList.Count;

        int WidthSpacing = 160;
        int HeightSpacing = 200;
        int HeightOffset = 30;
        int ScrollHeightOffset = 25;

        int xCount = 0;
        int yCount = 0;
        int yMod = Mathf.FloorToInt((float) position.width / 142f) - 1;

        int yMax = 0;
        if (yMod == 0)
        {
            yMax = 1;
        }
        else
        {
            yMax = Mathf.CeilToInt((float) oCount / (float) yMod);
        }

        bool bScrolling = false;
        if ((((yMax) * HeightSpacing) + 25) > position.height)
        {
            scrollPos = GUI.BeginScrollView(new Rect(0, 25, position.width - 10, position.height - 30), scrollPos, new Rect(0, 0, (yMod * WidthSpacing) + 25, (((yMax) * HeightSpacing) + 50)));
            bScrolling = true;
            HeightOffset = ScrollHeightOffset;
        }

        EditorGUILayout.BeginHorizontal();

        bool bClicked = false;
        for (int i = 0; i < oCount; i++)
        {
            if (i > 0)
            {
                if (yMod == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    yCount += 1;
                    xCount = 0;
                }
                else
                {
                    if (i % yMod == 0)
                    { EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal(); yCount += 1; xCount = 0; }
                }
            }

            if (xCount == 0)
            {
                bClicked = DoItem((xCount * WidthSpacing) + 5, (yCount * HeightSpacing) + HeightOffset, i);
            }
            else
            {
                bClicked = DoItem(xCount * WidthSpacing, (yCount * HeightSpacing) + HeightOffset, i);
            }

            if (bClicked)
            {
                if (tWindowType == WindowTypeEnum.Extrusion)
                {
                    GSD.Roads.Splination.SplinatedMeshMaker SMM = thisNode.AddSplinatedObject();
                    SMM.SetDefaultTimes(thisNode.bIsEndPoint, thisNode.tTime, thisNode.NextTime, thisNode.idOnSpline, thisNode.GSDSpline.distance);
                    SMM.LoadFromLibrary(oList[i].fileName, oList[i].isDefault);
                    SMM.isGSD = oList[i].isDefault;
                    SMM.Setup(true);
                }
                else if (tWindowType == WindowTypeEnum.Edge)
                {
                    GSD.Roads.EdgeObjects.EdgeObjectMaker EOM = thisNode.AddEdgeObject();
                    EOM.SetDefaultTimes(thisNode.bIsEndPoint, thisNode.tTime, thisNode.NextTime, thisNode.idOnSpline, thisNode.GSDSpline.distance);
                    EOM.LoadFromLibrary(oList[i].fileName, oList[i].isDefault);
                    EOM.isGSD = oList[i].isDefault;
                    EOM.Setup();
                }
                else if (tWindowType == WindowTypeEnum.Groups)
                {
                    thisNode.LoadWizardObjectsFromLibrary(oList[i].fileName, oList[i].isDefault, oList[i].isBridge);
                }
                else if (tWindowType == WindowTypeEnum.BridgeComplete)
                {
                    thisNode.LoadWizardObjectsFromLibrary(oList[i].fileName, oList[i].isDefault, oList[i].isBridge);
                }
                thisNode.bQuitGUI = true;
                oList.Clear();
                oList = null;
                EditorGUILayout.EndHorizontal();
                if (bScrolling)
                {
                    GUI.EndScrollView();
                }
                useNoGUI = true;
                Close();
                return;
            }
            xCount += 1;

        }
        EditorGUILayout.EndHorizontal();

        if (bScrolling)
        {
            GUI.EndScrollView();
        }
    }


    private bool DoItem(int x1, int y1, int i)
    {
        if (oList[i].thumb != null)
        {
            if (GUI.Button(new Rect(x1, y1, 132f, 132f), oList[i].thumb))
            {
                return true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(x1, y1, 132f, 132f), "No image"))
            {
                return true;
            }
        }

        GUI.Label(new Rect(x1, y1 + 132f, 148f, 20f), oList[i].displayName, EditorStyles.boldLabel);
        GUI.Label(new Rect(x1, y1 + 148f, 148f, 52f), oList[i].desc, EditorStyles.miniLabel);

        return false;
    }


    #region "Init"
    public Rect xRect;


    public void Initialize(WindowTypeEnum _windowType, GSDSplineN _node)
    {
        if (xRect.width < 1f && xRect.height < 1f)
        {
            xRect.x = 275f;
            xRect.y = 200f;
            xRect.width = 860f;
            xRect.height = 500f;
        }

        position = xRect;
        tWindowType = _windowType;
        thisNode = _node;
        InitWindow();
        Show();
    }


    private void InitWindow()
    {
        if (oList != null)
        {
            oList.Clear();
            oList = null;
        }
        oList = new List<GSDRoadUtil.WizardObject>();
        if (tWindowType == WindowTypeEnum.Extrusion)
        {
            titleContent.text = "Extrusion";
            InitObjs();
        }
        else if (tWindowType == WindowTypeEnum.Edge)
        {
            titleContent.text = "Edge objects";
            InitObjs();
        }
        else if (tWindowType == WindowTypeEnum.BridgeComplete)
        {
            titleContent.text = "Bridges";
            InitGroups(true);
        }
        else if (tWindowType == WindowTypeEnum.Groups)
        {
            titleContent.text = "Groups";
            InitGroups(false);
        }

        ThumbStyle = new GUIStyle(GUI.skin.button);
        ThumbStyle.contentOffset = new Vector2(0f, 0f);
        ThumbStyle.border = new RectOffset(0, 0, 0, 0);
        ThumbStyle.fixedHeight = 128f;
        ThumbStyle.fixedWidth = 128f;
        ThumbStyle.padding = new RectOffset(0, 0, 0, 0);
        ThumbStyle.normal.background = null;
        ThumbStyle.hover.background = null;
        ThumbStyle.active.background = null;

        EditorStyles.label.wordWrap = true;
        EditorStyles.miniLabel.wordWrap = true;
        GUI.skin.label.wordWrap = true;
    }


    #region "Init complete bridges"
    private void InitGroups(bool bIsBridge)
    {
        string[] tNames = null;
        string[] tPaths = null;
        //Load user custom ones first:
        GetGroupListing(out tNames, out tPaths, thisNode.GSDSpline.tRoad.laneAmount, false);
        LoadGroupObjs(ref tNames, ref tPaths, bIsBridge);
        //Load GSD ones last:
        GetGroupListing(out tNames, out tPaths, thisNode.GSDSpline.tRoad.laneAmount, true);
        LoadGroupObjs(ref tNames, ref tPaths, bIsBridge);
    }


    private void LoadGroupObjs(ref string[] tNames, ref string[] tPaths, bool bIsBridge)
    {
        int tCount = tNames.Length;
        string tPath = "";
        //		string tStringPath = "";
        //		string tDesc = "";
        //		string tDisplayName = "";
        //		string ThumbString = "";
        for (int index = 0; index < tCount; index++)
        {
            GSDRoadUtil.WizardObject tO = GSDRoadUtil.WizardObject.LoadFromLibrary(tPaths[index]);
            if (tO == null)
            {
                continue;
            }
            if (tO.isBridge != bIsBridge)
            {
                continue;
            }
            try
            {
                tO.thumb = (Texture2D) AssetDatabase.LoadAssetAtPath(tO.thumbString, typeof(Texture2D)) as Texture2D;
            }
            catch
            {
                tO.thumb = null;
            }
            tO.fileName = tNames[index];
            tO.FullPath = tPath;

            if (tO.isDefault && bIsBridge)
            {
                if (tO.displayName.Contains("SuspL") || tO.displayName.Contains("Large Suspension"))
                {
                    tO.displayName = "Large Suspension";
                    tO.desc = "Designed after the Golden Gate bridge. For lengths over 850m. Best results over 1300m.";
                    tO.sortID = 11;
                }
                else if (tO.displayName.Contains("SuspS") || tO.displayName.Contains("Small Suspension"))
                {
                    tO.displayName = "Small Suspension";
                    tO.desc = "Similar style as the large with smaller pillars. For lengths under 725m.";
                    tO.sortID = 10;
                }
                else if (tO.displayName.Contains("SemiArch1"))
                {
                    tO.displayName = "SemiArch 80 Degree";
                    tO.desc = "80 Degree arch. For lengths under 300m and small heights.";
                    tO.sortID = 40;
                }
                else if (tO.displayName.Contains("SemiArch2"))
                {
                    tO.displayName = "SemiArch 80 Girder";
                    tO.desc = "80 Degree arch with girder style. For lengths under 300m and small heights.";
                    tO.sortID = 41;
                }
                else if (tO.displayName.Contains("SemiArch3"))
                {
                    tO.displayName = "SemiArch 180 Degree";
                    tO.desc = "180 Degree arch. For lengths under 300m and small heights.";
                    tO.sortID = 42;
                }
                else if (tO.displayName.Contains("Arch12m"))
                {
                    tO.displayName = "Arch 12m Beams";
                    tO.desc = "Full deck arch bridge with 12m beams. Good for any length.";
                    tO.sortID = 0;
                }
                else if (tO.displayName.Contains("Arch24m"))
                {
                    tO.displayName = "Arch 24m Beams";
                    tO.desc = "Full deck arch bridge with 24m beams. Good for any length and non-small width roads.";
                    tO.sortID = 1;
                }
                else if (tO.displayName.Contains("Arch48m"))
                {
                    tO.displayName = "Arch 48m Beams";
                    tO.desc = "Full deck arch bridge with 48m beams. Good for any length and large width roads.";
                    tO.sortID = 3;
                }
                else if (tO.displayName.Contains("Grid"))
                {
                    tO.displayName = "Grid Steel";
                    tO.desc = "Grid based steel bridge. Good for any length depending on triangle counts.";
                    tO.sortID = 30;
                }
                else if (tO.displayName.Contains("Steel"))
                {
                    tO.displayName = "Steel Beam";
                    tO.desc = "Standard steel beam bridge. Good for any length depending on triangle counts.";
                    tO.sortID = 4;
                }
                else if (tO.displayName.Contains("Causeway1"))
                {
                    tO.displayName = "Causeway Federal";
                    tO.desc = "Standard federal highway bridge style. Good for any length depending on triangle counts.";
                    tO.sortID = 5;
                }
                else if (tO.displayName.Contains("Causeway2"))
                {
                    tO.displayName = "Causeway Overpass";
                    tO.desc = "Overpass style. Good for any length depending on triangle counts.";
                    tO.sortID = 8;
                }
                else if (tO.displayName.Contains("Causeway3"))
                {
                    tO.displayName = "Causeway Classic";
                    tO.desc = "Classic causeway style. Good for any length depending on triangle counts.";
                    tO.sortID = 7;
                }
                else if (tO.displayName.Contains("Causeway4"))
                {
                    tO.displayName = "Causeway Highway";
                    tO.desc = "State highway style. Good for any length depending on triangle counts.";
                    tO.sortID = 6;
                }
            }

            if (tO.isDefault && !bIsBridge)
            {
                if (tO.displayName.Contains("GSDTunnel"))
                {
                    tO.displayName = "Tunnel";
                    tO.desc = "Designed after the Eisenhower tunnel.";
                }
                else if (tO.displayName.Contains("GSDGroup-WBeamLeftTurn"))
                {
                    tO.displayName = "Left turn wbeams";
                    tO.desc = "Contains wbeam and signs on right side of road for left turn.";
                }
                else if (tO.displayName.Contains("GSDGroup-KRailLights"))
                {
                    tO.displayName = "K-rail with lights";
                    tO.desc = "Center divider k-rail with double sided lights. Best used on mostly straight highway type roads.";
                }
                else if (tO.displayName.Contains("GSDGroup-Rumblestrips"))
                {
                    tO.displayName = "Rumblestrips x2";
                    tO.desc = "Rumble strips on both sides of the road.";
                }
                else if (tO.displayName.Contains("GSDGroup-Fancy1"))
                {
                    tO.displayName = "Fancy railing x2";
                    tO.desc = "Luxurious railing with lighting on both sides of the road.";
                }
            }

            oList.Add(tO);
        }
        oListSort();
    }


    public static void GetGroupListing(out string[] tNames, out string[] tPaths, int Lanes, bool bIsDefault = false)
    {

        xPath = GSDRootUtil.GetDirLibrary();
        Debug.Log(xPath);

        string LaneText = "-2L";
        if (Lanes == 4)
        {
            LaneText = "-4L";
        }
        else if (Lanes == 6)
        {
            LaneText = "-6L";
        }

        tNames = null;
        tPaths = null;
        DirectoryInfo info;
        if (bIsDefault)
        {
            info = new DirectoryInfo(xPath + "B/W/");
        }
        else
        {
            info = new DirectoryInfo(xPath + "B/");
        }

        FileInfo[] fileInfo = info.GetFiles();
        int tCount = 0;
        foreach (FileInfo tInfo in fileInfo)
        {
            if (tInfo.Extension.ToLower().Contains("gsd"))
            {
                if (!bIsDefault)
                {
                    tCount += 1;
                }
                else
                {
                    if (tInfo.Name.Contains(LaneText))
                    {
                        tCount += 1;
                    }
                }
            }
        }

        tNames = new string[tCount];
        tPaths = new string[tCount];
        tCount = 0;
        foreach (FileInfo tInfo in fileInfo)
        {
            if (tInfo.Extension.ToLower().Contains("gsd"))
            {
                if (!bIsDefault)
                {
                    tNames[tCount] = tInfo.Name.Replace(".gsd", "");
                    tPaths[tCount] = tInfo.FullName;
                    tCount += 1;
                }
                else
                {
                    if (tInfo.Name.Contains(LaneText))
                    {
                        tNames[tCount] = tInfo.Name.Replace(".gsd", "");
                        tPaths[tCount] = tInfo.FullName;
                        tCount += 1;
                    }
                }
            }
        }
    }
    #endregion


    #region "Init objs"
    private void InitObjs()
    {
        string[] tNames = null;
        string[] tPaths = null;
        //Load user custom ones first:
        if (tWindowType == WindowTypeEnum.Extrusion)
        {
            SplinatedMeshMaker.GetLibraryFiles(out tNames, out tPaths, false);
        }
        else
        {
            EdgeObjectMaker.GetLibraryFiles(out tNames, out tPaths, false);
        }
        LoadObjs(ref tNames, ref tPaths, false);
        //Load GSD ones last:
        if (tWindowType == WindowTypeEnum.Extrusion)
        {
            SplinatedMeshMaker.GetLibraryFiles(out tNames, out tPaths, true);
        }
        else
        {
            EdgeObjectMaker.GetLibraryFiles(out tNames, out tPaths, true);
        }
        LoadObjs(ref tNames, ref tPaths, true);
    }


    private void LoadObjs(ref string[] tNames, ref string[] tPaths, bool bIsDefault = false)
    {
        int tCount = tNames.Length;
        string tPath = "";
        string tStringPath = "";
        string tDesc = "";
        string tDisplayName = "";
        string ThumbString = "";
        bool bIsBridge = false;
        for (int i = 0; i < tCount; i++)
        {
            bIsBridge = false;
            tPath = tPaths[i];

            if (tWindowType == WindowTypeEnum.Extrusion)
            {
                SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = (SplinatedMeshMaker.SplinatedMeshLibraryMaker) GSDRootUtil.LoadXML<SplinatedMeshMaker.SplinatedMeshLibraryMaker>(ref tPath);
                if (SLM == null)
                {
                    continue;
                }
                tStringPath = SLM.CurrentSplinationString;
                tDesc = SLM.desc;
                tDisplayName = SLM.displayName;
                ThumbString = SLM.thumbString;
                bIsBridge = SLM.isBridge;
            }
            else if (tWindowType == WindowTypeEnum.Edge)
            {
                EdgeObjectMaker.EdgeObjectLibraryMaker ELM = (EdgeObjectMaker.EdgeObjectLibraryMaker) GSDRootUtil.LoadXML<EdgeObjectMaker.EdgeObjectLibraryMaker>(ref tPath);
                if (ELM == null)
                {
                    continue;
                }
                tStringPath = ELM.edgeObjectString;
                tDesc = ELM.desc;
                tDisplayName = ELM.displayName;
                ThumbString = ELM.thumbString;
                bIsBridge = ELM.isBridge;
            }

            //Don't continue if bridge pieces and this is not a bridge piece:
            if (tWindowType == WindowTypeEnum.Extrusion && bIsBridge)
            {
                continue;
            }

            GSDRoadUtil.WizardObject tO = new GSDRoadUtil.WizardObject();
            try
            {
                tO.thumb = (Texture2D) AssetDatabase.LoadAssetAtPath(ThumbString, typeof(Texture2D)) as Texture2D;
            }
            catch
            {
                tO.thumb = null;
            }
            if (tO.thumb == null)
            {
                try
                {
                    GameObject xObj = (GameObject) UnityEditor.AssetDatabase.LoadAssetAtPath(tStringPath, typeof(GameObject)) as GameObject;
                    tO.thumb = AssetPreview.GetAssetPreview(xObj);
                }
                catch
                {
                    tO.thumb = null;
                }
            }
            tO.displayName = tDisplayName;
            tO.fileName = tNames[i];
            tO.FullPath = tPath;
            tO.desc = tDesc;
            tO.isDefault = bIsDefault;

            if (bIsDefault && tWindowType == WindowTypeEnum.Edge)
            {
                if (tO.displayName.Contains("GSDAtten"))
                {
                    tO.displayName = "Attenuator";
                    tO.desc = "Standard double WBeam with impact absorption.";
                }
                else if (tO.displayName.Contains("GSDGreenBlinder"))
                {
                    tO.displayName = "KRail Blinder";
                    tO.desc = "Best results when placed on KRail for KRail blinders.";
                    tO.sortID = 5;
                }
                else if (tO.displayName.Contains("GSDRoadBarrelStatic"))
                {
                    tO.displayName = "Sand Barrel Static";
                    tO.desc = "One static sand barrel. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadBarrelRigid"))
                {
                    tO.displayName = "Sand Barrel Rigid";
                    tO.desc = "One rigid sand barrel. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadBarrel3Static"))
                {
                    tO.displayName = "Sand Barrels Static 3";
                    tO.desc = "Three static sand barrels in a line. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadBarrel3Rigid"))
                {
                    tO.displayName = "Sand Barrels Rigid 3";
                    tO.desc = "Three rigid sand barrels in a line. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadBarrel7Static"))
                {
                    tO.displayName = "Sand Barrels Static 7";
                    tO.desc = "Seven static sand barrels in standard formation. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadBarrel7Rigid"))
                {
                    tO.displayName = "Sand Barrel Rigid 7";
                    tO.desc = "Seven rigid sand barrels in standard formation. Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDRoadConBarrelStatic"))
                {
                    tO.displayName = "Con Barrels Static";
                    tO.desc = "Static road construction barrels.";
                    tO.sortID = 3;
                }
                else if (tO.displayName.Contains("GSDRoadConBarrelRigid"))
                {
                    tO.displayName = "Con Barrels Rigid";
                    tO.desc = "Rigid road construction barrels.";
                    tO.sortID = 3;
                }
                else if (tO.displayName.Contains("GSDTrafficConeStatic"))
                {
                    tO.displayName = "Traffic cones Static";
                    tO.desc = "Static traffic cones.";
                    tO.sortID = 4;
                }
                else if (tO.displayName.Contains("GSDTrafficConeRigid"))
                {
                    tO.displayName = "Traffic cones Rigid";
                    tO.desc = "Rigid traffic cones.";
                    tO.sortID = 4;
                }
                else if (tO.displayName.Contains("GSDRoadReflector"))
                {
                    tO.displayName = "Road reflectors";
                    tO.desc = "Placed one center line of road for center line reflection.";
                    tO.sortID = 4;
                }
                else if (tO.displayName.Contains("GSDStopSign"))
                {
                    tO.displayName = "Stop sign";
                    tO.desc = "Standard specification non-interstate stop sign.";
                }
                else if (tO.displayName.Contains("GSDStreetLightSingle"))
                {
                    tO.displayName = "Streetlight Singlesided";
                    tO.desc = "Best used on side of roads.";
                }
                else if (tO.displayName.Contains("GSDStreetLightDouble"))
                {
                    tO.displayName = "Streetlight Doublesided";
                    tO.desc = "Best results when embedded in KRail in centerline of road.";
                }
                else if (tO.displayName.Contains("GSDWarningSign1"))
                {
                    tO.displayName = "Warning Sign #1";
                    tO.desc = "Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDWarningSign2"))
                {
                    tO.displayName = "Warning Sign #2";
                    tO.desc = "Best results when placed in front of railings or bridges.";
                }
                else if (tO.displayName.Contains("GSDSignRightTurnOnly"))
                {
                    tO.displayName = "Right turn only";
                    tO.desc = "Best results when placed near intersection right turn lane.";
                    tO.sortID = 4;
                }

                else if (tO.displayName.Contains("GSDSign330"))
                {
                    tO.displayName = "Signs 330";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-330\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign396"))
                {
                    tO.displayName = "Signs 396";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-396\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign617-Small"))
                {
                    tO.displayName = "Signs 617 small";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-617\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign617"))
                {
                    tO.displayName = "Signs 617";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-617\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign861-Small"))
                {
                    tO.displayName = "Signs 861 small";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-861\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign861"))
                {
                    tO.displayName = "Sign type 861";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-861\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign988-Small"))
                {
                    tO.displayName = "Signs 988 small";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-988\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSign988"))
                {
                    tO.displayName = "Signs 988";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-988\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSignDiamond"))
                {
                    tO.displayName = "Signs diamond";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-diamond\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSignSquare-Small"))
                {
                    tO.displayName = "Signs square small";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-Square\" as the search term.";
                    tO.sortID = 21;
                }
                else if (tO.displayName.Contains("GSDSignSquare"))
                {
                    tO.displayName = "Signs square";
                    tO.desc = "Interchangeable materials, use \"GSDFedSign-Square\" as the search term.";
                    tO.sortID = 21;
                }
            }

            if (bIsDefault && tWindowType == WindowTypeEnum.Extrusion)
            {
                if (tO.displayName.Contains("GSDKRail"))
                {
                    tO.displayName = "KRail";
                    tO.desc = "Federal spec cement KRailing (also known as Jersey Barriers). Variant with down ends.";
                }
                else if (tO.displayName.Contains("GSDKRailCurvedR"))
                {
                    tO.displayName = "KRail Curved Right";
                    tO.desc = "Federal spec cement KRailing (also known as Jersey Barriers). Variant with curved ends for right shoulder.";
                }
                else if (tO.displayName.Contains("GSDKRailCurvedL"))
                {
                    tO.displayName = "KRail Curved Left";
                    tO.desc = "Federal spec cement KRailing (also known as Jersey Barriers). Variant with curved ends for left shoulder.";
                }
                else if (tO.displayName.Contains("GSDWBeam1R"))
                {
                    tO.displayName = "WBeam Wood Right";
                    tO.desc = "Federal spec wooden pole WBeam railing. Best used as outer shoulder railing. Right shoulder.";
                }
                else if (tO.displayName.Contains("GSDWBeam1L"))
                {
                    tO.displayName = "WBeam Wood Left";
                    tO.desc = "Federal spec wooden pole WBeam railing. Best used as outer shoulder railing. Left shoulder.";
                }
                else if (tO.displayName.Contains("GSDWBeam2R"))
                {
                    tO.displayName = "WBeam Metal Right";
                    tO.desc = "Federal spec metal pole WBeam railing. Best used as outer shoulder railing. Right shoulder.";
                }
                else if (tO.displayName.Contains("GSDWBeam2L"))
                {
                    tO.displayName = "WBeam Metal Left";
                    tO.desc = "Federal spec metal pole WBeam railing. Best used as outer shoulder railing. Left shoulder.";
                }
                else if (tO.displayName.Contains("GSDRailing1"))
                {
                    tO.displayName = "Railing #1";
                    tO.desc = "Standard double square pole railing.";
                }
                else if (tO.displayName.Contains("GSDRailing2"))
                {
                    tO.displayName = "Railing #2";
                    tO.desc = "Standard concrete big block railing.";
                }
                else if (tO.displayName.Contains("GSDRailing3"))
                {
                    tO.displayName = "Railing #3";
                    tO.desc = "Standard four-strand metal railing.";
                }
                else if (tO.displayName.Contains("GSDRailing5"))
                {
                    tO.displayName = "Railing #5";
                    tO.desc = "Basic concrete railing with pylons.";
                }
                else if (tO.displayName.Contains("GSDRailing6"))
                {
                    tO.displayName = "Railing #6";
                    tO.desc = "Standard two-strand metal pole railing.";
                }
                else if (tO.displayName.Contains("GSDRailing7"))
                {
                    tO.displayName = "Railing #7";
                    tO.desc = "Rock-decorated concrete railing with pylons and double strand rusted look metal railing.";
                }
                else if (tO.displayName.Contains("GSDRailing8"))
                {
                    tO.displayName = "Railing #8";
                    tO.desc = "Rock-decorated concrete railing with standard single pole metal railing.";
                }
                else if (tO.displayName.Contains("GSDRailing9"))
                {
                    tO.displayName = "Railing #9";
                    tO.desc = "Very low poly railing used for mobile.";
                }
                else if (tO.displayName.Contains("GSDSidewalk"))
                {
                    tO.displayName = "Sidewalk";
                    tO.desc = "Sidewalk.";
                }
                else if (tO.displayName.Contains("GSDRumbleStrip"))
                {
                    tO.displayName = "Rumblestrip";
                    tO.desc = "State spec rumblestrip. For best results place several cm from road edge into shoulder.";
                }
                else if (tO.displayName.Contains("GSDRailing4R"))
                {
                    tO.displayName = "Railing #4 Right";
                    tO.desc = "Three bar angled pole railing. Right side of road.";
                }
                else if (tO.displayName.Contains("GSDRailing4L"))
                {
                    tO.displayName = "Railing #4 Left";
                    tO.desc = "Three bar angled pole railing. Left side of road.";
                }
                else if (tO.displayName.Contains("GSDRailing4-LightR"))
                {
                    tO.displayName = "Railing #4 Light Right";
                    tO.desc = "Three bar angled pole railing. Right side of road. Light version with fewer triangle count.";
                }
                else if (tO.displayName.Contains("GSDRailing4-LightL"))
                {
                    tO.displayName = "Railing #4 Light Left";
                    tO.desc = "Three bar angled pole railing. Left side of road. Light version with fewer triangle count.";
                }
                else if (tO.displayName.Contains("GSDRailingBase1"))
                {
                    tO.displayName = "Railing base #1";
                    tO.desc = "Use as a base on other railings to create more detail.";
                }
                else if (tO.displayName.Contains("GSDRailingBase2"))
                {
                    tO.displayName = "Railing base #2";
                    tO.desc = "Use as a base on other railings to create more detail.";
                }
                else if (tO.displayName.Contains("GSDCableBarrier-Light"))
                {
                    tO.displayName = "Cable barrier 10m";
                    tO.desc = "Cable barrier 10m light triangle version. Best used as center divider or as railing barriers.";
                    tO.sortID = 20;
                }
                else if (tO.displayName.Contains("GSDCableBarrier"))
                {
                    tO.displayName = "Cable barrier 5m";
                    tO.desc = "Cable barrier 5m. Best used as center divider or as railing barriers.";
                    tO.sortID = 20;
                }
            }

            oList.Add(tO);
        }
        oListSort();
    }


    private void oListSort()
    {
        oList.Sort((GSDRoadUtil.WizardObject t1, GSDRoadUtil.WizardObject t2) =>
        {
            if (t1.isDefault != t2.isDefault)
            {
                return t1.isDefault.CompareTo(t2.isDefault);
            }
            else if (t1.sortID != t2.sortID)
            {
                return t1.sortID.CompareTo(t2.sortID);
            }
            else
            {
                return t1.displayName.CompareTo(t2.displayName);
            }
        });
    }
    #endregion
    #endregion
}
#endif