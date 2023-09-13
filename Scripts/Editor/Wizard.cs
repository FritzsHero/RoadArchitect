#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
#endregion


namespace RoadArchitect
{
    public class Wizard : EditorWindow
    {
        public enum WindowTypeEnum
        {
            Extrusion,
            Edge,
            Groups,
            BridgeComplete,
        };


        private readonly string[] WindowTypeDescBridge = new string[]{
            "Extrusion items",
            "Edge objects",
            "Other groups",
            "Complete bridges",
        };


        private readonly string[] WindowTypeDesc = new string[]{
            "Extrusion items",
            "Edge objects",
            "Other groups"
        };


        #region "Vars"
        private WindowTypeEnum windowType = WindowTypeEnum.Extrusion;
        private WindowTypeEnum windowTypePrevious = WindowTypeEnum.Extrusion;
        private static string path = "";

        private GUIStyle thumbStyle;
        private Vector2 scrollPos = new Vector2(0f, 25f);
        private SplineN thisNode = null;
        private List<WizardObject> objectList = null;
        private bool isUsingNoGUI = false;
        public Rect rect;
        #endregion


        private void OnGUI()
        {
            DoGUI();
        }


        private void DoGUI()
        {
            if (isUsingNoGUI)
            {
                return;
            }
            if (objectList == null)
            {
                Close();
                return;
            }

            GUILayout.Space(4f);
            EditorGUILayout.BeginHorizontal();

            string[] options = thisNode.isBridgeStart ? WindowTypeDescBridge : WindowTypeDesc;
            if (!thisNode.isBridgeStart && windowType == WindowTypeEnum.BridgeComplete)
            {
                // Prevent category error when changing from bridge to normal node
                windowType = WindowTypeEnum.Extrusion;
            }
            windowType = (WindowTypeEnum)EditorGUILayout.Popup("Category: ", (int)windowType, options, GUILayout.Width(312f));

            if (windowType != windowTypePrevious)
            {
                windowTypePrevious = windowType;
                InitWindow();
            }

            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Single-click items to load", EditorStyles.boldLabel, GUILayout.Width(200f));

            EditorGUILayout.EndHorizontal();
            if (objectList.Count == 0)
            {
                return;
            }
            int objectCount = objectList.Count;

            int spacingWidth = 160;
            int spacingHeight = 200;
            int heightOffset = 30;
            int scrollHeightOffset = 25;

            int xCount = 0;
            int yCount = 0;
            int yMod = Mathf.FloorToInt((float)position.width / 142f) - 1;

            int yMax = 0;
            if (yMod == 0)
            {
                yMax = 1;
            }
            else
            {
                yMax = Mathf.CeilToInt((float)objectCount / (float)yMod);
            }

            bool isScrolling = false;
            if ((((yMax) * spacingHeight) + 25) > position.height)
            {
                scrollPos = GUI.BeginScrollView(new Rect(0, 25, position.width - 10, position.height - 30), scrollPos, new Rect(0, 0, (yMod * spacingWidth) + 25, (((yMax) * spacingHeight) + 50)));
                isScrolling = true;
                heightOffset = scrollHeightOffset;
            }

            EditorGUILayout.BeginHorizontal();

            bool isClicked = false;
            for (int i = 0; i < objectCount; i++)
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
                        {
                            EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal(); yCount += 1; xCount = 0;
                        }
                    }
                }

                if (xCount == 0)
                {
                    isClicked = DoItem((xCount * spacingWidth) + 5, (yCount * spacingHeight) + heightOffset, i);
                }
                else
                {
                    isClicked = DoItem(xCount * spacingWidth, (yCount * spacingHeight) + heightOffset, i);
                }

