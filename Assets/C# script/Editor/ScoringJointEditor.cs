using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScoringJoint))]
public class ScoringJointEditor : Editor {

    ScoringJoint visualScript;

    void OnEnable()
    {
        visualScript = (ScoringJoint)target;

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Readfile"))
        {
            visualScript.ReadJoint();
        }
        if (GUILayout.Button("OutputTofile"))
        {
            visualScript.Outputfile();
        }
        EditorGUILayout.EndHorizontal();

        base.DrawDefaultInspector();
    }
}
