using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TargetSetting))]
public class EditorWithTargetSetting : Editor
{

    TargetSetting tar;
    
    void OnEnable()
    {
        tar = (TargetSetting)target;

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Setting"))
        {
            tar.LoadSetting();
        }

        if (GUILayout.Button("Output Setting"))
        {
            tar.OutputSetting();
        }

        
        EditorGUILayout.EndHorizontal();

        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector();
    }
}
