#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using GSD;
#endregion


public class GSDSaveWindow : EditorWindow
{
    public enum WindowTypeEnum
    {
        Extrusion,
        Edge,
        BridgeWizard
    };


    #region "Vars"
    private WindowTypeEnum windowType = WindowTypeEnum.Extrusion;

    private Texture2D temp2D = null;
    private Texture2D temp2D2 = null;
    private string thumbString = "";
    private string desc = "";
    [UnityEngine.Serialization.FormerlySerializedAs("tFilename")]
    private string fileName = "DefaultName";
    [UnityEngine.Serialization.FormerlySerializedAs("tDisplayName")]
    private string displayName = "DefaultName";
    [UnityEngine.Serialization.FormerlySerializedAs("tDisplayName2")]
    private string displayName2 = "";
    [UnityEngine.Serialization.FormerlySerializedAs("TitleText")]
    private string titleText = "";
    //	private string tPath = "";
    [UnityEngine.Serialization.FormerlySerializedAs("bFileExists")]
    private bool isFileExisting = false;
    [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
    private bool isBridge = false;

    private GSD.Roads.Splination.SplinatedMeshMaker[] tSMMs = null;
    private GSD.Roads.EdgeObjects.EdgeObjectMaker[] tEOMs = null;
    private string path = "";
    private const int titleLabelHeight = 20;
    #endregion


    private void OnGUI()
    {
        GUILayout.Space(4f);
        EditorGUILayout.LabelField(titleText, EditorStyles.boldLabel);

        temp2D2 = (Texture2D) EditorGUILayout.ObjectField("Square thumb (optional):", temp2D, typeof(Texture2D), false);
        if (temp2D2 != temp2D)
        {
            temp2D = temp2D2;
            thumbString = AssetDatabase.GetAssetPath(temp2D);
        }

        if (path.Length < 5)
        {
            path = GSDRootUtil.GetDirLibrary();
        }

        EditorGUILayout.LabelField("Short description (optional):");
        desc = EditorGUILayout.TextArea(desc, GUILayout.Height(40f));
        displayName2 = EditorGUILayout.TextField("Display name:", displayName);
        if (string.Compare(displayName2, displayName) != 0)
        {
            displayName = displayName2;
            SanitizeFilename();

            if (windowType == WindowTypeEnum.Edge)
            {


                if (System.IO.File.Exists(path + "EOM" + fileName + ".gsd"))
                {
                    isFileExisting = true;
                }
                else
                {
                    isFileExisting = false;
                }
            }
            else if (windowType == WindowTypeEnum.Extrusion)
            {
                if (System.IO.File.Exists(path + "ESO" + fileName + ".gsd"))
                {
                    isFileExisting = true;
                }
                else
                {
                    isFileExisting = false;
                }
            }
            else
            {
                if (System.IO.File.Exists(path + "B/" + fileName + ".gsd"))
                {
                    isFileExisting = true;
                }
                else
                {
                    isFileExisting = false;
                }
            }
        }


        if (isFileExisting)
        {
            EditorGUILayout.LabelField("File exists already!", EditorStyles.miniLabel);
            if (windowType == WindowTypeEnum.Edge)
            {
                EditorGUILayout.LabelField(path + "EOM" + fileName + ".gsd", EditorStyles.miniLabel);
            }
            else if (windowType == WindowTypeEnum.Extrusion)
            {
                EditorGUILayout.LabelField(path + "ESO" + fileName + ".gsd", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField(path + "B/" + fileName + ".gsd", EditorStyles.miniLabel);
            }
        }
        else
        {
            if (windowType == WindowTypeEnum.Edge)
            {
                EditorGUILayout.LabelField(path + "EOM" + fileName + ".gsd", EditorStyles.miniLabel);
            }
            else if (windowType == WindowTypeEnum.Extrusion)
            {
                EditorGUILayout.LabelField(path + "ESO" + fileName + ".gsd", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField(path + "B/" + fileName + ".gsd", EditorStyles.miniLabel);
            }
        }

        GUILayout.Space(4f);

        isBridge = EditorGUILayout.Toggle("Is bridge related:", isBridge);
        GUILayout.Space(8f);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
        if (windowType == WindowTypeEnum.Extrusion)
        {
            DoExtrusion();
        }
        else if (windowType == WindowTypeEnum.Edge)
        {
            DoEdgeObject();
        }
        else if (windowType == WindowTypeEnum.BridgeWizard)
        {
            DoBridge();
        }

        EditorGUILayout.EndHorizontal();
    }


    private void DoExtrusion()
    {
        if (GUILayout.Button("Save extrusion"))
        {
            SanitizeFilename();
            tSMMs[0].isBridge = isBridge;
            tSMMs[0].ThumbString = thumbString;
            tSMMs[0].Desc = desc;
            tSMMs[0].DisplayName = displayName;
            tSMMs[0].SaveToLibrary(fileName, false);
            Close();
        }
    }


    private void DoEdgeObject()
    {
        if (GUILayout.Button("Save edge object"))
        {
            SanitizeFilename();
            tEOMs[0].isBridge = isBridge;
            tEOMs[0].thumbString = thumbString;
            tEOMs[0].desc = desc;
            tEOMs[0].displayName = displayName;
            tEOMs[0].SaveToLibrary(fileName, false);
            Close();
        }
    }


    private void DoBridge()
    {
        if (GUILayout.Button("Save group"))
        {
            SanitizeFilename();
            GSD.Roads.GSDRoadUtil.WizardObject WO = new GSD.Roads.GSDRoadUtil.WizardObject();
            WO.thumbString = thumbString;
            WO.desc = desc;
            WO.displayName = displayName;
            WO.fileName = fileName;
            WO.isBridge = isBridge;
            WO.isDefault = false;

            GSD.Roads.GSDRoadUtil.SaveNodeObjects(ref tSMMs, ref tEOMs, ref WO);
            Close();
        }
    }


    private void SanitizeFilename()
    {
        Regex regex = new Regex("[^a-zA-Z0-9 -]");
        fileName = regex.Replace(displayName, "");
        fileName = fileName.Replace(" ", "-");
        fileName = fileName.Replace("_", "-");
    }


    #region "Init"
    public void Initialize(ref Rect _rect, WindowTypeEnum _windowType, GSDSplineN _node, GSD.Roads.Splination.SplinatedMeshMaker _SMM = null, GSD.Roads.EdgeObjects.EdgeObjectMaker _EOM = null)
    {
        int rectHeight = 300;
        int rectWidth = 360;
        float Rx = ((float) _rect.width / 2f) - ((float) rectWidth / 2f) + _rect.x;
        float Ry = ((float) _rect.height / 2f) - ((float) rectHeight / 2f) + _rect.y;

        if (Rx < 0)
        {
            Rx = _rect.x;
        }
        if (Ry < 0)
        {
            Ry = _rect.y;
        }
        if (Rx > (_rect.width + _rect.x))
        {
            Rx = _rect.x;
        }
        if (Ry > (_rect.height + _rect.y))
        {
            Ry = _rect.y;
        }

        Rect rect = new Rect(Rx, Ry, rectWidth, rectHeight);

        if (rect.width < 300)
        {
            rect.width = 300;
            rect.x = _rect.x;
        }
        if (rect.height < 300)
        {
            rect.height = 300;
            rect.y = _rect.y;
        }

        position = rect;
        windowType = _windowType;
        Show();
        titleContent.text = "Save";
        if (windowType == WindowTypeEnum.Extrusion)
        {
            titleText = "Save extrusion";
            tSMMs = new GSD.Roads.Splination.SplinatedMeshMaker[1];
            tSMMs[0] = _SMM;
            if (_SMM != null)
            {
                fileName = _SMM.tName;
                displayName = fileName;
            }
        }
        else if (windowType == WindowTypeEnum.Edge)
        {
            titleText = "Save edge object";
            tEOMs = new GSD.Roads.EdgeObjects.EdgeObjectMaker[1];
            tEOMs[0] = _EOM;
            if (_EOM != null)
            {
                fileName = _EOM.objectName;
                displayName = fileName;
            }
        }
        else if (windowType == WindowTypeEnum.BridgeWizard)
        {
            isBridge = true;
            tSMMs = _node.SplinatedObjects.ToArray();
            tEOMs = _node.EdgeObjects.ToArray();
            titleText = "Save group";
            fileName = "Group" + Random.Range(0, 10000).ToString();
            displayName = fileName;
        }

        if (path.Length < 5)
        {
            path = GSDRootUtil.GetDirLibrary();
        }

        if (windowType == WindowTypeEnum.Edge)
        {
            if (System.IO.File.Exists(path + "EOM" + fileName + ".gsd"))
            {
                isFileExisting = true;
            }
            else
            {
                isFileExisting = false;
            }
        }
        else if (windowType == WindowTypeEnum.Extrusion)
        {
            if (System.IO.File.Exists(path + "ESO" + fileName + ".gsd"))
            {
                isFileExisting = true;
            }
            else
            {
                isFileExisting = false;
            }
        }
        else
        {
            if (System.IO.File.Exists(path + "B/" + fileName + ".gsd"))
            {
                isFileExisting = true;
            }
            else
            {
                isFileExisting = false;
            }
        }
    }
    #endregion
}
#endif