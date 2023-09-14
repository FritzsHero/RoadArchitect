#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using RoadArchitect.Splination;
using RoadArchitect.EdgeObjects;
#endregion


namespace RoadArchitect
{
    public class SplineN : MonoBehaviour
    {
        #region "Vars"
        /// <summary> Stores the position data </summary>
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 tangent;
        public Vector2 easeIO;
        public float dist = 0f;
        public float time = 0f;
        public float nextTime = 1f;
        public Vector3 nextTan = default(Vector3);
        public float oldTime = 0f;

        public float tempSegmentTime = 0f;
        public bool isSpecialEndNode = false;
        public SplineN specialNodeCounterpart = null;
        public SplineN specialNodeCounterpartMaster = null;
        /// <summary> Connected nodes array </summary>
        public SplineN[] originalConnectionNodes = null;

        public bool isSpecialEndNodeIsStart = false;
        public bool isSpecialEndNodeIsEnd = false;
        public bool isSpecialIntersection = false;
        public bool isSpecialRoadConnPrimary = false;
        public bool isRoadCut = false;
        public float minSplination = 0f;
        public float maxSplination = 1f;

        public int idOnSpline = -1;
        public SplineC spline;
        //Unique ID
        public string uID;
        public SplineN intersectionOtherNode;
        public string gradeToNext;
        public string gradeToPrev;
        public float gradeToNextValue;
        public float gradeToPrevValue;
        public float initialRoadHeight = -1f;
        //Navigation:
        public bool isNeverIntersect = false;
        /// <summary> Is this node used by an intersection </summary>
        public bool isIntersection = false;
        /// <summary> Defines end of road, if special end or start it is the second node/second last node </summary>
        public bool isEndPoint = false;
        public int id = 0;
        public int intersectionOtherNodeID = 0;
        /// <summary> Contains previous and next node ids </summary>
        public List<int> connectedID;
        /// <summary> Contains previous and next node </summary>
        public List<SplineN> connectedNode;
        public bool isIgnore = false;


        #region "Tunnels"
        public bool isTunnel = false;
        public bool isTunnelStart = false;
        public bool isTunnelEnd = false;
        public bool isTunnelMatched = false;
        public SplineN tunnelCounterpartNode = null;
        #endregion


        //Bridges:
        public bool isBridge = false;
        public bool isBridgeStart = false;
        public bool isBridgeEnd = false;
        public bool isBridgeMatched = false;
        public SplineN bridgeCounterpartNode = null;

        public RoadIntersection intersection = null;
        public iConstructionMaker intersectionConstruction;
        #endregion


        #region "Edge Objects"
        public List<EdgeObjectMaker> EdgeObjects;


        public void SetupEdgeObjects(bool _isCollecting = true)
        {
            if (EdgeObjects == null)
            {
                EdgeObjects = new List<EdgeObjectMaker>();
            }
            int eCount = EdgeObjects.Count;
            EdgeObjectMaker EOM = null;
            for (int index = 0; index < eCount; index++)
            {
                EOM = EdgeObjects[index];
                EOM.node = this;
                EOM.Setup(_isCollecting);
            }
        }


        public EdgeObjectMaker AddEdgeObject()
        {
            EdgeObjectMaker EOM = new EdgeObjectMaker();
            EOM.node = this;
            EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            EOM.startPos = spline.GetSplineValue(EOM.startTime);
            EOM.endPos = spline.GetSplineValue(EOM.endTime);
            EdgeObjects.Add(EOM);
            return EOM;
        }


        public void CheckRenameEdgeObject(EdgeObjectMaker _eom)
        {
            // We have _eom already in EdgeObjects
            List<string> names = new List<string>(EdgeObjects.Count - 1);
            for (int i = 0; i < EdgeObjects.Count; i++)
            {
                if (ReferenceEquals(_eom, EdgeObjects[i]))
                {
                    continue;
                }

                names.Add(EdgeObjects[i].objectName);
            }

            bool isNotUnique = true;
            string name = _eom.objectName;
            int counter = 1;
            while (isNotUnique)
            {
                if (names.Contains(_eom.objectName))
                {
                    _eom.objectName = name + counter.ToString();
                    counter++;
                    continue;
                }

                break;
            }
        }


        public void EdgeObjectQuickAdd(string _name)
        {
            EdgeObjectMaker EOM = AddEdgeObject();
            EOM.LoadFromLibrary(_name, true);
            EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            EOM.node = this;
            EOM.Setup();
        }


        public void RemoveEdgeObject(int _index = -1, bool _isSkippingUpdate = false)
        {
            if (EdgeObjects == null)
            {
                return;
            }
            if (EdgeObjects.Count == 0)
            {
                return;
            }
            if (_index < 0)
            {
                if (EdgeObjects.Count > 0)
                {
                    EdgeObjects[EdgeObjects.Count - 1].ClearEOM();
                    EdgeObjects.RemoveAt(EdgeObjects.Count - 1);
                }
            }
            else
            {
                if (EdgeObjects.Count > _index)
                {
                    EdgeObjects[_index].ClearEOM();
                    EdgeObjects.RemoveAt(_index);
                }
            }
            if (!_isSkippingUpdate)
            {
                SetupEdgeObjects();
            }
        }


        public void RemoveAllEdgeObjects(bool _isSkippingUpdate = false)
        {
            if (EdgeObjects == null)
            {
                return;
            }

            while (EdgeObjects.Count > 0)
            {
                RemoveEdgeObject(-1, _isSkippingUpdate);
            }
        }


        public void CopyEdgeObject(int _index)
        {
            EdgeObjectMaker EOM = EdgeObjects[_index].Copy();
            EdgeObjects.Add(EOM);
            CheckRenameEdgeObject(EOM);
            SetupEdgeObjects();
        }


