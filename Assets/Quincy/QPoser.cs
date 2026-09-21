using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//play 即输出

public class QPoser : MonoBehaviour {


    public Transform[] joints_trans;

    // 镜像序列
    public Transform left_upper,right_upper, left_hip,right_hip;
    public Transform[] mid_joints;

    //// Use this for initialization
    void Start()
    {
        Output_File();
    }

    //// Update is called once per frame
    //void Update () {
    //}

    //输出文件
    void Output_File()
    {
        string path = Application.dataPath + "/Quincy/data/";
        string buffPos = "";
        string buffRot = "";
        
        for (int i = 0; i < joints_trans.Length; i++)
        {
            if (i != 0)
            {
                buffPos += "|";
                buffRot += "|";
            }
            Vector3 pos = joints_trans[i].position;
            Vector3 rot = joints_trans[i].localRotation.eulerAngles;
            buffPos += pos.x + " " + pos.y + " " + pos.z;
            buffRot += rot.x + " " + rot.y + " " + rot.z;
        }

        StreamWriter possr = new StreamWriter(path + "keypose"+this.name + ".txt");
        possr.WriteLine(buffPos);
        possr.Close();

        StreamWriter rotsr = new StreamWriter(path + "keypose_local_rot" + this.name + ".txt");
        rotsr.WriteLine(buffRot);
        rotsr.Close();

        print("output to:" + path);
    }


    public void Do_Mirror()
    {
        //// 反转中轴
        //for (int i = 0; i < mid_joints.Length; i++)
        //{
        //    Vector3 rot = mid_joints[i].localRotation.eulerAngles;
        //    rot.z *= -1;
        //    mid_joints[i].localRotation = Quaternion.Euler(rot);
        //}

        //// 反转左右
        //Transform left = left_upper, right = right_upper;
        //while (left != null || right !=null)
        //{
        //    Vector3 posl = left.localPosition;
        //    Vector3 rotl = left.localRotation.eulerAngles;
        //    posl.x *= -1;
        //    rotl.y *= -1;
        //    rotl.z *= -1;


        //    Vector3 posr = right.localPosition;
        //    Vector3 rotr = right.localRotation.eulerAngles;
        //    posr.x *= -1;
        //    rotr.y *= -1;
        //    rotr.z *= -1;

        //    //left.position = posr;
        //    left.localRotation = Quaternion.Euler(rotr);

        //    //right.position = posl;
        //    right.localRotation = Quaternion.Euler(rotl);

        //    if (left.childCount == 1)
        //        left = left.GetChild(0);
        //    else
        //        left = null;

        //    if (right.childCount == 1)
        //        right = right.GetChild(0);
        //    else
        //        right = null;
        //}

        //left = left_hip;  right = right_hip;
        //while (left != null || right != null)
        //{

        //    Vector3 posl = left.localPosition;
        //    Vector3 rotl = left.localRotation.eulerAngles;
        //    posl.x *= -1;
        //    rotl.y *= -1;
        //    rotl.z *= -1;


        //    Vector3 posr = right.localPosition;
        //    Vector3 rotr = right.localRotation.eulerAngles;
        //    posr.x *= -1;
        //    rotr.y *= -1;
        //    rotr.z *= -1;

        //    //left.position = posr;
        //    left.localRotation = Quaternion.Euler(rotr);

        //    //right.position = posl;
        //    right.localRotation = Quaternion.Euler(rotl);

        //    if (left.childCount == 1)
        //        left = left.GetChild(0);
        //    else
        //        left = null;

        //    if (right.childCount == 1)
        //        right = right.GetChild(0);
        //    else
        //        right = null;
        //}

    }
}
