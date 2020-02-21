using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    [System.Serializable]
    public class TerrainHistoryMaker
    {
        #region "Vars"
        public int TID;
        //Heights:
        public int[] x1;
        public int[] y1;
        public float[] h;
        public int cI;
        public bool bHeightHistoryEnabled;
        //Details:
        public int DetailLayersCount;

        public int[] DetailsX;
        public int[] DetailsY;
        public int[] DetailsOldValue;
        public int[] DetailsI;

        [UnityEngine.Serialization.FormerlySerializedAs("bDetailHistoryEnabled")]
        public bool isDetailHistoryEnabled;
        //Trees:
        public TerrainTreeInstance[] TreesOld;
        public int TreesI;
        [UnityEngine.Serialization.FormerlySerializedAs("bTreeHistoryEnabled")]
        public bool isTreeHistoryEnabled;
        [UnityEngine.Serialization.FormerlySerializedAs("bDestroyMe")]
        public bool isDestroySheduled = false;
        #endregion


        public void Nullify()
        {
            //Heights:
            x1 = null;
            y1 = null;
            h = null;
            DetailsX = null;
            DetailsY = null;
            DetailsOldValue = null;
            DetailsI = null;
            //Trees:
            TreesOld = null;
        }


        [System.Serializable]
        public class TerrainTreeInstance
        {
            public float colorR;
            public float colorG;
            public float colorB;
            public float colorA;
            public float heightScale;
            public float lightmapColorR;
            public float lightmapColorG;
            public float lightmapColorB;
            public float lightmapColorA;
            public float positionX;
            public float positionY;
            public float positionZ;
            public int prototypeIndex;
            public float widthScale;
            // 4 bytes for int and float
            //56 Bytes
        }


        public void MakeRATrees(ref List<TreeInstance> _trees)
        {
            int tSize = _trees.Count;
            TreesOld = new TerrainTreeInstance[tSize];
            TerrainTreeInstance tTree = null;
            TreeInstance xTree;
            for (int index = 0; index < tSize; index++)
            {
                xTree = _trees[index];
                tTree = new TerrainTreeInstance();
                tTree.colorR = xTree.color.r;
                tTree.colorG = xTree.color.g;
                tTree.colorB = xTree.color.b;
                tTree.colorA = xTree.color.a;
                tTree.heightScale = xTree.heightScale;
                tTree.lightmapColorR = xTree.lightmapColor.r;
                tTree.lightmapColorG = xTree.lightmapColor.g;
                tTree.lightmapColorB = xTree.lightmapColor.b;
                tTree.lightmapColorA = xTree.lightmapColor.a;
                tTree.positionX = xTree.position.x;
                tTree.positionY = xTree.position.y;
                tTree.positionZ = xTree.position.z;
                tTree.prototypeIndex = xTree.prototypeIndex;
                tTree.widthScale = xTree.widthScale;
                TreesOld[index] = tTree;
            }
        }


        public TreeInstance[] MakeTrees()
        {
            if (TreesOld == null || TreesOld.Length < 1)
            {
                return null;
            }
            int tSize = TreesOld.Length;
            TreeInstance[] tTrees = new TreeInstance[tSize];
            TerrainTreeInstance tTree = null;
            TreeInstance xTree;
            for (int index = 0; index < tSize; index++)
            {
                tTree = TreesOld[index];
                xTree = new TreeInstance();
                xTree.color = new Color(tTree.colorR, tTree.colorG, tTree.colorB, tTree.colorA);
                xTree.heightScale = tTree.heightScale;
                xTree.lightmapColor = new Color(tTree.lightmapColorR, tTree.lightmapColorG, tTree.lightmapColorB, tTree.lightmapColorA);
                xTree.position = new Vector3(tTree.positionX, tTree.positionY, tTree.positionZ);
                xTree.prototypeIndex = tTree.prototypeIndex;
                xTree.widthScale = tTree.widthScale;
                tTrees[index] = xTree;
            }
            return tTrees;
        }


        public int GetSize()
        {
            int tSize = 4;
            if (x1 != null)
            {
                tSize += (x1.Length * 4);
                tSize += 20;
            }
            if (y1 != null)
            {
                tSize += (y1.Length * 4);
                tSize += 20;
            }
            if (h != null)
            {
                tSize += (h.Length * 4);
                tSize += 20;
            }
            tSize += 4;
            tSize += 1;
            //Details:
            tSize += 4;
            if (DetailsX != null)
            {
                tSize += (DetailsX.Length * 4);
                tSize += 20;
            }
            if (DetailsY != null)
            {
                tSize += (DetailsY.Length * 4);
                tSize += 20;
            }
            if (DetailsOldValue != null)
            {
                tSize += (DetailsOldValue.Length * 4);
                tSize += 20;
            }
            if (DetailsI != null)
            {
                tSize += (DetailsI.Length * 4);
                tSize += 20;
            }
            tSize += 1;
            //Trees:
            if (TreesOld != null)
            {
                tSize += (TreesOld.Length * 56);
                tSize += 20;
            }
            tSize += 4;
            tSize += 1;
            tSize += 1;

            return tSize;
        }
    }
}