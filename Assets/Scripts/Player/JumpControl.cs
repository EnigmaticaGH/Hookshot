using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(KeybindScript))]
public class JumpControl : MonoBehaviour
{
    private const float JUMP_INTERVAL = 0.15f;
    public float jumpForce;
    public float wallJumpForce;
    public float jumpSpeed;
    public float wallJumpSpeed;

    private Rigidbody2D body;
    private KeybindScript keybinds;
    private WallSensor wallSensorRight;
    private WallSensor wallSensorLeft;
    public SoundEffectHelper soundPlayer;

    private bool jump;
    private bool doubleJump;
    private bool wallJump;
    public bool canDoubleJump;
    private bool readyForJump;

    private bool grounded;
    private bool isRunning;
    private bool touchingRight;
    private bool touchingLeft;

    void Start()
    {
        GroundSensor.GroundSensorChange += Grounded;
        keybinds = GameObject.FindGameObjectWithTag("KeyBinds").GetComponent<KeybindScript>();
        body = GetComponent<Rigidbody2D>();
        FindWallSensors();
        touchingRight = false;
        touchingLeft = false;
        isRunning = false;
        readyForJump = true;
        soundPlayer = GameObject.Find("soundEffects").GetComponent<SoundEffectHelper>();
    }
    void OnDestroy()
    {
        GroundSensor.GroundSensorChange -= Grounded;
    }
    void Update()
    {
        if (readyForJump && keybinds.GetButtonDown("Jump") && Time.timeScale > 0f)
        {
            if (grounded)
            {
                //int []CS = new int[]{1, 2};
                //soundPlayer.playSoundRandom(transform.position, soundPlayer.jumpSound, 3, CS, 2);
                soundPlayer.playSound(transform.position, soundPlayer.jumpSound[1]);
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
            //body.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);
            jump = false;
            StartCoroutine(JumpInterval());
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
        soundPlayer.playSound(transform.position, soundPlayer.jumpSound[0]);
        int direction = 0;

        if (touchingLeft && touchingRight) direction = 0;
        else if (touchingRight) direction = -1;
        else if (touchingLeft) direction = 1;

        body.velocity = Vector2.zero;
        //body.AddForce(new Vector2(wallJumpForce * direction, jumpForce), ForceMode2D.Impulse);
        body.velocity = new Vector2(wallJumpSpeed * direction, jumpSpeed);
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
    void Grounded(bool isGrounded)
    {
        if (isGrounded)
        {
            grounded = true;
        }
        else
        {
            if (!isRunning) StartCoroutine(SetGrounded(isGrounded));
        }
    }
    IEnumerator SetGrounded(bool groundSensorState)
    {
        isRunning = true;
        grounded = true;
        yield return new WaitForSeconds(0.1f);
        grounded = groundSensorState;
        isRunning = false;
    }
    IEnumerator JumpInterval()
    {
        readyForJump = false;
        yield return new WaitForSeconds(JUMP_INTERVAL);
        readyForJump = true;
    }
}
