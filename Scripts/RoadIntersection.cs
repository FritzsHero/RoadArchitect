#region "Imports"
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion


namespace RoadArchitect
{
    public class RoadIntersection : MonoBehaviour
    {
        #region "Vars"
        [UnityEngine.Serialization.FormerlySerializedAs("Node1")]
        public SplineN node1;
        [UnityEngine.Serialization.FormerlySerializedAs("Node2")]
        public SplineN node2;

        [UnityEngine.Serialization.FormerlySerializedAs("Node1UID")]
        public string node1uID;
        [UnityEngine.Serialization.FormerlySerializedAs("Node2UID")]
        public string node2uID;

        //Unique ID
        [UnityEngine.Serialization.FormerlySerializedAs("UID")]
        protected string uID;
        [UnityEngine.Serialization.FormerlySerializedAs("tName")]
        public string intersectionName = "";

        [UnityEngine.Serialization.FormerlySerializedAs("bSameSpline")]
        public bool isSameSpline = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bDrawGizmo")]
        public bool isDrawingGizmo = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bFlipped")]
        public bool isFlipped = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bUseDefaultMaterials")]
        public bool isUsingDefaultMaterials = true;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_AutoUpdateIntersections")]
        public bool isAutoUpdatingIntersection = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bNode2B_LeftTurnLane")]
        public bool isNode2BLeftTurnLane = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bNode2B_RightTurnLane")]
        public bool isNode2BRightTurnLane = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bNode2F_LeftTurnLane")]
        public bool isNode2FLeftTurnLane = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bNode2F_RightTurnLane")]
        public bool isNode2FRightTurnLane = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bFirstSpecial_First")]
        public bool isFirstSpecialFirst = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFirstSpecial_Last")]
        public bool isFirstSpecialLast = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bSecondSpecial_First")]
        public bool isSecondSpecialFirst = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bSecondSpecial_Last")]
        public bool isSecondSpecialLast = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR1")]
        public bool isCornerRR1Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR2")]
        public bool isCornerRR2Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL1")]
        public bool isCornerRL1Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL2")]
        public bool isCornerRL2Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR1")]
        public bool isCornerLR1Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR2")]
        public bool isCornerLR2Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL1")]
        public bool isCornerLL1Enabled = false;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL2")]
        public bool isCornerLL2Enabled = false;

        #region "Marker materials"
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerCenter1")]
        public Material markerCenter1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerCenter2")]
        public Material markerCenter2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerCenter3")]
        public Material markerCenter3 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Stretch1")]
        public Material markerExtStretch1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Stretch2")]
        public Material markerExtStretch2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Stretch3")]
        public Material markerExtStretch3 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Tiled1")]
        public Material markerExtTiled1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Tiled2")]
        public Material markerExtTiled2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("MarkerExt_Tiled3")]
        public Material markerExtTiled3 = null;
        #endregion

        #region "Lane materials"
        [UnityEngine.Serialization.FormerlySerializedAs("Lane0Mat1")]
        public Material lane0Mat1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane0Mat2")]
        public Material lane0Mat2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat1")]
        public Material lane1Mat1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat2")]
        public Material lane1Mat2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat1")]
        public Material lane2Mat1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat2")]
        public Material lane2Mat2 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane3Mat1")]
        public Material lane3Mat1 = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane3Mat2")]
        public Material lane3Mat2 = null;

        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat1_Disabled")]
        public Material lane1Mat1Disabled = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat2_Disabled")]
        public Material lane1Mat2Disabled = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat1_DisabledActive")]
        public Material lane1Mat1DisabledActive = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane1Mat2_DisabledActive")]
        public Material lane1Mat2DisabledActive = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat1_Disabled")]
        public Material lane2Mat1Disabled = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat2_Disabled")]
        public Material lane2Mat2Disabled = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat1_DisabledActive")]
        public Material lane2Mat1DisabledActive = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat2_DisabledActive")]
        public Material lane2Mat2DisabledActive = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat1_DisabledActiveR")]
        public Material lane2Mat1DisabledActiveR = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane2Mat2_DisabledActiveR")]
        public Material lane2Mat2DisabledActiveR = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane3Mat1_Disabled")]
        public Material lane3Mat1Disabled = null;
        [UnityEngine.Serialization.FormerlySerializedAs("Lane3Mat2_Disabled")]
        public Material lane3Mat2Disabled = null;
        #endregion

