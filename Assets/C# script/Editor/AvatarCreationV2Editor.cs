using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AvatarCreationV2))]
public class AvatarCreationV2Editor : Editor {

    AvatarCreationV2 tar;


	void OnEnable () {
		tar = (AvatarCreationV2)target;
	
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button("Load Skeleton"))
		{
            tar.LoadSkeletonInfomation();
		}
        if (GUILayout.Button("Fixed Skeleton"))
        {
            tar.FixedAndSaveSkeletonInfo();
        }

        if (GUILayout.Button("Print Skeleton rotatioin"))
		{
           tar.PrintSkeletonBonesRotation();
		}
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Skeleton Rotation"))
		{
            tar.LoadBonesRotation();

        }
        if (GUILayout.Button("Redraw Skeleton Lines"))
        {
            tar.ReDrawSkeletonLine();
        }
        if (GUILayout.Button("Clear Skeleton Lines"))
        {
            tar.clearSkeletonLine();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Mirror Motion"))
        {
            tar.CreateMirrorMotion();
        }

        if (GUILayout.Button("Remap LR  position"))
        {
            tar.ReMapRLPosition();
        }
            EditorGUILayout.EndHorizontal();
        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector ();
	}

}