        public void EdgeObjectLoadFromLibrary(int _i, string _name)
        {
            if (EdgeObjects == null)
            {
                EdgeObjects = new List<EdgeObjectMaker>();
            }
            int eCount = EdgeObjects.Count;
            if (_i > -1 && _i < eCount)
            {
                EdgeObjectMaker EOM = EdgeObjects[_i];
                EOM.LoadFromLibrary(_name);
                EOM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
                EOM.node = this;
                EOM.Setup();
            }
        }


        public void DetectInvalidEdgeObjects()
        {
            int mCount = EdgeObjects.Count;
            List<int> InvalidList = new List<int>();

            for (int index = 0; index < mCount; index++)
            {
                if (EdgeObjects[index].edgeObject == null)
                {
                    InvalidList.Add(index);
                }
            }

            for (int index = (InvalidList.Count - 1); index >= 0; index--)
            {
                RemoveEdgeObject(InvalidList[index], true);
            }

            SetupEdgeObjects();
        }
        #endregion


        #region "Extruded objects"
        public List<SplinatedMeshMaker> SplinatedObjects;


        public void SetupSplinatedMeshes(bool _isCollecting = true)
        {
            if (SplinatedObjects == null)
            {
                SplinatedObjects = new List<SplinatedMeshMaker>();
            }
            int eCount = SplinatedObjects.Count;
            SplinatedMeshMaker SMM = null;
            for (int index = 0; index < eCount; index++)
            {
                SMM = SplinatedObjects[index];
                SMM.Setup(true, _isCollecting);
            }
        }


        public int SplinatedMeshGetIndex(ref string _uID)
        {
            if (SplinatedObjects == null)
            {
                SplinatedObjects = new List<SplinatedMeshMaker>();
            }
            int eCount = SplinatedObjects.Count;
            SplinatedMeshMaker SMM = null;
            for (int index = 0; index < eCount; index++)
            {
                SMM = SplinatedObjects[index];
                if (string.CompareOrdinal(SMM.uID, _uID) == 0)
                {
                    return index;
                }
            }
            return -1;
        }


        public void SetupSplinatedMesh(int _i, bool _isGettingStrings = false)
        {
            if (SplinatedObjects == null)
            {
                SplinatedObjects = new List<SplinatedMeshMaker>();
            }
            int eCount = SplinatedObjects.Count;
            if (_i > -1 && _i < eCount)
            {
                SplinatedMeshMaker SMM = SplinatedObjects[_i];
                SMM.Setup(_isGettingStrings);
            }
        }


        public SplinatedMeshMaker AddSplinatedObject()
        {
            if (SplinatedObjects == null)
            {
                SplinatedObjects = new List<SplinatedMeshMaker>();
            }
            SplinatedMeshMaker SMM = new SplinatedMeshMaker();
            SMM.Init(spline, this, transform);
            SplinatedObjects.Add(SMM);
            SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            SMM.StartPos = spline.GetSplineValue(SMM.StartTime);
            SMM.EndPos = spline.GetSplineValue(SMM.EndTime);
            return SMM;
        }


        public void CheckRenameSplinatedObject(SplinatedMeshMaker _smm)
        {
            // We have _smm already in SplinatedObjects
            List<string> names = new List<string>(SplinatedObjects.Count - 1);
            for(int i = 0; i < SplinatedObjects.Count; i++)
            {
                if (ReferenceEquals(_smm, SplinatedObjects[i]))
                {
                    continue;
                }
                
                names.Add(SplinatedObjects[i].objectName);
            }

            bool isNotUnique = true;
            string name = _smm.objectName;
            int counter = 1;
            while(isNotUnique)
            {
                if (names.Contains(_smm.objectName))
                {
                    _smm.objectName = name + counter.ToString();
                    counter++;
                    continue;
                }

                break;
            }
        }


        public void SplinatedObjectQuickAdd(string _name)
        {
            SplinatedMeshMaker SMM = AddSplinatedObject();
            SMM.LoadFromLibrary(_name, true);
            SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
            SMM.Setup(true);
        }


        public void SplinatedObjectLoadFromLibrary(int _i, string _name)
        {
            if (SplinatedObjects == null)
            {
                SplinatedObjects = new List<SplinatedMeshMaker>();
            }
            int eCount = SplinatedObjects.Count;
            if (_i > -1 && _i < eCount)
            {
                SplinatedMeshMaker SMM = SplinatedObjects[_i];
                SMM.SetDefaultTimes(isEndPoint, time, nextTime, idOnSpline, spline.distance);
                SMM.LoadFromLibrary(_name);
                SMM.Setup(true);
            }
        }


        public void CopySplinatedObject(ref SplinatedMeshMaker _SMM)
        {
            SplinatedMeshMaker SMM = _SMM.Copy();
            SplinatedObjects.Add(SMM);
            CheckRenameSplinatedObject(SMM);
            SetupSplinatedMeshes();
        }


        public void RemoveSplinatedObject(int _index = -1, bool _isSkippingUpdate = false)
        {
            if (SplinatedObjects == null)
            {
                return;
            }
            if (SplinatedObjects.Count == 0)
            {
                return;
            }
            SplinatedMeshMaker SMM = null;
            if (_index < 0)
            {
                if (SplinatedObjects.Count > 0)
                {
                    SMM = SplinatedObjects[SplinatedObjects.Count - 1];
                    SMM.Kill();
                    SplinatedObjects.RemoveAt(SplinatedObjects.Count - 1);
                    SMM = null;
                }
            }
            else
            {
                if (SplinatedObjects.Count > _index)
                {
                    SMM = SplinatedObjects[_index];
                    SMM.Kill();
                    SplinatedObjects.RemoveAt(_index);
                    SMM = null;
                }
            }
            if (!_isSkippingUpdate)
            {
                SetupSplinatedMeshes();
            }
        }


