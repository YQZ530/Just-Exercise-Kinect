using UnityEngine;
//using Windows.Kinect;
using System.Collections.Generic;
using System;
using System.IO;

public class AvatarCreationV2 : MonoBehaviour
{
    public GameObject Hip_Center;
    public GameObject Spine;
    public GameObject Neck;
    public GameObject Head;
    public GameObject Shoulder_Left;
    public GameObject Elbow_Left;
    public GameObject Wrist_Left;
    public GameObject Hand_Left;
    public GameObject Shoulder_Right;
    public GameObject Elbow_Right;
    public GameObject Wrist_Right;
    public GameObject Hand_Right;
    public GameObject Hip_Left;
    public GameObject Knee_Left;
    public GameObject Ankle_Left;
    public GameObject Foot_Left;
    public GameObject Hip_Right;
    public GameObject Knee_Right;
    public GameObject Ankle_Right;
    public GameObject Foot_Right;
    public GameObject Spine_Shoulder;
    public GameObject Hand_Tip_Left;
    public GameObject Thumb_Left;
    public GameObject Hand_Tip_Right;
    public GameObject Thumb_Right;

    public LineRenderer skeletonLine;
//    public LineRenderer debugLine;

    public GameObject[] bones;
    public List<float> BoneLength;
    public LineRenderer[] lines;
    public GameObject lineHolder;

    private LineRenderer lineTLeft;
    private LineRenderer lineTRight;
    private LineRenderer lineFLeft;
    private LineRenderer lineFRight;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Vector3 initialPosUser = Vector3.zero;
    private Vector3 initialPosOffset = Vector3.zero;
    private Int64 initialPosUserID = 0;
    public Vector3 txtinitialPosition = Vector3.zero;

    public string SkeletonfileName = "keypose1";
    public string SkeletonfilePath = "TestInput2";

    [Header("Print skeleton Rotation file path")]
    public string protFilePath = "PoseRotation";

    [Header("Load skeleton Rotation file path")]
    public string rotFileName = "keypose1";
    public string rotFilePath = "PoseRotation";
    

    void Start()
    {
        ////store bones in a list for easier access


        //// array holding the skeleton lines
        //lines = new LineRenderer[bones.Length];
        //LoadFileInfomation();
        ////		if(skeletonLine)
        ////		{
        ////			for(int i = 0; i < lines.Length; i++)
        ////			{
        ////				Debug.Log ("Line: " + i + " instantiate started.");
        ////
        ////				if((i == 22 || i == 24) && debugLine)
        ////					lines[i] = Instantiate(debugLine) as LineRenderer;
        ////				else
        ////					lines[i] = Instantiate(skeletonLine) as LineRenderer;
        ////
        ////				lines[i].transform.parent = transform;
        ////			}
        ////		}


    }

    // loading user skeleton 
    public void LoadSkeletonInfomation()
    {
        CreateBoneList();
        StreamReader reader = new StreamReader(Application.dataPath + "/" + SkeletonfilePath + "/" + SkeletonfileName + ".txt");

        Vector3[] skeletonTransform = new Vector3[bones.Length];
        //add all the position info to a chunk
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            if (line == null) { Debug.Log("EOF"); return; }

            string[] array = line.Split('|');

            for (int i = 0; i < bones.Length; i++)
            {

                string[] temp = array[i].Split(' ');
                float x = 0f, y = 0f, z = 0f;
                float.TryParse(temp[0], out x);
                float.TryParse(temp[1], out y);
                float.TryParse(temp[2], out z);

                skeletonTransform[i] = new Vector3(x, y, z);
            }

            CalculateBoneLength();
            Vector3 v = this.transform.position - skeletonTransform[0]; //current position
            bones[0].transform.position = this.transform.position;
            bones[1].transform.position = skeletonTransform[1] + v; ;
            bones[2].transform.position = skeletonTransform[2] + v ;
            bones[20].transform.position = skeletonTransform[20] + v ;
            for (int i = 3; i < bones.Length; i++)
            {
                if(i == 20) { continue;  }
               
                bones[i].transform.position = skeletonTransform[i] + v; 
            }

        }
        txtinitialPosition = skeletonTransform[0];

