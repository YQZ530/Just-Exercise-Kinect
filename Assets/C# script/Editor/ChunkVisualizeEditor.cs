using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*Editor that Work with MCMCPOsevisualization Script*/

[CustomEditor(typeof(MCMCPoseVisualization))]

public class ChunkVisualizeEditor : Editor
{
    MCMCPoseVisualization visualScript;

    void OnEnable()
    {
        visualScript = (MCMCPoseVisualization)target;

    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Display Default"))
        {
            visualScript.DisplayDefaultLevel();
        }


        if (GUILayout.Button("Easy"))
        {
            visualScript.DisplayEasyLevel();
        }

        if (GUILayout.Button("Clear"))
        {
            visualScript.ClearDisplayObject();
           
        }

        if (GUILayout.Button("Medium"))
        {
            visualScript.DisplayMediumLevel();
        }
        if (GUILayout.Button("Hard"))
        {
            visualScript.DisplayHardLevel();
        }


        EditorGUILayout.EndHorizontal();

       

        base.DrawDefaultInspector();
    }
}
