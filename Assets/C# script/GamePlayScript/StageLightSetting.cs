using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StageLightSetting : MonoBehaviour {
     RawImage stageimg;
    public float duration = 3.0f;
    //public bool startChanging = false;
    float timer = 0f;
    public  float lerp = 2f;
    public Color colorStart;
    public Color colorEnd;
    private const string EmissiveValue = "_EmissionScaleUI";
    // Use this for initialization
    void Start () {
        stageimg = this.GetComponent<RawImage>();
        // Turn on emission
        stageimg.material.EnableKeyword("_EMISSION");
       colorStart = new Color(0.8f,0.8f,0.8f);
        colorEnd = new Color(1f, 1f, 1f);
}
	
	// Update is called once per frame
	void Update () {
        lerp = Mathf.PingPong(Time.time, duration) / duration;
        //print(lerp);
        //stageimg.material.SetFloat(EmissiveValue, lerp);
        stageimg.material.SetColor("_EmissionColor", Color.Lerp(colorStart, colorEnd, lerp));

    }
}
