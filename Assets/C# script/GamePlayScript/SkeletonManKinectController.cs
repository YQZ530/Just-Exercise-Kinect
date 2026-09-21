using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SkeletonManKinectController : MonoBehaviour {

    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public long userID = 0;

    
    public LineRenderer skeletonLine;
    public LineRenderer[] lines;
    public GameObject lineHolder;

    public GameObject[] bones;
    KinectManager manager;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Vector3 initialPosUser = Vector3.zero;
    private Vector3 initialPosOffset = Vector3.zero;
    private Int64 initialPosUserID = 0;

    //For instance joint comparsion
    public Texture2D heatmap;
    //public SkeletonComparison skeletonScript;
    public GameObjectControllerV4 controllerScript;
    //public AvatarCreationV2 modelScript;
   
 

    float sumdiff = 0f;
    float[] angleDiffArr;
    private void Start()
    {
        if(controllerScript == null)
        {
            Debug.LogError("Script not assign");
        }
       
       
        CreateBones();
        CreateSkeletonLine();
        angleDiffArr = new float[25];
        for (int i = 0; i < 25; i++)
        {
            angleDiffArr[i] = 0f;
        }
    }
    // Update is called once per frame
    void Update()
    {
        manager = KinectManager.Instance;
        /*check if kinect detect a user*/
        if (!manager && !manager.IsInitialized()) { Debug.Log("Cannot find manager!! Cannot Record!!"); return; }
        if (!manager.IsUserDetected()) { /*Debug.Log("Cannot find user!!  Cannot Record!!");*/ return; }

        userID = manager.GetPrimaryUserID();
        //// set the position in space
        //Vector3 posPointMan = manager.GetUserPosition(userID);

        for (int i = 0; i < 25; i++)
        {
            bones[i].transform.position = manager.GetJointPosition(userID, i)+ transform.position;
            
        }
        if (controllerScript.GameIsStart)
        {
            InstanceComparsion();
           
        }

        ReDrawSkeletonLine();

    }
    void CreateBones()
            {
        
                bones = new GameObject[25];
                GameObject[] temp = GameObject.FindGameObjectsWithTag("Skeleton");
                foreach (GameObject g in temp)
                {
                       
                    string index = g.name.Substring(0, 2);
                    int num = int.Parse(index);
                    //int current = (int)  GetLeftRightJoint((KinectInterop.JointType) num); //make left to right 
                    bones[num] = g;

                }

            }
    void CreateSkeletonLine()
    {

       
        //if (!lines[0]) { return; }
        if (skeletonLine)
        {
            // array holding the skeleton lines
            lines = new LineRenderer[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                if (i == 19 || i == 15 || i ==21 || i==22|| i== 23|| i==24) { continue; } //skip  foot ankles

                // Debug.Log("Line: " + i + " instantiate started.");
                lines[i] = Instantiate(skeletonLine, lineHolder.transform) as LineRenderer;

                // lines[i].transform.parent = bones[i].transform.parent;
                // lines[i].transform = bones[i].transform;
                lines[i].gameObject.SetActive(true);
                Vector3 posJoint2 = bones[i].transform.position;
                int parent = (int)GetParentJoint((KinectInterop.JointType)i);

                lines[i].SetPosition(0, bones[parent].transform.position);
                lines[i].SetPosition(1, posJoint2);

            }
        }
    }
    public void ReDrawSkeletonLine()
    {

        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 19 || i == 15 || i == 21 || i == 22 || i == 23 || i == 24) { continue; } //skip hand ankles, foot ankles
            if (!lines[i]) { lines[i] = Instantiate(skeletonLine, lineHolder.transform) as LineRenderer; }

            Vector3 posJoint2 = bones[i].transform.position;
            int parent = (int)GetParentJoint((KinectInterop.JointType)i);

            lines[i].SetPosition(0, bones[parent].transform.position);
            lines[i].SetPosition(1, posJoint2);


            Color c = Get_Color(angleDiffArr[i]);
            c.a = 0.5f;
            bones[i].GetComponent<Renderer>().material.color = c;
            //lines[i].GetComponent<Renderer>().material.SetColor("_TintColor",c );
        }

       ////make hip line is between hip and knee
       //     lines[17].startColor = Get_Color(colorArray[16]);
       //     lines[17].endColor = Get_Color(colorArray[16]);
       // lines[13].startColor = Get_Color(colorArray[12]);
       // lines[13].endColor = Get_Color(colorArray[12]);

       // lines[14].startColor = Get_Color(colorArray[13]);
       // lines[14].endColor = Get_Color(colorArray[13]);
       // lines[18].startColor = Get_Color(colorArray[17]);
       // lines[18].endColor = Get_Color(colorArray[17]);
    }

    void  InstanceComparsion()
    {

        //modelScript = skeletonScript.modelScripts[controllerScript.currentLevelIndex];
        // skeletonScript.LoadImportantJoints(controllerScript.currentLevelIndex);
        // sumdiff = 0f;
        // skeletonScript.InstanceCompareHelper(ref controllerScript.currentLevelIndex, ref bones, ref sumdiff);
        // 
        //sumdiff /= 40f;
        sumdiff = controllerScript.InstanceComparison();
        controllerScript.InstanceArraryComparison(ref angleDiffArr);
        sumdiff /= 0.1f;

    }
    public Color Get_Color(float value)
    {
        if (value == 0f) { value += 0.001f; }
        else if(value >1f) { value = 0.99f; }
        int index = (int)(value * heatmap.width - 1);
        Color outputColor = heatmap.GetPixel(index, 0);
     
        return outputColor;

    }
    public KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint)
    {
        switch (joint)
        {

            case KinectInterop.JointType.Neck:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.Head:
                return KinectInterop.JointType.Neck;

            case KinectInterop.JointType.SpineShoulder:
                return KinectInterop.JointType.SpineMid; // I add
            case KinectInterop.JointType.SpineMid:
                return KinectInterop.JointType.SpineBase; // I add   

            case KinectInterop.JointType.ShoulderLeft:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.ElbowLeft:
                return KinectInterop.JointType.ShoulderLeft;
            case KinectInterop.JointType.WristLeft: //I add
                return KinectInterop.JointType.ElbowLeft;
            case KinectInterop.JointType.HandLeft: // I changed
                return KinectInterop.JointType.WristLeft;


            case KinectInterop.JointType.HandTipLeft: //I add
                return KinectInterop.JointType.HandLeft;
            case KinectInterop.JointType.ThumbLeft: //I add
                return KinectInterop.JointType.HandLeft;

            case KinectInterop.JointType.ShoulderRight:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.ElbowRight:
                return KinectInterop.JointType.ShoulderRight;
            case KinectInterop.JointType.WristRight: //I add
                return KinectInterop.JointType.ElbowRight;
            case KinectInterop.JointType.HandRight:
                return KinectInterop.JointType.WristRight;
            case KinectInterop.JointType.HandTipRight: //I add
                return KinectInterop.JointType.HandRight;
            case KinectInterop.JointType.ThumbRight: //I add
                return KinectInterop.JointType.HandRight;

            case KinectInterop.JointType.HipLeft:
                return KinectInterop.JointType.SpineBase;
            case KinectInterop.JointType.KneeLeft:
                return KinectInterop.JointType.HipLeft;
            case KinectInterop.JointType.AnkleLeft:
                return KinectInterop.JointType.KneeLeft;
            case KinectInterop.JointType.FootLeft: // Iadd
                return KinectInterop.JointType.AnkleLeft;


            case KinectInterop.JointType.HipRight:
                return KinectInterop.JointType.SpineBase;
            case KinectInterop.JointType.KneeRight:
                return KinectInterop.JointType.HipRight;
            case KinectInterop.JointType.AnkleRight:
                return KinectInterop.JointType.KneeRight;
            case KinectInterop.JointType.FootRight: // Iadd
                return KinectInterop.JointType.AnkleRight;
        }

        return joint;
    }

    public KinectInterop.JointType GetLeftRightJoint(KinectInterop.JointType joint)
    {
        switch (joint)
        {
            case KinectInterop.JointType.ShoulderLeft:
                return KinectInterop.JointType.ShoulderRight;
            case KinectInterop.JointType.ElbowLeft:
                return KinectInterop.JointType.ElbowRight;
            case KinectInterop.JointType.WristLeft: //I add
                return KinectInterop.JointType.WristRight;
            case KinectInterop.JointType.HandLeft: // I changed
                return KinectInterop.JointType.HandRight;


            case KinectInterop.JointType.HandTipLeft: //I add
                return KinectInterop.JointType.HandTipRight;
            case KinectInterop.JointType.ThumbLeft: //I add
                return KinectInterop.JointType.ThumbRight;

            case KinectInterop.JointType.ShoulderRight:
                return KinectInterop.JointType.ShoulderLeft; // I changed
            case KinectInterop.JointType.ElbowRight:
                return KinectInterop.JointType.ElbowLeft;
            case KinectInterop.JointType.WristRight: //I add
                return KinectInterop.JointType.WristLeft;
            case KinectInterop.JointType.HandRight:
                return KinectInterop.JointType.HandLeft;
            case KinectInterop.JointType.HandTipRight: //I add
                return KinectInterop.JointType.HandTipLeft;
            case KinectInterop.JointType.ThumbRight: //I add
                return KinectInterop.JointType.ThumbLeft;

            case KinectInterop.JointType.HipLeft:
                return KinectInterop.JointType.HipRight;
            case KinectInterop.JointType.KneeLeft:
                return KinectInterop.JointType.KneeRight;
            case KinectInterop.JointType.AnkleLeft:
                return KinectInterop.JointType.AnkleRight;
            case KinectInterop.JointType.FootLeft: // Iadd
                return KinectInterop.JointType.FootRight;

            case KinectInterop.JointType.HipRight:
                return KinectInterop.JointType.HipLeft;
            case KinectInterop.JointType.KneeRight:
                return KinectInterop.JointType.KneeLeft;
            case KinectInterop.JointType.AnkleRight:
                return KinectInterop.JointType.AnkleLeft;
            case KinectInterop.JointType.FootRight: // Iadd
                return KinectInterop.JointType.FootLeft;
        }

        return joint;
    }
}
