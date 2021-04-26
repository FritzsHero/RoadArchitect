using UnityEngine;


namespace RoadArchitect
{
    public static class Navigation
    {
        /// <summary> Resets navigation and then links each node with previous and next node </summary>
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


        /// <summary> Resets navigation data of all splines and their nodes </summary>
        public static void InitResetNavigationData()
        {
            Object[] allSplines = GameObject.FindObjectsOfType<SplineC>();
            foreach (SplineC spline in allSplines)
            {
                foreach (SplineN node in spline.nodes)
                {
                    node.ResetNavigationData();
                }
                spline.ResetNavigationData();
            }
        }
    }
}