        //draw skeleton line
        // CreateSkeletonLine();
        ReDrawSkeletonLine();
    }
    //print user skeleton after minor correction
    //cound overwrite the original file
    public void FixedAndSaveSkeletonInfo()
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/" + SkeletonfilePath + "/" + SkeletonfileName + ".txt", false);
        for (int i = 0; i < bones.Length; i++)
        {
            writer.Write(bones[i].transform.position.x + " " + bones[i].transform.position.y + " " +bones[i].transform.position.z);
            if(i < bones.Length - 1)
            {
                writer.Write("|");
            } 
        }
        writer.WriteLine();
        writer.Close();
        Debug.Log("Finish fixing and print");
    }

    //for easy to access each joint
    void CreateBoneList()
    {
        bones = new GameObject[] {
            Hip_Center,
            Spine,
            Neck,
            Head,
            Shoulder_Left,
            Elbow_Left,
            Wrist_Left,
            Hand_Left,
            Shoulder_Right,
            Elbow_Right,
            Wrist_Right,
            Hand_Right,
            Hip_Left,
            Knee_Left,
            Ankle_Left,
            Foot_Left,
            Hip_Right,
            Knee_Right,
            Ankle_Right,
            Foot_Right,
            Spine_Shoulder,
            Hand_Tip_Left,
            Thumb_Left,
            Hand_Tip_Right,
            Thumb_Right
        };
    }
   
    //Draw line that connect bones
    void CreateSkeletonLine()
    {
        
        // array holding the skeleton lines
        
        lines = new LineRenderer[bones.Length];
        //if (!lines[0]) { return; }
        if (skeletonLine)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                if (i == 19 || i == 15 || i == 7 || i == 11 || i == 21 || i == 22 || i == 23 || i == 24) { continue; } //skip  foot ankles

               // Debug.Log("Line: " + i + " instantiate started.");
                lines[i] = Instantiate(skeletonLine,lineHolder.transform) as LineRenderer;

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
        clearSkeletonLine();
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 19 || i == 15 || i == 7 || i == 11 || i == 21 || i == 22|| i == 23 || i == 24) { continue; } //skip hand ankles, foot ankles
            if (!lines[i]) { lines[i] = Instantiate(skeletonLine, lineHolder.transform) as LineRenderer; }
           
            Vector3 posJoint2 = bones[i].transform.position;
            int parent = (int)GetParentJoint((KinectInterop.JointType)i);
            
            lines[i].SetPosition(0, bones[parent].transform.position);
            lines[i].SetPosition(1, posJoint2);
        }
       // print("skeleton line have been redraw");
    }

    public void clearSkeletonLine()
    {
        //print(lineHolder.transform.childCount);
        for (int i = lineHolder.transform.childCount-1; i > 0; i--)
        {
            DestroyImmediate(lineHolder.transform.GetChild(i).gameObject);
        }
       
        //if(lines.Length> 0)
        //{
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        if (i == 19 || i == 15 || i == 7 || i == 11 || i == 21 || i == 22 || i == 23 || i == 24) { continue; }
        //       
        //    }
        //    Array.Clear(lines, 0, lines.Length);
        //}
    }

    public void PrintSkeletonBonesRotation()
    {
        CreateBoneList(); //initialize bones[]
        StreamWriter writer = new StreamWriter(Application.dataPath + "/" + protFilePath + "/" + SkeletonfileName + ".txt");

        Vector4[] bonesRot = new Vector4[bones.Length];
        //first calculaate bone rotation respect to its parents
        //Follow Hierachy of bones, rotation data store in bonesRot
        CalculateBonesRotation(0, ref bonesRot);
        CalculateBonesRotation(1, ref bonesRot);
        CalculateBonesRotation(20, ref bonesRot);
        for (int i = 2; i < bones.Length; i++)
        {
            if (i == 20) { continue; }
            CalculateBonesRotation(i, ref bonesRot);
        }

        for (int i = 0; i < bones.Length; i++)
        {
            writer.Write(bonesRot[i].x + " " + bonesRot[i].y + " " + bonesRot[i].z + " " + bonesRot[i].w);
            if (i < bones.Length - 1)
            {
                writer.Write("|");
            }
        }
        print("finish printting rotation;");
        writer.WriteLine();
        writer.Close();
    }
    void CalculateBonesRotation(int i, ref Vector4[] bonesrot)
    {
        int parent = (int)GetParentJoint((KinectInterop.JointType)i);
        Vector3 v = bones[i].transform.position - bones[parent].transform.position;
        float length = v.magnitude;
        float alpha = Mathf.Acos(v.x / length);
        float beta = Mathf.Acos(v.y / length);
        float gamma = Mathf.Acos(v.z / length);

        bonesrot[i] = new Vector4(alpha, beta, gamma, length);

    }


    public void LoadBonesRotation()
    {
       
        StreamReader reader = new StreamReader(Application.dataPath + "/" + rotFilePath + "/" + rotFileName + ".txt");

        Vector4[] boneRotBuffer = new Vector4[bones.Length];
        while (!reader.EndOfStream)
        {
            ReadInputFile(reader, ref boneRotBuffer); //add all the position info to a chunk
        }
        reader.Close();
        ReMapBone(ref boneRotBuffer);
        ReMapBone(ref boneRotBuffer);



        print("Finish Loading");
        ReDrawSkeletonLine();
    }

    void ReadInputFile(StreamReader reader, ref Vector4[] boneRotBuffer )
    {
        string line = reader.ReadLine();
        if (line == null) { Debug.Log("EOF"); return; }
        string[] array = line.Split('|');

       
        //ReMapBone(0, ref array); //base
        for (int i = 1; i < bones.Length; i++)
        {
            GetBoneRotion(i,  ref array, ref boneRotBuffer); //get all bone rotation from the txt file
        }

       
    }

    //get bone rotation from a line 
    void GetBoneRotion(int joint_i,  ref string[] array, ref Vector4[] boneRotBuffer)
    {
        string[] temp = array[joint_i].Split(' ');
        float alpha = 0f, beta = 0f, gamma = 0f, vectorLength = 0f;  // alpha beta gamma are angle to x y z coordinate
        float.TryParse(temp[0], out alpha);
        float.TryParse(temp[1], out beta);
        float.TryParse(temp[2], out gamma);
        float.TryParse(temp[3], out vectorLength);

        boneRotBuffer[joint_i] = new Vector4(alpha, beta, gamma, vectorLength);
        
    }


    //calibrate pose
    void ReMapBone(ref Vector4[] boneRotBuffer)
    {
        // CalculateBoneLength(); //calculate current avatar bone length
        ReMapBoneHelper(ref boneRotBuffer, 0);
        ReMapBoneHelper(ref boneRotBuffer, 1);

        ReMapBoneHelper(ref boneRotBuffer, 20);
        ReMapBoneHelper(ref boneRotBuffer, 2);
        ReMapBoneHelper(ref boneRotBuffer, 3);

        // ReMapBone(ref boneRotBuffer, 8);
        // ReMapBone(ref boneRotBuffer, 4);
        for (int i = 4; i < boneRotBuffer.Length; i++)
        {

            if (i == 20) { continue; }
         
            ReMapBoneHelper(ref boneRotBuffer, i);
        }


    }
    void ReMapBoneHelper(ref Vector4[] boneRotbuffer, int joint_i)
    {       
            float vectorLength = BoneLength[joint_i];
            float vx = vectorLength * Mathf.Cos(boneRotbuffer[joint_i].x);
            float vy = vectorLength * Mathf.Cos(boneRotbuffer[joint_i].y);
            float vz = vectorLength * Mathf.Cos(boneRotbuffer[joint_i].z);
            
            int parent = (int)GetParentJoint((KinectInterop.JointType)joint_i);
            bones[joint_i].transform.rotation = new Quaternion(); //reset object rotation
            bones[joint_i].transform.position = new Vector3(vx, vy, vz) + bones[parent].transform.position;
       
       
        //  print(new Vector3(vx, vy, vz));
    }

    void CalculateBoneLength()
    {
        BoneLength = new List<float>();

        for (int i = 0; i < bones.Length; i++)
        {
            int parent = (int)GetParentJoint((KinectInterop.JointType)i);
            float dist = Vector3.Distance(bones[parent].transform.position, bones[i].transform.position);
            BoneLength.Add(dist);
            //  print((KinectInterop.JointType)i + " leng is " + dis);
        }
    }
    
    //Given current pose, it transform to mirror pose
    public void CreateMirrorMotion()
    {
        List<Vector3> postion = new List<Vector3>();
        for (int i = 0; i < 25; i++)
        { //get the other  side position
         
            CreateMirrorMotionHelper(i,ref  postion);
           
        }
        print(postion.Count);
       
        for (int i = 0; i < 25; i++)
        {
            //int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);

                 //if(i == otherJoint) { continue;  } //skip those does not have R/L side
            if(i == 0) { continue; }
            bones[i].transform.position = postion[i]+ bones[0].transform.position;
            
        }

        }
    void CreateMirrorMotionHelper(int i, ref List<Vector3> position)
    {
        int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);

        Vector3 v1 = bones[i].transform.position - bones[0].transform.position;
        Vector3 v2 = bones[otherJoint].transform.position - bones[0].transform.position;
        //    new Vector3(-bones[otherJoint].transform.position.x, bones[otherJoint].transform.position.y, bones[otherJoint].transform.position.z);
        Vector3 v3 = new Vector3(-v2.x, v2.y, v2.z);

        position.Add ( v3);
        
    }
    public void ReMapRLPosition()
    {
        List<Vector3> postion = new List<Vector3>();
        for (int i = 0; i < 25; i++)
        {
            ReMapeRLPositionHelper(i, ref postion); //get the other side data
        }

        bones[2].transform.position = postion[2] + bones[20].transform.position;
        float x = bones[2].transform.position.x;
        bones[3].transform.position = new Vector3(x, bones[3].transform.position.y, bones[3].transform.position.z);  
        for (int i = 16; i < 20; i++)
        {
            bones[i].transform.position = postion[i] + bones[0].transform.position;
        }

        for (int i = 8; i < 12; i++)
        {

            bones[i].transform.position = postion[i] + bones[20].transform.position;
        }

        bones[23].transform.position = postion[23] + bones[20].transform.position;
        bones[24].transform.position = postion[24] + bones[20].transform.position;
        //make head and neck one same line with sphine
        
    }
    void ReMapeRLPositionHelper(int i, ref List<Vector3> position)
    {

        if((i >7 && i < 12) || i == 23 || i== 24) //hand part joints
        {
            int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
            Vector3 v1 = bones[otherJoint].transform.position - bones[20].transform.position;
            position.Add(new Vector3(-v1.x, v1.y, v1.z));
        }
        else if(i == 2|| i == 3)
        {
            int otherJoint = 20;
            Vector3 v1 = bones[otherJoint].transform.position - bones[20].transform.position;
            Vector3 v2 = bones[i].transform.position - bones[20].transform.position;
            position.Add(new Vector3(v1.x, v2.y, v2.z));

        }
        else
        {
            int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
            Vector3 v1 = bones[otherJoint].transform.position - bones[0].transform.position;
            position.Add(new Vector3(-v1.x, v1.y, v1.z));
        }
        

        //position.Add(new Vector3(-bones[otherJoint].transform.position.x, bones[otherJoint].transform.position.y, bones[otherJoint].transform.position.z));

    }

    //make left side and right side has same y and z
    public void RescalePose()
    {

    }
    public KinectInterop.JointType GetLeftRightJoint(KinectInterop.JointType joint)
	{
		switch(joint){
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

    public KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint)
    {
        switch (joint)
        {
            case KinectInterop.JointType.SpineBase:
                return KinectInterop.JointType.Neck;
            case KinectInterop.JointType.Neck:
                return KinectInterop.JointType.Head;

            case KinectInterop.JointType.ShoulderLeft:
                return KinectInterop.JointType.ElbowLeft;
            case KinectInterop.JointType.ElbowLeft:
                return KinectInterop.JointType.HandLeft;

            case KinectInterop.JointType.ShoulderRight:
                return KinectInterop.JointType.ElbowRight;
            case KinectInterop.JointType.ElbowRight:
                return KinectInterop.JointType.HandRight;

            case KinectInterop.JointType.HipLeft:
                return KinectInterop.JointType.KneeLeft;
            case KinectInterop.JointType.KneeLeft:
                return KinectInterop.JointType.AnkleLeft;

            case KinectInterop.JointType.HipRight:
                return KinectInterop.JointType.KneeRight;
            case KinectInterop.JointType.KneeRight:
                return KinectInterop.JointType.AnkleRight;
        }

        return joint;  // end joint
    }
}

