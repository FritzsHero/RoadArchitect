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
        [UnityEngine.Serialization.FormerlySerializedAs("mGSDID")]
        private int uID = -1;

        public int UID { get { return uID; } }

        [HideInInspector]
        [UnityEngine.Serialization.FormerlySerializedAs("tTerrain")]
        public Terrain terrain;

        //Splat map:
        [UnityEngine.Serialization.FormerlySerializedAs("SplatResoWidth")]
        public int splatResoWidth = 1024;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatResoHeight")]
        public int splatResoHeight = 1024;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatBackground")]
        public Color splatBackground = new Color(0f, 0f, 0f, 1f);
        [UnityEngine.Serialization.FormerlySerializedAs("SplatForeground")]
        public Color splatForeground = new Color(1f, 1f, 1f, 1f);
        [UnityEngine.Serialization.FormerlySerializedAs("SplatWidth")]
        public float splatWidth = 30f;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatSkipBridges")]
        public bool isSplatSkipBridges = false;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatSkipTunnels")]
        public bool isSplatSkipTunnels = false;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatSingleRoad")]
        public bool isSplatSingleRoad = false;
        [UnityEngine.Serialization.FormerlySerializedAs("SplatSingleChoiceIndex")]
        public int splatSingleChoiceIndex = 0;
        [UnityEngine.Serialization.FormerlySerializedAs("RoadSingleChoiceUID")]
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
