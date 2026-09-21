using System.IO;
using System.Collections.Generic;
using UnityEngine;


// Record User Bone information before MCMC 

public class UserCalibrationScript : MonoBehaviour {

   

    enum RecordingState { start, stop, empty };
    RecordingState curState;

    public KinectManager manager;
    public SpeechManager speechManager;
    long userId;
    [Tooltip("UI-Text to display information messages.")]
    public UnityEngine.UI.Text infoText;

   
    public int TotalaffJoint = 25;


    public int fileindex = 1;
    public string filePath = "/User/";
    public string keyposeFilename = "keypose";

   
    void FixedUpdate()
    {

        manager = KinectManager.Instance;

        if (speechManager == null)
        {
            speechManager = SpeechManager.Instance;
        }

        if (speechManager != null && speechManager.IsSapiInitialized())
        {
            if (speechManager.IsPhraseRecognized())
            {

                string sPhraseTag = speechManager.GetPhraseTagRecognized();
                
                switch (sPhraseTag)  //HAS TO BE CAPTITALIZE!!!!
                {

                    case "STOP":
                        RecordCurrentPose();
                        if (infoText != null)
                        {
                            infoText.text = " Output.";
                        }
                        break;

                }

                speechManager.ClearPhraseRecognized();
            }
        }


        

    }
    List<float> BoneLength;
    /*joint index are base on kinect def*/
    public void RecordCurrentPose() //from kinect sensor
    {
        /*check if kinect detect a user*/
        if (!manager && !manager.IsInitialized()) { Debug.Log("Cannot find manager!! Cannot Record!!"); return; }
        if (!manager.IsUserDetected()) { Debug.Log("Cannot find user!!  Cannot Record!!"); return; }
       
        manager = KinectManager.Instance;
        userId = manager.GetPrimaryUserID();

        StreamWriter writer = new StreamWriter(Application.dataPath+ filePath + keyposeFilename + fileindex.ToString() + ".txt", true);

        ///*Version one : Save all joints info */
        for (int i = 0; i < TotalaffJoint; i++)
        {
            Vector3 v3 = manager.GetJointPosition(userId, i);
            writer.Write(v3.x + " " + v3.y + " " + v3.z);
            if (i != TotalaffJoint - 1) { writer.Write("|"); }
        }
        writer.WriteLine();

        writer.Close();

        print("Key Pose is saved");

    }

}