//void Update()
//{
//    KinectManager manager = KinectManager.Instance;

//    // get 1st player
//    Int64 userID = manager ? manager.GetUserIdByIndex(playerIndex) : 0;

//    if (userID <= 0)
//    {
//        initialPosUserID = 0;
//        initialPosOffset = Vector3.zero;
//        initialPosUser = Vector3.zero;

//        // reset the pointman position and rotation
//        if (transform.position != initialPosition)
//        {
//            transform.position = initialPosition;
//        }

//        if (transform.rotation != initialRotation)
//        {
//            transform.rotation = initialRotation;
//        }

//        for (int i = 0; i < bones.Length; i++)
//        {
//            bones[i].gameObject.SetActive(true);

//            bones[i].transform.localPosition = Vector3.zero;
//            bones[i].transform.localRotation = Quaternion.identity;

//            if (lines[i] != null)
//            {
//                lines[i].gameObject.SetActive(false);
//            }
//        }

//        return;
//    }

//    // set the position in space
//    Vector3 posPointMan = manager.GetUserPosition(userID);
//    Vector3 posPointManMP = new Vector3(posPointMan.x, posPointMan.y, !mirroredMovement ? -posPointMan.z : posPointMan.z);

//    // store the initial position
//    if (initialPosUserID != userID)
//    {
//        initialPosUserID = userID;
//        //initialPosOffset = transform.position - (verticalMovement ? posPointMan * moveRate : new Vector3(posPointMan.x, 0, posPointMan.z) * moveRate);
//        initialPosOffset = posPointMan;

