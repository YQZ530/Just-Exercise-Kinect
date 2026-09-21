using System.Collections;
using UnityEngine.UI;

using UnityEngine;

public class DrawErrorJoint : MonoBehaviour {
    public string defaultLevel = "mcmcdefault";
    public string mediumrotlevel = "medianrot";
    public string highrotLevel = "highrot";
    public string mediumcmlevel = "mediumcm";
    public string highcmLevel = "highcm";
    public string graphfoldername = "errorfigure/";
    public int totalJoint = 25;
    public string filepath = "/MCMC_result/30chunks/";
    public Texture2D heatmapText;
    public AvatarCreationV2 avatarScript;
    public Color outputColor;
    public Camera camera;
    public string outputname = "b";
    public void DrawDefaultRot()
    {

        IniFile reader = new IniFile();
        if (!reader.Load_File(Application.dataPath + filepath + defaultLevel + ".ini"))
        {
            print(Application.dataPath + filepath + defaultLevel + ".ini");
        }
        else
        {
            print("reading " + Application.dataPath + filepath + defaultLevel + ".ini");
        }
        if (!reader.Goto_Section("MCMC Rot targets")) { print("cannot go to section"); };
        float[] targetrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            targetrot[i] = reader.Get_Float(i.ToString());
        }

        if (!reader.Goto_Section("Sum of Rotation for each joint")) { print("cannot go to section"); };
        float[] resultrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            resultrot[i] = reader.Get_Float(i.ToString());
        }

        for (int i = 0; i < totalJoint; i++)
        {
            float r = Mathf.Abs(targetrot[i] - resultrot[i]);
            if (targetrot[i] == 0f) { r = 0f; } //avoid zero division
            else { r /= targetrot[i]; }


            avatarScript.bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(r);
        }
        ScreenShotV2(outputname+"ErrdefaultRot");

    }
    public void DrawMediumRot()
    {

        IniFile reader = new IniFile();
        if (!reader.Load_File(Application.dataPath + filepath + mediumrotlevel + ".ini")) {
            print(Application.dataPath + filepath + mediumrotlevel + ".ini");
        }
        else{
            print("reading " + Application.dataPath + filepath + mediumrotlevel + ".ini");
        }
        if (!reader.Goto_Section("MCMC Rot targets")) { print("cannot go to section"); };
        float[] targetrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            targetrot[i] = reader.Get_Float(i.ToString());
        }

        if (!reader.Goto_Section("Sum of Rotation for each joint")) { print("cannot go to section"); };
        float[] resultrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            resultrot[i] = reader.Get_Float(i.ToString());
        }

        for (int i = 0; i < totalJoint; i++)
        {
            float r = Mathf.Abs(targetrot[i] - resultrot[i]);
            if (targetrot[i] == 0f) { r = 0f; } //avoid zero division
            else { r /= targetrot[i]; }

            // print(i);
            avatarScript.bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(r);
        }
        ScreenShotV2(outputname+"ErrmedianRot");

    }


    public void DrawHighRot()
    {

        IniFile reader = new IniFile();
        if (!reader.Load_File(Application.dataPath + filepath + highrotLevel + ".ini"))
        {
            print(Application.dataPath + filepath + highrotLevel + ".ini");
        }
        else
        {
            print("reading " + Application.dataPath + filepath + highrotLevel + ".ini");
        }
        if (!reader.Goto_Section("MCMC Rot targets")) { print("cannot go to section"); };
        float[] targetrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            targetrot[i] = reader.Get_Float(i.ToString());
        }

        if (!reader.Goto_Section("Sum of Rotation for each joint")) { print("cannot go to section"); };
        float[] resultrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            resultrot[i] = reader.Get_Float(i.ToString());
        }

        for (int i = 0; i < totalJoint; i++)
        {
            float r = Mathf.Abs(targetrot[i] - resultrot[i]);
            if (targetrot[i] == 0f) { r = 0f; } //avoid zero division
            else { r /= targetrot[i]; }


            avatarScript.bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(r);
        }
        ScreenShotV2(outputname+"ErrhighRot");

    }

    public void DrawMediumCM()
    {

        IniFile reader = new IniFile();
        if (!reader.Load_File(Application.dataPath + filepath + mediumcmlevel + ".ini"))
        {
            print(Application.dataPath + filepath + mediumcmlevel + ".ini");
        }
        else
        {
            print("reading " + Application.dataPath + filepath + mediumcmlevel + ".ini");
        }
        if (!reader.Goto_Section("MCMC Rot targets")) { print("cannot go to section"); };
        float[] targetrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            targetrot[i] = reader.Get_Float(i.ToString());
        }

        if (!reader.Goto_Section("Sum of Rotation for each joint")) { print("cannot go to section"); };
        float[] resultrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            resultrot[i] = reader.Get_Float(i.ToString());
        }

        for (int i = 0; i < totalJoint; i++)
        {
            float r = Mathf.Abs(targetrot[i] - resultrot[i]);
            if (targetrot[i] == 0f) { r = 0f; } //avoid zero division
            else { r /= targetrot[i]; }


            avatarScript.bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(r);
        }
        ScreenShotV2(outputname+"Errmediumcm");

    }

    public void DrawHighCM()
    {

        IniFile reader = new IniFile();
        if (!reader.Load_File(Application.dataPath + filepath + highcmLevel + ".ini"))
        {
            print(Application.dataPath + filepath + highcmLevel + ".ini");
        }
        else
        {
            print("reading " + Application.dataPath + filepath + highcmLevel + ".ini");
        }
        if (!reader.Goto_Section("MCMC Rot targets")) { print("cannot go to section"); };
        float[] targetrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            targetrot[i] = reader.Get_Float(i.ToString());
        }

        if (!reader.Goto_Section("Sum of Rotation for each joint")) { print("cannot go to section"); };
        float[] resultrot = new float[25];
        for (int i = 0; i < totalJoint; i++)
        {
            resultrot[i] = reader.Get_Float(i.ToString());
        }

        for (int i = 0; i < totalJoint; i++)
        {
            float r = Mathf.Abs(targetrot[i] - resultrot[i]);
            if (targetrot[i] == 0f) { r = 0f; } //avoid zero division
            else { r /= targetrot[i]; }
            print(r);
            avatarScript.bones[i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(r);
        }
        ScreenShotV2(outputname+"Errhighcm");

    }
     Color Get_Color(float value)
    {

        int index = 0;
        if (value == 0f) { index = 0; }
        else
        {
            index = (int)(value * heatmapText.width - 1);

        }
        outputColor = heatmapText.GetPixel(index, 0);
        return outputColor;


    }

     void ScreenShotV2(string name)
    {
        Screen.SetResolution(400, 400, false);

        Rect rect = new Rect(Screen.width * 0f, Screen.height * 0f, 400f, 400f);
        // 创建一个RenderTexture对象  
        // 24 才行，0是没有深度的
        // asdkmhsajfkhbasdkjhfgasiehkrjg
        RenderTexture rt = new RenderTexture((int)(rect.width), (int)(rect.height), 24);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        camera.targetTexture = rt;
        camera.Render();

        // 激活这个rt, 并从中中读取像素。  
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)(rect.width), (int)(rect.height), TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;  
        RenderTexture.active = null; // JC: added to avoid errors  
        DestroyImmediate(rt);
        // 最后将这些纹理数据，成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        //string filename = "C:/Users/z5308/Desktop/graph" + "/" + screenShotPath + "/" + name + ".png";
        //string filename = "C:/Users/KISTIST/Desktop/graph" + "/" + screenShotPath + "/" + name + ".png";
        //string filename = Application.dataPath + name;

        //string filename = "D:/Users/Biao/Dropbox/Dropbox/UnityProject/YQ Cloud Poses Screenshot/IK Demo/Capture/"+name;

        string filename = "C:/Users/z5308/Dropbox/level2Graph/" + graphfoldername + name + ".png";

        //string filename = Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));


    }

}
