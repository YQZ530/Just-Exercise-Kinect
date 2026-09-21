using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class CalculateAngleDiffFromFile : MonoBehaviour {
    [SerializeField]
    public class Frame
    {
        public List<float> Diff;

        public Frame(int totaljoint)
        {

            Diff = new List<float>(totaljoint);

        }
    }

    IniFile inputFile;
    public string presetDataPath;
    List<Vector3> prePos;
    List<Vector3> afterPos;
    

    List<Frame> outputFileData;
    // Use this for initialization
    void Start () {
        prePos = new List<Vector3>(25);
        afterPos = new List<Vector3>(25);
        outputFileData = new List<Frame>();


        inputFile = new IniFile();
        //Debug.Log(Application.dataPath + presetDataPath + "us033demo.ini");
        if (!inputFile.Load_File(Application.dataPath + presetDataPath + "us033demo.ini")) Debug.Log("Unable to load" + Application.dataPath + presetDataPath + "us033demo.ini");
        //CalculatePosDiff();
        CalculateAngleDiff();
    }

    public  void CalculateAngleDiff()
    {
        int totalframe = 0;
        inputFile.Goto_Section("TotalFrame");
        totalframe = inputFile.Get_Int("TotalFrame");

        for (int i = 5558; i < 5714; i++)
        {

            for (int j = 0; j < 25; j++)
            {
                inputFile.Goto_Section((i - 1).ToString());
                prePos.Add(inputFile.Get_Vector3("p" + j.ToString(), Vector3.zero));
                inputFile.Goto_Section((i).ToString());
                afterPos.Add(inputFile.Get_Vector3("p" + j.ToString(), Vector3.zero));
            }

            Frame tempFrame = new Frame(25);
            //It has to read all joint position data first before doing any angle calcualtion
            for (int j = 0; j < 25; j++)
            {
                float curangle = RotationCostHelperforVector3Type(j, afterPos);
                float preangle = RotationCostHelperforVector3Type(j, prePos);
                tempFrame.Diff.Add(Mathf.Abs(curangle - preangle));

            }

            outputFileData.Add(tempFrame);
            prePos.Clear();
            afterPos.Clear();
        }


            StreamWriter writer = new StreamWriter(Application.dataPath + presetDataPath + "AngleResult.txt");
            for (int i = 0; i < outputFileData.Count; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (j != 24)
                    {
                        writer.Write(outputFileData[i].Diff[j] + ",");
                    }
                    else
                    {
                        writer.Write(outputFileData[i].Diff[j]);
                    }
                }
                writer.WriteLine();

            }
        
    }

    public void CalculatePosDiff()
    {
        int totalframe = 0;
        inputFile.Goto_Section("TotalFrame");
        totalframe = inputFile.Get_Int("TotalFrame");

        for (int i = 954; i < totalframe; i++)
        {
            inputFile.Goto_Section((i - 1).ToString());
            for (int j = 0; j < 25; j++)
            {

                prePos.Add(inputFile.Get_Vector3("p" + j.ToString(), Vector3.zero));

            }

            Frame tempFrame = new Frame(25);
            inputFile.Goto_Section((i).ToString());
            for (int j = 0; j < 25; j++)
            {

                afterPos.Add(inputFile.Get_Vector3("p" + j.ToString(), Vector3.zero));
                tempFrame.Diff.Add(Vector3.Distance(afterPos[j], prePos[j]));


            }
            outputFileData.Add(tempFrame);
            prePos.Clear();
            afterPos.Clear();



        }

        StreamWriter writer = new StreamWriter(Application.dataPath + presetDataPath + "result.txt");
        for (int i = 0; i < outputFileData.Count; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (j != 24)
                {
                    writer.Write(outputFileData[i].Diff[j] + ",");
                }
                else
                {
                    writer.Write(outputFileData[i].Diff[j]);
                }
            }
            writer.WriteLine();

        }

    }
    //Biao version of angle calculation
    public static float RotationCostHelperforVector3Type(int curJoint, List<Vector3> model)
    {
        //Transform tar = transform, own = transform;
        Vector3 fromJoint = Vector3.zero, toJoint = Vector3.zero;

        switch (curJoint)
        {
            case 0: // spinebase with hips
                {
                    fromJoint = model[0] - model[1];
                    return -Vector3.SignedAngle(fromJoint, Vector3.down, Vector3.forward);
                }
            case 1: // spine mid with spinebase and neck
                fromJoint = model[8] - model[4];
                toJoint = model[12] - model[16];
                return Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);


            //return Vector3.Angle(model[0] - model[1], model[20] - model[1]);
            case 4: // left should with shoulder and elbow
                {
                    fromJoint = model[5] - model[4];
                    toJoint = model[20] - model[4];

                    break;
                }
            case 5: // left elbow with left shoulder and wrist
                {
                    return Vector3.Angle(model[4] - model[5], model[6] - model[5]);

                }
            case 8: // right shoulder with shoulder and elbow
                {

                    fromJoint = model[9] - model[8];
                    toJoint = model[20] - model[8];

                    break;
                }
            case 9: // right elbow with right shoulder and arm
                {
                    return Vector3.Angle(model[8] - model[9], model[10] - model[9]);


                }
            case 12: // left hip with base and knee
                     //16-12 13-12
                return Vector3.Angle(model[0] - model[12], model[13] - model[12]);
            case 13: // left knwee
                {
                    return Vector3.Angle(model[12] - model[13], model[14] - model[13]);
                }
            case 16: // left hip with base and knee
                // 12 -16 ,
                return Vector3.Angle(model[0] - model[16], model[17] - model[16]);
            case 17: // left knwee
                {
                    return Vector3.Angle(model[16] - model[17], model[18] - model[17]);
                }
        }



        if (fromJoint == Vector3.zero)
            return 0f;


        float angle = Vector3.SignedAngle(fromJoint, toJoint, Vector3.forward);

        return -angle;



    }

}
