using UnityEngine;
using System.Collections;

public class WallSensor : MonoBehaviour {
    public bool emptySpace;

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
}
