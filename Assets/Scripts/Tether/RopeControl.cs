using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Rope
{
    public float climbSpeed;
    public float boostSpeed;

    public float minLength;
    public float maxLength;

    public float initialDistancePortion;
}

public class RopeControl : MonoBehaviour {
    private GameObject player;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerRenderer;

    public HookshotControl hookshot;
    public GameObject hook;

    public Rope ropeProperties;
    public Vector2 anchorOffset;

    public GameObject springPrefab;

    private LineRenderer line;
    private DistanceJoint2D rope;
    private SpringJoint2D spring;

    private WallSensor leftWallSensor;
    private WallSensor rightWallSensor;
    private float moveForce;
    private float maxVerticalSpeed;

    private bool boostEnabled;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerBody = player.GetComponent<Rigidbody2D>();
        playerRenderer = player.GetComponentInChildren<SpriteRenderer>();
        FindWallSensors();
        EstablishConstants();
        line = GetComponent<LineRenderer>();
        boostEnabled = false;
    }

    void FindWallSensors() 
    {
        WallSensor[] sensors = player.GetComponentsInChildren<WallSensor>();
        foreach(WallSensor sensor in sensors) {
            if (sensor.name == "WallSensorL")
                leftWallSensor = sensor;
            else if (sensor.name == "WallSensorR")
                rightWallSensor = sensor;
        }
    }

    void EstablishConstants()
    {
        LateralMovement movement = player.GetComponent<LateralMovement>();
        moveForce = movement.moveForce;
        maxVerticalSpeed = movement.speed;
    }

    void Update() {
        if (hookshot.IsHooked()) {
            boostEnabled = Input.GetButton("Fire2");
        } else if (Vector3.Distance(hookshot.transform.position, hook.transform.position) > ropeProperties.maxLength) {
            hookshot.RetractRope();
        }
    }

    void FixedUpdate()
    {
        if (hookshot.IsHooked()) {
            ControlRope();
            RotateObjectTowardsRope();
        }
        DrawRope();
    }

    void ControlRope()
    {
        // When player is touching a wall, make the player walk up the wall.
        if (leftWallSensor.IsWallCollide() || rightWallSensor.IsWallCollide())
        {
            FaceWall();
            MoveAlongWall();
        }
        else
        {
            float vertical = boostEnabled ? ropeProperties.boostSpeed : Input.GetAxis("Vertical");
            float distance = vertical * ropeProperties.climbSpeed * Time.fixedDeltaTime;
            rope.distance = Mathf.Clamp(rope.distance - distance,
                                        ropeProperties.minLength,
                                        ropeProperties.maxLength);
        }
    }

    private void FaceWall()
    {
        // Yeah
    }

    private void MoveAlongWall()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector2 lateralForce = new Vector2(0, vertical * moveForce);
        if (Mathf.Abs(playerBody.velocity.y) < maxVerticalSpeed)
            playerBody.AddForce(lateralForce);

        // And shorten rope appropriately.
    }

    public void AttachRope()
    {
        MakeRope();
        DrawRope();
    }

    private void MakeRope()
    {
        // Adding spring joint first. The rope will attach to it
        float initialDistance = Vector2.Distance(player.transform.position, hook.transform.position);
        initialDistance *= ropeProperties.initialDistancePortion;

        // Rope Joint 
        rope = player.AddComponent<DistanceJoint2D>();
        rope.connectedBody = hook.GetComponent<Rigidbody2D>();
        rope.distance = Mathf.Clamp(initialDistance, ropeProperties.minLength, ropeProperties.maxLength);
        rope.maxDistanceOnly = true;

        RotateObjectTowardsRope();
    }

    public void DetachRope()
    {
        if (hookshot.IsHooked())
        {
            playerRenderer.gameObject.transform.rotation = Quaternion.identity;
            DestroyObject(rope);
        }
    }

    void RotateObjectTowardsRope()
    {
        Transform spriteTransform = playerRenderer.gameObject.transform;

        Vector2 jointDirection = hook.transform.position - spriteTransform.position;
        spriteTransform.rotation = Quaternion.FromToRotation(Vector2.right, jointDirection);

        rope.anchor = playerRenderer.transform.localPosition + spriteTransform.rotation * anchorOffset;
    }

    void DrawRope()
    {
        if (hookshot.IsHooked())
            line.SetPosition(0, rope.transform.position + (Vector3)rope.anchor);
        else
            line.SetPosition(0, player.transform.position
                              + playerRenderer.transform.localPosition
                              + playerRenderer.transform.rotation * anchorOffset);
        line.SetPosition(1, hook.transform.position);
    }
}
