using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


//Assign Prefab in inspector
//for every N second , it will generation a object and move to destination point;
public class GameObjectControllerV5 : MonoBehaviour
{
    public enum GameState { empty, start, playing, transition, stop };
    public GameObject[] prefabs;
    public bool GameIsStart = false;
    public Transform start;
    public Transform end;
    public Transform waittingPoint;
    public Transform waittingPoint2;
    public Transform gameobjectParent;
    [Header("User Input")]
    public float chunkMoveTime;
    public float curMovingTime = 0f;
    public string levelPath = "/MCpC_Result/";
    public string levelName = "default";
    [Header("Observation")]
    //public float distance;
    //public float speed;
    public int currentLevelIndex = 0;
    public bool ispass = false;
    public bool moveToNext = false;
    public GameState currentGameState;
    public float currentGameTime;
    public string currentLevelName = "default";
    public int[] levelIndexArray;
    public List<GameObject> currentLevel;
    public int currentCounter = 0;
    public int nextCounter = 1;
    public int next2Counter = 2;
    public SkeletonComparison comparisonScript;
    public UserStudyRecorderReader UserStudyRecorderScript;
    public SkeletonManKinectController UserInforScript;

    float instantScore = 0f;
    public Text UItext; //to display score
    public float currentScore = 0f;
    public float totalScore = 0f;

    float[] anglediffarray;
    // Use this for initialization
    void Start()
    {
        //totalChunkDurationtime = chunkMoveTime + extraTime;
        if (prefabs == null) { Debug.LogError("No prefab assign!!"); }
     //   if (comparisonScript == null || UserStudyRecorderScript == null || UserInforScript == null) { Debug.LogError("No script assign"); }

        if (UItext != null)
        {
            UItext.text = "Current Score " + currentScore + "\n total Score " + totalScore;
        }
        currentGameState = GameState.empty;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentGameState != GameState.empty)
        {
            currentGameTime += Time.deltaTime;
            curMovingTime += Time.deltaTime;
        }

        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
            //record start chunk frame 
            UserStudyRecorderScript.RecordChunkStartFrame();

            //activate avatars and start to move
            if (!currentLevel[currentCounter].activeSelf) {
                currentLevel[currentCounter].SetActive(true);
            }

