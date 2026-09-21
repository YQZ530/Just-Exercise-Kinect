using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Assign Prefab in inspector
//for every N second , it will generation a object and move to destination point;
public class GameObjectControllerV3 : MonoBehaviour
{
    public enum GameState { empty, start, playing, transition, stop };
    public GameObject[] prefabs;
    public bool GameIsStart = false;
    public Transform start;
    public Transform end;
    public Transform waittingPoint;
    public Transform waittingPoint2;

    public Transform startL; //start left
    public Transform waittingPointL; //waitting point left
    public Transform waittingPoint2L;
    public Transform gameobjectParent;
    public ParticleSystem flare;

    [Header("User Input")]
    public float slerpTime;
    public string levelPath = "/MCMC_Result/";
    public string levelName = "default";
    [Header("Observation")]
   
    public int currentLevelIndex = 0;
    public bool next = true;
    public GameState currentGameState;
    public float currentTime;
    public string currentLevelName = "default";
    public int[] levelIndexArray;
    public List<GameObject> currentLevel;

    public List<GameObject> leftSidecurrentLevel;
    public int currentCounter = 0;
    public int nextCounter = 1;
    public int next2Counter = 2;
    public SkeletonComparison comparisonScript;
    public UserStudyRecorderReader UserStudyRecorderScript;
    // Use this for initialization
    void Start()
    {

        if (prefabs == null) { Debug.LogError("No prefab assign!!"); }
        if (comparisonScript == null || UserStudyRecorderScript == null || flare == null) { Debug.LogError("No script assign"); }

        currentGameState = GameState.empty;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentGameState != GameState.empty)
        {
            currentTime += Time.deltaTime;
        }

        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
       
            if (!currentLevel[currentCounter].activeSelf) {
                currentLevel[currentCounter].SetActive(true);
                leftSidecurrentLevel[currentCounter].SetActive(true);

            }
            currentLevelIndex = levelIndexArray[currentCounter]; //update current level index

            if ((nextCounter > 0) && !currentLevel[nextCounter].activeSelf) {
                currentLevel[nextCounter].SetActive(true);
                leftSidecurrentLevel[nextCounter].SetActive(true);
            }
            if ((next2Counter > 0) && !currentLevel[next2Counter].activeSelf)
            {
                currentLevel[next2Counter].SetActive(true);
                currentLevel[next2Counter].GetComponent<ChangeMyselfColor>().startChanging = true;

                leftSidecurrentLevel[next2Counter].SetActive(true);
                leftSidecurrentLevel[next2Counter].GetComponent<ChangeMyselfColor>().startChanging = true;

            }

