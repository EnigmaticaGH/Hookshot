using UnityEngine;
using System.Collections;

public class WallSensor : MonoBehaviour
{
    private sbyte wallCollide = 0;

    void OnTriggerEnter2D(Collider2D c)
    {
        wallCollide = c.CompareTag("Environment") || c.CompareTag("Hookable") ? ++wallCollide : wallCollide;
    }

    void OnTriggerExit2D(Collider2D c)
    {
        wallCollide = c.CompareTag("Environment") || c.CompareTag("Hookable") ? (sbyte)0 : wallCollide;
    }

    public bool IsWallCollide()
    {
        return wallCollide > 0;
    }
}
