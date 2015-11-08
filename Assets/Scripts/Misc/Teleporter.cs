using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {
    public string zoneName;
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            Application.LoadLevel(zoneName);
        }
    }
}
