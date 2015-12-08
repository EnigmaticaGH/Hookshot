using UnityEngine;
using System.Collections;

public class ObstacleSensor : MonoBehaviour {

    public bool Obstacle = false;

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Environment") || c.CompareTag("Hookable"))
            Obstacle = true;
    }
    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Environment") || c.CompareTag("Hookable"))
            Obstacle = false;
    }

    public bool obstacleDetected()
    {
        return Obstacle;
    }
}
