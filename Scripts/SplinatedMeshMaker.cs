#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using RoadArchitect;
#endregion


namespace RoadArchitect.Splination
{
    public enum AxisTypeEnum { X, Z };
    public enum CollisionTypeEnum { None, SimpleMeshTriangle, SimpleMeshTrapezoid, MeshCollision, BoxCollision };
    public enum RepeatUVTypeEnum { None, X, Y };


    [System.Serializable]
    public class SplinatedMeshMaker
    {
        #region "Vars"
        public string uID = "";

        public bool isRequiringUpdate = false;
        public bool isDefault = false;
        public bool isMaterialOverriden = false;
        public bool isExactSplination = false;
        public bool isMatchingRoadDefinition = false;
        public bool isMatchingRoadIncrements = true;
        public bool isTrimEnd = false;
        public bool isTrimStart = false;
        public bool isToggled = false;
        public bool isMatchingTerrain = false;
        public bool isBridge = false;
        public bool isStretched = false;
        public bool isStretchedOffset = false;
        public bool isStretchedSize = false;
        public bool isBoxColliderFlippedOnX = false;
        public bool isBoxColliderFlippedOnZ = false;
        public bool isStraightLineMatchStartEnd = false;
        public bool isFlippingRotation = false;
        public bool isStatic = true;
        public bool isNotCenterMode = true;

        public Transform masterObjTrans = null;
        public GameObject currentSplination = null;
        public GameObject currentSplinationCap1 = null;
        public GameObject currentSplinationCap2 = null;
        public string currentSplinationString = "";
        public string currentSplinationCap1String = "";
        public string currentSplinationCap2String = "";
        public float capHeightOffset1 = 0f;
        public float capHeightOffset2 = 0f;
        public float minMaxMod = 0.002f;
        public float vertexMatchingPrecision = 0.005f;
        public float stretchUVThreshold = 0.05f;

        public GameObject Output = null;
        public Mesh mesh = null;
        public Material SplinatedMaterial1 = null;
        public Material SplinatedMaterial2 = null;
        public string SplinatedMaterial1String = "";
        public string SplinatedMaterial2String = "";

        public Vector3 boxColliderOffset = default(Vector3);
        public Vector3 boxColliderSize = default(Vector3);


        #region "Horizontal offsets"
        public AnimationCurve horizontalCurve;
        public float horizontalSep = 0f;
        #endregion


        #region "Vertical offsets"
        public AnimationCurve verticalCurve;
        public float verticalRaise = 0f;
        #endregion


        #region "Vertical cutoff"
        public float VerticalCutoff = 0f;
        public float VerticalMeshCutoffOffset = 0.04f;
        public bool isVerticalCutoff = false;
        public bool isVerticalCutoffDownwards = false;
        public bool isVerticalMeshCutoffOppositeDir = false;
        public bool isVerticalCutoffMatchingZero = false;
        #endregion


        public float RoadRaise = 0f;
        public float StartTime = 0f;
        public float EndTime = 1f;
        public float mMaxX = -1f;
        public float mMinX = -1f;
        public float mMaxY = -1f;
        public float mMinY = -1f;
        public float mMaxZ = -1f;
        public float mMinZ = -1f;

        public Vector3 CustomRotation = default(Vector3);
        public Vector3 StartPos = default(Vector3);
        public Vector3 EndPos = default(Vector3);
        public SplineC spline = null;
        public SplineN node = null;
        public AxisTypeEnum Axis = AxisTypeEnum.X;

        public RepeatUVTypeEnum RepeatUVType = RepeatUVTypeEnum.None;


        #region "End objects"
        public GameObject EndCapStart = null;
        public GameObject EndCapEnd = null;
        public GameObject EndCapStartOutput = null;
        public GameObject EndCapEndOutput = null;
        public string EndCapStartString = "";
        public string EndCapEndString = "";
        public bool isEndCapCustomMatchStart = true;
        public bool isEndObjectsMatchingGround = false;
        public Vector3 EndCapCustomOffsetStart = default(Vector3);
        public Vector3 EndCapCustomOffsetEnd = default(Vector3);
        public Vector3 EndCapCustomRotOffsetStart = default(Vector3);
        public Vector3 EndCapCustomRotOffsetEnd = new Vector3(0f, 180f, 0f);
        #endregion


        #region "Endings down"
        public bool isStartDown = false;
        public bool isStartTypeDownOverriden = false;
        public float startTypeDownOverride = 0f;
        public bool isEndDown = false;
        public bool isEndTypeDownOverriden = false;
        public float endTypeDownOverride = 0f;
        #endregion


        #region "Collision"
        public CollisionTypeEnum CollisionType = CollisionTypeEnum.SimpleMeshTriangle;
        public bool isCollisionConvex = false;
        public bool isSimpleCollisionAutomatic = true;
        public bool isCollisionExtrude = false;
        public bool isCollisionTrigger = false;

        // Bottom Left
        public Vector3 CollisionBoxBL = default(Vector3);
        // Bottom Right
        public Vector3 CollisionBoxBR = default(Vector3);
        // Top Left
        public Vector3 CollisionBoxTL = default(Vector3);
        // Top Right
        public Vector3 CollisionBoxTR = default(Vector3);

        public Vector3 CollisionTriBL = default(Vector3);
        public Vector3 CollisionTriBR = default(Vector3);
        public Vector3 CollisionTriT = default(Vector3);
        #endregion


        public string objectName = "ExtrudedObject";
        public string thumbString = "";
        public string desc = "";
        public string displayName = "";
        #endregion


        public void Init(SplineC _spline, SplineN _node, Transform _transform)
        {
            spline = _spline;
            node = _node;
            masterObjTrans = _transform;
            RootUtils.SetupUniqueIdentifier(ref uID);
        }


        public SplinatedMeshMaker Copy()
        {
            SplinatedMeshMaker SMM = new SplinatedMeshMaker();
            SMM.Init(spline, node, masterObjTrans);
            SMM.masterObjTrans = masterObjTrans;
            SMM.isDefault = isDefault;

            SMM.currentSplination = currentSplination;
            SMM.currentSplinationString = currentSplinationString;

            SMM.currentSplinationCap1 = currentSplinationCap1;
            SMM.currentSplinationCap1String = currentSplinationCap1String;
            SMM.currentSplinationCap2 = currentSplinationCap2;
            SMM.currentSplinationCap2String = currentSplinationCap2String;
            SMM.capHeightOffset1 = capHeightOffset1;
            SMM.capHeightOffset1 = capHeightOffset2;

            SMM.Output = Output;
            SMM.mesh = mesh;
            SMM.isMaterialOverriden = isMaterialOverriden;
            SMM.SplinatedMaterial1 = SplinatedMaterial1;
            SMM.SplinatedMaterial2 = SplinatedMaterial2;
            SMM.SplinatedMaterial1String = SplinatedMaterial1String;
            SMM.SplinatedMaterial2String = SplinatedMaterial2String;
            SMM.isExactSplination = isExactSplination;
            SMM.isMatchingRoadDefinition = isMatchingRoadDefinition;
            SMM.isMatchingRoadIncrements = isMatchingRoadIncrements;
            SMM.isTrimStart = isTrimStart;
            SMM.isTrimEnd = isTrimEnd;
            SMM.isMatchingTerrain = isMatchingTerrain;
            SMM.minMaxMod = minMaxMod;
            SMM.vertexMatchingPrecision = vertexMatchingPrecision;

            SMM.isStretched = isStretched;
            SMM.isStretchedOffset = isStretchedOffset;
            SMM.isStretchedSize = isStretchedSize;
            SMM.boxColliderOffset = boxColliderOffset;
            SMM.boxColliderSize = boxColliderSize;
            SMM.stretchUVThreshold = stretchUVThreshold;
            SMM.isStraightLineMatchStartEnd = isStraightLineMatchStartEnd;
            SMM.isBoxColliderFlippedOnX = isBoxColliderFlippedOnX;
            SMM.isBoxColliderFlippedOnZ = isBoxColliderFlippedOnZ;

            //Horizontal offsets:
            SMM.horizontalSep = horizontalSep;
            SMM.horizontalCurve = new AnimationCurve();
            if (horizontalCurve != null && horizontalCurve.keys.Length > 0)
            {
                for (int i = 0; i < horizontalCurve.keys.Length; i++)
                {
                    SMM.horizontalCurve.AddKey(horizontalCurve.keys[i]);
                }
            }
            //Vertical offset:
            SMM.verticalRaise = verticalRaise;
            SMM.verticalCurve = new AnimationCurve();
            if (verticalCurve != null && verticalCurve.keys.Length > 0)
            {
                for (int i = 0; i < verticalCurve.keys.Length; i++)
                {
                    SMM.verticalCurve.AddKey(verticalCurve.keys[i]);
                }
            }

            //Vertical cutoff:
            SMM.isVerticalCutoff = isVerticalCutoff;
            SMM.VerticalCutoff = VerticalCutoff;
            SMM.isVerticalCutoffDownwards = isVerticalCutoffDownwards;
            SMM.isVerticalMeshCutoffOppositeDir = isVerticalMeshCutoffOppositeDir;
            SMM.VerticalMeshCutoffOffset = VerticalMeshCutoffOffset;
            SMM.isVerticalCutoffMatchingZero = isVerticalCutoffMatchingZero;

            SMM.RoadRaise = RoadRaise;
            SMM.CustomRotation = CustomRotation;
            SMM.isFlippingRotation = isFlippingRotation;
            SMM.isStatic = isStatic;
            SMM.StartTime = StartTime;
            SMM.EndTime = EndTime;
            SMM.StartPos = StartPos;
            SMM.EndPos = EndPos;
            SMM.Axis = Axis;

            SMM.RepeatUVType = RepeatUVType;

            SMM.mMaxX = mMaxX;
            SMM.mMinX = mMinX;
            SMM.mMaxY = mMaxY;
            SMM.mMinY = mMinY;
            SMM.mMaxZ = mMaxZ;
            SMM.mMinZ = mMinZ;

            //End objects:
            SMM.EndCapStart = EndCapStart;
            SMM.EndCapStartString = EndCapStartString;
            SMM.EndCapEnd = EndCapEnd;
            SMM.EndCapEndString = EndCapEndString;
            SMM.isEndCapCustomMatchStart = isEndCapCustomMatchStart;
            SMM.EndCapCustomOffsetStart = EndCapCustomOffsetStart;
            SMM.EndCapCustomOffsetEnd = EndCapCustomOffsetEnd;
            SMM.EndCapCustomRotOffsetStart = EndCapCustomRotOffsetStart;
            SMM.EndCapCustomRotOffsetEnd = EndCapCustomRotOffsetEnd;
            SMM.isEndObjectsMatchingGround = isEndObjectsMatchingGround;
            SMM.isBridge = isBridge;
            //End down:
            SMM.isStartDown = isStartDown;
            SMM.isStartTypeDownOverriden = isStartTypeDownOverriden;
            SMM.startTypeDownOverride = startTypeDownOverride;
            SMM.isEndDown = isEndDown;
            SMM.isEndTypeDownOverriden = isEndTypeDownOverriden;
            SMM.endTypeDownOverride = endTypeDownOverride;

            //Collision:
            SMM.CollisionType = CollisionType;
            SMM.isCollisionConvex = isCollisionConvex;
            SMM.isSimpleCollisionAutomatic = isSimpleCollisionAutomatic;
            SMM.isCollisionTrigger = isCollisionTrigger;

            SMM.CollisionBoxBL = CollisionBoxBL;
            SMM.CollisionBoxBR = CollisionBoxBR;
            SMM.CollisionBoxTL = CollisionBoxTL;
            SMM.CollisionBoxTR = CollisionBoxTR;

            SMM.CollisionTriBL = CollisionTriBL;
            SMM.CollisionTriBR = CollisionTriBR;
            SMM.CollisionTriT = CollisionTriT;

            SMM.objectName = objectName;
            SMM.thumbString = thumbString;
            SMM.desc = desc;
            SMM.displayName = displayName;

            RootUtils.SetupUniqueIdentifier(ref SMM.uID);

            return SMM;
        }


        public void SetDefaultTimes(bool _isEndPoint, float _time, float _timeNext, int _idOnSpline, float _dist)
        {
            if (!_isEndPoint)
            {
                StartTime = _time;
                EndTime = _timeNext;
            }
            else
            {
                if (_idOnSpline < 2)
                {
                    StartTime = _time;
                    EndTime = _timeNext;
                }
                else
                {
                    StartTime = _time;
                    EndTime = _time - (125f / _dist);
                }
            }
        }


        public void UpdatePositions()
        {
            StartPos = spline.GetSplineValue(StartTime);
            EndPos = spline.GetSplineValue(EndTime);
        }


        /// <summary> Saves object as xml into Library folder. Auto prefixed with ESO and extension .rao </summary>
        public void SaveToLibrary(string _name = "", bool _isDefault = false)
        {
            SplinatedMeshLibraryMaker SLM = new SplinatedMeshLibraryMaker();
            SLM.Setup(this);
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(libraryPath, "ESO" + objectName + ".rao");
            if (_name.Length > 0)
            {
                if (_isDefault)
                {
                    // Q Folder is now ExtrudedObjects
                    filePath = Path.Combine(Path.Combine(libraryPath, "ExtrudedObjects"), "ESO" + _name + ".rao");
                }
                else
                {
                    filePath = Path.Combine(libraryPath, "ESO" + _name + ".rao");
                }
            }
            RootUtils.CreateXML<SplinatedMeshLibraryMaker>(ref filePath, SLM);
        }


        /// <summary> Loads _fileName from Library folder. Auto prefixed with ESO and extension .rao </summary>
        public void LoadFromLibrary(string _name, bool _isQuickAdd = false)
        {
            // Q Folder is now ExtrudedObjects
            string fileName = "ESO" + _name + ".rao";
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(libraryPath, fileName);
            if (_isQuickAdd)
            {
                RootUtils.CheckCreateSpecialLibraryDirs();
                filePath = Path.Combine(Path.Combine(libraryPath, "ExtrudedObjects"), fileName);
            }
            SplinatedMeshLibraryMaker SLM = RootUtils.LoadXML<SplinatedMeshLibraryMaker>(ref filePath);
            SLM.LoadToSMM(this);
            isRequiringUpdate = true;
        }


        // Same as also written in EdgeObjectMaker
        public void LoadFromLibraryWizard(string _name)
        {
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(Path.Combine(libraryPath, "W"), _name + ".rao");
            SplinatedMeshLibraryMaker SLM = RootUtils.LoadXML<SplinatedMeshLibraryMaker>(ref filePath);
            SLM.LoadToSMM(this);
            isRequiringUpdate = true;
        }


        /// <summary> Loads _SMLM into this SplinatedMeshMaker </summary>
        public void LoadFromLibraryBulk(ref SplinatedMeshLibraryMaker _SMLM)
        {
            _SMLM.LoadToSMM(this);
            //bNeedsUpdate = true;
        }


        public static SplinatedMeshLibraryMaker SLMFromData(string _data)
        {
            try
            {
                SplinatedMeshLibraryMaker SLM = RootUtils.LoadData<SplinatedMeshLibraryMaker>(ref _data);
                return SLM;
            }
            catch
            {
                return null;
            }
        }


        public string ConvertToString()
        {
            SplinatedMeshLibraryMaker SLM = new SplinatedMeshLibraryMaker();
            SLM.Setup(this);
            return RootUtils.GetString<SplinatedMeshLibraryMaker>(SLM);
        }


