using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultLevelSetting : MonoBehaviour
{

    public IniFile DefaultLevelfile;

    public string presetDataPath = "/MCMC_Preset/";
    public string TablesDataPath = "/MCMC_Preset/";
    //public IniFile DistDataFile;
    public IniFile RotDataFile;
    public IniFile CMDataFile;
    
    [Header("User Input")]
    //public int nDistJoint = 25;
    public int nRotJoint = 25;
    //public string DistTableName = "Qdistance";
    public string RotTableName = "Qrotation";
    public string cmTableName = "Qcenter";


    [Header("Default Level Info")]
    public string defaultLevelFileName = "Default";
    public int[] defaultLevelIndex;
    public int defaultLevelsize = 0;
   // public float[] jointDistSum;
   // public float jointDistTotalSum = 0;
    public float[] jointRotSum;
    public float rotSum = 0;
    public float jointCMSum;
    [Header("Importance Joint")]
    public float[] ImportantJointRot;
    //public float[] ImportantJointDist;
    [Header("OutputToTarget")]
    IniFile outputFile;
    public string OutputTargetFileName = "target";
    

    //hard code default level file location
    int ReadDefaultLevelSize(string filename)
    {
        DefaultLevelfile = new IniFile();
        //Output error if cannot open the file
        if (!DefaultLevelfile.Load_File(Application.dataPath + presetDataPath + filename + ".ini"))
        {
            Debug.LogError("Fail to open file!! Path is " + Application.dataPath + presetDataPath + filename + ".ini");
        }


        DefaultLevelfile.Goto_Section("Level Size");
        int size = DefaultLevelfile.Get_Int("LevelSize");

        return size;

    }
    int[] ReadDefaultLevelArray(string filename, int levelsize)
    {
        int[] array = new int[levelsize];

        // file.Load_File(Application.dataPath + "/MCMC_Result/" + filename + ".ini");
        DefaultLevelfile.Goto_Section("Level Index");
        array = DefaultLevelfile.Get_IntArray("index", array);
        return array;
    }

    //Load preset distance table, rot table, cm table
    void LoadPresetsTable()
    {

        RotDataFile = new IniFile();
        CMDataFile = new IniFile();
       
         RotDataFile.Load_File(Application.dataPath + TablesDataPath + RotTableName+".ini");
         CMDataFile.Load_File(Application.dataPath + TablesDataPath + cmTableName+ ".ini");

         Debug.Log("Finish Loading preset info and targets");
    }

    public void LoadDefaultLevel()
    {
        //Get Default LevelSize and level index
        defaultLevelsize = ReadDefaultLevelSize(defaultLevelFileName);
        defaultLevelIndex = ReadDefaultLevelArray(defaultLevelFileName, defaultLevelsize);

        
        ImportantJointRot = new float[nRotJoint];

        for (int i = 0; i < nRotJoint; i++)
        {
            ImportantJointRot[i] = 1f;
            //ImportantJointDist[i] = 1f;
        }
       
        DefaultLevelfile.Goto_Section("ImportanceRotJoint");
        ImportantJointRot = DefaultLevelfile.Get_FloatArray("ImportanceRotJoint", ImportantJointRot);


    }
    public void CalculateDefaultLevel()
    {
        LoadPresetsTable();  //Load preset distance table, rot table, cm table


        defaultLevelsize = defaultLevelIndex.Length;
        //Calculate target sum
       
        jointRotSum = CalculateEachJointRotation(defaultLevelIndex, defaultLevelsize);

        jointCMSum = CalculateCM(defaultLevelIndex, defaultLevelsize);

       
        rotSum = 0f;
        //calculate total dist total rot
        for (int i = 0; i < nRotJoint; i++)
        {
          //  jointDistTotalSum += jointDistSum[i];
            rotSum += jointRotSum[i];
        }
    }

    //float[] CalculateEachJointDistance(int[] level, int levelsize)
    //{
    //    //array to store sum of dist for each joint
    //    float[] jointsumdist = new float[nRotJoint];
    //    //calculate sum of dist for each joint in a level
    //    for (int i = 0; i < levelsize - 1; i++)
    //    {
    //        int j = i + 1;
    //        int prechunk = level[i];
    //        int curentchunk = level[j];

    //        DistDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
    //        for (int k = 0; k < nDistJoint; k++)
    //        {
    //            jointsumdist[k] += DistDataFile.Get_Float(k.ToString());
    //        }
    //    }

    //    return jointsumdist;
    //}

    float[] CalculateEachJointRotation(int[] level, int levelsize)
    {
        //array to store sum of rotation for each joint
        float[] jointrotsum = new float[nRotJoint];
        //calculate sum of roation for each joint in a level
        for (int i = 0; i < levelsize - 1; i++)
        {
            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //get rotation between these two chunks
            RotDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            for (int k = 0; k < nRotJoint; k++)
            {
                jointrotsum[k] += RotDataFile.Get_Float(k.ToString());
            }
        }
        return jointrotsum;
    }

    float CalculateCM(int[] level, int levelsize)
    {
        float sumCM = 0f;
        for (int i = 0; i < levelsize - 1; i++)
        {
            int j = i + 1;
            //get previous chunk and current chunk
            int prechunk = level[i];
            int curentchunk = level[j];
            //Load Center of mass shifting between these two chunks
            CMDataFile.Goto_Section(prechunk.ToString() + curentchunk.ToString());
            sumCM += CMDataFile.Get_Float("Center Shift");
        }
        return sumCM;
    }


    //Output purpose
    public void OutputToTargetFile()
    {
        outputFile = new IniFile();
        //outputFile.Create_Section("Distance");
        //for (int i = 0; i < jointDistSum.Length; i++)
        //{
        //    outputFile.Set_Float(i.ToString(), jointDistSum[i]);
        //}

        outputFile.Create_Section("Rotation");
        for (int i = 0; i < jointRotSum.Length; i++)
        {
            outputFile.Set_Float(i.ToString(), jointRotSum[i]);
        }
        outputFile.Create_Section("SumRotation");
        outputFile.Set_Float("SumRotation", rotSum);

        outputFile.Create_Section("CM");
        outputFile.Set_Float("Center Shift", jointCMSum);

        //outputFile.Create_Section("ImportanceDistJoint");
        //outputFile.Set_FloatArray("ImportanceDistJoint", ImportantJointDist);

        outputFile.Create_Section("ImportanceRotJoint");
        outputFile.Set_FloatArray("ImportanceRotJoint", ImportantJointRot);

        outputFile.SaveTo(Application.dataPath + presetDataPath + OutputTargetFileName + ".ini");
        print("Finish print Default to targe file");
    }
    public void UpdateDefaultFile()
    {
        outputFile = new IniFile();

        outputFile.Create_Section("Level Index");
        outputFile.Set_IntArray("index", defaultLevelIndex);
        outputFile.Create_Section("Level Size");
        outputFile.Set_Int("LevelSize", defaultLevelIndex.Length);
        

        // save default
        //outputFile.Create_Section("Distance");
        //for (int i = 0; i < jointDistSum.Length; i++)
        //{
        //    outputFile.Set_Float(i.ToString(), jointDistSum[i]);
        //}

        outputFile.Create_Section("Rotation");
        for (int i = 0; i < jointRotSum.Length; i++)
        {
            outputFile.Set_Float(i.ToString(), jointRotSum[i]);
        }
        outputFile.Create_Section("SumRotation");
        outputFile.Set_Float("SumRotation", rotSum);

        outputFile.Create_Section("CM");
        outputFile.Set_Float("Center Shift", jointCMSum);
        //outputFile.Create_Section("ImportanceDistJoint");
        //outputFile.Set_FloatArray("ImportanceDistJoint",ImportantJointDist);

        outputFile.Create_Section("ImportanceRotJoint");
        outputFile.Set_FloatArray("ImportanceRotJoint", ImportantJointRot);

        outputFile.SaveTo(Application.dataPath + presetDataPath + defaultLevelFileName + ".ini");
        print("Finish update Default file");
    }
}


    
