#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using RoadArchitect;
#endregion


namespace RoadArchitect.EdgeObjects
{
    [System.Serializable]
    public class EdgeObjectMaker
    {
        #region "Vars"
        [UnityEngine.Serialization.FormerlySerializedAs("bNeedsUpdate")]
        public bool isRequiringUpdate = false;
        public string UID = "";
        [UnityEngine.Serialization.FormerlySerializedAs("tNode")]
        public SplineN node = null;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsGSD")]
        public bool isDefault = false;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeObject")]
        public GameObject edgeObject = null;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectString")]
        public string edgeObjectString = "";
        [UnityEngine.Serialization.FormerlySerializedAs("bMaterialOverride")]
        public bool isMaterialOverriden = false;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial1")]
        public Material edgeMaterial1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial2")]
        public Material edgeMaterial2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial1String")]
        public string edgeMaterial1String = null;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial2String")]
        public string edgeMaterial2String = null;
        [UnityEngine.Serialization.FormerlySerializedAs("bMatchTerrain")]
        public bool isMatchingTerrain = true;

        //Temp editor buffers:
        [UnityEngine.Serialization.FormerlySerializedAs("bEdgeSignLabelInit")]
        public bool isEdgeSignLabelInit = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bEdgeSignLabel")]
        public bool isEdgeSignLabel = false;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeSignLabel")]
        public string edgeSignLabel = "";

        [UnityEngine.Serialization.FormerlySerializedAs("bCombineMesh")]
        public bool isCombinedMesh = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bCombineMeshCollider")]
        public bool isCombinedMeshCollider = false;
        [UnityEngine.Serialization.FormerlySerializedAs("MasterObj")]
        public GameObject masterObject = null;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectLocations")]
        public List<Vector3> edgeObjectLocations;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectRotations")]
        public List<Vector3> edgeObjectRotations;
        [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjects")]
        public List<GameObject> edgeObjects;
        [UnityEngine.Serialization.FormerlySerializedAs("SubType")]
        public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;
        [UnityEngine.Serialization.FormerlySerializedAs("MeterSep")]
        public float meterSep = 5f;
        [UnityEngine.Serialization.FormerlySerializedAs("bToggle")]
        public bool isToggled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
        public bool isBridge = false;


        #region "Horizontal offsets"
        [UnityEngine.Serialization.FormerlySerializedAs("HorizontalSep")]
        public float horizontalSep = 5f;
        [UnityEngine.Serialization.FormerlySerializedAs("HorizontalCurve")]
        public AnimationCurve horizontalCurve;
        #endregion


        #region "Vertical offsets"
        [UnityEngine.Serialization.FormerlySerializedAs("VerticalRaise")]
        public float verticalRaise = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("VerticalCurve")]
        public AnimationCurve verticalCurve;
        #endregion


        // Custom Rotation
        [UnityEngine.Serialization.FormerlySerializedAs("CustomRotation")]
        public Vector3 customRotation = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("bOncomingRotation")]
        public bool isOncomingRotation = true;

        // EdgeObject is static
        [UnityEngine.Serialization.FormerlySerializedAs("bStatic")]
        public bool isStatic = true;

        // The CustomScale of the EdgeObject
        [UnityEngine.Serialization.FormerlySerializedAs("CustomScale")]
        public Vector3 customScale = new Vector3(1f, 1f, 1f);

