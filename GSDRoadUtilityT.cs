#region "Imports"
using UnityEngine;
using System.Collections.Generic;
#endregion


namespace GSD.Threaded
{
#if UNITY_EDITOR


    public static class GSDTerraformingT
    {
        public class TerrainBoundsMaker
        {
            public List<GSD.Roads.GSDRoadUtil.Construction3DTri> triList;
            [UnityEngine.Serialization.FormerlySerializedAs("tRect")]
            public GSD.Roads.GSDRoadUtil.Construction2DRect constructRect;
            public float MinI = 0f;
            public float MaxI = 1f;
        }


        /*
        private static Vector3 ProcessLineHeights_PrevVect = new Vector3(0f, 0f, 0f);


        public static float ProcessLineHeights(GSDSplineC tSpline, ref Vector3 tVect, ref Vector3 POS, float tDistance, GSD.Roads.GSDTerraforming.TempTerrainData TTD, float PrevDesiredHeight)
        {
            Vector3 ShoulderR_rVect = new Vector3(0f, 0f, 0f);
            Vector3 ShoulderL_lVect = new Vector3(0f, 0f, 0f);

            float DesiredHeight = ProcessLineHeights_GetDesiredHeight(tVect, ref TTD, ref tSpline);
            float nResult = 0f;
            bool bIntersection = tSpline.IsNearIntersection(ref tVect, ref nResult);
            if(bIntersection)
            {
                if(nResult < tVect.y)
                {
                    tVect.y = nResult;
                    DesiredHeight = ProcessLineHeights_GetDesiredHeight(tVect, ref TTD, ref tSpline);
                }
            }

            int x1 = 0, y1 = 0;
            GetTempHeights_Coordinates(ref tVect, ref TTD, out x1, out y1);

            bool bOverride = false;
            int StepMod = (int) (1 / TTD.HMRatio);
            for(float i = tDistance; i >= 1f; i -= StepMod)
            {
                ShoulderR_rVect = (tVect + new Vector3(i * POS.normalized.z, 0, i * -POS.normalized.x));
                GetTempHeights_Coordinates(ref ShoulderR_rVect, ref TTD, out x1, out y1);
                if(TTD.heights[x1, y1] > DesiredHeight)
                {
                    bOverride = true;
                }
                if(bOverride || !TTD.tHeights[x1, y1])
                {
                    TTD.tHeights[x1, y1] = true;
                    TTD.cX[TTD.cI] = x1;
                    TTD.cY[TTD.cI] = y1;
                    TTD.cH[TTD.cI] = DesiredHeight;
                    TTD.oldH[TTD.cI] = TTD.heights[x1, y1];
                    TTD.cI += 1;
                }
                bOverride = false;

                ShoulderL_lVect = (tVect + new Vector3(i * -POS.normalized.z, 0, i * POS.normalized.x));
                GetTempHeights_Coordinates(ref ShoulderL_lVect, ref TTD, out x1, out y1);
                if(TTD.heights[x1, y1] > DesiredHeight)
                {
                    bOverride = true;
                }
                if(bOverride || !TTD.tHeights[x1, y1])
                {
                    TTD.tHeights[x1, y1] = true;
                    TTD.cX[TTD.cI] = x1;
                    TTD.cY[TTD.cI] = y1;
                    TTD.cH[TTD.cI] = DesiredHeight;
                    TTD.oldH[TTD.cI] = TTD.heights[x1, y1];
                    TTD.cI += 1;
                }
                bOverride = false;
            }

            GetTempHeights_Coordinates(ref tVect, ref TTD, out x1, out y1);
            if(TTD.heights[x1, y1] > DesiredHeight || (tVect.y < ProcessLineHeights_PrevVect.y))
            {
                bOverride = true;
            }
            if(bOverride || !TTD.tHeights[x1, y1])
            {
                TTD.tHeights[x1, y1] = true;
                TTD.cX[TTD.cI] = x1;
                TTD.cY[TTD.cI] = y1;
                if(tDistance > 15f && TTD.HMRatio > 0.24f)
                {
                    TTD.cH[TTD.cI] = DesiredHeight - 0.0002f;
                }
                else
                {
                    TTD.cH[TTD.cI] = DesiredHeight;
                }
                TTD.oldH[TTD.cI] = TTD.heights[x1, y1];
                TTD.cI += 1;
            }

            ProcessLineHeights_PrevVect = tVect;
            return DesiredHeight;
        }


        private static float ProcessLineHeights_GetDesiredHeight(Vector3 tVect, ref GSD.Roads.GSDTerraforming.TempTerrainData TTD, ref GSDSplineC tSpline)
        {
            return ((((tVect - TTD.TerrainPos).y) - tSpline.tRoad.opt_TerrainSubtract_Alt) / TTD.TerrainSize.y);
        }
        */


        private static void GetTempHeightsCoordinates(ref Vector3 _vector, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD, out int _x, out int _y)
        {
            //Get the normalized position of this game object relative to the terrain:
            Vector3 tempCoord = (_vector - _TTD.TerrainPos);

            Vector3 coord;
            coord.x = tempCoord.x / _TTD.TerrainSize.x;
            coord.y = tempCoord.y / _TTD.TerrainSize.y;
            coord.z = tempCoord.z / _TTD.TerrainSize.z;

            //Get the position of the terrain heightmap where this game object is:
            _y = (int) (coord.x * _TTD.HM);
            _x = (int) (coord.z * _TTD.HM);
        }


        private static void GetTempDetailsCoordinates(ref Vector3 _vector, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD, out int _x, out int _y)
        {
            //Get the normalized position of this game object relative to the terrain:
            Vector3 tempCoord = (_vector - _TTD.TerrainPos);

            Vector3 coord;
            coord.x = tempCoord.x / _TTD.TerrainSize.x;
            coord.y = tempCoord.y / _TTD.TerrainSize.y;
            coord.z = tempCoord.z / _TTD.TerrainSize.z;

            //Get the position of the terrain heightmap where this game object is:
            _y = (int) (coord.x * _TTD.DetailMaxIndex);
            _x = (int) (coord.z * _TTD.DetailMaxIndex);
        }


        //Privatized for obfuscate:
        public static void DoRects(GSDSplineC _spline, GSD.Roads.GSDTerraforming.TempTerrainData _TTD)
        {
            DoRectsDo(ref _spline, ref _TTD);
        }


        private static void DoRectsDo(ref GSDSplineC _spline, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD)
        {
            float Sep = _spline.tRoad.RoadWidth() * 0.5f;
            float HeightSep = Sep + (_spline.tRoad.matchHeightsDistance * 0.5f);
            List<TerrainBoundsMaker> TBMList = new List<TerrainBoundsMaker>();
            //List<GSD.Roads.GSDRoadUtil.Construction3DTri> triList = new List<GSD.Roads.GSDRoadUtil.Construction3DTri>();
            List<GSD.Roads.GSDRoadUtil.Construction2DRect> TreerectList = new List<GSD.Roads.GSDRoadUtil.Construction2DRect>();
            float tStep = _spline.tRoad.roadDefinition / _spline.distance;
            //tStep *= 0.5f;

            //Start initializing the loop. Convuluted to handle special control nodes, so roads don't get rendered where they aren't supposed to, while still preserving the proper curvature.
            float FinalMax = 1f;
            float StartMin = 0f;
            if (_spline.bSpecialEndControlNode)
            {   //If control node, start after the control node:
                FinalMax = _spline.mNodes[_spline.GetNodeCount() - 2].tTime;
            }
            if (_spline.bSpecialStartControlNode)
            {   //If ends in control node, end construction before the control node:
                StartMin = _spline.mNodes[1].tTime;
            }
            //			bool bFinalEnd = false;
            //			float RoadConnection_StartMin1 = StartMin;	//Storage of incremental start values for the road connection mesh construction at the end of this function.
            //			float RoadConnection_FinalMax1 = FinalMax; 	//Storage of incremental end values for the road connection mesh construction at the end of this function.
            if (_spline.bSpecialEndNode_IsStart_Delay)
            {
                StartMin += (_spline.SpecialEndNodeDelay_Start / _spline.distance); //If there's a start delay (in meters), delay the start of road construction: Due to special control nodes for road connections or 3 way intersections.
            }
            else if (_spline.bSpecialEndNode_IsEnd_Delay)
            {
                FinalMax -= (_spline.SpecialEndNodeDelay_End / _spline.distance);   //If there's a end delay (in meters), cut early the end of road construction: Due to special control nodes for road connections or 3 way intersections.
            }
            //			float RoadConnection_StartMin2 = StartMin;	//Storage of incremental start values for the road connection mesh construction at the end of this function.
            //			float RoadConnection_FinalMax2 = FinalMax; 	//Storage of incremental end values for the road connection mesh construction at the end of this function.

            FinalMax = FinalMax + tStep;

            Vector3 tVect1 = default(Vector3);
            Vector3 tVect2 = default(Vector3);
            Vector3 POS1 = default(Vector3);
            Vector3 POS2 = default(Vector3);
            if (FinalMax > 1f)
            {
                FinalMax = 1f;
            }

            float tNext = 0f;
            float fValue1, fValue2;
            float fValueMod = _spline.tRoad.roadDefinition / _spline.distance;
            bool bIsPastInter = false;
            float tIntStrength = 0f;
            float tIntStrength2 = 0f;
            //			bool bMaxIntersection = false;
            //			bool bFirstInterNode = false;
            GSDSplineN xNode = null;
            float tIntHeight = 0f;
            float tIntHeight2 = 0f;
            GSDRoadIntersection GSDRI = null;
            float T1SUB = 0f;
            float T2SUB = 0f;
            bool bIntStr1_Full = false;
            bool bIntStr1_FullPrev = false;
            bool bIntStr1_FullNext = false;
            bool bIntStr2_Full = false;
            bool bIntStr2_FullPrev = false;
            bool bIntStr2_FullNext = false;
            Vector3 tVect3 = default(Vector3);
            //			bool bStarted = false;
            //			bool T3Added = false;
            List<int[]> tXYs = new List<int[]>();
            float TreeClearDist = _spline.tRoad.clearTreesDistance;
            if (TreeClearDist < _spline.tRoad.RoadWidth())
            {
                TreeClearDist = _spline.tRoad.RoadWidth();
            }
            GSD.Roads.GSDRoadUtil.Construction2DRect tRect = null;
            float tGrade = 0f;
            for (float index = StartMin; index < FinalMax; index += tStep)
            {
                if (_spline.tRoad.isHeightModificationEnabled)
                {
                    if (index > 1f)
                    {
                        break;
                    }
                    tNext = index + tStep;
                    if (tNext > 1f)
                    {
                        break;
                    }

                    _spline.GetSplineValue_Both(index, out tVect1, out POS1);

                    if (tNext <= 1f)
                    {
                        _spline.GetSplineValue_Both(tNext, out tVect2, out POS2);
                    }
                    else
                    {
                        _spline.GetSplineValue_Both(1f, out tVect2, out POS2);
                    }

                    //Determine if intersection:
                    bIsPastInter = false;   //If past intersection
                    tIntStrength = _spline.IntersectionStrength(ref tVect1, ref tIntHeight, ref GSDRI, ref bIsPastInter, ref index, ref xNode);
                    //					if(IsApproximately(tIntStrength,1f,0.001f) || tIntStrength > 1f){
                    //						bMaxIntersection = true;
                    //					}else{
                    //						bMaxIntersection = false;	
                    //					}	
                    //					bFirstInterNode = false;	

                    tIntStrength2 = _spline.IntersectionStrength(ref tVect2, ref tIntHeight2, ref GSDRI, ref bIsPastInter, ref index, ref xNode);
                    if (tIntStrength2 > 1f)
                    {
                        tIntStrength2 = 1f;
                    }

                    T1SUB = tVect1.y;
                    T2SUB = tVect2.y;

                    if (tIntStrength > 1f)
                    {
                        tIntStrength = 1f;
                    }
                    if (tIntStrength >= 0f)
                    {// || IsApproximately(tIntStrength,0f,0.01f)){
                        if (GSDRootUtil.IsApproximately(tIntStrength, 1f, 0.01f))
                        {
                            T1SUB = tIntHeight;
                            bIntStr1_Full = true;
                            bIntStr1_FullNext = false;
                        }
                        else
                        {
                            bIntStr1_Full = false;
                            bIntStr1_FullNext = (tIntStrength2 >= 1f);
                            if (!GSDRootUtil.IsApproximately(tIntStrength, 0f, 0.01f))
                            {
                                T1SUB = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * tVect1.y);
                            }
                            //						if(tIntStrength <= 0f){ T1SUB = (tIntStrength*tIntHeight) + ((1-tIntStrength)*tVect1.y); }
                        }

                        if ((bIntStr1_Full && !bIntStr1_FullPrev) || (!bIntStr1_Full && bIntStr1_FullNext))
                        {
                            tGrade = _spline.GetCurrentNode(index).GradeToPrevValue;
                            if (tGrade < 0f)
                            {
                                T1SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T1SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, tGrade / 20f);
                            }

                            //							if(tGrade < 0f){
                            //								T1SUB *= -1f;
                            //							}
                        }
                        else if (bIntStr1_Full && !bIntStr1_FullNext)
                        {
                            tGrade = _spline.GetCurrentNode(index).GradeToNextValue;
                            if (tGrade < 0f)
                            {
                                T1SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T1SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, tGrade / 20f);
                            }
                            //							if(tGrade < 0f){
                            //								T1SUB *= -1f;
                            //							}
                        }
                        else
                        {
                            T1SUB -= 0.02f;
                        }
                        bIntStr1_FullPrev = bIntStr1_Full;
                    }

                    if (tIntStrength2 >= 0f || GSDRootUtil.IsApproximately(tIntStrength2, 0f, 0.01f))
                    {
                        //					if(!IsApproximately(tIntStrength,1f,0.01f)){ 
                        if (GSDRootUtil.IsApproximately(tIntStrength, 1f, 0.01f))
                        {
                            bIntStr2_Full = true;
                            T2SUB = tIntHeight2;
                        }
                        else
                        {
                            bIntStr2_Full = false;
                            if (!GSDRootUtil.IsApproximately(tIntStrength2, 0f, 0.01f))
                            {
                                T2SUB = (tIntStrength2 * tIntHeight) + ((1 - tIntStrength2) * tVect2.y);
                            }
                            //						if(tIntStrength2 <= 0f){ T2SUB = (tIntStrength2*tIntHeight) + ((1-tIntStrength2)*tVect2.y); }
                        }

                        if ((bIntStr2_Full && !bIntStr2_FullPrev))
                        {
                            tGrade = _spline.GetCurrentNode(index).GradeToPrevValue;
                            if (tGrade < 0f)
                            {
                                T2SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T2SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, tGrade / 20f);
                            }
                            //							T2SUB -= tIntHeight2 - tVect2.y;
                        }
                        else if (bIntStr2_Full && !bIntStr2_FullNext)
                        {
                            tGrade = _spline.GetCurrentNode(index).GradeToNextValue;
                            if (tGrade < 0f)
                            {
                                T2SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T2SUB -= Mathf.Lerp(0.02f, GSDRI.gradeMod, tGrade / 20f);
                            }
                            //							if(tGrade < 0f){
                            //								T2SUB *= -1f;
                            //							}
                            //							T2SUB -= tIntHeight2 - tVect2.y;
                        }
                        else if (!bIntStr2_Full)
                        {
                            if (tNext + tStep < 1f)
                            {
                                tVect3 = _spline.GetSplineValue(tNext + tStep, false);
                                tIntStrength2 = _spline.IntersectionStrength(ref tVect3, ref tIntHeight2, ref GSDRI, ref bIsPastInter, ref index, ref xNode);
                            }
                            else
                            {
                                tIntStrength2 = 0f;
                            }

                            if (tIntStrength2 >= 1f)
                            {
                                T2SUB -= 0.06f;
                            }
                            else
                            {
                                T2SUB -= 0.02f;
                            }
                        }
                        else
                        {
                            T2SUB -= 0.02f;
                        }
                        bIntStr2_FullPrev = bIntStr2_Full;
                    }

                    fValue1 = index - fValueMod;
                    fValue2 = index + fValueMod;
                    if (fValue1 < 0)
                    {
                        fValue1 = 0;
                    }
                    if (fValue2 > 1)
                    {
                        fValue2 = 1;
                    }

                    tXYs.Add(CreateTris(fValue1, fValue2, ref tVect1, ref POS1, ref tVect2, ref POS2, Sep, ref TBMList, ref T1SUB, ref T2SUB, ref _TTD, HeightSep));

