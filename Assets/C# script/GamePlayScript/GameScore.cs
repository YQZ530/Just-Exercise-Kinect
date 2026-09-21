using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour {
    KinectManager manager;
    long userId = 0;
   
    // Use this for initialization
    void Start () {
        manager = KinectManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
       // if (!manager && !manager.IsInitialized()) { Debug.Log("Cannot find manager!! Cannot Record!!"); return; }
       // if (!manager.IsUserDetected()) { Debug.Log("Cannot find user!!  Cannot Record!!"); return; }

       // userId = manager.GetPrimaryUserID();
       // manager.GetJointOrientation(userId, 4, false); // left
       // manager.GetJointOrientation(userId, 8, false); //r
       //if(UItext != null)
       // {
       //     UItext.text = "left " + manager.GetJointOrientation(userId, 4, false).ToString() + "  right  "
       //         + manager.GetJointOrientation(userId, 8, false).ToString();
       // }

       
    }
    float CalculationScore()
    {
        return 0f;
    }

}
