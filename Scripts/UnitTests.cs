#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using RoadArchitect.Roads;
#endregion


namespace RoadArchitect.Tests
{
#if UNITY_EDITOR
    public static class UnitTests
    {
        private static RoadSystem roadSystem;


        /// <summary> WARNING: You should only call this on an empty scene. The RoadArchitect team is not responsbile for loss of data or work if this function is called by user. </summary>
        public static void RoadArchitectUnitTests()
        {
            CleanupAllTests();

            //Create new road system and turn off updates:
            GameObject roadSystemObject = new GameObject("RoadArchitectSystem1");
            //Add road system component.
            roadSystem = roadSystemObject.AddComponent<RoadSystem>();
            roadSystem.isAllowingRoadUpdates = false;

            int numTests = 9;
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


        public static void RoadArchitectUnitTest1To5()
        {
            CleanupTest(1);

            //Create new road system and turn off updates:
            GameObject roadSystemObject = new GameObject("RoadArchitectSystem1");
            //Add road system component.
            roadSystem = roadSystemObject.AddComponent<RoadSystem>();
            roadSystem.isAllowingRoadUpdates = false;

            int numTests = 5;
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
                case 7:
                    // Double intersection:
                    RoadArchitectUnitTest7();
                    break;
                case 8:
                    // Road loop:
                    RoadArchitectUnitTest8();
                    break;
                case 9:
                    // 3 Intersections with end nodes
                    RoadArchitectUnitTest9();
                    break;
            }
            stopwatch.Stop();
            long testTime = stopwatch.ElapsedMilliseconds;
            return testTime;
        }


        public static void CleanupAllTests()
        {
            Debug.Log("Cleaning up tests");
            CleanupTest(1);

            CleanupTest(6);
            CleanupTest(7);
            CleanupTest(8);
            CleanupTest(9);
        }


        public static void CleanupTest(int _index)
        {
            //Get the existing road system, if it exists:
            GameObject roadSystem = (GameObject)GameObject.Find("RoadArchitectSystem" + _index);
            DestroyTerrainHistory(roadSystem);
            Object.DestroyImmediate(roadSystem);
            FlattenTerrains();
        }


        private static void DestroyTerrainHistory(GameObject _roadSystem)
        {
            //Destroy the terrain histories:
            if (_roadSystem != null)
            {
                Road[] roads = _roadSystem.GetComponents<Road>();
                foreach (Road road in roads)
                {
                    Terraforming.TerrainsReset(road);
                }
            }
        }


        /// <summary> Reset all terrains to 0,0 </summary>
        private static void FlattenTerrains()
        {
            Terrain[] allTerrains = Object.FindObjectsOfType<Terrain>();
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
                nodeLocations.Add(vector + new Vector3(mod * Mathf.Pow((float)index / ((float)25 * 0.15f), 2f), 0f, mod * index));
            }

            //Get road system create road:
            Road testRoad = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Bridge0: (Arch)
            testRoad.spline.nodes[4].isBridgeStart = true;
            testRoad.spline.nodes[4].isBridgeMatched = true;
            testRoad.spline.nodes[7].isBridgeEnd = true;
            testRoad.spline.nodes[7].isBridgeMatched = true;
            testRoad.spline.nodes[4].bridgeCounterpartNode = testRoad.spline.nodes[7];
            testRoad.spline.nodes[4].LoadWizardObjectsFromLibrary("Arch12m-2L", true, true);

            //Bridge1: (Federal causeway)
            testRoad.spline.nodes[8].isBridgeStart = true;
            testRoad.spline.nodes[8].isBridgeMatched = true;
            testRoad.spline.nodes[8].bridgeCounterpartNode = testRoad.spline.nodes[10];
            testRoad.spline.nodes[8].LoadWizardObjectsFromLibrary("Causeway1-2L", true, true);
            testRoad.spline.nodes[10].isBridgeEnd = true;
            testRoad.spline.nodes[10].isBridgeMatched = true;

            //Bridge2: (Steel)
            testRoad.spline.nodes[11].isBridgeStart = true;
            testRoad.spline.nodes[11].isBridgeMatched = true;
            testRoad.spline.nodes[11].bridgeCounterpartNode = testRoad.spline.nodes[13];
            testRoad.spline.nodes[11].LoadWizardObjectsFromLibrary("Steel-2L", true, true);
            testRoad.spline.nodes[13].isBridgeEnd = true;
            testRoad.spline.nodes[13].isBridgeMatched = true;

