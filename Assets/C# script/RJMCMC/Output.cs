
using UnityEngine;
using System.Collections.Generic;

public class Output : MonoBehaviour {

    IniFile outputFile;
    public string datapath = "/MCMC_Preset/";
    protected static Output instance = null;
    
    private void Awake()
    {

        // set the singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Multiple output script!!");
            Destroy(this);
            return;
        }

        
        
    }
    public static Output Instance
    {
        get
        {
            return instance;
        }
    }

    
    public void InitializeInifile()
    {
        outputFile = new IniFile();

    }
    public void FinishOutput(string filename)
    {
        print("Save to  " + Application.dataPath + datapath + filename + ".ini");
        outputFile.SaveTo(Application.dataPath + datapath + filename + ".ini");
    }
    public bool LoadFile(string filename)
    {

        return outputFile.Load_File(Application.dataPath + datapath + filename + ".ini");

    }
    public void CreateSection(string sectionName)
    {
        outputFile.Create_Section(sectionName);

    }

    public void PrintEntireFloatArray(float[] value, string name)
    {
        outputFile.Set_FloatArray(name, value);

    }
    public void printFloatArray(float[] value)
    {
        float sum = 0f;
        for (int i = 0; i < value.Length; i++)
        {
            outputFile.Set_Float(i.ToString(), value[i]);
            sum += value[i];
        }
        outputFile.Set_Float("Total", sum);


    }
    public void printFloat (float value)
    {
            outputFile.Set_Float(0.ToString(), value);
               
    }
    public void printString(string name, string value)
    {
        outputFile.Set_String(name, value);

    }

    public void printAllWeight(float distWeight, float rotWeight, float CMWeight)
    {
        outputFile.Create_Section("Weights");
        outputFile.Set_Float("disWeight", distWeight);
        outputFile.Set_Float("rotWeight", rotWeight);
        outputFile.Set_Float("cm weight", CMWeight);
    }


  

 

    /*  RJMCMC Result output*/
    
    public void printCostBtwChunks(int preChunk, int curChunk, float cost, string sectionName)
    {
        outputFile.Goto_Section(sectionName);
        outputFile.Set_Float(preChunk.ToString()+ " to " + curChunk.ToString(), cost);

    }
    public void printCurrentLevelCost(float adj, float dur, float cadj, float cdur, float wadj, float wdur, 
                                           float rot, float cm,  float wrot, float wcm)
    {
        outputFile.Set_Float("adj cost without subtract target", adj);
        outputFile.Set_Float("dur time without subtract target", dur);
        ////outputFile.Set_String("  ", "   ");
       // outputFile.Set_String("   ", "   ");
        outputFile.Set_Float("adj cost", cadj);
        outputFile.Set_Float("dur time cost", cdur);
        //outputFile.Set_Float("dist cost before multiply by weight", dist);
        outputFile.Set_Float("rot cost before multiply by weight", rot);
        outputFile.Set_Float("cm cost before multiply by weight", cm);
       // outputFile.Set_String("       ", "   ");
        //outputFile.Set_String("          ", "   ");
        outputFile.Set_Float("adj cost * weight", wadj);
        outputFile.Set_Float("dur time cost * weight", wdur);
        //outputFile.Set_Float("dist cost * weight", wdist);
        outputFile.Set_Float("rot cost * weight", wrot);
        outputFile.Set_Float("wcm cost * weight", wcm);
    }

    public void printLevelTargets(float adj,float dur)
    {
        outputFile.Set_Float("target adj cost ", adj);
        outputFile.Set_Float("target dur time", dur);
        
    }
    public void printLevelWeight(float adj, float dur,  float rot, float cm)
    {
        outputFile.Set_Float("weight for adj cost ", adj);
        outputFile.Set_Float("weight for dur time", dur);
        //outputFile.Set_Float("weight for dist cost ", dist);
        outputFile.Set_Float("weight for rot cost ", rot);
        outputFile.Set_Float("weight for cm cost ", cm);
    }
    public void FinalLevelBestCost(float best)
    {
        outputFile.Set_Float("best cost ", best);
    }
    public void printcurLevelInex(List<int> index)
    {
        outputFile.Set_IntArray("index", index.ToArray());
  
     
    }
    public void printcurLevelSize(int size)
    {
        outputFile.Set_Int("levelSize", size);
    }
}
