using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringJoint : MonoBehaviour {

    public string outputfilepath = "/GamePlay/";
    public string outputFileName = "AffectJointsForScoring";
    [Header("User Input")]
    public float[] chunk0;
    public float[] chunk1; //affected joint for scoring in chunk 1
    public float[] chunk2;
    public float[] chunk3;
    public float[] chunk4;
    public float[] chunk5;
    public float[] chunk6; //affected joint for scoring in chunk 6
    public float[] chunk7;
    public float[] chunk8;
    public float[] chunk9;

    

    public void ReadJoint()
    {
         IniFile Scorefile = new IniFile();
        if (!Scorefile.Load_File(Application.dataPath + outputfilepath + outputFileName + ".ini")) {
            print("Cannot open  " + Application.dataPath + outputfilepath + outputFileName + ".ini");
        };
        Scorefile.Goto_Section("0");
        chunk1 = Scorefile.Get_FloatArray("score", chunk0);
        Scorefile.Goto_Section("1");
        chunk1 = Scorefile.Get_FloatArray("score", chunk1);

        Scorefile.Goto_Section("2");
        chunk2 = Scorefile.Get_FloatArray("score", chunk2);

        Scorefile.Goto_Section("3");
        chunk3 = Scorefile.Get_FloatArray("score", chunk3);

        Scorefile.Goto_Section("4");
        chunk4= Scorefile.Get_FloatArray("score", chunk4);

        Scorefile.Goto_Section("5");
        chunk5 = Scorefile.Get_FloatArray("score", chunk5);

       

        Scorefile.Goto_Section("6");
        Scorefile.Get_FloatArray("score", chunk6);

        Scorefile.Goto_Section("7");
        Scorefile.Get_FloatArray("score", chunk7);

        Scorefile.Goto_Section("8");
        Scorefile.Get_FloatArray("score", chunk8);

        Scorefile.Goto_Section("9");
        Scorefile.Get_FloatArray("score", chunk9);

    }


    public void Outputfile()
    {
        IniFile outputFile = new IniFile();
        outputFile.Create_Section("0", "Chunk0");
        outputFile.Set_FloatArray("score", chunk0);

        outputFile.Create_Section("1", "Chunk1");
        outputFile.Set_FloatArray("score", chunk1);

        outputFile.Create_Section("2");
        outputFile.Set_FloatArray("score", chunk2);

        outputFile.Create_Section("3");
        outputFile.Set_FloatArray("score", chunk3);

        outputFile.Create_Section("4");
        outputFile.Set_FloatArray("score", chunk4);

        outputFile.Create_Section("5");
        outputFile.Set_FloatArray("score", chunk5);

        //SetmirrorMotionJoint();

        outputFile.Create_Section("6", "Chunk6");
        outputFile.Set_FloatArray("score", chunk6);

        outputFile.Create_Section("7");
        outputFile.Set_FloatArray("score", chunk7);

        outputFile.Create_Section("8");
        outputFile.Set_FloatArray("score", chunk8);

        outputFile.Create_Section("9");
        outputFile.Set_FloatArray("score", chunk9);

        
        outputFile.SaveTo(Application.dataPath + outputfilepath + outputFileName + ".ini");
    }
    void SetmirrorMotionJoint()
    {
        for (int i = 0; i < chunk1.Length; i++)
        {
            if(chunk1[i] == 1f)
            {
                int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
                chunk9[otherJoint] = 1;
            }

            if (chunk2[i] == 1f)
            {
                int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
                chunk8[otherJoint] = 1;
            }
            if (chunk3[i] == 1f)
            {
                int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
                chunk7[otherJoint] = 1;
            }
            if (chunk4[i] == 1f)
            {
                int otherJoint = (int)GetLeftRightJoint((KinectInterop.JointType)i);
                chunk6[otherJoint] = 1;
            }

        }

    }

    //return if this joint has left/right side, otherwise return itself; 
    //but we ignore those unnecssary joint we dont want to count
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


