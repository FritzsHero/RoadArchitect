#region "Imports"
using UnityEngine;
using System.Collections.Generic;
#endregion


namespace RoadArchitect
{
    public class SplineI : MonoBehaviour
    {
        #region "Vars"
        public List<SplinePreviewNode> nodes = new List<SplinePreviewNode>();
        public bool isClosed = false;
        public float distance = -1f;
        public Vector3 mousePos = new Vector3(0f, 0f, 0f);
        public SplineC spline;
        public SplinePreviewNode actionNode;
        // Gizmos
        public bool isDrawingGizmos = false;
        private float gizmoDrawMeters = 1f;
        #endregion


        public void DetermineInsertNodes()
        {
            int nodesCount = spline.nodes.Count;
            if (nodesCount < 2)
            {
                return;
            }
            SplinePreviewNode previewNode;
            SplineN xNode;
            nodes.Clear();
            float tParam = spline.GetClosestParam(mousePos, false, true);
            bool bEndInsert = false;
            bool bZeroInsert = false;
            int iStart = 0;
            if (Mathf.Approximately(tParam, 0f))
            {
                bZeroInsert = true;
                iStart = 0;
            }
            else if (Mathf.Approximately(tParam, 1f))
            {
                bEndInsert = true;
            }

            for (int index = 0; index < nodesCount; index++)
            {
                xNode = spline.nodes[index];
                previewNode = new SplinePreviewNode();
                previewNode.pos = xNode.pos;
                previewNode.idOnSpline = xNode.idOnSpline;
                previewNode.time = xNode.time;
                if (!bZeroInsert && !bEndInsert)
                {
                    if (tParam > previewNode.time)
                    {
                        iStart = previewNode.idOnSpline + 1;
                    }
                }
                nodes.Add(previewNode);
            }

            nodes.Sort(CompareListByName);
            int cCount = nodes.Count;
            if (bEndInsert)
            {
                iStart = cCount;
            }
            else
            {
                for (int index = iStart; index < cCount; index++)
                {
                    nodes[index].idOnSpline += 1;
                }
            }
            previewNode = new SplinePreviewNode();
            previewNode.pos = mousePos;
            previewNode.idOnSpline = iStart;
            previewNode.time = tParam;
            previewNode.isPreviewNode = true;
            if (bEndInsert)
            {
                nodes.Add(previewNode);
            }
            else
            {
                nodes.Insert(iStart, previewNode);
            }
            SetupSplineLength();
            actionNode = previewNode;
        }


        private int CompareListByName(SplinePreviewNode _i1, SplinePreviewNode _i2)
        {
            return _i1.idOnSpline.CompareTo(_i2.idOnSpline);
        }


        public void UpdateActionNode()
        {
            if (actionNode != null)
            {
                actionNode.pos = mousePos;
            }
            DetermineInsertNodes();
        }


        #region "Gizmos"
        private void OnDrawGizmos()
        {
            if (!isDrawingGizmos)
            {
                return;
            }
            if (actionNode == null)
            {
                return;
            }
            if (nodes == null || nodes.Count < 2)
            {
                return;
            }
            //Debug.Log ("lawl2");
            //mNodes[mNodes.Count-1].pos = MousePos;
            //Debug.Log ("lawl23");
            //Setup_SplineLength();

            float DistanceFromCam = Vector3.Distance(Camera.current.transform.position, nodes[0].pos);

            if (DistanceFromCam > 2048)
            {
                return;
            }
            else if (DistanceFromCam > 1024)
            {
                gizmoDrawMeters = 32f;
            }
            else if (DistanceFromCam > 512)
            {
                gizmoDrawMeters = 16f;
            }
            else if (DistanceFromCam > 256)
            {
                gizmoDrawMeters = 8f;
            }
            else if (DistanceFromCam > 128)
            {
                gizmoDrawMeters = 2f;
            }
            else if (DistanceFromCam > 64)
            {
                gizmoDrawMeters = 0.5f;
            }
            else
            {
                gizmoDrawMeters = 0.1f;
            }
            gizmoDrawMeters = 0.1f;

            Vector3 prevPos = nodes[0].pos;
            Vector3 tempVect = new Vector3(0f, 0f, 0f);
            //GizmoDrawMeters = 40f;
            float step = gizmoDrawMeters / distance;
            step = Mathf.Clamp(step, 0f, 1f);
            Gizmos.color = new Color(0f, 0f, 1f, 1f);
            float i = 0f;
            Vector3 cPos;

            float startI = 0f;
            float endI = 1f;
            if (actionNode.idOnSpline > 3)
            {
                startI = nodes[actionNode.idOnSpline - 2].time;
            }
            if (actionNode.idOnSpline < (nodes.Count - 3))
            {
                endI = nodes[actionNode.idOnSpline + 2].time;
            }

            prevPos = GetSplineValue(startI);
            for (i = startI; i <= endI; i += step)
            {
                cPos = GetSplineValue(i);
                Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                prevPos = cPos;
                if ((i + step) > 1f)
                {
                    cPos = GetSplineValue(1f);
                    Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                }
            }
        }
        #endregion


