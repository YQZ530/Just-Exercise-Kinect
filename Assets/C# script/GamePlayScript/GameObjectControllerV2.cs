using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectControllerV2 : MonoBehaviour {

    public enum GameState { empty, start, playing, transition, stop };
    public Texture[] prefabs;
    public GameObject[] gameObjects;
   
    public Transform parent;
    public RawImage LeftToMainImage;
    public RawImage Left1ToLeftImage;
    public RawImage EdgeToLeft1Image;

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
    public int waitCounter1 = 3;
    public SkeletonComparison comparisionScript;
    public UserStudyRecorderReader recorderScript;
    
    // Use this for initialization
    void Start()
    {

        if (prefabs == null) { Debug.LogError("No prefab assign!!"); }
        if(EdgeToLeft1Image == null) { Debug.LogError("No rawimage assign!!"); }


        EdgeToLeft1Image.GetComponent<Animation>().playAutomatically = false;
         LeftToMainImage.GetComponent<Animation>().playAutomatically = false;

        currentGameState = GameState.empty;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
            //comparisionScript.RecordCurrentMotion();
            print("chunk " + levelArray[levelCounter-1]);
            currentGameState = GameState.playing;
        }
      
        else if (currentGameState == GameState.playing)
        {
            //comparisionScript.RecordCurrentMotion();
        }

        //this chunk duration is end
        if (currentTime > slerpTime && currentGameState == GameState.playing)
        {
            
            currentGameState = GameState.transition;
            if(levelCounter != levelArray.Length)
            {
                //update score
              //  comparisionScript.Compare(levelArray[levelCounter-1]);
            }
           
            //comparisionScript.clearMotionFrame();
            UpdateLevelChunk();
            currentTime = 0f;

        }

    }
    public void StartGame()
    {
        print("Game start");

        levelCounter = 0;
        waitCounter = 1;
        waitCounter1 = 2;
        // main  <<- left    <<-left1
        EdgeToLeft1Image.texture = prefabs[levelArray[waitCounter1++]];
        Left1ToLeftImage.texture = prefabs[levelArray[waitCounter++]];
        LeftToMainImage.texture = prefabs[levelArray[levelCounter++]];
       
        //EdgeToLeftImage.GetComponent<Animation>().playAutomatically = true;
        //LeftToMainImage.GetComponent<Animation>().playAutomatically = true;


        EdgeToLeft1Image.GetComponent<Animation>().enabled = true;
        Left1ToLeftImage.GetComponent<Animation>().enabled = true;
        LeftToMainImage.GetComponent<Animation>().enabled = true;
        currentGameState = GameState.start;
        currentTime = 0f;
        //recorderScript.StartRecording();
    }
    void UpdateLevelChunk()
    {
        //this is for moving object
        if(levelCounter < levelArray.Length)
        {
            LeftToMainImage.texture = prefabs[levelArray[levelCounter++]];
          
        }
        else
        {
         
            currentGameState = GameState.stop;
            recorderScript.StopRecording("test");
        }
        //this is for waiting object
        if (waitCounter < (levelArray.Length ))
        {
            Left1ToLeftImage.texture = prefabs[levelArray[waitCounter++]];
        }
        else
        {
            Left1ToLeftImage.GetComponent<Animation>().Stop();
            Left1ToLeftImage.texture = null;
        }

        //this is for waiting object
        if (waitCounter1 < (levelArray.Length))
        {
            EdgeToLeft1Image.texture = prefabs[levelArray[waitCounter1++]];
        }
        else
        {
            EdgeToLeft1Image.GetComponent<Animation>().Stop();
            EdgeToLeft1Image.texture = null;
        }

    }
    public void LevelSelection(int i)
    {
        switch (i)
        {
            case 1:
               
                LoadlevelArray("default");
                Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 2:
                LoadlevelArray("medianrot");
                Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 3:
                LoadlevelArray("highrot");
               Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 4:
                LoadlevelArray("mediancm");
                Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 5:
                LoadlevelArray("highcm");
               Debug.Log("Level " + i.ToString() + "Loaded");
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
