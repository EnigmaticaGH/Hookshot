using UnityEngine;
using System.Collections;

public class LeftWallSensor : MonoBehaviour {

    public RopeControl triggers;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {

        }
    }
}
