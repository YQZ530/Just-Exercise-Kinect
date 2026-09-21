using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;


//Assign Prefab in inspector
//for every N second , it will generation a object and move to destination point;
public class GameObjectControllerV4 : MonoBehaviour
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
   
    public string levelPath = "/MCpC_Result/";
    public string levelName = "default";
    [Header("Observation")]
    //public float distance;
    //public float speed;
    public int currentLevelIndex = 0;
    public bool ispass = false;
    public bool moveToNext = false;
    public GameState currentGameState;
 
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

    public float totalMovingTime = 1f;
    public float extraWaitingtime = 1f;
    public float curTimer = 0f;
    float extraTimeNeedtoWait;

    float[] angleDiffArry;
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

        //initialization
        angleDiffArry = new float[25];
        for (int i = 0; i < 25; i++)
        {
            angleDiffArry[i] = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState != GameState.empty)
        {
            curTimer += Time.deltaTime;
        }

        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
            //record start chunk frame 
            UserStudyRecorderScript.RecordChunkStartFrame();

            //activate avatars and start to move
            if (!currentLevel[currentCounter].activeSelf)
            {
                currentLevel[currentCounter].SetActive(true);
            }

            currentLevelIndex = levelIndexArray[currentCounter];
            if ((nextCounter > 0) && !currentLevel[nextCounter].activeSelf) currentLevel[nextCounter].SetActive(true);
            if ((next2Counter > 0) && !currentLevel[next2Counter].activeSelf) currentLevel[next2Counter].SetActive(true);
            currentGameState = GameState.playing;
        }


        //if player is playing current chunk
        else if (currentGameState == GameState.playing)
        {
            //if avatar can move to next position
            if (moveToNext)
            {
                currentLevel[currentCounter].transform.position = Vector3.Lerp(start.position, end.position, curTimer / totalMovingTime);
                if (nextCounter > 0) currentLevel[nextCounter].transform.position = Vector3.Lerp(waittingPoint.position, start.position, curTimer / totalMovingTime);
                if (next2Counter > 0) currentLevel[next2Counter].transform.position = Vector3.Lerp(waittingPoint2.position, waittingPoint.position, curTimer / totalMovingTime);

            }


            //while avatar is moving or stopping, 
            //Instant comparison & update instant score
            if (!ispass) { InstanceComparison(); }

            //if the score is passing, then tell sys to pass cur chunk
            if (instantScore > 0f && !ispass)
            {
                ispass = true;
                currentScore = instantScore * 100f;
                UserStudyRecorderScript.RecordKeyFrame();
                extraTimeNeedtoWait = Mathf.Abs(extraWaitingtime + curTimer); 


            }
        }

        //if avatar is move to the center and game is still playing; then move to next chunk
        if (curTimer > totalMovingTime && (currentGameState !=GameState.stop) )
       // if ((currentGameState != GameState.stop) )
        {   
            // user did correct pose and system wait for 1 second(extrawaiting time)
            //move to next chunk
            if (ispass && (curTimer > extraTimeNeedtoWait))
            {
               
                //destory current model
                currentLevel[currentCounter].GetComponent<AutoDestroy>().DestoryMe();
                currentLevel[currentCounter].SetActive(false); //destory current gameobject;

                //record endchunkFrame
                UserStudyRecorderScript.RecordChunkEndFrame();

                if (currentCounter == levelIndexArray.Length - 1)
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
                curTimer = 0f; //reset timer 
            }
            else if (!ispass)//if user does not do correctly, then avatar stop to move to next position
            {
                moveToNext = false;
                curTimer = 0f;
            }
            //else   when ispass is true but still not wait for 1 s

            //in all cases 
           
           //  curTimer = 0f; //reset current timer




        }

    }

    //return avg angle diff// pass by ref
    public float InstanceComparison()
    {
        float anglediff = 0f;

        //Instant comparison
        instantScore = comparisonScript.InstanceCompare(levelIndexArray[currentCounter], ref UserInforScript.bones, ref anglediff, ref angleDiffArry);
        return anglediff;
    }

    public void InstanceArraryComparison(ref float[] anglediffarray)
    {

        anglediffarray = angleDiffArry;
        //comparisonScript.InstanceCompareArray(levelIndexArray[currentCounter], ref UserInforScript.bones, ref anglediffarray);
    }
    void IncrementCounters()
    {

        currentCounter++;
        nextCounter++;
        next2Counter++;
        if (next2Counter >= currentLevel.Count) { next2Counter = -2; }
        if (nextCounter >= currentLevel.Count) { nextCounter = -2; }

        if (currentCounter > currentLevel.Count - 1)
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

        Quaternion q = Quaternion.identity;

        q.eulerAngles = new Vector3(0, 45, 0);

        currentLevel.Add(Instantiate(prefabs[levelIndexArray[0]], end.position, end.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[1]], start.position, start.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[2]], waittingPoint.position, waittingPoint.rotation, gameobjectParent));

        for (int i = 3; i < levelIndexArray.Length; i++)
        {
            if (levelIndexArray[i] == 5)
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
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyObject(gameobjectParent.GetChild(i).gameObject, 1f);
        }
    }
}
