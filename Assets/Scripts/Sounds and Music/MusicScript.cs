using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioClip[] Music;
    public AudioSource source;

	// Use this for initialization
	void Start () {
        PlayMusic(Music[0]);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void PlayMusic(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
        //AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}
