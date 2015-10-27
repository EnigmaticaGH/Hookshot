using UnityEngine;
using System.Collections;

public class GroundSensor : MonoBehaviour {
    public JumpControl player;
    public RopeControl triggers;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            player.SetGrounded(true);
            triggers.setBot(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            player.SetGrounded(false);
            triggers.setBot(false);
        }
    }
}