        //Width of the largest of road connected
        /// <summary> 10 * 1.25f = intersectionWidth; Never written, only read </summary>
        [UnityEngine.Serialization.FormerlySerializedAs("IntersectionWidth")]
        public int intersectionWidth = 10;
        /// <summary> Amount of lanes from road of node1 </summary>
        [UnityEngine.Serialization.FormerlySerializedAs("Lanes")]
        public int lanesAmount;
        [UnityEngine.Serialization.FormerlySerializedAs("IgnoreSide")]
        public int ignoreSide = -1;
        [UnityEngine.Serialization.FormerlySerializedAs("IgnoreCorner")]
        public int ignoreCorner = -1;

        [UnityEngine.Serialization.FormerlySerializedAs("iType")]
        public IntersectionTypeEnum intersectionType = IntersectionTypeEnum.FourWay;
        [UnityEngine.Serialization.FormerlySerializedAs("iStopType")]
        public iStopTypeEnum intersectionStopType = iStopTypeEnum.StopSign_AllWay;
        [UnityEngine.Serialization.FormerlySerializedAs("rType")]
        public RoadTypeEnum roadType = RoadTypeEnum.NoTurnLane;
        [UnityEngine.Serialization.FormerlySerializedAs("lType")]
        public LightTypeEnum lightType = LightTypeEnum.Timed;

        #region "CalculationData"
        [UnityEngine.Serialization.FormerlySerializedAs("CornerPoints")]
        public CornerPositionMaker[] cornerPoints;

        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR")]
        public Vector3 cornerRR;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR_Outer")]
        public Vector3 cornerRROuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR_RampOuter")]
        public Vector3 cornerRRRampOuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL")]
        public Vector3 cornerRL;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL_Outer")]
        public Vector3 cornerRLOuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL_RampOuter")]
        public Vector3 cornerRLRampOuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR")]
        public Vector3 cornerLR;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR_Outer")]
        public Vector3 cornerLROuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR_RampOuter")]
        public Vector3 cornerLRRampOuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL")]
        public Vector3 cornerLL;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL_Outer")]
        public Vector3 cornerLLOuter;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL_RampOuter")]
        public Vector3 cornerLLRampOuter;

        [UnityEngine.Serialization.FormerlySerializedAs("CornerRR_2D")]
        public Vector2 cornerRR2D;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerRL_2D")]
        public Vector2 cornerRL2D;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLR_2D")]
        public Vector2 cornerLR2D;
        [UnityEngine.Serialization.FormerlySerializedAs("CornerLL_2D")]
        public Vector2 cornerLL2D;

        [UnityEngine.Serialization.FormerlySerializedAs("fCornerLR_CornerRR")]
        public Vector3[] cornerLRCornerRR;
        [UnityEngine.Serialization.FormerlySerializedAs("fCornerLL_CornerRL")]
        public Vector3[] cornerLLCornerRL;
        [UnityEngine.Serialization.FormerlySerializedAs("fCornerLL_CornerLR")]
        public Vector3[] cornerLLCornerLR;
        [UnityEngine.Serialization.FormerlySerializedAs("fCornerRL_CornerRR")]
        public Vector3[] cornerRLCornerRR;
        #endregion

