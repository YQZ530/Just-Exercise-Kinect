using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Assign Prefab in inspector
//for every N second , it will generation a object and move to destination point;
public class GameObjectController : MonoBehaviour {
    public  enum GameState { empty, start, playing, transition,stop};
    public GameObject[] prefabs;
    public Transform start;
    public Transform end;
    public Transform waittingPoint;
    public Transform waittingPoint2;
   
    public float slerpTime;

    [Header("Observation")]
    //public float distance;
    //public float speed;
    public GameState currentGameState;
    public float currentTime;
    public int[] levelArray;
    public List<GameObject> currentLevel;
    public int currentCounter = 0;
    public int nextCounter = 1;
    public int next2Counter = 2;

    // Use this for initialization
    void Start () {

		if(prefabs == null) { Debug.LogError("No prefab assign!!"); }

        //spaw the object
        levelArray = new int[] { 0,0,0, 1, 2, 2, 1, 1, 2, 1 ,2 };
        currentLevel = new List<GameObject>();
        for (int i = 0; i < levelArray.Length; i++)
        {
            currentLevel.Add(Instantiate(prefabs[levelArray[i]], waittingPoint2.position, waittingPoint2.rotation));
            currentLevel[i].SetActive(true);
        }

        currentGameState = GameState.empty;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentGameState != GameState.empty)
        {
            currentTime += Time.deltaTime;
        }

        if (currentGameState == GameState.start || currentGameState == GameState.transition)
        {
            currentLevel[currentCounter].SetActive(true);
            if (nextCounter > 0) currentLevel[nextCounter].SetActive(true);
            if (next2Counter >0 ) currentLevel[next2Counter].SetActive(true);
            currentGameState = GameState.playing;
        }
       
       
       if(currentGameState == GameState.playing)
        {

           
                currentLevel[currentCounter].transform.position = Vector3.Lerp(start.position, end.position, currentTime / slerpTime);
                if (nextCounter > 0) currentLevel[nextCounter].transform.position = Vector3.Lerp(waittingPoint.position, start.position, currentTime / slerpTime);
                if (next2Counter > 0) currentLevel[next2Counter].transform.position = Vector3.Lerp(waittingPoint2.position, waittingPoint.position, currentTime / slerpTime);
           
          
        }

       //this chunk duration is end
       if(currentTime > slerpTime)
        {
            currentTime = 0;
            currentLevel[currentCounter].SetActive(false); //destory current gameobject;
            IncrementCounters();
        }
       
	}

    void IncrementCounters()
    {

        currentCounter++;
        nextCounter++;
        next2Counter++;
        if (next2Counter >= currentLevel.Count) { next2Counter = -2; }
        if (nextCounter >= currentLevel.Count) { nextCounter = -2; }

        if (currentCounter >= currentLevel.Count)
        {
            currentGameState = GameState.stop;
            this.enabled = false;
        }
        else
        {
            currentGameState = GameState.transition;
        }

    }

    public void StarttheGame()
    {
        currentGameState = GameState.start;
    }
}