//        initialPosUser = initialPosition;
//        if (verticalMovement)
//            initialPosUser.y = 0f;  // posPointManMP.y provides the vertical position in this case
//    }

//    Vector3 relPosUser = (posPointMan - initialPosOffset);
//    relPosUser.z = !mirroredMovement ? -relPosUser.z : relPosUser.z;


//    transform.position = verticalMovement ? initialPosUser + posPointManMP * moveRate :
//        initialPosUser + new Vector3(posPointManMP.x, 0, posPointManMP.z) * moveRate;

//    //Debug.Log (userID + ", pos: " + posPointMan + ", ipos: " + initialPosUser + ", rpos: " + posPointManMP + ", tpos: " + transform.position);

//    // update the local positions of the bones
//    for (int i = 0; i < bones.Length; i++)
//    {
//        if (bones[i] != null)
//        {
//            int joint = !mirroredMovement ? i : (int)KinectInterop.GetMirrorJoint((KinectInterop.JointType)i);
//            if (joint < 0)
//                continue;

//            if (manager.IsJointTracked(userID, joint))
//            {
//                bones[i].gameObject.SetActive(true);

//                Vector3 posJoint = manager.GetJointPosition(userID, joint);
//                posJoint.z = !mirroredMovement ? -posJoint.z : posJoint.z;

