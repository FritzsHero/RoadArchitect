#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(SplineInsertionPreview))]
    #if UNITY_2019_3_OR_NEWER
    [MovedFrom(true, sourceClassName: "SplineIEditor")]
    #endif
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
