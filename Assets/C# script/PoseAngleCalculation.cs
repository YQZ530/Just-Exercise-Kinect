using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PoseAngleCalculation : MonoBehaviour {
    public GameObject Model;
    public string filePath = "PoseRotation";
    public string fileName = "PoseRot";
    public GameObject[] bones;
	// Use this for initialization
	void Start () {
 //       Model.GetComponent<AvatarCreation>().GetBoneList();

 //       bones = new GameObject[Model.GetComponent<AvatarCreation>().bones.Length];
 //       StreamWriter writer = new StreamWriter(Application.dataPath + "/" + filePath + "/" + fileName + ".txt");

 //       bones = Model.GetComponent<AvatarCreation>().bones;
 //       for (int i = 0; i < bones.Length; i++)
 //       {
 //           writer.Write(bones[i].transform.rotation.x+ " "+bones[i].transform.rotation.y + " " +bones[i].transform.rotation.z + " "+ bones[i].transform.rotation.w);

 //           if(i < bones.Length - 1)
 //           {
 //               writer.Write("|");
 //           }
 //       }
 //       writer.WriteLine();
 //       writer.Close();
	}
	
	
}
