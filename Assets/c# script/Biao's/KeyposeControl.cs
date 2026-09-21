using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyposeControl : MonoBehaviour {

	public List<Transform> children;
	bool loadChild;

	int currentDisplay;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			children.Add (child);
			loadChild = true;
		}
	}
	
	// Update is called once per frame 
	void Update () {
		
	}


	public void DisplayChild(int index)
	{
		//if (!(index >= 0 && index <= 9)) {
		//	Debug.Log ("Display Out of Bound");
		//	return;
		//}
		//
		//if (!loadChild) {
		//	Debug.Log ("Loading not completed");
		//	return;
		//}




		//children [currentDisplay].gameObject.SetActive (false);
		//children [index].gameObject.SetActive (true);
		//currentDisplay = index;
		TurnOnDisplay(index.ToString());



	}

	public void TurnOnDisplay(string name)
	{
		foreach (Transform t in children) {
			if (t.name == name)
				t.gameObject.SetActive (true);
			else
				t.gameObject.SetActive (false);
		}

	}


}
