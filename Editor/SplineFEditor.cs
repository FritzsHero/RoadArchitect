#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(GSDSplineF))]
    public class SplineFEditor : Editor
    {
        private GSDSplineF splineF;


        private void OnEnable()
        {
            splineF = (GSDSplineF)target;
        }


        public override void OnInspectorGUI()
        {
            //Intentionally left empty.
        }
    }
#endif
}