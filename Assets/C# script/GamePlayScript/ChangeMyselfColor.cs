using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMyselfColor : MonoBehaviour {

    public Renderer[] rend;
    public Color colorStart;
    public Color colorEnd;
    public float duration = 1.0f;
    public bool startChanging = false;
    float timer = 0f;
    // Use this for initialization
    void Start () {
        
        rend = this.GetComponentsInChildren<MeshRenderer>();
        colorStart = new Color(0f, 0f, 0f, 0.2f);
        colorEnd = new Color(0f, 0f, 0f, 1f);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (startChanging)
        {
            timer += Time.deltaTime;

            float lerp =  timer/ duration > 1f ? 1f : timer / duration;
            //print(lerp);
            for (int i = 0; i < rend.Length; i++)
            {
                rend[i].material.color = Color.Lerp(colorStart, colorEnd, lerp);
                //rend[i].material.color = colorEnd;
            }
        }
        
        //rend.material.color = Color.Lerp(colorStart, colorEnd, lerp);
       // rend.material.color = colorEnd;

    }


}
