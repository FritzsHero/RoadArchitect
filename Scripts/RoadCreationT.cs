#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using RoadArchitect;
using RoadArchitect.Threading;
#endregion


namespace RoadArchitect.Threading
{
    public static class RoadCreationT
    {
        #region "Road Prelim"
        public static void RoadJobPrelim(ref Road _road)
        {
            #region "Vars"
            SplineC spline = _road.spline;
            //Road,shoulder,ramp and lane widths:
            float roadWidth = _road.RoadWidth();
            float shoulderWidth = _road.shoulderWidth;
            float roadSeperation = roadWidth / 2f;
            float roadSeperationNoTurn = roadWidth / 2f;
            float shoulderSeperation = roadSeperation + shoulderWidth;
            float laneWidth = _road.laneWidth;
            float roadSep1Lane = (roadSeperation + (laneWidth * 0.5f));
            float roadSep2Lane = (roadSeperation + (laneWidth * 1.5f));
            float shoulderSep1Lane = (shoulderSeperation + (laneWidth * 0.5f));
            float shoulderSep2Lane = (shoulderSeperation + (laneWidth * 1.5f));

            //Vector3 buffers used in construction:
            Vector3 rightVector = default(Vector3);
            Vector3 leftVector = default(Vector3);
            Vector3 ShoulderR_rVect = default(Vector3);
            Vector3 ShoulderR_lVect = default(Vector3);
            Vector3 ShoulderL_rVect = default(Vector3);
            Vector3 ShoulderL_lVect = default(Vector3);
            Vector3 RampR_R = default(Vector3);
            Vector3 RampR_L = default(Vector3);
            Vector3 RampL_R = default(Vector3);
            Vector3 RampL_L = default(Vector3);

            //Previous temp storage values:
            Vector3 tVect_Prev = default(Vector3);
            Vector3 rVect_Prev = default(Vector3);
            Vector3 lVect_Prev = default(Vector3);
            Vector3 ShoulderR_PrevLVect = default(Vector3);
            Vector3 ShoulderL_PrevRVect = default(Vector3);
            Vector3 ShoulderR_PrevRVect = default(Vector3);
            Vector3 ShoulderL_PrevLVect = default(Vector3);
            Vector3 RampR_PrevR = default(Vector3);
            Vector3 RampR_PrevL = default(Vector3);
            Vector3 RampL_PrevR = default(Vector3);
            Vector3 RampL_PrevL = default(Vector3);

            //Height and angle variables, used to change certain parameters of road depending on past & future angle and height changes.
            float Step = _road.roadDefinition / spline.distance;
            Vector3 tHeight0 = new Vector3(0f, 0.1f, 0f);
            float OuterShoulderWidthR = 0f;
            float OuterShoulderWidthL = 0f;
            float RampOuterWidthR = (OuterShoulderWidthR / 6f) + OuterShoulderWidthR;
            float RampOuterWidthL = (OuterShoulderWidthL / 6f) + OuterShoulderWidthL;
            Vector3 tVect = default(Vector3);
            Vector3 POS = default(Vector3);
            float TempY = 0f;
            float heightAdded = 0f;
            Vector3 gHeight = default(Vector3);

            //Bridge variables:
            bool isBridge = false;
            bool isTempbridge = false;
            float BridgeUpComing;

            //Tunnel variables:	
            bool isTunnel = false;
            bool isTempTunnel = false;

            //Intersection variables:
            float tIntHeight = 0f;
            float tIntStrength = 0f;
            //float tIntStrength_temp = 0f;
            RoadIntersection intersection = null;
            bool isPastInter = false;
            bool isMaxIntersection = false;
            bool isWasPrevMaxInter = false;
            SplineN xNode = null;
            float tInterSubtract = 4f;
            float tLastInterHeight = -4f;
            bool isOverridenRampR = false;
            bool isOverridenRampL = false;
            Vector3 RampR_Override = default(Vector3);
            Vector3 RampL_Override = default(Vector3);
            bool isFirstInterNode = false;
            bool isInterPrevWasCorner = false;
            bool isInterCurrentIsCorner = false;
            bool isInterCurrentIsCornerRR = false;
            bool isInterCurreIsCornerRL = false;
            bool isInterCurreIsCornerLL = false;
            bool isInterCurreIsCornerLR = false;
            bool isInterPrevWasCornerRR = false;
            bool isInterPrevWasCornerRL = false;
            bool isInterPrevWasCornerLL = false;
            bool isInterPrevWasCornerLR = false;
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
            bool is0LAdded = false;
            bool is1LAdded = false;
            bool is2LAdded = false;
            bool isf0LAdded = false;
            bool isf1LAdded = false;
            bool isf2LAdded = false;
            bool isf3LAdded = false;
            bool is1RAdded = false;
            bool is2RAdded = false;
            bool is3RAdded = false;
            bool isShoulderSkipR = false;
            bool isShoulderSkipL = false;
            bool isShrinkRoadB = false;
            bool isShrinkRoadFNext = false;
            bool isShrinkRoadF = false;
            bool isNextInter = false;
            SplineN currentNode = null;
            int currentNodeID = -1;
            int previousNodeID = -1;
            int NodeCount = spline.GetNodeCount();
            bool isDynamicCut = false;
            float CullDistanceSQ = (3f * roadWidth) * (3f * roadWidth);
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
            bool isLRtoRR = false;
            bool isLLtoLR = false;
            bool isLine = false;
            bool isImmuneR = false;
            bool isImmuneL = false;
            bool isSpecAddedL = false;
            bool isSpecAddedR = false;
            bool isTriggerInterAddition = false;
            bool isSpecialThreeWayIgnoreR = false;
            bool isSpecialThreeWayIgnoreL = false;
            float bMod1 = 1.75f;
            float bMod2 = 1.25f;
            float t2DDist = -1f;
            List<Vector3> vList = null;
            List<int> eList = null;
            float param2 = 0f;
            float param1 = 0f;
            bool isRecordShoulderForNormals = false;
            bool isRecordShoulderLForNormals = false;
            //Prev storage of shoulder variable (2 step history).
            //Vector3 ShoulderR_PrevRVect2 = default(Vector3);
            //Prev storage of shoulder variable (2 step history).
            //Vector3 ShoulderL_PrevLVect2 = default(Vector3);
            //Prev storage of shoulder variable (3 step history).
            //Vector3 ShoulderR_PrevRVect3 = default(Vector3);
            //Prev storage of shoulder variable (3 step history).
            //Vector3 ShoulderL_PrevLVect3 = default(Vector3);
            //Prev storage of outer shoulder direction (euler).
            //Vector3 ShoulderR_OuterDirectionPrev = default(Vector3);
            //Prev storage of outer shoulder direction (euler).
            //Vector3 ShoulderL_OuterDirectionPrev = default(Vector3);
            //Vector3 ShoulderR_OuterDirection = default(Vector3);
            //Vector3 ShoulderL_OuterDirection = default(Vector3);
            //Vector3 tHeight2 = new Vector3(0f,0.15f,0f);
            //Vector3 tHeight1 = new Vector3(0f,0.2f,0f);
            //bool bTempYWasNegative = false;
            //Vector3 tY = new Vector3(0f,0f,0f);
            //float[] HeightChecks = new float[5];
            //int eCount = -1;
            //int eIndex = -1;
            //int uCount = -1;
            //int uIndex = -1;

            //Unused for now, for later partial construction methods:
            bool isInterseOn = _road.RCS.isInterseOn;
            isInterseOn = true;
            #endregion


            //Prelim intersection construction and profiling:
            RootUtils.StartProfiling(_road, "RoadJob_Prelim_Inter");
            if (isInterseOn)
            {
                RoadJobPrelimInter(ref _road);
            }
            RootUtils.EndStartProfiling(_road, "RoadPrelimForLoop");

            //Road/shoulder cuts: Init necessary since a road cut is added for the last segment after this function:
            if (_road.isRoadCutsEnabled || _road.isDynamicCutsEnabled)
            {
                _road.RCS.RoadCutNodes.Add(spline.nodes[0]);
            }
            if (_road.isShoulderCutsEnabled || _road.isDynamicCutsEnabled)
            {
                _road.RCS.ShoulderCutsLNodes.Add(spline.nodes[0]);
                _road.RCS.ShoulderCutsRNodes.Add(spline.nodes[0]);
            }

            //Start initializing the loop. Convuluted to handle special control nodes, so roads don't get rendered where they aren't supposed to, while still preserving the proper curvature.
            float FinalMax = 1f;
            float StartMin = 0f;
            if (spline.isSpecialEndControlNode)
            {
                //If control node, start after the control node:
                FinalMax = spline.nodes[spline.GetNodeCount() - 2].time;
            }
            if (spline.isSpecialStartControlNode)
            {
                //If ends in control node, end construction before the control node:
                StartMin = spline.nodes[1].time;
            }
            bool isFinalEnd = false;
            //Storage of incremental start values for the road connection mesh construction at the end of this function.
            float RoadConnection_StartMin1 = StartMin;
            //Storage of incremental end values for the road connection mesh construction at the end of this function.
            float RoadConnection_FinalMax1 = FinalMax;
            if (spline.isSpecialEndNodeIsStartDelay)
            {
                //If there's a start delay (in meters), delay the start of road construction: Due to special control nodes for road connections or 3 way intersections.
                StartMin += (spline.specialEndNodeDelayStart / spline.distance);
            }
            else if (spline.isSpecialEndNodeIsEndDelay)
            {
                //If there's a end delay (in meters), cut early the end of road construction: Due to special control nodes for road connections or 3 way intersections.
                FinalMax -= (spline.specialEndNodeDelayEnd / spline.distance);
            }
            //Storage of incremental start values for the road connection mesh construction at the end of this function.
            //float RoadConnection_StartMin2 = StartMin;
            //Storage of incremental end values for the road connection mesh construction at the end of this function.
            //float RoadConnection_FinalMax2 = FinalMax;
            float i = StartMin;

            //int StartIndex = tSpline.GetClosestRoadDefIndex(StartMin,true,false);
            //int EndIndex = tSpline.GetClosestRoadDefIndex(FinalMax,false,true);
            bool kSkip = true;
            bool kSkipFinal = false;
            int kCount = 0;
            int vCount = kCount;
            int kFinalCount = spline.RoadDefKeysArray.Length;
            int spamcheckmax1 = 18000;
            int spamcheck1 = 0;

            if (RootUtils.IsApproximately(StartMin, 0f, 0.0001f))
            {
                kSkip = false;
            }
            if (RootUtils.IsApproximately(FinalMax, 1f, 0.0001f))
            {
                kSkipFinal = true;
            }

            //If startmin > 0 then kcount needs to start at proper road def
            int StartMinIndex1 = 0;

            if (StartMin > 0f)
            {
                kCount = spline.GetClosestRoadDefIndex(StartMin, true, false);
                StartMinIndex1 = 1;
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
                        {
                            break;
                        }
                    }
                    else
                    {
                        i = spline.TranslateInverseParamToFloat(spline.RoadDefKeysArray[kCount]);
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

                if (RootUtils.IsApproximately(i, FinalMax, 0.00001f))
                {
                    isFinalEnd = true;
                }
                else if (i > FinalMax)
                {
                    if (spline.isSpecialEndControlNode)
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


                //Set the current node.
                currentNode = spline.GetCurrentNode(i);
                //Set the current node ID.
                currentNodeID = currentNode.idOnSpline;


                //If different than the previous node id, time to make a cut, if necessary:
                if (currentNodeID != previousNodeID && (_road.isRoadCutsEnabled || _road.isDynamicCutsEnabled))
                {
                    //Don't ever cut the first node, last node, intersection node, special control nodes, bridge nodes or bridge control nodes:
                    if (currentNodeID > StartMinIndex1 && currentNodeID < (NodeCount - 1) && !currentNode.isIntersection && !currentNode.isSpecialEndNode)
                    {
                        // && !cNode.bIsBridge_PreNode && !cNode.bIsBridge_PostNode){
                        if (_road.isDynamicCutsEnabled)
                        {
                            isDynamicCut = currentNode.isRoadCut;
                        }
                        else
                        {
                            isDynamicCut = true;
                        }

                        if (isDynamicCut)
                        {
                            //Add the vector index to cut later.
                            _road.RCS.RoadCuts.Add(_road.RCS.RoadVectors.Count);
                            //Store the node which was at the beginning of this cut.	
                            _road.RCS.RoadCutNodes.Add(currentNode);
                        }


                        if (_road.isShoulderCutsEnabled && isDynamicCut)
                        {
                            //If option shoulder cuts is on.
                            //Add the vector index to cut later.
                            _road.RCS.ShoulderCutsL.Add(_road.RCS.ShoulderL_Vectors.Count);
                            _road.RCS.ShoulderCutsR.Add(_road.RCS.ShoulderR_Vectors.Count);
                            //Store the node which was at the beginning of this cut.
                            _road.RCS.ShoulderCutsLNodes.Add(currentNode);
                            _road.RCS.ShoulderCutsRNodes.Add(currentNode);
                        }
                    }
                }


                //If different than the previous node id, we store the RoadVectorsHeights in initialRoadHeight
                if (currentNodeID != previousNodeID)
                {
                    if (_road.RCS.RoadVectors.Count > 0)
                    {
                        currentNode.initialRoadHeight = _road.RCS.RoadVectors[_road.RCS.RoadVectors.Count - 1].y;
                    }
                }


                //Store the current node ID as previous for the next round.
                //Done now with road cuts as far as this function is concerned.
                previousNodeID = currentNodeID;

                //Set all necessary intersection triggers to false:
                isInterCurrentIsCorner = false;
                isInterCurrentIsCornerRR = false;
                isInterCurreIsCornerRL = false;
                isInterCurreIsCornerLL = false;
                isInterCurreIsCornerLR = false;
                is0LAdded = false;
                is1LAdded = false;
                is2LAdded = false;
                isf0LAdded = false;
                isf1LAdded = false;
                isf2LAdded = false;
                isf3LAdded = false;
                is1RAdded = false;
                is2RAdded = false;
                is3RAdded = false;
                isShoulderSkipR = false;
                isShoulderSkipL = false;
                isShrinkRoadB = false;
                isShrinkRoadF = false;
                isNextInter = false;
                if (isShrinkRoadFNext)
                {
                    isShrinkRoadFNext = false;
                    isShrinkRoadF = true;
                }
                isRecordShoulderForNormals = false;
                isRecordShoulderLForNormals = false;

                //Bridges: Note: This is convoluted due to need for triggers:
                isTempbridge = spline.IsInBridge(i);


                if (!isBridge && isTempbridge)
                {
                    isBridge = true;
                }
                else if (isBridge && !isTempbridge)
                {
                    isBridge = false;
                }


                //Check if this is the last bridge run for this bridge:
                if (isBridge)
                {
                    isTempbridge = spline.IsInBridge(i + Step);
                }


                //Tunnels: Note: This is convoluted due to need for triggers:
                isTempTunnel = spline.IsInTunnel(i);


                if (!isTunnel && isTempTunnel)
                {
                    isTunnel = true;
                }
                else if (isTunnel && !isTempTunnel)
                {
                    isTunnel = false;
                }


                //Check if this is the last Tunnel run for this Tunnel:
                if (isTunnel)
                {
                    isTempTunnel = spline.IsInTunnel(i + Step);
                }


                //Master Vector3 for the current road construction location:
                spline.GetSplineValueBoth(i, out tVect, out POS);

                //Profiler.EndSample();
                //Profiler.BeginSample("Test2");

                //Detect downward or upward slope:
                TempY = POS.y;
                //bTempYWasNegative = false;
                if (TempY < 0f)
                {
                    //bTempYWasNegative = true;
                    TempY *= -1f;
                }
                if (tVect.y < 0f)
                {
                    tVect.y = 0f;
                }

                //Determine if intersection:
                if (isInterseOn)
                {
                    //If past intersection
                    isPastInter = false;
                    tIntStrength = _road.spline.IntersectionStrength(ref tVect, ref tIntHeight, ref intersection, ref isPastInter, ref i, ref xNode);
                    //1f strength = max intersection
                    isMaxIntersection = (tIntStrength >= 1f);
                    isFirstInterNode = false;
                }

                //Outer widths:
                if (isMaxIntersection && isInterseOn)
                {
                    intersection.signHeight = tIntHeight;
                    xNode.intersectionConstruction.isBLane0DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isBLane1DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isBLane2DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isBLane3DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isFLane0DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isFLane1DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isFLane2DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isFLane3DoneFinalThisRound = false;
                    xNode.intersectionConstruction.isFrontFirstRound = false;

                    // Intersections type
                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        OuterShoulderWidthR = shoulderSeperation;
                        OuterShoulderWidthL = shoulderSeperation;
                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        OuterShoulderWidthR = shoulderSep1Lane;
                        OuterShoulderWidthL = shoulderSep1Lane;
                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (isPastInter)
                        {
                            OuterShoulderWidthR = shoulderSep1Lane;
                            OuterShoulderWidthL = shoulderSep2Lane;
                        }
                        else
                        {
                            OuterShoulderWidthR = shoulderSep2Lane;
                            OuterShoulderWidthL = shoulderSep1Lane;
                        }
                    }
                }
                else
                {
                    if (TempY < 0.5f || isBridge || isTunnel)
                    {
                        OuterShoulderWidthR = shoulderSeperation;
                        OuterShoulderWidthL = shoulderSeperation;
                    }
                    else
                    {
                        OuterShoulderWidthR = shoulderSeperation + (TempY * 0.05f);
                        OuterShoulderWidthL = shoulderSeperation + (TempY * 0.05f);
                    }
                }

                if (isBridge)
                {
                    //No ramps for bridges:
                    RampOuterWidthR = OuterShoulderWidthR;
                    RampOuterWidthL = OuterShoulderWidthL;
                }
                else
                {
                    RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                    RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                }

                //The master outer road edges vector locations:
                if (isMaxIntersection && isInterseOn)
                {   
                    //If in maximum intersection, adjust road edge (also the shoulder inner edges):
                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        rightVector = (tVect + new Vector3(roadSeperationNoTurn * POS.normalized.z, 0, roadSeperationNoTurn * -POS.normalized.x));
                        leftVector = (tVect + new Vector3(roadSeperationNoTurn * -POS.normalized.z, 0, roadSeperationNoTurn * POS.normalized.x));
                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        rightVector = (tVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                        leftVector = (tVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        if (isPastInter)
                        {
                            rightVector = (tVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                            leftVector = (tVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                        }
                        else
                        {
                            rightVector = (tVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                            leftVector = (tVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                        }
                    }
                    else
                    {
                        rightVector = (tVect + new Vector3(roadSeperation * POS.normalized.z, 0, roadSeperation * -POS.normalized.x));
                        leftVector = (tVect + new Vector3(roadSeperation * -POS.normalized.z, 0, roadSeperation * POS.normalized.x));
                    }
                }
                else
                {
                    //Typical road/shoulder inner edge location:
                    rightVector = (tVect + new Vector3(roadSeperation * POS.normalized.z, 0, roadSeperation * -POS.normalized.x));
                    leftVector = (tVect + new Vector3(roadSeperation * -POS.normalized.z, 0, roadSeperation * POS.normalized.x));
                }

                //Shoulder right vectors:
                ShoulderR_rVect = (tVect + new Vector3(OuterShoulderWidthR * POS.normalized.z, 0, OuterShoulderWidthR * -POS.normalized.x));
                //Note that the shoulder inner edge is the same as the road edge vector.
                ShoulderR_lVect = rightVector;
                //Shoulder left vectors:
                //Note that the shoulder inner edge is the same as the road edge vector.
                ShoulderL_rVect = leftVector;
                ShoulderL_lVect = (tVect + new Vector3(OuterShoulderWidthL * -POS.normalized.z, 0, OuterShoulderWidthL * POS.normalized.x));

                //Profiler.EndSample();
                //Profiler.BeginSample("Test3");

                //Now to start the main lane construction for the intersection:
                if (isMaxIntersection && isInterseOn)
                {
                    //if(kCount >= tSpline.RoadDefKeysArray.Length)
                    //{
                    //	vCount = tSpline.RoadDefKeysArray.Length-1;
                    //}
                    //else
                    //{
                    //	vCount = kCount-1;	
                    //}
                    vCount = kCount;

                    param2 = spline.TranslateInverseParamToFloat(spline.RoadDefKeysArray[vCount]);
                    float tInterStrNext = _road.spline.IntersectionStrengthNext(spline.GetSplineValue(param2, false));
                    if (RootUtils.IsApproximately(tInterStrNext, 1f, 0.001f) || tInterStrNext > 1f)
                    {
                        isNextInter = true;
                    }
                    else
                    {
                        isNextInter = false;
                    }

                    if (string.Compare(xNode.uID, intersection.node1.uID) == 0)
                    {
                        isFirstInterNode = true;
                    }
                    else
                    {
                        isFirstInterNode = false;
                    }

                    tempIVect = tVect;
                    if (isPastInter)
                    {
                        bool isLLtoRL = isFirstInterNode;
                        bool isRLtoRR = !isFirstInterNode;
                        if (xNode.intersectionConstruction.iFLane0L.Count == 0)
                        {
                            xNode.intersectionConstruction.isFrontFirstRound = true;
                            xNode.intersectionConstruction.isFrontFirstRoundTriggered = true;
                            xNode.intersectionConstruction.isFLane0DoneFinalThisRound = true;
                            xNode.intersectionConstruction.isFLane1DoneFinalThisRound = true;
                            xNode.intersectionConstruction.isFLane2DoneFinalThisRound = true;
                            xNode.intersectionConstruction.isFLane3DoneFinalThisRound = true;

                            if (intersection.isFlipped && !isFirstInterNode)
                            {
                                if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerLR[0], tIntHeight));
                                    xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerLR[2], tIntHeight));
                                    xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerLLCornerLR[2], tIntHeight));
                                    xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerLLCornerLR[3], tIntHeight));
                                    xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(intersection.cornerLLCornerLR[3], tIntHeight));
                                    xNode.intersectionConstruction.iFLane3R.Add(ReplaceHeight(intersection.cornerLLCornerLR[4], tIntHeight));
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerLR[0], tIntHeight));
                                    xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerLR[2], tIntHeight));
                                    xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerLLCornerLR[2], tIntHeight));
                                    xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerLLCornerLR[3], tIntHeight));
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerLR[0], tIntHeight));
                                    xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerLR[1], tIntHeight));
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerLR[2], tIntHeight));
                                }
                            }
                            else
                            {
                                if (isLLtoRL)
                                {
                                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerRL[4], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerRL[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerRL[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerRL[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerLLCornerRL[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane3R.Add(ReplaceHeight(intersection.cornerLLCornerRL[0], tIntHeight));
                                    }
                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerRL[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerRL[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerRL[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerLLCornerRL[0], tIntHeight));
                                    }
                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerLLCornerRL[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerLLCornerRL[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerLLCornerRL[0], tIntHeight));
                                    }
                                }
                                else if (isRLtoRR)
                                {
                                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerRLCornerRR[4], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerRLCornerRR[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerRLCornerRR[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerRLCornerRR[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerRLCornerRR[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane3R.Add(ReplaceHeight(intersection.cornerRLCornerRR[0], tIntHeight));
                                    }
                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerRLCornerRR[3], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerRLCornerRR[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerRLCornerRR[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(intersection.cornerRLCornerRR[0], tIntHeight));
                                    }
                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                    {
                                        xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(intersection.cornerRLCornerRR[2], tIntHeight));
                                        xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(intersection.cornerRLCornerRR[1], tIntHeight));
                                        xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(intersection.cornerRLCornerRR[0], tIntHeight));
                                    }
                                }
                            }

                            xNode.intersectionConstruction.shoulderEndFR = xNode.intersectionConstruction.iFLane0L[0];
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.intersectionConstruction.shoulderEndFL = xNode.intersectionConstruction.iFLane3R[0];
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.intersectionConstruction.shoulderEndFL = xNode.intersectionConstruction.iFLane2R[0];
                            }
                            else
                            {
                                xNode.intersectionConstruction.shoulderEndFL = xNode.intersectionConstruction.iFLane1R[0];
                            }
                            xNode.intersectionConstruction.shoulderFLStartIndex = _road.RCS.ShoulderL_Vectors.Count - 2;
                            xNode.intersectionConstruction.shoulderFRStartIndex = _road.RCS.ShoulderR_Vectors.Count - 2;
                        }

                        //Line 0:
                        xNode.intersectionConstruction.f0LAttempt = rightVector;
                        if (!xNode.intersectionConstruction.isFLane0Done && !intersection.Contains(ref rightVector))
                        {
                            xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(rightVector, tIntHeight));
                            isf0LAdded = true;
                        }

                        //Line 1:
                        //	if(f0LAdded){
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = tVect;
                            if (!xNode.intersectionConstruction.isFLane1Done && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(tempIVect, rightVector))
                            {
                                if (isf0LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                }
                                xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                isf1LAdded = true;
                            }
                            else
                            {
                                if (isf0LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane0L.RemoveAt(xNode.intersectionConstruction.iFLane0L.Count - 1);
                                    isf0LAdded = false;
                                }
                            }
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((laneWidth * 0.5f) * POS.normalized.z, 0f, (laneWidth * 0.5f) * -POS.normalized.x));
                            if (!xNode.intersectionConstruction.isFLane1Done && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(tempIVect, rightVector))
                            {
                                if (isf0LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                }
                                xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                isf1LAdded = true;
                            }
                            else
                            {
                                if (isf0LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane0L.RemoveAt(xNode.intersectionConstruction.iFLane0L.Count - 1);
                                    isf0LAdded = false;
                                }
                            }
                        }
                        //}
                        xNode.intersectionConstruction.f0RAttempt = tempIVect;
                        xNode.intersectionConstruction.f1LAttempt = tempIVect;

                        //Line 2:
                        //if(f1LAdded){
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = leftVector;
                            if (!xNode.intersectionConstruction.isFLane2Done && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(tempIVect, rightVector))
                            {
                                if (isf1LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                }
                            }
                            else
                            {
                                if (isf1LAdded && xNode.intersectionConstruction.iFLane1L.Count > 1)
                                {
                                    xNode.intersectionConstruction.iFLane1L.RemoveAt(xNode.intersectionConstruction.iFLane1L.Count - 1);
                                    isf1LAdded = false;
                                }
                            }
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((laneWidth * 0.5f) * -POS.normalized.z, 0f, (laneWidth * 0.5f) * POS.normalized.x));
                            tempIVect_prev = tempIVect;
                            if (!xNode.intersectionConstruction.isFLane2Done && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(tempIVect, rightVector))
                            {
                                if (isf1LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                }
                                xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                isf2LAdded = true;
                            }
                            else
                            {
                                if (isf1LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane1L.RemoveAt(xNode.intersectionConstruction.iFLane1L.Count - 1);
                                    isf1LAdded = false;
                                }
                            }
                        }
                        //}
                        xNode.intersectionConstruction.f1RAttempt = tempIVect;
                        xNode.intersectionConstruction.f2LAttempt = tempIVect;

                        //Line 3 / 4:
                        //if(f2LAdded){

                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            tempIVect = (tVect + new Vector3(((laneWidth * 0.5f) + roadSeperation) * -POS.normalized.z, 0, ((laneWidth * 0.5f) + roadSeperation) * POS.normalized.x));
                            if (!xNode.intersectionConstruction.isFLane3Done && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(leftVector, tempIVect))
                            {

                                xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                isf3LAdded = true;
                                xNode.intersectionConstruction.iFLane3R.Add(ReplaceHeight(leftVector, tIntHeight));
                                //if(bIsNextInter && roadIntersection.iType == RoadIntersection.IntersectionTypeEnum.FourWay){
                                if (isf2LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                }
                                //}
                            }
                            else
                            {
                                if (isf2LAdded)
                                {
                                    xNode.intersectionConstruction.iFLane2L.RemoveAt(xNode.intersectionConstruction.iFLane2L.Count - 1);
                                    isf2LAdded = false;
                                }
                            }

                        }
                        else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            tempIVect = (tVect + new Vector3(((laneWidth * 0.5f) + roadSeperation) * -POS.normalized.z, 0, ((laneWidth * 0.5f) + roadSeperation) * POS.normalized.x));
                            if (isf2LAdded && !intersection.Contains(ref tempIVect) && !intersection.ContainsLine(rightVector, tempIVect))
                            {
                                xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                            }
                            else if (isf2LAdded)
                            {
                                xNode.intersectionConstruction.iFLane2L.RemoveAt(xNode.intersectionConstruction.iFLane2L.Count - 1);
                                isf2LAdded = false;
                            }
                        }

                        //	}
                        xNode.intersectionConstruction.f2RAttempt = tempIVect;
                        xNode.intersectionConstruction.f3LAttempt = tempIVect;
                        xNode.intersectionConstruction.f3RAttempt = leftVector;

                        if (!isNextInter && !xNode.intersectionConstruction.isFDone)
                        {
                            //xNode.intersectionConstruction.bFDone = true;
                            xNode.intersectionConstruction.isFLane0Done = true;
                            xNode.intersectionConstruction.isFLane1Done = true;
                            xNode.intersectionConstruction.isFLane2Done = true;
                            xNode.intersectionConstruction.isFLane3Done = true;

                            POS_Next = default(Vector3);
                            tVect_Next = default(Vector3);

                            param1 = spline.TranslateInverseParamToFloat(spline.RoadDefKeysArray[kCount]);
                            spline.GetSplineValueBoth(param1, out tVect_Next, out POS_Next);
                            rVect_Next = (tVect_Next + new Vector3(roadSeperation * POS_Next.normalized.z, 0, roadSeperation * -POS_Next.normalized.x));
                            lVect_Next = (tVect_Next + new Vector3(roadSeperation * -POS_Next.normalized.z, 0, roadSeperation * POS_Next.normalized.x));

                            xNode.intersectionConstruction.iFLane0L.Add(ReplaceHeight(rVect_Next, tIntHeight));
                            xNode.intersectionConstruction.iFLane0R.Add(ReplaceHeight(tVect_Next, tIntHeight));
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                if (_road.laneAmount == 2)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.475f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.488f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.492f) + lVect_Next, tIntHeight));
                                }

                                if (_road.laneAmount == 2)
                                {
                                    xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.03f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.015f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.intersectionConstruction.iFLane3L.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.01f) + lVect_Next, tIntHeight));
                                }

                                xNode.intersectionConstruction.iFLane3R.Add(ReplaceHeight(lVect_Next, tIntHeight));
                                //xNode.intersectionConstruction.iFLane2L.Add(GVC(tVect_Next,tIntHeight));	
                                //xNode.intersectionConstruction.iFLane2R.Add(GVC(lVect_Next,tIntHeight));

                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                if (_road.laneAmount == 2)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.475f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 4)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.488f) + lVect_Next, tIntHeight));
                                }
                                else if (_road.laneAmount == 6)
                                {
                                    xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(((rVect_Next - lVect_Next) * 0.492f) + lVect_Next, tIntHeight));
                                }
                                xNode.intersectionConstruction.iFLane2L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                xNode.intersectionConstruction.iFLane2R.Add(ReplaceHeight(lVect_Next, tIntHeight));

                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                xNode.intersectionConstruction.iFLane1L.Add(ReplaceHeight(tVect_Next, tIntHeight));
                                xNode.intersectionConstruction.iFLane1R.Add(ReplaceHeight(lVect_Next, tIntHeight));
                            }
                            isShrinkRoadFNext = true;
                            //bShrinkRoadF = true;
                        }

                    }
                    else
                    {
                        isLRtoRR = isFirstInterNode;
                        isLLtoLR = !isFirstInterNode;
                        //B:
                        //Line 0:
                        tempIVect = leftVector;
                        bool isFirst123 = false;
                        if (xNode.intersectionConstruction.iBLane0R.Count == 0)
                        {
                            xNode.intersectionConstruction.iBLane0L.Add(lVect_Prev);
                            xNode.intersectionConstruction.iBLane0R.Add(tVect_Prev);
                            isShrinkRoadB = true;

                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.intersectionConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.intersectionConstruction.iBLane1R.Add((tVect_Prev + new Vector3((laneWidth * 0.05f) * POS.normalized.z, 0, (laneWidth * 0.05f) * -POS.normalized.x)));
                                xNode.intersectionConstruction.iBLane3L.Add(((lVect_Prev - rVect_Prev) * 0.03f) + rVect_Prev);
                                xNode.intersectionConstruction.iBLane3R.Add(rVect_Prev);

                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.intersectionConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.intersectionConstruction.iBLane1R.Add((tVect_Prev + new Vector3((laneWidth * 0.05f) * POS.normalized.z, 0, (laneWidth * 0.05f) * -POS.normalized.x)));
                                xNode.intersectionConstruction.iBLane2L.Add(xNode.intersectionConstruction.iBLane1R[0]);
                                xNode.intersectionConstruction.iBLane2R.Add(rVect_Prev);

                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                xNode.intersectionConstruction.iBLane1L.Add(tVect_Prev);
                                xNode.intersectionConstruction.iBLane1R.Add(rVect_Prev);
                            }

                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                xNode.intersectionConstruction.shoulderStartBL = xNode.intersectionConstruction.iBLane0L[0];
                                xNode.intersectionConstruction.shoulderStartBR = xNode.intersectionConstruction.iBLane3R[0];
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                xNode.intersectionConstruction.shoulderStartBL = xNode.intersectionConstruction.iBLane0L[0];
                                xNode.intersectionConstruction.shoulderStartBR = xNode.intersectionConstruction.iBLane2R[0];
                            }
                            else
                            {
                                xNode.intersectionConstruction.shoulderStartBL = xNode.intersectionConstruction.iBLane0L[0];
                                xNode.intersectionConstruction.shoulderStartBR = xNode.intersectionConstruction.iBLane1R[0];
                            }

                            xNode.intersectionConstruction.shoulderBLStartIndex = _road.RCS.ShoulderL_Vectors.Count - 2;
                            xNode.intersectionConstruction.shoulderBRStartIndex = _road.RCS.ShoulderR_Vectors.Count - 2;
                            //goto InterSkip;
                        }

                        isLine = false;
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            isLine = !intersection.ContainsLine(tempIVect, (tVect + new Vector3((laneWidth * 0.5f) * -POS.normalized.z, 0, (laneWidth * 0.5f) * POS.normalized.x)));
                        }
                        else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            isLine = !intersection.ContainsLine(tempIVect, (tVect + new Vector3((laneWidth * 0.5f) * -POS.normalized.z, 0, (laneWidth * 0.5f) * POS.normalized.x)));
                        }
                        else if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            isLine = !intersection.ContainsLine(leftVector, tVect);
                        }
                        if (!xNode.intersectionConstruction.isBLane0Done && !intersection.Contains(ref tempIVect) && isLine)
                        {
                            xNode.intersectionConstruction.iBLane0L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            is0LAdded = true;
                        }
                        else if (!xNode.intersectionConstruction.isBLane0DoneFinal)
                        {
                            //Finalize lane 0:
                            InterFinalizeiBLane0(ref xNode, ref intersection, ref tIntHeight, isLRtoRR, isLLtoLR, isFirstInterNode);
                        }

                        //Line 1:
                        if (xNode.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            if (xNode.intersectionConstruction.iBLane0L.Count == 2)
                            {
                                tempIVect = (tVect + new Vector3((laneWidth * 0.5f) * -POS.normalized.z, 0, (laneWidth * 0.5f) * POS.normalized.x));
                                xNode.intersectionConstruction.iBLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                            }
                        }
                        tempIVect_Prev = tempIVect;
                        tempIVect = (tVect + new Vector3((laneWidth * 0.5f) * -POS.normalized.z, 0, (laneWidth * 0.5f) * POS.normalized.x));
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = tVect;
                        }
                        isLine = false;
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            isLine = !intersection.ContainsLine(tempIVect, (tVect + new Vector3((laneWidth * 0.5f) * POS.normalized.z, 0, (laneWidth * 0.5f) * -POS.normalized.x)));
                        }
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            isLine = !intersection.ContainsLine(tempIVect, rightVector);
                        }
                        else
                        {
                            isLine = !intersection.ContainsLine(tempIVect, rightVector);
                        }
                        tempIVect_Prev = tempIVect;
                        if (is0LAdded && !xNode.intersectionConstruction.isBLane1Done && !intersection.Contains(ref tempIVect) && isLine)
                        {
                            if (is0LAdded && (xNode.intersectionConstruction.iBLane0L.Count != 2 || intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane))
                            {
                                xNode.intersectionConstruction.iBLane0R.Add(ReplaceHeight(tempIVect, tIntHeight));
                            }
                            xNode.intersectionConstruction.iBLane1L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            is1LAdded = true;
                        }
                        else if (!xNode.intersectionConstruction.isBLane1DoneFinal)
                        {
                            //Finalize lane 1:
                            InterFinalizeiBLane1(ref xNode, ref intersection, ref tIntHeight, isLRtoRR, isLLtoLR, isFirstInterNode, ref is0LAdded, ref is1RAdded);
                        }

                        //Line 2:
                        if (xNode.intersectionConstruction.iBLane1R.Count == 0 && xNode.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            xNode.intersectionConstruction.iBLane1R.Add(ReplaceHeight(tVect, tIntHeight));
                            is1RAdded = true;
                            xNode.intersectionConstruction.iBLane2L.Add(ReplaceHeight(tVect, tIntHeight));
                            is2LAdded = true;
                            is2LAdded = true;
                        }
                        else
                        {
                            tempIVect = (tVect + new Vector3((laneWidth * 0.5f) * POS.normalized.z, 0, (laneWidth * 0.5f) * -POS.normalized.x));
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                tempIVect = rightVector;
                            }
                            if (is1LAdded)
                            {
                                isLine = !intersection.ContainsLine(tempIVect, tempIVect_Prev);
                            }
                            else
                            {
                                isLine = !intersection.ContainsLine(tempIVect, rightVector);
                            }
                            if (!xNode.intersectionConstruction.isBLane2Done && !intersection.Contains(ref tempIVect) && isLine)
                            {
                                if (is1LAdded)
                                {
                                    xNode.intersectionConstruction.iBLane1R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    is1RAdded = true;
                                }
                                xNode.intersectionConstruction.iBLane2L.Add(ReplaceHeight(tempIVect, tIntHeight));
                                is2LAdded = true;
                            }
                            else if (!xNode.intersectionConstruction.isBLane2DoneFinal)
                            {
                                InterFinalizeiBLane2(ref xNode, ref intersection, ref tIntHeight, isLRtoRR, isLLtoLR, isFirstInterNode, ref is2LAdded, ref is1LAdded, ref is0LAdded, ref is1RAdded);
                            }
                        }

                        //Line 3 / 4:
                        tempIVect = (tVect + new Vector3(((laneWidth * 0.5f) + roadSeperation) * POS.normalized.z, 0, ((laneWidth * 0.5f) + roadSeperation) * -POS.normalized.x));
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            tempIVect = rightVector;
                        }
                        if (!xNode.intersectionConstruction.isBLane3Done && !intersection.ContainsLine(rightVector, tempIVect) && !intersection.ContainsLine(rightVector, leftVector))
                        {
                            xNode.intersectionConstruction.iBLane3L.Add(ReplaceHeight(tempIVect, tIntHeight));
                            xNode.intersectionConstruction.iBLane3R.Add(ReplaceHeight(rightVector, tIntHeight));
                            is3RAdded = true;
                            if (!isFirst123 && intersection.intersectionType == RoadIntersection.IntersectionTypeEnum.FourWay)
                            {
                                if (is2LAdded)
                                {
                                    xNode.intersectionConstruction.iBLane2R.Add(ReplaceHeight(tempIVect, tIntHeight));
                                    is2RAdded = true;
                                }
                            }
                        }
                        else if (!xNode.intersectionConstruction.isBLane3DoneFinal)
                        {
                            InterFinalizeiBLane3(ref xNode, ref intersection, ref tIntHeight, isLRtoRR, isLLtoLR, isFirstInterNode, ref is2LAdded, ref is1LAdded, ref is0LAdded, ref is1RAdded);
                        }

                    }
                }

                //InterSkip:

                if (!isBridge)
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

                    leftVector += gHeight;
                    rightVector += gHeight;
                    ShoulderR_lVect += gHeight;
                    ShoulderL_rVect += gHeight;
                    ShoulderL_lVect += gHeight;
                    ShoulderR_rVect += gHeight;
                    heightAdded = gHeight.y;
                }


                if (tIntStrength >= 1f)
                {
                    tVect.y -= tInterSubtract;
                    tLastInterHeight = tVect.y;
                    rightVector.y -= tInterSubtract;
                    leftVector.y -= tInterSubtract;

                    ShoulderL_rVect.y = tIntHeight;
                    ShoulderR_lVect.y = tIntHeight;
                    ShoulderR_rVect.y = tIntHeight;
                    ShoulderL_lVect.y = tIntHeight;

                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderL_rVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  ShoulderL_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_rVect.y);
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderR_lVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  ShoulderR_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_lVect.y);
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderR_rVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  ShoulderR_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_rVect.y);
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderL_lVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  ShoulderL_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_lVect.y);
                    //}
                }
                else if (tIntStrength > 0f)
                {

                    rightVector.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * rightVector.y);
                    ShoulderR_lVect = rightVector;
                    leftVector.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * leftVector.y);
                    ShoulderL_rVect = leftVector;
                    ShoulderR_rVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * ShoulderR_rVect.y);
                    ShoulderL_lVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * ShoulderL_lVect.y);

                    //if(!Mathf.Approximately(tIntStrength,0f))
                    //{
                    //  tVect.y = (tIntStrength*tIntHeight) + ((1-tIntStrength)*tVect.y);
                    //}
                    //
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref rVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*rVect.y);
                    //  ShoulderR_lVect = rVect;
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref lVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*lVect.y);
                    //  ShoulderL_rVect = lVect;
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderR_rVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //  ShoulderR_rVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderR_rVect.y);
                    //}
                    //					
                    //tIntStrength_temp = tRoad.spline.IntersectionStrength(ref ShoulderL_lVect,ref tIntHeight, ref roadIntersection,ref bIsPastInter,ref i, ref xNode);
                    //if(!Mathf.Approximately(tIntStrength_temp,0f))
                    //{
                    //ShoulderL_lVect.y = (tIntStrength_temp*tIntHeight) + ((1-tIntStrength_temp)*ShoulderL_lVect.y);
                    //}
                }

                #region "Ramp:"
                RampR_L = ShoulderR_rVect;
                RampL_R = ShoulderL_lVect;
                if (isBridge)
                {
                    RampR_R = RampR_L;
                    RampL_L = RampL_R;
                }
                else
                {
                    RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x)) + gHeight;
                    SetVectorHeight2(ref RampR_R, ref i, ref spline.HeightHistory, ref spline);
                    RampR_R.y -= _road.desiredRampHeight;

                    RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x)) + gHeight;
                    SetVectorHeight2(ref RampL_L, ref i, ref spline.HeightHistory, ref spline);
                    RampL_L.y -= _road.desiredRampHeight;
                }
                #endregion

                //Merge points to intersection corners if necessary:
                if (isMaxIntersection && !isBridge && !isTunnel && isInterseOn)
                {
                    mCornerDist = _road.roadDefinition * 1.35f;
                    mCornerDist *= mCornerDist;

                    CornerRR = new Vector2(intersection.cornerRR.x, intersection.cornerRR.z);
                    CornerRL = new Vector2(intersection.cornerRL.x, intersection.cornerRL.z);
                    CornerLR = new Vector2(intersection.cornerLR.x, intersection.cornerLR.z);
                    CornerLL = new Vector2(intersection.cornerLL.x, intersection.cornerLL.z);
                    rVect2D = new Vector2(rightVector.x, rightVector.z);
                    lVect2D = new Vector2(leftVector.x, leftVector.z);
                    isOverridenRampR = false;
                    isOverridenRampL = false;
                    isImmuneR = false;
                    isImmuneL = false;
                    bMod1 = 1.75f;
                    bMod2 = 1.25f;
                    t2DDist = -1f;

                    //Find equatable lane vect and move it too
                    //					eCount = -1;
                    //					eIndex = -1;
                    //					uCount = -1;
                    //					uIndex = -1;

                    xHeight = new Vector3(0f, -0.1f, 0f);
                    isSpecAddedL = false;
                    isSpecAddedR = false;

                    if (isFirstInterNode)
                    {
                        isSpecAddedL = (is0LAdded || isf0LAdded);
                        if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                        {
                            isSpecAddedR = (is1RAdded || isf1LAdded);
                        }
                        else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                        {
                            isSpecAddedR = (is2RAdded || isf2LAdded);
                        }
                        else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            isSpecAddedR = (is3RAdded || isf3LAdded);
                        }
                    }

                    float tempRoadDef = Mathf.Clamp(_road.laneWidth, 3f, 5f);

                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {

                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {

                    }
                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {

                    }

                    //RR:
                    if (intersection.evenAngle > 90f)
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
                        isImmuneR = true;
                        isInterCurrentIsCorner = true;
                        isInterCurrentIsCornerRR = true;

                        if (isFirstInterNode)
                        {
                            vList = null;
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane1R;
                                if (xNode.intersectionConstruction.isBLane1DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane2R;
                                if (xNode.intersectionConstruction.isBLane2DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.intersectionConstruction.iBLane3R;
                                if (xNode.intersectionConstruction.isBLane3DoneFinalThisRound)
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
                                        if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRR.x, 0.01f) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRR.z, 0.01f)))
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
                            vList = xNode.intersectionConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRR.x, 0.01f) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRR.z, 0.01f)))
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
                        ShoulderR_rVect = new Vector3(intersection.cornerRROuter.x, tIntHeight, intersection.cornerRROuter.z);
                        RampR_Override = new Vector3(intersection.cornerRRRampOuter.x, tIntHeight, intersection.cornerRRRampOuter.z);
                        isRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerRR - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            isImmuneL = true;
                            isInterCurrentIsCorner = true;
                            isInterCurrentIsCornerRR = true;

                            //2nd node can come in via left
                            if (!isFirstInterNode)
                            {
                                vList = null;
                                vList = xNode.intersectionConstruction.iBLane0L;
                                if (xNode.intersectionConstruction.isBLane0DoneFinalThisRound)
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
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRR.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRR.z)))
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
                            ShoulderL_lVect = new Vector3(intersection.cornerRROuter.x, tIntHeight, intersection.cornerRROuter.z);
                            RampL_Override = new Vector3(intersection.cornerRRRampOuter.x, tIntHeight, intersection.cornerRRRampOuter.z);
                            isRecordShoulderLForNormals = true;
                        }
                    }
                    //RL:
                    if (intersection.oddAngle > 90f)
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
                        isImmuneR = true;
                        isInterCurrentIsCorner = true;
                        isInterCurreIsCornerRL = true;

                        if (isFirstInterNode)
                        {
                            vList = null;
                            vList = xNode.intersectionConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRL.z)))
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
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane1R;
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane2R;
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.intersectionConstruction.iBLane3R;
                            }

                            //Hitting RL from backside with second node:
                            if (!isFirstInterNode)
                            {
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 0; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                        {
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRL.z)))
                                            {
                                                eList.Add(m);
                                                if (m == vList.Count - 1)
                                                {
                                                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                                    {
                                                        is1RAdded = false;
                                                    }
                                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                                    {
                                                        is2RAdded = false;
                                                    }
                                                    else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                                    {
                                                        is3RAdded = false;
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
                        ShoulderR_rVect = new Vector3(intersection.cornerRLOuter.x, tIntHeight, intersection.cornerRLOuter.z);
                        RampR_Override = new Vector3(intersection.cornerRLRampOuter.x, tIntHeight, intersection.cornerRLRampOuter.z);
                        isRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerRL - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            isImmuneL = true;
                            isInterCurrentIsCorner = true;
                            isInterCurreIsCornerRL = true;

                            if (!isFirstInterNode)
                            {
                                vList = null;
                                if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane1R;
                                    if (xNode.intersectionConstruction.isFLane1DoneFinalThisRound)
                                    {
                                        vList = null;
                                    }
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane2R;
                                    if (xNode.intersectionConstruction.isFLane2DoneFinalThisRound)
                                    {
                                        vList = null;
                                    }
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.intersectionConstruction.iFLane3R;
                                    if (xNode.intersectionConstruction.isFLane3DoneFinalThisRound)
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
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerRL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerRL.z)))
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
                            ShoulderL_lVect = new Vector3(intersection.cornerRLOuter.x, tIntHeight, intersection.cornerRLOuter.z);
                            RampL_Override = new Vector3(intersection.cornerRLRampOuter.x, tIntHeight, intersection.cornerRLRampOuter.z);
                            isRecordShoulderLForNormals = true;
                        }
                    }
                    //LR:
                    if (intersection.oddAngle > 90f)
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
                        isImmuneR = true;
                        isInterCurrentIsCorner = true;
                        isInterCurreIsCornerLR = true;

                        if (!isFirstInterNode)
                        {
                            vList = null;
                            if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane1R;
                                if (xNode.intersectionConstruction.isBLane1DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                vList = xNode.intersectionConstruction.iBLane2R;
                                if (xNode.intersectionConstruction.isBLane2DoneFinalThisRound)
                                {
                                    vList = null;
                                }
                            }
                            else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                vList = xNode.intersectionConstruction.iBLane3R;
                                if (xNode.intersectionConstruction.isBLane3DoneFinalThisRound)
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
                                        if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLR.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLR.z)))
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
                        ShoulderR_rVect = new Vector3(intersection.cornerLROuter.x, tIntHeight, intersection.cornerLROuter.z);
                        RampR_Override = new Vector3(intersection.cornerLRRampOuter.x, tIntHeight, intersection.cornerLRRampOuter.z);
                        isRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerLR - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            isImmuneL = true;
                            isInterCurrentIsCorner = true;
                            isInterCurreIsCornerLR = true;

                            if (isFirstInterNode)
                            {
                                vList = null;
                                vList = xNode.intersectionConstruction.iBLane0L;
                                if (xNode.intersectionConstruction.isBLane0DoneFinalThisRound)
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
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLR.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLR.z)))
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
                                if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane1R;
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane2R;
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.intersectionConstruction.iFLane3R;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 1; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLR.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLR.z)))
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
                            ShoulderL_lVect = new Vector3(intersection.cornerLROuter.x, tIntHeight, intersection.cornerLROuter.z);
                            RampL_Override = new Vector3(intersection.cornerLRRampOuter.x, tIntHeight, intersection.cornerLRRampOuter.z);
                            isRecordShoulderLForNormals = true;
                        }
                    }
                    //LL:
                    if (intersection.evenAngle > 90f)
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
                        isImmuneR = true;
                        isInterCurrentIsCorner = true;
                        isInterCurreIsCornerLL = true;


                        if (!isFirstInterNode)
                        {
                            vList = null;
                            vList = xNode.intersectionConstruction.iFLane0L;
                            eList = new List<int>();
                            if (vList != null)
                            {
                                for (int m = 1; m < vList.Count; m++)
                                {
                                    if (Vector3.SqrMagnitude(vList[m] - ShoulderR_lVect) < 0.01f)
                                    {
                                        if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLL.z)))
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
                        ShoulderR_rVect = new Vector3(intersection.cornerLLOuter.x, tIntHeight, intersection.cornerLLOuter.z);
                        RampR_Override = new Vector3(intersection.cornerLLRampOuter.x, tIntHeight, intersection.cornerLLRampOuter.z);
                        isRecordShoulderForNormals = true;
                    }
                    else
                    {
                        t2DDist = Vector2.SqrMagnitude(CornerLL - lVect2D);
                        if (t2DDist < mCornerDist)
                        {
                            isImmuneL = true;
                            isInterCurrentIsCorner = true;
                            isInterCurreIsCornerLL = true;

                            if (isFirstInterNode)
                            {
                                vList = null;
                                if (intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane1R;
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                                {
                                    vList = xNode.intersectionConstruction.iFLane2R;
                                }
                                else if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                                {
                                    vList = xNode.intersectionConstruction.iFLane3R;
                                }
                                eList = new List<int>();
                                if (vList != null)
                                {
                                    for (int m = 1; m < vList.Count; m++)
                                    {
                                        if (Vector3.SqrMagnitude(vList[m] - ShoulderL_rVect) < 0.01f)
                                        {
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLL.z)))
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
                                vList = xNode.intersectionConstruction.iBLane0L;
                                if (xNode.intersectionConstruction.isBLane0DoneFinalThisRound)
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
                                            if (!(RootUtils.IsApproximately(vList[m].x, intersection.cornerLL.x) && RootUtils.IsApproximately(vList[m].z, intersection.cornerLL.z)))
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
                            ShoulderL_lVect = new Vector3(intersection.cornerLLOuter.x, tIntHeight, intersection.cornerLLOuter.z);
                            RampL_Override = new Vector3(intersection.cornerLLRampOuter.x, tIntHeight, intersection.cornerLLRampOuter.z);
                            isRecordShoulderLForNormals = true;
                        }
                    }

                    if (isImmuneR)
                    {
                        isOverridenRampR = true;
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderR_lVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderR_lVect);
                        }
                        if (!_road.RCS.ImmuneVects.Contains(ShoulderR_rVect))
                        {
                            _road.RCS.ImmuneVects.Add(ShoulderR_rVect);
                        }
                    }
                    if (isImmuneL)
                    {
                        isOverridenRampL = true;
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

                if (isShrinkRoadB)
                {

                    if (lVect_Prev != new Vector3(0f, 0f, 0f))
                    {
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                        _road.RCS.RoadVectors.Add(lVect_Prev);
                    }
                }
                if (isShrinkRoadF)
                {
                    if (leftVector != new Vector3(0f, 0f, 0f))
                    {
                        _road.RCS.RoadVectors.Add(leftVector);
                        _road.RCS.RoadVectors.Add(leftVector);
                        _road.RCS.RoadVectors.Add(leftVector);
                        _road.RCS.RoadVectors.Add(leftVector);
                    }
                }

                _road.RCS.RoadVectors.Add(leftVector);
                _road.RCS.RoadVectors.Add(leftVector);
                _road.RCS.RoadVectors.Add(rightVector);
                _road.RCS.RoadVectors.Add(rightVector);



                //Add bounds for later removal:
                if (!isBridge && !isTunnel && isMaxIntersection && isWasPrevMaxInter && isInterseOn)
                {
                    bool isGoAhead = true;
                    if (xNode.isEndPoint)
                    {
                        if (xNode.idOnSpline == 1)
                        {
                            if (i < xNode.time)
                            {
                                isGoAhead = false;
                            }
                        }
                        else
                        {
                            if (i > xNode.time)
                            {
                                isGoAhead = false;
                            }
                        }
                    }

                    //Get this and prev leftVect rightVect rects:
                    if ((Vector3.SqrMagnitude(xNode.pos - tVect) < CullDistanceSQ) && isGoAhead)
                    {
                        Construction2DRect vRect = new Construction2DRect(
                            new Vector2(leftVector.x, leftVector.z),
                            new Vector2(rightVector.x, rightVector.z),
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
                if (isBridge)
                {
                    RampR_R = RampR_L;
                    RampL_L = RampL_R;
                }
                else
                {
                    RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x)) + gHeight;
                    if (isOverridenRampR)
                    {
                        RampR_R = RampR_Override;
                    }   //Overrides will come from intersection.
                    SetVectorHeight2(ref RampR_R, ref i, ref spline.HeightHistory, ref spline);
                    RampR_R.y -= _road.desiredRampHeight;

                    RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x)) + gHeight;
                    if (isOverridenRampL)
                    {
                        RampL_L = RampL_Override;
                    }   //Overrides will come from intersection.
                    SetVectorHeight2(ref RampL_L, ref i, ref spline.HeightHistory, ref spline);
                    RampL_L.y -= _road.desiredRampHeight;
                    isOverridenRampR = false;
                    isOverridenRampL = false;
                }

                //If necessary during intersection construction, sometimes an addition will be created inbetween intersection corner points.
                //This addition will create a dip between corner points to 100% ensure there is no shoulder visible on the roads between corner points.
                isTriggerInterAddition = false;
                if (isMaxIntersection && isInterseOn)
                {
                    if (isFirstInterNode)
                    {
                        if ((isInterPrevWasCornerLR && isInterCurreIsCornerLL) || (isInterPrevWasCornerRR && isInterCurreIsCornerRL))
                        {
                            isTriggerInterAddition = true;
                        }
                    }
                    else
                    {
                        if (!intersection.isFlipped)
                        {
                            if ((isInterPrevWasCornerLL && isInterCurreIsCornerRL) || (isInterPrevWasCornerLR && isInterCurrentIsCornerRR) || (isInterPrevWasCornerRR && isInterCurreIsCornerLR))
                            {
                                isTriggerInterAddition = true;
                            }
                        }
                        else
                        {
                            if ((isInterPrevWasCornerRR && isInterCurreIsCornerLR) || (isInterPrevWasCornerLR && isInterCurrentIsCornerRR) || (isInterPrevWasCornerRL && isInterCurreIsCornerLL))
                            {
                                isTriggerInterAddition = true;
                            }
                        }
                    }

                    if (intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        isTriggerInterAddition = false;
                    }

                    //For 3-way intersections:
                    isSpecialThreeWayIgnoreR = false;
                    isSpecialThreeWayIgnoreL = false;
                    if (intersection.ignoreSide > -1)
                    {
                        if (intersection.ignoreSide == 0)
                        {
                            //RR to RL:
                            if (isFirstInterNode && (isInterPrevWasCornerRR && isInterCurreIsCornerRL))
                            {
                                isTriggerInterAddition = false;
                            }
                        }
                        else if (intersection.ignoreSide == 1)
                        {
                            //RL to LL:
                            if (!isFirstInterNode && ((isInterPrevWasCornerRL && isInterCurreIsCornerLL) || (isInterPrevWasCornerLL && isInterCurreIsCornerRL)))
                            {
                                //bTriggerInterAddition = false;	
                                if (intersection.isFlipped)
                                {
                                    isSpecialThreeWayIgnoreR = true;
                                }
                                else
                                {
                                    isSpecialThreeWayIgnoreL = true;
                                }
                            }
                        }
                        else if (intersection.ignoreSide == 2)
                        {
                            //LL to LR:
                            if (isFirstInterNode && (isInterPrevWasCornerLR && isInterCurreIsCornerLL))
                            {
                                isTriggerInterAddition = false;
                            }
                        }
                        else if (intersection.ignoreSide == 3)
                        {
                            //LR to RR:
                            if (!isFirstInterNode && ((isInterPrevWasCornerRR && isInterCurreIsCornerLR) || (isInterPrevWasCornerLR && isInterCurrentIsCornerRR)))
                            {
                                //bTriggerInterAddition = false;	
                                if (intersection.isFlipped)
                                {
                                    isSpecialThreeWayIgnoreL = true;
                                }
                                else
                                {
                                    isSpecialThreeWayIgnoreR = true;
                                }
                            }
                        }
                    }

                    if (isTriggerInterAddition)
                    {
                        iTemp_HeightVect = new Vector3(0f, 0f, 0f);
                        rVect_iTemp = (((rVect_Prev - rightVector) * 0.5f) + rightVector) + iTemp_HeightVect;
                        lVect_iTemp = (((lVect_Prev - leftVector) * 0.5f) + leftVector) + iTemp_HeightVect;
                        ShoulderR_R_iTemp = (((ShoulderR_PrevRVect - ShoulderR_rVect) * 0.5f) + ShoulderR_rVect) + iTemp_HeightVect;
                        ShoulderL_L_iTemp = (((ShoulderL_PrevLVect - ShoulderL_lVect) * 0.5f) + ShoulderL_lVect) + iTemp_HeightVect;
                        RampR_R_iTemp = (((RampR_PrevR - RampR_R) * 0.5f) + RampR_R) + iTemp_HeightVect;
                        RampR_L_iTemp = (((RampR_PrevL - RampR_L) * 0.5f) + RampR_L) + iTemp_HeightVect;
                        RampL_R_iTemp = (((RampL_PrevR - RampL_R) * 0.5f) + RampL_R) + iTemp_HeightVect;
                        RampL_L_iTemp = (((RampL_PrevL - RampL_L) * 0.5f) + RampL_L) + iTemp_HeightVect;

                        //ShoulderL_L_iTemp = lVect_iTemp;
                        //RampL_R_iTemp = lVect_iTemp;
                        //RampL_L_iTemp = lVect_iTemp;
                        //					
                        //ShoulderR_R_iTemp = rVect_iTemp;
                        //RampR_R_iTemp = rVect_iTemp;
                        //RampR_L_iTemp = rVect_iTemp;
                    }

                    if (isTriggerInterAddition && !(intersection.isFlipped && !isFirstInterNode))
                    {
                        if (isFirstInterNode)
                        {
                            if ((isInterPrevWasCornerRR && isInterCurreIsCornerRL && !isSpecialThreeWayIgnoreR))
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
                            if ((isInterPrevWasCornerLR && isInterCurreIsCornerLL && !isSpecialThreeWayIgnoreL))
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
                            if ((isInterPrevWasCornerLR && isInterCurrentIsCornerRR && !isSpecialThreeWayIgnoreR))
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
                            if ((isInterPrevWasCornerLL && isInterCurreIsCornerRL && !isSpecialThreeWayIgnoreL))
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
                    else if (isTriggerInterAddition && (intersection.isFlipped && !isFirstInterNode))
                    {
                        if ((isInterPrevWasCornerRR && isInterCurreIsCornerLR && !isSpecialThreeWayIgnoreL))
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
                        if ((isInterPrevWasCornerRL && isInterCurreIsCornerLL && !isSpecialThreeWayIgnoreR))
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
                if (!isShoulderSkipR)
                {
                    if (isRecordShoulderForNormals)
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
                    if (isImmuneR && isRecordShoulderForNormals)
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
                if (!isShoulderSkipL)
                {
                    if (isRecordShoulderLForNormals)
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
                    if (isImmuneL && isRecordShoulderForNormals)
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
                rVect_Prev = rightVector;
                lVect_Prev = leftVector;
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

                //Store more prev variables:
                isWasPrevMaxInter = isMaxIntersection;
                isInterPrevWasCorner = isInterCurrentIsCorner;
                isInterPrevWasCornerRR = isInterCurrentIsCornerRR;
                isInterPrevWasCornerRL = isInterCurreIsCornerRL;
                isInterPrevWasCornerLL = isInterCurreIsCornerLL;
                isInterPrevWasCornerLR = isInterCurreIsCornerLR;

                //i+=Step;//Master step incrementer.
            }

            RootUtils.EndStartProfiling(_road, "RoadJob_Prelim_FinalizeInter");

            //Finalize intersection vectors:
            if (isInterseOn)
            {
                RoadJobPrelimFinalizeInter(ref _road);
            }

            RootUtils.EndStartProfiling(_road, "RoadJob_Prelim_RoadConnections");

            //Creates road connections if necessary:
            //float ExtraHeight = 0f;
            //float RampPercent = 0.2f;
            if (spline.isSpecialEndNodeIsStartDelay)
            {
                Vector3[] RoadConn_verts = new Vector3[4];

                RampR_R = _road.RCS.ShoulderR_Vectors[7];
                ShoulderR_rVect = _road.RCS.ShoulderR_Vectors[3];
                rightVector = _road.RCS.ShoulderR_Vectors[0];

                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R);
                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Insert(0, rightVector);
                _road.RCS.ShoulderR_Vectors.Insert(0, rightVector);

                RampL_L = _road.RCS.ShoulderL_Vectors[4];
                ShoulderL_lVect = _road.RCS.ShoulderL_Vectors[0];
                leftVector = _road.RCS.ShoulderL_Vectors[3];

                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L);
                _road.RCS.ShoulderL_Vectors.Insert(0, leftVector);
                _road.RCS.ShoulderL_Vectors.Insert(0, leftVector);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect);

                RoadConn_verts[0] = leftVector;
                RoadConn_verts[1] = rightVector;
                spline.GetSplineValueBoth(RoadConnection_StartMin1, out tVect, out POS);
                roadSeperation = spline.specialEndNodeDelayStartResult / 2f;
                rightVector = (tVect + new Vector3(roadSeperation * POS.normalized.z, 0, roadSeperation * -POS.normalized.x));
                leftVector = (tVect + new Vector3(roadSeperation * -POS.normalized.z, 0, roadSeperation * POS.normalized.x));
                shoulderSeperation = roadSeperation + shoulderWidth;
                OuterShoulderWidthR = shoulderSeperation;
                OuterShoulderWidthL = shoulderSeperation;
                RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                ShoulderR_rVect = (tVect + new Vector3(shoulderSeperation * POS.normalized.z, 0, shoulderSeperation * -POS.normalized.x));
                ShoulderL_lVect = (tVect + new Vector3(shoulderSeperation * -POS.normalized.z, 0, shoulderSeperation * POS.normalized.x));
                RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x));
                SetVectorHeight2(ref RampR_R, ref i, ref spline.HeightHistory, ref spline);
                RampR_R.y -= (_road.desiredRampHeight + 0.10f);          // normal was 0.35f; Here was 0.45f
                RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x));
                SetVectorHeight2(ref RampL_L, ref i, ref spline.HeightHistory, ref spline);
                RampL_L.y -= (_road.desiredRampHeight + 0.10f);



                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, RampR_R + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, ShoulderR_rVect + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, rightVector + tHeight0);
                _road.RCS.ShoulderR_Vectors.Insert(0, rightVector + tHeight0);

                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, RampL_L + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, leftVector + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, leftVector + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);
                _road.RCS.ShoulderL_Vectors.Insert(0, ShoulderL_lVect + tHeight0);

                RoadConn_verts[2] = leftVector + tHeight0;
                RoadConn_verts[3] = rightVector + tHeight0;
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
                    tMod1 = 0.5f - (laneWidth / spline.specialEndNodeDelayStartResult);
                    tMod2 = 0.5f + (laneWidth / spline.specialEndNodeDelayStartResult);
                }
                else if (_road.laneAmount == 4)
                {
                    tMod1 = 0.5f - ((laneWidth * 2f) / spline.specialEndNodeDelayStartResult);
                    tMod2 = 0.5f + ((laneWidth * 2f) / spline.specialEndNodeDelayStartResult);
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
            else if (spline.isSpecialEndNodeIsEndDelay)
            {
                Vector3[] RoadConn_verts = new Vector3[4];
                int rrCount = _road.RCS.ShoulderR_Vectors.Count;
                RampR_R = _road.RCS.ShoulderR_Vectors[rrCount - 1];
                ShoulderR_rVect = _road.RCS.ShoulderR_Vectors[rrCount - 3];
                rightVector = _road.RCS.ShoulderR_Vectors[rrCount - 7];

                //Right shoulder:
                _road.RCS.ShoulderR_Vectors.Add(rightVector);
                _road.RCS.ShoulderR_Vectors.Add(rightVector);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);

                rrCount = _road.RCS.ShoulderL_Vectors.Count;
                RampL_L = _road.RCS.ShoulderL_Vectors[rrCount - 3];
                ShoulderL_lVect = _road.RCS.ShoulderL_Vectors[rrCount - 1];
                leftVector = _road.RCS.ShoulderL_Vectors[rrCount - 5];

                //Left shoulder:
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(leftVector);
                _road.RCS.ShoulderL_Vectors.Add(leftVector);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);

                RoadConn_verts[0] = leftVector;
                RoadConn_verts[1] = rightVector;
                spline.GetSplineValueBoth(RoadConnection_FinalMax1, out tVect, out POS);
                roadSeperation = spline.specialEndNodeDelayEndResult / 2f;
                rightVector = (tVect + new Vector3(roadSeperation * POS.normalized.z, 0, roadSeperation * -POS.normalized.x));
                leftVector = (tVect + new Vector3(roadSeperation * -POS.normalized.z, 0, roadSeperation * POS.normalized.x));
                shoulderSeperation = roadSeperation + shoulderWidth;
                OuterShoulderWidthR = shoulderSeperation;
                OuterShoulderWidthL = shoulderSeperation;
                RampOuterWidthR = (OuterShoulderWidthR / 4f) + OuterShoulderWidthR;
                RampOuterWidthL = (OuterShoulderWidthL / 4f) + OuterShoulderWidthL;
                ShoulderR_rVect = (tVect + new Vector3(shoulderSeperation * POS.normalized.z, 0, shoulderSeperation * -POS.normalized.x));
                ShoulderL_lVect = (tVect + new Vector3(shoulderSeperation * -POS.normalized.z, 0, shoulderSeperation * POS.normalized.x));
                RampR_R = (tVect + new Vector3(RampOuterWidthR * POS.normalized.z, 0, RampOuterWidthR * -POS.normalized.x));
                SetVectorHeight2(ref RampR_R, ref i, ref spline.HeightHistory, ref spline);
                RampR_R.y -= _road.desiredRampHeight;
                RampL_L = (tVect + new Vector3(RampOuterWidthL * -POS.normalized.z, 0, RampOuterWidthL * POS.normalized.x));
                SetVectorHeight2(ref RampL_L, ref i, ref spline.HeightHistory, ref spline);
                RampL_L.y -= _road.desiredRampHeight;

                //Right shoulder:
                _road.RCS.ShoulderR_Vectors.Add(rightVector);
                _road.RCS.ShoulderR_Vectors.Add(rightVector);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(ShoulderR_rVect);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);
                _road.RCS.ShoulderR_Vectors.Add(RampR_R);

                //Left shoulder:
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(leftVector);
                _road.RCS.ShoulderL_Vectors.Add(leftVector);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(RampL_L);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);
                _road.RCS.ShoulderL_Vectors.Add(ShoulderL_lVect);

                RoadConn_verts[2] = leftVector;
                RoadConn_verts[3] = rightVector;
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
                float tMod = (roadWidth / spline.specialEndNodeDelayEndResult) / 2f;
                RoadConn_uv[0] = new Vector2(tMod, 0f);
                RoadConn_uv[1] = new Vector2(tMod * 3f, 0f);
                RoadConn_uv[2] = new Vector2(0f, 1f);
                RoadConn_uv[3] = new Vector2(1f, 1f);
                _road.RCS.RoadConnections_verts.Add(RoadConn_verts);
                _road.RCS.RoadConnections_tris.Add(RoadConn_tris);
                _road.RCS.RoadConnections_normals.Add(RoadConn_normals);
                _road.RCS.RoadConnections_uv.Add(RoadConn_uv);
            }
            RootUtils.EndProfiling(_road);
        }


        #region "Road prelim helpers"
        /// <summary> Returns a new Vector3 with _v1.x, _height, _v1.z </summary>
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


        /// <summary> Returns a new Vector2 from _vect.x, _vect.z </summary>
        private static Vector2 ConvertVect3ToVect2(Vector3 _vect)
        {
            return new Vector2(_vect.x, _vect.z);
        }


        private static void InterFinalizeiBLane0(ref SplineN _node, ref RoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode)
        {
            if (_node.intersectionConstruction.isBLane0DoneFinal)
            {
                return;
            }

            _node.intersectionConstruction.isBLane0Done = true;
            if (_intersection.isFlipped && !_isFirstInterNode)
            {
                if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _node.intersectionConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[4], _intHeight));
                    _node.intersectionConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                }
                else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    _node.intersectionConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                    _node.intersectionConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                }
                else
                {
                    _node.intersectionConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                    _node.intersectionConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                }
            }
            else
            {
                if (_isLRtoRR)
                {
                    _node.intersectionConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[0], _intHeight));
                    _node.intersectionConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[1], _intHeight));
                }
                else if (_isLLtoLR)
                {
                    _node.intersectionConstruction.iBLane0L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[0], _intHeight));
                    _node.intersectionConstruction.iBLane0R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[1], _intHeight));
                }
            }
            _node.intersectionConstruction.isBLane0DoneFinal = true;
            _node.intersectionConstruction.isBLane0DoneFinalThisRound = true;
        }


        private static void InterFinalizeiBLane1(ref SplineN _node, ref RoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_node.intersectionConstruction.isBLane1DoneFinal)
            {
                return;
            }

            if (_is0LAdded && !_node.intersectionConstruction.isBLane0DoneFinal)
            {
                _node.intersectionConstruction.iBLane0L.RemoveAt(_node.intersectionConstruction.iBLane0L.Count - 1);
                _is0LAdded = false;
                InterFinalizeiBLane0(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode);
            }
            _node.intersectionConstruction.isBLane1Done = true;
            _node.intersectionConstruction.isBLane0Done = true;

            if (_intersection.isFlipped && !_isFirstInterNode)
            {
                if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _node.intersectionConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[3], _intHeight));
                    _node.intersectionConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                }
                else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    _node.intersectionConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                    _node.intersectionConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                }
                else
                {
                    _node.intersectionConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    _node.intersectionConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight)); //b1RAdded = true;
                }
            }
            else
            {
                if (_isLRtoRR)
                {
                    _node.intersectionConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[1], _intHeight));
                    _node.intersectionConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[2], _intHeight)); //b1RAdded = true;
                }
                else if (_isLLtoLR)
                {
                    _node.intersectionConstruction.iBLane1L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[1], _intHeight));
                    _node.intersectionConstruction.iBLane1R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[2], _intHeight)); //b1RAdded = true;
                }
            }
            _node.intersectionConstruction.isBLane1DoneFinal = true;
            _node.intersectionConstruction.isBLane1DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
            {
                _node.intersectionConstruction.isBackRRPassed = true;
            }
        }


        private static void InterFinalizeiBLane2(ref SplineN _node, ref RoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is2LAdded, ref bool _is1LAdded, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_node.intersectionConstruction.isBLane2DoneFinal)
            {
                return;
            }

            if (_is1LAdded && !_node.intersectionConstruction.isBLane1DoneFinal)
            {
                _node.intersectionConstruction.iBLane1L.RemoveAt(_node.intersectionConstruction.iBLane1L.Count - 1);
                _is1LAdded = false;
                InterFinalizeiBLane1(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode, ref _is0LAdded, ref _is1RAdded);
            }
            _node.intersectionConstruction.isBLane1Done = true;
            _node.intersectionConstruction.isBLane2Done = true;

            if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes || _intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
            {
                if (_intersection.isFlipped && !_isFirstInterNode)
                {
                    if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        _node.intersectionConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[2], _intHeight));
                        _node.intersectionConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    }
                    else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        _node.intersectionConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                        _node.intersectionConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight));
                    }
                }
                else
                {
                    if (_isLRtoRR)
                    {
                        _node.intersectionConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[2], _intHeight));
                        _node.intersectionConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[3], _intHeight));
                    }
                    else if (_isLLtoLR)
                    {
                        _node.intersectionConstruction.iBLane2L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[2], _intHeight));
                        _node.intersectionConstruction.iBLane2R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[3], _intHeight));
                    }
                }
            }
            _node.intersectionConstruction.isBLane2DoneFinal = true;
            _node.intersectionConstruction.isBLane2DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
            {
                _node.intersectionConstruction.isBackRRPassed = true;
            }
        }


        private static void InterFinalizeiBLane3(ref SplineN _node, ref RoadIntersection _intersection, ref float _intHeight, bool _isLRtoRR, bool _isLLtoLR, bool _isFirstInterNode, ref bool _is2LAdded, ref bool _is1LAdded, ref bool _is0LAdded, ref bool _is1RAdded)
        {
            if (_is2LAdded && !_node.intersectionConstruction.isBLane2DoneFinal)
            {
                _node.intersectionConstruction.iBLane2L.RemoveAt(_node.intersectionConstruction.iBLane2L.Count - 1);
                _is2LAdded = false;
                InterFinalizeiBLane2(ref _node, ref _intersection, ref _intHeight, _isLRtoRR, _isLLtoLR, _isFirstInterNode, ref _is2LAdded, ref _is1LAdded, ref _is0LAdded, ref _is1RAdded);
            }
            _node.intersectionConstruction.isBLane2Done = true;
            _node.intersectionConstruction.isBLane3Done = true;
            if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                if (_intersection.isFlipped && !_isFirstInterNode)
                {
                    _node.intersectionConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerRLCornerRR[1], _intHeight));
                    _node.intersectionConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerRLCornerRR[0], _intHeight));
                }
                else
                {
                    if (_isLRtoRR)
                    {
                        _node.intersectionConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerLRCornerRR[3], _intHeight));
                        _node.intersectionConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerLRCornerRR[4], _intHeight));
                    }
                    else if (_isLLtoLR)
                    {
                        _node.intersectionConstruction.iBLane3L.Add(ReplaceHeight(_intersection.cornerLLCornerLR[3], _intHeight));
                        _node.intersectionConstruction.iBLane3R.Add(ReplaceHeight(_intersection.cornerLLCornerLR[4], _intHeight));
                    }
                }
            }
            _node.intersectionConstruction.isBLane3DoneFinal = true;
            _node.intersectionConstruction.isBLane3DoneFinalThisRound = true;

            if (_isFirstInterNode && _intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                _node.intersectionConstruction.isBackRRPassed = true;
            }
        }
        #endregion
        #endregion


        #region "Intersection Prelim"
        private static void RoadJobPrelimInter(ref Road _road)
        {
            SplineC spline = _road.spline;
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

            //If left collides with left, etc

            //This will speed up later calculations for intersection 4 corner construction:
            int nodeCount = spline.GetNodeCount();
            float PreInter_RoadWidthMod = 4.5f;
            if (!isOldMethod)
            {
                PreInter_RoadWidthMod = 5.5f;
            }
            float preInterDistance = (spline.RoadWidth * PreInter_RoadWidthMod) / spline.distance;
            SplineN iNode = null;
            for (int j = 0; j < nodeCount; j++)
            {
                if (spline.nodes[j].isIntersection)
                {
                    iNode = spline.nodes[j];
                    //First node set min / max float:
                    if (iNode.intersectionConstruction == null)
                    {
                        iNode.intersectionConstruction = new iConstructionMaker();
                    }
                    if (!iNode.intersectionConstruction.isTempConstructionProcessedInter1)
                    {
                        preInterDistance = (iNode.spline.RoadWidth * PreInter_RoadWidthMod) / iNode.spline.distance;
                        iNode.intersectionConstruction.tempconstruction_InterStart = iNode.time - preInterDistance;
                        iNode.intersectionConstruction.tempconstruction_InterEnd = iNode.time + preInterDistance;
                       
                        iNode.intersectionConstruction.ClampConstructionValues();

                        iNode.intersectionConstruction.isTempConstructionProcessedInter1 = true;
                    }

                    if (string.Compare(iNode.uID, iNode.intersection.node1.uID) == 0)
                    {
                        iNode = iNode.intersection.node2;
                    }
                    else
                    {
                        iNode = iNode.intersection.node1;
                    }

                    //Grab other intersection node and set min / max float	
                    try
                    {
                        if (!iNode.intersectionConstruction.isTempConstructionProcessedInter1)
                        {
                            preInterDistance = (iNode.spline.RoadWidth * PreInter_RoadWidthMod) / iNode.spline.distance;
                            iNode.intersectionConstruction.tempconstruction_InterStart = iNode.time - preInterDistance;
                            iNode.intersectionConstruction.tempconstruction_InterEnd = iNode.time + preInterDistance;
                            
                            iNode.intersectionConstruction.ClampConstructionValues();

                            iNode.intersectionConstruction.isTempConstructionProcessedInter1 = true;
                        }
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            }

            //Now get the four points per intersection:
            SplineN oNode1 = null;
            SplineN oNode2 = null;
            float PreInterPrecision1 = -1f;
            float PreInterPrecision2 = -1f;
            Vector3 PreInterVect = default(Vector3);
            Vector3 PreInterVectR = default(Vector3);
            Vector3 PreInterVectR_RightTurn = default(Vector3);
            Vector3 PreInterVectL = default(Vector3);
            Vector3 PreInterVectL_RightTurn = default(Vector3);
            RoadIntersection roadIntersection = null;


            for (int j = 0; j < nodeCount; j++)
            {
                oNode1 = spline.nodes[j];
                if (oNode1.isIntersection)
                {
                    oNode1 = oNode1.intersection.node1;
                    oNode2 = oNode1.intersection.node2;
                    if (isOldMethod)
                    {
                        PreInterPrecision1 = 0.1f / oNode1.spline.distance;
                        PreInterPrecision2 = 0.1f / oNode2.spline.distance;
                    }
                    else
                    {
                        PreInterPrecision1 = 4f / oNode1.spline.distance;
                        PreInterPrecision2 = 4f / oNode2.spline.distance;
                    }
                    roadIntersection = oNode1.intersection;
                    try
                    {
                        if (oNode1.intersectionConstruction.isTempConstructionProcessedInter2 && oNode2.intersectionConstruction.isTempConstructionProcessedInter2)
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                    roadIntersection = oNode1.intersection;
                    roadIntersection.isCornerRR1Enabled = false;
                    roadIntersection.isCornerRR2Enabled = false;
                    roadIntersection.isCornerRL1Enabled = false;
                    roadIntersection.isCornerRL2Enabled = false;
                    roadIntersection.isCornerLR1Enabled = false;
                    roadIntersection.isCornerLR2Enabled = false;
                    roadIntersection.isCornerLL1Enabled = false;
                    roadIntersection.isCornerLL2Enabled = false;

                    if (!oNode1.intersectionConstruction.isTempConstructionProcessedInter2)
                    {
                        oNode1.intersectionConstruction.tempconstruction_R = new List<Vector2>();
                        oNode1.intersectionConstruction.tempconstruction_L = new List<Vector2>();
                        if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            oNode1.intersectionConstruction.tempconstruction_R_RightTurn = new List<Vector2>();
                            oNode1.intersectionConstruction.tempconstruction_L_RightTurn = new List<Vector2>();
                        }

                        for (float i = oNode1.intersectionConstruction.tempconstruction_InterStart; i < oNode1.intersectionConstruction.tempconstruction_InterEnd; i += PreInterPrecision1)
                        {
                            oNode1.spline.GetSplineValueBoth(i, out PreInterVect, out POS);

                            isPastInter = oNode1.spline.IntersectionIsPast(ref i, ref oNode1);
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                            else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                            }
                            else
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSeperationNoTurn * POS.normalized.z, 0, roadSeperationNoTurn * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSeperationNoTurn * -POS.normalized.z, 0, roadSeperationNoTurn * POS.normalized.x));
                            }

                            oNode1.intersectionConstruction.tempconstruction_R.Add(new Vector2(PreInterVectR.x, PreInterVectR.z));
                            oNode1.intersectionConstruction.tempconstruction_L.Add(new Vector2(PreInterVectL.x, PreInterVectL.z));

                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                PreInterVectR_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                oNode1.intersectionConstruction.tempconstruction_R_RightTurn.Add(ConvertVect3ToVect2(PreInterVectR_RightTurn));

                                PreInterVectL_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                oNode1.intersectionConstruction.tempconstruction_L_RightTurn.Add(ConvertVect3ToVect2(PreInterVectL_RightTurn));
                            }
                        }
                    }

                    //Process second node:
                    if (oNode2.intersectionConstruction == null)
                    {
                        oNode2.intersectionConstruction = new iConstructionMaker();
                    }
                    if (!oNode2.intersectionConstruction.isTempConstructionProcessedInter2)
                    {
                        oNode2.intersectionConstruction.tempconstruction_R = new List<Vector2>();
                        oNode2.intersectionConstruction.tempconstruction_L = new List<Vector2>();
                        if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                        {
                            oNode2.intersectionConstruction.tempconstruction_R_RightTurn = new List<Vector2>();
                            oNode2.intersectionConstruction.tempconstruction_L_RightTurn = new List<Vector2>();
                        }

                        for (float i = oNode2.intersectionConstruction.tempconstruction_InterStart; i < oNode2.intersectionConstruction.tempconstruction_InterEnd; i += PreInterPrecision2)
                        {
                            oNode2.spline.GetSplineValueBoth(i, out PreInterVect, out POS);

                            isPastInter = oNode2.spline.IntersectionIsPast(ref i, ref oNode2);
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                            else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSep1Lane * POS.normalized.z, 0, roadSep1Lane * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSep1Lane * -POS.normalized.z, 0, roadSep1Lane * POS.normalized.x));
                            }
                            else
                            {
                                PreInterVectR = (PreInterVect + new Vector3(roadSeperationNoTurn * POS.normalized.z, 0, roadSeperationNoTurn * -POS.normalized.x));
                                PreInterVectL = (PreInterVect + new Vector3(roadSeperationNoTurn * -POS.normalized.z, 0, roadSeperationNoTurn * POS.normalized.x));
                            }

                            oNode2.intersectionConstruction.tempconstruction_R.Add(new Vector2(PreInterVectR.x, PreInterVectR.z));
                            oNode2.intersectionConstruction.tempconstruction_L.Add(new Vector2(PreInterVectL.x, PreInterVectL.z));
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                PreInterVectR_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * POS.normalized.z, 0, roadSep2Lane * -POS.normalized.x));
                                oNode2.intersectionConstruction.tempconstruction_R_RightTurn.Add(ConvertVect3ToVect2(PreInterVectR_RightTurn));

                                PreInterVectL_RightTurn = (PreInterVect + new Vector3(roadSep2Lane * -POS.normalized.z, 0, roadSep2Lane * POS.normalized.x));
                                oNode2.intersectionConstruction.tempconstruction_L_RightTurn.Add(ConvertVect3ToVect2(PreInterVectL_RightTurn));
                            }
                        }
                    }



                    bool isFlipped = false;
                    bool isFlippedSet = false;
                    int hCount1 = oNode1.intersectionConstruction.tempconstruction_R.Count;
                    int hCount2 = oNode2.intersectionConstruction.tempconstruction_R.Count;
                    int N1RCount = oNode1.intersectionConstruction.tempconstruction_R.Count;
                    int N1LCount = oNode1.intersectionConstruction.tempconstruction_L.Count;
                    int N2RCount = oNode2.intersectionConstruction.tempconstruction_R.Count;
                    int N2LCount = oNode2.intersectionConstruction.tempconstruction_L.Count;

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
                                if (Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_R[k]) < _road.roadDefinition)
                                {
                                    isFlipped = false;
                                    isFlippedSet = true;
                                    break;
                                }
                            }
                            if (k < N2LCount)
                            {
                                if (Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_L[k]) < _road.roadDefinition)
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
                    oNode1.intersection.isFlipped = isFlipped;


                    //Three-way intersections lane specifics:
                    roadIntersection.isNode2BLeftTurnLane = true;
                    roadIntersection.isNode2BRightTurnLane = true;
                    roadIntersection.isNode2FLeftTurnLane = true;
                    roadIntersection.isNode2FRightTurnLane = true;

                    //Three-way intersections:
                    roadIntersection.ignoreSide = -1;
                    roadIntersection.ignoreCorner = -1;
                    roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.FourWay;
                    if (roadIntersection.isFirstSpecialFirst)
                    {
                        roadIntersection.ignoreSide = 3;
                        roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                        if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                        {
                            roadIntersection.ignoreCorner = 0;
                        }
                        else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                        {
                            roadIntersection.ignoreCorner = 1;
                        }

                        if (!oNode1.intersection.isFlipped)
                        {
                            roadIntersection.isNode2FLeftTurnLane = false;
                            roadIntersection.isNode2BRightTurnLane = false;
                        }
                        else
                        {
                            roadIntersection.isNode2BLeftTurnLane = false;
                            roadIntersection.isNode2FRightTurnLane = false;
                        }
                    }
                    else if (roadIntersection.isFirstSpecialLast)
                    {
                        roadIntersection.ignoreSide = 1;
                        roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                        if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                        {
                            roadIntersection.ignoreCorner = 2;
                        }
                        else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                        {
                            roadIntersection.ignoreCorner = 3;
                        }

                        if (!oNode1.intersection.isFlipped)
                        {
                            roadIntersection.isNode2BLeftTurnLane = false;
                            roadIntersection.isNode2FRightTurnLane = false;
                        }
                        else
                        {
                            roadIntersection.isNode2FLeftTurnLane = false;
                            roadIntersection.isNode2BRightTurnLane = false;
                        }

                    }
                    if (!isFlipped)
                    {
                        if (roadIntersection.isSecondSpecialFirst)
                        {
                            roadIntersection.ignoreSide = 2;
                            roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                roadIntersection.ignoreCorner = 3;
                            }
                            else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                roadIntersection.ignoreCorner = 0;
                            }

                            if (!oNode1.intersection.isFlipped)
                            {
                                roadIntersection.isNode2BLeftTurnLane = false;
                                roadIntersection.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                roadIntersection.isNode2FLeftTurnLane = false;
                                roadIntersection.isNode2BRightTurnLane = false;
                            }

                        }
                        else if (roadIntersection.isSecondSpecialLast)
                        {
                            roadIntersection.ignoreSide = 0;
                            roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                roadIntersection.ignoreCorner = 1;
                            }
                            else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                roadIntersection.ignoreCorner = 2;
                            }

                            if (!oNode1.intersection.isFlipped)
                            {
                                roadIntersection.isNode2BLeftTurnLane = false;
                                roadIntersection.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                roadIntersection.isNode2FLeftTurnLane = false;
                                roadIntersection.isNode2BRightTurnLane = false;
                            }

                        }
                    }
                    else
                    {
                        if (roadIntersection.isSecondSpecialFirst)
                        {
                            roadIntersection.ignoreSide = 0;
                            roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                roadIntersection.ignoreCorner = 1;
                            }
                            else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                roadIntersection.ignoreCorner = 2;
                            }

                            if (!oNode1.intersection.isFlipped)
                            {
                                roadIntersection.isNode2BLeftTurnLane = false;
                                roadIntersection.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                roadIntersection.isNode2FLeftTurnLane = false;
                                roadIntersection.isNode2BRightTurnLane = false;
                            }

                        }
                        else if (roadIntersection.isSecondSpecialLast)
                        {
                            roadIntersection.ignoreSide = 2;
                            roadIntersection.intersectionType = RoadIntersection.IntersectionTypeEnum.ThreeWay;
                            if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.StopSign_AllWay)
                            {
                                roadIntersection.ignoreCorner = 3;
                            }
                            else if (roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight1 || roadIntersection.intersectionStopType == RoadIntersection.iStopTypeEnum.TrafficLight2)
                            {
                                roadIntersection.ignoreCorner = 0;
                            }

                            if (!oNode1.intersection.isFlipped)
                            {
                                roadIntersection.isNode2BLeftTurnLane = false;
                                roadIntersection.isNode2FRightTurnLane = false;
                            }
                            else
                            {
                                roadIntersection.isNode2FLeftTurnLane = false;
                                roadIntersection.isNode2BRightTurnLane = false;
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
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R_RightTurn, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                            else
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                        }
                        else
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R_RightTurn, ref oNode2.intersectionConstruction.tempconstruction_L);
                            }
                            else
                            {
                                tFoundVectRR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_L);
                            }
                        }

                        //RL:
                        if (!isFlipped)
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_L_RightTurn);
                            }
                            else
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_L);
                            }
                        }
                        else
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_R_RightTurn);
                            }
                            else
                            {
                                tFoundVectRL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_R, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                        }

                        //LL:
                        if (!isFlipped)
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L_RightTurn, ref oNode2.intersectionConstruction.tempconstruction_L);
                            }
                            else
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_L);
                            }
                        }
                        else
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L_RightTurn, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                            else
                            {
                                tFoundVectLL = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                        }

                        //LR:
                        if (!isFlipped)
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_R_RightTurn);
                            }
                            else
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_R);
                            }
                        }
                        else
                        {
                            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_L_RightTurn);
                            }
                            else
                            {
                                tFoundVectLR = IntersectionCornerCalc(ref oNode1.intersectionConstruction.tempconstruction_L, ref oNode2.intersectionConstruction.tempconstruction_L);
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
                                        oDistanceRR = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_R[k]);
                                        if (oDistanceRR < eDistanceRR)
                                        {
                                            eDistanceRR = oDistanceRR;
                                            tFoundVectRR = oNode1.intersectionConstruction.tempconstruction_R[h]; //RR
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
                                        oDistanceRL = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_L[k]);
                                        if (oDistanceRL < eDistanceRL)
                                        {
                                            eDistanceRL = oDistanceRL;
                                            tFoundVectRL = oNode1.intersectionConstruction.tempconstruction_R[h]; //RL
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
                                        oDistanceLR = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_L[h], oNode2.intersectionConstruction.tempconstruction_R[k]);
                                        if (oDistanceLR < eDistanceLR)
                                        {
                                            eDistanceLR = oDistanceLR;
                                            tFoundVectLR = oNode1.intersectionConstruction.tempconstruction_L[h]; //LR
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
                                        oDistanceLL = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_L[h], oNode2.intersectionConstruction.tempconstruction_L[k]);
                                        if (oDistanceLL < eDistanceLL)
                                        {
                                            eDistanceLL = oDistanceLL;
                                            tFoundVectLL = oNode1.intersectionConstruction.tempconstruction_L[h]; //LL
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
                                        oDistanceRR = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_L[k]);
                                        if (oDistanceRR < eDistanceRR)
                                        {
                                            eDistanceRR = oDistanceRR;
                                            tFoundVectRR = oNode1.intersectionConstruction.tempconstruction_R[h]; //RR
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
                                        oDistanceRL = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_R[h], oNode2.intersectionConstruction.tempconstruction_R[k]);
                                        if (oDistanceRL < eDistanceRL)
                                        {
                                            eDistanceRL = oDistanceRL;
                                            tFoundVectRL = oNode1.intersectionConstruction.tempconstruction_R[h]; //RL
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
                                        oDistanceLR = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_L[h], oNode2.intersectionConstruction.tempconstruction_L[k]);
                                        if (oDistanceLR < eDistanceLR)
                                        {
                                            eDistanceLR = oDistanceLR;
                                            tFoundVectLR = oNode1.intersectionConstruction.tempconstruction_L[h]; //LR
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
                                        oDistanceLL = Vector2.Distance(oNode1.intersectionConstruction.tempconstruction_L[h], oNode2.intersectionConstruction.tempconstruction_R[k]);
                                        if (oDistanceLL < eDistanceLL)
                                        {
                                            eDistanceLL = oDistanceLL;
                                            tFoundVectLL = oNode1.intersectionConstruction.tempconstruction_L[h]; //LL
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

                    oNode1.intersectionConstruction.isTempConstructionProcessedInter2 = true;
                    oNode2.intersectionConstruction.isTempConstructionProcessedInter2 = true;

                    Vector3 tVectRR = new Vector3(tFoundVectRR.x, 0f, tFoundVectRR.y);
                    Vector3 tVectRL = new Vector3(tFoundVectRL.x, 0f, tFoundVectRL.y);
                    Vector3 tVectLR = new Vector3(tFoundVectLR.x, 0f, tFoundVectLR.y);
                    Vector3 tVectLL = new Vector3(tFoundVectLL.x, 0f, tFoundVectLL.y);

                    oNode1.intersection.cornerRR = tVectRR;
                    oNode1.intersection.cornerRL = tVectRL;
                    oNode1.intersection.cornerLR = tVectLR;
                    oNode1.intersection.cornerLL = tVectLL;

                    float[] tMaxFloats = new float[4];
                    tMaxFloats[0] = Vector3.Distance(((tVectRR - tVectRL) * 0.5f) + tVectRL, oNode1.pos) * 1.25f;
                    tMaxFloats[1] = Vector3.Distance(((tVectRR - tVectLR) * 0.5f) + tVectLR, oNode1.pos) * 1.25f;
                    tMaxFloats[2] = Vector3.Distance(((tVectRL - tVectLL) * 0.5f) + tVectLL, oNode1.pos) * 1.25f;
                    tMaxFloats[3] = Vector3.Distance(((tVectLR - tVectLL) * 0.5f) + tVectLL, oNode1.pos) * 1.25f;
                    roadIntersection.maxInterDistance = Mathf.Max(tMaxFloats);

                    float[] tMaxFloatsSQ = new float[4];
                    tMaxFloatsSQ[0] = Vector3.SqrMagnitude((((tVectRR - tVectRL) * 0.5f) + tVectRL) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[1] = Vector3.SqrMagnitude((((tVectRR - tVectLR) * 0.5f) + tVectLR) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[2] = Vector3.SqrMagnitude((((tVectRL - tVectLL) * 0.5f) + tVectLL) - oNode1.pos) * 1.25f;
                    tMaxFloatsSQ[3] = Vector3.SqrMagnitude((((tVectLR - tVectLL) * 0.5f) + tVectLL) - oNode1.pos) * 1.25f;
                    roadIntersection.maxInterDistanceSQ = Mathf.Max(tMaxFloatsSQ);

                    float TotalLanes = (int) (roadWidth / laneWidth);
                    float TotalLanesI = TotalLanes;
                    float LanesPerSide = TotalLanes / 2f;

                    if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {
                        TotalLanesI = TotalLanes + 2f;
                        //Lower left to lower right: 
                        roadIntersection.cornerLRCornerRR = new Vector3[5];
                        roadIntersection.cornerLRCornerRR[0] = tVectLR;
                        roadIntersection.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * (LanesPerSide / TotalLanesI)) + tVectLR;
                        roadIntersection.cornerLRCornerRR[2] = ((tVectRR - tVectLR) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLR;
                        roadIntersection.cornerLRCornerRR[3] = ((tVectRR - tVectLR) * ((LanesPerSide + 1 + LanesPerSide) / TotalLanesI)) + tVectLR;
                        roadIntersection.cornerLRCornerRR[4] = tVectRR;
                        //Upper right to lower right:
                        roadIntersection.cornerRLCornerRR = new Vector3[5];
                        roadIntersection.cornerRLCornerRR[0] = tVectRL;
                        roadIntersection.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * (1 / TotalLanesI)) + tVectRL;
                        roadIntersection.cornerRLCornerRR[2] = ((tVectRR - tVectRL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectRL;
                        roadIntersection.cornerRLCornerRR[3] = ((tVectRR - tVectRL) * ((LanesPerSide + 2) / TotalLanesI)) + tVectRL;
                        roadIntersection.cornerRLCornerRR[4] = tVectRR;
                        //Upper left to upper right:
                        roadIntersection.cornerLLCornerRL = new Vector3[5];
                        roadIntersection.cornerLLCornerRL[0] = tVectLL;
                        roadIntersection.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * (1 / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerRL[2] = ((tVectRL - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerRL[3] = ((tVectRL - tVectLL) * ((LanesPerSide + 2) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerRL[4] = tVectRL;
                        //Upper left to lower left:
                        roadIntersection.cornerLLCornerLR = new Vector3[5];
                        roadIntersection.cornerLLCornerLR[0] = tVectLL;
                        roadIntersection.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerLR[2] = ((tVectLR - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerLR[3] = ((tVectLR - tVectLL) * ((LanesPerSide + 1 + LanesPerSide) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerLR[4] = tVectLR;
                    }
                    else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        TotalLanesI = TotalLanes + 1;
                        //Lower left to lower right:
                        roadIntersection.cornerLRCornerRR = new Vector3[4];
                        roadIntersection.cornerLRCornerRR[0] = tVectLR;
                        roadIntersection.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * (LanesPerSide / TotalLanesI)) + tVectLR;
                        roadIntersection.cornerLRCornerRR[2] = ((tVectRR - tVectLR) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLR;
                        roadIntersection.cornerLRCornerRR[3] = tVectRR;
                        //Upper right to lower right:
                        roadIntersection.cornerRLCornerRR = new Vector3[4];
                        roadIntersection.cornerRLCornerRR[0] = tVectRL;
                        roadIntersection.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * (LanesPerSide / TotalLanesI)) + tVectRL;
                        roadIntersection.cornerRLCornerRR[2] = ((tVectRR - tVectRL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectRL;
                        roadIntersection.cornerRLCornerRR[3] = tVectRR;
                        //Upper left to upper right:
                        roadIntersection.cornerLLCornerRL = new Vector3[4];
                        roadIntersection.cornerLLCornerRL[0] = tVectLL;
                        roadIntersection.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerRL[2] = ((tVectRL - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerRL[3] = tVectRL;
                        //Upper left to lower left:
                        roadIntersection.cornerLLCornerLR = new Vector3[4];
                        roadIntersection.cornerLLCornerLR[0] = tVectLL;
                        roadIntersection.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * (LanesPerSide / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerLR[2] = ((tVectLR - tVectLL) * ((LanesPerSide + 1) / TotalLanesI)) + tVectLL;
                        roadIntersection.cornerLLCornerLR[3] = tVectLR;
                    }
                    else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        TotalLanesI = TotalLanes + 0;
                        //Lower left to lower right:
                        roadIntersection.cornerLRCornerRR = new Vector3[3];
                        roadIntersection.cornerLRCornerRR[0] = tVectLR;
                        roadIntersection.cornerLRCornerRR[1] = ((tVectRR - tVectLR) * 0.5f) + tVectLR;
                        roadIntersection.cornerLRCornerRR[2] = tVectRR;
                        //Upper right to lower right:
                        roadIntersection.cornerRLCornerRR = new Vector3[3];
                        roadIntersection.cornerRLCornerRR[0] = tVectRL;
                        roadIntersection.cornerRLCornerRR[1] = ((tVectRR - tVectRL) * 0.5f) + tVectRL;
                        roadIntersection.cornerRLCornerRR[2] = tVectRR;
                        //Upper left to upper right:
                        roadIntersection.cornerLLCornerRL = new Vector3[3];
                        roadIntersection.cornerLLCornerRL[0] = tVectLL;
                        roadIntersection.cornerLLCornerRL[1] = ((tVectRL - tVectLL) * 0.5f) + tVectLL;
                        roadIntersection.cornerLLCornerRL[2] = tVectRL;
                        //Upper left to lower left:
                        roadIntersection.cornerLLCornerLR = new Vector3[3];
                        roadIntersection.cornerLLCornerLR[0] = tVectLL;
                        roadIntersection.cornerLLCornerLR[1] = ((tVectLR - tVectLL) * 0.5f) + tVectLL;
                        roadIntersection.cornerLLCornerLR[2] = tVectLR;
                    }

                    //Use node1/node2 for angles instead
                    float tShoulderWidth = shoulderWidth * 1.75f;
                    float tRampWidth = shoulderWidth * 2f;

                    oNode1.intersection.oddAngle = Vector3.Angle(roadIntersection.node2.tangent, roadIntersection.node1.tangent);
                    oNode1.intersection.evenAngle = 180f - Vector3.Angle(roadIntersection.node2.tangent, roadIntersection.node1.tangent);

                    IntersectionObjects.GetFourPoints(roadIntersection, out roadIntersection.cornerRROuter, out roadIntersection.cornerRLOuter, out roadIntersection.cornerLLOuter, out roadIntersection.cornerLROuter, tShoulderWidth);
                    IntersectionObjects.GetFourPoints(roadIntersection, out roadIntersection.cornerRRRampOuter, out roadIntersection.cornerRLRampOuter, out roadIntersection.cornerLLRampOuter, out roadIntersection.cornerLRRampOuter, tRampWidth);

                    roadIntersection.ConstructBoundsRect();
                    roadIntersection.cornerRR2D = new Vector2(tVectRR.x, tVectRR.z);
                    roadIntersection.cornerRL2D = new Vector2(tVectRL.x, tVectRL.z);
                    roadIntersection.cornerLL2D = new Vector2(tVectLL.x, tVectLL.z);
                    roadIntersection.cornerLR2D = new Vector2(tVectLR.x, tVectLR.z);

                    if (!oNode1.intersection.isSameSpline)
                    {
                        if (string.Compare(_road.spline.uID, oNode1.spline.road.spline.uID) != 0)
                        {
                            AddIntersectionBounds(ref oNode1.spline.road, ref _road.RCS);
                        }
                        else if (string.Compare(_road.spline.uID, oNode2.spline.road.spline.uID) != 0)
                        {
                            AddIntersectionBounds(ref oNode2.spline.road, ref _road.RCS);
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
                    isDidIntersect = RootUtils.Intersects2D(ref t2D_Line1Start, ref t2D_Line1End, ref t2D_Line2Start, ref t2D_Line2End, out tIntersectLocation);
                    if (isDidIntersect)
                    {
                        return tIntersectLocation;
                    }
                }
            }
            return tIntersectLocation;
        }


        private static void AddIntersectionBounds(ref Road _road, ref RoadConstructorBufferMaker _RCS)
        {
            #region "Vars"
            bool isBridge = false;
            bool isTempBridge = false;

            bool isTunnel = false;
            bool isTempTunnel = false;

            RoadIntersection roadIntersection = null;
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
            SplineC tSpline = _road.spline;
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

            float Step = _road.roadDefinition / tSpline.distance;

            SplineN xNode = null;
            float tInterSubtract = 4f;
            float tLastInterHeight = -4f;
            #endregion

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
            if (tSpline.isSpecialEndControlNode)
            {
                FinalMax = tSpline.nodes[tSpline.GetNodeCount() - 2].time;
            }
            if (tSpline.isSpecialStartControlNode)
            {
                StartMin = tSpline.nodes[1].time;
            }

            //int StartIndex = tSpline.GetClosestRoadDefIndex(StartMin,true,false);
            //int EndIndex = tSpline.GetClosestRoadDefIndex(FinalMax,false,true);
            bool isSkip = true;
            bool isSkipFinal = false;
            int kCount = 0;
            int kFinalCount = tSpline.RoadDefKeysArray.Length;
            int spamcheckmax1 = 18000;
            int spamcheck1 = 0;

            if (RootUtils.IsApproximately(StartMin, 0f, 0.0001f))
            {
                isSkip = false;
            }
            if (RootUtils.IsApproximately(FinalMax, 1f, 0.0001f))
            {
                isSkipFinal = true;
            }

            while (!isFinalEnd && spamcheck1 < spamcheckmax1)
            {
                spamcheck1++;

                if (isSkip)
                {
                    i = StartMin;
                    isSkip = false;
                }
                else
                {
                    if (kCount >= kFinalCount)
                    {
                        i = FinalMax;
                        if (isSkipFinal)
                        {
                            break;
                        }
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

                if (RootUtils.IsApproximately(i, FinalMax, 0.00001f))
                {
                    isFinalEnd = true;
                }
                else if (i > FinalMax)
                {
                    if (tSpline.isSpecialEndControlNode)
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

                tSpline.GetSplineValueBoth(i, out tVect, out POS);
                isPastInter = false;
                tIntStrength = tSpline.IntersectionStrength(ref tVect, ref tIntHeight, ref roadIntersection, ref isPastInter, ref i, ref xNode);
                if (RootUtils.IsApproximately(tIntStrength, 1f, 0.001f) || tIntStrength > 1f)
                {
                    isMaxIntersection = true;
                }
                else
                {
                    isMaxIntersection = false;
                }

                if (isMaxIntersection)
                {
                    if (string.Compare(xNode.uID, roadIntersection.node1.uID) == 0)
                    {
                        isFirstInterNode = true;
                    }
                    else
                    {
                        isFirstInterNode = false;
                    }

                    //Convoluted for initial trigger:
                    isTempBridge = tSpline.IsInBridge(i);
                    if (!isBridge && isTempBridge)
                    {
                        isBridge = true;
                    }
                    else if (isBridge && !isTempBridge)
                    {
                        isBridge = false;
                    }
                    //Check if this is the last bridge run for this bridge:
                    if (isBridge)
                    {
                        isTempBridge = tSpline.IsInBridge(i + Step);
                    }


                    //Convoluted for initial trigger:
                    isTempTunnel = tSpline.IsInTunnel(i);
                    if (!isTunnel && isTempTunnel)
                    {
                        isTunnel = true;
                    }
                    else if (isTunnel && !isTempTunnel)
                    {
                        isTunnel = false;
                    }
                    //Check if this is the last Tunnel run for this Tunnel:
                    if (isTunnel)
                    {
                        isTempTunnel = tSpline.IsInTunnel(i + Step);
                    }

                    if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSeperation_NoTurn * POS.normalized.z, 0, RoadSeperation_NoTurn * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSeperation_NoTurn * -POS.normalized.z, 0, RoadSeperation_NoTurn * POS.normalized.x));
                    }
                    else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        rVect = (tVect + new Vector3(RoadSep1Lane * POS.normalized.z, 0, RoadSep1Lane * -POS.normalized.x));
                        lVect = (tVect + new Vector3(RoadSep1Lane * -POS.normalized.z, 0, RoadSep1Lane * POS.normalized.x));
                    }
                    else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                        if (!RootUtils.IsApproximately(tIntStrength, 0f, 0.001f))
                        {
                            tVect.y = (tIntStrength * tIntHeight) + ((1 - tIntStrength) * tVect.y);
                        }
                        tIntStrength_temp = _road.spline.IntersectionStrength(ref rVect, ref tIntHeight, ref roadIntersection, ref isPastInter, ref i, ref xNode);
                        if (!RootUtils.IsApproximately(tIntStrength_temp, 0f, 0.001f))
                        {
                            rVect.y = (tIntStrength_temp * tIntHeight) + ((1 - tIntStrength_temp) * rVect.y);
                            ShoulderR_lVect = rVect;
                        }
                    }

                    //Add bounds for later removal:
                    Construction2DRect vRect = null;
                    if (!isBridge && !isTunnel && isMaxIntersection && isWasPrevMaxInter)
                    {
                        bool isGoAhead = true;
                        if (xNode.isEndPoint)
                        {
                            if (xNode.idOnSpline == 1)
                            {
                                if (i < xNode.time)
                                {
                                    isGoAhead = false;
                                }
                            }
                            else
                            {
                                if (i > xNode.time)
                                {
                                    isGoAhead = false;
                                }
                            }
                        }
                        //Get this and prev lvect rvect rects:
                        if (Vector3.Distance(xNode.pos, tVect) < (3f * RoadWidth) && isGoAhead)
                        {
                            if (roadIntersection.isFlipped && !isFirstInterNode)
                            {
                                vRect = new Construction2DRect(
                                    new Vector2(rVect.x, rVect.z),
                                    new Vector2(lVect.x, lVect.z),
                                    new Vector2(rVect_Prev.x, rVect_Prev.z),
                                    new Vector2(lVect_Prev.x, lVect_Prev.z),
                                    tLastInterHeight
                                    );
                            }
                            else
                            {
                                vRect = new Construction2DRect(
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
        private static void RoadJobPrelimFinalizeInter(ref Road _road)
        {
            int mCount = _road.spline.GetNodeCount();
            SplineN tNode;
            for (int index = 0; index < mCount; index++)
            {
                tNode = _road.spline.nodes[index];
                if (tNode.isIntersection)
                {
                    Inter_OrganizeVertices(ref tNode, ref _road);
                    tNode.intersectionConstruction.Nullify();
                    tNode.intersectionConstruction = null;
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
                //if(t2D.x > 745f && t2D.x < 755f && t2D.y > 1240f && t2D.y < 1250f)
                //{
                //	int testInteger = 1;	
                //}
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


        private static void Inter_OrganizeVertices(ref SplineN _node, ref Road _road)
        {
            iConstructionMaker iCon = _node.intersectionConstruction;
            RoadIntersection roadIntersection = _node.intersection;

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
            if (_node.idOnSpline == 0 && string.CompareOrdinal(roadIntersection.node1uID, _node.uID) == 0)
            {
                bSkipB = true;
            }
            //Is primary node and is last node on a spline, meaning t junction: It does not have a F:
            if (_node.idOnSpline == (_node.spline.GetNodeCount() - 1) && string.CompareOrdinal(roadIntersection.node1uID, _node.uID) == 0)
            {
                isSkipF = true;
            }

            //Other node is t junction end node, meaning now we figure out which side we're on
            if (_node.intersectionOtherNode.idOnSpline == 0 || _node.idOnSpline == (_node.spline.GetNodeCount() - 1))
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

                if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iCon.shoulderStartFR = iCon.iFLane0L[0];
                    iCon.shoulderStartFL = iCon.iFLane3R[0];
                }
                else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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
                if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iCon.shoulderEndBL = iCon.iBLane0L[iCon.iBLane0L.Count - 1];
                    iCon.shoulderEndBR = iCon.iBLane3R[iCon.iBLane3R.Count - 1];
                }
                else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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
                InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iBLane0L, iCon.shoulderBLStartIndex, ref iCon.shoulderStartBL, ref iCon.shoulderEndBL, roadIntersection.height);
                if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane3R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, roadIntersection.height);
                }
                else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane2R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, roadIntersection.height);
                }
                else
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iBLane1R, iCon.shoulderBRStartIndex, ref iCon.shoulderStartBR, ref iCon.shoulderEndBR, roadIntersection.height);
                }
            }

            if (!isSkipF)
            {
                InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderR_Vectors, ref iCon.iFLane0L, iCon.shoulderFRStartIndex, ref iCon.shoulderStartFR, ref iCon.shoulderEndFR, roadIntersection.height, true);
                if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane3R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, roadIntersection.height, true);
                }
                else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane2R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, roadIntersection.height, true);
                }
                else
                {
                    InterOrganizeVerticesMatchShoulder(ref _road.RCS.ShoulderL_Vectors, ref iCon.iFLane1R, iCon.shoulderFLStartIndex, ref iCon.shoulderStartFL, ref iCon.shoulderEndFL, roadIntersection.height, true);
                }
            }

            bool bError = false;
            string tWarning = "Intersection " + roadIntersection.intersectionName + " in road " + _road.roadName + " at too extreme angle to process this intersection type. Reduce angle or reduce lane count.";

            if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
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
            else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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
            else if (roadIntersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    mCount = iCon.iBLane1R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iBMainPlateR.Add(iCon.iBLane1R[m]);
                    }
                }
                else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    mCount = iCon.iBLane2R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iBMainPlateR.Add(iCon.iBLane2R[m]);
                    }
                }
                else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    mCount = iCon.iFLane1R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iFMainPlateR.Add(iCon.iFLane1R[m]);
                    }
                }
                else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                {
                    mCount = iCon.iFLane2R.Count;
                    for (int m = 0; m < mCount; m++)
                    {
                        iCon.iFMainPlateR.Add(iCon.iFLane2R[m]);
                    }
                }
                else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                if (!_road.spline.nodes[0].isIntersection && !_road.spline.nodes[1].isIntersection)
                {
                    for (int i = 2; i < cCount; i++)
                    {
                        if (_road.spline.nodes[i].isIntersection)
                        {
                            if (i - 2 >= 1)
                            {
                                tStartI = (int) (_road.spline.nodes[i - 2].time * mCount);
                            }
                            break;
                        }
                    }
                }
            }
            if (cCount > 3)
            {
                if (!_road.spline.nodes[cCount - 1].isIntersection && !_road.spline.nodes[cCount - 2].isIntersection)
                {
                    for (int i = (cCount - 3); i >= 0; i--)
                    {
                        if (_road.spline.nodes[i].isIntersection)
                        {
                            if (i + 2 < cCount)
                            {
                                tEndI = (int) (_road.spline.nodes[i + 2].time * mCount);
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
                    if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
                    else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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

            //			if(tNode.roadIntersection.rType != RoadIntersection.RoadTypeEnum.NoTurnLane){ 
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

            if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
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
            else if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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
                iBLane0 = InterVertices(iCon.iBLane0L, iCon.iBLane0R, _node.intersection.height);
                iBLane1 = InterVertices(iCon.iBLane1L, iCon.iBLane1R, _node.intersection.height);
                if (_node.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    iBLane2 = InterVertices(iCon.iBLane2L, iCon.iBLane2R, _node.intersection.height);
                }
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iBLane3 = InterVertices(iCon.iBLane3L, iCon.iBLane3R, _node.intersection.height);
                }
            }

            //Front lanes:
            List<Vector3> iFLane0 = null;
            List<Vector3> iFLane1 = null;
            List<Vector3> iFLane2 = null;
            List<Vector3> iFLane3 = null;
            if (!isSkipF)
            {
                iFLane0 = InterVertices(iCon.iFLane0L, iCon.iFLane0R, _node.intersection.height);
                iFLane1 = InterVertices(iCon.iFLane1L, iCon.iFLane1R, _node.intersection.height);
                if (_node.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    iFLane2 = InterVertices(iCon.iFLane2L, iCon.iFLane2R, _node.intersection.height);
                }
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    iFLane3 = InterVertices(iCon.iFLane3L, iCon.iFLane3R, _node.intersection.height);
                }
            }

            //Main plates:
            List<Vector3> iBMainPlate = null;
            List<Vector3> iFMainPlate = null;
            if (!bSkipB)
            {
                iBMainPlate = InterVertices(iCon.iBMainPlateL, iCon.iBMainPlateR, _node.intersection.height);
            }
            if (!isSkipF)
            {
                iFMainPlate = InterVertices(iCon.iFMainPlateL, iCon.iFMainPlateR, _node.intersection.height);
            }
            //			//Marker plates:
            //			List<Vector3> iBMarkerPlate = InterVertices(iCon.iBMarkerPlateL,iCon.iBMarkerPlateR, tNode.roadIntersection.Height);
            //			List<Vector3> iFMarkerPlate = InterVertices(iCon.iFMarkerPlateL,iCon.iFMarkerPlateR, tNode.roadIntersection.Height);
            //			
            //Now add these to RCS:
            if (!bSkipB)
            {
                _road.RCS.iBLane0s.Add(iBLane0.ToArray());
                _road.RCS.iBLane0s_tID.Add(roadIntersection);
                _road.RCS.iBLane0s_nID.Add(_node);
                _road.RCS.iBLane1s.Add(iBLane1.ToArray());
                _road.RCS.iBLane1s_tID.Add(roadIntersection);
                _road.RCS.iBLane1s_nID.Add(_node);
                if (_node.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    if (iBLane2 != null)
                    {
                        _road.RCS.iBLane2s.Add(iBLane2.ToArray());
                        _road.RCS.iBLane2s_tID.Add(roadIntersection);
                        _road.RCS.iBLane2s_nID.Add(_node);
                    }
                }
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _road.RCS.iBLane3s.Add(iBLane3.ToArray());
                    _road.RCS.iBLane3s_tID.Add(roadIntersection);
                    _road.RCS.iBLane3s_nID.Add(_node);
                }
            }
            //Front lanes:
            if (!isSkipF)
            {
                _road.RCS.iFLane0s.Add(iFLane0.ToArray());
                _road.RCS.iFLane0s_tID.Add(roadIntersection);
                _road.RCS.iFLane0s_nID.Add(_node);
                _road.RCS.iFLane1s.Add(iFLane1.ToArray());
                _road.RCS.iFLane1s_tID.Add(roadIntersection);
                _road.RCS.iFLane1s_nID.Add(_node);
                if (_node.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
                {
                    _road.RCS.iFLane2s.Add(iFLane2.ToArray());
                    _road.RCS.iFLane2s_tID.Add(roadIntersection);
                    _road.RCS.iFLane2s_nID.Add(_node);
                }
                if (_node.intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                {
                    _road.RCS.iFLane3s.Add(iFLane3.ToArray());
                    _road.RCS.iFLane3s_tID.Add(roadIntersection);
                    _road.RCS.iFLane3s_nID.Add(_node);
                }
            }
            //Main plates:
            if (iBMainPlate != null && !bSkipB)
            {
                _road.RCS.iBMainPlates.Add(iBMainPlate.ToArray());
                _road.RCS.iBMainPlates_tID.Add(roadIntersection);
                _road.RCS.iBMainPlates_nID.Add(_node);
            }
            if (iFMainPlate != null && !isSkipF)
            {
                _road.RCS.iFMainPlates.Add(iFMainPlate.ToArray());
                _road.RCS.iFMainPlates_tID.Add(roadIntersection);
                _road.RCS.iFMainPlates_nID.Add(_node);
            }
            //			//Marker plates:
            //			tRoad.RCS.iBMarkerPlates.Add(iBMarkerPlate.ToArray());
            //			tRoad.RCS.iFMarkerPlates.Add(iFMarkerPlate.ToArray());
            //			tRoad.RCS.IntersectionTypes.Add((int)tNode.roadIntersection.rType);

            if (_node.intersection.roadType != RoadIntersection.RoadTypeEnum.NoTurnLane)
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


        /// <summary> Handles most triangles and normals construction. In certain scenarios for efficiency reasons UV might also be processed. </summary>
        /// <param name='_RCS'> The road construction buffer, by reference. </param>/
        public static void RoadJob1(ref RoadConstructorBufferMaker _RCS)
        {
            //Triangles and normals:
            //RootUtils.StartProfiling(RCS.tRoad, "ProcessRoad_IntersectionCleanup");
            if (_RCS.isInterseOn)
            {
                ProcessRoadIntersectionCleanup(ref _RCS);
            }
            //RootUtils.EndProfiling(RCS.tRoad);

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
        public static void RoadJob2(ref RoadConstructorBufferMaker _RCS)
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
                    _RCS.cut_tangents.Add(RootUtils.ProcessTangents(_RCS.cut_tris[index], _RCS.cut_normals[index], _RCS.cut_uv[index], _RCS.cut_RoadVectors[index].ToArray()));
                    _RCS.cut_tangents_world.Add(RootUtils.ProcessTangents(_RCS.cut_tris[index], _RCS.cut_normals[index], _RCS.cut_uv_world[index], _RCS.cut_RoadVectors[index].ToArray()));
                }
            }
            if (_RCS.road.isShoulderCutsEnabled || _RCS.road.isDynamicCutsEnabled)
            {
                // Add shoulders for right side
                int rCount = _RCS.cut_ShoulderR_Vectors.Count;
                for (int index = 0; index < rCount; index++)
                {
                    ProcessRoadUVsShoulderCut(ref _RCS, false, index);
                    _RCS.cut_tangents_SR.Add(RootUtils.ProcessTangents(_RCS.cut_tris_ShoulderR[index], _RCS.cut_normals_ShoulderR[index], _RCS.cut_uv_SR[index], _RCS.cut_ShoulderR_Vectors[index].ToArray()));
                    _RCS.cut_tangents_SR_world.Add(RootUtils.ProcessTangents(_RCS.cut_tris_ShoulderR[index], _RCS.cut_normals_ShoulderR[index], _RCS.cut_uv_SR_world[index], _RCS.cut_ShoulderR_Vectors[index].ToArray()));
                }

                // Add shoulders for left side
                int lCount = _RCS.cut_ShoulderL_Vectors.Count;
                for (int index = 0; index < lCount; index++)
                {
                    ProcessRoadUVsShoulderCut(ref _RCS, true, index);
                    _RCS.cut_tangents_SL.Add(RootUtils.ProcessTangents(_RCS.cut_tris_ShoulderL[index], _RCS.cut_normals_ShoulderL[index], _RCS.cut_uv_SL[index], _RCS.cut_ShoulderL_Vectors[index].ToArray()));
                    _RCS.cut_tangents_SL_world.Add(RootUtils.ProcessTangents(_RCS.cut_tris_ShoulderL[index], _RCS.cut_normals_ShoulderL[index], _RCS.cut_uv_SL_world[index], _RCS.cut_ShoulderL_Vectors[index].ToArray()));
                }
            }

            // Update type full or intersection
            if (_RCS.isInterseOn)
            {
                ProcessRoadUVsIntersections(ref _RCS);
            }

            //throw new System.Exception("FFFFFFFF");

            // Update type full, intersection or bridge
            if (_RCS.isRoadOn)
            {
                if (!_RCS.tMeshSkip)
                {
                    _RCS.tangents = RootUtils.ProcessTangents(_RCS.tris, _RCS.normals, _RCS.uv, _RCS.RoadVectors.ToArray());
                }
                if (!_RCS.tMeshSkip)
                {
                    _RCS.tangents2 = RootUtils.ProcessTangents(_RCS.tris, _RCS.normals, _RCS.uv2, _RCS.RoadVectors.ToArray());
                }
                if (!_RCS.tMesh_SRSkip)
                {
                    _RCS.tangents_SR = RootUtils.ProcessTangents(_RCS.tris_ShoulderR, _RCS.normals_ShoulderR, _RCS.uv_SR, _RCS.ShoulderR_Vectors.ToArray());
                }
                if (!_RCS.tMesh_SLSkip)
                {
                    _RCS.tangents_SL = RootUtils.ProcessTangents(_RCS.tris_ShoulderL, _RCS.normals_ShoulderL, _RCS.uv_SL, _RCS.ShoulderL_Vectors.ToArray());
                }
                for (int index = 0; index < _RCS.tMesh_RoadConnections.Count; index++)
                {
                    _RCS.RoadConnections_tangents.Add(RootUtils.ProcessTangents(_RCS.RoadConnections_tris[index], _RCS.RoadConnections_normals[index], _RCS.RoadConnections_uv[index], _RCS.RoadConnections_verts[index]));
                }
            }

            if (_RCS.isInterseOn)
            {
                //Back lanes:
                int vCount = _RCS.iBLane0s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane0s_tangents.Add(RootUtils.ProcessTangents(_RCS.iBLane0s_tris[index], _RCS.iBLane0s_normals[index], _RCS.iBLane0s_uv[index], _RCS.iBLane0s[index]));
                }
                vCount = _RCS.iBLane1s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane1s_tangents.Add(RootUtils.ProcessTangents(_RCS.iBLane1s_tris[index], _RCS.iBLane1s_normals[index], _RCS.iBLane1s_uv[index], _RCS.iBLane1s[index]));
                }
                vCount = _RCS.iBLane2s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane2s_tangents.Add(RootUtils.ProcessTangents(_RCS.iBLane2s_tris[index], _RCS.iBLane2s_normals[index], _RCS.iBLane2s_uv[index], _RCS.iBLane2s[index]));
                }
                vCount = _RCS.iBLane3s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBLane3s_tangents.Add(RootUtils.ProcessTangents(_RCS.iBLane3s_tris[index], _RCS.iBLane3s_normals[index], _RCS.iBLane3s_uv[index], _RCS.iBLane3s[index]));
                }
                //Front lanes:
                vCount = _RCS.iFLane0s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane0s_tangents.Add(RootUtils.ProcessTangents(_RCS.iFLane0s_tris[index], _RCS.iFLane0s_normals[index], _RCS.iFLane0s_uv[index], _RCS.iFLane0s[index]));
                }
                vCount = _RCS.iFLane1s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane1s_tangents.Add(RootUtils.ProcessTangents(_RCS.iFLane1s_tris[index], _RCS.iFLane1s_normals[index], _RCS.iFLane1s_uv[index], _RCS.iFLane1s[index]));
                }
                vCount = _RCS.iFLane2s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane2s_tangents.Add(RootUtils.ProcessTangents(_RCS.iFLane2s_tris[index], _RCS.iFLane2s_normals[index], _RCS.iFLane2s_uv[index], _RCS.iFLane2s[index]));
                }
                vCount = _RCS.iFLane3s.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFLane3s_tangents.Add(RootUtils.ProcessTangents(_RCS.iFLane3s_tris[index], _RCS.iFLane3s_normals[index], _RCS.iFLane3s_uv[index], _RCS.iFLane3s[index]));
                }
                //Main plates:
                vCount = _RCS.iBMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBMainPlates_tangents.Add(RootUtils.ProcessTangents(_RCS.iBMainPlates_tris[index], _RCS.iBMainPlates_normals[index], _RCS.iBMainPlates_uv[index], _RCS.iBMainPlates[index]));
                }
                vCount = _RCS.iBMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iBMainPlates_tangents2.Add(RootUtils.ProcessTangents(_RCS.iBMainPlates_tris[index], _RCS.iBMainPlates_normals[index], _RCS.iBMainPlates_uv2[index], _RCS.iBMainPlates[index]));
                }
                vCount = _RCS.iFMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFMainPlates_tangents.Add(RootUtils.ProcessTangents(_RCS.iFMainPlates_tris[index], _RCS.iFMainPlates_normals[index], _RCS.iFMainPlates_uv[index], _RCS.iFMainPlates[index]));
                }
                vCount = _RCS.iFMainPlates.Count;
                for (int index = 0; index < vCount; index++)
                {
                    _RCS.iFMainPlates_tangents2.Add(RootUtils.ProcessTangents(_RCS.iFMainPlates_tris[index], _RCS.iFMainPlates_normals[index], _RCS.iFMainPlates_uv2[index], _RCS.iFMainPlates[index]));
                }
            }
        }


        #region "Intersection Cleanup"
        private static void ProcessRoadIntersectionCleanup(ref RoadConstructorBufferMaker _RCS)
        {
            List<Construction2DRect> tList = _RCS.tIntersectionBounds;
            int constructionCount = tList.Count;
            _RCS.ShoulderR_Vectors = ProcessRoadIntersectionCleanupHelper(ref _RCS.ShoulderR_Vectors, ref tList, constructionCount, ref _RCS.ImmuneVects);
            _RCS.ShoulderL_Vectors = ProcessRoadIntersectionCleanupHelper(ref _RCS.ShoulderL_Vectors, ref tList, constructionCount, ref _RCS.ImmuneVects);
        }


        private static List<Vector3> ProcessRoadIntersectionCleanupHelper(ref List<Vector3> _vects, ref List<Construction2DRect> _list, int _count, ref HashSet<Vector3> _immuneVects)
        {
            Construction2DRect tRect = null;
            int MVL = _vects.Count;
            Vector2 Vect2D = default(Vector2);
            Vector2 tNearVect = default(Vector2);
            float tMax2 = 2000f;
            float tMax2SQ = 0f;
            //GameObject tObj = GameObject.Find("Inter1");
            //Vector2 tObj2D = ConvertVect3_To_Vect2(tObj.transform.position);
            //int fCount = 0;
            //bool bTempNow = false;
            for (int i = 0; i < _count; i++)
            {
                tRect = _list[i];
                tMax2 = tRect.MaxDistance * 1.5f;
                tMax2SQ = (tMax2 * tMax2);

                //Debug.Log (tRect.ToString());

                for (int j = 0; j < MVL; j++)
                {
                    Vect2D.x = _vects[j].x;
                    Vect2D.y = _vects[j].z;

                    if (Vector2.SqrMagnitude(Vect2D - tRect.P1) > tMax2SQ)
                    {
                        j += 32;
                        continue;
                    }

                    //if(Vector2.Distance(Vect2D,tObj2D) < 20f && (j % 16 == 0))
                    //{
                    //	fCount+=1;
                    //	GameObject xObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //	xObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                    //	xObj.transform.position = tVects[j];
                    //	xObj.name = "temp22";
                    //}

                    //bTempNow = false;
                    if (tRect.Contains(ref Vect2D))
                    {
                        if (_immuneVects.Contains(_vects[j]))
                        {
                            continue;
                        }
                        //if(Vect2D == tRect.P1)
                        //{
                        //	continue;
                        //}
                        //else if(Vect2D == tRect.P2)
                        //{
                        //	continue;
                        //}else if(Vect2D == tRect.P3)
                        //{
                        //	continue;
                        //}else if(Vect2D == tRect.P4)
                        //{
                        //	continue;
                        //}


                        //if(Mathf.Approximately(tVects[j].x,303.1898f))
                        //{
                        //  GameObject hObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //	hObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                        //	hObj.transform.position = tVects[j];
                        //	hObj.name = "temp23";
                        //	bTempNow = true;
                        //	Debug.Log (tVects[j]);
                        //}

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

                        //if(bTempNow)
                        //{
                        //	GameObject xObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        //	xObj.transform.localScale = new Vector3(0.05f,40f,0.05f);
                        //	xObj.transform.position = tVects[j];
                        //	xObj.name = "temp22";
                        //	Debug.Log ("to: " + tVects[j]);
                        //}
                    }
                }
            }
            //Debug.Log ("Fcount: " + fCount);

            return _vects;
        }
        #endregion


        #region "Tris"
        private static void ProcessRoadTrisBulk(ref RoadConstructorBufferMaker _RCS)
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
            //if(xCount < 0)
            //{
            //  xCount = 0;
            //}
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


        private static void ProcessRoadTrisRoadCuts(ref RoadConstructorBufferMaker _RCS)
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


        private static void ProcessRoadTrisShoulderCutsR(ref RoadConstructorBufferMaker _RCS)
        {
            int cutsCount = _RCS.ShoulderCutsR.Count;
            int PrevRoadCutIndex = 0;
            int CurrentRoadCutIndex = 0;
            List<List<Vector3>> tVects = new List<List<Vector3>>();
            List<Vector3> tVectListSingle = null;
            Vector3 xVect = default(Vector3);
            for (int j = 0; j < cutsCount; j++)
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


        private static void ProcessRoadTrisShoulderCutsL(ref RoadConstructorBufferMaker _RCS)
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
        private static void ProcessRoadNormalsBulk(ref RoadConstructorBufferMaker _RCS)
        {
            //A mesh with just the vertices and triangles set up will be visible in the editor but will not look very convincing since it is not correctly shaded without the normals. 
            //The normals for the flat plane are very simple - they are all identical and point in the negative Z direction in the plane's local space. 
            //With the normals added, the plane will be correctly shaded but remember that you need a light in the scene to see the effect. 
            //Bridge normals are processed at same time as tris.
            int MVL = _RCS.RoadVectors.Count;
            Vector3[] normals = new Vector3[MVL];
            //Vector3 tVect = -Vector3.forward;
            //for(int i=0;i<MVL;i++)
            //{
            //	normals[i] = tVect;
            //}
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


        private static void ProcessRoadNormalsRoadCuts(ref RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_RoadVectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_RoadVectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //Vector3 tVect = -Vector3.forward;
                //for(int i=0;i<MVL;i++)
                //{
                //  normals[i] = tVect;
                //}
                _RCS.cut_normals.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulderCutsR(ref RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_ShoulderR_Vectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_ShoulderR_Vectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //Vector3 tVect = -Vector3.forward;
                //for(int i=0;i<MVL;i++)
                //{
                //  normals[i] = tVect;
                //}
                _RCS.cut_normals_ShoulderR.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulderCutsL(ref RoadConstructorBufferMaker _RCS)
        {
            int cCount = _RCS.cut_ShoulderL_Vectors.Count;
            for (int j = 0; j < cCount; j++)
            {
                int MVL = _RCS.cut_ShoulderL_Vectors[j].Count;
                Vector3[] normals = new Vector3[MVL];
                //Vector3 tVect = -Vector3.forward;
                //for(int i=0;i<MVL;i++)
                //{
                //  normals[i] = tVect;
                //}
                _RCS.cut_normals_ShoulderL.Add(normals);
            }
        }


        private static void ProcessRoadNormalsShoulders(ref RoadConstructorBufferMaker _RCS)
        {
            //A mesh with just the vertices and triangles set up will be visible in the editor but will not look very convincing since it is not correctly shaded without the normals. 
            //The normals for the flat plane are very simple - they are all identical and point in the negative Z direction in the plane's local space. 
            //With the normals added, the plane will be correctly shaded but remember that you need a light in the scene to see the effect. 
            int MVL = _RCS.ShoulderL_Vectors.Count;
            Vector3[] normals = new Vector3[MVL];
            //Vector3 tVect = -Vector3.forward;
            //for(int i=0;i<MVL;i++)
            //{
            //	normals[i] = tVect;
            //}
            _RCS.normals_ShoulderL = normals;
            //Right:
            MVL = _RCS.ShoulderR_Vectors.Count;
            normals = new Vector3[MVL];
            //tVect = -Vector3.forward;
            //for(int i=0;i<MVL;i++)
            //{
            //	normals[i] = tVect;
            //}
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
            //Vector3 tVect = -Vector3.forward;
            for (int index = 0; index < vListCount; index++)
            {
                MVL = _vertexList[index].Length;
                normals = new Vector3[MVL];
                //for(int j=0;j<MVL;j++)
                //{
                //	normals[j] = tVect;
                //}
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
            bool isOddToggle = true;
            float distance = 0f;
            float distanceLeft = 0f;
            float distanceRight = 0f;
            float distanceLeftSum = 0f;
            float distanceRightSum = 0f;
            float distanceSum = 0f;

            while (i + 6 < MVL)
            {
                distance = Vector3.Distance(_verts[i], _verts[i + 4]);
                distance = distance / 5f;
                uv[i] = new Vector2(0f, distanceSum);
                uv[i + 2] = new Vector2(1f, distanceSum);
                uv[i + 4] = new Vector2(0f, distance + distanceSum);
                uv[i + 6] = new Vector2(1f, distance + distanceSum);

                //Last segment needs adjusted due to double vertices:
                if ((i + 7) == MVL)
                {
                    if (isOddToggle)
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

                if (isOddToggle)
                {
                    i += 5;
                }
                else
                {
                    i += 3;
                }

                distanceLeftSum += distanceLeft;
                distanceRightSum += distanceRight;
                distanceSum += distance;
                isOddToggle = !isOddToggle;
            }
            return uv;
        }


        /// <summary> Processes uvs for road cuts </summary>
        private static void ProcessRoadUVsRoadCuts(ref RoadConstructorBufferMaker _RCS)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;

            int cCount = _RCS.cut_RoadVectors.Count;
            float distance = 0f;
            float distanceSum = 0f;


            for (int j = 0; j < cCount; j++)
            {
                Vector3[] tVerts = _RCS.cut_RoadVectors[j].ToArray();
                int MVL = tVerts.Length;
                Vector2[] uv = new Vector2[MVL];
                Vector2[] uv_world = new Vector2[MVL];
                int i = 0;
                bool isOddToggle = true;


                while (i + 6 < MVL)
                {
                    distance = Vector3.Distance(tVerts[i], tVerts[i + 4]);
                    distance = distance / 5f;
                    uv[i] = new Vector2(0f, distanceSum);
                    uv[i + 2] = new Vector2(1f, distanceSum);
                    uv[i + 4] = new Vector2(0f, distance + distanceSum);
                    uv[i + 6] = new Vector2(1f, distance + distanceSum);

                    //Last segment needs adjusted due to double vertices:
                    if ((i + 7) == MVL)
                    {
                        if (isOddToggle)
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

                    if (isOddToggle)
                    {
                        i += 5;
                    }
                    else
                    {
                        i += 3;
                    }


                    distanceSum += distance;
                    isOddToggle = !isOddToggle;
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
            //bool bOddToggle = true;
            //float tDistance= 0f;
            //float tDistanceLeft = 0f;
            //float tDistanceRight = 0f;
            //float tDistanceLeftSum = 0f;
            //float tDistanceRightSum = 0f;
            //float tDistanceSum = 0f;
            //float xDistance = 0f;
            //float rDistance1 = 0f;
            //float rDistance2 = 0f;
            //float fDistance = Vector3.Distance(_verts[0],_verts[2]);


            for (i = 0; i < MVL; i++)
            {
                uv[i] = new Vector2(_verts[i].x * 0.2f, _verts[i].z * 0.2f);
            }
            return uv;

            //while(i+8 < MVL){
            //	tDistanceLeft = Vector3.Distance(_verts[i],_verts[i+8]);
            //	tDistanceRight = Vector3.Distance(_verts[i+2],_verts[i+10]);
            //				
            //	tDistance = tDistance / 5f;
            //	tDistanceLeft = tDistanceLeft / 5f;
            //	tDistanceRight = tDistanceRight / 5f;
            //				
            //	uv[i] = new Vector2(0f, tDistanceSum);
            //	uv[i+2] = new Vector2(1f, tDistanceSum);
            //	uv[i+8] = new Vector2(0f, tDistance+tDistanceSum);
            //	uv[i+10] = new Vector2(1f, tDistance+tDistanceSum);	
            //				
            //	rDistance1 = (Vector3.Distance(_verts[i+4],_verts[i+6]));
            //	rDistance2 = (Vector3.Distance(_verts[i+12],_verts[i+14]));
            //					
            //	if(!bIsLeft)
            //  {
            //		uv[i+4] = new Vector2(1f, tDistanceSum);
            //		xDistance = (rDistance1 / fDistance) + 1f;
            //		uv[i+6] = new Vector2(xDistance, tDistanceSum);
            //		uv[i+12] = new Vector2(1f, tDistance+tDistanceSum);
            //		xDistance = (rDistance2 / fDistance) + 1f;
            //		uv[i+14] = new Vector2(xDistance, tDistance+tDistanceSum);
            //  }
            //  else
            //  {
            //		xDistance = (rDistance1 / fDistance);
            //		uv[i+4] = new Vector2(-xDistance, tDistanceSum);
            //		uv[i+6] = new Vector2(0f, tDistanceSum);
            //		xDistance = (rDistance2 / fDistance);
            //		uv[i+12] = new Vector2(-xDistance, tDistance+tDistanceSum);
            //		uv[i+14] = new Vector2(0f, tDistance+tDistanceSum);
            //	}
            //				
            //	uv[i] = new Vector2(0f, tDistanceLeftSum);
            //	uv[i+2] = new Vector2(1f, tDistanceRightSum);
            //	uv[i+8] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            //	uv[i+10] = new Vector2(1f, tDistanceRight+tDistanceRightSum);	
            //
            //	uv[i] = new Vector2(_verts[i].x/5f,_verts[i].z/5f);
            //	uv[i+2] = new Vector2(_verts[i+2].x/5f,_verts[i+2].z/5f);
            //	uv[i+8] = new Vector2(_verts[i+8].x/5f,_verts[i+8].z/5f);
            //	uv[i+10] = new Vector2(_verts[i+10].x/5f,_verts[i+10].z/5f);
            //				
            //				
            //	rDistance1 = (Vector3.Distance(_verts[i+4],_verts[i+6]));
            //	rDistance2 = (Vector3.Distance(_verts[i+12],_verts[i+14]));
            //					
            //	if(!bIsLeft)
            //  {
            //  	uv[i+4] = new Vector2(1f, tDistanceRightSum);
            //		xDistance = (rDistance1 / fDistance) + 1f;
            //		uv[i+6] = new Vector2(xDistance, tDistanceRightSum);
            //		uv[i+12] = new Vector2(1f, tDistanceRight+tDistanceRightSum);
            //		xDistance = (rDistance2 / fDistance) + 1f;
            //		uv[i+14] = new Vector2(xDistance, tDistanceRight+tDistanceRightSum);
            //	}
            //  else
            //  {
            //		xDistance = (rDistance1 / fDistance);
            //		uv[i+4] = new Vector2(-xDistance, tDistanceLeftSum);
            //		uv[i+6] = new Vector2(0f, tDistanceLeftSum);
            //		xDistance = (rDistance2 / fDistance);
            //		uv[i+12] = new Vector2(-xDistance, tDistanceLeft+tDistanceLeftSum);
            //		uv[i+14] = new Vector2(0f, tDistanceLeft+tDistanceLeftSum);
            //	}
            //				
            //	uv[i+4] = new Vector2(_verts[i+4].x/5f,_verts[i+4].z/5f);
            //	uv[i+6] = new Vector2(_verts[i+6].x/5f,(_verts[i+6].z/5f));
            //	uv[i+12] = new Vector2(_verts[i+12].x/5f,_verts[i+12].z/5f);
            //	uv[i+14] = new Vector2(_verts[i+14].x/5f,(_verts[i+14].z/5f));
            //				
            //	//Last segment needs adjusted due to double vertices:
            //	if((i+11) == MVL)
            //  {
            //  	if(bOddToggle)
            //      {
            //			//First set: Debug.Log ("+5:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));		
            //			uv[MVL-3] = uv[i+4];
            //			uv[MVL-1] = uv[i+6];
            //		}
            //      else
            //      {
            //			//Last set: Debug.Log ("+3:"+i+" "+(i+2)+" "+(i+4)+" "+(i+6));	
            //			uv[MVL-4] = uv[i+4];
            //			uv[MVL-2] = uv[i+6];
            //		}
            //	}
            //				
            //	if(bOddToggle)
            //  {
            //		i+=9;	
            //	}
            //  else
            //  {
            //		i+=7;
            //	}
            //				
            //	tDistanceLeftSum+=tDistanceLeft;
            //	tDistanceRightSum+=tDistanceRight;
            //	tDistanceSum+=tDistance;
            //	bOddToggle = !bOddToggle;
            //}
            //return uv;
        }


        private static void ProcessRoadUVsShoulderCut(ref RoadConstructorBufferMaker _RCS, bool _isLeft, int _j)
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
            float distance = 0f;
            float distanceSum = 0f;
            Vector2[] uv = new Vector2[MVL];
            float rDistance1 = 0f;
            float rDistance2 = 0f;
            bool isOddToggle = true;
            float fDistance = Vector3.Distance(tVerts[0], tVerts[2]);
            float xDistance = 0f;
            i = 0;
            float TheOne = _RCS.road.shoulderWidth / _RCS.road.roadDefinition;
            while (i + 8 < MVL)
            {
                distance = Vector3.Distance(tVerts[i], tVerts[i + 8]) * 0.2f;

                uv[i] = new Vector2(0f, distanceSum);
                uv[i + 2] = new Vector2(TheOne, distanceSum);
                uv[i + 8] = new Vector2(0f, distance + distanceSum);
                uv[i + 10] = new Vector2(TheOne, distance + distanceSum);

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
                    uv[i + 6] = new Vector2(xDistance, distanceSum);

                    xDistance = TheOne + (rDistance2 / fDistance);
                    uv[i + 12] = uv[i + 10];
                    uv[i + 14] = new Vector2(xDistance, distance + distanceSum);
                }
                else
                {
                    //Left:
                    //12,13	   14,15    8,9    10,11
                    //4,5		6,7		0,1		2,3	
                    //0f-X	     0f	 	 0f		1f

                    xDistance = 0f - (rDistance1 / fDistance);
                    uv[i + 4] = new Vector2(xDistance, distanceSum);
                    uv[i + 6] = uv[i];
                    xDistance = 0f - (rDistance2 / fDistance);
                    uv[i + 12] = new Vector2(xDistance, distance + distanceSum);
                    uv[i + 14] = uv[i + 8];
                }

                //Last segment needs adjusted due to double vertices:
                if ((i + 11) == MVL)
                {
                    if (isOddToggle)
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

                if (isOddToggle)
                {
                    i += 9;
                }
                else
                {
                    i += 7;
                }

                distanceSum += distance;
                isOddToggle = !isOddToggle;
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
        private static void ProcessRoadUVsIntersections(ref RoadConstructorBufferMaker _RCS)
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


        private static Vector2[] ProcessRoadUVsIntersectionFullLane(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
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


        private static Vector2[] ProcessRoadUVsIntersectionLane4(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
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


        private static Vector2[] ProcessRoadUVsIntersectionMiddleLane(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
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


        private static Vector2[] ProcessRoadUVsIntersectionLane0(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
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


        private static Vector2[] ProcessRoad_UVs_Intersection_MarkerPlate(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
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


        private static Vector2[] ProcessRoadUVsIntersectionMainPlate(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts)
        {
            //Finally, adding texture coordinates to the mesh will enable it to display a material correctly. 
            //Assuming we want to show the whole image across the plane, the UV values will all be 0 or 1, corresponding to the corners of the texture. 
            //int MVL = tMesh.vertices.Length;
            int MVL = _verts.Length;
            Vector2[] uv = new Vector2[MVL];
            int i = 0;
            //bool bOddToggle = true;
            //float tDistance= 0f;
            //float tDistanceLeft = 0f;
            //float tDistanceRight = 0f;
            //float tDistanceLeftSum = 0f;
            //float tDistanceRightSum = 0f;
            //float tDistanceSum = 0f;
            //float DistRepresent = 5f;

            //float mDistanceL = Vector3.Distance(_verts[i],_verts[_verts.Length-3]);
            //float mDistanceR = Vector3.Distance(_verts[i+2],_verts[_verts.Length-1]);

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


        private static Vector2[] ProcessRoadUVsIntersectionMainPlate2(ref RoadConstructorBufferMaker _RCS, Vector3[] _verts, RoadIntersection _intersection)
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
            if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
            {
                iWidth = RoadWidth + (LaneWidth * 2f);
            }
            else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
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

                    if (_intersection.roadType == RoadIntersection.RoadTypeEnum.BothTurnLanes)
                    {

                        //(Lane width / 2)/roadwidth
                        //1-((lanewidth / 2)/roadwidth)



                        uv[i] = new Vector2((LaneWidth * 0.5f) / iWidth, 0f);
                        uv[i + 2] = new Vector2(1f - (((LaneWidth * 0.5f) + LaneWidth) / iWidth), 0f);
                        //Debug.Log (roadIntersection.name + " " + uv[i+2].x);
                        uv[i + 4] = new Vector2(0f, 0.125f);
                        uv[i + 6] = new Vector2(1f, 0.125f);
                    }
                    else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.TurnLane)
                    {
                        uv[i] = new Vector2((LaneWidth * 0.5f) / iWidth, 0f);
                        uv[i + 2] = new Vector2(1f - ((LaneWidth * 0.5f) / iWidth), 0f);
                        uv[i + 4] = new Vector2(0f, 0.125f);
                        uv[i + 6] = new Vector2(1f, 0.125f);
                    }
                    else if (_intersection.roadType == RoadIntersection.RoadTypeEnum.NoTurnLane)
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
        private static void SetVectorHeight2(ref Vector3 _worldVector, ref float _p, ref List<KeyValuePair<float, float>> _list, ref SplineC _spline)
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

            //if(p > 0.95f && RootUtils.IsApproximately(cValue,0f,0.001f)){
            //    float DeadValue = 0f;
            //    Vector3 tPos = tSpline.GetSplineValue(p,false);
            //    if(!tSpline.IsNearIntersection(ref tPos,ref DeadValue)){
            //        cValue = tList[tList.Count-1].Value;
            //    }
            //}

            //Zero protection: 
            if (RootUtils.IsApproximately(cValue, 0f, 0.001f) && _worldVector.y > 0f)
            {
                cValue = _worldVector.y - 0.35f;
            }

            _worldVector.y = cValue;
        }
        #endregion
    }
}
