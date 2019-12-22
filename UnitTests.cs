#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using GSD;
using GSD.Roads;
#endregion


namespace RoadArchitect.Tests
{
#if UNITY_EDITOR
    public static class UnitTests
    {
        private static GSDRoadSystem roadSystem;


        /// <summary> WARNING: Only call this on an empty scene that has some terrains on it. MicroGSD LLC is not responsbile for data loss if this function is called by user. </summary>
        public static void RoadArchitectUnitTests()
        {
            CleanupTests();

            //Create new road system and turn off updates:
            GameObject tRoadSystemObj = new GameObject("RoadArchitectSystem1");
            //Add road system component.
            roadSystem = tRoadSystemObj.AddComponent<GSDRoadSystem>();
            roadSystem.isAllowingRoadUpdates = false;

            int numTests = 6;
            double totalTestTime = 0f;
            for (int index = 1; index <= numTests; index++)
            {
                UnityEngine.Debug.Log("Running test " + index);
                double testTime = RunTest(index);
                totalTestTime += testTime;
                UnityEngine.Debug.Log("Test " + index + " complete.  Test time: " + testTime + "ms");
            }
            UnityEngine.Debug.Log("All tests completed.  Total test time: " + totalTestTime + "ms");

            //Turn updates back on and update road:
            roadSystem.isAllowingRoadUpdates = true;
            roadSystem.UpdateAllRoads();
        }


        private static long RunTest(int _testID)
        {
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            switch (_testID)
            {
                case 1:
                    //Bridges
                    RoadArchitectUnitTest1();
                    break;
                case 2:
                    //2L intersections
                    RoadArchitectUnitTest2();
                    break;
                case 3:
                    //4L intersections
                    RoadArchitectUnitTest3();
                    break;
                case 4:
                    //Large suspension bridge
                    RoadArchitectUnitTest4();
                    break;
                case 5:
                    //Long road:
                    RoadArchitectUnitTest5();
                    break;
                case 6:
                    //Very Long road:
                    RoadArchitectUnitTest6();
                    break;
            }
            stopwatch.Stop();
            long testTime = stopwatch.ElapsedMilliseconds;
            return testTime;
        }


        public static void CleanupTests()
        {
            Debug.Log("Cleaning up tests");
            //Get the existing road system, if it exists:
            GameObject GSDRS = (GameObject) GameObject.Find("RoadArchitectSystem1");
            DestroyTerrainHistory(GSDRS);
            Object.DestroyImmediate(GSDRS);
            FlattenTerrains();
        }


        private static void DestroyTerrainHistory(GameObject _roadSystem)
        {
            //Destroy the terrain histories:
            if (_roadSystem != null)
            {
                Object[] roads = _roadSystem.GetComponents<GSDRoad>();
                foreach (GSDRoad road in roads)
                {
                    GSD.Roads.GSDTerraforming.TerrainsReset(road);
                }
            }
        }


        private static void FlattenTerrains()
        {
            //Reset all terrains to 0,0
            Object[] allTerrains = Object.FindObjectsOfType<Terrain>();
            foreach (Terrain terrain in allTerrains)
            {
                terrain.terrainData.SetHeights(0, 0, new float[513, 513]);
            }
        }


