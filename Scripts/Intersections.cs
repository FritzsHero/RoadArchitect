using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    public static class Intersections
    {
        public static GameObject CreateIntersection(SplineN _node1, SplineN _node2)
        {
            float RoadMod = 10f;
            GameObject SystemObj = _node1.transform.parent.parent.parent.gameObject;
            if (!SystemObj)
            {
                Debug.LogWarning("Could not find road system master object.");
                return null;
            }
            GameObject InterMaster = null;
            int cCount = SystemObj.transform.childCount;
            for (int i = 0; i < cCount; i++)
            {
                if (SystemObj.transform.GetChild(i).transform.name.ToLower() == "intersections")
                {
                    InterMaster = SystemObj.transform.GetChild(i).gameObject;
                }
            }
            if (!InterMaster)
            {
                InterMaster = new GameObject("Intersections");
                InterMaster.transform.parent = SystemObj.transform;
            }
            if (!InterMaster)
            {
                Debug.LogWarning("Could not find intersections master object for this road system.");
                return null;
            }
            cCount = InterMaster.transform.childCount;

            GameObject intersectionObject = new GameObject("Inter" + (cCount + 1).ToString());
            intersectionObject.transform.parent = InterMaster.transform;
            RoadIntersection roadIntersection = intersectionObject.AddComponent<RoadIntersection>();
            roadIntersection.ignoreSide = -1;
            roadIntersection.isFirstSpecialFirst = false;
            roadIntersection.isFirstSpecialLast = false;
            roadIntersection.isSecondSpecialFirst = false;
            roadIntersection.isSecondSpecialLast = false;

            SplineN tNode1 = null;
            SplineN tNode2 = null;


            // same spline
            if (_node1.spline == _node2.spline)
            {
                if (_node1.idOnSpline < _node2.idOnSpline)
                {
                    tNode1 = _node1;
                    tNode2 = _node2;
                }
                else
                {
                    tNode1 = _node2;
                    tNode2 = _node1;
                }
            }
            else
            {
                // different spline
                tNode1 = _node1;
                tNode2 = _node2;
            }



            //If 3way, always add the single node as primary:
            if (_node1.isEndPoint)
            {
                tNode1 = _node1;
                tNode2 = _node2;
            }
            else if (_node2.isEndPoint)
            {
                tNode1 = _node2;
                tNode2 = _node1;
            }

            tNode1.intersectionOtherNode = tNode2;
            tNode2.intersectionOtherNode = tNode1;

            if (tNode1.isEndPoint || tNode2.isEndPoint)
            {
                roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
            }

            SplineN zNode = null;
            if (tNode1.isEndPoint)
            {
                bool isFirstNode = false;
                bool isAlreadyNode = false;
                if (tNode1.idOnSpline == 1 || tNode1.idOnSpline == 0)
                {
                    isFirstNode = true;
                }
                if (isFirstNode && tNode1.idOnSpline == 1 && tNode1.spline.nodes[0].isSpecialEndNodeIsStart)
                {
                    isAlreadyNode = true;
                }
                else if (!isFirstNode && tNode1.idOnSpline == tNode1.spline.GetNodeCount() - 2 && tNode1.spline.nodes[tNode1.spline.GetNodeCount() - 1].isSpecialEndNodeIsEnd)
                {
                    isAlreadyNode = true;
                }

                Vector3 tPos = default(Vector3);
                if (isFirstNode)
                {
                    tPos = ((tNode1.tangent * -1f).normalized * (tNode1.spline.road.RoadWidth() * RoadMod)) + tNode1.pos;
                }
                else
                {
                    tPos = (tNode1.spline.GetSplineValue(0.999f, true).normalized * (tNode1.spline.road.RoadWidth() * RoadMod)) + tNode1.pos;
                }

                if (!isAlreadyNode)
                {
                    if (isFirstNode)
                    {
                        zNode = Construction.InsertNode(tNode1.spline.road, true, tPos, false, 0, true, true);
                        zNode.isSpecialEndNodeIsStart = true;
                        zNode.isSpecialIntersection = true;
                        zNode.tangent = tNode1.tangent;
                    }
                    else
                    {
                        zNode = Construction.CreateNode(tNode1.spline.road, true, tPos, true);
                        zNode.isSpecialEndNodeIsEnd = true;
                        zNode.isSpecialIntersection = true;
                        zNode.tangent = tNode1.tangent;
                    }
                }
                else
                {
                    if (isFirstNode)
                    {
                        tNode1.spline.nodes[0].transform.position = tPos;
                    }
                    else
                    {
                        tNode1.spline.nodes[tNode1.spline.GetNodeCount() - 1].transform.position = tPos;
                    }
                }
                if (isFirstNode)
                {
                    tNode1.spline.isSpecialStartControlNode = true;
                    roadIntersection.isFirstSpecialFirst = true;
                }
                else
                {
                    tNode1.spline.isSpecialEndControlNode = true;
                    roadIntersection.isFirstSpecialLast = true;
                }

            }
            else if (tNode2.isEndPoint)
            {
                bool isFirstNode = false;
                bool isAlreadyNode = false;
                if (tNode2.idOnSpline == 1 || tNode2.idOnSpline == 0)
                {
                    isFirstNode = true;
                }
                if (isFirstNode && tNode2.idOnSpline == 1 && tNode2.spline.nodes[0].isSpecialEndNodeIsStart)
                {
                    isAlreadyNode = true;
                }
                else if (!isFirstNode && tNode2.idOnSpline == tNode2.spline.GetNodeCount() - 2 && tNode2.spline.nodes[tNode2.spline.GetNodeCount() - 1].isSpecialEndNodeIsEnd)
                {
                    isAlreadyNode = true;
                }

                Vector3 tPos = default(Vector3);
                if (isFirstNode)
                {
                    tPos = ((tNode2.tangent * -1f).normalized * (tNode2.spline.road.RoadWidth() * RoadMod)) + tNode2.pos;
                }
                else
                {
                    tPos = (tNode2.spline.GetSplineValue(0.999f, true).normalized * (tNode2.spline.road.RoadWidth() * RoadMod)) + tNode2.pos;
                }

                if (!isAlreadyNode)
                {
                    if (isFirstNode)
                    {
                        zNode = Construction.InsertNode(tNode2.spline.road, true, tPos, false, 0, true, true);
                        zNode.isSpecialEndNodeIsStart = true;
                        zNode.isSpecialIntersection = true;
                        zNode.tangent = tNode2.tangent;
                    }
                    else
                    {
                        zNode = Construction.CreateNode(tNode2.spline.road, true, tPos, true);
                        zNode.isSpecialEndNodeIsEnd = true;
                        zNode.isSpecialIntersection = true;
                        zNode.tangent = tNode2.tangent;
                    }
                }
                else
                {
                    if (isFirstNode)
                    {
                        tNode2.spline.nodes[0].transform.position = tPos;
                    }
                    else
                    {
                        tNode2.spline.nodes[tNode2.spline.GetNodeCount() - 1].transform.position = tPos;
                    }
                }
                if (isFirstNode)
                {
                    tNode2.spline.isSpecialStartControlNode = true;
                    roadIntersection.isSecondSpecialFirst = true;
                }
                else
                {
                    tNode2.spline.isSpecialEndControlNode = true;
                    roadIntersection.isSecondSpecialLast = true;
                }
            }

            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(intersectionObject, "Created intersection");
            #endif

            roadIntersection.Setup(tNode1, tNode2);
            intersectionObject.transform.position = _node1.transform.position;

            roadIntersection.ResetMaterialsAll();

            //if(roadIntersection.isSameSpline)
            //{
            //	roadIntersection.node1.spline.road.UpdateRoad();
            //}
            //else
            //{
            //  roadIntersection.node1.spline.road.UpdateRoad();
            //	roadIntersection.node2.spline.road.UpdateRoad();
            //}

            tNode1.ToggleHideFlags(true);
            tNode2.ToggleHideFlags(true);

            if (roadIntersection != null && roadIntersection.node1 != null && roadIntersection.node2 != null)
            {
                if (!roadIntersection.isSameSpline)
                {
                    roadIntersection.node1.spline.road.PiggyBacks = new SplineC[4];
                    roadIntersection.node1.spline.road.PiggyBacks[0] = roadIntersection.node2.spline;

                    roadIntersection.node1.spline.road.PiggyBacks[1] = roadIntersection.node1.spline;
                    roadIntersection.node1.spline.road.PiggyBacks[2] = roadIntersection.node2.spline;
                    roadIntersection.node1.spline.road.PiggyBacks[3] = roadIntersection.node1.spline;
                    //roadIntersection.node1.spline.road.PiggyBacks[4] = roadIntersection.node2.spline;
                }
                roadIntersection.node1.spline.road.isUpdateRequired = true;
            }

            return intersectionObject;
        }


        public static Vector3[] GetCornerVectorsTest(RoadIntersection _roadIntersection)
        {
            Vector3[] tVects = new Vector3[4];
            SplineN tNode;
            tNode = _roadIntersection.node1;
            SplineC tSpline = tNode.spline;

            //RR = Node1 - 5, Node2 + 5
            //RL = Node1 + 5, Node2 + 5
            //LL = Node1 + 5, Node2 - 5
            //LR = Node1 - 5, Node2 - 5

            float tOffset = 5f;
            float tPos1 = tNode.time - (tOffset / tSpline.distance);
            float tPos2 = tNode.time + (tOffset / tSpline.distance);
            Vector3 tVect1 = tSpline.GetSplineValue(tPos1);
            Vector3 POS1 = tSpline.GetSplineValue(tPos1, true);
            Vector3 tVect2 = tSpline.GetSplineValue(tPos2);
            Vector3 POS2 = tSpline.GetSplineValue(tPos2, true);

            tVects[0] = (tVect1 + new Vector3(5f * POS1.normalized.z, 0, 5f * -POS1.normalized.x));
            tVects[1] = (tVect1 + new Vector3(5f * -POS1.normalized.z, 0, 5f * POS1.normalized.x));
            tVects[2] = (tVect2 + new Vector3(5f * POS2.normalized.z, 0, 5f * -POS2.normalized.x));
            tVects[3] = (tVect2 + new Vector3(5f * -POS2.normalized.z, 0, 5f * POS2.normalized.x));

            return tVects;
        }


        #region "Old intersection"
        public static void CreateIntersection(RoadIntersection _roadIntersection)
        {
            //1. Overlap sphere to find all road objects within intersection:
            float intersectionWidth = _roadIntersection.intersectionWidth * 1.25f;
            Collider[] colliders = Physics.OverlapSphere(_roadIntersection.transform.position, intersectionWidth);
            if (colliders == null || colliders.Length < 1)
            {
                return;
            }
            List<Road> roads = new List<Road>();
            foreach (Collider collider in colliders)
            {
                if (collider.transform.parent)
                {
                    Road tRoad = collider.transform.parent.GetComponent<Road>();
                    if (tRoad)
                    {
                        if (!roads.Contains(tRoad))
                        {
                            roads.Add(tRoad);
                        }
                    }
                }
            }

            //Flatten intersection area:
            float height = -1f;
            FlattenIntersectionArea(ref roads, _roadIntersection, intersectionWidth, out height);

            //Create main intersection mesh:
            string name = _roadIntersection.transform.name;
            Vector3[] vects;
            CreateIntersectionMeshMain(_roadIntersection, height, out vects, ref name);

            //Now create the 4 text overlays:
            CreateIntersectionMeshOuter(_roadIntersection, vects, ref name);

            //Update connected nodes:
            Navigation.UpdateConnectedNodes();

            //Now initialize intersection objects:
            InitializeIntersectionObjects(_roadIntersection);
        }


        private static void FlattenIntersectionArea(ref List<Road> _roads, RoadIntersection _roadIntersection, float _width, out float _height)
        {
            //Cycle through each road and get all mesh vertices that are within range:
            Vector3 tInterPos = _roadIntersection.transform.position;
            float tInterPosY = tInterPos.y;
            foreach (Road tRoad in _roads)
            {
                MeshFilter MF_Road = tRoad.MeshRoad.GetComponent<MeshFilter>();
                MeshFilter MF_Road_SR = tRoad.MeshShoR.GetComponent<MeshFilter>();
                MeshFilter MF_Road_SL = tRoad.MeshShoL.GetComponent<MeshFilter>();

                Mesh Road = MF_Road.sharedMesh;
                Mesh Road_SR = MF_Road_SR.sharedMesh;
                Mesh Road_SL = MF_Road_SL.sharedMesh;

                Vector3[] tVects = Road.vertices;
                Vector3[] tVects_SR = Road_SR.vertices;
                Vector3[] tVects_SL = Road_SL.vertices;
                int VertCount = Road.vertices.Length;
                bool bLeftToggle = true;
                for (int i = 0; i < VertCount; i += 2)
                {
                    if (Vector3.Distance(tVects[i], tInterPos) < _width)
                    {
                        tVects[i].y = tInterPosY;
                        tVects[i + 1].y = tInterPosY;
                        if (bLeftToggle)
                        {
                            //Left:
                            tVects_SL[i + 2].y = tInterPosY;
                            tVects_SL[i + 3].y = tInterPosY;
                        }
                        else
                        {
                            //Right:
                            tVects_SR[i - 2].y = tInterPosY;
                            tVects_SR[i - 1].y = tInterPosY;
                        }
                    }
                    bLeftToggle = !bLeftToggle;
                }
                //Main road:
                Road.vertices = tVects;
                Road.RecalculateNormals();
                MF_Road.sharedMesh = Road;
                //Right shoulder:
                Road_SR.vertices = tVects_SR;
                Road_SR.RecalculateNormals();
                MF_Road_SR.sharedMesh = Road_SR;
                //Left shoulder:
                Road_SL.vertices = tVects_SL;
                Road_SL.RecalculateNormals();
                MF_Road_SL.sharedMesh = Road_SL;
            }
            _height = tInterPosY;
        }


        private static bool V3Equal(Vector3 _a, Vector3 _b)
        {
            return Vector3.SqrMagnitude(_a - _b) < 0.0001f;
        }


        private static Vector3[] GetCornerVectors(RoadIntersection _roadIntersection, bool _isPrimary = true)
        {
            Vector3[] tVects = new Vector3[4];
            SplineN tNode;
            if (_isPrimary)
            {
                tNode = _roadIntersection.node1;
            }
            else
            {
                tNode = _roadIntersection.node2;
            }
            SplineC tSpline = tNode.spline;

            float tOffset = 7f;
            float tPos1 = tNode.time - (tOffset / tSpline.distance);
            float tPos2 = tNode.time + (tOffset / tSpline.distance);
            Vector3 tVect1 = tSpline.GetSplineValue(tPos1);
            Vector3 POS1 = tSpline.GetSplineValue(tPos1, true);
            Vector3 tVect2 = tSpline.GetSplineValue(tPos2);
            Vector3 POS2 = tSpline.GetSplineValue(tPos2, true);

            tVects[0] = (tVect1 + new Vector3(5f * POS1.normalized.z, 0, 5f * -POS1.normalized.x));
            tVects[1] = (tVect1 + new Vector3(5f * -POS1.normalized.z, 0, 5f * POS1.normalized.x));
            tVects[2] = (tVect2 + new Vector3(5f * POS2.normalized.z, 0, 5f * -POS2.normalized.x));
            tVects[3] = (tVect2 + new Vector3(5f * -POS2.normalized.z, 0, 5f * POS2.normalized.x));

            return tVects;
        }


        private static Vector3[] GetExtendedVectors(RoadIntersection _roadIntersection, bool _isPrimary = true)
        {
            Vector3[] tVects = new Vector3[4];
            SplineN tNode;
            if (_isPrimary)
            {
                tNode = _roadIntersection.node1;
            }
            else
            {
                tNode = _roadIntersection.node2;
            }
            SplineC tSpline = tNode.spline;
            Vector3 NodePos = tNode.transform.position;

            float tOffset = tNode.spline.road.RoadWidth();
            float tOffset2 = tOffset * 0.5f;
            float tPos1 = tNode.time - (tOffset / tSpline.distance);
            float tPos2 = tNode.time + (tOffset / tSpline.distance);
            Vector3 tVect1 = tSpline.GetSplineValue(tPos1);
            Vector3 tVect2 = tSpline.GetSplineValue(tPos2);

            //Enforce distance:
            int SpamGuard = 0;
            int SGMax = 50;
            while (Vector3.Distance(tVect1, NodePos) < tOffset && SpamGuard < SGMax)
            {
                SpamGuard += 1;
                tPos1 -= (1f / tSpline.distance);
                tVect1 = tSpline.GetSplineValue(tPos1);
            }
            SpamGuard = 0;
            while (Vector3.Distance(tVect1, NodePos) > (tOffset * 1.2f) && SpamGuard < SGMax)
            {
                SpamGuard += 1;
                tPos1 += (0.25f / tSpline.distance);
                tVect1 = tSpline.GetSplineValue(tPos1);
            }
            SpamGuard = 0;
            while (Vector3.Distance(tVect2, NodePos) < tOffset && SpamGuard < SGMax)
            {
                SpamGuard += 1;
                tPos2 += (1f / tSpline.distance);
                tVect2 = tSpline.GetSplineValue(tPos2);
            }
            SpamGuard = 0;
            while (Vector3.Distance(tVect1, NodePos) > (tOffset * 1.2f) && SpamGuard < SGMax)
            {
                SpamGuard += 1;
                tPos2 -= (0.25f / tSpline.distance);
                tVect2 = tSpline.GetSplineValue(tPos2);
            }

            Vector3 POS1 = tSpline.GetSplineValue(tPos1, true);
            Vector3 POS2 = tSpline.GetSplineValue(tPos2, true);

            tVects[0] = (tVect1 + new Vector3(tOffset2 * POS1.normalized.z, 0, tOffset2 * -POS1.normalized.x));
            tVects[1] = (tVect1 + new Vector3(tOffset2 * -POS1.normalized.z, 0, tOffset2 * POS1.normalized.x));
            tVects[2] = (tVect2 + new Vector3(tOffset2 * POS2.normalized.z, 0, tOffset2 * -POS2.normalized.x));
            tVects[3] = (tVect2 + new Vector3(tOffset2 * -POS2.normalized.z, 0, tOffset2 * POS2.normalized.x));

            return tVects;
        }


        //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        //to each other. This function finds those two points. If the lines are not parallel, the function 
        //outputs true, otherwise false.
        private static bool ClosestPointsOnTwoLines(out Vector3 _closestPointLine1, out Vector3 _closestPointLine2, Vector3 _linePoint1, Vector3 _lineVec1, Vector3 _linePoint2, Vector3 _lineVec2)
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


        private static void CreateIntersectionMeshMain(RoadIntersection _roadIntersection, float _height, out Vector3[] _vectors, ref string _name)
        {
            //Get four points:
            Vector3[] pVects = GetCornerVectors(_roadIntersection);
            Vector3[] sVects = GetCornerVectors(_roadIntersection, false);
            _vectors = new Vector3[4];
            Vector3 oIntersection = new Vector3(0f, 0f, 0f);
            Vector3 oIntersection2 = new Vector3(0f, 0f, 0f);//Unused

            //			bool bIntersection;
            ClosestPointsOnTwoLines(out oIntersection, out oIntersection2, pVects[0], (pVects[2] - pVects[0]), sVects[0], (sVects[2] - sVects[0]));
            _vectors[0] = oIntersection;
            _vectors[0].y = _height;

            ClosestPointsOnTwoLines(out oIntersection, out oIntersection2, pVects[0], (pVects[2] - pVects[0]), sVects[1], (sVects[3] - sVects[1]));
            _vectors[1] = oIntersection;
            _vectors[1].y = _height;

            ClosestPointsOnTwoLines(out oIntersection, out oIntersection2, pVects[1], (pVects[3] - pVects[1]), sVects[0], (sVects[2] - sVects[0]));
            _vectors[2] = oIntersection;
            _vectors[2].y = _height;

            ClosestPointsOnTwoLines(out oIntersection, out oIntersection2, pVects[1], (pVects[3] - pVects[1]), sVects[1], (sVects[3] - sVects[1]));
            _vectors[3] = oIntersection;
            _vectors[3].y = _height;

            CreateIntersectionMeshMainInternal(_vectors, _roadIntersection.transform.gameObject, ref _name);
        }


        private static void CreateIntersectionMeshMainInternal(Vector3[] _verts, GameObject _obj, ref string _name)
        {
            Mesh tMesh = new Mesh();
            int MVL = 4;
            int triCount = (int)(4f * 1.5f);

            //			GameObject tObj;
            //			tObj = GameObject.Find("tInter1"); tObj.transform.position = tVerts[0];
            //			tObj = GameObject.Find("tInter2"); tObj.transform.position = tVerts[1];
            //			tObj = GameObject.Find("tInter3"); tObj.transform.position = tVerts[2];
            //			tObj = GameObject.Find("tInter4"); tObj.transform.position = tVerts[3];

            for (int index = 0; index < MVL; index++)
            {
                _verts[index] -= _obj.transform.position;
            }
            tMesh.vertices = _verts;
            tMesh.RecalculateBounds();

            int[] tri = new int[triCount];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            tMesh.triangles = tri;

            Vector3[] normals = new Vector3[MVL];
            for (int i = 0; i < MVL; i++)
            {
                normals[i] = -Vector3.forward;
            }
            tMesh.normals = normals;
            tMesh.RecalculateNormals();

            Vector2[] uv = new Vector2[MVL];
            uv[0] = new Vector2(0f, 0f);
            uv[1] = new Vector2(1f, 0f);
            uv[2] = new Vector2(0f, 1f);
            uv[3] = new Vector2(1f, 1f);
            tMesh.uv = uv;

            RootUtils.ProcessTangents(ref tMesh);

            //Final processing:
            MeshFilter MF = _obj.GetComponent<MeshFilter>();
            if (!MF)
            {
                MF = _obj.AddComponent<MeshFilter>();
            }
            MF.sharedMesh = tMesh;
            //MeshToFile(MF, RoadEditorUtility.GetBasePath() + "/Mesh/Intersections/" + name +".obj");

            MeshCollider MC = _obj.GetComponent<MeshCollider>();
            if (MC)
            {
                Object.DestroyImmediate(MC);
            }
            //if(!MC){ MC = iObj.AddComponent<MeshCollider>(); }
            //MC.sharedMesh = MF.sharedMesh;

            MeshRenderer MR = _obj.GetComponent<MeshRenderer>();
            if (!MR)
            {
                MR = _obj.AddComponent<MeshRenderer>();
            }
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MR.receiveShadows = true;
            RoadEditorUtility.SetRoadMaterial(RoadEditorUtility.GetBasePath() + "/Materials/RoadIntersection.mat", MR);
        }


        private static void CreateIntersectionMeshOuter(RoadIntersection _roadIntersection, Vector3[] _vects, ref string _name)
        {
            Vector3[] bVects1 = GetExtendedVectors(_roadIntersection);
            Vector3[] bVects2 = GetExtendedVectors(_roadIntersection, false);
            Vector3[] eVects = new Vector3[16];
            eVects[0] = _vects[1];
            eVects[1] = _vects[0];
            eVects[2] = bVects2[3];
            eVects[3] = bVects2[2];

            eVects[4] = _vects[3];
            eVects[5] = _vects[1];
            eVects[6] = bVects1[3];
            eVects[7] = bVects1[2];

            eVects[8] = _vects[2];
            eVects[9] = _vects[3];
            eVects[10] = bVects2[0];
            eVects[11] = bVects2[1];

            eVects[12] = _vects[0];
            eVects[13] = _vects[2];
            eVects[14] = bVects1[0];
            eVects[15] = bVects1[1];

            int cCount = _roadIntersection.transform.childCount;
            //bool bOuter = false;
            GameObject tOuter = null;
            for (int index = 0; index < cCount; index++)
            {
                if (_roadIntersection.transform.GetChild(index).transform.name == "outer")
                {
                    tOuter = _roadIntersection.transform.GetChild(index).transform.gameObject;
                }
            }
            if (!tOuter)
            {
                tOuter = new GameObject("outer");
                tOuter.transform.parent = _roadIntersection.transform;
            }
            tOuter.transform.position = _roadIntersection.transform.position;

            //			GameObject tObj;
            //			tObj = GameObject.Find("tInter1"); tObj.transform.position = bVects2[0];
            //			tObj = GameObject.Find("tInter2"); tObj.transform.position = bVects2[1];
            //			tObj = GameObject.Find("tInter3"); tObj.transform.position = bVects2[2];
            //			tObj = GameObject.Find("tInter4"); tObj.transform.position = bVects2[3];

            CreateIntersectionMeshOuterInternal(eVects, tOuter, _roadIntersection.transform.position, ref _name);
        }


        private static void CreateIntersectionMeshOuterInternal(Vector3[] _verts, GameObject _obj, Vector3 _vectorOffset, ref string _name)
        {
            Mesh tMesh = new Mesh();
            int MVL = 16;
            int triCount = (int)(16f * 1.5f);

            for (int index = 0; index < MVL; index += 4)
            {
                //				tVerts[index] += vOffset;
                //				tVerts[index+1] += vOffset;
                _verts[index + 2] -= _vectorOffset;
                _verts[index + 3] -= _vectorOffset;
            }
            tMesh.vertices = _verts;
            tMesh.RecalculateBounds();

            int[] tri = new int[triCount];
            int cTri = 0;
            for (int i = 0; i < triCount; i += 4)
            {
                if (i + 3 >= MVL)
                {
                    break;
                }
                tri[cTri] = i;
                cTri += 1;
                tri[cTri] = i + 2;
                cTri += 1;
                tri[cTri] = i + 1;
                cTri += 1;

                tri[cTri] = i + 2;
                cTri += 1;
                tri[cTri] = i + 3;
                cTri += 1;
                tri[cTri] = i + 1;
                cTri += 1;
            }
            tMesh.triangles = tri;

            Vector3[] normals = new Vector3[MVL];
            for (int i = 0; i < 4; i++)
            {
                normals[i] = -Vector3.forward;
            }
            tMesh.normals = normals;
            tMesh.RecalculateNormals();

            Vector2[] uv = new Vector2[MVL];
            for (int i = 0; i < MVL; i += 4)
            {
                uv[i] = new Vector2(1f, 1f);
                uv[i + 1] = new Vector2(0f, 1f);
                uv[i + 2] = new Vector2(1f, 0f);
                uv[i + 3] = new Vector2(0f, 0f);
            }
            tMesh.uv = uv;

            RootUtils.ProcessTangents(ref tMesh);

            //Final processing:
            MeshFilter MF = _obj.GetComponent<MeshFilter>();
            if (!MF)
            {
                MF = _obj.AddComponent<MeshFilter>();
            }
            MF.sharedMesh = tMesh;

            //MeshToFile(MF, RoadEditorUtility.GetBasePath() + "/Mesh/Intersections/" + name +"-overlay.obj");

            MeshCollider MC = _obj.GetComponent<MeshCollider>();
            if (MC)
            {
                Object.DestroyImmediate(MC);
            }
            //if(!MC){ MC = iObj.AddComponent<MeshCollider>(); }
            //MC.sharedMesh = MF.sharedMesh;

            MeshRenderer MR = _obj.GetComponent<MeshRenderer>();
            if (!MR)
            {
                MR = _obj.AddComponent<MeshRenderer>();
            }
            MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MR.receiveShadows = true;
            RoadEditorUtility.SetRoadMaterial(RoadEditorUtility.GetBasePath() + "/Materials/InterText.mat", MR);
        }


        #region "Intersection creation"
        private static void InitializeIntersectionObjects(RoadIntersection _roadIntersection)
        {
            if (_roadIntersection != null)
            {
                InitializeIntersectionObjectsInternal(_roadIntersection);
            }
            else
            {
                Object[] allIntersections = GameObject.FindObjectsOfType<RoadIntersection>();
                //Add intersection components, if necessary:
                foreach (RoadIntersection roadIntersection in allIntersections)
                {
                    InitializeIntersectionObjectsInternal(roadIntersection);
                }
            }
        }


        private static void InitializeIntersectionObjectsInternal(RoadIntersection _roadIntersection)
        {
            //1. Determine 3-way or 4-way. # of corners for 3-way: 2. 4-way = 4.
            if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
            {
                _roadIntersection.cornerPoints = new RoadIntersection.CornerPositionMaker[2];
            }
            else
            {
                _roadIntersection.cornerPoints = new RoadIntersection.CornerPositionMaker[4];
            }

            //Contains int IDs of connected nodes:			
            List<SplineN> tList = new List<SplineN>();
            //Get all connected nodes on intersection node1:
            int cCount = _roadIntersection.node1.connectedID.Count;
            SplineN tNode;
            for (int index = 0; index < cCount; index++)
            {
                //tNode = GetNodeByID(_roadIntersection.node1.connectedID[index]);
                tNode = _roadIntersection.node1.connectedNode[index];
                if (!tList.Contains(tNode))
                {
                    tList.Add(tNode);
                }
            }
            //Get all connected nodes on intersection node2:
            cCount = _roadIntersection.node2.connectedID.Count;
            for (int index = 0; index < cCount; index++)
            {
                //tNode = GetNodeByID(_roadIntersection.node2.connectedID[index]);
                tNode = _roadIntersection.node2.connectedNode[index];
                if (!tList.Contains(tNode))
                {
                    tList.Add(tNode);
                }
            }
            //Declare connected nodes:
            SplineN n1, n2, n3, n4;
            n1 = tList[0];
            n2 = tList[1];
            n3 = tList[2];
            n4 = null;
            if (tList.Count > 3)
            {
                n4 = tList[3];
            }

            //Determine most relevant spline:
            SplineC n1Spline = n1.spline;
            SplineC n2Spline = n2.spline;
            SplineC n3Spline = n3.spline;
            SplineC n4Spline = null;
            if (n4 != null)
            {
                n4Spline = n4.spline;
            }

            //Get the point:
            Vector3 n1Point = GetFourCornerPoint(ref n1Spline, ref n1, _roadIntersection);
            Vector3 n2Point = GetFourCornerPoint(ref n2Spline, ref n2, _roadIntersection);
            Vector3 n3Point = GetFourCornerPoint(ref n3Spline, ref n3, _roadIntersection);
            Vector3 n4Point = new Vector3(0f, 0f, 0f);
            if (n4Spline != null)
            {
                n4Point = GetFourCornerPoint(ref n4Spline, ref n4, _roadIntersection);
            }

            //2. If 3 way, we know that 2 of the nodes have the same spline.
            if (1 == 1 && _roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
            {
                //Should be size 3:
                if (tList.Count != 3)
                {
                    Debug.LogError("InitializeIntersections hashset != 3 connected on three way intersection, real size: " + tList.Count + " on intersection: " + _roadIntersection.transform.name);
                    return;
                }

                ProcessFourCorners(ref n1Point, ref n2Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n1Spline, n2Spline));
                ProcessFourCorners(ref n1Point, ref n3Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n1Spline, n3Spline));
                ProcessFourCorners(ref n2Point, ref n3Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n2Spline, n3Spline));

            }
            else if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.FourWay)
            {
                //Should be size 3:
                if (tList.Count != 4)
                {
                    Debug.LogError("InitializeIntersections hashset != 4 connected on four way intersection, real size: " + tList.Count + " on intersection: " + _roadIntersection.transform.name);
                    return;
                }

                ProcessFourCorners(ref n1Point, ref n2Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n1Spline, n2Spline));
                ProcessFourCorners(ref n1Point, ref n3Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n1Spline, n3Spline));
                ProcessFourCorners(ref n1Point, ref n4Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n1Spline, n4Spline));
                ProcessFourCorners(ref n2Point, ref n3Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n2Spline, n3Spline));
                ProcessFourCorners(ref n2Point, ref n4Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n2Spline, n4Spline));
                ProcessFourCorners(ref n3Point, ref n4Point, _roadIntersection.transform.gameObject, GetLongestSplineDistance(n3Spline, n4Spline));
            }
        }
        #endregion


        #region "Intersection creation helpers"
        private static float GetLongestSplineDistance(SplineC _spline1, SplineC _spline2)
        {
            if (_spline1.distance > _spline2.distance)
            {
                return _spline1.distance;
            }
            else
            {
                return _spline2.distance;
            }
        }


        private static SplineN GetNodeByID(int _ID)
        {
            Object[] SplineNodeObjects = GameObject.FindObjectsOfType<SplineN>();
            foreach (SplineN tNode in SplineNodeObjects)
            {
                if (tNode.id == _ID)
                {
                    return tNode;
                }
            }
            return null;
        }


        private static Vector3 GetFourCornerPoint(ref SplineC _spline, ref SplineN _node, RoadIntersection _roadIntersection)
        {
            SplineN iNode;
            if (_node.connectedNode.Contains(_roadIntersection.node1))
            {
                iNode = _roadIntersection.node1;
            }
            else
            {
                iNode = _roadIntersection.node2;
            }

            float Pos1 = _node.time;
            float iPos = iNode.time;

            float tFloat = 0;
            float NewSplinePos = 0;
            if (iPos >= Pos1)
            {
                tFloat = iPos - Pos1;
                tFloat = tFloat / 8;
                NewSplinePos = iPos - tFloat;
            }
            else
            {
                tFloat = Pos1 - iPos;
                tFloat = tFloat / 8;
                NewSplinePos = iPos + tFloat;
            }

            Vector3 tVect = new Vector3(0, 0, 0);

            bool bDone = false;
            int spamguard = 0;
            float tDist = 0f;
            while (!bDone && spamguard < 20000)
            {
                spamguard += 1;
                tVect = _spline.GetSplineValue(NewSplinePos);
                tDist = Vector3.Distance(tVect, iNode.transform.position);

                if (tDist > 22f)
                {
                    //Get closer to intersection:
                    if (iPos >= NewSplinePos)
                    {
                        NewSplinePos += 0.001f;
                    }
                    else
                    {
                        NewSplinePos -= 0.001f;
                    }
                }
                else if (tDist < 20f)
                {
                    //Move away from intersection:
                    if (iPos >= NewSplinePos)
                    {
                        NewSplinePos -= 0.001f;
                    }
                    else
                    {
                        NewSplinePos += 0.001f;
                    }
                }
                else
                {
                    bDone = true;
                }
            }
            return tVect;
        }


        private static void ProcessFourCorners(ref Vector3 _n1, ref Vector3 _n2, GameObject _intersectionObject, float _splineDistance)
        {
            float Side1, Side2, Side3;
            Side1 = Vector3.Distance(_n1, _n2);
            Side2 = Vector3.Distance(_intersectionObject.transform.position, _n1);
            Side3 = Vector3.Distance(_intersectionObject.transform.position, _n2);
            float tAngle = AngleOfTriangle(Side2, Side3, Side1);
            if (tAngle > 20f && tAngle < 140f)
            {
                ProcessTwoCorners(ref _intersectionObject, ref _n1, ref _n2, _splineDistance);
            }
        }


        private static float AngleOfTriangle(float _a, float _b, float _c)
        {
            float cAng = (_a * _a + _b * _b - _c * _c) / (2 * _a * _b);
            float radAngle = Mathf.Acos(cAng);
            float degAngle = Mathf.Rad2Deg * radAngle;
            return degAngle;
        }


        private static void ProcessTwoCorners(ref GameObject _intersectionObject, ref Vector3 _n1, ref Vector3 _n2, float _splineDistance)
        {
            GameObject tCorner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCorner.transform.localScale = new Vector3(0.5f, 20f, 0.5f);
            tCorner.name = "CornerPosition";
            tCorner.transform.parent = _intersectionObject.transform;
            tCorner.layer = 0;

            //Calculate the angle:
            Vector3 v3BA = _n2 - _intersectionObject.transform.position;
            Vector3 v3BC = _n1 - _intersectionObject.transform.position;
            Vector3 axis = Vector3.Cross(v3BA, v3BC);
            float angle = Vector3.Angle(v3BA, v3BC);
            Vector3 v3 = Quaternion.AngleAxis(angle / 2.0f, axis) * v3BA;
            //Vector3 v3 = (((n2.transform.position + n1.transform.position)/2) - tIntersectionObject.transform.position);

            tCorner.transform.rotation = Quaternion.LookRotation(v3);

            float tStep = 1.25f / _splineDistance;
            bool bSuccess = MoveCorner(tStep, 3f, ref tCorner, ref _intersectionObject, v3);
            if (!bSuccess)
            {
                Debug.Log("not success");
                Object.DestroyImmediate(tCorner);
            }
        }


        private static bool MoveCorner(float _step, float _dist, ref GameObject _corner, ref GameObject _intersectionObject, Vector3 _vector)
        {
            float tStart = 0.05f;
            bool isDone = false;
            int spamguard = 0;
            bool isRoadHit = false;
            Collider[] tCollider;
            while (!isDone)
            {
                spamguard += 1;
                _corner.transform.position = _intersectionObject.transform.position + (_vector * tStart);

                if (Vector3.Distance(_corner.transform.position, _intersectionObject.transform.position) > 25f)
                {
                    Debug.Log("too long");
                    isDone = true;
                    return false;
                }
                if (spamguard > 80000)
                {
                    Debug.Log("spamguard");
                    isDone = true;
                    return false;
                }

                //Cast sphere now
                isRoadHit = false;
                tCollider = Physics.OverlapSphere(_corner.transform.position, _dist);
                if (tCollider == null || tCollider.Length < 1)
                {
                    tStart += _step;
                    continue;
                }
                else
                {
                    for (int k = 0; k < tCollider.Length; k++)
                    {
                        if (tCollider[k].transform.name.ToLower().Contains("road"))
                        {
                            isRoadHit = true;
                            break;
                        }
                    }
                }

                if (isRoadHit)
                {
                    tStart += _step;
                    continue;
                }
                else
                {
                    //Debug.Log ("Distance: " + Vector3.Distance(tCorner.transform.position,tIntersectionObject.transform.position));
                    isDone = true;
                    break;
                }
            }
            return true;
        }
        #endregion
        #endregion
    }
}
