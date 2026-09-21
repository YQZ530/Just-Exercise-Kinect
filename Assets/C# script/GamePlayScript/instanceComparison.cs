using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class instanceComparison : MonoBehaviour {

    public GameObject[] models;
    public AvatarCreationV2[] modelScripts;
    //public SkeletonManKinectController userScript;
    public AvatarCreationV2 userScript;
    public int chunk = 3;
    public Text modeltext; //to display score
    public Text usertext;
    // Use this for initialization
    void Start () {
        modelScripts = new AvatarCreationV2[models.Length];
        for (int i = 0; i < models.Length; i++)
        {
            modelScripts[i] = models[i].GetComponent<AvatarCreationV2>();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3[] v3 = new Vector3[25];
        for (int i = 0; i < 25; i++)
        {
            v3[i] = userScript.bones[i].transform.position;
        }

        modeltext.text =
            " " + (KinectInterop.JointType)0 + RotationCostHelperforModel(0, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)1 + RotationCostHelperforModel(1, modelScripts[chunk].bones)
             + " \n" + (KinectInterop.JointType)20 + RotationCostHelperforModel(10, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)4 + RotationCostHelperforModel(4, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)5 + RotationCostHelperforModel(5, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)8 + RotationCostHelperforModel(8, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)9 + RotationCostHelperforModel(9, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)12 + RotationCostHelperforModel(12, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)13 + RotationCostHelperforModel(13, modelScripts[chunk].bones)
             + " \n" + (KinectInterop.JointType)16 + RotationCostHelperforModel(16, modelScripts[chunk].bones)
            + " \n" + (KinectInterop.JointType)17 + RotationCostHelperforModel(17, modelScripts[chunk].bones);

        usertext.text = " " + (KinectInterop.JointType)0 + RotationCostHelperforUser(0, v3)
            + " \n" + (KinectInterop.JointType)1 + RotationCostHelperforUser(1, v3)
             + " \n" + (KinectInterop.JointType)20 + RotationCostHelperforUser(10, v3)
            + " \n" + (KinectInterop.JointType)4 + RotationCostHelperforUser(4, v3)
            + " \n" + (KinectInterop.JointType)5 + RotationCostHelperforUser(5, v3)
            + " \n" + (KinectInterop.JointType)8 + RotationCostHelperforUser(8, v3)
            + " \n" + (KinectInterop.JointType)9 + RotationCostHelperforUser(9, v3)
            + " \n" + (KinectInterop.JointType)12 + RotationCostHelperforUser(12, v3)
            + " \n" + (KinectInterop.JointType)13 + RotationCostHelperforUser(13, v3)
             + " \n" + (KinectInterop.JointType)16 + RotationCostHelperforUser(16, v3)
            + " \n" + (KinectInterop.JointType)17 + RotationCostHelperforUser(17, v3);


    }

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
      
       if (curJoint == 0) //if is spine
       {
            curPos = bones[curJoint].transform.position-root;
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
            curPos = v_bones[curJoint]-root;
            parentPos = v_bones[1]- root;
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
                return KinectInterop.JointType.Neck; // I add
            case KinectInterop.JointType.SpineMid:
                return KinectInterop.JointType.SpineShoulder; // I add   

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
            //case KinectInterop.JointType.SpineBase:
            //    return KinectInterop.JointType.KneeLeft;
            //case KinectInterop.JointType.SpineShoulder: //I add
            //    return KinectInterop.JointType.SpineMid; 
            //case KinectInterop.JointType.SpineMid://I add
            //    return KinectInterop.JointType.SpineBase;

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
