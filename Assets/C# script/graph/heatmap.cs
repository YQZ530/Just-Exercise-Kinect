using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heatmap : MonoBehaviour {

	public Texture2D heatmapText;
	[Range(0,1f)]
	public float givenValue = 0f;
	public Color outputColor;
	
	
	// Update is called once per frame
	void Update () {
		Get_Color (givenValue);
	}

	public Color Get_Color(float value)
	{
        if (value == 0f) { value += 0.001f; }
		int index = (int)(value * heatmapText.width - 1);
		outputColor = heatmapText.GetPixel (index, 0);
		return outputColor;

	}
}
