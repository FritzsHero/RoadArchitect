using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    public static class Terraforming
    {
        public class TempTerrainData
        {
            public int HM;
            public int HMHeight;
            public float[,] heights;
            public bool[,] tHeights;

            public float HMRatio;
            public float MetersPerHM = 0f;

            //Heights:
            public ushort[] cX;
            public ushort[] cY;
            public float[] cH;
            public float[] oldH;
            public int Count = 0;
            public int TerrainMaxIndex;

            //Details:
            public int DetailLayersCount;

            public List<ushort> MainDetailsX;
            public List<ushort> MainDetailsY;

            public List<List<ushort>> DetailsX;
            public List<List<ushort>> DetailsY;
            public List<List<ushort>> OldDetailsValue;
            //public Dictionary<int,int[,]> DetailValues;
            public int[] detailsCount;
            public float DetailToHeightRatio;


            //public Dictionary<int,bool[,]> DetailHasProcessed;

            public HashSet<int> DetailHasProcessed;

            //public List<List<bool>> OldDetailsValue;

            public int DetailMaxIndex;
            public HashSet<int> DetailLayersSkip;

            //Trees
            public List<TreeInstance> TreesCurrent;
            public List<TreeInstance> TreesOld;
            public int treesCount;
            public int TreeSize;

            public Vector3 TerrainSize;
            public Vector3 TerrainPos;
            public int uID;


            public void Nullify()
            {
                heights = null;
                tHeights = null;
                cX = null;
                cY = null;
                cH = null;
                oldH = null;
                //DetailsX = null;
                //DetailsY = null;
                //DetailValues = null;
                OldDetailsValue = null;
                //DetailsI = null;
                TreesCurrent = null;
                TreesOld = null;
            }
        }


        /// <summary> Checks all terrains and adds RoadTerrain if necessary </summary>
        public static void CheckAllTerrains()
        {
            Object[] allTerrains = EngineIntegration.FindObjectsByType<Terrain>();
            RoadTerrain TID;
            GameObject terrainObj;
            foreach (Terrain terrain in allTerrains)
            {
                terrainObj = terrain.transform.gameObject;
                TID = terrainObj.GetComponent<RoadTerrain>();
                if (TID == null)
                {
                    TID = terrainObj.AddComponent<RoadTerrain>();
                }
                TID.CheckID();
            }
        }


        [System.Obsolete("Just use CheckAllTerrains")]
        /// <summary> Checks if every Terrain uses a RoadTerrain script </summary>
        public static void CheckAllTerrainsHeight0()
        {
            CheckAllTerrains();
        }


        /// <summary> Stores terrain infos and starts terrain calculations </summary>
        public static void ProcessRoadTerrainHook1(SplineC _spline, Road _road, bool _isMultithreaded = true)
        {
            ProcessRoadTerrainHook1Do(ref _spline, ref _road, _isMultithreaded);
        }


        private static void ProcessRoadTerrainHook1Do(ref SplineC _spline, ref Road _road, bool _isMultithreaded)
        {
            RootUtils.StartProfiling(_road, "ProcessRoadTerrainHook1");
            //First lets make sure all terrains have a RoadTerrain script:
            CheckAllTerrains();

            //Reset the terrain:
            RootUtils.StartProfiling(_road, "TerrainsReset");
            TerrainsReset(_road);
            RootUtils.EndProfiling(_road);

            float heightDistance = _road.matchHeightsDistance;
            //float treeDistance = _road.clearTreesDistance;
            float detailDistance = _road.clearDetailsDistance;

            Dictionary<Terrain, TempTerrainData> TempTerrainDict = new Dictionary<Terrain, TempTerrainData>();
            //Populate dictionary:
            Object[] allTerrains = EngineIntegration.FindObjectsByType<Terrain>();
            RoadTerrain TID;
            int aSize = 0;
            int dSize = 0;
            TempTerrainData TTD;
            bool isContaining = false;
            Construction2DRect tRect = null;
            //Construction2DRect rRect = null;


            foreach (Terrain terrain in allTerrains)
            {
                if (terrain.terrainData == null)
                {
                    continue;
                }
                tRect = GetTerrainBounds(terrain);
                isContaining = false;
                //Debug.Log(terrain.transform.name + " bounds: " + tRect.ToStringRA());
                //Debug.Log("  Road bounds: " + tSpline.RoadV0 + "," + tSpline.RoadV1 + "," + tSpline.RoadV2 + "," + tSpline.RoadV3);

                // Check if the terrain overlaps with a part of the spline
                if (isContaining != true && tRect.Contains(ref _spline.RoadV0))
                {
                    isContaining = true;
                }
                else if (isContaining != true && tRect.Contains(ref _spline.RoadV1))
                {
                    isContaining = true;
                }
                else if (isContaining != true && tRect.Contains(ref _spline.RoadV2))
                {
                    isContaining = true;
                }
                else if (isContaining != true && tRect.Contains(ref _spline.RoadV3))
                {
                    isContaining = true;
                }
                else
                {
                    int nodeCount = _road.spline.GetNodeCount();
                    Vector2 tVect2D_321 = default(Vector2);
                    for (int index = 0; index < nodeCount; index++)
                    {
                        tVect2D_321 = new Vector2(_road.spline.nodes[index].pos.x, _road.spline.nodes[index].pos.z);
                        if (tRect.Contains(ref tVect2D_321))
                        {
                            isContaining = true;
                            break;
                        }
                    }

                    if (!isContaining)
                    {
                        float tDef = 5f / _spline.distance;
                        Vector2 x2D = default(Vector2);
                        Vector3 x3D = default(Vector3);
                        for (float index = 0f; index <= 1f; index += tDef)
                        {
                            x3D = _spline.GetSplineValue(index);
                            x2D = new Vector2(x3D.x, x3D.z);
                            if (tRect.Contains(ref x2D))
                            {
                                isContaining = true;
                                break;
                            }
                        }
                    }
                }

                //rRect = new RoadUtility.Construction2DRect(tSpline.RoadV0,tSpline.RoadV1,tSpline.RoadV2,tSpline.RoadV3);


                if (isContaining && !TempTerrainDict.ContainsKey(terrain))
                {
                    TTD = new TempTerrainData();
                    TTD.HM = terrain.terrainData.heightmapResolution;
                    TTD.HMHeight = terrain.terrainData.heightmapResolution;
                    TTD.heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                    TTD.HMRatio = TTD.HM / terrain.terrainData.size.x;
                    TTD.MetersPerHM = terrain.terrainData.size.x / terrain.terrainData.heightmapResolution;
                    float DetailRatio = terrain.terrainData.detailResolution / terrain.terrainData.size.x;

                    //Heights:
                    RootUtils.StartProfiling(_road, "Heights");
                    if (_road.isHeightModificationEnabled)
                    {
                        aSize = (int)_spline.distance * ((int)(heightDistance * 1.65f * TTD.HMRatio) + 2);
                        if (aSize > (terrain.terrainData.heightmapResolution * terrain.terrainData.heightmapResolution))
                        {
                            aSize = terrain.terrainData.heightmapResolution * terrain.terrainData.heightmapResolution;
                        }
                        TTD.cX = new ushort[aSize];
                        TTD.cY = new ushort[aSize];
                        TTD.oldH = new float[aSize];
                        TTD.cH = new float[aSize];
                        TTD.Count = 0;
                        TTD.TerrainMaxIndex = terrain.terrainData.heightmapResolution;
                        TTD.TerrainSize = terrain.terrainData.size;
                        TTD.TerrainPos = terrain.transform.position;
                        TTD.tHeights = new bool[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
                        TID = terrain.transform.gameObject.GetComponent<RoadTerrain>();
                        if (TID != null)
                        {
                            TTD.uID = TID.UID;
                            TempTerrainDict.Add(terrain, TTD);
                        }
                    }

                    //Details:
                    RootUtils.EndStartProfiling(_road, "Details");
                    if (_road.isDetailModificationEnabled)
                    {
                        //TTD.DetailValues = new Dictionary<int, int[,]>();
                        TTD.DetailLayersCount = terrain.terrainData.detailPrototypes.Length;
                        //TTD.DetailHasProcessed = new Dictionary<int, bool[,]>();
                        TTD.DetailHasProcessed = new HashSet<int>();
                        TTD.MainDetailsX = new List<ushort>();
                        TTD.MainDetailsY = new List<ushort>();
                        TTD.detailsCount = new int[TTD.DetailLayersCount];
                        TTD.DetailToHeightRatio = (float)((float)terrain.terrainData.detailResolution) / ((float)terrain.terrainData.heightmapResolution);
                        TTD.DetailMaxIndex = terrain.terrainData.detailResolution;
                        TTD.DetailLayersSkip = new HashSet<int>();

                        // Get all of layer zero.
                        //int[] mMinMaxDetailEntryCount = new int[TTD.DetailLayersCount];
                        //RootUtils.StartProfiling(_road, "DetailValues");
                        //Vector3 bVect = default(Vector3);
                        //Vector2 bVect2D = default(Vector2);
                        //int DetailRes = tTerrain.terrainData.detailResolution;
                        //for(int i=0;i<TTD.DetailLayersCount;i++)
                        //{
                        //	int[,] tInts = tTerrain.terrainData.GetDetailLayer(0,0,tTerrain.terrainData.detailWidth,tTerrain.terrainData.detailHeight,i);
                        //	int Length1 = tInts.GetLength(0);
                        //	int Length2 = tInts.GetLength(1);
                        //	for (int y=0;y < Length1;y++)
                        //	{
                        //		for (int x=0;x < Length2;x++)
                        //		{
                        //			if(tInts[x,y] > 0)
                        //			    {
                        //				bVect = new Vector3(((float)y/(float)DetailRes) * TTD.TerrainSize.x,0f,((float)x/(float)DetailRes) * TTD.TerrainSize.z);
                        //				bVect = tTerrain.transform.TransformPoint(bVect);
                        //				bVect2D = new Vector2(bVect.z,bVect.x);
                        //				if(rRect.Contains(ref bVect2D))
                        //				{
                        //					mMinMaxDetailEntryCount[i] += 1;
                        //				}
                        //			}
                        //		}
                        //	}

                        //	if(mMinMaxDetailEntryCount[i] < 1)
                        //	{
                        //		TTD.DetailLayersSkip.Add(i);
                        //		tInts = null;
                        //		}
                        //		else
                        //		{
                        //			TTD.DetailValues.Add(i,tInts);
                        //			TTD.DetailHasProcessed.Add(i,new bool[tTerrain.terrainData.detailWidth,tTerrain.terrainData.detailHeight]);
                        //		}
                        //	}
                        //RootUtils.EndProfiling(_road);


                        dSize = (int)_spline.distance * ((int)(detailDistance * 3f * DetailRatio) + 2);
                        if (dSize > (terrain.terrainData.detailResolution * terrain.terrainData.detailResolution))
                        {
                            dSize = terrain.terrainData.detailResolution * terrain.terrainData.detailResolution;
                        }

                        //TTD.DetailsX = new List<ushort[]>();
                        //TTD.DetailsY = new List<ushort[]>();
                        //TTD.OldDetailsValue = new List<ushort[]>();
                        TTD.DetailsX = new List<List<ushort>>();
                        TTD.DetailsY = new List<List<ushort>>();
                        TTD.OldDetailsValue = new List<List<ushort>>();
                        //TTD.DetailHasProcessed = new List<List<bool>>();

                        for (int index = 0; index < TTD.DetailLayersCount; index++)
                        {
                            //if(TTD.DetailLayersSkip.Contains(index))
                            //{ 
                            //	TTD.DetailsX.Add(new ushort[0]);
                            //	TTD.DetailsY.Add(new ushort[0]);
                            //	TTD.OldDetailsValue.Add(new ushort[0]);
                            //	continue; 
                            //}
                            //int detailentrycount = (int)((float)mMinMaxDetailEntryCount[index] * 1.5f);
                            //int d_temp_Size = dSize;
                            //if(d_temp_Size > detailentrycount)
                            //{
                            //  d_temp_Size = detailentrycount;
                            //}
                            //if(d_temp_Size < 1)
                            //{
                            //  d_temp_Size = 1;
                            //}
                            //if(d_temp_Size > (tTerrain.terrainData.detailResolution * tTerrain.terrainData.detailResolution))
                            //{
                            //	d_temp_Size = tTerrain.terrainData.detailResolution * tTerrain.terrainData.detailResolution;	
                            //}
                            //
                            //TTD.DetailsX.Add(new ushort[d_temp_Size]);
                            //TTD.DetailsY.Add(new ushort[d_temp_Size]);
                            //TTD.OldDetailsValue.Add(new ushort[d_temp_Size]);

                            TTD.DetailsX.Add(new List<ushort>());
                            TTD.DetailsY.Add(new List<ushort>());
                            TTD.OldDetailsValue.Add(new List<ushort>());
                        }


                        //TTD.DetailsX = new ushort[TTD.DetailLayersCount,dSize];
                        //TTD.DetailsY = new ushort[TTD.DetailLayersCount,dSize];
                        //TTD.OldDetailsValue = new ushort[TTD.DetailLayersCount,dSize];
                    }

                    //Trees:
                    RootUtils.EndStartProfiling(_road, "Trees");
                    if (_road.isTreeModificationEnabled)
                    {
                        TTD.TreesCurrent = new List<TreeInstance>(terrain.terrainData.treeInstances);
                        TTD.TreeSize = TTD.TreesCurrent.Count;
                        TTD.treesCount = 0;
                        TTD.TreesOld = new List<TreeInstance>();
                    }
                    RootUtils.EndProfiling(_road);
                }
            }

            //Figure out relevant TTD to spline:
            List<TempTerrainData> EditorTTDList = new List<TempTerrainData>();
            if (TempTerrainDict != null)
            {
                foreach (Terrain tTerrain in allTerrains)
                {
                    if (TempTerrainDict.ContainsKey(tTerrain))
                    {
                        EditorTTDList.Add(TempTerrainDict[tTerrain]);
                    }
                }
            }

            RootUtils.EndProfiling(_road);

            //Start job now, for each relevant TTD:
            _road.SetEditorTerrainCalcs(ref EditorTTDList);
            if (_isMultithreaded)
            {
                Threading.TerrainCalcs terrainJob = new Threading.TerrainCalcs();
                terrainJob.Setup(ref EditorTTDList, _spline, _road);
                _road.TerrainCalcsJob = terrainJob;
                terrainJob.Start();
            }
            else
            {
                Threading.TerrainCalcsStatic.RunMe(ref EditorTTDList, _spline, _road);
            }
        }


        /// <summary> Returns an 2D rect of the terrain </summary>
        public static Construction2DRect GetTerrainBounds(Terrain _terrain)
        {
            float terrainWidth = _terrain.terrainData.size.x;
            float terrainLength = _terrain.terrainData.size.z;
            //Vector3 tPos = tTerrain.transform.TransformPoint(tTerrain.transform.position);

            Vector3 X0 = new Vector3(0f, 0f, 0f);
            Vector3 X1 = new Vector3(terrainWidth, 0f, 0f);
            Vector3 X2 = new Vector3(terrainWidth, 0f, terrainLength);
            Vector3 X3 = new Vector3(0f, 0f, terrainLength);

            X0 = _terrain.transform.TransformPoint(X0);
            X1 = _terrain.transform.TransformPoint(X1);
            X2 = _terrain.transform.TransformPoint(X2);
            X3 = _terrain.transform.TransformPoint(X3);

            Vector2 P0 = new Vector2(X0.x, X0.z);
            Vector2 P1 = new Vector2(X1.x, X1.z);
            Vector2 P2 = new Vector2(X2.x, X2.z);
            Vector2 P3 = new Vector2(X3.x, X3.z);


            //OLD CODE:
            //Vector2 P0 = new Vector2(0f, 0f);
            //Vector2 P1 = new Vector2(terrainWidth, 0f);
            //Vector2 P2 = new Vector2(terrainWidth, terrainLength);
            //Vector2 P3 = new Vector2(0f, terrainLength);

            //P0 = tTerrain.transform.TransformPoint(P0);
            //P1 = tTerrain.transform.TransformPoint(P1);
            //P2 = tTerrain.transform.TransformPoint(P2);
            //P3 = tTerrain.transform.TransformPoint(P3);

            return new Construction2DRect(P0, P1, P2, P3, _terrain.transform.position.y);
        }


        /// <summary> Assign calculated values to terrains </summary>
        public static void ProcessRoadTerrainHook2(SplineC _spline, ref List<TempTerrainData> _TTDList)
        {
            RootUtils.StartProfiling(_spline.road, "ProcessRoadTerrainHook2");
            ProcessRoadTerrainHook2Do(ref _spline, ref _TTDList);
            RootUtils.EndProfiling(_spline.road);
        }


        private static void ProcessRoadTerrainHook2Do(ref SplineC _spline, ref List<TempTerrainData> _TTDList)
        {
            if (!_spline.road.isTreeModificationEnabled && !_spline.road.isHeightModificationEnabled && !_spline.road.isDetailModificationEnabled)
            {
                //Exit if no mod taking place.
                return;
            }
            Object[] TIDs = EngineIntegration.FindObjectsByType<RoadTerrain>();
            foreach (TempTerrainData TTD in _TTDList)
            {
                Terrain terrain = FindTerrain(TIDs, TTD.uID);
                if (terrain == null)
                {
                    continue;
                }

                ApplyDetails(terrain, TTD, _spline.road.isDetailModificationEnabled);
                ApplyTrees(terrain, TTD, _spline.road.isTreeModificationEnabled);
                ApplyHeights(terrain, TTD, _spline.road.isHeightModificationEnabled);
            }
        }

        private static Terrain FindTerrain(Object[] roadTerrains, int terrainId)
        {
            foreach (RoadTerrain roadTerrain in roadTerrains)
            {
                if (roadTerrain.UID == terrainId)
                {
                    return roadTerrain.transform.gameObject.GetComponent<Terrain>();
                }
            }
            return null;
        }

        private static Terrain FindTerrainReference(Object[] roadTerrains, int terrainId)
        {
            foreach (RoadTerrain roadTerrain in roadTerrains)
            {
                if (roadTerrain.UID == terrainId)
                {
                    return roadTerrain.terrain;
                }
            }
            return null;
        }

        private static void ApplyDetails(Terrain terrain, TempTerrainData terrainData, bool isEnabled)
        {
            if (!isEnabled || terrainData.MainDetailsX == null || terrainData.MainDetailsX.Count < 1)
            {
                return;
            }

            for (int layer = 0; layer < terrainData.DetailLayersCount; layer++)
            {
                if (terrainData.DetailLayersSkip.Contains(layer))
                {
                    continue;
                }

                int[,] details = terrain.terrainData.GetDetailLayer(0, 0, terrainData.DetailMaxIndex, terrainData.DetailMaxIndex, layer);
                for (int index = 0; index < terrainData.MainDetailsX.Count; index++)
                {
                    int x = terrainData.MainDetailsX[index];
                    int y = terrainData.MainDetailsY[index];
                    int value = details[x, y];
                    if (value <= 0)
                    {
                        continue;
                    }

                    terrainData.DetailsX[layer].Add((ushort)x);
                    terrainData.DetailsY[layer].Add((ushort)y);
                    terrainData.OldDetailsValue[layer].Add((ushort)value);
                    details[x, y] = 0;
                }

                terrainData.detailsCount[layer] = terrainData.DetailsX[layer].Count;
                terrain.terrainData.SetDetailLayer(0, 0, layer, details);
                terrainData.DetailHasProcessed = null;
            }

            terrainData.MainDetailsX = null;
            terrainData.MainDetailsY = null;
            System.GC.Collect();
        }

        private static void ApplyTrees(Terrain terrain, TempTerrainData terrainData, bool isEnabled)
        {
            if (isEnabled && terrainData.TreesCurrent != null && terrainData.treesCount > 0)
            {
                terrain.terrainData.treeInstances = terrainData.TreesCurrent.ToArray();
            }
        }

        // Trigger Height last for proper collisions
        private static void ApplyHeights(Terrain terrain, TempTerrainData terrainData, bool isEnabled)
        {
            if (isEnabled && terrainData.heights != null && terrainData.Count > 0)
            {
                terrain.terrainData.SetHeights(0, 0, terrainData.heights);
            }
        }


        public static void TerrainsReset(Road _road)
        {
            if (_road.TerrainHistory == null)
            {
                return;
            }
            if (_road.TerrainHistory.Count < 1)
            {
                return;
            }

            Object[] TIDs = EngineIntegration.FindObjectsByType<RoadTerrain>();
            foreach (TerrainHistoryMaker TH in _road.TerrainHistory)
            {
                Terrain terrain = FindTerrainReference(TIDs, TH.terrainID);
                if (!terrain)
                {
                    continue;
                }

                if (TH.heightmapResolution != terrain.terrainData.heightmapResolution)
                {
                    TH.Nullify();
                    continue;
                }

                RestoreHeights(terrain, TH);
                RestoreDetails(terrain, TH);
                RestoreTrees(terrain, TH);
            }
            System.GC.Collect();
        }

        private static void RestoreHeights(Terrain terrain, TerrainHistoryMaker history)
        {
            if (history.x1 == null)
            {
                return;
            }

            float[,] heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
            for (int index = 0; index < history.Count; index++)
            {
                heights[history.x1[index], history.y1[index]] = history.height[index];
            }
            terrain.terrainData.SetHeights(0, 0, heights);
        }

        private static void RestoreDetails(Terrain terrain, TerrainHistoryMaker history)
        {
            if (history.detailsCount == null || history.detailsX == null || history.detailsY == null || history.detailsOldValue == null)
            {
                return;
            }

            int layerCount = terrain.terrainData.detailPrototypes.Length;
            int startIndex = 0;
            for (int layer = 0; layer < history.detailLayersCount && layer < layerCount; layer++)
            {
                if (layer >= history.detailsCount.Length || history.detailsX.Length < 1 || history.detailsY.Length < 1 || history.detailsOldValue.Length < 1)
                {
                    break;
                }

                int count = history.detailsCount[layer];
                if (count == 0)
                {
                    continue;
                }

                if (count < 0 || startIndex >= history.detailsX.Length || startIndex >= history.detailsY.Length || startIndex >= history.detailsOldValue.Length)
                {
                    break;
                }

                int availableCount = history.detailsX.Length - startIndex;
                if (history.detailsY.Length - startIndex < availableCount)
                {
                    availableCount = history.detailsY.Length - startIndex;
                }
                if (history.detailsOldValue.Length - startIndex < availableCount)
                {
                    availableCount = history.detailsOldValue.Length - startIndex;
                }
                if (count > availableCount)
                {
                    count = availableCount;
                }

                int[,] details = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, layer);
                int endIndex = startIndex + count;
                for (int index = startIndex; index < endIndex; index++)
                {
                    int x = history.detailsX[index];
                    int y = history.detailsY[index];
                    if (x >= 0 && x < details.GetLength(0) && y >= 0 && y < details.GetLength(1))
                    {
                        details[x, y] = history.detailsOldValue[index];
                    }
                }
                terrain.terrainData.SetDetailLayer(0, 0, layer, details);
                startIndex = endIndex;
            }
        }

        private static void RestoreTrees(Terrain terrain, TerrainHistoryMaker history)
        {
            TreeInstance[] restoredTrees = history.MakeTrees();
            if (restoredTrees == null || restoredTrees.Length == 0 || history.oldTrees == null)
            {
                return;
            }

            TreeInstance[] currentTrees = terrain.terrainData.treeInstances;
            TreeInstance[] trees = new TreeInstance[restoredTrees.Length + currentTrees.Length];
            System.Array.Copy(currentTrees, 0, trees, 0, currentTrees.Length);
            System.Array.Copy(restoredTrees, 0, trees, currentTrees.Length, restoredTrees.Length);
            terrain.terrainData.treeInstances = trees;
        }
    }
}
