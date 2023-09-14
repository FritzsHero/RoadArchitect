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
        public bool isRequiringUpdate = false;
        public string UID = "";
        public SplineN node = null;
        public bool isDefault = false;
        public GameObject edgeObject = null;
        public string edgeObjectString = "";
        public bool isMaterialOverriden = false;
        public Material edgeMaterial1 = null;
        public Material edgeMaterial2 = null;
        public string edgeMaterial1String = null;
        public string edgeMaterial2String = null;
        public bool isMatchingTerrain = true;

        public bool isCombinedMesh = false;
        public bool isCombinedMeshCollider = false;
        public GameObject masterObject = null;
        public List<Vector3> edgeObjectLocations;
        public List<Vector3> edgeObjectRotations;
        public List<GameObject> edgeObjects;
        public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;
        public float meterSep = 5f;
        public bool isToggled = false;
        public bool isBridge = false;


        #region "Horizontal offsets"
        public float horizontalSep = 5f;
        public AnimationCurve horizontalCurve;
        #endregion


        #region "Vertical offsets"
        public float verticalRaise = 0f;
        public AnimationCurve verticalCurve;
        #endregion


        // Custom Rotation
        public Vector3 customRotation = default(Vector3);
        public bool isRotationAligning = true;
        public bool isXRotationLocked = true;
        public bool isYRotationLocked = false;
        public bool isZRotationLocked = false;
        public bool isOncomingRotation = true;

        // EdgeObject is static
        public bool isStatic = true;

        // The CustomScale of the EdgeObject
        public Vector3 customScale = new Vector3(1f, 1f, 1f);

        // Start and EndTime
        public float startTime = 0f;
        public float endTime = 1f;

        public float singleOnlyBridgePercent = 0f;
        public Vector3 startPos = default(Vector3);
        public Vector3 endPos = default(Vector3);
        public bool isSingle = false;

        // Should it be only on a single position
        public float singlePosition;
        public bool isStartMatchRoadDefinition = false;
        public float startMatchRoadDef = 0f;

        // EdgeObjectName
        public string objectName = "EdgeObject";
        public string thumbString = "";
        public string desc = "";
        public string displayName = "";
        #endregion


        public EdgeObjectMaker Copy()
        {
            EdgeObjectMaker EOM = new EdgeObjectMaker();

            EOM.edgeObjectString = edgeObjectString;

            EOM.edgeObject = EngineIntegration.LoadAssetFromPath<GameObject>(edgeObjectString);

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
            EOM.isRotationAligning = isRotationAligning;
            EOM.isXRotationLocked = isXRotationLocked;
            EOM.isYRotationLocked = isYRotationLocked;
            EOM.isZRotationLocked = isZRotationLocked;
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
            if (!node.spline.road.isSavingMeshes)
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

            finalName = EngineIntegration.GetUnityFilePath(finalName);
            EngineIntegration.CreateAsset(_mesh, finalName);
            EngineIntegration.SaveAssets();
        }


        #region "Library object"
        [System.Serializable]
        public class EdgeObjectLibraryMaker
        {
            #region "Vars"
            public string edgeObjectString = "";
            public bool isCombinedMesh = false;
            public bool isCombinedMeshCollider = false;
            public List<Vector3> edgeObjectLocations;
            public List<Vector3> edgeObjectRotations;
            public SignPlacementSubTypeEnum subType = SignPlacementSubTypeEnum.Right;
            public float meterSep = 5f;
            public bool isToggled = false;
            public bool isBridge = false;
            public bool isDefault = false;
            public bool isOncomingRotation = true;
            public bool isStatic = true;
            public bool isMatchingTerrain = true;
            public bool isSingle = false;

            public bool isMaterialOverriden = false;
            public string edgeMaterial1String = "";
            public string edgeMaterial2String = "";

            //Horizontal offsets:
            public float horizontalSep = 5f;
            public AnimationCurve horizontalCurve;
            //Vertical offsets:
            public float verticalRaise = 0f;
            public AnimationCurve verticalCurve;

            public Vector3 customRotation = default(Vector3);
            public bool isRotationAligning = true;
            public bool isXRotationLocked = true;
            public bool isYRotationLocked = false;
            public bool isZRotationLocked = false;

            public float startTime = 0f;
            public float endTime = 1f;
            public float singleOnlyBridgePercent = 0f;
            public float singlePosition;

            public bool isStartMatchingRoadDefinition = false;
            public float startMatchRoadDef = 0f;

            public string objectName = "EdgeObject";
            public string thumbString = "";
            public string desc = "";
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
                isRotationAligning = _EOM.isRotationAligning;
                isXRotationLocked = _EOM.isXRotationLocked;
                isYRotationLocked = _EOM.isYRotationLocked;
                isZRotationLocked = _EOM.isZRotationLocked;
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
                _EOM.edgeObject = EngineIntegration.LoadAssetFromPath<GameObject>(edgeObjectString);

                if (edgeMaterial1String.Length > 0)
                {
                    _EOM.edgeMaterial1 = EngineIntegration.LoadAssetFromPath<Material>(edgeMaterial1String);
                }
                if (edgeMaterial2String.Length > 0)
                {
                    _EOM.edgeMaterial2 = EngineIntegration.LoadAssetFromPath<Material>(edgeMaterial2String);
                }

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
                _EOM.isRotationAligning = isRotationAligning;
                _EOM.isXRotationLocked = isXRotationLocked;
                _EOM.isYRotationLocked = isYRotationLocked;
                _EOM.isZRotationLocked = isZRotationLocked;
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


        #region "Setup and processing"
        public void Setup(bool _isCollecting = true)
        {
            List<GameObject> errorObjs = new List<GameObject>();
            try
            {
                SetupDo(ref errorObjs);
                if (_isCollecting)
                {
                    node.spline.road.isTriggeringGC = true;
                }
            }
            catch (System.Exception exception)
            {
                if (errorObjs != null && errorObjs.Count > 0)
                {
                    int objCount = errorObjs.Count;
                    for (int index = 0; index < objCount; index++)
                    {
                        Object.DestroyImmediate(errorObjs[index]);
                    }
                    throw exception;
                }
            }
        }


        private void SetupDo(ref List<GameObject> _errorObjs)
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

            if (edgeMaterial1 != null)
            {
                edgeMaterial1String = EngineIntegration.GetAssetPath(edgeMaterial1);
            }
            if (edgeMaterial2 != null)
            {
                edgeMaterial2String = EngineIntegration.GetAssetPath(edgeMaterial2);
            }

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
                    tObj = EngineIntegration.InstantiatePrefab(edgeObject);
                    tObj.transform.position = edgeObjectLocations[j];

                    if (edgeObjectRotations[j] == default(Vector3))
                    {
                        _errorObjs.Add(tObj);
                    }
                    else
                    {
                        if(isRotationAligning)
                        {
                            tObj.transform.rotation = Quaternion.LookRotation(edgeObjectRotations[j]);
                        }
                        else
                        {
                            Quaternion rotation = Quaternion.LookRotation(edgeObjectRotations[j]);
                            Vector3 eulerRotation = rotation.eulerAngles;

                            if(isXRotationLocked)
                            {
                                eulerRotation.x = 0;
                            }
                            if (isYRotationLocked)
                            {
                                eulerRotation.y = 0;
                            }
                            if (isZRotationLocked)
                            {
                                eulerRotation.z = 0;
                            }

                            rotation = Quaternion.Euler(eulerRotation);
                            tObj.transform.rotation = rotation;
                        }
                        _errorObjs.Add(tObj);
                    }
                    //OrigRot = tObj.transform.rotation;
                    tObj.transform.rotation *= xRot;
                    tObj.transform.localScale = customScale;

                    // Turn object by 180 for other side of road
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

                    for (int index = 0; index < OrigMVL; index++)
                    {
                        hVerts[j][index] = (tTrans.rotation * hVerts[j][index]) + tTrans.localPosition;
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
                Object.DestroyImmediate(BC);
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
                    Object.DestroyImmediate(MC);
                    MC = null;
                }

                if (node.spline.road.isSavingMeshes && MF != null && isCombinedMesh)
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
            RootUtils.NullifyList(ref edgeObjectLocations);
            RootUtils.NullifyList(ref edgeObjectRotations);
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
            RootUtils.NullifyList(ref edgeObjectLocations);
            RootUtils.NullifyList(ref edgeObjectRotations);
            edgeObjectLocations = new List<Vector3>();
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
            Object.DestroyImmediate(masterObject);
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
