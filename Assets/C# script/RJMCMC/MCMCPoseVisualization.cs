using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Used to displace three levels*/
public class MCMCPoseVisualization : MonoBehaviour {

    public GameObject[] prefabs;
    List<GameObject> currentDefaultLevelObject;
    List<GameObject> currentEasyLevelObject;
    List<GameObject> currentMediumLevelObject;
    List<GameObject> currentHardLevelObject;

    public string defaultLevel = "Default";
    public string defaultfilepath = "/MCMC_Preset/";
    public int  defaultLevelsize = 0;
    [Header("MCMC Levels")]
    public string levelfilepath = "/MCMC_Result/";
    public string easyLevel = "easy";
    public string mediumLevel = "medium";
    public string hardLevel = "hard";
    public int numPerRow = 10;

    public Transform parent;


    IniFile defaultfile;
    IniFile easyfile;
    IniFile medianfile;
    IniFile hardfile;

    public int ReadLevelSize(string filename, string filepath, ref IniFile file)
    {
    
        if (!file.Load_File(Application.dataPath + filepath + filename + ".ini")) {
            Debug.LogError("Fail to open file!! Path is "+ Application.dataPath + filepath + filename + ".ini");
        }
        // file.Goto_Section("Level Size");
        // int size = file.Get_Int("LevelSize");

        //return size;
        return 0;
    }
    public int[] ReadLevelArray(string filename, int levelsize, ref IniFile file)
    {
        int[] array = new int[levelsize];
        file.Goto_Section("Level Index");
        array = file.Get_IntArray("index", array);
        return array;
    }
    void InitializeList()
    {
        if(currentDefaultLevelObject == null) currentDefaultLevelObject = new List<GameObject>();
        if (currentEasyLevelObject == null) currentEasyLevelObject = new List<GameObject>();
        if (currentMediumLevelObject == null) currentMediumLevelObject = new List<GameObject>();
        if (currentHardLevelObject == null) currentHardLevelObject = new List<GameObject>();
    }

    public void DisplayDefaultLevel()
    {
        if (prefabs.Length <= 0)
        {
            Debug.LogError("Did not assign prefab");
            return;
        }
        defaultfile = null;
        defaultfile = new IniFile();
        InitializeList();
        Vector3 pos = new Vector3(0, 9.2f, 0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        defaultLevelsize = ReadLevelSize(defaultLevel,defaultfilepath, ref defaultfile);

        int[] levelindex = ReadLevelArray(defaultLevel, defaultLevelsize, ref defaultfile);
        DisplayLevelHelper(levelindex,ref currentDefaultLevelObject,pos, rot );

        Debug.Log("Finish displaying");
    }

    public void DisplayEasyLevel()
    {
        if(prefabs.Length <= 0)
        {
            Debug.LogError("Did not assign prefab");
            return;
        }

        InitializeList();
        easyfile = new IniFile();

        Vector3 pos = new Vector3(0, 4.6f, 0);
        Quaternion  rot = new Quaternion(0,0,0,0);
        int levelsize= ReadLevelSize(easyLevel,levelfilepath, ref easyfile);

        int[] levelindex=  ReadLevelArray(easyLevel, levelsize, ref easyfile);
        DisplayLevelHelper(levelindex, ref currentEasyLevelObject, pos, rot);
        //for (int i = 0; i < levelindex.Length; i++)
        //{
          
        //  currentEasyLevelObject.Add(Instantiate(prefabs[levelindex[i]], pos, rot, parent));
        //    pos.x += 1.6f;
        //}

      
        Debug.Log("Finish displaying");
    }

    public void DisplayMediumLevel()
    {
        if (prefabs.Length <= 0)
        {
            Debug.LogError("Did not assign prefab");
            return;
        }
        medianfile = new IniFile();
        InitializeList();
        Vector3 pos = new Vector3(0,2.32f,0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        int levelsize = ReadLevelSize(mediumLevel, levelfilepath, ref medianfile);
        int[] levelindex = ReadLevelArray(mediumLevel, levelsize, ref medianfile);
        //for (int i = 0; i < levelindex.Length; i++)
        //{

        //    currentMediumLevelObject.Add(Instantiate(prefabs[levelindex[i]], pos, rot, parent));
        //    pos.x += 1.6f;
        //}
        DisplayLevelHelper(levelindex, ref currentMediumLevelObject, pos, rot);
        Debug.Log("Finish displaying");
    }
    public void DisplayHardLevel()
    {
        if (prefabs.Length <= 0)
        {
            Debug.LogError("Did not assign prefab");
            return;
        }
        hardfile = new IniFile();
        InitializeList();
        Vector3 pos = new Vector3(0, 0, 0);
        Quaternion rot = new Quaternion(0, 0, 0, 0);
        int levelsize = ReadLevelSize(hardLevel, levelfilepath,ref  hardfile);
        int[] levelindex = ReadLevelArray(hardLevel, levelsize, ref hardfile);
        //for (int i = 0; i < levelindex.Length; i++)
        //{

        //    currentHardLevelObject.Add(Instantiate(prefabs[levelindex[i]], pos, rot, parent));
        //    pos.x += 1.6f;
        //}
        DisplayLevelHelper(levelindex, ref currentHardLevelObject, pos, rot);

        Debug.Log("Finish displaying");
    }

    public void ClearDisplayObject()
    {
        //if (currentDefaultLevelObject.Count > 0)
        //{
        //    for (int i = 0; i < currentDefaultLevelObject.Count; i++)
        //    {
        //        DestroyImmediate(currentDefaultLevelObject[i]);
        //    }
        //}
        
        //if (currentEasyLevelObject.Count > 0)
        //{
        //    for (int i = 0; i < currentEasyLevelObject.Count; i++)
        //    {
        //        DestroyImmediate(currentEasyLevelObject[i]);
        //    }
        //}

        //if(currentMediumLevelObject.Count > 0)
        //{
        //    for (int i = 0; i < currentMediumLevelObject.Count; i++)
        //    {
        //        DestroyImmediate(currentMediumLevelObject[i]);
        //    }

        //}
        //if (currentHardLevelObject.Count > 0)
        //{
        //    for (int i = 0; i < currentHardLevelObject.Count; i++)
        //    {
        //        DestroyImmediate(currentHardLevelObject[i]);
        //    }
        //}
           
       
        currentEasyLevelObject.Clear();
      
       //It has to delete from the last one
        for (int j = parent.childCount-1; j >-1; j--)
        {
            DestroyImmediate(parent.GetChild(j).gameObject);
        }
            

            
       
        Debug.Log("Finish clearing");
    }

    // helper method that  display an level in a specific way
    void DisplayLevelHelper(int[] index, ref List<GameObject> currentlevelObject,Vector3 pos, Quaternion rot)
    {
        for (int i = 0; i < index.Length; i++)
        {   //each display row has at most numPerRow gameobject
            if (i % numPerRow == 0) { //print(i);
                pos.y -= 2.2f; pos.x -= (2.2f * numPerRow);
            }
            currentlevelObject.Add(Instantiate(prefabs[index[i]], pos, rot, parent));
            pos.x += 2.2f;
        }

    }
}
