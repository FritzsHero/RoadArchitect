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
    public static class GSDConstruction
    {
        public static SplineN CreateNode(Road _road, bool _isSpecialEndNode = false, Vector3 _vectorSpecialLoc = default(Vector3), bool _isInterNode = false)
        {
            Object[] worldNodeCount = GameObject.FindObjectsOfType(typeof(SplineN));
            GameObject nodeObj = new GameObject("Node" + worldNodeCount.Length.ToString());
            if (!_isInterNode)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(nodeObj, "Created node");
            }
            SplineN node = nodeObj.AddComponent<SplineN>();

            if (_isSpecialEndNode)
            {
                node.isSpecialEndNode = true;
                nodeObj.transform.position = _vectorSpecialLoc;
            }
            else
            {
                nodeObj.transform.position = _road.editorMousePos;
                //This helps prevent double clicks:
                int nodeCount = _road.spline.GetNodeCount();
                for (int index = 0; index < nodeCount; index++)
                {
                    if (Vector3.Distance(_road.editorMousePos, _road.spline.nodes[index].pos) < 5f)
                    {
                        Object.DestroyImmediate(nodeObj);
                        return null;
                    }
                }
                //End double click prevention
            }
            Vector3 xVect = nodeObj.transform.position;
            if (xVect.y < 0.03f)
            {
                xVect.y = 0.03f;
            }
            nodeObj.transform.position = xVect;

            nodeObj.transform.parent = _road.GSDSplineObj.transform;
            node.idOnSpline = _road.spline.GetNodeCount() + 1;
            node.spline = _road.spline;

            //Enforce max road grade:
            if (_road.isMaxGradeEnabled && !_isSpecialEndNode)
            {
                node.EnsureGradeValidity(-1, true);
            }

            if (!_isInterNode && !_isSpecialEndNode)
            {
                _road.UpdateRoad();
            }
            return node;
        }


        /// <summary> Insert
        /// Detect closest node (if end node, auto select other node)
        /// Determine which node is closest (up or down) on spline
        /// Place node, adjust all id on splines
        /// Setup spline </summary>
        public static SplineN InsertNode(Road _road, bool _isForcedLoc = false, Vector3 _forcedLoc = default(Vector3), bool _isPreNode = false, int _insertIndex = -1, bool _isSpecialEndNode = false, bool _isInterNode = false)
        {
            GameObject nodeObj;
            Object[] worldNodeCount = GameObject.FindObjectsOfType(typeof(SplineN));
            if (!_isForcedLoc)
            {
                nodeObj = new GameObject("Node" + worldNodeCount.Length.ToString());
            }
            else if (_isForcedLoc && !_isSpecialEndNode)
            {
                nodeObj = new GameObject("Node" + worldNodeCount.Length.ToString() + "Ignore");
            }
            else
            {
                nodeObj = new GameObject("Node" + worldNodeCount.Length.ToString());
            }
            if (!_isInterNode)
            {
                UnityEditor.Undo.RegisterCreatedObjectUndo(nodeObj, "Inserted node");
            }

            if (!_isForcedLoc)
            {
                nodeObj.transform.position = _road.editorMousePos;

                //This helps prevent double clicks:
                int nodeCount = _road.spline.GetNodeCount();
                for (int index = 0; index < nodeCount; index++)
                {
                    if (Vector3.Distance(_road.editorMousePos, _road.spline.nodes[index].pos) < 15f)
                    {
                        Object.DestroyImmediate(nodeObj);
                        return null;
                    }
                }
                //End double click prevention
            }
            else
            {
                nodeObj.transform.position = _forcedLoc;
            }
            Vector3 xVect = nodeObj.transform.position;
            if (xVect.y < 0.03f)
            {
                xVect.y = 0.03f;
            }
            nodeObj.transform.position = xVect;
            nodeObj.transform.parent = _road.GSDSplineObj.transform;

            int childCount = _road.spline.nodes.Count;
            //float mDistance = 50000f;
            //float tDistance = 0f;

            float param;
            if (!_isForcedLoc)
            {
                param = _road.spline.GetClosestParam(_road.editorMousePos, false, true);
            }
            else
            {
                param = _road.spline.GetClosestParam(_forcedLoc, false, true);
            }
            bool isEndInsert = false;
            bool isZeroInsert = false;
            int iStart = 0;
            if (RootUtils.IsApproximately(param, 0f, 0.0001f))
            {
                isZeroInsert = true;
                iStart = 0;
            }
            else if (RootUtils.IsApproximately(param, 1f, 0.0001f))
            {
                isEndInsert = true;
            }

            if (_isForcedLoc)
            {
                iStart = _insertIndex;
            }
            else
            {
                for (int index = 0; index < childCount; index++)
                {
                    SplineN xNode = _road.spline.nodes[index];
                    if (!isZeroInsert && !isEndInsert)
                    {
                        if (param > xNode.time)
                        {
                            iStart = xNode.idOnSpline + 1;
                        }
                    }
                }
            }

            if (isEndInsert)
            {
                iStart = _road.spline.nodes.Count;
            }
            else
            {
                for (int i = iStart; i < childCount; i++)
                {
                    _road.spline.nodes[i].idOnSpline += 1;
                }
            }

            SplineN node = nodeObj.AddComponent<SplineN>();
            if (_isForcedLoc && !_isSpecialEndNode)
            {
                node.isBridge = true;
                node.isIgnore = true;
                //tNode.bIsBridge_PreNode = bIsPreNode;
                //tNode.bIsBridge_PostNode = !bIsPreNode;	
            }
            node.spline = _road.spline;
            node.idOnSpline = iStart;
            node.isSpecialEndNode = _isSpecialEndNode;
            if (!_isForcedLoc)
            {
                node.pos = _road.editorMousePos;
            }
            else
            {
                node.pos = _forcedLoc;
            }

            _road.spline.nodes.Insert(iStart, node);

            //Enforce maximum road grade:
            if (!_isForcedLoc && !_isSpecialEndNode && _road.isMaxGradeEnabled)
            {
                node.EnsureGradeValidity(iStart);
            }

            if (!_isInterNode && !_isSpecialEndNode)
            {
                if (!_isForcedLoc)
                {
                    _road.UpdateRoad();
                }
            }

            return node;
        }
    }


    public class RoadConstructorBufferMaker
    {
        [UnityEngine.Serialization.FormerlySerializedAs("tRoad")]
        public Road road;

        public List<Vector3> RoadVectors;
        public List<Vector3> ShoulderR_Vectors;
        public List<Vector3> ShoulderL_Vectors;

        public int[] tris;
        public int[] tris_ShoulderR;
        public int[] tris_ShoulderL;

        public Vector3[] normals;
        public Vector3[] normals_ShoulderR;
        public Vector3[] normals_ShoulderL;
        public List<int> normals_ShoulderR_averageStartIndexes;
        public List<int> normals_ShoulderL_averageStartIndexes;

        public Vector2[] uv;
        public Vector2[] uv2;
        public Vector2[] uv_SR;
        public Vector2[] uv_SL;

        public Vector4[] tangents;
        public Vector4[] tangents2;
        public Vector4[] tangents_SR;
        public Vector4[] tangents_SL;

        public List<List<Vector3>> cut_RoadVectors;
        public List<Vector3> cut_RoadVectorsHome;
        public List<List<Vector3>> cut_ShoulderR_Vectors;
        public List<List<Vector3>> cut_ShoulderL_Vectors;
        public List<Vector3> cut_ShoulderR_VectorsHome;
        public List<Vector3> cut_ShoulderL_VectorsHome;
        public List<int[]> cut_tris;
        public List<int[]> cut_tris_ShoulderR;
        public List<int[]> cut_tris_ShoulderL;
        public List<Vector3[]> cut_normals;
        public List<Vector3[]> cut_normals_ShoulderR;
        public List<Vector3[]> cut_normals_ShoulderL;
        public List<Vector2[]> cut_uv;
        public List<Vector2[]> cut_uv_SR;
        public List<Vector2[]> cut_uv_SL;
        public List<Vector4[]> cut_tangents;
        public List<Vector4[]> cut_tangents_SR;
        public List<Vector4[]> cut_tangents_SL;

        public List<Vector2[]> cut_uv_world;
        public List<Vector2[]> cut_uv_SR_world;
        public List<Vector2[]> cut_uv_SL_world;
        public List<Vector4[]> cut_tangents_world;
        public List<Vector4[]> cut_tangents_SR_world;
        public List<Vector4[]> cut_tangents_SL_world;

        //Road connections:
        public List<Vector3[]> RoadConnections_verts;
        public List<int[]> RoadConnections_tris;
        public List<Vector3[]> RoadConnections_normals;
        public List<Vector2[]> RoadConnections_uv;
        public List<Vector4[]> RoadConnections_tangents;

        //Back lanes:
        public List<Vector3[]> iBLane0s;
        public List<Vector3[]> iBLane1s;
        public List<bool> iBLane1s_IsMiddleLane;
        public List<Vector3[]> iBLane2s;
        public List<Vector3[]> iBLane3s;
        //Front lanes:
        public List<Vector3[]> iFLane0s;
        public List<Vector3[]> iFLane1s;
        public List<bool> iFLane1s_IsMiddleLane;
        public List<Vector3[]> iFLane2s;
        public List<Vector3[]> iFLane3s;
        //Main plates:
        public List<Vector3[]> iBMainPlates;
        public List<Vector3[]> iFMainPlates;
        //Marker plates:
        public List<Vector3[]> iBMarkerPlates;
        public List<Vector3[]> iFMarkerPlates;

        //Back lanes:
        public List<int[]> iBLane0s_tris;
        public List<int[]> iBLane1s_tris;
        public List<int[]> iBLane2s_tris;
        public List<int[]> iBLane3s_tris;
        //Front lanes:
        public List<int[]> iFLane0s_tris;
        public List<int[]> iFLane1s_tris;
        public List<int[]> iFLane2s_tris;
        public List<int[]> iFLane3s_tris;
        //Main plates:
        public List<int[]> iBMainPlates_tris;
        public List<int[]> iFMainPlates_tris;
        //Marker plates:
        public List<int[]> iBMarkerPlates_tris;
        public List<int[]> iFMarkerPlates_tris;

        //Back lanes:
        public List<Vector3[]> iBLane0s_normals;
        public List<Vector3[]> iBLane1s_normals;
        public List<Vector3[]> iBLane2s_normals;
        public List<Vector3[]> iBLane3s_normals;
        //Front lanes:
        public List<Vector3[]> iFLane0s_normals;
        public List<Vector3[]> iFLane1s_normals;
        public List<Vector3[]> iFLane2s_normals;
        public List<Vector3[]> iFLane3s_normals;
        //Main plates:
        public List<Vector3[]> iBMainPlates_normals;
        public List<Vector3[]> iFMainPlates_normals;
        //Marker plates:
        public List<Vector3[]> iBMarkerPlates_normals;
        public List<Vector3[]> iFMarkerPlates_normals;

        //Back lanes:
        public List<RoadIntersection> iBLane0s_tID;
        public List<RoadIntersection> iBLane1s_tID;
        public List<RoadIntersection> iBLane2s_tID;
        public List<RoadIntersection> iBLane3s_tID;
        //Front lanes:
        public List<RoadIntersection> iFLane0s_tID;
        public List<RoadIntersection> iFLane1s_tID;
        public List<RoadIntersection> iFLane2s_tID;
        public List<RoadIntersection> iFLane3s_tID;
        //Main plates:
        public List<RoadIntersection> iBMainPlates_tID;
        public List<RoadIntersection> iFMainPlates_tID;
        //Marker plates:
        public List<RoadIntersection> iBMarkerPlates_tID;
        public List<RoadIntersection> iFMarkerPlates_tID;

        //Back lanes:
        public List<SplineN> iBLane0s_nID;
        public List<SplineN> iBLane1s_nID;
        public List<SplineN> iBLane2s_nID;
        public List<SplineN> iBLane3s_nID;
        //Front lanes:
        public List<SplineN> iFLane0s_nID;
        public List<SplineN> iFLane1s_nID;
        public List<SplineN> iFLane2s_nID;
        public List<SplineN> iFLane3s_nID;
        //Main plates:
        public List<SplineN> iBMainPlates_nID;
        public List<SplineN> iFMainPlates_nID;
        //Marker plates:
        public List<SplineN> iBMarkerPlates_nID;
        public List<SplineN> iFMarkerPlates_nID;

        //Back lanes:
        public List<Vector2[]> iBLane0s_uv;
        public List<Vector2[]> iBLane1s_uv;
        public List<Vector2[]> iBLane2s_uv;
        public List<Vector2[]> iBLane3s_uv;
        //Front lanes:
        public List<Vector2[]> iFLane0s_uv;
        public List<Vector2[]> iFLane1s_uv;
        public List<Vector2[]> iFLane2s_uv;
        public List<Vector2[]> iFLane3s_uv;
        //Main plates:
        public List<Vector2[]> iBMainPlates_uv;
        public List<Vector2[]> iFMainPlates_uv;
        public List<Vector2[]> iBMainPlates_uv2;
        public List<Vector2[]> iFMainPlates_uv2;
        //Marker plates:
        public List<Vector2[]> iBMarkerPlates_uv;
        public List<Vector2[]> iFMarkerPlates_uv;

        //Back lanes:
        public List<Vector4[]> iBLane0s_tangents;
        public List<Vector4[]> iBLane1s_tangents;
        public List<Vector4[]> iBLane2s_tangents;
        public List<Vector4[]> iBLane3s_tangents;
        //Front lanes:
        public List<Vector4[]> iFLane0s_tangents;
        public List<Vector4[]> iFLane1s_tangents;
        public List<Vector4[]> iFLane2s_tangents;
        public List<Vector4[]> iFLane3s_tangents;
        //Main plates:
        public List<Vector4[]> iBMainPlates_tangents;
        public List<Vector4[]> iFMainPlates_tangents;
        public List<Vector4[]> iBMainPlates_tangents2;
        public List<Vector4[]> iFMainPlates_tangents2;
        //Marker plates:
        public List<Vector4[]> iBMarkerPlates_tangents;
        public List<Vector4[]> iFMarkerPlates_tangents;

        public Terrain tTerrain;

        public List<GSDRoadUtil.Construction2DRect> tIntersectionBounds;
        public HashSet<Vector3> ImmuneVects;

        public Mesh tMesh;
        public Mesh tMesh_SR;
        public Mesh tMesh_SL;
        public bool tMeshSkip = false;
        public bool tMesh_SRSkip = false;
        public bool tMesh_SLSkip = false;
        public List<Mesh> tMesh_RoadCuts;
        public List<Mesh> tMesh_SRCuts;
        public List<Mesh> tMesh_SLCuts;
        public List<Mesh> tMesh_RoadCuts_world;
        public List<Mesh> tMesh_SRCuts_world;
        public List<Mesh> tMesh_SLCuts_world;

        public List<Mesh> tMesh_RoadConnections;

        public List<Mesh> tMesh_iBLanes0;
        public List<Mesh> tMesh_iBLanes1;
        public List<Mesh> tMesh_iBLanes2;
        public List<Mesh> tMesh_iBLanes3;
        public List<Mesh> tMesh_iFLanes0;
        public List<Mesh> tMesh_iFLanes1;
        public List<Mesh> tMesh_iFLanes2;
        public List<Mesh> tMesh_iFLanes3;
        public List<Mesh> tMesh_iBMainPlates;
        public List<Mesh> tMesh_iFMainPlates;
        public List<Mesh> tMesh_iBMarkerPlates;
        public List<Mesh> tMesh_iFMarkerPlates;

        public RoadUpdateTypeEnum tUpdateType;

        [UnityEngine.Serialization.FormerlySerializedAs("bRoadOn")]
        public bool isRoadOn = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bTerrainOn")]
        public bool isTerrainOn = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bBridgesOn")]
        public bool isBridgesOn = true;
        [UnityEngine.Serialization.FormerlySerializedAs("bInterseOn")]
        public bool isInterseOn = true;

        public List<int> RoadCuts;
        public List<SplineN> RoadCutNodes;
        public List<int> ShoulderCutsR;
        public List<SplineN> ShoulderCutsRNodes;
        public List<int> ShoulderCutsL;
        public List<SplineN> ShoulderCutsLNodes;

        public enum SaveMeshTypeEnum { Road, Shoulder, Intersection, Railing, Center, Bridge, RoadCut, SCut, BSCut, RoadConn };


        public RoadConstructorBufferMaker(Road _road, RoadUpdateTypeEnum _updateType)
        {
            tUpdateType = _updateType;
            isRoadOn = (tUpdateType == RoadUpdateTypeEnum.Full || tUpdateType == RoadUpdateTypeEnum.Intersection || tUpdateType == RoadUpdateTypeEnum.Bridges);
            isTerrainOn = (tUpdateType == RoadUpdateTypeEnum.Full || tUpdateType == RoadUpdateTypeEnum.Intersection || tUpdateType == RoadUpdateTypeEnum.Bridges);
            isBridgesOn = (tUpdateType == RoadUpdateTypeEnum.Full || tUpdateType == RoadUpdateTypeEnum.Bridges);
            isInterseOn = (tUpdateType == RoadUpdateTypeEnum.Full || tUpdateType == RoadUpdateTypeEnum.Intersection);

            road = _road;
            Nullify();
            RoadVectors = new List<Vector3>();
            ShoulderR_Vectors = new List<Vector3>();
            ShoulderL_Vectors = new List<Vector3>();
            normals_ShoulderR_averageStartIndexes = new List<int>();
            normals_ShoulderL_averageStartIndexes = new List<int>();

            cut_RoadVectors = new List<List<Vector3>>();
            cut_RoadVectorsHome = new List<Vector3>();
            cut_ShoulderR_Vectors = new List<List<Vector3>>();
            cut_ShoulderL_Vectors = new List<List<Vector3>>();
            cut_ShoulderR_VectorsHome = new List<Vector3>();
            cut_ShoulderL_VectorsHome = new List<Vector3>();
            cut_tris = new List<int[]>();
            cut_tris_ShoulderR = new List<int[]>();
            cut_tris_ShoulderL = new List<int[]>();
            cut_normals = new List<Vector3[]>();
            cut_normals_ShoulderR = new List<Vector3[]>();
            cut_normals_ShoulderL = new List<Vector3[]>();
            cut_uv = new List<Vector2[]>();
            cut_uv_SR = new List<Vector2[]>();
            cut_uv_SL = new List<Vector2[]>();
            cut_tangents = new List<Vector4[]>();
            cut_tangents_SR = new List<Vector4[]>();
            cut_tangents_SL = new List<Vector4[]>();

            cut_uv_world = new List<Vector2[]>();
            cut_uv_SR_world = new List<Vector2[]>();
            cut_uv_SL_world = new List<Vector2[]>();
            cut_tangents_world = new List<Vector4[]>();
            cut_tangents_SR_world = new List<Vector4[]>();
            cut_tangents_SL_world = new List<Vector4[]>();

            RoadCutNodes = new List<SplineN>();
            ShoulderCutsRNodes = new List<SplineN>();
            ShoulderCutsLNodes = new List<SplineN>();

            RoadConnections_verts = new List<Vector3[]>();
            RoadConnections_tris = new List<int[]>();
            RoadConnections_normals = new List<Vector3[]>();
            RoadConnections_uv = new List<Vector2[]>();
            RoadConnections_tangents = new List<Vector4[]>();

            RoadCuts = new List<int>();
            ShoulderCutsR = new List<int>();
            ShoulderCutsL = new List<int>();

            //if(bInterseOn){
            //Back lanes:
            iBLane0s = new List<Vector3[]>();
            iBLane1s = new List<Vector3[]>();
            iBLane2s = new List<Vector3[]>();
            iBLane3s = new List<Vector3[]>();
            //Front lanes:
            iFLane0s = new List<Vector3[]>();
            iFLane1s = new List<Vector3[]>();
            iFLane2s = new List<Vector3[]>();
            iFLane3s = new List<Vector3[]>();
            //Main plates:
            iBMainPlates = new List<Vector3[]>();
            iFMainPlates = new List<Vector3[]>();
            //Marker plates:
            iBMarkerPlates = new List<Vector3[]>();
            iFMarkerPlates = new List<Vector3[]>();

            //Back lanes:
            iBLane0s_tris = new List<int[]>();
            iBLane1s_tris = new List<int[]>();
            iBLane2s_tris = new List<int[]>();
            iBLane3s_tris = new List<int[]>();
            //Front lanes:
            iFLane0s_tris = new List<int[]>();
            iFLane1s_tris = new List<int[]>();
            iFLane2s_tris = new List<int[]>();
            iFLane3s_tris = new List<int[]>();
            //Main plates:
            iBMainPlates_tris = new List<int[]>();
            iFMainPlates_tris = new List<int[]>();
            //Marker plates:
            iBMarkerPlates_tris = new List<int[]>();
            iFMarkerPlates_tris = new List<int[]>();

            //Back lanes:
            iBLane0s_normals = new List<Vector3[]>();
            iBLane1s_normals = new List<Vector3[]>();
            iBLane2s_normals = new List<Vector3[]>();
            iBLane3s_normals = new List<Vector3[]>();
            //Front lanes:
            iFLane0s_normals = new List<Vector3[]>();
            iFLane1s_normals = new List<Vector3[]>();
            iFLane2s_normals = new List<Vector3[]>();
            iFLane3s_normals = new List<Vector3[]>();
            //Main plates:
            iBMainPlates_normals = new List<Vector3[]>();
            iFMainPlates_normals = new List<Vector3[]>();
            //Marker plates:
            iBMarkerPlates_normals = new List<Vector3[]>();
            iFMarkerPlates_normals = new List<Vector3[]>();

            //Back lanes:
            iBLane0s_uv = new List<Vector2[]>();
            iBLane1s_uv = new List<Vector2[]>();
            iBLane2s_uv = new List<Vector2[]>();
            iBLane3s_uv = new List<Vector2[]>();
            //Front lanes:
            iFLane0s_uv = new List<Vector2[]>();
            iFLane1s_uv = new List<Vector2[]>();
            iFLane2s_uv = new List<Vector2[]>();
            iFLane3s_uv = new List<Vector2[]>();
            //Main plates:
            iBMainPlates_uv = new List<Vector2[]>();
            iFMainPlates_uv = new List<Vector2[]>();
            iBMainPlates_uv2 = new List<Vector2[]>();
            iFMainPlates_uv2 = new List<Vector2[]>();
            //Marker plates:
            iBMarkerPlates_uv = new List<Vector2[]>();
            iFMarkerPlates_uv = new List<Vector2[]>();

            //Back lanes:
            iBLane0s_tangents = new List<Vector4[]>();
            iBLane1s_tangents = new List<Vector4[]>();
            iBLane2s_tangents = new List<Vector4[]>();
            iBLane3s_tangents = new List<Vector4[]>();
            //Front lanes:
            iFLane0s_tangents = new List<Vector4[]>();
            iFLane1s_tangents = new List<Vector4[]>();
            iFLane2s_tangents = new List<Vector4[]>();
            iFLane3s_tangents = new List<Vector4[]>();
            //Main plates:
            iBMainPlates_tangents = new List<Vector4[]>();
            iFMainPlates_tangents = new List<Vector4[]>();
            iBMainPlates_tangents2 = new List<Vector4[]>();
            iFMainPlates_tangents2 = new List<Vector4[]>();
            //Marker plates:
            iBMarkerPlates_tangents = new List<Vector4[]>();
            iFMarkerPlates_tangents = new List<Vector4[]>();

            iFLane1s_IsMiddleLane = new List<bool>();
            iBLane1s_IsMiddleLane = new List<bool>();

            //Back lanes:
            iBLane0s_tID = new List<RoadIntersection>();
            iBLane1s_tID = new List<RoadIntersection>();
            iBLane2s_tID = new List<RoadIntersection>();
            iBLane3s_tID = new List<RoadIntersection>();
            //Front lanes:
            iFLane0s_tID = new List<RoadIntersection>();
            iFLane1s_tID = new List<RoadIntersection>();
            iFLane2s_tID = new List<RoadIntersection>();
            iFLane3s_tID = new List<RoadIntersection>();
            //Main plates:
            iBMainPlates_tID = new List<RoadIntersection>();
            iFMainPlates_tID = new List<RoadIntersection>();
            //Marker plates:
            iBMarkerPlates_tID = new List<RoadIntersection>();
            iFMarkerPlates_tID = new List<RoadIntersection>();

            iBLane0s_nID = new List<SplineN>();
            iBLane1s_nID = new List<SplineN>();
            iBLane2s_nID = new List<SplineN>();
            iBLane3s_nID = new List<SplineN>();
            //Front lanes:
            iFLane0s_nID = new List<SplineN>();
            iFLane1s_nID = new List<SplineN>();
            iFLane2s_nID = new List<SplineN>();
            iFLane3s_nID = new List<SplineN>();
            //Main plates:
            iBMainPlates_nID = new List<SplineN>();
            iFMainPlates_nID = new List<SplineN>();
            //Marker plates:
            iBMarkerPlates_nID = new List<SplineN>();
            iFMarkerPlates_nID = new List<SplineN>();
            //}

            tTerrain = null;

            tMesh = new Mesh();
            tMesh_SR = new Mesh();
            tMesh_SL = new Mesh();
            tMesh_RoadCuts = new List<Mesh>();
            tMesh_SRCuts = new List<Mesh>();
            tMesh_SLCuts = new List<Mesh>();
            tMesh_RoadCuts_world = new List<Mesh>();
            tMesh_SRCuts_world = new List<Mesh>();
            tMesh_SLCuts_world = new List<Mesh>();

            tMesh_RoadConnections = new List<Mesh>();

            //if(bInterseOn){
            tMesh_iBLanes0 = new List<Mesh>();
            tMesh_iBLanes1 = new List<Mesh>();
            tMesh_iBLanes2 = new List<Mesh>();
            tMesh_iBLanes3 = new List<Mesh>();
            tMesh_iFLanes0 = new List<Mesh>();
            tMesh_iFLanes1 = new List<Mesh>();
            tMesh_iFLanes2 = new List<Mesh>();
            tMesh_iFLanes3 = new List<Mesh>();
            tMesh_iBMainPlates = new List<Mesh>();
            tMesh_iFMainPlates = new List<Mesh>();
            tMesh_iBMarkerPlates = new List<Mesh>();
            tMesh_iFMarkerPlates = new List<Mesh>();
            tIntersectionBounds = new List<GSDRoadUtil.Construction2DRect>();
            ImmuneVects = new HashSet<Vector3>();
            //}

            InitGameObjects();
        }


        #region "Init and nullify"
        private void InitGameObjects()
        {
            //Destry past objects:
            if (road.MainMeshes != null)
            {
                MeshFilter[] MFArray = road.MainMeshes.GetComponentsInChildren<MeshFilter>();
                MeshCollider[] MCArray = road.MainMeshes.GetComponentsInChildren<MeshCollider>();

                int MFArrayCount = MFArray.Length;
                int MCArrayCount = MCArray.Length;
                for (int index = (MFArrayCount - 1); index > -1; index--)
                {
                    MFArray[index].sharedMesh = null;
                }
                for (int index = (MCArrayCount - 1); index > -1; index--)
                {
                    MCArray[index].sharedMesh = null;
                }

                Object.DestroyImmediate(road.MainMeshes);
            }

            //Main mesh object:
            road.MainMeshes = new GameObject("MainMeshes");
            road.MainMeshes.transform.parent = road.transform;

            //Road and shoulders:
            road.MeshRoad = new GameObject("RoadMesh");
            road.MeshShoR = new GameObject("ShoulderR");
            road.MeshShoL = new GameObject("ShoulderL");
            road.MeshRoad.transform.parent = road.MainMeshes.transform;
            road.MeshShoR.transform.parent = road.MainMeshes.transform;
            road.MeshShoL.transform.parent = road.MainMeshes.transform;

            //Intersections:
            road.MeshiLanes = new GameObject("MeshiLanes");
            road.MeshiLanes0 = new GameObject("MeshiLanes0");
            road.MeshiLanes1 = new GameObject("MeshiLanes1");
            road.MeshiLanes2 = new GameObject("MeshiLanes2");
            road.MeshiLanes3 = new GameObject("MeshiLanes3");
            road.MeshiMainPlates = new GameObject("MeshiMainPlates");
            road.MeshiMarkerPlates = new GameObject("MeshiMarkerPlates");
            road.MeshiLanes.transform.parent = road.MainMeshes.transform;
            road.MeshiLanes0.transform.parent = road.MainMeshes.transform;
            road.MeshiLanes1.transform.parent = road.MainMeshes.transform;
            road.MeshiLanes2.transform.parent = road.MainMeshes.transform;
            road.MeshiLanes3.transform.parent = road.MainMeshes.transform;
            road.MeshiMainPlates.transform.parent = road.MainMeshes.transform;
            road.MeshiMarkerPlates.transform.parent = road.MainMeshes.transform;
        }


        public void Nullify()
        {
            RoadVectors = null;
            ShoulderR_Vectors = null;
            ShoulderL_Vectors = null;
            tris = null;
            normals = null;
            uv = null;
            uv_SR = null;
            uv_SL = null;
            tangents = null;
            tangents_SR = null;
            tangents_SL = null;
            tTerrain = null;
            tIntersectionBounds = null;
            ImmuneVects = null;
            iBLane0s = null;
            iBLane1s = null;
            iBLane2s = null;
            iBLane3s = null;
            iFLane0s = null;
            iFLane1s = null;
            iFLane2s = null;
            iFLane3s = null;
            iBMainPlates = null;
            iFMainPlates = null;
            iBMarkerPlates = null;
            iFMarkerPlates = null;
            tMesh = null;
            tMesh_SR = null;
            tMesh_SL = null;
            if (tMesh_iBLanes0 != null)
            {
                tMesh_iBLanes0.Clear();
                tMesh_iBLanes0 = null;
            }
            if (tMesh_iBLanes1 != null)
            {
                tMesh_iBLanes1.Clear();
                tMesh_iBLanes1 = null;
            }
            if (tMesh_iBLanes2 != null)
            {
                tMesh_iBLanes2.Clear();
                tMesh_iBLanes2 = null;
            }
            if (tMesh_iBLanes3 != null)
            {
                tMesh_iBLanes3.Clear();
                tMesh_iBLanes3 = null;
            }
            if (tMesh_iFLanes0 != null)
            {
                tMesh_iFLanes0.Clear();
                tMesh_iFLanes0 = null;
            }
            if (tMesh_iFLanes1 != null)
            {
                tMesh_iFLanes1.Clear();
                tMesh_iFLanes1 = null;
            }
            if (tMesh_iFLanes2 != null)
            {
                tMesh_iFLanes2.Clear();
                tMesh_iFLanes2 = null;
            }
            if (tMesh_iFLanes3 != null)
            {
                tMesh_iFLanes3.Clear();
                tMesh_iFLanes3 = null;
            }
            if (tMesh_iBMainPlates != null)
            {
                tMesh_iBMainPlates.Clear();
                tMesh_iBMainPlates = null;
            }
            if (tMesh_iFMainPlates != null)
            {
                tMesh_iFMainPlates.Clear();
                tMesh_iFMainPlates = null;
            }
            if (tMesh_iBMarkerPlates != null)
            {
                tMesh_iBMarkerPlates.Clear();
                tMesh_iBMarkerPlates = null;
            }
            if (tMesh_iFMarkerPlates != null)
            {
                tMesh_iFMarkerPlates.Clear();
                tMesh_iFMarkerPlates = null;
            }
            tMesh_RoadConnections = null;

            iFLane1s_IsMiddleLane = null;
            iBLane1s_IsMiddleLane = null;

            RoadConnections_verts = null;
            RoadConnections_tris = null;
            RoadConnections_normals = null;
            RoadConnections_uv = null;
            RoadConnections_tangents = null;

            if (cut_uv_world != null)
            {
                cut_uv_world.Clear();
                cut_uv_world = null;
            }
            if (cut_uv_SR_world != null)
            {
                cut_uv_SR_world.Clear();
                cut_uv_SR_world = null;
            }
            if (cut_uv_SL_world != null)
            {
                cut_uv_SL_world.Clear();
                cut_uv_SL_world = null;
            }
            if (cut_tangents_world != null)
            {
                cut_tangents_world.Clear();
                cut_tangents_world = null;
            }
            if (cut_tangents_SR_world != null)
            {
                cut_tangents_SR_world.Clear();
                cut_tangents_SR_world = null;
            }
            if (cut_tangents_SL_world != null)
            {
                cut_tangents_SL_world.Clear();
                cut_tangents_SL_world = null;
            }


            tMesh = null;
            tMesh_SR = null;
            tMesh_SL = null;
            tMesh_SR = null;
            tMesh_SL = null;
            tMesh_RoadCuts = null;
            tMesh_SRCuts = null;
            tMesh_SLCuts = null;
            tMesh_RoadCuts_world = null;
            tMesh_SRCuts_world = null;
            tMesh_SLCuts_world = null;
        }
        #endregion


        #region "Mesh Setup1"	
        public void MeshSetup1()
        {
            MeshSetup1Do();
        }


        /// <summary>
        /// Creates meshes and assigns vertices, triangles and normals. If multithreading enabled, this occurs inbetween threaded jobs since unity library can't be used in threads.
        /// </summary>
        private void MeshSetup1Do()
        {
            Mesh MeshBuffer = null;

            if (isInterseOn)
            {
                MeshSetup1IntersectionObjectsSetup();
            }

            if (isRoadOn)
            {
                //Main road:
                if (RoadVectors.Count < 64000)
                {
                    if (tMesh == null)
                    {
                        tMesh = new Mesh();
                    }
                    tMesh = MeshSetup1Helper(ref tMesh, RoadVectors.ToArray(), ref tris, ref normals);
                    tMeshSkip = false;
                }
                else
                {
                    tMeshSkip = true;
                }

                //Right shoulder:
                if (ShoulderR_Vectors.Count < 64000)
                {
                    if (tMesh_SR == null)
                    {
                        tMesh_SR = new Mesh();
                    }
                    tMesh_SR = MeshSetup1Helper(ref tMesh_SR, ShoulderR_Vectors.ToArray(), ref tris_ShoulderR, ref normals_ShoulderR);
                    tMesh_SRSkip = false;
                }
                else
                {
                    tMesh_SRSkip = true;
                }

                //Left shoulder:
                if (ShoulderL_Vectors.Count < 64000)
                {
                    if (tMesh_SL == null)
                    {
                        tMesh_SL = new Mesh();
                    }
                    tMesh_SL = MeshSetup1Helper(ref tMesh_SL, ShoulderL_Vectors.ToArray(), ref tris_ShoulderL, ref normals_ShoulderL);
                    tMesh_SLSkip = false;
                }
                else
                {
                    tMesh_SLSkip = true;
                }

                if (RoadConnections_verts.Count > 0)
                {
                    Mesh qMesh = null;
                    for (int index = 0; index < RoadConnections_verts.Count; index++)
                    {
                        qMesh = new Mesh();
                        qMesh.vertices = RoadConnections_verts[index];
                        qMesh.triangles = RoadConnections_tris[index];
                        qMesh.normals = RoadConnections_normals[index];
                        qMesh.uv = RoadConnections_uv[index];
                        qMesh.RecalculateNormals();
                        RoadConnections_normals[index] = qMesh.normals;
                        tMesh_RoadConnections.Add(qMesh);
                    }
                }


                if ((road.isRoadCutsEnabled || road.isDynamicCutsEnabled) && RoadCuts.Count > 0)
                {
                    int[] tTris = null;
                    Vector3[] tNormals = null;
                    int cCount = cut_RoadVectors.Count;
                    for (int index = 0; index < cCount; index++)
                    {
                        tTris = cut_tris[index];
                        tNormals = cut_normals[index];
                        MeshBuffer = new Mesh();
                        tMesh_RoadCuts.Add(MeshSetup1Helper(ref MeshBuffer, cut_RoadVectors[index].ToArray(), ref tTris, ref tNormals));
                        MeshBuffer = new Mesh();
                        tMesh_RoadCuts_world.Add(MeshSetup1Helper(ref MeshBuffer, cut_RoadVectors[index].ToArray(), ref tTris, ref tNormals));
                        cut_normals[index] = tNormals;
                        tMeshSkip = true;
                    }
                }
                if (road.isShoulderCutsEnabled || road.isDynamicCutsEnabled)
                {
                    int[] tTris = null;
                    Vector3[] tNormals = null;
                    int rCount = cut_ShoulderR_Vectors.Count;
                    for (int index = 0; index < rCount; index++)
                    {
                        tTris = cut_tris_ShoulderR[index];
                        tNormals = cut_normals_ShoulderR[index];
                        MeshBuffer = new Mesh();
                        tMesh_SRCuts.Add(MeshSetup1Helper(ref MeshBuffer, cut_ShoulderR_Vectors[index].ToArray(), ref tTris, ref tNormals));
                        MeshBuffer = new Mesh();
                        tMesh_SRCuts_world.Add(MeshSetup1Helper(ref MeshBuffer, cut_ShoulderR_Vectors[index].ToArray(), ref tTris, ref tNormals));
                        cut_normals_ShoulderR[index] = tNormals;
                        tMesh_SRSkip = true;
                    }
                    if (rCount <= 0)
                    {
                        tMesh_SRSkip = false;
                    }
                    int lCount = cut_ShoulderL_Vectors.Count;
                    for (int index = 0; index < lCount; index++)
                    {
                        tTris = cut_tris_ShoulderL[index];
                        tNormals = cut_normals_ShoulderL[index];
                        MeshBuffer = new Mesh();
                        tMesh_SLCuts.Add(MeshSetup1Helper(ref MeshBuffer, cut_ShoulderL_Vectors[index].ToArray(), ref tTris, ref tNormals));
                        MeshBuffer = new Mesh();
                        tMesh_SLCuts_world.Add(MeshSetup1Helper(ref MeshBuffer, cut_ShoulderL_Vectors[index].ToArray(), ref tTris, ref tNormals));
                        cut_normals_ShoulderL[index] = tNormals;
                        tMesh_SLSkip = true;
                    }
                    if (lCount <= 0)
                    {
                        tMesh_SLSkip = false;
                    }
                }
            }

            if (isInterseOn)
            {
                MeshSetup1IntersectionParts();
            }

            MeshBuffer = null;
        }


        #region "Intersection for MeshSetup1"
        private void MeshSetup1IntersectionObjectsSetup()
        {
            int nodeCount = road.spline.GetNodeCount();
            List<RoadIntersection> tGSDRIs = new List<RoadIntersection>();
            for (int index = 0; index < nodeCount; index++)
            {
                if (road.spline.nodes[index].isIntersection)
                {
                    if (!tGSDRIs.Contains(road.spline.nodes[index].intersection))
                    {
                        tGSDRIs.Add(road.spline.nodes[index].intersection);
                    }
                }
            }

            //Cleanups:
            foreach (RoadIntersection GSDRI in tGSDRIs)
            {
                IntersectionObjects.CleanupIntersectionObjects(GSDRI.transform.gameObject);
                if (GSDRI.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                {
                    IntersectionObjects.CreateStopSignsAllWay(GSDRI.transform.gameObject, true);
                }
                else if (GSDRI.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1)
                {
                    IntersectionObjects.CreateTrafficLightBases(GSDRI.transform.gameObject, true);
                }
                else if (GSDRI.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                {

                }
                else if (GSDRI.intersectionStopType == RoadIntersection.iStopTypeEnum.None)
                {
                    //Do nothing.
                }
            }
        }


        private void MeshSetup1IntersectionParts()
        {
            int mCount = road.spline.GetNodeCount();
            bool bHasInter = false;
            for (int index = 0; index < mCount; index++)
            {
                if (road.spline.nodes[index].isIntersection)
                {
                    bHasInter = true;
                    break;
                }
            }
            if (!bHasInter)
            {
                return;
            }

            int vCount = -1;
            Mesh MeshBuffer = null;
            Vector3[] tNormals = null;
            int[] tTris = null;
            //Back lanes:
            vCount = iBLane0s.Count;
            for (int i = 0; i < vCount; i++)
            {
                tNormals = iBLane0s_normals[i];
                tTris = iBLane0s_tris[i];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iBLane0s[i], ref tTris, ref tNormals);
                tMesh_iBLanes0.Add(MeshBuffer);
            }
            vCount = iBLane1s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iBLane1s_normals[index];
                tTris = iBLane1s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iBLane1s[index], ref tTris, ref tNormals);
                tMesh_iBLanes1.Add(MeshBuffer);
            }
            vCount = iBLane2s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iBLane2s_normals[index];
                tTris = iBLane2s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iBLane2s[index], ref tTris, ref tNormals);
                tMesh_iBLanes2.Add(MeshBuffer);
            }
            vCount = iBLane3s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iBLane3s_normals[index];
                tTris = iBLane3s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iBLane3s[index], ref tTris, ref tNormals);
                tMesh_iBLanes3.Add(MeshBuffer);
            }
            //Front lanes:
            vCount = iFLane0s.Count;
            for (int i = 0; i < vCount; i++)
            {
                tNormals = iFLane0s_normals[i];
                tTris = iFLane0s_tris[i];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iFLane0s[i], ref tTris, ref tNormals);
                tMesh_iFLanes0.Add(MeshBuffer);
            }
            vCount = iFLane1s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iFLane1s_normals[index];
                tTris = iFLane1s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iFLane1s[index], ref tTris, ref tNormals);
                tMesh_iFLanes1.Add(MeshBuffer);
            }
            vCount = iFLane2s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iFLane2s_normals[index];
                tTris = iFLane2s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iFLane2s[index], ref tTris, ref tNormals);
                tMesh_iFLanes2.Add(MeshBuffer);
            }
            vCount = iFLane3s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iFLane3s_normals[index];
                tTris = iFLane3s_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iFLane3s[index], ref tTris, ref tNormals);
                tMesh_iFLanes3.Add(MeshBuffer);
            }
            //Main plates:
            vCount = iBMainPlates.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iBMainPlates_normals[index];
                tTris = iBMainPlates_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iBMainPlates[index], ref tTris, ref tNormals);
                tMesh_iBMainPlates.Add(MeshBuffer);
            }
            vCount = iFMainPlates.Count;
            for (int index = 0; index < vCount; index++)
            {
                tNormals = iFMainPlates_normals[index];
                tTris = iFMainPlates_tris[index];
                MeshBuffer = new Mesh();
                MeshBuffer = MeshSetup1Helper(ref MeshBuffer, iFMainPlates[index], ref tTris, ref tNormals);
                tMesh_iFMainPlates.Add(MeshBuffer);
            }

            MeshBuffer = null;
        }
        #endregion


        private Mesh MeshSetup1Helper(ref Mesh _mesh, Vector3[] _verts, ref int[] _tris, ref Vector3[] _normals)
        {
            _mesh.vertices = _verts;
            _mesh.triangles = _tris;
            _mesh.normals = _normals;
            _mesh.RecalculateNormals();
            _normals = _mesh.normals;
            //			xMesh.hideFlags = HideFlags.DontSave;
            return _mesh;
        }
        #endregion


        #region "Mesh Setup2"
        public void MeshSetup2()
        {
            MeshSetup2Do();
        }


        /// <summary>
        /// Assigns UV and tangents to meshes. If multithreading enabled, this occurs after the last threaded job since unity library can't be used in threads.
        /// </summary>
        private void MeshSetup2Do()
        {
            Mesh MeshMainBuffer = null;
            Mesh MeshMarkerBuffer = null;

            if (isRoadOn)
            {
                //If road cuts is off, full size UVs:
                if ((!road.isRoadCutsEnabled && !road.isDynamicCutsEnabled) || (RoadCuts == null || RoadCuts.Count <= 0))
                {
                    if (tMesh != null)
                    {
                        tMesh = MeshSetup2Helper(ref tMesh, uv, tangents, ref road.MeshRoad, true);
                        SaveMesh(SaveMeshTypeEnum.Road, tMesh, road, road.MeshRoad.transform.name);

                        Vector3[] ooVerts = new Vector3[tMesh.vertexCount];
                        int[] ooTris = new int[tMesh.triangles.Length];
                        Vector3[] ooNormals = new Vector3[tMesh.normals.Length];
                        Vector2[] ooUV = new Vector2[uv2.Length];
                        Vector4[] ooTangents = new Vector4[tangents2.Length];

                        System.Array.Copy(tMesh.vertices, ooVerts, ooVerts.Length);
                        System.Array.Copy(tMesh.triangles, ooTris, ooTris.Length);
                        System.Array.Copy(tMesh.normals, ooNormals, ooNormals.Length);
                        System.Array.Copy(uv2, ooUV, ooUV.Length);
                        System.Array.Copy(tangents2, ooTangents, ooTangents.Length);

                        Mesh pMesh = new Mesh();
                        pMesh.vertices = ooVerts;
                        pMesh.triangles = ooTris;
                        pMesh.normals = ooNormals;
                        pMesh.uv = ooUV;
                        pMesh.tangents = ooTangents;

                        GameObject gObj = new GameObject("Pavement");
                        pMesh = MeshSetup2Helper(ref pMesh, uv2, tangents2, ref gObj, false);
                        gObj.transform.parent = road.MeshRoad.transform;   //Road markers stored on parent "MeshRoad" game object, with a "Pavement" child game object storing the asphalt.
                        SaveMesh(SaveMeshTypeEnum.Road, pMesh, road, gObj.transform.name);
                    }
                }
                else
                {
                    //If road cuts, change it to one material (pavement) with world mapping
                    int cCount = cut_RoadVectors.Count;
                    //					Vector2[] tUV;
                    GameObject CreatedMainObj;
                    GameObject CreatedMarkerObj;
                    for (int i = 0; i < cCount; i++)
                    {
                        CreatedMainObj = null;
                        MeshMainBuffer = tMesh_RoadCuts_world[i];
                        if (MeshMainBuffer != null)
                        {
                            MeshSetup2HelperRoadCuts(i, ref MeshMainBuffer, cut_uv_world[i], cut_tangents_world[i], ref road.MeshRoad, false, out CreatedMainObj);
                            SaveMesh(SaveMeshTypeEnum.RoadCut, MeshMainBuffer, road, "RoadCut" + i.ToString());
                        }

                        CreatedMarkerObj = null;
                        MeshMarkerBuffer = tMesh_RoadCuts[i];
                        bool bHasMats = false;
                        if (MeshMarkerBuffer != null)
                        {
                            bHasMats = MeshSetup2HelperRoadCuts(i, ref MeshMarkerBuffer, cut_uv[i], cut_tangents[i], ref CreatedMainObj, true, out CreatedMarkerObj);
                            if (bHasMats)
                            {
                                SaveMesh(SaveMeshTypeEnum.RoadCut, MeshMarkerBuffer, road, "RoadCutMarker" + i.ToString());
                            }
                            else
                            {
                                //Destroy if no marker materials:
                                Object.DestroyImmediate(CreatedMarkerObj);
                                Object.DestroyImmediate(MeshMarkerBuffer);
                            }
                        }
                    }

                    //Remove main mesh stuff if necessary:
                    if (road.MeshRoad != null)
                    {
                        MeshCollider tMC = road.MeshRoad.GetComponent<MeshCollider>();
                        MeshRenderer tMR = road.MeshRoad.GetComponent<MeshRenderer>();
                        if (tMC != null)
                        {
                            Object.DestroyImmediate(tMC);
                        }
                        if (tMR != null)
                        {
                            Object.DestroyImmediate(tMR);
                        }
                    }
                    if (tMesh != null)
                    {
                        Object.DestroyImmediate(tMesh);
                    }
                }

                //Shoulders:
                if (road.isShouldersEnabled)
                {
                    if ((!road.isShoulderCutsEnabled && !road.isDynamicCutsEnabled) || (ShoulderCutsL == null || cut_ShoulderL_Vectors.Count <= 0))
                    {
                        //Right road shoulder:
                        if (tMesh_SR != null)
                        {
                            tMesh_SR = MeshSetup2Helper(ref tMesh_SR, uv_SR, tangents_SR, ref road.MeshShoR, false, true);
                            SaveMesh(SaveMeshTypeEnum.Shoulder, tMesh_SR, road, road.MeshShoR.transform.name);
                        }

                        //Left road shoulder:
                        if (tMesh_SL != null)
                        {
                            tMesh_SL = MeshSetup2Helper(ref tMesh_SL, uv_SL, tangents_SL, ref road.MeshShoL, false, true);
                            SaveMesh(SaveMeshTypeEnum.Shoulder, tMesh_SL, road, road.MeshShoL.transform.name);
                        }
                    }
                    else
                    {
                        GameObject CreatedMainObj;
                        GameObject CreatedMarkerObj;
                        int rCount = cut_ShoulderR_Vectors.Count;
                        for (int index = 0; index < rCount; index++)
                        {
                            bool bHasMats = false;
                            CreatedMainObj = null;
                            MeshMainBuffer = tMesh_SRCuts_world[index];
                            if (MeshMainBuffer != null)
                            {
                                MeshSetup2HelperCutsShoulder(index, ref MeshMainBuffer, cut_uv_SR_world[index], cut_tangents_SR_world[index], ref road.MeshShoR, false, false, out CreatedMainObj);
                                SaveMesh(SaveMeshTypeEnum.SCut, MeshMainBuffer, road, "SCutR" + index.ToString());
                            }

                            CreatedMarkerObj = null;
                            MeshMarkerBuffer = tMesh_SRCuts[index];
                            if (MeshMarkerBuffer != null)
                            {
                                bHasMats = MeshSetup2HelperCutsShoulder(index, ref MeshMarkerBuffer, cut_uv_SR[index], cut_tangents_SR[index], ref CreatedMainObj, false, true, out CreatedMarkerObj);
                                if (bHasMats)
                                {
                                    SaveMesh(SaveMeshTypeEnum.SCut, MeshMarkerBuffer, road, "SCutRMarker" + index.ToString());
                                }
                                else
                                {
                                    //Destroy if no marker materials:
                                    Object.DestroyImmediate(CreatedMarkerObj);
                                    Object.DestroyImmediate(MeshMarkerBuffer);
                                }
                            }
                        }

                        int lCount = cut_ShoulderL_Vectors.Count;
                        for (int index = 0; index < lCount; index++)
                        {
                            bool bHasMats = false;
                            CreatedMainObj = null;
                            MeshMainBuffer = tMesh_SLCuts_world[index];
                            if (MeshMainBuffer != null)
                            {
                                MeshSetup2HelperCutsShoulder(index, ref MeshMainBuffer, cut_uv_SL_world[index], cut_tangents_SL_world[index], ref road.MeshShoL, true, false, out CreatedMainObj);
                                SaveMesh(SaveMeshTypeEnum.SCut, MeshMainBuffer, road, "SCutL" + index.ToString());
                            }


                            CreatedMarkerObj = null;
                            MeshMarkerBuffer = tMesh_SLCuts[index];
                            if (MeshMarkerBuffer != null)
                            {
                                bHasMats = MeshSetup2HelperCutsShoulder(index, ref MeshMarkerBuffer, cut_uv_SL[index], cut_tangents_SL[index], ref CreatedMainObj, true, true, out CreatedMarkerObj);
                                if (bHasMats)
                                {
                                    SaveMesh(SaveMeshTypeEnum.SCut, MeshMarkerBuffer, road, "SCutLMarker" + index.ToString());
                                }
                                else
                                {
                                    //Destroy if no marker materials:
                                    Object.DestroyImmediate(CreatedMarkerObj);
                                    Object.DestroyImmediate(MeshMarkerBuffer);
                                }
                            }
                        }

                        if (road.isUsingMeshColliders)
                        {
                            //						MeshSetup2_Intersections_FixNormals();	
                        }

                        //Remove main mesh stuff if necessary:
                        if (tMesh_SR != null)
                        {
                            Object.DestroyImmediate(tMesh_SR);
                        }
                        if (tMesh_SL != null)
                        {
                            Object.DestroyImmediate(tMesh_SL);
                        }

                        if (road.MeshShoR != null)
                        {
                            MeshCollider tMC = road.MeshShoR.GetComponent<MeshCollider>();
                            MeshRenderer tMR = road.MeshShoR.GetComponent<MeshRenderer>();
                            if (tMC != null)
                            {
                                Object.DestroyImmediate(tMC);
                            }
                            if (tMR != null)
                            {
                                Object.DestroyImmediate(tMR);
                            }
                        }
                        if (road.MeshShoL != null)
                        {
                            MeshCollider tMC = road.MeshShoL.GetComponent<MeshCollider>();
                            MeshRenderer tMR = road.MeshShoL.GetComponent<MeshRenderer>();
                            if (tMC != null)
                            {
                                Object.DestroyImmediate(tMC);
                            }
                            if (tMR != null)
                            {
                                Object.DestroyImmediate(tMR);
                            }
                        }
                    }
                }
                else
                {
                    //Remove main mesh stuff if necessary:
                    if (tMesh_SR != null)
                    {
                        Object.DestroyImmediate(tMesh_SR);
                    }
                    if (tMesh_SL != null)
                    {
                        Object.DestroyImmediate(tMesh_SL);
                    }

                    if (road.MeshShoR != null)
                    {
                        MeshCollider tMC = road.MeshShoR.GetComponent<MeshCollider>();
                        MeshRenderer tMR = road.MeshShoR.GetComponent<MeshRenderer>();
                        if (tMC != null)
                        {
                            Object.DestroyImmediate(tMC);
                        }
                        if (tMR != null)
                        {
                            Object.DestroyImmediate(tMR);
                        }
                    }
                    if (road.MeshShoL != null)
                    {
                        MeshCollider tMC = road.MeshShoL.GetComponent<MeshCollider>();
                        MeshRenderer tMR = road.MeshShoL.GetComponent<MeshRenderer>();
                        if (tMC != null)
                        {
                            Object.DestroyImmediate(tMC);
                        }
                        if (tMR != null)
                        {
                            Object.DestroyImmediate(tMR);
                        }
                    }

                    Mesh tBuffer = null;
                    int xCount = tMesh_SRCuts_world.Count;
                    for (int index = 0; index < xCount; index++)
                    {
                        tBuffer = tMesh_SRCuts_world[index];
                        Object.DestroyImmediate(tBuffer);
                        tMesh_SRCuts_world[index] = null;
                    }
                    xCount = tMesh_SRCuts.Count;
                    for (int index = 0; index < xCount; index++)
                    {
                        tBuffer = tMesh_SRCuts[index];
                        Object.DestroyImmediate(tBuffer);
                        tMesh_SRCuts[index] = null;
                    }
                    xCount = tMesh_SLCuts_world.Count;
                    for (int index = 0; index < xCount; index++)
                    {
                        tBuffer = tMesh_SLCuts_world[index];
                        Object.DestroyImmediate(tBuffer);
                        tMesh_SLCuts_world[index] = null;
                    }
                    xCount = tMesh_SLCuts.Count;
                    for (int index = 0; index < xCount; index++)
                    {
                        tBuffer = tMesh_SLCuts[index];
                        Object.DestroyImmediate(tBuffer);
                        tMesh_SLCuts[index] = null;
                    }

                    if (road.MeshShoR != null)
                    {
                        Object.DestroyImmediate(road.MeshShoR);
                    }

                    if (road.MeshShoL != null)
                    {
                        Object.DestroyImmediate(road.MeshShoL);
                    }
                }

                string basePath = RoadEditorUtility.GetBasePath();

                for (int index = 0; index < RoadConnections_tangents.Count; index++)
                {
                    tMesh_RoadConnections[index].tangents = RoadConnections_tangents[index];
                    GameObject tObj = new GameObject("RoadConnectionMarker");
                    MeshFilter MF = tObj.AddComponent<MeshFilter>();
                    MeshRenderer MR = tObj.AddComponent<MeshRenderer>();
                    float fDist = Vector3.Distance(RoadConnections_verts[index][2], RoadConnections_verts[index][3]);
                    fDist = Mathf.Round(fDist);

                    if (road.laneAmount == 2)
                    {
                        if (fDist == Mathf.Round(road.RoadWidth() * 2f))
                        {
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/GSDRoadConn-4L.mat", MR);
                        }
                        else if (fDist == Mathf.Round(road.RoadWidth() * 3f))
                        {
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/GSDRoadConn-6L-2L.mat", MR);
                        }
                    }
                    else if (road.laneAmount == 4)
                    {
                        if (fDist == Mathf.Round(road.RoadWidth() * 1.5f))
                        {
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/GSDRoadConn-6L-4L.mat", MR);
                        }
                    }
                    MF.sharedMesh = tMesh_RoadConnections[index];
                    tObj.transform.parent = road.MeshRoad.transform;

                    Mesh vMesh = new Mesh();
                    vMesh.vertices = RoadConnections_verts[index];
                    vMesh.triangles = RoadConnections_tris[index];
                    vMesh.normals = RoadConnections_normals[index];
                    Vector2[] vUV = new Vector2[4];
                    vUV[0] = new Vector2(RoadConnections_verts[index][0].x / 5f, RoadConnections_verts[index][0].z / 5f);
                    vUV[1] = new Vector2(RoadConnections_verts[index][1].x / 5f, RoadConnections_verts[index][1].z / 5f);
                    vUV[2] = new Vector2(RoadConnections_verts[index][2].x / 5f, RoadConnections_verts[index][2].z / 5f);
                    vUV[3] = new Vector2(RoadConnections_verts[index][3].x / 5f, RoadConnections_verts[index][3].z / 5f);
                    vMesh.uv = vUV;
                    vMesh.RecalculateNormals();
                    RoadConnections_normals[index] = vMesh.normals;
                    vMesh.tangents = RootUtils.ProcessTangents(vMesh.triangles, vMesh.normals, vMesh.uv, vMesh.vertices);

                    tObj = new GameObject("RoadConnectionBase");
                    MF = tObj.AddComponent<MeshFilter>();
                    MR = tObj.AddComponent<MeshRenderer>();
                    MeshCollider MC = tObj.AddComponent<MeshCollider>();
                    MF.sharedMesh = vMesh;
                    MC.sharedMesh = MF.sharedMesh;
                    RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/GSDRoad1.mat", MR);
                    tObj.transform.parent = road.MeshRoad.transform;

                    SaveMesh(SaveMeshTypeEnum.RoadConn, vMesh, road, "RoadConn" + index.ToString());
                }
            }

            if (isInterseOn)
            {
                MeshSetup2Intersections();
            }

            if (road.MeshiLanes != null)
            {
                Object.DestroyImmediate(road.MeshiLanes);
            }
            if (road.MeshiLanes0 != null)
            {
                Object.DestroyImmediate(road.MeshiLanes0);
            }
            if (road.MeshiLanes1 != null)
            {
                Object.DestroyImmediate(road.MeshiLanes1);
            }
            if (road.MeshiLanes2 != null)
            {
                Object.DestroyImmediate(road.MeshiLanes2);
            }
            if (road.MeshiLanes3 != null)
            {
                Object.DestroyImmediate(road.MeshiLanes3);
            }
            if (road.MeshiMainPlates != null)
            {
                Object.DestroyImmediate(road.MeshiMainPlates);
            }
            if (road.MeshiMarkerPlates != null)
            {
                Object.DestroyImmediate(road.MeshiMarkerPlates);
            }

            //Updates the road and shoulder cut materials if necessary. Note: Cycling through all nodes in case the road cuts and shoulder cut numbers don't match.
            if (road.isRoadCutsEnabled || road.isShoulderCutsEnabled || road.isDynamicCutsEnabled)
            {
                int mCount = road.spline.GetNodeCount();
                for (int index = 0; index < mCount; index++)
                {
                    road.spline.nodes[index].UpdateCuts();
                }
            }
        }


        #region "MeshSetup2 - Intersections"
        //		private void MeshSetup2_Intersections_FixNormals(){
        //			int mCount = tRoad.GSDSpline.GetNodeCount();
        //			GSDSplineN tNode = null;
        //			GSDRoadIntersection GSDRI = null;
        //			float MaxDist = 0f;
        //			float[] tDists = new float[2];
        //			Collider[] tColliders = null;
        //			List<GameObject> tCuts = null;
        //
        //			for(int h=0;h<mCount;h++){
        //				tNode=tRoad.GSDSpline.mNodes[h];
        //				if(tNode.bIsIntersection){
        //					GSDRI = tNode.GSDRI;
        //					
        //					
        //					
        //
        //					tColliders = Physics.OverlapSphere(GSDRI.CornerRR_Outer,tRoad.opt_ShoulderWidth*1.25f);
        //					tCuts = new List<GameObject>();
        //					foreach(Collider tCollider in tColliders){
        //						if(tCollider.transform.name.Contains("cut")){
        //							tCuts.Add(tCollider.transform.gameObject);
        //						}
        //					}
        //					
        //					
        //					
        //					foreach(GameObject tObj in tCuts){
        //						MeshFilter MF1 = tCuts[0].GetComponent<MeshFilter>();
        //						if(MF1 == null){ continue; }
        //						Mesh zMesh1 = MF1.sharedMesh;
        //						Vector3[] tVerts1 = zMesh1.vertices;
        //						Vector3[] tNormals1 = zMesh1.normals;
        //						int MVL1 = tVerts1.Length;
        //						for(int i=0;i<MVL1;i++){
        //							if(tVerts1[i] == GSDRI.CornerRR){
        //								tNormals1[i] = Vector3.up;
        //							}else if(tVerts1[i] == GSDRI.CornerRR_Outer){
        //								tNormals1[i] = Vector3.up;
        //							}
        //						}
        //					}
        //
        //					
        //				}
        //			}
        //		}


        private void MeshSetup2Intersections()
        {
            int mCount = road.spline.GetNodeCount();
            bool bHasInter = false;
            for (int index = 0; index < mCount; index++)
            {
                if (road.spline.nodes[index].isIntersection)
                {
                    bHasInter = true;
                    break;
                }
            }
            if (!bHasInter)
            {
                return;
            }


            int vCount = -1;
            Mesh xMesh = null;
            Vector2[] tUV = null;
            Vector4[] tTangents = null;
            MeshFilter MF = null;
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane0 = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane1 = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane2 = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane3 = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_MainPlate = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_MainPlateM = new Dictionary<RoadIntersection, List<MeshFilter>>();
            HashSet<RoadIntersection> UniqueGSDRI = new HashSet<RoadIntersection>();

            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane1_Disabled = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane2_Disabled = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane2_DisabledActive = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane2_DisabledActiveR = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane3_Disabled = new Dictionary<RoadIntersection, List<MeshFilter>>();
            Dictionary<RoadIntersection, List<MeshFilter>> tCombineDict_Lane1_DisabledActive = new Dictionary<RoadIntersection, List<MeshFilter>>();

            string basePath = RoadEditorUtility.GetBasePath();

            vCount = iBLane0s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tUV = iBLane0s_uv[index];
                tTangents = iBLane0s_tangents[index];
                xMesh = tMesh_iBLanes0[index];
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes0, "Lane0B", basePath + "/Materials/Markers/GSDInterWhiteLYellowR.mat");
                if (!tCombineDict_Lane0.ContainsKey(iBLane0s_tID[index]))
                {
                    tCombineDict_Lane0.Add(iBLane0s_tID[index], new List<MeshFilter>());
                }
                tCombineDict_Lane0[iBLane0s_tID[index]].Add(MF);
            }
            vCount = iBLane1s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool isPrimaryNode = (iBLane1s_tID[index].node1 == iBLane1s_nID[index]);
                if (!isPrimaryNode && iBLane1s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && (iBLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.TurnLane || iBLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes) && !iBLane1s_tID[index].isNode2BLeftTurnLane)
                {
                    tUV = iBLane1s_uv[index];
                    tTangents = iBLane1s_tangents[index];
                    xMesh = tMesh_iBLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneD1B", basePath + "/Materials/Markers/GSDInterLaneDisabled.mat");
                    if (!tCombineDict_Lane1_Disabled.ContainsKey(iBLane1s_tID[index]))
                    {
                        tCombineDict_Lane1_Disabled.Add(iBLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1_Disabled[iBLane1s_tID[index]].Add(MF);
                }
                else if (isPrimaryNode && iBLane1s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iBLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    tUV = iBLane1s_uv[index];
                    tTangents = iBLane1s_tangents[index];
                    xMesh = tMesh_iBLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneDA1B", basePath + "/Materials/Markers/GSDInterLaneDisabledOuter.mat");
                    if (!tCombineDict_Lane1_DisabledActive.ContainsKey(iBLane1s_tID[index]))
                    {
                        tCombineDict_Lane1_DisabledActive.Add(iBLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1_DisabledActive[iBLane1s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iBLane1s_uv[index];
                    tTangents = iBLane1s_tangents[index];
                    xMesh = tMesh_iBLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "Lane1B", basePath + "/Materials/Markers/GSDInterYellowLWhiteR.mat");
                    if (!tCombineDict_Lane1.ContainsKey(iBLane1s_tID[index]))
                    {
                        tCombineDict_Lane1.Add(iBLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1[iBLane1s_tID[index]].Add(MF);
                }
            }
            vCount = iBLane2s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool bPrimaryNode = (iBLane2s_tID[index].node1 == iBLane2s_nID[index]);
                if (!bPrimaryNode && iBLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && (iBLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.TurnLane || iBLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes) && !iBLane2s_tID[index].isNode2BLeftTurnLane)
                {
                    tUV = iBLane2s_uv[index];
                    tTangents = iBLane2s_tangents[index];
                    xMesh = tMesh_iBLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2B", basePath + "/Materials/Markers/GSDInterLaneDisabledOuter.mat");
                    if (!tCombineDict_Lane2_DisabledActive.ContainsKey(iBLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_DisabledActive.Add(iBLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_DisabledActive[iBLane2s_tID[index]].Add(MF);
                }
                else if (!bPrimaryNode && iBLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iBLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes && !iBLane2s_tID[index].isNode2BRightTurnLane)
                {
                    tUV = iBLane2s_uv[index];
                    tTangents = iBLane2s_tangents[index];
                    xMesh = tMesh_iBLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2B", basePath + "/Materials/Markers/GSDInterLaneDisabledOuterR.mat");
                    if (!tCombineDict_Lane2_DisabledActiveR.ContainsKey(iBLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_DisabledActiveR.Add(iBLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_DisabledActiveR[iBLane2s_tID[index]].Add(MF);
                }
                else if (bPrimaryNode && iBLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iBLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    tUV = iBLane2s_uv[index];
                    tTangents = iBLane2s_tangents[index];
                    xMesh = tMesh_iBLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneD2B", basePath + "/Materials/Markers/GSDInterLaneDisabled.mat");
                    if (!tCombineDict_Lane2_Disabled.ContainsKey(iBLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_Disabled.Add(iBLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_Disabled[iBLane2s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iBLane2s_uv[index];
                    tTangents = iBLane2s_tangents[index];
                    xMesh = tMesh_iBLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "Lane2B", basePath + "/Materials/Markers/GSDInterWhiteR.mat");
                    if (!tCombineDict_Lane2.ContainsKey(iBLane2s_tID[index]))
                    {
                        tCombineDict_Lane2.Add(iBLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2[iBLane2s_tID[index]].Add(MF);
                }
            }
            vCount = iBLane3s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool bPrimaryNode = (iBLane3s_tID[index].node1 == iBLane3s_nID[index]);
                if (!bPrimaryNode && iBLane3s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iBLane3s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes && !iBLane3s_tID[index].isNode2BRightTurnLane)
                {
                    tUV = iBLane3s_uv[index];
                    tTangents = iBLane3s_tangents[index];
                    xMesh = tMesh_iBLanes3[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "LaneD3B", basePath + "/Materials/Markers/GSDInterLaneDisabled.mat");
                    if (!tCombineDict_Lane3_Disabled.ContainsKey(iBLane3s_tID[index]))
                    {
                        tCombineDict_Lane3_Disabled.Add(iBLane3s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane3_Disabled[iBLane3s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iBLane3s_uv[index];
                    tTangents = iBLane3s_tangents[index];
                    xMesh = tMesh_iBLanes3[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "Lane3B", basePath + "/Materials/Markers/GSDInterWhiteR.mat");
                    if (!tCombineDict_Lane3.ContainsKey(iBLane3s_tID[index]))
                    {
                        tCombineDict_Lane3.Add(iBLane3s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane3[iBLane3s_tID[index]].Add(MF);
                }
            }

            //Front lanes:
            vCount = iFLane0s.Count;
            for (int index = 0; index < vCount; index++)
            {
                tUV = iFLane0s_uv[index];
                tTangents = iFLane0s_tangents[index];
                xMesh = tMesh_iFLanes0[index];
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes0, "Lane0F", basePath + "/Materials/Markers/GSDInterWhiteLYellowR.mat");
                if (!tCombineDict_Lane0.ContainsKey(iFLane0s_tID[index]))
                {
                    tCombineDict_Lane0.Add(iFLane0s_tID[index], new List<MeshFilter>());
                }
                tCombineDict_Lane0[iFLane0s_tID[index]].Add(MF);
            }
            vCount = iFLane1s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool bPrimaryNode = (iFLane1s_tID[index].node1 == iFLane1s_nID[index]);
                if (!bPrimaryNode && iFLane1s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && (iFLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes || iFLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.TurnLane) && !iFLane1s_tID[index].isNode2FLeftTurnLane)
                {
                    tUV = iFLane1s_uv[index];
                    tTangents = iFLane1s_tangents[index];
                    xMesh = tMesh_iFLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneD1F", basePath + "/Materials/Markers/GSDInterLaneDisabled.mat");
                    if (!tCombineDict_Lane1_Disabled.ContainsKey(iFLane1s_tID[index]))
                    {
                        tCombineDict_Lane1_Disabled.Add(iFLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1_Disabled[iFLane1s_tID[index]].Add(MF);
                }
                else if (bPrimaryNode && iFLane1s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iFLane1s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    tUV = iFLane1s_uv[index];
                    tTangents = iFLane1s_tangents[index];
                    xMesh = tMesh_iFLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneDAR1F", basePath + "/Materials/Markers/GSDInterLaneDisabledOuterR.mat");
                    if (!tCombineDict_Lane1_DisabledActive.ContainsKey(iFLane1s_tID[index]))
                    {
                        tCombineDict_Lane1_DisabledActive.Add(iFLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1_DisabledActive[iFLane1s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iFLane1s_uv[index];
                    tTangents = iFLane1s_tangents[index];
                    xMesh = tMesh_iFLanes1[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "Lane1F", basePath + "/Materials/Markers/GSDInterYellowLWhiteR.mat");
                    if (!tCombineDict_Lane1.ContainsKey(iFLane1s_tID[index]))
                    {
                        tCombineDict_Lane1.Add(iFLane1s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane1[iFLane1s_tID[index]].Add(MF);
                }
            }
            vCount = iFLane2s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool bPrimaryNode = (iFLane2s_tID[index].node1 == iFLane2s_nID[index]);
                if (!bPrimaryNode && iFLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && (iFLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes || iFLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.TurnLane) && !iFLane2s_tID[index].isNode2FLeftTurnLane)
                {
                    tUV = iFLane2s_uv[index];
                    tTangents = iFLane2s_tangents[index];
                    xMesh = tMesh_iFLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2F", basePath + "/Materials/Markers/GSDInterLaneDisabledOuter.mat");
                    if (!tCombineDict_Lane2_DisabledActive.ContainsKey(iFLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_DisabledActive.Add(iFLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_DisabledActive[iFLane2s_tID[index]].Add(MF);
                }
                else if (!bPrimaryNode && iFLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iFLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes && !iFLane2s_tID[index].isNode2FRightTurnLane)
                {
                    tUV = iFLane2s_uv[index];
                    tTangents = iFLane2s_tangents[index];
                    xMesh = tMesh_iFLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDAR2F", basePath + "/Materials/Markers/GSDInterLaneDisabledOuterR.mat");
                    if (!tCombineDict_Lane2_DisabledActiveR.ContainsKey(iFLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_DisabledActiveR.Add(iFLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_DisabledActiveR[iFLane2s_tID[index]].Add(MF);
                }
                else if (bPrimaryNode && iFLane2s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iFLane2s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    tUV = iFLane2s_uv[index];
                    tTangents = iFLane2s_tangents[index];
                    xMesh = tMesh_iFLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneD2F", basePath + "/Materials/Markers/GSDInterLaneDisabled.mat");
                    if (!tCombineDict_Lane2_Disabled.ContainsKey(iFLane2s_tID[index]))
                    {
                        tCombineDict_Lane2_Disabled.Add(iFLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2_Disabled[iFLane2s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iFLane2s_uv[index];
                    tTangents = iFLane2s_tangents[index];
                    xMesh = tMesh_iFLanes2[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "Lane2F", basePath + "/Materials/Markers/GSDInterWhiteR.mat");
                    if (!tCombineDict_Lane2.ContainsKey(iFLane2s_tID[index]))
                    {
                        tCombineDict_Lane2.Add(iFLane2s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane2[iFLane2s_tID[index]].Add(MF);
                }
            }
            vCount = iFLane3s.Count;
            for (int index = 0; index < vCount; index++)
            {
                bool bPrimaryNode = (iFLane3s_tID[index].node1 == iFLane3s_nID[index]);
                if (!bPrimaryNode && iFLane3s_tID[index].intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay && iFLane3s_tID[index].roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes && !iFLane3s_tID[index].isNode2FRightTurnLane)
                {
                    tUV = iFLane3s_uv[index];
                    tTangents = iFLane3s_tangents[index];
                    xMesh = tMesh_iFLanes3[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "LaneD3F", basePath + "/Materials/Markers/GSDInterWhiteR.mat");
                    if (!tCombineDict_Lane3_Disabled.ContainsKey(iFLane3s_tID[index]))
                    {
                        tCombineDict_Lane3_Disabled.Add(iFLane3s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane3_Disabled[iFLane3s_tID[index]].Add(MF);
                }
                else
                {
                    tUV = iFLane3s_uv[index];
                    tTangents = iFLane3s_tangents[index];
                    xMesh = tMesh_iFLanes3[index];
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "Lane3F", basePath + "/Materials/Markers/GSDInterWhiteR.mat");
                    if (!tCombineDict_Lane3.ContainsKey(iFLane3s_tID[index]))
                    {
                        tCombineDict_Lane3.Add(iFLane3s_tID[index], new List<MeshFilter>());
                    }
                    tCombineDict_Lane3[iFLane3s_tID[index]].Add(MF);
                }
            }

            //Main plates:
            vCount = iBMainPlates.Count;
            for (int index = 0; index < vCount; index++)
            {
                tUV = iBMainPlates_uv[index];
                tTangents = iBMainPlates_tangents[index];
                xMesh = tMesh_iBMainPlates[index];
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateB", basePath + "/Materials/GSDRoad1.mat", false);
                if (!tCombineDict_MainPlate.ContainsKey(iBMainPlates_tID[index]))
                {
                    tCombineDict_MainPlate.Add(iBMainPlates_tID[index], new List<MeshFilter>());
                }
                tCombineDict_MainPlate[iBMainPlates_tID[index]].Add(MF);

                Mesh fMesh = new Mesh();
                fMesh.vertices = iBMainPlates[index];
                fMesh.triangles = iBMainPlates_tris[index];
                fMesh.normals = iBMainPlates_normals[index];
                tUV = iBMainPlates_uv2[index];
                tTangents = iBMainPlates_tangents2[index];
                MF = MeshSetup2IntersectionHelper(ref fMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateBM", basePath + "/Materials/GSDInterMainPlate1.mat");
                if (!tCombineDict_MainPlateM.ContainsKey(iBMainPlates_tID[index]))
                {
                    tCombineDict_MainPlateM.Add(iBMainPlates_tID[index], new List<MeshFilter>());
                }
                tCombineDict_MainPlateM[iBMainPlates_tID[index]].Add(MF);
            }
            vCount = iFMainPlates.Count;
            for (int index = 0; index < vCount; index++)
            {
                tUV = iFMainPlates_uv[index];
                tTangents = iFMainPlates_tangents[index];
                xMesh = tMesh_iFMainPlates[index];
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateFM", basePath + "/Materials/GSDRoad1.mat", false);

                if (!tCombineDict_MainPlate.ContainsKey(iFMainPlates_tID[index]))
                {
                    tCombineDict_MainPlate.Add(iFMainPlates_tID[index], new List<MeshFilter>());
                }
                tCombineDict_MainPlate[iFMainPlates_tID[index]].Add(MF);

                Mesh tMesh = new Mesh();
                tMesh.vertices = iFMainPlates[index];
                tMesh.triangles = iFMainPlates_tris[index];
                tMesh.normals = iFMainPlates_normals[index];
                tUV = iFMainPlates_uv2[index];
                tTangents = iFMainPlates_tangents2[index];
                MF = MeshSetup2IntersectionHelper(ref tMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateFM", basePath + "/Materials/GSDInterMainPlate1.mat");
                if (!tCombineDict_MainPlateM.ContainsKey(iFMainPlates_tID[index]))
                {
                    tCombineDict_MainPlateM.Add(iFMainPlates_tID[index], new List<MeshFilter>());
                }
                tCombineDict_MainPlateM[iFMainPlates_tID[index]].Add(MF);
            }

            vCount = road.spline.GetNodeCount();
            SplineN tNode = null;
            for (int index = 0; index < vCount; index++)
            {
                tNode = road.spline.nodes[index];
                if (tNode.isIntersection && tNode.intersection != null && tNode.intersection.node1 == tNode)
                {
                    //Create center plate
                    Vector3[] xVerts = new Vector3[4];
                    xVerts[0] = tNode.intersection.cornerLR;
                    xVerts[1] = tNode.intersection.cornerRR;
                    xVerts[2] = tNode.intersection.cornerLL;
                    xVerts[3] = tNode.intersection.cornerRL;


                    int[] xTris = new int[6];
                    xTris[0] = 0;
                    xTris[1] = 2;
                    xTris[2] = 1;
                    xTris[3] = 2;
                    xTris[4] = 3;
                    xTris[5] = 1;

                    Vector2[] xUV = new Vector2[4];
                    xUV[0] = new Vector2(xVerts[0].x / 5f, xVerts[0].z / 5f);
                    xUV[1] = new Vector2(xVerts[1].x / 5f, xVerts[1].z / 5f);
                    xUV[2] = new Vector2(xVerts[2].x / 5f, xVerts[2].z / 5f);
                    xUV[3] = new Vector2(xVerts[3].x / 5f, xVerts[3].z / 5f);

                    Vector2[] xUV2 = new Vector2[4];
                    xUV2[0] = new Vector2(0f, 0f);
                    xUV2[1] = new Vector2(1f, 0f);
                    xUV2[2] = new Vector2(0f, 1f);
                    xUV2[3] = new Vector2(1f, 1f);

                    Mesh vMesh = new Mesh();
                    vMesh.vertices = xVerts;
                    vMesh.triangles = xTris;
                    vMesh.normals = new Vector3[4];
                    vMesh.uv = xUV;
                    vMesh.RecalculateBounds();
                    vMesh.RecalculateNormals();
                    vMesh.tangents = RootUtils.ProcessTangents(xTris, vMesh.normals, xUV, xVerts);
                    if (road.isLightmapped)
                    {
                        //UnityEditor.Unwrapping.GenerateSecondaryUVSet(vMesh);
                    }

                    int cCount = tNode.intersection.transform.childCount;
                    List<GameObject> GOToDelete = new List<GameObject>();
                    for (int j = 0; j < cCount; j++)
                    {
                        if (tNode.intersection.transform.GetChild(j).name.ToLower() == "tcenter")
                        {
                            GOToDelete.Add(tNode.intersection.transform.GetChild(j).gameObject);
                        }
                        if (tNode.intersection.transform.GetChild(j).name.ToLower() == "markcenter")
                        {
                            GOToDelete.Add(tNode.intersection.transform.GetChild(j).gameObject);
                        }
                    }
                    for (int j = GOToDelete.Count - 1; j >= 0; j--)
                    {
                        Object.DestroyImmediate(GOToDelete[j]);
                    }

                    GameObject tCenter = new GameObject("tCenter");
                    MF = tCenter.AddComponent<MeshFilter>();
                    MF.sharedMesh = vMesh;
                    tCenter.transform.parent = tNode.intersection.transform;
                    if (road.isLightmapped)
                    {
                        #if UNITY_2019_2_OR_NEWER
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tCenter, UnityEditor.StaticEditorFlags.ContributeGI);
                        #else
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tCenter, UnityEditor.StaticEditorFlags.LightmapStatic);
                        #endif
                    }
                    if (road.isStatic)
                    {
                        tCenter.isStatic = true;
                    }



                    Mesh mMesh = new Mesh();
                    Vector3[] bVerts = new Vector3[4];
                    mMesh.vertices = xVerts;
                    mMesh.triangles = xTris;
                    mMesh.normals = new Vector3[4];
                    mMesh.uv = xUV2;
                    mMesh.RecalculateBounds();
                    mMesh.RecalculateNormals();
                    mMesh.tangents = RootUtils.ProcessTangents(xTris, vMesh.normals, xUV, xVerts);


                    GameObject tMarker = new GameObject("CenterMarkers");
                    //tMarker.transform.localPosition = default(Vector3);
                    MF = tMarker.AddComponent<MeshFilter>();
                    MF.sharedMesh = mMesh;
                    MeshRenderer MR = tMarker.AddComponent<MeshRenderer>();
                    MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    //					if(tNode.GSDRI.MarkerCenter != null){
                    ////						MR.material = tNode.GSDRI.MarkerCenter;
                    //					}
                    tMarker.transform.parent = tNode.intersection.transform;
                    if (road.isLightmapped)
                    {
                        #if UNITY_2019_2_OR_NEWER
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tMarker, UnityEditor.StaticEditorFlags.ContributeGI);
                        #else
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tMarker, UnityEditor.StaticEditorFlags.LightmapStatic);
                        #endif
                    }
                    if (road.isStatic)
                    {
                        tMarker.isStatic = true;
                    }

                    bVerts = MF.sharedMesh.vertices;
                    for (int j = 0; j < 4; j++)
                    {
                        bVerts[j].y = tNode.intersection.signHeight;
                    }


                    int zCount = bVerts.Length;
                    for (int z = 0; z < zCount; z++)
                    {
                        bVerts[z] -= tNode.transform.position;
                    }

                    MF.sharedMesh.vertices = bVerts;



                    MR.transform.position = tNode.intersection.transform.position;

                    mMesh.RecalculateBounds();
                    if (road.isLightmapped)
                    {
                        //UnityEditor.Unwrapping.GenerateSecondaryUVSet(mMesh);
                    }
                    SaveMesh(SaveMeshTypeEnum.Intersection, MF.sharedMesh, road, tNode.intersection.transform.name + "-" + "CenterMarkers");
                }
            }

            //			List<Mesh> MeshToDelete = new List<Mesh>();

            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane0)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane0");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane1");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane3)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane3");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_MainPlate)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "TiledExt", true);
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_MainPlateM)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "StretchExt");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1_Disabled)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD1");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane3_Disabled)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD3");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_DisabledActive)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDA2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_DisabledActiveR)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDAR2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_Disabled)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1_DisabledActive)
            {
                if (!UniqueGSDRI.Contains(KVP.Key))
                {
                    UniqueGSDRI.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDA1");
            }

            foreach (RoadIntersection GSDRI in UniqueGSDRI)
            {
                GSDRI.UpdateMaterials();
            }

        }


        private void MeshSetup2CombineIntersections(KeyValuePair<RoadIntersection, List<MeshFilter>> _valuePair, string _name, bool _isMainPlates = false)
        {
            int vCount = _valuePair.Value.Count;
            if (vCount < 1)
            {
                return;
            }
            _name = road.name + "-" + _name;
            GameObject tCenter = null;
            int cCount = _valuePair.Key.transform.childCount;
            List<GameObject> GOToDelete = new List<GameObject>();
            for (int index = 0; index < cCount; index++)
            {
                if (_valuePair.Key.transform.GetChild(index).name.ToLower() == _name.ToLower())
                {
                    GOToDelete.Add(_valuePair.Key.transform.GetChild(index).gameObject);
                }
                if (_isMainPlates && _valuePair.Key.transform.GetChild(index).name.ToLower() == "tcenter")
                {
                    tCenter = _valuePair.Key.transform.GetChild(index).gameObject;
                }
            }
            for (int index = GOToDelete.Count - 1; index >= 0; index--)
            {
                Object.DestroyImmediate(GOToDelete[index]);
            }

            int CombineCount = vCount;
            if (tCenter != null)
            {
                CombineCount += 1;
            }
            CombineInstance[] combine = new CombineInstance[CombineCount];
            for (int index = 0; index < vCount; index++)
            {
                combine[index].mesh = _valuePair.Value[index].sharedMesh;
                combine[index].transform = _valuePair.Value[index].transform.localToWorldMatrix;
            }

            int SpecialVertCount = 0;
            if (tCenter != null)
            {
                for (int index = 0; index < (CombineCount - 1); index++)
                {
                    SpecialVertCount += combine[index].mesh.vertexCount;
                }
                MeshFilter tMF = tCenter.GetComponent<MeshFilter>();
                Vector3[] xVerts = tMF.sharedMesh.vertices;
                float xHeight = combine[0].mesh.vertices[combine[0].mesh.vertexCount - 1].y;
                for (int index = 0; index < xVerts.Length; index++)
                {
                    xVerts[index].y = xHeight;
                }
                tMF.sharedMesh.vertices = xVerts;
                combine[CombineCount - 1].mesh = tMF.sharedMesh;
                combine[CombineCount - 1].transform = tMF.transform.localToWorldMatrix;
            }

            GameObject tObj = new GameObject(_name);
            MeshFilter MF = tObj.AddComponent<MeshFilter>();
            Mesh tMesh = new Mesh();
            tMesh.CombineMeshes(combine);
            MF.sharedMesh = tMesh;
            MeshRenderer MR = tObj.AddComponent<MeshRenderer>();
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tObj.transform.parent = _valuePair.Key.transform;
            Vector3[] tVerts = MF.sharedMesh.vertices;
            Vector3 tVect = tObj.transform.localPosition;
            //			float tHeight = 0f;
            for (int index = 0; index < tVerts.Length; index++)
            {
                tVerts[index] += tVect;
                if (_name.ToLower().EndsWith("-stretchext"))
                    tVerts[index] += new Vector3(0f, 0.01f);
            }
            MF.sharedMesh.vertices = tVerts;
            tObj.transform.localPosition = new Vector3(0f, 0f, 0f);
            MF.sharedMesh.RecalculateBounds();
            MF.sharedMesh.RecalculateNormals();
            MF.sharedMesh.tangents = RootUtils.ProcessTangents(MF.sharedMesh.triangles, MF.sharedMesh.normals, MF.sharedMesh.uv, MF.sharedMesh.vertices);
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(MF.sharedMesh);
            }
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            if (road.isStatic)
            {
                tObj.isStatic = true;
            }


            if (_isMainPlates)
            {
                MeshCollider MC = tObj.AddComponent<MeshCollider>();
                MC.sharedMesh = MF.sharedMesh;
                MC.material = road.RoadPhysicMaterial;
                MC.material.name = MC.material.name.Replace(" (Instance)", "");
            }

            if (_isMainPlates && tCenter != null)
            {
                Object.DestroyImmediate(tCenter);
            }

            SaveMesh(SaveMeshTypeEnum.Intersection, MF.sharedMesh, road, _name);
        }



        private MeshFilter MeshSetup2IntersectionHelper(ref Mesh _mesh, ref Vector2[] _uv, ref Vector4[] _tangents, ref GameObject _masterObj, string _name, string _mat, bool _isCollider = false)
        {
            if (_mesh == null)
            {
                return null;
            }
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }

            GameObject tObj = new GameObject(_name);
            tObj.transform.parent = _masterObj.transform;
            MeshFilter MF = tObj.AddComponent<MeshFilter>();
            MF.sharedMesh = _mesh;
            if (_isCollider)
            {
                MeshCollider MC = tObj.AddComponent<MeshCollider>();
                MC.sharedMesh = MF.sharedMesh;
            }
            if (_mat.Length < 1)
            {
                return null;
            }
            MeshRenderer MR = tObj.AddComponent<MeshRenderer>();
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            RoadEditorUtility.SetRoadMaterial(_mat, MR);
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            if (road.isStatic)
            {
                tObj.isStatic = true;
            }

            return MF;
        }
        #endregion


        private Mesh MeshSetup2Helper(ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _obj, bool _isMarker, bool _isShoulder = false, bool _isBridge = false)
        {
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            MeshFilter MF = _obj.AddComponent<MeshFilter>();
            MF.sharedMesh = _mesh;
            MeshCollider MC = null;
            if (road.isUsingMeshColliders)
            {
                MC = _obj.AddComponent<MeshCollider>();
                MC.sharedMesh = MF.sharedMesh;
            }
            MeshRenderer MR = _obj.AddComponent<MeshRenderer>();

            if (_isShoulder)
            {
                int mCounter = 0;
                if (road.ShoulderMaterial1 != null)
                {
                    mCounter += 1;
                    if (road.ShoulderMaterial2 != null)
                    {
                        mCounter += 1;
                        if (road.ShoulderMaterial3 != null)
                        {
                            mCounter += 1;
                            if (road.ShoulderMaterial4 != null)
                            {
                                mCounter += 1;
                            }
                        }
                    }
                }

                if (mCounter > 0)
                {
                    Material[] tMats = new Material[mCounter];
                    if (road.ShoulderMaterial1 != null)
                    {
                        tMats[0] = road.ShoulderMaterial1;
                        if (road.ShoulderMaterial2 != null)
                        {
                            tMats[1] = road.ShoulderMaterial2;
                            if (road.ShoulderMaterial3 != null)
                            {
                                tMats[2] = road.ShoulderMaterial3;
                                if (road.ShoulderMaterial4 != null)
                                {
                                    tMats[3] = road.ShoulderMaterial4;
                                }
                            }
                        }
                    }
                    MR.materials = tMats;
                }
            }
            else
            {
                if (_isMarker)
                {
                    int mCounter = 0;
                    if (road.RoadMaterialMarker1 != null)
                    {
                        mCounter += 1;
                        if (road.RoadMaterialMarker2 != null)
                        {
                            mCounter += 1;
                            if (road.RoadMaterialMarker3 != null)
                            {
                                mCounter += 1;
                                if (road.RoadMaterialMarker4 != null)
                                {
                                    mCounter += 1;
                                }
                            }
                        }
                    }

                    if (mCounter > 0)
                    {
                        Material[] tMats = new Material[mCounter];
                        if (road.RoadMaterialMarker1 != null)
                        {
                            tMats[0] = road.RoadMaterialMarker1;
                            if (road.RoadMaterialMarker2 != null)
                            {
                                tMats[1] = road.RoadMaterialMarker2;
                                if (road.RoadMaterialMarker3 != null)
                                {
                                    tMats[2] = road.RoadMaterialMarker3;
                                    if (road.RoadMaterialMarker4 != null)
                                    {
                                        tMats[3] = road.RoadMaterialMarker4;
                                    }
                                }
                            }
                        }
                        MR.materials = tMats;
                    }
                }
                else
                {
                    int mCounter = 0;
                    if (road.RoadMaterial1 != null)
                    {
                        mCounter += 1;
                        if (road.RoadMaterial2 != null)
                        {
                            mCounter += 1;
                            if (road.RoadMaterial3 != null)
                            {
                                mCounter += 1;
                                if (road.RoadMaterial4 != null)
                                {
                                    mCounter += 1;
                                }
                            }
                        }
                    }
                    if (mCounter > 0)
                    {
                        Material[] tMats = new Material[mCounter];
                        if (road.RoadMaterial1 != null)
                        {
                            tMats[0] = road.RoadMaterial1;
                            if (road.RoadMaterial2 != null)
                            {
                                tMats[1] = road.RoadMaterial2;
                                if (road.RoadMaterial3 != null)
                                {
                                    tMats[2] = road.RoadMaterial3;
                                    if (road.RoadMaterial4 != null)
                                    {
                                        tMats[3] = road.RoadMaterial4;
                                    }
                                }
                            }
                        }
                        MR.materials = tMats;
                    }

                    if (MC)
                    {
                        MC.sharedMaterial = road.RoadPhysicMaterial;
                    }
                }
            }

            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_obj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            if (road.isStatic)
            {
                _obj.isStatic = true;
            }

            return _mesh;
        }


        private bool MeshSetup2HelperRoadCuts(int _i, ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _masterObj, bool _isMarkers, out GameObject _createdObj)
        {
            string tName = "RoadCut" + _i.ToString();
            if (_isMarkers)
            {
                tName = "Markers" + _i.ToString();
            }
            _createdObj = new GameObject(tName);

            if (!_isMarkers)
            {
                RoadCutNodes[_i].roadCutWorld = _createdObj;
            }
            else
            {
                RoadCutNodes[_i].roadCutMarker = _createdObj;
            }

            _createdObj.transform.position = cut_RoadVectorsHome[_i];
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            MeshFilter MF = _createdObj.AddComponent<MeshFilter>();
            MF.sharedMesh = _mesh;

            MeshCollider MC = null;
            if (road.isUsingMeshColliders && !_isMarkers)
            {
                MC = _createdObj.AddComponent<MeshCollider>();
                if (MC.sharedMesh == null)
                {
                    MC.sharedMesh = MF.sharedMesh;
                }
            }
            MeshRenderer MR = _createdObj.AddComponent<MeshRenderer>();

            //Disable shadows for road cuts and markers:
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            int mCounter = 0;
            bool bHasMats = false;

            if (_isMarkers)
            {


                //Get the mat count:
                if (road.RoadMaterialMarker1 != null)
                {
                    mCounter += 1;
                    if (road.RoadMaterialMarker2 != null)
                    {
                        mCounter += 1;
                        if (road.RoadMaterialMarker3 != null)
                        {
                            mCounter += 1;
                            if (road.RoadMaterialMarker4 != null)
                            {
                                mCounter += 1;
                            }
                        }
                    }
                }
                //Apply mats:
                if (mCounter > 0)
                {
                    Material[] tMats = new Material[mCounter];
                    if (road.RoadMaterialMarker1 != null)
                    {
                        tMats[0] = road.RoadMaterialMarker1;
                        if (road.RoadMaterialMarker2 != null)
                        {
                            tMats[1] = road.RoadMaterialMarker2;
                            if (road.RoadMaterialMarker3 != null)
                            {
                                tMats[2] = road.RoadMaterialMarker3;
                                if (road.RoadMaterialMarker4 != null)
                                {
                                    tMats[3] = road.RoadMaterialMarker4;
                                }
                            }
                        }
                    }
                    MR.materials = tMats;
                    bHasMats = true;
                }
            }
            else
            {
                //Get the mat count:
                if (road.RoadMaterial1 != null)
                {
                    mCounter += 1;
                    if (road.RoadMaterial2 != null)
                    {
                        mCounter += 1;
                        if (road.RoadMaterial3 != null)
                        {
                            mCounter += 1;
                            if (road.RoadMaterial4 != null)
                            {
                                mCounter += 1;
                            }
                        }
                    }
                }
                //Apply mats:
                if (mCounter > 0)
                {
                    Material[] tMats = new Material[mCounter];
                    if (road.RoadMaterial1 != null)
                    {
                        tMats[0] = road.RoadMaterial1;
                        if (road.RoadMaterial2 != null)
                        {
                            tMats[1] = road.RoadMaterial2;
                            if (road.RoadMaterial3 != null)
                            {
                                tMats[2] = road.RoadMaterial3;
                                if (road.RoadMaterial4 != null)
                                {
                                    tMats[3] = road.RoadMaterial4;
                                }
                            }
                        }
                    }
                    MR.materials = tMats;
                    bHasMats = true;
                }
            }

            _createdObj.transform.parent = _masterObj.transform;
            if (!_isMarkers && MC != null)
            {
                MC.sharedMaterial = road.RoadPhysicMaterial;
            }
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(CreatedObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            if (road.isStatic)
            {
                _createdObj.isStatic = true;
            }

            return bHasMats;
        }


        private bool MeshSetup2HelperCutsShoulder(int _i, ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _masterObj, bool _isLeft, bool _isMarkers, out GameObject _createdObj)
        {

            string tName = "";
            if (_isMarkers)
            {
                if (_isLeft)
                {
                    tName = "Markers" + _i.ToString();
                }
                else
                {
                    tName = "Markers" + _i.ToString();
                }
            }
            else
            {
                if (_isLeft)
                {
                    tName = "SCutL" + _i.ToString();
                }
                else
                {
                    tName = "SCutR" + _i.ToString();
                }
            }

            _createdObj = new GameObject(tName);
            if (_isLeft)
            {
                _createdObj.transform.position = cut_ShoulderL_VectorsHome[_i];
                if (!_isMarkers)
                {
                    ShoulderCutsLNodes[_i].shoulderCutLWorld = _createdObj;
                }
                else
                {
                    ShoulderCutsLNodes[_i].shoulderCutLMarker = _createdObj;
                }

            }
            else
            {
                _createdObj.transform.position = cut_ShoulderR_VectorsHome[_i];
                if (!_isMarkers)
                {
                    ShoulderCutsRNodes[_i].shoulderCutRWorld = _createdObj;
                }
                else
                {
                    ShoulderCutsRNodes[_i].shoulderCutRMarker = _createdObj;
                }
            }

            MeshCollider MC = null;
            if (road.isUsingMeshColliders)
            {
                MC = _createdObj.AddComponent<MeshCollider>();
            }

            _mesh.uv = _uv;
            _mesh.tangents = _tangents;
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            MeshFilter MF = _createdObj.AddComponent<MeshFilter>();
            MF.sharedMesh = _mesh;

            if (road.isUsingMeshColliders)
            {
                if (MC.sharedMesh == null)
                {
                    MC.sharedMesh = MF.sharedMesh;
                }
            }
            int mCounter = 0;
            bool bHasMats = false;

            //Disable shadows for road cuts and markers:
            MeshRenderer MR = _createdObj.AddComponent<MeshRenderer>();
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (_isMarkers)
            {
                if (road.ShoulderMaterialMarker1 != null)
                {
                    mCounter += 1;
                    if (road.ShoulderMaterialMarker2 != null)
                    {
                        mCounter += 1;
                        if (road.ShoulderMaterialMarker3 != null)
                        {
                            mCounter += 1;
                            if (road.ShoulderMaterialMarker4 != null)
                            {
                                mCounter += 1;
                            }
                        }
                    }
                }
                if (mCounter > 0)
                {
                    Material[] tMats = new Material[mCounter];
                    if (road.ShoulderMaterialMarker1 != null)
                    {
                        tMats[0] = road.ShoulderMaterialMarker1;
                        if (road.ShoulderMaterialMarker2 != null)
                        {
                            tMats[1] = road.ShoulderMaterialMarker2;
                            if (road.ShoulderMaterialMarker3 != null)
                            {
                                tMats[2] = road.ShoulderMaterialMarker3;
                                if (road.ShoulderMaterialMarker4 != null)
                                {
                                    tMats[3] = road.ShoulderMaterialMarker4;
                                }
                            }
                        }
                    }

                    MR.materials = tMats;
                    bHasMats = true;
                }
            }
            else
            {
                if (road.ShoulderMaterial1 != null)
                {
                    mCounter += 1;
                    if (road.ShoulderMaterial2 != null)
                    {
                        mCounter += 1;
                        if (road.ShoulderMaterial3 != null)
                        {
                            mCounter += 1;
                            if (road.ShoulderMaterial4 != null)
                            {
                                mCounter += 1;
                            }
                        }
                    }
                }
                if (mCounter > 0)
                {
                    Material[] tMats = new Material[mCounter];
                    if (road.ShoulderMaterial1 != null)
                    {
                        tMats[0] = road.ShoulderMaterial1;
                        if (road.ShoulderMaterial2 != null)
                        {
                            tMats[1] = road.ShoulderMaterial2;
                            if (road.ShoulderMaterial3 != null)
                            {
                                tMats[2] = road.ShoulderMaterial3;
                                if (road.ShoulderMaterial4 != null)
                                {
                                    tMats[3] = road.ShoulderMaterial4;
                                }
                            }
                        }
                    }
                    MR.materials = tMats;
                    MR = null;
                    bHasMats = true;
                }
            }

            if (!_isMarkers && MC != null)
            {
                MC.sharedMaterial = road.ShoulderPhysicMaterial;
            }
            _createdObj.transform.parent = _masterObj.transform;
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(CreatedObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            if (road.isStatic)
            {
                _createdObj.isStatic = true;
            }

            MF = null;
            MC = null;

            return bHasMats;
        }
        #endregion


        private static void SaveMesh(SaveMeshTypeEnum _saveType, Mesh _mesh, Road _road, string _name)
        {
            if (!_road.GSDRS.isSavingMeshes)
            {
                return;
            }

            //string tSceneName = System.IO.Path.GetFileName(UnityEditor.EditorApplication.currentScene).ToLower().Replace(".unity","");
            string tSceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            tSceneName = tSceneName.Replace("/", "");
            tSceneName = tSceneName.Replace(".", "");
            string tFolderName = "";

            string basePath = RoadEditorUtility.GetBasePath();

            if (_saveType == SaveMeshTypeEnum.Road)
            {
                tFolderName = basePath + "/Mesh/Generated/Roads/";
            }
            else if (_saveType == SaveMeshTypeEnum.Shoulder)
            {
                tFolderName = basePath + "/Mesh/Generated/Shoulders/";
            }
            else if (_saveType == SaveMeshTypeEnum.Intersection)
            {
                tFolderName = basePath + "/Mesh/Generated/Intersections/";
            }
            else if (_saveType == SaveMeshTypeEnum.Railing)
            {
                tFolderName = basePath + "/Mesh/Generated/Railings/";
            }
            else if (_saveType == SaveMeshTypeEnum.Center)
            {
                tFolderName = basePath + "/Mesh/Generated/CenterDividers/";
            }
            else if (_saveType == SaveMeshTypeEnum.RoadCut)
            {
                tFolderName = basePath + "/Mesh/Generated/Roads/Cuts/";
            }
            else if (_saveType == SaveMeshTypeEnum.SCut)
            {
                tFolderName = basePath + "/Mesh/Generated/Shoulders/Cuts/";
            }
            else if (_saveType == SaveMeshTypeEnum.RoadConn)
            {
                tFolderName = basePath + "/Mesh/Generated/RoadConn/";
            }

            string xPath = Application.dataPath.Replace("/Assets", "/" + tFolderName);
            if (!System.IO.Directory.Exists(xPath))
            {
                System.IO.Directory.CreateDirectory(xPath);
            }

            string tRoadName = _road.transform.name;
            string FinalName = tFolderName + tSceneName + "-" + tRoadName + "-" + _name + ".asset";
            if (_saveType == SaveMeshTypeEnum.Intersection)
            {
                FinalName = tFolderName + tSceneName + "-" + _name + ".asset";
            }

            UnityEditor.AssetDatabase.CreateAsset(_mesh, FinalName);
        }
    }


    public static class GSDRoadUtil
    {
        private const string FileSepString = "\n!!!! MICROGSD !!!!\n";
        private const string FileSepStringCRLF = "\r\n!!!! MICROGSD !!!!\r\n";


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
                    if (!tTIDS.Contains(THMaker.TID))
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
                //Get TID:
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
                    if (_road.TerrainHistory[index].TID == TID.UID)
                    {
                        isContainingTID = true;
                        break;
                    }
                }

                if (!isContainingTID)
                {
                    TerrainHistoryMaker THMaker = new TerrainHistoryMaker();
                    THMaker.TID = TID.UID;
                    _road.TerrainHistory.Add(THMaker);
                }

                TH = null;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].TID == TID.UID)
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
                        TH.h = new float[TTD.cI];
                        System.Array.Copy(TTD.oldH, 0, TH.h, 0, TTD.cI);
                        TH.cI = TTD.cI;
                    }
                }
                else
                {
                    TH.x1 = null;
                    TH.y1 = null;
                    TH.h = null;
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

                    TH.DetailsX = new int[TotalSize];
                    TH.DetailsY = new int[TotalSize];
                    TH.DetailsOldValue = new int[TotalSize];

                    int RunningIndex = 0;
                    int cLength = 0;
                    for (int index = 0; index < TTD.DetailLayersCount; index++)
                    {
                        cLength = TTD.DetailsI[index];
                        if (cLength < 1)
                        {
                            continue;
                        }
                        System.Array.Copy(TTD.DetailsX[index].ToArray(), 0, TH.DetailsX, RunningIndex, cLength);
                        System.Array.Copy(TTD.DetailsY[index].ToArray(), 0, TH.DetailsY, RunningIndex, cLength);
                        System.Array.Copy(TTD.OldDetailsValue[index].ToArray(), 0, TH.DetailsOldValue, RunningIndex, cLength);
                        RunningIndex += TTD.DetailsI[index];
                    }

                    //                    TH.DetailsX = TTD.DetailsX;
                    //                    TH.DetailsY = TTD.DetailsY;
                    //                    TH.DetailsOldValue = TTD.OldDetailsValue;
                    TH.DetailsI = TTD.DetailsI;
                    TH.DetailLayersCount = TTD.DetailLayersCount;
                }
                else
                {
                    TH.DetailsX = null;
                    TH.DetailsY = null;
                    TH.DetailsOldValue = null;
                    TH.DetailsI = null;
                    TH.DetailLayersCount = 0;
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
                    TH.TreesOld = null;
                    TH.TreesI = 0;
                }
            }

            //			//TerrainHistoryRaw
            //			RootUtils.StartProfiling(tRoad, "TerrainHistorySerialize");
            //			TerrainHistorySerialize(ref tRoad);
            //			RootUtils.EndProfiling(tRoad);
        }


        //		private static void TerrainHistorySerialize(ref GSDRoad tRoad) {
        //			MemoryStream ms = new MemoryStream();
        //	        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //			formatter.Serialize(ms,tRoad.TerrainHistory);
        //			ms.Close();
        //			tRoad.TerrainHistoryRaw = ms.ToArray();
        //	        ms = null;
        //	    }
        //		
        //		private static void TerrainHistoryDeserialize(ref GSDRoad tRoad) {
        //			MemoryStream ms = new MemoryStream(tRoad.TerrainHistoryRaw);
        //	        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //			tRoad.TerrainHistory = (List<GSDTerrainHistoryMaker>)formatter.Deserialize(ms);
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


        public static GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo[] GetRoadTerrainInfos()
        {
            Object[] tTerrainsObj = GameObject.FindObjectsOfType(typeof(Terrain));
            GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo tInfo;
            List<GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo> tInfos = new List<GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo>();
            foreach (Terrain tTerrain in tTerrainsObj)
            {
                tInfo = new GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo();
                tInfo.uID = tTerrain.transform.gameObject.GetComponent<RoadTerrain>().UID;
                tInfo.bounds = new Rect(tTerrain.transform.position.x, tTerrain.transform.position.z, tTerrain.terrainData.size.x, tTerrain.terrainData.size.z);
                tInfo.hmWidth = tTerrain.terrainData.heightmapWidth;
                tInfo.hmHeight = tTerrain.terrainData.heightmapHeight;
                tInfo.pos = tTerrain.transform.position;
                tInfo.size = tTerrain.terrainData.size;
                tInfo.heights = tTerrain.terrainData.GetHeights(0, 0, tInfo.hmWidth, tInfo.hmHeight);
                tInfos.Add(tInfo);
            }
            GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo[] fInfos = new GSD.Threaded.GSDRoadCreationT.RoadTerrainInfo[tInfos.Count];
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


        public static void SaveNodeObjects(ref Splination.SplinatedMeshMaker[] _splinatedObjects, ref GSD.Roads.EdgeObjects.EdgeObjectMaker[] _edgeObjects, ref WizardObject _wizardObj)
        {
            int sCount = _splinatedObjects.Length;
            int eCount = _edgeObjects.Length;
            //Splinated objects first:
            Splination.SplinatedMeshMaker SMM = null;
            RootUtils.CheckCreateSpecialLibraryDirs();
            string xPath = RootUtils.GetDirLibrary();
            string tPath = xPath + "B/" + _wizardObj.fileName + ".gsd";
            if (_wizardObj.isDefault)
            {
                tPath = xPath + "B/W/" + _wizardObj.fileName + ".gsd";
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

            GSD.Roads.EdgeObjects.EdgeObjectMaker EOM = null;
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

            string tPath = "";
            RootUtils.CheckCreateSpecialLibraryDirs();
            string xPath = RootUtils.GetDirLibrary();
            if (_isDefault)
            {
                tPath = xPath + "B/W/" + _fileName + ".gsd";
            }
            else
            {
                tPath = xPath + "B/" + _fileName + ".gsd";
            }

            string tData = System.IO.File.ReadAllText(tPath);
            string[] tSep = new string[2];
            tSep[0] = FileSepString;
            tSep[1] = FileSepStringCRLF;
            string[] tSplit = tData.Split(tSep, System.StringSplitOptions.RemoveEmptyEntries);

            Splination.SplinatedMeshMaker SMM = null;
            Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = null;
            GSD.Roads.EdgeObjects.EdgeObjectMaker EOM = null;
            GSD.Roads.EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker ELM = null;
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
                ELM = GSD.Roads.EdgeObjects.EdgeObjectMaker.ELMFromData(tSplit[index]);
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