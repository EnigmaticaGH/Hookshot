using UnityEngine;
using System.Collections;

public class AimAtMouse : MonoBehaviour {
    private bool aimingEnabled;

    public GameObject frontSensor;

    void Start()
    {
        aimingEnabled = true;
        frontSensor = GameObject.Find("FrontSensorR");
    }

    void Update()
    {
        if (!aimingEnabled) {
            return;
        }

        if(Time.timeScale != 0f)
        {
            // Figure out angle from horizontal
            Vector3 pivotPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 coords = Input.mousePosition;
            Vector3 direction = coords - pivotPos;
            direction = new Vector3(direction.x, direction.y, 0);

            if (direction.x < 0)
            {
                direction.x = -direction.x;
                Vector3 angles = Quaternion.FromToRotation(Vector3.right, direction).eulerAngles;
                transform.rotation = Quaternion.Euler(0f, 180f, angles.z);
            }
            else
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            }
        }
    }

    public void ToggleAiming(bool flag)
    {
        aimingEnabled = flag;
    }
}