                if (isClicked)
                {
                    if (windowType == WindowTypeEnum.Extrusion)
                    {
                        Splination.SplinatedMeshMaker SMM = thisNode.AddSplinatedObject();
                        SMM.SetDefaultTimes(thisNode.isEndPoint, thisNode.time, thisNode.nextTime, thisNode.idOnSpline, thisNode.spline.distance);
                        SMM.LoadFromLibrary(objectList[i].fileName, objectList[i].isDefault);
                        thisNode.CheckRenameSplinatedObject(SMM);
                        SMM.isDefault = objectList[i].isDefault;
                        SMM.Setup(true);
                    }
                    else if (windowType == WindowTypeEnum.Edge)
                    {
                        EdgeObjects.EdgeObjectMaker EOM = thisNode.AddEdgeObject();
                        EOM.SetDefaultTimes(thisNode.isEndPoint, thisNode.time, thisNode.nextTime, thisNode.idOnSpline, thisNode.spline.distance);
                        EOM.LoadFromLibrary(objectList[i].fileName, objectList[i].isDefault);
                        thisNode.CheckRenameEdgeObject(EOM);
                        EOM.isDefault = objectList[i].isDefault;
                        EOM.Setup();
                    }
                    else if (windowType == WindowTypeEnum.Groups)
                    {
                        thisNode.LoadWizardObjectsFromLibrary(objectList[i].fileName, objectList[i].isDefault, objectList[i].isBridge);
                    }
                    else if (windowType == WindowTypeEnum.BridgeComplete)
                    {
                        thisNode.LoadWizardObjectsFromLibrary(objectList[i].fileName, objectList[i].isDefault, objectList[i].isBridge);
                    }
                    objectList.Clear();
                    objectList = null;
                    EditorGUILayout.EndHorizontal();
                    if (isScrolling)
                    {
                        GUI.EndScrollView();
                    }
                    isUsingNoGUI = true;
                    Close();
                    return;
                }
                xCount += 1;

            }
            EditorGUILayout.EndHorizontal();

            if (isScrolling)
            {
                GUI.EndScrollView();
            }
        }


        private bool DoItem(int _x1, int _y1, int _i)
        {
            if (objectList[_i].thumb != null)
            {
                if (GUI.Button(new Rect(_x1, _y1, 132f, 132f), objectList[_i].thumb))
                {
                    return true;
                }
            }
            else
            {
                if (GUI.Button(new Rect(_x1, _y1, 132f, 132f), "No image"))
                {
                    return true;
                }
            }

            GUI.Label(new Rect(_x1, _y1 + 132f, 148f, 20f), objectList[_i].displayName, EditorStyles.boldLabel);
            GUI.Label(new Rect(_x1, _y1 + 148f, 148f, 52f), objectList[_i].desc, EditorStyles.miniLabel);

            return false;
        }


        #region "Init"
        /// <summary> Initializes the wizard </summary>
        public void Initialize(WindowTypeEnum _windowType, SplineN _node)
        {
            if (rect.width < 1f && rect.height < 1f)
            {
                rect.x = 275f;
                rect.y = 200f;
                rect.width = 860f;
                rect.height = 500f;
            }

            position = rect;
            windowType = _windowType;
            windowTypePrevious = _windowType;
            thisNode = _node;
            InitWindow();
            Show();
        }


        private void InitWindow()
        {
            if (objectList != null)
            {
                objectList.Clear();
                objectList = null;
            }
            objectList = new List<WizardObject>();
            if (windowType == WindowTypeEnum.Extrusion)
            {
                titleContent.text = "Extrusion";
                InitObjs();
            }
            else if (windowType == WindowTypeEnum.Edge)
            {
                titleContent.text = "Edge objects";
                InitObjs();
            }
            else if (windowType == WindowTypeEnum.BridgeComplete)
            {
                titleContent.text = "Bridges";
                InitGroups(true);
            }
            else if (windowType == WindowTypeEnum.Groups)
            {
                titleContent.text = "Groups";
                InitGroups(false);
            }

            thumbStyle = new GUIStyle(GUI.skin.button);
            thumbStyle.contentOffset = new Vector2(0f, 0f);
            thumbStyle.border = new RectOffset(0, 0, 0, 0);
            thumbStyle.fixedHeight = 128f;
            thumbStyle.fixedWidth = 128f;
            thumbStyle.padding = new RectOffset(0, 0, 0, 0);
            thumbStyle.normal.background = null;
            thumbStyle.hover.background = null;
            thumbStyle.active.background = null;

            EditorStyles.label.wordWrap = true;
            EditorStyles.miniLabel.wordWrap = true;
            GUI.skin.label.wordWrap = true;
        }


        #region "Init complete bridges"
        private void InitGroups(bool _isBridge)
        {
            string[] names = null;
            string[] paths = null;
            //Load user custom ones first:
            GetGroupListing(out names, out paths, thisNode.spline.road.laneAmount, false);
            LoadGroupObjs(ref names, ref paths, _isBridge);
            //Load RoadArchitect ones last:
            GetGroupListing(out names, out paths, thisNode.spline.road.laneAmount, true);
            LoadGroupObjs(ref names, ref paths, _isBridge);
        }


        private void LoadGroupObjs(ref string[] _names, ref string[] _paths, bool _isBridge)
        {
            int nameCount = _names.Length;
            string path = "";
            //string thumbString = "";
            for (int index = 0; index < nameCount; index++)
            {
                WizardObject tO = WizardObject.LoadFromLibrary(_paths[index]);
                if (tO == null)
                {
                    continue;
                }
                if (tO.isBridge != _isBridge)
                {
                    continue;
                }
                try
                {
                    tO.thumb = EngineIntegration.LoadAssetFromPath<Texture2D>(tO.thumbString);
                }
                catch
                {
                    tO.thumb = null;
                }
                tO.fileName = _names[index];
                tO.FullPath = path;

                objectList.Add(tO);
            }
            oListSort();
        }


        public static void GetGroupListing(out string[] _names, out string[] _paths, int _lanes, bool _isDefault = false)
        {
            path = RootUtils.GetDirLibrary();
            Debug.Log(path);

            string laneText = "-2L";
            if (_lanes == 4)
            {
                laneText = "-4L";
            }
            else if (_lanes == 6)
            {
                laneText = "-6L";
            }

            _names = null;
            _paths = null;
            DirectoryInfo info;
            if (_isDefault)
            {
                // W folder is now the Default folder
                info = new DirectoryInfo(Path.Combine(Path.Combine(path, "Groups"), "Default"));
            }
            else
            {
                info = new DirectoryInfo(Path.Combine(path, "Groups"));
            }

            FileInfo[] fileInfo = info.GetFiles();
            int count = 0;
            foreach (FileInfo tInfo in fileInfo)
            {
                if (tInfo.Extension.ToLower().Contains("rao"))
                {
                    if (!_isDefault)
                    {
                        count += 1;
                    }
                    else
                    {
                        if (tInfo.Name.Contains(laneText))
                        {
                            count += 1;
                        }
                    }
                }
            }

            _names = new string[count];
            _paths = new string[count];
            count = 0;
            foreach (FileInfo tInfo in fileInfo)
            {
                if (tInfo.Extension.ToLower().Contains("rao"))
                {
                    if (!_isDefault)
                    {
                        _names[count] = tInfo.Name.Replace(".rao", "");
                        _paths[count] = tInfo.FullName;
                        count += 1;
                    }
                    else
                    {
                        if (tInfo.Name.Contains(laneText))
                        {
                            _names[count] = tInfo.Name.Replace(".rao", "");
                            _paths[count] = tInfo.FullName;
                            count += 1;
                        }
                    }
                }
            }
        }
        #endregion


        #region "Init objs"
        private void InitObjs()
        {
            string[] names = null;
            string[] paths = null;

            //Load user custom ones first:
            if (windowType == WindowTypeEnum.Extrusion)
            {
                Splination.SplinatedMeshMaker.GetLibraryFiles(out names, out paths, false);
            }
            else
            {
                EdgeObjects.EdgeObjectMaker.GetLibraryFiles(out names, out paths, false);
            }
            LoadObjs(ref names, ref paths, false);


            //Load RoadArchitect ones last:
            if (windowType == WindowTypeEnum.Extrusion)
            {
                Splination.SplinatedMeshMaker.GetLibraryFiles(out names, out paths, true);
            }
            else
            {
                EdgeObjects.EdgeObjectMaker.GetLibraryFiles(out names, out paths, true);
            }
            LoadObjs(ref names, ref paths, true);
        }


        private void LoadObjs(ref string[] _names, ref string[] _paths, bool _isDefault = false)
        {
            int namesCount = _names.Length;
            string path = "";
            string stringPath = "";
            string desc = "";
            string displayName = "";
            string thumbString = "";
            bool isBridge;

            for (int i = 0; i < namesCount; i++)
            {
                isBridge = false;
                path = _paths[i];

                if (windowType == WindowTypeEnum.Extrusion)
                {
                    Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = RootUtils.LoadXML<Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker>(ref path);
                    if (SLM == null)
                    {
                        continue;
                    }
                    stringPath = SLM.CurrentSplinationString;
                    desc = SLM.desc;
                    displayName = SLM.displayName;
                    thumbString = SLM.thumbString;
                    isBridge = SLM.isBridge;
                }
                else if (windowType == WindowTypeEnum.Edge)
                {
                    EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker ELM = RootUtils.LoadXML<EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker>(ref path);
                    if (ELM == null)
                    {
                        continue;
                    }
                    stringPath = ELM.edgeObjectString;
                    desc = ELM.desc;
                    displayName = ELM.displayName;
                    thumbString = ELM.thumbString;
                    isBridge = ELM.isBridge;
                }

                //Don't continue if bridge pieces and this is not a bridge piece:
                if (windowType == WindowTypeEnum.Extrusion && isBridge)
                {
                    continue;
                }

                WizardObject tO = new WizardObject();
                #region "Image"
                try
                {
                    tO.thumb = EngineIntegration.LoadAssetFromPath<Texture2D>(thumbString);
                }
                catch
                {
                    tO.thumb = null;
                }


                if (tO.thumb == null)
                {
                    try
                    {
                        GameObject xObj = EngineIntegration.LoadAssetFromPath<GameObject>(stringPath);
                        tO.thumb = AssetPreview.GetAssetPreview(xObj);
                    }
                    catch
                    {
                        tO.thumb = null;
                    }
                }
                #endregion

                tO.displayName = displayName;
                tO.fileName = _names[i];
                tO.FullPath = path;
                tO.desc = desc;
                tO.isDefault = _isDefault;

                objectList.Add(tO);
            }
            oListSort();
        }


        private void oListSort()
        {
            objectList.Sort((WizardObject object1, WizardObject object2) =>
            {
                if (object1.isDefault != object2.isDefault)
                {
                    return object1.isDefault.CompareTo(object2.isDefault);
                }
                else if (object1.sortID != object2.sortID)
                {
                    return object1.sortID.CompareTo(object2.sortID);
                }
                else
                {
                    return object1.displayName.CompareTo(object2.displayName);
                }
            });
        }
        #endregion
        #endregion
    }
}
#endif
