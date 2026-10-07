#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(SplineInsertionPreview))]
    [MovedFrom(true, sourceClassName: "SplineIEditor")]
    public class SplineInsertionPreviewEditor : Editor
    {
        private SplineInsertionPreview splineI;


        private void OnEnable()
        {
            splineI = (SplineInsertionPreview)target;
        }


        public override void OnInspectorGUI()
        {
            //Intentionally left empty.
        }
    }
}
#endif
