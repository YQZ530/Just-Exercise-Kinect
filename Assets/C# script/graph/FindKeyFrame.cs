using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindKeyFrame : MonoBehaviour
{

    public class Pair
    {
        public int start;
        public int end;

        public Pair(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    public class Player
    {
        public int id;
        public string level;
        public IniFile userData;
        public int[] EndChunkFrame;
        public int[] StartChunkFrame;
        public int endframeCounter;
        public List<Pair> StartEndPair;
    }

    public GameObject[] prefab;
    public AvatarCreationV2[] modelScripts;
    public Player player;
    public GameObject UserPose;
    public List<Transform> current;
    public AvatarCreationV2 userPose;
   // public GameObject targetPose;
   // public List<Transform> target;

    public string path;
    public int userCount;
    public string[] levels;
    public int selectLevel;

    public bool[] check;
   // public float[] jointAngle;

    public IniFile levelIndexFile;
    public string levelIndexPath = "MCMC_Result/30chunks/";
    public int[] levelIndex;
    //public List<GameObject> modelPose;

   // public IniFile result;
   // public string resultPath;
   // public string resultName;

    void Start()
    {
        //jointAngle = new float[25];

        if (prefab == null) { Debug.LogError("not assign prefabs"); }
        //get bone scripts;
        modelScripts = new AvatarCreationV2[prefab.Length];
        for (int i = 0; i < prefab.Length; i++)
        {
            modelScripts[i] = prefab[i].GetComponent<AvatarCreationV2>();
        }

        current = new List<Transform>();
        get_model_transform_list(current, UserPose.transform);
       
        //load level 
        levelIndexFile = new IniFile();
        if (!levelIndexFile.Load_File(Application.dataPath + levelIndexPath + levels[selectLevel].ToString() + ".ini"))
        { Debug.Log("Unable to reading" + Application.dataPath + levelIndexPath + levels[selectLevel].ToString() + ".ini"); }
        Debug.Log("reading" + Application.dataPath + levelIndexPath + levels[selectLevel].ToString() + ".ini");
        print("GO TO Section " + levelIndexFile.Goto_Section("Level Size"));
        int size = levelIndexFile.Get_Int("levelSize");
        levelIndex = new int[size];
        levelIndexFile.Goto_Section("Level Index");
        levelIndex = levelIndexFile.Get_IntArray("index", levelIndex);

        // result = new IniFile();
        //result.Load_File(resultPath + levels[selectLevel] + resultName);

        //load model pose 
        //modelPose = new List<GameObject>(size);

        //for (int i = 0; i < levelIndex.Length; i++)
        //{
        //    GameObject g = Instantiate(prefab[levelIndex[i]]);
        //    modelPose.Add(g);
        //}
        StartCoroutine(Run());
        //Debug.Log (print_all(levels[selectLevel]));

    }

    private void get_model_transform_list(List<Transform> list, Transform t)
    {
        list.Add(t);
        if (t.childCount == 0)
            return;
        foreach (Transform child in t)
        {
            get_model_transform_list(list, child);
        }
    }
    Player get_user_data(int uID, string level)
    {
        string fileName = get_file_name(uID, level);
        //Debug.Log (fileName);
        // Debug.Log (path);

        Player p = new Player();
        p.id = uID;
        p.level = level;
        p.userData = new IniFile();
        print("USer data" + p.userData.Load_File(path + fileName));
        p.StartEndPair = new List<Pair>(); //initialization
        if (p.userData == null)
        {
            Debug.Log("No " + fileName + " found!");
            return null;
        }



        // get end chunk  key frames
        p.userData.Goto_Section("endchunkkeyframe");
      // p.userData.Goto_Section("keyframe");
        p.EndChunkFrame = new int[1];
        p.EndChunkFrame = p.userData.Get_IntArray("endchunkkeyframe", p.EndChunkFrame);
        //p.EndChunkFrame = p.userData.Get_IntArray("keyframe", p.EndChunkFrame);

        // get start chunk  key frames
        p.userData.Goto_Section("startchunkkeyframe");
        p.StartChunkFrame = new int[1];
        p.StartChunkFrame = p.userData.Get_IntArray("startchunkkeyframe", p.StartChunkFrame);
        // get keyframe count
        p.endframeCounter = 0;
       // print("EndchunkFrame total " + p.EndChunkFrame.Length);
        // print(p.EndChunkFrame.Length + "   " + p.StartChunkFrame.Length);
        return p;
    }

    string get_file_name(int uID, string level)
    {
        string name = "us";
        if (uID < 10)
        {
            name += "00" + uID;
        }
        else
        {
            name += "0" + uID;
        }

        return name + level + ".ini";
    }

    void set_user_data(List<int> frames)
    {

        player.userData.Create_Section("RecalculatedKeyFrame");
        player.userData.Set_IntArray("RecalculatedKeyFrame", frames.ToArray());
        player.userData.Save();


    }

    IEnumerator Run()
    {
        for (int user = 3; user <= 3; user++)
        {
            player = get_user_data(user, levels[selectLevel]);
            Debug.Log("User " + player.id);
            if (player == null)
                continue;
            for (int i = 0; i < player.EndChunkFrame.Length; i++)
            {
                int skeyframe = 0, ekeyframe = 0;
                FindStartFrameEndFrame(ref skeyframe, ref ekeyframe);
                player.StartEndPair.Add(new Pair(skeyframe, ekeyframe));
            }
            
            if(player.StartEndPair.Count > levelIndex.Length)
            {
                Debug.Log("User " + player.id+ "does not have enough data");
                continue;
            }
            // print("EndchunkFrame total " + player.EndChunkFrame.Length + " start pair" + player.StartEndPair.Count);
            //total chunk has to = num of startendPair

            List<int> keyframes = new List<int>();

            keyframes.Add(ReplayUserStudy(0, player.StartEndPair[0].end, levelIndex[0]));
            for (int i = 0; i < player.StartEndPair.Count-1; i++) ////levelIndex[i] is chunk index
            { 
               
                keyframes.Add( ReplayUserStudy(player.StartEndPair[i].end, player.StartEndPair[i+1].end+20, levelIndex[i+1]));
               // yield return new WaitForSeconds(1f);
            }
            set_user_data(keyframes);
           
        }

        // Debug.Log(print_all(levels[selectLevel]));
        yield return null;
    }


    //find the closest start frame for a end chunk frame
    void FindStartFrameEndFrame(ref int skeyframe, ref int ekeyframe)
    {
        ekeyframe = player.EndChunkFrame[player.endframeCounter];
        //find the closest start frame for end chunk frame
        skeyframe = player.StartChunkFrame[0];// start frame
        for (int i = 0; i < player.StartChunkFrame.Length; i++)
        {
            if (player.StartChunkFrame[i] > ekeyframe)
            {
                break;
            }
            skeyframe = player.StartChunkFrame[i];
        }

        //Debug.Log("start " + skeyframe + " end " + ekeyframe);
        player.endframeCounter++;
    }

    int ReplayUserStudy(int startf, int endframe, int curChunkName)
    {

        print("Start and end are " + startf +" " + endframe+ "compare chunk" + curChunkName );
        float minDiff = Mathf.Infinity;
        int keyframe = -1;
        for (int i = startf; i < endframe; i++)
        {
           
            UpdateModel(UserPose, i);
            //print("frame i th " + i);
            float diff = PoseComparsion(ref userPose.bones, ref modelScripts[curChunkName].bones);
            if (diff < minDiff)
            {
                
                minDiff = diff;
                keyframe = i;
              //  print("frame "+ i+ "  diff "  + diff);
            }
            
        }

        return keyframe;
    }
    void UpdateModel(GameObject model, int frame)
    {
        player.userData.Goto_Section(frame.ToString());
        //Debug.Log (player.keyFrame [frame].ToString ());
        //Debug.Log ("U model:"+frame.ToString ());
        update_all_joints(model.transform);
    }
    private void update_all_joints(Transform t)
    {
        int jointID = int.Parse(t.name.Substring(0, 2));
        //Debug.Log (jointID);
        t.rotation = player.userData.Get_Quaternion("r" + jointID, t.rotation);
        t.position = player.userData.Get_Vector3("p" + jointID, t.position);

        if (t.childCount == 0)
            return;

        foreach (Transform child in t)
        {
            update_all_joints(child);
        }
    }
    public float PoseComparsion(ref GameObject[] model, ref GameObject[] v_user)
    //public float PoseComparsion(ref List<Transform> model, ref GameObject[] v_user)
    {
        float angleDiff = 0f;
        float sumPercDiff = 0f;
       // int counter = 0;

        for (int i = 0; i < 25; i++)
        {
           // print("joint " + i);
            if (check[i])
            {
                float modelAngle = calculateAngle(i, model);
                //print("Model Angle" + modelAngle);
                float userAngle = calculateAngle(i, v_user);

                //if they are all positive
                if (modelAngle >= 0f && userAngle >= 0f)
                {
                    angleDiff = Mathf.Abs(modelAngle - userAngle);
                }
                else if (modelAngle < 0f && userAngle < 0f)
                {
                    angleDiff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
                }
                else
                {
                    angleDiff = 360 - Mathf.Abs(modelAngle) - Mathf.Abs(userAngle);
                }

                //if (angleDiff > maxAngleDiff)
                //{
                //    return false;
                //}

                sumPercDiff += angleDiff;
              //  counter++;

            }

        }
        //sumPercDiff = sumPercDiff / (1.0f * counter);

        return sumPercDiff;

    }

    private float calculateAngle(int curjoint, List<Transform> model)
    {
        Vector3 v1, v2;
        if (curjoint == 0)
        {
            // current
            v1 = Vector3.down;
            v2 = (model[1].position - model[0].position).normalized;
        }
        else
        {
            int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curjoint);
            int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curjoint);

           
           
            v1 = model[curParentJoint].transform.position - model[curjoint].transform.position;
            v2 = (model[curChildJoint].transform.position - model[curjoint].transform.position).normalized;
            //v2 = (model[position].GetChild(0).position - model[position].position).normalized;
        }
        return Vector3.Angle(v1, v2);
    }

    private float calculateAngle(int curjoint, GameObject[]  avatar)
    {
        Vector3 v1, v2;
        if (curjoint == 0)
        {
            // current
            v1 = Vector3.down;
            v2 = (avatar[1].transform.position - avatar[0].transform.position).normalized;
        }
        else
        {
            int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curjoint);
            int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curjoint);



            v1 = avatar[curParentJoint].transform.position - avatar[curjoint].transform.position;
            v2 = (avatar[curChildJoint].transform.position - avatar[curjoint].transform.position).normalized;
            //v2 = (model[position].GetChild(0).position - model[position].position).normalized;
        }
        return Vector3.Angle(v1, v2);
    }
    float RotationCostHelperforModel(int curJoint, GameObject[] bones)
    {

        int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
        int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);

        Vector3 root = bones[0].transform.position;
        //local position for current joint parent jont and child joint
        Vector3 curPos = bones[curJoint].transform.position - root;
        Vector3 parentPos = bones[curParentJoint].transform.position - root;
        Vector3 childPos = bones[curChildJoint].transform.position - root;

        /*calculate current joint angle*/
        Vector3 v1 = (parentPos - curPos).normalized;
        Vector3 v2 = (childPos - curPos).normalized;
        Vector3 vn = Vector3.forward;

        if (curJoint == 0) //if is spine
        {
            curPos = bones[curJoint].transform.position - root;
            parentPos = bones[1].transform.position - root;
            childPos = Vector3.down;
            v1 = (parentPos - curPos).normalized;
            v2 = (childPos - curPos).normalized;
            vn = Vector3.forward;
        }

        Vector3 cross = Vector3.Cross(v1, v2);
        float angle = Vector3.Angle(v1, v2);
        if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

        return angle;
    }
    float RotationCostHelperforModel(int curJoint, List<Transform> bones)
    {

        int curParentJoint = (int)GetParentJoint((KinectInterop.JointType)curJoint);
        int curChildJoint = (int)GetNextJoint((KinectInterop.JointType)curJoint);

        Vector3 root = bones[0].position;
        //local position for current joint parent jont and child joint
        Vector3 curPos = bones[curJoint].position - root;
        Vector3 parentPos = bones[curParentJoint].position - root;
        Vector3 childPos = bones[curChildJoint].position - root;

        /*calculate current joint angle*/
        Vector3 v1 = (parentPos - curPos).normalized;
        Vector3 v2 = (childPos - curPos).normalized;
        Vector3 vn = Vector3.forward;

        if (curJoint == 0) //if is spine
        {
            curPos = bones[curJoint].position - root;
            parentPos = bones[1].position - root;
            childPos = Vector3.down;
            v1 = (parentPos - curPos).normalized;
            v2 = (childPos - curPos).normalized;
            vn = Vector3.forward;
        }

        Vector3 cross = Vector3.Cross(v1, v2);
        float angle = Vector3.Angle(v1, v2);
        if (Vector3.Dot(vn, cross) < 0) { angle = -angle; }

        return angle;
    }
    public KinectInterop.JointType GetParentJoint(KinectInterop.JointType joint)
    {
        switch (joint)
        {

            case KinectInterop.JointType.Neck:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.Head:
                return KinectInterop.JointType.Neck;

            case KinectInterop.JointType.SpineShoulder:
                return KinectInterop.JointType.SpineMid; // I add
            case KinectInterop.JointType.SpineMid:
                return KinectInterop.JointType.SpineBase; // I add   

            case KinectInterop.JointType.ShoulderLeft:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.ElbowLeft:
                return KinectInterop.JointType.ShoulderLeft;
            case KinectInterop.JointType.WristLeft: //I add
                return KinectInterop.JointType.ElbowLeft;
            case KinectInterop.JointType.HandLeft: // I changed
                return KinectInterop.JointType.WristLeft;


            case KinectInterop.JointType.HandTipLeft: //I add
                return KinectInterop.JointType.HandLeft;
            case KinectInterop.JointType.ThumbLeft: //I add
                return KinectInterop.JointType.HandLeft;

            case KinectInterop.JointType.ShoulderRight:
                return KinectInterop.JointType.SpineShoulder; // I changed
            case KinectInterop.JointType.ElbowRight:
                return KinectInterop.JointType.ShoulderRight;
            case KinectInterop.JointType.WristRight: //I add
                return KinectInterop.JointType.ElbowRight;
            case KinectInterop.JointType.HandRight:
                return KinectInterop.JointType.WristRight;
            case KinectInterop.JointType.HandTipRight: //I add
                return KinectInterop.JointType.HandRight;
            case KinectInterop.JointType.ThumbRight: //I add
                return KinectInterop.JointType.HandRight;

            case KinectInterop.JointType.HipLeft:
                return KinectInterop.JointType.SpineBase;
            case KinectInterop.JointType.KneeLeft:
                return KinectInterop.JointType.HipLeft;
            case KinectInterop.JointType.AnkleLeft:
                return KinectInterop.JointType.KneeLeft;
            case KinectInterop.JointType.FootLeft: // Iadd
                return KinectInterop.JointType.AnkleLeft;


            case KinectInterop.JointType.HipRight:
                return KinectInterop.JointType.SpineBase;
            case KinectInterop.JointType.KneeRight:
                return KinectInterop.JointType.HipRight;
            case KinectInterop.JointType.AnkleRight:
                return KinectInterop.JointType.KneeRight;
            case KinectInterop.JointType.FootRight: // Iadd
                return KinectInterop.JointType.AnkleRight;
        }

        return joint;
    }

    public KinectInterop.JointType GetNextJoint(KinectInterop.JointType joint)
    {
        switch (joint)
        {
            case KinectInterop.JointType.SpineBase:
                return KinectInterop.JointType.Neck;
            case KinectInterop.JointType.Neck:
                return KinectInterop.JointType.Head;

            case KinectInterop.JointType.ShoulderLeft:
                return KinectInterop.JointType.ElbowLeft;
            case KinectInterop.JointType.ElbowLeft:
                return KinectInterop.JointType.HandLeft;

            case KinectInterop.JointType.ShoulderRight:
                return KinectInterop.JointType.ElbowRight;
            case KinectInterop.JointType.ElbowRight:
                return KinectInterop.JointType.HandRight;

            case KinectInterop.JointType.HipLeft:
                return KinectInterop.JointType.KneeLeft;
            case KinectInterop.JointType.KneeLeft:
                return KinectInterop.JointType.AnkleLeft;

            case KinectInterop.JointType.HipRight:
                return KinectInterop.JointType.KneeRight;
            case KinectInterop.JointType.KneeRight:
                return KinectInterop.JointType.AnkleRight;
        }

        return joint;  // end joint
    }



}

