using UnityEngine;
using System.Collections;

public class WallSensor : MonoBehaviour
{

    private bool wallCollide;

    void OnTriggerStay2D(Collider2D c)
    {
        wallCollide = c.CompareTag("Environment") || c.CompareTag("Hookable");
    }

    void OnTriggerExit2D(Collider2D c)
    {
        wallCollide = false;
    }

    public bool IsWallCollide()
    {
        return wallCollide;
    }
}
