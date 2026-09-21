using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrawJointColor))]
public class DrawJointColorEditor : Editor {

    DrawJointColor tar;
    
    void OnEnable()
        {
            tar = (DrawJointColor)target;

        }

   public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("DrawDistJoint"))
            {
                tar.DrawDistJoint();
            }
        if (GUILayout.Button("DrawRotJoint"))
        {
            tar.DrawRotJoint();
        }

        EditorGUILayout.EndHorizontal();

      
            base.DrawDefaultInspector();
        }

    }
   

