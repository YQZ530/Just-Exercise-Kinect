
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FindMatchKeyFrame : MonoBehaviour {
	
	[System.Serializable]
	public class Level
	{
		public string levelName;
		public int[] keypose;
	}


	public KeyposeControl keyposeControl;
	public ReadUserData userData;

	public string keyposeDataFile;

	public string levelsPath;
	public Level[] levels;
	public int levelPtr = -1;
	public int keyposePtr; 



	public List<int> matchFrame;
	IniFile keyposeAngle;


	public float[] weight;

	// Use this for initialization
	void Start () {
		if (keyposeDataFile == "") {
			keyposeDataFile = Application.dataPath + "/BiaoLevel/UserResult/keyposeAngle.ini";
		}

		if (levelsPath == "")
			levelsPath = Application.dataPath + "/BiaoLevel/levels.ini";
		LoadLevels (levelsPath);

		userData = GetComponent<ReadUserData> ();

		keyposeAngle = new IniFile ();
		keyposeAngle.Load_File (keyposeDataFile);

		for (int i = 0; i < levels.Length; i++) {
			if (userData.level == levels [i].levelName) 
			{
				levelPtr = i;
				break;
			}
		}

		weight = new float[25];

		//runMatchPose ();

	}



	void Update()
	{
		

	}


	public void runMatchPose()
	{
		StartCoroutine (findMatchPose ());

	}


	IEnumerator findMatchPose()
	{

		while (!userData.loadingCompleted) {
			yield return new WaitForSeconds (1f);
		}





		int startFrame = 0, endFrame = 0;

		for (int i = 0; i < levels [levelPtr].keypose.Length; i++) {
			
			startFrame = userData.keyFrameIndexList [i];



			if ((i + 2) < levels [levelPtr].keypose.Length)
				endFrame = userData.keyFrameIndexList [i + 2];
			else if (i == 0)
				endFrame = userData.keyFrameIndexList [i + 2];
			else
				endFrame = userData.totalFrame;

			// visualize
			keyposeControl.DisplayChild (levels [levelPtr].keypose [i]);

			UpdateWeight (levels [levelPtr].keypose [i]);

			keyposeAngle.Goto_Section (levels [levelPtr].keypose [i].ToString ());

			float closest = float.MaxValue;
			int closestFrame = startFrame;


			// load keypose angle data
			float[] keypose = new float[25];
			for (int k = 0; k < 25; k++) {
				keypose [k] = keyposeAngle.Get_Float (k.ToString ());
			}


			// run comparison
			for (int j = startFrame; j < endFrame; j++) {
				userData.GetFrameAt (j);
				userData.GetFrameAt (j);


				float score = getMatchRate(keypose, userData.jointAngle);

				userData.matchRate = score;

				if (score < closest) {
					closest = score;
					closestFrame = j;
				} 

				yield return null; 
			
			}

			matchFrame.Add (closestFrame);
			userData.GetFrameAt (closestFrame);
			userData.GetFrameAt (closestFrame);

			keyposeControl.DisplayChild (levels [levelPtr].keypose [i]);


			yield return null;


			//ScreenCapture.CaptureScreenshot (Application.dataPath.Substring(0,Application.dataPath.Length - 7) + "/Match/" + userData.user + "_" + closestFrame + "_" + Mathf.Round(closest) + ".jpg"); 

			Debug.Log ("Frame: " + closestFrame + " with Score: " + closest);

			//yield return null;
		
		}

		SaveMatchToFile ();


		userData.GetNextUser ();

		yield return null;

	}


	private void SaveMatchToFile()
	{
		string s = "";

		for (int i = 0; i < matchFrame.Count - 1; i++) {
			s += matchFrame [i] + ",";
		
		}
		s += matchFrame [matchFrame.Count - 1];

		userData.SaveUserData ("Match", "key frame", s);

	}


	public float UpdateKeyposeDegree(int key)
	{
		keyposeAngle.Goto_Section (levels [levelPtr].keypose [key].ToString ());
		float[] keypose = new float[25];
		for (int k = 0; k < 25; k++) {
			keypose [k] = keyposeAngle.Get_Float (k.ToString ());
		}
		return (getMatchRate (keypose, userData.jointAngle));
	}



	public float getMatchRate(float[] keyposeDegree, float[] userDegree)
	{
		float percent = 0.0f; 
		int count = 0;
		for (int i = 1; i < keyposeDegree.Length; i++) {
			if (keyposeDegree [i] == 0.0f)
				continue;
			//float p = Mathf.Abs (keyposeDegree [i] - userDegree [i]) / 180f;



			float p = 0f;

			if (keyposeDegree [i] <= 0f && userDegree [i] <= 0f) {
				p = Mathf.Abs( Mathf.Abs (keyposeDegree [i]) - Mathf.Abs (userDegree [i]));
			} else if (keyposeDegree [i] > 0f && userDegree [i] > 0f) {
				p = Mathf.Abs (keyposeDegree [i] - userDegree [i]);
			} else {
				p = 360f - Mathf.Abs (keyposeDegree [i]) - Mathf.Abs (userDegree [i]);
			}

			p /= 180;

			//Debug.Log (p);

			percent += p;  
			count++;
			
		}


		return percent / count;

	}


	public void UpdateWeight(int key)
	{
		weight = new float[25];
		switch (key) {
		case 1:
			weight [8] = 3f;
			return;
		case 3:
			weight [8] = 1.5f;
			weight [4] = 1.5f;
			return;
		case 7:
			weight [8] = 1.5f;
			weight [4] = 1.5f;
			return;
		case 9:
			weight [4] = 3f;
			return;
		}

	}



	// loading goes here

	private void LoadLevels(string path)
	{
		IniFile data = new IniFile ();
		data.Load_File (path);

		for (int i = 0; i < levels.Length; i++) {
			string name = levels [i].levelName;

			data.Goto_Section (name);

			levels [i].keypose = new int[50];
			levels [i].keypose = data.Get_IntArray ("keypose", levels [i].keypose);
			
		}

	}
}
