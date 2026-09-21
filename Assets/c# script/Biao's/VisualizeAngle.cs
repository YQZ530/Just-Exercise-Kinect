using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeAngle : MonoBehaviour {




	public QPoser p;
	public Transform[] joints;

	public float[] jointAngle;

	public string path;

	// Use this for initialization
	void Start () {

		if (path == "") {
			path = "/BiaoLevel/UserResult/keyposeAngle.ini";
		}

		p = GetComponent<QPoser> ();


		joints = p.joints_trans;
		jointAngle = new float[joints.Length];
		SaveAngle ();
	}

	// Update is called once per frame
	void Update () {
		
	} 


	void SaveAngle()
	{
		

		IniFile file = new IniFile ();
		file.Load_File (Application.dataPath + path);

		file.Create_Section (name);

		for (int i = 0; i < 25; i++) {
			jointAngle[i] = calSignedAngle (i);
			file.Set_Float (i.ToString (), jointAngle [i], joints[i].name);
		}
		file.Save ();

		Debug.Log ("keypose " + name + " angle result is saved!");

	}

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
				//tar = joints [20]; own = joints [4];

				fromJoint = joints [5].position - joints [4].position;
				toJoint = joints [1].position - joints [20].position;

				break;
			}
		case 5: // left elbow with left shoulder and wrist
			{
				return Vector3.Angle(joints[4].position - joints[5].position, joints[6].position - joints[5].position);
			}
		case 8: // right shoulder with shoulder and elbow
			{
				//tar = joints [20]; own = joints [8];
				fromJoint = joints[9].position - joints[8].position;
				toJoint = joints [1].position - joints [20].position;

				break;
			}
		case 9: // right elbow with right shoulder and arm
			{
				return Vector3.Angle(joints[8].position - joints[9].position, joints[10].position - joints[9].position);
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

		return angle;



	}


}
 