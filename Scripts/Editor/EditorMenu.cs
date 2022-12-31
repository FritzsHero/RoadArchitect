#if UNITY_EDITOR
#region "Imports"
using UnityEngine;
using UnityEditor;
using RoadArchitect;
#endregion


namespace RoadArchitect
{
    /// <summary> Provides the menu items inside the editor  </summary>
    public class EditorMenu
    {
        /// <summary> Creates the road system. </summary>
        [MenuItem("Window/Road Architect/Create road system")]
        public static void CreateRoadSystem()
        {
            Object[] allRoadSystemObjects = GameObject.FindObjectsOfType<RoadSystem>();
            int nextCount = (allRoadSystemObjects.Length + 1);
            allRoadSystemObjects = null;

            GameObject newRoadSystemObject = new GameObject("RoadArchitectSystem" + nextCount.ToString());
            RoadSystem newRoadSystem = newRoadSystemObject.AddComponent<RoadSystem>();
            //Add road for new road system.
            newRoadSystem.AddRoad(true);

            GameObject masterIntersectionsObject = new GameObject("Intersections");
            masterIntersectionsObject.transform.parent = newRoadSystemObject.transform;
        }


        /// <summary> Add road to gameobject. Not sure if this is necessary. </summary>
        [MenuItem("Window/Road Architect/Add road")]
        public static void AddRoad()
        {
            Object[] allRoadSystemObjects = GameObject.FindObjectsOfType<RoadSystem>();
            if (allRoadSystemObjects != null && allRoadSystemObjects.Length == 0)
            {
                CreateRoadSystem();
                return;
            }
            else
            {
                RoadSystem firstRoadSystem = (RoadSystem)allRoadSystemObjects[0];
                Selection.activeGameObject = firstRoadSystem.AddRoad();
            }
        }


        /// <summary> Updates all roads. Used when things get out of sync. </summary>
        [MenuItem("Window/Road Architect/Update All Roads")]
        public static void UpdateAllRoads()
        {
            Road[] allRoadObjects = GameObject.FindObjectsOfType<Road>();

            int roadCount = allRoadObjects.Length;

            Road singleRoad = null;
            SplineC[] tPiggys = null;
            if (roadCount > 1)
            {
                tPiggys = new SplineC[roadCount - 1];
            }

            for (int count = 0; count < roadCount; count++)
            {
                singleRoad = allRoadObjects[count];
                if (count > 0)
                {
                    tPiggys[count - 1] = singleRoad.spline;
                }
            }

            singleRoad = allRoadObjects[0];
            if (tPiggys != null && tPiggys.Length > 0)
            {
                singleRoad.PiggyBacks = tPiggys;
            }
            singleRoad.UpdateRoad();
        }


        /// <summary> Show the help screen. </summary>
        [MenuItem("Window/Road Architect/Help")]
        public static void ShowHelpWindow()
        {
            HelpWindow helpWindow = EditorWindow.GetWindow<HelpWindow>();
            helpWindow.Initialize();
        }


        /// <summary> WARNING: Only call this on an empty scene that has some terrains on it. We are not responsbile for data loss if this function is called by the user. </summary>
        [MenuItem("Window/Road Architect/Testing/Run all unit tests (caution)")]
        public static void TestProgram()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your RoadSystem 1, 6, 7, 8 and 9 and will create a lot of test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTests();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 1-5 (caution)")]
        public static void Test1To5()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your first RoadSystem and will create a lot of test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest1To5();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 6 (caution)")]
        public static void Test6()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your sixth RoadSystem and will create a lot of test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest6();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 7 (caution)")]
        public static void Test7()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your seventh RoadSystem and will create a lot of test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest7();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 8 (caution)")]
        public static void Test8()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your eigth RoadSystem and will create a test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest8();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 9 (caution)")]
        public static void Test9()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your RoadArchitectSystem9 and will create test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest9();
            }
        }


        [MenuItem("Window/Road Architect/Testing/Run unit test 10 (caution)")]
        public static void Test10()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your RoadArchitectSystem10 and will create test roads.", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.RoadArchitectUnitTest10();
            }
        }


        /// <summary> WARNING: Only call this on an empty scene that has some terrains on it. We are not responsbile for data loss if this function is called by the user. </summary>
        [MenuItem("Window/Road Architect/Testing/Clean up tests (caution)")]
        public static void TestCleanup()
        {
            if (EditorUtility.DisplayDialog("Warning !", "This will delete your RoadSystem 1, 6, 7, 8 and 9", "OK", "Cancel"))
            {
                RoadArchitect.Tests.UnitTests.CleanupAllTests();
            }
        }


        /// <summary> Get code line count for RA project </summary>
        [MenuItem("Window/Road Architect/Testing/Get line count of RA")]
        public static void TestCodeCount()
        {
            string mainDir = System.IO.Path.Combine(System.Environment.CurrentDirectory, RoadEditorUtility.GetBasePathForIO());
            string[] files = System.IO.Directory.GetFiles(mainDir, "*.cs", System.IO.SearchOption.AllDirectories);
            int lineCount = 0;
            foreach (string file in files)
            {
                lineCount += System.IO.File.ReadAllLines(file).Length;
            }
            Debug.Log(string.Format("{0:n0}", lineCount) + " lines of code in Road Architect.");
        }


        [MenuItem("Window/Road Architect/Report a Bug")]
        public static void ReportBug()
        {
            Application.OpenURL("https://github.com/FritzsHero/RoadArchitect/issues");
        }
    }
}
#endif
