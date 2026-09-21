using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GenerateKeyPose))]
public class KeyPoseGenerationEditor : Editor {

    GenerateKeyPose tar;


    void OnEnable()
    {
        tar = (GenerateKeyPose)target;

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate")) 
        {
            tar.GenerateAllKeyPoses();
        }
        
        EditorGUILayout.EndHorizontal();

        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector();
    }

}
