using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QPoser))]
public class QPoserEditor : Editor {

    QPoser tar;

    public void OnEnable()
    {
        tar = (QPoser)target;
    }

    public override void OnInspectorGUI()
    {
        //if (GUILayout.Button("Mirror"))
        //{
        //    tar.Do_Mirror();
        //}

        //if (GUILayout.Button("Screenshot"))
        //{
        //    string filePath = EditorUtility.SaveFilePanel("screen shot", Application.dataPath+"/Screenshots/", "Keypose3d_" + tar.name, "png");
        //    if (!string.IsNullOrEmpty(filePath))
        //    {
        //        //ScreenCapture.CaptureScreenshot(filePath);
        //    }
        //}

        base.OnInspectorGUI();
        
    }

}
