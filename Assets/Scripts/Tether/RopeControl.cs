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

    public float minScale;
    public float maxScale;
}

public class RopeControl : MonoBehaviour {
    private LateralMovement player;
    private Rigidbody2D playerBody;

    [HideInInspector]
    public HookshotControl hookshot;
    [HideInInspector]
    public GameObject hook;

    public Rope ropeProperties;
    private Vector2 anchorOffset;
    private Vector3 frontSensorAngle;
    private Transform hand;

    private DistanceJoint2D rope;
    private SpringJoint2D spring;

    private WallSensor leftWallSensor;
    private WallSensor rightWallSensor;
    private float moveForce;

    private bool boostEnabled;

    void Start() {
        boostEnabled = false;
        hand = player.getSprite().GetComponentInChildren<AimAtMouse>().transform;
        anchorOffset = hand.localPosition;
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<LateralMovement>();
        playerBody = player.getRigidBody();
        FindWallSensors();
        moveForce = player.moveForce;
        RescaleY(0.0f);
    }

    void FindWallSensors() 
    {
        WallSensor[] sensors = player.getWallSensors();
        foreach(WallSensor sensor in sensors) {
            if (sensor.name == "WallSensorL")
                leftWallSensor = sensor;
            else if (sensor.name == "WallSensorR")
                rightWallSensor = sensor;
        }
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
        //frontSensorAngle = Quaternion.FromToRotation(hand.gameObject.transform.position, hook.transform.position).eulerAngles;
        //frontSensorAngle.z += 90;
    }

    void ControlRope()
    {
        // When player is touching a wall, make the player walk up the wall.
        if (!isTouchingWall())
        {
            float vertical = boostEnabled ? ropeProperties.boostSpeed : Input.GetAxis("Vertical");
            if (!obstacleBlocking(vertical))
            {
                float distance = vertical * ropeProperties.climbSpeed * Time.fixedDeltaTime;
                rope.distance = Mathf.Clamp(rope.distance - distance,
                                            ropeProperties.minLength,
                                            ropeProperties.maxLength);
            }
        }
    }

    private bool isTouchingWall()
    {
        return leftWallSensor.IsWallCollide() || rightWallSensor.IsWallCollide();
    }

    private void FaceWall()
    {
        player.transform.rotation = Quaternion.identity;
        player.transform.rotation = Quaternion.Euler(0, 
            leftWallSensor.IsWallCollide() ? 180f : 0f, 90f);
    }

    public void MoveAlongWall()
    {
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0)
        {
            Vector2 lateralForce = new Vector2(0, vertical * moveForce);
            if (Mathf.Abs(playerBody.velocity.y) < ropeProperties.climbSpeed)
                playerBody.AddForce(lateralForce);

            rope.distance = Mathf.Clamp(PhysicalRopeLength(),
                                        ropeProperties.minLength,
                                        ropeProperties.maxLength);
        }
        else
        {
            float distance = vertical * ropeProperties.climbSpeed * Time.fixedDeltaTime;
            rope.distance = Mathf.Clamp(rope.distance - distance,
                                        ropeProperties.minLength,
                                        ropeProperties.maxLength);
        }
    }

    private float PhysicalRopeLength()
    {
        Vector2 connPos = rope.connectedBody.transform.position;
        Vector2 startPos = rope.transform.position + rope.transform.rotation * rope.anchor;
        return Vector2.Distance(connPos, startPos);
    }

    void RotateObjectTowardsRope()
    {

        Transform spriteTransform = player.transform;

        Vector2 jointDirection = hook.transform.position - spriteTransform.position;
        spriteTransform.rotation = Quaternion.FromToRotation(Vector2.up, jointDirection);

        rope.anchor = anchorOffset;

        //if(isTouchingWall())
            //FaceWall();
    }


    public void AttachRope()
    {
        MakeRope();
    }

    private void MakeRope()
    {
        float initialDistance = Vector2.Distance(player.transform.position, hook.transform.position);
        initialDistance *= ropeProperties.initialDistancePortion;

        rope = player.gameObject.AddComponent<DistanceJoint2D>();
        rope.connectedBody = hook.GetComponent<Rigidbody2D>();
        rope.distance = Mathf.Clamp(initialDistance, ropeProperties.minLength, ropeProperties.maxLength);
        rope.maxDistanceOnly = true;

        RotateObjectTowardsRope();
    }

    public void DetachRope()
    {
        if (hookshot.IsHooked())
        {
            player.transform.rotation = Quaternion.identity;
            DestroyObject(rope);
        }
    }

    void DrawRope()
    {
        // Track tongue!
        Vector3 playerOffset = GetPlayerOffset();
        Vector3 hookPos = hook.transform.position;
        transform.position = Vector2.Lerp(playerOffset, hookPos, 0.5f);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f) * Quaternion.FromToRotation(Vector3.right, hookPos - playerOffset);

        float minScale = ropeProperties.minScale;
        float maxScale = ropeProperties.maxScale;
        //float distance = hookshot.IsHooked() ? distance = rope.distance : Vector3.Distance(playerOffset, hookPos);
        float distance = Vector3.Distance(playerOffset, hookPos);

        RescaleY(minScale + ((maxScale - minScale) * ((distance - ropeProperties.minLength)) / (ropeProperties.maxLength - ropeProperties.minLength)));
        //RescaleY(distance / 7);
    }

    private Vector2 GetPlayerOffset()
    {
        //Debug.Log(hand.position);
        return hand.position;
    }

    private void setRopeSensors(Vector3 angle)
    {
        Transform sensor = hand.GetComponent<AimAtMouse>().frontSensor.transform;
        sensor.localRotation = Quaternion.Euler(angle);
    }

    private void RescaleY(float y)
    {
        Vector3 scale = transform.localScale;
        scale.y = y;
        transform.localScale = scale;
    }

    private bool obstacleBlocking(float a)
    {
        if (a > 0)
        {
            //setRopeSensors(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90));
            setRopeSensors(new Vector3(0, 0, 90));
            return hand.GetComponent<AimAtMouse>().frontSensor.GetComponent<ObstacleSensor>().obstacleDetected();
        }
        else if (a < 0)
        {
            setRopeSensors(new Vector3(0, 0, -90));
            return hand.GetComponent<AimAtMouse>().frontSensor.GetComponent<ObstacleSensor>().obstacleDetected();
        }
        else
            return false;
    }
}
