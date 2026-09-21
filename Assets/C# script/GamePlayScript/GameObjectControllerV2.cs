using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectControllerV2 : MonoBehaviour {

    public enum GameState { empty, start, playing, transition, stop };
    public Texture[] prefabs;
    public GameObject[] gameObjects;
   
    public Transform parent;
    public RawImage rawImage;
    public RawImage waittingImage;
    public float slerpTime;

    public string levelPath = "/MCMC_Result/";
    public string levelName = "default";

    [Header("Observation")]
    //public float distance;
    //public float speed;
    public GameState currentGameState;
    public float currentTime;
    public int[] levelArray;
    //public List<GameObject> currentLevel;
    public int levelCounter = 1;
    public int waitCounter = 2;
    public SkeletonComparison comparisionScript;
    public UserStudyRecorderReader recorderScript;
    
    // Use this for initialization
    void Start()
    {

        if (prefabs == null) { Debug.LogError("No prefab assign!!"); }
        if(waittingImage == null) { Debug.LogError("No rawimage assign!!"); }
       
       
         waittingImage.GetComponent<Animation>().enabled = false;


        currentGameState = GameState.empty;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
            comparisionScript.RecordCurrentMotion();
            print("chunk " + levelArray[levelCounter-1]);
            currentGameState = GameState.playing;
        }
      
        else if (currentGameState == GameState.playing)
        {
            comparisionScript.RecordCurrentMotion();
        }

        //this chunk duration is end
        if (currentTime > slerpTime && currentGameState == GameState.playing)
        {
            
            currentGameState = GameState.transition;
            if(levelCounter != levelArray.Length)
            {
              //  comparisionScript.Compare(levelArray[levelCounter-1]);
            }
           
            comparisionScript.clearMotionFrame();
            UpdateLevelChunk();
            currentTime = 0f;

        }

    }
    public void StartGame()
    {
        print("Game start");
        levelCounter = 1;
        waitCounter = 2;
      
        waittingImage.texture = prefabs[levelArray[waitCounter++]];
        currentGameState = GameState.start;
        currentTime = 0f;
     
        Instantiate(  gameObjects[levelArray[levelCounter++]],parent);
        waittingImage.GetComponent<Animation>().enabled = true;
       
        recorderScript.StartRecording();
    }
    void UpdateLevelChunk()
    {
        //this is for moving object
        if(levelCounter < levelArray.Length)
        {  
            Instantiate(gameObjects[levelArray[levelCounter++]],parent);
        }
        else
        {
         
            currentGameState = GameState.stop;
            recorderScript.StopRecording("test");
        }
        //this is for waiting object
        if (waitCounter < (levelArray.Length ))
        { waittingImage.texture = prefabs[levelArray[waitCounter++]]; }
        else
        {
            waittingImage.GetComponent<Animation>().Stop();
            waittingImage.texture = null;
        }
    }
    public void LevelSelection(int i)
    {
        switch (i)
        {
            case 1:
               
                LoadlevelArray("default");
               
                break;
            case 2:
                LoadlevelArray("medianrot");
              //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 3:
                LoadlevelArray("highrot");
            //    Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 4:
                LoadlevelArray("mediancm");
             //   Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 5:
                LoadlevelArray("highcm");
              //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            
            default:

                Debug.Log("none");
                break;

        }
        
        

        //rawImage.texture = prefabs[levelArray[levelCounter++]];
        //waittingImage.texture = prefabs[levelArray[waitCounter++]];

        //waittingtexture

    }

    void LoadlevelArray(string selection)
    {
        IniFile reader = new IniFile();
        Debug.Log("reading " + Application.dataPath + levelPath + levelName + ".ini");
        if (! reader.Load_File(Application.dataPath+ levelPath+ selection + ".ini"))
        {
            Debug.LogError("Unable to open level file");
        }
        
 
         reader.Goto_Section("Level Size");
        int levelsize = reader.Get_Int("levelSize");
        levelArray = new int[levelsize];
        for (int i = 0; i < levelsize; i++)
        {
            levelArray[i] = 0;
        }
        reader.Goto_Section("Level Index");
        levelArray = reader.Get_IntArray("index", levelArray);
        Debug.Log("Level " + selection + "Loaded");
    }
}