        // Start and EndTime
        [UnityEngine.Serialization.FormerlySerializedAs("StartTime")]
        public float startTime = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("EndTime")]
        public float endTime = 1f;

        [UnityEngine.Serialization.FormerlySerializedAs("SingleOnlyBridgePercent")]
        public float singleOnlyBridgePercent = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("StartPos")]
        public Vector3 startPos = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("EndPos")]
        public Vector3 endPos = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("bSingle")]
        public bool isSingle = false;

        // Should it be only on a single position
        [UnityEngine.Serialization.FormerlySerializedAs("SinglePosition")]
        public float singlePosition;
        [UnityEngine.Serialization.FormerlySerializedAs("bStartMatchRoadDefinition")]
        public bool isStartMatchRoadDefinition = false;
        [UnityEngine.Serialization.FormerlySerializedAs("StartMatchRoadDef")]
        public float startMatchRoadDef = 0f;

        // EdgeObjectName
        [UnityEngine.Serialization.FormerlySerializedAs("tName")]
        public string objectName = "EdgeObject";
        [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
        public string thumbString = "";
        [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
        public string desc = "";
        [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
        public string displayName = "";
        [UnityEngine.Serialization.FormerlySerializedAs("EM")]
        public EdgeObjectEditorMaker edgeMaker;
        #endregion


        public EdgeObjectMaker Copy()
        {
            EdgeObjectMaker EOM = new EdgeObjectMaker();

            EOM.edgeObjectString = edgeObjectString;

            #if UNITY_EDITOR
            EOM.edgeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(edgeObjectString);
            #endif

            EOM.isDefault = isDefault;

            EOM.isCombinedMesh = isCombinedMesh;
            EOM.isCombinedMeshCollider = isCombinedMeshCollider;
            EOM.subType = subType;
            EOM.meterSep = meterSep;
            EOM.isToggled = isToggled;
            EOM.isMatchingTerrain = isMatchingTerrain;

            EOM.isMaterialOverriden = isMaterialOverriden;
            EOM.edgeMaterial1 = edgeMaterial1;
            EOM.edgeMaterial2 = edgeMaterial2;

            EOM.masterObject = masterObject;
            EOM.edgeObjectLocations = edgeObjectLocations;
            EOM.edgeObjectRotations = edgeObjectRotations;
            EOM.node = node;
            EOM.startTime = startTime;
            EOM.endTime = endTime;
            EOM.startPos = startPos;
            EOM.endPos = endPos;
            EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
            EOM.isBridge = isBridge;

            EOM.horizontalSep = horizontalSep;
            EOM.horizontalCurve = new AnimationCurve();
            if (horizontalCurve != null && horizontalCurve.keys.Length > 0)
            {
                for (int i = 0; i < horizontalCurve.keys.Length; i++)
                {
                    EOM.horizontalCurve.AddKey(horizontalCurve.keys[i]);
                }
            }

            EOM.verticalRaise = verticalRaise;
            EOM.verticalCurve = new AnimationCurve();
            if (verticalCurve != null && verticalCurve.keys.Length > 0)
            {
                for (int index = 0; index < verticalCurve.keys.Length; index++)
                {
                    EOM.verticalCurve.AddKey(verticalCurve.keys[index]);
                }
            }

            EOM.customRotation = customRotation;
            EOM.isOncomingRotation = isOncomingRotation;
            EOM.isStatic = isStatic;
            EOM.isSingle = isSingle;
            EOM.singlePosition = singlePosition;

            EOM.isStartMatchRoadDefinition = isStartMatchRoadDefinition;
            EOM.startMatchRoadDef = startMatchRoadDef;

            RootUtils.SetupUniqueIdentifier(ref EOM.UID);

            EOM.objectName = objectName;
            EOM.thumbString = thumbString;
            EOM.desc = desc;
            EOM.displayName = displayName;

            return EOM;
        }


        public void UpdatePositions()
        {
            startPos = node.spline.GetSplineValue(startTime);
            endPos = node.spline.GetSplineValue(endTime);
        }


        #region "Library"
        /// <summary> Saves object as xml into Library folder. Auto prefixed with EOM and extension .rao </summary>
        public void SaveToLibrary(string _fileName = "", bool _isDefault = false)
        {
            EdgeObjectLibraryMaker EOLM = new EdgeObjectLibraryMaker();
            EOLM.Setup(this);
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(libraryPath, "EOM" + objectName + ".rao");
            if (_fileName.Length > 0)
            {
                if (_isDefault)
                {
                    filePath = Path.Combine(Path.Combine(libraryPath, "EdgeObjects"), "EOM" + _fileName + ".rao");
                }
                else
                {
                    filePath = Path.Combine(libraryPath, "EOM" + _fileName + ".rao");
                }
            }
            RootUtils.CreateXML<EdgeObjectLibraryMaker>(ref filePath, EOLM);
        }


        /// <summary> Loads _fileName from Library folder. Auto prefixed with EOM and extension .rao </summary>
        public void LoadFromLibrary(string _fileName, bool _isQuickAdd = false)
        {
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(libraryPath, "EOM" + _fileName + ".rao");
            if (_isQuickAdd)
            {
                filePath = Path.Combine(Path.Combine(libraryPath, "EdgeObjects"), "EOM" + _fileName + ".rao");
            }
            EdgeObjectLibraryMaker ELM = RootUtils.LoadXML<EdgeObjectLibraryMaker>(ref filePath);
            ELM.LoadTo(this);
            isRequiringUpdate = true;
        }


        public void LoadFromLibraryWizard(string _fileName)
        {
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(Path.Combine(libraryPath, "W"), _fileName + ".rao");
            EdgeObjectLibraryMaker ELM = RootUtils.LoadXML<EdgeObjectLibraryMaker>(ref filePath);
            ELM.LoadTo(this);
            isRequiringUpdate = true;
        }


        public string ConvertToString()
        {
            EdgeObjectLibraryMaker EOLM = new EdgeObjectLibraryMaker();
            EOLM.Setup(this);
            return RootUtils.GetString<EdgeObjectLibraryMaker>(EOLM);
        }


        /// <summary> Loads _EOLM into this EdgeObjectMaker </summary>
        public void LoadFromLibraryBulk(ref EdgeObjectLibraryMaker _EOLM)
        {
            _EOLM.LoadTo(this);
        }


        public static EdgeObjectLibraryMaker ELMFromData(string _data)
        {
            try
            {
                EdgeObjectLibraryMaker ELM = RootUtils.LoadData<EdgeObjectLibraryMaker>(ref _data);
                return ELM;
            }
            catch
            {
                return null;
            }
        }


        /// <summary> Stores .rao files which begin with EOM from Library folder into _names and _paths </summary>
        public static void GetLibraryFiles(out string[] _names, out string[] _paths, bool _isDefault = false)
        {
            _names = null;
            _paths = null;
            DirectoryInfo info;
            string libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault)
            {
                info = new DirectoryInfo(Path.Combine(libraryPath, "EdgeObjects"));
            }
            else
            {
                info = new DirectoryInfo(libraryPath);
            }

            FileInfo[] fileInfos = info.GetFiles();
            int count = 0;


            foreach (FileInfo tInfo in fileInfos)
            {
                if (tInfo.Name.Contains("EOM") && tInfo.Extension.ToLower().Contains("rao"))
                {
                    count += 1;
                }
            }

            _names = new string[count];
            _paths = new string[count];
            count = 0;
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Name.Contains("EOM") && fileInfo.Extension.ToLower().Contains("rao"))
                {
                    _names[count] = fileInfo.Name.Replace(".rao", "").Replace("EOM", "");
                    _paths[count] = fileInfo.FullName;
                    count += 1;
                }
            }
        }


        /// <summary> Saves _mesh as an asset into /Mesh/Generated/CombinedEdgeObj folder beside the /Asset folder </summary>
        private void SaveMesh(Mesh _mesh, bool _isCollider)
        {
            if (!node.spline.road.roadSystem.isSavingMeshes)
            {
                return;
            }

            string sceneName;
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");

            string folderName = Path.Combine(RoadEditorUtility.GetBasePath(), "Mesh");
            folderName = Path.Combine(folderName, "Generated");
            folderName = Path.Combine(folderName, "CombinedEdgeObj");

            string roadName = node.spline.road.transform.name;
            string fileName = sceneName + "-" + roadName + "-" + objectName;
            string finalName = Path.Combine(folderName, fileName + ".asset");
            if (_isCollider)
            {
                finalName = Path.Combine(folderName, fileName + "-collider.asset");
            }

            string path = Path.GetDirectoryName(Application.dataPath);
            path = Path.Combine(path, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            #if UNITY_EDITOR
            // Unity works with forward slash so we convert
            // If you want to implement your own Asset creation and saving you should just use finalName
            finalName = finalName.Replace(Path.DirectorySeparatorChar, '/');
            finalName = finalName.Replace(Path.AltDirectorySeparatorChar, '/');
            UnityEditor.AssetDatabase.CreateAsset(_mesh, finalName);
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }


        #region "Library object"
        [System.Serializable]
        public class EdgeObjectLibraryMaker
        {
            #region "Vars"
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectString")]
            public string edgeObjectString = "";
            [UnityEngine.Serialization.FormerlySerializedAs("bCombineMesh")]
            public bool isCombinedMesh = false;
            [UnityEngine.Serialization.FormerlySerializedAs("bCombineMeshCollider")]
            public bool isCombinedMeshCollider = false;
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectLocations")]
            public List<Vector3> edgeObjectLocations;
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectRotations")]
            public List<Vector3> edgeObjectRotations;
            [UnityEngine.Serialization.FormerlySerializedAs("SubType")]
            public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;
            [UnityEngine.Serialization.FormerlySerializedAs("MeterSep")]
            public float meterSep = 5f;
            [UnityEngine.Serialization.FormerlySerializedAs("bToggle")]
            public bool isToggled = false;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
            public bool isBridge = false;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsGSD")]
            public bool isDefault = false;
            [UnityEngine.Serialization.FormerlySerializedAs("bOncomingRotation")]
            public bool isOncomingRotation = true;
            [UnityEngine.Serialization.FormerlySerializedAs("bStatic")]
            public bool isStatic = true;
            [UnityEngine.Serialization.FormerlySerializedAs("bMatchTerrain")]
            public bool isMatchingTerrain = true;
            [UnityEngine.Serialization.FormerlySerializedAs("bSingle")]
            public bool isSingle = false;

            [UnityEngine.Serialization.FormerlySerializedAs("bMaterialOverride")]
            public bool isMaterialOverriden = false;
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial1String")]
            public string edgeMaterial1String = "";
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial2String")]
            public string edgeMaterial2String = "";

            //Horizontal offsets:
            [UnityEngine.Serialization.FormerlySerializedAs("HorizontalSep")]
            public float horizontalSep = 5f;
            [UnityEngine.Serialization.FormerlySerializedAs("HorizontalCurve")]
            public AnimationCurve horizontalCurve;
            //Vertical offsets:
            [UnityEngine.Serialization.FormerlySerializedAs("VerticalRaise")]
            public float verticalRaise = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("VerticalCurve")]
            public AnimationCurve verticalCurve;

            [UnityEngine.Serialization.FormerlySerializedAs("CustomRotation")]
            public Vector3 customRotation = default(Vector3);

            [UnityEngine.Serialization.FormerlySerializedAs("StartTime")]
            public float startTime = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("EndTime")]
            public float endTime = 1f;
            [UnityEngine.Serialization.FormerlySerializedAs("SingleOnlyBridgePercent")]
            public float singleOnlyBridgePercent = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("SinglePosition")]
            public float singlePosition;

            [UnityEngine.Serialization.FormerlySerializedAs("bStartMatchRoadDefinition")]
            public bool isStartMatchingRoadDefinition = false;
            [UnityEngine.Serialization.FormerlySerializedAs("StartMatchRoadDef")]
            public float startMatchRoadDef = 0f;

            [UnityEngine.Serialization.FormerlySerializedAs("tName")]
            public string objectName = "EdgeObject";
            [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
            public string thumbString = "";
            [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
            public string desc = "";
            [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
            public string displayName = "";
            #endregion


            /// <summary> Setup using _EOM </summary>
            public void Setup(EdgeObjectMaker _EOM)
            {
                edgeObjectString = _EOM.edgeObjectString;
                isCombinedMesh = _EOM.isCombinedMesh;
                isCombinedMeshCollider = _EOM.isCombinedMeshCollider;
                //SignPlacementSubTypeEnum SubType = _EOM.SubType;
                meterSep = _EOM.meterSep;
                isToggled = _EOM.isToggled;
                isDefault = _EOM.isDefault;

                isMaterialOverriden = _EOM.isMaterialOverriden;
                edgeMaterial1String = _EOM.edgeMaterial1String;
                edgeMaterial2String = _EOM.edgeMaterial2String;

                horizontalSep = _EOM.horizontalSep;
                horizontalCurve = _EOM.horizontalCurve;
                verticalRaise = _EOM.verticalRaise;
                verticalCurve = _EOM.verticalCurve;
                isMatchingTerrain = _EOM.isMatchingTerrain;

                customRotation = _EOM.customRotation;
                isOncomingRotation = _EOM.isOncomingRotation;
                isStatic = _EOM.isStatic;
                isSingle = _EOM.isSingle;
                singlePosition = _EOM.singlePosition;
                objectName = _EOM.objectName;
                singleOnlyBridgePercent = _EOM.singleOnlyBridgePercent;
                isStartMatchingRoadDefinition = _EOM.isStartMatchRoadDefinition;
                startMatchRoadDef = _EOM.startMatchRoadDef;
                thumbString = _EOM.thumbString;
                desc = _EOM.desc;
                isBridge = _EOM.isBridge;
                displayName = _EOM.displayName;
            }


            /// <summary> Copy relevant attributes to _EOM </summary>
            public void LoadTo(EdgeObjectMaker _EOM)
            {
                _EOM.edgeObjectString = edgeObjectString;
                #if UNITY_EDITOR
                _EOM.edgeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(edgeObjectString);

                if (edgeMaterial1String.Length > 0)
                {
                    _EOM.edgeMaterial1 = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(edgeMaterial1String);
                }
                if (edgeMaterial2String.Length > 0)
                {
                    _EOM.edgeMaterial2 = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(edgeMaterial2String);
                }
                #endif

                _EOM.isMaterialOverriden = isMaterialOverriden;

                _EOM.isCombinedMesh = isCombinedMesh;
                _EOM.isCombinedMeshCollider = isCombinedMeshCollider;
                _EOM.subType = subType;
                _EOM.meterSep = meterSep;
                _EOM.isToggled = isToggled;
                _EOM.isDefault = isDefault;

                _EOM.horizontalSep = horizontalSep;
                _EOM.horizontalCurve = horizontalCurve;
                _EOM.verticalRaise = verticalRaise;
                _EOM.verticalCurve = verticalCurve;
                _EOM.isMatchingTerrain = isMatchingTerrain;

                _EOM.customRotation = customRotation;
                _EOM.isOncomingRotation = isOncomingRotation;
                _EOM.isStatic = isStatic;
                _EOM.isSingle = isSingle;
                _EOM.singlePosition = singlePosition;
                _EOM.objectName = objectName;
                _EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
                _EOM.isStartMatchRoadDefinition = isStartMatchingRoadDefinition;
                _EOM.startMatchRoadDef = startMatchRoadDef;
                _EOM.thumbString = thumbString;
                _EOM.desc = desc;
                _EOM.isBridge = isBridge;
                _EOM.displayName = displayName;
            }
        }
        #endregion
        #endregion


        public class EdgeObjectEditorMaker
        {
            #region "Vars"
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObject")]
            public GameObject edgeObject = null;

            // Should we combine the Mesh?
            [UnityEngine.Serialization.FormerlySerializedAs("bCombineMesh")]
            public bool isCombinedMesh = false;

            // Should it also combine the Colliders
            [UnityEngine.Serialization.FormerlySerializedAs("bCombineMeshCollider")]
            public bool isCombinedMeshCollider = false;

            // Seems to be a List with all Locations for the EdgeObjects
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectLocations")]
            public List<Vector3> edgeObjectLocations;
            // Seems to be a List with all Rotations for the EdgeObjects
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeObjectRotations")]
            public List<Vector3> edgeObjectRotations;
            [UnityEngine.Serialization.FormerlySerializedAs("SubType")]
            public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;

            // Sounds like Speration
            [UnityEngine.Serialization.FormerlySerializedAs("MeterSep")]
            public float meterSep = 5f;

            // A Toggle for?
            [UnityEngine.Serialization.FormerlySerializedAs("bToggle")]
            public bool isToggled = false;

            [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
            public bool isBridge = false;

            [UnityEngine.Serialization.FormerlySerializedAs("bIsGSD")]
            public bool isDefault = false;

            // Materials of EdgeObject
            [UnityEngine.Serialization.FormerlySerializedAs("bMaterialOverride")]
            public bool isMaterialOverriden = false;
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial1")]
            public Material edgeMaterial1;
            [UnityEngine.Serialization.FormerlySerializedAs("EdgeMaterial2")]
            public Material edgeMaterial2;

            //Horizontal offsets:
            [UnityEngine.Serialization.FormerlySerializedAs("HorizontalSep")]
            public float horizontalSep = 5f;
            [UnityEngine.Serialization.FormerlySerializedAs("HorizontalCurve")]
            public AnimationCurve horizontalCurve;
            //Vertical offsets:
            [UnityEngine.Serialization.FormerlySerializedAs("VerticalRaise")]
            public float verticalRaise = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("VerticalCurve")]
            public AnimationCurve verticalCurve;

            [UnityEngine.Serialization.FormerlySerializedAs("bOncomingRotation")]
            public bool isOncomingRotation = true;
            [UnityEngine.Serialization.FormerlySerializedAs("bStatic")]
            public bool isStatic = true;
            [UnityEngine.Serialization.FormerlySerializedAs("bMatchTerrain")]
            public bool isMatchingTerrain = true;

            [UnityEngine.Serialization.FormerlySerializedAs("CustomScale")]
            public Vector3 customScale = new Vector3(1f, 1f, 1f);
            [UnityEngine.Serialization.FormerlySerializedAs("CustomRotation")]
            public Vector3 customRotation = default(Vector3);

            // Start and EndTime
            [UnityEngine.Serialization.FormerlySerializedAs("StartTime")]
            public float startTime = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("EndTime")]
            public float endTime = 1f;

            [UnityEngine.Serialization.FormerlySerializedAs("SingleOnlyBridgePercent")]
            public float singleOnlyBridgePercent = 0f;
            [UnityEngine.Serialization.FormerlySerializedAs("bSingle")]
            public bool isSingle = false;
            [UnityEngine.Serialization.FormerlySerializedAs("SinglePosition")]
            public float singlePosition;
            [UnityEngine.Serialization.FormerlySerializedAs("tName")]
            public string objectName;

            [UnityEngine.Serialization.FormerlySerializedAs("bStartMatchRoadDefinition")]
            public bool isStartMatchingRoadDefinition = false;
            [UnityEngine.Serialization.FormerlySerializedAs("StartMatchRoadDef")]
            public float startMatchRoadDef = 0f;
            #endregion


            /// <summary> Setup using _EOM </summary>
            public void Setup(EdgeObjectMaker _EOM)
            {
                edgeObject = _EOM.edgeObject;
                isCombinedMesh = _EOM.isCombinedMesh;
                isCombinedMeshCollider = _EOM.isCombinedMeshCollider;
                meterSep = _EOM.meterSep;
                isToggled = _EOM.isToggled;

                isMaterialOverriden = _EOM.isMaterialOverriden;
                edgeMaterial1 = _EOM.edgeMaterial1;
                edgeMaterial2 = _EOM.edgeMaterial2;

                // Horizontal
                horizontalSep = _EOM.horizontalSep;
                horizontalCurve = _EOM.horizontalCurve;

                // Vertical
                verticalRaise = _EOM.verticalRaise;
                verticalCurve = _EOM.verticalCurve;

                isMatchingTerrain = _EOM.isMatchingTerrain;

                // Rotation
                customRotation = _EOM.customRotation;
                isOncomingRotation = _EOM.isOncomingRotation;

                customScale = _EOM.customScale;
                isStatic = _EOM.isStatic;

                // Is it Single and if yes Position
                isSingle = _EOM.isSingle;
                singlePosition = _EOM.singlePosition;

                // Name of EdgeObject
                objectName = _EOM.objectName;

                // Start and EndTime of EdgeObject
                startTime = _EOM.startTime;
                endTime = _EOM.endTime;

                singleOnlyBridgePercent = _EOM.singleOnlyBridgePercent;
                isStartMatchingRoadDefinition = _EOM.isStartMatchRoadDefinition;
                startMatchRoadDef = _EOM.startMatchRoadDef;
            }


            /// <summary> Copy relevant attributes to _EOM </summary>
            public void LoadTo(EdgeObjectMaker _EOM)
            {
                _EOM.edgeObject = edgeObject;
                _EOM.isMaterialOverriden = isMaterialOverriden;
                _EOM.edgeMaterial1 = edgeMaterial1;
                _EOM.edgeMaterial2 = edgeMaterial2;

                _EOM.isCombinedMesh = isCombinedMesh;
                _EOM.isCombinedMeshCollider = isCombinedMeshCollider;
                _EOM.subType = subType;
                _EOM.meterSep = meterSep;
                _EOM.isToggled = isToggled;

                _EOM.horizontalSep = horizontalSep;
                _EOM.horizontalCurve = horizontalCurve;
                _EOM.verticalRaise = verticalRaise;
                _EOM.verticalCurve = verticalCurve;
                _EOM.isMatchingTerrain = isMatchingTerrain;

                _EOM.customRotation = customRotation;
                _EOM.customScale = customScale;
                _EOM.isOncomingRotation = isOncomingRotation;
                _EOM.isStatic = isStatic;
                _EOM.isSingle = isSingle;


                _EOM.startTime = startTime;
                _EOM.endTime = endTime;


                _EOM.singlePosition = singlePosition;


                _EOM.objectName = objectName;
                _EOM.singleOnlyBridgePercent = singleOnlyBridgePercent;
                _EOM.isStartMatchRoadDefinition = isStartMatchingRoadDefinition;
                _EOM.startMatchRoadDef = startMatchRoadDef;
            }


            public bool IsEqual(EdgeObjectMaker _EOM)
            {
                if (_EOM.edgeObject != edgeObject)
                {
                    return false;
                }
                if (_EOM.isMaterialOverriden != isMaterialOverriden)
                {
                    return false;
                }
                if (_EOM.edgeMaterial1 != edgeMaterial1)
                {
                    return false;
                }
                if (_EOM.edgeMaterial2 != edgeMaterial2)
                {
                    return false;
                }

                if (_EOM.isCombinedMesh != isCombinedMesh)
                {
                    return false;
                }
                if (_EOM.isCombinedMeshCollider != isCombinedMeshCollider)
                {
                    return false;
                }
                if (_EOM.subType != subType)
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.meterSep, meterSep, 0.001f))
                {
                    return false;
                }
                //if(EOM.isToggled != isToggled)
                //{
                //  return false;
                //}

                if (!RootUtils.IsApproximately(_EOM.horizontalSep, horizontalSep, 0.001f))
                {
                    return false;
                }
                if (_EOM.horizontalCurve != horizontalCurve)
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.verticalRaise, verticalRaise, 0.001f))
                {
                    return false;
                }
                if (_EOM.verticalCurve != verticalCurve)
                {
                    return false;
                }
                if (_EOM.isMatchingTerrain != isMatchingTerrain)
                {
                    return false;
                }

                if (_EOM.customRotation != customRotation)
                {
                    return false;
                }
                if (_EOM.customScale != customScale)
                {
                    return false;
                }
                if (_EOM.isOncomingRotation != isOncomingRotation)
                {
                    return false;
                }
                if (_EOM.isStatic != isStatic)
                {
                    return false;
                }
                if (_EOM.isSingle != isSingle)
                {
                    return false;
                }

                if (!RootUtils.IsApproximately(_EOM.singlePosition, singlePosition, 0.001f))
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.startTime, startTime, 0.001f))
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.endTime, endTime, 0.001f))
                {
                    return false;
                }
                if (_EOM.objectName != objectName)
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.singleOnlyBridgePercent, singleOnlyBridgePercent, 0.001f))
                {
                    return false;
                }
                if (_EOM.isStartMatchRoadDefinition != isStartMatchingRoadDefinition)
                {
                    return false;
                }
                if (!RootUtils.IsApproximately(_EOM.startMatchRoadDef, startMatchRoadDef, 0.001f))
                {
                    return false;
                }

                return true;
            }
        }


        #region "Setup and processing"
        public void Setup(bool _isCollecting = true)
        {
            List<GameObject> errorObjs = new List<GameObject>();
            try
            {
                SetupDo(_isCollecting, ref errorObjs);
            }
            catch (System.Exception exception)
            {
                if (errorObjs != null && errorObjs.Count > 0)
                {
                    int objCount = errorObjs.Count;
                    for (int index = 0; index < objCount; index++)
                    {
                        if (errorObjs[index] != null)
                        {
                            Object.DestroyImmediate(errorObjs[index]);
                        }
                    }
                    throw exception;
                }
            }
        }


        private void SetupDo(bool _isCollecting, ref List<GameObject> _errorObjs)
        {
            if (edgeObjects == null)
            {
                edgeObjects = new List<GameObject>();
            }
            if (horizontalCurve == null)
            {
                horizontalCurve = new AnimationCurve();
                horizontalCurve.AddKey(0f, 1f);
                horizontalCurve.AddKey(1f, 1f);
            }
            if (verticalCurve == null)
            {
                verticalCurve = new AnimationCurve();
                verticalCurve.AddKey(0f, 1f);
                verticalCurve.AddKey(1f, 1f);
            }

            RootUtils.SetupUniqueIdentifier(ref UID);

            SetupLocations();

            edgeObjectString = RootUtils.GetPrefabString(edgeObject);

            #if UNITY_EDITOR
            if (edgeMaterial1 != null)
            {
                edgeMaterial1String = UnityEditor.AssetDatabase.GetAssetPath(edgeMaterial1);
            }
            if (edgeMaterial2 != null)
            {
                edgeMaterial2String = UnityEditor.AssetDatabase.GetAssetPath(edgeMaterial2);
            }
            #endif

            edgeObjects = new List<GameObject>();

            Quaternion xRot = default(Quaternion);
            xRot = Quaternion.identity;
            xRot.eulerAngles = customRotation;
            int lCount = edgeObjectLocations.Count;
            //Quaternion OrigRot = Quaternion.identity;

            Material[] tMats = null;
            GameObject tObj = null;

            if (edgeObject != null)
            {
                GameObject mObj = new GameObject(edgeObject.name);
                masterObject = mObj;
                _errorObjs.Add(masterObject);
                mObj.transform.position = node.transform.position;
                mObj.transform.parent = node.transform;
                mObj.name = objectName;
                MeshRenderer OrigMR = edgeObject.GetComponent<MeshRenderer>();
                for (int j = 0; j < lCount; j++)
                {
                    if (edgeObjectRotations[j] == default(Vector3))
                    {
                        #if UNITY_EDITOR
                        // Instantiate prefab instead of object
                        tObj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(edgeObject);
                        #else
                        // Line to instantiate the object instead of an prefab
                        tObj = GameObject.Instantiate(edgeObject);
                        #endif
                        _errorObjs.Add(tObj);
                        tObj.transform.position = edgeObjectLocations[j];
                    }
                    else
                    {
                        #if UNITY_EDITOR
                        // Instantiate prefab instead of object
                        tObj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(edgeObject);
                        #else
                        // Line to instantiate the object instead of an prefab
                        tObj = GameObject.Instantiate(edgeObject);
                        #endif
                        tObj.transform.position = edgeObjectLocations[j];
                        tObj.transform.rotation = Quaternion.LookRotation(edgeObjectRotations[j]);
                        _errorObjs.Add(tObj);
                    }
                    //OrigRot = tObj.transform.rotation;
                    tObj.transform.rotation *= xRot;
                    tObj.transform.localScale = customScale;
                    if (isOncomingRotation && subType == SignPlacementSubTypeEnum.Left)
                    {
                        Quaternion tRot = new Quaternion(0f, 0f, 0f, 0f);
                        tRot = Quaternion.identity;
                        tRot.eulerAngles = new Vector3(0f, 180f, 0f);
                        tObj.transform.rotation *= tRot;
                    }
                    tObj.isStatic = isStatic;
                    tObj.transform.parent = mObj.transform;
                    edgeObjects.Add(tObj);

                    MeshRenderer NewMR = tObj.GetComponent<MeshRenderer>();
                    if (NewMR == null)
                    {
                        NewMR = tObj.AddComponent<MeshRenderer>();
                    }

                    if (!isMaterialOverriden && OrigMR != null && OrigMR.sharedMaterials.Length > 0 && NewMR != null)
                    {
                        NewMR.sharedMaterials = OrigMR.sharedMaterials;
                    }
                    else
                    {
                        if (edgeMaterial1 != null)
                        {
                            if (edgeMaterial2 != null)
                            {
                                tMats = new Material[2];
                                tMats[0] = edgeMaterial1;
                                tMats[1] = edgeMaterial2;
                            }
                            else
                            {
                                tMats = new Material[1];
                                tMats[0] = edgeMaterial1;
                            }
                            NewMR.sharedMaterials = tMats;
                        }
                    }
                }
            }

            lCount = edgeObjects.Count;
            if (lCount > 1 && isCombinedMesh)
            {
                Material[] tMat = null;
                Mesh xMeshBuffer = null;
                xMeshBuffer = edgeObject.GetComponent<MeshFilter>().sharedMesh;
                if (isMaterialOverriden)
                {
                    if (edgeMaterial1 != null)
                    {
                        if (edgeMaterial2 != null)
                        {
                            tMat = new Material[2];
                            tMat[0] = edgeMaterial1;
                            tMat[1] = edgeMaterial2;
                        }
                        else
                        {
                            tMat = new Material[1];
                            tMat[0] = edgeMaterial1;
                        }
                    }
                }
                else
                {
                    tMat = edgeObject.GetComponent<MeshRenderer>().sharedMaterials;
                }

                Vector3[] kVerts = xMeshBuffer.vertices;
                int[] kTris = xMeshBuffer.triangles;
                Vector2[] kUV = xMeshBuffer.uv;
                int OrigMVL = kVerts.Length;
                int OrigTriCount = xMeshBuffer.triangles.Length;

                List<Vector3[]> hVerts = new List<Vector3[]>();
                List<int[]> hTris = new List<int[]>();
                List<Vector2[]> hUV = new List<Vector2[]>();


                Transform tTrans;
                for (int j = 0; j < lCount; j++)
                {
                    tTrans = edgeObjects[j].transform;
                    hVerts.Add(new Vector3[OrigMVL]);
                    hTris.Add(new int[OrigTriCount]);
                    hUV.Add(new Vector2[OrigMVL]);

                    //Vertex copy:
                    System.Array.Copy(kVerts, hVerts[j], OrigMVL);
                    //Tri copy:
                    System.Array.Copy(kTris, hTris[j], OrigTriCount);
                    //UV copy:
                    System.Array.Copy(kUV, hUV[j], OrigMVL);

                    Vector3 tVect = default(Vector3);
                    for (int index = 0; index < OrigMVL; index++)
                    {
                        tVect = hVerts[j][index];
                        hVerts[j][index] = tTrans.rotation * tVect;
                        hVerts[j][index] += tTrans.localPosition;
                    }
                }

                GameObject xObj = new GameObject(objectName);
                MeshRenderer MR = xObj.GetComponent<MeshRenderer>();
                if (MR == null)
                {
                    MR = xObj.AddComponent<MeshRenderer>();
                }
                xObj.isStatic = isStatic;
                xObj.transform.parent = masterObject.transform;
                _errorObjs.Add(xObj);
                xObj.transform.name = xObj.transform.name + "Combined";
                xObj.transform.name = xObj.transform.name.Replace("(Clone)", "");
                MeshFilter MF = xObj.GetComponent<MeshFilter>();
                if (MF == null)
                {
                    MF = xObj.AddComponent<MeshFilter>();
                }
                MF.sharedMesh = CombineMeshes(ref hVerts, ref hTris, ref hUV, OrigMVL, OrigTriCount);
                MeshCollider MC = xObj.GetComponent<MeshCollider>();
                if (MC == null)
                {
                    MC = xObj.AddComponent<MeshCollider>();
                }
                xObj.transform.position = node.transform.position;
                xObj.transform.rotation = Quaternion.identity;

                for (int j = (lCount - 1); j >= 0; j--)
                {
                    Object.DestroyImmediate(edgeObjects[j]);
                }
                for (int j = 0; j < edgeObjects.Count; j++)
                {
                    edgeObjects[j] = null;
                }
                edgeObjects.RemoveRange(0, lCount);
                edgeObjects.Add(xObj);

                if (tMat != null && MR != null)
                {
                    MR.sharedMaterials = tMat;
                }

                BoxCollider BC = xObj.GetComponent<BoxCollider>();
                if (BC != null)
                {
                    Object.DestroyImmediate(BC);
                }
                int cCount = xObj.transform.childCount;
                int spamc = 0;
                while (cCount > 0 && spamc < 10)
                {
                    Object.DestroyImmediate(xObj.transform.GetChild(0).gameObject);
                    cCount = xObj.transform.childCount;
                    spamc += 1;
                }

                if (isCombinedMeshCollider)
                {
                    if (MC == null)
                    {
                        MC = xObj.AddComponent<MeshCollider>();
                    }
                    MC.sharedMesh = MF.sharedMesh;
                }
                else
                {
                    if (MC != null)
                    {
                        Object.DestroyImmediate(MC);
                        MC = null;
                    }
                }

                if (node.spline.road.roadSystem.isSavingMeshes && MF != null && isCombinedMesh)
                {
                    SaveMesh(MF.sharedMesh, false);
                    if (MC != null)
                    {
                        if (MF.sharedMesh != MC.sharedMesh)
                        {
                            SaveMesh(MC.sharedMesh, true);
                        }
                    }
                }

                //tMesh = null;
            }

            //Zero these out, as they are not needed anymore:
            if (edgeObjectLocations != null)
            {
                edgeObjectLocations.Clear();
                edgeObjectLocations = null;
            }
            if (edgeObjectRotations != null)
            {
                edgeObjectRotations.Clear();
                edgeObjectRotations = null;
            }

            if (_isCollecting)
            {
                node.spline.road.isTriggeringGC = true;
            }
        }


        /// <summary> Setup objects positions and rotations </summary>
        private void SetupLocations()
        {
            float origHeight = 0f;
            startTime = node.spline.GetClosestParam(startPos);
            endTime = node.spline.GetClosestParam(endPos);

            float fakeStartTime = startTime;
            if (isStartMatchRoadDefinition)
            {
                int index = node.spline.GetClosestRoadDefIndex(startTime, false, true);
                float time1 = node.spline.TranslateInverseParamToFloat(node.spline.RoadDefKeysArray[index]);
                float time2 = time1;
                if (index + 1 < node.spline.RoadDefKeysArray.Length)
                {
                    time2 = node.spline.TranslateInverseParamToFloat(node.spline.RoadDefKeysArray[index + 1]);
                }
                fakeStartTime = time1 + ((time2 - time1) * startMatchRoadDef);
            }


            //int eCount = EdgeObjects.Count;
            //Vector3 rVect = default(Vector3);
            //Vector3 lVect = default(Vector3);
            //float fTimeMax = -1f;
            int mCount = node.spline.GetNodeCount();
            if (node.idOnSpline >= mCount - 1)
            {
                return;
            }
            //fTimeMax = tNode.spline.mNodes[tNode.idOnSpline+1].tTime;
            //float tStep = -1f;
            Vector3 tVect = default(Vector3);
            Vector3 POS = default(Vector3);


            //tStep = MeterSep / tNode.spline.distance;
            //Destroy old objects:
            ClearEOM();
            //Make sure old locs and rots are fresh:
            if (edgeObjectLocations != null)
            {
                edgeObjectLocations.Clear();
                edgeObjectLocations = null;
            }
            edgeObjectLocations = new List<Vector3>();
            if (edgeObjectRotations != null)
            {
                edgeObjectRotations.Clear();
                edgeObjectRotations = null;
            }
            edgeObjectRotations = new List<Vector3>();
            bool bIsCenter = RootUtils.IsApproximately(horizontalSep, 0f, 0.02f);


            //Set rotation and locations:
            //Vector2 temp2DVect = default(Vector2);
            Ray tRay = default(Ray);
            RaycastHit[] tRayHit = null;
            float[] tRayYs = null;
            if (isSingle)
            {
                // If the Object is a SingleObject


                node.spline.GetSplineValueBoth(singlePosition, out tVect, out POS);
                origHeight = tVect.y;

                //Horizontal offset:
                if (!bIsCenter)
                {
                    //if(HorizontalSep > 0f)
                    //{
                    tVect = (tVect + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x));
                    //}
                    //else
                    //{
                    //  tVect = (tVect + new Vector3(HorizontalSep*-POS.normalized.z,0,HorizontalSep*POS.normalized.x));
                    //}
                }



                //Vertical:
                if (isMatchingTerrain)
                {
                    tRay = new Ray(tVect + new Vector3(0f, 1f, 0f), Vector3.down);
                    tRayHit = Physics.RaycastAll(tRay);
                    if (tRayHit.Length > 0)
                    {
                        tRayYs = new float[tRayHit.Length];
                        for (int g = 0; g < tRayHit.Length; g++)
                        {
                            tRayYs[g] = tRayHit[g].point.y;
                        }
                        tVect.y = Mathf.Max(tRayYs);
                    }
                }
                tVect.y += verticalRaise;

                startPos = tVect;
                endPos = tVect;

                if (float.IsNaN(tVect.y))
                {
                    tVect.y = origHeight;
                }

                edgeObjectLocations.Add(tVect);
                edgeObjectRotations.Add(POS);
            }
            else
            {
                // If this Object is not marked as a single Object

                //Get the vector series that this mesh is interpolated on:
                List<float> tTimes = new List<float>();
                float cTime = fakeStartTime;
                tTimes.Add(cTime);
                int SpamGuard = 5000;
                int SpamGuardCounter = 0;
                float pDiffTime = endTime - fakeStartTime;
                float CurrentH = 0f;
                float fHeight = 0f;
                Vector3 xVect = default(Vector3);
                while (cTime < endTime && SpamGuardCounter < SpamGuard)
                {
                    node.spline.GetSplineValueBoth(cTime, out tVect, out POS);

                    fHeight = horizontalCurve.Evaluate((cTime - fakeStartTime) / pDiffTime);
                    CurrentH = fHeight * horizontalSep;

                    // Hoirzontal1:
                    if (CurrentH < 0f)
                    {
                        // So we get a positiv Number again
                        CurrentH *= -1f;
                        tVect = (tVect + new Vector3(CurrentH * (-POS.normalized.x + (POS.normalized.y / 2)), 0, CurrentH * (POS.normalized.z + +(POS.normalized.y / 2))));
                        // I implemented the POS.normalized.y value to make sure we get to a value of 1 overall to ensure 50m distance, is this mathematicly correct?
                        // Original: tVect = (tVect + new Vector3(CurrentH * -POS.normalized.z, 0, CurrentH * POS.normalized.x));
                    }
                    else if (CurrentH > 0f)
                    {
                        tVect = (tVect + new Vector3(CurrentH * (-POS.normalized.x + (POS.normalized.y / 2)), 0, CurrentH * (POS.normalized.z + (POS.normalized.y / 2))));
                        // I implemented the POS.normalized.y value to make sure we get to a value of 1 overall to ensure 50m distance, is this mathematicly correct?
                        //Original: tVect = (tVect + new Vector3(CurrentH * POS.normalized.z, 0, CurrentH * -POS.normalized.x));
                    }

                    xVect = (POS.normalized * meterSep) + tVect;

                    cTime = node.spline.GetClosestParam(xVect, false, false);

                    if (cTime > endTime)
                    {
                        break;
                    }
                    tTimes.Add(cTime);
                    SpamGuardCounter += 1;
                }
                int vSeriesCount = tTimes.Count;

                float min = fakeStartTime;
                float max = endTime;
                float percent = 0;
                for (int index = 0; index < vSeriesCount; index++)
                {
                    node.spline.GetSplineValueBoth(tTimes[index], out tVect, out POS);

                    percent = ((tTimes[index] - min) / (max - min));

                    //Horiz:
                    CurrentH = (horizontalCurve.Evaluate(percent) * horizontalSep);
                    if (CurrentH < 0f)
                    {
                        CurrentH *= -1f;
                        // Why has this Code a "wrong" logic, it multiplies z to x and x to z.
                        // Original Code: tVect = (tVect + new Vector3(CurrentH * -POS.normalized.z, 0, CurrentH * POS.normalized.x));
                        tVect = (tVect + new Vector3(CurrentH * (-POS.normalized.z + (POS.normalized.y / 2)), 0, CurrentH * (POS.normalized.x + (POS.normalized.y / 2))));
                    }
                    else if (CurrentH > 0f)
                    {
                        // Original Code: tVect = (tVect + new Vector3(CurrentH * POS.normalized.z, 0, CurrentH * -POS.normalized.x));
                        // Look at the Bug embeddedt/RoadArchitect/issues/4
                        tVect = (tVect + new Vector3(CurrentH * (POS.normalized.z + (POS.normalized.y / 2)), 0, CurrentH * (-POS.normalized.x + (POS.normalized.y / 2))));
                    }

                    //Vertical:
                    if (isMatchingTerrain)
                    {
                        tRay = new Ray(tVect + new Vector3(0f, 1f, 0f), Vector3.down);
                        tRayHit = Physics.RaycastAll(tRay);
                        if (tRayHit.Length > 0)
                        {
                            tRayYs = new float[tRayHit.Length];
                            for (int g = 0; g < tRayHit.Length; g++)
                            {
                                tRayYs[g] = tRayHit[g].point.y;
                            }
                            tVect.y = Mathf.Max(tRayYs);
                        }
                    }

                    // Adds the Height to the Node including the VerticalRaise
                    tVect.y += (verticalCurve.Evaluate(percent) * verticalRaise);

                    // Adds the Vector and the POS to the List of the EdgeObjects, so they can be created
                    edgeObjectLocations.Add(tVect);
                    edgeObjectRotations.Add(POS);
                }
                startPos = node.spline.GetSplineValue(startTime);
                endPos = node.spline.GetSplineValue(endTime);
            }
        }


        //ref _verts, ref _tris, ref hNormals, ref _uv, ref hTangents
        private Mesh CombineMeshes(ref List<Vector3[]> _verts, ref List<int[]> _tris, ref List<Vector2[]> _uv, int _origMVL, int _origTriCount)
        {
            int mCount = _verts.Count;
            int NewMVL = _origMVL * mCount;
            Vector3[] tVerts = new Vector3[NewMVL];
            int[] tTris = new int[_origTriCount * mCount];
            Vector3[] tNormals = new Vector3[NewMVL];
            Vector2[] tUV = new Vector2[NewMVL];

            int CurrentMVLIndex = 0;
            int CurrentTriIndex = 0;
            for (int j = 0; j < mCount; j++)
            {
                CurrentMVLIndex = _origMVL * j;
                CurrentTriIndex = _origTriCount * j;

                if (j > 0)
                {
                    for (int index = 0; index < _origTriCount; index++)
                    {
                        _tris[j][index] += CurrentMVLIndex;
                    }
                }

                System.Array.Copy(_verts[j], 0, tVerts, CurrentMVLIndex, _origMVL);
                System.Array.Copy(_tris[j], 0, tTris, CurrentTriIndex, _origTriCount);
                System.Array.Copy(_uv[j], 0, tUV, CurrentMVLIndex, _origMVL);
            }

            Mesh mesh = new Mesh();
            mesh.vertices = tVerts;
            mesh.triangles = tTris;
            mesh.uv = tUV;
            mesh.normals = tNormals;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.tangents = RootUtils.ProcessTangents(tTris, tNormals, tUV, tVerts);
            return mesh;
        }


        /// <summary> Destroys edgeObjects and masterObject </summary>
        public void ClearEOM()
        {
            if (edgeObjects != null)
            {
                int hCount = edgeObjects.Count - 1;
                for (int h = hCount; h >= 0; h--)
                {
                    if (edgeObjects[h] != null)
                    {
                        Object.DestroyImmediate(edgeObjects[h].transform.gameObject);
                    }
                }
            }
            if (masterObject != null)
            {
                Object.DestroyImmediate(masterObject);
            }
        }
        #endregion


        public void SetDefaultTimes(bool _isEndPoint, float _time, float _timeNext, int _idOnSpline, float _dist)
        {
            if (!_isEndPoint)
            {
                startTime = _time;
                endTime = _timeNext;
            }
            else
            {
                if (_idOnSpline < 2)
                {
                    startTime = _time;
                    endTime = _timeNext;
                }
                else
                {
                    startTime = _time;
                    endTime = _time - (125f / _dist);
                }
            }
        }
    }
}
