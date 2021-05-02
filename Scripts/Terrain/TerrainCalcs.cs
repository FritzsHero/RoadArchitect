using System.Collections.Generic;


namespace RoadArchitect.Threading
{
    public class TerrainCalcs : ThreadedJob
    {
        private object handle = new object();
        private List<Terraforming.TempTerrainData> TTDList;
        private SplineC spline;
        private Road road;


        public void Setup(ref List<Terraforming.TempTerrainData> _TTDList, SplineC _tSpline, Road _tRoad)
        {
            TTDList = _TTDList;
            spline = _tSpline;
            road = _tRoad;
        }


        protected override void ThreadFunction()
        {
            float Step = (road.roadDefinition * 0.4f) / spline.distance;
            if (Step > 2f)
            {
                Step = 2f;
            }
            if (Step < 1f)
            {
                Step = 1f;
            }
            //float tDistance = tRoad.RoadWidth()*2f;

            //Vector3 tVect, POS;
            foreach (Terraforming.TempTerrainData TTD in TTDList)
            {
                //float PrevHeight = 0f;
                //float FinalMax = 1f;
                //float StartMin = 0f;
                //if(tSpline.bSpecialEndControlNode)
                //{
                //	FinalMax = tSpline.mNodes[tSpline.GetNodeCount()-2].tTime;
                //}
                //if(tSpline.bSpecialStartControlNode)
                //{
                //	StartMin = tSpline.mNodes[1].tTime;
                //}

                //if(tRoad.opt_MatchTerrain)
                //{
                try
                {
                    TerraformingThreaded.DoRects(spline, TTD);
                }
                catch (System.Exception e)
                {
                    lock (handle)
                    {
                        road.isEditorError = true;
                        road.exceptionError = e;
                    }
                    throw e;
                }
                //}
                //else
                //{
                //  for(float i=StartMin;i<=FinalMax;i+=Step)
                //  {
                //	    if(tSpline.IsInBridgeTerrain(i))
                //      {
                //		    float tFloat = tSpline.GetBridgeEnd(i);
                //		    if(IsApproximately(tFloat,1f,0.00001f) || tFloat > 1f)
                //          {
                //            continue;
                //          }
                //		    if(tFloat < 0f)
                //          {
                //            continue;
                //          }
                //		    i = tFloat;
                //	    }
                //	    tSpline.GetSplineValue_Both(i,out tVect,out POS);
                //	    PrevHeight = TerraformingThreaded.ProcessLineHeights(tSpline,ref tVect,ref POS,tDistance,TTD,PrevHeight);
                //	    tSpline.HeightHistory.Add(new KeyValuePair<float,float>(i,PrevHeight*TTD.TerrainSize.y));
                //  }	
                //					
                //	for(int i=0;i<TTD.cI;i++)
                //  {
                //		TTD.heights[TTD.cX[i],TTD.cY[i]] = TTD.cH[i];
                //	}
                //}
            }
            spline.HeightHistory.Sort(CompareKeys);
            IsDone = true;
        }


        private int CompareKeys(KeyValuePair<float, float> _a, KeyValuePair<float, float> _b)
        {
            return _a.Key.CompareTo(_b.Key);
        }
    }
}
