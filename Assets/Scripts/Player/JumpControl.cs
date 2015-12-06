using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(KeybindScript))]
public class JumpControl : MonoBehaviour
{
    public float jumpForce;
    public float wallJumpForce;

    private Rigidbody2D body;
    private KeybindScript keybinds;
    private WallSensor wallSensorRight;
    private WallSensor wallSensorLeft;
    private GroundSensor groundSensor;

    private bool jump;
    private bool doubleJump;
    private bool wallJump;
    public bool canDoubleJump;

    private bool grounded;
    private bool touchingRight;
    private bool touchingLeft;

    void Start()
    {
        keybinds = GameObject.FindGameObjectWithTag("KeyBinds").GetComponent<KeybindScript>();
        body = GetComponent<Rigidbody2D>();
        groundSensor = GameObject.Find("GroundSensor").GetComponent<GroundSensor>();
        FindWallSensors();
        touchingRight = false;
        touchingLeft = false;
    }

    void Update()
    {
        grounded = groundSensor.isGrounded();
        if (keybinds.GetButtonDown("Jump") && Time.timeScale > 0f)
        {
            if (grounded)
            {
                jump = true;
                wallJump = false;
            }
            else if (doubleJump && canDoubleJump)
            {
                jump = true;
                wallJump = false;
                doubleJump = false;
            }
        }
        else if(keybinds.GetButtonUp("Jump") && Time.timeScale > 0f)
        {
            wallJump = true;
        }
    }

    void FixedUpdate()
    {
        touchingRight = wallSensorRight.IsWallCollide();
        touchingLeft = wallSensorLeft.IsWallCollide();
        if (jump)
        {
            body.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
    }

    void FindWallSensors()
    {
        WallSensor[] sensors = GetComponent<LateralMovement>().getWallSensors();
        foreach (WallSensor sensor in sensors)
        {
            if (sensor.name == "WallSensorL")
                wallSensorLeft = sensor;
            else if (sensor.name == "WallSensorR")
                wallSensorRight = sensor;
        }
    }

    public void WallJump()
    {
        int direction = 0;

        if (touchingLeft && touchingRight) direction = 0;
        else if (touchingRight) direction = -1;
        else if (touchingLeft) direction = 1;

        body.velocity = Vector2.zero;
        body.AddForce(new Vector2(wallJumpForce * direction, jumpForce), ForceMode2D.Impulse);
        StartCoroutine(GetComponent<LateralMovement>().DisableMovement(0.5f));
    }

    public bool isGrounded()
    {
        return grounded;
    }

    public bool CanWallJump()
    {
        return wallJump;
    }

    public KeybindScript Keybinds()
    {
        return keybinds;
    }
}
