using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawErrorJoint))]
public class ErrorJointEditor : Editor {
    DrawErrorJoint tar;
    // Use this for initialization

    void OnEnable()
    {
        tar = (DrawErrorJoint)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("DrawDefaultFigure"))
        {
            tar.DrawDefaultRot();
        }
        if (GUILayout.Button("DrawMediumRotFigure"))
        {
            tar.DrawMediumRot();
        }
        if (GUILayout.Button("DrawHighRotFigure"))
        {
            tar.DrawHighRot();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("DrawMediumCMFigure"))
        {
            tar.DrawMediumCM();
        }
        if (GUILayout.Button("DrawHighCMFigure"))
        {
            tar.DrawHighCM();

        }


        EditorGUILayout.EndHorizontal();

        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector();
    }
}
