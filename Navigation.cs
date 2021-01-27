using UnityEngine;


namespace RoadArchitect
{
    public static class Navigation
    {
        public static void UpdateConnectedNodes()
        {
            InitResetNavigationData();

            Object[] allSplines = GameObject.FindObjectsOfType<SplineC>();

            //Store connected spline nodes on each other:
            SplineN node;
            foreach (SplineC spline in allSplines)
            {
                int nodeCount = spline.nodes.Count;
                for (int i = 0; i < nodeCount; i++)
                {
                    node = spline.nodes[i];
                    //Add next node if not last node:
                    if ((i + 1) < nodeCount)
                    {
                        node.connectedID.Add(spline.nodes[i + 1].id);
                        node.connectedNode.Add(spline.nodes[i + 1]);
                    }
                    //Add prev node if not first node:
                    if (i > 0)
                    {
                        node.connectedID.Add(spline.nodes[i - 1].id);
                        node.connectedNode.Add(spline.nodes[i - 1]);
                    }
                }
            }
        }


        public static void InitResetNavigationData()
        {
            Object[] allSplines = GameObject.FindObjectsOfType<SplineC>();
            int splineCount = 0;
            int nodeCount = 0;
            foreach (SplineC spline in allSplines)
            {
                splineCount += 1;
                foreach (SplineN node in spline.nodes)
                {
                    nodeCount += 1;
                    node.ResetNavigationData();
                }
                spline.ResetNavigationData();
            }
        }
    }
}