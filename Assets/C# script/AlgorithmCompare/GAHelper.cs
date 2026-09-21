using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class GAHelper : MonoBehaviour
{
    public int PrefabSize = 10;
  
    //public int LevelSize;
    
    public float CurrentAdjVarCost = 0f;
    public float currentLevelRotCost = 0f;
    public float CurrentDurCost = 0f;
    public float currentLevelCMCost = 0f;
    public float CurrentDur;

    public int nRotJoint = 10;
    public string presetDataPath = "/MCMC_Preset/";

    [Header("Target Setting")]
    public float targetDuration;
    public float targetAdjVar;
    public float[] targetRot;
    public float targetSumRot;
    public float targetCM;
    

    [Header("Importance Joint")]
    public float[] ImportantJointRot;
    public int numImportantJointRot = 0;
  
    [Header("Weight")]
   
    public float rotWeight = 1f;
    public float cmWeight = 1f;
    public float adVarWeight = 0.5f;
    public  float DurationWeight = 1f;

    [Header("Filepath")]
    public string targetfile = "target";
    // public string DistTableName = "Qdistance";
    public string RotTableName = "Qrotation";
    public string cmTableName = "Qcenter";
    public IniFile targetAndWeightFile;
    //public IniFile DistDataFile;
    public IniFile RotDataFile;
    public IniFile CMDataFile;

    protected static GAHelper instance = null;


    public static GAHelper Instance
    {
        get
        {
            return instance;
        }
    }
    public   int[] level;
    IniFile rotTable;
    IniFile cmTable;
    public string tableFilePath = "/MCMC_Preset/";
 

    private void Awake()
    {

        // set the singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Multiple optimization helper script!!");
            Destroy(this);
            return;
        }

        LoadPresetAndTargetfiles();
    }
    void LoadPresetAndTargetfiles()
    {
        //DistDataFile = new IniFile();
        RotDataFile = new IniFile();
        CMDataFile = new IniFile();

        //if(!DistDataFile.Load_File(Application.dataPath+ presetDataPath + DistTableName+".ini")) Debug.Log("Unable to load distance.ini " + Application.dataPath + presetDataPath + DistTableName + ".ini"); 
        if (!RotDataFile.Load_File(Application.dataPath + presetDataPath + RotTableName + ".ini")) Debug.Log("Unable to load rotation.ini");
        if (!CMDataFile.Load_File(Application.dataPath + presetDataPath + cmTableName + ".ini")) Debug.Log("Unable to load cm.ini");
        else { print("Loaded " + Application.dataPath + presetDataPath + cmTableName + ".ini"); }
        //simpleTest();
        LoadTargetsFile();
        Debug.Log("Finish Loading preset info and targets");
        //CalculateSumRotation();

        //FindMaxDistTransition();
    }
    public void LoadTargetsFile()
    {
        targetAndWeightFile = new IniFile();
        if (!targetAndWeightFile.Load_File(Application.dataPath + presetDataPath + targetfile + ".ini")) Debug.Log("Unable to load target and weight.ini"); ;
        //Load target dist , rot and cm
        //targetDist = new float[nDistJoint];
        targetRot = new float[nRotJoint];

        for (int i = 0; i < nRotJoint; i++)
        {
            //targetAndWeightFile.Goto_Section("Distance");
            //targetDist[i] = targetAndWeightFile.Get_Float(i.ToString());

            targetAndWeightFile.Goto_Section("Rotation");
            targetRot[i] = targetAndWeightFile.Get_Float(i.ToString());

        }
        targetAndWeightFile.Goto_Section("SumRotation");
        targetSumRot = targetAndWeightFile.Get_Float("SumRotation");

        targetAndWeightFile.Goto_Section("CM");
        targetCM = targetAndWeightFile.Get_Float("Center Shift");
        //Load Weight
        targetAndWeightFile.Goto_Section("Weights");
        //distWeight = targetAndWeightFile.Get_Float("dist");
        rotWeight = targetAndWeightFile.Get_Float("rot");
        cmWeight = targetAndWeightFile.Get_Float("cm");
        //Load joint importance

        //for output purpose only;
        //initialize array
      
        ImportantJointRot = new float[nRotJoint];
        for (int i = 0; i < nRotJoint; i++)
        {
            ImportantJointRot[i] = 1f;
          
        }
        //get importance joint value
        // if (!targetAndWeightFile.Goto_Section("ImportanceDistJoint")) { Debug.LogError("cannot go to this section"); }
        //ImportantJointDist = targetAndWeightFile.Get_FloatArray("ImportanceDistJoint", ImportantJointDist);
        targetAndWeightFile.Goto_Section("ImportanceRotJoint");
        ImportantJointRot = targetAndWeightFile.Get_FloatArray("ImportanceRotJoint", ImportantJointRot);
        countNumImportantJoint();
    }
    void countNumImportantJoint()
    {
        numImportantJointRot = 0;
        for (int i = 0; i < nRotJoint; i++)
        {
            //if(ImportantJointDist[i] != 0f)
            //{
            //    numImportantJointDist++;

            //}
            if (ImportantJointRot[i] != 0f)
            {
                numImportantJointRot++;
            }
        }
    }


    //public void SetScalar(float varScalar, float durationScalar, float calScalar, float meanScalar, float intensVarScalar){
    //    VarWeight = 0.1f; DurationWeight = 0.5f; RotatioinScalar = 1f; COMScalar = 0.1f; IntensVarScalar = 0.1f;

    
    public float CalculateCost( ref List<int> currentLevel)
    {
        float cost = 0f;
       
        CurrentDurCost = CalculateChunkDurationCost(ref currentLevel);
        cost += CurrentDurCost * DurationWeight;
       
        CurrentAdjVarCost = CalculateAdVariationCost(ref currentLevel);
        cost += CurrentAdjVarCost * adVarWeight;

      
        currentLevelRotCost = CalculateRotCost(ref currentLevel);
        cost += currentLevelRotCost * rotWeight;

        currentLevelCMCost = CalculateCMCost(ref currentLevel);
        cost += currentLevelCMCost * cmWeight;
        return cost;
    }
    public float CalculateRotCost(ref List<int> level)
    {
        //array to store sum of rotation for each joint
        float[] sumrot = new float[nRotJoint];

        for (int i = 0; i < nRotJoint; i++)
        {
            sumrot[i] = 0f;
        }
        //calculate sum of roation for each joint in a level
        for (int i = 0; i < level.Count - 1; i++)
        {

            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //get rotation between these two chunks
            RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            //print("chunk " + );
            for (int k = 0; k < nRotJoint; k++)
            {

                sumrot[k] += RotDataFile.Get_Float(k.ToString());
                // print("chunk"+prechunk.ToString() + curentchunk.ToString() + "joint "+ k +" " +sumrot[k]);
            }
        }

        //calculate cost
        float finalrot = 0f;
        int counterAffectJoint = 0;
        for (int k = 0; k < nRotJoint; k++)
        {
            if (targetRot[k] == 0f)
            { //avoid zero division, so make exp =0; final = 1-0 =1
                finalrot += (1 * ImportantJointRot[k]);
                if (ImportantJointRot[k] != 0f)
                {
                    counterAffectJoint++;
                }


            }
            else
            {
                float r = sumrot[k] - targetRot[k];  //sumrot[k] is total rot of level for joint k
                float r2 = -1 * r * r / (0.5f * targetRot[k] * targetRot[k]);
                float r3 = (1 - Mathf.Exp(r2));
                finalrot += ImportantJointRot[k] * r3;
                counterAffectJoint++;
                // print("target rot k is not zero" + k);
            }

        }
        //if(counterAffectJoint != 14) { print("counter joint is  "+ counterAffectJoint); }
        finalrot /= counterAffectJoint;

        if (counterAffectJoint != numImportantJointRot) { Debug.LogError("joint is not 25!"); }
        // print("finalrot" + finalrot+"  total join in cal "+counterAffectJoint);

        return finalrot;
    }
    
    public float CalculateCMCost(ref List<int> level)
    {

        float sumCM = 0f;
        for (int i = 0; i < level.Count - 1; i++)
        {
            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //Load Center of mass shifting between these two chunks
            CMDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            sumCM += CMDataFile.Get_Float("Center Shift");
        }
        float r1 = (-1) * (sumCM - targetCM) * (sumCM - targetCM);
        float r2 = 0.5f * targetCM * targetCM;
        if (r2 == 0f) Debug.LogError("Stop");
        float final = 1 - Mathf.Exp(r1 / r2);

        return final;
    }

    private float CalculateAdVariationCost(ref List<int> currentLevel)
    {
        float c = 0f;
        if (currentLevel.Count <= 1)
        {
            return c;
        }

        for (int i = 0; i < (currentLevel.Count - 1); i++)
        {
            if (currentLevel[i] == currentLevel[i + 1])
            {
                c += 1;
            }
        }

        c = c * (1f / (currentLevel.Count - 1));
        CurrentAdjVarCost = c;
        c = Mathf.Abs(c - targetAdjVar);  //if target Var is 1, it means many same chunks
                                         // print("Adjacent cost " + c);
        return c;
    }

    float CalculateChunkDurationCost(ref List<int> currentLevel)
    {
        float levelduration = 0f; //level duratioin
        float sigma = targetDuration;

        levelduration = (currentLevel.Count - 1) * 1f;
        CurrentDur = levelduration; //update in inspector

        float cost = (levelduration - targetDuration) * (levelduration - targetDuration);
        cost = -1f * cost / (2f * sigma * sigma);
        cost = 1f - Mathf.Exp(cost);
        //  print("totaltime" + c + "  durationC " + answer +"  best  cost   "+bestCost + "iteration" + Iteration);
        return cost;
    }
   

}
