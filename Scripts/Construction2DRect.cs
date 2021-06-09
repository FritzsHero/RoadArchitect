using UnityEngine;


namespace RoadArchitect
{
    public class Construction2DRect
    {
        public Vector2 P1, P2, P3, P4;
        public float MaxDistance = 200f;
        public float MaxDistanceSQ = 200f;
        public float Height = 0f;
        public float MinI = 0f;
        public float MaxI = 0f;

        private const float NearDist = 0.15f;
        private const float NearDistSQ = 0.0225f;
        private Vector2[] poly;
        private Vector2 x1 = default(Vector2);
        private Vector2 x2 = default(Vector2);
        private Vector2 oldPoint = default(Vector2);
        private Vector2 newPoint = default(Vector2);
        private bool inside = false;


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


        /// <summary> Returns true if _p is inside the rect </summary>
        public bool Contains(ref Vector2 _p)
        {
            //if(Vector2.Distance(_p,P1) > MaxDistance)
            //{
            //  return false;
            //}
            if (Vector2.SqrMagnitude(_p - P1) > MaxDistanceSQ)
            {
                return false;
            }
            //if(poly.Length != 4)
            //{
            //  return false;
            //}

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
                if ((newPoint.x < _p.x) == (_p.x <= oldPoint.x) && (_p.y - x1.y) * (x2.x - x1.x) < (x2.y - x1.y) * (_p.x - x1.x))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }


        /// <summary> Returns true if _vect is near the P1-P4 values </summary>
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
}
