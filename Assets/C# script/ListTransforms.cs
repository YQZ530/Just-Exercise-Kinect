using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListTransforms : MonoBehaviour {

    public Transform root;
    public List<Transform> bones;
    
    public int boneCount;
    public int JointCount;
    
    // Public variables that will get matched to bones. If empty, the Kinect will simply not track it.

    public List<Transform> keyJoint;
   
    public Transform SpineBase;
    public Transform SpineMid;
    public Transform SpineShoulder;
    public Transform Neck;
    public Transform Head;

   // public Transform ClavicleLeft;
    public Transform ShoulderLeft;
    public Transform ElbowLeft;
    public Transform WristLeft;
    public Transform HandLeft;
    public Transform HandTipLeft;
    public Transform ThumbLeft;

    //public Transform ClavicleRight;
    public Transform ShoulderRight;
    public Transform ElbowRight;
    public Transform WristRight;
    public Transform HandRight;
    public Transform HandTipRight;
    public Transform ThumbRight;


    public Transform HipLeft;
    public Transform KneeLeft;
    public Transform AnkleLeft;
    public Transform FootLeft;

    public Transform HipRight;
    public Transform KneeRight;
    public Transform AnkleRight;
    public Transform FootRight;

    // Use this for initialization
    void Awake () {

        //automatically get all children transform of the object
        ListTrans();
        boneCount = bones.Count;
        keyJoint = new List<Transform>() { SpineBase,SpineMid,SpineShoulder, Neck, Head,  ShoulderLeft, ElbowLeft,   WristLeft, HandLeft, HandTipLeft,ThumbLeft,
                                  ShoulderRight, ElbowRight, WristRight, HandRight,HandTipRight, ThumbRight, HipLeft, KneeLeft, AnkleLeft,FootLeft, HipRight, KneeRight, AnkleRight,FootRight };
        JointCount = keyJoint.Count;


    }
	

    public void ListTrans()
    {
        bones = new List<Transform>();
        Transform cur = root;
        ListTrans_Helper(cur);
    }

    public void ListTrans_Helper(Transform cur)
    {
        bones.Add(cur);
        for (int i = 0; i < cur.childCount; i++)
        {
            ListTrans_Helper(cur.GetChild(i));
        }

    }

    
}
