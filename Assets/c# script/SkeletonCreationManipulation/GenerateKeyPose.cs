using System;
using System.IO;
using UnityEngine;

public class GenerateKeyPose : MonoBehaviour {
    public string[] poseName;
    public AvatarCreationV2 creationV2Script;
    //public string keyposesPath = "UserKeyPose";
    public int file_i;
	void GetPoseName()
    {
        poseName = new string[] {  "keypose0", "keypose1", "keypose2", "keypose3", "keypose4", "keypose5", "keypose6", "keypose7", "keypose8", "keypose9" };

    }
    public void GenerateAllKeyPoses()
    {
      //  GetPoseName();
        //for (int i = 0; i < poseName.Length; i++)
        //{
        //    creationV2Script.rotFileName = poseName[i];
        //    creationV2Script.LoadBonesRotation();
        //    OutputUserKeyPose();
        //}
        OutputUserKeyPose();

    }
    void OutputUserKeyPose()
    {
        StreamWriter writer = new StreamWriter(Application.dataPath + "/" + creationV2Script.SkeletonfilePath + "/" +  "MirrorPose" + file_i.ToString() + ".txt", true);
        //StreamWriter writer = new StreamWriter(Application.dataPath + "/" + creationV2Script.SkeletonfilePath + "/" + keyposesPath + "/" + creationV2Script.SkeletonfileName + creationV2Script.rotFileName+ ".txt", true);
        Vector3 offset = this.transform.position - creationV2Script.txtinitialPosition; //offset
        

        for (int i = 0; i < creationV2Script.bones.Length; i++)
        {
          Vector3 v  = creationV2Script.bones[i].transform.position - offset;
            writer.Write(v.x + " " + v.y + " " + v.z);
            if (i < creationV2Script.bones.Length - 1)
            {
                writer.Write("|");
            }
        }
        writer.WriteLine();

        writer.Close();
        print("save to " + Application.dataPath + "/" + creationV2Script.SkeletonfilePath + "/" + "MirrorPose" + file_i.ToString() + ".txt");
      
        //StreamWriter writer = new StreamWriter(Application.dataPath + "/" + creationV2Script.SkeletonfilePath + "/" + keyp
    }
}
