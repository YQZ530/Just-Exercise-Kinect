using UnityEngine;


//DrawJointColor base on distance table and rotation table
public class DrawJointColor : MonoBehaviour
{

    public AvatarCreationV2 avatarScript;
    public Camera camera;
	public Camera[] cams;
    public Camera jointCam;
    public Camera cmCam;
    public Texture2D heatmapText;
    [Range(0, 1f)]
    public float givenValue = 0f;
    public Color outputColor;
    [Header("Read/Write")]
    public string filepath = "MCMC_Preset";
    public string filename = "dist";
    public string graphfoldername = "graph";
    public string screenShotPath = "Matrics";

    public string distancematricpath = "/distance_matrics/";
    public string rotationmatricpath = "/rotation_matrics/";

    public int totalJoint = 25;
    public int totalChunk = 10;
    [Header("Observation only")]
    public float[] maxValue;
    IniFile reader;

	public void ScreenShotV2_old(string name)
	{
		Screen.SetResolution(400, 400, false);

		Rect rect = new Rect(Screen.width * 0f, Screen.height * 0f, 400f, 400f);
		// 创建一个RenderTexture对象  


		Texture2D screenShot = new Texture2D((int)(rect.width), (int)(rect.height), TextureFormat.RGB24, false);

		for (int i = 0; i < cams.Length; i++) {

			RenderTexture rt = new RenderTexture ((int)(rect.width), (int)(rect.height), 0);
			// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
			cams[i].targetTexture = rt;
			cams[i].Render ();

			// 激活这个rt, 并从中中读取像素。  
			RenderTexture.active = rt;
			screenShot.ReadPixels (rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
			screenShot.Apply ();

			// 重置相关参数，以使用camera继续在屏幕上显示  
			cams[i].targetTexture = null;
			//ps: camera2.targetTexture = null;  
			RenderTexture.active = null; // JC: added to avoid errors  
			DestroyImmediate (rt);
		}
		// 最后将这些纹理数据，成一个png图片文件  
		byte[] bytes = screenShot.EncodeToPNG();
		string filename = "D:/Users/Biao/Dropbox/Dropbox/UnityProject/YQ Cloud Poses Screenshot/IK Demo/Capture/"+name;
		//string filename = Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png";
		System.IO.File.WriteAllBytes(filename, bytes);
		Debug.Log(string.Format("截屏了一张照片: {0}", filename));


	}


    private void Update()
    {
        Get_Color(givenValue); 
    }

    //value 0-1
    public Color Get_Color(float value)
    {
        ////if is zero output gray color
        //if (value == 0f) { outputColor = Color.grey;  }
        //else
        //{
        //    int index = (int)(value * heatmapText.width - 1);
        //    outputColor = heatmapText.GetPixel(index, 0);
        //}
        //return outputColor;

        int index = 0;
        if (value == 0f) {  index = 0; }
        else
        {
             index = (int)(value * heatmapText.width - 1);
           
        }
        outputColor = heatmapText.GetPixel(index, 0);
        return outputColor;


    }
    public void DisplayJoint()
    {
        ReadFile();
        GetMaxValues();
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            {
                ChangeJointColor(i.ToString()+j.ToString());
                ScreenShotV2(i.ToString() + j.ToString());
            }
            
        }
       
        print("Finish drawing");
    }
    public void DrawDistJoint()
    {
        if (jointCam == null || cmCam == null) { Debug.LogError("Camera not assign"); }
        else { jointCam.enabled = true; cmCam.enabled = false; }
        ReadFile();
        GetMaxValues();
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            { 
                ChangeJointColor(i.ToString() + j.ToString());
                
                ScreenShotV2(distancematricpath+i.ToString() + j.ToString());
            }

        }

        print("Finish drawing");
    }
    public void DrawRotJoint()
    {
        if (jointCam == null || cmCam == null) { Debug.LogError("Camera not assign"); }
        else { jointCam.enabled = true; cmCam.enabled = false; }
        ReadFile();
        GetMaxValues();
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            {
                ChangeJointColor(i.ToString() + j.ToString());
                ScreenShotV2(rotationmatricpath + i.ToString() + j.ToString());
            }

        }

        print("Finish drawing");
    }

    public void SetupTableColor()
	{
		ReadFile();
		GetMaxValues();
	}

    public void ChangeJointColor(string sectionname)
    {   
        
        reader.Goto_Section(sectionname);
        for (int i = 0; i < totalJoint; i++)
        {
            float value = reader.Get_Float(i.ToString());
            ChangeJointColorHelper(i, value);
        }
    }
    void ChangeJointColorHelper(int joint_i, float value)
    {
        
        float normalizeValue = value / (0.0000001f+maxValue[joint_i]);
      //  print(normalizeValue);
       // if (joint_i == 20) print(normalizeValue);
         avatarScript.bones[joint_i].gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = Get_Color(normalizeValue);
        //avatarScript.bones[joint_i].gameObject.GetComponent<MeshRenderer>().material.color = Get_Color(normalizeValue);
    }

    void ReadFile()
    {
        reader = new IniFile();
        reader.Load_File(Application.dataPath + "/" + filepath + "/" + filename + ".ini");

        print("reading " + Application.dataPath + "/" + filepath + "/" + filename + ".ini"); 
    }

    //get each joint max dist or rotation
    void GetMaxValues()
    {
        maxValue = new float[totalJoint];
        for (int i = 0; i < totalChunk; i++)
        {
            for (int j = 0; j < totalChunk; j++)
            {
              //   if (j == 10) print(i + "   " + j);
                //print("go to chunk" + i.ToString() + j.ToString());
                reader.Goto_Section(i.ToString() + j.ToString());
                for (int k = 0; k < totalJoint; k++)
                {
                    float value = reader.Get_Float(k.ToString());
                  // if(k == 18)  print(k + "   " + value);
                    if (maxValue[k] < value)
                    {
                        
                        maxValue[k] = value;
                    }
                }
            }

        }

    }

    //public void ScreenShot(string name)
    //{
    //    //print(Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png");
    //    //Application.CaptureScreenshot(Application.dataPath+"/"+filepath+"/"+screenShotPath+"/"+ name +".png");
    //    //print("finish screenshot");

    //    Rect rect = new Rect(Screen.width * 0f, Screen.height * 0f, Screen.width * 1f, Screen.height * 1f);
    //    // 先创建一个的空纹理，大小可根据实现需要来设置  
    //    Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);

    //    // 读取屏幕像素信息并存储为纹理数据，  
    //    screenShot.ReadPixels(rect, 0, 0);
    //    screenShot.Apply();

    //    // 然后将这些纹理数据，成一个png图片文件  
    //    byte[] bytes = screenShot.EncodeToPNG();
    //    string filename = Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png";
    //    System.IO.File.WriteAllBytes(filename, bytes);
    //    Debug.Log(string.Format("截屏了一张图片: {0}", filename));

       
    //}
    public void ScreenShotV2(string name)
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
       
        string filename =  "C:/Users/z5308/Dropbox/level2Graph/"+graphfoldername + name + ".png";
       
        //string filename = Application.dataPath + "/" + filepath + "/" + screenShotPath + "/" + name + ".png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        
    }

}
