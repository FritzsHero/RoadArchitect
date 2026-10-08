#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
using UnityEngine.Scripting.APIUpdating;
#endregion


namespace RoadArchitect
{
    [CustomEditor(typeof(SplineCreationPreview))]
    #if UNITY_2019_3_OR_NEWER
    [MovedFrom(true, sourceClassName: "SplineFEditor")]
    #endif
    public class SplineCreationPreviewEditor : Editor
    {
        private SplineCreationPreview splineF;


        private void OnEnable()
        {
            splineF = (SplineCreationPreview)target;
        }


        public override void OnInspectorGUI()
        {
            //Intentionally left empty.
        }
    }
}
#endif
