using System.Collections;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GenMass))]

public class GenMassEditor:Editor{
	GenMass tar;

	void OnEnable()
	{
		tar = (GenMass)target;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Get Bones")) {
			tar.GetBones ();
		}
		if (GUILayout.Button ("Get Pose")) {
			tar.GetPose();
			tar.GetPose();
		}
		if (GUILayout.Button ("Dist")) {
			tar.CaptureDist ();
		}

		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
        if (GUILayout.Button("Rot"))
        {
            tar.CaptureRot();
        }

        if (GUILayout.Button ("COM Cap")) {
			tar.CaptureCOM ();
		}
		EditorGUILayout.EndHorizontal ();
		base.DrawDefaultInspector ();
	}

}