        [UnityEngine.Serialization.FormerlySerializedAs("GradeMod")]
        public float gradeMod = 0.375f;
        [UnityEngine.Serialization.FormerlySerializedAs("GradeModNegative")]
        public float gradeModNegative = 0.75f;
        [UnityEngine.Serialization.FormerlySerializedAs("ScalingSense")]
        public float scalingSense = 3f;
        [UnityEngine.Serialization.FormerlySerializedAs("OddAngle")]
        public float oddAngle;
        [UnityEngine.Serialization.FormerlySerializedAs("EvenAngle")]
        public float evenAngle;
        [UnityEngine.Serialization.FormerlySerializedAs("MaxInterDistance")]
        public float maxInterDistance = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("MaxInterDistanceSQ")]
        public float maxInterDistanceSQ = 0f;
        [UnityEngine.Serialization.FormerlySerializedAs("Height")]
        public float height = 50000f;
        [UnityEngine.Serialization.FormerlySerializedAs("SignHeight")]
        public float signHeight = -2000f;

        #region "Traffic Light Vars"
        [UnityEngine.Serialization.FormerlySerializedAs("bLightsEnabled")]
        public bool isLightsEnabled = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bRegularPoleAlignment")]
        public bool isRegularPoleAlignment = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bTrafficPoleStreetLight")]
        public bool isTrafficPoleStreetLight = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bTrafficLightGray")]
        public bool isTrafficLightGray = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bLeftTurnYieldOnGreen")]
        public bool isLeftTurnYieldOnGreen = true;
        [UnityEngine.Serialization.FormerlySerializedAs("StreetLight_Range")]
        public float streetLightRange = 30f;
        [UnityEngine.Serialization.FormerlySerializedAs("StreetLight_Intensity")]
        public float streetLightIntensity = 1f;
        [UnityEngine.Serialization.FormerlySerializedAs("StreetLight_Color")]
        public Color streetLightColor = new Color(1f, 0.7451f, 0.27451f, 1f);
        [UnityEngine.Serialization.FormerlySerializedAs("FixedTimeIndex")]
        private int fixedTimeIndex = 0;
        [UnityEngine.Serialization.FormerlySerializedAs("LightsRR")]
        public TrafficLightController lightsRR;
        [UnityEngine.Serialization.FormerlySerializedAs("LightsRL")]
        public TrafficLightController lightsRL;
        [UnityEngine.Serialization.FormerlySerializedAs("LightsLL")]
        public TrafficLightController lightsLL;
        [UnityEngine.Serialization.FormerlySerializedAs("LightsLR")]
        public TrafficLightController lightsLR;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_FixedTime_RegularLightLength")]
        public float fixedTimeRegularLightLength = 10f;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_FixedTime_LeftTurnLightLength")]
        public float fixedTimeLeftTurnLightLength = 5f;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_FixedTime_AllRedLightLength")]
        public float fixedTimeAllRedLightLength = 1f;
        [UnityEngine.Serialization.FormerlySerializedAs("opt_FixedTime_YellowLightLength")]
        public float fixedTimeYellowLightLength = 2f;
        [UnityEngine.Serialization.FormerlySerializedAs("FixedTimeSequenceList")]
        public List<TrafficLightSequence> fixedTimeSequenceList;
        #endregion
        #endregion


        public enum IntersectionTypeEnum { ThreeWay, FourWay };
        public enum iStopTypeEnum { StopSign_AllWay, TrafficLight1, None, TrafficLight2 };
        public enum RoadTypeEnum { NoTurnLane, TurnLane, BothTurnLanes };
        public enum LightTypeEnum { Timed, Sensors };


        // A struct may be better and faster
        public class CornerPositionMaker
        {
            public Vector3 position;
            public Quaternion rotation;
            [UnityEngine.Serialization.FormerlySerializedAs("DirectionFromCenter")]
            public Vector3 directionFromCenter;
        }


        [UnityEngine.Serialization.FormerlySerializedAs("BoundsRect")]
        private Construction2DRect boundsRect;


        #region "Setup"
        /// <summary> Links nodes and intersection </summary>
        public void Setup(SplineN _node1, SplineN _node2)
        {
            if (_node1.spline == _node2.spline)
            {
                isSameSpline = true;
            }

            if (isSameSpline)
            {
                if (_node1.idOnSpline < _node2.idOnSpline)
                {
                    node1 = _node1;
                    node2 = _node2;
                }
                else
                {
                    node1 = _node2;
                    node2 = _node1;
                }
            }
            else
            {
                node1 = _node1;
                node2 = _node2;
            }

            node1.intersectionOtherNode = node2;
            node2.intersectionOtherNode = node1;

            node1.ToggleHideFlags(true);
            node2.ToggleHideFlags(true);

            node1uID = node1.uID;
            node2uID = node2.uID;
            node1.isIntersection = true;
            node2.isIntersection = true;
            node1.intersection = this;
            node2.intersection = this;
        }


        /// <summary> Deletes Meshes based on road name and the centermarker if _node is intersections node1 </summary>
        public void DeleteRelevantChildren(SplineN _node, string _string)
        {
            Transform transformChild;
            int childCount = transform.childCount;
            for (int index = childCount - 1; index >= 0; index--)
            {
                transformChild = transform.GetChild(index);
                if (transformChild.name.ToLower().Contains(_string.ToLower()))
                {
                    Object.DestroyImmediate(transformChild.gameObject);
                }
                else if (_node == node1)
                {
                    if (transformChild.name.ToLower().Contains("centermarkers"))
                    {
                        Object.DestroyImmediate(transformChild.gameObject);
                    }
                }
            }
        }
        #endregion


        #region "Utility"
        /// <summary> Attach other spline to PiggyBacks if not same spline and setup </summary>
        public void UpdateRoads()
        {
            if (!isSameSpline)
            {
                SplineC[] piggys = new SplineC[1];
                piggys[0] = node2.spline;
                node1.spline.road.PiggyBacks = piggys;
                node1.spline.TriggerSetup();
            }
            else
            {
                node1.spline.TriggerSetup();
            }
        }


        public void ConstructBoundsRect()
        {
            boundsRect = null;
            boundsRect = new Construction2DRect(new Vector2(cornerRR.x, cornerRR.z), new Vector2(cornerRL.x, cornerRL.z), new Vector2(cornerLR.x, cornerLR.z), new Vector2(cornerLL.x, cornerLL.z));
        }


        /// <summary> Creates boundsRect if null and returns true if _vector is inside the rect </summary>
        public bool Contains(ref Vector3 _vector)
        {
            Vector2 vector2D = new Vector2(_vector.x, _vector.z);
            if (boundsRect == null)
            {
                ConstructBoundsRect();
            }
            return boundsRect.Contains(ref vector2D);
        }


        private bool ContainsLineOld(Vector3 _vector1, Vector3 _vector2, int _lineDef = 30)
        {
            int MaxDef = _lineDef;
            float MaxDefF = (float)MaxDef;

            Vector3[] tVects = new Vector3[MaxDef];

            tVects[0] = _vector1;
            float mMod = 0f;
            float fcounter = 1f;
            for (int index = 1; index < (MaxDef - 1); index++)
            {
                mMod = fcounter / MaxDefF;
                tVects[index] = ((_vector2 - _vector1) * mMod) + _vector1;
                fcounter += 1f;
            }
            tVects[MaxDef - 1] = _vector2;

            Vector2 xVect = default(Vector2);
            for (int index = 0; index < MaxDef; index++)
            {
                xVect = new Vector2(tVects[index].x, tVects[index].z);
                if (boundsRect.Contains(ref xVect))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary> Returns true when the Vectors or the line between them are inside the intersection </summary>
        public bool ContainsLine(Vector3 _vector1, Vector3 _vector2)
        {
            Vector2 tVectStart = new Vector2(_vector1.x, _vector1.z);
            Vector2 tVectEnd = new Vector2(_vector2.x, _vector2.z);
            bool bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerRR2D, ref cornerRL2D);
            if (bIntersects)
            {
                return true;
            }
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerRL2D, ref cornerLL2D);
            if (bIntersects)
            {
                return true;
            }
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerLL2D, ref cornerLR2D);
            if (bIntersects)
            {
                return true;
            }
            bIntersects = Intersects2D(ref tVectStart, ref tVectEnd, ref cornerLR2D, ref cornerRR2D);
            return bIntersects;
        }


        // Returns true if the lines intersect, otherwise false. If the lines
        // intersect, intersectionPoint holds the intersection point.
        private static bool Intersects2D(ref Vector2 _line1S, ref Vector2 _line1E, ref Vector2 _line2S, ref Vector2 _line2E)
        {
            float firstLineSlopeX, firstLineSlopeY, secondLineSlopeX, secondLineSlopeY;

            firstLineSlopeX = _line1E.x - _line1S.x;
            firstLineSlopeY = _line1E.y - _line1S.y;

            secondLineSlopeX = _line2E.x - _line2S.x;
            secondLineSlopeY = _line2E.y - _line2S.y;

            float s, t;
            s = (-firstLineSlopeY * (_line1S.x - _line2S.x) + firstLineSlopeX * (_line1S.y - _line2S.y)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);
            t = (secondLineSlopeX * (_line1S.y - _line2S.y) - secondLineSlopeY * (_line1S.x - _line2S.x)) / (-secondLineSlopeX * firstLineSlopeY + firstLineSlopeX * secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                return true;
            }
            // No collision
            return false;
        }
        #endregion


        #region "Gizmos"
        private void OnDrawGizmos()
        {
            if (!isDrawingGizmo)
            {
                return;
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position + new Vector3(0f, 5f, 0f), new Vector3(2f, 11f, 2f));
        }
        #endregion


        #region "Traffic light controlling"
        private void Start()
        {
            lightsRR.Setup(isLeftTurnYieldOnGreen);
            lightsRL.Setup(isLeftTurnYieldOnGreen);
            lightsLL.Setup(isLeftTurnYieldOnGreen);
            lightsLR.Setup(isLeftTurnYieldOnGreen);
            if (lightType == LightTypeEnum.Timed)
            {
                CreateFixedSequence();
                FixedTimeIncrement();
            }
            else
            {
                //Do your custom stuff
                //A Traffic addon could include sensor mode.
            }
        }


        private void CreateFixedSequence()
        {
            TrafficLightSequence SequenceMaker = null;
            fixedTimeSequenceList = new List<TrafficLightSequence>();
            if (roadType != RoadTypeEnum.NoTurnLane)
            {
                SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.LeftTurn, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeLeftTurnLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }
            if (roadType != RoadTypeEnum.NoTurnLane)
            {
                SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.LeftTurn, TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Regular, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeRegularLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Regular, TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);

            if (roadType != RoadTypeEnum.NoTurnLane)
            {
                SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.LeftTurn, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeLeftTurnLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }
            if (roadType != RoadTypeEnum.NoTurnLane)
            {
                SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.LeftTurn, TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
                fixedTimeSequenceList.Add(SequenceMaker);
            }
            SequenceMaker = new TrafficLightSequence(true, TrafficLightController.iLightControllerEnum.Red, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Regular, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeRegularLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Regular, TrafficLightController.iLightSubStatusEnum.Yellow, fixedTimeYellowLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
            SequenceMaker = new TrafficLightSequence(false, TrafficLightController.iLightControllerEnum.Red, TrafficLightController.iLightSubStatusEnum.Green, fixedTimeAllRedLightLength);
            fixedTimeSequenceList.Add(SequenceMaker);
        }


        private IEnumerator TrafficLightFixedUpdate(float _time)
        {
            yield return new WaitForSeconds(_time);
            FixedTimeIncrement();
        }


        /// <summary> Executes the next traffic light sequence and shedules a new increment </summary>
        private void FixedTimeIncrement()
        {
            TrafficLightSequence SequenceMaker = fixedTimeSequenceList[fixedTimeIndex];
            fixedTimeIndex += 1;
            if (fixedTimeIndex > (fixedTimeSequenceList.Count - 1))
            {
                fixedTimeIndex = 0;
            }

            TrafficLightController lights1 = null;
            TrafficLightController lights2 = null;

            TrafficLightController lightsOuter1 = null;
            TrafficLightController lightsOuter2 = null;

            if (SequenceMaker.isLightMasterPath1)
            {
                lights1 = lightsRL;
                lights2 = lightsLR;

                if (isFlipped)
                {
                    lightsOuter1 = lightsRR;
                    lightsOuter2 = lightsLL;
                }
                else
                {
                    lightsOuter1 = lightsRR;
                    lightsOuter2 = lightsLL;
                }
            }
            else
            {
                if (isFlipped)
                {
                    lights1 = lightsRR;
                    lights2 = lightsLL;
                }
                else
                {
                    lights1 = lightsRR;
                    lights2 = lightsLL;
                }

                lightsOuter1 = lightsRL;
                lightsOuter2 = lightsLR;
            }

            TrafficLightController.iLightControllerEnum LCE = SequenceMaker.lightController;
            TrafficLightController.iLightSubStatusEnum LCESub = SequenceMaker.lightSubcontroller;

            if (LCE == TrafficLightController.iLightControllerEnum.Regular)
            {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.Regular, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.Regular, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
            }
            else if (LCE == TrafficLightController.iLightControllerEnum.LeftTurn)
            {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.LeftTurn, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.LeftTurn, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.RightTurn, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.RightTurn, LCESub, isLightsEnabled);
            }
            else if (LCE == TrafficLightController.iLightControllerEnum.Red)
            {
                lights1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lights2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter1.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
                lightsOuter2.UpdateLights(TrafficLightController.iLightStatusEnum.Red, LCESub, isLightsEnabled);
            }

            //Debug.Log ("Starting: " + SMaker.ToString());
            StartCoroutine(TrafficLightFixedUpdate(SequenceMaker.time));
        }
        #endregion


        #region "Materials"
        public void ResetMaterialsAll()
        {
            ResetCenterMaterials(false);
            ResetExtStrechtedMaterials(false);
            ResetExtTiledMaterials(false);
            ResetLanesMaterials(false);
            UpdateMaterials();
        }


        public void ResetCenterMaterials(bool _isUpdate = true)
        {
            string lanesNumber = "-2L";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
            {
                lanesNumber = "-4L";
            }
            else if (lanesAmount == 6)
            {
                lanesNumber = "-6L";
            }
            if (intersectionType == IntersectionTypeEnum.ThreeWay)
            {
                lanesNumber += "-3";
                if (node1.idOnSpline < 2 || node2.idOnSpline < 2)
                {
                    //if(isFirstSpecialFirst || isFirstSpecialLast)
                    //{
                    //Reverse if from node 0
                    //stands for "Center Reversed"
                    lanesNumber += "-crev";
                    //}
                }
            }

            string basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes)
            {
                markerCenter1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-Both" + lanesNumber + ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane)
            {
                markerCenter1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-Left" + lanesNumber + ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane)
            {
                markerCenter1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterCenter-None" + lanesNumber + ".mat");
                markerCenter2 = null;
                markerCenter3 = null;
            }
            if (_isUpdate)
            {
                UpdateMaterials();
            }
        }


        public void ResetExtStrechtedMaterials(bool _isUpdate = true)
        {
            string lanesNumber = "-2L";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
            {
                lanesNumber = "-4L";
            }
            else if (lanesAmount == 6)
            {
                lanesNumber = "-6L";
            }

            string basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes)
            {
                markerExtStretch1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-Both" + lanesNumber + ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane)
            {
                markerExtStretch1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-Left" + lanesNumber + ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane)
            {
                markerExtStretch1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterStretch-None" + lanesNumber + ".mat");
                markerExtStretch2 = null;
                markerExtStretch3 = null;
            }
            if (_isUpdate)
            {
                UpdateMaterials();
            }
        }


        public void ResetExtTiledMaterials(bool _isUpdate = true)
        {
            string basePath = RoadEditorUtility.GetBasePath();

            if (roadType == RoadTypeEnum.BothTurnLanes)
            {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane)
            {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane)
            {
                markerExtTiled1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Road1.mat");
                markerExtTiled2 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/RoadDetailOverlay1.mat");
                markerExtTiled3 = null;
            }
            if (_isUpdate)
            {
                UpdateMaterials();
            }
        }


        public void ResetLanesMaterials(bool _isUpdate = true)
        {
            string lanesNumber = "";
            lanesAmount = node1.spline.road.laneAmount;
            if (lanesAmount == 4)
            {
                lanesNumber = "-4L";
            }
            else if (lanesAmount == 6)
            {
                lanesNumber = "-6L";
            }

            string basePath = RoadEditorUtility.GetBasePath();

            if (intersectionType == IntersectionTypeEnum.ThreeWay)
            {
                lane1Mat1Disabled = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabled.mat");
                lane1Mat2Disabled = null;
                if (roadType == RoadTypeEnum.BothTurnLanes)
                {
                    lane1Mat1DisabledActive = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuterRR.mat");
                    lane1Mat2DisabledActive = null;
                    lane2Mat1Disabled = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledR.mat");
                    lane2Mat2Disabled = null;
                }
                else
                {
                    lane2Mat1Disabled = null;
                    lane2Mat2Disabled = null;
                    lane2Mat1DisabledActive = null;
                    lane2Mat2DisabledActive = null;
                }
                lane2Mat1DisabledActive = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuter" + lanesNumber + ".mat");
                lane2Mat2DisabledActive = null;
                if (roadType == RoadTypeEnum.BothTurnLanes)
                {
                    lane2Mat1DisabledActiveR = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledOuterR.mat");
                    lane2Mat2DisabledActiveR = null;
                    lane3Mat1Disabled = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterLaneDisabledR.mat");
                    lane3Mat2Disabled = null;
                }
                else
                {
                    lane2Mat1DisabledActiveR = null;
                    lane2Mat2DisabledActiveR = null;
                    lane3Mat1Disabled = null;
                    lane3Mat2Disabled = null;
                }
            }
            else
            {
                lane1Mat1Disabled = null;
                lane1Mat2Disabled = null;
                lane2Mat1Disabled = null;
                lane2Mat2Disabled = null;
                lane2Mat1DisabledActive = null;
                lane2Mat2DisabledActive = null;
                lane2Mat1DisabledActiveR = null;
                lane2Mat2DisabledActiveR = null;
                lane3Mat1Disabled = null;
                lane3Mat2Disabled = null;
            }

            if (roadType == RoadTypeEnum.BothTurnLanes)
            {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" + lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
                lane1Mat2 = null;
                lane2Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR" + lanesNumber + ".mat");
                lane2Mat2 = null;
                lane3Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR.mat");
                lane3Mat2 = null;
            }
            else if (roadType == RoadTypeEnum.TurnLane)
            {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" + lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
                lane1Mat2 = null;
                lane2Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteR" + lanesNumber + ".mat");
                lane2Mat2 = null;
                lane3Mat1 = null;
                lane3Mat2 = null;
            }
            else if (roadType == RoadTypeEnum.NoTurnLane)
            {
                lane0Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterWhiteLYellowR" + lanesNumber + ".mat");
                lane0Mat2 = null;
                lane1Mat1 = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Markers/InterYellowLWhiteR" + lanesNumber + ".mat");
                lane1Mat2 = null;
                lane2Mat1 = null;
                lane2Mat2 = null;
                lane3Mat1 = null;
                lane3Mat2 = null;
            }

            if (_isUpdate)
            {
                UpdateMaterials();
            }
        }


        private void ApplyMaterials(List<MeshRenderer> _meshRenderers, Material _material1 = null, Material _material2 = null, Material _material3 = null)
        {
            if (_meshRenderers != null && _meshRenderers.Count > 0)
            {
                List<Material> MarkerExtStretchMats = new List<Material>();

                if (_material1 != null)
                {
                    MarkerExtStretchMats.Add(_material1);
                    if (_material2 != null)
                    {
                        MarkerExtStretchMats.Add(_material2);
                        if (_material3 != null)
                        {
                            MarkerExtStretchMats.Add(_material3);
                        }
                    }
                }

                Material[] meshMaterials = MarkerExtStretchMats.ToArray();
                for (int i = 0; i < _meshRenderers.Count; i++)
                {
                    _meshRenderers[i].materials = meshMaterials;
                }
            }
        }


        public void UpdateMaterials()
        {
            int childCount = transform.childCount;
            List<MeshRenderer> extStretchMR = new List<MeshRenderer>();
            List<MeshRenderer> extTiledMR = new List<MeshRenderer>();
            MeshRenderer centerMR = null;
            List<MeshRenderer> lane0MR = new List<MeshRenderer>();
            List<MeshRenderer> lane1MR = new List<MeshRenderer>();
            List<MeshRenderer> lane2MR = new List<MeshRenderer>();
            List<MeshRenderer> lane3MR = new List<MeshRenderer>();
            List<MeshRenderer> laneD1MR = new List<MeshRenderer>();
            List<MeshRenderer> laneD3MR = new List<MeshRenderer>();
            List<MeshRenderer> laneDA2MR = new List<MeshRenderer>();
            List<MeshRenderer> laneDAR2MR = new List<MeshRenderer>();
            List<MeshRenderer> laneD2MR = new List<MeshRenderer>();
            List<MeshRenderer> laneDA1MR = new List<MeshRenderer>();

            MeshRenderer childMesh;
            string transformName = "";
            for (int i = 0; i < childCount; i++)
            {
                childMesh = transform.GetChild(i).GetComponent<MeshRenderer>();
                if(childMesh == null)
                {
                    continue;
                }
                
                transformName = childMesh.transform.name.ToLower();
                if (transformName.Contains("-stretchext"))
                {
                    extStretchMR.Add(childMesh);
                    continue;
                }
                if (transformName.Contains("-tiledext"))
                {
                    extTiledMR.Add(childMesh);
                    continue;
                }
                if (transformName.Contains("centermarkers"))
                {
                    centerMR = childMesh;
                    continue;
                }
                if (transformName.Contains("lane0"))
                {
                    lane0MR.Add(childMesh);
                    continue;
                }
                if (transformName.Contains("lane1"))
                {
                    lane1MR.Add(childMesh);
                    continue;
                }
                if (transformName.Contains("lane2"))
                {
                    lane2MR.Add(childMesh);
                    continue;
                }
                if (transformName.Contains("lane3"))
                {
                    lane3MR.Add(childMesh);
                    continue;
                }
                if (intersectionType == IntersectionTypeEnum.ThreeWay)
                {
                    if (transformName.Contains("laned1"))
                    {
                        laneD1MR.Add(childMesh);
                        continue;
                    }
                    if (transformName.Contains("laned3"))
                    {
                        laneD3MR.Add(childMesh);
                        continue;
                    }
                    if (transformName.Contains("laneda2"))
                    {
                        laneDA2MR.Add(childMesh);
                        continue;
                    }
                    if (transformName.Contains("lanedar2"))
                    {
                        laneDAR2MR.Add(childMesh);
                        continue;
                    }
                    if (transformName.Contains("laned2"))
                    {
                        laneD2MR.Add(childMesh);
                        continue;
                    }
                    if (transformName.Contains("laneda1"))
                    {
                        laneDA1MR.Add(childMesh);
                        continue;
                    }
                }
            }

            ApplyMaterials(extStretchMR, markerExtStretch1, markerExtStretch2, markerExtStretch3);
            ApplyMaterials(extTiledMR, markerExtTiled1, markerExtTiled2, markerExtTiled3);

            // Center only uses 1 mesh renderer
            if (centerMR != null)
            {
                List<Material> centerMats = new List<Material>();

                if (markerCenter1 != null)
                {
                    centerMats.Add(markerCenter1);
                    if (markerCenter2 != null)
                    {
                        centerMats.Add(markerCenter2);
                        if (markerCenter3 != null)
                        {
                            centerMats.Add(markerCenter3);
                        }
                    }
                }
                
                centerMR.materials = centerMats.ToArray();
            }


            ApplyMaterials(lane0MR, lane0Mat1, lane0Mat2);
            ApplyMaterials(lane1MR, lane1Mat1, lane1Mat2);
            ApplyMaterials(lane2MR, lane2Mat1, lane2Mat2);
            ApplyMaterials(lane3MR, lane3Mat1, lane3Mat2);
            ApplyMaterials(laneD1MR, lane1Mat1Disabled, lane1Mat2Disabled);
            ApplyMaterials(laneD3MR, lane3Mat1Disabled, lane3Mat2Disabled);
            ApplyMaterials(laneDA2MR, lane2Mat1DisabledActive, lane2Mat2DisabledActive);
            ApplyMaterials(laneDAR2MR, lane2Mat1DisabledActiveR, lane2Mat2DisabledActiveR);
            ApplyMaterials(laneD2MR, lane2Mat1Disabled, lane2Mat2Disabled);
            ApplyMaterials(laneDA1MR, lane1Mat1DisabledActive, lane1Mat2DisabledActive);
        }
        #endregion


        #region "TrafficLights"
        public void ToggleTrafficLightPoleColor()
        {
            string basePath = RoadEditorUtility.GetBasePath();

            Material trafficLightMaterial = null;
            if (isTrafficLightGray)
            {
                trafficLightMaterial = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Signs/InterTLB2.mat");
            }
            else
            {
                trafficLightMaterial = RoadEditorUtility.LoadMaterial(basePath + "/Materials/Signs/InterTLB1.mat");
            }
            int childCount = transform.childCount;
            string transformName = "";
            MeshRenderer MR = null;
            Material[] materials = new Material[1];
            materials[0] = trafficLightMaterial;
            for (int index = 0; index < childCount; index++)
            {
                transformName = transform.GetChild(index).name.ToLower();
                if (transformName.Contains("trafficlight"))
                {
                    MR = transform.GetChild(index).GetComponent<MeshRenderer>();
                    MR.materials = materials;
                }
            }
        }


        public void TogglePointLights(bool _isLightsEnabled)
        {
            isLightsEnabled = _isLightsEnabled;
            int cCount = transform.childCount;
            Light[] fLights = null;
            Transform tTrans = null;
            for (int index = 0; index < cCount; index++)
            {
                if (transform.GetChild(index).name.ToLower().Contains("trafficlight"))
                {
                    tTrans = transform.GetChild(index);
                    int childCount = tTrans.childCount;
                    for (int k = 0; k < childCount; k++)
                    {
                        if (tTrans.GetChild(k).name.ToLower().Contains("streetlight"))
                        {
                            fLights = tTrans.GetChild(k).GetComponentsInChildren<Light>();
                            if (fLights != null)
                            {
                                for (int j = 0; j < fLights.Length; j++)
                                {
                                    fLights[j].enabled = isLightsEnabled;
                                    fLights[j].range = streetLightRange;
                                    fLights[j].intensity = streetLightIntensity;
                                    fLights[j].color = streetLightColor;
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }


        public void ResetStreetLightSettings()
        {
            streetLightRange = 30f;
            streetLightIntensity = 1f;
            streetLightColor = new Color(1f, 0.7451f, 0.27451f, 1f);
            TogglePointLights(isLightsEnabled);
        }
        #endregion
    }
}
