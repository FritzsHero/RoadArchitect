#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
#endregion


namespace RoadArchitect
{
    public class SaveWindow : EditorWindow
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
        private string fileName = "DefaultName";
        private string displayName = "DefaultName";
        private string displayName2 = "";
        private string titleText = "";
        //	private string tPath = "";
        private bool isFileExisting = false;
        private bool isBridge = false;

        private Splination.SplinatedMeshMaker[] tSMMs = null;
        private EdgeObjects.EdgeObjectMaker[] tEOMs = null;
        private string path = "";
        private const int titleLabelHeight = 20;
        #endregion


        private void OnGUI()
        {
            GUILayout.Space(4f);
            EditorGUILayout.LabelField(titleText, EditorStyles.boldLabel);

            temp2D2 = (Texture2D)EditorGUILayout.ObjectField("Square thumb (optional):", temp2D, typeof(Texture2D), false);
            if (temp2D2 != temp2D)
            {
                temp2D = temp2D2;
                thumbString = EngineIntegration.GetAssetPath(temp2D);
            }

            if (path.Length < 5)
            {
                path = RootUtils.GetDirLibrary();
            }

            EditorGUILayout.LabelField("Short description (optional):");
            desc = EditorGUILayout.TextArea(desc, GUILayout.Height(40f));
            displayName2 = EditorGUILayout.TextField("Display name:", displayName);
            if (string.Compare(displayName2, displayName) != 0)
            {
                displayName = displayName2;
                SanitizeFilename();

                CheckFileExistence();
            }


            if (isFileExisting)
            {
                EditorGUILayout.LabelField("File exists already!", EditorStyles.miniLabel);
            }

            if (windowType == WindowTypeEnum.Edge)
            {
                EditorGUILayout.LabelField(Path.Combine(path, "EOM" + fileName + ".rao"), EditorStyles.miniLabel);
            }
            else if (windowType == WindowTypeEnum.Extrusion)
            {
                EditorGUILayout.LabelField(Path.Combine(path, "ESO" + fileName + ".rao"), EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField(Path.Combine(Path.Combine(path, "Groups"), fileName + ".rao"), EditorStyles.miniLabel);
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
                tSMMs[0].thumbString = thumbString;
                tSMMs[0].desc = desc;
                tSMMs[0].displayName = displayName;
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
                WizardObject WO = new WizardObject();
                WO.thumbString = thumbString;
                WO.desc = desc;
                WO.displayName = displayName;
                WO.fileName = fileName;
                WO.isBridge = isBridge;
                WO.isDefault = false;

                RoadUtility.SaveNodeObjects(ref tSMMs, ref tEOMs, ref WO);
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


        private void CheckFileExistence()
        {
            if (windowType == WindowTypeEnum.Edge)
            {
                if (File.Exists(Path.Combine(path, "EOM" + fileName + ".rao")))
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
                if (File.Exists(Path.Combine(path, "ESO" + fileName + ".rao")))
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
                if (File.Exists(Path.Combine(Path.Combine(path, "B"), fileName + ".rao")))
                {
                    isFileExisting = true;
                }
                else
                {
                    isFileExisting = false;
                }
            }
        }


        #region "Init"
        public void Initialize(ref Rect _rect, WindowTypeEnum _windowType, SplineN _node, Splination.SplinatedMeshMaker _SMM = null, EdgeObjects.EdgeObjectMaker _EOM = null)
        {
            int rectHeight = 300;
            int rectWidth = 360;
            float Rx = ((float)_rect.width / 2f) - ((float)rectWidth / 2f) + _rect.x;
            float Ry = ((float)_rect.height / 2f) - ((float)rectHeight / 2f) + _rect.y;

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
                tSMMs = new Splination.SplinatedMeshMaker[1];
                tSMMs[0] = _SMM;
                if (_SMM != null)
                {
                    fileName = _SMM.objectName;
                    displayName = fileName;
                }
            }
            else if (windowType == WindowTypeEnum.Edge)
            {
                titleText = "Save edge object";
                tEOMs = new EdgeObjects.EdgeObjectMaker[1];
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
                path = RootUtils.GetDirLibrary();
            }

            CheckFileExistence();
        }
        #endregion
    }
}
#endif
