using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace RoadArchitect
{
    public class RoadConstructorBufferMaker
    {
        #region "Vars"
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

        /// <summary> Road UVs </summary>
        public Vector2[] uv;
        /// <summary> Pavement UVs </summary>
        public Vector2[] uv2;
        /// <summary> Shoulder right UVs </summary>
        public Vector2[] uv_SR;
        /// <summary> Shoulder left UVs </summary>
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

        public List<Construction2DRect> tIntersectionBounds;
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
        #endregion


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
            tIntersectionBounds = new List<Construction2DRect>();
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
        /// <summary>
        /// Creates meshes and assigns vertices, triangles and normals. If multithreading enabled, this occurs inbetween threaded jobs since unity library can't be used in threads.
        /// </summary>
        public void MeshSetup1()
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
        /// <summary> Cleanup and create intersection objects </summary>
        private void MeshSetup1IntersectionObjectsSetup()
        {
            int nodeCount = road.spline.GetNodeCount();
            List<RoadIntersection> roadIntersections = new List<RoadIntersection>();
            for (int index = 0; index < nodeCount; index++)
            {
                if (road.spline.nodes[index].isIntersection)
                {
                    if (!roadIntersections.Contains(road.spline.nodes[index].intersection))
                    {
                        roadIntersections.Add(road.spline.nodes[index].intersection);
                    }
                }
            }

            //Cleanups:
            foreach (RoadIntersection intersection in roadIntersections)
            {
                IntersectionObjects.CleanupIntersectionObjects(intersection.transform.gameObject);
                if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                {
                    IntersectionObjects.CreateStopSignsAllWay(intersection.transform.gameObject, true);
                }
                else if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1)
                {
                    IntersectionObjects.CreateTrafficLightBases(intersection.transform.gameObject, true);
                }
                else if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                {

                }
                else if (intersection.intersectionStopType == RoadIntersection.iStopTypeEnum.None)
                {
                    //Do nothing.
                }
            }
        }


        /// <summary> Creates intersection meshes if road contains intersections </summary>
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


        /// <summary> Assigns mesh values to _mesh and returns _mesh </summary>
        private Mesh MeshSetup1Helper(ref Mesh _mesh, Vector3[] _verts, ref int[] _tris, ref Vector3[] _normals)
        {
            _mesh.vertices = _verts;
            _mesh.triangles = _tris;
            _mesh.normals = _normals;
            _mesh.RecalculateNormals();
            _normals = _mesh.normals;
            //_mesh.hideFlags = HideFlags.DontSave;
            return _mesh;
        }
        #endregion


        #region "Mesh Setup2"
        /// <summary>
        /// Assigns UV and tangents to meshes. If multithreading enabled, this occurs after the last threaded job since unity library can't be used in threads.
        /// </summary>
        public void MeshSetup2()
        {
            Mesh MeshMainBuffer = null;
            Mesh MeshMarkerBuffer = null;

            if (isRoadOn)
            {
                // Materials batched and extracted from inner for loops of mesh creation
                Material[] markerMaterialsField = road.GetRoadMarkerMaterials();
                Material[] roadMaterialsField = road.GetRoadWorldMaterials();
                // shoulder fields only contain data when isShouldersEnabled
                Material[] shoulderMaterialsField = road.GetShoulderWorldMaterials();
                Material[] shoulderMarkerMaterialsField = road.GetShoulderMarkerMaterials();


                //If road cuts is off, full size UVs:
                if ((!road.isRoadCutsEnabled && !road.isDynamicCutsEnabled) || (RoadCuts == null || RoadCuts.Count <= 0))
                {
                    if (tMesh != null)
                    {
                        tMesh = MeshSetup2Helper(ref tMesh, uv, tangents, ref road.MeshRoad, shoulderMaterialsField, markerMaterialsField, roadMaterialsField, true);
                        SaveMesh(SaveMeshTypeEnum.Road, tMesh, road, road.MeshRoad.transform.name);

                        Vector3[] ooVerts = new Vector3[tMesh.vertexCount];
                        int[] ooTris = new int[tMesh.triangles.Length];
                        Vector3[] ooNormals = new Vector3[tMesh.normals.Length];
                        Vector2[] ooUV = new Vector2[uv2.Length];
                        Vector4[] ooTangents = new Vector4[tangents2.Length];


                        // Copy mesh values
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
                        pMesh = MeshSetup2Helper(ref pMesh, uv2, tangents2, ref gObj, shoulderMaterialsField, markerMaterialsField, roadMaterialsField, false);
                        //Road markers stored on parent "MeshRoad" game object, with a "Pavement" child game object storing the asphalt.
                        gObj.transform.parent = road.MeshRoad.transform;
                        SaveMesh(SaveMeshTypeEnum.Road, pMesh, road, gObj.transform.name);
                    }
                }
                else
                {
                    //If road cuts, change it to one material (pavement) with world mapping
                    int cCount = cut_RoadVectors.Count;
                    //Vector2[] tUV;
                    GameObject CreatedMainObj;
                    GameObject CreatedMarkerObj;
                    for (int i = 0; i < cCount; i++)
                    {
                        CreatedMainObj = null;
                        MeshMainBuffer = tMesh_RoadCuts_world[i];
                        if (MeshMainBuffer != null)
                        {
                            MeshSetup2HelperRoadCuts(i, ref MeshMainBuffer, cut_uv_world[i], cut_tangents_world[i], ref road.MeshRoad, false, out CreatedMainObj, markerMaterialsField, roadMaterialsField);
                            SaveMesh(SaveMeshTypeEnum.RoadCut, MeshMainBuffer, road, "RoadCut" + i.ToString());
                        }

                        CreatedMarkerObj = null;
                        MeshMarkerBuffer = tMesh_RoadCuts[i];
                        bool bHasMats = false;
                        if (MeshMarkerBuffer != null)
                        {
                            bHasMats = MeshSetup2HelperRoadCuts(i, ref MeshMarkerBuffer, cut_uv[i], cut_tangents[i], ref CreatedMainObj, true, out CreatedMarkerObj, markerMaterialsField, roadMaterialsField);
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
                            tMesh_SR = MeshSetup2Helper(ref tMesh_SR, uv_SR, tangents_SR, ref road.MeshShoR, shoulderMaterialsField, markerMaterialsField, roadMaterialsField, false, true);
                            SaveMesh(SaveMeshTypeEnum.Shoulder, tMesh_SR, road, road.MeshShoR.transform.name);
                        }

                        //Left road shoulder:
                        if (tMesh_SL != null)
                        {
                            tMesh_SL = MeshSetup2Helper(ref tMesh_SL, uv_SL, tangents_SL, ref road.MeshShoL, shoulderMaterialsField, markerMaterialsField, roadMaterialsField, false, true);
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
                                MeshSetup2HelperCutsShoulder(index, ref MeshMainBuffer, cut_uv_SR_world[index], cut_tangents_SR_world[index], ref road.MeshShoR, false, false, out CreatedMainObj, shoulderMarkerMaterialsField, shoulderMaterialsField);
                                SaveMesh(SaveMeshTypeEnum.SCut, MeshMainBuffer, road, "SCutR" + index.ToString());
                            }

                            CreatedMarkerObj = null;
                            MeshMarkerBuffer = tMesh_SRCuts[index];
                            if (MeshMarkerBuffer != null)
                            {
                                bHasMats = MeshSetup2HelperCutsShoulder(index, ref MeshMarkerBuffer, cut_uv_SR[index], cut_tangents_SR[index], ref CreatedMainObj, false, true, out CreatedMarkerObj, shoulderMarkerMaterialsField, shoulderMaterialsField);
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
                                MeshSetup2HelperCutsShoulder(index, ref MeshMainBuffer, cut_uv_SL_world[index], cut_tangents_SL_world[index], ref road.MeshShoL, true, false, out CreatedMainObj, shoulderMarkerMaterialsField, shoulderMaterialsField);
                                SaveMesh(SaveMeshTypeEnum.SCut, MeshMainBuffer, road, "SCutL" + index.ToString());
                            }


                            CreatedMarkerObj = null;
                            MeshMarkerBuffer = tMesh_SLCuts[index];
                            if (MeshMarkerBuffer != null)
                            {
                                bHasMats = MeshSetup2HelperCutsShoulder(index, ref MeshMarkerBuffer, cut_uv_SL[index], cut_tangents_SL[index], ref CreatedMainObj, true, true, out CreatedMarkerObj, shoulderMarkerMaterialsField, shoulderMaterialsField);
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
                            //MeshSetup2IntersectionsFixNormals();	
                        }


                        RemoveMainMeshes();
                    }
                }
                else
                {
                    RemoveMainMeshes();


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
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/RoadConn-4L.mat", MR);
                        }
                        else if (fDist == Mathf.Round(road.RoadWidth() * 3f))
                        {
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/RoadConn-6L-2L.mat", MR);
                        }
                    }
                    else if (road.laneAmount == 4)
                    {
                        if (fDist == Mathf.Round(road.RoadWidth() * 1.5f))
                        {
                            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Markers/RoadConn-6L-4L.mat", MR);
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
                    RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Road1.mat", MR);
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

            //Updates the road and shoulder cut materials if necessary.
            //Note: Cycling through all nodes in case the road cuts and shoulder cut numbers don't match.
            if (road.isRoadCutsEnabled || road.isShoulderCutsEnabled || road.isDynamicCutsEnabled)
            {
                int mCount = road.spline.GetNodeCount();
                for (int index = 0; index < mCount; index++)
                {
                    road.spline.nodes[index].UpdateCuts();
                }
            }
        }


        private void RemoveMainMeshes()
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
        }


        #region "MeshSetup2 - Intersections"
        //private void MeshSetup2IntersectionsFixNormals()
        //{
        //  int mCount = tRoad.spline.GetNodeCount();
        //	SplineN tNode = null;
        //	RoadIntersection roadIntersection = null;
        //	float MaxDist = 0f;
        //	float[] tDists = new float[2];
        //	Collider[] tColliders = null;
        //	List<GameObject> tCuts = null;
        //
        //	for(int h=0; h<mCount; h++)
        //  {
        //		tNode=tRoad.spline.mNodes[h];
        //		if(tNode.bIsIntersection)
        //      {
        //			roadIntersection = tNode.roadIntersection;
        //
        //			tColliders = Physics.OverlapSphere(roadIntersection.CornerRR_Outer,tRoad.opt_ShoulderWidth*1.25f);
        //			tCuts = new List<GameObject>();
        //			foreach(Collider tCollider in tColliders)
        //          {
        //				if(tCollider.transform.name.Contains("cut"))
        //              {
        //					tCuts.Add(tCollider.transform.gameObject);
        //				}
        //			}
        //					
        //							
        //			foreach(GameObject tObj in tCuts)
        //          {
        //				MeshFilter MF1 = tCuts[0].GetComponent<MeshFilter>();
        //				if(MF1 == null)
        //              {
        //                  continue;
        //              }
        //				Mesh zMesh1 = MF1.sharedMesh;
        //				Vector3[] tVerts1 = zMesh1.vertices;
        //				Vector3[] tNormals1 = zMesh1.normals;
        //				int MVL1 = tVerts1.Length;
        //				for(int i=0; i<MVL1; i++)
        //              {
        //					if(tVerts1[i] == roadIntersection.CornerRR)
        //                  {
        //						tNormals1[i] = Vector3.up;
        //					}
        //                  else if(tVerts1[i] == roadIntersection.CornerRR_Outer)
        //                  {
        //						tNormals1[i] = Vector3.up;
        //					}
        //				}
        //			}			
        //		}
        //	}
        //}


        /// <summary> Creates main roads of the intersections if road contains intersections </summary>
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
            HashSet<RoadIntersection> uniqueRoadIntersection = new HashSet<RoadIntersection>();

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
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes0, "Lane0B", basePath + "/Materials/Markers/InterWhiteLYellowR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneD1B", basePath + "/Materials/Markers/InterLaneDisabled.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneDA1B", basePath + "/Materials/Markers/InterLaneDisabledOuter.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "Lane1B", basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2B", basePath + "/Materials/Markers/InterLaneDisabledOuter.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2B", basePath + "/Materials/Markers/InterLaneDisabledOuterR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneD2B", basePath + "/Materials/Markers/InterLaneDisabled.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "Lane2B", basePath + "/Materials/Markers/InterWhiteR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "LaneD3B", basePath + "/Materials/Markers/InterLaneDisabled.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "Lane3B", basePath + "/Materials/Markers/InterWhiteR.mat");
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
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes0, "Lane0F", basePath + "/Materials/Markers/InterWhiteLYellowR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneD1F", basePath + "/Materials/Markers/InterLaneDisabled.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "LaneDAR1F", basePath + "/Materials/Markers/InterLaneDisabledOuterR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes1, "Lane1F", basePath + "/Materials/Markers/InterYellowLWhiteR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDA2F", basePath + "/Materials/Markers/InterLaneDisabledOuter.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneDAR2F", basePath + "/Materials/Markers/InterLaneDisabledOuterR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "LaneD2F", basePath + "/Materials/Markers/InterLaneDisabled.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes2, "Lane2F", basePath + "/Materials/Markers/InterWhiteR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "LaneD3F", basePath + "/Materials/Markers/InterWhiteR.mat");
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
                    MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiLanes3, "Lane3F", basePath + "/Materials/Markers/InterWhiteR.mat");
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
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateB", basePath + "/Materials/Road1.mat", false);
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
                MF = MeshSetup2IntersectionHelper(ref fMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateBM", basePath + "/Materials/InterMainPlate1.mat");
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
                MF = MeshSetup2IntersectionHelper(ref xMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateFM", basePath + "/Materials/Road1.mat", false);

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
                MF = MeshSetup2IntersectionHelper(ref tMesh, ref tUV, ref tTangents, ref road.MeshiMainPlates, "MainPlateFM", basePath + "/Materials/InterMainPlate1.mat");
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


                    #if UNITY_EDITOR
                    if (road.isLightmapped)
                    {
                        //UnityEditor.Unwrapping.GenerateSecondaryUVSet(vMesh);
                    }
                    #endif


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


                    #if UNITY_EDITOR
                    if (road.isLightmapped)
                    {
                        #if UNITY_2019_2_OR_NEWER
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tCenter, UnityEditor.StaticEditorFlags.ContributeGI);
                        #else
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tCenter, UnityEditor.StaticEditorFlags.LightmapStatic);
                        #endif
                    }
                    #endif


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
                    //if(tNode.roadIntersection.MarkerCenter != null)
                    //{
                    ////	MR.material = tNode.roadIntersection.MarkerCenter;
                    //}
                    tMarker.transform.parent = tNode.intersection.transform;


                    #if UNITY_EDITOR
                    if (road.isLightmapped)
                    {
                        #if UNITY_2019_2_OR_NEWER
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tMarker, UnityEditor.StaticEditorFlags.ContributeGI);
                        #else
                        UnityEditor.GameObjectUtility.SetStaticEditorFlags(tMarker, UnityEditor.StaticEditorFlags.LightmapStatic);
                        #endif
                    }
                    #endif


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


                    #if UNITY_EDITOR
                    if (road.isLightmapped)
                    {
                        //UnityEditor.Unwrapping.GenerateSecondaryUVSet(mMesh);
                    }
                    #endif


                    SaveMesh(SaveMeshTypeEnum.Intersection, MF.sharedMesh, road, tNode.intersection.transform.name + "-" + "CenterMarkers");
                }
            }

            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane0)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane0");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane1");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane3)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "Lane3");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_MainPlate)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "TiledExt", true);
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_MainPlateM)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "StretchExt");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1_Disabled)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD1");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane3_Disabled)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD3");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_DisabledActive)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDA2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_DisabledActiveR)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDAR2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane2_Disabled)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneD2");
            }
            foreach (KeyValuePair<RoadIntersection, List<MeshFilter>> KVP in tCombineDict_Lane1_DisabledActive)
            {
                if (!uniqueRoadIntersection.Contains(KVP.Key))
                {
                    uniqueRoadIntersection.Add(KVP.Key);
                }
                MeshSetup2CombineIntersections(KVP, KVP.Key.transform.name + "-" + "LaneDA1");
            }

            foreach (RoadIntersection roadIntersection in uniqueRoadIntersection)
            {
                roadIntersection.UpdateMaterials();
            }
        }


        /// <summary> Combines all intersections in _valuePair into an single mesh </summary>
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
            for (int index = 0; index < tVerts.Length; index++)
            {
                tVerts[index] += tVect;
                if (_name.ToLower().EndsWith("-stretchext"))
                {
                    tVerts[index] += new Vector3(0f, 0.01f); 
                }
            }
            MF.sharedMesh.vertices = tVerts;
            tObj.transform.localPosition = new Vector3(0f, 0f, 0f);
            MF.sharedMesh.RecalculateBounds();
            MF.sharedMesh.RecalculateNormals();
            MF.sharedMesh.tangents = RootUtils.ProcessTangents(MF.sharedMesh.triangles, MF.sharedMesh.normals, MF.sharedMesh.uv, MF.sharedMesh.vertices);


            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(MF.sharedMesh);

                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            #endif


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


        /// <summary> Assigns values to _mesh and returns the MeshFilter of a new GO </summary>
        private MeshFilter MeshSetup2IntersectionHelper(ref Mesh _mesh, ref Vector2[] _uv, ref Vector4[] _tangents, ref GameObject _masterObj, string _name, string _mat, bool _isCollider = false)
        {
            if (_mesh == null)
            {
                return null;
            }
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            #endif

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

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(tObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            #endif


            if (road.isStatic)
            {
                tObj.isStatic = true;
            }

            return MF;
        }
        #endregion


        /// <summary> Assigns values to _mesh </summary>
        private Mesh MeshSetup2Helper(ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _obj, Material[] _shoulderMaterials, Material[] _markerMaterials, Material[] _roadMaterials, bool _isMarker, bool _isShoulder = false, bool _isBridge = false)
        {
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            #endif

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
                if (_shoulderMaterials.Length > 0)
                {
                    MR.materials = _shoulderMaterials;
                }
            }
            else
            {
                if (_isMarker)
                {
                    // It should not be necessary to check this, but I let it until I'm sure this does not break anything.
                    if (_markerMaterials.Length > 0)
                    {
                        MR.materials = _markerMaterials;
                    }
                }
                else
                {
                    if (_roadMaterials.Length > 0)
                    {
                        MR.materials = _roadMaterials;
                    }

                    if (MC)
                    {
                        MC.sharedMaterial = road.RoadPhysicMaterial;
                    }
                }
            }

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_obj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_obj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            #endif

            if (road.isStatic)
            {
                _obj.isStatic = true;
            }

            return _mesh;
        }


        /// <summary> Creates a GameObject and adds mesh collider / renderer and configures mesh to be static etc. </summary>
        private bool MeshSetup2HelperRoadCuts(int _i, ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _masterObj, bool _isMarkers, out GameObject _createdObj, Material[] _markerMaterials, Material[] _roadMaterials)
        {
            if (!_isMarkers)
            {
                _createdObj = new GameObject("RoadCut" + _i.ToString());
                RoadCutNodes[_i].roadCutWorld = _createdObj;
            }
            else
            {
                _createdObj = new GameObject("Markers" + _i.ToString());
                RoadCutNodes[_i].roadCutMarker = _createdObj;
            }

            _createdObj.transform.position = cut_RoadVectorsHome[_i];
            _mesh.uv = _uv;
            _mesh.tangents = _tangents;

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            #endif

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
            bool isUsingMaterials = false;

            //Apply Materials:
            if (_isMarkers)
            {
                if (_markerMaterials.Length > 0)
                {
                    MR.materials = _markerMaterials;
                    isUsingMaterials = true;
                }
            }
            else
            {
                if (_roadMaterials.Length > 0)
                {
                    MR.materials = _roadMaterials;
                    isUsingMaterials = true;
                }
            }


            _createdObj.transform.parent = _masterObj.transform;
            if (!_isMarkers && MC != null)
            {
                MC.sharedMaterial = road.RoadPhysicMaterial;
            }

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            #endif

            if (road.isStatic)
            {
                _createdObj.isStatic = true;
            }

            return isUsingMaterials;
        }


        /// <summary> Inserts new GO into according shoulder collection at _i; Assigns values to _mesh </summary>
        private bool MeshSetup2HelperCutsShoulder(int _i, ref Mesh _mesh, Vector2[] _uv, Vector4[] _tangents, ref GameObject _masterObj, bool _isLeft, bool _isMarkers, out GameObject _createdObj, Material[] _shoulderMarkerMaterials, Material[] _shoulderMaterials)
        {
            if (_isMarkers)
            {
                _createdObj = new GameObject("Markers" + _i.ToString());

                if (_isLeft)
                {
                    _createdObj.transform.position = cut_ShoulderL_VectorsHome[_i];
                    ShoulderCutsLNodes[_i].shoulderCutLMarker = _createdObj;
                }
                else
                {
                    _createdObj.transform.position = cut_ShoulderR_VectorsHome[_i];
                    ShoulderCutsRNodes[_i].shoulderCutRMarker = _createdObj;
                }
            }
            else
            {
                if (_isLeft)
                {
                    _createdObj = new GameObject("SCutL" + _i.ToString());
                    _createdObj.transform.position = cut_ShoulderL_VectorsHome[_i];
                    ShoulderCutsLNodes[_i].shoulderCutLWorld = _createdObj;
                }
                else
                {
                    _createdObj = new GameObject("SCutR" + _i.ToString());
                    _createdObj.transform.position = cut_ShoulderR_VectorsHome[_i];
                    ShoulderCutsRNodes[_i].shoulderCutRWorld = _createdObj;
                }
            }


            MeshCollider MC = null;
            if (road.isUsingMeshColliders)
            {
                MC = _createdObj.AddComponent<MeshCollider>();
            }

            _mesh.uv = _uv;
            _mesh.tangents = _tangents;

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                UnityEditor.Unwrapping.GenerateSecondaryUVSet(_mesh);
            }
            #endif

            MeshFilter MF = _createdObj.AddComponent<MeshFilter>();
            MF.sharedMesh = _mesh;

            if (road.isUsingMeshColliders)
            {
                if (MC.sharedMesh == null)
                {
                    MC.sharedMesh = MF.sharedMesh;
                }
            }
            bool bHasMats = false;

            //Disable shadows for road cuts and markers:
            MeshRenderer MR = _createdObj.AddComponent<MeshRenderer>();
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (_isMarkers)
            {
                if (_shoulderMarkerMaterials.Length > 0)
                {
                    MR.materials = _shoulderMarkerMaterials;
                    bHasMats = true;
                }
            }
            else
            {
                if (_shoulderMaterials.Length > 0)
                {
                    MR.materials = _shoulderMaterials;
                    MR = null;
                    bHasMats = true;
                }
            }

            if (!_isMarkers && MC != null)
            {
                MC.sharedMaterial = road.ShoulderPhysicMaterial;
            }
            _createdObj.transform.parent = _masterObj.transform;

            #if UNITY_EDITOR
            if (road.isLightmapped)
            {
                #if UNITY_2019_2_OR_NEWER
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.ContributeGI);
                #else
                UnityEditor.GameObjectUtility.SetStaticEditorFlags(_createdObj, UnityEditor.StaticEditorFlags.LightmapStatic);
                #endif
            }
            #endif

            if (road.isStatic)
            {
                _createdObj.isStatic = true;
            }

            MF = null;
            MC = null;

            return bHasMats;
        }
        #endregion


        /// <summary> Saves Mesh as an asset </summary>
        private static void SaveMesh(SaveMeshTypeEnum _saveType, Mesh _mesh, Road _road, string _name)
        {
            if (!_road.roadSystem.isSavingMeshes)
            {
                return;
            }


            string sceneName;
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");

            string folderName = Path.Combine(RoadEditorUtility.GetBasePath(), "Mesh");
            folderName = Path.Combine(folderName, "Generated");

            if (_saveType == SaveMeshTypeEnum.Road)
            {
                folderName = Path.Combine(folderName, "Roads");
            }
            else if (_saveType == SaveMeshTypeEnum.Shoulder)
            {
                folderName = Path.Combine(folderName, "Shoulders");
            }
            else if (_saveType == SaveMeshTypeEnum.Intersection)
            {
                folderName = Path.Combine(folderName, "Intersections");
            }
            else if (_saveType == SaveMeshTypeEnum.Railing)
            {
                folderName = Path.Combine(folderName, "Railings");
            }
            else if (_saveType == SaveMeshTypeEnum.Center)
            {
                folderName = Path.Combine(folderName, "CenterDividers");
            }
            else if (_saveType == SaveMeshTypeEnum.RoadCut)
            {
                folderName = Path.Combine(Path.Combine(folderName, "Roads"), "Cuts");
            }
            else if (_saveType == SaveMeshTypeEnum.SCut)
            {
                folderName = Path.Combine(Path.Combine(folderName, "Shoulders"), "Cuts");
            }
            else if (_saveType == SaveMeshTypeEnum.RoadConn)
            {
                folderName = Path.Combine(folderName, "RoadConn");
            }

            string path = Path.GetDirectoryName(Application.dataPath);
            path = Path.Combine(path, folderName);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            string roadName = _road.transform.name;
            string finalName = Path.Combine(folderName, sceneName + "-" + roadName + "-" + _name + ".asset");
            if (_saveType == SaveMeshTypeEnum.Intersection)
            {
                finalName = Path.Combine(folderName, sceneName + "-" + _name + ".asset");
            }

            #if UNITY_EDITOR
            // Unity works with forward slash so we convert
            // If you want to implement your own Asset creation and saving you should just use finalName
            finalName = finalName.Replace(Path.DirectorySeparatorChar, '/');
            finalName = finalName.Replace(Path.AltDirectorySeparatorChar, '/');
            UnityEditor.AssetDatabase.CreateAsset(_mesh, finalName);
            #endif
        }
    }
}