        public void RemoveAllSplinatedObjects(bool _isSkippingUpdate = false)
        {
            if (SplinatedObjects == null)
            {
                return;
            }

            while (SplinatedObjects.Count > 0)
            {
                RemoveSplinatedObject(-1, _isSkippingUpdate);
            }
        }


        public void DetectInvalidSplinatedObjects()
        {
            int mCount = SplinatedObjects.Count;
            List<int> InvalidList = new List<int>();

            for (int index = 0; index < mCount; index++)
            {
                if (SplinatedObjects[index].Output == null)
                {
                    InvalidList.Add(index);
                }
            }

            for (int index = (InvalidList.Count - 1); index >= 0; index--)
            {
                RemoveSplinatedObject(InvalidList[index], true);
            }

            SetupSplinatedMeshes();
        }
        #endregion


        public void LoadWizardObjectsFromLibrary(string _fileName, bool _isDefault, bool _isBridge)
        {
            if (_isBridge)
            {
                RemoveAllSplinatedObjects(true);
                RemoveAllEdgeObjects(true);
            }
            RoadUtility.LoadNodeObjects(_fileName, this, _isDefault, _isBridge);
        }


        public void Setup(Vector3 _pos, Quaternion _rot, Vector2 _io, float _time, string _name)
        {
            pos = _pos;
            rot = _rot;
            easeIO = _io;
            time = _time;
            name = _name;
            if (EdgeObjects == null)
            {
                EdgeObjects = new List<EdgeObjectMaker>();
            }
        }


