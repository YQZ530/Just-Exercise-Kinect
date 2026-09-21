using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseGeneration : MonoBehaviour
{

    enum State { recording, stop, ready }
    State curState = State.ready;
    [SerializeField]
    public class Frame
    {
        public List<Vector3> kinectPos;
        public List<Vector3> pos;
        public List<Quaternion> rot;
        public List<Quaternion> mrot;
        public Vector3 bodypos;
        public Quaternion bodyrot;
        public List<float> rotAngle;

        public Frame(int totaljoint)
        {
            pos = new List<Vector3>(totaljoint);
            rot = new List<Quaternion>(totaljoint);
            mrot = new List<Quaternion>(totaljoint);
            kinectPos = new List<Vector3>(totaljoint);
            rotAngle = new List<float>(totaljoint);
            bodypos = Vector3.zero;
            bodyrot = Quaternion.identity;

        }
    }
    List<Frame> dataFrame;

    KinectManager manager;
    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public LineRenderer skeletonLine;
    public LineRenderer[] lines;
    public GameObject lineHolder;

    public GameObject[] bones;
    long userId;
    public int totalJoint = 25;

  
    //For instance joint comparsion
    public Texture2D heatmap;

    public string outputfileName;
    public string outputPath;
    public string currentPoseNum = "0";
    // Use this for initialization
    void Start()
    {
        dataFrame = new List<Frame>();
        CreateBones();
        CreateSkeletonLine();

    }



    void FixedUpdate()
    {
        manager = KinectManager.Instance;
        /*check if kinect detect a user*/
        if (!manager && !manager.IsInitialized()) { Debug.Log("Cannot find manager!! Cannot Record!!"); return; }
        if (!manager.IsUserDetected()) { /*Debug.Log("Cannot find user!!  Cannot Record!!");*/ return; }

        userId = manager.GetPrimaryUserID();
        ReDrawSkeletonLine();

        for (int i = 0; i < 25; i++)
        {
            bones[i].transform.position = manager.GetJointPosition(userId, i) + transform.position;
        }


        if (curState == State.recording)
        {
            Record();
        }
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
                if (i == 19 || i == 15 || i == 21 || i == 22 || i == 23 || i == 24) { continue; } //skip  foot ankles

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


            //Color c = Get_Color(angleDiffArr[i]);
            //c.a = 0.5f;
            //bones[i].GetComponent<Renderer>().material.color = c;
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
    /*Control by the game; when to start recording*/
    public void StartRecording()
    {
        curState = State.recording;
    }
    /*StopRecording and Given levelname for output */
    public void StopRecording()
    {

        Debug.Log("Stop Recording, ready to output result");
        curState = State.stop;


        OutputData();

        //clear lists of data
        for (int i = 0; i < dataFrame.Count; i++)
        {
            dataFrame[i].kinectPos.Clear();
            dataFrame[i].kinectPos.TrimExcess();
            dataFrame[i].mrot.Clear();
            dataFrame[i].mrot.TrimExcess();
            dataFrame[i].rot.Clear();
            dataFrame[i].rot.TrimExcess();
            dataFrame[i].pos.Clear();
            dataFrame[i].pos.TrimExcess();
            dataFrame[i].rotAngle.Clear();
            dataFrame[i].rotAngle.TrimExcess();
        }
        dataFrame.Clear();
        dataFrame.TrimExcess();


    }
    public void OutputData()
    {
        IniFile outputfile = new IniFile();
        //    StreamWriter writer = new StreamWriter("Assets/SmallUserStudy/" + OutputfileName + levelName + ".txt", true);
        for (int i = 0; i < dataFrame.Count; i++)
        { //for all frames
            outputfile.Create_Section(i.ToString());
            outputfile.Set_Vector3("bodypose", dataFrame[i].bodypos);
            outputfile.Set_Quaternion("bodyRot", dataFrame[i].bodyrot);
            for (int j = 0; j < totalJoint; j++)
            {
                outputfile.Set_Vector3("p" + j.ToString(), dataFrame[i].pos[j]);
                outputfile.Set_Quaternion("r" + j.ToString(), dataFrame[i].rot[j]);
                outputfile.Set_Quaternion("mirrorR" + j.ToString(), dataFrame[i].mrot[j]);
                outputfile.Set_Float("rotAngle" + j.ToString(), dataFrame[i].rotAngle[j]);
               

            }
        }

        outputfile.Create_Section("AvgPose" + currentPoseNum,"avg joint angle for all frames");
        float[] avgJointArr = new float[totalJoint];
        CalculateAvgPoseAngle(ref avgJointArr);
        for (int i = 0; i < totalJoint; i++)
        {
            outputfile.Set_Float(i.ToString(), avgJointArr[i]);
        }

        outputfile.SaveTo(Application.dataPath + "/NewPoseGeneration/" + outputfileName + ".ini");
    }

    public void Record()
    {
       
        Frame oneframe = new Frame(totalJoint);
        oneframe.bodypos = manager.GetUserPosition(userId);
        oneframe.bodyrot = manager.GetUserOrientation(userId, true);

        //save all joint
        for (int i = 0; i < totalJoint; i++)
        {
            oneframe.pos.Add(manager.GetJointPosition(userId, i));
            oneframe.rot.Add(manager.GetJointOrientation(userId, i, true)); //non mirror rotation
            oneframe.rotAngle.Add(RotationCostHelperforModel(i, bones));
            oneframe.mrot.Add(manager.GetJointOrientation(userId, i, false)); //mirror rotation
            // oneframe.kinectPos.Add(manager.GetJointKinectPosition(userId, i));
        }

        dataFrame.Add(oneframe);
    }
    void CalculateAvgPoseAngle(ref float[] jointsAngle)
    {

        float[] tempAngle = new float[totalJoint];
        for (int i = 0; i < totalJoint; i++)
        {
            tempAngle[i] = 0f;
        }

        for (int i = 0; i < dataFrame.Count; i++)
        { //for all frames

            for (int j = 0; j < totalJoint; j++)
            {
                tempAngle[j] += dataFrame[i].rotAngle[j];
            }
        }

        for (int i = 0; i < totalJoint; i++)
        {
            jointsAngle[i] = tempAngle[i]/ (dataFrame.Count*1.0f);
        }
    }

    //Biao version of angle calculation 
    public float RotationCostHelperforModel(int curJoint, GameObject[] model)
    {
        Transform tar = transform, own = transform;
        Vector3 fromJoint = Vector3.zero, toJoint = Vector3.zero;

        switch (curJoint)
        {
            case 0: // spinebase with hips
                {
                    fromJoint = model[0].transform.position - model[1].transform.position;
                    return Vector3.SignedAngle(fromJoint, Vector3.down, Vector3.forward);
                }
            case 1: // spine mid with spinebase and neck
                fromJoint = model[8].transform.position - model[4].transform.position;
                toJoint = Vector3.left;
                return Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);


            //return Vector3.Angle(model[0].transform.position - model[1].transform.position, model[20].transform.position - model[1].transform.position);
            case 4: // left should with shoulder and elbow
                {
                    fromJoint = model[5].transform.position - model[4].transform.position;
                    toJoint = model[20].transform.position - model[4].transform.position;

                    break;
                }
            case 5: // left elbow with left shoulder and wrist
                {
                    return Vector3.Angle(model[4].transform.position - model[5].transform.position, model[6].transform.position - model[5].transform.position);

                }
            case 8: // right shoulder with shoulder and elbow
                {

                    fromJoint = model[9].transform.position - model[8].transform.position;
                    toJoint = model[20].transform.position - model[8].transform.position;

                    break;
                }
            case 9: // right elbow with right shoulder and arm
                {
                    return Vector3.Angle(model[8].transform.position - model[9].transform.position, model[10].transform.position - model[9].transform.position);


                }
            case 12: // left hip with base and knee
                     //16-12 13-12
                return Vector3.Angle(model[0].transform.position - model[12].transform.position, model[13].transform.position - model[12].transform.position);
            case 13: // left knwee
                {
                    return Vector3.Angle(model[12].transform.position - model[13].transform.position, model[14].transform.position - model[13].transform.position);
                }
            case 16: // left hip with base and knee
                // 12 -16 ,
                return Vector3.Angle(model[0].transform.position - model[16].transform.position, model[17].transform.position - model[16].transform.position);
            case 17: // left knwee
                {
                    return Vector3.Angle(model[16].transform.position - model[17].transform.position, model[18].transform.position - model[17].transform.position);
                }
        }



        if (fromJoint == Vector3.zero)
            return 0f;


        float angle = Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);

        return -angle;



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

