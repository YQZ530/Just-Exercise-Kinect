using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AvatarCreation))]
public class DemoEditor : Editor {

    AvatarCreation tar;


	void OnEnable () {
		tar = (AvatarCreation)target;
	
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button("Load Skeleton"))
		{
            tar.LoadSkeletonInfomation();
		}

		if (GUILayout.Button("Print Skeleton rotatioin"))
		{
           tar.PrintSkeletonBonesRotation();
		}

		if (GUILayout.Button("Load Skeleton Rotation"))
		{
            tar.LoadBonesRotation();

        }

		EditorGUILayout.EndHorizontal ();

		//tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);
	
		// tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

		base.DrawDefaultInspector ();
	}

}
