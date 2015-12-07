using UnityEngine;
using System.Collections;

public class GroundSensor : MonoBehaviour {
    private bool canControlRope;
    private sbyte grounded;
    public delegate void GroundSensorEvent(bool sensorState);
    public static event GroundSensorEvent GroundSensorChange;

    void Awake()
    {
        KillEnemies.OnRespawn += Reset;
        grounded = 0;
    }

    void OnDestroy()
    {
        KillEnemies.OnRespawn -= Reset;
    }

    void Update()
    {
        canControlRope = (Input.GetAxis("Vertical") >= 0 && grounded > 0) || !(grounded > 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            grounded = 1;
            GroundSensorChange(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Environment") || other.CompareTag("Hookable"))
        {
            grounded = 0;
            GroundSensorChange(false);
        }
    }
    void Reset()
    {
        grounded = 0;
    }
    public bool CanControlRope()
    {
        return canControlRope;
    }
}
