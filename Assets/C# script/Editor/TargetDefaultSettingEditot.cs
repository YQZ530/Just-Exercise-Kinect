using System.Collections;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(DefaultLevelSetting))]
public class DefaultLevelSettingEditot : Editor {

    DefaultLevelSetting tar;
    // Use this for initialization
    void OnEnable()
    {
		tar = (DefaultLevelSetting)target;
	}

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("LoadDefaultLevel"))
        {
            tar.LoadDefaultLevel();
        }
        if (GUILayout.Button("CalculateDefaultLevel"))
        {
            tar.CalculateDefaultLevel();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("PrintDefaultToTargetFile"))
        {
            tar.OutputToTargetFile();
        }
        if (GUILayout.Button("UpdateDefaultFile"))
        {
            tar.UpdateDefaultFile();
        }
        EditorGUILayout.EndHorizontal();

        //tar.str = EditorGUILayout.DelayedTextField ("sta:",tar.str);

        // tar.go = (GameObject)EditorGUILayout.ObjectField(tar.go);

        base.DrawDefaultInspector();
    }
}
