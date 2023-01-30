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
            Terrain terrain;
            //Sphere cast 5m first. Then raycast down 1000m, then up 1000m.
            Collider[] colliders = Physics.OverlapSphere(_vect, 10f);
            if (colliders != null)
            {
                int collidersLength = colliders.Length;
                for (int index = 0; index < collidersLength; index++)
                {
                    terrain = colliders[index].transform.GetComponent<Terrain>();
                    if (terrain)
                    {
                        colliders = null;
                        return terrain;
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
                    terrain = hits[index].collider.transform.GetComponent<Terrain>();
                    if (terrain)
                    {
                        hits = null;
                        return terrain;
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
                    terrain = hits[i].collider.transform.GetComponent<Terrain>();
                    if (terrain)
                    {
                        hits = null;
                        return terrain;
                    }
                }
                hits = null;
            }
            return null;
        }


        #region "Terrain history"
        public static void ConstructRoadStoreTerrainHistory(ref Road _road)
        {
            Object[] allTerrains = GameObject.FindObjectsOfType<RoadTerrain>();

            HashSet<int> terrainIDs = new HashSet<int>();
            foreach (RoadTerrain terrain in allTerrains)
            {
                terrainIDs.Add(terrain.UID);
            }

            if (_road.TerrainHistory == null)
            {
                _road.TerrainHistory = new List<TerrainHistoryMaker>();
            }

            if (_road.TerrainHistory.Count > 0)
            {
                //Delete unnecessary terrain histories:
                foreach (TerrainHistoryMaker THMaker in _road.TerrainHistory)
                {
                    if (!terrainIDs.Contains(THMaker.terrainID))
                    {
                        THMaker.Nullify();
                        _road.TerrainHistory.Remove(THMaker);
                    }
                }
            }


            TerrainHistoryMaker TH;
            RoadTerrain roadTerrain;
            foreach (Terraforming.TempTerrainData TTD in _road.EditorTTDList)
            {
                roadTerrain = null;
                //Get terrainID:
                foreach (RoadTerrain terrain in allTerrains)
                {
                    if (terrain.UID == TTD.uID)
                    {
                        roadTerrain = terrain;
                    }
                }

                if (roadTerrain == null)
                {
                    continue;
                }

                TH = null;
                int THCount = _road.TerrainHistory.Count;
                bool isContainingTID = false;
                for (int index = 0; index < THCount; index++)
                {
                    if (_road.TerrainHistory[index].terrainID == roadTerrain.UID)
                    {
                        isContainingTID = true;
                        TH = _road.TerrainHistory[index];
                        break;
                    }
                }

                if (!isContainingTID)
                {
                    TerrainHistoryMaker THMaker = new TerrainHistoryMaker();
                    THMaker.terrainID = roadTerrain.UID;
                    _road.TerrainHistory.Add(THMaker);
                    TH = THMaker;
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
                    int cLength;
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
            //Splinated objects first:
            RootUtils.CheckCreateSpecialLibraryDirs();
            string libraryPath = RootUtils.GetDirLibrary();
            string filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), _wizardObj.fileName + ".rao");
            if (_wizardObj.isDefault)
            {
                filePath = Path.Combine(Path.Combine(libraryPath, "Groups"), "Default");
                filePath = Path.Combine(filePath, _wizardObj.fileName + ".rao");
            }
            StringBuilder builder = new StringBuilder(32768);

            //Wizard object:
            builder.Append(_wizardObj.ConvertToString());
            builder.Append(FileSepString);

            int sCount = _splinatedObjects.Length;
            Splination.SplinatedMeshMaker SMM = null;
            for (int index = 0; index < sCount; index++)
            {
                SMM = _splinatedObjects[index];
                builder.Append(SMM.ConvertToString());
                builder.Append(FileSepString);
            }

            int eCount = _edgeObjects.Length;
            EdgeObjects.EdgeObjectMaker EOM = null;
            for (int index = 0; index < eCount; index++)
            {
                EOM = _edgeObjects[index];
                builder.Append(EOM.ConvertToString());
                builder.Append(FileSepString);
            }

            File.WriteAllText(filePath, builder.ToString());
        }


        /// <summary> Loads splinated objects for this _node </summary>
        public static void LoadNodeObjects(string _fileName, SplineN _node, bool _isDefault = false, bool _isBridge = false)
        {
            string filePath;
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

            string fileData = File.ReadAllText(filePath);
            string[] seperators = new string[2];
            seperators[0] = FileSepString;
            seperators[1] = FileSepStringCRLF;
            string[] fileSplitted = fileData.Split(seperators, System.StringSplitOptions.RemoveEmptyEntries);

            Splination.SplinatedMeshMaker SMM;
            Splination.SplinatedMeshMaker.SplinatedMeshLibraryMaker SLM;
            EdgeObjects.EdgeObjectMaker EOM;
            EdgeObjects.EdgeObjectMaker.EdgeObjectLibraryMaker ELM;
            int fileSplitCount = fileSplitted.Length;

            for (int index = 0; index < fileSplitCount; index++)
            {
                SLM = Splination.SplinatedMeshMaker.SLMFromData(fileSplitted[index]);
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

                ELM = EdgeObjects.EdgeObjectMaker.ELMFromData(fileSplitted[index]);
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
            Texture2D texture = new Texture2D(_width, _height, TextureFormat.RGB24, false);

            //Set background color:
            Color[] backgroundColors = new Color[_width * _height];
            int backgroundCount = backgroundColors.Length;
            for (int i = 0; i < backgroundCount; i++)
            {
                backgroundColors[i] = _BG;
            }
            texture.SetPixels(0, 0, _width, _height, backgroundColors);


            Object[] roadObjects;
            if (_roadUID != "")
            {
                roadObjects = new Object[1];
                Object[] roads = GameObject.FindObjectsOfType<Road>();
                foreach (Road road in roads)
                {
                    if (string.CompareOrdinal(road.UID, _roadUID) == 0)
                    {
                        roadObjects[0] = road;
                        break;
                    }
                }
            }
            else
            {
                roadObjects = GameObject.FindObjectsOfType<Road>();
            }


            Vector3 terrainPos = _terrain.transform.position;
            Vector3 terrainSize = _terrain.terrainData.size;
            foreach (Road road in roadObjects)
            {
                SplineC spline = road.spline;
                int tCount = spline.RoadDefKeysArray.Length;

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
                bool isXBad = false;
                bool isYBad = false;
                for (int i = 0; i < (tCount - 1); i++)
                {
                    isXBad = false;
                    isYBad = false;
                    p1 = spline.TranslateInverseParamToFloat(spline.RoadDefKeysArray[i]);
                    p2 = spline.TranslateInverseParamToFloat(spline.RoadDefKeysArray[i + 1]);

                    //Skip bridges:
                    if (_isSkippingBridge)
                    {
                        if (spline.IsInBridgeTerrain(p1))
                        {
                            continue;
                        }
                    }

                    //Skip tunnels:
                    if (_isSkippingTunnel)
                    {
                        if (spline.IsInTunnelTerrain(p1))
                        {
                            continue;
                        }
                    }

                    spline.GetSplineValueBoth(p1, out tVect, out POS1);
                    spline.GetSplineValueBoth(p2, out tVect2, out POS2);
                    lVect1 = (tVect + new Vector3(_splatWidth * -POS1.normalized.z, 0, _splatWidth * POS1.normalized.x));
                    rVect1 = (tVect + new Vector3(_splatWidth * POS1.normalized.z, 0, _splatWidth * -POS1.normalized.x));
                    lVect2 = (tVect2 + new Vector3(_splatWidth * -POS2.normalized.z, 0, _splatWidth * POS2.normalized.x));
                    rVect2 = (tVect2 + new Vector3(_splatWidth * POS2.normalized.z, 0, _splatWidth * -POS2.normalized.x));

                    TranslateWorldVectToCustom(_width, _height, lVect1, ref terrainPos, ref terrainSize, out x1, out y1);
                    tX[0] = x1;
                    tY[0] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect1, ref terrainPos, ref terrainSize, out x1, out y1);
                    tX[1] = x1;
                    tY[1] = y1;
                    TranslateWorldVectToCustom(_width, _height, lVect2, ref terrainPos, ref terrainSize, out x1, out y1);
                    tX[2] = x1;
                    tY[2] = y1;
                    TranslateWorldVectToCustom(_width, _height, rVect2, ref terrainPos, ref terrainSize, out x1, out y1);
                    tX[3] = x1;
                    tY[3] = y1;

                    MinX = Mathf.Min(tX);
                    MaxX = Mathf.Max(tX);
                    MinY = Mathf.Min(tY);
                    MaxY = Mathf.Max(tY);


                    if (MinX < 0)
                    {
                        MinX = 0;
                        isXBad = true;
                    }
                    if (MaxX < 0)
                    {
                        MaxX = 0;
                        isXBad = true;
                    }
                    if (MinY < 0)
                    {
                        MinY = 0;
                        isYBad = true;
                    }
                    if (MaxY < 0)
                    {
                        MaxY = 0;
                        isYBad = true;
                    }

                    if (MinX > (_width - 1))
                    {
                        MinX = (_width - 1);
                        isXBad = true;
                    }
                    if (MaxX > (_width - 1))
                    {
                        MaxX = (_width - 1);
                        isXBad = true;
                    }
                    if (MinY > (_height - 1))
                    {
                        MinY = (_height - 1);
                        isYBad = true;
                    }
                    if (MaxY > (_height - 1))
                    {
                        MaxY = (_height - 1);
                        isYBad = true;
                    }

                    if (isXBad && isYBad)
                    {
                        continue;
                    }

                    xDiff = MaxX - MinX;
                    yDiff = MaxY - MinY;

                    Color[] colors = new Color[xDiff * yDiff];
                    int colorCount = colors.Length;
                    for (int j = 0; j < colorCount; j++)
                    {
                        colors[j] = _FG;
                    }

                    if (xDiff > 0 && yDiff > 0)
                    {
                        texture.SetPixels(MinX, MinY, xDiff, yDiff, colors);
                    }
                }
            }

            texture.Apply();
            byte[] imageBytes = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            return imageBytes;
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
