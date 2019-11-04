using UnityEngine;


public class RAEditorUtilitys : MonoBehaviour
{


    public static void DrawLine()
    {
        //Horizontal bar
        GUILayout.Space(4f);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
    }


    public static void Line(float _spacing = 4f, float _size = 1f)
    {
        //Horizontal bar
        GUILayout.Space(_spacing);
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(_size));
        GUILayout.Space(_spacing);
    }
}