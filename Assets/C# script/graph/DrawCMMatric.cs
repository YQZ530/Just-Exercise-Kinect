using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCMMatric : MonoBehaviour {
    IniFile reader;
    
    public Camera jointCam;
    public Camera cmCam;
    [Header("Read/Write")]
    public string filepath = "MCMC_Preset";
    public string readerfilename = "Qcenter";

    //public string screenShotPath = "Matrics";
    public string graphfoldername = "graph";
    [Header("Observation only")]
    public Texture2D heatmapText;
    public Camera camera;
    public int totalJoint = 25;
    public int totalChunk = 10;
    public float maxValue;
    
    void ReadFile()
    {
        reader = new IniFile();
        print(reader.Load_File(Application.dataPath + "/" + filepath + "/" + readerfilename + ".ini"));

    }


    public void PrintCMPicture()
    {
        if (jointCam == null || cmCam == null) { Debug.LogError("Camera not assign"); }
        else { jointCam.enabled = false; cmCam.enabled = true; }
        ReadFile();
        GetMaxValues();
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            {
                ChangeCameraColor(i.ToString() + j.ToString());
                ScreenShotV2(i.ToString() + j.ToString());
            }

        }

        print("Finish drawing");
    }
    void ChangeCameraColor(string sectionname)
    {
        if (!reader.Goto_Section(sectionname)) { print("initfile!!"); }
        
        float value = reader.Get_Float("Center Shift");
        float normalizeValue = value / (0.0000001f + maxValue);
        camera.backgroundColor = Get_Color(normalizeValue);

        //if (normalizeValue == 0f) print(normalizeValue);
        //avatarScript.bones[joint_i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(normalizeValue);


    }
    //get each joint max dist or rotation
    void GetMaxValues()
    {
        maxValue = 0f;
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            {
                //print("go to chunk" + i.ToString() + j.ToString());
                reader.Goto_Section(i.ToString() + j.ToString());
                
                    float value = reader.Get_Float("Center Shift");
                   
                    if (maxValue< value)
                    {

                        maxValue = value;
                    }
                }
            }

        }
    Color Get_Color(float value)
    {
        Color outputColor;
        //if is zero output gray color
        if (value == 0f) { value += 0.001f; }
      
            int index = (int)(value * heatmapText.width - 1);
            outputColor = heatmapText.GetPixel(index, 0);
      
        return outputColor;

    }

    Color Get_ColorV2(float value)
    {
        Color outputColor = new Color(0, 90, 0);
        //if is zero output gray color
        if (value == 0f) { outputColor =Color.red; }
        else
        {
            float index = (value *0.23f); // 0.3 is about green
            outputColor = Color.HSVToRGB(0.05f + index,1f,1f);
        }
        return outputColor;

    }
    public void DrawLegend()
    {
        int totalColor = 25;
        for (int i = 0; i <= totalColor; i++)
        {
            camera.backgroundColor = Get_ColorV2(i / (1.0f* totalColor));
            ScreenShotLegend(i.ToString());
        }
        
    }
    void ScreenShotLegend(string name)
    {
        Rect rect = new Rect(Screen.width * 0f, Screen.height * 0f, 100, 100);
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)(rect.width), (int)(rect.height), 0);
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
        string filename = "C:/Users/z5308/Desktop/graph" + "/" + "legend" + "/" + name + ".png";
        //string filename = Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        camera.backgroundColor = Color.white;
    }
    public void ScreenShotV2(string name)
    {
        Rect rect = new Rect(Screen.width * 0f, Screen.height * 0f, 200, 200);
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)(rect.width), (int)(rect.height), 0);
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
        string filename = "C:/Users/z5308/Dropbox/BackupGraph/" + graphfoldername + name + ".png";

        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        camera.backgroundColor = Color.white;
    }
}