        #region "Setup"
        private void SetupSplineLength()
        {
            //First lets get the general distance, node to node:
            nodes[0].time = 0f;
            nodes[nodes.Count - 1].time = 1f;
            Vector3 tVect1 = new Vector3(0f, 0f, 0f);
            Vector3 tVect2 = new Vector3(0f, 0f, 0f);
            float mDistance = 0f;
            float mDistance_NoMod = 0f;
            for (int j = 0; j < nodes.Count; j++)
            {
                tVect2 = nodes[j].pos;
                if (j > 0)
                {
                    mDistance += Vector3.Distance(tVect1, tVect2);
                }
                tVect1 = tVect2;
            }
            mDistance_NoMod = mDistance;
            mDistance = mDistance * 1.05f;
            //float step = 0.1f / mDistance;

            //Get a slightly more accurate portrayal of the time:
            float tTime = 0f;
            for (int j = 0; j < (nodes.Count - 1); j++)
            {
                tVect2 = nodes[j].pos;
                if (j > 0)
                {
                    tTime += (Vector3.Distance(tVect1, tVect2) / mDistance_NoMod);
                    nodes[j].time = tTime;
                }
                tVect1 = tVect2;
            }
            distance = mDistance_NoMod;
        }
        #endregion


        #region "Hermite math"
        /// <summary> Gets the spline value. </summary>
        /// <param name='f'> The relevant param (0-1) of the spline. </param>
        /// <param name='isTangent'> True for is tangent, false (default) for vector3 position. </param>
        public Vector3 GetSplineValue(float _value, bool _isTangent = false)
        {
            int index;
            int idx = -1;

            if (nodes.Count == 0)
            {
                return default(Vector3);
            }
            if (nodes.Count == 1)
            {
                return nodes[0].pos;
            }


            // FH / Do note, that someone outcommented stuff here, for whatever Reason, but why?
            /*
            if (RootUtils.IsApproximately(_value, 0f, 0.00001f))
            {
                if (_isTangent)
                {
                    return mNodes[0].tangent;
                }
                else
                {
                    return mNodes[0].pos;
                }
            }
            else
            if (RootUtils.IsApproximately(_value, 1f, 0.00001f) || _value > 1f)
            {
                if (_isTangent)
                {
                    return mNodes[mNodes.Count - 1].tangent;
                }
                else
                {
                    return mNodes[mNodes.Count - 1].pos;
                }
            }
            else
            {
                */
            // FH / Do note, that someone outcommented stuff here, for whatever Reason, but why?


            for (index = 1; index < nodes.Count; index++)
            {
                if (index == nodes.Count - 1)
                {
                    idx = index - 1;
                    break;
                }
                if (nodes[index].time >= _value)
                {
                    idx = index - 1;
                    break;
                }
            }
            if (idx < 0)
            {
                idx = 0;
            }
            //}    // FH/ Do note, that someone outcommented stuff here, for whatever Reason, but why?

            float param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RoadArchitect.RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);
            return GetHermiteInternal(idx, param, _isTangent);
        }


