using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeControl : MonoBehaviour {
    public GameObject linkPrefab;

    public float speedBoostFactor;
    public float climbSpeed;

    public RopeProperties properties;

    private GameObject player;
    private SpriteRenderer playerRenderer;
    public HookshotControl hookshot;
    public GameObject hook;

    private bool attached;

    private LineRenderer line;
    private Rope rope;

    private bool boostEnabled;

	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        line = GetComponent<LineRenderer>();
        attached = false;
        boostEnabled = false;
	}

	void Update () {
        if (attached) {
            boostEnabled = Input.GetButton("Fire2");
        } else if (Vector3.Distance(hookshot.transform.position, hook.transform.position) > properties.maxLength) {
            hookshot.RetractRope();
        }
        rope.FixedUpdate();
    }

    void FixedUpdate()
    {
        if (attached) {
            ControlRope();
            RotateObjectTowardsRope();
        }
        DrawRope();
    }

    void ControlRope()
    {
        float vertical = boostEnabled ? speedBoostFactor : Input.GetAxis("Vertical");
        float distance = vertical * climbSpeed * Time.deltaTime;
        rope.ChangeLength(-distance);
    }

    public void AttachRope()
    {
        MakeNewRope();
        playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        attached = true;
    }

    void MakeNewRope()
    {
        properties.linkPrefab = linkPrefab;
        properties.tetherObject = hook;
        properties.connectedObject = player;
        properties.parentObject = gameObject;
        rope = new Rope(properties);
    }

    void RotateObjectTowardsRope()
    {
        Vector2 jointDirection =  properties.connectedPosition - properties.tetherPosition;
        Quaternion rotation = Quaternion.FromToRotation(Vector2.right, -jointDirection);
        playerRenderer.gameObject.transform.rotation = rotation;
    }

    void DrawRope()
    {
        if (attached) {
            rope.Render(line);
        } else {
            line.SetPosition(0, hookshot.transform.position);
            line.SetPosition(1, hook.transform.position);
        }
    }

    void OnDestroy()
    {
        if (attached)
        {
            rope.Destroy();
            playerRenderer.gameObject.transform.rotation = Quaternion.identity;

        }
    }
}
