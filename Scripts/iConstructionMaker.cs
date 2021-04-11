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

        [UnityEngine.Serialization.FormerlySerializedAs("tempconstruction_HasProcessed_Inter1")]
        public bool isTempConstructionProcessedInter1 = false;
        [UnityEngine.Serialization.FormerlySerializedAs("tempconstruction_HasProcessed_Inter2")]
        public bool isTempConstructionProcessedInter2 = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane0Done")]
        public bool isBLane0Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane1Done")]
        public bool isBLane1Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane2Done")]
        public bool isBLane2Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane3Done")]
        public bool isBLane3Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane0Done")]
        public bool isFLane0Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane1Done")]
        public bool isFLane1Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane2Done")]
        public bool isFLane2Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane3Done")]
        public bool isFLane3Done = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane0Done_Final")]
        public bool isBLane0DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane1Done_Final")]
        public bool isBLane1DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane2Done_Final")]
        public bool isBLane2DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane3Done_Final")]
        public bool isBLane3DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane0Done_Final")]
        public bool isFLane0DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane1Done_Final")]
        public bool isFLane1DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane2Done_Final")]
        public bool isFLane2DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane3Done_Final")]
        public bool isFLane3DoneFinal = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane0Done_Final_ThisRound")]
        public bool isBLane0DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane1Done_Final_ThisRound")]
        public bool isBLane1DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane2Done_Final_ThisRound")]
        public bool isBLane2DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBLane3Done_Final_ThisRound")]
        public bool isBLane3DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane0Done_Final_ThisRound")]
        public bool isFLane0DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane1Done_Final_ThisRound")]
        public bool isFLane1DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane2Done_Final_ThisRound")]
        public bool isFLane2DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFLane3Done_Final_ThisRound")]
        public bool isFLane3DoneFinalThisRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bFDone")]
        public bool isFDone = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBDone")]
        public bool isBDone = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsFrontFirstRound")]
        public bool isFrontFirstRound = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bIsFrontFirstRoundTriggered")]
        public bool isFrontFirstRoundTriggered = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bNode1RLTriggered")]
        public bool isNode1RLTriggered = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bDepressDoneR")]
        public bool isDepressDoneR = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bDepressDoneL")]
        public bool isDepressDoneL = false;
        [UnityEngine.Serialization.FormerlySerializedAs("bBackRRpassed")]
        public bool isBackRRPassed = false;

        public Vector3 f0LAttempt = default(Vector3);
        public Vector3 f1LAttempt = default(Vector3);
        public Vector3 f2LAttempt = default(Vector3);
        public Vector3 f3LAttempt = default(Vector3);
        public Vector3 f0RAttempt = default(Vector3);
        public Vector3 f1RAttempt = default(Vector3);
        public Vector3 f2RAttempt = default(Vector3);
        public Vector3 f3RAttempt = default(Vector3);

        [UnityEngine.Serialization.FormerlySerializedAs("iBLane0_Real")]
        public List<Vector3> iBLane0Real;

        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBL_Start")]
        public Vector3 shoulderStartBL = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBR_Start")]
        public Vector3 shoulderStartBR = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFL_Start")]
        public Vector3 shoulderStartFL = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFR_Start")]
        public Vector3 shoulderStartFR = default(Vector3);

        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBL_End")]
        public Vector3 shoulderEndBL = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBR_End")]
        public Vector3 shoulderEndBR = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFL_End")]
        public Vector3 shoulderEndFL = default(Vector3);
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFR_End")]
        public Vector3 shoulderEndFR = default(Vector3);

        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBL_StartIndex")]
        public int shoulderBLStartIndex = -1;
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderBR_StartIndex")]
        public int shoulderBRStartIndex = -1;
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFL_StartIndex")]
        public int shoulderFLStartIndex = -1;
        [UnityEngine.Serialization.FormerlySerializedAs("ShoulderFR_StartIndex")]
        public int shoulderFRStartIndex = -1;
        #endregion


        public void Nullify()
        {
            //Intersection construction:
            NullifyList(iBLane0L);
            NullifyList(iBLane0R);
            NullifyList(iBLane1L);
            NullifyList(iBLane1R);
            NullifyList(iBLane2L);
            NullifyList(iBLane2R);
            NullifyList(iBLane3L);
            NullifyList(iBLane3R);
            NullifyList(iFLane0L);
            NullifyList(iFLane0R);
            NullifyList(iFLane1L);
            NullifyList(iFLane1R);
            NullifyList(iFLane2L);
            NullifyList(iFLane2R);
            NullifyList(iFLane3L);
            NullifyList(iFLane3R);
            NullifyList(iBMainPlateL);
            NullifyList(iBMainPlateR);
            NullifyList(iFMainPlateL);
            NullifyList(iFMainPlateR);
            NullifyList(iBMarkerPlateL);
            NullifyList(iBMarkerPlateR);
            NullifyList(iFMarkerPlateL);
            NullifyList(iFMarkerPlateR);


            if (tempconstruction_R != null)
            {
                tempconstruction_R = null;
            }
            if (tempconstruction_L != null)
            {
                tempconstruction_L = null;
            }
        }


        private void NullifyList(List<Vector3> _constructionMaker)
        {
            if (_constructionMaker != null)
            {
                _constructionMaker.Clear();
                _constructionMaker = null;
            }
        }


        public void ClampConstructionValues()
        {
            if (tempconstruction_InterStart > 1f)
            {
                tempconstruction_InterStart = 1f;
            }
            if (tempconstruction_InterStart < 0f)
            {
                tempconstruction_InterStart = 0f;
            }
            if (tempconstruction_InterEnd > 1f)
            {
                tempconstruction_InterEnd = 1f;
            }
            if (tempconstruction_InterEnd < 0f)
            {
                tempconstruction_InterEnd = 0f;
            }
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
