using UnityEngine;
using System.Collections;

public class GroundSensor : MonoBehaviour {
    public JumpControl player;
    private bool canControlRope;
    private bool inTrigger;

    void Update()
    {
        canControlRope = (Input.GetAxis("Vertical") >= 0 && inTrigger) || !inTrigger;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            player.SetGrounded(true);
            inTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            player.SetGrounded(false);
            inTrigger = false;
        }
    }

    public bool CanControlRope()
    {
        return canControlRope;
    }
}
