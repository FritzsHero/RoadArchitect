using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    public class iConstructionMaker
    {
        #region "Vars"
        //Lanes:
        public List<Vector3> iBLane0L, iBLane0R;
        public List<Vector3> iBLane1L, iBLane1R;
        public List<Vector3> iBLane2L, iBLane2R;
        public List<Vector3> iBLane3L, iBLane3R;
        public List<Vector3> iFLane0L, iFLane0R;
        public List<Vector3> iFLane1L, iFLane1R;
        public List<Vector3> iFLane2L, iFLane2R;
        public List<Vector3> iFLane3L, iFLane3R;
        //Main plate:
        public List<Vector3> iBMainPlateL;
        public List<Vector3> iBMainPlateR;
        public List<Vector3> iFMainPlateL;
        public List<Vector3> iFMainPlateR;
        //Front marker plates:
        public List<Vector3> iBMarkerPlateL;
        public List<Vector3> iBMarkerPlateR;
        public List<Vector3> iFMarkerPlateL;
        public List<Vector3> iFMarkerPlateR;

        public List<Vector2> tempconstruction_R_RightTurn;
        public List<Vector2> tempconstruction_L_RightTurn;
        public List<Vector2> tempconstruction_R;
        public List<Vector2> tempconstruction_L;

        public float tempconstruction_InterStart;
        public float tempconstruction_InterEnd;

        public float tempconstruction_MinXR = 0f;
        public float tempconstruction_MaxXR = 0f;
        public float tempconstruction_MinXL = 0f;
        public float tempconstruction_MaxXL = 0f;

        public float tempconstruction_MinYR = 0f;
        public float tempconstruction_MaxYR = 0f;
        public float tempconstruction_MinYL = 0f;
        public float tempconstruction_MaxYL = 0f;

        public bool isTempConstructionProcessedInter1 = false;
        public bool isTempConstructionProcessedInter2 = false;
        public bool isBLane0Done = false;
        public bool isBLane1Done = false;
        public bool isBLane2Done = false;
        public bool isBLane3Done = false;
        public bool isFLane0Done = false;
        public bool isFLane1Done = false;
        public bool isFLane2Done = false;
        public bool isFLane3Done = false;
        public bool isBLane0DoneFinal = false;
        public bool isBLane1DoneFinal = false;
        public bool isBLane2DoneFinal = false;
        public bool isBLane3DoneFinal = false;
        public bool isFLane0DoneFinal = false;
        public bool isFLane1DoneFinal = false;
        public bool isFLane2DoneFinal = false;
        public bool isFLane3DoneFinal = false;
        public bool isBLane0DoneFinalThisRound = false;
        public bool isBLane1DoneFinalThisRound = false;
        public bool isBLane2DoneFinalThisRound = false;
        public bool isBLane3DoneFinalThisRound = false;
        public bool isFLane0DoneFinalThisRound = false;
        public bool isFLane1DoneFinalThisRound = false;
        public bool isFLane2DoneFinalThisRound = false;
        public bool isFLane3DoneFinalThisRound = false;
        public bool isFDone = false;
        public bool isBDone = false;
        public bool isFrontFirstRound = false;
        public bool isFrontFirstRoundTriggered = false;
        public bool isNode1RLTriggered = false;
        public bool isDepressDoneR = false;
        public bool isDepressDoneL = false;
        public bool isBackRRPassed = false;

        public Vector3 f0LAttempt = default(Vector3);
        public Vector3 f1LAttempt = default(Vector3);
        public Vector3 f2LAttempt = default(Vector3);
        public Vector3 f3LAttempt = default(Vector3);
        public Vector3 f0RAttempt = default(Vector3);
        public Vector3 f1RAttempt = default(Vector3);
        public Vector3 f2RAttempt = default(Vector3);
        public Vector3 f3RAttempt = default(Vector3);

        public List<Vector3> iBLane0Real;

        public Vector3 shoulderStartBL = default(Vector3);
        public Vector3 shoulderStartBR = default(Vector3);
        public Vector3 shoulderStartFL = default(Vector3);
        public Vector3 shoulderStartFR = default(Vector3);

        public Vector3 shoulderEndBL = default(Vector3);
        public Vector3 shoulderEndBR = default(Vector3);
        public Vector3 shoulderEndFL = default(Vector3);
        public Vector3 shoulderEndFR = default(Vector3);

        public int shoulderBLStartIndex = -1;
        public int shoulderBRStartIndex = -1;
        public int shoulderFLStartIndex = -1;
        public int shoulderFRStartIndex = -1;
        #endregion


        public void Nullify()
        {
            //Intersection construction:
            RootUtils.NullifyList(ref iBLane0L);
            RootUtils.NullifyList(ref iBLane0R);
            RootUtils.NullifyList(ref iBLane1L);
            RootUtils.NullifyList(ref iBLane1R);
            RootUtils.NullifyList(ref iBLane2L);
            RootUtils.NullifyList(ref iBLane2R);
            RootUtils.NullifyList(ref iBLane3L);
            RootUtils.NullifyList(ref iBLane3R);
            RootUtils.NullifyList(ref iFLane0L);
            RootUtils.NullifyList(ref iFLane0R);
            RootUtils.NullifyList(ref iFLane1L);
            RootUtils.NullifyList(ref iFLane1R);
            RootUtils.NullifyList(ref iFLane2L);
            RootUtils.NullifyList(ref iFLane2R);
            RootUtils.NullifyList(ref iFLane3L);
            RootUtils.NullifyList(ref iFLane3R);
            RootUtils.NullifyList(ref iBMainPlateL);
            RootUtils.NullifyList(ref iBMainPlateR);
            RootUtils.NullifyList(ref iFMainPlateL);
            RootUtils.NullifyList(ref iFMainPlateR);
            RootUtils.NullifyList(ref iBMarkerPlateL);
            RootUtils.NullifyList(ref iBMarkerPlateR);
            RootUtils.NullifyList(ref iFMarkerPlateL);
            RootUtils.NullifyList(ref iFMarkerPlateR);

            RootUtils.NullifyList(ref tempconstruction_R);
            RootUtils.NullifyList(ref tempconstruction_L);
        }


        public void ClampConstructionValues()
        {
            tempconstruction_InterStart = Mathf.Clamp01(tempconstruction_InterStart);
            tempconstruction_InterEnd = Mathf.Clamp01(tempconstruction_InterEnd);
        }


        public iConstructionMaker()
        {
            Nullify();

            iBLane0Real = new List<Vector3>();

            //Lanes:
            iBLane0L = new List<Vector3>();
            iBLane0R = new List<Vector3>();
            iBLane1L = new List<Vector3>();
            iBLane1R = new List<Vector3>();
            iBLane2L = new List<Vector3>();
            iBLane2R = new List<Vector3>();
            iBLane3L = new List<Vector3>();
            iBLane3R = new List<Vector3>();
            iFLane0L = new List<Vector3>();
            iFLane0R = new List<Vector3>();
            iFLane1L = new List<Vector3>();
            iFLane1R = new List<Vector3>();
            iFLane2L = new List<Vector3>();
            iFLane2R = new List<Vector3>();
            iFLane3L = new List<Vector3>();
            iFLane3R = new List<Vector3>();
            //Main plate:
            iBMainPlateL = new List<Vector3>();
            iBMainPlateR = new List<Vector3>();
            iFMainPlateL = new List<Vector3>();
            iFMainPlateR = new List<Vector3>();

            iBMarkerPlateL = new List<Vector3>();
            iBMarkerPlateR = new List<Vector3>();
            iFMarkerPlateL = new List<Vector3>();
            iFMarkerPlateR = new List<Vector3>();

            isTempConstructionProcessedInter1 = false;
            isTempConstructionProcessedInter2 = false;
            tempconstruction_MinXR = 20000000f;
            tempconstruction_MaxXR = 0f;
            tempconstruction_MinXL = 20000000f;
            tempconstruction_MaxXL = 0f;
            tempconstruction_MinYR = 20000000f;
            tempconstruction_MaxYR = 0f;
            tempconstruction_MinYL = 20000000f;
            tempconstruction_MaxYL = 0f;

            isBLane0Done = false;
            isBLane1Done = false;
            isBLane2Done = false;
            isBLane3Done = false;
            isFLane0Done = false;
            isFLane1Done = false;
            isFLane2Done = false;
            isFLane3Done = false;
        }
    }
}
