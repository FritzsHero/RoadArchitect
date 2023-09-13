using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    public static class IntersectionObjects
    {
        /// <summary> Destroy all intersection objects </summary>
        public static void CleanupIntersectionObjects(GameObject _masterGameObj)
        {
            int childCount = _masterGameObj.transform.childCount - 1;
            if (childCount < 0)
            {
                return;
            }

            Transform child;
            for (int index = childCount; index >= 0; index--)
            {
                child = _masterGameObj.transform.GetChild(index);

                if (child.name.ToLower().Contains("stopsign"))
                {
                    Object.DestroyImmediate(child.gameObject);
                    continue;
                }
                if (child.name.ToLower().Contains("trafficlight"))
                {
                    Object.DestroyImmediate(child.gameObject);
                }
            }
        }


        #region "Stop Sign All Way"
        public static void CreateStopSignsAllWay(GameObject _masterGameObj, bool _isRB = true)
        {
            CreateStopSignsAllWayDo(ref _masterGameObj, _isRB);
        }


        /// <summary> Adds a rigidbody to the stop signs </summary>
        private static void AddRigidbodyToSign(GameObject _obj, bool _isRB)
        {
            if (_isRB)
            {
                Rigidbody RB = _obj.AddComponent<Rigidbody>();
                RB.mass = 100f;
                RB.centerOfMass = new Vector3(0f, -10f, 0f);
                RB.useGravity = false;
                RB.isKinematic = true;
            }
        }


        /// <summary> Creates the stop signs on a cross or T intersection </summary>
        private static void CreateStopSignsAllWayDo(ref GameObject _masterGameObj, bool _isRB)
        {
            Object prefab;
            prefab = EngineIntegration.LoadAssetFromPath<GameObject>(RoadEditorUtility.GetBasePath() + "/Prefabs/Signs/StopSignAllway.prefab");

            RoadIntersection roadIntersection = _masterGameObj.GetComponent<RoadIntersection>();
            SplineC spline = roadIntersection.node1.spline;

            GameObject tObj = null;
            //Vector3 xDir = default(Vector3);
            Vector3 tDir = default(Vector3);
            //float roadWidth = spline.road.RoadWidth();
            //float laneWidth = spline.road.laneWidth;
            float ShoulderWidth = spline.road.shoulderWidth;

            //Cleanup:
            CleanupIntersectionObjects(_masterGameObj);

            //Get four points:
            float DistFromCorner = (ShoulderWidth * 0.45f);
            Vector3 tPosRR = default(Vector3);
            Vector3 tPosRL = default(Vector3);
            Vector3 tPosLR = default(Vector3);
            Vector3 tPosLL = default(Vector3);
            GetFourPoints(roadIntersection, out tPosRR, out tPosRL, out tPosLL, out tPosLR, DistFromCorner);

            //RR:
            spline = roadIntersection.node1.spline;
            tObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            //xDir = (roadIntersection.CornerRR - roadIntersection.transform.position).normalized;
            tDir = StopSignGetRotRR(roadIntersection, spline);
            tObj.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(0f, 180f, 0f);
            AddRigidbodyToSign(tObj, _isRB);

            tObj.transform.parent = _masterGameObj.transform;
            tObj.transform.position = tPosRR;
            tObj.name = "StopSignRR";
            if (roadIntersection.ignoreCorner == 0)
            {
                Object.DestroyImmediate(tObj);
            }

            //LL:
            spline = roadIntersection.node1.spline;
            tObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            //xDir = (roadIntersection.CornerLL - roadIntersection.transform.position).normalized;
            tDir = StopSignGetRotLL(roadIntersection, spline);
            tObj.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(0f, 180f, 0f);
            AddRigidbodyToSign(tObj, _isRB);

            tObj.transform.parent = _masterGameObj.transform;
            tObj.transform.position = tPosLL;
            tObj.name = "StopSignLL";
            if (roadIntersection.ignoreCorner == 2)
            {
                Object.DestroyImmediate(tObj);
            }

            //RL:
            spline = roadIntersection.node2.spline;
            tObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            //xDir = (roadIntersection.CornerRL - roadIntersection.transform.position).normalized;
            tDir = StopSignGetRotRL(roadIntersection, spline);
            tObj.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(0f, 180f, 0f);
            AddRigidbodyToSign(tObj, _isRB);

            tObj.transform.parent = _masterGameObj.transform;
            tObj.transform.position = tPosRL;
            tObj.name = "StopSignRL";
            if (roadIntersection.ignoreCorner == 1)
            {
                Object.DestroyImmediate(tObj);
            }

            //LR:
            spline = roadIntersection.node2.spline;
            tObj = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            //xDir = (roadIntersection.CornerLR - roadIntersection.transform.position).normalized;
            tDir = StopSignGetRotLR(roadIntersection, spline);
            tObj.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(0f, 180f, 0f);
            AddRigidbodyToSign(tObj, _isRB);

            tObj.transform.parent = _masterGameObj.transform;
            tObj.transform.position = tPosLR;
            tObj.name = "StopSignLR";
            if (roadIntersection.ignoreCorner == 3)
            {
                Object.DestroyImmediate(tObj);
            }
        }


        /// <summary> Returns rotation for stop sign right right </summary>
        private static Vector3 StopSignGetRotRR(RoadIntersection _intersection, SplineC _spline)
        {
            float tDist = ((Vector3.Distance(_intersection.cornerRL, _intersection.cornerRR) / 2f) + (0.025f * Vector3.Distance(_intersection.cornerLL, _intersection.cornerRR))) / _spline.distance;
            float p = Mathf.Clamp(_intersection.node1.time - tDist, 0f, 1f);
            Vector3 rotation = _spline.GetSplineValue(p, true);
            return (rotation * -1);
        }


        /// <summary> Returns rotation for stop sign left left </summary>
        private static Vector3 StopSignGetRotLL(RoadIntersection _intersection, SplineC _spline)
        {
            float tDist = ((Vector3.Distance(_intersection.cornerLR, _intersection.cornerLL) / 2f) + (0.025f * Vector3.Distance(_intersection.cornerLL, _intersection.cornerRR))) / _spline.distance;
            float p = Mathf.Clamp(_intersection.node1.time + tDist, 0f, 1f);
            Vector3 rotation = _spline.GetSplineValue(p, true);
            return rotation;
        }


        /// <summary> Returns rotation for stop sign right left </summary>
        private static Vector3 StopSignGetRotRL(RoadIntersection _intersetion, SplineC _spline)
        {
            float tDist = ((Vector3.Distance(_intersetion.cornerLL, _intersetion.cornerRL) / 2f) + (0.025f * Vector3.Distance(_intersetion.cornerLR, _intersetion.cornerRL))) / _spline.distance;
            float p = -1f;
            if (_intersetion.isFlipped)
            {
                p = Mathf.Clamp(_intersetion.node2.time - tDist, 0f, 1f);
            }
            else
            {
                p = Mathf.Clamp(_intersetion.node2.time + tDist, 0f, 1f);
            }
            Vector3 rotation = _spline.GetSplineValue(p, true);
            //rotation = Vector3.Cross(rotation, Vector3.up);
            if (_intersetion.isFlipped)
            {
                return (rotation * -1);
            }
            else
            {
                return rotation;
            }
        }


        /// <summary> Returns rotation for stop sign left right </summary>
        private static Vector3 StopSignGetRotLR(RoadIntersection _intersection, SplineC _spline)
        {
            float tDist = ((Vector3.Distance(_intersection.cornerRR, _intersection.cornerLR) / 2f) + (0.025f * Vector3.Distance(_intersection.cornerLR, _intersection.cornerRL))) / _spline.distance;
            float p = -1f;
            if (_intersection.isFlipped)
            {
                p = Mathf.Clamp(_intersection.node2.time + tDist, 0f, 1f);
            }
            else
            {
                p = Mathf.Clamp(_intersection.node2.time - tDist, 0f, 1f);
            }
            Vector3 rotation = _spline.GetSplineValue(p, true);
            //rotation = Vector3.Cross(rotation, Vector3.up);
            if (_intersection.isFlipped)
            {
                return rotation;
            }
            else
            {
                return (rotation * -1);
            }
        }
        #endregion


        #region "Traffic light bases"
        /// <summary> Creates and rotates the traffic light bases </summary>
        public static void CreateTrafficLightBases(GameObject _masterGameObj, bool _isTrafficLight1 = true)
        {
            RoadIntersection intersection = _masterGameObj.GetComponent<RoadIntersection>();
            SplineC spline = intersection.node1.spline;
            bool isRB = true;

            //float RoadWidth = spline.road.RoadWidth();
            float LaneWidth = spline.road.laneWidth;
            float ShoulderWidth = spline.road.shoulderWidth;

            int Lanes = spline.road.laneAmount;
            int LanesHalf = Lanes / 2;
            float LanesForInter = -1;
            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                LanesForInter = LanesHalf + 1f;
            }
            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
            {
                LanesForInter = LanesHalf + 1f;
            }
            else
            {
                LanesForInter = LanesHalf;
            }
            float DistFromCorner = (ShoulderWidth * 0.45f);
            float TLDistance = (LanesForInter * LaneWidth) + DistFromCorner;

            GameObject tObjRR = null;
            GameObject tObjRL = null;
            GameObject tObjLL = null;
            GameObject tObjLR = null;
            //Vector3 xDir = default(Vector3);
            Vector3 tDir = default(Vector3);
            Vector3 zeroVect = new Vector3(0f, 0f, 0f);
            Vector3 StartVec = default(Vector3);
            Vector3 EndVec = default(Vector3);
            //bool bContains = false;
            //MeshFilter MF = null;
            //Vector3[] tVerts = null;

            //Get four points:
            Vector3 tPosRR = default(Vector3);
            Vector3 tPosRL = default(Vector3);
            Vector3 tPosLR = default(Vector3);
            Vector3 tPosLL = default(Vector3);
            GetFourPoints(intersection, out tPosRR, out tPosRL, out tPosLL, out tPosLR, DistFromCorner);

            //Cleanup:
            CleanupIntersectionObjects(_masterGameObj);

            float[] tempDistances = new float[4];
            tempDistances[0] = Vector3.Distance(intersection.cornerRL, intersection.cornerLL);
            tempDistances[1] = Vector3.Distance(intersection.cornerRL, intersection.cornerRR);
            tempDistances[2] = Vector3.Distance(intersection.cornerLR, intersection.cornerLL);
            tempDistances[3] = Vector3.Distance(intersection.cornerLR, intersection.cornerRR);
            float MaxDistanceStart = Mathf.Max(tempDistances);
            bool OrigPoleAlignment = intersection.isRegularPoleAlignment;

            //Node1:
            //RL:
            tObjRL = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
            //xDir = (intersection.cornerRL - intersection.transform.position).normalized;
            tDir = TrafficLightBaseGetRotRL(intersection, spline, DistFromCorner);
            if (tDir == zeroVect)
            {
                tDir = new Vector3(0f, 0.0001f, 0f);
            }
            tObjRL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
            tObjRL.transform.parent = _masterGameObj.transform;
            StartVec = tPosRL;
            EndVec = (tDir.normalized * TLDistance) + StartVec;
            if (!intersection.isRegularPoleAlignment && intersection.ContainsLine(StartVec, EndVec))
            {
                //Convert to regular alignment if necessary
                tObjRL.transform.parent = null;
                tDir = TrafficLightBaseGetRotRL(intersection, spline, DistFromCorner, true);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjRL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjRL.transform.parent = _masterGameObj.transform;
            }
            else
            {
                intersection.isRegularPoleAlignment = true;
                Object.DestroyImmediate(tObjRL);
                tObjRL = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
                //xDir = (intersection.cornerRL - intersection.transform.position).normalized;
                tDir = TrafficLightBaseGetRotRL(intersection, spline, DistFromCorner);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjRL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjRL.transform.parent = _masterGameObj.transform;
                StartVec = tPosRL;
                EndVec = (tDir.normalized * TLDistance) + StartVec;
                intersection.isRegularPoleAlignment = OrigPoleAlignment;
            }
            tObjRL.transform.position = tPosRL;
            tObjRL.transform.name = "TrafficLightRL";
            AddRigidbodyToTrafficLight(tObjRL, isRB);
            //LR:
            tObjLR = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
            //xDir = (intersection.cornerLR - intersection.transform.position).normalized;
            tDir = TrafficLightBaseGetRotLR(intersection, spline, DistFromCorner);
            if (tDir == zeroVect)
            {
                tDir = new Vector3(0f, 0.0001f, 0f);
            }
            tObjLR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
            tObjLR.transform.parent = _masterGameObj.transform;
            StartVec = tPosLR;
            EndVec = (tDir.normalized * TLDistance) + StartVec;
            if (!intersection.isRegularPoleAlignment && intersection.ContainsLine(StartVec, EndVec))
            {
                //Convert to regular alignment if necessary
                tObjLR.transform.parent = null;
                tDir = TrafficLightBaseGetRotLR(intersection, spline, DistFromCorner, true);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjLR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjLR.transform.parent = _masterGameObj.transform;
            }
            else
            {
                intersection.isRegularPoleAlignment = true;
                Object.DestroyImmediate(tObjLR);
                tObjLR = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
                //xDir = (intersection.cornerLR - intersection.transform.position).normalized;
                tDir = TrafficLightBaseGetRotLR(intersection, spline, DistFromCorner);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjLR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjLR.transform.parent = _masterGameObj.transform;
                StartVec = tPosLR;
                EndVec = (tDir.normalized * TLDistance) + StartVec;
                intersection.isRegularPoleAlignment = OrigPoleAlignment;
            }
            tObjLR.transform.position = tPosLR;
            tObjLR.transform.name = "TrafficLightLR";
            AddRigidbodyToTrafficLight(tObjLR, isRB);
            //Node2:
            //RR:
            tObjRR = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
            //xDir = (intersection.cornerRR - intersection.transform.position).normalized;
            tDir = TrafficLightBaseGetRotRR(intersection, spline, DistFromCorner);
            if (tDir == zeroVect)
            {
                tDir = new Vector3(0f, 0.0001f, 0f);
            }
            tObjRR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
            tObjRR.transform.parent = _masterGameObj.transform;
            StartVec = tPosRR;
            EndVec = (tDir.normalized * TLDistance) + StartVec;
            if (!intersection.isRegularPoleAlignment && intersection.ContainsLine(StartVec, EndVec))
            {
                //Convert to regular alignment if necessary
                tObjRR.transform.parent = null;
                tDir = TrafficLightBaseGetRotRR(intersection, spline, DistFromCorner, true);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjRR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, 0f, 0f);
                tObjRR.transform.parent = _masterGameObj.transform;
            }
            else
            {
                intersection.isRegularPoleAlignment = true;
                Object.DestroyImmediate(tObjRR);
                tObjRR = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
                //xDir = (intersection.cornerRR - intersection.transform.position).normalized;
                tDir = TrafficLightBaseGetRotRR(intersection, spline, DistFromCorner);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjRR.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjRR.transform.parent = _masterGameObj.transform;
                StartVec = tPosRR;
                EndVec = (tDir.normalized * TLDistance) + StartVec;
                intersection.isRegularPoleAlignment = OrigPoleAlignment;
            }
            tObjRR.transform.position = tPosRR;
            tObjRR.transform.name = "TrafficLightRR";
            AddRigidbodyToTrafficLight(tObjRR, isRB);

            //LL:
            tObjLL = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
            //xDir = (intersection.cornerLL - intersection.transform.position).normalized;
            tDir = TrafficLightBaseGetRotLL(intersection, spline, DistFromCorner);
            if (tDir == zeroVect)
            {
                tDir = new Vector3(0f, 0.0001f, 0f);
            }
            tObjLL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
            tObjLL.transform.parent = _masterGameObj.transform;
            StartVec = tPosLL;
            EndVec = (tDir.normalized * TLDistance) + StartVec;
            if (!intersection.isRegularPoleAlignment && intersection.ContainsLine(StartVec, EndVec))
            {
                //Convert to regular alignment if necessary
                tObjLL.transform.parent = null;
                tDir = TrafficLightBaseGetRotLL(intersection, spline, DistFromCorner, true);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjLL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, 0f, 0f);
                tObjLL.transform.parent = _masterGameObj.transform;
            }
            else
            {
                intersection.isRegularPoleAlignment = true;
                Object.DestroyImmediate(tObjLL);
                tObjLL = CreateTrafficLight(TLDistance, true, true, MaxDistanceStart, intersection.isTrafficPoleStreetLight, spline.road.isSavingMeshes);
                //xDir = (intersection.cornerLL - intersection.transform.position).normalized;
                tDir = TrafficLightBaseGetRotLL(intersection, spline, DistFromCorner);
                if (tDir == zeroVect)
                {
                    tDir = new Vector3(0f, 0.0001f, 0f);
                }
                tObjLL.transform.rotation = Quaternion.LookRotation(tDir) * Quaternion.Euler(-90f, -180f, 0f);
                tObjLL.transform.parent = _masterGameObj.transform;
                StartVec = tPosLL;
                EndVec = (tDir.normalized * TLDistance) + StartVec;
                intersection.isRegularPoleAlignment = OrigPoleAlignment;
            }
            tObjLL.transform.position = tPosLL;
            tObjLL.transform.name = "TrafficLightLL";
            AddRigidbodyToTrafficLight(tObjLL, isRB);

            //Create the actual lights:
            CreateTrafficLightMains(_masterGameObj, tObjRR, tObjRL, tObjLL, tObjLR);

            intersection.TogglePointLights(intersection.isLightsEnabled);
        }


        /// <summary> Adds a rigidbody to the traffic lights </summary>
        private static void AddRigidbodyToTrafficLight(GameObject _obj, bool _isRB)
        {
            if (_isRB)
            {
                Rigidbody RB = _obj.AddComponent<Rigidbody>();
                RB.mass = 12500f;
                RB.centerOfMass = new Vector3(0f, 0f, 4f);
                _obj.AddComponent<RoadArchitect.RigidBody>();
            }
        }


        private static bool IsTrafficLightBaseInIntersection(RoadIntersection _intersection, ref Vector3 _startVec, ref Vector3 _endVec)
        {
            return _intersection.ContainsLine(_startVec, _endVec);
        }


        private static GameObject CreateTrafficLight(float _distance, bool _isTrafficLight1, bool _isOptionalCollider, float _distanceX, bool _isLight, bool _isSavingAsset)
        {
            GameObject tObj = null;
            string tTrafficLightNumber = "1";
            if (!_isTrafficLight1)
            {
                tTrafficLightNumber = "2";
            }

            bool bDoCustom = false;
            //Round up to nearest whole F
            _distanceX = Mathf.Ceil(_distanceX);
            _distance = Mathf.Ceil(_distance);
            //string assetName = "InterTLB" + tTrafficLightNumber + "_" + tDistance.ToString("F0") + "_" + xDistance.ToString("F0") + ".prefab";
            string assetNameAsset = "InterTLB" + tTrafficLightNumber + "_" + _distance.ToString("F0") + "_" + _distanceX.ToString("F0") + ".asset";
            string BackupFBX = "InterTLB" + tTrafficLightNumber + ".FBX";
            float tMod = _distance / 5f;
            float hMod = (_distance / 10f) * 0.7f;
            float xMod = ((_distanceX / 20f) + 2f) * 0.3334f;
            xMod = Mathf.Clamp(xMod, 1f, 20f);
            tMod = Mathf.Clamp(tMod, 1f, 20f);
            hMod = Mathf.Clamp(hMod, 1f, 20f);

            bool bXMod = false;
            if (!RootUtils.IsApproximately(xMod, 1f, 0.0001f))
            {
                bXMod = true;
            }

            string basePath = RoadEditorUtility.GetBasePath();

            Mesh xMesh;
            xMesh = EngineIntegration.LoadAssetFromPath<Mesh>(basePath + "/Mesh/Signs/TrafficLightBases/" + assetNameAsset);
            if (xMesh == null)
            {
                xMesh = EngineIntegration.LoadAssetFromPath<Mesh>(basePath + "/Mesh/Signs/TrafficLightBases/" + BackupFBX);
                bDoCustom = true;
            }

            tObj = new GameObject("TempTrafficLight");
            MeshFilter MF = tObj.AddComponent<MeshFilter>();
            MeshRenderer MR = tObj.AddComponent<MeshRenderer>();
            RoadEditorUtility.SetRoadMaterial(basePath + "/Materials/Signs/InterTLB" + tTrafficLightNumber + ".mat", MR);

            if (!bDoCustom)
            {
                MF.sharedMesh = xMesh;
            }

            float tempMaxHeight = 7.6f * hMod;
            float xValue = tempMaxHeight - 7.6f;
            if (bDoCustom)
            {
                Mesh tMesh = new Mesh();
                tMesh.vertices = xMesh.vertices;
                tMesh.triangles = xMesh.triangles;
                tMesh.uv = xMesh.uv;
                tMesh.normals = xMesh.normals;
                tMesh.tangents = xMesh.tangents;
                MF.sharedMesh = tMesh;
                Vector3[] tVerts = tMesh.vertices;

                xValue = (xMod * 6f) - 6f;
                if ((xMod * 6f) > (tempMaxHeight - 1f))
                {
                    xValue = (tempMaxHeight - 1f) - 6f;
                }

                bool bIgnoreMe = false;

                int mCount = tVerts.Length;
                Vector2[] uv = tMesh.uv;
                //List<int> tUVInts = new List<int>();
                for (int index = 0; index < mCount; index++)
                {
                    bIgnoreMe = false;
                    // Scale width of traffic light
                    if (RootUtils.IsApproximately(tVerts[index].y, 5f, 0.01f))
                    {
                        tVerts[index].y = _distance;
                        if (uv[index].y > 3.5f)
                        {
                            uv[index].y *= tMod;
                        }
                        bIgnoreMe = true;
                    }
                    // Scale height of traffic light
                    if (!bIgnoreMe && tVerts[index].z > 7.5f)
                    {
                        tVerts[index].z *= hMod;
                        if (uv[index].y > 3.8f)
                        {
                            uv[index].y *= hMod;
                        }
                    }

                    // Increase overall height of mesh
                    if (bXMod && tVerts[index].z > 4.8f && tVerts[index].z < 6.2f)
                    {
                        //tVerts[index].z += xValue;
                    }
                }
                tMesh.vertices = tVerts;
                tMesh.uv = uv;
                tMesh.RecalculateNormals();
                tMesh.RecalculateBounds();

                //Save:
                if (_isSavingAsset)
                {
                    EngineIntegration.CreateAsset(tMesh, basePath + "/Mesh/Signs/TrafficLightBases/" + assetNameAsset);
                }
            }

            BoxCollider BC = tObj.AddComponent<BoxCollider>();
            float MaxHeight = MF.sharedMesh.bounds.size.z;
            BC.size = new Vector3(0.35f, 0.35f, MaxHeight);
            BC.center = new Vector3(0f, 0f, (MaxHeight / 2f));

            // Todo: Should be moved where tMesh is getting scaled with lanes
            /*
            if (_isOptionalCollider)
            {
                float MaxWidth = MF.sharedMesh.bounds.size.y;
                GameObject tObjCollider = new GameObject("col2");
                BC = tObjCollider.AddComponent<BoxCollider>();
                BC.size = new Vector3(0.175f, MaxWidth, 0.175f);
                // Needs adjustment depending on lanes and turn lanes
                BC.center = new Vector3(0f, MaxWidth / 2f, 5.808f);
                tObjCollider.transform.parent = tObj.transform;
            }
            */

            if (_isLight)
            {
                GameObject yObj;
                yObj = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/TrafficLight.prefab");

                GameObject kObj = (GameObject)GameObject.Instantiate(yObj);
                kObj.transform.position = tObj.transform.position;
                kObj.transform.position += new Vector3(0f, 0f, MaxHeight - 7.6f);
                kObj.transform.parent = tObj.transform;
                kObj.transform.rotation = Quaternion.identity;
                kObj.name = "StreetLight";
            }


            //Bounds calcs:
            MeshFilter[] tMeshes = tObj.GetComponents<MeshFilter>();
            for (int index = 0; index < tMeshes.Length; index++)
            {
                tMeshes[index].sharedMesh.RecalculateBounds();
            }

            return tObj;
        }


        private static Vector3 TrafficLightBaseGetRotRL(RoadIntersection _intersection, SplineC _spline, float _distFromCorner, bool _isOverridingRegular = false)
        {
            Vector3 rotation = default(Vector3);
            if (!_intersection.isRegularPoleAlignment && !_isOverridingRegular)
            {
                //float dist = ((Vector3.Distance(_intersection.cornerRR, _intersection.cornerRL) / 2f) + _distFromCorner) / _spline.distance;
                float p = Mathf.Clamp(_intersection.node1.time, 0f, 1f);
                rotation = _spline.GetSplineValue(p, true);
                rotation = Vector3.Cross(rotation, Vector3.up);
                return rotation;
            }
            else
            {
                rotation = _intersection.cornerRL - _intersection.cornerLL;
                return rotation * -1;
            }
        }


        private static Vector3 TrafficLightBaseGetRotLR(RoadIntersection _intersection, SplineC _spline, float _distFromCorner, bool _isOverridingRegular = false)
        {
            Vector3 rotation = default(Vector3);
            if (!_intersection.isRegularPoleAlignment && !_isOverridingRegular)
            {
                //float dist = ((Vector3.Distance(_intersection.cornerLR, _intersection.cornerLL) / 2f) + _distFromCorner) / _spline.distance;
                float p = Mathf.Clamp(_intersection.node1.time, 0f, 1f);
                rotation = _spline.GetSplineValue(p, true);
                rotation = Vector3.Cross(rotation, Vector3.up);
                return rotation * -1;
            }
            else
            {
                rotation = _intersection.cornerRR - _intersection.cornerLR;
                return rotation;
            }
        }


        private static Vector3 TrafficLightBaseGetRotRR(RoadIntersection _intersection, SplineC _spline, float _distFromCorner, bool _isOverridingRegular = false)
        {
            Vector3 rotation = default(Vector3);
            if (!_intersection.isRegularPoleAlignment && !_isOverridingRegular)
            {
                //float dist = ((Vector3.Distance(_intersection.cornerRR, _intersection.cornerLR) / 2f) + _distFromCorner) / _spline.distance;
                float p = Mathf.Clamp(_intersection.node2.time, 0f, 1f);
                rotation = _spline.GetSplineValue(p, true);
                rotation = Vector3.Cross(rotation, Vector3.up);
                if (_intersection.isFlipped)
                {
                    rotation = rotation * -1;
                }
            }
            else
            {
                rotation = _intersection.cornerLL - _intersection.cornerLR;
            }
            return rotation;
        }


        private static Vector3 TrafficLightBaseGetRotLL(RoadIntersection _intersection, SplineC _spline, float _distFromCorner, bool _isOverridingRegular = false)
        {
            Vector3 rotation = default(Vector3);
            if (!_intersection.isRegularPoleAlignment && !_isOverridingRegular)
            {
                //float dist = ((Vector3.Distance(_intersection.cornerLL, _intersection.cornerRL) / 2f) + _distFromCorner) / _spline.distance;
                float p = Mathf.Clamp(_intersection.node2.time, 0f, 1f);
                rotation = _spline.GetSplineValue(p, true);
                rotation = Vector3.Cross(rotation, Vector3.up);
                if (_intersection.isFlipped)
                {
                    rotation = rotation * -1;
                }
            }
            else
            {
                rotation = _intersection.cornerRL - _intersection.cornerRR;
            }
            return rotation * -1;
        }
        #endregion


        #region "Traffic light mains"
        private static void CreateTrafficLightMains(GameObject _masterGameObj, GameObject _RR, GameObject _RL, GameObject _LL, GameObject _LR)
        {
            RoadIntersection roadIntersection = _masterGameObj.GetComponent<RoadIntersection>();
            SplineC tSpline = roadIntersection.node1.spline;

            float tDist = (Vector3.Distance(roadIntersection.cornerRL, roadIntersection.cornerRR) / 2f) / tSpline.distance;
            Vector3 tan = tSpline.GetSplineValue(roadIntersection.node1.time + tDist, true);
            ProcessPole(_masterGameObj, _RL, tan * -1, 1, Vector3.Distance(roadIntersection.cornerRL, roadIntersection.cornerRR));
            tDist = (Vector3.Distance(roadIntersection.cornerLR, roadIntersection.cornerLL) / 2f) / tSpline.distance;
            tan = tSpline.GetSplineValue(roadIntersection.node1.time - tDist, true);
            ProcessPole(_masterGameObj, _LR, tan, 3, Vector3.Distance(roadIntersection.cornerLR, roadIntersection.cornerLL));


            float InterDist = Vector3.Distance(roadIntersection.cornerRL, roadIntersection.cornerLL);
            tDist = (InterDist / 2f) / tSpline.distance;
            tan = tSpline.GetSplineValue(roadIntersection.node1.time + tDist, true);

            float fTime1 = roadIntersection.node2.time + tDist;
            float fTime2neg = roadIntersection.node2.time - tDist;

            tSpline = roadIntersection.node2.spline;
            if (roadIntersection.isFlipped)
            {
                tan = tSpline.GetSplineValue(fTime1, true);
                ProcessPole(_masterGameObj, _RR, tan, 0, InterDist);
                tan = tSpline.GetSplineValue(fTime2neg, true);
                ProcessPole(_masterGameObj, _LL, tan * -1, 2, InterDist);
            }
            else
            {
                tan = tSpline.GetSplineValue(fTime2neg, true);
                ProcessPole(_masterGameObj, _RR, tan * -1, 0, InterDist);
                tan = tSpline.GetSplineValue(fTime1, true);
                ProcessPole(_masterGameObj, _LL, tan, 2, InterDist);
            }

            // Delete objects on specific corners
            if (roadIntersection.ignoreCorner == 0)
            {
                Object.DestroyImmediate(_RR);
            }
            else if (roadIntersection.ignoreCorner == 1)
            {
                Object.DestroyImmediate(_RL);
            }
            else if (roadIntersection.ignoreCorner == 2)
            {
                Object.DestroyImmediate(_LL);
            }
            else if (roadIntersection.ignoreCorner == 3)
            {
                Object.DestroyImmediate(_LR);
            }
        }


        private static void AdjustLightPrefab(GameObject _light)
        {
            string basePath = RoadEditorUtility.GetBasePath();

            foreach (Light light in _light.GetComponentsInChildren<Light>())
            {
                if (light.type == LightType.Point)
                {
                    light.flare = EngineIntegration.LoadAssetFromPath<Flare>(basePath + "/Flares/SodiumBulb.flare");
                }
            }
        }


        /// <summary> Creates traffic lights </summary>
        private static void ProcessPole(GameObject _masterGameObj, GameObject _obj, Vector3 _tan, int _corner, float _interDist)
        {
            RoadIntersection intersection = _masterGameObj.GetComponent<RoadIntersection>();
            SplineC spline = intersection.node1.spline;

            //float RoadWidth = tSpline.road.RoadWidth();
            float LaneWidth = spline.road.laneWidth;
            float ShoulderWidth = spline.road.shoulderWidth;

            int Lanes = spline.road.laneAmount;
            int LanesHalf = Lanes / 2;
            float LanesForInter = -1;
            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                LanesForInter = LanesHalf + 1f;
            }
            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
            {
                LanesForInter = LanesHalf + 1f;
            }
            else
            {
                LanesForInter = LanesHalf;
            }
            float DistFromCorner = (ShoulderWidth * 0.35f);
            float TLDistance = (LanesForInter * LaneWidth) + DistFromCorner;

            MeshFilter MF = _obj.GetComponent<MeshFilter>();
            Mesh tMesh = MF.sharedMesh;
            Vector3[] tVerts = tMesh.vertices;
            Vector3 StartVec = new Vector3(0f, 0.5f, 5.8f);
            Vector3 EndVec = new Vector3(0f, tMesh.bounds.extents.y * 2, 5.8f);


            StartVec = (((EndVec - StartVec) * (DistFromCorner / TLDistance)) + StartVec);
            Vector3 tLanePosR = StartVec - new Vector3(0f, 1.6f, 0.5f);

            float SmallerDist = Vector3.Distance(StartVec, EndVec);

            //StartVec = Corner
            //2.5m = lane
            //7.5m = lane
            //12.5m = lane
            Vector3[] tLanePos = new Vector3[LanesHalf];
            for (int index = 0; index < LanesHalf; index++)
            {
                tLanePos[index] = (((EndVec - StartVec) * (((LaneWidth * 0.5f) + (index * LaneWidth)) / SmallerDist)) + StartVec);
            }
            Vector3 tLanePosL = default(Vector3);
            Vector3 tLanePosL_Sign = default(Vector3);

            if (intersection.isLeftTurnYieldOnGreen)
            {
                tLanePosL = ((EndVec - StartVec) * ((SmallerDist - 1.8f) / SmallerDist)) + StartVec;
                tLanePosL_Sign = ((EndVec - StartVec) * ((SmallerDist - 3.2f) / SmallerDist)) + StartVec;
            }
            else
            {
                tLanePosL = ((EndVec - StartVec) * ((SmallerDist - 2.5f) / SmallerDist)) + StartVec;
            }

            Vector3 tPos1 = default(Vector3);
            if (_corner == 0)
            {
                //RR
                tPos1 = intersection.cornerLR;
            }
            else if (_corner == 1)
            {
                //RL
                tPos1 = intersection.cornerRR;
            }
            else if (_corner == 2)
            {
                //LL
                tPos1 = intersection.cornerRL;
            }
            else if (_corner == 3)
            {
                //LR
                tPos1 = intersection.cornerLL;
            }

            int mCount = tLanePos.Length;
            float mDistance = -50000f;
            float tDistance = 0f;
            for (int index = 0; index < mCount; index++)
            {
                tDistance = Vector3.Distance(_obj.transform.TransformPoint(tLanePos[index]), tPos1);
                if (tDistance > mDistance)
                {
                    mDistance = tDistance;
                }
            }
            tDistance = Vector3.Distance(_obj.transform.TransformPoint(tLanePosL), tPos1);
            if (tDistance > mDistance)
            {
                mDistance = tDistance;
            }
            tDistance = Vector3.Distance(_obj.transform.TransformPoint(tLanePosR), tPos1);
            if (tDistance > mDistance)
            {
                mDistance = tDistance;
            }

            float tScaleSense = ((200f - intersection.scalingSense) / 200f) * 200f;
            tScaleSense = Mathf.Clamp(tScaleSense * 0.1f, 0f, 20f);
            float ScaleMod = ((mDistance / 17f) + tScaleSense) * (1f / (tScaleSense + 1f));
            if (RootUtils.IsApproximately(tScaleSense, 20f, 0.05f))
            {
                ScaleMod = 1f;
            }
            ScaleMod = Mathf.Clamp(ScaleMod, 1f, 1.5f);
            Vector3 tScale = new Vector3(ScaleMod, ScaleMod, ScaleMod);
            bool bScale = true;
            if (RootUtils.IsApproximately(ScaleMod, 1f, 0.001f))
            {
                bScale = false;
            }

            //Debug.Log (mDistance + " " + ScaleMod + " " + tScaleSense);

            GameObject tRight = null;
            GameObject tLeft = null;
            GameObject tLeft_Sign = null;
            Object prefab = null;

            MeshRenderer MR_Left = null;
            MeshRenderer MR_Right = null;
            MeshRenderer[] MR_Mains = new MeshRenderer[LanesHalf];
            int cCount = -1;

            string basePath = RoadEditorUtility.GetBasePath();

            if (intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                if (intersection.isLeftTurnYieldOnGreen)
                {
                    prefab = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/TrafficLightLeftYield.prefab");
                }
                else
                {
                    prefab = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/TrafficLightLeft.prefab");
                }


                tLeft = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                AdjustLightPrefab(tLeft);
                tLeft.transform.position = _obj.transform.TransformPoint(tLanePosL);
                tLeft.transform.rotation = Quaternion.LookRotation(_tan) * Quaternion.Euler(0f, 90f, 0f);
                tLeft.transform.parent = _obj.transform;
                tLeft.transform.name = "LightLeft";

                cCount = tLeft.transform.childCount;
                for (int index = 0; index < cCount; index++)
                {
                    if (tLeft.transform.GetChild(index).name.ToLower() == "lights")
                    {
                        MR_Left = tLeft.transform.GetChild(index).GetComponent<MeshRenderer>();
                    }
                }

                if (bScale)
                {
                    tLeft.transform.localScale = tScale;
                }

                if (intersection.isLeftTurnYieldOnGreen)
                {
                    prefab = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/YieldOnGreenSign.prefab");

                    tLeft_Sign = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                    tLeft_Sign.transform.position = _obj.transform.TransformPoint(tLanePosL_Sign);
                    tLeft_Sign.transform.rotation = Quaternion.LookRotation(_tan) * Quaternion.Euler(-90f, 90f, 0f);
                    tLeft_Sign.transform.parent = _obj.transform;
                    tLeft_Sign.transform.name = "SignYieldOnGreen";
                    if (bScale)
                    {
                        tLeft_Sign.transform.localScale = tScale;
                    }
                }
            }

            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                prefab = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/TrafficLightRight.prefab");

                tRight = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                AdjustLightPrefab(tRight);
                tRight.transform.position = _obj.transform.TransformPoint(tLanePosR);
                tRight.transform.rotation = Quaternion.LookRotation(_tan) * Quaternion.Euler(0f, 90f, 0f);
                tRight.transform.parent = _obj.transform;
                tRight.transform.name = "LightRight";
                if (bScale)
                {
                    tRight.transform.localScale = tScale;
                }

                cCount = tRight.transform.childCount;
                for (int index = 0; index < cCount; index++)
                {
                    if (tRight.transform.GetChild(index).name.ToLower() == "lights")
                    {
                        MR_Right = tRight.transform.GetChild(index).GetComponent<MeshRenderer>();
                    }
                }
            }

            GameObject[] tLanes = new GameObject[LanesHalf];
            prefab = EngineIntegration.LoadAssetFromPath<GameObject>(basePath + "/Prefabs/Signs/TrafficLightMain.prefab");
            for (int index = 0; index < LanesHalf; index++)
            {
                tLanes[index] = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
                AdjustLightPrefab(tLanes[index]);
                tLanes[index].transform.position = _obj.transform.TransformPoint(tLanePos[index]);
                tLanes[index].transform.rotation = Quaternion.LookRotation(_tan) * Quaternion.Euler(0f, 90f, 0f);
                tLanes[index].transform.parent = _obj.transform;
                tLanes[index].transform.name = "Light" + index.ToString();
                if (bScale)
                {
                    tLanes[index].transform.localScale = tScale;
                }

                cCount = tLanes[index].transform.childCount;
                for (int j = 0; j < cCount; j++)
                {
                    if (tLanes[index].transform.GetChild(j).name.ToLower() == "lights")
                    {
                        MR_Mains[index] = tLanes[index].transform.GetChild(j).GetComponent<MeshRenderer>();
                    }
                }
            }

            TrafficLightController LM = new TrafficLightController(ref tLeft, ref tRight, ref tLanes, ref MR_Left, ref MR_Right, ref MR_Mains);
            if (_corner == 0)
            {
                intersection.lightsRR = LM;
            }
            else if (_corner == 1)
            {
                intersection.lightsRL = LM;
            }
            else if (_corner == 2)
            {
                intersection.lightsLL = LM;
            }
            else if (_corner == 3)
            {
                intersection.lightsLR = LM;
            }
        }
        #endregion


        /// <summary> Calculates and assigns the corner positions of the intersections </summary>
        public static void GetFourPoints(RoadIntersection _roadIntersection, out Vector3 _posRR, out Vector3 _posRL, out Vector3 _posLL, out Vector3 _posLR, float _distFromCorner)
        {
            //Get four points:
            float pos1 = 0f;
            float pos2 = 0f;
            Vector3 tan1 = default(Vector3);
            Vector3 tan2 = default(Vector3);
            float node1Width = -1f;
            float node2Width = -1f;
            Vector3 vectorRR = _roadIntersection.cornerRR;
            Vector3 vectorRL = _roadIntersection.cornerRL;
            Vector3 vectorLR = _roadIntersection.cornerLR;
            Vector3 vectorLL = _roadIntersection.cornerLL;
            Vector3 direction = default(Vector3);
            float shoulderWidth1 = _roadIntersection.node1.spline.road.shoulderWidth;
            float shoulderWidth2 = _roadIntersection.node2.spline.road.shoulderWidth;

            if (!_roadIntersection.isFlipped)
            {
                //RR:
                node1Width = (Vector3.Distance(vectorRR, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorRR, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time - node1Width;
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true) * -1f;
                pos2 = _roadIntersection.node2.time + node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true);
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posRR = vectorRR + (direction * _distFromCorner);
                //RL:
                node1Width = (Vector3.Distance(vectorRL, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorRL, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time + node1Width;
                if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
                {
                    pos1 = _roadIntersection.node1.time;
                }
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true);
                pos2 = _roadIntersection.node2.time + node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true);
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posRL = vectorRL + (direction * _distFromCorner);
                //LL:
                node1Width = (Vector3.Distance(vectorLL, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorLL, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time + node1Width;
                if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
                {
                    pos1 = _roadIntersection.node1.time;
                }
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true);
                pos2 = _roadIntersection.node2.time - node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true) * -1f;
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posLL = vectorLL + (direction * _distFromCorner);
                //LR:
                node1Width = (Vector3.Distance(vectorLR, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorLR, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time - node1Width;
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true) * -1f;
                pos2 = _roadIntersection.node2.time - node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true) * -1f;
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posLR = vectorLR + (direction * _distFromCorner);
            }
            else
            {
                //RR:
                node1Width = (Vector3.Distance(vectorRR, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorRR, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time - node1Width;
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true) * -1f;
                pos2 = _roadIntersection.node2.time - node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true) * -1f;
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posRR = vectorRR + (direction * _distFromCorner);
                //RL:
                node1Width = (Vector3.Distance(vectorRL, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorRL, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time + node1Width;
                if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
                {
                    pos1 = _roadIntersection.node1.time;
                }
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true);
                pos2 = _roadIntersection.node2.time - node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true) * -1f;
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posRL = vectorRL + (direction * _distFromCorner);
                //LL:
                node1Width = (Vector3.Distance(vectorLL, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorLL, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time + node1Width;
                if (_roadIntersection.intersectionType == RoadIntersection.IntersectionTypeEnum.ThreeWay)
                {
                    pos1 = _roadIntersection.node1.time;
                }
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true);
                pos2 = _roadIntersection.node2.time + node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true);
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posLL = vectorLL + (direction * _distFromCorner);
                //LR:
                node1Width = (Vector3.Distance(vectorLR, _roadIntersection.node1.pos) + shoulderWidth1) / _roadIntersection.node1.spline.distance;
                node2Width = (Vector3.Distance(vectorLR, _roadIntersection.node2.pos) + shoulderWidth2) / _roadIntersection.node2.spline.distance;
                pos1 = _roadIntersection.node1.time - node1Width;
                tan1 = _roadIntersection.node1.spline.GetSplineValue(pos1, true) * -1f;
                pos2 = _roadIntersection.node2.time + node2Width;
                tan2 = _roadIntersection.node2.spline.GetSplineValue(pos2, true);
                direction = (tan1.normalized + tan2.normalized).normalized;
                _posLR = vectorLR + (direction * _distFromCorner);
            }


            // Todo: Remove this limitation in future
            // Prevent corners of intersections to have different heights
            _posRR.y = _roadIntersection.signHeight;
            _posRL.y = _roadIntersection.signHeight;
            _posLL.y = _roadIntersection.signHeight;
            _posLR.y = _roadIntersection.signHeight;

            //DebugFourPoints(_posRR, _posRL, _posLL, _posLR);
        }


        /// <summary> Creates dummy cubes. Does not work with multi threading </summary>
        private static void DebugFourPoints(Vector3 _posRR, Vector3 _posRL, Vector3 _posLL, Vector3 _posLR)
        {
            GameObject tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tObj.transform.localScale = new Vector3(0.2f, 20f, 0.2f);
            tObj.transform.name = "temp22_RR";
            tObj.transform.position = _posRR;
            tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tObj.transform.localScale = new Vector3(0.2f, 20f, 0.2f);
            tObj.transform.name = "temp22_RL";
            tObj.transform.position = _posRL;
            tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tObj.transform.localScale = new Vector3(0.2f, 20f, 0.2f);
            tObj.transform.name = "temp22_LL";
            tObj.transform.position = _posLL;
            tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tObj.transform.localScale = new Vector3(0.2f, 20f, 0.2f);
            tObj.transform.name = "temp22_LR";
            tObj.transform.position = _posLR;
        }


        /*
        public static void GetOrigFour(RoadIntersection roadIntersection, out Vector3 tPosRR, out Vector3 tPosRL, out Vector3 tPosLL, out Vector3 tPosLR)
        {
            //Get four points:
            float tPos1 = 0f;
            float tPos2 = 0f;
            Vector3 tTan1 = default(Vector3);
            Vector3 tTan2 = default(Vector3);
            float Node1Width = -1f;
            float Node2Width = -1f;
            Vector3 tDirRR = default(Vector3);
            Vector3 tDirRL = default(Vector3);
            Vector3 tDirLL = default(Vector3);
            Vector3 tDirLR = default(Vector3);
            float tAngleRR = 85f;
            float tAngleRL = 85f;
            float tAngleLL = 85f;
            float tAngleLR = 85f;
            float ShoulderWidth1 = roadIntersection.Node1.spline.road.opt_ShoulderWidth;
            float ShoulderWidth2 = roadIntersection.Node2.spline.road.opt_ShoulderWidth;
            Vector3 xPos1 = default(Vector3);
            Vector3 xPos2 = default(Vector3);

            if(!roadIntersection.bFlipped)
            {
                //RR:
                Node1Width = (Vector3.Distance(roadIntersection.CornerRR,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerRR,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime - Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true) * -1f;
                tPos2 = roadIntersection.Node2.tTime + Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tDirRR = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleRR = Vector3.Angle(tTan1,tTan2);
                tAngleRR = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //RL:
                Node1Width = (Vector3.Distance(roadIntersection.CornerRL,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerRL,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime + Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true);
                tPos2 = roadIntersection.Node2.tTime + Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tDirRL = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleRL = Vector3.Angle(tTan1,tTan2);
                tAngleRL = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //LL:
                Node1Width = (Vector3.Distance(roadIntersection.CornerLL,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerLL,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime + Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true);
                tPos2 = roadIntersection.Node2.tTime - Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true) * -1f;
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tDirLL = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleLL = Vector3.Angle(tTan1,tTan2);
                tAngleLL = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //LR:
                Node1Width = (Vector3.Distance(roadIntersection.CornerLR,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerLR,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime - Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true) * -1f;
                tPos2 = roadIntersection.Node2.tTime - Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true) * -1f;
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tDirLR = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleLR = Vector3.Angle(tTan1,tTan2);
                tAngleLR = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
            }
            else
            {
                //RR:
                Node1Width = (Vector3.Distance(roadIntersection.CornerRR,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerRR,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime - Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true) * -1f;
                tPos2 = roadIntersection.Node2.tTime - Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true) * -1f;
                tDirRR = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleRR = Vector3.Angle(tTan1,tTan2);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tAngleRR = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //RL:
                Node1Width = (Vector3.Distance(roadIntersection.CornerRL,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerRL,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime + Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true);
                tPos2 = roadIntersection.Node2.tTime - Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true) * -1f;
                tDirRL = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleRL = Vector3.Angle(tTan1,tTan2);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tAngleRL = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //LL:
                Node1Width = (Vector3.Distance(roadIntersection.CornerLL,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerLL,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime + Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true);
                tPos2 = roadIntersection.Node2.tTime + Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true);
                tDirLL = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleLL = Vector3.Angle(tTan1,tTan2);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tAngleLL = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
                //LR:
                Node1Width = (Vector3.Distance(roadIntersection.CornerLR,roadIntersection.Node1.pos) + ShoulderWidth1)/roadIntersection.Node1.spline.distance;
                Node2Width = (Vector3.Distance(roadIntersection.CornerLR,roadIntersection.Node2.pos) + ShoulderWidth2)/roadIntersection.Node2.spline.distance;
                tPos1 = roadIntersection.Node1.tTime - Node1Width;
                tTan1 = roadIntersection.Node1.spline.GetSplineValue(tPos1,true) * -1f;
                tPos2 = roadIntersection.Node2.tTime + Node2Width;
                tTan2 = roadIntersection.Node2.spline.GetSplineValue(tPos2,true);
                tDirLR = (tTan1.normalized + tTan2.normalized).normalized;
                //tAngleLR = Vector3.Angle(tTan1,tTan2);
                xPos1 = roadIntersection.Node1.spline.GetSplineValue(tPos1);
                xPos2 = roadIntersection.Node1.spline.GetSplineValue(tPos2);
                tAngleLR = Vector3.Angle(xPos1 - roadIntersection.Node1.pos,xPos2 - roadIntersection.Node1.pos);
            }	
        		
            //D = B*cos(angle/2)
            float tWidth = roadIntersection.Node1.spline.road.opt_RoadWidth * 0.5f;
            float tAngleRR_Opp = 180f - tAngleRR;
            float tAngleRL_Opp = 180f - tAngleRL;
            float tAngleLL_Opp = 180f - tAngleLL;
            float tAngleLR_Opp = 180f - tAngleLR;
        
            float tOffSetRR = tWidth*(Mathf.Cos((tAngleRR*0.5f)*Mathf.Deg2Rad));
            float tOffSetRL = tWidth*(Mathf.Cos((tAngleRL*0.5f)*Mathf.Deg2Rad));
            float tOffSetLL = tWidth*(Mathf.Cos((tAngleLL*0.5f)*Mathf.Deg2Rad));
            float tOffSetLR = tWidth*(Mathf.Cos((tAngleLR*0.5f)*Mathf.Deg2Rad));
        		
            float tOffSetRR_opp = tWidth*(Mathf.Cos((tAngleRR_Opp*0.5f)*Mathf.Deg2Rad));
            float tOffSetRL_opp = tWidth*(Mathf.Cos((tAngleRL_Opp*0.5f)*Mathf.Deg2Rad));
            float tOffSetLL_opp = tWidth*(Mathf.Cos((tAngleLL_Opp*0.5f)*Mathf.Deg2Rad));
            float tOffSetLR_opp = tWidth*(Mathf.Cos((tAngleLR_Opp*0.5f)*Mathf.Deg2Rad));
        	
            Vector3 tPos = roadIntersection.Node1.pos;
        			
            //tOffSetRR *=2f;
            //tOffSetRL *=2f;
            //tOffSetLL *=2f;
            //tOffSetLR *=2f;
        		
            tPosRR = tPos + (tDirRR * (tOffSetRR+tOffSetRR_opp));
            tPosRL = tPos + (tDirRL * (tOffSetRL+tOffSetRL_opp));
            tPosLL = tPos + (tDirLL * (tOffSetLL+tOffSetLL_opp));
            tPosLR = tPos + (tDirLR * (tOffSetLR+tOffSetLR_opp));
        			
            GameObject tObj = GameObject.Find("temp22_RR");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_RL");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_LL");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_LR");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
        		
            GameObject tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosRR;
            tCubeRR.transform.name = "temp22_RR";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
    			
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosRL;
            tCubeRR.transform.name = "temp22_RL";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
   			
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosLL;
            tCubeRR.transform.name = "temp22_LL";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
       		
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosLR;
            tCubeRR.transform.name = "temp22_LR";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
        }
  	    

        public static void GetCornerVectors_Test(RoadIntersection roadIntersection, out Vector3 tPosRR, out Vector3 tPosRL, out Vector3 tPosLL, out Vector3 tPosLR)
        {
            SplineN tNode = null;
            SplineC tSpline = null;
      
            tNode = roadIntersection.Node1;
            tSpline = tNode.spline;
            float tOffset = tSpline.road.opt_RoadWidth * 0.5f;
            float tPos1 = tNode.tTime - (tOffset/tSpline.distance);
            float tPos2 = tNode.tTime + (tOffset/tSpline.distance);
            Vector3 tVect1 = tSpline.GetSplineValue(tPos1);	
            Vector3 POS1 = tSpline.GetSplineValue(tPos1,true);
            Vector3 tVect2 = tSpline.GetSplineValue(tPos2);	
            Vector3 POS2 = tSpline.GetSplineValue(tPos2,true);
            tPosRR = (tVect1 + new Vector3(5f*POS1.normalized.z,0,5f*-POS1.normalized.x));
            tPosLR = (tVect1 + new Vector3(5f*-POS1.normalized.z,0,5f*POS1.normalized.x));
            tPosRL = (tVect2 + new Vector3(5f*POS2.normalized.z,0,5f*-POS2.normalized.x));
            tPosLL = (tVect2 + new Vector3(5f*-POS2.normalized.z,0,5f*POS2.normalized.x));
       		
            tNode = roadIntersection.Node2;
            tSpline = tNode.spline;
            tOffset = tSpline.road.opt_RoadWidth * 0.5f;
            tPos1 = tNode.tTime - (tOffset/tSpline.distance);
            tPos2 = tNode.tTime + (tOffset/tSpline.distance);
            tVect1 = tSpline.GetSplineValue(tPos1);	
            POS1 = tSpline.GetSplineValue(tPos1,true);
            tVect2 = tSpline.GetSplineValue(tPos2);	
            POS2 = tSpline.GetSplineValue(tPos2,true);
            Vector3 tPosRR2 = default(Vector3);
            Vector3 tPosLR2 = default(Vector3);
            Vector3 tPosRL2 = default(Vector3);
            Vector3 tPosLL2 = default(Vector3);
       		
            if(roadIntersection.bFlipped)
            {
                tPosRL2 = (tVect1 + new Vector3(5f*POS1.normalized.z,0,5f*-POS1.normalized.x));
                tPosRR2 = (tVect1 + new Vector3(5f*-POS1.normalized.z,0,5f*POS1.normalized.x));
                tPosLL2 = (tVect2 + new Vector3(5f*POS2.normalized.z,0,5f*-POS2.normalized.x));
                tPosLR2 = (tVect2 + new Vector3(5f*-POS2.normalized.z,0,5f*POS2.normalized.x));
            }
            else
            {
                tPosLR2 = (tVect1 + new Vector3(5f*POS1.normalized.z,0,5f*-POS1.normalized.x));
                tPosLL2 = (tVect1 + new Vector3(5f*-POS1.normalized.z,0,5f*POS1.normalized.x));
                tPosRR2 = (tVect2 + new Vector3(5f*POS2.normalized.z,0,5f*-POS2.normalized.x));
                tPosRL2 = (tVect2 + new Vector3(5f*-POS2.normalized.z,0,5f*POS2.normalized.x));
            }
        			
            tPosRR = ((tPosRR-tPosRR2)*0.5f)+tPosRR;
            tPosLR = ((tPosLR-tPosLR2)*0.5f)+tPosLR;
            tPosRL = ((tPosRL-tPosRL2)*0.5f)+tPosRL;
            tPosLL = ((tPosLL-tPosLL2)*0.5f)+tPosLL;
  			
		
            GameObject tObj = GameObject.Find("temp22_RR");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_RL");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_LL");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
            tObj = GameObject.Find("temp22_LR");
            if(tObj != null)
            {
                Object.DestroyImmediate(tObj);
            }
        		
            GameObject tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosRR;
            tCubeRR.transform.name = "temp22_RR";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
        			
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosRL;
            tCubeRR.transform.name = "temp22_RL";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
        		
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosLL;
            tCubeRR.transform.name = "temp22_LL";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
        		
            tCubeRR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tCubeRR.transform.position = tPosLR;
            tCubeRR.transform.name = "temp22_LR";
            tCubeRR.transform.localScale = new Vector3(0.2f,20f,0.2f);
        }
        */
    }
}