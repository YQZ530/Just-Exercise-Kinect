using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Player{
	public int id;
	public string level;
	public IniFile userData;
	public int[] keyFrame;
	public int keyItr;
}

public class UserDataReader : MonoBehaviour {
	public Player player;
	public GameObject currentPose;
	public List<Transform> current;
	public GameObject targetPose;
	public List<Transform> target;

	public string path;
	public int userCount;
	public string[] levels;
	public int selectLevel;

	public bool[] check;
	public float[] jointAngle;


	public IniFile result;
	public string resultPath;
	public string resultName;

	// Use this for initialization
	void Start () {
		jointAngle = new float[25];


		current = new List<Transform> ();
		target = new List<Transform> ();

		get_model_transform_list (current, currentPose.transform);
		get_model_transform_list (target, targetPose.transform);



		result = new IniFile ();
		result.Load_File ( resultPath + levels[selectLevel] + resultName);

		StartCoroutine (run ());
		//Debug.Log (print_all(levels[selectLevel]));

	}

	IEnumerator run()
	{
		for (int user = 1; user <= 25; user++) {
			player = get_user_data (user, levels[selectLevel]);
			Debug.Log ("User " + player.id);
			if (player == null)
				continue;
			for (int i = 0; i < player.keyFrame.Length - 2; i++) {
				UpdatePose (player.keyFrame [i], player.keyFrame [i + 1]);

				yield return new WaitForSeconds (0f);
			}
			UpdateResult ("sum");
		}

		Debug.Log (print_all(levels[selectLevel]));
		yield return null;
	}
	
	

	Player get_user_data(int uID, string level)
	{
		string fileName = get_file_name (uID, level);
		//Debug.Log (fileName);
		//Debug.Log (path);

		Player p = new Player ();

		p.id = uID;
		p.level = level;
		p.userData = new IniFile ();
		p.userData.Load_File (path + fileName);

		if (p.userData == null) {
			Debug.Log ("No " + fileName + " found!");
			return null;
		}



		// get key frames
		p.userData.Goto_Section ("endchunkkeyframe");
		p.keyFrame = new int[1];
		p.keyFrame = p.userData.Get_IntArray ("endchunkkeyframe", p.keyFrame);

		// get keyframe count
		p.keyItr = 0;

		return p;
	}
 	
	void UpdatePose(int currentFrame,int targetFrame )
	{
		if (player == null) {
			Debug.Log ("Pls ini player");
			return;
		}

		UpdateModel (currentPose, currentFrame);
		UpdateModel (targetPose, targetFrame);
		UpdateJointAngle ();
	
	}

	void UpdateModel(GameObject model, int frame)
	{
		player.userData.Goto_Section (frame.ToString());
		//Debug.Log (player.keyFrame [frame].ToString ());
		//Debug.Log ("U model:"+frame.ToString ());
		update_all_joints (model.transform);
	}
	private void update_all_joints(Transform t )
	{
		int jointID =int.Parse(t.name.Substring (0, 2));
		//Debug.Log (jointID);
		t.rotation = player.userData.Get_Quaternion ("r" + jointID, t.rotation);
		t.position = player.userData.Get_Vector3 ("p" + jointID, t.position);

		if (t.childCount == 0)
			return;

		foreach (Transform child in t) {
			update_all_joints (child);
		}
	}
	private void get_model_transform_list(List<Transform> list, Transform t)
	{
		list.Add (t);
		if (t.childCount == 0)
			return;
		foreach (Transform child in t) {
			get_model_transform_list (list, child);
		}
	}

	string get_file_name(int uID, string level)
	{
		string name = "us";
		if (uID < 10) {
			name += "00" + uID;
		} else {
			name += "0" + uID;
		}

		return name + level + ".ini";
	}

	void UpdateJointAngle()
	{
		//ClearAngle ();
		for (int i = 0; i < current.Count; i++) {
			int jointID =int.Parse(current [i].name.Substring (0, 2));
			if (!check[jointID])
				continue;

			if (jointID == 4 || jointID == 8 || jointID == 1) {
				jointAngle [jointID] += Quaternion.Angle (current [i].localRotation, target [i].localRotation);
			} else {
			
				float a1 = calculateAngle (jointID, current, i);
				float a2 = calculateAngle (jointID, target, i);

				jointAngle [jointID] += Mathf.Abs (a1 - a2);
			}

			//jointAngle [jointID] += Quaternion.Angle (current [i].localRotation, target [i].localRotation);
		}
	}

	private float calculateAngle(int jointID, List<Transform> model, int position)
	{
		Vector3 v1, v2;
		if (jointID == 0) {
			// current
			v1 = Vector3.down;
			v2 = (model [9].position - model [0].position).normalized;
		} 
		else
		{
			// current
			v1 = model [position].parent.position - model [position].position;
			v2 = (model [position].GetChild(0).position - model [position].position).normalized;
		}
		return Vector3.Angle (v1, v2);
	}

	void UpdateResult(string target)
	{
		string sectionName = player.id.ToString () + player.level;
		if (!result.Is_Section (sectionName))
			result.Create_Section (sectionName);
		result.Goto_Section (sectionName);

		result.Set_FloatArray (target, jointAngle, "");
		result.Save ();
		ClearAngle ();
			
	}

	void ClearAngle()
	{
		for (int i = 0; i < 25; i++) {
			jointAngle [i] = 0f;
		}
	}
	string print_all(string level)
	{
		string s = "";
		for (int i = 1; i <= 25; i++) {
			s+="\n";
			s += "user" + i.ToString () + ",";
			result.Goto_Section (i.ToString () + level);
			float[] angle = new float[25];
			angle = result.Get_FloatArray ("sum", angle);
			for (int j = 0; j < 25; j++)
			{
				if (angle [j] == 0f)
					continue;
				s+=angle[j]+",";
			}
			//s+="\n";
		}

		return s;
	}

}
