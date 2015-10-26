using UnityEngine;
using System.Collections;

public class CeilingSensor : MonoBehaviour {

    public RopeControl triggers;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            triggers.setTop(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            triggers.setTop(false);
        }
    }
}