            currentGameState = GameState.playing;
        }
      


        if (currentGameState == GameState.playing)
        {
            if (next) //if can be moving
            {
                currentLevel[currentCounter].transform.position = Vector3.Lerp(start.position, end.position, currentTime / slerpTime);
                if (nextCounter > 0) currentLevel[nextCounter].transform.position = Vector3.Lerp(waittingPoint.position, start.position, currentTime / slerpTime);
                if (next2Counter > 0) currentLevel[next2Counter].transform.position = Vector3.Lerp(waittingPoint2.position, waittingPoint.position, currentTime / slerpTime);

                leftSidecurrentLevel[currentCounter].transform.position = Vector3.Lerp(startL.position, end.position, currentTime / slerpTime);
                if (nextCounter > 0) leftSidecurrentLevel[nextCounter].transform.position = Vector3.Lerp(waittingPointL.position, startL.position, currentTime / slerpTime);
                if (next2Counter > 0) leftSidecurrentLevel[next2Counter].transform.position = Vector3.Lerp(waittingPoint2L.position, waittingPointL.position, currentTime / slerpTime);

            }
            comparisonScript.RecordCurrentMotion();

        }

        //this chunk duration is end and game is not end;
        if (currentTime > slerpTime && (currentGameState !=GameState.stop) )
        {
            
            //calculate score 
            float score = 0f;
            //calculate  score
            if (currentCounter < currentLevel.Count)
            {
                score = comparisonScript.Compare(levelIndexArray[currentCounter]);
                //score = Random.Range(60f, 100f);
                comparisonScript.UpdateScore(score);
            }

            if(score ==0f) //if user does not do that action, the game object stop
            {
                next = false;

            }
            else
            {
                flare.Play();
                next = true;
                currentLevel[currentCounter].GetComponent<AutoDestroy>().DestoryMe();
                currentLevel[currentCounter].SetActive(false); //destory current gameobject;

                leftSidecurrentLevel[currentCounter].GetComponent<AutoDestroy>().DestoryMe();
                leftSidecurrentLevel[currentCounter].SetActive(false); //destory current gameobject;

            
                if (currentCounter == levelIndexArray.Length-1)
                {
                    //print("last one" + currentCounter);
                   // UserStudyRecorderScript.StopRecording(currentLevelName);
                    
                }
                
                comparisonScript.clearMotionFrame();
                IncrementCounters(); //move avatar object
            }
            currentTime = 0;
           
            comparisonScript.clearMotionFrame();
        }

    }

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
    //UI drop down will call this method
    public void LevelSelection(int i)
    {
        switch (i)
        {
            case 1:
                ClearGameObject();
                LoadlevelArray("Default");
                currentLevelName = "Default";
                break;
            case 2:
                ClearGameObject();
                currentLevelName = "medianrot";
                LoadlevelArray("medianrot");
                //  Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 3:
                ClearGameObject();
                currentLevelName = "highrot";
                LoadlevelArray("highrot");
                //    Debug.Log("Level " + i.ToString() + "Loaded");
                break;
            case 4:
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
        //set up color 
        ChangeMyselfColor script1 = currentLevel[currentCounter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script1, 1f, new Color(0, 0, 0, 0.6f));
        ChangeMyselfColor script2 = currentLevel[nextCounter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script2, 2f, new Color(0, 0, 0, 0.4f));
        ChangeMyselfColor script3 = currentLevel[next2Counter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script3, 3f, new Color(0, 0, 0, 0.3f));

        ChangeMyselfColor script4 = leftSidecurrentLevel[currentCounter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script4, 1f, new Color(0, 0, 0, 0.6f));
        ChangeMyselfColor script5 = leftSidecurrentLevel[nextCounter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script5, 2f, new Color(0, 0, 0, 0.4f));
        ChangeMyselfColor script6 = leftSidecurrentLevel[next2Counter].GetComponent<ChangeMyselfColor>();
        SetUpGameObjectColor(script6, 3f, new Color(0, 0, 0, 0.3f));


        
        currentLevel[nextCounter].GetComponent<ChangeMyselfColor>().startChanging = true;
        currentLevel[next2Counter].GetComponent<ChangeMyselfColor>().startChanging = true;

        leftSidecurrentLevel[nextCounter].GetComponent<ChangeMyselfColor>().startChanging = true;
        leftSidecurrentLevel[next2Counter].GetComponent<ChangeMyselfColor>().startChanging = true;
        //UserStudyRecorderScript.StartRecording();
        GameIsStart = true;
            currentGameState = GameState.start;
        }
    //for the first three game objects
    void SetUpGameObjectColor(ChangeMyselfColor script, float dur, Color color)
    {
        script.duration = dur;
        script.colorStart = color;
        script.startChanging = true;
    }
    void LoadlevelArray(string selection)
    {
        IniFile reader = new IniFile();
        Debug.Log("reading " + Application.dataPath + levelPath + levelName + ".ini");
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
       
        currentLevel = new List<GameObject>();
        leftSidecurrentLevel = new List<GameObject>();
       // Quaternion q =  Quaternion.identity;

       // q.eulerAngles= new Vector3(0, 0, 0);
           
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[0]], start.position, start.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[1]], waittingPoint.position, waittingPoint.rotation, gameobjectParent));
        currentLevel.Add(Instantiate(prefabs[levelIndexArray[2]], waittingPoint2.position, waittingPoint2.rotation, gameobjectParent));
        //add same thing in the left side
        leftSidecurrentLevel.Add(Instantiate(prefabs[levelIndexArray[0]], startL.position, startL.rotation, gameobjectParent));
        leftSidecurrentLevel.Add(Instantiate(prefabs[levelIndexArray[1]], waittingPointL.position, waittingPointL.rotation, gameobjectParent));
        leftSidecurrentLevel.Add(Instantiate(prefabs[levelIndexArray[2]], waittingPoint2L.position, waittingPoint2L.rotation, gameobjectParent));

        for (int i = 3; i < levelIndexArray.Length; i++)
        {  
            currentLevel.Add(Instantiate(prefabs[levelIndexArray[i]], waittingPoint2.position, waittingPoint2.rotation, gameobjectParent));
            currentLevel[i].SetActive(false);

            leftSidecurrentLevel.Add(Instantiate(prefabs[levelIndexArray[i]], waittingPoint2L.position, waittingPoint2L.rotation, gameobjectParent));
            leftSidecurrentLevel[i].SetActive(false);
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
