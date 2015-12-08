using UnityEngine;
using System.Collections;

public class SoundEffectHelper : MonoBehaviour {

    public AudioClip []jumpSound;
    public AudioClip []foliageSound;
    public AudioClip []waterSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void playSound(Vector3 soundDeployment, AudioClip sound)
    {
        AudioSource.PlayClipAtPoint(sound, soundDeployment);
    }

    public void playSoundRandom(Vector3 soundPos, AudioClip []arrayOfSounds, int sizeOfArray, int []availableSounds, int lenghtOfAvailableSounds)
    {
        bool numIsUsable = false;
        int ranNum = 0;

        for (int a = 0; a < 20; a++)
        {
            ranNum = Mathf.RoundToInt(Random.Range(0, sizeOfArray));

            for (int b = 0; b < lenghtOfAvailableSounds; b++)
            {
                if (ranNum == availableSounds[b])
                {
                    numIsUsable = true;
                    break;
                }
            }

            if (numIsUsable)
                break;
        }

        if (numIsUsable)
        {
            AudioSource.PlayClipAtPoint(arrayOfSounds[ranNum], soundPos);
            Debug.Log(ranNum);
        }
    }
}