        public Vector3 GetSplineValueSkipOpt(float _value, bool _isTangent = false)
        {
            int index;
            int idx = -1;

            if (nodes.Count == 0)
            {
                return default(Vector3);
            }
            if (nodes.Count == 1)
            {
                return nodes[0].pos;
            }

            //		if(RootUtils.IsApproximately(_value,0f,0.00001f)){
            //			if(_isTangent){
            //				return mNodes[0].tangent;
            //			}else{
            //				return mNodes[0].pos;	
            //			}
            //		}else 
            //		if(RootUtils.IsApproximately(_value,1f,0.00001f) || _value > 1f){
            //			if(_isTangent){
            //				return mNodes[mNodes.Count-1].tangent;
            //			}else{
            //				return mNodes[mNodes.Count-1].pos;	
            //			}
            //		}else{
            for (index = 1; index < nodes.Count; index++)
            {
                if (index == nodes.Count - 1)
                {
                    idx = index - 1;
                    break;
                }
                if (nodes[index].time >= _value)
                {
                    idx = index - 1;
                    break;
                }
            }
            if (idx < 0)
            {
                idx = 0;
            }
            // }      

            float param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RoadArchitect.RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);
            return GetHermiteInternal(idx, param, _isTangent);
        }


        private Vector3 GetHermiteInternal(int _i, double _t, bool _isTangent = false)
        {
            double t2, t3;
            float BL0, BL1, BL2, BL3, tension;

            if (!_isTangent)
            {
                t2 = _t * _t;
                t3 = t2 * _t;
            }
            else
            {
                t2 = _t * _t;
                _t = _t * 2.0;
                t2 = t2 * 3.0;
                //Necessary for compiler error.
                t3 = 0;
            }

            //Vectors:
            Vector3 P0 = nodes[NGI(_i, NI[0])].pos;
            Vector3 P1 = nodes[NGI(_i, NI[1])].pos;
            Vector3 P2 = nodes[NGI(_i, NI[2])].pos;
            Vector3 P3 = nodes[NGI(_i, NI[3])].pos;

            //Tension:
            // 0.5 equivale a catmull-rom
            tension = 0.5f;

            //Tangents:
            P2 = (P1 - P2) * tension;
            P3 = (P3 - P0) * tension;

            if (!_isTangent)
            {
                BL0 = (float)(CM[0] * t3 + CM[1] * t2 + CM[2] * _t + CM[3]);
                BL1 = (float)(CM[4] * t3 + CM[5] * t2 + CM[6] * _t + CM[7]);
                BL2 = (float)(CM[8] * t3 + CM[9] * t2 + CM[10] * _t + CM[11]);
                BL3 = (float)(CM[12] * t3 + CM[13] * t2 + CM[14] * _t + CM[15]);
            }
            else
            {
                BL0 = (float)(CM[0] * t2 + CM[1] * _t + CM[2]);
                BL1 = (float)(CM[4] * t2 + CM[5] * _t + CM[6]);
                BL2 = (float)(CM[8] * t2 + CM[9] * _t + CM[10]);
                BL3 = (float)(CM[12] * t2 + CM[13] * _t + CM[14]);
            }

            return BL0 * P0 + BL1 * P1 + BL2 * P2 + BL3 * P3;
        }


        private static readonly double[] CM = new double[] {
         2.0, -3.0,  0.0,  1.0,
        -2.0,  3.0,  0.0,  0.0,
         1.0, -2.0,  1.0,  0.0,
         1.0, -1.0,  0.0,  0.0
    };


        private static readonly int[] NI = new int[] { 0, 1, -1, 2 };


        private int NGI(int _i, int _o)
        {
            int NGITI = _i + _o;
            if (isClosed)
            {
                return (NGITI % nodes.Count + nodes.Count) % nodes.Count;
            }
            else
            {
                return Mathf.Clamp(NGITI, 0, nodes.Count - 1);
            }
        }
        #endregion


        public int GetNodeCount()
        {
            return nodes.Count;
        }
    }
}
