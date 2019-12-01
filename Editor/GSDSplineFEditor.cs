#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDSplineF))]
public class GSDSplineFEditor : Editor
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