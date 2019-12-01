#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDSplineI))]
public class GSDSplineIEditor : Editor
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