            currentLevelIndex = levelIndexArray[currentCounter];
            if ((nextCounter > 0) && !currentLevel[nextCounter].activeSelf) currentLevel[nextCounter].SetActive(true);
            if ((next2Counter > 0) && !currentLevel[next2Counter].activeSelf) currentLevel[next2Counter].SetActive(true);
            currentGameState = GameState.playing;
        }
      


        else if (currentGameState == GameState.playing)
        { 
            //if avatar can move to next position
            if (moveToNext) 
            {
                currentLevel[currentCounter].transform.position = Vector3.Lerp(start.position, end.position, curMovingTime / chunkMoveTime);
                if (nextCounter > 0) currentLevel[nextCounter].transform.position = Vector3.Lerp(waittingPoint.position, start.position, curMovingTime / chunkMoveTime);
                if (next2Counter > 0) currentLevel[next2Counter].transform.position = Vector3.Lerp(waittingPoint2.position, waittingPoint.position, curMovingTime / chunkMoveTime);
         
            }


            //while avatar is moving or stopping, 
            //Instant comparison & update instant score
            InstanceComparison();

            if (instantScore >0f && !ispass) 
            {

                ispass = true;
                currentScore = instantScore * 100f;
                UserStudyRecorderScript.RecordKeyFrame();
                currentGameTime = 0f; //reset game timer
            }
        }

        //this chunk duration is end and game is not end;
        //if (currentGameTime > chunkMoveTime && (currentGameState !=GameState.stop) )
        if (currentGameState != GameState.stop)
        {   
            if(ispass) // user did correct pose in N second(slerptime) 
            {
                //destory current model
                currentLevel[currentCounter].GetComponent<AutoDestroy>().DestoryMe();
                currentLevel[currentCounter].SetActive(false); //destory current gameobject;
               
                //record endchunkFrame
                UserStudyRecorderScript.RecordChunkEndFrame();

                if (currentCounter == levelIndexArray.Length-1)
                {
                    print("last one" + currentCounter);
                    UserStudyRecorderScript.StopRecording(currentLevelName);
                    
                }

                //increase chunk counter to move avatar object
                IncrementCounters(); 

               //update total score
                totalScore += currentScore; 
                if (UItext != null)
                {
                    UItext.text = " Current Score " + currentScore + "\n total Score " + totalScore;
                }
                moveToNext = true; //let avatar move to next position
                ispass = false; //turn off is pass
            }
            else //if user does not do correctly, then avatar stop to move to next position
            {
                moveToNext = false;
            }
            

            //in all cases 
            //currentGameTime = 0; //reset game timer
            curMovingTime = 0f; //reset chunk moving timer




        }

    }

    //return avg angle diff// pass by ref
    public float InstanceComparison()
    {
        float anglediff = 0f;

        //Instant comparison
        instantScore = comparisonScript.InstanceCompare(levelIndexArray[currentCounter], ref UserInforScript.bones, ref anglediff, ref anglediffarray);
        return anglediff;
    }

   //public void InstanceArraryComparison(ref float[] anglediffarray)
    //{
     //  comparisonScript.InstanceCompareArray(levelIndexArray[currentCounter], ref UserInforScript.bones, ref anglediffarray);
    //}
    void IncrementCounters()
    {

        currentCounter++;
        nextCounter++;
        next2Counter++;
        if (next2Counter >= currentLevel.Count) { next2Counter = -2; }
        if (nextCounter >= currentLevel.Count) { nextCounter = -2; }

        if (currentCounter > currentLevel.Count-1)
        {
            StopTheGame();
        }
        else
        {
            currentGameState = GameState.transition;
        }

    }
    public void LevelSelection(int i)
    {
        switch (i)
        {
            case 1: //A
                ClearGameObject();
                LoadlevelArray("Default");
                currentLevelName = "Default";
                break;
            case 2: //B
                ClearGameObject();
                currentLevelName = "medianrot";
                LoadlevelArray("medianrot");
                //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 3://C
                ClearGameObject();
                currentLevelName = "highrot";
                LoadlevelArray("highrot");
                //    Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 4://D
                ClearGameObject();
                currentLevelName = "mediancm";
                LoadlevelArray("mediancm");
                //   Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 5:
                ClearGameObject();
                currentLevelName = "highcm";
                LoadlevelArray("highcm");
                //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 6:
                ClearGameObject();
                currentLevelName = "demo";
                LoadlevelArray("demo");
                //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            default:
                currentLevelName = " ";
                Debug.Log("none");
                break;

        }
    }
    public void StarttheGame()
        {
            currentCounter = 0;
            nextCounter = 1;
            next2Counter = 2;
            UserStudyRecorderScript.StartRecording();
            GameIsStart = true;
            currentGameState = GameState.start;
        }
    
    void LoadlevelArray(string selection)
    {
        IniFile reader = new IniFile();
        Debug.Log("reading " + Application.dataPath + levelPath + currentLevelName + ".ini");
        if (!reader.Load_File(Application.dataPath + levelPath + selection + ".ini"))
        {
            Debug.LogError("Unable to open level file");
        }


        reader.Goto_Section("Level Size");
        int levelsize = reader.Get_Int("levelSize");
        levelIndexArray = new int[levelsize];
        for (int i = 0; i < levelsize; i++)
        {
            levelIndexArray[i] = 0;
        }
        reader.Goto_Section("Level Index");
        levelIndexArray = reader.Get_IntArray("index", levelIndexArray);


        LoadGameObject();
        Debug.Log("Level " + selection + "Loaded");
    }

    void LoadGameObject()
    {
        //spaw the object
       // levelArray = new int[] { 0, 1, 2, 2, 1, 1, 2, 1, 2 };
        currentLevel = new List<GameObject>();

        Quaternion q =  Quaternion.identity;

        q.eulerAngles= new Vector3(0, 45, 0);
           
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[0]], start.position, start.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[1]], waittingPoint.position, waittingPoint.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[2]], waittingPoint2.position, waittingPoint2.rotation, gameobjectParent));

        for (int i = 3; i < levelIndexArray.Length; i++)
        {   if(levelIndexArray[i] == 5)
            {
                currentLevel.Add(Instantiate(prefabs[levelIndexArray[i]], waittingPoint2.position, q, gameobjectParent));
            }
            else
            {
                currentLevel.Add(Instantiate(prefabs[levelIndexArray[i]], waittingPoint2.position, waittingPoint2.rotation, gameobjectParent));
            }
            
            currentLevel[i].SetActive(false);
        }

        
    }
    //stop the game and recorder
    public void StopTheGame()
    {
        currentGameState = GameState.stop;
        GameIsStart = false;
        print("Game is stop");
       
        //this.enabled = false;
    }

    //clear all the avatar that is loaded in the game
    //use before load a new level
    void ClearGameObject()
    {
        int count = gameobjectParent.childCount;
        for (int i = count-1; i >= 0; i--)
        {
            DestroyObject(gameobjectParent.GetChild(i).gameObject, 1f);
        }
    }
}
