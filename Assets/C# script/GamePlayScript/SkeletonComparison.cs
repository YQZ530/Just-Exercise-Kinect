using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// compare two skeleton and give a scores
public class SkeletonComparison : MonoBehaviour {


    public GameObject[] models;
    public AvatarCreationV2[] modelScripts;
    //public GameObject model;
    public GameObject user;
    public int totalJoint = 25;


    AvatarCreationV2 modelScript;
    SkeletonManKinectController userScript;

    public UserStudyRecorderReader RecorderScript;
    public ScoringMusic musicScript;
    List<MotionFrame> frames;

    [Header("Scoring part")]
    public float maxAngleDiff = 30f;
    public float currentScore = 0f;
    public float totalScore = 0f;
    public Text UItext; //to display score
    public Text UItxt2;
    IniFile importantJointFile;
    public float[] scoringJoint;
    public class MotionFrame {
        int totalJoint = 25;
        public List<Vector3> rotInfo;

        public MotionFrame()
        {
            rotInfo = new List<Vector3>(totalJoint);
        }
    }

    private void Start()
    {
        if (models == null)
        {
            Debug.LogError("Did not assign model avatars!!");

        }
        musicScript = ScoringMusic.Instance;

        modelScripts = new AvatarCreationV2[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            modelScripts[i] = models[i].GetComponent<AvatarCreationV2>();
        }

        userScript = user.GetComponent<SkeletonManKinectController>();

        /////
        frames = new List<MotionFrame>();
        //load affected joint to score
        LoadImportantJointFile();
        if (UItext != null)
        {
            UItext.text = "Current Score " + currentScore + "\n total Score " + totalScore;
        }
    }

    private void FixedUpdate()
    {
        //Vector3[] v3 = new Vector3[totalJoint];
        //for (int i = 0; i < totalJoint; i++)
        //{
        //    v3[i] =userScript.bones[i].transform.position;
        //}

        //UItxt2.text = " " + (KinectInterop.JointType)4 + RotationCostHelperforModel(4, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)5 + RotationCostHelperforModel(5, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)8 + RotationCostHelperforModel(8, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)9 + RotationCostHelperforModel(9, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)12 + RotationCostHelperforModel(12, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)13 + RotationCostHelperforModel(13, modeltemp.bones)
        //     + " \n" + (KinectInterop.JointType)16 + RotationCostHelperforModel(16, modeltemp.bones)
        //    + " \n" + (KinectInterop.JointType)17 + RotationCostHelperforModel(17, modeltemp.bones); 

        //UItxt2.text =        modeltemp.bones[4].transform.position;

        //UItxt2.text = " " + (KinectInterop.JointType)4 + RotationCostHelper(4, v3)
        //    + " \n" + (KinectInterop.JointType)5 + RotationCostHelper(5, v3)
        //    + " \n" + (KinectInterop.JointType)8 + RotationCostHelper(8, v3)
        //    + " \n" + (KinectInterop.JointType)9 + RotationCostHelper(9, v3)
        //    + " \n" + (KinectInterop.JointType)12 + RotationCostHelper(12, v3)
        //    + " \n" + (KinectInterop.JointType)13 + RotationCostHelper(13, v3)
        //     + " \n" + (KinectInterop.JointType)16 + RotationCostHelper(16, v3)
        //    + " \n" + (KinectInterop.JointType)17 + RotationCostHelper(17, v3);
    }
    void LoadImportantJointFile()
    {
        importantJointFile = new IniFile();
        if (!importantJointFile.Load_File(Application.dataPath + "/GamePlay/AffectJointsforScoring.ini"))
        {
            Debug.LogError("cannot open inifile path " + Application.dataPath + "/GamePlay/AffectJointsforScoring.ini");

        }
        //scoringJoint = new float[25];
        //importantJointFile.Goto_Section("ImportanceRotJoint");
        //scoringJoint = importantJointFile.Get_FloatArray("ImportanceRotJoint", scoringJoint);
    }
    public bool LoadImportantJoints(int chunk)
    {
        importantJointFile.Goto_Section(chunk.ToString());
        scoringJoint = importantJointFile.Get_FloatArray("score", scoringJoint);
        return true;
    }
    public void RecordCurrentMotion()
    {

        MotionFrame f = new MotionFrame();
        for (int i = 0; i < totalJoint; i++)
        {
            f.rotInfo.Add(userScript.bones[i].transform.position);
        }
        frames.Add(f);
    }
    public void clearMotionFrame()
    {
        frames.RemoveRange(0, frames.Count);
    }
    public float Compare(int chunk)
    {
        modelScript = modelScripts[chunk];
        LoadImportantJoints(chunk);

        float score = 0f;
        float anglediff = 0f;

        bool ispass = false;
        for (int i = 0; i < frames.Count; i++)
        {
            ispass = ComparingRotation(ref modelScript.bones, frames[i].rotInfo.ToArray(), ref anglediff);

            if (ispass) //if all the joint are within maxangledifferent
            {
                RecorderScript.RecordKeyFrame(i);
                break;
            }

        }

        if (ispass)
        {
            if (anglediff < 15f)
            {
                score = 1;
                musicScript.playGoodAudio();
            }
            else if (anglediff >= 15f)
            {
                musicScript.playGreatAudio();
                score = 0.6f;
            }
        }
        else //if is not match
        {
            musicScript.playOhNoAudio();
            score = 0f;
        }
        //if (angle <15 ) {
        //    score = 1;
        //    musicScript.playGoodAudio();
        //}
        //else if(angle >= 15f && angle <= 20f)
        //{
        //    musicScript.playGreatAudio();
        //    score = 0.6f;
        //}

        //else
        //{
        //    musicScript.playOhNoAudio();
        //    score = 0f;
        //}
        currentScore = score * 100f;
        totalScore += currentScore;

        //if (UItext != null)
        //{
        //    UItext.text = " Current Score " + currentScore + "\n total Score " + totalScore;
        //}
        UpdateScore(currentScore);
        return score;
    }

