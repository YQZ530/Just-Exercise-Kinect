using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//In the inspector, press output it save all target information ((distance, rotation, cm)
//and weight infomation to given filename file
//
public class TargetSetting : MonoBehaviour {

    public string filepath = "/MCMC_Preset/";
    public string LoadedFilename = "target";

    public string OutputFilename = "target";

    [Header("Weight")]
    //public float distWeight = 0.5f;
    public float rotWeight = 0.5f;
    public float cmWeight = 1f;

    [Header("Target Setting")]
    //public float[] targetDist;
    public float[] targetRot;
    public float targetSumRot = 0f;
    public float targetCM;
    [Header("ImportanceJoint Setting")]
    public float[] ImportantJointRot;
    [Header("Increase rotation angle and cm")]
    public float percentAngleIncrease = 0.5f;
    public float percentCMIncrase = 0f;
    //public float[] ImportantJointDist;
  
    public void OutputSetting()
    {
        IniFile outputFile = new IniFile();

        //outputFile.Create_Section("ImportanceDistJoint");
        //outputFile.Set_FloatArray("ImportanceDistJoint", ImportantJointDist);

        outputFile.Create_Section("ImportanceRotJoint");
        outputFile.Set_FloatArray("ImportanceRotJoint", ImportantJointRot);

        // save targets
        //float sumDist = 0f;
        //outputFile.Create_Section("Distance");
        //for (int i = 0; i < targetDist.Length; i++)
        //{
          
        //    outputFile.Set_Float(i.ToString(), targetDist[i]);
        //    sumDist += targetDist[i];1
        //}
        //outputFile.Create_Section("SumDistance");
        //outputFile.Set_Float("SumDistance", sumDist);

        float sumRot = 0f;
        float avgIncrased = 0f;
        int counter = 0;
        outputFile.Create_Section("Rotation");
        for (int i = 0; i < targetRot.Length; i++)
        {
            if (ImportantJointRot[i] != 0f)
            {
                counter++;
                float ranIncrease = 0.0f;
                if (percentAngleIncrease <= 0.4f && (percentAngleIncrease > 0f))
                {
                    ranIncrease = Random.Range(0.1f, percentAngleIncrease);
                }
                else if(percentAngleIncrease > 0.4f)
                {
                    ranIncrease = Random.Range(0.4f, percentAngleIncrease);
                }

                avgIncrased += ranIncrease;
                float newangle = ranIncrease * targetRot[i] + targetRot[i];
                outputFile.Set_Float(i.ToString(), newangle);
                sumRot += newangle;
            }
           
        }
        
        avgIncrased /= (1f*counter);
        print("avg increase " + avgIncrased);
        outputFile.Create_Section("AvgIncreasedRate");
        outputFile.Set_Float("AvgIncreasedRate", avgIncrased);
        outputFile.Create_Section("SumRotation");
        outputFile.Set_Float("SumRotation", sumRot);

        outputFile.Create_Section("CM");
        float newcm = targetCM * percentCMIncrase+ targetCM;
        outputFile.Set_Float("Center Shift", newcm);

        

        //save weights
        outputFile.Create_Section("Weights");
        //outputFile.Set_Float("dist", distWeight);
        outputFile.Set_Float("rot", rotWeight);
        outputFile.Set_Float("cm",  cmWeight);
        outputFile.SaveTo(Application.dataPath + filepath + OutputFilename + ".ini");
        print("finish output  data to " + Application.dataPath + filepath + OutputFilename + ".ini");
    }
    public void LoadSetting()
    {
        IniFile inputFile = new IniFile();
        inputFile.Load_File(Application.dataPath + filepath + LoadedFilename + ".ini");
        // load targets
        //inputFile.Goto_Section("Distance");
        //for (int i = 0; i < targetDist.Length; i++)
        //{
        //    targetDist[i] = inputFile.Get_Float(i.ToString());
        //}

        inputFile.Goto_Section("Rotation");
        for (int i = 0; i < targetRot.Length; i++)
        {
            targetRot[i] =inputFile.Get_Float(i.ToString() );
        }

        inputFile.Goto_Section("SumRotation");
        targetSumRot = inputFile.Get_Float("SumRotation");
        inputFile.Goto_Section("CM");
         targetCM =inputFile.Get_Float("Center Shift");

        //load weights
        if (inputFile.Is_Section("Weights"))
        {
            inputFile.Goto_Section("Weights");
            //inputFile.Get_Float("dist", distWeight);
            inputFile.Get_Float("rot", rotWeight);
            inputFile.Get_Float("cm", cmWeight);
        }
        // print(inputFile.Goto_Section("ImportanceDistJoint"));
        //ImportantJointDist = inputFile.Get_FloatArray("ImportanceDistJoint", ImportantJointDist);
        inputFile.Goto_Section("ImportanceRotJoint");
        ImportantJointRot = inputFile.Get_FloatArray("ImportanceRotJoint", ImportantJointRot);

        print("finish loading data on "+ Application.dataPath + filepath + LoadedFilename + ".ini");


    }
    
}
