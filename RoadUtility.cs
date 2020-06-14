#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
#endregion


namespace RoadArchitect
{
    //Generic http://www.fhwa.dot.gov/bridge/bridgerail/br053504.cfm
    public enum RailingTypeEnum { None, Generic1, Generic2, K_Rail, WBeam };
    public enum RailingSubTypeEnum { Both, Left, Right };
    public enum SignPlacementSubTypeEnum { Center, Left, Right };
    public enum CenterDividerTypeEnum { None, K_Rail, KRail_Blinds, Wire, Markers };
    public enum EndCapTypeEnum { None, WBeam, Barrels3Static, Barrels3Rigid, Barrels7Static, Barrels7Rigid };
    public enum RoadUpdateTypeEnum { Full, Intersection, Railing, CenterDivider, Bridges };
    public enum AxisTypeEnum { X, Y, Z };


#if UNITY_EDITOR
    public static class RoadUtility
    {
        private const string FileSepString = "\n#### RoadArchitect ####\n";
        private const string FileSepStringCRLF = "\r\n#### RoadArchitect ####\r\n";
        // old: !!!! MICROGSD !!!!


        public static Terrain GetTerrain(Vector3 _vect)
        {
            return GetTerrainDo(ref _vect);
        }


        private static Terrain GetTerrainDo(ref Vector3 _vect)
        {
            //Sphere cast 5m first. Then raycast down 1000m, then up 1000m.
            Collider[] colliders = Physics.OverlapSphere(_vect, 10f);
            if (colliders != null)
            {
                int collidersLength = colliders.Length;
                for (int index = 0; index < collidersLength; index++)
                {
                    Terrain tTerrain = colliders[index].transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        colliders = null;
                        return tTerrain;
                    }
                }
                colliders = null;
            }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(_vect, Vector3.down, 1000f);
            int hitsLength = 0;
            if (hits != null)
            {
                hitsLength = hits.Length;
                for (int index = 0; index < hitsLength; index++)
                {
                    Terrain tTerrain = hits[index].collider.transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        hits = null;
                        return tTerrain;
                    }
                }
                hits = null;
            }