        #region "Gizmos"
        //	private void TerrainDebugging(){
        //			Construction3DTri tTri = null;
        //			Vector3 tHeightVec = new Vector3(0f,10f,0f);
        //			if(tTriList != null){
        //				
        //
        ////				bool bOddToggle = false;
        ////				for(int i=0;i<tTriList.Count;i+=2){
        //////					if(i < 210){ continue; }
        //////					if(i > 230){ break; }
        ////					if(i < 1330){ continue; }
        ////					if(i > 1510 || i > (tTriList.Count-10)){ break; }
        ////					tTri = tTriList[i];
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1,tTri.P2);
        ////					Gizmos.DrawLine(tTri.P1,tTri.P3);
        ////					Gizmos.DrawLine(tTri.P2,tTri.P3);
        //////					Gizmos.color = new Color(0f,1f,0f,1f);
        //////					Gizmos.DrawLine(tTri.pMiddle,(tTri.normal*100f)+tTri.pMiddle);
        ////
        ////					
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P3+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P3+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawLine(tTri.pMiddle+tHeightVec,(tTri.normal*100f)+tTri.pMiddle+tHeightVec);
        ////					
        //////					
        ////					
        ////					tTri = tTriList[i+1];
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1,tTri.P2);
        ////					Gizmos.DrawLine(tTri.P1,tTri.P3);
        ////					Gizmos.DrawLine(tTri.P2,tTri.P3);
        ////					
        ////					if(bOddToggle){
        ////						Gizmos.color = new Color(0f,1f,1f,1f);
        ////					}else{
        ////						Gizmos.color = new Color(1f,1f,0f,1f);
        ////					}
        ////					
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P1+tHeightVec,tTri.P3+tHeightVec);
        ////					Gizmos.DrawLine(tTri.P3+tHeightVec,tTri.P2+tHeightVec);
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawLine(tTri.pMiddle+tHeightVec,(tTri.normal*100f)+tTri.pMiddle+tHeightVec);
        ////					
        ////	
        ////					
        ////					bOddToggle = !bOddToggle;
        //////					Gizmos.DrawCube(tRectList[i].pMiddle,new Vector3(1f,0.5f,1f));
        ////				}
        //				
        //				
        ////				Gizmos.color = new Color(0f,1f,0f,1f);
        ////				Terrain tTerrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        ////				Vector3 HMVect = default(Vector3);
        ////				float tHeight = 0f;
        ////				for(int i=0;i<tHMList.Count;i++){
        ////					Gizmos.color = new Color(0f,1f,0f,1f);
        ////					Gizmos.DrawCube(tHMList[i] + new Vector3(0f,1f,0f),new Vector3(0.1f,2f,0.1f));
        ////				
        ////					if(tTerrain){
        ////						tHeight = tTerrain.SampleHeight(tHMList[i]) + tTerrain.transform.position.y;
        ////						
        ////						if(tHeight != tHMList[i].y){
        ////							HMVect = new Vector3(tHMList[i].x,tHeight,tHMList[i].z);
        ////							
        ////							if(Mathf.Approximately(9.584141f,tHMList[i].y)){
        ////								int sdagsdgsd =1;	
        ////							}
        ////							
        ////							Gizmos.color = new Color(0f,0f,1f,1f);
        ////							Gizmos.DrawWireCube(HMVect + new Vector3(0f,1f,0f),new Vector3(0.15f,2f,0.15f));
        ////						}
        ////					}
        ////				}
        //			}
        //			
        ////			Vector3 P1 = new Vector3(480.7f,50f,144.8f);
        ////			Vector3 P2 = new Vector3(517.3f,60f,128.9f);
        ////			Vector3 P3 = new Vector3(518.8f,60f,132.7f);
        ////			Vector3 P4 = new Vector3(481.3f,50f,146.4f);
        ////			
        ////			
        ////			Gizmos.color = new Color(1f,0f,0f,1f);
        ////			Gizmos.DrawCube(P1,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P2,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P3,new Vector3(1f,1f,1f));
        ////			Gizmos.DrawCube(P4,new Vector3(1f,1f,1f));
        ////			
        ////			tRect = new Construction3DRect(P1,P2,P3,P4);
        ////
        ////			Gizmos.color = new Color(0f,0f,1f,1f);
        ////			Gizmos.DrawCube(tRect.pMiddle,new Vector3(0.1f,0.1f,0.1f));
        ////			
        ////			//This creates normalized line:
        ////			Gizmos.color = new Color(0f,1f,0f,1f);
        ////			Vector3 tVect2 = (tRect.normal.normalized*100f)+tRect.pMiddle;
        ////			Gizmos.DrawLine(tRect.pMiddle,tVect2);
        ////			
        ////			//This creates emulated terrain point and straight up line:
        ////			Gizmos.color = new Color(0f,1f,1f,1f);
        ////			Vector3 F1 = new Vector3(500f,0f,138.5f);
        ////			Gizmos.DrawLine(F1,F1+ new Vector3(0f,100f,0f));
        ////			
        ////			//This creates diagonal line of plane.
        ////			Gizmos.color = new Color(1f,0f,1f,1f);
        ////			Gizmos.DrawLine(P1,P3);
        ////			Gizmos.DrawLine(P2,P4);
        ////			
        ////			//This creates intersection point:
        ////			Vector3 tPos = default(Vector3);
        ////			LinePlaneIntersection(out tPos,F1,Vector3.up,tRect.normal.normalized,tRect.pMiddle);
        ////			Gizmos.color = new Color(1f,1f,0f,1f);
        ////			Gizmos.DrawCube(tPos,new Vector3(0.3f,0.3f,0.3f));
        //	}
        //	

        public List<Construction3DTri> tTriList;
        public List<Vector3> tHMList;


        private void OnDrawGizmos()
        {
            DrawGizmos(false);
        }


        private void OnDrawGizmosSelected()
        {
            DrawGizmos(true);
        }


        private void DrawGizmos(bool _isSelected)
        {
            if (!spline.road.isGizmosEnabled == true)
            {
                return;
            }
            if (isIgnore)
            {
                return;
            }
            if (isIntersection)
            {
                return;
            }
            if (isSpecialEndNode)
            {
                return;
            }
            if (isSpecialEndNodeIsEnd || isSpecialEndNodeIsStart)
            {
                return;
            }


            if (_isSelected)
            {
                Gizmos.color = spline.road.selectedColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 6.25f, 0f), new Vector3(3.5f, 12.5f, 3.5f));
                return;
            }

            // Not Selected
            if (isSpecialRoadConnPrimary)
            {
                Gizmos.color = spline.road.Color_NodeConnColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 7.5f, 0f), new Vector3(5f, 15f, 5f));
            }
            else
            {
                Gizmos.color = spline.road.defaultNodeColor;
                Gizmos.DrawCube(transform.position + new Vector3(0f, 6f, 0f), new Vector3(2f, 11f, 2f));
            }
        }
        #endregion


        #region "Grade"
        public void SetGradePercent(int _nodeCount)
        {
            Vector3 P1 = default(Vector3);
            Vector3 P2 = default(Vector3);
            float dist;
            float grade;
            bool isNone = false;

            if (_nodeCount < 2)
            {
                gradeToPrev = "NA";
                gradeToNext = "NA";
                gradeToNextValue = 0f;
                gradeToPrevValue = 0f;
            }

            if (!isEndPoint && !isSpecialEndNode && _nodeCount > 1 && ((idOnSpline + 1) < spline.nodes.Count))
            {
                P1 = pos;
                P2 = spline.nodes[idOnSpline + 1].pos;

                if (isNone)
                {
                    grade = 0f;
                }
                else
                {
                    dist = Vector2.Distance(new Vector2(P1.x, P1.z), new Vector2(P2.x, P2.z));
                    grade = ((P2.y - P1.y) / dist) * 100;
                }
                gradeToNextValue = grade;
                gradeToNext = grade.ToString("0.##\\%");
            }
            else
            {
                gradeToNext = "NA";
                gradeToNextValue = 0f;
            }

            if (idOnSpline > 0 && !isSpecialEndNode && _nodeCount > 1 && ((idOnSpline - 1) >= 0))
            {
                P1 = pos;
                P2 = spline.nodes[idOnSpline - 1].pos;

                if (isNone)
                {
                    grade = 0f;
                }
                else
                {
                    dist = Vector2.Distance(new Vector2(P1.x, P1.z), new Vector2(P2.x, P2.z));
                    grade = ((P2.y - P1.y) / dist) * 100;
                }
                gradeToPrevValue = grade;
                gradeToPrev = grade.ToString("0.##\\%");
            }
            else
            {
                gradeToPrev = "NA";
                gradeToPrevValue = 0f;
            }
        }


        public Vector3 FilterMaxGradeHeight(Vector3 _pos, out float _minY, out float _maxY)
        {
            Vector3 tVect = _pos;
            tVect.y = pos.y;
            float CurrentDistance = Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(_pos.x, _pos.z));
            //		float CurrentDistance2 = Vector3.Distance(pos,tVect);
            //		float CurrentYDiff = tPos.y - pos.y;
            //		float CurrentGrade = CurrentYDiff/CurrentDistance;
            //Get max/min grade height position for this currrent tDist distance:
            _maxY = (spline.road.maxGrade * CurrentDistance) + pos.y;
            _minY = pos.y - (spline.road.maxGrade * CurrentDistance);

            //(tPos.y-pos.y)/CurrentDistance


            //		float DifferenceFromMax = -1;
            if (_pos.y > _maxY)
            {
                //			DifferenceFromMax = tPos.y - MaximumY;
                _pos.y = _maxY;
            }
            if (_pos.y < _minY)
            {
                //			DifferenceFromMax = MinimumY - tPos.y;
                _pos.y = _minY;
            }

            return _pos;
        }


        public void EnsureGradeValidity(int _iStart = -1, bool _isAddToEnd = false)
        {
            if (spline == null)
            {
                return;
            }
            SplineN PrevNode = null;
            SplineN NextNode = null;

            if (_isAddToEnd && spline.GetNodeCount() > 0)
            {
                PrevNode = spline.nodes[spline.GetNodeCount() - 1];
            }
            else
            {
                if (_iStart == -1)
                {
                    PrevNode = spline.GetPrevLegitimateNode(idOnSpline);
                }
                else
                {
                    PrevNode = spline.GetPrevLegitimateNode(_iStart);
                }
            }
            if (PrevNode == null)
            {
                return;
            }
            Vector3 tVect = transform.position;

            float tMinY1 = 0f;
            float tMinY2 = 0f;
            float tMaxY1 = 0f;
            float tMaxY2 = 0f;
            if (PrevNode != null)
            {
                PrevNode.FilterMaxGradeHeight(tVect, out tMinY1, out tMaxY1);
            }
            if (NextNode != null)
            {
                NextNode.FilterMaxGradeHeight(tVect, out tMinY2, out tMaxY2);
            }

            bool bPrevNodeGood = false;
            bool bNextNodeGood = false;

            if (tVect.y > tMinY1 && tVect.y < tMaxY1)
            {
                bPrevNodeGood = true;
            }
            if (tVect.y > tMinY2 && tVect.y < tMaxY2)
            {
                bNextNodeGood = true;
            }

            if (!bPrevNodeGood && !bNextNodeGood && PrevNode != null && NextNode != null)
            {
                float tMaxY3 = Mathf.Min(tMaxY1, tMaxY2);
                float tMinY3 = Mathf.Max(tMinY1, tMinY2);
                if (tVect.y < tMinY3)
                {
                    tVect.y = tMinY3;
                }
                else if (tVect.y > tMaxY3)
                {
                    tVect.y = tMaxY3;
                }
            }
            else
            {
                if (!bPrevNodeGood && PrevNode != null)
                {
                    if (tVect.y < tMinY1)
                    {
                        tVect.y = tMinY1;
                    }
                    else if (tVect.y > tMaxY1)
                    {
                        tVect.y = tMaxY1;
                    }
                }
                else if (!bNextNodeGood && NextNode != null)
                {
                    if (tVect.y < tMinY2)
                    {
                        tVect.y = tMinY2;
                    }
                    else if (tVect.y > tMaxY2)
                    {
                        tVect.y = tMaxY2;
                    }
                }
            }

            transform.position = tVect;
        }
        #endregion


        #region "Util"
        public void ResetNavigationData()
        {
            connectedID = null;
            connectedID = new List<int>();
            connectedNode = null;
            connectedNode = new List<SplineN>();
        }


        public void BreakConnection()
        {
            SplineN tNode2 = specialNodeCounterpart;

            if (isSpecialEndNodeIsStart)
            {
                spline.isSpecialStartControlNode = false;
                spline.isSpecialEndNodeIsStartDelay = false;
            }
            else if (isSpecialEndNodeIsEnd)
            {
                spline.isSpecialEndControlNode = false;
                spline.isSpecialEndNodeIsEndDelay = false;
            }
            if (tNode2.isSpecialEndNodeIsStart)
            {
                tNode2.spline.isSpecialStartControlNode = false;
                tNode2.spline.isSpecialEndNodeIsStartDelay = false;
            }
            else if (tNode2.isSpecialEndNodeIsEnd)
            {
                tNode2.spline.isSpecialEndControlNode = false;
                tNode2.spline.isSpecialEndNodeIsEndDelay = false;
            }

            specialNodeCounterpart = null;
            isSpecialEndNode = false;
            isSpecialEndNodeIsEnd = false;
            isSpecialEndNodeIsStart = false;
            isSpecialRoadConnPrimary = false;
            tNode2.specialNodeCounterpart = null;
            tNode2.isSpecialEndNode = false;
            tNode2.isSpecialEndNodeIsEnd = false;
            tNode2.isSpecialEndNodeIsStart = false;
            tNode2.isSpecialRoadConnPrimary = false;

            tNode2.specialNodeCounterpartMaster.isSpecialRoadConnPrimary = false;
            specialNodeCounterpartMaster.isSpecialRoadConnPrimary = false;
            try
            {
                Object.DestroyImmediate(tNode2.transform.gameObject);
                Object.DestroyImmediate(transform.gameObject);
            }
            catch (MissingReferenceException)
            {

            }
        }


        public void SetupSplinationLimits()
        {
            //Disallowed nodes:
            if (!CanSplinate())
            {
                minSplination = time;
                maxSplination = time;
                return;
            }

            //Figure out min splination:
            SplineN node = null;
            minSplination = time;
            for (int index = idOnSpline; index >= 0; index--)
            {
                node = spline.nodes[index];
                if (node.CanSplinate())
                {
                    minSplination = node.time;
                }
                else
                {
                    break;
                }
            }

            //Figure out max splination:
            maxSplination = time;
            int nodeCount = spline.GetNodeCount();
            for (int index = idOnSpline; index < nodeCount; index++)
            {
                node = spline.nodes[index];
                if (node.CanSplinate())
                {
                    maxSplination = node.time;
                }
                else
                {
                    break;
                }
            }
        }
        #endregion


        #region "Cut materials storage and setting"
        public GameObject roadCutWorld = null;
        public GameObject shoulderCutRWorld = null;
        public GameObject shoulderCutLWorld = null;
        public GameObject roadCutMarker = null;
        public GameObject shoulderCutRMarker = null;
        public GameObject shoulderCutLMarker = null;

        private Material[] roadCutWorldMats;
        private Material[] shoulderCutRWorldMats;
        private Material[] shoulderCutLWorldMats;
        private Material[] roadCutMarkerMats;
        private Material[] shoulderCutRMarkerMats;
        private Material[] shoulderCutLMarkerMats;
        private PhysicMaterial roadCutPhysicMat;
        private PhysicMaterial shoulderCutRPhysicMat;
        private PhysicMaterial shoulderCutLPhysicMat;


        /// <summary> Stores the cut materials. For use in UpdateCuts(). See UpdateCuts() in this code file for further description of this system. </summary>
        public void StoreCuts()
        {
            //Buffers:
            MeshRenderer MR = null;
            MeshCollider MC = null;

            //World cuts first:
            if (roadCutWorld != null)
            {
                MR = roadCutWorld.GetComponent<MeshRenderer>();
                MC = roadCutWorld.GetComponent<MeshCollider>();
                if (MR != null)
                {
                    roadCutWorldMats = MR.sharedMaterials;
                }
                if (MC != null)
                {
                    roadCutPhysicMat = MC.material;
                    roadCutPhysicMat.name = roadCutPhysicMat.name.Replace(" (Instance)", "");
                }
                //Nullify reference only
                roadCutWorld = null;
            }
            if (shoulderCutRWorld != null)
            {
                MR = shoulderCutRWorld.GetComponent<MeshRenderer>();
                MC = shoulderCutRWorld.GetComponent<MeshCollider>();
                if (MR != null)
                {
                    shoulderCutRWorldMats = MR.sharedMaterials;
                }
                if (MC != null)
                {
                    shoulderCutRPhysicMat = MC.material;
                    shoulderCutRPhysicMat.name = shoulderCutRPhysicMat.name.Replace(" (Instance)", "");
                }
                shoulderCutRWorld = null;
            }
            if (shoulderCutLWorld != null)
            {
                MR = shoulderCutLWorld.GetComponent<MeshRenderer>();
                MC = shoulderCutLWorld.GetComponent<MeshCollider>();
                if (MR != null)
                {
                    shoulderCutLWorldMats = MR.sharedMaterials;
                }
                if (MC != null)
                {
                    shoulderCutLPhysicMat = MC.material;
                    shoulderCutLPhysicMat.name = shoulderCutLPhysicMat.name.Replace(" (Instance)", "");
                }
                shoulderCutLWorld = null;
            }
            //Markers:
            if (roadCutMarker != null)
            {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (MR != null)
                {
                    roadCutMarkerMats = MR.sharedMaterials;
                }
                roadCutMarker = null;
            }
            if (shoulderCutRMarker != null)
            {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (MR != null)
                {
                    shoulderCutRMarkerMats = MR.sharedMaterials;
                }
                shoulderCutRMarker = null;
            }
            if (shoulderCutLMarker != null)
            {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (MR != null)
                {
                    shoulderCutLMarkerMats = MR.sharedMaterials;
                }
                shoulderCutLMarker = null;
            }
        }


        /// <summary>
        /// Updates the cut materials. Called upon creation of the cuts to set the newly cut materials to previously set materials.
        /// For instance, if the user set a material on a road cut, and then regenerated the road, this function will apply the mats that the user applied.
        /// </summary>
        public void UpdateCuts()
        {
            //Buffers:
            MeshRenderer MR = null;
            MeshCollider MC = null;

            #region "Physic Materials"
            //World:
            if (roadCutPhysicMat != null && roadCutWorld)
            {
                MC = roadCutWorld.GetComponent<MeshCollider>();
                if (MC != null)
                {
                    MC.material = roadCutPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }

            if (shoulderCutRPhysicMat != null && shoulderCutRWorld)
            {
                MC = shoulderCutRWorld.GetComponent<MeshCollider>();
                if (MC != null)
                {
                    MC.material = shoulderCutRPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }

            if (shoulderCutLPhysicMat != null && shoulderCutLWorld)
            {
                MC = shoulderCutLWorld.GetComponent<MeshCollider>();
                if (MC != null)
                {
                    MC.material = shoulderCutLPhysicMat;
                    MC.material.name = MC.material.name.Replace(" (Instance)", "");
                }
            }
            #endregion

            #region "Get or Add MeshRenderer"
            if (roadCutWorldMats != null && roadCutWorldMats.Length > 0 && roadCutWorld)
            {
                MR = roadCutWorld.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    roadCutWorld.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = roadCutWorldMats;
                }
            }
            if (shoulderCutRWorldMats != null && shoulderCutRWorldMats.Length > 0 && shoulderCutRWorld)
            {
                MR = shoulderCutRWorld.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    shoulderCutRWorld.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = shoulderCutRWorldMats;
                }
            }
            if (shoulderCutLWorldMats != null && shoulderCutLWorldMats.Length > 0 && shoulderCutLWorld)
            {
                MR = shoulderCutLWorld.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    shoulderCutLWorld.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = shoulderCutLWorldMats;
                }
            }


            //Markers:
            if (roadCutMarkerMats != null && roadCutMarkerMats.Length > 0 && roadCutMarker)
            {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    roadCutMarker.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = roadCutMarkerMats;
                }
            }
            if (shoulderCutRMarkerMats != null && shoulderCutRMarkerMats.Length > 0 && shoulderCutRMarker)
            {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    shoulderCutRMarker.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = shoulderCutRMarkerMats;
                }
            }
            if (shoulderCutLMarkerMats != null && shoulderCutLMarkerMats.Length > 0 && shoulderCutLMarker)
            {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (!MR)
                {
                    shoulderCutLMarker.AddComponent<MeshRenderer>();
                }
                if (MR != null)
                {
                    MR.materials = shoulderCutLMarkerMats;
                }
            }
            #endregion

            #region "Destroy if empty"
            if (roadCutMarker != null)
            {
                MR = roadCutMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null)
                {
                    Object.DestroyImmediate(roadCutMarker);
                }
            }
            if (shoulderCutRMarker != null)
            {
                MR = shoulderCutRMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null)
                {
                    Object.DestroyImmediate(shoulderCutRMarker);
                }
            }
            if (shoulderCutLMarker != null)
            {
                MR = shoulderCutLMarker.GetComponent<MeshRenderer>();
                if (MR == null || MR.sharedMaterial == null)
                {
                    Object.DestroyImmediate(shoulderCutLMarker);
                }
            }
            #endregion
        }


        /// <summary> Clears the cut materials </summary>
        public void ClearCuts()
        {
            roadCutWorldMats = null;
            shoulderCutRWorldMats = null;
            shoulderCutLWorldMats = null;
            roadCutMarkerMats = null;
            shoulderCutRMarkerMats = null;
            shoulderCutLMarkerMats = null;
            roadCutPhysicMat = null;
            shoulderCutRPhysicMat = null;
            shoulderCutLPhysicMat = null;
        }
        #endregion


        #region "Bridges"
        public void BridgeToggleStart()
        {
            //If switching to end, find associated bridge 
            if (isBridgeStart)
            {
                BridgeStart();
            }
            else
            {
                BridgeDestroy();
            }
        }


        public void BridgeToggleEnd()
        {
            //If switching to end, find associated bridge 
            if (isBridgeEnd)
            {
                int nodeCount = spline.GetNodeCount();
                SplineN node = null;
                for (int i = 1; i < (nodeCount - 1); i++)
                {
                    node = spline.nodes[i];
                    if (node.isBridgeStart && !node.isBridgeMatched)
                    {
                        node.BridgeToggleStart();
                        if (node.isBridgeMatched && node.bridgeCounterpartNode == this)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                BridgeDestroy();
            }
        }


        private void BridgeStart()
        {
            //Cycle through nodes until you find another end or another start (in this case, no creation, encountered another bridge):
            int EndID = idOnSpline + 1;
            int nodeCount = spline.GetNodeCount();
            if (isEndPoint)
            {
                //Attempted to make end point node a bridge node:
                isBridgeStart = false;
                return;
            }
            if (EndID >= nodeCount)
            {
                //Attempted to make last node a bridge node:
                isBridgeStart = false;
                return;
            }
            else if (idOnSpline == 0)
            {
                //Attempted to make first node a bridge node:
                isBridgeStart = false;
                return;
            }

            isBridgeMatched = false;
            bridgeCounterpartNode = null;
            int StartI = idOnSpline + 1;
            SplineN tNode = null;
            for (int i = StartI; i < nodeCount; i++)
            {
                tNode = spline.nodes[i];
                if (tNode.isIntersection)
                {
                    //Encountered intersection. End search.
                    return;
                }
                if (tNode.isBridgeStart)
                {
                    //Encountered another bridge. Return:
                    return;
                }
                if (tNode.isIgnore)
                {
                    continue;
                }
                if (tNode.isBridgeEnd)
                {
                    isBridgeMatched = true;
                    tNode.isBridgeMatched = true;
                    bridgeCounterpartNode = tNode;
                    tNode.bridgeCounterpartNode = this;
                    spline.TriggerSetup();
                    return;
                }
            }
        }


        private void BridgeDestroy()
        {
            if (bridgeCounterpartNode != null)
            {
                bridgeCounterpartNode.BridgeResetValues();
            }
            BridgeResetValues();
            spline.TriggerSetup();
        }


        public void BridgeResetValues()
        {
            isBridge = false;
            isBridgeStart = false;
            isBridgeEnd = false;
            isBridgeMatched = false;
            bridgeCounterpartNode = null;
        }


        public bool CanBridgeStart()
        {
            if (isBridgeStart)
            {
                return true;
            }
            if (isBridgeEnd)
            {
                return false;
            }
            if (isEndPoint)
            {
                return false;
            }

            int mCount = spline.GetNodeCount();

            if (idOnSpline > 0)
            {
                if (spline.nodes[idOnSpline - 1].isBridgeStart)
                {
                    return false;
                }
            }

            if (idOnSpline < (mCount - 1))
            {
                if (spline.nodes[idOnSpline + 1].isBridgeStart)
                {
                    return false;
                }

                if (spline.isSpecialEndControlNode)
                {
                    if ((mCount - 3 > 0) && idOnSpline == mCount - 3)
                    {
                        return false;
                    }
                }
                else
                {
                    if ((mCount - 2 > 0) && idOnSpline == mCount - 2)
                    {
                        return false;
                    }
                }
            }

            if (spline.IsInBridge(time))
            {
                return false;
            }

            return true;
        }


        public bool CanBridgeEnd()
        {
            if (isBridgeEnd)
            {
                return true;
            }
            if (isBridgeStart)
            {
                return false;
            }
            if (isEndPoint)
            {
                return false;
            }

            int mCount = spline.GetNodeCount();

            if (idOnSpline < (mCount - 1))
            {
                if (spline.isSpecialStartControlNode)
                {
                    if (idOnSpline == 2)
                    {
                        return false;
                    }
                }
                else
                {
                    if (idOnSpline == 1)
                    {
                        return false;
                    }
                }
            }

            for (int i = idOnSpline; i >= 0; i--)
            {
                if (spline.nodes[i].isBridgeStart)
                {
                    if (!spline.nodes[i].isBridgeMatched)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
        #endregion


        #region "Tunnels"
        public void TunnelToggleStart()
        {
            //If switching to end, find associated Tunnel 
            if (isTunnelStart)
            {
                TunnelStart();
            }
            else
            {
                TunnelDestroy();
            }
        }


        public void TunnelToggleEnd()
        {
            //If switching to end, find associated Tunnel 
            if (isTunnelEnd)
            {
                int nodeCount = spline.GetNodeCount();
                SplineN node = null;
                for (int index = 1; index < (nodeCount - 1); index++)
                {
                    node = spline.nodes[index];
                    if (node.isTunnelStart && !node.isTunnelMatched)
                    {
                        node.TunnelToggleStart();
                        if (node.isTunnelMatched && node.tunnelCounterpartNode == this)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                TunnelDestroy();
            }
        }


        private void TunnelStart()
        {
            //Cycle through nodes until you find another end or another start (in this case, no creation, encountered another Tunnel):
            int EndID = idOnSpline + 1;
            int mCount = spline.GetNodeCount();
            if (isEndPoint)
            {
                //Attempted to make end point node a Tunnel node:
                isTunnelStart = false;
                return;
            }
            if (EndID >= mCount)
            {
                //Attempted to make last node a Tunnel node:
                isTunnelStart = false;
                return;
            }
            else if (idOnSpline == 0)
            {
                //Attempted to make first node a Tunnel node:
                isTunnelStart = false;
                return;
            }

            isTunnelMatched = false;
            tunnelCounterpartNode = null;
            int StartI = idOnSpline + 1;
            SplineN node = null;
            for (int i = StartI; i < mCount; i++)
            {
                node = spline.nodes[i];
                if (node.isIntersection)
                {
                    //Encountered intersection. End search.
                    return;
                }
                if (node.isTunnelStart)
                {
                    //Encountered another Tunnel. Return:
                    return;
                }
                if (node.isIgnore)
                {
                    continue;
                }
                if (node.isTunnelEnd)
                {
                    isTunnelMatched = true;
                    node.isTunnelMatched = true;
                    tunnelCounterpartNode = node;
                    node.tunnelCounterpartNode = this;
                    spline.TriggerSetup();
                    return;
                }
            }
        }


        private void TunnelDestroy()
        {
            if (tunnelCounterpartNode != null)
            {
                tunnelCounterpartNode.TunnelResetValues();
            }
            TunnelResetValues();
            spline.TriggerSetup();
        }


        public void TunnelResetValues()
        {
            isTunnel = false;
            isTunnelStart = false;
            isTunnelEnd = false;
            isTunnelMatched = false;
            tunnelCounterpartNode = null;
        }


        public bool CanTunnelStart()
        {
            if (isTunnelStart)
            {
                return true;
            }
            if (isTunnelEnd)
            {
                return false;
            }
            if (isEndPoint)
            {
                return false;
            }

            int mCount = spline.GetNodeCount();

            if (idOnSpline > 0)
            {
                if (spline.nodes[idOnSpline - 1].isTunnelStart)
                {
                    return false;
                }
            }

            if (idOnSpline < (mCount - 1))
            {
                if (spline.nodes[idOnSpline + 1].isTunnelStart)
                {
                    return false;
                }

                if (spline.isSpecialEndControlNode)
                {
                    if ((mCount - 3 > 0) && idOnSpline == mCount - 3)
                    {
                        return false;
                    }
                }
                else
                {
                    if ((mCount - 2 > 0) && idOnSpline == mCount - 2)
                    {
                        return false;
                    }
                }
            }

            if (spline.IsInTunnel(time))
            {
                return false;
            }

            return true;
        }


        public bool CanTunnelEnd()
        {
            if (isTunnelEnd)
            {
                return true;
            }
            if (isTunnelStart)
            {
                return false;
            }
            if (isEndPoint)
            {
                return false;
            }

            int nodeCount = spline.GetNodeCount();

            if (idOnSpline < (nodeCount - 1))
            {
                if (spline.isSpecialStartControlNode)
                {
                    if (idOnSpline == 2)
                    {
                        return false;
                    }
                }
                else
                {
                    if (idOnSpline == 1)
                    {
                        return false;
                    }
                }
            }

            for (int i = idOnSpline; i >= 0; i--)
            {
                if (spline.nodes[i].isTunnelStart)
                {
                    if (!spline.nodes[i].isTunnelMatched)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
        #endregion


        #region "Is straight line to next node"
        /// <summary> Returns true if two of 3 pos Vectors to previous and next 2 nodes approx. match </summary>
        public bool IsStraight()
        {
            int id1 = idOnSpline - 1;
            int id2 = idOnSpline + 1;
            int id3 = idOnSpline + 2;
            int nodeCount = spline.GetNodeCount();

            if (id1 > -1 && id1 < nodeCount)
            {
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id1].pos))
                {
                    return false;
                }
            }

            if (id2 > -1 && id2 < nodeCount)
            {
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id2].pos))
                {
                    return false;
                }
            }

            if (id3 > -1 && id3 < nodeCount)
            {
                if (!IsApproxTwoThirds(ref pos, spline.nodes[id3].pos))
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary> Returns <see langword="true"/> if exactly 2 values are approximately the same </summary>
        private static bool IsApproxTwoThirds(ref Vector3 _v1, Vector3 _v2)
        {
            int cCount = 0;
            if (RootUtils.IsApproximately(_v1.x, _v2.x, 0.02f))
            {
                cCount += 1;
            }
            if (RootUtils.IsApproximately(_v1.y, _v2.y, 0.02f))
            {
                cCount += 1;
            }
            if (RootUtils.IsApproximately(_v1.z, _v2.z, 0.02f))
            {
                cCount += 1;
            }

            if (cCount == 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region "Non-editor util"
        /// <summary> Returns false when isSpecialEndNode </summary>
        public bool CanSplinate()
        {
            if (isSpecialEndNode)
            {
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary> Returns false when isIntersection or isSpecialEndNode </summary>
        public bool IsLegitimate()
        {
            if (isIntersection || isSpecialEndNode)
            {
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary> Returns false when isSpecialEndNode </summary>
        public bool IsLegitimateGrade()
        {
            if (isSpecialEndNode)
            {
                // || bIsBridge_PreNode || bIsBridge_PostNode){
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion


        /// <summary> Hide or unhide this node in hierarchy </summary>
        public void ToggleHideFlags(bool _isHidden)
        {
            if (_isHidden)
            {
                this.hideFlags = HideFlags.HideInHierarchy;
                transform.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                this.hideFlags = HideFlags.None;
                transform.hideFlags = HideFlags.None;
            }
        }
    }
}