using System.Collections.Generic;


namespace RoadArchitect.Threading
{
    public static class TerrainCalcsStatic
    {
        public static void RunMe(ref List<Terraforming.TempTerrainData> _TTDList, SplineC _spline, Road _road)
        {
            float Step = (_road.roadDefinition * 0.4f) / _spline.distance;
            if (Step > 2f)
            {
                Step = 2f;
            }
            if (Step < 1f)
            {
                Step = 1f;
            }
            //float tDistance = tRoad.RoadWidth()*2f;

            //Vector3 tVect,POS;

            foreach (Terraforming.TempTerrainData TTD in _TTDList)
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
                RootUtils.StartProfiling(_road, "DoRects");
                TerraformingThreaded.DoRects(_spline, TTD);
                RootUtils.EndProfiling(_road);
                //}
                //else
                //{
                //	for(float i=StartMin;i<=FinalMax;i+=Step)
                //  {
                //		if(tSpline.IsInBridgeTerrain(i))
                //      {
                //			float tFloat = tSpline.GetBridgeEnd(i);
                //			if(IsApproximately(tFloat,1f,0.00001f) || tFloat > 1f)
                //          {
                //              continue;
                //          }
                //			if(tFloat < 0f)
                //          {
                //              continue;
                //          }
                //			i = tFloat;
                //		}
                //		tSpline.GetSplineValue_Both(i,out tVect,out POS);
                //		PrevHeight = TerraformingThreaded.ProcessLineHeights(tSpline,ref tVect,ref POS,tDistance,TTD,PrevHeight);
                //		tSpline.HeightHistory.Add(new KeyValuePair<float,float>(i,PrevHeight*TTD.TerrainSize.y));
                //	}	
                //					
                //	for(int i=0;i<TTD.cI;i++)
                //  {
                //		TTD.heights[TTD.cX[i],TTD.cY[i]] = TTD.cH[i];
                //	}
                //}
            }
            _spline.HeightHistory.Sort(Compare1);
        }


        private static int Compare1(KeyValuePair<float, float> _a, KeyValuePair<float, float> _b)
        {
            return _a.Key.CompareTo(_b.Key);
        }
    }
}
