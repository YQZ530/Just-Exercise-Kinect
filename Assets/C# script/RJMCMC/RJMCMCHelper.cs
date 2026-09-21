using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//each joint has a target dist, rotation that want to complete in a level
public class RJMCMCHelper : MonoBehaviour
{

    public int nDistJoint = 10;
    public int nRotJoint = 10;
    //public int nCMJoint = 10;
    public string presetDataPath = "/MCMC_Preset/";

    [Header("Target Setting")]
    //public float[] targetDist;
    public float[] targetRot;
    public float targetSumRot;
    public float targetCM;

    [Header("Importance Joint")]
    public float[] ImportantJointRot;
    //public float[] ImportantJointDist;
    public int numImportantJointRot =0;
   // public int numImportantJointDist = 0;

    [Header("Weight")]
    //public float distWeight = 0.5f;
    public float rotWeight = 0.5f;
    public float cmWeight = 1f;
    [Header("Filepath")]
    public string targetfile = "target";
   // public string DistTableName = "Qdistance";
    public string RotTableName = "Qrotation";
    public string cmTableName = "Qcenter";
    public IniFile targetAndWeightFile;
   //public IniFile DistDataFile;
   public IniFile RotDataFile;
   public IniFile CMDataFile;

    protected static RJMCMCHelper instance = null;
   

    public static RJMCMCHelper Instance
    {
        get
        {
            return instance;
        }
    }

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

    private void Start()
    {
        
        //test case  
        //DistDataFile.Goto_Section("12");
        //for (int k = 0; k < 3; k++)
        //{
        //    print(DistDataFile.Get_Float(k.ToString()));
        //}
    }

   

