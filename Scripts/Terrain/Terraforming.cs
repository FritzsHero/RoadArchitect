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
            [UnityEngine.Serialization.FormerlySerializedAs("GSDID")]
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
        private static void CheckAllTerrains()
        {
            Object[] allTerrains = GameObject.FindObjectsOfType<Terrain>();
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


        /// <summary> Checks if every Terrain uses a RoadTerrain script and set it to 0 on y </summary>
        public static void CheckAllTerrainsHeight0()
        {
            CheckAllTerrains();
            Object[] allTerrains = GameObject.FindObjectsOfType<Terrain>();
            foreach (Terrain terrain in allTerrains)
            {
                if (!RootUtils.IsApproximately(terrain.transform.position.y, 0f, 0.0001f))
                {
                    Vector3 tVect = terrain.transform.position;
                    tVect.y = 0f;
                    terrain.transform.position = tVect;
                }
            }
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
            Object[] allTerrains = GameObject.FindObjectsOfType<Terrain>();
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
            Object[] TIDs = GameObject.FindObjectsOfType<RoadTerrain>();
            Terrain terrain;
            int[,] tDetails = null;
            int IntBufferX = 0;
            int IntBufferY = 0;
            int tVal = 0;
            foreach (TempTerrainData TTD in _TTDList)
            {
                foreach (RoadTerrain TID in TIDs)
                {
                    if (TID.UID == TTD.uID)
                    {
                        terrain = TID.transform.gameObject.GetComponent<Terrain>();
                        if (terrain != null)
                        {
                            //Details:
                            if (_spline.road.isDetailModificationEnabled)
                            {
                                for (int index = 0; index < TTD.DetailLayersCount; index++)
                                {
                                    //if(TTD.DetailLayersSkip.Contains(i) || TTD.DetailValues[i] == null)
                                    //{
                                    //  continue;
                                    //}
                                    //if(TTD.DetailsI[i] > 0)
                                    //{
                                    //	tTerrain.terrainData.SetDetailLayer(0, 0, i, TTD.DetailValues[i]);	
                                    //}

                                    if (TTD.DetailLayersSkip.Contains(index) || TTD.MainDetailsX == null || TTD.MainDetailsX.Count < 1)
                                    {
                                        continue;
                                    }
                                    tDetails = terrain.terrainData.GetDetailLayer(0, 0, TTD.DetailMaxIndex, TTD.DetailMaxIndex, index);

                                    int MaxCount = TTD.MainDetailsX.Count;
                                    for (int j = 0; j < MaxCount; j++)
                                    {
                                        IntBufferX = TTD.MainDetailsX[j];
                                        IntBufferY = TTD.MainDetailsY[j];
                                        tVal = tDetails[IntBufferX, IntBufferY];
                                        if (tVal > 0)
                                        {
                                            TTD.DetailsX[index].Add((ushort)IntBufferX);
                                            TTD.DetailsY[index].Add((ushort)IntBufferY);
                                            TTD.OldDetailsValue[index].Add((ushort)tVal);
                                            tDetails[IntBufferX, IntBufferY] = 0;
                                        }
                                    }
                                    TTD.detailsCount[index] = TTD.DetailsX[index].Count;

                                    terrain.terrainData.SetDetailLayer(0, 0, index, tDetails);
                                    tDetails = null;
                                    TTD.DetailHasProcessed = null;
                                }
                                TTD.MainDetailsX = null;
                                TTD.MainDetailsY = null;
                                System.GC.Collect();
                            }
                            //Trees:
                            if (_spline.road.isTreeModificationEnabled && TTD.TreesCurrent != null && TTD.treesCount > 0)
                            {
                                terrain.terrainData.treeInstances = TTD.TreesCurrent.ToArray();
                            }
                            //Heights:
                            if (_spline.road.isHeightModificationEnabled && TTD.heights != null && TTD.Count > 0)
                            {
                                //Do heights last to trigger collisions and stuff properly:
                                terrain.terrainData.SetHeights(0, 0, TTD.heights);
                            }
                        }
                    }
                }
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

            Object[] TIDs = GameObject.FindObjectsOfType<RoadTerrain>();
            float[,] heights;
            int[,] tDetails;
            int ArrayCount;
            foreach (TerrainHistoryMaker TH in _road.TerrainHistory)
            {
                Terrain terrain = null;
                foreach (RoadTerrain TID in TIDs)
                {
                    if (TID.UID == TH.terrainID)
                    {
                        terrain = TID.terrain;
                    }
                }
                if (!terrain)
                {
                    continue;
                }

                if (TH.heightmapResolution != terrain.terrainData.heightmapResolution)
                {
                    TH.Nullify();
                    continue;
                }

                //Heights:
                if (TH.x1 != null)
                {
                    heights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
                    ArrayCount = TH.Count;
                    for (int index = 0; index < ArrayCount; index++)
                    {
                        heights[TH.x1[index], TH.y1[index]] = TH.height[index];
                    }
                    terrain.terrainData.SetHeights(0, 0, heights);
                }
                //Details:
                if (TH.detailsCount != null && TH.detailsX != null && TH.detailsY != null && TH.detailsOldValue != null)
                {
                    int RealLayerCount = terrain.terrainData.detailPrototypes.Length;
                    int StartIndex = 0;
                    int EndIndex = 0;
                    for (int index = 0; index < TH.detailLayersCount; index++)
                    {
                        if (index >= RealLayerCount)
                        {
                            break;
                        }
                        if (TH.detailsX.Length <= index)
                        {
                            break;
                        }
                        if (TH.detailsY.Length <= index)
                        {
                            break;
                        }
                        if (TH.detailsX.Length < 1)
                        {
                            continue;
                        }

                        tDetails = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, index);
                        ArrayCount = TH.detailsCount[index];
                        if (ArrayCount == 0)
                        {
                            continue;
                        }
                        EndIndex += ArrayCount;
                        for (int j = StartIndex; j < EndIndex; j++)
                        {
                            tDetails[TH.detailsX[j], TH.detailsY[j]] = TH.detailsOldValue[j];
                        }
                        StartIndex = EndIndex;
                        terrain.terrainData.SetDetailLayer(0, 0, index, tDetails);
                        tDetails = null;
                    }
                }
                //Trees:
                TreeInstance[] xTress = TH.MakeTrees();
                if (xTress != null)
                {
                    ArrayCount = xTress.Length;
                    if (ArrayCount > 0 && TH.oldTrees != null)
                    {
                        int TerrainTreeCount = terrain.terrainData.treeInstances.Length;
                        TreeInstance[] tTrees = new TreeInstance[ArrayCount + TerrainTreeCount];
                        System.Array.Copy(terrain.terrainData.treeInstances, 0, tTrees, 0, TerrainTreeCount);
                        System.Array.Copy(xTress, 0, tTrees, TerrainTreeCount, ArrayCount);
                        terrain.terrainData.treeInstances = tTrees;
                    }
                    xTress = null;
                }
            }
            System.GC.Collect();
        }
    }
}
