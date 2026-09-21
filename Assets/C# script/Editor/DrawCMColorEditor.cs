using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawCMMatric))]
public class DrawCMColorEditor : Editor {

    DrawCMMatric tar;
    // Use this for initialization
    void OnEnable()
    {
        tar = (DrawCMMatric)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

       
        if (GUILayout.Button("DrawCMMatrics"))
        {
            tar.PrintCMPicture();
        }
        if (GUILayout.Button("DrawLegend"))
        {
            tar.DrawLegend();
        }


        EditorGUILayout.EndHorizontal();

        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector();
    }
}