            //Bridge3: (Causeway)
            testRoad.spline.nodes[14].isBridgeStart = true;
            testRoad.spline.nodes[14].isBridgeMatched = true;
            testRoad.spline.nodes[16].isBridgeEnd = true;
            testRoad.spline.nodes[16].isBridgeMatched = true;
            testRoad.spline.nodes[14].bridgeCounterpartNode = testRoad.spline.nodes[16];
            testRoad.spline.nodes[14].LoadWizardObjectsFromLibrary("Causeway4-2L", true, true);
        }


        /// <summary> Create 2L intersections: </summary>
        private static void RoadArchitectUnitTest2()
        {
            //Create node locations:
            float startLocX = 800f;
            float startLocY = 200f;
            float startLocYSep = 200f;
            float height = 20f;
            Road road1 = null;
            Road road2 = null;

            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 9; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (index * 200f), height, 600f));
            }
            road1 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Get road system, create road #1:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX, height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);

            //Get road system, create road #2:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 2f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.TurnLane);

            //Get road system, create road #3:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 4f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.BothTurnLanes);

            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 6f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.TurnLane);

            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 8f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            //UnitTest_IntersectionHelper(bRoad, tRoad, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.TurnLane);

            RoadAutomation.CreateIntersectionsProgrammaticallyForRoad(road1, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);

            //Now count road intersections, if not 5 throw error
            int intersctionsCount = 0;
            foreach (SplineN node in road1.spline.nodes)
            {
                if (node.isIntersection)
                {
                    intersctionsCount += 1;
                }
            }

            if (intersctionsCount != 5)
            {
                Debug.LogError("Unit Test #2 failed: " + intersctionsCount.ToString() + " intersections instead of 5.");
            }
        }


        /// <summary> This will create an intersection if two nodes overlap on the road. Only good if the roads only overlap once. </summary>
        private static void UnitTestIntersectionHelper(Road _road1, Road _road2, RoadIntersection.iStopTypeEnum _iStopType, RoadIntersection.RoadTypeEnum _roadType)
        {
            SplineN nodeInter1 = null;
            SplineN nodeInter2 = null;

            // Loop through every node of _road1 and _road2
            // Finds the first nodes which are close enough for a intersection
            foreach (SplineN node in _road1.spline.nodes)
            {
                foreach (SplineN node2 in _road2.spline.nodes)
                {
                    if (RootUtils.IsApproximately(Vector3.Distance(node.transform.position, node2.transform.position), 0f, 0.05f))
                    {
                        nodeInter1 = node;
                        nodeInter2 = node2;
                        break;
                    }
                }
            }


            if (nodeInter1 != null && nodeInter2 != null)
            {
                GameObject IntersectionsObject = Intersections.CreateIntersection(nodeInter1, nodeInter2);
                RoadIntersection roadIntersction = IntersectionsObject.GetComponent<RoadIntersection>();
                roadIntersction.intersectionStopType = _iStopType;
                roadIntersction.roadType = _roadType;
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
            Road road1;
            Road road2;

            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();
            for (int index = 0; index < 9; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (index * startLocYSep), height, startLocY + (startLocYSep * 2f)));
            }
            road1 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road1.laneAmount = 4;


            //Get road system, create road #1:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX, height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #2:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 2f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #3:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 4f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #4:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 6f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #5:
            nodeLocations.Clear();
            for (int index = 0; index < 5; index++)
            {
                nodeLocations.Add(new Vector3(startLocX + (startLocYSep * 8f), height, startLocY + (index * startLocYSep)));
            }
            road2 = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
            road2.laneAmount = 4;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.TrafficLight1, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Now count road intersections, if not 5 throw error
            int intersectionsCount = 0;
            foreach (SplineN node in road1.spline.nodes)
            {
                if (node.isIntersection)
                {
                    intersectionsCount += 1;
                }
            }
            if (intersectionsCount != 5)
            {
                Debug.LogError("Unit Test #3 failed: " + intersectionsCount.ToString() + " intersections instead of 5.");
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

            Road testRoad = RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);

            //Suspension bridge:
            testRoad.spline.nodes[1].isBridgeStart = true;
            testRoad.spline.nodes[1].isBridgeMatched = true;
            testRoad.spline.nodes[3].isBridgeEnd = true;
            testRoad.spline.nodes[3].isBridgeMatched = true;
            testRoad.spline.nodes[1].bridgeCounterpartNode = testRoad.spline.nodes[3];
            testRoad.spline.nodes[1].LoadWizardObjectsFromLibrary("SuspL-2L", true, true);
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
            RoadAutomation.CreateRoadProgrammatically(roadSystem, ref nodeLocations);
        }


        /// <summary> Very long road </summary>
        public static void RoadArchitectUnitTest6()
        {
            CleanupTest(6);

            GameObject unitSystem6Object = new GameObject("RoadArchitectSystem6");
            RoadSystem unitSystem6 = unitSystem6Object.AddComponent<RoadSystem>();
            unitSystem6.isAllowingRoadUpdates = false;

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
            for (int index = 0; index < 50; index++)
            {
                nodeLocations.Add(new Vector3(-200f, 30f, 5950f - (79f * index)));
            }
            for (int index = 0; index < 50; index++)
            {
                nodeLocations.Add(new Vector3(-200f + (79f * index), 30f, 10f));
            }
            nodeLocations.Add(new Vector3(5995f, 40f, 10f));
            RoadAutomation.CreateRoadProgrammatically(unitSystem6, ref nodeLocations);


            //Turn updates back on and update road:
            unitSystem6.isAllowingRoadUpdates = true;
            unitSystem6.UpdateAllRoads();
        }


        /// <summary> Create 2L double intersections: </summary>
        public static void RoadArchitectUnitTest7()
        {
            CleanupTest(7);


            GameObject unitSystem7Object = new GameObject("RoadArchitectSystem7");
            RoadSystem unitSystem7 = unitSystem7Object.AddComponent<RoadSystem>();
            unitSystem7.isAllowingRoadUpdates = false;


            Road road1;
            Road road2;
            Road road3;

            // Road1
            new Vector3(678f, 0.03f, 244f);
            new Vector3(660f, 0.03f, 347f);
            new Vector3(639f, 0.03f, 401f);
            new Vector3(577f, 0.03f, 398f);  //Intersection1
            new Vector3(406f, 0.03f, 396f);  //Intersection2
            new Vector3(402f, 0.03f, 516f);
            new Vector3(417f, 0.03f, 626f);

            // Road2
            new Vector3(581f, 0.03f, 237f);
            new Vector3(580f, 0.03f, 360f);
            //Intersection1
            new Vector3(551f, 0.03f, 440f);
            new Vector3(564f, 0.03f, 485f);

            // Road3
            new Vector3(357f, 0.03f, 347f);
            //Intersection2
            new Vector3(494f, 0.03f, 450f);


            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();

            nodeLocations.Add(new Vector3(678f, 0.03f, 244f));
            nodeLocations.Add(new Vector3(660f, 0.03f, 347f));
            nodeLocations.Add(new Vector3(639f, 0.03f, 401f));
            nodeLocations.Add(new Vector3(577f, 0.03f, 398f));
            nodeLocations.Add(new Vector3(406f, 0.03f, 396f));
            nodeLocations.Add(new Vector3(402f, 0.03f, 516f));
            nodeLocations.Add(new Vector3(417f, 0.03f, 626f));


            road1 = RoadAutomation.CreateRoadProgrammatically(unitSystem7, ref nodeLocations);
            road1.laneAmount = 2;


            //Get road system, create road #1:
            nodeLocations.Clear();

            nodeLocations.Add(new Vector3(581f, 0.03f, 237f));
            nodeLocations.Add(new Vector3(580f, 0.03f, 360f));
            nodeLocations.Add(new Vector3(577f, 0.03f, 398f));  //Intersection1
            nodeLocations.Add(new Vector3(551f, 0.03f, 440f));
            nodeLocations.Add(new Vector3(564f, 0.03f, 485f));


            road2 = RoadAutomation.CreateRoadProgrammatically(unitSystem7, ref nodeLocations);
            road2.laneAmount = 2;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #2:
            nodeLocations.Clear();

            nodeLocations.Add(new Vector3(357f, 0.03f, 347f));
            nodeLocations.Add(new Vector3(406f, 0.03f, 396f));//Intersection2
            nodeLocations.Add(new Vector3(494f, 0.03f, 450f));


            road3 = RoadAutomation.CreateRoadProgrammatically(unitSystem7, ref nodeLocations);
            road3.laneAmount = 2;
            UnitTestIntersectionHelper(road1, road3, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Turn updates back on and update road:
            unitSystem7.isAllowingRoadUpdates = true;
            unitSystem7.UpdateAllRoads();
        }


        /// <summary> Creates a self closed road; road loop </summary>
        public static void RoadArchitectUnitTest8()
        {
            CleanupTest(8);


            Road road;

            // Road1
            new Vector3(575f, 0.03f, 550f);
            new Vector3(475f, 0.03f, 650f);
            new Vector3(575f, 0.03f, 750f);
            new Vector3(675f, 0.03f, 650f);
            new Vector3(575f, 0.03f, 550f);



            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();

            nodeLocations.Add(new Vector3(575f, 0.03f, 550f));
            nodeLocations.Add(new Vector3(525f, 0.03f, 600f));
            nodeLocations.Add(new Vector3(575f, 0.03f, 650f));
            nodeLocations.Add(new Vector3(625f, 0.03f, 600f));
            nodeLocations.Add(new Vector3(575f, 0.03f, 550f));


            GameObject unitSystem8Object = new GameObject("RoadArchitectSystem8");
            RoadSystem unitSystem8 = unitSystem8Object.AddComponent<RoadSystem>();
            unitSystem8.isAllowingRoadUpdates = false;
            road = RoadAutomation.CreateRoadProgrammatically(unitSystem8, ref nodeLocations);
            road.laneAmount = 2;

            // creates a intersection instead of a road connection
            //UnitTestIntersectionHelper(road1, road1, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);

            road.spline.ActivateEndNodeConnection(road.spline.nodes[road.spline.nodes.Count - 1], road.spline.nodes[0]);


            //Turn updates back on and update road:
            unitSystem8.isAllowingRoadUpdates = true;
            unitSystem8.UpdateAllRoads();
        }


        /// <summary> Create 3 intersections with end node situations: </summary>
        public static void RoadArchitectUnitTest9()
        {
            CleanupTest(9);


            GameObject unitSystem9Object = new GameObject("RoadArchitectSystem9");
            RoadSystem unitSystem9 = unitSystem9Object.AddComponent<RoadSystem>();
            unitSystem9.isAllowingRoadUpdates = false;


            Road road1;
            Road road2;
            Road road3;
            Road road4;

            // Road1
            new Vector3(724f, 0.03f, 220f);
            //Intersection1
            new Vector3(711f, 0.03f, 345f);
            //Intersection2
            new Vector3(686f, 0.03f, 475f);
            //Intersection3
            new Vector3(679f, 0.03f, 560f);

            // Road2
            new Vector3(760f, 0.03f, 345f);
            new Vector3(735f, 0.03f, 345f);
            //Intersection1
            new Vector3(711f, 0.03f, 345f);

            // Road3
            new Vector3(615f, 0.03f, 475f);
            new Vector3(640f, 0.03f, 475f);
            //Intersection2
            new Vector3(686f, 0.03f, 475f);

            // Road4
            new Vector3(615f, 0.03f, 545f);
            new Vector3(640f, 0.03f, 549f);
            //Intersection3
            new Vector3(679f, 0.03f, 553f);
            new Vector3(725f, 0.03f, 560f);


            //Create base road:
            List<Vector3> nodeLocations = new List<Vector3>();

            nodeLocations.Add(new Vector3(724f, 0.03f, 220f));
            nodeLocations.Add(new Vector3(711f, 0.03f, 345f));
            nodeLocations.Add(new Vector3(686f, 0.03f, 475f));
            nodeLocations.Add(new Vector3(679f, 0.03f, 560f));


            road1 = RoadAutomation.CreateRoadProgrammatically(unitSystem9, ref nodeLocations);
            road1.laneAmount = 2;


            //Get road system, create road #1:
            nodeLocations.Clear();

            nodeLocations.Add(new Vector3(762f, 0.03f, 345f));
            nodeLocations.Add(new Vector3(744f, 0.03f, 345f));
            //Intersection1
            nodeLocations.Add(new Vector3(711f, 0.03f, 345f));


            road2 = RoadAutomation.CreateRoadProgrammatically(unitSystem9, ref nodeLocations);
            road2.laneAmount = 2;
            UnitTestIntersectionHelper(road1, road2, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #2:
            nodeLocations.Clear();

            nodeLocations.Add(new Vector3(615f, 0.03f, 475f));
            nodeLocations.Add(new Vector3(640f, 0.03f, 475f));
            //Intersection2
            nodeLocations.Add(new Vector3(686f, 0.03f, 475f));


            road3 = RoadAutomation.CreateRoadProgrammatically(unitSystem9, ref nodeLocations);
            road3.laneAmount = 2;
            UnitTestIntersectionHelper(road1, road3, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);


            //Get road system, create road #3:
            nodeLocations.Clear();

            nodeLocations.Add(new Vector3(615f, 0.03f, 560f));
            nodeLocations.Add(new Vector3(640f, 0.03f, 560f));
            //Intersection3
            nodeLocations.Add(new Vector3(679f, 0.03f, 560f));
            nodeLocations.Add(new Vector3(725f, 0.03f, 560f));


            road4 = RoadAutomation.CreateRoadProgrammatically(unitSystem9, ref nodeLocations);
            road4.laneAmount = 2;
            UnitTestIntersectionHelper(road1, road4, RoadIntersection.iStopTypeEnum.None, RoadIntersection.RoadTypeEnum.NoTurnLane);



            //Turn updates back on and update road:
            unitSystem9.isAllowingRoadUpdates = true;
            unitSystem9.UpdateAllRoads();
        }
    }
#endif
}
