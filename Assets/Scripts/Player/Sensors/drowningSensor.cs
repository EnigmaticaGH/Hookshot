using UnityEngine;
using System.Collections;

public class drowningSensor : MonoBehaviour {

    public bool submerged;
    public TriggerCheck playerHealth;
    public HookshotControl hooked;
    public float timeForDrowning = 3.0f;

    private float timeSubmerged = 0;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
        {
            submerged = true;
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
            submerged = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!submerged)
        {
            timeSubmerged = 0;
            //Debug.Log("Is not Submerged");
        }
        else
        {
            timeSubmerged += 1 * Time.deltaTime;
            //Debug.Log("Time Submerged: " + timeSubmerged);

            if (timeSubmerged >= timeForDrowning)
            {
                playerHealth.SendMessage("Respawn");
                timeSubmerged = 0;
            }
        }
	}

    void OnGUI()
    {
        GUI.Box(new Rect(new Vector2(0, 0), new Vector2(100, 25)), "Oxygen: " + (timeForDrowning - timeSubmerged));
    }
}
