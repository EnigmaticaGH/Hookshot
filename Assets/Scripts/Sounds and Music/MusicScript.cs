using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

    public AudioClip[] Music;
    public AudioSource source;

    public float MasterVolume = 1.0f;
    public float MusicVolume = 1.0f;

	// Use this for initialization
	void Start () {
        source.volume = MusicVolume * MasterVolume;
        PlayMusic(Music[0]);
	}

    void PlayMusic(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
        //AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    public void refreshVolume()
    {
        source.volume = MusicVolume * MasterVolume;
    }

    public void OnMasterChange(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
        refreshVolume();
    }

    public void OnMusicChange(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
        refreshVolume();
    }
}
