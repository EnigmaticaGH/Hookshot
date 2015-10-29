using UnityEngine;
using System.Collections;

public class WallSensor : MonoBehaviour
{
    private bool wallCollide;

    void OnTriggerStay2D(Collider2D c)
    {
        wallCollide = c.CompareTag("Environment");
    }

    void OnTriggerExit2D(Collider2D c)
    {
        wallCollide = false;
    }
}