            hits = Physics.RaycastAll(_vect, Vector3.up, 1000f);
            if (hits != null)
            {
                hitsLength = hits.Length;
                for (int i = 0; i < hitsLength; i++)
                {
                    Terrain tTerrain = hits[i].collider.transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        hits = null;
                        return tTerrain;
                    }
                }
                hits = null;
            }
            return null;
        }


        #region "Terrain history"
        public static void ConstructRoadStoreTerrainHistory(ref Road _road)
        {
            ConstructRoadStoreTerrainHistoryDo(ref _road);
        }


        private static void ConstructRoadStoreTerrainHistoryDo(ref Road _road)
        {
            Object[] TIDs = GameObject.FindObjectsOfType(typeof(RoadTerrain));

            HashSet<int> tTIDS = new HashSet<int>();
            foreach (RoadTerrain TID in TIDs)
            {
                tTIDS.Add(TID.UID);
            }

            if (_road.TerrainHistory != null && _road.TerrainHistory.Count > 0)
            {
                //Delete unnecessary terrain histories:
                foreach (TerrainHistoryMaker THMaker in _road.TerrainHistory)
                {
                    if (!tTIDS.Contains(THMaker.terrainID))
                    {
                        THMaker.Nullify();
                        THMaker.isDestroySheduled = true;
                    }
                }

                int hCount = _road.TerrainHistory.Count;
                for (int index = hCount - 1; index >= 0; index--)
                {
                    if (_road.TerrainHistory[index].isDestroySheduled)
                    {
                        TerrainHistoryMaker THMaker = _road.TerrainHistory[index];
                        _road.TerrainHistory.RemoveAt(index);
                        if (THMaker != null)
                        {
                            THMaker = null;
                        }
                    }
                }
            }

            if (_road.TerrainHistory == null)
            {
                _road.TerrainHistory = new List<TerrainHistoryMaker>();
            }
            foreach (Terraforming.TempTerrainData TTD in _road.EditorTTDList)
            {
                TerrainHistoryMaker TH = null;
                RoadTerrain TID = null;
                //Get terrainID:
                foreach (RoadTerrain _TID in TIDs)
                {
                    if (_TID.UID == TTD.uID)
                    {
                        TID = _TID;
                    }
                }

                if (_road.TerrainHistory == null)
                {
                    _road.TerrainHistory = new List<TerrainHistoryMaker>();
                }
                if (TID == null)
                {
                    continue;
                }

                int THCount = _road.TerrainHistory.Count;
                bool isContainingTID = false;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].terrainID == TID.UID)
                    {
                        isContainingTID = true;
                        break;
                    }
                }

                if (!isContainingTID)
                {
                    TerrainHistoryMaker THMaker = new TerrainHistoryMaker();
                    THMaker.terrainID = TID.UID;
                    _road.TerrainHistory.Add(THMaker);
                }

                TH = null;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].terrainID == TID.UID)
                    {
                        TH = _road.TerrainHistory[index];
                        break;
                    }
                }
                if (TH == null)
                {
                    continue;
                }

                //Heights:
                if (_road.isHeightModificationEnabled)
                {
                    if (TTD.cX != null && TTD.cY != null)
                    {
                        TH.x1 = new int[TTD.cI];
                        System.Array.Copy(TTD.cX, 0, TH.x1, 0, TTD.cI);
                        TH.y1 = new int[TTD.cI];
                        System.Array.Copy(TTD.cY, 0, TH.y1, 0, TTD.cI);
                        TH.height = new float[TTD.cI];
                        System.Array.Copy(TTD.oldH, 0, TH.height, 0, TTD.cI);
                        TH.cI = TTD.cI;
                    }
                }
                else
                {
                    TH.x1 = null;
                    TH.y1 = null;
                    TH.height = null;
                    TH.cI = 0;
                }
                //Details:
                if (_road.isDetailModificationEnabled)
                {
                    int TotalSize = 0;
                    for (int i = 0; i < TTD.DetailLayersCount; i++)
                    {
                        TotalSize += TTD.DetailsI[i];
                    }

                    TH.detailsX = new int[TotalSize];
                    TH.detailsY = new int[TotalSize];
                    TH.detailsOldValue = new int[TotalSize];

                    int RunningIndex = 0;
                    int cLength = 0;
                    for (int index = 0; index < TTD.DetailLayersCount; index++)
                    {
                        cLength = TTD.DetailsI[index];
                        if (cLength < 1)
                        {
                            continue;
                        }
                        System.Array.Copy(TTD.DetailsX[index].ToArray(), 0, TH.detailsX, RunningIndex, cLength);
                        System.Array.Copy(TTD.DetailsY[index].ToArray(), 0, TH.detailsY, RunningIndex, cLength);
                        System.Array.Copy(TTD.OldDetailsValue[index].ToArray(), 0, TH.detailsOldValue, RunningIndex, cLength);
                        RunningIndex += TTD.DetailsI[index];
                    }

                    //TH.detailsX = TTD.detailsX;
                    //TH.detailsY = TTD.detailsY;
                    //TH.detailsOldValue = TTD.OldDetailsValue;
                    TH.detailsI = TTD.DetailsI;
                    TH.detailLayersCount = TTD.DetailLayersCount;
                }
                else
                {
                    TH.detailsX = null;
                    TH.detailsY = null;
                    TH.detailsOldValue = null;
                    TH.detailsI = null;
                    TH.detailLayersCount = 0;
                }
                //Trees:
                if (_road.isTreeModificationEnabled)
                {
                    if (TTD.TreesOld != null)
                    {
                        TH.MakeRATrees(ref TTD.TreesOld);
                        TTD.TreesOld.Clear();
                        TTD.TreesOld = null;
                        TH.TreesI = TTD.TreesI;
                    }
                }
                else
                {
                    TH.oldTrees = null;
                    TH.TreesI = 0;
                }
            }

            //			//TerrainHistoryRaw
            //			RootUtils.StartProfiling(tRoad, "TerrainHistorySerialize");
            //			TerrainHistorySerialize(ref tRoad);
            //			RootUtils.EndProfiling(tRoad);
        }


        //		private static void TerrainHistorySerialize(ref Road tRoad) {
        //			MemoryStream ms = new MemoryStream();
        //	        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //			formatter.Serialize(ms,tRoad.TerrainHistory);
        //			ms.Close();
        //			tRoad.TerrainHistoryRaw = ms.ToArray();
        //	        ms = null;
        //	    }
        //		
        //		private static void TerrainHistoryDeserialize(ref Road tRoad) {
        //			MemoryStream ms = new MemoryStream(tRoad.TerrainHistoryRaw);
        //	        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //			tRoad.TerrainHistory = (List<TerrainHistoryMaker>)formatter.Deserialize(ms);
        //			ms.Close();
        //	        ms = null;
        //	    }


        public static void ConstructRoadResetTerrainHistory(ref Road _road)
        {
            ConstructRoadResetTerrainHistoryDo(ref _road);
        }


        private static void ConstructRoadResetTerrainHistoryDo(ref Road _road)
        {
            if (_road.TerrainHistory != null)
            {
                _road.TerrainHistory.Clear();
                _road.TerrainHistory = null;
            }
        }
        #endregion


        [System.Serializable]
        public class Construction3DTri
        {
            public Vector3 P1, P2, P3;
            const float NearDist = 0.15f;
            const float NearDistSQ = 0.0225f;
            Vector2[] poly2D;
            Vector3[] poly3D;
            public float MaxDistance = 200f;
            public float MaxDistanceSq = 200f;
            public Vector3 normal = default(Vector3);
            public Vector3 pMiddle = default(Vector3);
            public float MinI = 0f;
            public float MaxI = 1f;


            public Construction3DTri(Vector3 _P1, Vector3 _P2, Vector3 _P3, float _MinI, float _MaxI)
            {
                MinI = _MinI;
                MaxI = _MaxI;
                P1 = _P1;
                P2 = _P2;
                P3 = _P3;

                poly2D = new Vector2[3];
                poly2D[0] = new Vector2(P1.x, P1.z);
                poly2D[1] = new Vector2(P2.x, P2.z);
                poly2D[2] = new Vector2(P3.x, P3.z);

                poly3D = new Vector3[3];
                poly3D[0] = P1;
                poly3D[1] = P2;
                poly3D[2] = P3;

                float[] tMaxes = new float[3];
                tMaxes[0] = Vector3.Distance(P1, P2);
                tMaxes[1] = Vector3.Distance(P1, P3);
                tMaxes[2] = Vector3.Distance(P2, P3);
                MaxDistance = Mathf.Max(tMaxes) * 1.5f;

                float[] tMaxesSQ = new float[3];
                tMaxesSQ[0] = Vector3.SqrMagnitude(P1 - P2);
                tMaxesSQ[1] = Vector3.SqrMagnitude(P1 - P3);
                tMaxesSQ[2] = Vector3.SqrMagnitude(P2 - P3);
                MaxDistanceSq = Mathf.Max(tMaxesSQ) * 1.5f;

                PlaneFrom3Points(out normal, out pMiddle, P1, P2, P3);

                normal = Vector3.Cross((P3 - P1), (P2 - P1));

                ////				//This creates middle point:
                //				Vector3 tMiddle1 = ((P3-P1)*0.5f)+P1;
                //				Vector3 tMiddle2 = ((P2-P1)*0.5f)+P1;
                //				pMiddle = ((tMiddle2-tMiddle1)*0.5f)+tMiddle1;
            }


            //Get the intersection between a line and a plane. 
            //If the line and plane are not parallel, the function outputs true, otherwise false.
            public Vector3 LinePlaneIntersection(ref Vector3 _F1)
            {
                _F1.y = 0f;

                //calculate the distance between the linePoint and the line-plane intersection point
                float dotNumerator = Vector3.Dot((pMiddle - _F1), normal);
                float dotDenominator = Vector3.Dot(Vector3.up.normalized, normal);

                //line and plane are not parallel
                if (!RootUtils.IsApproximately(0f, dotDenominator, 0.001f))
                {
                    //get the coordinates of the line-plane intersection point
                    return (_F1 + (Vector3.up.normalized * (dotNumerator / dotDenominator)));
                }
                else
                {
                    //output not valid
                    return default(Vector3);
                }
            }


            //Convert a plane defined by 3 points to a plane defined by a vector and a point. 
            //The plane point is the middle of the triangle defined by the 3 points.
            public static void PlaneFrom3Points(out Vector3 _planeNormal, out Vector3 _planePoint, Vector3 _pointA, Vector3 _pointB, Vector3 _pointC)
            {
                _planeNormal = Vector3.zero;
                _planePoint = Vector3.zero;

                //Make two vectors from the 3 input points, originating from point A
                Vector3 AB = _pointB - _pointA;
                Vector3 AC = _pointC - _pointA;

                //Calculate the normal
                _planeNormal = Vector3.Normalize(Vector3.Cross(AB, AC));

                //Get the points in the middle AB and AC
                Vector3 middleAB = _pointA + (AB / 2.0f);
                Vector3 middleAC = _pointA + (AC / 2.0f);

                //Get vectors from the middle of AB and AC to the point which is not on that line.
                Vector3 middleABtoC = _pointC - middleAB;
                Vector3 middleACtoB = _pointB - middleAC;

                //Calculate the intersection between the two lines. This will be the center 
                //of the triangle defined by the 3 points.
                //We could use LineLineIntersection instead of ClosestPointsOnTwoLines but due to rounding errors 
                //this sometimes doesn't work.
                Vector3 temp;
                ClosestPointsOnTwoLines(out _planePoint, out temp, middleAB, middleABtoC, middleAC, middleACtoB);
            }


            //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
            //to each other. This function finds those two points. If the lines are not parallel, the function 
            //outputs true, otherwise false.
            public static bool ClosestPointsOnTwoLines(out Vector3 _closestPointLine1, out Vector3 _closestPointLine2, Vector3 _linePoint1, Vector3 _lineVec1, Vector3 _linePoint2, Vector3 _lineVec2)
            {

                _closestPointLine1 = Vector3.zero;
                _closestPointLine2 = Vector3.zero;

                float a = Vector3.Dot(_lineVec1, _lineVec1);
                float b = Vector3.Dot(_lineVec1, _lineVec2);
                float e = Vector3.Dot(_lineVec2, _lineVec2);

                float d = a * e - b * b;

                //lines are not parallel
                if (d != 0.0f)
                {

                    Vector3 r = _linePoint1 - _linePoint2;
                    float c = Vector3.Dot(_lineVec1, r);
                    float f = Vector3.Dot(_lineVec2, r);

                    float s = (b * f - c * e) / d;
                    float t = (a * f - c * b) / d;

                    _closestPointLine1 = _linePoint1 + _lineVec1 * s;
                    _closestPointLine2 = _linePoint2 + _lineVec2 * t;

                    return true;
                }

                else
                {
                    return false;
                }
            }


            //create a vector of direction "_vector" with length "_size"
            public static Vector3 SetVectorLength(Vector3 _vector, float _size)
            {

                //normalize the vector
                Vector3 vectorNormalized = Vector3.Normalize(_vector);

                //scale the vector
                return vectorNormalized *= _size;
            }


            //public bool Contains2D(ref Vector2 p)
            //{
            //	return Contains2D_Do(ref p);
            //}


            public bool Contains2D(ref Vector2 _p)
            {
                if (Vector2.SqrMagnitude(_p - poly2D[0]) > MaxDistanceSq)
                {
                    return false;
                }
                //				if(Vector2.Distance(p,P1) > MaxDistance){ return false; }
                //				if(poly2D.Length != 3){ return false; }

                Vector2 x1 = default(Vector2);
                Vector2 x2 = default(Vector2);
                Vector2 oldPoint = default(Vector2);
                Vector2 newPoint = default(Vector2);
                bool inside = false;

                inside = false;
                oldPoint = new Vector2(poly2D[3 - 1].x, poly2D[3 - 1].y);
                for (int index = 0; index < 3; index++)
                {
                    newPoint = new Vector2(poly2D[index].x, poly2D[index].y);
                    if (newPoint.x > oldPoint.x)
                    {
                        x1 = oldPoint;
                        x2 = newPoint;
                    }
                    else
                    {
                        x1 = newPoint;
                        x2 = oldPoint;
                    }
                    if ((newPoint.x < _p.x) == (_p.x <= oldPoint.x) && (_p.y - x1.y) * (x2.x - x1.x) < (x2.y - x1.y) * (_p.x - x1.x))
                    {
                        inside = !inside;
                    }
                    oldPoint = newPoint;
                }
                return inside;
            }


            public bool Contains2D(ref Vector3 _p)
            {
                Vector2 tVect = new Vector2(_p.x, _p.z);
                return Contains2D(ref tVect);
            }


            public bool Near(ref Vector3 _vect, out Vector3 _nearVect)
            {
                if (Vector3.SqrMagnitude(_vect - P1) > MaxDistanceSq)
                {
                    //if(Vector3.Distance(tVect,P1) > MaxDistance){ 
                    _nearVect = default(Vector3);
                    return false;
                }

                //if(Vector3.Distance(tVect,P1) < NearDist){
                if (Vector3.SqrMagnitude(_vect - P1) < NearDistSQ)
                {
                    _nearVect = P1;
                    return true;
                }
                //if(Vector3.Distance(tVect,P2) < NearDist){
                if (Vector3.SqrMagnitude(_vect - P2) < NearDistSQ)
                {
                    _nearVect = P2;
                    return true;
                }
                //if(Vector3.Distance(tVect,P3) < NearDist){
                if (Vector3.SqrMagnitude(_vect - P3) < NearDistSQ)
                {
                    _nearVect = P3;
                    return true;
                }
                _nearVect = default(Vector3);
                return false;
            }


            public string ToStringRA()
            {
                return ("P1:" + P1.ToString() + " P2:" + P2.ToString() + " P3:" + P3.ToString());
            }
        }


        public class Construction2DRect
        {
            public Vector2 P1, P2, P3, P4;
            private const float NearDist = 0.15f;
            private const float NearDistSQ = 0.0225f;
            private Vector2[] poly;
            public float MaxDistance = 200f;
            public float MaxDistanceSQ = 200f;
            public float Height = 0f;
            public float MinI = 0f;
            public float MaxI = 0f;


            public Construction2DRect(Vector2 _P1, Vector2 _P2, Vector2 _P3, Vector2 _P4, float tHeight = 0f)
            {
                Construction2DRectDo(ref _P1, ref _P2, ref _P3, ref _P4, ref tHeight);
            }


            private void Construction2DRectDo(ref Vector2 _P1, ref Vector2 _P2, ref Vector2 _P3, ref Vector2 _P4, ref float _height)
            {
                P1 = _P1;
                P2 = _P2;
                P3 = _P3;
                P4 = _P4;
                Height = _height;

                if (RootUtils.IsApproximately(P1.x, P2.x, 0.0001f))
                {
                    P2.x += 0.0002f;
                }
                if (RootUtils.IsApproximately(P1.x, P3.x, 0.0001f))
                {
                    P3.x += 0.0002f;
                }
                if (RootUtils.IsApproximately(P1.x, P4.x, 0.0001f))
                {
                    P4.x += 0.0002f;
                }
                if (RootUtils.IsApproximately(P2.x, P3.x, 0.0001f))
                {
                    P3.x += 0.0002f;
                }
                if (RootUtils.IsApproximately(P2.x, P4.x, 0.0001f))
                {
                    P4.x += 0.0002f;
                }
                if (RootUtils.IsApproximately(P3.x, P4.x, 0.0001f))
                {
                    P4.x += 0.0002f;
                }

                if (RootUtils.IsApproximately(P1.y, P2.y, 0.0001f))
                {
                    P2.y += 0.0002f;
                }
                if (RootUtils.IsApproximately(P1.y, P3.y, 0.0001f))
                {
                    P3.y += 0.0002f;
                }
                if (RootUtils.IsApproximately(P1.y, P4.y, 0.0001f))
                {
                    P4.y += 0.0002f;
                }
                if (RootUtils.IsApproximately(P2.y, P3.y, 0.0001f))
                {
                    P3.y += 0.0002f;
                }
                if (RootUtils.IsApproximately(P2.y, P4.y, 0.0001f))
                {
                    P4.y += 0.0002f;
                }
                if (RootUtils.IsApproximately(P3.y, P4.y, 0.0001f))
                {
                    P4.y += 0.0002f;
                }

                //Find two with smallest x, etc		
                float[] tX = new float[4];
                float[] tY = new float[4];

                tX[0] = P1.x;
                tX[1] = P2.x;
                tX[2] = P3.x;
                tX[3] = P4.x;

                tY[0] = P1.y;
                tY[1] = P2.y;
                tY[2] = P3.y;
                tY[3] = P4.y;

                float MinX1, MinX2;
                bool bIgnoreP1, bIgnoreP2, bIgnoreP3, bIgnoreP4;
                bIgnoreP1 = bIgnoreP2 = bIgnoreP3 = bIgnoreP4 = false;

                //Get top two minimum X
                MinX1 = Mathf.Min(tX);
                tX = new float[3];
                int tCounter = 0;
                if (!RootUtils.IsApproximately(MinX1, P1.x, 0.0001f))
                {
                    tX[tCounter] = P1.x;
                    tCounter += 1;
                }
                if (!RootUtils.IsApproximately(MinX1, P2.x, 0.0001f))
                {
                    tX[tCounter] = P2.x;
                    tCounter += 1;
                }
                if (!RootUtils.IsApproximately(MinX1, P3.x, 0.0001f))
                {
                    tX[tCounter] = P3.x;
                    tCounter += 1;
                }
                if (!RootUtils.IsApproximately(MinX1, P4.x, 0.0001f))
                {
                    tX[tCounter] = P4.x;
                    tCounter += 1;
                }
                MinX2 = Mathf.Min(tX);

                Vector2 xMin1 = default(Vector2);
                Vector2 xMin2 = default(Vector2);
                if (RootUtils.IsApproximately(MinX1, P1.x, 0.0001f))
                {
                    xMin1 = P1;
                    bIgnoreP1 = true;
                }
                else if (RootUtils.IsApproximately(MinX1, P2.x, 0.0001f))
                {
                    xMin1 = P2;
                    bIgnoreP2 = true;
                }
                else if (RootUtils.IsApproximately(MinX1, P3.x, 0.0001f))
                {
                    xMin1 = P3;
                    bIgnoreP3 = true;
                }
                else if (RootUtils.IsApproximately(MinX1, P4.x, 0.0001f))
                {
                    xMin1 = P4;
                    bIgnoreP4 = true;
                }

                if (RootUtils.IsApproximately(MinX2, P1.x, 0.0001f))
                {
                    xMin2 = P1;
                    bIgnoreP1 = true;
                }
                else if (RootUtils.IsApproximately(MinX2, P2.x, 0.0001f))
                {
                    xMin2 = P2;
                    bIgnoreP2 = true;
                }
                else if (RootUtils.IsApproximately(MinX2, P3.x, 0.0001f))
                {
                    xMin2 = P3;
                    bIgnoreP3 = true;
                }
                else if (RootUtils.IsApproximately(MinX2, P4.x, 0.0001f))
                {
                    xMin2 = P4;
                    bIgnoreP4 = true;
                }

                Vector2 TopLeft = default(Vector2);
                Vector2 BottomLeft = default(Vector2);
                if (xMin1.y > xMin2.y)
                {
                    TopLeft = xMin1;
                    BottomLeft = xMin2;
                }
                else
                {
                    TopLeft = xMin2;
                    BottomLeft = xMin1;
                }

                Vector2 xMax1 = default(Vector2);
                Vector2 xMax2 = default(Vector2);
                bool bXmax1 = false;
                if (!bIgnoreP1)
                {
                    xMax1 = P1;
                    bXmax1 = true;
                }
                if (!bIgnoreP2)
                {
                    if (bXmax1)
                    {
                        xMax2 = P2;
                    }
                    else
                    {
                        xMax1 = P2;
                        bXmax1 = true;
                    }
                }
                if (!bIgnoreP3)
                {
                    if (bXmax1)
                    {
                        xMax2 = P3;
                    }
                    else
                    {
                        xMax1 = P3;
                        bXmax1 = true;
                    }
                }
                if (!bIgnoreP4)
                {
                    if (bXmax1)
                    {
                        xMax2 = P4;
                    }
                    else
                    {
                        xMax1 = P4;
                        bXmax1 = true;
                    }
                }

                Vector2 TopRight = default(Vector2);
                Vector2 BottomRight = default(Vector2);
                if (xMax1.y > xMax2.y)
                {
                    TopRight = xMax1;
                    BottomRight = xMax2;
                }
                else
                {
                    TopRight = xMax2;
                    BottomRight = xMax1;
                }

                P1 = BottomLeft;
                P2 = BottomRight;
                P3 = TopRight;
                P4 = TopLeft;

                poly = new Vector2[4];
                poly[0] = P1;
                poly[1] = P2;
                poly[2] = P3;
                poly[3] = P4;

                float[] tMaxes = new float[6];
                tMaxes[0] = Vector2.Distance(P1, P2);
                tMaxes[1] = Vector2.Distance(P1, P3);
                tMaxes[2] = Vector2.Distance(P1, P4);
                tMaxes[3] = Vector2.Distance(P2, P3);
                tMaxes[4] = Vector2.Distance(P2, P4);
                tMaxes[5] = Vector2.Distance(P3, P4);
                MaxDistance = Mathf.Max(tMaxes) * 1.5f;

                float[] tMaxesSQ = new float[6];
                tMaxesSQ[0] = Vector2.SqrMagnitude(P1 - P2);
                tMaxesSQ[1] = Vector2.SqrMagnitude(P1 - P3);
                tMaxesSQ[2] = Vector2.SqrMagnitude(P1 - P4);
                tMaxesSQ[3] = Vector2.SqrMagnitude(P2 - P3);
                tMaxesSQ[4] = Vector2.SqrMagnitude(P2 - P4);
                tMaxesSQ[5] = Vector2.SqrMagnitude(P3 - P4);
                MaxDistanceSQ = Mathf.Max(tMaxesSQ) * 1.5f;
            }



            private Vector2 x1 = default(Vector2);
            private Vector2 x2 = default(Vector2);
            private Vector2 oldPoint = default(Vector2);
            private Vector2 newPoint = default(Vector2);
            private bool inside = false;

            //				public bool Contains(ref Vector2 p){
            //					return Contains_Do(ref p);
            //				}
            public bool Contains(ref Vector2 p)
            {
                //					if(Vector2.Distance(p,P1) > MaxDistance){ return false; }
                if (Vector2.SqrMagnitude(p - P1) > MaxDistanceSQ)
                {
                    return false;
                }
                //					if(poly.Length != 4){ return false; }

                inside = false;
                oldPoint = new Vector2(poly[4 - 1].x, poly[4 - 1].y);
                for (int index = 0; index < 4; index++)
                {
                    newPoint = new Vector2(poly[index].x, poly[index].y);
                    if (newPoint.x > oldPoint.x)
                    {
                        x1 = oldPoint;
                        x2 = newPoint;
                    }
                    else
                    {
                        x1 = newPoint;
                        x2 = oldPoint;
                    }
                    if ((newPoint.x < p.x) == (p.x <= oldPoint.x) && (p.y - x1.y) * (x2.x - x1.x) < (x2.y - x1.y) * (p.x - x1.x))
                    {
                        inside = !inside;
                    }
                    oldPoint = newPoint;
                }
                return inside;
            }


            public bool Near(ref Vector2 _vect, out Vector2 _nearVect)
            {
                if (Vector2.SqrMagnitude(_vect - P1) > MaxDistanceSQ)
                {
                    //if(Vector2.Distance(tVect,P1) > MaxDistance){ 
                    _nearVect = default(Vector2);
                    return false;
                }

                if (Vector2.SqrMagnitude(_vect - P1) < NearDistSQ)
                {
                    //if(Vector2.Distance(tVect,P1) < NearDist){
                    _nearVect = P1;
                    return true;
                }
                if (Vector2.SqrMagnitude(_vect - P2) < NearDistSQ)
                {
                    //if(Vector2.Distance(tVect,P2) < NearDist){
                    _nearVect = P2;
                    return true;
                }
                if (Vector2.SqrMagnitude(_vect - P3) < NearDistSQ)
                {
                    //if(Vector2.Distance(tVect,P3) < NearDist){
                    _nearVect = P3;
                    return true;
                }
                if (Vector2.SqrMagnitude(_vect - P4) < NearDistSQ)
                {
                    //if(Vector2.Distance(tVect,P4) < NearDist){
                    _nearVect = P4;
                    return true;
                }
                _nearVect = default(Vector2);
                return false;
            }


            public string ToStringRA()
            {
                return ("P1:" + P1.ToString() + " P2:" + P2.ToString() + " P3:" + P3.ToString() + " P4:" + P4.ToString());
            }
        }


        public static Threading.RoadCreationT.RoadTerrainInfo[] GetRoadTerrainInfos()
        {
            Object[] tTerrainsObj = GameObject.FindObjectsOfType(typeof(Terrain));
            Threading.RoadCreationT.RoadTerrainInfo tInfo;
            List<Threading.RoadCreationT.RoadTerrainInfo> tInfos = new List<Threading.RoadCreationT.RoadTerrainInfo>();
            foreach (Terrain tTerrain in tTerrainsObj)
            {
                tInfo = new Threading.RoadCreationT.RoadTerrainInfo();
                tInfo.uID = tTerrain.transform.gameObject.GetComponent<RoadTerrain>().UID;
                tInfo.bounds = new Rect(tTerrain.transform.position.x, tTerrain.transform.position.z, tTerrain.terrainData.size.x, tTerrain.terrainData.size.z);
                tInfo.hmWidth = tTerrain.terrainData.heightmapResolution;
                tInfo.hmHeight = tTerrain.terrainData.heightmapResolution;
                tInfo.pos = tTerrain.transform.position;
                tInfo.size = tTerrain.terrainData.size;
                tInfo.heights = tTerrain.terrainData.GetHeights(0, 0, tInfo.hmWidth, tInfo.hmHeight);
                tInfos.Add(tInfo);
            }
            Threading.RoadCreationT.RoadTerrainInfo[] fInfos = new Threading.RoadCreationT.RoadTerrainInfo[tInfos.Count];
            int fInfosLength = fInfos.Length;
            for (int index = 0; index < fInfosLength; index++)
            {
                fInfos[index] = tInfos[index];
            }
            tInfos = null;
            return fInfos;
        }


        // RenderQueue provides ID's for Unity render queues. These can be applied to sub-shader tags,
        // but it's easier to just set material.renderQueue. Static class instead of enum because these
        // are int's, so this way client code doesn't need to use typecasting.
        //
        // From the documentation:
        // For special uses in-between queues can be used. Internally each queue is represented
        // by integer index; Background is 1000, Geometry is 2000, Transparent is 3000 and
        // Overlay is 4000.
        //
        // NOTE: Keep these in numerical order for ease of understanding. Use plurals for start of
        // a group of layers.
        public static class RenderQueue
        {
            public const int Background = 1000;

            // Mid-ground.
            // +1, 2, 3, ... for additional layers
            public const int ParallaxLayers = Background + 100;

            // Lines on the ground.
            public const int GroundLines = Background + 200;

            public const int Tracks = GroundLines + 0;
            public const int Routes = GroundLines + 1;
            public const int IndicatorRings = GroundLines + 2;
            public const int Road = GroundLines + 3;

            public const int Geometry = 2000;


            public const int Transparent = 3000;

            // Lines on the screen. (Over world, but under GUI.)
            public const int ScreenLines = Transparent + 100;

            public const int Overlay = 4000;
        }


        public static void SaveNodeObjects(ref Splination.SplinatedMeshMaker[] _splinatedObjects, ref EdgeObjects.EdgeObjectMaker[] _edgeObjects, ref WizardObject _wizardObj)
        {
            int sCount = _splinatedObjects.Length;
            int eCount = _edgeObjects.Length;
            //Splinated objects first:
            Splination.SplinatedMeshMaker SMM = null;
            RootUtils.CheckCreateSpecialLibraryDirs();
            string basePath = RootUtils.GetDirLibrary();
            string tPath = basePath + "Groups/" + _wizardObj.fileName + ".rao";
            if (_wizardObj.isDefault)
            {
                tPath = basePath + "Groups/Default/" + _wizardObj.fileName + ".rao";
            }
            StringBuilder builder = new StringBuilder(32768);

            //Wizard object:
            builder.Append(_wizardObj.ConvertToString());
            builder.Append(FileSepString);

            for (int index = 0; index < sCount; index++)
            {
                SMM = _splinatedObjects[index];
                builder.Append(SMM.ConvertToString());
                builder.Append(FileSepString);
            }

            EdgeObjects.EdgeObjectMaker EOM = null;
            for (int index = 0; index < eCount; index++)
            {
                EOM = _edgeObjects[index];
                builder.Append(EOM.ConvertToString());
                builder.Append(FileSepString);
            }

#if UNITY_WEBPLAYER
			
#else
            System.IO.File.WriteAllText(tPath, builder.ToString());
#endif
        }


        public static void LoadNodeObjects(string _fileName, SplineN _node, bool _isDefault = false, bool _isBridge = false)
        {
#if UNITY_WEBPLAYER
			return;
#else

            string filePath = "";
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault)
            {
                filePath = libraryPath + "Groups/Default/" + _fileName + ".rao";
            }
            else
            {
                filePath = libraryPath + "Groups/" + _fileName + ".rao";
            }

            string fileData = System.IO.File.ReadAllText(filePath);
            string[] tSep = new string[2];
            tSep[0] = FileSepString;
            tSep[1] = FileSepStringCRLF;
            string[] tSplit = fileData.Split(tSep, System.StringSplitOptions.RemoveEmptyEntries);

            Splination.SplinatedMeshMaker SMM = null;
            Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = null;
            EdgeObjects.EdgeObjectMaker EOM = null;
            EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker ELM = null;
            int tSplitCount = tSplit.Length;

            for (int index = 0; index < tSplitCount; index++)
            {
                SLM = null;
                SLM = Splination.SplinatedMeshMaker.SLMFromData(tSplit[index]);
                if (SLM != null)
                {
                    SMM = _node.AddSplinatedObject();
                    SMM.LoadFromLibraryBulk(ref SLM);
                    SMM.isToggled = false;
                    if (_isBridge && _node.isBridgeStart && _node.isBridgeMatched && _node.bridgeCounterpartNode != null)
                    {
                        SMM.StartTime = _node.time;
                        SMM.EndTime = _node.bridgeCounterpartNode.time;
                        SMM.StartPos = _node.spline.GetSplineValue(SMM.StartTime);
                        SMM.EndPos = _node.spline.GetSplineValue(SMM.EndTime);
                    }
                    continue;
                }

                ELM = null;
                ELM = EdgeObjects.EdgeObjectMaker.ELMFromData(tSplit[index]);
                if (ELM != null)
                {
                    EOM = _node.AddEdgeObject();
                    EOM.LoadFromLibraryBulk(ref ELM);
                    EOM.isToggled = false;
                    if (!EOM.isSingle && _isBridge && _node.isBridgeStart && _node.isBridgeMatched && _node.bridgeCounterpartNode != null)
                    {
                        EOM.startTime = _node.time;
                        EOM.endTime = _node.bridgeCounterpartNode.time;
                        EOM.startPos = _node.spline.GetSplineValue(EOM.startTime);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.endTime);
                    }
                    else if (EOM.isSingle && _isBridge && _node.bridgeCounterpartNode != null && _node.isBridgeStart)
                    {
                        float tDist = (EOM.singleOnlyBridgePercent * (_node.bridgeCounterpartNode.dist - _node.dist) + _node.dist);
                        EOM.singlePosition = _node.spline.TranslateDistBasedToParam(tDist);
                        EOM.startPos = _node.spline.GetSplineValue(EOM.singlePosition);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.singlePosition);
                    }
                    continue;
                }
            }

            _node.SetupSplinatedMeshes();
            _node.SetupEdgeObjects();

