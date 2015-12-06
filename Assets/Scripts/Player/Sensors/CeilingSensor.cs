using UnityEngine;
using System.Collections;

public class CeilingSensor : MonoBehaviour {

    private bool canControlRope;
    private sbyte inTrigger = 0;

    void Update()
    {
        canControlRope = (Input.GetAxis("Vertical") <= 0 && inTrigger > 0) || inTrigger <= 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            inTrigger++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            inTrigger = 0;
        }
    }

    public bool onCeiling()
    {
        return inTrigger > 0;
    }

    public bool CanControlRope()
    {
        return canControlRope;
    }
}