    void LoadPresetAndTargetfiles()
    {
        //DistDataFile = new IniFile();
        RotDataFile = new IniFile();
        CMDataFile = new IniFile();
       
        //if(!DistDataFile.Load_File(Application.dataPath+ presetDataPath + DistTableName+".ini")) Debug.Log("Unable to load distance.ini " + Application.dataPath + presetDataPath + DistTableName + ".ini"); 
        if(!RotDataFile.Load_File(Application.dataPath + presetDataPath + RotTableName+".ini")) Debug.Log("Unable to load rotation.ini");
        if (!CMDataFile.Load_File(Application.dataPath + presetDataPath + cmTableName + ".ini")) Debug.Log("Unable to load cm.ini");
       // else { print("Loaded " + Application.dataPath + presetDataPath + cmTableName + ".ini"); }
        //simpleTest();
        LoadTargetsFile();
       // Debug.Log("Finish Loading preset info and targets");
        //CalculateSumRotation();
        
        //FindMaxDistTransition();
    }
    public void LoadTargetsFile()
    {
        targetAndWeightFile = new IniFile();
        if(!targetAndWeightFile.Load_File(Application.dataPath + presetDataPath + targetfile + ".ini")) Debug.Log("Unable to load target and weight.ini"); ;
        //Load target dist , rot and cm
        //targetDist = new float[nDistJoint];
        targetRot = new float[nDistJoint];

        for (int i = 0; i < nDistJoint; i++)
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
        //ImportantJointDist = new float[nDistJoint];
        ImportantJointRot = new float[nRotJoint];
        for (int i = 0; i < nDistJoint; i++)
        {
            ImportantJointRot[i] = 1f;
            //ImportantJointDist[i] = 1f;
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
        for (int i = 0; i < nDistJoint; i++)
        {
            //if(ImportantJointDist[i] != 0f)
            //{
            //    numImportantJointDist++;
                
            //}
            if(ImportantJointRot[i] != 0f)
            {
                numImportantJointRot++;
            }
        }
    }

   

    //public float CalculateDistCost(List<int> level)
    //{
    //    //array to store sum of dist for each joint
    //    float[] jointsumdist = new float[nDistJoint];
    //    //calculate sum of dist for each joint in a level
    //    for (int i = 0; i < level.Count-1; i++)
    //    { 
    //        int j = i + 1;
    //        int prechunk = level[i];
    //        int curentchunk = level[j];

    //        DistDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
    //        for (int k = 0; k < nDistJoint; k++)
    //        {
    //            jointsumdist[k] += DistDataFile.Get_Float(k.ToString());
    //           // print("chunk ij " + prechunk+" " + curentchunk + " "+"current"+ k + " " + jointsumdist[k]);
    //        }
    //    }

    //    //calculate dist cost
    //    float final = 0f;
    //    int counterAffectJoint = 0; //count num joint that are use in calculation
    //    for (int k = 0; k < nDistJoint; k++)
    //    {
    //       if(targetDist[k] == 0f) //if target are zero: could because of pose ii or important = 0
    //        { 
    //            final += ( 1* ImportantJointDist[k] );
    //            if(ImportantJointDist[k] != 0f)
    //            {
    //                counterAffectJoint++;
    //            }
                
    //        }
    //        else
    //        {
    //            float r = jointsumdist[k] - targetDist[k];
    //            float r2 = -1 * r * r / (2 * targetDist[k] * targetDist[k]);
    //             float r3 = (1 - Mathf.Exp(r2));
    //            final += ImportantJointDist[k] * r3;
    //            counterAffectJoint++;
    //        }
          
    //    }
    //   // print(final);
    //    final /= counterAffectJoint;
    //  //  print("final dist cost"+final );
    //    return final;
    //}

    
    public float CalculateRotCost(List<int> level)
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
                finalrot += (1*ImportantJointRot [k]) ;
                if (ImportantJointRot[k] != 0f)
                {
                    counterAffectJoint++;
                }


            } 
            else
            {
                float r = sumrot[k] - targetRot[k];  //sumrot[k] is total rot of level for joint k
                float r2 = -1 * r * r / ( 0.5f* targetRot[k] * targetRot[k]);
                float r3 = (1 - Mathf.Exp(r2));
                finalrot += ImportantJointRot[k] * r3;
                counterAffectJoint++;
               // print("target rot k is not zero" + k);
            }
            
        }
        //if(counterAffectJoint != 14) { print("counter joint is  "+ counterAffectJoint); }
        finalrot /= counterAffectJoint;

        if(counterAffectJoint != numImportantJointRot) { Debug.LogError("joint is not 25!"); }
       // print("finalrot" + finalrot+"  total join in cal "+counterAffectJoint);

        return finalrot;
    }
    public float CalculateRotCostV2(List<int> level)
    {
        //array to store sum of rotation of a level
        float sumrot = 0f;
       
        //calculate sum of roation for each joint in a level
        for (int i = 0; i < level.Count - 1; i++)
        {

            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //get rotation between these two chunks and then add up to sumrot
            RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            
            for (int k = 0; k < nRotJoint; k++)
            {

                sumrot += RotDataFile.Get_Float("total");
              
            }
        }

        //calculate cost
        float finalcost = 0f;
                float r = sumrot - targetSumRot;  //sumrot[k] is total rot of level for joint k
                float r2 = -1 * r * r / (0.5f * targetSumRot * targetSumRot);
            if (targetSumRot == 0f) Debug.LogError("Stop; zero division");
             finalcost = (1 - Mathf.Exp(r2));
       // print("sumrot " + sumrot + "target rot " + targetSumRot + " r2 " + r2+ "  final cost " + finalcost);
        return finalcost;
    }
    public float CalculateCMCost(List<int> level)
    {
       
        float sumCM = 0f ;
        for (int i = 0; i < level.Count-1; i++)
        {
            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //Load Center of mass shifting between these two chunks
            CMDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            sumCM += CMDataFile.Get_Float("Center Shift");
        }
        float r1 = (-1) * (sumCM - targetCM) * (sumCM - targetCM) ;
        float r2 = 0.5f * targetCM * targetCM;
        if (r2 == 0f) Debug.LogError("Stop");
        float final = 1 - Mathf.Exp(r1 / r2);

        return final;
    }

    
    void FindMaxDistTransition()
    {
        string maxtranName = "00";
        float maxSum = 0f;
        //calculate sum of dist for each joint in a level
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                int prechunk = i;
                int curentchunk = j;

                RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
                float sum = 0f;
                for (int k = 0; k < nDistJoint; k++)
                {
                    sum += RotDataFile.Get_Float(k.ToString());
                    if(maxSum < sum) {
                        maxSum = sum;
                        maxtranName = prechunk.ToString() + curentchunk.ToString();
                    
                    }

                }
            }
            
        }
       // print(maxtranName);
    }

    void simpleTest()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                RotDataFile.Goto_Section(i.ToString() + j.ToString());
                for (int k = 0; k < 25; k++)
                {
                    if (RotDataFile.Get_Float(k.ToString()) == 0f)
                    {
                        print(k);

                    }
                }
            }

        }
    }
    //void CalculateSumRotation()
    //{
    //    int totalchunk = 10;
    //    for (int i = 0; i < totalchunk; i++)
    //    {
    //        for (int j = 0; j < totalchunk; j++)
    //        {
    //            RotDataFile.Goto_Section(i.ToString() + j.ToString());
    //            float sumrot = 0f;
    //            for (int k = 0; k < 25; k++)
    //            {
    //                sumrot += RotDataFile.Get_Float(k.ToString());
    //                if (k == 24)
    //                {
    //                    RotDataFile.Set_Float("total", sumrot);
    //                }
    //            }
    //        }
    //    }
    //    RotDataFile.Save();
    //}
    //void CreateSimpleTestFile()
    //{
    //    IniFile test = new IniFile();
    //    for (int i = 0; i < 3; i++)
    //    {
    //        for (int j = 0; j < 3; j++)
    //        {
    //            test.Create_Section(i.ToString() + j.ToString());
    //            for (int k = 0; k < 3; k++)
    //            {
    //                test.Set_Float(k.ToString(), Random.Range(0f, 1f));
    //            }
    //        }


    //    }

    //    test.SaveTo(Application.dataPath + "/dist.ini");
    //}
}
