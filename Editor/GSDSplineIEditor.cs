#if UNITY_EDITOR
#region "Imports"
using UnityEditor;
#endregion


[CustomEditor(typeof(GSDSplineI))]
public class GSDSplineIEditor : Editor
{
    protected GSDSplineI splineI { get { return (GSDSplineI) target; } }


    public override void OnInspectorGUI()
    {
        //Intentionally left empty.
    }
}
#endif