using UnityEngine;
using System.Collections;

public class CeilingSensor : MonoBehaviour {

    private bool canControlRope;
    private bool inTrigger;

    void Update()
    {
        canControlRope = (Input.GetAxis("Vertical") <= 0 && inTrigger) || !inTrigger;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            inTrigger = false;
        }
    }

    public bool onCeiling()
    {
        return inTrigger;
    }

    public bool CanControlRope()
    {
        return canControlRope;
    }
}
