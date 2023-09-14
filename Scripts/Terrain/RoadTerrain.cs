#region "Imports"
using UnityEngine;
using System.Collections.Generic;
#endregion


namespace RoadArchitect
{
    [ExecuteInEditMode]
    public class RoadTerrain : MonoBehaviour
    {
        #region "Vars"
        [SerializeField]
        [HideInInspector]
        private int uID = -1;

        public int UID { get { return uID; } }

        [HideInInspector]
        public Terrain terrain;

        //Splat map:
        public int splatResoWidth = 1024;
        public int splatResoHeight = 1024;
        public Color splatBackground = new Color(0f, 0f, 0f, 1f);
        public Color splatForeground = new Color(1f, 1f, 1f, 1f);
        public float splatWidth = 30f;
        public bool isSplatSkipBridges = false;
        public bool isSplatSkipTunnels = false;
        public bool isSplatSingleRoad = false;
        public int splatSingleChoiceIndex = 0;
        public string roadSingleChoiceUID = "";
        #endregion


        private void OnEnable()
        {
            CheckID();
            if (!terrain)
            {
                terrain = transform.gameObject.GetComponent<Terrain>();
            }
        }


        /// <summary> Check for unique id and assign terrain </summary>
        public void CheckID()
        {
            if (uID < 0)
            {
                uID = GetNewID();
            }
            if (!terrain)
            {
                terrain = transform.gameObject.GetComponent<Terrain>();
            }
        }


        /// <summary> Return new id preventing terrain id duplication </summary>
        private int GetNewID()
        {
            Object[] allTerrainObjs = GameObject.FindObjectsOfType<RoadTerrain>();
            List<int> allIDS = new List<int>(allTerrainObjs.Length);
            foreach (RoadTerrain Terrain in allTerrainObjs)
            {
                if (Terrain.UID > 0)
                {
                    allIDS.Add(Terrain.UID);
                }
            }

            bool isNotDone = true;
            int spamChecker = 0;
            int spamCheckerMax = allIDS.Count + 64;
            int random;
            while (isNotDone)
            {
                if (spamChecker > spamCheckerMax)
                {
                    Debug.LogError("Failed to generate terrainID");
                    break;
                }
                random = Random.Range(1, 2000000000);
                if (!allIDS.Contains(random))
                {
                    isNotDone = false;
                    return random;
                }
                spamChecker += 1;
            }

            return -1;
        }


        private void Start()
        {
            CheckID();
            if (!terrain)
            {
                terrain = transform.gameObject.GetComponent<Terrain>();
            }
        }
    }
}
