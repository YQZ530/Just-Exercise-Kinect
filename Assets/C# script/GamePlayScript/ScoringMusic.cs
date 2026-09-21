using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ScoringMusic : MonoBehaviour {
   
   
        protected static ScoringMusic instance = null;

        public AudioSource audio;
        public AudioClip ohNo;
        public AudioClip good;
        public AudioClip great;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("Multiple background musci script!!");
                Destroy(this);
                return;
            }
        }
        public static ScoringMusic Instance
        {
            get
            {
                return instance;
            }
        }
        // Use this for initialization
        void Start()
        {
            audio = gameObject.GetComponent<AudioSource>();
        }

        public void playOhNoAudio()
        {

           
        if(audio.isPlaying ) { return;  }
        audio.clip = ohNo;
        audio.Play();

        }

        public void playGoodAudio()
        {
        if (audio.isPlaying) { return; }
            audio.clip = good;
            audio.Play();

        }
        public void playGreatAudio()
        {
        if (audio.isPlaying) { return; }
        audio.clip = great;
            audio.Play();

        }
    }
