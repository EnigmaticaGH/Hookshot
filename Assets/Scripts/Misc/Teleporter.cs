using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {
    public string zoneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Application.LoadLevel(zoneName);
        }
    }
}
