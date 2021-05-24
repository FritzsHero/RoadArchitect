#region "Imports"
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
#endregion


namespace RoadArchitect
{
    //Generic http://www.fhwa.dot.gov/bridge/bridgerail/br053504.cfm
    public enum RailingTypeEnum { None, Generic1, Generic2, K_Rail, WBeam };
    public enum RailingSubTypeEnum { Both, Left, Right };
    public enum SignPlacementSubTypeEnum { Center, Left, Right };
    public enum CenterDividerTypeEnum { None, K_Rail, KRail_Blinds, Wire, Markers };
    public enum EndCapTypeEnum { None, WBeam, Barrels3Static, Barrels3Rigid, Barrels7Static, Barrels7Rigid };
    public enum RoadUpdateTypeEnum { Full, Intersection, Railing, CenterDivider, Bridges };
    public enum AxisTypeEnum { X, Y, Z };


    public static class RoadUtility
    {
        public const string FileSepString = "\n#### RoadArchitect ####\n";
        public const string FileSepStringCRLF = "\r\n#### RoadArchitect ####\r\n";


        /// <summary> Returns closest terrain to _vect </summary>
        public static Terrain GetTerrain(Vector3 _vect)
        {
            return GetTerrainDo(ref _vect);
        }


        /// <summary> Returns closest terrain to _vect </summary>
        private static Terrain GetTerrainDo(ref Vector3 _vect)
        {
            //Sphere cast 5m first. Then raycast down 1000m, then up 1000m.
            Collider[] colliders = Physics.OverlapSphere(_vect, 10f);
            if (colliders != null)
            {
                int collidersLength = colliders.Length;
                for (int index = 0; index < collidersLength; index++)
                {
                    Terrain tTerrain = colliders[index].transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        colliders = null;
                        return tTerrain;
                    }
                }
                colliders = null;
            }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(_vect, Vector3.down, 1000f);
            int hitsLength = 0;
            if (hits != null)
            {
                hitsLength = hits.Length;
                for (int index = 0; index < hitsLength; index++)
                {
                    Terrain tTerrain = hits[index].collider.transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        hits = null;
                        return tTerrain;
                    }
                }
                hits = null;
            }

