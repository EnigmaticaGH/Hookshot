using UnityEngine;
using System.Collections;

public class WallSensor2 : MonoBehaviour
{

    private bool wallCollide;

    void OnTriggerEnter2D(Collider2D c)
    {
        wallCollide = c.CompareTag("Environment");
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
