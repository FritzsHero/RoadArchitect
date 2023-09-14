using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect.Threading
{
    public static class TerraformingThreaded
    {
        public class TerrainBoundsMaker
        {
            public List<Construction3DTri> triList;
            public Construction2DRect constructRect;
            public float MinI = 0f;
            public float MaxI = 1f;
        }


        /*
        private static Vector3 ProcessLineHeights_PrevVect = new Vector3(0f, 0f, 0f);


        public static float ProcessLineHeights(SplineC tSpline, ref Vector3 tVect, ref Vector3 POS, float tDistance, Terraforming.TempTerrainData TTD, float PrevDesiredHeight)
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


        private static float ProcessLineHeights_GetDesiredHeight(Vector3 tVect, ref Terraforming.TempTerrainData TTD, ref SplineC tSpline)
        {
            return ((((tVect - TTD.TerrainPos).y) - tSpline.tRoad.opt_TerrainSubtract_Alt) / TTD.TerrainSize.y);
        }
        */


        /// <summary> Writes heightmap location of _vector into _x and _y </summary>
        private static void GetTempHeightsCoordinates(ref Vector3 _vector, ref Terraforming.TempTerrainData _TTD, out int _x, out int _y)
        {
            //Get the normalized position of this game object relative to the terrain:
            Vector3 tempCoord = (_vector - _TTD.TerrainPos);

            Vector3 coord;
            coord.x = tempCoord.x / _TTD.TerrainSize.x;
            coord.y = tempCoord.y / _TTD.TerrainSize.y;
            coord.z = tempCoord.z / _TTD.TerrainSize.z;

            //Get the position of the terrain heightmap where this game object is:
            _y = (int)(coord.x * _TTD.HM);
            _x = (int)(coord.z * _TTD.HM);
        }


        /// <summary> Writes heightmap location of _vector into _x and _y </summary>
        private static void GetTempDetailsCoordinates(ref Vector3 _vector, ref Terraforming.TempTerrainData _TTD, out int _x, out int _y)
        {
            //Get the normalized position of this game object relative to the terrain:
            Vector3 tempCoord = (_vector - _TTD.TerrainPos);

            Vector3 coord;
            coord.x = tempCoord.x / _TTD.TerrainSize.x;
            coord.y = tempCoord.y / _TTD.TerrainSize.y;
            coord.z = tempCoord.z / _TTD.TerrainSize.z;

            //Get the position of the terrain heightmap where this game object is:
            _y = (int)(coord.x * _TTD.DetailMaxIndex);
            _x = (int)(coord.z * _TTD.DetailMaxIndex);
        }


        //Privatized for obfuscate:
        public static void DoRects(SplineC _spline, Terraforming.TempTerrainData _TTD)
        {
            DoRectsDo(ref _spline, ref _TTD);
        }


        private static void DoRectsDo(ref SplineC _spline, ref Terraforming.TempTerrainData _TTD)
        {
            float Sep = _spline.road.RoadWidth() * 0.5f;
            float HeightSep = Sep + (_spline.road.matchHeightsDistance * 0.5f);
            List<TerrainBoundsMaker> TBMList = new List<TerrainBoundsMaker>();
            //List<RoadUtility.Construction3DTri> triList = new List<RoadUtility.Construction3DTri>();
            List<Construction2DRect> TreerectList = new List<Construction2DRect>();
            float tStep = _spline.road.roadDefinition / _spline.distance;
            //tStep *= 0.5f;

            //Start initializing the loop. Convuluted to handle special control nodes, so roads don't get rendered where they aren't supposed to, while still preserving the proper curvature.
            float FinalMax = 1f;
            float StartMin = 0f;
            if (_spline.isSpecialEndControlNode)
            {
                //If control node, start after the control node:
                FinalMax = _spline.nodes[_spline.GetNodeCount() - 2].time;
            }
            if (_spline.isSpecialStartControlNode)
            {
                //If ends in control node, end construction before the control node:
                StartMin = _spline.nodes[1].time;
            }
            //Storage of incremental start values for the road connection mesh construction at the end of this function.
            //float RoadConnection_StartMin1 = StartMin;
            //Storage of incremental end values for the road connection mesh construction at the end of this function.
            //float RoadConnection_FinalMax1 = FinalMax;
            if (_spline.isSpecialEndNodeIsStartDelay)
            {
                //If there's a start delay (in meters), delay the start of road construction: Due to special control nodes for road connections or 3 way intersections.
                StartMin += (_spline.specialEndNodeDelayStart / _spline.distance);
            }
            else if (_spline.isSpecialEndNodeIsEndDelay)
            {
                //If there's a end delay (in meters), cut early the end of road construction: Due to special control nodes for road connections or 3 way intersections.
                FinalMax -= (_spline.specialEndNodeDelayEnd / _spline.distance);
            }
            //Storage of incremental start values for the road connection mesh construction at the end of this function.
            //float RoadConnection_StartMin2 = StartMin;
            //Storage of incremental end values for the road connection mesh construction at the end of this function.
            //float RoadConnection_FinalMax2 = FinalMax;

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
            float fValueMod = _spline.road.roadDefinition / _spline.distance;
            bool bIsPastInter = false;
            float tIntStrength = 0f;
            float tIntStrength2 = 0f;
            //bool bMaxIntersection = false;
            SplineN xNode = null;
            float tIntHeight = 0f;
            float tIntHeight2 = 0f;
            RoadIntersection roadIntersection = null;
            float T1SUB = 0f;
            float T2SUB = 0f;
            bool bIntStr1_Full = false;
            bool bIntStr1_FullPrev = false;
            bool bIntStr1_FullNext = false;
            bool bIntStr2_Full = false;
            bool bIntStr2_FullPrev = false;
            bool bIntStr2_FullNext = false;
            Vector3 tVect3 = default(Vector3);
            List<int[]> tXYs = new List<int[]>();
            float TreeClearDist = _spline.road.clearTreesDistance;
            if (TreeClearDist < _spline.road.RoadWidth())
            {
                TreeClearDist = _spline.road.RoadWidth();
            }
            Construction2DRect tRect = null;
            float tGrade = 0f;
            for (float index = StartMin; index < FinalMax; index += tStep)
            {
                if (_spline.road.isHeightModificationEnabled)
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

                    _spline.GetSplineValueBoth(index, out tVect1, out POS1);

                    if (tNext <= 1f)
                    {
                        _spline.GetSplineValueBoth(tNext, out tVect2, out POS2);
                    }
                    else
                    {
                        _spline.GetSplineValueBoth(1f, out tVect2, out POS2);
                    }

                    //Determine if intersection:
                    bIsPastInter = false;
                    //If past intersection
                    tIntStrength = _spline.IntersectionStrength(ref tVect1, ref tIntHeight, ref roadIntersection, ref bIsPastInter, ref index, ref xNode);
                    //if(IsApproximately(tIntStrength,1f,0.001f) || tIntStrength > 1f)
                    //{
                    //	bMaxIntersection = true;
                    //}
                    //else
                    //{
                    //	bMaxIntersection = false;	
                    //}

                    tIntStrength2 = _spline.IntersectionStrength(ref tVect2, ref tIntHeight2, ref roadIntersection, ref bIsPastInter, ref index, ref xNode);
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
                    {
                        // || IsApproximately(tIntStrength,0f,0.01f)){
                        if (RootUtils.IsApproximately(tIntStrength, 1f, 0.01f))
                        {
                            T1SUB = tIntHeight;
                            bIntStr1_Full = true;
                            bIntStr1_FullNext = false;
                        }
                        else
                        {
                            bIntStr1_Full = false;
                            bIntStr1_FullNext = (tIntStrength2 >= 1f);
                            if (!RootUtils.IsApproximately(tIntStrength, 0f, 0.01f))
                            {
                                T1SUB = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * tVect1.y);
                            }
                            //if(tIntStrength <= 0f)
                            //{
                            //  T1SUB = (tIntStrength*tIntHeight) + ((1-tIntStrength)*tVect1.y);
                            //}
                        }

                        if ((bIntStr1_Full && !bIntStr1_FullPrev) || (!bIntStr1_Full && bIntStr1_FullNext))
                        {
                            tGrade = _spline.GetCurrentNode(index).gradeToPrevValue;
                            if (tGrade < 0f)
                            {
                                T1SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T1SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, tGrade / 20f);
                            }

                            //if(tGrade < 0f)
                            //{
                            //	T1SUB *= -1f;
                            //}
                        }
                        else if (bIntStr1_Full && !bIntStr1_FullNext)
                        {
                            tGrade = _spline.GetCurrentNode(index).gradeToNextValue;
                            if (tGrade < 0f)
                            {
                                T1SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T1SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, tGrade / 20f);
                            }
                            //if(tGrade < 0f)
                            //{
                            //	T1SUB *= -1f;
                            //}
                        }
                        else
                        {
                            T1SUB -= 0.02f;
                        }
                        bIntStr1_FullPrev = bIntStr1_Full;
                    }

                    if (tIntStrength2 >= 0f || RootUtils.IsApproximately(tIntStrength2, 0f, 0.01f))
                    {
                        //if(!IsApproximately(tIntStrength,1f,0.01f)){ 
                        if (RootUtils.IsApproximately(tIntStrength, 1f, 0.01f))
                        {
                            bIntStr2_Full = true;
                            T2SUB = tIntHeight2;
                        }
                        else
                        {
                            bIntStr2_Full = false;
                            if (!RootUtils.IsApproximately(tIntStrength2, 0f, 0.01f))
                            {
                                T2SUB = (tIntStrength2 * tIntHeight) + ((1 - tIntStrength2) * tVect2.y);
                            }
                            //if(tIntStrength2 <= 0f)
                            //{
                            //T2SUB = (tIntStrength2*tIntHeight) + ((1-tIntStrength2)*tVect2.y);
                            //}
                        }

                        if ((bIntStr2_Full && !bIntStr2_FullPrev))
                        {
                            tGrade = _spline.GetCurrentNode(index).gradeToPrevValue;
                            if (tGrade < 0f)
                            {
                                T2SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T2SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, tGrade / 20f);
                            }
                            //T2SUB -= tIntHeight2 - tVect2.y;
                        }
                        else if (bIntStr2_Full && !bIntStr2_FullNext)
                        {
                            tGrade = _spline.GetCurrentNode(index).gradeToNextValue;
                            if (tGrade < 0f)
                            {
                                T2SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, (tGrade / 20f) * -1f);
                            }
                            else
                            {
                                T2SUB -= Mathf.Lerp(0.02f, roadIntersection.gradeMod, tGrade / 20f);
                            }
                            //if(tGrade < 0f)
                            //{
                            //	T2SUB *= -1f;
                            //}
                            //T2SUB -= tIntHeight2 - tVect2.y;
                        }
                        else if (!bIntStr2_Full)
                        {
                            if (tNext + tStep < 1f)
                            {
                                tVect3 = _spline.GetSplineValue(tNext + tStep, false);
                                tIntStrength2 = _spline.IntersectionStrength(ref tVect3, ref tIntHeight2, ref roadIntersection, ref bIsPastInter, ref index, ref xNode);
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
                    tRect = SetDetailCoords(index, ref tVect1, ref POS1, ref tVect2, ref POS2, _spline.road.clearDetailsDistance, TreeClearDist, ref _TTD, ref _spline);
                    if (_spline.road.isTreeModificationEnabled && tRect != null)
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

                    _spline.GetSplineValueBoth(index, out tVect1, out POS1);

                    if (tNext <= 1f)
                    {
                        _spline.GetSplineValueBoth(tNext, out tVect2, out POS2);
                    }
                    else
                    {
                        _spline.GetSplineValueBoth(1f, out tVect2, out POS2);
                    }

                    //Details and trees:
                    tRect = SetDetailCoords(index, ref tVect1, ref POS1, ref tVect2, ref POS2, _spline.road.clearDetailsDistance, TreeClearDist, ref _TTD, ref _spline);
                    if (_spline.road.isTreeModificationEnabled && tRect != null)
                    {
                        TreerectList.Add(tRect);
                    }
                }
            }

            RootUtils.StartProfiling(_spline.road, "DoRectsTree");
            if (_spline.road.isTreeModificationEnabled && TreerectList != null && TreerectList.Count > 0)
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
                            _TTD.treesCount += 1;
                            break;
                        }
                    }
                }
                _TTD.TreesCurrent.RemoveAll(item => item.prototypeIndex < -1);
            }
            RootUtils.EndProfiling(_spline.road);

            if (!_spline.road.isHeightModificationEnabled)
            {
                return;
            }

            ////Temp testing:
            //tSpline.mNodes[22].tTriList = new List<RoadUtility.Construction3DTri>();
            //int tCount = triList.Count;
            //for(int i=0;i<tCount;i++)
            //{
            //	tSpline.mNodes[22].tTriList.Add(triList[i]);	
            //}
            //tSpline.mNodes[22].tHMList = new List<Vector3>();


            float tFloat = -1f;
            Sep = _spline.road.RoadWidth() * 1.5f;
            int k = 0;
            int[] tXY = null;
            int tXYsCount = tXYs.Count;
            bool isBridge = false;
            bool isTunnel = false;
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

                //If in bridge mode:
                if (_spline.IsInBridgeTerrain(index))
                {
                    isBridge = true;
                }
                else
                {
                    isBridge = false;
                }
                //If in tunnel mode:
                if (_spline.IsInTunnelTerrain(index))
                {
                    isTunnel = true;
                }
                else
                {
                    isTunnel = false;
                }

                if (k < tXYsCount)
                {
                    tXY = tXYs[k];
                    tFloat = ProcessCoordinateGrabber(ref index, ref _spline, ref _TTD, ref TBMList, ref tXY, isBridge, isTunnel);
                    if (!RootUtils.IsApproximately(tFloat, 0f, 0.0001f))
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
            //mCount = rectList.Count;
            //LastIndexToRemove = -1;
            //for(int i=0;i<mCount;i++)
            //{
            //  if(tList[i].MaxI < CurrentI)
            //  {
            //	    LastIndexToRemove = i;
            //  }
            //  else
            //  {
            //	    break;
            //  }
            //}
            //if(LastIndexToRemove >= 0)
            //{
            //	rectList.RemoveRange(0,LastIndexToRemove);
            //}
        }


        private static int[] CreateTris(float _i, float _i2, ref Vector3 _vect1, ref Vector3 _POS1, ref Vector3 _vect2, ref Vector3 _POS2, float _sep, ref List<TerrainBoundsMaker> _list, ref float _T1SUB, ref float _T2SUB, ref Terraforming.TempTerrainData _TTD, float _heightSep)
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
            TBM.triList = new List<Construction3DTri>();

            TBM.triList.Add(new Construction3DTri(lVect1, rVect1, lVect2, _i, _i2));
            TBM.triList.Add(new Construction3DTri(lVect2, rVect1, rVect2, _i, _i2));

            Vector3 lVect1far = (_vect1 + new Vector3(_heightSep * -_POS1.normalized.z, 0, _heightSep * _POS1.normalized.x));
            Vector3 rVect1far = (_vect1 + new Vector3(_heightSep * _POS1.normalized.z, 0, _heightSep * -_POS1.normalized.x));
            Vector3 lVect2far = (_vect2 + new Vector3(_heightSep * -_POS2.normalized.z, 0, _heightSep * _POS2.normalized.x));
            Vector3 rVect2far = (_vect2 + new Vector3(_heightSep * _POS2.normalized.z, 0, _heightSep * -_POS2.normalized.x));

            lVect1far.y = lVect1.y;
            lVect2far.y = lVect2.y;
            rVect1far.y = rVect1.y;
            rVect2far.y = rVect2.y;

            TBM.triList.Add(new Construction3DTri(lVect1far, lVect1, lVect2far, _i, _i2));
            TBM.triList.Add(new Construction3DTri(lVect2far, lVect1, lVect2, _i, _i2));
            TBM.triList.Add(new Construction3DTri(rVect1, rVect1far, rVect2, _i, _i2));
            TBM.triList.Add(new Construction3DTri(rVect2, rVect1far, rVect2far, _i, _i2));

            TBM.constructRect = new Construction2DRect(new Vector2(lVect1far.x, lVect1far.z), new Vector2(rVect1far.x, rVect1far.z), new Vector2(rVect2far.x, rVect2far.z), new Vector2(lVect2far.x, lVect2far.z), 0f);
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


        private static Construction2DRect SetDetailCoords(float _param, ref Vector3 _vect1, ref Vector3 _POS1, ref Vector3 _vect2, ref Vector3 POS2, float _sep, float _treeSep, ref Terraforming.TempTerrainData _TTD, ref SplineC _spline)
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

            float tDiff1 = ((_TTD.heights[x2, y2] * (float)_TTD.TerrainSize.y) - _vect1.y);
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
            float tDiff2 = ((_TTD.heights[x3, y3] * (float)_TTD.TerrainSize.y) - _vect2.y);



            Construction2DRect tRect = null;
            if (_spline.road.isTreeModificationEnabled)
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
                    if (tDiff1 > _spline.road.clearTreesDistanceHeight)
                    {
                        isQuit = true;
                    }
                    if (tDiff2 > _spline.road.clearTreesDistanceHeight)
                    {
                        isQuit = true;
                    }
                }
                if (isInTunnel && !isQuit)
                {
                    if (tDiff1 < 0f)
                    {
                        if ((tDiff1 * -1f) > _spline.road.clearTreesDistanceHeight)
                        {
                            isQuit = true;
                        }
                    }
                    else
                    {
                        if (tDiff1 > (_spline.road.clearTreesDistanceHeight * 0.1f))
                        {
                            isQuit = true;
                        }
                    }
                    if (tDiff2 < 0f)
                    {
                        if ((tDiff2 * -1f) > _spline.road.clearTreesDistanceHeight)
                        {
                            isQuit = true;
                        }
                    }
                    else
                    {
                        if (tDiff2 > (_spline.road.clearTreesDistanceHeight * 0.1f))
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
                    tRect = new Construction2DRect(new Vector2(lVect1far.x, lVect1far.z), new Vector2(rVect1far.x, rVect1far.z), new Vector2(rVect2far.x, rVect2far.z), new Vector2(lVect2far.x, lVect2far.z), 0f);
                }
            }

            if (_spline.road.isDetailModificationEnabled)
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

                    if (tDiff1 > _spline.road.clearDetailsDistanceHeight)
                    {
                        isQuit = true;
                    }
                    if (tDiff2 > _spline.road.clearDetailsDistanceHeight)
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
                RootUtils.StartProfiling(_spline.road, "Dorectsdetails");
                int tInt = 0;
                for (int index = MinX; index <= MaxX; index++)
                {
                    for (int k = MinY; k <= MaxY; k++)
                    {
                        //Bitfield for identification:
                        tInt = k;
                        tInt = tInt << 16;
                        tInt = tInt | (ushort)index;
                        if (!_TTD.DetailHasProcessed.Contains(tInt))
                        {
                            //for(int h=0;h<TTD.DetailLayersCount;h++)
                            //{
                            //  if(TTD.DetailLayersSkip.Contains(h))
                            //  {
                            //      continue;
                            //  }
                            //	if(!TTD.DetailHasProcessed[h][i,k]){// && TTD.DetailValues[h][i,k] > 0){

                            _TTD.MainDetailsX.Add((ushort)index);
                            _TTD.MainDetailsY.Add((ushort)k);

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
                RootUtils.EndProfiling(_spline.road);
            }

            return tRect;
        }


        private static float ProcessCoordinateGrabber(ref float _param, ref SplineC _spline, ref Terraforming.TempTerrainData _TTD, ref List<TerrainBoundsMaker> _terrainList, ref int[] _XY, bool _isBridge, bool _isTunnel)
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
            bool isAdjusted = false;
            float tHeight = -1f;
            float tReturnFloat = 0f;

            for (int index = MinX; index <= MaxX; index++)
            {
                for (int k = MinY; k <= MaxY; k++)
                {
                    if (_TTD.tHeights[index, k] != true)
                    {
                        if (_TTD.cX.Length <= _TTD.Count)
                        {
                            break;
                        }

                        xVect = ConvertTerrainCoordToWorldVect(index, k, _TTD.heights[index, k], ref _TTD);
                        AdjustedTerrainVectTri(ref _param, out isAdjusted, out tHeight, ref xVect, ref _terrainList, _isBridge, _isTunnel);

                        if (isAdjusted)
                        {
                            tHeight -= _spline.road.matchTerrainSubtraction;
                            if (tHeight < 0f)
                            {
                                tHeight = 0f;
                            }
                            xVect.y = tHeight;
                            tHeight = ((tHeight) / _TTD.TerrainSize.y);

                            //Set height values:
                            _TTD.tHeights[index, k] = true;
                            _TTD.cX[_TTD.Count] = (ushort)index;
                            _TTD.cY[_TTD.Count] = (ushort)k;
                            _TTD.oldH[_TTD.Count] = _TTD.heights[index, k];
                            _TTD.heights[index, k] = tHeight;
                            _TTD.Count += 1;

                            tReturnFloat = xVect.y;
                        }
                    }
                    else
                    {
                        xVect = ConvertTerrainCoordToWorldVect(index, k, _TTD.heights[index, k], ref _TTD);
                        AdjustedTerrainVectTri(ref _param, out isAdjusted, out tHeight, ref xVect, ref _terrainList, _isBridge, _isTunnel);

                        if (isAdjusted)
                        {
                            tHeight -= _spline.road.matchTerrainSubtraction;
                            if (tHeight < 0f)
                            {
                                tHeight = 0f;
                            }
                            tReturnFloat = tHeight;
                        }
                    }
                }
            }

            if (_isBridge && RootUtils.IsApproximately(tReturnFloat, 0f, 0.0001f))
            {
                tReturnFloat = _spline.GetSplineValue(_param, false).y;
            }

            return tReturnFloat;
        }


        private static Vector3 ConvertTerrainCoordToWorldVect(int _x, int _y, float _height, ref Terraforming.TempTerrainData _TTD)
        {
            //Get the normalized position of this game object relative to the terrain:
            float x1 = _x / ((float)_TTD.HM - 1f);
            x1 = x1 * _TTD.TerrainSize.x;

            float z1 = _y / ((float)_TTD.HM - 1f);
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
            Construction3DTri tTri;
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
}