        private static void RoadArchitectUnitTest1()
        {
            //Create node locations:
            List<Vector3> nodeLocations = new List<Vector3>();
            int maxCount = 18;
            float mod = 100f;
            Vector3 vector = new Vector3(50f, 40f, 50f);
            for (int index = 0; index < maxCount; index++)
            {
                //tLocs.Add(xVect + new Vector3(tMod * Mathf.Pow((float)i / ((float)MaxCount * 0.15f), 2f), 1f*((float)i*1.25f), tMod * i));
                nodeLocations.Add(vector + new Vector3(mod * Mathf.Pow((float) index / ((float) 25 * 0.15f), 2f), 0f, mod * index));
            }

            //Get road system create road:
            GSDRoad tRoad = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Bridge0: (Arch)
            tRoad.spline.nodes[4].isBridgeStart = true;
            tRoad.spline.nodes[4].isBridgeMatched = true;
            tRoad.spline.nodes[7].isBridgeEnd = true;
            tRoad.spline.nodes[7].isBridgeMatched = true;
            tRoad.spline.nodes[4].bridgeCounterpartNode = tRoad.spline.nodes[7];
            tRoad.spline.nodes[4].LoadWizardObjectsFromLibrary("Arch12m-2L", true, true);

            //Bridge1: (Federal causeway)
            tRoad.spline.nodes[8].isBridgeStart = true;
            tRoad.spline.nodes[8].isBridgeMatched = true;
            tRoad.spline.nodes[8].bridgeCounterpartNode = tRoad.spline.nodes[10];
            tRoad.spline.nodes[8].LoadWizardObjectsFromLibrary("Causeway1-2L", true, true);
            tRoad.spline.nodes[10].isBridgeEnd = true;
            tRoad.spline.nodes[10].isBridgeMatched = true;

            //Bridge2: (Steel)
            tRoad.spline.nodes[11].isBridgeStart = true;
            tRoad.spline.nodes[11].isBridgeMatched = true;
            tRoad.spline.nodes[11].bridgeCounterpartNode = tRoad.spline.nodes[13];
            tRoad.spline.nodes[11].LoadWizardObjectsFromLibrary("Steel-2L", true, true);
            tRoad.spline.nodes[13].isBridgeEnd = true;
            tRoad.spline.nodes[13].isBridgeMatched = true;

            //Bridge3: (Causeway)
            tRoad.spline.nodes[14].isBridgeStart = true;
            tRoad.spline.nodes[14].isBridgeMatched = true;
            tRoad.spline.nodes[16].isBridgeEnd = true;
            tRoad.spline.nodes[16].isBridgeMatched = true;
            tRoad.spline.nodes[14].bridgeCounterpartNode = tRoad.spline.nodes[16];
            tRoad.spline.nodes[14].LoadWizardObjectsFromLibrary("Causeway4-2L", true, true);
        }


