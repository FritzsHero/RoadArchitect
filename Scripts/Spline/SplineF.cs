#region "Imports"
using UnityEngine;
using System.Collections.Generic;
#endregion


namespace RoadArchitect
{
    public class SplineF : MonoBehaviour
    {
        #region "Vars"
        public List<SplinePreviewNode> nodes = new List<SplinePreviewNode>();
        public bool isClosed = false;
        public float distance = -1f;
        public Vector3 mousePos = new Vector3(0f, 0f, 0f);
        public SplineC spline;
        // Gizmos
        public bool isDrawingGizmos = false;
        private float gizmoDrawMeters = 1f;
        #endregion


        #region "Gizmos"
        private void OnDrawGizmos()
        {
            if (!isDrawingGizmos)
            {
                return;
            }
            if (nodes == null || nodes.Count < 2)
            {
                return;
            }

            //Debug.Log ("lawl2");
            nodes[nodes.Count - 1].pos = mousePos;
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


            Vector3 prevPos = nodes[0].pos;
            Vector3 tempVect = new Vector3(0f, 0f, 0f);
            //GizmoDrawMeters = 40f;
            float step = gizmoDrawMeters / distance;
            step = Mathf.Clamp(step, 0f, 1f);
            Gizmos.color = new Color(0f, 0f, 1f, 1f);
            float index = 0f;
            Vector3 cPos;

            float startI = 0f;
            if (nodes.Count > 4)
            {
                startI = nodes[nodes.Count - 4].time;
            }

            prevPos = GetSplineValue(startI);
            for (index = startI; index <= 1f; index += step)
            {
                cPos = GetSplineValue(index);
                Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                prevPos = cPos;
                if ((index + step) > 1f)
                {
                    cPos = GetSplineValue(1f);
                    Gizmos.DrawLine(prevPos + tempVect, cPos + tempVect);
                }
            }
        }
        #endregion


        #region "Setup"
        /// <summary> Setup relevant data for preview spline </summary>
        public void Setup(ref Vector3[] _vects)
        {
            //Create spline nodes:
            SetupNodes(ref _vects);

            //Setup spline length, if more than 1 node:
            if (GetNodeCount() > 1)
            {
                SetupSplineLength();
            }
        }


        /// <summary> Setup preview spline and preview nodes </summary>
        private void SetupNodes(ref Vector3[] _vects)
        {
            //Process nodes:
            int index = 0;
            if (nodes != null)
            {
                nodes.Clear();
                nodes = null;
            }

            // Setup preview nodes positions
            nodes = new List<SplinePreviewNode>();
            SplinePreviewNode previewNode;
            for (index = 0; index < _vects.Length; index++)
            {
                previewNode = new SplinePreviewNode();
                previewNode.pos = _vects[index];
                nodes.Add(previewNode);
            }


            // Setup preview nodes rotations
            float step;
            Quaternion rot;
            step = (isClosed) ? 1f / nodes.Count : 1f / (nodes.Count - 1);
            for (index = 0; index < nodes.Count; index++)
            {
                previewNode = nodes[index];

                rot = Quaternion.identity;

                // if not last node
                if (index != nodes.Count - 1)
                {
                    // Only rotate the node if the next node is on a different position
                    if (nodes[index + 1].pos - previewNode.pos == Vector3.zero)
                    {
                        rot = Quaternion.identity;
                    }
                    else
                    {
                        rot = Quaternion.LookRotation(nodes[index + 1].pos - previewNode.pos, transform.up);
                    }

                    //rot = Quaternion.LookRotation(mNodes[i+1].pos - previewNode.pos, transform.up);
                }
                else if (isClosed)
                {
                    rot = Quaternion.LookRotation(nodes[0].pos - previewNode.pos, transform.up);
                }
                else
                {
                    rot = Quaternion.identity;
                }

                previewNode.Setup(previewNode.pos, rot, new Vector2(0, 1), step * index, "pNode");
            }


            previewNode = null;
            _vects = null;
        }


        /// <summary> I think this is the general Length of the Spline? </summary>
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
            //		float step = 0.1f / mDistance;

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
        /// <param name='_value'> The relevant param (0-1) of the spline. </param>
        /// <param name='_isTangent'> True for is tangent, false (default) for vector3 position. </param>
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
            if (RootUtils.IsApproximately(_value, 1f, 0.00001f) || f > 1f)
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
            //		}

            float param = (_value - nodes[idx].time) / (nodes[idx + 1].time - nodes[idx].time);
            param = RoadArchitect.RootUtils.Ease(param, nodes[idx].easeIO.x, nodes[idx].easeIO.y);
            return GetHermiteInternal(idx, param, _isTangent);
        }


        public Vector3 GetSplineValueSkipOpt(float _value, bool _isTangent = false)
        {
            int i;
            int idx = -1;

            if (nodes.Count == 0)
            {
                return default(Vector3);
            }
            if (nodes.Count == 1)
            {
                return nodes[0].pos;
            }


            /* 
            if (RootUtils.IsApproximately(f, 0f, 0.00001f))
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
            if (RootUtils.IsApproximately(f, 1f, 0.00001f) || f > 1f)
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


            for (i = 1; i < nodes.Count; i++)
            {
                if (i == nodes.Count - 1)
                {
                    idx = i - 1;
                    break;
                }
                if (nodes[i].time >= _value)
                {
                    idx = i - 1;
                    break;
                }
            }
            if (idx < 0)
            {
                idx = 0;
            }
            //		}

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
                //Prevent compiler error.
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


        private int NGI(int i, int o)
        {
            int NGITI = i + o;
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