                    //Details and trees:
                    tRect = SetDetailCoords(index, ref tVect1, ref POS1, ref tVect2, ref POS2, _spline.tRoad.clearDetailsDistance, TreeClearDist, ref _TTD, ref _spline);
                    if (_spline.tRoad.isTreeModificationEnabled && tRect != null)
                    {
                        TreerectList.Add(tRect);
                    }
                }
                else
                {
                    if (index > 1f)
                    {
                        break;
                    }
                    tNext = index + tStep;
                    if (tNext > 1f)
                    {
                        break;
                    }

                    _spline.GetSplineValue_Both(index, out tVect1, out POS1);

                    if (tNext <= 1f)
                    {
                        _spline.GetSplineValue_Both(tNext, out tVect2, out POS2);
                    }
                    else
                    {
                        _spline.GetSplineValue_Both(1f, out tVect2, out POS2);
                    }

                    //Details and trees:
                    tRect = SetDetailCoords(index, ref tVect1, ref POS1, ref tVect2, ref POS2, _spline.tRoad.clearDetailsDistance, TreeClearDist, ref _TTD, ref _spline);
                    if (_spline.tRoad.isTreeModificationEnabled && tRect != null)
                    {
                        TreerectList.Add(tRect);
                    }
                }
            }

            GSDRootUtil.StartProfiling(_spline.tRoad, "DoRectsTree");
            if (_spline.tRoad.isTreeModificationEnabled && TreerectList != null && TreerectList.Count > 0)
            {
                int tCount = _TTD.TreeSize;
                int jCount = TreerectList.Count;
                Vector3 tVect3D = default(Vector3);
                Vector2 tVect2D = default(Vector2);
                TreeInstance tTree;
                for (int i = 0; i < tCount; i++)
                {
                    tTree = _TTD.TreesCurrent[i];

                    tVect3D = tTree.position;
                    tVect3D.x *= _TTD.TerrainSize.z;
                    tVect3D.y *= _TTD.TerrainSize.y;
                    tVect3D.z *= _TTD.TerrainSize.x;
                    tVect3D += _TTD.TerrainPos;
                    tVect2D.x = tVect3D.x;
                    tVect2D.y = tVect3D.z;

                    for (int j = 0; j < jCount; j++)
                    {
                        if (TreerectList[j].Contains(ref tVect2D))
                        {
                            _TTD.TreesOld.Add(_TTD.TreesCurrent[i]);
                            tTree = _TTD.TreesCurrent[i];
                            tTree.prototypeIndex = -2;
                            _TTD.TreesCurrent[i] = tTree;
                            _TTD.TreesI += 1;
                            break;
                        }
                    }
                }
                _TTD.TreesCurrent.RemoveAll(item => item.prototypeIndex < -1);
            }
            GSDRootUtil.EndProfiling(_spline.tRoad);

            if (!_spline.tRoad.isHeightModificationEnabled)
            {
                return;
            }

            //			//Temp testing:
            //			tSpline.mNodes[22].tTriList = new List<GSD.Roads.GSDRoadUtil.Construction3DTri>();
            //			int tCount = triList.Count;
            //			for(int i=0;i<tCount;i++){
            //				tSpline.mNodes[22].tTriList.Add(triList[i]);	
            //			}
            //			tSpline.mNodes[22].tHMList = new List<Vector3>();


            float tFloat = -1f;
            Sep = _spline.tRoad.RoadWidth() * 1.5f;
            int k = 0;
            int[] tXY = null;
            int tXYsCount = tXYs.Count;
            bool bIsBridge = false;
            bool bIsTunnel = false;
            for (float index = StartMin; index < FinalMax; index += tStep)
            {
                if (TBMList.Count > 0)
                {
                    if (TBMList[0].MaxI < index)
                    {
                        CleanupTris(index, ref TBMList);
                    }
                }
                else
                {
                    break;
                }

                //If in bridg mode:
                if (_spline.IsInBridgeTerrain(index))
                {
                    bIsBridge = true;
                }
                else
                {
                    bIsBridge = false;
                }
                //If in tunnel mode:
                if (_spline.IsInTunnelTerrain(index))
                {
                    bIsTunnel = true;
                }
                else
                {
                    bIsTunnel = false;
                }

                if (k < tXYsCount)
                {
                    tXY = tXYs[k];
                    tFloat = ProcessCoordinateGrabber(ref index, ref _spline, ref _TTD, ref TBMList, ref tXY, bIsBridge, bIsTunnel);
                    if (!GSDRootUtil.IsApproximately(tFloat, 0f, 0.0001f))
                    {
                        _spline.HeightHistory.Add(new KeyValuePair<float, float>(index, tFloat));
                    }
                }
                else
                {
                    break;
                }
                k += 1;
            }
        }


        private static void CleanupTris(float _currentI, ref List<TerrainBoundsMaker> _terrainBoundsList)
        {
            int mCount = _terrainBoundsList.Count;
            int LastIndexToRemove = -1;
            for (int index = 0; index < mCount; index++)
            {
                if (_terrainBoundsList[index].MaxI < _currentI)
                {
                    LastIndexToRemove = index;
                }
                else
                {
                    break;
                }
            }
            if (LastIndexToRemove >= 0)
            {
                _terrainBoundsList.RemoveRange(0, LastIndexToRemove);
            }
            //			
            //			mCount = rectList.Count;
            //			LastIndexToRemove = -1;
            //			for(int i=0;i<mCount;i++){
            //				if(tList[i].MaxI < CurrentI){
            //					LastIndexToRemove = i;
            //				}else{
            //					break;
            //				}
            //			}
            //			if(LastIndexToRemove >= 0){
            //				rectList.RemoveRange(0,LastIndexToRemove);
            //			}
        }


        private static int[] CreateTris(float _i, float _i2, ref Vector3 _vect1, ref Vector3 _POS1, ref Vector3 _vect2, ref Vector3 _POS2, float _sep, ref List<TerrainBoundsMaker> _list, ref float _T1SUB, ref float _T2SUB, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD, float _heightSep)
        {
            Vector3 lVect1 = (_vect1 + new Vector3(_sep * -_POS1.normalized.z, 0, _sep * _POS1.normalized.x));
            Vector3 rVect1 = (_vect1 + new Vector3(_sep * _POS1.normalized.z, 0, _sep * -_POS1.normalized.x));
            Vector3 lVect2 = (_vect2 + new Vector3(_sep * -_POS2.normalized.z, 0, _sep * _POS2.normalized.x));
            Vector3 rVect2 = (_vect2 + new Vector3(_sep * _POS2.normalized.z, 0, _sep * -_POS2.normalized.x));

            lVect1.y = _T1SUB;
            rVect1.y = _T1SUB;
            lVect2.y = _T2SUB;
            rVect2.y = _T2SUB;

            TerrainBoundsMaker TBM = new TerrainBoundsMaker();
            TBM.triList = new List<GSD.Roads.GSDRoadUtil.Construction3DTri>();

            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(lVect1, rVect1, lVect2, _i, _i2));
            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(lVect2, rVect1, rVect2, _i, _i2));

            Vector3 lVect1far = (_vect1 + new Vector3(_heightSep * -_POS1.normalized.z, 0, _heightSep * _POS1.normalized.x));
            Vector3 rVect1far = (_vect1 + new Vector3(_heightSep * _POS1.normalized.z, 0, _heightSep * -_POS1.normalized.x));
            Vector3 lVect2far = (_vect2 + new Vector3(_heightSep * -_POS2.normalized.z, 0, _heightSep * _POS2.normalized.x));
            Vector3 rVect2far = (_vect2 + new Vector3(_heightSep * _POS2.normalized.z, 0, _heightSep * -_POS2.normalized.x));

            lVect1far.y = lVect1.y;
            lVect2far.y = lVect2.y;
            rVect1far.y = rVect1.y;
            rVect2far.y = rVect2.y;

            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(lVect1far, lVect1, lVect2far, _i, _i2));
            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(lVect2far, lVect1, lVect2, _i, _i2));
            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(rVect1, rVect1far, rVect2, _i, _i2));
            TBM.triList.Add(new GSD.Roads.GSDRoadUtil.Construction3DTri(rVect2, rVect1far, rVect2far, _i, _i2));

            TBM.constructRect = new GSD.Roads.GSDRoadUtil.Construction2DRect(new Vector2(lVect1far.x, lVect1far.z), new Vector2(rVect1far.x, rVect1far.z), new Vector2(rVect2far.x, rVect2far.z), new Vector2(lVect2far.x, lVect2far.z), 0f);
            //tRect.MinI = i;
            //tRect.MaxI = i2;

            TBM.MinI = _i;
            TBM.MaxI = _i2;

            _list.Add(TBM);

            int[] Xs = new int[4];
            int[] Ys = new int[4];

            int x1, y1;
            GetTempHeightsCoordinates(ref lVect1far, ref _TTD, out x1, out y1);
            Xs[0] = x1;
            Ys[0] = y1;
            GetTempHeightsCoordinates(ref lVect2far, ref _TTD, out x1, out y1);
            Xs[1] = x1;
            Ys[1] = y1;
            GetTempHeightsCoordinates(ref rVect1far, ref _TTD, out x1, out y1);
            Xs[2] = x1;
            Ys[2] = y1;
            GetTempHeightsCoordinates(ref rVect2far, ref _TTD, out x1, out y1);
            Xs[3] = x1;
            Ys[3] = y1;

            int Min = Mathf.Min(Xs);
            int Max = Mathf.Max(Xs);
            Xs[0] = Min - 2;
            Xs[2] = Max + 2;
            Min = Mathf.Min(Ys);
            Max = Mathf.Max(Ys);
            Xs[1] = Min - 2;
            Xs[3] = Max + 2;

            return Xs;
        }


        private static GSD.Roads.GSDRoadUtil.Construction2DRect SetDetailCoords(float _param, ref Vector3 _vect1, ref Vector3 _POS1, ref Vector3 _vect2, ref Vector3 POS2, float _sep, float _treeSep, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD, ref GSDSplineC _spline)
        {
            Vector3 lVect1far = default(Vector3);
            Vector3 rVect1far = default(Vector3);
            Vector3 lVect2far = default(Vector3);
            Vector3 rVect2far = default(Vector3);

            bool isInBridge = _spline.IsInBridgeTerrain(_param);
            bool isInTunnel = _spline.IsInTunnelTerrain(_param);
            int x2, y2, x3, y3;
            GetTempHeightsCoordinates(ref _vect1, ref _TTD, out x2, out y2);
            if (x2 >= _TTD.HM)
            {
                x2 = -1;
            }
            if (y2 >= _TTD.HM)
            {
                y2 = -1;
            }
            if (x2 < 0)
            {
                x2 = -1;
            }
            if (y2 < 0)
            {
                y2 = -1;
            }
            if (x2 == -1)
            {
                return null;
            }
            if (y2 == -1)
            {
                return null;
            }

            float tDiff1 = ((_TTD.heights[x2, y2] * (float) _TTD.TerrainSize.y) - _vect1.y);
            GetTempHeightsCoordinates(ref _vect2, ref _TTD, out x3, out y3);
            if (x3 >= _TTD.HM)
            {
                x3 = -1;
            }
            if (y3 >= _TTD.HM)
            {
                y3 = -1;
            }
            if (x3 < 0)
            {
                x3 = -1;
            }
            if (y3 < 0)
            {
                y3 = -1;
            }
            if (x3 == -1)
            {
                return null;
            }
            if (y3 == -1)
            {
                return null;
            }
            float tDiff2 = ((_TTD.heights[x3, y3] * (float) _TTD.TerrainSize.y) - _vect2.y);



            GSD.Roads.GSDRoadUtil.Construction2DRect tRect = null;
            if (_spline.tRoad.isTreeModificationEnabled)
            {
                bool isQuit = false;
                if (x2 == -1)
                {
                    isQuit = true;
                }
                if (y2 == -1)
                {
                    isQuit = true;
                }

                if (isInBridge && !isQuit)
                {
                    if (tDiff1 < 0f)
                    {
                        tDiff1 *= -1f;
                    }
                    if (tDiff2 < 0f)
                    {
                        tDiff2 *= -1f;
                    }
                    if (tDiff1 > _spline.tRoad.clearTreesDistanceHeight)
                    {
                        isQuit = true;
                    }
                    if (tDiff2 > _spline.tRoad.clearTreesDistanceHeight)
                    {
                        isQuit = true;
                    }
                }
                if (isInTunnel && !isQuit)
                {
                    if (tDiff1 < 0f)
                    {
                        if ((tDiff1 * -1f) > _spline.tRoad.clearTreesDistanceHeight)
                        {
                            isQuit = true;
                        }
                    }
                    else
                    {
                        if (tDiff1 > (_spline.tRoad.clearTreesDistanceHeight * 0.1f))
                        {
                            isQuit = true;
                        }
                    }
                    if (tDiff2 < 0f)
                    {
                        if ((tDiff2 * -1f) > _spline.tRoad.clearTreesDistanceHeight)
                        {
                            isQuit = true;
                        }
                    }
                    else
                    {
                        if (tDiff2 > (_spline.tRoad.clearTreesDistanceHeight * 0.1f))
                        {
                            isQuit = true;
                        }
                    }
                }

                if (!isQuit)
                {
                    _treeSep = _treeSep * 0.5f;
                    lVect1far = (_vect1 + new Vector3(_treeSep * -_POS1.normalized.z, 0, _treeSep * _POS1.normalized.x));
                    rVect1far = (_vect1 + new Vector3(_treeSep * _POS1.normalized.z, 0, _treeSep * -_POS1.normalized.x));
                    lVect2far = (_vect2 + new Vector3(_treeSep * -POS2.normalized.z, 0, _treeSep * POS2.normalized.x));
                    rVect2far = (_vect2 + new Vector3(_treeSep * POS2.normalized.z, 0, _treeSep * -POS2.normalized.x));
                    tRect = new GSD.Roads.GSDRoadUtil.Construction2DRect(new Vector2(lVect1far.x, lVect1far.z), new Vector2(rVect1far.x, rVect1far.z), new Vector2(rVect2far.x, rVect2far.z), new Vector2(lVect2far.x, lVect2far.z), 0f);
                }
            }

            if (_spline.tRoad.isDetailModificationEnabled)
            {
                if (isInBridge || isInTunnel)
                {
                    if (tDiff1 < 0f)
                    {
                        tDiff1 *= -1f;
                    }
                    if (tDiff2 < 0f)
                    {
                        tDiff2 *= -1f;
                    }

                    bool isQuit = false;
                    if (x2 == -1)
                    {
                        isQuit = true;
                    }
                    if (y2 == -1)
                    {
                        isQuit = true;
                    }

                    if (tDiff1 > _spline.tRoad.clearDetailsDistanceHeight)
                    {
                        isQuit = true;
                    }
                    if (tDiff2 > _spline.tRoad.clearDetailsDistanceHeight)
                    {
                        isQuit = true;
                    }

                    if (isQuit)
                    {
                        return tRect;
                    }
                }

                _sep = _sep * 0.5f;

                lVect1far = (_vect1 + new Vector3(_sep * -_POS1.normalized.z, 0, _sep * _POS1.normalized.x));
                rVect1far = (_vect1 + new Vector3(_sep * _POS1.normalized.z, 0, _sep * -_POS1.normalized.x));
                lVect2far = (_vect2 + new Vector3(_sep * -POS2.normalized.z, 0, _sep * POS2.normalized.x));
                rVect2far = (_vect2 + new Vector3(_sep * POS2.normalized.z, 0, _sep * -POS2.normalized.x));

                int[] Xs = new int[4];
                int[] Ys = new int[4];

                int x1, y1;
                GetTempDetailsCoordinates(ref lVect1far, ref _TTD, out x1, out y1);
                Xs[0] = x1;
                Ys[0] = y1;
                GetTempDetailsCoordinates(ref lVect2far, ref _TTD, out x1, out y1);
                Xs[1] = x1;
                Ys[1] = y1;
                GetTempDetailsCoordinates(ref rVect1far, ref _TTD, out x1, out y1);
                Xs[2] = x1;
                Ys[2] = y1;
                GetTempDetailsCoordinates(ref rVect2far, ref _TTD, out x1, out y1);
                Xs[3] = x1;
                Ys[3] = y1;

                //if(TTD.DetailLayersCount == 1 && x1 > 0 && y1 > 0)
                //{
                //	Debug.Log(Xs[0]+","+Ys[0] + " " + Xs[1]+","+Ys[1]);
                //}

                int MinX = Mathf.Min(Xs);
                int MinY = Mathf.Min(Ys);
                int MaxX = Mathf.Max(Xs);
                int MaxY = Mathf.Max(Ys);

                if (MinX >= _TTD.DetailMaxIndex)
                {
                    MinX = _TTD.DetailMaxIndex - 1;
                }
                if (MinY >= _TTD.DetailMaxIndex)
                {
                    MinY = _TTD.DetailMaxIndex - 1;
                }
                if (MaxX >= _TTD.DetailMaxIndex)
                {
                    MaxX = _TTD.DetailMaxIndex - 1;
                }
                if (MaxY >= _TTD.DetailMaxIndex)
                {
                    MaxY = _TTD.DetailMaxIndex - 1;
                }

                if (MinX < 0)
                {
                    MinX = 0;
                }
                if (MinY < 0)
                {
                    MinY = 0;
                }
                if (MaxX < 0)
                {
                    MaxX = 0;
                }
                if (MaxY < 0)
                {
                    MaxY = 0;
                }

                //int DetailI = 0;
                GSDRootUtil.StartProfiling(_spline.tRoad, "Dorectsdetails");
                int tInt = 0;
                for (int index = MinX; index <= MaxX; index++)
                {
                    for (int k = MinY; k <= MaxY; k++)
                    {
                        //Bitfield for identification:
                        tInt = k;
                        tInt = tInt << 16;
                        tInt = tInt | (ushort) index;
                        if (!_TTD.DetailHasProcessed.Contains(tInt))
                        {
                            //for(int h=0;h<TTD.DetailLayersCount;h++)
                            //{
                            //  if(TTD.DetailLayersSkip.Contains(h))
                            //  {
                            //      continue;
                            //  }
                            //	if(!TTD.DetailHasProcessed[h][i,k]){// && TTD.DetailValues[h][i,k] > 0){

                            _TTD.MainDetailsX.Add((ushort) index);
                            _TTD.MainDetailsY.Add((ushort) k);

                            //DetailI = TTD.DetailsI[h];

                            //TTD.DetailsX[h].Add((ushort)i);
                            //TTD.DetailsY[h].Add((ushort)k);

                            //TTD.DetailsX[h][DetailI] = (ushort)i;	
                            //TTD.DetailsY[h][DetailI] = (ushort)k;
                            //TTD.OldDetailsValue[h][DetailI] = (ushort)TTD.DetailValues[h][i,k];
                            //TTD.DetailValues[h][i,k] = 0;

                            //TTD.DetailsI[h]+=1;

                            //}
                            _TTD.DetailHasProcessed.Add(tInt);
                        }
                    }
                }
                GSDRootUtil.EndProfiling(_spline.tRoad);
            }

            return tRect;
        }


        private static float ProcessCoordinateGrabber(ref float _param, ref GSDSplineC _spline, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD, ref List<TerrainBoundsMaker> _terrainList, ref int[] _XY, bool _isBridge, bool _isTunnel)
        {
            int MinX = _XY[0];
            int MinY = _XY[1];
            int MaxX = _XY[2];
            int MaxY = _XY[3];

            if (MinX >= _TTD.TerrainMaxIndex)
            {
                MinX = _TTD.TerrainMaxIndex - 1;
            }
            if (MinY >= _TTD.TerrainMaxIndex)
            {
                MinY = _TTD.TerrainMaxIndex - 1;
            }
            if (MaxX >= _TTD.TerrainMaxIndex)
            {
                MaxX = _TTD.TerrainMaxIndex - 1;
            }
            if (MaxY >= _TTD.TerrainMaxIndex)
            {
                MaxY = _TTD.TerrainMaxIndex - 1;
            }

            if (MinX < 0)
            {
                MinX = 0;
            }
            if (MinY < 0)
            {
                MinY = 0;
            }
            if (MaxX < 0)
            {
                MaxX = 0;
            }
            if (MaxY < 0)
            {
                MaxY = 0;
            }

            Vector3 xVect = default(Vector3);
            bool bAdjusted = false;
            float tHeight = -1f;
            float tReturnFloat = 0f;
            //			int dX = 0;
            //			int dY = 0;
            //			int tdX = 0;
            //			int tdY = 0;
            //			bool bOneHit = false;

            for (int index = MinX; index <= MaxX; index++)
            {
                for (int k = MinY; k <= MaxY; k++)
                {
                    if (_TTD.tHeights[index, k] != true)
                    {
                        if (_TTD.cX.Length <= _TTD.cI)
                        {
                            break;
                        }

                        xVect = ConvertTerrainCoordToWorldVect(index, k, _TTD.heights[index, k], ref _TTD);
                        AdjustedTerrainVectTri(ref _param, out bAdjusted, out tHeight, ref xVect, ref _terrainList, _isBridge, _isTunnel);

                        if (bAdjusted)
                        {
                            tHeight -= _spline.tRoad.matchTerrainSubtraction;
                            if (tHeight < 0f)
                            {
                                tHeight = 0f;
                            }
                            xVect.y = tHeight;
                            tHeight = ((tHeight) / _TTD.TerrainSize.y);

                            //Set height values:
                            _TTD.tHeights[index, k] = true;
                            _TTD.cX[_TTD.cI] = (ushort) index;
                            _TTD.cY[_TTD.cI] = (ushort) k;
                            _TTD.oldH[_TTD.cI] = _TTD.heights[index, k];
                            _TTD.heights[index, k] = tHeight;
                            _TTD.cI += 1;

                            tReturnFloat = xVect.y;
                            //							bOneHit = true;
                        }
                    }
                    else
                    {
                        xVect = ConvertTerrainCoordToWorldVect(index, k, _TTD.heights[index, k], ref _TTD);
                        AdjustedTerrainVectTri(ref _param, out bAdjusted, out tHeight, ref xVect, ref _terrainList, _isBridge, _isTunnel);

                        if (bAdjusted)
                        {
                            tHeight -= _spline.tRoad.matchTerrainSubtraction;
                            if (tHeight < 0f)
                            {
                                tHeight = 0f;
                            }
                            tReturnFloat = tHeight;
                            //							bOneHit = true;
                        }
                    }
                }
            }

            if (_isBridge && GSDRootUtil.IsApproximately(tReturnFloat, 0f, 0.0001f))
            {
                tReturnFloat = _spline.GetSplineValue(_param, false).y;
            }

            return tReturnFloat;
        }


        private static Vector3 ConvertTerrainCoordToWorldVect(int _x, int _y, float _height, ref GSD.Roads.GSDTerraforming.TempTerrainData _TTD)
        {
            //Get the normalized position of this game object relative to the terrain:
            float x1 = _x / ((float) _TTD.HM - 1f);
            x1 = x1 * _TTD.TerrainSize.x;

            float z1 = _y / ((float) _TTD.HM - 1f);
            z1 = z1 * _TTD.TerrainSize.z;

            float y1 = _height * _TTD.TerrainSize.y;

            Vector3 xVect = new Vector3(z1, y1, x1);
            xVect += _TTD.TerrainPos;

            return xVect;
        }


        private static void AdjustedTerrainVectTri(ref float _param, out bool _isAdjusted, out float _height, ref Vector3 _vect, ref List<TerrainBoundsMaker> _terrainList, bool _isBridge, bool _isTunnel)
        {
            float OrigHeight = _vect.y;
            int mCount = _terrainList.Count;
            int tCount = 0;
            GSD.Roads.GSDRoadUtil.Construction3DTri tTri;
            TerrainBoundsMaker TBM;
            _isAdjusted = false;
            _height = 0f;
            Vector2 t2D = new Vector2(_vect.x, _vect.z);
            for (int index = 0; index < mCount; index++)
            {
                TBM = _terrainList[index];
                if (_param < TBM.MinI)
                {
                    return;
                }
                if (_param > TBM.MaxI)
                {
                    continue;
                }
                if (TBM.constructRect.Contains(ref t2D))
                {
                    tCount = TBM.triList.Count;
                    for (int k = 0; k < tCount; k++)
                    {
                        tTri = TBM.triList[k];
                        if (tTri.Contains2D(ref t2D))
                        {
                            _height = tTri.LinePlaneIntersection(ref _vect).y;
                            if (_isBridge)
                            {
                                if (OrigHeight > (_height - 0.03f))
                                {
                                    _height -= 0.03f;
                                    _isAdjusted = true;
                                    return;
                                }
                            }
                            else if (_isTunnel)
                            {
                                if (OrigHeight < (_height + 0.03f))
                                {
                                    _height += 0.03f;
                                    _isAdjusted = true;
                                    return;
                                }
                            }
                            else
                            {
                                _isAdjusted = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }


    public static class GSDRoadCreationT
    {
        #region "Road Prelim"
        //Privatized for obfuscate:
        public static void RoadJobPrelim(ref GSDRoad _road)
        {
            RoadJobPrelimDo(ref _road);
        }


        private static void RoadJobPrelimDo(ref GSDRoad _road)
        {
            GSDSplineC tSpline = _road.spline;
            //Road,shoulder,ramp and lane widths:
            float RoadWidth = _road.RoadWidth();
            float ShoulderWidth = _road.shoulderWidth;
            float RoadSeperation = RoadWidth / 2f;
            float RoadSeperation_NoTurn = RoadWidth / 2f;
            float ShoulderSeperation = RoadSeperation + ShoulderWidth;
            float LaneWidth = _road.laneWidth;
            float RoadSep1Lane = (RoadSeperation + (LaneWidth * 0.5f));
            float RoadSep2Lane = (RoadSeperation + (LaneWidth * 1.5f));
            float ShoulderSep1Lane = (ShoulderSeperation + (LaneWidth * 0.5f));
            float ShoulderSep2Lane = (ShoulderSeperation + (LaneWidth * 1.5f));

            //Vector3 buffers used in construction:
            Vector3 rVect = default(Vector3);
            Vector3 lVect = default(Vector3);
            Vector3 ShoulderR_rVect = default(Vector3);
            Vector3 ShoulderR_lVect = default(Vector3);
            Vector3 ShoulderL_rVect = default(Vector3);
            Vector3 ShoulderL_lVect = default(Vector3);
            Vector3 RampR_R = default(Vector3);
            Vector3 RampR_L = default(Vector3);
            Vector3 RampL_R = default(Vector3);
            Vector3 RampL_L = default(Vector3);
#pragma warning disable CS0219
            float ShoulderR_OuterAngle = 0f;
            float ShoulderL_OuterAngle = 0f;

            //			Vector3 ShoulderR_OuterDirection = default(Vector3);
            //			Vector3 ShoulderL_OuterDirection = default(Vector3);

            //Previous temp storage values:
            Vector3 tVect_Prev = default(Vector3);
            Vector3 rVect_Prev = default(Vector3);
            Vector3 lVect_Prev = default(Vector3);
            Vector3 ShoulderR_PrevLVect = default(Vector3);
            Vector3 ShoulderL_PrevRVect = default(Vector3);
            Vector3 ShoulderR_PrevRVect = default(Vector3);
            Vector3 ShoulderL_PrevLVect = default(Vector3);
            //			Vector3 ShoulderR_PrevRVect2 = default(Vector3);	//Prev storage of shoulder variable (2 step history).
            //			Vector3 ShoulderL_PrevLVect2 = default(Vector3);	//Prev storage of shoulder variable (2 step history).
            //			Vector3 ShoulderR_PrevRVect3 = default(Vector3);	//Prev storage of shoulder variable (3 step history).
            //			Vector3 ShoulderL_PrevLVect3 = default(Vector3);	//Prev storage of shoulder variable (3 step history).
            Vector3 RampR_PrevR = default(Vector3);
            Vector3 RampR_PrevL = default(Vector3);
            Vector3 RampL_PrevR = default(Vector3);
            Vector3 RampL_PrevL = default(Vector3);
            //			Vector3 ShoulderR_OuterDirectionPrev = default(Vector3);	//Prev storage of outer shoulder direction (euler).
            //			Vector3 ShoulderL_OuterDirectionPrev = default(Vector3);	//Prev storage of outer shoulder direction (euler).

            //Height and angle variables, used to change certain parameters of road depending on past & future angle and height changes.
            //			float tAngle = 0f;
            //			float OrigStep = 0.06f;
            float Step = _road.roadDefinition / tSpline.distance;
            //			float AngleStep = 5f;
            Vector3 tHeight0 = new Vector3(0f, 0.1f, 0f);
            //			Vector3 tHeight2 = new Vector3(0f,0.15f,0f);
            //			Vector3 tHeight1 = new Vector3(0f,0.2f,0f);
            float OuterShoulderWidthR = 0f;
            float OuterShoulderWidthL = 0f;
            float RampOuterWidthR = (OuterShoulderWidthR / 6f) + OuterShoulderWidthR;
            float RampOuterWidthL = (OuterShoulderWidthL / 6f) + OuterShoulderWidthL;
            Vector3 tVect = default(Vector3);
            Vector3 POS = default(Vector3);
            float TempY = 0f;
            //			bool bTempYWasNegative = false;
            //			Vector3 tY = new Vector3(0f,0f,0f);
            float tHeightAdded = 0f;
            //			float[] HeightChecks = new float[5];
            Vector3 gHeight = default(Vector3);

#pragma warning disable CS0219

            //Bridge variables:
            bool bIsBridge = false;
            bool bTempbridge = false;
            bool bBridgeInitial = false;
            bool bBridgeLast = false;
            float BridgeUpComing;
            //			int BridgeLIndex;
            //			int BridgeRIndex;

            //Tunnel variables:	
            bool bIsTunnel = false;
            bool bTempTunnel = false;
            bool bTunnelInitial = false;
            bool bTunnelLast = false;
            float TunnelUpComing = 0f;
            //			int TunnelLIndex;
            //			int TunnelRIndex;

            //Intersection variables:
            float tIntHeight = 0f;
            float tIntStrength = 0f;
            float tIntStrength_temp = 0f;
            //			float tIntDistCheck = 75f;
            GSDRoadIntersection GSDRI = null;
            bool bIsPastInter = false;
            bool bMaxIntersection = false;
            bool bWasPrevMaxInter = false;
            GSDSplineN xNode = null;
            float tInterSubtract = 4f;
            float tLastInterHeight = -4f;
            bool bOverrideRampR = false;
            bool bOverrideRampL = false;
            Vector3 RampR_Override = default(Vector3);
            Vector3 RampL_Override = default(Vector3);
            bool bFirstInterNode = false;
            bool bInter_PrevWasCorner = false;
            bool bInter_CurreIsCorner = false;
            bool bInter_CurreIsCornerRR = false;
            bool bInter_CurreIsCornerRL = false;
            bool bInter_CurreIsCornerLL = false;
            bool bInter_CurreIsCornerLR = false;
            bool bInter_PrevWasCornerRR = false;
            bool bInter_PrevWasCornerRL = false;
            bool bInter_PrevWasCornerLL = false;
            bool bInter_PrevWasCornerLR = false;
            Vector3 iTemp_HeightVect = default(Vector3);
            Vector3 rVect_iTemp = default(Vector3);
            Vector3 lVect_iTemp = default(Vector3);
            Vector3 ShoulderR_R_iTemp = default(Vector3);
            Vector3 ShoulderL_L_iTemp = default(Vector3);
            Vector3 RampR_R_iTemp = default(Vector3);
            Vector3 RampR_L_iTemp = default(Vector3);
            Vector3 RampL_R_iTemp = default(Vector3);
            Vector3 RampL_L_iTemp = default(Vector3);
            Vector3 tempIVect_Prev = default(Vector3);
            Vector3 tempIVect = tVect;
            bool b0LAdded = false;
            bool b1LAdded = false;
            bool b2LAdded = false;
            bool b3LAdded = false;
            bool f0LAdded = false;
            bool f1LAdded = false;
            bool f2LAdded = false;
            bool f3LAdded = false;
            bool b0RAdded = false;
            bool b1RAdded = false;
            bool b2RAdded = false;
            bool b3RAdded = false;
            bool f0RAdded = false;
            bool f1RAdded = false;
            bool f2RAdded = false;
            bool f3RAdded = false;
            bool bInterTestAddAfterR = false;
            bool bInterTestAddAfterL = false;
            //			Vector3 InterTestVect1 = default(Vector3);
            //			Vector3 InterTestVect2 = default(Vector3);
            //			Vector3 InterTestVect3 = default(Vector3);
            //			Vector3 InterTestVect4 = default(Vector3);
            bool bShoulderSkipR = false;
            bool bShoulderSkipL = false;
            bool bShrinkRoadB = false;
            bool bShrinkRoadFNext = false;
            bool bShrinkRoadF = false;
            bool bIsNextInter = false;
            GSDSplineN cNode = null;
            int NodeID = -1;
            int NodeIDPrev = -1;
            int NodeCount = tSpline.GetNodeCount();
            bool bDynamicCut = false;
            float CullDistanceSQ = (3f * RoadWidth) * (3f * RoadWidth);
            float mCornerDist = 0f;
            Vector2 CornerRR = default(Vector2);
            Vector2 CornerRL = default(Vector2);
            Vector2 CornerLR = default(Vector2);
            Vector2 CornerLL = default(Vector2);
            Vector2 rVect2D = default(Vector2);
            Vector2 lVect2D = default(Vector2);
            Vector3 tempIVect_prev = default(Vector3);
            Vector3 POS_Next = default(Vector3);
            Vector3 tVect_Next = default(Vector3);
            Vector3 rVect_Next = default(Vector3);
            Vector3 lVect_Next = default(Vector3);
            Vector3 xHeight = default(Vector3);
            bool bLRtoRR = false;
            bool bLLtoLR = false;
            bool bLine = false;
            bool bImmuneR = false;
            bool bImmuneL = false;
            bool bSpecAddedL = false;
            bool bSpecAddedR = false;
            bool bTriggerInterAddition = false;
            bool bSpecialThreeWayIgnoreR = false;
            bool bSpecialThreeWayIgnoreL = false;
            //			int eCount = -1;
            //			int eIndex = -1;
            //			int uCount = -1;
            //			int uIndex = -1;
            float bMod1 = 1.75f;
            float bMod2 = 1.25f;
            float t2DDist = -1f;
            List<Vector3> vList = null;
            List<int> eList = null;
            float tParam2 = 0f;
            float tParam1 = 0f;
            bool bRecordShoulderForNormals = false;
            bool bRecordShoulderLForNormals = false;

            //Unused for now, for later partial construction methods:
            bool bInterseOn = _road.RCS.isInterseOn;
            //			bool bBridgesOn = tRoad.RCS.bBridgesOn;
            //			if(tRoad.RCS.bRoadOn){
            bInterseOn = true;
            //			}
#pragma warning restore CS0219


            //Prelim intersection construction and profiling:
            GSDRootUtil.StartProfiling(_road, "RoadJob_Prelim_Inter");
            if (bInterseOn)
            {
                RoadJobPrelimInter(ref _road);
            }
            GSDRootUtil.EndStartProfiling(_road, "RoadPrelimForLoop");

            //Road/shoulder cuts: Init necessary since a road cut is added for the last segment after this function:
            if (_road.isRoadCutsEnabled || _road.isDynamicCutsEnabled)
            {
                _road.RCS.RoadCutNodes.Add(tSpline.mNodes[0]);
            }
            if (_road.isShoulderCutsEnabled || _road.isDynamicCutsEnabled)
            {
                _road.RCS.ShoulderCutsLNodes.Add(tSpline.mNodes[0]);
                _road.RCS.ShoulderCutsRNodes.Add(tSpline.mNodes[0]);
            }

            //Start initializing the loop. Convuluted to handle special control nodes, so roads don't get rendered where they aren't supposed to, while still preserving the proper curvature.
            float FinalMax = 1f;
            float StartMin = 0f;
            if (tSpline.bSpecialEndControlNode)
            {   //If control node, start after the control node:
                FinalMax = tSpline.mNodes[tSpline.GetNodeCount() - 2].tTime;
            }
            if (tSpline.bSpecialStartControlNode)
            {   //If ends in control node, end construction before the control node:
                StartMin = tSpline.mNodes[1].tTime;
            }
            bool bFinalEnd = false;
            float RoadConnection_StartMin1 = StartMin;  //Storage of incremental start values for the road connection mesh construction at the end of this function.
            float RoadConnection_FinalMax1 = FinalMax;  //Storage of incremental end values for the road connection mesh construction at the end of this function.
            if (tSpline.bSpecialEndNode_IsStart_Delay)
            {
                StartMin += (tSpline.SpecialEndNodeDelay_Start / tSpline.distance); //If there's a start delay (in meters), delay the start of road construction: Due to special control nodes for road connections or 3 way intersections.
            }
            else if (tSpline.bSpecialEndNode_IsEnd_Delay)
            {
                FinalMax -= (tSpline.SpecialEndNodeDelay_End / tSpline.distance);   //If there's a end delay (in meters), cut early the end of road construction: Due to special control nodes for road connections or 3 way intersections.
            }
            //			float RoadConnection_StartMin2 = StartMin;	//Storage of incremental start values for the road connection mesh construction at the end of this function.
            //			float RoadConnection_FinalMax2 = FinalMax; 	//Storage of incremental end values for the road connection mesh construction at the end of this function.
            float i = StartMin;

            //			int StartIndex = tSpline.GetClosestRoadDefIndex(StartMin,true,false);
            //			int EndIndex = tSpline.GetClosestRoadDefIndex(FinalMax,false,true);
            //			float cDist = 0f;
            bool kSkip = true;
            bool kSkipFinal = false;
            int kCount = 0;
            int vCount = kCount;
            int kFinalCount = tSpline.RoadDefKeysArray.Length;
            int spamcheckmax1 = 18000;
            int spamcheck1 = 0;

            if (GSDRootUtil.IsApproximately(StartMin, 0f, 0.0001f))
            {
                kSkip = false;
            }
            if (GSDRootUtil.IsApproximately(FinalMax, 1f, 0.0001f))
            {
                kSkipFinal = true;
            }

            //If startmin > 0 then kcount needs to start at proper road def
            //			bool bStartMinEnabled = false;
            int StartMinIndex1 = 0;

            if (StartMin > 0f)
            {
                kCount = tSpline.GetClosestRoadDefIndex(StartMin, true, false);
                //				bStartMinEnabled = true;
                StartMinIndex1 = 1;
            }

            while (!bFinalEnd && spamcheck1 < spamcheckmax1)
            {
                spamcheck1++;

                if (kSkip)
                {
                    i = StartMin;
                    kSkip = false;
                }
                else
                {
                    if (kCount >= kFinalCount)
                    {
                        i = FinalMax;
                        if (kSkipFinal)
                        { break; }
                    }
                    else
                    {
                        i = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[kCount]);
                        kCount += 1;
                    }
                }

                if (i > 1f)
                {
                    break;
                }
                if (i < 0f)
                {
                    i = 0f;
                }

                if (GSDRootUtil.IsApproximately(i, FinalMax, 0.00001f))
                {
                    bFinalEnd = true;
                }
                else if (i > FinalMax)
                {
                    if (tSpline.bSpecialEndControlNode)
                    {
                        i = FinalMax;
                        bFinalEnd = true;
                    }
                    else
                    {
                        bFinalEnd = true;
                        break;
                    }
                }
                cNode = tSpline.GetCurrentNode(i);  //Set the current node.
                NodeID = cNode.idOnSpline;          //Set the current node ID.
                if (NodeID != NodeIDPrev && (_road.isRoadCutsEnabled || _road.isDynamicCutsEnabled))
                {   //If different than the previous node id, time to make a cut, if necessary:
                    //Don't ever cut the first node, last node, intersection node, special control nodes, bridge nodes or bridge control nodes:
                    if (NodeID > StartMinIndex1 && NodeID < (NodeCount - 1) && !cNode.bIsIntersection && !cNode.bSpecialEndNode)
                    { // && !cNode.bIsBridge_PreNode && !cNode.bIsBridge_PostNode){
                        if (_road.isDynamicCutsEnabled)
                        {
                            bDynamicCut = cNode.bRoadCut;
                        }
                        else
                        {
                            bDynamicCut = true;
                        }

                        if (bDynamicCut)
                        {
                            _road.RCS.RoadCuts.Add(_road.RCS.RoadVectors.Count);            //Add the vector index to cut later.
                            _road.RCS.RoadCutNodes.Add(cNode);                              //Store the node which was at the beginning of this cut.	
                        }
                        if (_road.isShoulderCutsEnabled && bDynamicCut)
                        {   //If option shoulder cuts is on.
                            _road.RCS.ShoulderCutsL.Add(_road.RCS.ShoulderL_Vectors.Count); //Add the vector index to cut later.
                            _road.RCS.ShoulderCutsLNodes.Add(cNode);                        //Store the node which was at the beginning of this cut.
                            _road.RCS.ShoulderCutsR.Add(_road.RCS.ShoulderR_Vectors.Count); //Add the vector index to cut later.
                            _road.RCS.ShoulderCutsRNodes.Add(cNode);                        //Store the node which was at the beginning of this cut.
                        }
                    }
                }
                if (NodeID != NodeIDPrev)
                {
                    if (_road.RCS.RoadVectors.Count > 0)
                    {
                        cNode.bInitialRoadHeight = _road.RCS.RoadVectors[_road.RCS.RoadVectors.Count - 1].y;
                    }
                }
                 //Store the previous node ID for the next round. Done now with road cuts as far as this function is concerned.
                NodeIDPrev = NodeID;

                //Set all necessary intersection triggers to false:
                bInter_CurreIsCorner = false;
                bInter_CurreIsCornerRR = false;
                bInter_CurreIsCornerRL = false;
                bInter_CurreIsCornerLL = false;
                bInter_CurreIsCornerLR = false;
                b0LAdded = false;
                b1LAdded = false;
                b2LAdded = false;
                b3LAdded = false;
                f0LAdded = false;
                f1LAdded = false;
                f2LAdded = false;
                f3LAdded = false;
                b0RAdded = false;
                b1RAdded = false;
                b2RAdded = false;
                b3RAdded = false;
                f0RAdded = false;
                f1RAdded = false;
                f2RAdded = false;
                f3RAdded = false;
                bInterTestAddAfterR = false;
                bInterTestAddAfterL = false;
                bShoulderSkipR = false;
                bShoulderSkipL = false;
                bShrinkRoadB = false;
                bShrinkRoadF = false;
                bIsNextInter = false;
                if (bShrinkRoadFNext)
                {
                    bShrinkRoadFNext = false;
                    bShrinkRoadF = true;
                }
                bRecordShoulderForNormals = false;
                bRecordShoulderLForNormals = false;

                //Bridges: Note: This is convoluted due to need for triggers:
                bBridgeInitial = false;
                bBridgeLast = false;
                bTempbridge = tSpline.IsInBridge(i);
                if (!bIsBridge && bTempbridge)
                {
                    bIsBridge = true;
                    bBridgeInitial = true;
                }
                else if (bIsBridge && !bTempbridge)
                {
                    bIsBridge = false;
                }
                //Check if this is the last bridge run for this bridge:
                if (bIsBridge)
                {
                    bTempbridge = tSpline.IsInBridge(i + Step);
                    if (!bTempbridge)
                    {
                        bBridgeLast = true;
                    }
                }

                //Tunnels: Note: This is convoluted due to need for triggers:
                bTunnelInitial = false;
                bTunnelLast = false;
                bTempTunnel = tSpline.IsInTunnel(i);
                if (!bIsTunnel && bTempTunnel)
                {
                    bIsTunnel = true;
                    bTunnelInitial = true;
                }
                else if (bIsTunnel && !bTempTunnel)
                {
                    bIsTunnel = false;
                }
                //Check if this is the last Tunnel run for this Tunnel:
                if (bIsTunnel)
                {
                    bTempTunnel = tSpline.IsInTunnel(i + Step);
                    if (!bTempTunnel)
                    {
                        bTunnelLast = true;
                    }
                }

                //Master Vector3 for the current road construction location:
                tSpline.GetSplineValue_Both(i, out tVect, out POS);

                //				Profiler.EndSample();
                //				Profiler.BeginSample("Test2");

                //Detect downward or upward slope:
                TempY = POS.y;
                //				bTempYWasNegative = false;
                if (TempY < 0f)
                {
                    //					bTempYWasNegative = true;
                    TempY *= -1f;
                }
                if (tVect.y < 0f)
                {
                    tVect.y = 0f;
                }

                //Determine if intersection:
                if (bInterseOn)
                {
                    bIsPastInter = false;   //If past intersection
                    tIntStrength = _road.spline.IntersectionStrength(ref tVect, ref tIntHeight, ref GSDRI, ref bIsPastInter, ref i, ref xNode);
                    bMaxIntersection = (tIntStrength >= 1f);    //1f strength = max intersection
                    bFirstInterNode = false;
                }

                //Outer widths:
                if (bMaxIntersection && bInterseOn)
                {
                    GSDRI.signHeight = tIntHeight;
                    xNode.iConstruction.isBLane0DoneFinalThisRound = false;
                    xNode.iConstruction.isBLane1DoneFinalThisRound = false;
                    xNode.iConstruction.isBLane2DoneFinalThisRound = false;
                    xNode.iConstruction.isBLane3DoneFinalThisRound = false;
                    xNode.iConstruction.isFLane0DoneFinalThisRound = false;
                    xNode.iConstruction.isFLane1DoneFinalThisRound = false;
                    xNode.iConstruction.isFLane2DoneFinalThisRound = false;
                    xNode.iConstruction.isFLane3DoneFinalThisRound = false;
                    xNode.iConstruction.isFrontFirstRound = false;

                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        OuterShoulderWidthR = ShoulderSeperation;
                        OuterShoulderWidthL = ShoulderSeperation;
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        OuterShoulderWidthR = ShoulderSep1Lane;
                        OuterShoulderWidthL = ShoulderSep1Lane;
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (bIsPastInter)
                        {
                            OuterShoulderWidthR = ShoulderSep1Lane;
                            OuterShoulderWidthL = ShoulderSep2Lane;
                        }
                        else
                        {
                            OuterShoulderWidthR = ShoulderSep2Lane;
                            OuterShoulderWidthL = ShoulderSep1Lane;
                        }
                    }
                }
                else
                {
                    if (TempY < 0.5f || bIsBridge || bIsTunnel)
                    {
                        OuterShoulderWidthR = ShoulderSeperation;
                        OuterShoulderWidthL = ShoulderSeperation;
                    }
                    else
                    {
                        OuterShoulderWidthR = ShoulderSeperation + (TempY * 0.05f);
                        OuterShoulderWidthL = ShoulderSeperation + (TempY * 0.05f);
                    }
                }

                if (bIsBridge)
                { //No ramps for bridges:
                    RampOuterWidthR = OuterShoulderWidthR;
                    RampOuterWidthL = OuterShoulderWidthL;
                }
                else
                {
                    RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                    RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                }

                //The master outer road edges vector locations:
                if (bMaxIntersection && bInterseOn)
                {   //If in maximum intersection, adjust road edge (also the shoulder inner edges):
                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSeperation_NoTurn * POS.normalized.z, 0, RoadSeperation_NoTurn * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSeperation_NoTurn * -POS.normalized.z, 0, RoadSeperation_NoTurn * POS.normalized.x));
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSep1Lane * POS.normalized.z, 0, RoadSep1Lane * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSep1Lane * -POS.normalized.z, 0, RoadSep1Lane * POS.normalized.x));
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (bIsPastInter)
                        {
                            rVect = (tVect + new Vector3(RoadSep1Lane * POS.normalized.z, 0, RoadSep1Lane * -POS.normalized.x));
                            lVect = (tVect + new Vector3(RoadSep2Lane * -POS.normalized.z, 0, RoadSep2Lane * POS.normalized.x));
                            ;
                        }
                        else
                        {
                            rVect = (tVect + new Vector3(RoadSep2Lane * POS.normalized.z, 0, RoadSep2Lane * -POS.normalized.x));
                            lVect = (tVect + new Vector3(RoadSep1Lane * -POS.normalized.z, 0, RoadSep1Lane * POS.normalized.x));
                        }
                    }
                    else
                    {
                        rVect = (tVect + new Vector3(RoadSeperation * POS.normalized.z, 0, RoadSeperation * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSeperation * -POS.normalized.z, 0, RoadSeperation * POS.normalized.x));
                    }
                }
                else
                {
                    //Typical road/shoulder inner edge location:
                    rVect = (tVect + new Vector3(RoadSeperation * POS.normalized.z, 0, RoadSeperation * -POS.normalized.x));
                    lVect = (tVect + new Vector3(RoadSeperation * -POS.normalized.z, 0, RoadSeperation * POS.normalized.x));
                }

                //Shoulder right vectors:
                ShoulderR_rVect = (tVect + new Vector3(OuterShoulderWidthR * POS.normalized.z, 0, OuterShoulderWidthR * -POS.normalized.x));
                ShoulderR_lVect = rVect;    //Note that the shoulder inner edge is the same as the road edge vector.
                                            //Shoulder left vectors:
                ShoulderL_rVect = lVect;    //Note that the shoulder inner edge is the same as the road edge vector.
                ShoulderL_lVect = (tVect + new Vector3(OuterShoulderWidthL * -POS.normalized.z, 0, OuterShoulderWidthL * POS.normalized.x));

                //				Profiler.EndSample();
                //				Profiler.BeginSample("Test3");

                //Now to start the main lane construction for the intersection:
                if (bMaxIntersection && bInterseOn)
                {
                    //					if(kCount >= tSpline.RoadDefKeysArray.Length){
                    //						vCount = tSpline.RoadDefKeysArray.Length-1;
                    //					}else{
                    //						vCount = kCount-1;	
                    //					}
                    vCount = kCount;

                    tParam2 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[vCount]);
                    float tInterStrNext = _road.spline.IntersectionStrength_Next(tSpline.GetSplineValue(tParam2, false));
                    if (GSDRootUtil.IsApproximately(tInterStrNext, 1f, 0.001f) || tInterStrNext > 1f)
                    {
                        bIsNextInter = true;
                    }
                    else
                    {
                        bIsNextInter = false;
                    }

                    if (string.Compare(xNode.UID, GSDRI.node1.UID) == 0)
                    {
                        bFirstInterNode = true;
                    }
                    else
                    {
                        bFirstInterNode = false;
                    }

                    tempIVect = tVect;
                    if (bIsPastInter)
                    {
                        bool bLLtoRL = bFirstInterNode;
                        bool bRLtoRR = !bFirstInterNode;
                        if (xNode.iConstruction.iFLane0L.Count == 0)
                        {
                            xNode.iConstruction.isFrontFirstRound = true;
                            xNode.iConstruction.isFrontFirstRoundTriggered = true;
                            xNode.iConstruction.isFLane0DoneFinalThisRound = true;
                            xNode.iConstruction.isFLane1DoneFinalThisRound = true;
                            xNode.iConstruction.isFLane2DoneFinalThisRound = true;
                            xNode.iConstruction.isFLane3DoneFinalThisRound = true;

                            if (GSDRI.isFlipped && !bFirstInterNode)
                            {
                                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[0], tIntHeight));
                                    xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[2], tIntHeight));
                                    xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[2], tIntHeight));
                                    xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[3], tIntHeight));
                                    xNode.iConstruction.iFLane3L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[3], tIntHeight));
                                    xNode.iConstruction.iFLane3R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[4], tIntHeight));
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[0], tIntHeight));
                                    xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[2], tIntHeight));
                                    xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[2], tIntHeight));
                                    xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[3], tIntHeight));
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[0], tIntHeight));
                                    xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[1], tIntHeight));
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerLR[2], tIntHeight));
                                }
                            }
                            else
                            {
                                if (bLLtoRL)
                                {
                                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[4], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[3], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[3], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[2], tIntHeight));
                                        xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[2], tIntHeight));
                                        xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane3L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane3R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[0], tIntHeight));
                                    }
                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[3], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[2], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[2], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[0], tIntHeight));
                                    }
                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[2], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[1], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerLLCornerRL[0], tIntHeight));
                                    }
                                }
                                else if (bRLtoRR)
                                {
                                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[4], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[3], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[3], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[2], tIntHeight));
                                        xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[2], tIntHeight));
                                        xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane3L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane3R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[0], tIntHeight));
                                    }
                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[3], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[2], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[2], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane2L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane2R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[0], tIntHeight));
                                    }
                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                    {
                                        xNode.iConstruction.iFLane0L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[2], tIntHeight));
                                        xNode.iConstruction.iFLane0R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane1L.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[1], tIntHeight));
                                        xNode.iConstruction.iFLane1R.Add(ReplaceHeight(GSDRI.cornerRLCornerRR[0], tIntHeight));
                                    }
                                }
                            }

                            xNode.iConstruction.shoulderEndFR = xNode.iConstruction.iFLane0L[0];
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.iConstruction.shoulderEndFL = xNode.iConstruction.iFLane3R[0];
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.iConstruction.shoulderEndFL = xNode.iConstruction.iFLane2R[0];
                            }
                            else
                            {
                                xNode.iConstruction.shoulderEndFL = xNode.iConstruction.iFLane1R[0];
                            }
                            xNode.iConstruction.shoulderFLStartIndex = _road.RCS.ShoulderL_Vectors.Count - 2;
                            xNode.iConstruction.shoulderFRStartIndex = _road.RCS.ShoulderR_Vectors.Count - 2;
                        }

                        //Line 0:
                        xNode.iConstruction.f0LAttempt = rVect;
                        if (!xNode.iConstruction.isFLane0Done && !GSDRI.Contains(ref rVect))
                        {
                            xNode.iConstruction.iFLane0L.Add(ReplaceHeight(rVect, tIntHeight));
                            f0LAdded = true;
                        }

                        //Line 1:
                        //	if(f0LAdded){
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = tVect;
                            if (!xNode.iConstruction.isFLane1Done && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(tempIVect, rVect))
                            {
                                if (f0LAdded)
                                {
                                    xNode.iConstruction.iFLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    f0RAdded = true;
                                }
                                xNode.iConstruction.iFLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                f1LAdded = true;
                            }
                            else
                            {
                                if (f0LAdded)
                                {
                                    xNode.iConstruction.iFLane0L.RemoveAt(xNode.iConstruction.iFLane0L.Count - 1);
                                    f0LAdded = false;
                                }
                            }
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((LaneWidth * 0.5f) * POS.normalized.z, 0f, (LaneWidth * 0.5f) * -POS.normalized.x));
                            if (!xNode.iConstruction.isFLane1Done && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(tempIVect, rVect))
                            {
                                if (f0LAdded)
                                {
                                    xNode.iConstruction.iFLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    f0RAdded = true;
                                }
                                xNode.iConstruction.iFLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                f1LAdded = true;
                            }
                            else
                            {
                                if (f0LAdded)
                                {
                                    xNode.iConstruction.iFLane0L.RemoveAt(xNode.iConstruction.iFLane0L.Count - 1);
                                    f0LAdded = false;
                                }
                            }
                        }
                        //}
                        xNode.iConstruction.f0RAttempt = tempIVect;
                        xNode.iConstruction.f1LAttempt = tempIVect;

                        //Line 2:
                        //if(f1LAdded){
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = lVect;
                            if (!xNode.iConstruction.isFLane2Done && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(tempIVect, rVect))
                            {
                                if (f1LAdded)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    f1RAdded = true;
                                }
                            }
                            else
                            {
                                if (f1LAdded && xNode.iConstruction.iFLane1L.Count > 1)
                                {
                                    xNode.iConstruction.iFLane1L.RemoveAt(xNode.iConstruction.iFLane1L.Count - 1);
                                    f1LAdded = false;
                                }
                            }
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((LaneWidth * 0.5f) * -POS.normalized.z, 0f, (LaneWidth * 0.5f) * POS.normalized.x));
                            tempIVect_prev = tempIVect;
                            if (!xNode.iConstruction.isFLane2Done && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(tempIVect, rVect))
                            {
                                if (f1LAdded)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    f1RAdded = true;
                                }
                                xNode.iConstruction.iFLane2L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                f2LAdded = true;
                            }
                            else
                            {
                                if (f1LAdded)
                                {
                                    xNode.iConstruction.iFLane1L.RemoveAt(xNode.iConstruction.iFLane1L.Count - 1);
                                    f1LAdded = false;
                                    f1RAdded = false;
                                }
                            }
                        }
                        //}
                        xNode.iConstruction.f1RAttempt = tempIVect;
                        xNode.iConstruction.f2LAttempt = tempIVect;

                        //Line 3 / 4:
                        //if(f2LAdded){

                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            tempIVect = (tVect + new Vector3(((LaneWidth * 0.5f) + RoadSeperation) * -POS.normalized.z, 0, ((LaneWidth * 0.5f) + RoadSeperation) * POS.normalized.x));
                            if (!xNode.iConstruction.isFLane3Done && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(lVect, tempIVect))
                            {

                                xNode.iConstruction.iFLane3L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                f3LAdded = true;
                                xNode.iConstruction.iFLane3R.Add(ReplaceHeight(lVect, tIntHeight));
                                f3RAdded = true;
                                //								if(bIsNextInter && GSDRI.iType == GSDRoadIntersection.IntersectionTypeEnum.FourWay){
                                if (f2LAdded)
                                {
                                    xNode.iConstruction.iFLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    f2RAdded = true;
                                }
                                //								}
                            }
                            else
                            {
                                if (f2LAdded)
                                {
                                    xNode.iConstruction.iFLane2L.RemoveAt(xNode.iConstruction.iFLane2L.Count - 1);
                                    f2LAdded = false;
                                }
                            }

                        }
                        else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            tempIVect = (tVect + new Vector3(((LaneWidth * 0.5f) + RoadSeperation) * -POS.normalized.z, 0, ((LaneWidth * 0.5f) + RoadSeperation) * POS.normalized.x));
                            if (f2LAdded && !GSDRI.Contains(ref tempIVect) && !GSDRI.ContainsLine(rVect, tempIVect))
                            {
                                xNode.iConstruction.iFLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                f2RAdded = true;
                            }
                            else if (f2LAdded)
                            {
                                xNode.iConstruction.iFLane2L.RemoveAt(xNode.iConstruction.iFLane2L.Count - 1);
                                f2LAdded = false;
                            }
                        }

                        //	}
                        xNode.iConstruction.f2RAttempt = tempIVect;
                        xNode.iConstruction.f3LAttempt = tempIVect;
                        xNode.iConstruction.f3RAttempt = lVect;

                        if (!bIsNextInter && !xNode.iConstruction.isFDone)
                        {
                            //							xNode.iConstruction.bFDone = true;
                            xNode.iConstruction.isFLane0Done = true;
                            xNode.iConstruction.isFLane1Done = true;
                            xNode.iConstruction.isFLane2Done = true;
                            xNode.iConstruction.isFLane3Done = true;

                            POS_Next = default(Vector3);
                            tVect_Next = default(Vector3);

                            tParam1 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[kCount]);
                            tSpline.GetSplineValue_Both(tParam1, out tVect_Next, out POS_Next);
                            rVect_Next = (tVect_Next + new Vector3(RoadSeperation * POS_Next.normalized.z, 0, RoadSeperation * -POS_Next.normalized.x));
                            lVect_Next = (tVect_Next + new Vector3(RoadSeperation * -POS_Next.normalized.z, 0, RoadSeperation * POS_Next.normalized.x));

                            xNode.iConstruction.iFLane0L.Add(ReplaceHeight(rVect_Next, tIntHeight));
                            xNode.iConstruction.iFLane0R.Add(ReplaceHeight(tVect_Next, tIntHeight));
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.iConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                if (_road.laneAmount == 2)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.475f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.488f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.492f) + lVect_Next, tIntHeight));
                                }

                                if (_road.laneAmount == 2)
                                {
                                    xNode.iConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.03f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.iConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.015f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.iConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.01f) + lVect_Next, tIntHeight));
                                }

                                xNode.iConstruction.iFLane3R.Add(ReplaceHeight(lVect_Next, tIntHeight));
                                //								xNode.iConstruction.iFLane2L.Add(GVC(tVect_Next,tIntHeight));	
                                //								xNode.iConstruction.iFLane2R.Add(GVC(lVect_Next,tIntHeight));

                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.iConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                if (_road.laneAmount == 2)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.475f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.488f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.iConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.492f) + lVect_Next, tIntHeight));
                                }
                                xNode.iConstruction.iFLane2L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                xNode.iConstruction.iFLane2R.Add(ReplaceHeight(lVect_Next, tIntHeight));

                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                xNode.iConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                xNode.iConstruction.iFLane1R.Add(ReplaceHeight(lVect_Next, tIntHeight));
                            }
                            bShrinkRoadFNext = true;
                            //							bShrinkRoadF = true;
                        }

                    }
                    else
                    {
                        bLRtoRR = bFirstInterNode;
                        bLLtoLR = !bFirstInterNode;
                        //B:
                        //Line 0:
                        tempIVect = lVect;
                        bool bFirst123 = false;
                        if (xNode.iConstruction.iBLane0R.Count == 0)
                        {
                            xNode.iConstruction.iBLane0L.Add(lVect_Prev);
                            xNode.iConstruction.iBLane0R.Add(tVect_Prev);
                            bShrinkRoadB = true;

                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.iConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.iConstruction.iBLane1R.Add((tVect_Prev + new Vector3((LaneWidth * 0.05f) * POS.normalized.z, 0, (LaneWidth * 0.05f) * -POS.normalized.x)));
                                xNode.iConstruction.iBLane3L.Add(((lVect_Prev - rVect_Prev) * 0.03f) + rVect_Prev);
                                xNode.iConstruction.iBLane3R.Add(rVect_Prev);

                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.iConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.iConstruction.iBLane1R.Add((tVect_Prev + new Vector3((LaneWidth * 0.05f) * POS.normalized.z, 0, (LaneWidth * 0.05f) * -POS.normalized.x)));
                                xNode.iConstruction.iBLane2L.Add(xNode.iConstruction.iBLane1R[0]);
                                xNode.iConstruction.iBLane2R.Add(rVect_Prev);

                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                xNode.iConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.iConstruction.iBLane1R.Add(rVect_Prev);
                            }

                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.iConstruction.shoulderStartBL = xNode.iConstruction.iBLane0L[0];
                                xNode.iConstruction.shoulderStartBR = xNode.iConstruction.iBLane3R[0];
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.iConstruction.shoulderStartBL = xNode.iConstruction.iBLane0L[0];
                                xNode.iConstruction.shoulderStartBR = xNode.iConstruction.iBLane2R[0];
                            }
                            else
                            {
                                xNode.iConstruction.shoulderStartBL = xNode.iConstruction.iBLane0L[0];
                                xNode.iConstruction.shoulderStartBR = xNode.iConstruction.iBLane1R[0];
                            }

                            xNode.iConstruction.shoulderBLStartIndex = _road.RCS.ShoulderL_Vectors.Count - 2;
                            xNode.iConstruction.shoulderBRStartIndex = _road.RCS.ShoulderR_Vectors.Count - 2;
                            //							bFirst123 = true;
                            //							goto InterSkip;
                        }

                        bLine = false;
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            bLine = !GSDRI.ContainsLine(tempIVect, (tVect + new Vector3((LaneWidth * 0.5f) * -POS.normalized.z, 0, (LaneWidth * 0.5f) * POS.normalized.x)));
                        }
                        else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            bLine = !GSDRI.ContainsLine(tempIVect, (tVect + new Vector3((LaneWidth * 0.5f) * -POS.normalized.z, 0, (LaneWidth * 0.5f) * POS.normalized.x)));
                        }
                        else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            bLine = !GSDRI.ContainsLine(lVect, tVect);
                        }
                        if (!xNode.iConstruction.isBLane0Done && !GSDRI.Contains(ref tempIVect) && bLine)
                        {
                            xNode.iConstruction.iBLane0L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            b0LAdded = true;
                        }
                        else if (!xNode.iConstruction.isBLane0DoneFinal)
                        {
                            //Finalize lane 0:
                            InterFinalizeiBLane0(ref xNode, ref GSDRI, ref tIntHeight, bLRtoRR, bLLtoLR, bFirstInterNode);
                        }

                        //Line 1:
                        if (xNode.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            if (xNode.iConstruction.iBLane0L.Count == 2)
                            {
                                tempIVect = (tVect + new Vector3((LaneWidth * 0.5f) * -POS.normalized.z, 0, (LaneWidth * 0.5f) * POS.normalized.x));
                                xNode.iConstruction.iBLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                b0RAdded = true;
                            }
                        }
                        tempIVect_Prev = tempIVect;
                        tempIVect = (tVect + new Vector3((LaneWidth * 0.5f) * -POS.normalized.z, 0, (LaneWidth * 0.5f) * POS.normalized.x));
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = tVect;
                        }
                        bLine = false;
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            bLine = !GSDRI.ContainsLine(tempIVect, (tVect + new Vector3((LaneWidth * 0.5f) * POS.normalized.z, 0, (LaneWidth * 0.5f) * -POS.normalized.x)));
                        }
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            bLine = !GSDRI.ContainsLine(tempIVect, rVect);
                        }
                        else
                        {
                            bLine = !GSDRI.ContainsLine(tempIVect, rVect);
                        }
                        tempIVect_Prev = tempIVect;
                        if (b0LAdded && !xNode.iConstruction.isBLane1Done && !GSDRI.Contains(ref tempIVect) && bLine)
                        {
                            if (b0LAdded && (xNode.iConstruction.iBLane0L.Count != 2 || GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane))
                            {
                                xNode.iConstruction.iBLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                b0RAdded = true;
                            }
                            xNode.iConstruction.iBLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            b1LAdded = true;
                        }
                        else if (!xNode.iConstruction.isBLane1DoneFinal)
                        {
                            //Finalize lane 1:
                            InterFinalizeiBLane1(ref xNode, ref GSDRI, ref tIntHeight, bLRtoRR, bLLtoLR, bFirstInterNode, ref b0LAdded, ref b1RAdded);
                        }

                        //Line 2:
                        if (xNode.iConstruction.iBLane1R.Count == 0 && xNode.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            xNode.iConstruction.iBLane1R.Add(ReplaceHeight(tVect, tIntHeight));
                            b1RAdded = true;
                            xNode.iConstruction.iBLane2L.Add(ReplaceHeight(tVect, tIntHeight));
                            b2LAdded = true;
                            b2LAdded = true;
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((LaneWidth * 0.5f) * POS.normalized.z, 0, (LaneWidth * 0.5f) * -POS.normalized.x));
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                tempIVect = rVect;
                            }
                            if (b1LAdded)
                            {
                                bLine = !GSDRI.ContainsLine(tempIVect, tempIVect_Prev);
                            }
                            else
                            {
                                bLine = !GSDRI.ContainsLine(tempIVect, rVect);
                            }
                            if (!xNode.iConstruction.isBLane2Done && !GSDRI.Contains(ref tempIVect) && bLine)
                            {
                                if (b1LAdded)
                                {
                                    xNode.iConstruction.iBLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    b1RAdded = true;
                                }
                                xNode.iConstruction.iBLane2L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                b2LAdded = true;
                            }
                            else if (!xNode.iConstruction.isBLane2DoneFinal)
                            {
                                InterFinalizeiBLane2(ref xNode, ref GSDRI, ref tIntHeight, bLRtoRR, bLLtoLR, bFirstInterNode, ref b2LAdded, ref b1LAdded, ref b0LAdded, ref b1RAdded);
                            }
                        }

                        //Line 3 / 4:
                        tempIVect = (tVect + new Vector3(((LaneWidth * 0.5f) + RoadSeperation) * POS.normalized.z, 0, ((LaneWidth * 0.5f) + RoadSeperation) * -POS.normalized.x));
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = rVect;
                        }
                        if (!xNode.iConstruction.isBLane3Done && !GSDRI.ContainsLine(rVect, tempIVect) && !GSDRI.ContainsLine(rVect, lVect))
                        {
                            xNode.iConstruction.iBLane3L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            b3LAdded = true;
                            xNode.iConstruction.iBLane3R.Add(ReplaceHeight(rVect, tIntHeight));
                            b3RAdded = true;
                            if (!bFirst123 && GSDRI.intersectionType == GSDRoadIntersection.IntersectionTypeEnum.FourWay)
                            {
                                if (b2LAdded)
                                {
                                    xNode.iConstruction.iBLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    b2RAdded = true;
                                }
                            }
                        }
                        else if (!xNode.iConstruction.isBLane3DoneFinal)
                        {
                            InterFinalizeiBLane3(ref xNode, ref GSDRI, ref tIntHeight, bLRtoRR, bLLtoLR, bFirstInterNode, ref b2LAdded, ref b1LAdded, ref b0LAdded, ref b1RAdded);
                        }

                    }
                }

                //			InterSkip:

                if (!bIsBridge)
                {
                    BridgeUpComing = _road.spline.BridgeUpComing(i);
                    //					if(TempY < 0.5f){
                    //						gHeight = tHeight0;
                    //					}else if(TempY < 2f){
                    //						gHeight = tHeight2;
                    //					}else{
                    //						if(bTempYWasNegative){
                    //							tY = new Vector3(0f,(TempY*0.035f),0f);	
                    //						}
                    //						if(tY.y < tHeight1.y){
                    //							tY = tHeight1;	
                    //						}
                    //						gHeight = tY;
                    //					}
                    if (BridgeUpComing < 0.2f)
                    {
                        BridgeUpComing = 0.2f;
                    }
                    //					gHeight.y = gHeight.y * BridgeUpComing;

                    //					if(tRoad.opt_MatchTerrain){
                    gHeight.y = 0f;
                    //					}

                    lVect += gHeight;
                    rVect += gHeight;
                    ShoulderR_lVect += gHeight;
                    ShoulderL_rVect += gHeight;
                    ShoulderL_lVect += gHeight;
                    ShoulderR_rVect += gHeight;
                    tHeightAdded = gHeight.y;
                }


                if (tIntStrength >= 1f)
                {
                    tVect.y -= tInterSubtract;
                    tLastInterHeight = tVect.y;
                    rVect.y -= tInterSubtract;
                    lVect.y -= tInterSubtract;

                    ShoulderL_rVect.y = tIntHeight;
                    ShoulderR_lVect.y = tIntHeight;
                    ShoulderR_rVect.y = tIntHeight;
                    ShoulderL_lVect.y = tIntHeight;

                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderL_rVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderL_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_rVect.y); }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderR_lVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderR_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_lVect.y); }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderR_rVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderR_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_rVect.y); }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderL_lVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderL_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_lVect.y); }
                }
                else if (tIntStrength > 0f)
                {

                    rVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * rVect.y);
                    ShoulderR_lVect = rVect;
                    lVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * lVect.y);
                    ShoulderL_rVect = lVect;
                    ShoulderR_rVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * ShoulderR_rVect.y);
                    ShoulderL_lVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * ShoulderL_lVect.y);

                    //					if(!Mathf.Approximately(tIntStrength,0f)){ tVect.y = (tIntStrength*tIntHeight) + ((1-tIntStrength)*tVect.y); }
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref rVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*rVect.y); ShoulderR_lVect = rVect; }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref lVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*lVect.y); ShoulderL_rVect = lVect; }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderR_rVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderR_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_rVect.y); }
                    //					
                    //					tIntStrength_temp = tRoad.GSDSpline.IntersectionStrength(ref ShoulderL_lVect,ref tIntHeight, ref GSDRI,ref bIsPastInter,ref i, ref xNode);
                    //					if(!Mathf.Approximately(tIntStrength_temp,0f)){ ShoulderL_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_lVect.y); }
                }

                //Ramp:
                RampR_L = ShoulderR_rVect;
                RampL_R = ShoulderL_lVect;
                if (bIsBridge)
                {
                    RampR_R = RampR_L;
                    RampL_L = RampL_R;
                }
                else
                {
                    RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x)) + gHeight;
                    SetVectorHeight2(ref RampR_R, ref i, ref tSpline.HeightHistory, ref tSpline);
                    RampR_R.y -= _road.desiredRampHeight;

                    RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x)) + gHeight;
                    SetVectorHeight2(ref RampL_L, ref i, ref tSpline.HeightHistory, ref tSpline);
                    RampL_L.y -= _road.desiredRampHeight;
                }

                //Merge points to intersection corners if necessary:
                if (bMaxIntersection && !bIsBridge && !bIsTunnel && bInterseOn)
                {
                    mCornerDist = _road.roadDefinition * 1.35f;
                    mCornerDist *= mCornerDist;

                    CornerRR = new Vector2(GSDRI.cornerRR.x, GSDRI.cornerRR.z);
                    CornerRL = new Vector2(GSDRI.cornerRL.x, GSDRI.cornerRL.z);
                    CornerLR = new Vector2(GSDRI.cornerLR.x, GSDRI.cornerLR.z);
                    CornerLL = new Vector2(GSDRI.cornerLL.x, GSDRI.cornerLL.z);
                    rVect2D = new Vector2(rVect.x, rVect.z);
                    lVect2D = new Vector2(lVect.x, lVect.z);
                    bOverrideRampR = false;
                    bOverrideRampL = false;
                    bImmuneR = false;
                    bImmuneL = false;
                    bMod1 = 1.75f;
                    bMod2 = 1.25f;
                    t2DDist = -1f;

                    //Find equatable lane vect and move it too
                    //					eCount = -1;
                    //					eIndex = -1;
                    //					uCount = -1;
                    //					uIndex = -1;

                    xHeight = new Vector3(0f, -0.1f, 0f);
                    bSpecAddedL = false;
                    bSpecAddedR = false;

                    if (bFirstInterNode)
                    {
                        bSpecAddedL = (b0LAdded || f0LAdded);
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            bSpecAddedR = (b1RAdded || f1LAdded);
                        }
                        else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            bSpecAddedR = (b2RAdded || f2LAdded);
                        }
                        else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            bSpecAddedR = (b3RAdded || f3LAdded);
                        }
                    }

                    float tempRoadDef = Mathf.Clamp(_road.laneWidth, 3f, 5f);

                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {

                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {

                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {

                    }

                    //RR:
                    if (GSDRI.evenAngle > 90f)
                    {
                        mCornerDist = tempRoadDef * bMod1;
                    }
                    else
                    {
                        mCornerDist = tempRoadDef * bMod2;
                    }
                    mCornerDist *= mCornerDist;
                    t2DDist = Vector2.SqrMagnitude(CornerRR - rVect2D);
                    if (t2DDist < mCornerDist)
                    {
                        bImmuneR = true;
                        bInter_CurreIsCorner = true;
                        bInter_CurreIsCornerRR = true;

                        if (bFirstInterNode)
                        {
                            vList = null;
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.iConstruction.iBLane1R;
                                if (xNode.iConstruction.isBLane1DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.iConstruction.iBLane2R;
                                if (xNode.iConstruction.isBLane2DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.iConstruction.iBLane3R;
                                if (xNode.iConstruction.isBLane3DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }

                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 0; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRR.x, 0.01f) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRR.z, 0.01f)))
                                        {
                                            eList.Add(m);
                                        }
                                    }
                                }
                                for (int m = (eList.Count - 1); m >= 0; m--)
                                {
                                    vList.RemoveAt(eList[m]);
                                }
                            }
                            eList = null;
                        }
                        else
                        {
                            //2nd node can only come through RR as front with R
                            vList = null;
                            vList = xNode.iConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRR.x, 0.01f) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRR.z, 0.01f)))
                                        {
                                            eList.Add(m);
                                        }
                                    }
                                }
                                for (int m = (eList.Count - 1); m >= 0; m--)
                                {
                                    vList.RemoveAt(eList[m]);
                                }
                            }
                            eList = null;
                        }

                        ShoulderR_lVect = new Vector3(CornerRR.x, tIntHeight, CornerRR.y);
                        ShoulderR_rVect = new Vector3(GSDRI.cornerRROuter.x, tIntHeight, GSDRI.cornerRROuter.z);
                        RampR_Override = new Vector3(GSDRI.cornerRRRampOuter.x, tIntHeight, GSDRI.cornerRRRampOuter.z);
                        bRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerRR - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            bImmuneL = true;
                            bInter_CurreIsCorner = true;
                            bInter_CurreIsCornerRR = true;

                            //2nd node can come in via left
                            if (!bFirstInterNode)
                            {
                                vList = null;
                                vList = xNode.iConstruction.iBLane0L;
                                if (xNode.iConstruction.isBLane0DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 0; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRR.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRR.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }

                            ShoulderL_rVect = new Vector3(CornerRR.x, tIntHeight, CornerRR.y);
                            ShoulderL_lVect = new Vector3(GSDRI.cornerRROuter.x, tIntHeight, GSDRI.cornerRROuter.z);
                            RampL_Override = new Vector3(GSDRI.cornerRRRampOuter.x, tIntHeight, GSDRI.cornerRRRampOuter.z);
                            bRecordShoulderLForNormals = true;
                        }
                    }
                    //RL:
                    if (GSDRI.oddAngle > 90f)
                    {
                        mCornerDist = tempRoadDef * bMod1;
                    }
                    else
                    {
                        mCornerDist = tempRoadDef * bMod2;
                    }
                    mCornerDist *= mCornerDist;
                    t2DDist = Vector2.SqrMagnitude(CornerRL - rVect2D);
                    if (t2DDist < mCornerDist)
                    {
                        bImmuneR = true;
                        bInter_CurreIsCorner = true;
                        bInter_CurreIsCornerRL = true;

                        if (bFirstInterNode)
                        {
                            vList = null;
                            vList = xNode.iConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRL.z)))
                                        {
                                            eList.Add(m);
                                        }
                                    }
                                }
                                for (int m = (eList.Count - 1); m >= 0; m--)
                                {
                                    vList.RemoveAt(eList[m]);
                                }
                            }
                            eList = null;
                        }
                        else
                        {
                            vList = null;
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.iConstruction.iBLane1R;
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.iConstruction.iBLane2R;
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.iConstruction.iBLane3R;
                            }

                            //Hitting RL from backside with second node:
                            if (!bFirstInterNode)
                            {
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 0; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRL.z)))
                                            {
                                                eList.Add(m);
                                                if (m == vList.Count - 1)
                                                {
                                                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                                    {
                                                        b1RAdded = false;
                                                    }
                                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                                    {
                                                        b2RAdded = false;
                                                    }
                                                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                                    {
                                                        b3RAdded = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                            }
                            eList = null;
                        }

                        ShoulderR_lVect = new Vector3(CornerRL.x, tIntHeight, CornerRL.y);
                        ShoulderR_rVect = new Vector3(GSDRI.cornerRLOuter.x, tIntHeight, GSDRI.cornerRLOuter.z);
                        RampR_Override = new Vector3(GSDRI.cornerRLRampOuter.x, tIntHeight, GSDRI.cornerRLRampOuter.z);
                        bRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerRL - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            bImmuneL = true;
                            bInter_CurreIsCorner = true;
                            bInter_CurreIsCornerRL = true;

                            if (!bFirstInterNode)
                            {
                                vList = null;
                                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane1R;
                                    if (xNode.iConstruction.isFLane1DoneFinalThisRound)
                                    {
                                        vList = null;
                                    }
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane2R;
                                    if (xNode.iConstruction.isFLane2DoneFinalThisRound)
                                    {
                                        vList = null;
                                    }
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.iConstruction.iFLane3R;
                                    if (xNode.iConstruction.isFLane3DoneFinalThisRound)
                                    {
                                        vList = null;
                                    }
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 1; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerRL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerRL.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }

                            ShoulderL_rVect = new Vector3(CornerRL.x, tIntHeight, CornerRL.y);
                            ShoulderL_lVect = new Vector3(GSDRI.cornerRLOuter.x, tIntHeight, GSDRI.cornerRLOuter.z);
                            RampL_Override = new Vector3(GSDRI.cornerRLRampOuter.x, tIntHeight, GSDRI.cornerRLRampOuter.z);
                            bRecordShoulderLForNormals = true;
                        }
                    }
                    //LR:
                    if (GSDRI.oddAngle > 90f)
                    {
                        mCornerDist = tempRoadDef * bMod1;
                    }
                    else
                    {
                        mCornerDist = tempRoadDef * bMod2;
                    }
                    mCornerDist *= mCornerDist;
                    t2DDist = Vector2.SqrMagnitude(CornerLR - rVect2D);
                    if (t2DDist < mCornerDist)
                    {
                        bImmuneR = true;
                        bInter_CurreIsCorner = true;
                        bInter_CurreIsCornerLR = true;

                        if (!bFirstInterNode)
                        {
                            vList = null;
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.iConstruction.iBLane1R;
                                if (xNode.iConstruction.isBLane1DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.iConstruction.iBLane2R;
                                if (xNode.iConstruction.isBLane2DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.iConstruction.iBLane3R;
                                if (xNode.iConstruction.isBLane3DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }

                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 0; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLR.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLR.z)))
                                        {
                                            eList.Add(m);
                                        }
                                    }
                                }
                                for (int m = (eList.Count - 1); m >= 0; m--)
                                {
                                    vList.RemoveAt(eList[m]);
                                }
                            }
                            eList = null;
                        }

                        ShoulderR_lVect = new Vector3(CornerLR.x, tIntHeight, CornerLR.y);
                        ShoulderR_rVect = new Vector3(GSDRI.cornerLROuter.x, tIntHeight, GSDRI.cornerLROuter.z);
                        RampR_Override = new Vector3(GSDRI.cornerLRRampOuter.x, tIntHeight, GSDRI.cornerLRRampOuter.z);
                        bRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerLR - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            bImmuneL = true;
                            bInter_CurreIsCorner = true;
                            bInter_CurreIsCornerLR = true;

                            if (bFirstInterNode)
                            {
                                vList = null;
                                vList = xNode.iConstruction.iBLane0L;
                                if (xNode.iConstruction.isBLane0DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 0; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLR.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLR.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }
                            else
                            {
                                //2nd node can only come through LR as front with L
                                vList = null;
                                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane1R;
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane2R;
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.iConstruction.iFLane3R;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 1; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLR.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLR.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }

                            ShoulderL_rVect = new Vector3(CornerLR.x, tIntHeight, CornerLR.y);
                            ShoulderL_lVect = new Vector3(GSDRI.cornerLROuter.x, tIntHeight, GSDRI.cornerLROuter.z);
                            RampL_Override = new Vector3(GSDRI.cornerLRRampOuter.x, tIntHeight, GSDRI.cornerLRRampOuter.z);
                            bRecordShoulderLForNormals = true;
                        }
                    }
                    //LL:
                    if (GSDRI.evenAngle > 90f)
                    {
                        mCornerDist = tempRoadDef * bMod1;
                    }
                    else
                    {
                        mCornerDist = tempRoadDef * bMod2;
                    }
                    mCornerDist *= mCornerDist;
                    t2DDist = Vector2.SqrMagnitude(CornerLL - rVect2D);
                    if (t2DDist < mCornerDist)
                    {
                        bImmuneR = true;
                        bInter_CurreIsCorner = true;
                        bInter_CurreIsCornerLL = true;


                        if (!bFirstInterNode)
                        {
                            vList = null;
                            vList = xNode.iConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLL.z)))
                                        {
                                            eList.Add(m);
                                        }
                                    }
                                }
                                for (int m = (eList.Count - 1); m >= 0; m--)
                                {
                                    vList.RemoveAt(eList[m]);
                                }
                            }
                            eList = null;
                        }

                        ShoulderR_lVect = new Vector3(CornerLL.x, tIntHeight, CornerLL.y);
                        ShoulderR_rVect = new Vector3(GSDRI.cornerLLOuter.x, tIntHeight, GSDRI.cornerLLOuter.z);
                        RampR_Override = new Vector3(GSDRI.cornerLLRampOuter.x, tIntHeight, GSDRI.cornerLLRampOuter.z);
                        bRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerLL - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            bImmuneL = true;
                            bInter_CurreIsCorner = true;
                            bInter_CurreIsCornerLL = true;

                            if (bFirstInterNode)
                            {
                                vList = null;
                                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane1R;
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.iConstruction.iFLane2R;
                                }
                                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.iConstruction.iFLane3R;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 1; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLL.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }
                            else
                            {
                                vList = null;
                                vList = xNode.iConstruction.iBLane0L;
                                if (xNode.iConstruction.isBLane0DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 0; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(GSDRootUtil.IsApproximately(vList[m].x, GSDRI.cornerLL.x) && GSDRootUtil.IsApproximately(vList[m].z, GSDRI.cornerLL.z)))
                                            {
                                                eList.Add(m);
                                            }
                                        }
                                    }
                                    for (int m = (eList.Count - 1); m >= 0; m--)
                                    {
                                        vList.RemoveAt(eList[m]);
                                    }
                                }
                                eList = null;
                            }

                            ShoulderL_rVect = new Vector3(CornerLL.x, tIntHeight, CornerLL.y);
                            ShoulderL_lVect = new Vector3(GSDRI.cornerLLOuter.x, tIntHeight, GSDRI.cornerLLOuter.z);
                            RampL_Override = new Vector3(GSDRI.cornerLLRampOuter.x, tIntHeight, GSDRI.cornerLLRampOuter.z);
                            bRecordShoulderLForNormals = true;
                        }
                    }

                    if (bImmuneR)
                    {
                        bOverrideRampR = true;
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderR_lVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderR_lVect);
                        }
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderR_rVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderR_rVect);
                        }
                    }
                    if (bImmuneL)
                    {
                        bOverrideRampL = true;
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderL_rVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderL_rVect);
                        }
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderL_lVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderL_lVect);
                        }
                    }
                }

                if (bShrinkRoadB)
                {

                    if (lVect_Prev != new Vector3(0f, 0f, 0f))
                    {
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                    }
                }
                if (bShrinkRoadF)
                {
                    if (lVect != new Vector3(0f, 0f, 0f))
                    {
                        _road.RCS.RoadVectors.Add(lVect);
                        _road.RCS.RoadVectors.Add(lVect);
                        _road.RCS.RoadVectors.Add(lVect);
                        _road.RCS.RoadVectors.Add(lVect);
                    }
                }

                _road.RCS.RoadVectors.Add(lVect);
                _road.RCS.RoadVectors.Add(lVect);
                _road.RCS.RoadVectors.Add(rVect);
                _road.RCS.RoadVectors.Add(rVect);



                //Add bounds for later removal:
                if (!bIsBridge && !bIsTunnel && bMaxIntersection && bWasPrevMaxInter && bInterseOn)
                {
                    bool bGoAhead = true;
                    if (xNode.bIsEndPoint)
                    {
                        if (xNode.idOnSpline == 1)
                        {
                            if (i < xNode.tTime)
                            {
                                bGoAhead = false;
                            }
                        }
                        else
                        {
                            if (i > xNode.tTime)
                            {
                                bGoAhead = false;
                            }
                        }
                    }

                    //Get this and prev lvect rvect rects:
                    if ((Vector3.SqrMagnitude(xNode.pos - tVect) < CullDistanceSQ) && bGoAhead)
                    {
                        GSD.Roads.GSDRoadUtil.Construction2DRect vRect = new GSD.Roads.GSDRoadUtil.Construction2DRect(
                            new Vector2(lVect.x, lVect.z),
                            new Vector2(rVect.x, rVect.z),
                            new Vector2(lVect_Prev.x, lVect_Prev.z),
                            new Vector2(rVect_Prev.x, rVect_Prev.z),
                            tLastInterHeight
                            );

                        _road.RCS.tIntersectionBounds.Add(vRect);
                        //						GameObject tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //						tObj.transform.position = lVect;
                        //						tObj.transform.localScale = new Vector3(0.2f,20f,0.2f);
                        //						tObj.transform.name = "temp22";
                        //						
                        //						tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //						tObj.transform.position = rVect;
                        //						tObj.transform.localScale = new Vector3(0.2f,20f,0.2f);
                        //						tObj.transform.name = "temp22";
                    }
                }

                //Ramp construction:
                RampR_L = ShoulderR_rVect;
                RampL_R = ShoulderL_lVect;
                if (bIsBridge)
                {
                    RampR_R = RampR_L;
                    RampL_L = RampL_R;
                }
                else
                {
                    RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x)) + gHeight;
                    if (bOverrideRampR)
                    {
                        RampR_R = RampR_Override;
                    }   //Overrides will come from intersection.
                    SetVectorHeight2(ref RampR_R, ref i, ref tSpline.HeightHistory, ref tSpline);
                    RampR_R.y -= _road.desiredRampHeight;

                    RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x)) + gHeight;
                    if (bOverrideRampL)
                    {
                        RampL_L = RampL_Override;
                    }   //Overrides will come from intersection.
                    SetVectorHeight2(ref RampL_L, ref i, ref tSpline.HeightHistory, ref tSpline);
                    RampL_L.y -= _road.desiredRampHeight;
                    bOverrideRampR = false;
                    bOverrideRampL = false;
                }

                //If necessary during intersection construction, sometimes an addition will be created inbetween intersection corner points.
                //This addition will create a dip between corner points to 100% ensure there is no shoulder visible on the roads between corner points.
                bTriggerInterAddition = false;
                if (bMaxIntersection && bInterseOn)
                {
                    if (bFirstInterNode)
                    {
                        if ((bInter_PrevWasCornerLR && bInter_CurreIsCornerLL) || (bInter_PrevWasCornerRR && bInter_CurreIsCornerRL))
                        {
                            bTriggerInterAddition = true;
                        }
                    }
                    else
                    {
                        if (!GSDRI.isFlipped)
                        {
                            if ((bInter_PrevWasCornerLL && bInter_CurreIsCornerRL) || (bInter_PrevWasCornerLR && bInter_CurreIsCornerRR) || (bInter_PrevWasCornerRR && bInter_CurreIsCornerLR))
                            {
                                bTriggerInterAddition = true;
                            }
                        }
                        else
                        {
                            if ((bInter_PrevWasCornerRR && bInter_CurreIsCornerLR) || (bInter_PrevWasCornerLR && bInter_CurreIsCornerRR) || (bInter_PrevWasCornerRL && bInter_CurreIsCornerLL))
                            {
                                bTriggerInterAddition = true;
                            }
                        }
                    }

                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        bTriggerInterAddition = false;
                    }

                    //For 3-way intersections:
                    bSpecialThreeWayIgnoreR = false;
                    bSpecialThreeWayIgnoreL = false;
                    if (GSDRI.ignoreSide > -1)
                    {
                        if (GSDRI.ignoreSide == 0)
                        {
                            //RR to RL:
                            if (bFirstInterNode && (bInter_PrevWasCornerRR && bInter_CurreIsCornerRL))
                            {
                                bTriggerInterAddition = false;
                            }
                        }
                        else if (GSDRI.ignoreSide == 1)
                        {
                            //RL to LL:
                            if (!bFirstInterNode && ((bInter_PrevWasCornerRL && bInter_CurreIsCornerLL) || (bInter_PrevWasCornerLL && bInter_CurreIsCornerRL)))
                            {
                                //bTriggerInterAddition = false;	
                                if (GSDRI.isFlipped)
                                {
                                    bSpecialThreeWayIgnoreR = true;
                                }
                                else
                                {
                                    bSpecialThreeWayIgnoreL = true;
                                }
                            }
                        }
                        else if (GSDRI.ignoreSide == 2)
                        {
                            //LL to LR:
                            if (bFirstInterNode && (bInter_PrevWasCornerLR && bInter_CurreIsCornerLL))
                            {
                                bTriggerInterAddition = false;
                            }
                        }
                        else if (GSDRI.ignoreSide == 3)
                        {
                            //LR to RR:
                            if (!bFirstInterNode && ((bInter_PrevWasCornerRR && bInter_CurreIsCornerLR) || (bInter_PrevWasCornerLR && bInter_CurreIsCornerRR)))
                            {
                                //bTriggerInterAddition = false;	
                                if (GSDRI.isFlipped)
                                {
                                    bSpecialThreeWayIgnoreL = true;
                                }
                                else
                                {
                                    bSpecialThreeWayIgnoreR = true;
                                }
                            }
                        }
                    }

                    if (bTriggerInterAddition)
                    {
                        iTemp_HeightVect = new Vector3(0f, 0f, 0f);
                        rVect_iTemp = (((rVect_Prev - rVect) * 0.5f) + rVect) + iTemp_HeightVect;
                        lVect_iTemp = (((lVect_Prev - lVect) * 0.5f) + lVect) + iTemp_HeightVect;
                        ShoulderR_R_iTemp = (((ShoulderR_PrevRVect - ShoulderR_rVect) * 0.5f) + ShoulderR_rVect) + iTemp_HeightVect;
                        ShoulderL_L_iTemp = (((ShoulderL_PrevLVect - ShoulderL_lVect) * 0.5f) + ShoulderL_lVect) + iTemp_HeightVect;
                        RampR_R_iTemp = (((RampR_PrevR - RampR_R) * 0.5f) + RampR_R) + iTemp_HeightVect;
                        RampR_L_iTemp = (((RampR_PrevL - RampR_L) * 0.5f) + RampR_L) + iTemp_HeightVect;
                        RampL_R_iTemp = (((RampL_PrevR - RampL_R) * 0.5f) + RampL_R) + iTemp_HeightVect;
                        RampL_L_iTemp = (((RampL_PrevL - RampL_L) * 0.5f) + RampL_L) + iTemp_HeightVect;

                        //						ShoulderL_L_iTemp = lVect_iTemp;
                        //						RampL_R_iTemp = lVect_iTemp;
                        //						RampL_L_iTemp = lVect_iTemp;
                        //					
                        //						ShoulderR_R_iTemp = rVect_iTemp;
                        //						RampR_R_iTemp = rVect_iTemp;
                        //						RampR_L_iTemp = rVect_iTemp;
                    }

                    if (bTriggerInterAddition && !(GSDRI.isFlipped && !bFirstInterNode))
                    {
                        if (bFirstInterNode)
                        {
                            if ((bInter_PrevWasCornerRR && bInter_CurreIsCornerRL && !bSpecialThreeWayIgnoreR))
                            {
                                //Right shoulder:
                                _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                                //Ramps:
                                _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                            }
                            if ((bInter_PrevWasCornerLR && bInter_CurreIsCornerLL && !bSpecialThreeWayIgnoreL))
                            {
                                //Left shoulder:
                                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                                //Ramp:
                                _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                            }
                        }
                        else
                        {
                            if ((bInter_PrevWasCornerLR && bInter_CurreIsCornerRR && !bSpecialThreeWayIgnoreR))
                            {
                                //Right shoulder:
                                _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                                //Ramps:
                                _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                                _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                            }
                            if ((bInter_PrevWasCornerLL && bInter_CurreIsCornerRL && !bSpecialThreeWayIgnoreL))
                            {
                                //Left shoulder:
                                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                                //Ramp:
                                _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                                _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                            }
                        }
                    }
                    else if (bTriggerInterAddition && (GSDRI.isFlipped && !bFirstInterNode))
                    {
                        if ((bInter_PrevWasCornerRR && bInter_CurreIsCornerLR && !bSpecialThreeWayIgnoreL))
                        {
                            //Left shoulder:
                            _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(ShoulderL_L_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(lVect_iTemp);
                            //Ramp:
                            _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(RampL_L_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                            _road.RCS.ShoulderL_Vectors.Add(RampL_R_iTemp);
                        }
                        if ((bInter_PrevWasCornerRL && bInter_CurreIsCornerLL && !bSpecialThreeWayIgnoreR))
                        {
                            //Right shoulder:
                            _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(rVect_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(ShoulderR_R_iTemp);
                            //Ramps:
                            _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(RampR_L_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                            _road.RCS.ShoulderR_Vectors.Add(RampR_R_iTemp);
                        }
                    }
                }




                //Right shoulder:
                if (!bShoulderSkipR)
                {
                    if (bRecordShoulderForNormals)
                    {
                        _road.RCS.normals_ShoulderR_averageStartIndexes.Add(_road.RCS.ShoulderR_Vectors.Count);
                    }

                    _road.RCS.ShoulderR_Vectors.Add(ShoulderR_lVect);
                    _road.RCS.ShoulderR_Vectors.Add(ShoulderR_lVect);
                    _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                    _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                    _road.RCS.ShoulderR_Vectors.Add(RampR_L);
                    _road.RCS.ShoulderR_Vectors.Add(RampR_L);
                    _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                    _road.RCS.ShoulderR_Vectors.Add(RampR_R);

                    //Double up to prevent normal errors from intersection subtraction:
                    if (bImmuneR && bRecordShoulderForNormals)
                    {
                        _road.RCS.ShoulderR_Vectors.Add(ShoulderR_lVect);
                        _road.RCS.ShoulderR_Vectors.Add(ShoulderR_lVect);
                        _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                        _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                        _road.RCS.ShoulderR_Vectors.Add(RampR_L);
                        _road.RCS.ShoulderR_Vectors.Add(RampR_L);
                        _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                        _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                    }
                }

                //Left shoulder:
                if (!bShoulderSkipL)
                {
                    if (bRecordShoulderLForNormals)
                    {
                        _road.RCS.normals_ShoulderL_averageStartIndexes.Add(_road.RCS.ShoulderL_Vectors.Count);
                    }
                    _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                    _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                    _road.RCS.ShoulderL_Vectors.Add(ShoulderL_rVect);
                    _road.RCS.ShoulderL_Vectors.Add(ShoulderL_rVect);
                    _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                    _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                    _road.RCS.ShoulderL_Vectors.Add(RampL_R);
                    _road.RCS.ShoulderL_Vectors.Add(RampL_R);

                    //Double up to prevent normal errors from intersection subtraction:
                    if (bImmuneL && bRecordShoulderForNormals)
                    {
                        _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                        _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                        _road.RCS.ShoulderL_Vectors.Add(ShoulderL_rVect);
                        _road.RCS.ShoulderL_Vectors.Add(ShoulderL_rVect);
                        _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                        _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                        _road.RCS.ShoulderL_Vectors.Add(RampL_R);
                        _road.RCS.ShoulderL_Vectors.Add(RampL_R);
                    }
                }

                //Previous storage:
                tVect_Prev = tVect;
                rVect_Prev = rVect;
                lVect_Prev = lVect;
                ShoulderR_PrevLVect = ShoulderR_lVect;
                ShoulderL_PrevRVect = ShoulderL_rVect;
                //				ShoulderR_PrevRVect3 = ShoulderR_PrevRVect2;
                //				ShoulderL_PrevLVect3 = ShoulderL_PrevLVect2;
                //				ShoulderR_PrevRVect2 = ShoulderR_PrevRVect;
                //				ShoulderL_PrevLVect2 = ShoulderL_PrevLVect;
                ShoulderR_PrevRVect = ShoulderR_rVect;
                ShoulderL_PrevLVect = ShoulderL_lVect;
                RampR_PrevR = RampR_R;
                RampR_PrevL = RampR_L;
                RampL_PrevR = RampL_R;
                RampL_PrevL = RampL_L;

                //Store more prev variables:
                bWasPrevMaxInter = bMaxIntersection;
                bInter_PrevWasCorner = bInter_CurreIsCorner;
                bInter_PrevWasCornerRR = bInter_CurreIsCornerRR;
                bInter_PrevWasCornerRL = bInter_CurreIsCornerRL;
                bInter_PrevWasCornerLL = bInter_CurreIsCornerLL;
                bInter_PrevWasCornerLR = bInter_CurreIsCornerLR;

                //				i+=Step;//Master step incrementer.
            }
            GSDRootUtil.EndStartProfiling(_road, "RoadJob_Prelim_FinalizeInter");

            //Finalize intersection vectors:
            if (bInterseOn)
            {
                RoadJobPrelimFinalizeInter(ref _road);
            }
            GSDRootUtil.EndStartProfiling(_road, "RoadJob_Prelim_RoadConnections");

            //Creates road connections if necessary:
            //			float ExtraHeight = 0f;
            //			float RampPercent = 0.2f;
            if (tSpline.bSpecialEndNode_IsStart_Delay)
            {
                Vector3[] RoadConn_verts = new Vector3[4];

                RampR_R = _road.RCS.ShoulderR_Vectors[7];
                ShoulderR_rVect = _road.RCS.ShoulderR_Vectors[3];
                rVect = _road.RCS.ShoulderR_Vectors[0];

                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R);
                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, rVect);

                RampL_L = _road.RCS.ShoulderL_Vectors[4];
                ShoulderL_lVect = _road.RCS.ShoulderL_Vectors[0];
                lVect = _road.RCS.ShoulderL_Vectors[3];

                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L);
                _road.RCS.ShoulderL_Vectors.Insert(0, lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);

                RoadConn_verts[0] = lVect;
                RoadConn_verts[1] = rVect;
                tSpline.GetSplineValue_Both(RoadConnection_StartMin1, out tVect, out POS);
                RoadSeperation = tSpline.SpecialEndNodeDelay_Start_Result / 2f;
                rVect = (tVect + new Vector3(RoadSeperation * POS.normalized.z, 0, RoadSeperation * -POS.normalized.x));
                lVect = (tVect + new Vector3(RoadSeperation * -POS.normalized.z, 0, RoadSeperation * POS.normalized.x));
                ShoulderSeperation = RoadSeperation + ShoulderWidth;
                OuterShoulderWidthR = ShoulderSeperation;
                OuterShoulderWidthL = ShoulderSeperation;
                RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                ShoulderR_rVect = (tVect + new Vector3(ShoulderSeperation * POS.normalized.z, 0, ShoulderSeperation * -POS.normalized.x));
                ShoulderL_lVect = (tVect + new Vector3(ShoulderSeperation * -POS.normalized.z, 0, ShoulderSeperation * POS.normalized.x));
                RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x));
                SetVectorHeight2(ref RampR_R, ref i, ref tSpline.HeightHistory, ref tSpline);
                RampR_R.y -= (_road.desiredRampHeight + 0.10f);          // normal was 0.35f; Here was 0.45f
                RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x));
                SetVectorHeight2(ref RampL_L, ref i, ref tSpline.HeightHistory, ref tSpline);
                RampL_L.y -= (_road.desiredRampHeight + 0.10f);



                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, rVect + tHeight0);

                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);

                RoadConn_verts[2] = lVect + tHeight0;
                RoadConn_verts[3] = rVect + tHeight0;
                //Tris:
                int[] RoadConn_tris = new int[6];
                RoadConn_tris[0] = 2;
                RoadConn_tris[1] = 0;
                RoadConn_tris[2] = 3;
                RoadConn_tris[3] = 0;
                RoadConn_tris[4] = 1;
                RoadConn_tris[5] = 3;

                Vector3[] RoadConn_normals = new Vector3[4];
                RoadConn_normals[0] = -Vector3.forward;
                RoadConn_normals[1] = -Vector3.forward;
                RoadConn_normals[2] = -Vector3.forward;
                RoadConn_normals[3] = -Vector3.forward;
                Vector2[] RoadConn_uv = new Vector2[4];
                float tMod1 = -1;
                float tMod2 = -1;

                if (_road.laneAmount == 2)
                {
                    tMod1 = 0.5f - (LaneWidth / tSpline.SpecialEndNodeDelay_Start_Result);
                    tMod2 = 0.5f + (LaneWidth / tSpline.SpecialEndNodeDelay_Start_Result);
                }
                else if (_road.laneAmount == 4)
                {
                    tMod1 = 0.5f - ((LaneWidth * 2f) / tSpline.SpecialEndNodeDelay_Start_Result);
                    tMod2 = 0.5f + ((LaneWidth * 2f) / tSpline.SpecialEndNodeDelay_Start_Result);
                }
                RoadConn_uv[0] = new Vector2(tMod1, 0f);
                RoadConn_uv[1] = new Vector2(tMod2, 0f);
                RoadConn_uv[2] = new Vector2(0f, 1f);
                RoadConn_uv[3] = new Vector2(1f, 1f);


                _road.RCS.RoadConnections_verts.Add(RoadConn_verts);
                _road.RCS.RoadConnections_tris.Add(RoadConn_tris);
                _road.RCS.RoadConnections_normals.Add(RoadConn_normals);
                _road.RCS.RoadConnections_uv.Add(RoadConn_uv);
            }
            else if (tSpline.bSpecialEndNode_IsEnd_Delay)
            {
                Vector3[] RoadConn_verts = new Vector3[4];
                int rrCount = _road.RCS.ShoulderR_Vectors.Count;
                RampR_R = _road.RCS.ShoulderR_Vectors[rrCount - 1];
                ShoulderR_rVect = _road.RCS.ShoulderR_Vectors[rrCount - 3];
                rVect = _road.RCS.ShoulderR_Vectors[rrCount - 7];

                //Right shoulder:
                _road.RCS.ShoulderR_Vectors.Add(rVect);
                _road.RCS.ShoulderR_Vectors.Add(rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);

                rrCount = _road.RCS.ShoulderL_Vectors.Count;
                RampL_L = _road.RCS.ShoulderL_Vectors[rrCount - 3];
                ShoulderL_lVect = _road.RCS.ShoulderL_Vectors[rrCount - 1];
                lVect = _road.RCS.ShoulderL_Vectors[rrCount - 5];

                //Left shoulder:
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(lVect);
                _road.RCS.ShoulderL_Vectors.Add(lVect);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);

                RoadConn_verts[0] = lVect;
                RoadConn_verts[1] = rVect;
                tSpline.GetSplineValue_Both(RoadConnection_FinalMax1, out tVect, out POS);
                RoadSeperation = tSpline.SpecialEndNodeDelay_End_Result / 2f;
                rVect = (tVect + new Vector3(RoadSeperation * POS.normalized.z, 0, RoadSeperation * -POS.normalized.x));
                lVect = (tVect + new Vector3(RoadSeperation * -POS.normalized.z, 0, RoadSeperation * POS.normalized.x));
                ShoulderSeperation = RoadSeperation + ShoulderWidth;
                OuterShoulderWidthR = ShoulderSeperation;
                OuterShoulderWidthL = ShoulderSeperation;
                RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                ShoulderR_rVect = (tVect + new Vector3(ShoulderSeperation * POS.normalized.z, 0, ShoulderSeperation * -POS.normalized.x));
                ShoulderL_lVect = (tVect + new Vector3(ShoulderSeperation * -POS.normalized.z, 0, ShoulderSeperation * POS.normalized.x));
                RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x));
                SetVectorHeight2(ref RampR_R, ref i, ref tSpline.HeightHistory, ref tSpline);
                RampR_R.y -= _road.desiredRampHeight;
                RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x));
                SetVectorHeight2(ref RampL_L, ref i, ref tSpline.HeightHistory, ref tSpline);
                RampL_L.y -= _road.desiredRampHeight;

                //Right shoulder:
                _road.RCS.ShoulderR_Vectors.Add(rVect);
                _road.RCS.ShoulderR_Vectors.Add(rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);

                //Left shoulder:
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(lVect);
                _road.RCS.ShoulderL_Vectors.Add(lVect);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);

                RoadConn_verts[2] = lVect;
                RoadConn_verts[3] = rVect;
                //Tris:
                int[] RoadConn_tris = new int[6];
                RoadConn_tris[0] = 0;
                RoadConn_tris[1] = 2;
                RoadConn_tris[2] = 1;
                RoadConn_tris[3] = 2;
                RoadConn_tris[4] = 3;
                RoadConn_tris[5] = 1;

                Vector3[] RoadConn_normals = new Vector3[4];
                RoadConn_normals[0] = -Vector3.forward;
                RoadConn_normals[1] = -Vector3.forward;
                RoadConn_normals[2] = -Vector3.forward;
                RoadConn_normals[3] = -Vector3.forward;
                Vector2[] RoadConn_uv = new Vector2[4];
                float tMod = (RoadWidth / tSpline.SpecialEndNodeDelay_End_Result) / 2f;
                RoadConn_uv[0] = new Vector2(tMod, 0f);
                RoadConn_uv[1] = new Vector2(tMod * 3f, 0f);
                RoadConn_uv[2] = new Vector2(0f, 1f);
                RoadConn_uv[3] = new Vector2(1f, 1f);
                _road.RCS.RoadConnections_verts.Add(RoadConn_verts);
                _road.RCS.RoadConnections_tris.Add(RoadConn_tris);
                _road.RCS.RoadConnections_normals.Add(RoadConn_normals);
                _road.RCS.RoadConnections_uv.Add(RoadConn_uv);
            }
            GSDRootUtil.EndProfiling(_road);
        }


        #region "Road prelim helpers"
        private static Vector3 ReplaceHeight(Vector3 _v1, float _height)
        {
            return new Vector3(_v1.x, _height, _v1.z);
        }


        /// <summary> Usage: tDir = forward dir of player. tVect = direction from player to enemy </summary>
        /// <returns> <c>true</c> if this instance is vect in front the specified tDir tVect; otherwise, <c>false</c>. </returns>
        /// <param name='_dir'> If set to <c>true</c> t dir. </param>
        /// <param name='_vect'> If set to <c>true</c> t vect. </param>
        private static bool IsVectInFront(Vector3 _dir, Vector3 _vect)
        {
            return (Vector3.Dot(_dir.normalized, _vect) > 0);
        }


        private static Vector2 ConvertVect3ToVect2(Vector3 _vect)
        {
            return new Vector2(_vect.x, _vect.z);
        }


        private static void InterFinalizeiBLane0(ref GSDSplineN _node, ref GSDRoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode)
        {
            if (_node.iConstruction.isBLane0DoneFinal)
            {
                return;
            }

            _node.iConstruction.isBLane0Done = true;
            if (_intersection.isFlipped && !_isFirstInterNode)
            {
                if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _node.iConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[4], _intHeight));
                    _node.iConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                }
                else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    _node.iConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                    _node.iConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                }
                else
                {
                    _node.iConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                    _node.iConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                }
            }
            else
            {
                if (_isLRtoRR)
                {
                    _node.iConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[0], _intHeight));
                    _node.iConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[1], _intHeight));
                }
                else if (_isLLtoLR)
                {
                    _node.iConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[0], _intHeight));
                    _node.iConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[1], _intHeight));
                }
            }
            _node.iConstruction.isBLane0DoneFinal = true;
            _node.iConstruction.isBLane0DoneFinalThisRound = true;
        }


        private static void InterFinalizeiBLane1(ref GSDSplineN _node, ref GSDRoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_node.iConstruction.isBLane1DoneFinal)
            {
                return;
            }

            if (_is0LAdded && !_node.iConstruction.isBLane0DoneFinal)
            {
                _node.iConstruction.iBLane0L.RemoveAt(_node.iConstruction.iBLane0L.Count - 1);
                _is0LAdded = false;
                InterFinalizeiBLane0(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode);
            }
            _node.iConstruction.isBLane1Done = true;
            _node.iConstruction.isBLane0Done = true;

            if (_intersection.isFlipped && !_isFirstInterNode)
            {
                if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _node.iConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                    _node.iConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                }
                else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    _node.iConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                    _node.iConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                }
                else
                {
                    _node.iConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    _node.iConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight)); //b1RAdded = true;
                }
            }
            else
            {
                if (_isLRtoRR)
                {
                    _node.iConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[1], _intHeight));
                    _node.iConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[2], _intHeight)); //b1RAdded = true;
                }
                else if (_isLLtoLR)
                {
                    _node.iConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[1], _intHeight));
                    _node.iConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[2], _intHeight)); //b1RAdded = true;
                }
            }
            _node.iConstruction.isBLane1DoneFinal = true;
            _node.iConstruction.isBLane1DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                _node.iConstruction.isBackRRPassed = true;
            }
        }


        private static void InterFinalizeiBLane2(ref GSDSplineN _node, ref GSDRoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is2LAdded, ref bool _is1LAdded, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_node.iConstruction.isBLane2DoneFinal)
            {
                return;
            }

            if (_is1LAdded && !_node.iConstruction.isBLane1DoneFinal)
            {
                _node.iConstruction.iBLane1L.RemoveAt(_node.iConstruction.iBLane1L.Count - 1);
                _is1LAdded = false;
                InterFinalizeiBLane1(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode, ref _is0LAdded, ref _is1RAdded);
            }
            _node.iConstruction.isBLane1Done = true;
            _node.iConstruction.isBLane2Done = true;

            if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes || _intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
            {
                if (_intersection.isFlipped && !_isFirstInterNode)
                {
                    if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        _node.iConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                        _node.iConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    }
                    else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        _node.iConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                        _node.iConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight));
                    }
                }
                else
                {
                    if (_isLRtoRR)
                    {
                        _node.iConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[2], _intHeight));
                        _node.iConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[3], _intHeight));
                    }
                    else if (_isLLtoLR)
                    {
                        _node.iConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[2], _intHeight));
                        _node.iConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[3], _intHeight));
                    }
                }
            }
            _node.iConstruction.isBLane2DoneFinal = true;
            _node.iConstruction.isBLane2DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
            {
                _node.iConstruction.isBackRRPassed = true;
            }
        }


        private static void InterFinalizeiBLane3(ref GSDSplineN _node, ref GSDRoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is2LAdded, ref bool _is1LAdded, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_is2LAdded && !_node.iConstruction.isBLane2DoneFinal)
            {
                _node.iConstruction.iBLane2L.RemoveAt(_node.iConstruction.iBLane2L.Count - 1);
                _is2LAdded = false;
                InterFinalizeiBLane2(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode, ref _is2LAdded, ref _is1LAdded, ref _is0LAdded, ref _is1RAdded);
            }
            _node.iConstruction.isBLane2Done = true;
            _node.iConstruction.isBLane3Done = true;
            if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                if (_intersection.isFlipped && !_isFirstInterNode)
                {
                    _node.iConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    _node.iConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight));
                }
                else
                {
                    if (_isLRtoRR)
                    {
                        _node.iConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[3], _intHeight));
                        _node.iConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[4], _intHeight));
                    }
                    else if (_isLLtoLR)
                    {
                        _node.iConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[3], _intHeight));
                        _node.iConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[4], _intHeight));
                    }
                }
            }
            _node.iConstruction.isBLane3DoneFinal = true;
            _node.iConstruction.isBLane3DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                _node.iConstruction.isBackRRPassed = true;
            }
        }
        #endregion

        #endregion


        #region "Intersection Prelim"
        private static void RoadJobPrelimInter(ref GSDRoad _road)
        {
            GSDSplineC spline = _road.spline;
            float roadWidth = _road.RoadWidth();
            float shoulderWidth = _road.shoulderWidth;
            float roadSeperation = roadWidth / 2f;
            float roadSeperationNoTurn = roadWidth / 2f;
            float shoulderSeperation = roadSeperation + shoulderWidth;
            float laneWidth = _road.laneWidth;
            float roadSep1Lane = (roadSeperation + (laneWidth * 0.5f));
            float roadSep2Lane = (roadSeperation + (laneWidth * 1.5f));
            Vector3 POS = default(Vector3);
            bool isPastInter = false;
            bool isOldMethod = false;
            //bool bCancel = false; if (bTempCancel) { }
#pragma warning disable CS0219
            bool isTempCancel = false;
#pragma warning restore CS0219

            //If left collides with left, etc

            //This will speed up later calculations for intersection 4 corner construction:
            int nodeCount = spline.GetNodeCount();
            float PreInter_RoadWidthMod = 4.5f;
            if (!isOldMethod)
            {
                PreInter_RoadWidthMod = 5.5f;
            }
            float preInterDistance = (spline.RoadWidth * PreInter_RoadWidthMod) / spline.distance;
            GSDSplineN iNode = null;
            for (int j = 0; j < nodeCount; j++)
            {
                isTempCancel = false;
                if (spline.mNodes[j].bIsIntersection)
                {
                    iNode = spline.mNodes[j];
                    //First node set min / max float:
                    if (iNode.iConstruction == null)
                    {
                        iNode.iConstruction = new GSD.Roads.GSDIntersections.iConstructionMaker();
                    }
                    if (!iNode.iConstruction.isTempConstructionProcessedInter1)
                    {
                        preInterDistance = (iNode.GSDSpline.RoadWidth * PreInter_RoadWidthMod) / iNode.GSDSpline.distance;
                        iNode.iConstruction.tempconstruction_InterStart = iNode.tTime - preInterDistance;
                        iNode.iConstruction.tempconstruction_InterEnd = iNode.tTime + preInterDistance;
                        if (iNode.iConstruction.tempconstruction_InterStart > 1f)
                        {
                            iNode.iConstruction.tempconstruction_InterStart = 1f;
                        }
                        if (iNode.iConstruction.tempconstruction_InterStart < 0f)
                        {
                            iNode.iConstruction.tempconstruction_InterStart = 0f;
                        }
                        if (iNode.iConstruction.tempconstruction_InterEnd > 1f)
                        {
                            iNode.iConstruction.tempconstruction_InterEnd = 1f;
                        }
                        if (iNode.iConstruction.tempconstruction_InterEnd < 0f)
                        {
                            iNode.iConstruction.tempconstruction_InterEnd = 0f;
                        }
                        iNode.iConstruction.isTempConstructionProcessedInter1 = true;
                    }

                    if (string.Compare(iNode.UID, iNode.GSDRI.node1.UID) == 0)
                    {
                        iNode = iNode.GSDRI.node2;
                    }
                    else
                    {
                        iNode = iNode.GSDRI.node1;
                    }

                    //Grab other intersection node and set min / max float	
                    try
                    {
                        if (!iNode.iConstruction.isTempConstructionProcessedInter1)
                        {
                            preInterDistance = (iNode.GSDSpline.RoadWidth * PreInter_RoadWidthMod) / iNode.GSDSpline.distance;
                            iNode.iConstruction.tempconstruction_InterStart = iNode.tTime - preInterDistance;
                            iNode.iConstruction.tempconstruction_InterEnd = iNode.tTime + preInterDistance;
                            if (iNode.iConstruction.tempconstruction_InterStart > 1f)
                            {
                                iNode.iConstruction.tempconstruction_InterStart = 1f;
                            }
                            if (iNode.iConstruction.tempconstruction_InterStart < 0f)
                            {
                                iNode.iConstruction.tempconstruction_InterStart = 0f;
                            }
                            if (iNode.iConstruction.tempconstruction_InterEnd > 1f)
                            {
                                iNode.iConstruction.tempconstruction_InterEnd = 1f;
                            }
                            if (iNode.iConstruction.tempconstruction_InterEnd < 0f)
                            {
                                iNode.iConstruction.tempconstruction_InterEnd = 0f;
                            }
                            iNode.iConstruction.isTempConstructionProcessedInter1 = true;
                        }
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }

            //Now get the four points per intersection:
            GSDSplineN oNode1 = null;
            GSDSplineN oNode2 = null;
            float PreInterPrecision1 = -1f;
            float PreInterPrecision2 = -1f;
            Vector3 PreInterVect = default(Vector3);
            Vector3 PreInterVectR = default(Vector3);
            Vector3 PreInterVectR_RightTurn = default(Vector3);
            Vector3 PreInterVectL = default(Vector3);
            Vector3 PreInterVectL_RightTurn = default(Vector3);
            GSDRoadIntersection GSDRI = null;

            for (int j = 0; j < nodeCount; j++)
            {
                oNode1 = spline.mNodes[j];
                if (oNode1.bIsIntersection)
                {
                    oNode1 = oNode1.GSDRI.node1;
                    oNode2 = oNode1.GSDRI.node2;
                    if (isOldMethod)
                    {
                        PreInterPrecision1 = 0.1f / oNode1.GSDSpline.distance;
                        PreInterPrecision2 = 0.1f / oNode2.GSDSpline.distance;
                    }
                    else
                    {
                        PreInterPrecision1 = 4f / oNode1.GSDSpline.distance;
                        PreInterPrecision2 = 4f / oNode2.GSDSpline.distance;
                    }
                    GSDRI = oNode1.GSDRI;
                    try
                    {
                        if (oNode1.iConstruction.isTempConstructionProcessedInter2 && oNode2.iConstruction.isTempConstructionProcessedInter2)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    GSDRI = oNode1.GSDRI;
                    GSDRI.isCornerRR1Enabled = false;
                    GSDRI.isCornerRR2Enabled = false;
                    GSDRI.isCornerRL1Enabled = false;
                    GSDRI.isCornerRL2Enabled = false;
                    GSDRI.isCornerLR1Enabled = false;
                    GSDRI.isCornerLR2Enabled = false;
                    GSDRI.isCornerLL1Enabled = false;
                    GSDRI.isCornerLL2Enabled = false;

                    if (!oNode1.iConstruction.isTempConstructionProcessedInter2)
                    {
                        oNode1.iConstruction.tempconstruction_R = new List<Vector2>();
                        oNode1.iConstruction.tempconstruction_L = new List<Vector2>();
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            oNode1.iConstruction.tempconstruction_R_RightTurn = new List<Vector2>();
                            oNode1.iConstruction.tempconstruction_L_RightTurn = new List<Vector2>();
                        }

                        for (float i = oNode1.iConstruction.tempconstruction_InterStart; i < oNode1.iConstruction.tempconstruction_InterEnd; i += PreInterPrecision1)
                        {
                            oNode1.GSDSpline.GetSplineValue_Both(i, out PreInterVect, out POS);

                            isPastInter = oNode1.GSDSpline.IntersectionIsPast(ref i, ref oNode1);
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                if (isPastInter)
                                {
                                    PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                    PreInterVectL = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                }
                                else
                                {
                                    PreInterVectR = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                    PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                            }
                            else
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSeperationNoTurn * POS.normalized.z, 0, roadSeperationNoTurn * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSeperationNoTurn * -POS.normalized.z, 0, roadSeperationNoTurn * POS.normalized.x));
                            }

                            oNode1.iConstruction.tempconstruction_R.Add(new Vector2(PreInterVectR.x, PreInterVectR.z));
                            oNode1.iConstruction.tempconstruction_L.Add(new Vector2(PreInterVectL.x, PreInterVectL.z));

                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                PreInterVectR_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                oNode1.iConstruction.tempconstruction_R_RightTurn.Add(ConvertVect3ToVect2(PreInterVectR_RightTurn));

                                PreInterVectL_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                oNode1.iConstruction.tempconstruction_L_RightTurn.Add(ConvertVect3ToVect2(PreInterVectL_RightTurn));
                            }
                        }
                    }

                    //Process second node:
                    if (oNode2.iConstruction == null)
                    {
                        oNode2.iConstruction = new GSD.Roads.GSDIntersections.iConstructionMaker();
                    }
                    if (!oNode2.iConstruction.isTempConstructionProcessedInter2)
                    {
                        oNode2.iConstruction.tempconstruction_R = new List<Vector2>();
                        oNode2.iConstruction.tempconstruction_L = new List<Vector2>();
                        if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            oNode2.iConstruction.tempconstruction_R_RightTurn = new List<Vector2>();
                            oNode2.iConstruction.tempconstruction_L_RightTurn = new List<Vector2>();
                        }

                        for (float i = oNode2.iConstruction.tempconstruction_InterStart; i < oNode2.iConstruction.tempconstruction_InterEnd; i += PreInterPrecision2)
                        {
                            oNode2.GSDSpline.GetSplineValue_Both(i, out PreInterVect, out POS);

                            isPastInter = oNode2.GSDSpline.IntersectionIsPast(ref i, ref oNode2);
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                if (isPastInter)
                                {
                                    PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                    PreInterVectL = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                }
                                else
                                {
                                    PreInterVectR = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                    PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                                }
                            }
                            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                            }
                            else
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSeperationNoTurn * POS.normalized.z, 0, roadSeperationNoTurn * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSeperationNoTurn * -POS.normalized.z, 0, roadSeperationNoTurn * POS.normalized.x));
                            }

                            oNode2.iConstruction.tempconstruction_R.Add(new Vector2(PreInterVectR.x, PreInterVectR.z));
                            oNode2.iConstruction.tempconstruction_L.Add(new Vector2(PreInterVectL.x, PreInterVectL.z));
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                PreInterVectR_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                oNode2.iConstruction.tempconstruction_R_RightTurn.Add(ConvertVect3ToVect2(PreInterVectR_RightTurn));

                                PreInterVectL_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                oNode2.iConstruction.tempconstruction_L_RightTurn.Add(ConvertVect3ToVect2(PreInterVectL_RightTurn));
                            }
                        }
                    }



                    bool isFlipped = false;
                    bool isFlippedSet = false;
                    int hCount1 = oNode1.iConstruction.tempconstruction_R.Count;
                    int hCount2 = oNode2.iConstruction.tempconstruction_R.Count;
                    int N1RCount = oNode1.iConstruction.tempconstruction_R.Count;
                    int N1LCount = oNode1.iConstruction.tempconstruction_L.Count;
                    int N2RCount = oNode2.iConstruction.tempconstruction_R.Count;
                    int N2LCount = oNode2.iConstruction.tempconstruction_L.Count;

                    int[] tCounts = new int[4];
                    tCounts[0] = N1RCount;
                    tCounts[1] = N1LCount;
                    tCounts[2] = N2RCount;
                    tCounts[3] = N2LCount;

                    //RR:
                    int MaxCount = -1;
                    MaxCount = Mathf.Max(N2RCount, N2LCount);
                    for (int h = 0; h < hCount1; h++)
                    {
                        for (int k = 0; k < MaxCount; k++)
                        {
                            if (k < N2RCount)
                            {
                                if (Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_R[k]) < _road.roadDefinition)
                                {
                                    isFlipped = false;
                                    isFlippedSet = true;
                                    break;
                                }
                            }
                            if (k < N2LCount)
                            {
                                if (Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_L[k]) < _road.roadDefinition)
                                {
                                    isFlipped = true;
                                    isFlippedSet = true;
                                    break;
                                }
                            }
                        }
                        if (isFlippedSet)
                        {
                            break;
                        }
                    }
                    oNode1.GSDRI.isFlipped = isFlipped;


                    //Three-way intersections lane specifics:
                    GSDRI.isNode2BLeftTurnLane = true;
                    GSDRI.isNode2BRightTurnLane = true;
                    GSDRI.isNode2FLeftTurnLane = true;
                    GSDRI.isNode2FRightTurnLane = true;

                    //Three-way intersections:
                    GSDRI.ignoreSide = -1;
                    GSDRI.ignoreCorner = -1;
                    GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.FourWay;
                    if (GSDRI.isFirstSpecialFirst)
                    {
                        GSDRI.ignoreSide = 3;
                        GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                        if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                        {
                            GSDRI.ignoreCorner = 0;
                        }
                        else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                        {
                            GSDRI.ignoreCorner = 1;
                        }

                        if (!oNode1.GSDRI.isFlipped)
                        {
                            GSDRI.isNode2FLeftTurnLane = false;
                            GSDRI.isNode2BRightTurnLane = false;
                        }
                        else
                        {
                            GSDRI.isNode2BLeftTurnLane = false;
                            GSDRI.isNode2FRightTurnLane = false;
                        }


                    }
                    else if (GSDRI.isFirstSpecialLast)
                    {
                        GSDRI.ignoreSide = 1;
                        GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                        if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                        {
                            GSDRI.ignoreCorner = 2;
                        }
                        else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                        {
                            GSDRI.ignoreCorner = 3;
                        }

                        if (!oNode1.GSDRI.isFlipped)
                        {
                            GSDRI.isNode2BLeftTurnLane = false;
                            GSDRI.isNode2FRightTurnLane = false;
                        }
                        else
                        {
                            GSDRI.isNode2FLeftTurnLane = false;
                            GSDRI.isNode2BRightTurnLane = false;
                        }

                    }
                    if (!isFlipped)
                    {
                        if (GSDRI.isSecondSpecialFirst)
                        {
                            GSDRI.ignoreSide = 2;
                            GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                GSDRI.ignoreCorner = 3;
                            }
                            else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                GSDRI.ignoreCorner = 0;
                            }

                            if (!oNode1.GSDRI.isFlipped)
                            {
                                GSDRI.isNode2BLeftTurnLane = false;
                                GSDRI.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                GSDRI.isNode2FLeftTurnLane = false;
                                GSDRI.isNode2BRightTurnLane = false;
                            }

                        }
                        else if (GSDRI.isSecondSpecialLast)
                        {
                            GSDRI.ignoreSide = 0;
                            GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                GSDRI.ignoreCorner = 1;
                            }
                            else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                GSDRI.ignoreCorner = 2;
                            }

                            if (!oNode1.GSDRI.isFlipped)
                            {
                                GSDRI.isNode2BLeftTurnLane = false;
                                GSDRI.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                GSDRI.isNode2FLeftTurnLane = false;
                                GSDRI.isNode2BRightTurnLane = false;
                            }

                        }
                    }
                    else
                    {
                        if (GSDRI.isSecondSpecialFirst)
                        {
                            GSDRI.ignoreSide = 0;
                            GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                GSDRI.ignoreCorner = 1;
                            }
                            else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                GSDRI.ignoreCorner = 2;
                            }

                            if (!oNode1.GSDRI.isFlipped)
                            {
                                GSDRI.isNode2BLeftTurnLane = false;
                                GSDRI.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                GSDRI.isNode2FLeftTurnLane = false;
                                GSDRI.isNode2BRightTurnLane = false;
                            }

                        }
                        else if (GSDRI.isSecondSpecialLast)
                        {
                            GSDRI.ignoreSide = 2;
                            GSDRI.intersectionType = GSDRoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                GSDRI.ignoreCorner = 3;
                            }
                            else if (GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight1 || GSDRI.intersectionStopType == GSDRoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                GSDRI.ignoreCorner = 0;
                            }

                            if (!oNode1.GSDRI.isFlipped)
                            {
                                GSDRI.isNode2BLeftTurnLane = false;
                                GSDRI.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                GSDRI.isNode2FLeftTurnLane = false;
                                GSDRI.isNode2BRightTurnLane = false;
                            }
                        }
                    }

                    //Find corners:
                    Vector2 tFoundVectRR = default(Vector2);
                    Vector2 tFoundVectRL = default(Vector2);
                    Vector2 tFoundVectLR = default(Vector2);
                    Vector2 tFoundVectLL = default(Vector2);
                    if (!isOldMethod)
                    {
                        //RR:
                        if (!isFlipped)
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R_RightTurn, ref oNode2.iConstruction.tempconstruction_R);
                            }
                            else
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_R);
                            }
                        }
                        else
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R_RightTurn, ref oNode2.iConstruction.tempconstruction_L);
                            }
                            else
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_L);
                            }
                        }

                        //RL:
                        if (!isFlipped)
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_L_RightTurn);
                            }
                            else
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_L);
                            }
                        }
                        else
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_R_RightTurn);
                            }
                            else
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_R, ref oNode2.iConstruction.tempconstruction_R);
                            }
                        }

                        //LL:
                        if (!isFlipped)
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L_RightTurn, ref oNode2.iConstruction.tempconstruction_L);
                            }
                            else
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_L);
                            }
                        }
                        else
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L_RightTurn, ref oNode2.iConstruction.tempconstruction_R);
                            }
                            else
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_R);
                            }
                        }

                        //LR:
                        if (!isFlipped)
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_R_RightTurn);
                            }
                            else
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_R);
                            }
                        }
                        else
                        {
                            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_L_RightTurn);
                            }
                            else
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.iConstruction.tempconstruction_L, ref oNode2.iConstruction.tempconstruction_L);
                            }
                        }
                    }
                    else
                    {
                        //Now two lists of R and L on each intersection node, now match:
                        float eDistanceRR = 5000f;
                        float oDistanceRR = 0f;
                        float eDistanceRL = 5000f;
                        float oDistanceRL = 0f;
                        float eDistanceLR = 5000f;
                        float oDistanceLR = 0f;
                        float eDistanceLL = 5000f;
                        float oDistanceLL = 0f;
                        bool isHasBeen1mRR = false;
                        bool isHasBeen1mRL = false;
                        bool isHasBeen1mLR = false;
                        bool isHasBeen1mLL = false;
                        bool isHasBeen1mRR_ignore = false;
                        bool isHasBeen1mRL_ignore = false;
                        bool isHasBeen1mLR_ignore = false;
                        bool isHasBeen1mLL_ignore = false;
                        bool isHasBeen1mRRIgnoreMax = false;
                        bool isHasBeen1mRLIgnoreMax = false;
                        bool isHasBeen1mLRIgnoreMax = false;
                        bool isHasBeen1mLLIgnoreMax = false;
                        float mMin = 0.2f;
                        float mMax = 0.5f;

                        MaxCount = Mathf.Max(tCounts);
                        int MaxHCount = Mathf.Max(hCount1, hCount2);
                        for (int h = 0; h < MaxHCount; h++)
                        {
                            isHasBeen1mRR = false;
                            isHasBeen1mRL = false;
                            isHasBeen1mLR = false;
                            isHasBeen1mLL = false;
                            isHasBeen1mRR_ignore = false;
                            isHasBeen1mRL_ignore = false;
                            isHasBeen1mLR_ignore = false;
                            isHasBeen1mLL_ignore = false;
                            for (int k = 0; k < MaxCount; k++)
                            {
                                if (!isFlipped)
                                {
                                    //RR:
                                    if (!isHasBeen1mRRIgnoreMax && !isHasBeen1mRR_ignore && (h < N1RCount && k < N2RCount))
                                    {
                                        oDistanceRR = Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_R[k]);
                                        if (oDistanceRR < eDistanceRR)
                                        {
                                            eDistanceRR = oDistanceRR;
                                            tFoundVectRR = oNode1.iConstruction.tempconstruction_R[h]; //RR
                                            if (eDistanceRR < 0.07f)
                                            {
                                                isHasBeen1mRRIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceRR > mMax && isHasBeen1mRR)
                                        {
                                            isHasBeen1mRR_ignore = true;
                                        }
                                        if (oDistanceRR < mMin)
                                        {
                                            isHasBeen1mRR = true;
                                        }
                                    }
                                    //RL:
                                    if (!isHasBeen1mRLIgnoreMax && !isHasBeen1mRL_ignore && (h < N1RCount && k < N2LCount))
                                    {
                                        oDistanceRL = Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_L[k]);
                                        if (oDistanceRL < eDistanceRL)
                                        {
                                            eDistanceRL = oDistanceRL;
                                            tFoundVectRL = oNode1.iConstruction.tempconstruction_R[h]; //RL
                                            if (eDistanceRL < 0.07f)
                                            {
                                                isHasBeen1mRLIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceRL > mMax && isHasBeen1mRL)
                                        {
                                            isHasBeen1mRL_ignore = true;
                                        }
                                        if (oDistanceRL < mMin)
                                        {
                                            isHasBeen1mRL = true;
                                        }
                                    }
                                    //LR:
                                    if (!isHasBeen1mLRIgnoreMax && !isHasBeen1mLR_ignore && (h < N1LCount && k < N2RCount))
                                    {
                                        oDistanceLR = Vector2.Distance(oNode1.iConstruction.tempconstruction_L[h], oNode2.iConstruction.tempconstruction_R[k]);
                                        if (oDistanceLR < eDistanceLR)
                                        {
                                            eDistanceLR = oDistanceLR;
                                            tFoundVectLR = oNode1.iConstruction.tempconstruction_L[h]; //LR
                                            if (eDistanceLR < 0.07f)
                                            {
                                                isHasBeen1mLRIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceLR > mMax && isHasBeen1mLR)
                                        {
                                            isHasBeen1mLR_ignore = true;
                                        }
                                        if (oDistanceLR < mMin)
                                        {
                                            isHasBeen1mLR = true;
                                        }
                                    }
                                    //LL:
                                    if (!isHasBeen1mLLIgnoreMax && !isHasBeen1mLL_ignore && (h < N1LCount && k < N2LCount))
                                    {
                                        oDistanceLL = Vector2.Distance(oNode1.iConstruction.tempconstruction_L[h], oNode2.iConstruction.tempconstruction_L[k]);
                                        if (oDistanceLL < eDistanceLL)
                                        {
                                            eDistanceLL = oDistanceLL;
                                            tFoundVectLL = oNode1.iConstruction.tempconstruction_L[h]; //LL
                                            if (eDistanceLL < 0.07f)
                                            {
                                                isHasBeen1mLLIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceLL > mMax && isHasBeen1mLL)
                                        {
                                            isHasBeen1mLL_ignore = true;
                                        }
                                        if (oDistanceLL < mMin)
                                        {
                                            isHasBeen1mLL = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //RR:
                                    if (!isHasBeen1mRRIgnoreMax && !isHasBeen1mRR_ignore && (h < N1RCount && k < N2LCount))
                                    {
                                        oDistanceRR = Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_L[k]);
                                        if (oDistanceRR < eDistanceRR)
                                        {
                                            eDistanceRR = oDistanceRR;
                                            tFoundVectRR = oNode1.iConstruction.tempconstruction_R[h]; //RR
                                            if (eDistanceRR < 0.07f)
                                            {
                                                isHasBeen1mRRIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceRR > mMax && isHasBeen1mRR)
                                        {
                                            isHasBeen1mRR_ignore = true;
                                        }
                                        if (oDistanceRR < mMin)
                                        {
                                            isHasBeen1mRR = true;
                                        }
                                    }
                                    //RL:
                                    if (!isHasBeen1mRLIgnoreMax && !isHasBeen1mRL_ignore && (h < N1RCount && k < N2RCount))
                                    {
                                        oDistanceRL = Vector2.Distance(oNode1.iConstruction.tempconstruction_R[h], oNode2.iConstruction.tempconstruction_R[k]);
                                        if (oDistanceRL < eDistanceRL)
                                        {
                                            eDistanceRL = oDistanceRL;
                                            tFoundVectRL = oNode1.iConstruction.tempconstruction_R[h]; //RL
                                            if (eDistanceRL < 0.07f)
                                            {
                                                isHasBeen1mRLIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceRL > mMax && isHasBeen1mRL)
                                        {
                                            isHasBeen1mRL_ignore = true;
                                        }
                                        if (oDistanceRL < mMin)
                                        {
                                            isHasBeen1mRL = true;
                                        }
                                    }
                                    //LR:
                                    if (!isHasBeen1mLRIgnoreMax && !isHasBeen1mLR_ignore && (h < N1LCount && k < N2LCount))
                                    {
                                        oDistanceLR = Vector2.Distance(oNode1.iConstruction.tempconstruction_L[h], oNode2.iConstruction.tempconstruction_L[k]);
                                        if (oDistanceLR < eDistanceLR)
                                        {
                                            eDistanceLR = oDistanceLR;
                                            tFoundVectLR = oNode1.iConstruction.tempconstruction_L[h]; //LR
                                            if (eDistanceLR < 0.07f)
                                            {
                                                isHasBeen1mLRIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceLR > mMax && isHasBeen1mLR)
                                        {
                                            isHasBeen1mLR_ignore = true;
                                        }
                                        if (oDistanceLR < mMin)
                                        {
                                            isHasBeen1mLR = true;
                                        }
                                    }
                                    //LL:
                                    if (!isHasBeen1mLLIgnoreMax && !isHasBeen1mLL_ignore && (h < N1LCount && k < N2RCount))
                                    {
                                        oDistanceLL = Vector2.Distance(oNode1.iConstruction.tempconstruction_L[h], oNode2.iConstruction.tempconstruction_R[k]);
                                        if (oDistanceLL < eDistanceLL)
                                        {
                                            eDistanceLL = oDistanceLL;
                                            tFoundVectLL = oNode1.iConstruction.tempconstruction_L[h]; //LL
                                            if (eDistanceLL < 0.07f)
                                            {
                                                isHasBeen1mLLIgnoreMax = true;
                                            }
                                        }
                                        if (oDistanceLL > mMax && isHasBeen1mLL)
                                        {
                                            isHasBeen1mLL_ignore = true;
                                        }
                                        if (oDistanceLL < mMin)
                                        {
                                            isHasBeen1mLL = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    oNode1.iConstruction.isTempConstructionProcessedInter2 = true;
                    oNode2.iConstruction.isTempConstructionProcessedInter2 = true;

                    Vector3 tVectRR = new Vector3(tFoundVectRR.x, 0f, tFoundVectRR.y);
                    Vector3 tVectRL = new Vector3(tFoundVectRL.x, 0f, tFoundVectRL.y);
                    Vector3 tVectLR = new Vector3(tFoundVectLR.x, 0f, tFoundVectLR.y);
                    Vector3 tVectLL = new Vector3(tFoundVectLL.x, 0f, tFoundVectLL.y);

                    oNode1.GSDRI.cornerRR = tVectRR;
                    oNode1.GSDRI.cornerRL = tVectRL;
                    oNode1.GSDRI.cornerLR = tVectLR;
                    oNode1.GSDRI.cornerLL = tVectLL;

                    float[] tMaxFloats = new float[4];
                    tMaxFloats[0] = Vector3.Distance(((tVectRR - tVectRL) * 0.5f) + tVectRL, oNode1.pos) * 1.25f;
                    tMaxFloats[1] = Vector3.Distance(((tVectRR - tVectLR) * 0.5f) + tVectLR, oNode1.pos) * 1.25f;
                    tMaxFloats[2] = Vector3.Distance(((tVectRL - tVectLL) * 0.5f) + tVectLL, oNode1.pos) * 1.25f;
                    tMaxFloats[3] = Vector3.Distance(((tVectLR - tVectLL) * 0.5f) + tVectLL, oNode1.pos) * 1.25f;
                    GSDRI.maxInterDistance = Mathf.Max(tMaxFloats);

                    float[] tMaxFloatsSQ = new float[4];
                    tMaxFloatsSQ[0] = Vector3.SqrMagnitude((((tVectRR - tVectRL) * 0.5f) + tVectRL) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[1] = Vector3.SqrMagnitude((((tVectRR - tVectLR) * 0.5f) + tVectLR) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[2] = Vector3.SqrMagnitude((((tVectRL - tVectLL) * 0.5f) + tVectLL) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[3] = Vector3.SqrMagnitude((((tVectLR - tVectLL) * 0.5f) + tVectLL) - oNode1.pos) * 1.25f;
                    GSDRI.maxInterDistanceSQ = Mathf.Max(tMaxFloatsSQ);

                    float TotalLanes = (int) (roadWidth / laneWidth);
                    float TotalLanesI = TotalLanes;
                    float LanesPerSide = TotalLanes / 2f;

                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        TotalLanesI = TotalLanes + 2f;
                        //Lower left to lower right: 
                        GSDRI.cornerLRCornerRR = new Vector3[5];
                        GSDRI.cornerLRCornerRR[0] = tVectLR;
                        GSDRI.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * (LanesPerSide / TotalLanesI)) + tVectLR;
                        GSDRI.cornerLRCornerRR[2] = ((tVectRR - tVectLR) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLR;
                        GSDRI.cornerLRCornerRR[3] = ((tVectRR - tVectLR) * ((LanesPerSide + 1 + LanesPerSide) / TotalLanesI)) + tVectLR;
                        GSDRI.cornerLRCornerRR[4] = tVectRR;
                        //Upper right to lower right:
                        GSDRI.cornerRLCornerRR = new Vector3[5];
                        GSDRI.cornerRLCornerRR[0] = tVectRL;
                        GSDRI.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * (1 / TotalLanesI)) + tVectRL;
                        GSDRI.cornerRLCornerRR[2] = ((tVectRR - tVectRL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectRL;
                        GSDRI.cornerRLCornerRR[3] = ((tVectRR - tVectRL) * ((LanesPerSide + 2) / TotalLanesI)) + tVectRL;
                        GSDRI.cornerRLCornerRR[4] = tVectRR;
                        //Upper left to upper right:
                        GSDRI.cornerLLCornerRL = new Vector3[5];
                        GSDRI.cornerLLCornerRL[0] = tVectLL;
                        GSDRI.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * (1 / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerRL[2] = ((tVectRL - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerRL[3] = ((tVectRL - tVectLL) * ((LanesPerSide + 2) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerRL[4] = tVectRL;
                        //Upper left to lower left:
                        GSDRI.cornerLLCornerLR = new Vector3[5];
                        GSDRI.cornerLLCornerLR[0] = tVectLL;
                        GSDRI.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerLR[2] = ((tVectLR - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerLR[3] = ((tVectLR - tVectLL) * ((LanesPerSide + 1 + LanesPerSide) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerLR[4] = tVectLR;
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        TotalLanesI = TotalLanes + 1;
                        //Lower left to lower right:
                        GSDRI.cornerLRCornerRR = new Vector3[4];
                        GSDRI.cornerLRCornerRR[0] = tVectLR;
                        GSDRI.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * (LanesPerSide / TotalLanesI)) + tVectLR;
                        GSDRI.cornerLRCornerRR[2] = ((tVectRR - tVectLR) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLR;
                        GSDRI.cornerLRCornerRR[3] = tVectRR;
                        //Upper right to lower right:
                        GSDRI.cornerRLCornerRR = new Vector3[4];
                        GSDRI.cornerRLCornerRR[0] = tVectRL;
                        GSDRI.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * (LanesPerSide / TotalLanesI)) + tVectRL;
                        GSDRI.cornerRLCornerRR[2] = ((tVectRR - tVectRL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectRL;
                        GSDRI.cornerRLCornerRR[3] = tVectRR;
                        //Upper left to upper right:
                        GSDRI.cornerLLCornerRL = new Vector3[4];
                        GSDRI.cornerLLCornerRL[0] = tVectLL;
                        GSDRI.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerRL[2] = ((tVectRL - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerRL[3] = tVectRL;
                        //Upper left to lower left:
                        GSDRI.cornerLLCornerLR = new Vector3[4];
                        GSDRI.cornerLLCornerLR[0] = tVectLL;
                        GSDRI.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerLR[2] = ((tVectLR - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        GSDRI.cornerLLCornerLR[3] = tVectLR;
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        TotalLanesI = TotalLanes + 0;
                        //Lower left to lower right:
                        GSDRI.cornerLRCornerRR = new Vector3[3];
                        GSDRI.cornerLRCornerRR[0] = tVectLR;
                        GSDRI.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * 0.5f) + tVectLR;
                        GSDRI.cornerLRCornerRR[2] = tVectRR;
                        //Upper right to lower right:
                        GSDRI.cornerRLCornerRR = new Vector3[3];
                        GSDRI.cornerRLCornerRR[0] = tVectRL;
                        GSDRI.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * 0.5f) + tVectRL;
                        GSDRI.cornerRLCornerRR[2] = tVectRR;
                        //Upper left to upper right:
                        GSDRI.cornerLLCornerRL = new Vector3[3];
                        GSDRI.cornerLLCornerRL[0] = tVectLL;
                        GSDRI.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * 0.5f) + tVectLL;
                        GSDRI.cornerLLCornerRL[2] = tVectRL;
                        //Upper left to lower left:
                        GSDRI.cornerLLCornerLR = new Vector3[3];
                        GSDRI.cornerLLCornerLR[0] = tVectLL;
                        GSDRI.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * 0.5f) + tVectLL;
                        GSDRI.cornerLLCornerLR[2] = tVectLR;
                    }

                    //Use node1/node2 for angles instead
                    float tShoulderWidth = shoulderWidth * 1.75f;
                    float tRampWidth = shoulderWidth * 2f;

                    oNode1.GSDRI.oddAngle = Vector3.Angle(GSDRI.node2.tangent, GSDRI.node1.tangent);
                    oNode1.GSDRI.evenAngle = 180f - Vector3.Angle(GSDRI.node2.tangent, GSDRI.node1.tangent);

                    GSD.Roads.GSDIntersectionObjects.GetFourPoints(GSDRI, out GSDRI.cornerRROuter, out GSDRI.cornerRLOuter, out GSDRI.cornerLLOuter, out GSDRI.cornerLROuter, tShoulderWidth);
                    GSD.Roads.GSDIntersectionObjects.GetFourPoints(GSDRI, out GSDRI.cornerRRRampOuter, out GSDRI.cornerRLRampOuter, out GSDRI.cornerLLRampOuter, out GSDRI.cornerLRRampOuter, tRampWidth);

                    GSDRI.ConstructBoundsRect();
                    GSDRI.cornerRR2D = new Vector2(tVectRR.x, tVectRR.z);
                    GSDRI.cornerRL2D = new Vector2(tVectRL.x, tVectRL.z);
                    GSDRI.cornerLL2D = new Vector2(tVectLL.x, tVectLL.z);
                    GSDRI.cornerLR2D = new Vector2(tVectLR.x, tVectLR.z);

                    if (!oNode1.GSDRI.isSameSpline)
                    {
                        if (string.Compare(_road.spline.UID, oNode1.GSDSpline.tRoad.spline.UID) != 0)
                        {
                            AddIntersectionBounds(ref oNode1.GSDSpline.tRoad, ref _road.RCS);
                        }
                        else if (string.Compare(_road.spline.UID, oNode2.GSDSpline.tRoad.spline.UID) != 0)
                        {
                            AddIntersectionBounds(ref oNode2.GSDSpline.tRoad, ref _road.RCS);
                        }
                    }
                }
            }
        }


        private static Vector2 IntersectionCornerCalc(ref List<Vector2> _primaryList, ref List<Vector2> _secondaryList)
        {
            int PrimaryCount = _primaryList.Count;
            int SecondaryCount = _secondaryList.Count;
            Vector2 t2D_Line1Start = default(Vector2);
            Vector2 t2D_Line1End = default(Vector2);
            Vector2 t2D_Line2Start = default(Vector2);
            Vector2 t2D_Line2End = default(Vector2);
            bool isDidIntersect = false;
            Vector2 tIntersectLocation = default(Vector2);
            for (int i = 1; i < PrimaryCount; i++)
            {
                isDidIntersect = false;
                t2D_Line1Start = _primaryList[i - 1];
                t2D_Line1End = _primaryList[i];
                for (int k = 1; k < SecondaryCount; k++)
                {
                    isDidIntersect = false;
                    t2D_Line2Start = _secondaryList[k - 1];
                    t2D_Line2End = _secondaryList[k];
                    isDidIntersect = GSDRootUtil.Intersects2D(ref t2D_Line1Start, ref t2D_Line1End, ref t2D_Line2Start, ref t2D_Line2End, out tIntersectLocation);
                    if (isDidIntersect)
                    {
                        return tIntersectLocation;
                    }
                }
            }
            return tIntersectLocation;
        }


        private static void AddIntersectionBounds(ref GSDRoad _road, ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
#pragma warning disable CS0219
            bool isBridge = false;
            bool isBridgeInitial = false;
            bool isTempBridge = false;
            bool isBridgeLast = false;

            bool isTunnel = false;
            bool isTunnelInitial = false;
            bool isTempTunnel = false;
            bool isTunnelLast = false;

            GSDRoadIntersection GSDRI = null;
            bool isPastInter = false;
            bool isMaxIntersection = false;
            bool isWasPrevMaxInter = false;
            Vector3 tVect = default(Vector3);
            Vector3 POS = default(Vector3);
            float tIntHeight = 0f;
            float tIntStrength = 0f;
            float tIntStrength_temp = 0f;
            //float tIntDistCheck = 75f;
            bool isFirstInterNode = false;
            Vector3 tVect_Prev = default(Vector3);
            Vector3 rVect_Prev = default(Vector3);
            Vector3 lVect_Prev = default(Vector3);
            Vector3 rVect = default(Vector3);
            Vector3 lVect = default(Vector3);
            Vector3 ShoulderR_rVect = default(Vector3);
            Vector3 ShoulderR_lVect = default(Vector3);
            Vector3 ShoulderL_rVect = default(Vector3);
            Vector3 ShoulderL_lVect = default(Vector3);

            Vector3 RampR_R = default(Vector3);
            Vector3 RampR_L = default(Vector3);
            Vector3 RampL_R = default(Vector3);
            Vector3 RampL_L = default(Vector3);

            float ShoulderR_OuterAngle = 0f;
            float ShoulderL_OuterAngle = 0f;
#pragma warning restore CS0219
            Vector3 ShoulderR_PrevLVect = default(Vector3);
            Vector3 ShoulderL_PrevRVect = default(Vector3);
            Vector3 ShoulderR_PrevRVect = default(Vector3);
            Vector3 ShoulderL_PrevLVect = default(Vector3);
            //Vector3 ShoulderR_PrevRVect2 = default(Vector3);
            //Vector3 ShoulderL_PrevLVect2 = default(Vector3);
            //Vector3 ShoulderR_PrevRVect3 = default(Vector3);
            //Vector3 ShoulderL_PrevLVect3 = default(Vector3);
            Vector3 RampR_PrevR = default(Vector3);
            Vector3 RampR_PrevL = default(Vector3);
            Vector3 RampL_PrevR = default(Vector3);
            Vector3 RampL_PrevL = default(Vector3);
            GSDSplineC tSpline = _road.spline;
            //Road width:
            float RoadWidth = _road.RoadWidth();
            float ShoulderWidth = _road.shoulderWidth;
            float RoadSeperation = RoadWidth / 2f;
            float RoadSeperation_NoTurn = RoadWidth / 2f;
            float ShoulderSeperation = RoadSeperation + ShoulderWidth;
            float LaneWidth = _road.laneWidth;
            float RoadSep1Lane = (RoadSeperation + (LaneWidth * 0.5f));
            float RoadSep2Lane = (RoadSeperation + (LaneWidth * 1.5f));
            float ShoulderSep1Lane = (ShoulderSeperation + (LaneWidth * 0.5f));
            float ShoulderSep2Lane = (ShoulderSeperation + (LaneWidth * 1.5f));

            //float tAngle = 0f;
            //float OrigStep = 0.06f;
            float Step = _road.roadDefinition / tSpline.distance;

            GSDSplineN xNode = null;
            float tInterSubtract = 4f;
            float tLastInterHeight = -4f;

            //GameObject xObj = null;
            //xObj = GameObject.Find("temp22");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp22");
            //}
            //xObj = GameObject.Find("temp23");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp23");
            //}
            //xObj = GameObject.Find("temp22_RR");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp22_RR");
            //}
            //xObj = GameObject.Find("temp22_RL");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp22_RL");
            //}
            //xObj = GameObject.Find("temp22_LR");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp22_LR");
            //}
            //xObj = GameObject.Find("temp22_LL");
            //while(xObj != null)
            //{
            //	Object.DestroyImmediate(xObj);
            //	xObj = GameObject.Find("temp22_LL");
            //}

            bool isFinalEnd = false;
            float i = 0f;

            float FinalMax = 1f;
            float StartMin = 0f;
            if (tSpline.bSpecialEndControlNode)
            {
                FinalMax = tSpline.mNodes[tSpline.GetNodeCount() - 2].tTime;
            }
            if (tSpline.bSpecialStartControlNode)
            {
                StartMin = tSpline.mNodes[1].tTime;
            }

            //			int StartIndex = tSpline.GetClosestRoadDefIndex(StartMin,true,false);
            //			int EndIndex = tSpline.GetClosestRoadDefIndex(FinalMax,false,true);
            //			float cDist = 0f;
            bool kSkip = true;
            bool kSkipFinal = false;
            int kCount = 0;
            int kFinalCount = tSpline.RoadDefKeysArray.Length;
            int spamcheckmax1 = 18000;
            int spamcheck1 = 0;

            if (GSDRootUtil.IsApproximately(StartMin, 0f, 0.0001f))
            {
                kSkip = false;
            }
            if (GSDRootUtil.IsApproximately(FinalMax, 1f, 0.0001f))
            {
                kSkipFinal = true;
            }

            while (!isFinalEnd && spamcheck1 < spamcheckmax1)
            {
                spamcheck1++;

                if (kSkip)
                {
                    i = StartMin;
                    kSkip = false;
                }
                else
                {
                    if (kCount >= kFinalCount)
                    {
                        i = FinalMax;
                        if (kSkipFinal)
                        { break; }
                    }
                    else
                    {
                        i = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[kCount]);
                        kCount += 1;
                    }
                }

                if (i > 1f)
                {
                    break;
                }
                if (i < 0f)
                {
                    i = 0f;
                }

                if (GSDRootUtil.IsApproximately(i, FinalMax, 0.00001f))
                {
                    isFinalEnd = true;
                }
                else if (i > FinalMax)
                {
                    if (tSpline.bSpecialEndControlNode)
                    {
                        i = FinalMax;
                        isFinalEnd = true;
                    }
                    else
                    {
                        isFinalEnd = true;
                        break;
                    }
                }

                tSpline.GetSplineValue_Both(i, out tVect, out POS);
                isPastInter = false;
                tIntStrength = tSpline.IntersectionStrength(ref tVect, ref tIntHeight, ref GSDRI, ref isPastInter, ref i, ref xNode);
                if (GSDRootUtil.IsApproximately(tIntStrength, 1f, 0.001f) || tIntStrength > 1f)
                {
                    isMaxIntersection = true;
                }
                else
                {
                    isMaxIntersection = false;
                }

                if (isMaxIntersection)
                {
                    if (string.Compare(xNode.UID, GSDRI.node1.UID) == 0)
                    {
                        isFirstInterNode = true;
                    }
                    else
                    {
                        isFirstInterNode = false;
                    }

                    //Convoluted for initial trigger:
                    isBridgeInitial = false;
                    isBridgeLast = false;
                    isTempBridge = tSpline.IsInBridge(i);
                    if (!isBridge && isTempBridge)
                    {
                        isBridge = true;
                        isBridgeInitial = true;
                    }
                    else if (isBridge && !isTempBridge)
                    {
                        isBridge = false;
                    }
                    //Check if this is the last bridge run for this bridge:
                    if (isBridge)
                    {
                        isTempBridge = tSpline.IsInBridge(i + Step);
                        if (!isTempBridge)
                        {
                            isBridgeLast = true;
                        }
                    }


                    //Convoluted for initial trigger:
                    isTunnelInitial = false;
                    isTunnelLast = false;
                    isTempTunnel = tSpline.IsInTunnel(i);
                    if (!isTunnel && isTempTunnel)
                    {
                        isTunnel = true;
                        isTunnelInitial = true;
                    }
                    else if (isTunnel && !isTempTunnel)
                    {
                        isTunnel = false;
                    }
                    //Check if this is the last Tunnel run for this Tunnel:
                    if (isTunnel)
                    {
                        isTempTunnel = tSpline.IsInTunnel(i + Step);
                        if (!isTempTunnel)
                        {
                            isTunnelLast = true;
                        }
                    }

                    if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSeperation_NoTurn * POS.normalized.z, 0, RoadSeperation_NoTurn * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSeperation_NoTurn * -POS.normalized.z, 0, RoadSeperation_NoTurn * POS.normalized.x));
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSep1Lane * POS.normalized.z, 0, RoadSep1Lane * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSep1Lane * -POS.normalized.z, 0, RoadSep1Lane * POS.normalized.x));
                    }
                    else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (isPastInter)
                        {
                            rVect = (tVect + new Vector3(RoadSep1Lane * POS.normalized.z, 0, RoadSep1Lane * -POS.normalized.x));
                            lVect = (tVect + new Vector3(RoadSep2Lane * -POS.normalized.z, 0, RoadSep2Lane * POS.normalized.x));
                        }
                        else
                        {
                            rVect = (tVect + new Vector3(RoadSep2Lane * POS.normalized.z, 0, RoadSep2Lane * -POS.normalized.x));
                            lVect = (tVect + new Vector3(RoadSep1Lane * -POS.normalized.z, 0, RoadSep1Lane * POS.normalized.x));
                        }
                    }
                    else
                    {
                        rVect = (tVect + new Vector3(RoadSeperation * POS.normalized.z, 0, RoadSeperation * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSeperation * -POS.normalized.z, 0, RoadSeperation * POS.normalized.x));
                    }

                    if (tIntStrength >= 1f)
                    {
                        tVect.y -= tInterSubtract;
                        tLastInterHeight = tVect.y;
                        rVect.y -= tInterSubtract;
                        lVect.y -= tInterSubtract;
                    }
                    else
                    {
                        if (!GSDRootUtil.IsApproximately(tIntStrength, 0f, 0.001f))
                        {
                            tVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * tVect.y);
                        }
                        tIntStrength_temp = _road.spline.IntersectionStrength(ref rVect, ref tIntHeight, ref GSDRI, ref isPastInter, ref i, ref xNode);
                        if (!GSDRootUtil.IsApproximately(tIntStrength_temp, 0f, 0.001f))
                        {
                            rVect.y = (tIntStrength_temp * tIntHeight) + ((1 - tIntStrength_temp) * rVect.y);
                            ShoulderR_lVect = rVect;
                        }
                    }

                    //Add bounds for later removal:
                    GSD.Roads.GSDRoadUtil.Construction2DRect vRect = null;
                    if (!isBridge && !isTunnel && isMaxIntersection && isWasPrevMaxInter)
                    {
                        bool isGoAhead = true;
                        if (xNode.bIsEndPoint)
                        {
                            if (xNode.idOnSpline == 1)
                            {
                                if (i < xNode.tTime)
                                {
                                    isGoAhead = false;
                                }
                            }
                            else
                            {
                                if (i > xNode.tTime)
                                {
                                    isGoAhead = false;
                                }
                            }
                        }
                        //Get this and prev lvect rvect rects:
                        if (Vector3.Distance(xNode.pos, tVect) < (3f * RoadWidth) && isGoAhead)
                        {
                            if (GSDRI.isFlipped && !isFirstInterNode)
                            {
                                vRect = new GSD.Roads.GSDRoadUtil.Construction2DRect(
                                    new Vector2(rVect.x, rVect.z),
                                    new Vector2(lVect.x, lVect.z),
                                    new Vector2(rVect_Prev.x, rVect_Prev.z),
                                    new Vector2(lVect_Prev.x, lVect_Prev.z),
                                    tLastInterHeight
                                    );
                            }
                            else
                            {
                                vRect = new GSD.Roads.GSDRoadUtil.Construction2DRect(
                                   new Vector2(lVect.x, lVect.z),
                                   new Vector2(rVect.x, rVect.z),
                                   new Vector2(lVect_Prev.x, lVect_Prev.z),
                                   new Vector2(rVect_Prev.x, rVect_Prev.z),
                                   tLastInterHeight
                                   );
                            }
                            //GameObject tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //tObj.transform.position = lVect;
                            //tObj.transform.localScale = new Vector3(0.2f,20f,0.2f);
                            //tObj.transform.name = "temp22";
                            //							
                            //tObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            //tObj.transform.position = rVect;
                            //tObj.transform.localScale = new Vector3(0.2f,20f,0.2f);
                            //tObj.transform.name = "temp22";

                            _RCS.tIntersectionBounds.Add(vRect);
                        }
                    }
                }

                isWasPrevMaxInter = isMaxIntersection;
                tVect_Prev = tVect;
                rVect_Prev = rVect;
                lVect_Prev = lVect;
                ShoulderR_PrevLVect = ShoulderR_lVect;
                ShoulderL_PrevRVect = ShoulderL_rVect;
                //ShoulderR_PrevRVect3 = ShoulderR_PrevRVect2;
                //ShoulderL_PrevLVect3 = ShoulderL_PrevLVect2;
                //ShoulderR_PrevRVect2 = ShoulderR_PrevRVect;
                //ShoulderL_PrevLVect2 = ShoulderL_PrevLVect;
                ShoulderR_PrevRVect = ShoulderR_rVect;
                ShoulderL_PrevLVect = ShoulderL_lVect;
                RampR_PrevR = RampR_R;
                RampR_PrevL = RampR_L;
                RampL_PrevR = RampL_R;
                RampL_PrevL = RampL_L;
                //i+=Step; 
            }
        }
        #endregion


        #region "Intersection Prelim Finalization"		
        private static void RoadJobPrelimFinalizeInter(ref GSDRoad _road)
        {
            int mCount = _road.spline.GetNodeCount();
            GSDSplineN tNode;
            for (int index = 0; index < mCount; index++)
            {
                tNode = _road.spline.mNodes[index];
                if (tNode.bIsIntersection)
                {
                    Inter_OrganizeVertices(ref tNode, ref _road);
                    tNode.iConstruction.Nullify();
                    tNode.iConstruction = null;
                }
            }
        }


        private static bool InterOrganizeVerticesMatchEdges(ref List<Vector3> _list1, ref List<Vector3> _list2, bool _isSkip1 = false, bool _isSkippingFirstListOne = false, bool _isSkippingBoth = false)
        {
            List<Vector3> PrimaryList;
            List<Vector3> SecondaryList;

            List<Vector3> tList1New;
            List<Vector3> tList2New;

            if (_isSkip1)
            {
                if (_isSkippingBoth)
                {
                    tList1New = new List<Vector3>();
                    tList2New = new List<Vector3>();
                    for (int index = 1; index < _list1.Count; index++)
                    {
                        tList1New.Add(_list1[index]);
                    }
                    for (int index = 1; index < _list2.Count; index++)
                    {
                        tList2New.Add(_list2[index]);
                    }
                }
                else
                {
                    if (_isSkippingFirstListOne)
                    {
                        tList1New = new List<Vector3>();
                        for (int index = 1; index < _list1.Count; index++)
                        {
                            tList1New.Add(_list1[index]);
                        }
                        tList2New = _list2;
                    }
                    else
                    {
                        tList2New = new List<Vector3>();
                        for (int index = 1; index < _list2.Count; index++)
                        {
                            tList2New.Add(_list2[index]);
                        }
                        tList1New = _list1;
                    }
                }
            }
            else
            {
                tList1New = _list1;
                tList2New = _list2;
            }

            int tList1Count = tList1New.Count;
            int tList2Count = tList2New.Count;
            if (tList1Count == tList2Count)
            {
                return false;
            }

            if (tList1Count > tList2Count)
            {
                PrimaryList = tList1New;
                SecondaryList = tList2New;
            }
            else
            {
                PrimaryList = tList2New;
                SecondaryList = tList1New;
            }

            if (SecondaryList == null || SecondaryList.Count == 0)
            {
                return true;
            }
            SecondaryList.Clear();
            SecondaryList = null;
            SecondaryList = new List<Vector3>();
            for (int index = 0; index < PrimaryList.Count; index++)
            {
                SecondaryList.Add(PrimaryList[index]);
            }


            if (tList1Count > tList2Count)
            {
                _list2 = SecondaryList;
            }
            else
            {
                _list1 = SecondaryList;
            }

            return false;
        }


        private static void InterOrganizeVerticesMatchShoulder(ref List<Vector3> _shoulderList, ref List<Vector3> _toMatch, int _startI, ref Vector3 _startVec, ref Vector3 _endVect, float _height, bool _isF = false)
        {
            List<Vector3> BackupList = new List<Vector3>();
            for (int index = 0; index < _toMatch.Count; index++)
            {
                BackupList.Add(_toMatch[index]);
            }
            Vector2 t2D = default(Vector2);
            Vector2 t2D_Start = ConvertVect3ToVect2(_startVec);
            Vector2 t2D_End = ConvertVect3ToVect2(_endVect);
            int RealStartID = -1;
            _startI = _startI - 30;
            if (_startI < 0)
            {
                _startI = 0;
            }
            for (int index = _startI; index < _shoulderList.Count; index++)
            {
                t2D = ConvertVect3ToVect2(_shoulderList[index]);
                //				if(t2D.x > 745f && t2D.x < 755f && t2D.y > 1240f && t2D.y < 1250f){
                //					int agfsdajgsd = 1;	
                //				}
                if (t2D == t2D_Start)
                {
                    //if(tShoulderList[i] == StartVec){
                    RealStartID = index;
                    break;
                }
            }

            _toMatch.Clear();
            _toMatch = null;
            _toMatch = new List<Vector3>();

            int spamcounter = 0;
            bool bBackup = false;
            if (RealStartID == -1)
            {
                bBackup = true;
            }

            if (!bBackup)
            {
                if (_isF)
                {
                    for (int index = RealStartID; index > 0; index -= 8)
                    {
                        t2D = ConvertVect3ToVect2(_shoulderList[index]);
                        _toMatch.Add(_shoulderList[index]);
                        if (t2D == t2D_End)
                        {
                            //if(tShoulderList[i] == EndVect){
                            break;
                        }
                        spamcounter += 1;
                        if (spamcounter > 100)
                        {
                            bBackup = true;
                            break;
                        }
                    }
                }
                else
                {
                    for (int index = RealStartID; index < _shoulderList.Count; index += 8)
                    {
                        t2D = ConvertVect3ToVect2(_shoulderList[index]);
                        _toMatch.Add(_shoulderList[index]);
                        if (t2D == t2D_End)
                        {
                            //if(tShoulderList[i] == EndVect){
                            break;
                        }
                        spamcounter += 1;
                        if (spamcounter > 100)
                        {
                            bBackup = true;
                            break;
                        }
                    }
                }
            }
            ////			
            //			if(!bBackup){
            //				for(int i=0;i<tToMatch.Count;i++){
            //					tToMatch[i] = new Vector3(tToMatch[i].x,tHeight,tToMatch[i].z);
            //				}
            //			}
            //			
            //			//Backup if above fails:
            //			if(bBackup){
            //				tToMatch.Clear();
            //				tToMatch = new List<Vector3>();
            //				for(int i=0;i<BackupList.Count;i++){
            //					tToMatch.Add(BackupList[i]);
            //				}
            //			}
        }


        private static void Inter_OrganizeVertices(ref GSDSplineN _node, ref GSDRoad _road)
        {
            GSD.Roads.GSDIntersections.iConstructionMaker iCon = _node.iConstruction;
            GSDRoadIntersection GSDRI = _node.GSDRI;

            //Skipping (3 ways):
            bool isSkipF = false;
            if (iCon.iFLane0L.Count == 0)
            {
                isSkipF = true;
            }
            bool bSkipB = false;
            if (iCon.iBLane0L.Count == 0)
            {
                bSkipB = true;
            }

            //Is primary node and is first node on a spline, meaning t junction: It does not have a B:
            if (_node.idOnSpline == 0 && string.CompareOrdinal(GSDRI.node1uID, _node.UID) == 0)
            {
                bSkipB = true;
            }
            //Is primary node and is last node on a spline, meaning t junction: It does not have a F:
            if (_node.idOnSpline == (_node.GSDSpline.GetNodeCount() - 1) && string.CompareOrdinal(GSDRI.node1uID, _node.UID) == 0)
            {
                isSkipF = true;
            }

            //Other node is t junction end node, meaning now we figure out which side we're on
            if (_node.Intersection_OtherNode.idOnSpline == 0 || _node.idOnSpline == (_node.GSDSpline.GetNodeCount() - 1))
            {

            }

            //Reverse all fronts:
            if (!isSkipF)
            {
                iCon.iFLane0L.Reverse();
                iCon.iFLane0R.Reverse();

                iCon.iFLane1L.Reverse();
                iCon.iFLane2L.Reverse();
                iCon.iFLane3L.Reverse();
                iCon.iFLane1R.Reverse();
                iCon.iFLane2R.Reverse();
                iCon.iFLane3R.Reverse();

                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iCon.shoulderStartFR = iCon.iFLane0L[0];
                    iCon.shoulderStartFL = iCon.iFLane3R[0];
                }
                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    iCon.shoulderStartFR = iCon.iFLane0L[0];
                    iCon.shoulderStartFL = iCon.iFLane2R[0];
                }
                else
                {
                    iCon.shoulderStartFR = iCon.iFLane0L[0];
                    iCon.shoulderStartFL = iCon.iFLane1R[0];
                }
            }

            if (!bSkipB)
            {
                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iCon.shoulderEndBL = iCon.iBLane0L[iCon.iBLane0L.Count - 1];
                    iCon.shoulderEndBR = iCon.iBLane3R[iCon.iBLane3R.Count - 1];
                }
                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    iCon.shoulderEndBL = iCon.iBLane0L[iCon.iBLane0L.Count - 1];
                    iCon.shoulderEndBR = iCon.iBLane2R[iCon.iBLane2R.Count - 1];
                }
                else
                {
                    iCon.shoulderEndBL = iCon.iBLane0L[iCon.iBLane0L.Count - 1];
                    iCon.shoulderEndBR = iCon.iBLane1R[iCon.iBLane1R.Count - 1];
                }
            }

            if (!bSkipB)
            {
                InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iBLane0L, iCon.shoulderBLStartIndex, ref iCon.shoulderStartBL, ref iCon.shoulderEndBL, GSDRI.height);
                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane3R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, GSDRI.height);
                }
                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane2R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, GSDRI.height);
                }
                else
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane1R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, GSDRI.height);
                }
            }

            if (!isSkipF)
            {
                InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iFLane0L, iCon.shoulderFRStartIndex, ref iCon.shoulderStartFR, ref iCon.shoulderEndFR, GSDRI.height, true);
                if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane3R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, GSDRI.height, true);
                }
                else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane2R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, GSDRI.height, true);
                }
                else
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane1R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, GSDRI.height, true);
                }
            }

            bool bError = false;
            string tWarning = "Intersection " + GSDRI.intersectionName + " in road " + _road.roadName + " at too extreme angle to process this intersection type. Reduce angle or reduce lane count.";

            if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                if (!bSkipB)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iBLane0R, ref iCon.iBLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
                if (!isSkipF)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iFLane0R, ref iCon.iFLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
            }
            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
            {
                if (!bSkipB)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iBLane0R, ref iCon.iBLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
                if (!isSkipF)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iFLane0R, ref iCon.iFLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }

                if (!bSkipB)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iBLane1R, ref iCon.iBLane2L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
                if (!isSkipF)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iFLane1R, ref iCon.iFLane2L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
            }
            else if (GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                if (!bSkipB)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iBLane0R, ref iCon.iBLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
                if (!isSkipF)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iFLane0R, ref iCon.iFLane1L);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }

                if (!bSkipB)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iBLane1R, ref iCon.iBLane2L, true, true);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }
                if (!isSkipF)
                {
                    bError = InterOrganizeVerticesMatchEdges(ref iCon.iFLane1R, ref iCon.iFLane2L, true, true);
                    if (bError)
                    {
                        Debug.Log(tWarning);
                    }
                }

                //				if(!bSkipB){ bError = Inter_OrganizeVerticesMatchEdges(ref iCon.iBLane2R, ref iCon.iBLane3L,true,false); if(bError){ Debug.Log(tWarning); } }
                //				if(!bSkipF){ bError = Inter_OrganizeVerticesMatchEdges(ref iCon.iFLane2R, ref iCon.iFLane3L,true,false); if(bError){ Debug.Log(tWarning); } }
            }

            //Back main plate left:
            int mCount = -1;
            if (!bSkipB)
            {
                mCount = iCon.iBLane0L.Count;
                for (int m = 0; m < mCount; m++)
                {
                    iCon.iBMainPlateL.Add(iCon.iBLane0L[m]);
                }
            }
            //Front main plate left:
            if (!isSkipF)
            {
                mCount = iCon.iFLane0L.Count;
                for (int m = 0; m < mCount; m++)
                {
                    iCon.iFMainPlateL.Add(iCon.iFLane0L[m]);
                }
            }

            //Back main plate right:
            if (!bSkipB)
            {
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    mCount = iCon.iBLane1R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iBMainPlateR.Add(iCon.iBLane1R[m]);
                    }
                }
                else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    mCount = iCon.iBLane2R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iBMainPlateR.Add(iCon.iBLane2R[m]);
                    }
                }
                else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    mCount = iCon.iBLane3R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iBMainPlateR.Add(iCon.iBLane3R[m]);
                    }
                }
            }

            //Front main plate right:
            if (!isSkipF)
            {
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    mCount = iCon.iFLane1R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iFMainPlateR.Add(iCon.iFLane1R[m]);
                    }
                }
                else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                {
                    mCount = iCon.iFLane2R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iFMainPlateR.Add(iCon.iFLane2R[m]);
                    }
                }
                else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    mCount = iCon.iFLane3R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iFMainPlateR.Add(iCon.iFLane3R[m]);
                    }
                }
            }

            mCount = _road.RCS.RoadVectors.Count;
            //			float mDistance = 0.05f;
            Vector3 tVect = default(Vector3);

            bool biBLane0L = (iCon.iBLane0L.Count > 0);
            bool biBLane0R = (iCon.iBLane0R.Count > 0);
            bool biBMainPlateL = (iCon.iBMainPlateL.Count > 0);
            bool biBMainPlateR = (iCon.iBMainPlateR.Count > 0);
            bool biFLane0L = (iCon.iFLane0L.Count > 0);
            bool biFLane0R = (iCon.iFLane0R.Count > 0);
            bool biFMainPlateL = (iCon.iFMainPlateL.Count > 0);
            bool biFMainPlateR = (iCon.iFMainPlateR.Count > 0);
            bool biBLane2L = (iCon.iBLane2L.Count > 0);
            bool biBLane2R = (iCon.iBLane2R.Count > 0);
            bool biFLane2L = (iCon.iFLane2L.Count > 0);
            bool biFLane2R = (iCon.iFLane2R.Count > 0);
            bool biBLane3L = (iCon.iBLane3L.Count > 0);
            bool biBLane3R = (iCon.iBLane3R.Count > 0);
            bool biFLane3L = (iCon.iFLane3L.Count > 0);
            bool biFLane3R = (iCon.iFLane3R.Count > 0);

            mCount = _road.RCS.RoadVectors.Count;
            int cCount = _road.spline.GetNodeCount();
            int tStartI = 0;
            int tEndI = mCount;
            //Start and end the next loop after this one later for opt:
            if (cCount > 2)
            {
                if (!_road.spline.mNodes[0].bIsIntersection && !_road.spline.mNodes[1].bIsIntersection)
                {
                    for (int i = 2; i < cCount; i++)
                    {
                        if (_road.spline.mNodes[i].bIsIntersection)
                        {
                            if (i - 2 >= 1)
                            {
                                tStartI = (int) (_road.spline.mNodes[i - 2].tTime * mCount);
                            }
                            break;
                        }
                    }
                }
            }
            if (cCount > 3)
            {
                if (!_road.spline.mNodes[cCount - 1].bIsIntersection && !_road.spline.mNodes[cCount - 2].bIsIntersection)
                {
                    for (int i = (cCount - 3); i >= 0; i--)
                    {
                        if (_road.spline.mNodes[i].bIsIntersection)
                        {
                            if (i + 2 < cCount)
                            {
                                tEndI = (int) (_road.spline.mNodes[i + 2].tTime * mCount);
                            }
                            break;
                        }
                    }
                }
            }

            if (tStartI > 0)
            {
                if (tStartI % 2 != 0)
                {
                    tStartI += 1;
                }
            }
            if (tStartI > mCount)
            {
                tStartI = mCount - 4;
            }
            if (tStartI < 0)
            {
                tStartI = 0;
            }
            if (tEndI < mCount)
            {
                if (tEndI % 2 != 0)
                {
                    tEndI += 1;
                }
            }
            if (tEndI > mCount)
            {
                tEndI = mCount - 4;
            }
            if (tEndI < 0)
            {
                tEndI = 0;
            }

            for (int i = tStartI; i < tEndI; i += 2)
            {
                tVect = _road.RCS.RoadVectors[i];
                for (int j = 0; j < 1; j++)
                {
                    if (biBLane0L && Vector3.SqrMagnitude(tVect - iCon.iBLane0L[j]) < 0.01f && !bSkipB)
                    {
                        iCon.iBLane0L[j] = tVect;
                    }
                    if (biBMainPlateL && Vector3.SqrMagnitude(tVect - iCon.iBMainPlateL[j]) < 0.01f && !bSkipB)
                    {
                        iCon.iBMainPlateL[j] = tVect;
                    }
                    if (biBMainPlateR && Vector3.SqrMagnitude(tVect - iCon.iBMainPlateR[j]) < 0.01f && !bSkipB)
                    {
                        iCon.iBMainPlateR[j] = tVect;
                    }
                    if (biFLane0L && Vector3.SqrMagnitude(tVect - iCon.iFLane0L[j]) < 0.01f && !isSkipF)
                    {
                        iCon.iFLane0L[j] = tVect;
                    }
                    if (biFMainPlateL && Vector3.SqrMagnitude(tVect - iCon.iFMainPlateL[j]) < 0.01f && !isSkipF)
                    {
                        iCon.iFMainPlateL[j] = tVect;
                    }
                    if (biFMainPlateR && Vector3.SqrMagnitude(tVect - iCon.iFMainPlateR[j]) < 0.01f && !isSkipF)
                    {
                        iCon.iFMainPlateR[j] = tVect;
                    }
                    if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (biBLane3L && Vector3.SqrMagnitude(tVect - iCon.iBLane3L[j]) < 0.01f && !bSkipB)
                        {
                            iCon.iBLane3L[j] = tVect;
                        }
                        if (biBLane3R && Vector3.SqrMagnitude(tVect - iCon.iBLane3R[j]) < 0.01f && !bSkipB)
                        {
                            iCon.iBLane3R[j] = tVect;
                        }
                        if (biFLane3L && Vector3.SqrMagnitude(tVect - iCon.iFLane3L[j]) < 0.01f && !isSkipF)
                        {
                            iCon.iFLane3L[j] = tVect;
                        }
                        if (biFLane3R && Vector3.SqrMagnitude(tVect - iCon.iFLane3R[j]) < 0.01f && !isSkipF)
                        {
                            iCon.iFLane3R[j] = tVect;
                        }
                    }
                    else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        if (biBLane2L && Vector3.SqrMagnitude(tVect - iCon.iBLane2L[j]) < 0.01f && !bSkipB)
                        {
                            iCon.iBLane2L[j] = tVect;
                        }
                        if (biBLane2R && Vector3.SqrMagnitude(tVect - iCon.iBLane2R[j]) < 0.01f && !bSkipB)
                        {
                            iCon.iBLane2R[j] = tVect;
                        }
                        if (biFLane2L && Vector3.SqrMagnitude(tVect - iCon.iFLane2L[j]) < 0.01f && !isSkipF)
                        {
                            iCon.iFLane2L[j] = tVect;
                        }
                        if (biFLane2R && Vector3.SqrMagnitude(tVect - iCon.iFLane2R[j]) < 0.01f && !isSkipF)
                        {
                            iCon.iFLane2R[j] = tVect;
                        }
                    }
                }
            }

            //			float b0 = -1f;
            //			float f0 = -1f;
            //			
            //			if(!bSkipB){ b0 = iCon.iBMainPlateL[0].y; }
            //			if(!bSkipF){ f0 = iCon.iFMainPlateL[0].y; }
            //			
            //			if(iCon.iBLane0R == null || iCon.iBLane0R.Count == 0){
            //				bSkipB = true;	
            //			}
            if (iCon.iBMainPlateR == null || iCon.iBMainPlateR.Count == 0)
            {
                bSkipB = true;
            }
            if (iCon.iBMainPlateL == null || iCon.iBMainPlateL.Count == 0)
            {
                bSkipB = true;
            }

            if (!bSkipB)
            {
                iCon.iBLane0R[0] = ((iCon.iBMainPlateR[0] - iCon.iBMainPlateL[0]) * 0.5f + iCon.iBMainPlateL[0]);
            }
            if (!isSkipF)
            {
                iCon.iFLane0R[0] = ((iCon.iFMainPlateR[0] - iCon.iFMainPlateL[0]) * 0.5f + iCon.iFMainPlateL[0]);
            }

            //			if(tNode.GSDRI.rType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane){ 
            if (!bSkipB)
            {
                iCon.iBLane1L[0] = iCon.iBLane0R[0];
                iCon.iBLane1R[0] = new Vector3(iCon.iBLane1R[0].x, iCon.iBLane1L[0].y, iCon.iBLane1R[0].z);
            }

            if (!isSkipF)
            {
                iCon.iFLane1L[0] = iCon.iFLane0R[0];
                iCon.iFLane1R[0] = new Vector3(iCon.iFLane1R[0].x, iCon.iFLane1L[0].y, iCon.iFLane1R[0].z);
            }
            //			}

            if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                if (!bSkipB)
                {
                    iCon.iBLane3L[0] = new Vector3(iCon.iBLane3L[0].x, iCon.iBLane3R[0].y, iCon.iBLane3L[0].z);
                }
                if (!isSkipF)
                {
                    iCon.iFLane3L[0] = new Vector3(iCon.iFLane3L[0].x, iCon.iFLane3R[0].y, iCon.iFLane3L[0].z);
                }
            }
            else if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
            {
                if (!bSkipB)
                {
                    iCon.iBLane2L[0] = new Vector3(iCon.iBLane2L[0].x, iCon.iBLane2R[0].y, iCon.iBLane2L[0].z);
                }
                if (!isSkipF)
                {
                    iCon.iFLane2L[0] = new Vector3(iCon.iFLane2L[0].x, iCon.iFLane2R[0].y, iCon.iFLane2L[0].z);
                }
            }

            List<Vector3> iBLane0 = null;
            List<Vector3> iBLane1 = null;
            List<Vector3> iBLane2 = null;
            List<Vector3> iBLane3 = null;
            if (!bSkipB)
            {
                iBLane0 = InterVertices(iCon.iBLane0L, iCon.iBLane0R, _node.GSDRI.height);
                iBLane1 = InterVertices(iCon.iBLane1L, iCon.iBLane1R, _node.GSDRI.height);
                if (_node.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    iBLane2 = InterVertices(iCon.iBLane2L, iCon.iBLane2R, _node.GSDRI.height);
                }
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iBLane3 = InterVertices(iCon.iBLane3L, iCon.iBLane3R, _node.GSDRI.height);
                }
            }

            //Front lanes:
            List<Vector3> iFLane0 = null;
            List<Vector3> iFLane1 = null;
            List<Vector3> iFLane2 = null;
            List<Vector3> iFLane3 = null;
            if (!isSkipF)
            {
                iFLane0 = InterVertices(iCon.iFLane0L, iCon.iFLane0R, _node.GSDRI.height);
                iFLane1 = InterVertices(iCon.iFLane1L, iCon.iFLane1R, _node.GSDRI.height);
                if (_node.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    iFLane2 = InterVertices(iCon.iFLane2L, iCon.iFLane2R, _node.GSDRI.height);
                }
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iFLane3 = InterVertices(iCon.iFLane3L, iCon.iFLane3R, _node.GSDRI.height);
                }
            }

            //Main plates:
            List<Vector3> iBMainPlate = null;
            List<Vector3> iFMainPlate = null;
            if (!bSkipB)
            {
                iBMainPlate = InterVertices(iCon.iBMainPlateL, iCon.iBMainPlateR, _node.GSDRI.height);
            }
            if (!isSkipF)
            {
                iFMainPlate = InterVertices(iCon.iFMainPlateL, iCon.iFMainPlateR, _node.GSDRI.height);
            }
            //			//Marker plates:
            //			List<Vector3> iBMarkerPlate = InterVertices(iCon.iBMarkerPlateL,iCon.iBMarkerPlateR, tNode.GSDRI.Height);
            //			List<Vector3> iFMarkerPlate = InterVertices(iCon.iFMarkerPlateL,iCon.iFMarkerPlateR, tNode.GSDRI.Height);
            //			
            //Now add these to RCS:
            if (!bSkipB)
            {
                _road.RCS.iBLane0s.Add(iBLane0.ToArray());
                _road.RCS.iBLane0s_tID.Add(GSDRI);
                _road.RCS.iBLane0s_nID.Add(_node);
                _road.RCS.iBLane1s.Add(iBLane1.ToArray());
                _road.RCS.iBLane1s_tID.Add(GSDRI);
                _road.RCS.iBLane1s_nID.Add(_node);
                if (_node.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    if (iBLane2 != null)
                    {
                        _road.RCS.iBLane2s.Add(iBLane2.ToArray());
                        _road.RCS.iBLane2s_tID.Add(GSDRI);
                        _road.RCS.iBLane2s_nID.Add(_node);
                    }
                }
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _road.RCS.iBLane3s.Add(iBLane3.ToArray());
                    _road.RCS.iBLane3s_tID.Add(GSDRI);
                    _road.RCS.iBLane3s_nID.Add(_node);
                }
            }
            //Front lanes:
            if (!isSkipF)
            {
                _road.RCS.iFLane0s.Add(iFLane0.ToArray());
                _road.RCS.iFLane0s_tID.Add(GSDRI);
                _road.RCS.iFLane0s_nID.Add(_node);
                _road.RCS.iFLane1s.Add(iFLane1.ToArray());
                _road.RCS.iFLane1s_tID.Add(GSDRI);
                _road.RCS.iFLane1s_nID.Add(_node);
                if (_node.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    _road.RCS.iFLane2s.Add(iFLane2.ToArray());
                    _road.RCS.iFLane2s_tID.Add(GSDRI);
                    _road.RCS.iFLane2s_nID.Add(_node);
                }
                if (_node.GSDRI.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _road.RCS.iFLane3s.Add(iFLane3.ToArray());
                    _road.RCS.iFLane3s_tID.Add(GSDRI);
                    _road.RCS.iFLane3s_nID.Add(_node);
                }
            }
            //Main plates:
            if (iBMainPlate != null && !bSkipB)
            {
                _road.RCS.iBMainPlates.Add(iBMainPlate.ToArray());
                _road.RCS.iBMainPlates_tID.Add(GSDRI);
                _road.RCS.iBMainPlates_nID.Add(_node);
            }
            if (iFMainPlate != null && !isSkipF)
            {
                _road.RCS.iFMainPlates.Add(iFMainPlate.ToArray());
                _road.RCS.iFMainPlates_tID.Add(GSDRI);
                _road.RCS.iFMainPlates_nID.Add(_node);
            }
            //			//Marker plates:
            //			tRoad.RCS.iBMarkerPlates.Add(iBMarkerPlate.ToArray());
            //			tRoad.RCS.iFMarkerPlates.Add(iFMarkerPlate.ToArray());
            //			tRoad.RCS.IntersectionTypes.Add((int)tNode.GSDRI.rType);

            if (_node.GSDRI.roadType != GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                if (!bSkipB)
                {
                    _road.RCS.iBLane1s_IsMiddleLane.Add(true);
                }
                if (!isSkipF)
                {
                    _road.RCS.iFLane1s_IsMiddleLane.Add(true);
                }
            }
            else
            {
                if (!bSkipB)
                {
                    _road.RCS.iBLane1s_IsMiddleLane.Add(false);
                }
                if (!isSkipF)
                {
                    _road.RCS.iFLane1s_IsMiddleLane.Add(false);
                }
            }
        }


        private static bool IsVecSame(ref Vector3 _vect1, Vector3 _vect2)
        {
            return ((Vector3.SqrMagnitude(_vect1 - _vect2) < 0.01f));
        }


        private static List<Vector3> InterVertices(List<Vector3> _left, List<Vector3> _right, float _height)
        {
            if (_left.Count == 0 || _right.Count == 0)
            {
                return null;
            }

            List<Vector3> tList = new List<Vector3>();
            int tCountL = _left.Count;
            int tCountR = _right.Count;
            int spamcheck = 0;

            while (tCountL < tCountR && spamcheck < 5000)
            {
                _left.Add(_left[tCountL - 1]);
                tCountL = _left.Count;
                spamcheck += 1;
            }

            spamcheck = 0;
            while (tCountR < tCountL && spamcheck < 5000)
            {
                _right.Add(_right[tCountR - 1]);
                tCountR = _right.Count;
                spamcheck += 1;
            }

            if (spamcheck > 4000)
            {
                Debug.LogWarning("spamcheck InterVertices");
            }

            int tCount = Mathf.Max(tCountL, tCountR);
            for (int i = 0; i < tCount; i++)
            {
                tList.Add(_left[i]);
                tList.Add(_left[i]);
                tList.Add(_right[i]);
                tList.Add(_right[i]);
            }
            return tList;
        }
        #endregion


        /// <summary>
        /// Handles most triangles and normals construction. In certain scenarios for efficiency reasons UV might also be processed.
        /// </summary>
        /// <param name='_RCS'> The road construction buffer, by reference. </param>/
        public static void RoadJob1(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //Triangles and normals:
            //GSDRootUtil.StartProfiling(RCS.tRoad, "ProcessRoad_IntersectionCleanup");
            if (_RCS.isInterseOn)
            {
                ProcessRoadIntersectionCleanup(ref _RCS);
            }
            //GSDRootUtil.EndProfiling(RCS.tRoad);

            ProcessRoadTrisBulk(ref _RCS);

            _RCS.tris_ShoulderR = ProcessRoadTrisShoulder(_RCS.ShoulderR_Vectors.Count);
            _RCS.tris_ShoulderL = ProcessRoadTrisShoulder(_RCS.ShoulderL_Vectors.Count);
            if (_RCS.road.isShoulderCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                ProcessRoadTrisShoulderCutsR(ref _RCS);
                ProcessRoadTrisShoulderCutsL(ref _RCS);
            }

            ProcessRoadNormalsBulk(ref _RCS);
            ProcessRoadNormalsShoulders(ref _RCS);
        }


        /// <summary>
        /// Handles most UV and tangent construction. Some scenarios might involve triangles and normals or lack UV construction for efficiency reasons.
        /// </summary>
        /// <param name='_RCS'> The road construction buffer, by reference. </param>
        public static void RoadJob2(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //Bridge UV is processed with tris and normals.

            //For one big road mesh:
            if (_RCS.isRoadOn)
            {
                if (!_RCS.tMeshSkip)
                {
                    _RCS.uv = ProcessRoadUVs(_RCS.RoadVectors.ToArray());
                }
                if (!_RCS.tMesh_SRSkip)
                {
                    _RCS.uv_SR = ProcessRoadUVsShoulder(_RCS.ShoulderR_Vectors.ToArray());
                }
                if (!_RCS.tMesh_SLSkip)
                {
                    _RCS.uv_SL = ProcessRoadUVsShoulder(_RCS.ShoulderL_Vectors.ToArray());
                }

                //UVs for pavement:
                if (!_RCS.tMeshSkip)
                {
                    int vCount = _RCS.RoadVectors.Count;
                    _RCS.uv2 = new Vector2[vCount];
                    for (int index = 0; index < vCount; index++)
                    {
                        _RCS.uv2[index] = new Vector2(_RCS.RoadVectors[index].x * 0.2f, _RCS.RoadVectors[index].z * 0.2f);
                    }
                }
            }

            //For road cuts:
            if (_RCS.road.isRoadCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                ProcessRoadUVsRoadCuts(ref _RCS);
                int cCount = _RCS.cut_RoadVectors.Count;
                for (int index = 0; index < cCount; index++)
                {
                    _RCS.cut_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris[index], _RCS.cut_normals[index], _RCS.cut_uv[index], _RCS.cut_RoadVectors[index].ToArray()));
                    _RCS.cut_tangents_world.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris[index], _RCS.cut_normals[index], _RCS.cut_uv_world[index], _RCS.cut_RoadVectors[index].ToArray()));
                }
            }
            if (_RCS.road.isShoulderCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                int rCount = _RCS.cut_ShoulderR_Vectors.Count;
                for (int index = 0; index < rCount; index++)
                {
                    ProcessRoadUVsShoulderCut(ref _RCS, false, index);
                    _RCS.cut_tangents_SR.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris_ShoulderR[index], _RCS.cut_normals_ShoulderR[index], _RCS.cut_uv_SR[index], _RCS.cut_ShoulderR_Vectors[index].ToArray()));
                    _RCS.cut_tangents_SR_world.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris_ShoulderR[index], _RCS.cut_normals_ShoulderR[index], _RCS.cut_uv_SR_world[index], _RCS.cut_ShoulderR_Vectors[index].ToArray()));
                }
                int lCount = _RCS.cut_ShoulderL_Vectors.Count;
                for (int index = 0; index < lCount; index++)
                {
                    ProcessRoadUVsShoulderCut(ref _RCS, true, index);
                    _RCS.cut_tangents_SL.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris_ShoulderL[index], _RCS.cut_normals_ShoulderL[index], _RCS.cut_uv_SL[index], _RCS.cut_ShoulderL_Vectors[index].ToArray()));
                    _RCS.cut_tangents_SL_world.Add(GSDRootUtil.ProcessTangents(_RCS.cut_tris_ShoulderL[index], _RCS.cut_normals_ShoulderL[index], _RCS.cut_uv_SL_world[index], _RCS.cut_ShoulderL_Vectors[index].ToArray()));
                }
            }
            if (_RCS.isInterseOn)
            {
                ProcessRoadUVsIntersections(ref _RCS);
            }

            //						throw new System.Exception("FFFFFFFF");

            if (_RCS.isRoadOn)
            {
                if (!_RCS.tMeshSkip)
                {
                    _RCS.tangents = GSDRootUtil.ProcessTangents(_RCS.tris, _RCS.normals, _RCS.uv, _RCS.RoadVectors.ToArray());
                }
                if (!_RCS.tMeshSkip)
                {
                    _RCS.tangents2 = GSDRootUtil.ProcessTangents(_RCS.tris, _RCS.normals, _RCS.uv2, _RCS.RoadVectors.ToArray());
                }
                if (!_RCS.tMesh_SRSkip)
                {
                    _RCS.tangents_SR = GSDRootUtil.ProcessTangents(_RCS.tris_ShoulderR, _RCS.normals_ShoulderR, _RCS.uv_SR, _RCS.ShoulderR_Vectors.ToArray());
                }
                if (!_RCS.tMesh_SLSkip)
                {
                    _RCS.tangents_SL = GSDRootUtil.ProcessTangents(_RCS.tris_ShoulderL, _RCS.normals_ShoulderL, _RCS.uv_SL, _RCS.ShoulderL_Vectors.ToArray());
                }
                for (int index = 0; index < _RCS.tMesh_RoadConnections.Count; index++)
                {
                    _RCS.RoadConnections_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.RoadConnections_tris[index], _RCS.RoadConnections_normals[index], _RCS.RoadConnections_uv[index], _RCS.RoadConnections_verts[index]));
                }
            }

            if (_RCS.isInterseOn)
            {
                //Back lanes:
                int vCount = _RCS.iBLane0s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane0s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iBLane0s_tris[index], _RCS.iBLane0s_normals[index], _RCS.iBLane0s_uv[index], _RCS.iBLane0s[index]));
                }
                vCount = _RCS.iBLane1s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane1s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iBLane1s_tris[index], _RCS.iBLane1s_normals[index], _RCS.iBLane1s_uv[index], _RCS.iBLane1s[index]));
                }
                vCount = _RCS.iBLane2s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane2s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iBLane2s_tris[index], _RCS.iBLane2s_normals[index], _RCS.iBLane2s_uv[index], _RCS.iBLane2s[index]));
                }
                vCount = _RCS.iBLane3s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane3s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iBLane3s_tris[index], _RCS.iBLane3s_normals[index], _RCS.iBLane3s_uv[index], _RCS.iBLane3s[index]));
                }
                //Front lanes:
                vCount = _RCS.iFLane0s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane0s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iFLane0s_tris[index], _RCS.iFLane0s_normals[index], _RCS.iFLane0s_uv[index], _RCS.iFLane0s[index]));
                }
                vCount = _RCS.iFLane1s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane1s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iFLane1s_tris[index], _RCS.iFLane1s_normals[index], _RCS.iFLane1s_uv[index], _RCS.iFLane1s[index]));
                }
                vCount = _RCS.iFLane2s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane2s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iFLane2s_tris[index], _RCS.iFLane2s_normals[index], _RCS.iFLane2s_uv[index], _RCS.iFLane2s[index]));
                }
                vCount = _RCS.iFLane3s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane3s_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iFLane3s_tris[index], _RCS.iFLane3s_normals[index], _RCS.iFLane3s_uv[index], _RCS.iFLane3s[index]));
                }
                //Main plates:
                vCount = _RCS.iBMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBMainPlates_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iBMainPlates_tris[index], _RCS.iBMainPlates_normals[index], _RCS.iBMainPlates_uv[index], _RCS.iBMainPlates[index]));
                }
                vCount = _RCS.iBMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBMainPlates_tangents2.Add(GSDRootUtil.ProcessTangents(_RCS.iBMainPlates_tris[index], _RCS.iBMainPlates_normals[index], _RCS.iBMainPlates_uv2[index], _RCS.iBMainPlates[index]));
                }
                vCount = _RCS.iFMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFMainPlates_tangents.Add(GSDRootUtil.ProcessTangents(_RCS.iFMainPlates_tris[index], _RCS.iFMainPlates_normals[index], _RCS.iFMainPlates_uv[index], _RCS.iFMainPlates[index]));
                }
                vCount = _RCS.iFMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFMainPlates_tangents2.Add(GSDRootUtil.ProcessTangents(_RCS.iFMainPlates_tris[index], _RCS.iFMainPlates_normals[index], _RCS.iFMainPlates_uv2[index], _RCS.iFMainPlates[index]));
                }
            }
        }


        #region "Intersection Cleanup"
        private static void ProcessRoadIntersectionCleanup(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            List<GSD.Roads.GSDRoadUtil.Construction2DRect> tList = _RCS.tIntersectionBounds;
            int mCount = tList.Count;
            _RCS.ShoulderR_Vectors = ProcessRoadIntersectionCleanupHelper(ref _RCS.ShoulderR_Vectors, ref tList, mCount, ref _RCS.ImmuneVects);
            _RCS.ShoulderL_Vectors = ProcessRoadIntersectionCleanupHelper(ref _RCS.ShoulderL_Vectors, ref tList, mCount, ref _RCS.ImmuneVects);
        }


        private static List<Vector3> ProcessRoadIntersectionCleanupHelper(ref List<Vector3> _vects, ref List<GSD.Roads.GSDRoadUtil.Construction2DRect> _list, int _count, ref HashSet<Vector3> _immuneVects)
        {
            GSD.Roads.GSDRoadUtil.Construction2DRect tRect = null;
            int MVL = _vects.Count;
            //Vector3 tVect = default(Vector3);
            Vector2 Vect2D = default(Vector2);
            Vector2 tNearVect = default(Vector2);
            float tMax2 = 2000f;
            float tMax2SQ = 0f;
            //			GameObject tObj = GameObject.Find("Inter1");
            //			Vector2 tObj2D = ConvertVect3_To_Vect2(tObj.transform.position);
            //			int fCount = 0;
            //			bool bTempNow = false;
            for (int i = 0; i < _count; i++)
            {
                tRect = _list[i];
                tMax2 = tRect.MaxDistance * 1.5f;
                tMax2SQ = (tMax2 * tMax2);

                //				Debug.Log (tRect.ToString());

                for (int j = 0; j < MVL; j++)
                {
                    Vect2D.x = _vects[j].x;
                    Vect2D.y = _vects[j].z;

                    if (Vector2.SqrMagnitude(Vect2D - tRect.P1) > tMax2SQ)
                    {
                        j += 32;
                        continue;
                    }

                    //					if(Vector2.Distance(Vect2D,tObj2D) < 20f && (j % 16 == 0)){
                    //						fCount+=1;
                    //						GameObject xObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //							xObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                    //							xObj.transform.position = tVects[j];
                    //							xObj.name = "temp22";
                    //					}

                    //					bTempNow = false;
                    if (tRect.Contains(ref Vect2D))
                    {
                        if (_immuneVects.Contains(_vects[j]))
                        {
                            continue;
                        }
                        //						if(Vect2D == tRect.P1){
                        //							continue;
                        //						}else if(Vect2D == tRect.P2){
                        //							continue;
                        //						}else if(Vect2D == tRect.P3){
                        //							continue;
                        //						}else if(Vect2D == tRect.P4){
                        //							continue;
                        //						}


                        //						if(Mathf.Approximately(tVects[j].x,303.1898f)){
                        //							GameObject hObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //							hObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                        //							hObj.transform.position = tVects[j];
                        //							hObj.name = "temp23";
                        //							bTempNow = true;
                        //							Debug.Log (tVects[j]);
                        //						}

                        //Calling near when it shouldn't ?
                        if (tRect.Near(ref Vect2D, out tNearVect))
                        {   //If near the rect, set it equal
                            _vects[j] = new Vector3(tNearVect.x, _vects[j].y, tNearVect.y);
                        }
                        else
                        {
                            _vects[j] = new Vector3(_vects[j].x, tRect.Height, _vects[j].z);
                        }





                        //ImmuneVects.Add(tVects[j]);

                        //						if(bTempNow){
                        //							GameObject xObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //							xObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                        //							xObj.transform.position = tVects[j];
                        //							xObj.name = "temp22";
                        //							Debug.Log ("to: " + tVects[j]);
                        //						}
                    }
                }
            }
            //			Debug.Log ("Fcount: " + fCount);

            return _vects;
        }
        #endregion


        #region "Tris"
        private static void ProcessRoadTrisBulk(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
         //, ref Mesh tShoulderR, ref Mesh tShoulderL){
         //Next come the triangles. Since we want two triangles, each defined by three integers, the triangles array will have six elements in total. 
         //Remembering the clockwise rule for ordering the corners, the lower left triangle will use 0, 2, 1 as its corner indices, while the upper right one will use 2, 3, 1. 

            _RCS.tris = ProcessRoadTrisBulkHelper(_RCS.RoadVectors.Count);
            if (_RCS.road.isRoadCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                ProcessRoadTrisRoadCuts(ref _RCS);
            }

            if (_RCS.isInterseOn)
            {
                //For intersection parts:
                //Back lanes:
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iBLane0s_tris, ref _RCS.iBLane0s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iBLane1s_tris, ref _RCS.iBLane1s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iBLane2s_tris, ref _RCS.iBLane2s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iBLane3s_tris, ref _RCS.iBLane3s);
                //Front lanes:
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iFLane0s_tris, ref _RCS.iFLane0s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iFLane1s_tris, ref _RCS.iFLane1s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iFLane2s_tris, ref _RCS.iFLane2s);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iFLane3s_tris, ref _RCS.iFLane3s);
                //Main plates:
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iBMainPlates_tris, ref _RCS.iBMainPlates);
                ProcessRoadTrisIntersectionProcessor(ref _RCS.iFMainPlates_tris, ref _RCS.iFMainPlates);
            }
        }


        private static int[] ProcessRoadTrisBulkHelper(int _MVL)
        {
            int TriCount = 0;
            int x1, x2, x3;
            int xCount = (int) (_MVL * 0.25f * 6) - 6;
            //			if(xCount < 0){ xCount = 0; }
            int[] tri = new int[xCount];
            for (int i = 0; i < _MVL; i += 4)
            {
                if (i + 4 == _MVL)
                {
                    break;
                }

                x1 = i;
                x2 = i + 4;
                x3 = i + 2;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;

                x1 = i + 4;
                x2 = i + 6;
                x3 = i + 2;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;
            }
            return tri;
        }


        private static void ProcessRoadTrisRoadCuts(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //Road cuts aren't working right for the special nodes on cuts
            int cCount = _RCS.RoadCuts.Count;
            int PrevRoadCutIndex = 0;
            int CurrentRoadCutIndex = 0;
            List<List<Vector3>> tVects = new List<List<Vector3>>();
            List<Vector3> tVectListSingle = null;
            Vector3 xVect = default(Vector3);
            for (int j = 0; j < cCount; j++)
            {
                CurrentRoadCutIndex = _RCS.RoadCuts[j];
                tVectListSingle = new List<Vector3>();
                _RCS.cut_RoadVectorsHome.Add(_RCS.RoadVectors[PrevRoadCutIndex]);
                xVect = _RCS.RoadVectors[PrevRoadCutIndex];
                for (int i = PrevRoadCutIndex; i < CurrentRoadCutIndex; i++)
                {
                    tVectListSingle.Add(_RCS.RoadVectors[i] - xVect);
                }
                tVects.Add(tVectListSingle);
                PrevRoadCutIndex = CurrentRoadCutIndex - 4;
                if (PrevRoadCutIndex < 0)
                {
                    PrevRoadCutIndex = 0;
                }
            }
            int mMax = _RCS.RoadVectors.Count;
            tVectListSingle = new List<Vector3>();
            _RCS.cut_RoadVectorsHome.Add(_RCS.RoadVectors[PrevRoadCutIndex]);
            xVect = _RCS.RoadVectors[PrevRoadCutIndex];
            for (int i = PrevRoadCutIndex; i < mMax; i++)
            {
                tVectListSingle.Add(_RCS.RoadVectors[i] - xVect);
            }
            tVects.Add(tVectListSingle);

            int vCount = tVects.Count;
            List<int[]> tTris = new List<int[]>();
            for (int i = 0; i < vCount; i++)
            {
                int[] tTriSingle = ProcessRoadTrisBulkHelper(tVects[i].Count);
                tTris.Add(tTriSingle);
            }

            _RCS.cut_RoadVectors = tVects;
            _RCS.cut_tris = tTris;
        }


        private static void ProcessRoadTrisShoulderCutsR(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.ShoulderCutsR.Count;
            int PrevRoadCutIndex = 0;
            int CurrentRoadCutIndex = 0;
            List<List<Vector3>> tVects = new List<List<Vector3>>();
            List<Vector3> tVectListSingle = null;
            Vector3 xVect = default(Vector3);
            for (int j = 0; j < cCount; j++)
            {
                CurrentRoadCutIndex = _RCS.ShoulderCutsR[j];
                tVectListSingle = new List<Vector3>();
                _RCS.cut_ShoulderR_VectorsHome.Add(_RCS.ShoulderR_Vectors[PrevRoadCutIndex]);
                xVect = _RCS.ShoulderR_Vectors[PrevRoadCutIndex];
                for (int i = PrevRoadCutIndex; i < CurrentRoadCutIndex; i++)
                {
                    tVectListSingle.Add(_RCS.ShoulderR_Vectors[i] - xVect);
                }
                tVects.Add(tVectListSingle);
                PrevRoadCutIndex = CurrentRoadCutIndex - 8;
                if (PrevRoadCutIndex < 0)
                {
                    PrevRoadCutIndex = 0;
                }
            }
            int mMax = _RCS.ShoulderR_Vectors.Count;
            tVectListSingle = new List<Vector3>();
            _RCS.cut_ShoulderR_VectorsHome.Add(_RCS.ShoulderR_Vectors[PrevRoadCutIndex]);
            xVect = _RCS.ShoulderR_Vectors[PrevRoadCutIndex];
            for (int i = PrevRoadCutIndex; i < mMax; i++)
            {
                tVectListSingle.Add(_RCS.ShoulderR_Vectors[i] - xVect);
            }
            tVects.Add(tVectListSingle);

            int vCount = tVects.Count;
            List<int[]> tTris = new List<int[]>();
            for (int i = 0; i < vCount; i++)
            {
                int[] tTriSingle = ProcessRoadTrisShoulder(tVects[i].Count);
                tTris.Add(tTriSingle);
            }

            _RCS.cut_ShoulderR_Vectors = tVects;
            _RCS.cut_tris_ShoulderR = tTris;
        }


        private static void ProcessRoadTrisShoulderCutsL(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.ShoulderCutsL.Count;
            int PrevRoadCutIndex = 0;
            int CurrentRoadCutIndex = 0;
            List<List<Vector3>> tVects = new List<List<Vector3>>();
            List<Vector3> tVectListSingle = null;
            Vector3 xVect = default(Vector3);
            for (int j = 0; j < cCount; j++)
            {
                CurrentRoadCutIndex = _RCS.ShoulderCutsR[j];
                tVectListSingle = new List<Vector3>();
                _RCS.cut_ShoulderL_VectorsHome.Add(_RCS.ShoulderL_Vectors[PrevRoadCutIndex]);
                xVect = _RCS.ShoulderL_Vectors[PrevRoadCutIndex];
                for (int i = PrevRoadCutIndex; i < CurrentRoadCutIndex; i++)
                {
                    tVectListSingle.Add(_RCS.ShoulderL_Vectors[i] - xVect);
                }
                tVects.Add(tVectListSingle);
                PrevRoadCutIndex = CurrentRoadCutIndex - 8;
                if (PrevRoadCutIndex < 0)
                {
                    PrevRoadCutIndex = 0;
                }
            }
            int mMax = _RCS.ShoulderL_Vectors.Count;
            tVectListSingle = new List<Vector3>();
            _RCS.cut_ShoulderL_VectorsHome.Add(_RCS.ShoulderL_Vectors[PrevRoadCutIndex]);
            xVect = _RCS.ShoulderL_Vectors[PrevRoadCutIndex];
            for (int i = PrevRoadCutIndex; i < mMax; i++)
            {
                tVectListSingle.Add(_RCS.ShoulderL_Vectors[i] - xVect);
            }
            tVects.Add(tVectListSingle);

            int vCount = tVects.Count;
            List<int[]> tTris = new List<int[]>();
            for (int i = 0; i < vCount; i++)
            {
                int[] tTriSingle = ProcessRoadTrisShoulder(tVects[i].Count);
                tTris.Add(tTriSingle);
            }

            _RCS.cut_ShoulderL_Vectors = tVects;
            _RCS.cut_tris_ShoulderL = tTris;
        }


        private static int[] ProcessRoadTrisShoulder(int _MVL)
        {
            int TriCount = 0;
            int x1, x2, x3;
            int xCount = (int) ((_MVL / 2) * 0.25f * 6) - 6;
            if (xCount < 0)
            {
                xCount = 0;
            }
            xCount = xCount * 2;

            int[] tri = new int[xCount];
            for (int i = 0; i < _MVL; i += 8)
            {
                if (i + 8 == _MVL)
                {
                    break;
                }

                x1 = i;
                x2 = i + 8;
                x3 = i + 2;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;

                x1 = i + 8;
                x2 = i + 10;
                x3 = i + 2;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;

                x1 = i + 4;
                x2 = i + 12;
                x3 = i + 6;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;

                x1 = i + 12;
                x2 = i + 14;
                x3 = i + 6;

                tri[TriCount] = x1;
                TriCount += 1;
                tri[TriCount] = x2;
                TriCount += 1;
                tri[TriCount] = x3;
                TriCount += 1;
            }
            return tri;
        }


        //For intersection parts:
        private static void ProcessRoadTrisIntersectionProcessor(ref List<int[]> _triList, ref List<Vector3[]> _vertexList)
        {
            if (_triList == null)
            {
                _triList = new List<int[]>();
            }
            int vListCount = _vertexList.Count;
            int[] tris;
            for (int i = 0; i < vListCount; i++)
            {
                tris = ProcessRoadTrisBulkHelper(_vertexList[i].Length);
                _triList.Add(tris);
            }
        }
        #endregion


        #region "Normals"
        private static void ProcessRoadNormalsBulk(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //A mesh with just the vertices and triangles set up will be visible in the editor but will not look very convincing since it is not correctly shaded without the normals. 
            //The normals for the flat plane are very simple - they are all identical and point in the negative Z direction in the plane's local space. 
            //With the normals added, the plane will be correctly shaded but remember that you need a light in the scene to see the effect. 
            //Bridge normals are processed at same time as tris.
            int MVL = _RCS.RoadVectors.Count;
            Vector3[] normals = new Vector3[MVL];
            //			Vector3 tVect = -Vector3.forward;
            //			for(int i=0;i<MVL;i++){
            //				normals[i] = tVect;
            //			}
            _RCS.normals = normals;

            //Road cuts normals:
            if (_RCS.road.isRoadCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                ProcessRoadNormalsRoadCuts(ref _RCS);
            }
            if (_RCS.road.isShoulderCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                ProcessRoadNormalsShoulderCutsR(ref _RCS);
                ProcessRoadNormalsShoulderCutsL(ref _RCS);
            }

            //Intersection normals:
            if (_RCS.isInterseOn)
            {
                //For intersection parts:
                //Back lanes:
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBLane0s_normals, ref _RCS.iBLane0s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBLane1s_normals, ref _RCS.iBLane1s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBLane2s_normals, ref _RCS.iBLane2s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBLane3s_normals, ref _RCS.iBLane3s);
                //Front lanes:
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFLane0s_normals, ref _RCS.iFLane0s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFLane1s_normals, ref _RCS.iFLane1s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFLane2s_normals, ref _RCS.iFLane2s);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFLane3s_normals, ref _RCS.iFLane3s);
                //Main plates:
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBMainPlates_normals, ref _RCS.iBMainPlates);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFMainPlates_normals, ref _RCS.iFMainPlates);
                //Marker plates:
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iBMarkerPlates_normals, ref _RCS.iBMarkerPlates);
                ProcessRoadNormalsIntersectionsProcessor(ref _RCS.iFMarkerPlates_normals, ref _RCS.iFMarkerPlates);
            }
        }


        private static void ProcessRoadNormalsRoadCuts(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_RoadVectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_RoadVectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //				Vector3 tVect = -Vector3.forward;
                //				for(int i=0;i<MVL;i++){
                //					normals[i] = tVect;
                //				}
                _RCS.cut_normals.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulderCutsR(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_ShoulderR_Vectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_ShoulderR_Vectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //				Vector3 tVect = -Vector3.forward;
                //				for(int i=0;i<MVL;i++){
                //					normals[i] = tVect;
                //				}
                _RCS.cut_normals_ShoulderR.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulderCutsL(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_ShoulderL_Vectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_ShoulderL_Vectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //				Vector3 tVect = -Vector3.forward;
                //				for(int i=0;i<MVL;i++){
                //					normals[i] = tVect;
                //				}
                _RCS.cut_normals_ShoulderL.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulders(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //A mesh with just the vertices and triangles set up will be visible in the editor but will not look very convincing since it is not correctly shaded without the normals. 
            //The normals for the flat plane are very simple - they are all identical and point in the negative Z direction in the plane's local space. 
            //With the normals added, the plane will be correctly shaded but remember that you need a light in the scene to see the effect. 
            int MVL = _RCS.ShoulderL_Vectors.Count;
            Vector3[] normals = new Vector3[MVL];
            //			Vector3 tVect = -Vector3.forward;
            //			for(int i=0;i<MVL;i++){
            //				normals[i] = tVect;
            //			}
            _RCS.normals_ShoulderL = normals;
            //Right:
            MVL = _RCS.ShoulderR_Vectors.Count;
            normals = new Vector3[MVL];
            //			tVect = -Vector3.forward;
            //			for(int i=0;i<MVL;i++){
            //				normals[i] = tVect;
            //			}
            _RCS.normals_ShoulderR = normals;
        }


        //For intersection parts:
        private static void ProcessRoadNormalsIntersectionsProcessor(ref List<Vector3[]> _normalList, ref List<Vector3[]> _vertexList)
        {
            if (_normalList == null)
            {
                _normalList = new List<Vector3[]>();
            }
            int vListCount = _vertexList.Count;
            Vector3[] normals;
            int MVL = -1;
            //			Vector3 tVect = -Vector3.forward;
            for (int index = 0; index < vListCount; index++)
            {
                MVL = _vertexList[index].Length;
                normals = new Vector3[MVL];
                //				for(int j=0;j<MVL;j++){
                //					normals[j] = tVect;
                //				}
                _normalList.Add(normals);
            }
        }
        #endregion


        #region "UVs"
        private static Vector2[] ProcessRoadUVs(Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;

            while (i + 6 < MVL)
            {
                tDistance = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistance = tDistance / 5f;
                uv[i] = new Vector2(0f, tDistanceSum);
                uv[i + 2] = new Vector2(1f, tDistanceSum);
                uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }
            return uv;
        }


        private static void ProcessRoadUVsRoadCuts(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;

            int cCount = _RCS.cut_RoadVectors.Count;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;
            for (int j = 0; j < cCount; j++)
            {
                Vector3[] tVerts = _RCS.cut_RoadVectors[j].ToArray();
                int MVL = tVerts.Length;
                Vector2[] uv = new Vector2[MVL];
                Vector2[] uv_world = new Vector2[MVL];
                int i = 0;
                bool bOddToggle = true;
                while (i + 6 < MVL)
                {
                    tDistance = Vector3.Distance(tVerts[i], tVerts[i + 4]);
                    tDistance = tDistance / 5f;
                    uv[i] = new Vector2(0f, tDistanceSum);
                    uv[i + 2] = new Vector2(1f, tDistanceSum);
                    uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                    uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);

                    //Last segment needs adjusted due to double vertices:
                    if ((i + 7) == MVL)
                    {
                        if (bOddToggle)
                        {
                            //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                            uv[MVL - 3] = uv[i + 4];
                            uv[MVL - 1] = uv[i + 6];
                        }
                        else
                        {
                            //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                            uv[MVL - 4] = uv[i + 4];
                            uv[MVL - 2] = uv[i + 6];
                        }
                    }

                    if (bOddToggle)
                    {
                        i += 5;
                    }
                    else
                    {
                        i += 3;
                    }

                    tDistanceLeftSum += tDistanceLeft;
                    tDistanceRightSum += tDistanceRight;
                    tDistanceSum += tDistance;
                    bOddToggle = !bOddToggle;
                }
                for (i = 0; i < MVL; i++)
                {
                    uv_world[i] = new Vector2(tVerts[i].x * 0.2f, tVerts[i].z * 0.2f);
                }
                _RCS.cut_uv_world.Add(uv_world);
                _RCS.cut_uv.Add(uv);
            }
        }


        private static Vector2[] ProcessRoadUVsShoulder(Vector3[] _verts)
        {
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            //			bool bOddToggle = true;
            //			float tDistance= 0f;
            //			float tDistanceLeft = 0f;
            //			float tDistanceRight = 0f;
            //			float tDistanceLeftSum = 0f;
            //			float tDistanceRightSum = 0f;
            //			float tDistanceSum = 0f;
            //			float xDistance = 0f;
            //			float rDistance1 = 0f;
            //			float rDistance2 = 0f;
            //			float fDistance = Vector3.Distance(_verts[0],_verts[2]);


            for (i = 0; i < MVL; i++)
            {
                uv[i] = new Vector2(_verts[i].x * 0.2f, _verts[i].z * 0.2f);
            }
            return uv;

            //			while(i+8 < MVL){
            //				tDistanceLeft = Vector3.Distance(_verts[i],_verts[i+8]);
            //				tDistanceRight = Vector3.Distance(_verts[i+2],_verts[i+10]);
            //				
            //				tDistance = tDistance / 5f;
            //				tDistanceLeft = tDistanceLeft / 5f;
            //				tDistanceRight = tDistanceRight / 5f;
            //				
            //				uv[i] = new Vector2(0f, tDistanceSum);
            //				uv[i+2] = new Vector2(1f, tDistanceSum);
            //				uv[i+8] = new Vector2(0f, tDistance+tDistanceSum);
            //				uv[i+10] = new Vector2(1f, tDistance+tDistanceSum);	
            //				
            //				rDistance1 = (Vector3.Distance(_verts[i+4],_verts[i+6]));
            //				rDistance2 = (Vector3.Distance(_verts[i+12],_verts[i+14]));
            //					
            //				if(!bIsLeft){
            //					uv[i+4] = new Vector2(1f, tDistanceSum);
            //					xDistance = (rDistance1 / fDistance) + 1f;
            //					uv[i+6] = new Vector2(xDistance, tDistanceSum);
            //					uv[i+12] = new Vector2(1f, tDistance+tDistanceSum);
            //					xDistance = (rDistance2 / fDistance) + 1f;
            //					uv[i+14] = new Vector2(xDistance, tDistance+tDistanceSum);
            //				}else{
            //					xDistance = (rDistance1 / fDistance);
            //					uv[i+4] = new Vector2(-xDistance, tDistanceSum);
            //					uv[i+6] = new Vector2(0f, tDistanceSum);
            //					xDistance = (rDistance2 / fDistance);
            //					uv[i+12] = new Vector2(-xDistance, tDistance+tDistanceSum);
            //					uv[i+14] = new Vector2(0f, tDistance+tDistanceSum);
            //				}
            //				
            //				uv[i] = new Vector2(0f, tDistanceLeftSum);
            //				uv[i+2] = new Vector2(1f, tDistanceRightSum);
            //				uv[i+8] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            //				uv[i+10] = new Vector2(1f, tDistanceRight+tDistanceRightSum);	
            //
            //				uv[i] = new Vector2(_verts[i].x/5f,_verts[i].z/5f);
            //				uv[i+2] = new Vector2(_verts[i+2].x/5f,_verts[i+2].z/5f);
            //				uv[i+8] = new Vector2(_verts[i+8].x/5f,_verts[i+8].z/5f);
            //				uv[i+10] = new Vector2(_verts[i+10].x/5f,_verts[i+10].z/5f);
            //				
            //				
            //				rDistance1 = (Vector3.Distance(_verts[i+4],_verts[i+6]));
            //				rDistance2 = (Vector3.Distance(_verts[i+12],_verts[i+14]));
            //					
            //				if(!bIsLeft){
            //					uv[i+4] = new Vector2(1f, tDistanceRightSum);
            //					xDistance = (rDistance1 / fDistance) + 1f;
            //					uv[i+6] = new Vector2(xDistance, tDistanceRightSum);
            //					uv[i+12] = new Vector2(1f, tDistanceRight+tDistanceRightSum);
            //					xDistance = (rDistance2 / fDistance) + 1f;
            //					uv[i+14] = new Vector2(xDistance, tDistanceRight+tDistanceRightSum);
            //				}else{
            //					xDistance = (rDistance1 / fDistance);
            //					uv[i+4] = new Vector2(-xDistance, tDistanceLeftSum);
            //					uv[i+6] = new Vector2(0f, tDistanceLeftSum);
            //					xDistance = (rDistance2 / fDistance);
            //					uv[i+12] = new Vector2(-xDistance, tDistanceLeft+tDistanceLeftSum);
            //					uv[i+14] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            //				}
            //				
            //				uv[i+4] = new Vector2(_verts[i+4].x/5f,_verts[i+4].z/5f);
            //				uv[i+6] = new Vector2(_verts[i+6].x/5f,(_verts[i+6].z/5f));
            //				uv[i+12] = new Vector2(_verts[i+12].x/5f,_verts[i+12].z/5f);
            //				uv[i+14] = new Vector2(_verts[i+14].x/5f,(_verts[i+14].z/5f));
            //				
            //				//Last segment needs adjusted due to double vertices:
            //				if((i+11) == MVL){
            //					if(bOddToggle){
            //						//First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
            //						uv[MVL-3] = uv[i+4];
            //						uv[MVL-1] = uv[i+6];
            //					}else{
            //						//Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
            //						uv[MVL-4] = uv[i+4];
            //						uv[MVL-2] = uv[i+6];
            //					}
            //				}
            //				
            //				if(bOddToggle){
            //					i+=9;	
            //				}else{
            //					i+=7;
            //				}
            //				
            //				tDistanceLeftSum+=tDistanceLeft;
            //				tDistanceRightSum+=tDistanceRight;
            //				tDistanceSum+=tDistance;
            //				bOddToggle = !bOddToggle;
            //			}
            //			return uv;
        }


        private static void ProcessRoadUVsShoulderCut(ref GSD.Roads.RoadConstructorBufferMaker _RCS, bool _isLeft, int _j)
        {
            int i = 0;
            Vector3[] tVerts;
            if (_isLeft)
            {
                tVerts = _RCS.cut_ShoulderL_Vectors[_j].ToArray();
            }
            else
            {
                tVerts = _RCS.cut_ShoulderR_Vectors[_j].ToArray();
            }
            int MVL = tVerts.Length;

            //World:
            Vector2[] uv_world = new Vector2[MVL];
            for (i = 0; i < MVL; i++)
            {
                uv_world[i] = new Vector2(tVerts[i].x * 0.2f, tVerts[i].z * 0.2f);
            }
            if (_isLeft)
            {
                _RCS.cut_uv_SL_world.Add(uv_world);
            }
            else
            {
                _RCS.cut_uv_SR_world.Add(uv_world);
            }

            //Marks:
            float tDistance = 0f;
            float tDistanceSum = 0f;
            Vector2[] uv = new Vector2[MVL];
            float rDistance1 = 0f;
            float rDistance2 = 0f;
            bool bOddToggle = true;
            float fDistance = Vector3.Distance(tVerts[0], tVerts[2]);
            float xDistance = 0f;
            i = 0;
            float TheOne = _RCS.road.shoulderWidth / _RCS.road.roadDefinition;
            while (i + 8 < MVL)
            {
                tDistance = Vector3.Distance(tVerts[i], tVerts[i + 8]) * 0.2f;

                uv[i] = new Vector2(0f, tDistanceSum);
                uv[i + 2] = new Vector2(TheOne, tDistanceSum);
                uv[i + 8] = new Vector2(0f, tDistance + tDistanceSum);
                uv[i + 10] = new Vector2(TheOne, tDistance + tDistanceSum);

                rDistance1 = (Vector3.Distance(tVerts[i + 4], tVerts[i + 6]));
                rDistance2 = (Vector3.Distance(tVerts[i + 12], tVerts[i + 14]));

                if (!_isLeft)
                {
                    //Right
                    //8	   10   12   14
                    //0		2	 4	  6
                    //0f   1f	1f	  X

                    xDistance = TheOne + (rDistance1 / fDistance);
                    uv[i + 4] = uv[i + 2];
                    uv[i + 6] = new Vector2(xDistance, tDistanceSum);

                    xDistance = TheOne + (rDistance2 / fDistance);
                    uv[i + 12] = uv[i + 10];
                    uv[i + 14] = new Vector2(xDistance, tDistance + tDistanceSum);
                }
                else
                {
                    //Left:
                    //12,13	   14,15    8,9    10,11
                    //4,5		6,7		0,1		2,3	
                    //0f-X	     0f	 	 0f		1f

                    xDistance = 0f - (rDistance1 / fDistance);
                    uv[i + 4] = new Vector2(xDistance, tDistanceSum);
                    uv[i + 6] = uv[i];
                    xDistance = 0f - (rDistance2 / fDistance);
                    uv[i + 12] = new Vector2(xDistance, tDistance + tDistanceSum);
                    uv[i + 14] = uv[i + 8];
                }

                //Last segment needs adjusted due to double vertices:
                if ((i + 11) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 9;
                }
                else
                {
                    i += 7;
                }

                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }

            if (_isLeft)
            {
                _RCS.cut_uv_SL.Add(uv);
            }
            else
            {
                _RCS.cut_uv_SR.Add(uv);
            }
        }


        #region "Intersection UV"
        private static void ProcessRoadUVsIntersections(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            int tCount = -1;

            //Lanes:
            tCount = _RCS.iBLane0s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBLane0s_uv.Add(ProcessRoadUVsIntersectionLane0(ref _RCS, _RCS.iBLane0s[i]));
            }
            tCount = _RCS.iBLane1s.Count;
            for (int i = 0; i < tCount; i++)
            {
                if (_RCS.iBLane1s_IsMiddleLane[i])
                {
                    _RCS.iBLane1s_uv.Add(ProcessRoadUVsIntersectionMiddleLane(ref _RCS, _RCS.iBLane1s[i]));
                }
                else
                {
                    _RCS.iBLane1s_uv.Add(ProcessRoadUVsIntersectionFullLane(ref _RCS, _RCS.iBLane1s[i]));
                }
            }
            tCount = _RCS.iBLane2s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBLane2s_uv.Add(ProcessRoadUVsIntersectionFullLane(ref _RCS, _RCS.iBLane2s[i]));
            }
            tCount = _RCS.iBLane3s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBLane3s_uv.Add(ProcessRoadUVsIntersectionLane4(ref _RCS, _RCS.iBLane3s[i]));
            }

            //Lanes:
            tCount = _RCS.iFLane0s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFLane0s_uv.Add(ProcessRoadUVsIntersectionLane0(ref _RCS, _RCS.iFLane0s[i]));
            }
            tCount = _RCS.iFLane1s.Count;
            for (int i = 0; i < tCount; i++)
            {
                if (_RCS.iFLane1s_IsMiddleLane[i])
                {
                    _RCS.iFLane1s_uv.Add(ProcessRoadUVsIntersectionMiddleLane(ref _RCS, _RCS.iFLane1s[i]));
                }
                else
                {
                    _RCS.iFLane1s_uv.Add(ProcessRoadUVsIntersectionFullLane(ref _RCS, _RCS.iFLane1s[i]));
                }
            }
            tCount = _RCS.iFLane2s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFLane2s_uv.Add(ProcessRoadUVsIntersectionFullLane(ref _RCS, _RCS.iFLane2s[i]));
            }
            tCount = _RCS.iFLane3s.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFLane3s_uv.Add(ProcessRoadUVsIntersectionLane4(ref _RCS, _RCS.iFLane3s[i]));
            }

            //Main plates:
            tCount = _RCS.iBMainPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBMainPlates_uv.Add(ProcessRoadUVsIntersectionMainPlate(ref _RCS, _RCS.iBMainPlates[i]));
            }
            tCount = _RCS.iFMainPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFMainPlates_uv.Add(ProcessRoadUVsIntersectionMainPlate(ref _RCS, _RCS.iFMainPlates[i]));
            }
            tCount = _RCS.iBMainPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBMainPlates_uv2.Add(ProcessRoadUVsIntersectionMainPlate2(ref _RCS, _RCS.iBMainPlates[i], _RCS.iBMainPlates_tID[i]));
            }
            tCount = _RCS.iFMainPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFMainPlates_uv2.Add(ProcessRoadUVsIntersectionMainPlate2(ref _RCS, _RCS.iFMainPlates[i], _RCS.iFMainPlates_tID[i]));
            }

            //Marker plates:
            tCount = _RCS.iBMarkerPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iBMarkerPlates_uv.Add(ProcessRoad_UVs_Intersection_MarkerPlate(ref _RCS, _RCS.iBMarkerPlates[i]));
            }
            tCount = _RCS.iFMarkerPlates.Count;
            for (int i = 0; i < tCount; i++)
            {
                _RCS.iFMarkerPlates_uv.Add(ProcessRoad_UVs_Intersection_MarkerPlate(ref _RCS, _RCS.iFMarkerPlates[i]));
            }
        }


        private static Vector2[] ProcessRoadUVsIntersectionFullLane(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;

            while (i + 6 < MVL)
            {
                tDistance = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistance = tDistance / 5f;
                uv[i] = new Vector2(0f, tDistanceSum);
                uv[i + 2] = new Vector2(1f, tDistanceSum);
                uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }

            return uv;
        }


        private static Vector2[] ProcessRoadUVsIntersectionLane4(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;

            while (i + 6 < MVL)
            {
                tDistance = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistance = tDistance / 5f;


                if (i == 0)
                {
                    uv[i] = new Vector2(0.94f, tDistanceSum);
                    uv[i + 2] = new Vector2(1f, tDistanceSum);
                    uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                    uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);
                }
                else
                {
                    uv[i] = new Vector2(0f, tDistanceSum);
                    uv[i + 2] = new Vector2(1f, tDistanceSum);
                    uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                    uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);
                }

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }

            return uv;
        }


        private static Vector2[] ProcessRoadUVsIntersectionMiddleLane(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;

            while (i + 6 < MVL)
            {
                tDistance = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistance = tDistance / 5f;


                if (i == 0)
                {
                    uv[i] = new Vector2(0f, tDistanceSum);
                    uv[i + 2] = new Vector2(0.05f, tDistanceSum);
                    uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                    uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);
                }
                else
                {
                    uv[i] = new Vector2(0f, tDistanceSum);
                    uv[i + 2] = new Vector2(1f, tDistanceSum);
                    uv[i + 4] = new Vector2(0f, tDistance + tDistanceSum);
                    uv[i + 6] = new Vector2(1f, tDistance + tDistanceSum);
                }

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }
            return uv;
        }


        private static Vector2[] ProcessRoadUVsIntersectionLane0(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistance = 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            float tDistanceSum = 0f;

            while (i + 6 < MVL)
            {
                tDistanceLeft = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistanceRight = Vector3.Distance(_verts[i + 2], _verts[i + 6]);

                tDistanceLeft = tDistanceLeft / 5f;
                tDistanceRight = tDistanceRight / 5f;

                //Below is for uniform
                //				if(i==0){
                //					uv[i] = new Vector2(0.5f, tDistanceLeftSum);
                //					uv[i+2] = new Vector2(1.5f, tDistanceRightSum);
                //					uv[i+4] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
                //					uv[i+6] = new Vector2(1.5f, tDistanceRight+tDistanceRightSum);
                //				}else{
                //					uv[i] = new Vector2(0f, tDistanceLeftSum);
                //					uv[i+2] = new Vector2(1f, tDistanceRightSum);
                //					uv[i+4] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
                //					uv[i+6] = new Vector2(1f, tDistanceRight+tDistanceRightSum);
                //				}

                //Stretched:
                uv[i] = new Vector2(0f, tDistanceLeftSum);
                uv[i + 2] = new Vector2(1f, tDistanceRightSum);
                uv[i + 4] = new Vector2(0f, tDistanceLeft + tDistanceLeftSum);
                uv[i + 6] = new Vector2(1f, tDistanceRight + tDistanceRightSum);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                tDistanceSum += tDistance;
                bOddToggle = !bOddToggle;
            }
            return uv;
        }


        private static Vector2[] ProcessRoad_UVs_Intersection_MarkerPlate(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0.1f;
            float tDistanceRightSum = 0.1f;

            float mDistanceL = Vector3.Distance(_verts[i], _verts[_verts.Length - 3]);
            float mDistanceR = Vector3.Distance(_verts[i + 2], _verts[_verts.Length - 1]);

            while (i + 6 < MVL)
            {
                tDistanceLeft = Vector3.Distance(_verts[i], _verts[i + 4]);
                tDistanceRight = Vector3.Distance(_verts[i + 2], _verts[i + 6]);

                tDistanceLeft = tDistanceLeft / mDistanceL;
                tDistanceRight = tDistanceRight / mDistanceR;

                uv[i] = new Vector2(0f, tDistanceLeftSum);
                uv[i + 2] = new Vector2(1f, tDistanceRightSum);
                uv[i + 4] = new Vector2(0f, tDistanceLeft + tDistanceLeftSum);
                uv[i + 6] = new Vector2(1f, tDistanceRight + tDistanceRightSum);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }

                if (bOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                bOddToggle = !bOddToggle;
            }
            return uv;
        }


        private static Vector2[] ProcessRoadUVsIntersectionMainPlate(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            //			bool bOddToggle = true;
            //			float tDistance= 0f;
            //			float tDistanceLeft = 0f;
            //			float tDistanceRight = 0f;
            //			float tDistanceLeftSum = 0f;
            //			float tDistanceRightSum = 0f;
            //			float tDistanceSum = 0f;
            //			float DistRepresent = 5f;

            //			float mDistanceL = Vector3.Distance(_verts[i],_verts[_verts.Length-3]);
            //			float mDistanceR = Vector3.Distance(_verts[i+2],_verts[_verts.Length-1]);

            for (i = 0; i < MVL; i++)
            {
                uv[i] = new Vector2(_verts[i].x * 0.2f, _verts[i].z * 0.2f);
            }
            return uv;

            //			while(i+6 < MVL){
            //				tDistanceLeft = Vector3.Distance(_verts[i],_verts[i+4]);
            //				tDistanceRight = Vector3.Distance(_verts[i+2],_verts[i+6]);
            //				
            //				tDistanceLeft = tDistanceLeft / 5f;
            //				tDistanceRight = tDistanceRight / 5f;
            //				
            ////				if(i==0){
            ////					uv[i] = new Vector2(0.25f, tDistanceLeftSum);
            ////					uv[i+2] = new Vector2(1.25f, tDistanceRightSum);
            ////					uv[i+4] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            ////					uv[i+6] = new Vector2(2f, tDistanceRight+tDistanceRightSum);
            ////				}else{
            ////					uv[i] = new Vector2(0f, tDistanceLeftSum);
            ////					uv[i+2] = new Vector2(2f, tDistanceRightSum);
            ////					uv[i+4] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            ////					uv[i+6] = new Vector2(2f, tDistanceRight+tDistanceRightSum);
            ////				}
            //				
            //				uv[i] = new Vector2(_verts[i].x/5f, _verts[i].z/5f);
            //				uv[i+2] = new Vector2(_verts[i+2].x/5f, _verts[i+2].z/5f);
            //				uv[i+4] = new Vector2(_verts[i+4].x/5f, _verts[i+4].z/5f);
            //				uv[i+6] = new Vector2(_verts[i+6].x/5f, _verts[i+6].z/5f);
            //
            //				//Last segment needs adjusted due to double vertices:
            //				if((i+7) == MVL){
            //					if(bOddToggle){
            //						//First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
            //						uv[MVL-3] = uv[i+4];
            //						uv[MVL-1] = uv[i+6];
            //					}else{
            //						//Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
            //						uv[MVL-4] = uv[i+4];
            //						uv[MVL-2] = uv[i+6];
            //					}
            //				}
            //				
            //				if(bOddToggle){
            //					i+=5;	
            //				}else{
            //					i+=3;
            //				}
            //				
            //				tDistanceLeftSum+=tDistanceLeft;
            //				tDistanceRightSum+=tDistanceRight;
            //				//tDistanceSum+=tDistance;
            //				bOddToggle = !bOddToggle;
            //			}
            //			return uv;
        }


        private static Vector2[] ProcessRoadUVsIntersectionMainPlate2(ref GSD.Roads.RoadConstructorBufferMaker _RCS, Vector3[] _verts, GSDRoadIntersection _intersection)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            bool bOddToggle = true;
            //			float tDistance= 0f;
            float tDistanceLeft = 0f;
            float tDistanceRight = 0f;
            float tDistanceLeftSum = 0f;
            float tDistanceRightSum = 0f;
            //			float tDistanceSum = 0f;
            //			float DistRepresent = 5f;

            float mDistanceL = Vector3.Distance(_verts[i + 4], _verts[_verts.Length - 3]);
            float mDistanceR = Vector3.Distance(_verts[i + 6], _verts[_verts.Length - 1]);
            mDistanceL = mDistanceL * 1.125f;
            mDistanceR = mDistanceR * 1.125f;

            //			int bHitMaxL = 0;
            //			int bHitMaxR = 0;

            float tAdd1;
            float tAdd2;
            float tAdd3;
            float tAdd4;

            float RoadWidth = _RCS.road.RoadWidth();
            float LaneWidth = _RCS.road.laneWidth;
            float iWidth = -1;
            if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                iWidth = RoadWidth + (LaneWidth * 2f);
            }
            else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
            {
                iWidth = RoadWidth + (LaneWidth * 1f);
            }
            else
            {
                iWidth = RoadWidth;
            }


            while (i + 6 < MVL)
            {
                if (i == 0)
                {

                    if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {

                        //(Lane width / 2)/roadwidth
                        //1-((lanewidth / 2)/roadwidth)



                        uv[i] = new Vector2((LaneWidth * 0.5f) / iWidth, 0f);
                        uv[i + 2] = new Vector2(1f - (((LaneWidth * 0.5f) + LaneWidth) / iWidth), 0f);
                        //Debug.Log (GSDRI.tName + " " + uv[i+2].x);
                        uv[i + 4] = new Vector2(0f, 0.125f);
                        uv[i + 6] = new Vector2(1f, 0.125f);
                    }
                    else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        uv[i] = new Vector2((LaneWidth * 0.5f) / iWidth, 0f);
                        uv[i + 2] = new Vector2(1f - ((LaneWidth * 0.5f) / iWidth), 0f);
                        uv[i + 4] = new Vector2(0f, 0.125f);
                        uv[i + 6] = new Vector2(1f, 0.125f);
                    }
                    else if (_intersection.roadType == GSDRoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        uv[i] = new Vector2(0f, 0f);
                        uv[i + 2] = new Vector2(1f, 0f);
                        uv[i + 4] = new Vector2(0f, 0.125f);
                        uv[i + 6] = new Vector2(1f, 0.125f);
                    }
                    tDistanceLeft = 0.125f;
                    tDistanceRight = 0.125f;
                }
                else
                {
                    tDistanceLeft = Vector3.Distance(_verts[i], _verts[i + 4]);
                    tDistanceRight = Vector3.Distance(_verts[i + 2], _verts[i + 6]);
                    tDistanceLeft = tDistanceLeft / mDistanceL;
                    tDistanceRight = tDistanceRight / mDistanceR;


                    //					if(bHitMaxL > 0 || (tDistanceLeftSum+tDistanceLeft) > 1f){ 
                    //						tDistanceLeftSum = 0.998f + (0.0001f*bHitMaxL); 
                    //						tDistanceLeft = 0.001f; 
                    //						bHitMaxL+=1;
                    //					}
                    //					if(bHitMaxR > 0 || (tDistanceRightSum+tDistanceRight) > 1f){ 
                    //						tDistanceRightSum = 0.998f + (0.0001f*bHitMaxR); 
                    //						tDistanceRight = 0.001f;
                    //						bHitMaxR+=1;
                    //					}

                    tAdd1 = tDistanceLeftSum;
                    if (tAdd1 > 1f)
                    {
                        tAdd1 = 1f;
                    }
                    tAdd2 = tDistanceRightSum;
                    if (tAdd2 > 1f)
                    {
                        tAdd2 = 1f;
                    }
                    tAdd3 = tDistanceLeft + tDistanceLeftSum;
                    if (tAdd3 > 1f)
                    {
                        tAdd3 = 1f;
                    }
                    tAdd4 = tDistanceRight + tDistanceRightSum;
                    if (tAdd4 > 1f)
                    {
                        tAdd4 = 1f;
                    }

                    uv[i] = new Vector2(0f, tAdd1);
                    uv[i + 2] = new Vector2(1f, tAdd2);
                    uv[i + 4] = new Vector2(0f, tAdd3);
                    uv[i + 6] = new Vector2(1f, tAdd4);
                    //Debug.Log (tAdd3 + " R:"+ tAdd4 + " RLoc: " + _verts[i+6]);
                }



                //Debug.Log ("1.0 R:1.0 RLoc: " + _verts[i+6]);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (bOddToggle)
                    {
                        //First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
                        uv[MVL - 3] = uv[i + 4];
                        uv[MVL - 1] = uv[i + 6];
                    }
                    else
                    {
                        //Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
                        uv[MVL - 4] = uv[i + 4];
                        uv[MVL - 2] = uv[i + 6];
                    }
                }



                if (bOddToggle)
                {
                    i += 5;

                    if (i + 6 >= MVL)
                    {
                        uv[i + 4 - 5] = new Vector2(0f, 1f);
                        uv[i + 6 - 5] = new Vector2(1f, 1f);
                    }

                }
                else
                {
                    i += 3;

                    if (i + 6 >= MVL)
                    {
                        uv[i + 4 - 3] = new Vector2(0f, 1f);
                        uv[i + 6 - 3] = new Vector2(1f, 1f);
                    }
                }



                tDistanceLeftSum += tDistanceLeft;
                tDistanceRightSum += tDistanceRight;
                //tDistanceSum+=tDistance;
                bOddToggle = !bOddToggle;
            }

            //			uv[MVL-1].y = 1f;
            //			uv[MVL-2].y = 1f;
            //			uv[MVL-3].y = 1f;
            //			uv[MVL-4].y = 1f;

            return uv;
        }
        #endregion
        #endregion


        #region "Set vector heights"
        private static void SetVectorHeight2(ref Vector3 _worldVector, ref float _p, ref List<KeyValuePair<float, float>> _list, ref GSDSplineC _spline)
        {
            int mCount = _list.Count;
            int index = 0;

            if (mCount < 1)
            {
                _worldVector.y = 0f;
                return;
            }

            float cValue = 0f;
            for (index = 0; index < (mCount - 1); index++)
            {
                if (_p >= _list[index].Key && _p < _list[index + 1].Key)
                {
                    cValue = _list[index].Value;
                    if (index > 3)
                    {
                        if (_list[index - 1].Value < cValue)
                        {
                            cValue = _list[index - 1].Value;
                        }
                        if (_list[index - 2].Value < cValue)
                        {
                            cValue = _list[index - 2].Value;
                        }
                        if (_list[index - 3].Value < cValue)
                        {
                            cValue = _list[index - 3].Value;
                        }
                    }
                    if (index < (mCount - 3))
                    {
                        if (_list[index + 1].Value < cValue)
                        {
                            cValue = _list[index + 1].Value;
                        }
                        if (_list[index + 2].Value < cValue)
                        {
                            cValue = _list[index + 2].Value;
                        }
                        if (_list[index + 3].Value < cValue)
                        {
                            cValue = _list[index + 3].Value;
                        }
                    }
                    break;
                }
            }

            //if(p > 0.95f && GSDRootUtil.IsApproximately(cValue,0f,0.001f)){
            //    float DeadValue = 0f;
            //    Vector3 tPos = tSpline.GetSplineValue(p,false);
            //    if(!tSpline.IsNearIntersection(ref tPos,ref DeadValue)){
            //        cValue = tList[tList.Count-1].Value;
            //    }
            //}

            //Zero protection: 
            if (GSDRootUtil.IsApproximately(cValue, 0f, 0.001f) && _worldVector.y > 0f)
            {
                cValue = _worldVector.y - 0.35f;
            }

            _worldVector.y = cValue;
        }
        #endregion


        public class RoadTerrainInfo
        {
            [UnityEngine.Serialization.FormerlySerializedAs("tBounds")]
            public Rect bounds;
            [UnityEngine.Serialization.FormerlySerializedAs("GSDID")]
            public int uID;
            public int hmWidth;
            public int hmHeight;
            [UnityEngine.Serialization.FormerlySerializedAs("tPos")]
            public Vector3 pos;
            [UnityEngine.Serialization.FormerlySerializedAs("tSize")]
            public Vector3 size;
            public float[,] heights;
        }
    }


    #region "Threading core"
    public class GSDThreadedJob
    {
        [UnityEngine.Serialization.FormerlySerializedAs("m_IsDone")]
        private bool isDone = false;
        [UnityEngine.Serialization.FormerlySerializedAs("m_Handle")]
        private object handle = new object();
        [UnityEngine.Serialization.FormerlySerializedAs("m_Thread")]
        private System.Threading.Thread thread = null;


        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (handle)
                {
                    tmp = isDone;
                }
                return tmp;
            }
            set
            {
                lock (handle)
                {
                    isDone = value;
                }
            }
        }


        public virtual void Start()
        {
            thread = new System.Threading.Thread(Run);
            thread.Start();
        }


        public virtual void Abort()
        {
            thread.Abort();
        }


        protected virtual void ThreadFunction() { }


        protected virtual void OnFinished() { }


        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }
            return false;
        }


        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }


    public class GSDJob : GSDThreadedJob
    {
        //public Vector3[] InData; // arbitary job data
        //public Vector3[] OutData; // arbitary job data


        protected override void ThreadFunction()
        {
            // Do your threaded task. DON'T use the Unity API here
            //for (int i = 0; i < 100000000; i++){
            //InData[i % InData.Length] += InData[(i+1) % InData.Length];
            //}
        }


        protected override void OnFinished()
        {
            // This is executed by the Unity main thread when the job is finished
            //for (int i = 0; i < InData.Length; i++){
            //Debug.Log("Results(" + i + "): " + InData[i]);
            //}
        }
    }
    #endregion


    public class TerrainCalcs : GSDThreadedJob
    {
        [UnityEngine.Serialization.FormerlySerializedAs("GSDm_Handle")]
        private object handle = new object();
        private List<GSD.Roads.GSDTerraforming.TempTerrainData> TTDList;
        [UnityEngine.Serialization.FormerlySerializedAs("tSpline")]
        private GSDSplineC spline;
        [UnityEngine.Serialization.FormerlySerializedAs("tRoad")]
        private GSDRoad road;


        public void Setup(ref List<GSD.Roads.GSDTerraforming.TempTerrainData> _TTDList, GSDSplineC _tSpline, GSDRoad _tRoad)
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
            foreach (GSD.Roads.GSDTerraforming.TempTerrainData TTD in TTDList)
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
                    GSDTerraformingT.DoRects(spline, TTD);
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
                //		    if(IsApproximately(tFloat,1f,0.00001f) || tFloat > 1f){ continue; }
                //		    if(tFloat < 0f){ continue; }
                //		    i = tFloat;
                //	    }
                //	    tSpline.GetSplineValue_Both(i,out tVect,out POS);
                //	    PrevHeight = GSDTerraformingT.ProcessLineHeights(tSpline,ref tVect,ref POS,tDistance,TTD,PrevHeight);
                //	    tSpline.HeightHistory.Add(new KeyValuePair<float,float>(i,PrevHeight*TTD.TerrainSize.y));
                //  }	
                //					
                //	for(int i=0;i<TTD.cI;i++)
                //  {
                //		TTD.heights[TTD.cX[i],TTD.cY[i]] = TTD.cH[i];
                //	}
                //}
            }
            spline.HeightHistory.Sort(Compare1);
            IsDone = true;
        }


        private int Compare1(KeyValuePair<float, float> _a, KeyValuePair<float, float> _b)
        {
            return _a.Key.CompareTo(_b.Key);
        }
    }


    public static class TerrainCalcsStatic
    {
        public static void RunMe(ref List<GSD.Roads.GSDTerraforming.TempTerrainData> _TTDList, GSDSplineC _spline, GSDRoad _road)
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

            foreach (GSD.Roads.GSDTerraforming.TempTerrainData TTD in _TTDList)
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
                GSDRootUtil.StartProfiling(_road, "DoRects");
                GSDTerraformingT.DoRects(_spline, TTD);
                GSDRootUtil.EndProfiling(_road);
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
                //		PrevHeight = GSDTerraformingT.ProcessLineHeights(tSpline,ref tVect,ref POS,tDistance,TTD,PrevHeight);
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


    public class RoadCalcs1 : GSDThreadedJob
    {
        [UnityEngine.Serialization.FormerlySerializedAs("GSDm_Handle")]
        private object handle = new object();
        private GSD.Roads.RoadConstructorBufferMaker RCS;
        [UnityEngine.Serialization.FormerlySerializedAs("tRoad")]
        private GSDRoad road;


        public void Setup(ref GSD.Roads.RoadConstructorBufferMaker _RCS, ref GSDRoad _road)
        {
            RCS = _RCS;
            road = _road;
        }


        protected override void ThreadFunction()
        {
            try
            {
                GSDRoadCreationT.RoadJobPrelim(ref road);
                GSDRoadCreationT.RoadJob1(ref RCS);
            }
            catch (System.Exception exception)
            {
                lock (handle)
                {
                    road.isEditorError = true;
                    road.exceptionError = exception;
                }
                throw exception;
            }
        }


        public GSD.Roads.RoadConstructorBufferMaker GetRCS()
        {
            GSD.Roads.RoadConstructorBufferMaker refrenceRCS;
            lock (handle)
            {
                refrenceRCS = RCS;
            }
            return refrenceRCS;
        }
    }


    public static class RoadCalcs1Static
    {
        public static void RunMe(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            GSDRoadCreationT.RoadJob1(ref _RCS);
        }
    }


    public class RoadCalcs2 : GSDThreadedJob
    {
        [UnityEngine.Serialization.FormerlySerializedAs("GSDm_Handle")]
        private object handle = new object();
        private GSD.Roads.RoadConstructorBufferMaker RCS;


        public void Setup(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            RCS = _RCS;
        }


        protected override void ThreadFunction()
        {
            try
            {
                GSDRoadCreationT.RoadJob2(ref RCS);
            }
            catch (System.Exception exception)
            {
                lock (handle)
                {
                    RCS.road.isEditorError = true;
                    RCS.road.exceptionError = exception;
                }
            }
        }


        public GSD.Roads.RoadConstructorBufferMaker GetRCS()
        {
            GSD.Roads.RoadConstructorBufferMaker tRCS;
            lock (handle)
            {
                tRCS = RCS;
            }
            return tRCS;
        }
    }


    public static class RoadCalcs2Static
    {
        public static void RunMe(ref GSD.Roads.RoadConstructorBufferMaker _RCS)
        {
            GSDRoadCreationT.RoadJob2(ref _RCS);
        }
    }
#endif
}