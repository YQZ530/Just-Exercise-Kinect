using System.Collections;
using UnityEngine.UI;

using UnityEngine;

public class CalculatePoseDiff : MonoBehaviour {
   
    IniFile AllPosesFile;
    IniFile importantJointFile;
    IniFile OutPutFile;
    public float[] scoringJoint;

    public string outputfilename = "level";
    public AvatarCreationV2 userScript;
    public int curChunk = 1;
    public Text debugText;
    public float maxPerDiff = 0.1f;
    public static bool cancompare = false;
    // Use this for initialization
    void Start () {
        //load affected joint to score
        LoadImportantJointFile();
        //load models infor
        LoadModelsJointDataFile();
        scoringJoint = new float[25];

    }
    public void Iscompare()
    {
        cancompare = !cancompare;
    }
    // Update is called once per frame
    void Update () {
        if (cancompare)
        {
            debugText.text = "";

            InstanceCalculatePoseDifferent();
        }
      

    }

    void LoadImportantJointFile()
    {
        importantJointFile = new IniFile();
        if (!importantJointFile.Load_File(Application.dataPath + "/GamePlay/AffectJointsforScoring.ini"))
        {
            Debug.LogError("cannot open inifile path " + Application.dataPath + "/GamePlay/AffectJointsforScoring.ini");

        }
       
    }
    void LoadModelsJointDataFile()
    {
        AllPosesFile = new IniFile();
        if (!AllPosesFile.Load_File(Application.dataPath + "/BiaoLevel/UserResult/keyposeAngle.ini"))
        {
            Debug.LogError("cannot open inifile path " + Application.dataPath + "/BiaoLevel/UserResult/keyposeAngle.ini");

        }

    }
    bool LoadImportantJoints(int chunk)
    {
        importantJointFile.Goto_Section(chunk.ToString());
        scoringJoint = importantJointFile.Get_FloatArray("score", scoringJoint);
        return true;
    }
    float LoadModelInfo(int chunk, ref int joint)
    {
        float jointAngle = 0f;

        if (!AllPosesFile.Goto_Section(chunk.ToString()))
        {

            Debug.LogError("cannot go to section");
        };
        jointAngle = AllPosesFile.Get_Float(joint.ToString());
        return jointAngle;
    }
   
    void CalculatePoseDifferent(int poseA, int PoseB)
    {

    }

    //instant compare two 
    void InstanceCalculatePoseDifferent()
    {
        LoadImportantJoints(curChunk);

        float sumPercDiff = 0f;
        for (int i = 0; i < 25; i++)
        {
            if (scoringJoint[i] != 0f)
            {
                //print(i);
                float userAngle = SkeletonComparison.RotationCostHelperforGameObjType(i, userScript.bones);
                float modelAngle = LoadModelInfo(curChunk, ref i);
                float angleDiff = 0f;
                InstanceCalculatePoseDifferentHelper(modelAngle, userAngle, ref angleDiff);
                debugText.text += "joint " + i + "  model angle" + Mathf.RoundToInt(modelAngle) + "                user Angle " + Mathf.RoundToInt(userAngle);
                debugText.text += "   angle diff " + Mathf.RoundToInt(angleDiff) +"  max anglediff set "+ (18 /scoringJoint[i]) +"\n";
                //if (angleDiff > maxPercDiff)
                if ((angleDiff / 180f) * scoringJoint[i] > maxPerDiff) //if angle one of important joint are > than the setting, return false
                {
                    //debugText.text += "joint " + i + "  model angle" + modelAngle + "                user Angle " + userAngle;
                    // debugText.text += "   angle diff " + angleDiff + "\n";

                }
                angleDiff /= 180f;
                sumPercDiff += (angleDiff);

               
            }

        }
        debugText.text += "sum percent diff " + sumPercDiff;

    }

    void InstanceCalculatePoseDifferentHelper(float modelAngle, float userAngle, ref float anglediff)
    {
        //if they are all positive
        if (modelAngle >= 0f && userAngle >= 0f)
        {
            anglediff = Mathf.Abs(modelAngle - userAngle);
        }
        else if (modelAngle < 0f && userAngle < 0f)
        {
            anglediff = Mathf.Abs(Mathf.Abs(modelAngle) - Mathf.Abs(userAngle));
        }
        else //one positive one negative
        {
           
            float mAngle = Mathf.Abs(modelAngle);
            float uAngle = Mathf.Abs(userAngle);
            float r1 = 360 - mAngle - uAngle;
            float r2 = mAngle + uAngle;
            anglediff = Mathf.Min(r1, r2);

        }



       
    }
}