            hits = Physics.RaycastAll(_vect, Vector3.up, 1000f);
            if (hits != null)
            {
                hitsLength = hits.Length;
                for (int i = 0; i < hitsLength; i++)
                {
                    Terrain tTerrain = hits[i].collider.transform.GetComponent<Terrain>();
                    if (tTerrain)
                    {
                        hits = null;
                        return tTerrain;
                    }
                }
                hits = null;
            }
            return null;
        }


        #region "Terrain history"
        public static void ConstructRoadStoreTerrainHistory(ref Road _road)
        {
            Object[] TIDs = GameObject.FindObjectsOfType<RoadTerrain>();

            HashSet<int> tTIDS = new HashSet<int>();
            foreach (RoadTerrain TID in TIDs)
            {
                tTIDS.Add(TID.UID);
            }

            if (_road.TerrainHistory != null && _road.TerrainHistory.Count > 0)
            {
                //Delete unnecessary terrain histories:
                foreach (TerrainHistoryMaker THMaker in _road.TerrainHistory)
                {
                    if (!tTIDS.Contains(THMaker.terrainID))
                    {
                        THMaker.Nullify();
                        _road.TerrainHistory.Remove(THMaker);
                    }
                }
            }

            if (_road.TerrainHistory == null)
            {
                _road.TerrainHistory = new List<TerrainHistoryMaker>();
            }
            foreach (Terraforming.TempTerrainData TTD in _road.EditorTTDList)
            {
                TerrainHistoryMaker TH = null;
                RoadTerrain TID = null;
                //Get terrainID:
                foreach (RoadTerrain _TID in TIDs)
                {
                    if (_TID.UID == TTD.uID)
                    {
                        TID = _TID;
                    }
                }

                if (_road.TerrainHistory == null)
                {
                    _road.TerrainHistory = new List<TerrainHistoryMaker>();
                }
                if (TID == null)
                {
                    continue;
                }

                int THCount = _road.TerrainHistory.Count;
                bool isContainingTID = false;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].terrainID == TID.UID)
                    {
                        isContainingTID = true;
                        break;
                    }
                }

                if (!isContainingTID)
                {
                    TerrainHistoryMaker THMaker = new TerrainHistoryMaker();
                    THMaker.terrainID = TID.UID;
                    _road.TerrainHistory.Add(THMaker);
                }

                TH = null;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].terrainID == TID.UID)
                    {
                        TH = _road.TerrainHistory[index];
                        break;
                    }
                }
                if (TH == null)
                {
                    continue;
                }

                //Heights:
                if (_road.isHeightModificationEnabled)
                {
                    if (TTD.cX != null && TTD.cY != null)
                    {
                        TH.x1 = new int[TTD.Count];
                        System.Array.Copy(TTD.cX, 0, TH.x1, 0, TTD.Count);
                        TH.y1 = new int[TTD.Count];
                        System.Array.Copy(TTD.cY, 0, TH.y1, 0, TTD.Count);
                        TH.height = new float[TTD.Count];
                        System.Array.Copy(TTD.oldH, 0, TH.height, 0, TTD.Count);
                        TH.Count = TTD.Count;
                        TH.heightmapResolution = TTD.TerrainMaxIndex;
                    }
                }
                else
                {
                    TH.x1 = null;
                    TH.y1 = null;
                    TH.height = null;
                    TH.Count = 0;
                }
                //Details:
                if (_road.isDetailModificationEnabled)
                {
                    int TotalSize = 0;
                    for (int i = 0; i < TTD.DetailLayersCount; i++)
                    {
                        TotalSize += TTD.detailsCount[i];
                    }

                    TH.detailsX = new int[TotalSize];
                    TH.detailsY = new int[TotalSize];
                    TH.detailsOldValue = new int[TotalSize];

                    int RunningIndex = 0;
                    int cLength = 0;
                    for (int index = 0; index < TTD.DetailLayersCount; index++)
                    {
                        cLength = TTD.detailsCount[index];
                        if (cLength < 1)
                        {
                            continue;
                        }
                        System.Array.Copy(TTD.DetailsX[index].ToArray(), 0, TH.detailsX, RunningIndex, cLength);
                        System.Array.Copy(TTD.DetailsY[index].ToArray(), 0, TH.detailsY, RunningIndex, cLength);
                        System.Array.Copy(TTD.OldDetailsValue[index].ToArray(), 0, TH.detailsOldValue, RunningIndex, cLength);
                        RunningIndex += TTD.detailsCount[index];
                    }

                    //TH.detailsX = TTD.detailsX;
                    //TH.detailsY = TTD.detailsY;
                    //TH.detailsOldValue = TTD.OldDetailsValue;
                    TH.detailsCount = TTD.detailsCount;
                    TH.detailLayersCount = TTD.DetailLayersCount;
                }
                else
                {
                    TH.detailsX = null;
                    TH.detailsY = null;
                    TH.detailsOldValue = null;
                    TH.detailsCount = null;
                    TH.detailLayersCount = 0;
                }
                //Trees:
                if (_road.isTreeModificationEnabled)
                {
                    if (TTD.TreesOld != null)
                    {
                        TH.MakeRATrees(ref TTD.TreesOld);
                        TTD.TreesOld.Clear();
                        TTD.TreesOld = null;
                        TH.treesCount = TTD.treesCount;
                    }
                }
                else
                {
                    TH.oldTrees = null;
                    TH.treesCount = 0;
                }
            }
        }


        /// <summary> Clears the terrain history of _road </summary>
        public static void ResetTerrainHistory(ref Road _road)
        {
            if (_road.TerrainHistory != null)
            {
                _road.TerrainHistory.Clear();
                _road.TerrainHistory = null;
            }
        }
        #endregion


        public static void SaveNodeObjects(ref Splination.SplinatedMeshMaker[] _splinatedObjects, ref EdgeObjects.EdgeObjectMaker[] _edgeObjects, ref WizardObject _wizardObj)
        {
            int sCount = _splinatedObjects.Length;
            int eCount = _edgeObjects.Length;
            //Splinated objects first:
            Splination.SplinatedMeshMaker SMM = null;
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string tPath = Path.Combine(Path.Combine(libraryPath, "Groups"), _wizardObj.fileName + ".rao");
            if (_wizardObj.isDefault)
            {
                tPath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
                tPath = Path.Combine(tPath, _wizardObj.fileName + ".rao");
            }
            StringBuilder builder = new StringBuilder(32768);

            //Wizard object:
            builder.Append(_wizardObj.ConvertToString());
            builder.Append(FileSepString);

            for (int index = 0; index < sCount; index++)
            {
                SMM = _splinatedObjects[index];
                builder.Append(SMM.ConvertToString());
                builder.Append(FileSepString);
            }

            EdgeObjects.EdgeObjectMaker EOM = null;
            for (int index = 0; index < eCount; index++)
            {
                EOM = _edgeObjects[index];
                builder.Append(EOM.ConvertToString());
                builder.Append(FileSepString);
            }

            System.IO.File.WriteAllText(tPath, builder.ToString());
        }


        /// <summary> Loads splinated objects for this _node </summary>
        public static void LoadNodeObjects(string _fileName, SplineN _node, bool _isDefault = false, bool _isBridge = false)
        {
            string filePath = "";
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            if (_isDefault)
            {
                filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
                filePath = Path.Combine(filePath, _fileName + ".rao");
            }
            else
            {
                filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), _fileName + ".rao");
            }

            string fileData = System.IO.File.ReadAllText(filePath);
            string[] tSep = new string[2];
            tSep[0] = FileSepString;
            tSep[1] = FileSepStringCRLF;
            string[] tSplit = fileData.Split(tSep, System.StringSplitOptions.RemoveEmptyEntries);

            Splination.SplinatedMeshMaker SMM = null;
            Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM = null;
            EdgeObjects.EdgeObjectMaker EOM = null;
            EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker ELM = null;
            int tSplitCount = tSplit.Length;

            for (int index = 0; index < tSplitCount; index++)
            {
                SLM = null;
                SLM = Splination.SplinatedMeshMaker.SLMFromData(tSplit[index]);
                if (SLM != null)
                {
                    SMM = _node.AddSplinatedObject();
                    SMM.LoadFromLibraryBulk(ref SLM);
                    SMM.isToggled = false;
                    if (_isBridge && _node.isBridgeStart && _node.isBridgeMatched && _node.bridgeCounterpartNode != null)
                    {
                        SMM.StartTime = _node.time;
                        SMM.EndTime = _node.bridgeCounterpartNode.time;
                        SMM.StartPos = _node.spline.GetSplineValue(SMM.StartTime);
                        SMM.EndPos = _node.spline.GetSplineValue(SMM.EndTime);
                    }
                    continue;
                }

                ELM = null;
                ELM = EdgeObjects.EdgeObjectMaker.ELMFromData(tSplit[index]);
                if (ELM != null)
                {
                    EOM = _node.AddEdgeObject();
                    EOM.LoadFromLibraryBulk(ref ELM);
                    EOM.isToggled = false;
                    if (!EOM.isSingle && _isBridge && _node.isBridgeStart && _node.isBridgeMatched && _node.bridgeCounterpartNode != null)
                    {
                        EOM.startTime = _node.time;
                        EOM.endTime = _node.bridgeCounterpartNode.time;
                        EOM.startPos = _node.spline.GetSplineValue(EOM.startTime);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.endTime);
                    }
                    else if (EOM.isSingle && _isBridge && _node.bridgeCounterpartNode != null && _node.isBridgeStart)
                    {
                        float tDist = (EOM.singleOnlyBridgePercent * (_node.bridgeCounterpartNode.dist - _node.dist) + _node.dist);
                        EOM.singlePosition = _node.spline.TranslateDistBasedToParam(tDist);
                        EOM.startPos = _node.spline.GetSplineValue(EOM.singlePosition);
                        EOM.endPos = _node.spline.GetSplineValue(EOM.singlePosition);
                    }
                    continue;
                }
            }

            _node.SetupSplinatedMeshes();
            _node.SetupEdgeObjects();
        }


        #region "Splat maps"
        /// <summary> Returns a splat map texture encoded as png </summary>
        public static byte[] MakeSplatMap(Terrain _terrain, Color _BG, Color _FG, int _width, int _height, float _splatWidth, bool _isSkippingBridge, bool _isSkippingTunnel, string _roadUID = "")
        {
            Texture2D tTexture = new Texture2D(_width, _height, TextureFormat.RGB24, false);

            //Set background color:
            Color[] tColorsBG = new Color[_width * _height];
            int tBGCount = tColorsBG.Length;
            for (int i = 0; i < tBGCount; i++)
            {
                tColorsBG[i] = _BG;
            }
            tTexture.SetPixels(0, 0, _width, _height, tColorsBG);
            tColorsBG = null;

            Object[] tRoads = null;
            if (_roadUID != "")
            {
                tRoads = new Object[1];
                Object[] roads = GameObject.FindObjectsOfType<Road>();
                foreach (Road road in roads)
                {
                    if (string.CompareOrdinal(road.UID, _roadUID) == 0)
                    {
                        tRoads[0] = road;
                        break;
                    }
                }
            }
            else
            {
                tRoads = GameObject.FindObjectsOfType<Road>();
            }
            Vector3 tPos = _terrain.transform.position;
            Vector3 tSize = _terrain.terrainData.size;
            foreach (Road tRoad in tRoads)
            {
                SplineC tSpline = tRoad.spline;
                int tCount = tSpline.RoadDefKeysArray.Length;

                Vector3 POS1 = default(Vector3);
                Vector3 POS2 = default(Vector3);

                Vector3 tVect = default(Vector3);
                Vector3 tVect2 = default(Vector3);
                Vector3 lVect1 = default(Vector3);
                Vector3 lVect2 = default(Vector3);
                Vector3 rVect1 = default(Vector3);
                Vector3 rVect2 = default(Vector3);

                int x1, y1;
                int[] tX = new int[4];
                int[] tY = new int[4];
                int MinX = -1;
                int MaxX = -1;
                int MinY = -1;
                int MaxY = -1;
                int xDiff = -1;
                int yDiff = -1;
                float p1 = 0f;
                float p2 = 0f;
                bool bXBad = false;
                bool bYBad = false;
                for (int i = 0; i < (tCount - 1); i++)
                {
                    bXBad = false;
                    bYBad = false;
                    p1 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i]);
                    p2 = tSpline.TranslateInverseParamToFloat(tSpline.RoadDefKeysArray[i + 1]);

                    //Skip bridges:
                    if (_isSkippingBridge)
                    {
                        if (tSpline.IsInBridgeTerrain(p1))
                        {
                            continue;
                        }
                    }

                    //Skip tunnels:
                    if (_isSkippingTunnel)
                    {
                        if (tSpline.IsInTunnelTerrain(p1))
                        {
                            continue;
                        }
                    }

                    tSpline.GetSplineValueBoth(p1, out tVect, out POS1);
                    tSpline.GetSplineValueBoth(p2, out tVect2, out POS2);
                    lVect1 = (tVect + new Vector3(_splatWidth * -POS1.normalized.z, 0, _splatWidth * POS1.normalized.x));
                    rVect1 = (tVect + new Vector3(_splatWidth * POS1.normalized.z, 0, _splatWidth * -POS1.normalized.x));
                    lVect2 = (tVect2 + new Vector3(_splatWidth * -POS2.normalized.z, 0, _splatWidth * POS2.normalized.x));
                    rVect2 = (tVect2 + new Vector3(_splatWidth * POS2.normalized.z, 0, _splatWidth * -POS2.normalized.x));

                    TranslateWorldVectToCustom(_width, _height, lVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[0] = x1;
                    tY[0] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect1, ref tPos, ref tSize, out x1, out y1);
                    tX[1] = x1;
                    tY[1] = y1;
                    TranslateWorldVectToCustom(_width, _height, lVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[2] = x1;
                    tY[2] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect2, ref tPos, ref tSize, out x1, out y1);
                    tX[3] = x1;
                    tY[3] = y1;

                    MinX = Mathf.Min(tX);
                    MaxX = Mathf.Max(tX);
                    MinY = Mathf.Min(tY);
                    MaxY = Mathf.Max(tY);


                    if (MinX < 0)
                    {
                        MinX = 0; bXBad = true;
                    }
                    if (MaxX < 0)
                    {
                        MaxX = 0; bXBad = true;
                    }
                    if (MinY < 0)
                    {
                        MinY = 0; bYBad = true;
                    }
                    if (MaxY < 0)
                    {
                        MaxY = 0; bYBad = true;
                    }

                    if (MinX > (_width - 1))
                    {
                        MinX = (_width - 1); bXBad = true;
                    }
                    if (MaxX > (_width - 1))
                    {
                        MaxX = (_width - 1); bXBad = true;
                    }
                    if (MinY > (_height - 1))
                    {
                        MinY = (_height - 1); bYBad = true;
                    }
                    if (MaxY > (_height - 1))
                    {
                        MaxY = (_height - 1); bYBad = true;
                    }

                    if (bXBad && bYBad)
                    {
                        continue;
                    }

                    xDiff = MaxX - MinX;
                    yDiff = MaxY - MinY;

                    Color[] tColors = new Color[xDiff * yDiff];
                    int cCount = tColors.Length;
                    for (int j = 0; j < cCount; j++)
                    {
                        tColors[j] = _FG;
                    }

                    if (xDiff > 0 && yDiff > 0)
                    {
                        tTexture.SetPixels(MinX, MinY, xDiff, yDiff, tColors);
                    }
                }
            }

            tTexture.Apply();
            byte[] tBytes = tTexture.EncodeToPNG();
            Object.DestroyImmediate(tTexture);
            return tBytes;
        }


        /// <summary> Writes _vect location into _x1 and _y1 relative to the terrain on a 2D map </summary>
        private static void TranslateWorldVectToCustom(int _width, int _height, Vector3 _vect, ref Vector3 _pos, ref Vector3 _size, out int _x1, out int _y1)
        {
            //Get the normalized position of this game object relative to the terrain:
            _vect -= _pos;

            _vect.x = _vect.x / _size.x;
            _vect.z = _vect.z / _size.z;

            //Get the position of the terrain heightmap where this game object is:
            _x1 = (int) (_vect.x * _width);
            _y1 = (int) (_vect.z * _height);
        }
        #endregion
    }
}
