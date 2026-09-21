using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadUserData : MonoBehaviour {

	public class Frame
    {
        public List<Vector3> kinectPos;
        public List<Vector3> pos;
        public List<Quaternion> rot;
        public List<Quaternion> mrot;
        public Vector3 bodypos;
        public Quaternion bodyrot;

        public Frame(int totaljoint)
        {
            pos = new List<Vector3>(totaljoint);
            rot = new List<Quaternion>(totaljoint);
            mrot = new List<Quaternion>(totaljoint);
            kinectPos = new List<Vector3>(totaljoint);
            bodypos = Vector3.zero;
            bodyrot = Quaternion.identity;
        }
    }


	public AnimationCurve plot;

	public AvatarCreationV2 root;


	public string path;
	public string user;
	public string level;



	public int totalFrame;
	public int displayFrame;
	int currentFrame = -1;

	public bool autoRun;

	public List<Frame> userMovementData = new List<Frame> ();
	public int[] keyFrameIndexList = new int[50];
	public int[] endFrameIndexList = new int[50];

	public float loadingProgress;
	public bool loadingCompleted;

	IniFile file;


	Transform[] joints;
	public float[] jointAngle;

	public string keyposeDataFile;


	FindMatchKeyFrame findMatch;



	public List<int> manualMatchFrame;

	public int keyCount = 0;

	public Text score;
	public float matchRate = 0f;

	// Use this for initialization
	void Start () {


		if (path == "") {
			path = Application.dataPath + "/UserStudy/user/";
		}



		joints = new Transform[root.bones.Length];
		for (int i = 0; i < 25; i++) {
			joints [i] = root.bones [i].transform;
			
		}


		findMatch = GetComponent<FindMatchKeyFrame> ();


		StartLoading ();


	}

	void Update()
	{

		score.text = "Frame = " + displayFrame + "/" + totalFrame +  "\tScore = " + matchRate;
		UpdateThisFrame ();

		if (!loadingCompleted)
			return;

		if (autoRun) {

			if (displayFrame < totalFrame - 1)
				displayFrame++;
			else
				displayFrame = 0;
			
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			manualMatchFrame.Add (displayFrame);
			findMatch.keyposeControl.DisplayChild (findMatch.levels [findMatch.levelPtr].keypose [++keyCount]);
			Debug.Log ("Recorded! " + displayFrame); 
		} else if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
			SaveMatchToFile ();
		
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!loadingCompleted)
			return;


		if (Input.GetKey (KeyCode.RightArrow)) {
			displayFrame = (displayFrame < totalFrame) ? displayFrame+1 : displayFrame;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			displayFrame = (displayFrame > 0) ? displayFrame-1 : displayFrame;
		}

		//UpdateThisFrame ();


			//Debug.Log ("Update");


		//for (int i = 0; i < 25; i++)
		//{
			// update user result
		//	root.bones[i].transform.position = userMovementData[displayFrame].pos[i];
		//	root.ReDrawSkeletonLine();



		//}


	}




	public void GetFrameAt(int frame)
	{
		if (frame == displayFrame)
			return;
		else if (frame >= totalFrame)
			return;

		displayFrame = frame;

		UpdateThisFrame ();

	}

	private void UpdateThisFrame()
	{

		for (int i = 0; i < 25; i++)
		{
			// update user result
			root.bones[i].transform.position = userMovementData[displayFrame].pos[i];
			root.ReDrawSkeletonLine();

			// calculate angle
			jointAngle[i] = calSignedAngle(i);

		}

	}











	// loading goes here

	public void StartLoading()
	{
		StartCoroutine(Load());

	}

	IEnumerator Load()
	{
		string loadingPath = path + user + level + ".ini";
		Debug.Log(loadingPath);
		file = new IniFile ();
		file.Load_File (loadingPath);
		// get total frame;
		file.Goto_Section ("TotalFrame");
		totalFrame = file.Get_Int ("TotalFrame");

		// get keyFrame;
		file.Goto_Section("keyframe");
		keyFrameIndexList = file.Get_IntArray ("keyframe", keyFrameIndexList);

		// get endchunkkeyFrame;
		file.Goto_Section("endchunkkeyframe");
		endFrameIndexList = file.Get_IntArray ("endchunkkeyframe", endFrameIndexList);

		// 

		for (int i = 0; i < totalFrame; i++) {
			file.Goto_Section (i.ToString());

			Frame frame = new Frame(25);

			for (int j = 0; j < 25; j++)
			{
				Vector3 v = file.Get_Vector3("p" + j.ToString(), Vector3.zero);
				Quaternion q = file.Get_Quaternion("r" + j.ToString(), Quaternion.identity);
				Quaternion mirrorq = file.Get_Quaternion("mirrorR" + j.ToString(), Quaternion.identity);
				frame.pos.Add(v);
				frame.rot.Add(q); 
				frame.mrot.Add(mirrorq);

			} 

			userMovementData.Add (frame);
			loadingProgress = i*1.0f / totalFrame * 100;

			if (loadingProgress % 20 <0.1)
				yield return null;

		
		}
		Debug.Log (userMovementData.Count);
		loadingCompleted = true;


		//findMatch.runMatchPose (); 
	

	}

	 
	public void GetNextUser()
	{

		int id = int.Parse(user.Substring (3, 2)); 




		id++;

		if (id > 25)
			return;


		user = "us0";
		if (id < 10)
			user += "0" + id;
		else
			user += id;



		StartLoading ();


	}













	// calcualte angle

	private float calculateAngle(int index)
	{
		switch (index) {
		case 0: // spinebase with hips
			return Vector3.Angle(joints[13].position - joints[0].position, joints[17].position - joints[0].position);
		case 1: // spine mid with spinebase and neck
			return Vector3.Angle(joints[0].position - joints[1].position, joints[20].position - joints[1].position);
		case 4: // left should with shoulder and elbow
			return Vector3.Angle(joints[5].position - joints[4].position, joints[0].position - joints[1].position);
		case 5: // left elbow with left shoulder and wrist
			return Vector3.Angle(joints[4].position - joints[5].position, joints[6].position - joints[5].position);
		case 8: // right shoulder with shoulder and elbow
			return Vector3.Angle(joints[9].position - joints[8].position, joints[0].position - joints[1].position);
		case 9: // right elbow with right shoulder and arm
			return Vector3.Angle(joints[8].position - joints[9].position, joints[10].position - joints[9].position);
		case 12: // left hip with base and knee
			return Vector3.Angle(joints[1].position - joints[0].position, joints[13].position - joints[12].position);
		case 13: // left knwee
			return Vector3.Angle(joints[12].position - joints[13].position, joints[14].position - joints[13].position);
		case 16: // left hip with base and knee
			return Vector3.Angle(joints[1].position - joints[0].position, joints[17].position - joints[16].position);
		case 17: // left knwee
			return Vector3.Angle(joints[16].position - joints[17].position, joints[18].position - joints[17].position);
		}



		return 0.0f;

	}



	public float calSignedAngle(int index)
	{
		//Transform tar = transform, own = transform;
		Vector3 fromJoint = Vector3.zero, toJoint = Vector3.zero;

		switch (index) {
		case 0: // spinebase with hips
			{
				return Vector3.Angle (joints [12].position - joints [0].position, joints [16].position - joints [0].position);
			}
		case 1: // spine mid with spinebase and neck
			return Vector3.Angle(joints[0].position - joints[1].position, joints[20].position - joints[1].position);
		case 4: // left should with shoulder and elbow
			{


				fromJoint = joints [5].position - joints [4].position;
				toJoint = joints [1].position - joints [20].position;

				//swap
				//fromJoint = joints[9].position - joints[8].position;
				//toJoint = joints [1].position - joints [20].position;


				break;
			}
		case 5: // left elbow with left shoulder and wrist
			{
				return Vector3.Angle(joints[4].position - joints[5].position, joints[6].position - joints[5].position);
				//return Vector3.Angle(joints[8].position - joints[9].position, joints[10].position - joints[9].position);
			}
		case 8: // right shoulder with shoulder and elbow
			{

				fromJoint = joints[9].position - joints[8].position;
				toJoint = joints [1].position - joints [20].position;

				//swap
				//fromJoint = joints [5].position - joints [4].position;
				//toJoint = joints [1].position - joints [20].position;


				break;
			}
		case 9: // right elbow with right shoulder and arm
			{
				return Vector3.Angle(joints[8].position - joints[9].position, joints[10].position - joints[9].position);

				//swap
				//return Vector3.Angle(joints[4].position - joints[5].position, joints[6].position - joints[5].position);
			
			}
		case 12: // left hip with base and knee
			return Vector3.Angle(joints[0].position - joints[12].position, joints[13].position - joints[12].position);
		case 13: // left knwee
			{
				return Vector3.Angle(joints[12].position - joints[13].position, joints[14].position - joints[13].position);
			}
		case 16: // left hip with base and knee
			return Vector3.Angle(joints[0].position - joints[16].position, joints[17].position - joints[16].position);
		case 17: // left knwee
			{
				return Vector3.Angle(joints[16].position - joints[17].position, joints[18].position - joints[17].position);
			}
		}

		//tar = joints [0];

		//Vector3 targetDir = tar.position - own.position;
		//Vector3 forward = own.forward;

		//float angle = Vector3.SignedAngle (targetDir, Vector3.down, Vector3.forward);
		//angle = (angle < 0f) ? (Mathf.Abs(angle) + 180) : angle;

		if (fromJoint == Vector3.zero)
			return 0f;


		float angle = Vector3.SignedAngle (fromJoint, toJoint, Vector3.forward);

		return -angle; 



	}




	private void SaveMatchToFile()
	{
		string s = "";

		for (int i = 0; i < manualMatchFrame.Count - 1; i++) {
			s += manualMatchFrame [i] + ",";

		}
		s += manualMatchFrame [manualMatchFrame.Count - 1];

		SaveUserData ("Match", "key frame", s);

	}



	public void SaveUserData(string section, string name, string value)
	{
		if (!file.Is_Section (section))
			file.Create_Section (section);

		file.Goto_Section (section);

		file.Set_String (name, value);

		file.Save ();
		Debug.Log ("Saved");

	}



}
