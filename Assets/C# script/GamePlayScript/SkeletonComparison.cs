using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// compare two skeleton and give a scores
public class SkeletonComparison : MonoBehaviour {
    
    
    public GameObject[] modelSkeletons;
    public AvatarCreationV2[] modelSkeletonScripts;
    public GameObject user;
    public int totalJoint = 25;
    
    
    AvatarCreationV2 modelSkeletonScript;
    SkeletonManKinectController userScript;

    public UserStudyRecorderReader RecorderScript;
    public ScoringMusic musicScript;
    List<MotionFrame> frames;
    
    [Header("Scoring part")]
    public float maxPercDiff = 30f;
    public float currentScore = 0f;
    public float totalScore = 0f;
    public Text UItext; //to display score
    public Text debugText;
    IniFile importantJointFile;
    IniFile modelsJointAngleInfoFile;
    public float[] scoringJoint;
    public class MotionFrame{
        int totalJoint = 25;
        public List<Vector3> rotInfo;

        public MotionFrame()
        {
            rotInfo = new List<Vector3>(totalJoint);
        }
    }

    private void Start()
    {
        if(modelSkeletons == null)
        {
            Debug.LogError("Did not assign model avatars!!");
        
        }
        musicScript = ScoringMusic.Instance;

        modelSkeletonScripts = new AvatarCreationV2[modelSkeletons.Length];
        for (int i = 0; i < modelSkeletons.Length; i++)
        {
            modelSkeletonScripts[i] = modelSkeletons[i].GetComponent<AvatarCreationV2>();
        }

        userScript = user.GetComponent<SkeletonManKinectController>();
      
        /////
        frames = new List<MotionFrame>();
        //load affected joint to score
        LoadImportantJointFile();
        //load models infor
        LoadModelsJointDataFile();


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

void LoadModelsJointDataFile()
    {
        modelsJointAngleInfoFile = new IniFile();
        if (!modelsJointAngleInfoFile.Load_File(Application.dataPath + "/BiaoLevel/UserResult/keyposeAngle.ini"))
        {
            Debug.LogError("cannot open inifile path " + Application.dataPath + "/BiaoLevel/UserResult/keyposeAngle.ini");

        }
       
    }
    public float LoadModelInfo(int chunk, ref int joint )
    {
        float jointAngle = 0f;
       
        if (!modelsJointAngleInfoFile.Goto_Section(chunk.ToString())) {

            Debug.LogError("cannot go to section");
        };
            jointAngle = modelsJointAngleInfoFile.Get_Float(joint.ToString());
        return jointAngle;
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
        //modelSkeletonScript = modelSkeletonScripts[chunk];
        //LoadImportantJoints(chunk);
        
        float score = 0f;
        //float anglediff = 0f;
        
        //bool ispass = false;
        //for (int i = 0; i < frames.Count; i++)
        //{
        //    ispass = ComparingRotation(ref modelSkeletonScript.bones, frames[i].rotInfo.ToArray(), ref anglediff);

        //    if (ispass) //if all the joint are within maxangledifferent
        //    {
        //       // RecorderScript.RecordKeyFrame(i);
        //        break;
        //    }

        //}

        //if (ispass)
        //{
        //    if (anglediff < 15f)
        //    {
        //        score = 1;
        //        musicScript.playGoodAudio();
        //    }
        //    else if (anglediff >= 15f)
        //    {
        //        musicScript.playGreatAudio();
        //        score = 0.6f;
        //    }
        //}
        //else //if is not match
        //{
        //    musicScript.playOhNoAudio();
        //    score = 0f;
        //}
     
        ////currentScore = score * 100f;
        ////totalScore += currentScore;

        ////if (UItext != null)
        ////{
        ////    UItext.text =   " Current Score " + currentScore + "\n total Score " + totalScore;
        ////}
        return score;
    }

    
    
    //Instance compare model and user

    public float InstanceCompare(int chunk, ref GameObject[] user, ref float anglediff, ref float[] angleDiffPerc)
    {
       
        
        float score = 0f;
        bool ispass = false;
      
        ispass = InstanceCompareHelper(ref chunk, ref user,  ref anglediff, ref angleDiffPerc );

        if (ispass){

            if (anglediff < 0.1f)
            {
                score = 1;
                musicScript.playGoodAudio();
            }
            else if (anglediff >= 0.1f)
            {
                musicScript.playGreatAudio();
                score = 0.6f;
            }
        }
        else //if is not match
        {
           // musicScript.playOhNoAudio();
            score = 0f;
        }

       
        return score;

    }

    //
    public bool InstanceCompareHelper(ref int chunk, ref GameObject[] user, ref float sumPercDiff, ref float[] angleDiffPerc)
    {
        bool ispass = true;
        LoadImportantJoints(chunk);

        debugText.text = " ";
        float angleDiff = 0f;
        //int tolerance= 0; //define how many joint angle diff outside 
        int counter = 0;

        for (int i = 0; i < totalJoint; i++)
        {
           
            if (scoringJoint[i] > 0f)
            {
                float modelAngle = LoadModelInfo(chunk, ref i);

                float userAngle = RotationCostHelperforGameObjType(i, user);
               
                //if they are all positive
                if (modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiff = Mathf.Abs(modelAngle - userAngle);
                }
                else if (modelAngle < 0f && userAngle < 0f)
                {
                    angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else //one positive one negative
                {
                    //float mAngle = Mathf.Abs(modelAngle);
                    //float uAngle = Mathf.Abs(userAngle);
                    //if (mAngle >= 90 && uAngle >= 90)
                    //{
                    //    angleDiff = 360 - mAngle - uAngle;
                    //}
                    //else if (mAngle < 90 && uAngle < 90)
                    //{
                    //    angleDiff = mAngle + uAngle;

                    //}
                    //else //one of them is > 90 one of them is < 90
                    //{
                    //    angleDiff = 180 - mAngle - uAngle;
                    //}
                    float mAngle = Mathf.Abs(modelAngle);
                    float uAngle = Mathf.Abs(userAngle);
                    float r1 = 360 - mAngle - uAngle;
                    float r2 = mAngle + uAngle;
                    angleDiff = Mathf.Min(r1, r2);

                }
                angleDiffPerc[i] = angleDiff / 180f;
                
                //compare to maxPercDiff; return value from 0~1
               

                
                //if current joint is not match with model 
                if ((angleDiff/180f) * scoringJoint[i] > maxPercDiff ) //if angle one of important joint are > than the setting, return false
                {
                    debugText.text += "joint " + i + "  model angle" +  Mathf.RoundToInt( modelAngle )+ "                user Angle " 
                                  + Mathf.RoundToInt(userAngle);
                    debugText.text += "   angle diff " + Mathf.RoundToInt(angleDiff) + "\n";
                    angleDiffPerc[i] = (angleDiffPerc[i] * scoringJoint[i]) / maxPercDiff;
                    ispass = false;
                }
                else
                {
                    angleDiffPerc[i] = 0.1f; //set to zero only for color the joint
                }
                angleDiff /= 180f;
                sumPercDiff += (angleDiff);
                
                counter++;

            }

        }

        sumPercDiff = sumPercDiff / (1.0f * counter); //update perc diff
        debugText.text += "      total diff " + sumPercDiff + "\n";
        // print("avg sum diff" + sumPercDiff);
      
  

        return ispass;
    }



    //Biao version of angle calculation
    public static float RotationCostHelperforGameObjType(int curJoint, GameObject[] model)
    {
        //Transform tar = transform, own = transform;
        Vector3 fromJoint = Vector3.zero, toJoint = Vector3.zero;

        switch (curJoint)
        {
            case 0: // spinebase with hips
                {
                    fromJoint = model[0].transform.position - model[1].transform.position;
                    return -Vector3.SignedAngle(fromJoint, Vector3.down, Vector3.forward);
                }
            case 1: // spine mid with spinebase and neck
                    fromJoint = model[8].transform.position - model[4].transform.position;
                     toJoint = model[12].transform.position - model[16].transform.position;
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

    //maxdiff: max different angle torelent difference btw user and model
    //public bool InstanceComparsion(ref GameObject[] model, ref GameObject[] v_user, 
    //                      ref float sumPercDiff)
    //{
    //    float angleDiff = 0f;
    //     sumPercDiff = 0f;
    //    int counter = 0;

    //    for (int i = 0; i < totalJoint; i++)
    //    {
    //        //print("joint " + i);
    //        if (scoringJoint[i] == 1f)
    //        {
    //            float modelAngle = RotationCostHelperforModel(i, model);

    //            float userAngle = RotationCostHelperforModel(i, v_user);

    //            //if they are all positive
    //            if (modelAngle >= 0f && userAngle >= 0f)
    //            {
    //                angleDiff = Mathf.Abs(modelAngle - userAngle);
    //            }
    //            else if (modelAngle < 0f && userAngle < 0f)
    //            {
    //                angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
    //            }
    //            else
    //            {
    //                angleDiff = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
    //            }

    //            if(angleDiff > maxPercDiff)
    //            {
    //                return false;
    //            }

    //            sumPercDiff += angleDiff;
    //            counter++;

    //        }

    //    }
    //    sumPercDiff = sumPercDiff / (1.0f * counter);

    //    return true;

    //}
    //return an array of difference
    public void InstanceCompareArray(int chunk, ref GameObject[]  v_user, ref float[] angleDiffPerc)
    {
        LoadImportantJoints(chunk);


        for (int i = 0; i < totalJoint; i++)
        {

            if (scoringJoint[i] > 0f)
            {
                float modelAngle = LoadModelInfo(chunk, ref i);

                float userAngle = RotationCostHelperforGameObjType(i, v_user);

                // compare user pose rotation to model rotation
                //initialPoseRot is initial rotation, angle is current rotation

                //if they are all positive
                if (modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiffPerc[i] = Mathf.Abs(modelAngle - userAngle);
                }
                else if (modelAngle < 0f && userAngle < 0f)
                {
                    angleDiffPerc[i] = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else //one pos one neg
                {
                    float mAngle = Mathf.Abs(modelAngle);
                    float uAngle = Mathf.Abs(userAngle);
                    float r1 = 360 - mAngle - uAngle;
                    float r2 = mAngle + uAngle;
                    angleDiffPerc[i] = Mathf.Min(r1, r2);

                }
            }
            angleDiffPerc[i] /= 180f; 
            //compare to maxPercDiff; return value from 0~1
            angleDiffPerc[i] =  (angleDiffPerc[i]*scoringJoint[i]) / maxPercDiff ;
        }
      
      
    }

    //float RotationCostHelperforUser(int curJoint, Vector3[] user)
    //{
    //    //Transform tar = transform, own = transform;
    //    Vector3 fromJoint = Vector3.zero, toJoint = Vector3.zero;

    //    switch (curJoint)
    //    {
    //        case 0: // spinebase with hips
    //            {
    //                fromJoint = user[0] - user[1];
    //                return Vector3.SignedAngle(fromJoint, Vector3.down, Vector3.forward);
    //            }
    //        case 1: // spine mid with spinebase and neck
    //            return Vector3.Angle(user[0] - user[1], user[20] - user[1]);
    //        //fromJoint = user[8] - user[4];
    //        // toJoint = user[12] - user[16];
    //        // return Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);

    //        case 4: // left should with shoulder and elbow
    //            {

    //                fromJoint = user[5] - user[4];
    //                toJoint = user[1] - user[20];

    //                break;
    //            }
    //        case 5: // left elbow with left shoulder and wrist
    //            {
    //                return Vector3.Angle(user[4] - user[5], user[6] - user[5]);
    //                //return Vector3.Angle(joints[8].position - joints[9].position, joints[10].position - joints[9].position);
    //            }
    //        case 8: // right shoulder with shoulder and elbow
    //            {

    //                fromJoint = user[9] - user[8];
    //                toJoint = user[1] - user[20];

    //                break;
    //            }
    //        case 9: // right elbow with right shoulder and arm
    //            {
    //                return Vector3.Angle(user[8] - user[9], user[10] - user[9]);

    //                //swap
    //                //return Vector3.Angle(joints[4].position - joints[5].position, joints[6].position - joints[5].position);

    //            }
    //        case 12: // left hip with base and knee
    //            return Vector3.Angle(user[0] - user[12], user[13] - user[12]);
    //        case 13: // left knwee
    //            {
    //                return Vector3.Angle(user[12] - user[13], user[14] - user[13]);
    //            }
    //        case 16: // left hip with base and knee
    //            return Vector3.Angle(user[0] - user[16], user[17] - user[16]);
    //        case 17: // left knwee
    //            {
    //                return Vector3.Angle(user[16] - user[17], user[18] - user[17]);
    //            }
    //    }


    //    if (fromJoint == Vector3.zero)
    //        return 0f;


    //    float angle = Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);

    //    return -angle;



    //}
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

   
    bool ComparingRotation(ref GameObject[] model, Vector3[] v_user, ref float sumdiff)
    {
        // float angleDiff = 0f;
        // sumdiff = 0f;
        // int counter = 0;

        // for (int i = 0; i < totalJoint; i++)
        // {
        //     if (scoringJoint[i] > 0f)
        //     {
        //         float modelAngle = RotationCostHelperforModel(i, model);

        //         float userAngle = RotationCostHelperforUser(i, v_user);

        //         // compare user pose rotation to model rotation
        //         //initialPoseRot is initial rotation, angle is current rotation

        //         //if they are all positive
        //         if(modelAngle >= 0f && userAngle >= 0f)
        //         {
        //             angleDiff = Mathf.Abs(modelAngle - userAngle);
        //         }
        //        else if(modelAngle < 0f && userAngle < 0f)
        //         {
        //             angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
        //         }
        //         else
        //         {
        //             angleDiff = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
        //         }
        //         if(angleDiff > maxPercDiff*scoringJoint[i])
        //         {
        //             return false;
        //         }
        //         sumdiff += angleDiff;
        //         counter++;
        //         //if (UItext != null)
        //         //{
        //         //    UItext.text += (KinectInterop.JointType)i + "angle diff " + angleDiff
        //         //                   + "   model angle " + modelAngle
        //         //                       + "  userangle  " + userAngle + " \n";

        //         //}

        //     }

        // }
        // // UItext.text += "sum perc diff " + sumPercDiff;
        // // sumPercDiff = (counter == 0) ? 0 : sumPercDiff / (1.0f * counter);
        // sumdiff = sumdiff / (1.0f * counter);
        //// UItext.text += "sum perc diff avg " + sumPercDiff + " counter " + counter;

        return true;
    }

    //old version
    //float RotationCostHelperforModel(int curJoint, GameObject[] bones)
    //{

    //    int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
    //    int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);

    //    Vector3 root = bones[0].transform.position;
    //    //local position for current joint parent jont and child joint
    //    Vector3 curPos = bones[curJoint].transform.position - root;
    //    Vector3 parentPos = bones[curParentJoint].transform.position - root;
    //    Vector3 childPos = bones[curChildJoint].transform.position - root;

    //    /*calculate current joint angle*/
    //    Vector3 v1 = (parentPos - curPos).normalized;
    //    Vector3 v2 = (childPos - curPos).normalized;
    //    Vector3 vn = Vector3.forward;

    //    if ( curJoint == 0) //if is spine
    //    {
    //        curPos = bones[curJoint].transform.position - root;
    //        parentPos = bones[1].transform.position - root;
    //        childPos = Vector3.down;
    //        v1 = (parentPos - curPos).normalized;
    //        v2 = (childPos - curPos).normalized;
    //        vn = Vector3.forward;
    //    }

    //    Vector3 cross = Vector3.Cross(v1, v2);
    //    float angle = Vector3.Angle(v1, v2);
    //    if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

    //    return angle;
    //}

    //float RotationCostHelperforUser(int curJoint, Vector3[] v_bones)
    //{

    //    int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
    //    int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);

    //    Vector3 root = v_bones[0];
    //    //local position for current joint parent jont and child joint
    //    Vector3 curPos = v_bones[curJoint] - root;
    //    Vector3 parentPos = v_bones[curParentJoint] - root;
    //    Vector3 childPos = v_bones[curChildJoint] - root;

    //    /*calculate current joint angle*/
    //    Vector3 v1 = (parentPos - curPos).normalized;
    //    Vector3 v2 = (childPos - curPos).normalized;
    //    //Vector3 vn = Vector3.forward;

    //    Vector3 vn = Vector3.forward;
    //    if (curJoint == 0) //if is spine
    //    {
    //        curPos = v_bones[curJoint] - root;
    //        parentPos = v_bones[1] - root;
    //        childPos = Vector3.down;
    //        v1 = (parentPos - curPos).normalized;
    //        v2 = (childPos - curPos).normalized;
    //        vn = Vector3.back;

    //    }

    //    Vector3 cross = Vector3.Cross(v1, v2);
    //    float angle = Vector3.Angle(v1, v2);
    //    if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

    //    return angle;
    //}

  



}

