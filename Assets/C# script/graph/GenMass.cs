using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenMass : MonoBehaviour {
	public string path,name;
    public int totalchunk = 10;
	public int cnt;
	public GameObject[] bones;
	public AvatarCreationV2 avatar;
	public DrawJointColor color;
	public GameObject arrow;
	public GameObject target;
    public string distancepath = "/massresult/Dist/";
    public string rotationpath = "/massresult/Rot/";
    public string cmpath = "/massresult/cm/";

    public IniFile centerIni;
    public string readCOMPath = "/Quincy /table/";
    public string COMFilename = "Qcenter";
	public float maxCen;

    
	// Use this for initialization
	void Start () {
		//path = Application.dataPath + path + name + cnt + ".txt";
		//Capture();
	}
	
	// Update is called once per frame
	void Update () {
		arrow.transform.LookAt (target.transform);
	}
	public void GetBones()
	{
		AvatarCreationV2 ac = GetComponent<AvatarCreationV2> ();
		bones = ac.bones;

	}

	public void GetPose()
	{
		string currentP = Application.dataPath + path + name + cnt + ".txt";
		StreamReader reader = new StreamReader (currentP);
		currentP = reader.ReadLine ();
		string[] data = currentP.Split(new char[]{'|',' '});

		Vector3[] pos = new Vector3[25];

		for (int i = 0; i < data.Length; i += 3) {
			float x = float.Parse (data [i]);
			float y = float.Parse (data [i+1]);
			float z = float.Parse (data [i+2]);

			pos [i / 3] = new Vector3 (x, y, z);
		}

		for (int i = 0 ; i <25; i++) {
			bones [i].transform.position = pos [i];
		}

        bones[1].transform.position += new Vector3(0, 0.072f, 0);
        avatar.ReDrawSkeletonLine ();
	}

    

    public void CaptureDist()
    {
        color.SetupTableColor();

        for (int i = 0; i < totalchunk; i++)
        {
            for (int j = 0; j < totalchunk; j++)
            {
                cnt = j;
                GetPose();
                GetPose();

                color.ChangeJointColor(i.ToString() + j.ToString());
                avatar.ReDrawSkeletonLine();
                
                color.ScreenShotV2(distancepath+i + "" + j);
            }

        }
    }
    public void CaptureRot()
    {
        color.SetupTableColor();

        for (int i = 0; i < totalchunk; i++)
        {
            for (int j = 0; j < totalchunk; j++)
            {
                cnt = j;
                GetPose();
                GetPose();

                color.ChangeJointColor(i.ToString() + j.ToString());
                
           
                color.ScreenShotV2(rotationpath+i + "" + j);
            }

        }
    }
	public void Capture()
	{
		color.SetupTableColor ();
		
		for (int i = 0; i < totalchunk; i++) {
			for (int j = 0; j < totalchunk; j++) {
				cnt = j;
				GetPose ();
				GetPose ();
                
                color.ChangeJointColor (i.ToString () + j.ToString ());
                avatar.ReDrawSkeletonLine();
                color.ScreenShotV2 ( i+""+j );
			}

		}
	}

	public void CaptureCOM()
	{
		ReadCOM ();

		for (int i = 0; i < totalchunk; i++) {
			for (int j = 0; j < totalchunk; j++) {
				cnt = j;
				GetPose ();
				GetPose ();
				GetCenterLength (i, j);
				GetCenterDirection (i, j);
                avatar.ReDrawSkeletonLine();
                color.ScreenShotV2 ( cmpath+i.ToString()+""+j.ToString());
			}
		
		}

	}

	public void ReadCOM()
	{
		centerIni = new IniFile ();
		centerIni.Load_File (Application.dataPath + readCOMPath+COMFilename+".ini");
		for (int i = 0; i < totalchunk; i++) {
			for (int j = 0; j < totalchunk; j++) {
				if (!centerIni.Goto_Section (i.ToString () + j.ToString ())) {
					Debug.Log ("no ij");
					return;
				}
				centerIni.Goto_Section (i.ToString () + j.ToString ());
				float cen = centerIni.Get_Float ("Center Shift");
				if (cen > maxCen)
					maxCen = cen;
			
			}
		
		}
		Debug.Log ("MAX shift is " + maxCen);
	}

	public void GetCenterLength(int i, int j)
	{
		if (!centerIni.Goto_Section (i.ToString () + j.ToString ())) {
			Debug.Log ("no ij");
			return;
		}
		centerIni.Goto_Section (i.ToString () + j.ToString ());
		float cen = centerIni.Get_Float ("Center Shift");
		MeshRenderer[] arrows = arrow.GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer mr in arrows) {
			mr.sharedMaterial.color = color.Get_Color (cen/maxCen);
		}


	}
	public void GetCenterDirection(int i, int j)
	{
		Vector3 start = new Vector3 ();
		Vector3 end = new Vector3();
		start = centerIni.Get_Vector3 ("start",start);
		end = centerIni.Get_Vector3 ("target", end);
		arrow.transform.position = start;
		arrow.transform.LookAt(end);



	}


	void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		
	}






}
