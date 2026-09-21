using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayUserStudy : MonoBehaviour {
    public UserStudyRecorderReader UserScript;
    public AvatarCreationV2 avatarScript;
    bool start = false;
    public int counter = 0;
    UserStudyRecorderReader.FileData FileDatafile;
    float totalFrame = 0f;
    private void Start()
    {
        UserScript.ReadFilesData();
        FileDatafile = UserScript.GetFileFrameData(0);
        totalFrame = UserScript.GetFileTotalFrame(0);
    }
    public void StartReplay()
    {
        counter = 0;
        FileDatafile =  UserScript.GetFileFrameData(0);
        totalFrame = UserScript.GetFileTotalFrame(0);
        print("file coutn" + FileDatafile.dataFrame);
        start = true;
        
    }

    private void FixedUpdate()
    {
        //counter++;
        //if (start && (counter < totalFrame))
        //{

        //    for (int i = 0; i < 25; i++)
        //    {

        //         avatarScript.bones[i].transform.position = FileDatafile.dataFrame[counter].pos[i];
        //        avatarScript.ReDrawSkeletonLine();
        //        //avatarScript.bones[i].transform.rotation = file[counter].rot[i];
        //    }
        //}

        for (int i = 0; i < 25; i++)
            {
			
                     avatarScript.bones[i].transform.position = FileDatafile.dataFrame[counter].pos[i];
                   avatarScript.ReDrawSkeletonLine();
            //        //avatarScript.bones[i].transform.rotation = file[counter].rot[i];
            }
    }
}
