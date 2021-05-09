using System.Collections.Generic;
using UnityEngine;


namespace RoadArchitect
{
    [System.Serializable]
    public class TerrainHistoryMaker
    {
        #region "Vars"
        public int terrainID;
        public int heightmapResolution;
        //Heights:
        public int[] x1;
        public int[] y1;
        public float[] height;
        public int Count;
        //Details:
        public int detailLayersCount;
        public int[] detailsCount;
        public int[] detailsX;
        public int[] detailsY;
        public int[] detailsOldValue;

        //Trees:
        public TerrainTreeInstance[] oldTrees;
        public int treesCount;
        #endregion


        public void Nullify()
        {
            //Heights:
            x1 = null;
            y1 = null;
            height = null;
            detailsX = null;
            detailsY = null;
            detailsOldValue = null;
            detailsCount = null;
            //Trees:
            oldTrees = null;
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


        /// <summary> Stores _trees into oldTrees </summary>
        public void MakeRATrees(ref List<TreeInstance> _trees)
        {
            int tSize = _trees.Count;
            oldTrees = new TerrainTreeInstance[tSize];
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
                oldTrees[index] = tTree;
            }
        }


        /// <summary> Returns a new tree array copy of oldTrees </summary>
        public TreeInstance[] MakeTrees()
        {
            if (oldTrees == null || oldTrees.Length < 1)
            {
                return null;
            }
            int tSize = oldTrees.Length;
            TreeInstance[] tTrees = new TreeInstance[tSize];
            TerrainTreeInstance tTree = null;
            TreeInstance xTree;
            for (int index = 0; index < tSize; index++)
            {
                tTree = oldTrees[index];
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


        /// <summary> Returns size of this terrain history </summary>
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
            if (height != null)
            {
                tSize += (height.Length * 4);
                tSize += 20;
            }
            tSize += 4;
            tSize += 1;
            //Details:
            tSize += 4;
            if (detailsX != null)
            {
                tSize += (detailsX.Length * 4);
                tSize += 20;
            }
            if (detailsY != null)
            {
                tSize += (detailsY.Length * 4);
                tSize += 20;
            }
            if (detailsOldValue != null)
            {
                tSize += (detailsOldValue.Length * 4);
                tSize += 20;
            }
            if (detailsCount != null)
            {
                tSize += (detailsCount.Length * 4);
                tSize += 20;
            }
            tSize += 1;
            //Trees:
            if (oldTrees != null)
            {
                tSize += (oldTrees.Length * 56);
                tSize += 20;
            }
            tSize += 4;
            tSize += 1;
            tSize += 1;

            return tSize;
        }
    }
}
