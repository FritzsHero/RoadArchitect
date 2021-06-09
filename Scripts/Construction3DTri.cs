using UnityEngine;


namespace RoadArchitect
{
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

            //////This creates middle point:
            //Vector3 tMiddle1 = ((P3 - P1) * 0.5f) + P1;
            //Vector3 tMiddle2 = ((P2 - P1) * 0.5f) + P1;
            //pMiddle = ((tMiddle2 - tMiddle1) * 0.5f) + tMiddle1;
        }


        /// <summary> Get the intersection between a line and a plane. 
        /// If the line and plane are not parallel, the function outputs true, otherwise false. </summary>
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


        /// <summary> Convert a plane defined by 3 points to a plane defined by a vector and a point. 
        /// The plane point is the middle of the triangle defined by the 3 points. </summary>
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


        /// <summary> Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        /// to each other. This function finds those two points. If the lines are not parallel, the function 
        /// outputs true, otherwise false. </summary>
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


        /// <summary> Creates a vector of direction _vector with length _size </summary>
        public static Vector3 SetVectorLength(Vector3 _vector, float _size)
        {
            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(_vector);

            //scale the vector
            return vectorNormalized *= _size;
        }


        /// <summary> Returns true if _p is contained in this </summary>
        public bool Contains2D(ref Vector2 _p)
        {
            if (Vector2.SqrMagnitude(_p - poly2D[0]) > MaxDistanceSq)
            {
                return false;
            }
            //if(Vector2.Distance(p,P1) > MaxDistance)
            //{
            //  return false;
            //}
            //if(poly2D.Length != 3)
            //{
            //  return false;
            //}

            Vector2 x1;
            Vector2 x2;
            Vector2 oldPoint;
            Vector2 newPoint;
            bool inside = false;

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


        /// <summary> Returns true if Vector2(_p.x, _p.z) is contained in this </summary>
        public bool Contains2D(ref Vector3 _p)
        {
            Vector2 tVect = new Vector2(_p.x, _p.z);
            return Contains2D(ref tVect);
        }


        /// <summary> Returns true if _vect is near the P1-P3 values </summary>
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
}
