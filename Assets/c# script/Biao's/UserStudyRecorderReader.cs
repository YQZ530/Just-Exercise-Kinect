using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserStudyRecorderReader : MonoBehaviour
{
    enum State { recording, stop, ready }
    State curState = State.ready;
    [SerializeField]
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
    [Serializable]
    public class FileData
    {
        public float totalFrame = 0;
        public List<Frame> dataFrame;
        public FileData()
        {
            dataFrame = new List<Frame>();
        }
        public List<Frame> GetDataFrame()
        {
            return dataFrame;
        }


    }
    List<Frame> dataFrame;
    KinectManager manager;
    long userId;
    
    public string levelName;
    [Header("For print out")]
    public string OutputfileName = "us001";
    [Header("For reading input")]
    public string InputfileName = "us001";
    public int totalJoint = 25;


    [Header("For Reading")]
    StreamReader reader;
    public string Filedatapath = "Assets/SmallUserStudy/";
    public List<string> filesPath;
    public int totalFiles = 10; //number of file that reader read and store data
    List<FileData> resultDataFiles;
    int frameCounter = 0;
    List<int> keyFrameCounter; // record key frame that user match the avatar pose
    List<int> startChunkKeyFrameCounter;
    List<int> endChunkKeyFrameCounter;

    int startChunkKeyFrame; // record key frame that user match the avatar pose
    int endChunkKeyFrame;
    public SkeletonManKinectController skeletonScript;
    
    // Use this for initialization
    void Start()
    {
        dataFrame = new List<Frame>();
        keyFrameCounter = new List<int>();
        startChunkKeyFrameCounter = new List<int>();
        endChunkKeyFrameCounter = new List<int>();
        startChunkKeyFrame = 0;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
    
        if (curState == State.recording)
        {
            
            Record();
     
        }
        
    }

    /*Control by the game; when to start recording*/
    public void StartRecording()
    {
        curState = State.recording;
    }
    /*StopRecording and Given levelname for output */
    public void StopRecording(string levelname)
    {

        Debug.Log("Stop Recording, ready to output result");
        curState = State.stop;
        levelName = levelname;
       
        OutputData();

        //clear lists of data
        for (int i = 0; i < dataFrame.Count; i++)
        {
                dataFrame[i].kinectPos.Clear();
                dataFrame[i].kinectPos.TrimExcess();
                dataFrame[i].mrot.Clear();
            dataFrame[i].mrot.TrimExcess();
            dataFrame[i].rot.Clear();
            dataFrame[i].rot.TrimExcess();
            dataFrame[i].pos.Clear();
            dataFrame[i].pos.TrimExcess();
        }
         dataFrame.Clear();
         dataFrame.TrimExcess();
         keyFrameCounter.Clear();
        keyFrameCounter.TrimExcess();
        startChunkKeyFrameCounter.Clear();
        startChunkKeyFrameCounter.TrimExcess();
        endChunkKeyFrameCounter.Clear();
        endChunkKeyFrameCounter.TrimExcess();


    }

    public void RecordKeyFrame()
    {
        //keyFrameCounter.Add(startChunkKeyFrame + i);
        keyFrameCounter.Add(dataFrame.Count);
    }
    public void RecordChunkStartFrame()
    {

        startChunkKeyFrame= dataFrame.Count;
        startChunkKeyFrameCounter.Add(startChunkKeyFrame);
    }
    public void RecordChunkEndFrame()
    {
        endChunkKeyFrame = dataFrame.Count;
      
        endChunkKeyFrameCounter.Add(endChunkKeyFrame);
    }
    public void OutputData()
    {
        IniFile outputfile = new IniFile();
  
        for (int i = 0; i < dataFrame.Count; i++)
        { //for all frames
            outputfile.Create_Section(i.ToString());
            outputfile.Set_Vector3("bodypose", dataFrame[i].bodypos);
            outputfile.Set_Quaternion("bodyRot", dataFrame[i].bodyrot);
            for (int j = 0; j < totalJoint; j++)
            {
                outputfile.Set_Vector3("p" + j.ToString(), dataFrame[i].pos[j]);
                outputfile.Set_Quaternion("r" + j.ToString(), dataFrame[i].rot[j]);
                outputfile.Set_Quaternion("mirrorR" + j.ToString(), dataFrame[i].mrot[j]);
                outputfile.Set_Vector3("kpos" + j.ToString(), dataFrame[i].kinectPos[j]);

            }
        }

        outputfile.Create_Section("TotalFrame");
        outputfile.Set_Float("TotalFrame", dataFrame.Count);
        //outputfile.Create_Section("angleDifference", "angle different for each frame");
        //outputfile.Set_FloatArray("angleDifference", skeletonScript.score.ToArray());
        outputfile.Create_Section("keyframe", "key frame when user match the pose, int array");
        outputfile.Set_IntArray("keyframe", keyFrameCounter.ToArray());

        outputfile.Create_Section("startchunkkeyframe", "key frame when user start a new chunk, int array");
        outputfile.Set_IntArray("startchunkkeyframe", startChunkKeyFrameCounter.ToArray());

        outputfile.Create_Section("endchunkkeyframe", "key frame when user end a chunk, int array");
        outputfile.Set_IntArray("endchunkkeyframe", endChunkKeyFrameCounter.ToArray());

        outputfile.SaveTo(Application.dataPath + "/UserStudy/" + OutputfileName + levelName+ ".ini");
    }

    public void Record()
    {
        manager = KinectManager.Instance;
        /*check if kinect detect a user*/
        if (!manager && !manager.IsInitialized()) { /*Debug.Log("Cannot find manager!! Cannot Record!!");*/ return; }
        if (!manager.IsUserDetected()) {/* Debug.Log("Cannot find user!!  Cannot Record!!");*/ return; }

        userId = manager.GetPrimaryUserID();
        Frame oneframe = new Frame(totalJoint);
        oneframe.bodypos = manager.GetUserPosition(userId);
        oneframe.bodyrot = manager.GetUserOrientation(userId,true);

        //save all joint
        for (int i = 0; i < totalJoint; i++)
        {
            oneframe.pos.Add(manager.GetJointPosition(userId, i));
            oneframe.rot.Add(manager.GetJointOrientation(userId, i, true)); //non mirror rotation
            oneframe.mrot.Add(manager.GetJointOrientation(userId, i, false)); //mirror rotation
            oneframe.kinectPos.Add(manager.GetJointKinectPosition(userId, i));
        }

        dataFrame.Add(oneframe);
    }

    /* For reading part */
    public void ReadFilesData()
    {
        GetFilePath();
        resultDataFiles = new List<FileData>(totalFiles);
        IniFile reader = new IniFile();

        ReadAFile(ref reader, 0);

        
        //for (int i = 0; i < filesPath.Count; i++)
        //{
        //    reader = new StreamReader(filesPath[i], true);
        //    ReadAFile(ref reader, i);
        //    reader.Close();
        //}

    }

    //List out all file path that need to read
    void GetFilePath()
    {
        filesPath = new List<string>();
        //InputfileName+ i.ToString() + levelName = us001Easy, us002Easy etc
        //for (int i = 0; i < totalFiles; i++)
        //{
            print("getting path" + Filedatapath + InputfileName + levelName + ".ini");
            filesPath.Add(Filedatapath + InputfileName + levelName + ".ini");

        //}


    }
    bool ReadAFile(ref IniFile rd, int ithFile)
    {
        List<Frame> onefileDataFrame = new List<Frame>();
        FileData file = new FileData();
        string path = filesPath[ithFile];
       print( rd.Load_File(path));
        
        rd.Goto_Section("TotalFrame");
        float totalFrame = rd.Get_Float("TotalFrame");
        file.totalFrame = totalFrame;
        print(file.totalFrame);
        for (int i = 0; i < totalFrame; i++)
        {
            rd.Goto_Section(i.ToString());
            Frame f = new Frame(25);
           
           
            for (int j = 0; j < totalJoint; j++)
            {
                Vector3 v = rd.Get_Vector3("p" + j.ToString(), Vector3.zero);
                Quaternion q = rd.Get_Quaternion("r" + j.ToString(), Quaternion.identity);
                Quaternion mirrorq = rd.Get_Quaternion("mirrorR" + j.ToString(), Quaternion.identity);
                f.pos.Add(v);
                f.rot.Add(q); 
                f.mrot.Add(mirrorq);
                //print(v);
                //rot ...

            }
            onefileDataFrame.Add(f);
        }
     
        file.dataFrame = onefileDataFrame;
        
        resultDataFiles.Add(file);
        Debug.Log("finish reading file" + ithFile.ToString());

        return true;
    }

    bool ReadALine(ref StreamReader rd, ref List<Frame> dataframe)
    {
        string line = rd.ReadLine();
        if (line == null) { print("EOF"); return false; }


        Frame oneFrame = new Frame(totalJoint);

        string[] array = line.Split('|');
        for (int i = 0; i < totalJoint; i++)
        {

            string[] temp = array[i].Split(' ');
            float x = 0f, y = 0f, z = 0f;
            float.TryParse(temp[0], out x);
            float.TryParse(temp[1], out y);
            float.TryParse(temp[2], out z);

            oneFrame.pos.Add(new Vector3(x, y, z));

        }
        dataframe.Add(oneFrame);
        return true;
    }

    /*For evaluationi*/
    public FileData GetFileFrameData(int i)
    {
        print("total frame " + resultDataFiles[i].totalFrame);
        print(resultDataFiles[i].dataFrame.Count);
        print(resultDataFiles[i].dataFrame[0].pos[24].ToString());
        return resultDataFiles[i];
    }
    public float GetFileTotalFrame(int i)
    {
        return resultDataFiles[i].totalFrame;
    }

    public string GetLevelName()
    {
        return levelName;
    }

    public string GetDataPath()
    {
        return Filedatapath;
    }
}