        /// <summary> Stores .rao files which begin with ESO from Library folder into _names and _paths </summary>
        public static void GetLibraryFiles(out string[] _names, out string[] _paths, bool _isDefault = false)
        {
            _names = null;
            _paths = null;
            DirectoryInfo info;
            string libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault)
            {
                info = new DirectoryInfo(Path.Combine(libraryPath, "ExtrudedObjects"));
            }
            else
            {
                info = new DirectoryInfo(libraryPath);
            }
            FileInfo[] fileInfos = info.GetFiles();
            int esoCount = 0;
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Name.Contains("ESO") && fileInfo.Extension.ToLower().Contains("rao"))
                {
                    esoCount += 1;
                }
            }

            _names = new string[esoCount];
            _paths = new string[esoCount];
            esoCount = 0;
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Name.Contains("ESO") && fileInfo.Extension.ToLower().Contains("rao"))
                {
                    _names[esoCount] = fileInfo.Name.Replace(".rao", "").Replace("ESO", "");
                    _paths[esoCount] = fileInfo.FullName;
                    esoCount += 1;
                }
            }
        }


        /// <summary> Destroys Output, EndCapStartOutput and EndCapEndOutput </summary>
        public void Kill()
        {
            Object.DestroyImmediate(Output);
            Object.DestroyImmediate(EndCapStartOutput);
            Object.DestroyImmediate(EndCapEndOutput);
        }


        [System.Serializable]
        public class SplinatedMeshLibraryMaker
        {
            #region "Vars"
            public string CurrentSplinationString = "";
            public string CurrentSplinationCap1String = "";
            public string CurrentSplinationCap2String = "";
            public bool isDefault = false;

            public bool isMaterialOverriden = false;
            public string SplinatedMaterial1String = "";
            public string SplinatedMaterial2String = "";
            public float CapHeightOffset1 = 0f;
            public float CapHeightOffset2 = 0f;
            public bool isExactSplination = false;
            public bool isMatchingRoadDefinition = false;
            public bool isMatchingRoadIncrements = true;
            public bool isTrimStart = false;
            public bool isTrimEnd = false;
            public bool isToggled = false;
            public bool isMatchingTerrain = false;
            public float MinMaxMod = 0.002f;
            public float VertexMatchingPrecision = 0.005f;

            public bool isStretched = false;
            public bool isStretchedLocOffset = false;
            public bool isStretchedSize = false;
            public Vector3 boxColliderOffset = default(Vector3);
            public Vector3 boxColliderSize = default(Vector3);
            public float stretchUVThreshold = 0.05f;
            public bool isStraightLineMatchingStartEnd = false;
            public bool isBoxColliderFlippedOnX = false;
            public bool isBoxColliderFlippedOnZ = false;

            //Horizontal offsets:
            public float horizontalSep = 5f;
            public AnimationCurve horizontalCurve;
            //Vertical offsets:
            public float verticalRaise = 0f;
            public AnimationCurve verticalCurve;
            //Vertical cutoff:
            public float VerticalCutoff = 0f;
            public bool isVerticalCutoff = false;
            public bool isVerticalCutoffDownwards = false;
            public bool isVerticalMeshCutoffOppositeDir = false;
            public float VerticalMeshCutoffOffset = 0.04f;
            public bool isVerticalCutoffMatchingZero = false;

            public float RoadRaise = 0f;
            public Vector3 CustomRotation = default(Vector3);
            public bool isFlippedRotation = false;
            public bool isStatic = true;
            public float StartTime = 0f;
            public float EndTime = 1f;
            public int Axis = 0;
            public bool isBridge = false;

            public float mMaxX = -1f;
            public float mMinX = -1f;
            public float mMaxY = -1f;
            public float mMinY = -1f;
            public float mMaxZ = -1f;
            public float mMinZ = -1f;

            public int RepeatUVType = 0;
            public bool isNotCenterMode = true;

            //End objects:
            public string EndCapStartString = "";
            public string EndCapEndString = "";
            public bool isEndCapCustomMatchStart = true;
            public Vector3 EndCapCustomOffsetStart = default(Vector3);
            public Vector3 EndCapCustomOffsetEnd = default(Vector3);
            public Vector3 EndCapCustomRotOffsetStart = default(Vector3);
            public Vector3 EndCapCustomRotOffsetEnd = default(Vector3);
            public bool isEndObjectsMatchingGround = false;
            //Endings down:
            public bool isStartDown = false;
            public bool isStartTypeDownOverriden = false;
            public float StartTypeDownOverride = 0f;
            public bool isEndDown = false;
            public bool isEndTypeDownOverriden = false;
            public float EndTypeDownOverride = 0f;

            //Collision:
            public int CollisionType = 0;
            public bool isCollisionConvex = false;
            public bool isSimpleCollisionAutomatic = true;
            public bool isCollisionTrigger = false;

            public Vector3 CollisionBoxBL = default(Vector3);
            public Vector3 CollisionBoxBR = default(Vector3);
            public Vector3 CollisionBoxTL = default(Vector3);
            public Vector3 CollisionBoxTR = default(Vector3);

            public Vector3 CollisionTriBL = default(Vector3);
            public Vector3 CollisionTriBR = default(Vector3);
            public Vector3 CollisionTriT = default(Vector3);

            public string objectName = "ExtrudedObject";
            public string thumbString = "";
            public string desc = "";
            public string displayName = "";
            #endregion


            /// <summary> Setup using _SMM </summary>
            public void Setup(SplinatedMeshMaker _SMM)
            {
                CurrentSplinationString = _SMM.currentSplinationString;
                if (_SMM.currentSplinationCap1 == null)
                {
                    CurrentSplinationCap1String = "";
                }
                else
                {
                    CurrentSplinationCap1String = _SMM.currentSplinationCap1String;
                }

                if (_SMM.currentSplinationCap2 == null)
                {
                    CurrentSplinationCap2String = "";
                }
                else
                {
                    CurrentSplinationCap2String = _SMM.currentSplinationCap2String;
                }
                isDefault = _SMM.isDefault;

                CapHeightOffset1 = _SMM.capHeightOffset1;
                CapHeightOffset2 = _SMM.capHeightOffset2;

                isMaterialOverriden = _SMM.isMaterialOverriden;
                SplinatedMaterial1String = _SMM.SplinatedMaterial1String;
                SplinatedMaterial2String = _SMM.SplinatedMaterial2String;
                isExactSplination = _SMM.isExactSplination;
                isMatchingRoadDefinition = _SMM.isMatchingRoadDefinition;
                isMatchingRoadIncrements = _SMM.isMatchingRoadIncrements;
                isTrimStart = _SMM.isTrimStart;
                isTrimEnd = _SMM.isTrimEnd;
                isMatchingTerrain = _SMM.isMatchingTerrain;
                MinMaxMod = _SMM.minMaxMod;
                isBridge = _SMM.isBridge;
                VertexMatchingPrecision = _SMM.vertexMatchingPrecision;

                isStretched = _SMM.isStretched;
                isStretchedLocOffset = _SMM.isStretchedOffset;
                isStretchedSize = _SMM.isStretchedSize;
                boxColliderOffset = _SMM.boxColliderOffset;
                boxColliderSize = _SMM.boxColliderSize;
                stretchUVThreshold = _SMM.stretchUVThreshold;
                isStraightLineMatchingStartEnd = _SMM.isStraightLineMatchStartEnd;
                isBoxColliderFlippedOnX = _SMM.isBoxColliderFlippedOnX;
                isBoxColliderFlippedOnZ = _SMM.isBoxColliderFlippedOnZ;

                //Horizontal offsets:
                horizontalSep = _SMM.horizontalSep;
                horizontalCurve = _SMM.horizontalCurve;
                //Vertical offset:
                verticalRaise = _SMM.verticalRaise;
                verticalCurve = _SMM.verticalCurve;
                //Vertical cutoff
                VerticalCutoff = _SMM.VerticalCutoff;
                isVerticalCutoff = _SMM.isVerticalCutoff;
                isVerticalCutoffDownwards = _SMM.isVerticalCutoffDownwards;
                isVerticalMeshCutoffOppositeDir = _SMM.isVerticalMeshCutoffOppositeDir;
                VerticalMeshCutoffOffset = _SMM.VerticalMeshCutoffOffset;
                isVerticalCutoffMatchingZero = _SMM.isVerticalCutoffMatchingZero;

                RoadRaise = _SMM.RoadRaise;
                CustomRotation = _SMM.CustomRotation;
                isFlippedRotation = _SMM.isFlippingRotation;
                isStatic = _SMM.isStatic;
                StartTime = _SMM.StartTime;
                EndTime = _SMM.EndTime;
                Axis = (int) _SMM.Axis;

                RepeatUVType = (int) _SMM.RepeatUVType;

                mMaxX = _SMM.mMaxX;
                mMinX = _SMM.mMinX;
                mMaxY = _SMM.mMaxY;
                mMinY = _SMM.mMinY;
                mMaxZ = _SMM.mMaxZ;
                mMinZ = _SMM.mMinZ;

                //End objects:
                if (_SMM.EndCapStart == null)
                {
                    EndCapStartString = "";
                }
                else
                {
                    EndCapStartString = _SMM.EndCapStartString;
                }
                if (_SMM.EndCapEnd == null)
                {
                    EndCapEndString = "";
                }
                else
                {
                    EndCapEndString = _SMM.EndCapEndString;
                }
                isEndCapCustomMatchStart = _SMM.isEndCapCustomMatchStart;
                EndCapCustomOffsetStart = _SMM.EndCapCustomOffsetStart;
                EndCapCustomOffsetEnd = _SMM.EndCapCustomOffsetEnd;
                EndCapCustomRotOffsetStart = _SMM.EndCapCustomRotOffsetStart;
                EndCapCustomRotOffsetEnd = _SMM.EndCapCustomRotOffsetEnd;
                isEndObjectsMatchingGround = _SMM.isEndObjectsMatchingGround;
                //Endings down:
                isStartDown = _SMM.isStartDown;
                isStartTypeDownOverriden = _SMM.isStartTypeDownOverriden;
                StartTypeDownOverride = _SMM.startTypeDownOverride;
                isEndDown = _SMM.isEndDown;
                isEndTypeDownOverriden = _SMM.isEndTypeDownOverriden;
                EndTypeDownOverride = _SMM.endTypeDownOverride;

                //Collision:
                CollisionType = (int) _SMM.CollisionType;
                isCollisionConvex = _SMM.isCollisionConvex;
                isSimpleCollisionAutomatic = _SMM.isSimpleCollisionAutomatic;
                isCollisionTrigger = _SMM.isCollisionTrigger;

                CollisionBoxBL = _SMM.CollisionBoxBL;
                CollisionBoxBR = _SMM.CollisionBoxBR;
                CollisionBoxTL = _SMM.CollisionBoxTL;
                CollisionBoxTR = _SMM.CollisionBoxTR;

                CollisionTriBL = _SMM.CollisionTriBL;
                CollisionTriBR = _SMM.CollisionTriBR;
                CollisionTriT = _SMM.CollisionTriT;

                objectName = _SMM.objectName;
                thumbString = _SMM.thumbString;
                desc = _SMM.desc;
                displayName = _SMM.displayName;
            }


            /// <summary> Copy relevant attributes to _SMM </summary>
            public void LoadToSMM(SplinatedMeshMaker _SMM)
            {
                _SMM.currentSplinationString = CurrentSplinationString;
                _SMM.currentSplinationCap1String = CurrentSplinationCap1String;
                _SMM.currentSplinationCap2String = CurrentSplinationCap2String;


                _SMM.capHeightOffset1 = CapHeightOffset1;
                _SMM.capHeightOffset2 = CapHeightOffset2;

                _SMM.isMaterialOverriden = isMaterialOverriden;
                _SMM.SplinatedMaterial1String = SplinatedMaterial1String;
                _SMM.SplinatedMaterial2String = SplinatedMaterial2String;


                _SMM.currentSplination = EngineIntegration.LoadAssetFromPath<GameObject>(CurrentSplinationString);
                if (CurrentSplinationCap1String != null && CurrentSplinationCap1String.Length > 1)
                {
                    _SMM.currentSplinationCap1 = EngineIntegration.LoadAssetFromPath<GameObject>(CurrentSplinationCap1String);
                }
                if (CurrentSplinationCap2String != null && CurrentSplinationCap2String.Length > 1)
                {
                    _SMM.currentSplinationCap2 = EngineIntegration.LoadAssetFromPath<GameObject>(CurrentSplinationCap2String);
                }
                if (isMaterialOverriden)
                {
                    if (SplinatedMaterial1String != null && SplinatedMaterial1String.Length > 0)
                    {
                        _SMM.SplinatedMaterial1 = EngineIntegration.LoadAssetFromPath<Material>(SplinatedMaterial1String);
                    }
                    if (SplinatedMaterial2String != null && SplinatedMaterial2String.Length > 0)
                    {
                        _SMM.SplinatedMaterial2 = EngineIntegration.LoadAssetFromPath<Material>(SplinatedMaterial2String);
                    }
                }


                _SMM.isDefault = isDefault;
                _SMM.isExactSplination = isExactSplination;
                _SMM.isMatchingRoadDefinition = isMatchingRoadDefinition;
                _SMM.isMatchingRoadIncrements = isMatchingRoadIncrements;
                _SMM.isTrimStart = isTrimStart;
                _SMM.isTrimEnd = isTrimEnd;
                _SMM.isMatchingTerrain = isMatchingTerrain;
                _SMM.minMaxMod = MinMaxMod;
                _SMM.isBridge = isBridge;
                _SMM.vertexMatchingPrecision = VertexMatchingPrecision;

                _SMM.isStretched = isStretched;
                _SMM.isStretchedOffset = isStretchedLocOffset;
                _SMM.isStretchedSize = isStretchedSize;
                _SMM.boxColliderOffset = boxColliderOffset;
                _SMM.boxColliderSize = boxColliderSize;
                _SMM.stretchUVThreshold = stretchUVThreshold;
                _SMM.isStraightLineMatchStartEnd = isStraightLineMatchingStartEnd;
                _SMM.isBoxColliderFlippedOnX = isBoxColliderFlippedOnX;
                _SMM.isBoxColliderFlippedOnZ = isBoxColliderFlippedOnZ;

                //Horizontal offsets:
                _SMM.horizontalSep = horizontalSep;
                _SMM.horizontalCurve = horizontalCurve;
                //Vertical offset:
                _SMM.verticalRaise = verticalRaise;
                _SMM.verticalCurve = verticalCurve;
                //Vertical cutoff:
                _SMM.VerticalCutoff = VerticalCutoff;
                _SMM.isVerticalCutoff = isVerticalCutoff;
                _SMM.isVerticalCutoffDownwards = isVerticalCutoffDownwards;
                _SMM.isVerticalMeshCutoffOppositeDir = isVerticalMeshCutoffOppositeDir;
                _SMM.VerticalMeshCutoffOffset = VerticalMeshCutoffOffset;
                _SMM.isVerticalCutoffMatchingZero = isVerticalCutoffMatchingZero;

                _SMM.RoadRaise = RoadRaise;
                _SMM.CustomRotation = CustomRotation;
                _SMM.isFlippingRotation = isFlippedRotation;
                _SMM.isStatic = isStatic;
                _SMM.StartTime = StartTime;
                _SMM.EndTime = EndTime;
                _SMM.Axis = (AxisTypeEnum) Axis;

                _SMM.RepeatUVType = (RepeatUVTypeEnum) RepeatUVType;

                _SMM.mMaxX = mMaxX;
                _SMM.mMinX = mMinX;
                _SMM.mMaxY = mMaxY;
                _SMM.mMinY = mMinY;
                _SMM.mMaxZ = mMaxZ;
                _SMM.mMinZ = mMinZ;

                //Ending objects:
                _SMM.EndCapStartString = EndCapStartString;
                _SMM.EndCapEndString = EndCapEndString;

                if (EndCapStartString != null && EndCapStartString.Length > 0)
                {
                    _SMM.EndCapStart = EngineIntegration.LoadAssetFromPath<GameObject>(EndCapStartString);
                }
                if (EndCapEndString != null && EndCapEndString.Length > 0)
                {
                    _SMM.EndCapEnd = EngineIntegration.LoadAssetFromPath<GameObject>(EndCapEndString);
                }

                _SMM.isEndCapCustomMatchStart = isEndCapCustomMatchStart;
                _SMM.EndCapCustomOffsetStart = EndCapCustomOffsetStart;
                _SMM.EndCapCustomOffsetEnd = EndCapCustomOffsetEnd;
                _SMM.EndCapCustomRotOffsetStart = EndCapCustomRotOffsetStart;
                _SMM.EndCapCustomRotOffsetEnd = EndCapCustomRotOffsetEnd;
                _SMM.isEndObjectsMatchingGround = isEndObjectsMatchingGround;

                //Endings down:
                _SMM.isStartDown = isStartDown;
                _SMM.isStartTypeDownOverriden = isStartTypeDownOverriden;
                _SMM.startTypeDownOverride = StartTypeDownOverride;
                _SMM.isEndDown = isEndDown;
                _SMM.isEndTypeDownOverriden = isEndTypeDownOverriden;
                _SMM.endTypeDownOverride = EndTypeDownOverride;

                //Collision:
                _SMM.CollisionType = (CollisionTypeEnum) CollisionType;
                _SMM.isCollisionConvex = isCollisionConvex;
                _SMM.isSimpleCollisionAutomatic = isSimpleCollisionAutomatic;
                _SMM.isCollisionTrigger = isCollisionTrigger;

                _SMM.CollisionBoxBL = CollisionBoxBL;
                _SMM.CollisionBoxBR = CollisionBoxBR;
                _SMM.CollisionBoxTL = CollisionBoxTL;
                _SMM.CollisionBoxTR = CollisionBoxTR;

                _SMM.CollisionTriBL = CollisionTriBL;
                _SMM.CollisionTriBR = CollisionTriBR;
                _SMM.CollisionTriT = CollisionTriT;

                _SMM.objectName = objectName;
                _SMM.thumbString = thumbString;
                _SMM.desc = desc;
                _SMM.displayName = displayName;
            }
        }


        #region "Static util"
        public static Vector3 GetVector3Average(Vector3[] _vectors)
        {
            int tCount = _vectors.Length;
            Vector3 mVect = default(Vector3);
            for (int index = 0; index < tCount; index++)
            {
                mVect += _vectors[index];
            }
            mVect /= tCount;
            return mVect;
        }


        private static bool FloatsNear(float _near, float _val1, float _val2)
        {
            if (RootUtils.IsApproximately(_val1, _val2, _near))
            {
                return true;
            }

            if (_val1 < (_val2 + _near) && _val1 > (_val2 - _near))
            {
                return true;
            }
            if (_val2 < (_val1 + _near) && _val2 > (_val1 - _near))
            {
                return true;
            }
            return false;
        }


        /// <summary> Returns triangles for CollisionType SimpleMeshTriangle </summary>
        private static int[] GetCollisionTrisTri(int _meshCount, int _cTriCount, int _cCount)
        {
            int tCounter = 0;
            int[] tTris = new int[_cTriCount * 3];

            //Front side: **
            tTris[tCounter] = 0;
            tCounter += 1;
            tTris[tCounter] = 2;
            tCounter += 1;
            tTris[tCounter] = 1;
            tCounter += 1;
            int tMod = -1;
            for (int index = 0; index < (_meshCount); index++)
            {
                tMod = (index * 3);
                //Bottom side: ***
                tTris[tCounter] = 1 + tMod;
                tCounter += 1;
                tTris[tCounter] = 4 + tMod;
                tCounter += 1;
                tTris[tCounter] = 0 + tMod;
                tCounter += 1;
                tTris[tCounter] = 4 + tMod;
                tCounter += 1;
                tTris[tCounter] = 3 + tMod;
                tCounter += 1;
                tTris[tCounter] = 0 + tMod;
                tCounter += 1;
                //Left side: ***
                tTris[tCounter] = 3 + tMod;
                tCounter += 1;
                tTris[tCounter] = 5 + tMod;
                tCounter += 1;
                tTris[tCounter] = 0 + tMod;
                tCounter += 1;
                tTris[tCounter] = 5 + tMod;
                tCounter += 1;
                tTris[tCounter] = 2 + tMod;
                tCounter += 1;
                tTris[tCounter] = 0 + tMod;
                tCounter += 1;
                //Right side: ***
                tTris[tCounter] = 1 + tMod;
                tCounter += 1;
                tTris[tCounter] = 2 + tMod;
                tCounter += 1;
                tTris[tCounter] = 4 + tMod;
                tCounter += 1;
                tTris[tCounter] = 2 + tMod;
                tCounter += 1;
                tTris[tCounter] = 5 + tMod;
                tCounter += 1;
                tTris[tCounter] = 4 + tMod;
                tCounter += 1;
            }
            //Back side: **
            tTris[tCounter] = _cCount - 2;
            tCounter += 1;
            tTris[tCounter] = _cCount - 1;
            tCounter += 1;
            tTris[tCounter] = _cCount - 3;
            tCounter += 1;

            return tTris;
        }


        /// <summary> Returns triangles for CollisionType SimpleMeshTrapezoid </summary>
        private static int[] GetCollisionTrisBox(int _meshCount, int _cTriCount, int _cCount)
        {
            int tCounter = 0;
            int[] tTris = new int[_cTriCount * 3];

            //Front side: ***
            tTris[tCounter] = 0;
            tCounter += 1;
            tTris[tCounter] = 2;
            tCounter += 1;
            tTris[tCounter] = 1;
            tCounter += 1;
            tTris[tCounter] = 2;
            tCounter += 1;
            tTris[tCounter] = 3;
            tCounter += 1;
            tTris[tCounter] = 1;
            tCounter += 1;

            int tMod = -1;
            for (int index = 0; index < (_meshCount); index++)
            {
                tMod = (index * 4);
                //Bottom side: ***
                tTris[tCounter] = tMod + 1;
                tCounter += 1;
                tTris[tCounter] = tMod + 5;
                tCounter += 1;
                tTris[tCounter] = tMod + 0;
                tCounter += 1;
                tTris[tCounter] = tMod + 5;
                tCounter += 1;
                tTris[tCounter] = tMod + 4;
                tCounter += 1;
                tTris[tCounter] = tMod + 0;
                tCounter += 1;
                //Top side: ***
                tTris[tCounter] = tMod + 2;
                tCounter += 1;
                tTris[tCounter] = tMod + 6;
                tCounter += 1;
                tTris[tCounter] = tMod + 3;
                tCounter += 1;
                tTris[tCounter] = tMod + 6;
                tCounter += 1;
                tTris[tCounter] = tMod + 7;
                tCounter += 1;
                tTris[tCounter] = tMod + 3;
                tCounter += 1;
                //Left side: ***
                tTris[tCounter] = tMod + 4;
                tCounter += 1;
                tTris[tCounter] = tMod + 6;
                tCounter += 1;
                tTris[tCounter] = tMod + 0;
                tCounter += 1;
                tTris[tCounter] = tMod + 6;
                tCounter += 1;
                tTris[tCounter] = tMod + 2;
                tCounter += 1;
                tTris[tCounter] = tMod + 0;
                tCounter += 1;
                //Right side: ***
                tTris[tCounter] = tMod + 1;
                tCounter += 1;
                tTris[tCounter] = tMod + 3;
                tCounter += 1;
                tTris[tCounter] = tMod + 5;
                tCounter += 1;
                tTris[tCounter] = tMod + 3;
                tCounter += 1;
                tTris[tCounter] = tMod + 7;
                tCounter += 1;
                tTris[tCounter] = tMod + 5;
                tCounter += 1;
            }

            //Back side: ***
            tTris[tCounter] = _cCount - 3;
            tCounter += 1;
            tTris[tCounter] = _cCount - 1;
            tCounter += 1;
            tTris[tCounter] = _cCount - 4;
            tCounter += 1;
            tTris[tCounter] = _cCount - 1;
            tCounter += 1;
            tTris[tCounter] = _cCount - 2;
            tCounter += 1;
            tTris[tCounter] = _cCount - 4;
            tCounter += 1;

            return tTris;
        }


        /// <summary> Returns true if two coordinates of Vector _v1 and _v2 match </summary>
        private static bool IsApproxTwoThirds(ref Vector3 _v1, Vector3 _v2, float _precision = 0.005f)
        {
            int cCount = 0;
            if (RootUtils.IsApproximately(_v1.x, _v2.x, _precision))
            {
                cCount += 1;
            }
            if (RootUtils.IsApproximately(_v1.y, _v2.y, _precision))
            {
                cCount += 1;
            }
            if (RootUtils.IsApproximately(_v1.z, _v2.z, _precision))
            {
                cCount += 1;
            }

            if (cCount == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private static bool IsApproxWithNeg(ref Vector3 _v1, ref Vector3 _v2)
        {
            int cCount = 0;
            bool bXMatch = false;
            bool bYMatch = false;
            bool bZMatch = false;

            if (RootUtils.IsApproximately(_v1.x, _v2.x, 0.02f))
            {
                cCount += 1;
                bXMatch = true;
            }
            if (RootUtils.IsApproximately(_v1.y, _v2.y, 0.02f))
            {
                cCount += 1;
                bYMatch = true;
            }
            if (RootUtils.IsApproximately(_v1.z, _v2.z, 0.02f))
            {
                cCount += 1;
                bZMatch = true;
            }

            if (cCount == 2)
            {
                if (!bXMatch && RootUtils.IsApproximately(_v1.x, _v2.x * -1f, 0.02f))
                {
                    return true;
                }
                else if (!bYMatch && RootUtils.IsApproximately(_v1.y, _v2.y * -1f, 0.02f))
                {
                    return true;
                }
                else if (!bZMatch && RootUtils.IsApproximately(_v1.z, _v2.z * -1f, 0.02f))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary> Returns true if all coordinates of _v1 are 0f </summary>
        private static bool V3EqualToNone(Vector3 _v1)
        {
            if (!RootUtils.IsApproximately(_v1.x, 0f, 0.0001f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.y, 0f, 0.0001f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.z, 0f, 0.0001f))
            {
                return false;
            }
            return true;
        }


        /// <summary> Returns true if coordinates of _v1 and _v2 are equal with 0.01f tolerance </summary>
        private static bool V3EqualNormal(Vector3 _v1, Vector3 _v2)
        {
            if (!RootUtils.IsApproximately(_v1.x, _v2.x, 0.01f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.y, _v2.y, 0.01f))
            {
                return false;
            }
            if (!RootUtils.IsApproximately(_v1.z, _v2.z, 0.01f))
            {
                return false;
            }
            return true;
        }


        /// <summary> Returns true if coordinates of _v1 and _v2 do not match with tolerance 0.02f; _isZAxis checks x coordinate instead of z </summary>
        private static bool IsApproxExtruded(ref Vector3 _v1, ref Vector3 _v2, bool _isZAxis)
        {
            if (!RootUtils.IsApproximately(_v1.y, _v2.y, 0.02f))
            {
                return false;
            }

            if (_isZAxis)
            {
                if (!RootUtils.IsApproximately(_v1.x, _v2.x, 0.02f))
                {
                    return false;
                }
            }
            else
            {
                if (!RootUtils.IsApproximately(_v1.z, _v2.z, 0.02f))
                {
                    return false;
                }
            }

            return true;
        }


        private static float GetVHeightAtXY(ref Vector3 _vect1, ref Vector3 _vect2, ref Vector3 _vect3)
        {
            Vector2 tVect2D1 = new Vector2(_vect1.x, _vect1.z);
            Vector2 tVect2D2 = new Vector2(_vect2.x, _vect2.z);
            Vector2 tVect2D3 = new Vector2(_vect3.x, _vect3.z);

            float tDist1 = Vector2.Distance(tVect2D1, tVect2D3);
            float tDist2 = Vector2.Distance(tVect2D2, tVect2D3);
            float tDistSum = tDist1 + tDist2;

            float CloseTo1 = (tDist1 / tDistSum);

            Vector3 tVect = ((_vect2 - _vect1) * CloseTo1) + _vect1;

            return tVect.y;
        }
        #endregion


        public void Setup(bool _isGettingStrings = false, bool _isCollecting = true)
        {
            GameObject[] objects = new GameObject[5];
            try
            {
                SplinateMeshDo(_isGettingStrings, ref objects, _isCollecting);
            }
            catch (System.Exception exception)
            {
                if (objects != null)
                {
                    for (int index = 0; index < 5; index++)
                    {
                        Object.DestroyImmediate(objects[index]);
                    }
                }
                throw exception;
            }
        }


        private void SplinateMeshDo(bool _isGettingStrings, ref GameObject[] _errorObj, bool _isCollecting)
        {
            isRequiringUpdate = false;
            RootUtils.SetupUniqueIdentifier(ref uID);

            //Buffers:
            Vector3 tVect1 = default(Vector3);
            Vector3 tVect2 = default(Vector3);
            Vector3 tDir = default(Vector3);
            Vector3 xVect = default(Vector3);
            float tFloat1 = default(float);

            StartTime = spline.GetClosestParam(StartPos);
            EndTime = spline.GetClosestParam(EndPos);

            if (EndTime < StartTime)
            {
                EndTime = node.nextTime;
                EndPos = spline.GetSplineValue(EndTime, false);
            }
            if (EndTime > 0.99995f)
            {
                EndTime = 0.99995f;
                EndPos = spline.GetSplineValue(EndTime, false);
            }

            Kill();
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


            //Setup strings:
            if (_isGettingStrings)
            {
                currentSplinationString = RootUtils.GetPrefabString(currentSplination);
                if (currentSplinationCap1 != null)
                {
                    currentSplinationCap1String = RootUtils.GetPrefabString(currentSplinationCap1);
                }
                if (currentSplinationCap2 != null)
                {
                    currentSplinationCap2String = RootUtils.GetPrefabString(currentSplinationCap2);
                }
                if (EndCapStart != null)
                {
                    EndCapStartString = RootUtils.GetPrefabString(EndCapStart);
                }
                if (EndCapEnd != null)
                {
                    EndCapEndString = RootUtils.GetPrefabString(EndCapEnd);
                }

                if (SplinatedMaterial1 != null)
                {
                    SplinatedMaterial1String = EngineIntegration.GetAssetPath(SplinatedMaterial1);
                }
                if (SplinatedMaterial2 != null)
                {
                    SplinatedMaterial2String = EngineIntegration.GetAssetPath(SplinatedMaterial2);
                }
            }

            if (currentSplination == null)
            {
                return;
            }
            GameObject tObj = (GameObject) GameObject.Instantiate(currentSplination);
            _errorObj[0] = tObj;

            GameObject EndCapStartObj = null;
            GameObject EndCapEndObj = null;
            if (EndCapStart != null)
            {
                EndCapStartObj = (GameObject) GameObject.Instantiate(EndCapStart);
                _errorObj[1] = EndCapStartObj;
            }
            if (EndCapEnd != null)
            {
                EndCapEndObj = (GameObject) GameObject.Instantiate(EndCapEnd);
                _errorObj[2] = EndCapEndObj;
            }

            GameObject Cap1 = null;
            GameObject Cap2 = null;
            if (isFlippingRotation)
            {
                if (currentSplinationCap2 != null)
                {
                    Cap1 = (GameObject) GameObject.Instantiate(currentSplinationCap2);
                    _errorObj[3] = Cap1;
                }
                if (currentSplinationCap1 != null)
                {
                    Cap2 = (GameObject) GameObject.Instantiate(currentSplinationCap1);
                    _errorObj[4] = Cap2;
                }
            }
            else
            {
                if (currentSplinationCap1 != null)
                {
                    Cap1 = (GameObject) GameObject.Instantiate(currentSplinationCap1);
                    _errorObj[3] = Cap1;
                }
                if (currentSplinationCap2 != null)
                {
                    Cap2 = (GameObject) GameObject.Instantiate(currentSplinationCap2);
                    _errorObj[4] = Cap2;
                }
            }

            MeshFilter MF = null;
            Mesh CapMesh1 = null;
            Mesh CapMesh2 = null;
            HashSet<int> tCapMatchIndices1 = new HashSet<int>();
            HashSet<int> tCapMatchIndices2 = new HashSet<int>();
            if (Cap1 != null)
            {
                MF = Cap1.GetComponent<MeshFilter>();
                CapMesh1 = MF.sharedMesh;
            }
            if (Cap2 != null)
            {
                MF = Cap2.GetComponent<MeshFilter>();
                CapMesh2 = MF.sharedMesh;
            }

            MF = tObj.GetComponent<MeshFilter>();
            mesh = MF.sharedMesh;

            //			Quaternion OrigRot = tObj.transform.rotation;
            if (isFlippingRotation)
            {
                tVect1 = new Vector3(0f, 180f, 0f);
                tObj.transform.Rotate(tVect1, Space.World);
                if (Cap1 != null)
                {
                    Cap1.transform.Rotate(tVect1, Space.World);
                }
                if (Cap2 != null)
                {
                    Cap2.transform.Rotate(tVect1, Space.World);
                }
            }
            tObj.transform.Rotate(CustomRotation, Space.World);
            if (Cap1 != null)
            {
                Cap1.transform.Rotate(CustomRotation, Space.World);
            }
            if (Cap2 != null)
            {
                Cap2.transform.Rotate(CustomRotation, Space.World);
            }

            if (mesh == null)
            {
                GameObject.DestroyImmediate(tObj);
                Debug.LogError("Mesh was null");
                return;
            }

            Vector3[] CapOrigVerts1 = null;
            Vector3[] CapOrigVerts2 = null;
            int CapOrigMVL1 = 0;
            int CapOrigMVL2 = 0;
            if (CapMesh1 != null)
            {
                CapOrigVerts1 = CapMesh1.vertices;
                CapOrigMVL1 = CapOrigVerts1.Length;
            }
            if (CapMesh2 != null)
            {
                CapOrigVerts2 = CapMesh2.vertices;
                CapOrigMVL2 = CapOrigVerts2.Length;
            }

            Vector3[] OrigVerts = mesh.vertices;
            int OrigMVL = OrigVerts.Length;

            //Transform vertices:
            Vector3[] OrigNormals = mesh.normals;
            bool bCheckingNormal = true;
            for (int index = 0; index < OrigMVL; index++)
            {
                OrigVerts[index] = tObj.transform.TransformPoint(OrigVerts[index]);
                if (bCheckingNormal)
                {
                    if (!V3EqualToNone(OrigNormals[index]))
                    {
                        bCheckingNormal = false;
                    }
                }
            }
            //If no normals on base mesh, recalc them
            if (bCheckingNormal)
            {
                mesh.RecalculateNormals();
                OrigNormals = mesh.normals;
            }
            //Cap mesh:
            Vector3[] CapOrigNormals1 = null;
            Vector3[] CapOrigNormals2 = null;
            int[] CapOrigTris1 = null;
            int[] CapOrigTris2 = null;
            Vector2[] CapOrigUV1 = null;
            Vector2[] CapOrigUV2 = null;
            int CapTriCount1 = 0;
            int CapTriCount2 = 0;
            if (CapMesh1 != null)
            {
                for (int index = 0; index < CapOrigMVL1; index++)
                {
                    CapOrigVerts1[index] = Cap1.transform.TransformPoint(CapOrigVerts1[index]);
                }

                float[] oMinMaxX = new float[CapOrigMVL1];
                float[] oMinMaxY = new float[CapOrigMVL1];
                float[] oMinMaxZ = new float[CapOrigMVL1];
                for (int i = 0; i < CapOrigMVL1; i++)
                {
                    oMinMaxX[i] = CapOrigVerts1[i].x;
                    oMinMaxY[i] = CapOrigVerts1[i].y;
                    oMinMaxZ[i] = CapOrigVerts1[i].z;
                }
                //				float oMinX = Mathf.Min(oMinMaxX);
                float oMaxX = Mathf.Max(oMinMaxX);
                //				float oMinY = Mathf.Min(oMinMaxY);
                //				float oMaxY = Mathf.Max(oMinMaxY);
                //				float oMinZ = Mathf.Min(oMinMaxZ);
                float oMaxZ = Mathf.Max(oMinMaxZ);

                for (int index = 0; index < CapOrigMVL1; index++)
                {
                    if (Axis == AxisTypeEnum.Z)
                    {
                        if (RootUtils.IsApproximately(CapOrigVerts1[index].z, oMaxZ, minMaxMod))
                        {
                            tCapMatchIndices1.Add(index);
                        }
                    }
                    else
                    {
                        if (RootUtils.IsApproximately(CapOrigVerts1[index].x, oMaxX, minMaxMod))
                        {
                            tCapMatchIndices1.Add(index);
                        }
                    }
                }

                CapMesh1.RecalculateNormals();
                CapOrigTris1 = CapMesh1.triangles;
                CapOrigUV1 = CapMesh1.uv;
                CapTriCount1 = CapOrigTris1.Length;
                CapOrigNormals1 = CapMesh1.normals;
            }
            if (CapMesh2 != null)
            {
                for (int index = 0; index < CapOrigMVL2; index++)
                {
                    CapOrigVerts2[index] = Cap2.transform.TransformPoint(CapOrigVerts2[index]);
                }

                float[] oMinMaxX = new float[CapOrigMVL2];
                float[] oMinMaxY = new float[CapOrigMVL2];
                float[] oMinMaxZ = new float[CapOrigMVL2];
                for (int index = 0; index < CapOrigMVL2; index++)
                {
                    oMinMaxX[index] = CapOrigVerts2[index].x;
                    oMinMaxY[index] = CapOrigVerts2[index].y;
                    oMinMaxZ[index] = CapOrigVerts2[index].z;
                }
                float oMinX = Mathf.Min(oMinMaxX);
                //				float oMaxX = Mathf.Max(oMinMaxX);
                //				float oMinY = Mathf.Min(oMinMaxY);
                //				float oMaxY = Mathf.Max(oMinMaxY);
                float oMinZ = Mathf.Min(oMinMaxZ);
                //				float oMaxZ = Mathf.Max(oMinMaxZ);

                for (int index = 0; index < CapOrigMVL2; index++)
                {
                    if (Axis == AxisTypeEnum.Z)
                    {
                        if (RootUtils.IsApproximately(CapOrigVerts2[index].z, oMinZ, minMaxMod))
                        {
                            tCapMatchIndices2.Add(index);
                        }
                    }
                    else
                    {
                        if (RootUtils.IsApproximately(CapOrigVerts2[index].x, oMinX, minMaxMod))
                        {
                            tCapMatchIndices2.Add(index);
                        }
                    }
                }

                CapMesh2.RecalculateNormals();
                CapOrigTris2 = CapMesh2.triangles;
                CapOrigUV2 = CapMesh2.uv;
                CapTriCount2 = CapOrigTris2.Length;
                CapOrigNormals2 = CapMesh2.normals;
            }

            int[] OrigTris = mesh.triangles;
            int OrigTriCount = OrigTris.Length;
            Vector2[] OrigUV = mesh.uv;
            float[] tMinMax = new float[OrigMVL];
            float[] tMinMaxX = new float[OrigMVL];
            float[] tMinMaxY = new float[OrigMVL];
            float[] tMinMaxZ = new float[OrigMVL];
            float[] tMinMaxUV = null;
            if (RepeatUVType != RepeatUVTypeEnum.None)
            {
                tMinMaxUV = new float[OrigMVL];
            }
            for (int i = 0; i < OrigMVL; i++)
            {
                if (Axis == AxisTypeEnum.X)
                {
                    tMinMax[i] = OrigVerts[i].x;
                }
                else
                {
                    tMinMax[i] = OrigVerts[i].z;
                }
                tMinMaxX[i] = OrigVerts[i].x;
                tMinMaxY[i] = OrigVerts[i].y;
                tMinMaxZ[i] = OrigVerts[i].z;
                if (RepeatUVType == RepeatUVTypeEnum.X)
                {
                    tMinMaxUV[i] = OrigUV[i].x;
                }
                else if (RepeatUVType == RepeatUVTypeEnum.Y)
                {
                    tMinMaxUV[i] = OrigUV[i].y;
                }
            }

            float mMax = Mathf.Max(tMinMax);
            float mMin = Mathf.Min(tMinMax);
            float mMaxX = Mathf.Max(tMinMaxX);
            float mMinX = Mathf.Min(tMinMaxX);
            float mMaxY = Mathf.Max(tMinMaxY);
            float mMinY = Mathf.Min(tMinMaxY);
            float mMaxZ = Mathf.Max(tMinMaxZ);
            float mMinZ = Mathf.Min(tMinMaxZ);
            float mMinUV = -1f;
            float mMaxUV = -1f;
            float mUVDiff = -1f;
            if (RepeatUVType != RepeatUVTypeEnum.None)
            {
                mMinUV = Mathf.Min(tMinMaxUV);
                mMaxUV = Mathf.Max(tMinMaxUV);
                mUVDiff = mMaxUV - mMinUV;
            }
            float mMaxDiff = mMax - mMin;
            float mMaxHeight = mMaxY - mMinY;
            float mMaxThreshold = mMax - minMaxMod;
            float mMinThreshold = mMin + minMaxMod;
            List<int> MinVectorIndices = new List<int>();
            List<int> MaxVectorIndices = new List<int>();
            List<int> MiddleVectorIndicies = new List<int>();
            float tBuffer = 0f;
            for (int index = 0; index < OrigMVL; index++)
            {
                if (Axis == AxisTypeEnum.X)
                {
                    tBuffer = OrigVerts[index].x;
                }
                else
                {
                    tBuffer = OrigVerts[index].z;
                }

                if (tBuffer > mMaxThreshold)
                {
                    MaxVectorIndices.Add(index);
                }
                else if (tBuffer < mMinThreshold)
                {
                    MinVectorIndices.Add(index);
                }
                else
                {
                    MiddleVectorIndicies.Add(index);
                }
            }
            int MiddleCount = MiddleVectorIndicies.Count;

            //Match up min/max vertices:
            Dictionary<int, int> MatchingIndices = new Dictionary<int, int>();
            Dictionary<int, int> MatchingIndices_Min = new Dictionary<int, int>();
            Dictionary<int, List<int>> MatchingIndices_Min_Cap = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> MatchingIndices_Max_Cap = new Dictionary<int, List<int>>();
            int tCount1 = MaxVectorIndices.Count;
            int tCount2 = MinVectorIndices.Count;
            int tIntBuffer1 = -1;
            int tIntBuffer2 = -1;
            int tIntBuffer3 = -1;
            int tIntBuffer4 = -1;
            //			Dictionary<int,float> UVStep = null;
            //			if(RepeatUVType != RepeatUVTypeEnum.None){
            //				UVStep = new Dictionary<int, float>();
            //			}
            List<int> AlreadyAddedList = new List<int>();
            for (int index = 0; index < tCount1; index++)
            {
                tIntBuffer1 = MaxVectorIndices[index];
                tVect1 = OrigVerts[tIntBuffer1];

                bool bAdded = false;
                for (int j = 0; j < OrigTriCount; j += 3)
                {
                    if (OrigTris[j] == tIntBuffer1)
                    {
                        tIntBuffer3 = OrigTris[j + 1];
                        tIntBuffer4 = OrigTris[j + 2];
                    }
                    else if (OrigTris[j + 1] == tIntBuffer1)
                    {
                        tIntBuffer3 = OrigTris[j];
                        tIntBuffer4 = OrigTris[j + 2];
                    }
                    else if (OrigTris[j + 2] == tIntBuffer1)
                    {
                        tIntBuffer3 = OrigTris[j];
                        tIntBuffer4 = OrigTris[j + 1];
                    }
                    else
                    {
                        continue;
                    }
                    if (MinVectorIndices.Contains(tIntBuffer3))
                    {
                        for (int k = 0; k < tCount2; k++)
                        {
                            tIntBuffer2 = MinVectorIndices[k];
                            if (tIntBuffer2 == tIntBuffer3)
                            {
                                if (AlreadyAddedList.Contains(tIntBuffer2))
                                {
                                    break;
                                }
                                if (IsApproxTwoThirds(ref tVect1, OrigVerts[tIntBuffer2], vertexMatchingPrecision))
                                {
                                    MatchingIndices.Add(tIntBuffer1, tIntBuffer2);
                                    AlreadyAddedList.Add(tIntBuffer2);
                                    MatchingIndices_Min.Add(tIntBuffer2, tIntBuffer1);
                                    bAdded = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!bAdded && MinVectorIndices.Contains(tIntBuffer4))
                    {
                        for (int k = 0; k < tCount2; k++)
                        {
                            tIntBuffer2 = MinVectorIndices[k];
                            if (tIntBuffer2 == tIntBuffer4)
                            {
                                if (AlreadyAddedList.Contains(tIntBuffer2))
                                {
                                    break;
                                }
                                if (IsApproxTwoThirds(ref tVect1, OrigVerts[tIntBuffer2], vertexMatchingPrecision))
                                {
                                    MatchingIndices.Add(tIntBuffer1, tIntBuffer2);
                                    AlreadyAddedList.Add(tIntBuffer2);
                                    MatchingIndices_Min.Add(tIntBuffer2, tIntBuffer1);
                                    bAdded = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (bAdded)
                    {
                        break;
                    }
                }
            }

            //Tris don't match, so need further refinement:
            if (MatchingIndices.Count < MaxVectorIndices.Count)
            {
                bool bIsZAxis = (Axis == AxisTypeEnum.Z);
                for (int index = 0; index < tCount1; index++)
                {
                    tIntBuffer1 = MaxVectorIndices[index];
                    if (MatchingIndices.ContainsKey(tIntBuffer1))
                    {
                        continue;
                    }
                    tVect1 = OrigVerts[tIntBuffer1];
                    if (Axis == AxisTypeEnum.Z)
                    {
                        for (int j = 0; j < tCount2; j++)
                        {
                            tIntBuffer2 = MinVectorIndices[j];
                            if (!AlreadyAddedList.Contains(tIntBuffer2))
                            {
                                tVect2 = OrigVerts[tIntBuffer2];
                                if (IsApproxExtruded(ref tVect1, ref tVect2, bIsZAxis) && V3EqualNormal(OrigNormals[tIntBuffer1], OrigNormals[tIntBuffer2]))
                                {
                                    MatchingIndices.Add(tIntBuffer1, tIntBuffer2);
                                    AlreadyAddedList.Add(tIntBuffer2);
                                    MatchingIndices_Min.Add(tIntBuffer2, tIntBuffer1);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Caps:
            if (CapMesh1 != null)
            {
                bool bDidAdd = false;
                foreach (KeyValuePair<int, int> KVP in MatchingIndices_Min)
                {
                    List<int> tList = new List<int>();
                    tVect1 = OrigVerts[KVP.Key];
                    for (int index = 0; index < CapOrigMVL1; index++)
                    {
                        if (tCapMatchIndices1.Contains(index) && IsApproxTwoThirds(ref tVect1, CapOrigVerts1[index], vertexMatchingPrecision))
                        {
                            tList.Add(index);
                            bDidAdd = true;
                        }
                    }
                    MatchingIndices_Min_Cap.Add(KVP.Key, tList);
                }
                if (!bDidAdd)
                {
                    Debug.LogWarning("Start cap error (still processing extrusion, ignoring start cap). No matching vertices found for start cap. Most likely the cap mesh is aligned improperly or along the wrong axis relative to the main mesh.");

                    Object.DestroyImmediate(Cap1);
                    CapMesh1 = null;
                    CapOrigMVL1 = 0;
                    CapTriCount1 = 0;
                }
            }
            if (CapMesh2 != null)
            {
                bool bDidAdd = false;
                foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                {
                    List<int> tList = new List<int>();
                    tVect1 = OrigVerts[KVP.Key];
                    for (int index = 0; index < CapOrigMVL2; index++)
                    {
                        if (tCapMatchIndices2.Contains(index) && IsApproxTwoThirds(ref tVect1, CapOrigVerts2[index], vertexMatchingPrecision))
                        {
                            tList.Add(index);
                            bDidAdd = true;
                        }
                    }
                    MatchingIndices_Max_Cap.Add(KVP.Key, tList);
                }
                if (!bDidAdd)
                {
                    Debug.LogError("End cap error (still processing extrusion, ignoring end cap). No matching vertices found for end cap. Most likely the cap mesh is aligned improperly or along the wrong axis relative to the main mesh.");

                    Object.DestroyImmediate(Cap2);
                    CapMesh2 = null;
                    CapOrigMVL2 = 0;
                    CapTriCount2 = 0;
                }
            }

            //Road definition matching:
            if (isMatchingRoadDefinition)
            {
                float RoadDefStart = (spline.road.roadDefinition / 2f) * -1;
                float UVChange = spline.road.roadDefinition / mMaxDiff;
                foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                {
                    //Vertex change:
                    if (Axis == AxisTypeEnum.X)
                    {
                        OrigVerts[KVP.Value].x = RoadDefStart;
                        OrigVerts[KVP.Key].x = (OrigVerts[KVP.Value].x + spline.road.roadDefinition);
                    }
                    else if (Axis == AxisTypeEnum.Z)
                    {
                        OrigVerts[KVP.Value].z = RoadDefStart;
                        OrigVerts[KVP.Key].z = (OrigVerts[KVP.Value].z + spline.road.roadDefinition);
                    }
                    //UV Change:
                    if (RepeatUVType == RepeatUVTypeEnum.X)
                    {
                        OrigUV[KVP.Key].x *= UVChange;
                    }
                    else if (RepeatUVType == RepeatUVTypeEnum.Y)
                    {
                        OrigUV[KVP.Key].y *= UVChange;
                    }
                }

                //Settings:
                tMinMaxUV = new float[OrigMVL];
                tMinMax = new float[OrigMVL];
                tMinMaxX = new float[OrigMVL];
                tMinMaxY = new float[OrigMVL];
                tMinMaxZ = new float[OrigMVL];
                for (int index = 0; index < OrigMVL; index++)
                {
                    if (Axis == AxisTypeEnum.X)
                    {
                        tMinMax[index] = OrigVerts[index].x;
                    }
                    else
                    {
                        tMinMax[index] = OrigVerts[index].z;
                    }
                    tMinMaxX[index] = OrigVerts[index].x;
                    tMinMaxY[index] = OrigVerts[index].y;
                    tMinMaxZ[index] = OrigVerts[index].z;
                    if (RepeatUVType == RepeatUVTypeEnum.X)
                    {
                        tMinMaxUV[index] = OrigUV[index].x;
                    }
                    else if (RepeatUVType == RepeatUVTypeEnum.Y)
                    {
                        tMinMaxUV[index] = OrigUV[index].y;
                    }
                }
                //UV Changes:
                mMax = Mathf.Max(tMinMax);
                mMin = Mathf.Min(tMinMax);
                mMaxX = Mathf.Max(tMinMaxX);
                mMinX = Mathf.Min(tMinMaxX);
                mMaxY = Mathf.Max(tMinMaxY);
                mMinY = Mathf.Min(tMinMaxY);
                mMaxZ = Mathf.Max(tMinMaxZ);
                mMinZ = Mathf.Min(tMinMaxZ);
                mMinUV = -1f;
                mMaxUV = -1f;
                mUVDiff = -1f;
                if (RepeatUVType != RepeatUVTypeEnum.None)
                {
                    mMinUV = Mathf.Min(tMinMaxUV);
                    mMaxUV = Mathf.Max(tMinMaxUV);
                    mUVDiff = mMaxUV - mMinUV;
                }
                mMaxDiff = mMax - mMin;
                mMaxHeight = mMaxY - mMinY;
                mMaxThreshold = mMax - minMaxMod;
                mMinThreshold = mMin + minMaxMod;
            }

            //For vert reverse cut:
            int VertCutTriIndex1 = -1;
            int VertCutTriIndex2 = -1;
            if (isVerticalMeshCutoffOppositeDir)
            {
                float[] tMatchingMaxY = new float[MatchingIndices.Count];
                int tempcount141 = 0;
                foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                {
                    tMatchingMaxY[tempcount141] = OrigVerts[KVP.Key].y;
                    tempcount141 += 1;
                }

                float tMatchingMaxY_f = Mathf.Max(tMatchingMaxY);
                foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                {
                    if (RootUtils.IsApproximately(OrigVerts[KVP.Key].y, tMatchingMaxY_f, 0.0001f))
                    {
                        VertCutTriIndex1 = KVP.Key;
                        VertCutTriIndex2 = KVP.Value;
                        break;
                    }
                }
            }


            //Set auto simple collision points: 
            if (isSimpleCollisionAutomatic)
            {
                if (Axis == AxisTypeEnum.X)
                {
                    CollisionTriBL = new Vector3(mMinX, mMinY, mMinZ);
                    CollisionTriBR = new Vector3(mMinX, mMinY, mMaxZ);
                    CollisionTriT = new Vector3(mMinX, mMaxY, ((mMaxZ - mMinZ) * 0.5f) + mMinZ);
                }
                else if (Axis == AxisTypeEnum.Z)
                {
                    CollisionTriBL = new Vector3(mMinX, mMinY, mMinZ);
                    CollisionTriBR = new Vector3(mMaxX, mMinY, mMinZ);
                    CollisionTriT = new Vector3(((mMaxX - mMinX) * 0.5f) + mMinX, mMaxY, mMinZ);
                }

                if (Axis == AxisTypeEnum.X)
                {
                    CollisionBoxBL = new Vector3(mMinX, mMinY, mMinZ);
                    CollisionBoxBR = new Vector3(mMinX, mMinY, mMaxZ);
                    CollisionBoxTL = new Vector3(mMinX, mMaxY, mMinZ);
                    CollisionBoxTR = new Vector3(mMinX, mMaxY, mMaxZ);
                }
                else if (Axis == AxisTypeEnum.Z)
                {
                    CollisionBoxBL = new Vector3(mMinX, mMinY, mMinZ);
                    CollisionBoxBR = new Vector3(mMaxX, mMinY, mMinZ);
                    CollisionBoxTL = new Vector3(mMinX, mMaxY, mMinZ);
                    CollisionBoxTR = new Vector3(mMaxX, mMaxY, mMinZ);
                }
            }

            Vector3[] tVerts = null;
            Vector2[] tUV = null;

            //Get the vector series that this mesh is interpolated on:
            List<float> tTimes = new List<float>();
            float cTime = StartTime;


            tTimes.Add(cTime);
            int SpamGuard = 5000;
            int SpamGuardCounter = 0;
            float pDiffTime = EndTime - StartTime;
            float CurrentH = 0f;
            float fHeight = 0f;
            //			Vector3 tStartPos = tSpline.GetSplineValue(StartTime);
            //			Vector3 tEndPos = tSpline.GetSplineValue(EndTime);

            while (cTime < EndTime && SpamGuardCounter < SpamGuard)
            {
                spline.GetSplineValueBoth(cTime, out tVect1, out tDir);
                fHeight = horizontalCurve.Evaluate((cTime - StartTime) / pDiffTime);
                CurrentH = fHeight * horizontalSep;

                if (CurrentH < 0f)
                {
                    CurrentH *= -1f;
                    tVect1 = (tVect1 + new Vector3(CurrentH * -tDir.normalized.z, 0, CurrentH * tDir.normalized.x));
                }
                else if (CurrentH > 0f)
                {
                    tVect1 = (tVect1 + new Vector3(CurrentH * tDir.normalized.z, 0, CurrentH * -tDir.normalized.x));
                }

                xVect = (tDir.normalized * mMaxDiff) + tVect1;

                cTime = spline.GetClosestParam(xVect, false, false);
                if (cTime > EndTime)
                {
                    cTime = EndTime;
                }
                tTimes.Add(cTime);
                SpamGuardCounter += 1;
            }
            if (isTrimStart)
            {
                tTimes.RemoveAt(0);
            }
            else if (isTrimEnd)
            {
                tTimes.RemoveAt(tTimes.Count - 1);
            }
            int vSeriesCount = tTimes.Count;

            //Dynamic vertical and horiz:
            List<float> DynamicVerticalRaise = null;
            List<float> DynamicHoriz = null;
            DynamicVerticalRaise = new List<float>();
            DynamicHoriz = new List<float>();
            float tStartTime = tTimes[0];
            float tEndTime = tTimes[vSeriesCount - 1];
            //			float tDiffTime = tEndTime - tStartTime;
            //			float cDiff = 0f;

            float jDistance = 0f;
            float jStartDistance = spline.TranslateParamToDist(tStartTime);
            float jEndDistance = spline.TranslateParamToDist(tEndTime);
            float jDistanceDiff = jEndDistance - jStartDistance;
            //			float jLastTime = 0f;
            float jCurrTime = 0f;
            //			float jStep = 0.02f / tSpline.distance;
            //			Vector3 jVect1 = default(Vector3);
            //			Vector3 jVect2 = default(Vector3);
            //			float prevFHeight = 0f;
            //			bool basfsafa = false;
            for (int i = 0; i < vSeriesCount; i++)
            {
                //				cDiff = tTimes[i] - tStartTime;
                //				cDiff = cDiff / tDiffTime;

                //Vertical curve:
                if (verticalCurve.keys == null || verticalCurve.length < 1)
                {
                    fHeight = 1f;
                }
                else
                {
                    jDistance = spline.TranslateParamToDist(tTimes[i]);
                    jCurrTime = (jDistance - jStartDistance) / jDistanceDiff;
                    fHeight = verticalCurve.Evaluate(jCurrTime);
                }
                DynamicVerticalRaise.Add(fHeight);

                //Horizontal curve:
                if (horizontalCurve.keys == null || horizontalCurve.length < 1)
                {
                    fHeight = 1f;
                }
                else
                {
                    fHeight = horizontalCurve.Evaluate(jCurrTime);
                }
                DynamicHoriz.Add(fHeight);
            }

            Vector3[] VectorSeries = new Vector3[vSeriesCount];
            Vector3[] VectorSeriesTangents = new Vector3[vSeriesCount];
            //			bool bIsCenter = RootUtils.IsApproximately(HorizontalSep,0f,0.02f);
            float tIntStrength = 0f;
            float tIntHeight = 0f;
            RoadIntersection roadIntersection = null;
            bool bIsPastInter = false;
            SplineN xNode = null;
            List<float> tOrigHeights = new List<float>();

            //			List<Terrain> xTerrains = null;
            //			List<RoadUtility.Construction2DRect> tTerrainRects = null;
            //			int TerrainCount = 0;
            //			if(bMatchTerrain){
            //				tTerrainRects = new List<RoadUtility.Construction2DRect>();
            //				xTerrains = new List<Terrain>();
            //				Object[] tTerrains = GameObject.FindObjectsOfType<Terrain>();
            //				RoadUtility.Construction2DRect tTerrainRect = null;
            //				Vector2 tPos2D = default(Vector2);
            //				Vector2 P1,P2,P3,P4;
            //				foreach(Terrain xTerrain in tTerrains){
            //					tPos2D = new Vector2(xTerrain.transform.position.x,xTerrain.transform.position.z);
            //					P1 = new Vector2(0f,0f) + tPos2D;
            //					P2 = new Vector2(0f,xTerrain.terrainData.size.y) + tPos2D;
            //					P3 = new Vector2(xTerrain.terrainData.size.x,xTerrain.terrainData.size.y) + tPos2D;
            //					P4 = new Vector2(xTerrain.terrainData.size.x,0f) + tPos2D;
            //					tTerrainRect = new RoadUtility.Construction2DRect(P1,P2,P3,P4,xTerrain.transform.position.y);
            //					tTerrainRects.Add(tTerrainRect);
            //					xTerrains.Add(xTerrain);
            //					TerrainCount+=1;
            //				}
            //			}

            //			Vector2 temp2DVect = default(Vector2);
            Ray tRay = default(Ray);
            RaycastHit[] tRayHit = null;
            float[] tRayYs = null;
            for (int i = 0; i < vSeriesCount; i++)
            {
                cTime = tTimes[i];
                spline.GetSplineValueBoth(cTime, out tVect1, out tVect2);
                tOrigHeights.Add(tVect1.y);

                //Horizontal offset:
                CurrentH = DynamicHoriz[i] * horizontalSep;

                if (CurrentH < 0f)
                {
                    CurrentH *= -1f;
                    tVect1 = (tVect1 + new Vector3(CurrentH * -tVect2.normalized.z, 0, CurrentH * tVect2.normalized.x));
                }
                else if (CurrentH > 0f)
                {
                    tVect1 = (tVect1 + new Vector3(CurrentH * tVect2.normalized.z, 0, CurrentH * -tVect2.normalized.x));
                }

                tIntStrength = spline.IntersectionStrength(ref tVect1, ref tIntHeight, ref roadIntersection, ref bIsPastInter, ref cTime, ref xNode);

                if (RootUtils.IsApproximately(tIntStrength, 1f, 0.0001f))
                {
                    tVect1.y = tIntHeight;
                }
                else if (!RootUtils.IsApproximately(tIntStrength, 0f, 0.001f))
                {
                    tVect1.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * tVect1.y);
                }

                //Terrain matching:
                if (isMatchingTerrain)
                {
                    //					temp2DVect = new Vector2(tVect1.x,tVect1.z);
                    //					for(int j=0;j<TerrainCount;j++){
                    //						if(tTerrainRects[j].Contains(ref temp2DVect)){
                    //							tVect1.y = xTerrains[j].SampleHeight(tVect1);
                    //							break;
                    //						}
                    //					}


                    tRay = new Ray(tVect1 + new Vector3(0f, 1f, 0f), Vector3.down);
                    tRayHit = Physics.RaycastAll(tRay);
                    if (tRayHit.Length > 0)
                    {
                        tRayYs = new float[tRayHit.Length];
                        for (int g = 0; g < tRayHit.Length; g++)
                        {
                            tRayYs[g] = tRayHit[g].point.y;
                        }
                        tVect1.y = Mathf.Max(tRayYs);
                    }
                }

                tVect1.y += (DynamicVerticalRaise[i] * verticalRaise);

                VectorSeries[i] = tVect1;
                VectorSeriesTangents[i] = tVect2;
            }
            int MeshCount = (vSeriesCount - 1);

            //			float yDiff = 0f;
            //			float tDistance = 0f;
            int MVL = MeshCount * OrigMVL;
            #if UNITY_2017_3_OR_NEWER
            if (MVL > 4000000)
            {
                throw new System.Exception("Over 4000000 vertices detected, exiting extrusion. Try switching splination axis and make sure your imported FBX file has proper import scale. Make sure the mesh isn't too small and make sure the distance isn't too large.");
            }
            #else
			if(MVL > 64900)
            {
				throw new System.Exception("Over 65000 vertices detected, exiting extrusion. Try switching splination axis and make sure your imported FBX file has proper import scale. Make sure the mesh isn't too small and make sure the distance isn't too large.");
			}
            #endif
            int MaxCount = MaxVectorIndices.Count;
            int MinCount = MinVectorIndices.Count;
            int TriCount = MeshCount * OrigTriCount;
            //			int MatchCount = MatchingIndices.Count;
            tVerts = new Vector3[MVL];
            tUV = new Vector2[MVL];
            int[] tTris = new int[TriCount];
            Vector3[] tNormals = new Vector3[MVL];
            int vManuver = 0;
            int vManuver_Prev = 0;
            int TriManuver = 0;
            Vector3[] cVerts = null;
            int[] cTris = null;
            int cCount = -1;
            int cTriCount = -1;
            bool bSimpleCollisionOn = false;
            float tOrigHeightBuffer = 0f;
            float tFloat5 = 0f;
            if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
            {
                cVerts = new Vector3[3 * (MeshCount + 1)];
                cCount = cVerts.Length;
                cTriCount = (6 * cCount) + 2;
                bSimpleCollisionOn = true;
            }
            else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
            {
                cVerts = new Vector3[4 * (MeshCount + 1)];
                cCount = cVerts.Length;
                cTriCount = (8 * cCount) + 4;
                bSimpleCollisionOn = true;
            }

            //			List<RoadUtility.Construction3DTri> tTriList = null;
            //			RoadUtility.Construction3DTri VertOppCutTri = null;
            //			int VertCutBufferIndex1 = -1;
            //			int VertCutBufferIndex2 = -1;
            Vector3 VertCutBuffer1 = default(Vector3);
            Vector3 VertCutBuffer2 = default(Vector3);
            Vector3 VertCutBuffer3 = default(Vector3);
            float tOrigHeightBuffer_Orig = 0f;
            //			if(bVerticalMeshCutoff_OppositeDir){
            //				tTriList = new List<RoadUtility.Construction3DTri>();
            //			}

            if (isStretched)
            {
                DoStretch(ref OrigVerts, ref OrigUV, ref OrigTris, ref MaxVectorIndices, ref MinVectorIndices, mMaxDiff, out tVerts, out tUV, out tNormals, out tTris);
                goto StretchSkip;
            }

            //Main loop:
            Matrix4x4 tMatrix = new Matrix4x4();
            for (int j = 0; j < MeshCount; j++)
            {
                TriManuver = j * OrigTriCount;
                vManuver = j * OrigMVL;
                vManuver_Prev = (j - 1) * OrigMVL;

                if (!isStretched)
                {
                    tVect1 = VectorSeries[j];
                    tVect2 = VectorSeries[j + 1];
                }

                //				yDiff = tVect2.y - tVect1.y;
                //				tDistance = Vector3.Distance(tVect1,tVect2);

                //				if(j==0){ tStartPos = tVect1; }
                //				if(j==(MeshCount-1)){ tEndPos = tVect1; }

                if (isExactSplination && MiddleCount < 2)
                {
                    tDir = (tVect2 - tVect1).normalized;
                }
                else
                {
                    tDir = VectorSeriesTangents[j].normalized;
                }

                tOrigHeightBuffer = tOrigHeights[j] + VerticalCutoff;
                tOrigHeightBuffer_Orig = tOrigHeights[j];
                tMatrix.SetTRS(tVect1, Quaternion.LookRotation(tDir), new Vector3(1f, 1f, 1f));

                //Rotate and set vertex positions:
                for (int index = 0; index < OrigMVL; index++)
                {
                    xVect = OrigVerts[index];
                    tVerts[vManuver + index] = tMatrix.MultiplyPoint3x4(xVect);
                    //					tVerts[vManuver+i] = (Quaternion.LookRotation(tDir)*xVect) + tVect1;

                    //UV:
                    tUV[vManuver + index] = OrigUV[index];

                    //Vertical cutoff:
                    if (isVerticalCutoff)
                    {
                        if (MiddleVectorIndicies.Contains(index))
                        {
                            tFloat5 = tVerts[vManuver + index].y;
                            if (isVerticalCutoffDownwards)
                            {
                                if (isVerticalCutoffMatchingZero)
                                {
                                    if (tFloat5 < tOrigHeightBuffer_Orig)
                                    {
                                        tVerts[vManuver + index].y = tOrigHeightBuffer_Orig;
                                    }
                                }
                                else
                                {
                                    if (tFloat5 < tOrigHeightBuffer)
                                    {
                                        tVerts[vManuver + index].y = tOrigHeightBuffer;
                                    }
                                }

                                tFloat1 = (tOrigHeightBuffer_Orig - tOrigHeightBuffer) / mMaxHeight;
                                tUV[vManuver + index].x *= tFloat1;

                            }
                            else
                            {
                                if (isVerticalCutoffMatchingZero)
                                {
                                    if (tFloat5 > tOrigHeightBuffer_Orig)
                                    {
                                        tVerts[vManuver + index].y = tOrigHeightBuffer_Orig;
                                    }
                                }
                                else
                                {
                                    if (tFloat5 > tOrigHeightBuffer)
                                    {
                                        tVerts[vManuver + index].y = tOrigHeightBuffer;
                                    }
                                }

                                tFloat1 = (tOrigHeightBuffer - tOrigHeightBuffer_Orig) / mMaxHeight;
                                tUV[vManuver + index].x *= tFloat1;
                            }
                        }
                    }
                }

                if (RepeatUVType != RepeatUVTypeEnum.None)
                {
                    for (int index = 0; index < MaxCount; index++)
                    {
                        tIntBuffer1 = MaxVectorIndices[index];
                        if (RepeatUVType == RepeatUVTypeEnum.X)
                        {
                            tUV[vManuver + tIntBuffer1].x = mUVDiff * (j + 1);
                        }
                        else
                        {
                            tUV[vManuver + tIntBuffer1].y = mUVDiff * (j + 1);
                        }
                    }
                    for (int index = 0; index < MinCount; index++)
                    {
                        tIntBuffer1 = MinVectorIndices[index];
                        if (RepeatUVType == RepeatUVTypeEnum.X)
                        {
                            tUV[vManuver + tIntBuffer1].x = mUVDiff * j;
                        }
                        else
                        {
                            tUV[vManuver + tIntBuffer1].y = mUVDiff * j;
                        }
                    }
                }

                //Simple collision (triangle or trap):
                if (bSimpleCollisionOn)
                {
                    if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                    {
                        cVerts[0 + (j * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriBL);
                        cVerts[1 + (j * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriBR);
                        cVerts[2 + (j * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriT);

                        //						cVerts[0+(j*3)] = (Quaternion.LookRotation(tDir)*CollisionTriBL) + tVect1;
                        //						cVerts[1+(j*3)] = (Quaternion.LookRotation(tDir)*CollisionTriBR) + tVect1;
                        //						cVerts[2+(j*3)] = (Quaternion.LookRotation(tDir)*CollisionTriT) + tVect1;
                    }
                    else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                    {
                        cVerts[0 + (j * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxBL);
                        cVerts[1 + (j * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxBR);
                        cVerts[2 + (j * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxTL);
                        cVerts[3 + (j * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxTR);

                        //						cVerts[0+(j*4)] = (Quaternion.LookRotation(tDir)*CollisionBoxBL) + tVect1;
                        //						cVerts[1+(j*4)] = (Quaternion.LookRotation(tDir)*CollisionBoxBR) + tVect1;
                        //						cVerts[2+(j*4)] = (Quaternion.LookRotation(tDir)*CollisionBoxTL) + tVect1;
                        //						cVerts[3+(j*4)] = (Quaternion.LookRotation(tDir)*CollisionBoxTR) + tVect1;
                    }

                    if (j == (MeshCount - 1))
                    {
                        Vector3 tAdd = default(Vector3);
                        if (Axis == AxisTypeEnum.X)
                        {
                            tAdd = new Vector3(mMaxDiff * -1f, 0f, 0f);
                        }
                        else
                        {
                            tAdd = new Vector3(0f, 0f, mMaxDiff);
                        }

                        if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                        {
                            cVerts[0 + ((j + 1) * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriBL + tAdd);
                            cVerts[1 + ((j + 1) * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriBR + tAdd);
                            cVerts[2 + ((j + 1) * 3)] = tMatrix.MultiplyPoint3x4(CollisionTriT + tAdd);

                            //							cVerts[0+((j+1)*3)] = (Quaternion.LookRotation(tDir)*(CollisionTriBL + tAdd)) + tVect1;
                            //							cVerts[1+((j+1)*3)] = (Quaternion.LookRotation(tDir)*(CollisionTriBR + tAdd)) + tVect1;
                            //							cVerts[2+((j+1)*3)] = (Quaternion.LookRotation(tDir)*(CollisionTriT + tAdd)) + tVect1;
                        }
                        else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                        {
                            cVerts[0 + ((j + 1) * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxBL + tAdd);
                            cVerts[1 + ((j + 1) * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxBR + tAdd);
                            cVerts[2 + ((j + 1) * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxTL + tAdd);
                            cVerts[3 + ((j + 1) * 4)] = tMatrix.MultiplyPoint3x4(CollisionBoxTR + tAdd);

                            //							cVerts[0+((j+1)*4)] = (Quaternion.LookRotation(tDir)*(CollisionBoxBL + tAdd)) + tVect1;
                            //							cVerts[1+((j+1)*4)] = (Quaternion.LookRotation(tDir)*(CollisionBoxBR + tAdd)) + tVect1;
                            //							cVerts[2+((j+1)*4)] = (Quaternion.LookRotation(tDir)*(CollisionBoxTL + tAdd)) + tVect1;
                            //							cVerts[3+((j+1)*4)] = (Quaternion.LookRotation(tDir)*(CollisionBoxTR + tAdd)) + tVect1;
                        }
                    }
                }

                //If j > 0, the previous max vects need to match current min vects:
                Vector3 mVect = default(Vector3);
                if (j > 0)
                {
                    //					foreach(KeyValuePair<int,int> KVP in MatchingIndices){
                    //						tNormals[vManuver+KVP.Key] = tNormals[KVP.Value];
                    //					}
                    foreach (KeyValuePair<int, int> KVP in MatchingIndices_Min)
                    {
                        mVect = tVerts[vManuver + KVP.Key] - tVerts[vManuver_Prev + KVP.Value];
                        tVerts[vManuver + KVP.Key] = tVerts[vManuver_Prev + KVP.Value];
                    }

                    for (int g = 0; g < MinVectorIndices.Count; g++)
                    {
                        if (!MatchingIndices_Min.ContainsKey(MinVectorIndices[g]))
                        {
                            tVerts[vManuver + MinVectorIndices[g]] -= mVect;
                        }
                    }

                    //Simple collision (triangle or trap):
                    if (bSimpleCollisionOn)
                    {
                        if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                        {
                            cVerts[0 + (j * 3)] -= mVect;
                            cVerts[1 + (j * 3)] -= mVect;
                            cVerts[2 + (j * 3)] -= mVect;

                        }
                        else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                        {
                            cVerts[0 + (j * 4)] -= mVect;
                            cVerts[1 + (j * 4)] -= mVect;
                            cVerts[2 + (j * 4)] -= mVect;
                            cVerts[3 + (j * 4)] -= mVect;
                        }
                    }
                }

                //Triangles:
                for (int index = 0; index < OrigTriCount; index++)
                {
                    tTris[index + TriManuver] = OrigTris[index] + vManuver;
                }

                //Vert cut reverse:
                if (isVerticalCutoff)
                {
                    if (isVerticalMeshCutoffOppositeDir)
                    {
                        VertCutBuffer1 = tVerts[vManuver + VertCutTriIndex1];
                        VertCutBuffer2 = tVerts[vManuver + VertCutTriIndex2];

                        for (int index = 0; index < MiddleCount; index++)
                        {
                            VertCutBuffer3 = tVerts[vManuver + MiddleVectorIndicies[index]];

                            if (!isVerticalCutoffDownwards)
                            {
                                tBuffer = GetVHeightAtXY(ref VertCutBuffer1, ref VertCutBuffer2, ref VertCutBuffer3) + VerticalMeshCutoffOffset;
                                if (VertCutBuffer3.y < tBuffer)
                                {
                                    tVerts[vManuver + MiddleVectorIndicies[index]].y = tBuffer;
                                }
                            }
                            else
                            {
                                tBuffer = GetVHeightAtXY(ref VertCutBuffer1, ref VertCutBuffer2, ref VertCutBuffer3) - VerticalMeshCutoffOffset;
                                if (VertCutBuffer3.y > tBuffer)
                                {
                                    tVerts[vManuver + MiddleVectorIndicies[index]].y = tBuffer;
                                }
                            }
                        }
                    }
                }


                //Ending push down:
                if (isStartDown)
                {
                    tFloat1 = mMaxHeight * 1.05f;
                    if (isStartTypeDownOverriden)
                    {
                        tFloat1 = startTypeDownOverride;
                    }
                    if (j == 0)
                    {
                        for (int index = 0; index < MinCount; index++)
                        {
                            tIntBuffer1 = MinVectorIndices[index];
                            tVerts[vManuver + tIntBuffer1].y -= tFloat1;
                        }

                        float tTotalDistDown = 0f;
                        Vector3 pVect1 = default(Vector3);
                        Vector3 pVect2 = default(Vector3);
                        foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                        {
                            pVect1 = tVerts[vManuver + KVP.Key];
                            pVect2 = tVerts[vManuver + KVP.Value];
                            tTotalDistDown = Vector3.Distance(pVect1, pVect2);
                            break;
                        }

                        for (int index = 0; index < MiddleCount; index++)
                        {
                            tIntBuffer1 = MiddleVectorIndicies[index];
                            float tDistTo1 = Vector3.Distance(tVerts[vManuver + tIntBuffer1], pVect1);
                            tVerts[vManuver + tIntBuffer1].y -= (tFloat1 * (tDistTo1 / tTotalDistDown));
                        }

                        if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                        {
                            cVerts[0 + (j * 3)].y -= tFloat1;
                            cVerts[1 + (j * 3)].y -= tFloat1;
                            cVerts[2 + (j * 3)].y -= tFloat1;
                        }
                        else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                        {
                            cVerts[0 + (j * 4)].y -= tFloat1;
                            cVerts[1 + (j * 4)].y -= tFloat1;
                            cVerts[2 + (j * 4)].y -= tFloat1;
                            cVerts[3 + (j * 4)].y -= tFloat1;
                        }
                    }
                }

                if (isEndDown)
                {
                    tFloat1 = mMaxHeight * 1.05f;
                    if (isEndTypeDownOverriden)
                    {
                        tFloat1 = endTypeDownOverride;
                    }
                    if (j == (MeshCount - 1))
                    {
                        for (int index = 0; index < MaxCount; index++)
                        {
                            tIntBuffer1 = MaxVectorIndices[index];
                            tVerts[vManuver + tIntBuffer1].y -= tFloat1;
                        }

                        float tTotalDistDown = 0f;
                        Vector3 pVect1 = default(Vector3);
                        Vector3 pVect2 = default(Vector3);
                        foreach (KeyValuePair<int, int> KVP in MatchingIndices)
                        {
                            pVect1 = tVerts[vManuver + KVP.Key];
                            pVect2 = tVerts[vManuver + KVP.Value];
                            tTotalDistDown = Vector3.Distance(pVect1, pVect2);
                            break;
                        }

                        for (int i = 0; i < MiddleCount; i++)
                        {
                            tIntBuffer1 = MiddleVectorIndicies[i];
                            float tDistTo1 = Vector3.Distance(tVerts[vManuver + tIntBuffer1], pVect2);
                            tVerts[vManuver + tIntBuffer1].y -= (tFloat1 * (tDistTo1 / tTotalDistDown));
                        }

                        if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                        {
                            cVerts[0 + ((j + 1) * 3)].y -= tFloat1;
                            cVerts[1 + ((j + 1) * 3)].y -= tFloat1;
                            cVerts[2 + ((j + 1) * 3)].y -= tFloat1;
                        }
                        else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                        {
                            cVerts[0 + ((j + 1) * 4)].y -= tFloat1;
                            cVerts[1 + ((j + 1) * 4)].y -= tFloat1;
                            cVerts[2 + ((j + 1) * 4)].y -= tFloat1;
                            cVerts[3 + ((j + 1) * 4)].y -= tFloat1;
                        }
                    }
                }

                //Ending objects:
                if (j == 0 && EndCapStartObj != null)
                {
                    if (isEndCapCustomMatchStart && MinVectorIndices.Count > 0)
                    {
                        Vector3[] bVerts = new Vector3[MinVectorIndices.Count];
                        for (int g = 0; g < MinVectorIndices.Count; g++)
                        {
                            bVerts[g] = tVerts[vManuver + MinVectorIndices[g]];
                        }
                        Vector3 tVect5 = GetVector3Average(bVerts);
                        Vector3 tVect6 = spline.GetSplineValue(spline.GetClosestParam(tVect5, false, false), false);
                        tVect5.y = tVect6.y;
                        EndCapStartObj.transform.position = tVect5;
                    }
                    else
                    {
                        EndCapStartObj.transform.position = tVect1;
                    }

                    if (isEndObjectsMatchingGround)
                    {
                        tRay = default(Ray);
                        tRayHit = null;
                        float tHitY = 0f;
                        //						int tHitIndex = 0;
                        Vector3 HitVect = EndCapStartObj.transform.position;
                        tRay = new Ray(HitVect + new Vector3(0f, 1f, 0f), Vector3.down);
                        tRayHit = Physics.RaycastAll(tRay);
                        if (tRayHit.Length > 0)
                        {
                            tRayYs = new float[tRayHit.Length];
                            for (int g = 0; g < tRayHit.Length; g++)
                            {
                                if (g == 0)
                                {
                                    tHitY = tRayHit[g].point.y;
                                    //									tHitIndex=0;
                                }
                                else
                                {
                                    if (tRayHit[g].point.y > tHitY)
                                    {
                                        tHitY = tRayHit[g].point.y;
                                        //										tHitIndex = g;
                                    }
                                }
                            }
                            HitVect.y = tHitY;
                            EndCapStartObj.transform.position = HitVect;
                        }
                    }
                    EndCapStartObj.transform.rotation = Quaternion.LookRotation(tDir);
                    EndCapStartObj.transform.Rotate(EndCapCustomRotOffsetStart, Space.World);
                    EndCapStartObj.transform.position += EndCapCustomOffsetStart;

                }
                else if (j == (MeshCount - 1) && EndCapEndObj != null)
                {
                    if (isEndCapCustomMatchStart && MaxVectorIndices.Count > 0)
                    {
                        Vector3[] bVerts = new Vector3[MaxVectorIndices.Count];
                        for (int g = 0; g < MaxVectorIndices.Count; g++)
                        {
                            bVerts[g] = tVerts[vManuver + MaxVectorIndices[g]];
                        }
                        Vector3 tVect5 = GetVector3Average(bVerts);
                        Vector3 tVect6 = spline.GetSplineValue(spline.GetClosestParam(tVect5, false, false), false);
                        if (!float.IsNaN(tVect6.y))
                        {
                            tVect5.y = tVect6.y;
                        }
                        EndCapEndObj.transform.position = tVect5;
                    }
                    else
                    {
                        EndCapEndObj.transform.position = tVect2;
                    }

                    if (isEndObjectsMatchingGround)
                    {
                        tRay = default(Ray);
                        tRayHit = null;
                        float tHitY = 0f;
                        //						int tHitIndex = 0;
                        Vector3 HitVect = EndCapEndObj.transform.position;
                        tRay = new Ray(HitVect + new Vector3(0f, 1f, 0f), Vector3.down);
                        tRayHit = Physics.RaycastAll(tRay);

                        if (tRayHit.Length > 0)
                        {
                            tRayYs = new float[tRayHit.Length];
                            for (int g = 0; g < tRayHit.Length; g++)
                            {
                                if (g == 0)
                                {
                                    tHitY = tRayHit[g].point.y;
                                    //									tHitIndex=0;
                                }
                                else
                                {
                                    if (tRayHit[g].point.y > tHitY)
                                    {
                                        tHitY = tRayHit[g].point.y;
                                        //										tHitIndex = g;
                                    }
                                }
                            }
                            HitVect.y = tHitY;
                            EndCapEndObj.transform.position = HitVect;
                        }
                    }
                    EndCapEndObj.transform.rotation = Quaternion.LookRotation(tDir);
                    EndCapEndObj.transform.Rotate(EndCapCustomRotOffsetEnd, Space.World);
                    EndCapEndObj.transform.position += EndCapCustomOffsetEnd;
                }
            }

            StretchSkip:
            if (isStretched)
            {
                vManuver = 0;
            }

            //End/Start for stretch:
            if (isStretched)
            {
                //Ending objects:
                if (EndCapStartObj != null)
                {
                    tVect1 = tVerts[MinVectorIndices[0]];
                    tFloat1 = spline.GetClosestParam(tVect1);
                    tVect2 = spline.GetSplineValue(tFloat1, false);
                    tVect1.y = tVect2.y;

                    EndCapStartObj.transform.position = tVect1;
                    EndCapStartObj.transform.rotation = Quaternion.LookRotation(tDir);
                    EndCapStartObj.transform.Rotate(EndCapCustomRotOffsetStart, Space.World);
                    EndCapStartObj.transform.position += EndCapCustomOffsetStart;
                }
                if (EndCapEndObj != null)
                {
                    tVect1 = tVerts[MaxVectorIndices[0]];
                    tFloat1 = spline.GetClosestParam(tVect1);
                    tVect2 = spline.GetSplineValue(tFloat1, false);
                    tVect1.y = tVect2.y;

                    EndCapEndObj.transform.position = tVect1;
                    EndCapEndObj.transform.rotation = Quaternion.LookRotation(tDir);
                    EndCapEndObj.transform.Rotate(EndCapCustomRotOffsetEnd, Space.World);
                    EndCapEndObj.transform.position += EndCapCustomOffsetEnd;
                }
            }

            if (bSimpleCollisionOn && !isStretched)
            {
                if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
                {
                    cTris = GetCollisionTrisTri(MeshCount, cTriCount, cCount);
                }
                else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
                {
                    cTris = GetCollisionTrisBox(MeshCount, cTriCount, cCount);
                }
            }

            if (CapMesh1 != null)
            {
                Vector3[] cap1_verts = new Vector3[CapOrigMVL1];
                System.Array.Copy(CapOrigVerts1, cap1_verts, CapOrigMVL1);
                int[] cap1_tris = new int[CapTriCount1];
                System.Array.Copy(CapOrigTris1, cap1_tris, CapTriCount1);
                Vector2[] cap1_uv = new Vector2[CapOrigMVL1];
                System.Array.Copy(CapOrigUV1, cap1_uv, CapOrigMVL1);
                Vector3[] cap1_normals = new Vector3[CapOrigMVL1];
                System.Array.Copy(CapOrigNormals1, cap1_normals, CapOrigMVL1);
                bool[] cap1_hit = new bool[CapOrigMVL1];
                bool bcapstart = true;
                float tHeight = 0f;

                foreach (KeyValuePair<int, List<int>> KVP in MatchingIndices_Min_Cap)
                {
                    int wCount = KVP.Value.Count;
                    for (int index = 0; index < wCount; index++)
                    {
                        if (bcapstart)
                        {
                            tVect1 = cap1_verts[KVP.Value[index]] - tVerts[KVP.Key];
                        }
                        cap1_verts[KVP.Value[index]] = tVerts[KVP.Key];
                        cap1_hit[KVP.Value[index]] = true;
                        if (bcapstart)
                        {
                            tHeight = spline.GetSplineValue(spline.GetClosestParam(cap1_verts[KVP.Value[index]]), false).y;
                            bcapstart = false;
                        }
                    }
                }

                float tParam = 0f;
                for (int index = 0; index < CapOrigMVL1; index++)
                {
                    if (!cap1_hit[index])
                    {
                        cap1_verts[index] -= tVect1;
                        tParam = spline.GetClosestParam(cap1_verts[index]);
                        tVect2 = spline.GetSplineValue(tParam, false);
                        cap1_verts[index].y -= (tHeight - tVect2.y);
                        cap1_verts[index].y += capHeightOffset1;
                    }
                }

                Vector3[] nVerts = new Vector3[CapOrigMVL1 + tVerts.Length];
                Vector3[] nNormals = new Vector3[CapOrigMVL1 + tNormals.Length];
                int[] nTris = new int[CapTriCount1 + tTris.Length];
                Vector2[] nUV = new Vector2[CapOrigMVL1 + tUV.Length];
                int OldTriCount = tTris.Length;
                int OldMVL = tVerts.Length;

                System.Array.Copy(cap1_verts, nVerts, CapOrigMVL1);
                System.Array.Copy(cap1_normals, nNormals, CapOrigMVL1);
                System.Array.Copy(cap1_tris, nTris, CapTriCount1);
                System.Array.Copy(cap1_uv, nUV, CapOrigMVL1);

                System.Array.Copy(tVerts, 0, nVerts, CapOrigMVL1, OldMVL);
                System.Array.Copy(tNormals, 0, nNormals, CapOrigMVL1, OldMVL);
                System.Array.Copy(tTris, 0, nTris, CapTriCount1, OldTriCount);
                System.Array.Copy(tUV, 0, nUV, CapOrigMVL1, OldMVL);

                for (int index = CapTriCount1; index < (CapTriCount1 + OldTriCount); index++)
                {
                    nTris[index] += CapOrigMVL1;
                }

                tVerts = nVerts;
                tTris = nTris;
                tNormals = nNormals;
                tUV = nUV;
            }

            if (CapMesh2 != null)
            {
                Vector3[] cap2_verts = new Vector3[CapOrigMVL2];
                System.Array.Copy(CapOrigVerts2, cap2_verts, CapOrigMVL2);
                int[] cap2_tris = new int[CapTriCount2];
                System.Array.Copy(CapOrigTris2, cap2_tris, CapTriCount2);
                Vector2[] cap2_uv = new Vector2[CapOrigMVL2];
                System.Array.Copy(CapOrigUV2, cap2_uv, CapOrigMVL2);
                Vector3[] cap2_normals = new Vector3[CapOrigMVL2];
                System.Array.Copy(CapOrigNormals2, cap2_normals, CapOrigMVL2);
                bool[] cap2_hit = new bool[CapOrigMVL2];
                bool bcapstart = true;
                float tHeight = 0f;

                foreach (KeyValuePair<int, List<int>> KVP in MatchingIndices_Max_Cap)
                {
                    int wCount = KVP.Value.Count;
                    for (int index = 0; index < wCount; index++)
                    {
                        if (bcapstart)
                        {
                            tVect1 = cap2_verts[KVP.Value[index]] - tVerts[vManuver + KVP.Key + CapOrigMVL1];
                        }
                        cap2_verts[KVP.Value[index]] = tVerts[vManuver + KVP.Key + CapOrigMVL1];
                        cap2_hit[KVP.Value[index]] = true;

                        if (bcapstart)
                        {
                            tHeight = spline.GetSplineValue(spline.GetClosestParam(cap2_verts[KVP.Value[index]]), false).y;
                            bcapstart = false;
                        }
                    }
                }

                float tParam = 0f;
                for (int index = 0; index < CapOrigMVL2; index++)
                {

                    if (!cap2_hit[index])
                    {
                        cap2_verts[index] -= tVect1;
                        tParam = spline.GetClosestParam(cap2_verts[index]);
                        tVect2 = spline.GetSplineValue(tParam, false);
                        cap2_verts[index].y -= (tHeight - tVect2.y);
                        cap2_verts[index].y += capHeightOffset2;
                    }
                }

                Vector3[] nVerts = new Vector3[CapOrigMVL2 + tVerts.Length];
                Vector3[] nNormals = new Vector3[CapOrigMVL2 + tNormals.Length];
                int[] nTris = new int[CapTriCount2 + tTris.Length];
                Vector2[] nUV = new Vector2[CapOrigMVL2 + tUV.Length];
                int OldTriCount = tTris.Length;
                int OldMVL = tVerts.Length;

                System.Array.Copy(tVerts, 0, nVerts, 0, OldMVL);
                System.Array.Copy(tNormals, 0, nNormals, 0, OldMVL);
                System.Array.Copy(tTris, 0, nTris, 0, OldTriCount);
                System.Array.Copy(tUV, 0, nUV, 0, OldMVL);

                System.Array.Copy(cap2_verts, 0, nVerts, OldMVL, CapOrigMVL2);
                System.Array.Copy(cap2_normals, 0, nNormals, OldMVL, CapOrigMVL2);
                System.Array.Copy(cap2_tris, 0, nTris, OldTriCount, CapTriCount2);
                System.Array.Copy(cap2_uv, 0, nUV, OldMVL, CapOrigMVL2);

                for (int index = OldTriCount; index < nTris.Length; index++)
                {
                    nTris[index] += OldMVL;
                }

                tVerts = nVerts;
                tTris = nTris;
                tNormals = nNormals;
                tUV = nUV;
            }

            int tVertCount = tVerts.Length;
            for (int index = 0; index < tVertCount; index++)
            {
                tVerts[index] -= node.pos;
            }
            if (cVerts != null)
            {
                int cVertCount = cVerts.Length;
                for (int index = 0; index < cVertCount; index++)
                {
                    cVerts[index] -= node.pos;
                }
            }

            //Mesh creation:
            Mesh xMesh = new Mesh();
            xMesh.vertices = tVerts;
            xMesh.triangles = tTris;
            xMesh.normals = tNormals;
            xMesh.uv = tUV;
            xMesh.RecalculateNormals();
            tNormals = xMesh.normals;
            Vector3 tAvgNormal = default(Vector3);
            tIntBuffer1 = 0;
            if (!isStretched)
            {
                for (int j = 1; j < MeshCount; j++)
                {
                    vManuver = j * OrigMVL;
                    vManuver_Prev = (j - 1) * OrigMVL;
                    if (CapMesh1 != null)
                    {
                        tIntBuffer1 = CapOrigMVL1;
                    }
                    foreach (KeyValuePair<int, int> KVP in MatchingIndices_Min)
                    {
                        tAvgNormal = (tNormals[tIntBuffer1 + vManuver + KVP.Key] + tNormals[tIntBuffer1 + vManuver_Prev + KVP.Value]) * 0.5f;
                        tNormals[tIntBuffer1 + vManuver + KVP.Key] = tAvgNormal;
                        tNormals[tIntBuffer1 + vManuver_Prev + KVP.Key] = tAvgNormal;
                    }
                }
                xMesh.normals = tNormals;
            }
            xMesh.tangents = RootUtils.ProcessTangents(tTris, tNormals, tUV, tVerts);

            if (objectName == null || objectName.Length < 1)
            {
                objectName = "ExtrudedMesh";
            }

            Output = new GameObject(objectName);
            Output.transform.position = node.pos;

            MF = Output.AddComponent<MeshFilter>();
            MF.sharedMesh = xMesh;

            if (node.spline.road.isSavingMeshes)
            {
                SaveMesh(ref xMesh, false);
            }

            //Colliders:
            MeshCollider MC = null;
            if (CollisionType == CollisionTypeEnum.SimpleMeshTriangle)
            {
                MC = Output.AddComponent<MeshCollider>();
                Mesh cMesh = new Mesh();
                cMesh.vertices = cVerts;
                cMesh.triangles = cTris;
                cMesh.normals = new Vector3[cVerts.Length];
                if (MC != null)
                {
                    MC.sharedMesh = cMesh;
                }
                if (MC != null)
                {
                    MC.convex = isCollisionConvex;
                    MC.isTrigger = isCollisionTrigger;
                    if (node.spline.road.isSavingMeshes)
                    {
                        cMesh.uv = new Vector2[cVerts.Length];
                        cMesh.tangents = RootUtils.ProcessTangents(cTris, cMesh.normals, cMesh.uv, cVerts);
                        SaveMesh(ref cMesh, true);
                    }
                }
            }
            else if (CollisionType == CollisionTypeEnum.SimpleMeshTrapezoid)
            {
                MC = Output.AddComponent<MeshCollider>();
                Mesh cMesh = new Mesh();
                cMesh.vertices = cVerts;
                cMesh.triangles = cTris;
                cMesh.normals = new Vector3[cVerts.Length];
                if (MC != null)
                {
                    MC.sharedMesh = cMesh;
                }
                if (MC != null)
                {
                    MC.convex = isCollisionConvex;
                    MC.isTrigger = isCollisionTrigger;
                    if (node.spline.road.isSavingMeshes)
                    {
                        cMesh.uv = new Vector2[cVerts.Length];
                        cMesh.tangents = RootUtils.ProcessTangents(cTris, cMesh.normals, cMesh.uv, cVerts);
                        SaveMesh(ref cMesh, true);
                    }
                }
            }
            else if (CollisionType == CollisionTypeEnum.MeshCollision)
            {
                MC = Output.AddComponent<MeshCollider>();
                if (MC != null)
                {
                    MC.sharedMesh = xMesh;
                }
                if (MC != null)
                {
                    MC.convex = isCollisionConvex;
                    MC.isTrigger = isCollisionTrigger;
                }
            }
            else if (CollisionType == CollisionTypeEnum.BoxCollision)
            {
                //Primitive collider:
                GameObject BC_Obj = new GameObject("Primitive");
                BoxCollider BC = BC_Obj.AddComponent<BoxCollider>();
                BC_Obj.transform.position = node.pos;
                BC_Obj.transform.rotation = Quaternion.LookRotation(node.tangent);

                Vector3 BCCenter = default(Vector3);
                //				if(bStraightLineMatchStartEnd){
                //					if(tNode.bIsBridge && tNode.bIsBridgeMatched && tNode.BridgeCounterpartNode != null){
                //						BCCenter = ((tNode.pos - tNode.BridgeCounterpartNode.pos)*0.5f)+tNode.BridgeCounterpartNode.pos;
                //					}else if(tNode.idOnSpline < (tSpline.GetNodeCount()-1)){
                //						BCCenter = ((tNode.pos - tSpline.mNodes[tNode.idOnSpline+1].pos)*0.5f)+tSpline.mNodes[tNode.idOnSpline+1].pos;
                //					}else{
                //						
                //					}
                //					BCCenter.y -= VerticalRaise;
                //					BCCenter.y -= (mMaxHeight*0.5f);
                //				}else{
                Vector3 POS = default(Vector3);
                spline.GetSplineValueBoth(StartTime, out tVect1, out POS);
                //Goes right if not neg:
                tVect1 = (tVect1 + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x));
                spline.GetSplineValueBoth(EndTime, out tVect2, out POS);
                tVect2 = (tVect2 + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x));
                tVect1.y += verticalRaise;
                tVect2.y += verticalRaise;
                BCCenter = ((tVect1 - tVect2) * 0.5f) + tVect2;
                BCCenter.y += ((mMinY + mMaxY) * 0.5f);
                //				}

                BCCenter -= node.pos;
                BCCenter.z *= -1f;

                if (isBoxColliderFlippedOnX)
                {
                    BCCenter.z *= -1f;
                }
                if (isBoxColliderFlippedOnZ)
                {
                    BCCenter.x *= -1f;
                }

                //				

                Vector3 BCCenter2 = new Vector3(BCCenter.z, BCCenter.y, BCCenter.x);


                BCCenter2 += boxColliderOffset;


                BC.center = BCCenter2;

                tFloat1 = Vector3.Distance(node.pos, node.bridgeCounterpartNode.pos);

                if (isStretchedSize)
                {
                    BC.size = boxColliderSize;
                }
                else
                {
                    if (Axis == AxisTypeEnum.X)
                    {
                        BC.size = new Vector3(tFloat1, mMaxHeight, (mMaxZ - mMinZ));
                    }
                    else
                    {
                        BC.size = new Vector3((mMaxX - mMinX), mMaxHeight, tFloat1);
                    }
                    boxColliderSize = BC.size;
                }
                BC_Obj.transform.parent = Output.transform;
            }


            //Use prefab mats if no material override:
            MeshRenderer MR = Output.AddComponent<MeshRenderer>();
            if (SplinatedMaterial1 == null && !isMaterialOverriden)
            {
                MeshRenderer PrefabMR = tObj.GetComponent<MeshRenderer>();
                if (PrefabMR != null && PrefabMR.sharedMaterials != null)
                {
                    MR.materials = PrefabMR.sharedMaterials;
                }
            }
            else
            {
                //Else, use override mats:
                tIntBuffer1 = 0;
                if (SplinatedMaterial1 != null)
                {
                    tIntBuffer1 += 1;
                    if (SplinatedMaterial2 != null)
                    {
                        tIntBuffer1 += 1;
                    }
                }
                if (tIntBuffer1 > 0)
                {
                    Material[] tMats = new Material[tIntBuffer1];
                    if (SplinatedMaterial1 != null)
                    {
                        tMats[0] = SplinatedMaterial1;
                        if (SplinatedMaterial2 != null)
                        {
                            tMats[1] = SplinatedMaterial2;
                        }
                    }
                    MR.materials = tMats;
                }
            }

            mMaxX = mMaxX * 1.5f;
            mMinX = mMinX * 1.5f;
            mMaxY = mMaxY * 1.5f;
            mMinY = mMinY * 1.5f;
            mMaxZ = mMaxZ * 1.5f;
            mMinZ = mMinZ * 1.5f;

            StartPos = spline.GetSplineValue(StartTime);
            EndPos = spline.GetSplineValue(EndTime);

            //Destroy the instantiated prefab:
            Object.DestroyImmediate(tObj);
            Object.DestroyImmediate(Cap1);
            Object.DestroyImmediate(Cap2);

            Material[] fMats = MR.sharedMaterials;

            //Set the new object with the specified vertical raise:
            Output.transform.name = objectName;
            Output.transform.parent = masterObjTrans;
            if (EndCapStartObj != null)
            {
                EndCapStartObj.transform.parent = Output.transform;
                EndCapStartOutput = EndCapStartObj;

                MeshRenderer eMR = EndCapStartObj.GetComponent<MeshRenderer>();
                if (eMR == null)
                {
                    eMR = EndCapStartObj.AddComponent<MeshRenderer>();
                }
                if (eMR.sharedMaterials == null || (eMR.sharedMaterial != null && eMR.sharedMaterial.name.ToLower().Contains("default-diffuse")))
                {
                    eMR.sharedMaterials = fMats;
                }
            }
            if (EndCapEndObj != null)
            {
                EndCapEndObj.transform.parent = Output.transform;
                EndCapEndOutput = EndCapEndObj;
                MeshRenderer eMR = EndCapEndObj.GetComponent<MeshRenderer>();
                if (eMR == null)
                {
                    eMR = EndCapEndObj.AddComponent<MeshRenderer>();
                }
                if (eMR.sharedMaterials == null || (eMR.sharedMaterial != null && eMR.sharedMaterial.name.ToLower().Contains("default-diffuse")))
                {
                    eMR.sharedMaterials = fMats;
                }
            }

            if (_isCollecting)
            {
                node.spline.road.isTriggeringGC = true;
            }
        }


        /// <summary> Saves _mesh as an asset into /Mesh/Generated/Extrusions folder beside the /Asset folder </summary>
        private void SaveMesh(ref Mesh _mesh, bool _isCollider)
        {
            if (!node.spline.road.isSavingMeshes)
            {
                return;
            }

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");
            string folderName = Path.Combine(RoadEditorUtility.GetBasePath(),  "Mesh");
            folderName = Path.Combine(folderName, "Generated");
            folderName = Path.Combine(folderName ,"Extrusions");
            string roadName = node.spline.road.transform.name;
            string fileName = sceneName + "-" + roadName + "-" + objectName;
            string finalName = Path.Combine(folderName, fileName + ".asset");
            if (_isCollider)
            {
                finalName = Path.Combine(folderName, fileName + "-collider.asset");
            }

            string savingPath = Path.GetDirectoryName(Application.dataPath);
            savingPath = Path.Combine(savingPath, folderName);
            if (!System.IO.Directory.Exists(savingPath))
            {
                System.IO.Directory.CreateDirectory(savingPath);
            }

            finalName = EngineIntegration.GetUnityFilePath(finalName);
            EngineIntegration.CreateAsset(_mesh, finalName);
            EngineIntegration.SaveAssets();
        }


        private void DoStretch(ref Vector3[] _origVerts, ref Vector2[] _origUV, ref int[] _origTris, ref List<int> _maxVectorIndices, ref List<int> _minVectorIndices, float _maxDiff, out Vector3[] _verts, out Vector2[] _UV, out Vector3[] _normals, out int[] _tris)
        {
            Vector3 tVect1 = node.pos;
            Vector3 tVect2 = default(Vector3);

            //if(bStraightLineMatchStartEnd)
            //{
            //  if(tNode.bIsBridge && tNode.bIsBridgeMatched && tNode.BridgeCounterpartNode != null)
            //  {
            //      tVect2 = tNode.BridgeCounterpartNode.pos;
            //  }
            //  else if(tNode.idOnSpline < (tSpline.GetNodeCount()-1))
            //  {
            //	    tVect2 = tSpline.mNodes[tNode.idOnSpline+1].pos;	
            //  }
            //}

            Vector3 POS = default(Vector3);
            Vector3 tDir = node.tangent;

            //if(!bStraightLineMatchStartEnd){
            spline.GetSplineValueBoth(StartTime, out tVect1, out POS);
            //Goes right if not neg:
            tVect1 = (tVect1 + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x));

            spline.GetSplineValueBoth(EndTime, out tVect2, out POS);
            tVect2 = (tVect2 + new Vector3(horizontalSep * POS.normalized.z, 0, horizontalSep * -POS.normalized.x));

            tVect1.y += verticalRaise;
            tVect2.y += verticalRaise;

            tDir = spline.GetSplineValue(StartTime, true);
            //			}

            Matrix4x4 tMatrixStart = new Matrix4x4();
            Matrix4x4 tMatrixEnd = new Matrix4x4();
            int OrigMVL = _origVerts.Length;

            _verts = new Vector3[OrigMVL];
            _UV = new Vector2[OrigMVL];
            _normals = new Vector3[OrigMVL];
            _tris = new int[_origTris.Length];
            System.Array.Copy(_origVerts, _verts, OrigMVL);
            System.Array.Copy(_origTris, _tris, _origTris.Length);
            System.Array.Copy(_origUV, _UV, OrigMVL);

            tMatrixStart.SetTRS(tVect1, Quaternion.LookRotation(tDir), new Vector3(1f, 1f, 1f));
            tMatrixEnd.SetTRS(tVect2, Quaternion.LookRotation(tDir), new Vector3(1f, 1f, 1f));

            //Rotate and set vertex positions:
            float NewDiff = Vector3.Distance(tVect1, tVect2);
            float UVMod = NewDiff / _maxDiff;
            Vector3 xVect = default(Vector3);
            for (int index = 0; index < OrigMVL; index++)
            {
                xVect = _origVerts[index];
                if (_maxVectorIndices.Contains(index))
                {
                    _verts[index] = tMatrixEnd.MultiplyPoint3x4(xVect);
                }
                else
                {
                    _verts[index] = tMatrixStart.MultiplyPoint3x4(xVect);
                }

                if (RepeatUVType == RepeatUVTypeEnum.X)
                {
                    if (_origUV[index].x > stretchUVThreshold)
                    {
                        _UV[index].x = _origUV[index].x * UVMod;
                    }
                }
                else
                {
                    if (_origUV[index].y > stretchUVThreshold)
                    {
                        _UV[index].y = _origUV[index].y * UVMod;
                    }
                }
            }
        }


        private Vector3 GetAverageNormalToGround(GameObject _object)
        {
            Ray ray = default(Ray);
            RaycastHit[] rayHit = null;
            float hitY = 0f;
            int hitIndex = 0;
            Vector3 hitNormal = default(Vector3);

            Bounds bounds = _object.GetComponent<MeshFilter>().sharedMesh.bounds;

            Vector3[] tVects = new Vector3[8];
            tVects[0] = bounds.min;
            tVects[1] = bounds.max;
            tVects[2] = new Vector3(tVects[0].x, tVects[0].y, tVects[1].z);
            tVects[3] = new Vector3(tVects[0].x, tVects[1].y, tVects[0].z);
            tVects[4] = new Vector3(tVects[1].x, tVects[0].y, tVects[0].z);
            tVects[5] = new Vector3(tVects[0].x, tVects[1].y, tVects[1].z);
            tVects[6] = new Vector3(tVects[1].x, tVects[0].y, tVects[1].z);
            tVects[7] = new Vector3(tVects[1].x, tVects[1].y, tVects[0].z);

            List<Vector3> xVects = new List<Vector3>();

            for (int index = 0; index < 8; index++)
            {
                ray = new Ray(tVects[index] + new Vector3(0f, 1f, 0f), Vector3.down);
                rayHit = Physics.RaycastAll(ray);
                hitIndex = -1;
                hitY = -1f;
                if (rayHit.Length > 0)
                {
                    for (int g = 0; g < rayHit.Length; g++)
                    {
                        if (g == 0)
                        {
                            hitY = rayHit[g].point.y;
                            hitIndex = 0;
                        }
                        else
                        {
                            if (rayHit[g].point.y > hitY)
                            {
                                hitY = rayHit[g].point.y;
                                hitIndex = g;
                            }
                        }
                    }
                    xVects.Add(rayHit[hitIndex].normal);
                }
            }


            for (int index = 0; index < xVects.Count; index++)
            {
                hitNormal += xVects[index];
            }
            hitNormal /= xVects.Count;

            return hitNormal;
        }
    }
}