//                Quaternion rotJoint = manager.GetJointOrientation(userID, joint, !mirroredMovement);
//                rotJoint = initialRotation * rotJoint;

//                posJoint -= posPointManMP;

//                if (mirroredMovement)
//                {
//                    posJoint.x = -posJoint.x;
//                    posJoint.z = -posJoint.z;
//                }

//                bones[i].transform.localPosition = posJoint;
//                bones[i].transform.rotation = rotJoint;

//                if (lines[i] == null && skeletonLine != null)
//                {
//                    lines[i] = Instantiate((i == 22 || i == 24) && debugLine ? debugLine : skeletonLine) as LineRenderer;
//                    lines[i].transform.parent = transform;
//                }

//                if (lines[i] != null)
//                {
//                    lines[i].gameObject.SetActive(true);
//                    Vector3 posJoint2 = bones[i].transform.position;

//                    Vector3 dirFromParent = manager.GetJointDirection(userID, joint, false, false);
//                    dirFromParent.z = !mirroredMovement ? -dirFromParent.z : dirFromParent.z;
//                    Vector3 posParent = posJoint2 - dirFromParent;

//                    //lines[i].SetVertexCount(2);
//                    lines[i].SetPosition(0, posParent);
//                    lines[i].SetPosition(1, posJoint2);
//                }