#endif
        }


        #region "Splat maps"
        public static byte[] MakeSplatMap(Terrain _terrain, Color _BG, Color _FG, int _width, int _height, float _splatWidth, bool _isSkippingBridge, bool _isSkippingTunnel, string _roadUID = "")
        {
            return MakeSplatMapDo(_terrain, _BG, _FG, _width, _height, _splatWidth, _isSkippingBridge, _isSkippingTunnel, _roadUID);
        }


        private static byte[] MakeSplatMapDo(Terrain _terrain, Color _BG, Color _FG, int _width, int _height, float _splatWidth, bool _isSkippingBridge, bool _isSkippingTunnel, string _roadUID)
        {
            Texture2D tTexture = new Texture2D(_width, _height, TextureFormat.RGB24, false);

            //Set background color:
            Color[] tColorsBG = new Color[_width * _height];
            int tBGCount = tColorsBG.Length;
            for (int i = 0; i < tBGCount; i++)
            {
                tColorsBG[i] = _BG;
            }
            tTexture.SetPixels(0, 0, _width, _height, tColorsBG);
            tColorsBG = null;

            Object[] tRoads = null;
            if (_roadUID != "")
            {
                tRoads = new Object[1];
                Object[] roads = GameObject.FindObjectsOfType(typeof(Road));
                foreach (Road road in roads)
                {
                    if (string.CompareOrdinal(road.UID, _roadUID) == 0)
                    {
                        tRoads[0] = road;
                        break;
                    }
                }
            }
            else
            {
                tRoads = GameObject.FindObjectsOfType(typeof(Road));
            }
            Vector3 tPos = _terrain.transform.position;
            Vector3 tSize = _terrain.terrainData.size;
            foreach (Road tRoad in tRoads)
            {
                SplineC tSpline = tRoad.spline;
                int tCount = tSpline.RoadDefKeysArray.Length;

                Vector3 POS1 = default(Vector3);
                Vector3 POS2 = default(Vector3);

                Vector3 tVect = default(Vector3);
                Vector3 tVect2 = default(Vector3);
                Vector3 lVect1 = default(Vector3);
                Vector3 lVect2 = default(Vector3);
                Vector3 rVect1 = default(Vector3);
                Vector3 rVect2 = default(Vector3);

                int x1, y1;
                int[] tX = new int[4];
                int[] tY = new int[4];
                int MinX = -1;
                int MaxX = -1;
                int MinY = -1;
                int MaxY = -1;
                int xDiff = -1;
                int yDiff = -1;
                float p1 = 0f;
                float p2 = 0f;
                bool bXBad = false;
                bool bYBad = false;
                for (int i = 0; i < (tCount - 1); i++)
                {
                    bXBad = false;
                    bYBad = false;
                    p1 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i]);
                    p2 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i + 1]);

                    //Skip bridges:
                    if (_isSkippingBridge)
                    {
                        if (tSpline.IsInBridgeTerrain(p1))
                        {
                            continue;
                        }
                    }

                    //Skip tunnels:
                    if (_isSkippingTunnel)
                    {
                        if (tSpline.IsInTunnelTerrain(p1))
                        {
                            continue;
                        }
                    }

                    tSpline.GetSplineValueBoth(p1, out tVect, out POS1);
                    tSpline.GetSplineValueBoth(p2, out tVect2, out POS2);
                    lVect1 = (tVect + new Vector3(_splatWidth * -POS1.normalized.z, 0, _splatWidth * POS1.normalized.x));
                    rVect1 = (tVect + new Vector3(_splatWidth * POS1.normalized.z, 0, _splatWidth * -POS1.normalized.x));
                    lVect2 = (tVect2 + new Vector3(_splatWidth * -POS2.normalized.z, 0, _splatWidth * POS2.normalized.x));
                    rVect2 = (tVect2 + new Vector3(_splatWidth * POS2.normalized.z, 0, _splatWidth * -POS2.normalized.x));

                    TranslateWorldVectToCustom(_width, _height, lVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[0] = x1;
                    tY[0] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[1] = x1;
                    tY[1] = y1;
                    TranslateWorldVectToCustom(_width, _height, lVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[2] = x1;
                    tY[2] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[3] = x1;
                    tY[3] = y1;

                    MinX = Mathf.Min(tX);
                    MaxX = Mathf.Max(tX);
                    MinY = Mathf.Min(tY);
                    MaxY = Mathf.Max(tY);


                    if (MinX < 0)
                    {
                        MinX = 0; bXBad = true;
                    }
                    if (MaxX < 0)
                    {
                        MaxX = 0; bXBad = true;
                    }
                    if (MinY < 0)
                    {
                        MinY = 0; bYBad = true;
                    }
                    if (MaxY < 0)
                    {
                        MaxY = 0; bYBad = true;
                    }

                    if (MinX > (_width - 1))
                    {
                        MinX = (_width - 1); bXBad = true;
                    }
                    if (MaxX > (_width - 1))
                    {
                        MaxX = (_width - 1); bXBad = true;
                    }
                    if (MinY > (_height - 1))
                    {
                        MinY = (_height - 1); bYBad = true;
                    }
                    if (MaxY > (_height - 1))
                    {
                        MaxY = (_height - 1); bYBad = true;
                    }

                    if (bXBad && bYBad)
                    {
                        continue;
                    }

                    xDiff = MaxX - MinX;
                    yDiff = MaxY - MinY;

                    Color[] tColors = new Color[xDiff * yDiff];
                    int cCount = tColors.Length;
                    for (int j = 0; j < cCount; j++)
                    {
                        tColors[j] = _FG;
                    }

                    if (xDiff > 0 && yDiff > 0)
                    {
                        tTexture.SetPixels(MinX, MinY, xDiff, yDiff, tColors);
                    }
                }
            }

            tTexture.Apply();
            byte[] tBytes = tTexture.EncodeToPNG();
            Object.DestroyImmediate(tTexture);
            return tBytes;
        }


        private static void TranslateWorldVectToCustom(int _width, int _height, Vector3 _vect, ref Vector3 _pos, ref Vector3 _size, out int _x1, out int _y1)
        {
            //Get the normalized position of this game object relative to the terrain:
            _vect -= _pos;

            _vect.x = _vect.x / _size.x;
            _vect.z = _vect.z / _size.z;

            //Get the position of the terrain heightmap where this game object is:
            _x1 = (int) (_vect.x * _width);
            _y1 = (int) (_vect.z * _height);
        }
        #endregion


        #region "Wizard objects"
        public class WizardObject
        {
            [UnityEngine.Serialization.FormerlySerializedAs("Thumb")]
            public Texture2D thumb;
            [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
            public string thumbString;
            [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
            public string displayName;
            [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
            public string desc;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsDefault")]
            public bool isDefault;
            [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
            public bool isBridge;
            [UnityEngine.Serialization.FormerlySerializedAs("FileName")]
            public string fileName;
            [UnityEngine.Serialization.FormerlySerializedAs("FullPath")]
            public string FullPath;
            public int sortID = 0;


            public string ConvertToString()
            {
                WizardObjectLibrary WOL = new WizardObjectLibrary();
                WOL.LoadFrom(this);
                return RootUtils.GetString<WizardObjectLibrary>(WOL);
            }


            public void LoadDataFromWOL(WizardObjectLibrary _wizardObjLib)
            {
                thumbString = _wizardObjLib.thumbString;
                displayName = _wizardObjLib.displayName;
                desc = _wizardObjLib.desc;
                isDefault = _wizardObjLib.isDefault;
                fileName = _wizardObjLib.fileName;
                isBridge = _wizardObjLib.isBridge;
            }


            public static WizardObject LoadFromLibrary(string _path)
            {
#if UNITY_WEBPLAYER
				return null;
#else
                string tData = System.IO.File.ReadAllText(_path);
                string[] tSep = new string[2];
                tSep[0] = FileSepString;
                tSep[1] = FileSepStringCRLF;
                string[] tSplit = tData.Split(tSep, System.StringSplitOptions.RemoveEmptyEntries);
                int tSplitCount = tSplit.Length;
                WizardObjectLibrary WOL = null;
                for (int i = 0; i < tSplitCount; i++)
                {
                    WOL = WizardObject.WizardObjectLibrary.WOLFromData(tSplit[i]);
                    if (WOL != null)
                    {
                        WizardObject WO = new WizardObject();
                        WO.LoadDataFromWOL(WOL);
                        return WO;
                    }
                }
                return null;
#endif
            }


            [System.Serializable]
            public class WizardObjectLibrary
            {
                [UnityEngine.Serialization.FormerlySerializedAs("ThumbString")]
                public string thumbString;
                [UnityEngine.Serialization.FormerlySerializedAs("DisplayName")]
                public string displayName;
                [UnityEngine.Serialization.FormerlySerializedAs("Desc")]
                public string desc;
                [UnityEngine.Serialization.FormerlySerializedAs("bIsDefault")]
                public bool isDefault;
                [UnityEngine.Serialization.FormerlySerializedAs("bIsBridge")]
                public bool isBridge;
                [UnityEngine.Serialization.FormerlySerializedAs("FileName")]
                public string fileName;


                public void LoadFrom(WizardObject _wizardObj)
                {
                    thumbString = _wizardObj.thumbString;
                    displayName = _wizardObj.displayName;
                    desc = _wizardObj.desc;
                    isDefault = _wizardObj.isDefault;
                    fileName = _wizardObj.fileName;
                    isBridge = _wizardObj.isBridge;
                }


                public static WizardObjectLibrary WOLFromData(string _data)
                {
                    try
                    {
                        WizardObjectLibrary WOL = (WizardObjectLibrary) RootUtils.LoadData<WizardObjectLibrary>(ref _data);
                        return WOL;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        #endregion
    }
#endif
}