        /// <summary> Create 2L intersections: </summary>
        private static void RoadArchitectUnitTest2()
        {
            //Create node locations:
            float StartLocX = 800f;
            float StartLocY = 200f;
            float StartLocYSep = 200f;
            float tHeight = 20f;
            GSDRoad road1 = null;
            GSDRoad road2 = null;

            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 9; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX + (index * 200f), tHeight, 600f));
            }
            road1 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Get road system, create road #1:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX, tHeight, StartLocY + (index * StartLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #2:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX + (StartLocYSep * 2f), tHeight, StartLocY + (index * StartLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.TurnLane);

            //Get road system, create road #3:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX + (StartLocYSep * 4f), tHeight, StartLocY + (index * StartLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.BothTurnLanes);

            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX + (StartLocYSep * 6f), tHeight, StartLocY + (index * StartLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.TurnLane);

            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(StartLocX + (StartLocYSep * 8f), tHeight, StartLocY + (index * StartLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.TurnLane);

            GSDRoadAutomation.CreateIntersectionsProgrammaticallyForRoad(road1, GSDRoadIntersection.iStopTypeEnum.None, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Now count road intersections, if not 5 throw error
            int iCount = 0;
            foreach (GSDSplineN tNode in road1.spline.nodes)
            {
                if (tNode.isIntersection)
                {
                    iCount += 1;
                }
            }
            if (iCount != 5)
            {
                Debug.LogError("Unit Test #2 failed: " + iCount.ToString() + " intersections instead of 5.");
            }
        }


        /// <summary> This will create an intersection if two nodes overlap on the road. Only good if the roads only overlap once. </summary>
        private static void UnitTestIntersectionHelper(GSDRoad _road1, GSDRoad _road2, GSDRoadIntersection.iStopTypeEnum _iStopType, GSDRoadIntersection.RoadTypeEnum _roadType)
        {
            GSDSplineN nodeInter1 = null;
            GSDSplineN nodeInter2 = null;
            foreach (GSDSplineN node in _road1.spline.nodes)
            {
                foreach (GSDSplineN xNode in _road2.spline.nodes)
                {
                    if (GSDRootUtil.IsApproximately(Vector3.Distance(node.transform.position, xNode.transform.position), 0f, 0.05f))
                    {
                        nodeInter1 = node;
                        nodeInter2 = xNode;
                        break;
                    }
                }
            }

            if (nodeInter1 != null && nodeInter2 != null)
            {
                GameObject tInter = GSD.Roads.GSDIntersections.CreateIntersection(nodeInter1, nodeInter2);
                GSDRoadIntersection GSDRI = tInter.GetComponent<GSDRoadIntersection>();
                GSDRI.intersectionStopType = _iStopType;
                GSDRI.roadType = _roadType;
            }
        }


        /// <summary> Create 4L intersections: </summary>
        private static void RoadArchitectUnitTest3()
        {
            //Create node locations:
            float startLocX = 200f;
            float startLocY = 2500f;
            float startLocYSep = 300f;
            float height = 20f;
            GSDRoad road1;
            GSDRoad road2;

            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 9; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (index * startLocYSep), height, startLocY + (startLocYSep * 2f)));
            }
            road1 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road1.laneAmount = 4;


            //Get road system, create road #1:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX, height, startLocY + (index * startLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #2:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 2f), height, startLocY + (index * startLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #3:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 4f), height, startLocY + (index * startLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 6f), height, startLocY + (index * startLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #5:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 8f), height, startLocY + (index * startLocYSep)));
            }
            road2 = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, GSDRoadIntersection.iStopTypeEnum.TrafficLight1, GSDRoadIntersection.RoadTypeEnum.NoTurnLane);

            //Now count road intersections, if not 5 throw error
            int iCount = 0;
            foreach (GSDSplineN node in road1.spline.nodes)
            {
                if (node.isIntersection)
                {
                    iCount += 1;
                }
            }
            if (iCount != 5)
            {
                Debug.LogError("Unit Test #3 failed: " + iCount.ToString() + " intersections instead of 5.");
            }
        }


        //Large suspension bridge:
        private static void RoadArchitectUnitTest4()
        {
            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(3500f, 90f, 200f + (800f * index)));
            }
            GSDRoad tRoad = GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Suspension bridge:
            tRoad.spline.nodes[1].isBridgeStart = true;
            tRoad.spline.nodes[1].isBridgeMatched = true;
            tRoad.spline.nodes[3].isBridgeEnd = true;
            tRoad.spline.nodes[3].isBridgeMatched = true;
            tRoad.spline.nodes[1].bridgeCounterpartNode = tRoad.spline.nodes[3];
            tRoad.spline.nodes[1].LoadWizardObjectsFromLibrary("SuspL-2L", true, true);
        }


        //Long road
        private static void RoadArchitectUnitTest5()
        {
            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 48; index++)
            {
                nodeLocations.Add(new Vector3(3000f, 40f, 10f + (79f * index)));
            }
            for (int index = 0; index < 35; index++)
            {
                nodeLocations.Add(new Vector3(2900f - (79f * index), 30f, 3960f));
            }
            for (int index = 0; index < 40; index++)
            {
                nodeLocations.Add(new Vector3(30, 30f, 3960f - (79f * index)));
            }
            GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
        }


        //Very Long road
        private static void RoadArchitectUnitTest6()
        {
            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 50; index++)
            {
                nodeLocations.Add(new Vector3(6000f, 40f, 10f + (79f * index)));
            }
            for (int index = 0; index < 50; index++)
            {
                nodeLocations.Add(new Vector3(5900f - (79f * index), 30f, 5950f));
            }
            for (int index = 0; index < 200; index++)
            {
                nodeLocations.Add(new Vector3(-200f, 30f, 5950f - (79f * index)));
            }
            GSDRoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
        }
    }
#endif
}