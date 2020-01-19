#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(GSDSplineI))]
    public class SplineIEditor : Editor
    {
        private GSDSplineI splineI;


        private void OnEnable()
        {
            splineI = (GSDSplineI)target;
        }


        public override void OnInspectorGUI()
        {
            //Intentionally left empty.
        }
    }
#endif
}