//            }
//            else
//            {
//                bones[i].gameObject.SetActive(false);

//                if (lines[i] != null)
//                {
//                    lines[i].gameObject.SetActive(false);
//                }
//            }
//        }
//    }
//}

////****//
//public KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint)
//{
//    switch (joint)
//    {
//        case KinectInterop.JointType.SpineBase:
//            return KinectInterop.JointType.Neck;
//        case KinectInterop.JointType.Neck:
//            return KinectInterop.JointType.Head;

//        case KinectInterop.JointType.ShoulderLeft:
//            return KinectInterop.JointType.ElbowLeft;
//        case KinectInterop.JointType.ElbowLeft:
//            return KinectInterop.JointType.HandLeft;

//        case KinectInterop.JointType.ShoulderRight:
//            return KinectInterop.JointType.ElbowRight;
//        case KinectInterop.JointType.ElbowRight:
//            return KinectInterop.JointType.HandRight;

//        case KinectInterop.JointType.HipLeft:
//            return KinectInterop.JointType.KneeLeft;
//        case KinectInterop.JointType.KneeLeft:
//            return KinectInterop.JointType.AnkleLeft;

//        case KinectInterop.JointType.HipRight:
//            return KinectInterop.JointType.KneeRight;
//        case KinectInterop.JointType.KneeRight:
//            return KinectInterop.JointType.AnkleRight;
//    }

//    return joint;  // end joint
//}

/****/
//public void PrintBonesRotation()
//{
//    CreateBoneList();
//    StreamWriter writer = new StreamWriter(Application.dataPath + "/" + "PoseRotation" + "/" + fileName + ".txt");

//    print("print rotation");
//    for (int i = 0; i < bones.Length; i++)
//    {
//        Vector3 v = bones[i].transform.position - bones[0].transform.position;
//        float length = v.magnitude;
//        float alpha = Mathf.Acos(v.x / length);
//        float beta = Mathf.Acos(v.y / length);
//        float gamma = Mathf.Acos(v.z / length);
//        writer.Write(alpha + " " + beta + " " + gamma + " " + length);
//        if (i < bones.Length - 1)
//        {
//            writer.Write("|");
//        }
//        print(v);
//    }
//    print("finish printting rotation;");
//    writer.WriteLine();
//    writer.Close();
//}

//public void LoadBonesRotation()
//{
//    StreamReader reader = new StreamReader(Application.dataPath + "/" + "PoseRotation" + "/" + "keypose3" + ".txt");

//    while (!reader.EndOfStream)
//    {
//        ReadInputFile(reader); //add all the position info to a chunk
//    }
//    reader.Close();


//    print("Finish Loading");

//}

//void ReadInputFile(StreamReader reader)
//{
//    string line = reader.ReadLine();
//    if (line == null) { Debug.Log("EOF"); return; }
//    string[] array = line.Split('|');

//    for (int i = 1; i < bones.Length; i++)
//    {

//        string[] temp = array[i].Split(' ');
//        float alpha = 0f, beta = 0f, gamma = 0f, vectorLength = 0f;  // alpha beta gamma are angle to x y z coordinate
//        float.TryParse(temp[0], out alpha);
//        float.TryParse(temp[1], out beta);
//        float.TryParse(temp[2], out gamma);
//        float.TryParse(temp[3], out vectorLength);


//        float vx = vectorLength * Mathf.Cos(alpha);
//        float vy = vectorLength * Mathf.Cos(beta);
//        float vz = vectorLength * Mathf.Cos(gamma);
//        // print(vx+ " "  + vy + " " + vz);
//        // print(new Vector3(vx, vy, vz));
//        bones[i].transform.position = new Vector3(vx, vy, vz) + bones[0].transform.position;
//        //  print(new Vector3(vx, vy, vz));
//    }



//}