    public void UpdateScore(float score)
    {

        totalScore += score;
        if (UItext != null)
        {
            UItext.text = " Current Score " + score + "\n total Score " + totalScore;
        }
    }
    bool ComparingRotation(ref GameObject[] model, Vector3[] v_user, ref float sumdiff)
    {
        float angleDiff = 0f;
        sumdiff = 0f;
        int counter = 0;
      
        for (int i = 0; i < totalJoint; i++)
        {
            if (scoringJoint[i] == 1f)
            {
                float modelAngle = RotationCostHelperforModel(i, model);
               
                float userAngle = RotationCostHelperforUser(i, v_user);

                // compare user pose rotation to model rotation
                //initialPoseRot is initial rotation, angle is current rotation

                //if they are all positive
                if(modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiff = Mathf.Abs(modelAngle - userAngle);
                }
               else if(modelAngle < 0f && userAngle < 0f)
                {
                    angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else
                {
                    angleDiff = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
                }
                if(angleDiff > maxAngleDiff)
                {
                    return false;
                }
                sumdiff += angleDiff;
                counter++;
                //if (UItext != null)
                //{
                //    UItext.text += (KinectInterop.JointType)i + "angle diff " + angleDiff
                //                   + "   model angle " + modelAngle
                //                       + "  userangle  " + userAngle + " \n";

                //}

            }

        }
        // UItext.text += "sum perc diff " + sumPercDiff;
        // sumPercDiff = (counter == 0) ? 0 : sumPercDiff / (1.0f * counter);
        sumdiff = sumdiff / (1.0f * counter);
       // UItext.text += "sum perc diff avg " + sumPercDiff + " counter " + counter;

        return true;
    }

    //maxdiff: max different angle torelent difference btw user and model
    public bool InstanceComparsion(ref GameObject[] model, ref GameObject[] v_user, 
                          ref float sumPercDiff)
    {
        float angleDiff = 0f;
         sumPercDiff = 0f;
        int counter = 0;
       
        for (int i = 0; i < totalJoint; i++)
        {
            print("joint " + i);
            if (scoringJoint[i] == 1f)
            {
                float modelAngle = RotationCostHelperforModel(i, model);

                float userAngle = RotationCostHelperforModel(i, v_user);
                
                //if they are all positive
                if (modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiff = Mathf.Abs(modelAngle - userAngle);
                }
                else if (modelAngle < 0f && userAngle < 0f)
                {
                    angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else
                {
                    angleDiff = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
                }

                if(angleDiff > maxAngleDiff)
                {
                    return false;
                }

                sumPercDiff += angleDiff;
                counter++;
               
            }

        }
        sumPercDiff = sumPercDiff / (1.0f * counter);

        return true;

    }
    //return an array of difference
    public float[] ComparingRotationArray(ref GameObject[] model, ref GameObject[]  v_user)
    {
       // float angleDiff = 0f;
        float[] angleDiff = new float[25];

        for (int i = 0; i < totalJoint; i++)
        {

            if (scoringJoint[i] == 1f)
            {
                float modelAngle = RotationCostHelperforModel(i, model);

                float userAngle = RotationCostHelperforModel(i, v_user);

                // compare user pose rotation to model rotation
                //initialPoseRot is initial rotation, angle is current rotation

                //if they are all positive
                if (modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiff[i] = Mathf.Abs(modelAngle - userAngle);
                }
                else if (modelAngle < 0f && userAngle < 0f)
                {
                    angleDiff[i] = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else
                {
                    angleDiff[i] = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
                }
            }

        }
        // UItext.text += "sum perc diff " + sumPercDiff;
        // sumPercDiff = (counter == 0) ? 0 : sumPercDiff / (1.0f * counter);
       
        // UItext.text += "sum perc diff avg " + sumPercDiff + " counter " + counter;

        return angleDiff;
    }
    //float ComparingRotation()
    //{
    //    float angleDiff = 0f;
    //    float sumAngle = 0f;
    //    int counter = 0;
    //    string s = "";
    //    for (int i = 0; i < totalJoint; i++)
    //    {

    //        if(scoringJoint[i] ==1f)
    //        { 
    //            float modelAngle = RotationCostHelper(i, modelScript.bones);


    //            float userAngle = RotationCostHelper(i, userScript.bones);

    //            // compare user pose rotation to model rotation
    //            //initialPoseRot is initial rotation, angle is current rotation
    //            if (modelAngle >= 0 && userAngle >= 0)
    //            {
    //                //if they are all positive
    //                angleDiff = Mathf.Abs(modelAngle - userAngle);
    //            }
    //            else if (modelAngle < 0 && userAngle < 0)
    //            {
    //                angleDiff = Mathf.Abs(modelAngle - userAngle);
    //            }
    //            else
    //            {
    //                //if they are diff sign; get minimum rotation angle
    //                angleDiff = 360 - (Mathf.Abs(modelAngle) + Mathf.Abs(userAngle));
    //            }

    //            //if (angleDiff > 20f ) {
    //            //    //print(modelAngle + " " + userAngle + " " + (KinectInterop.JointType)i + " " + angleDiff);
    //            //    //bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(angleDiff/180f);
    //            //}
    //            //print((KinectInterop.JointType)i + " " + angleDiff);
    //            sumAngle += angleDiff;
    //            counter++;
    //            //if (UItext != null)
    //            //{
    //            //    UItext.text += (KinectInterop.JointType)i + "angle diff " + angleDiff
    //            //                   + "  \n model angle" + modelAngle
    //            //                       + "  userangle  " + userAngle +" \n";

    //            //}
    //            s += (KinectInterop.JointType)i + "angle diff " + angleDiff
    //                              + "  \n model angle" + modelAngle
    //                                   + "  userangle  " + userAngle +" \n";
    //        }

    //    }
    //    print(s);
    //    sumAngle =  (counter ==0) ? 0: sumAngle / (1.0f * counter);


    //    return sumAngle;
    //}

    float RotationCostHelperforModel(int curJoint, GameObject[] bones)
    {
        
        int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
        int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);
       
        Vector3 root = bones[0].transform.position;
        //local position for current joint parent jont and child joint
        Vector3 curPos = bones[curJoint].transform.position - root;
        Vector3 parentPos = bones[curParentJoint].transform.position - root;
        Vector3 childPos = bones[curChildJoint].transform.position - root;

        /*calculate current joint angle*/
        Vector3 v1 = (parentPos - curPos).normalized;
        Vector3 v2 = (childPos - curPos).normalized;
        Vector3 vn = Vector3.forward;
        
        if ( curJoint == 0) //if is spine
        {
            curPos = bones[curJoint].transform.position - root;
            parentPos = bones[1].transform.position - root;
            childPos = Vector3.down;
            v1 = (parentPos - curPos).normalized;
            v2 = (childPos - curPos).normalized;
            vn = Vector3.forward;
        }

        Vector3 cross = Vector3.Cross(v1, v2);
        float angle = Vector3.Angle(v1, v2);
        if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

        return angle;
    }

    float RotationCostHelperforUser(int curJoint, Vector3[] v_bones)
    {

        int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
        int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);
       
        Vector3 root = v_bones[0];
        //local position for current joint parent jont and child joint
        Vector3 curPos = v_bones[curJoint] - root;
        Vector3 parentPos = v_bones[curParentJoint] - root;
        Vector3 childPos = v_bones[curChildJoint] - root;

        /*calculate current joint angle*/
        Vector3 v1 = (parentPos - curPos).normalized;
        Vector3 v2 = (childPos - curPos).normalized;
        //Vector3 vn = Vector3.forward;

        Vector3 vn = Vector3.forward;
        if (curJoint == 0) //if is spine
        {
            curPos = v_bones[curJoint] - root;
            parentPos = v_bones[1] - root;
            childPos = Vector3.down;
            v1 = (parentPos - curPos).normalized;
            v2 = (childPos - curPos).normalized;
            vn = Vector3.back;

        }

        Vector3 cross = Vector3.Cross(v1, v2);
        float angle = Vector3.Angle(v1, v2);
        if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

        return angle;
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
//if (!importantJointFile.Load_File(Application.dataPath + "/GamePlay/ImportantJoint.ini"))
//        {
//            Debug.LogError("cannot open inifile path " + Application.dataPath + "/GamePlay/ImportantJoint.ini");

//        }
//scoringJoint = new float[25];
//        importantJointFile.Goto_Section("ImportanceRotJoint");
//        scoringJoint = importantJointFile.Get_FloatArray("ImportanceRotJoint", scoringJoint);

//void readAFile()
//{
//    CreateBoneList();
//    StreamReader reader = new StreamReader(Application.dataPath + "/" + SkeletonfilePath + "/" + SkeletonfileName + ".txt");

//    Vector3[] skeletonTransform = new Vector3[bones.Length];
//    //add all the position info to a chunk
//    while (!reader.EndOfStream)
//    {
//        string line = reader.ReadLine();

//        if (line == null) { Debug.Log("EOF"); return; }

//        string[] array = line.Split('|');

//        for (int i = 0; i < bones.Length; i++)
//        {

//            string[] temp = array[i].Split(' ');
//            float x = 0f, y = 0f, z = 0f;
//            float.TryParse(temp[0], out x);
//            float.TryParse(temp[1], out y);
//            float.TryParse(temp[2], out z);

//            skeletonTransform[i] = new Vector3(x, y, z);
//        }

//        CalculateBoneLength();
//        Vector3 v = this.transform.position - skeletonTransform[0]; //current position
//        bones[0].transform.position = this.transform.position;

//        bones[1].transform.position = skeletonTransform[1] + v; ;

//        bones[2].transform.position = skeletonTransform[2] + v;

//        bones[20].transform.position = skeletonTransform[20] + v;
//        for (int i = 3; i < bones.Length; i++)
//        {
//            if (i == 20) { continue; }

//            bones[i].transform.position = skeletonTransform[i] + v;
//        }

//    }

//}
////for easy to access each joint
//void CreateBoneList()
//{
//    bones = new GameObject[] {
//        Hip_Center,
//        Spine,
//        Neck,
//        Head,
//        Shoulder_Left,
//        Elbow_Left,
//        Wrist_Left,
//        Hand_Left,
//        Shoulder_Right,
//        Elbow_Right,
//        Wrist_Right,
//        Hand_Right,
//        Hip_Left,
//        Knee_Left,
//        Ankle_Left,
//        Foot_Left,
//        Hip_Right,
//        Knee_Right,
//        Ankle_Right,
//        Foot_Right,
//        Spine_Shoulder,
//        Hand_Tip_Left,
//        Thumb_Left,
//        Hand_Tip_Right,
//        Thumb_Right
//    };
//}

