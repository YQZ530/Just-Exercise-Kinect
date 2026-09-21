using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    public float destoryTime = 5f;
	// Use this for initialization
	public void DestoryMe () {
        DestroyObject(this.gameObject, destoryTime);
	}

}
