using UnityEngine;
using System.Collections;

public class WallSensor : MonoBehaviour {

    public bool emptySpace;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Environment"))
            emptySpace = false;
        else
            emptySpace = true;
    }

    void OnTriggerExit2D(Collider2D c)
    {
            emptySpace = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}


}
