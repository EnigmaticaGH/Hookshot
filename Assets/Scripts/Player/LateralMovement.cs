using UnityEngine;
using System.Collections;

public class LateralMovement : MonoBehaviour
{
    enum MovementState
    {
        GROUND,
        AIR,
        HOOKED,
        WALLJUMP,
        WALLWALK,
        DISABLED
    }
    private delegate void StateFunction();
    private StateFunction[] stateProcesses;
    private MovementState state;

    void MapStateFunctions()
    {
        stateProcesses = new StateFunction[] {
            this.Ground,
            this.Air,
            this.Hooked,
            this.WallJump,
            this.WallWalk,
            this.Disabled
        };
    }

    public float speed;
    public float speedInWater;
    private float regularSpeed;
    public float force;
    public float moveForce;
    private float horizontal;

    public HookshotControl hookshotControl;
    public SpriteRenderer characterSprite;
    private JumpControl jump;
    private Rigidbody2D player;
    private Vector2 contactNormal;

    private WallSensor wallSensorRight;
    private WallSensor wallSensorLeft;

    private const float AIR_STOP_TIME = 0.08f;
    private bool canMove;

    void Start()
    {
        regularSpeed = speed;
        horizontal = 0;
        canMove = true;
        FindPlayerParts();
        MapStateFunctions();
    }

    private void FindPlayerParts()
    {
        player = GetComponent<Rigidbody2D>();
        jump = GetComponent<JumpControl>();
        wallSensorRight = GameObject.Find("WallSensorR").GetComponent<WallSensor>();
        wallSensorLeft = GameObject.Find("WallSensorL").GetComponent<WallSensor>();
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        UpdateState();
        stateProcesses[(int)state]();
        Orient();
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
            speed = speedInWater;
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.CompareTag("Water"))
            speed = regularSpeed;
    }

    void ChangeState(MovementState newState)
    {
        state = newState;
    }

    void UpdateState()
    {
        if (!canMove) ChangeState(MovementState.DISABLED);
        else if (isGrounded() && !isHooked()) ChangeState(MovementState.GROUND);
        else if (!isGrounded() && !isHooked()) ChangeState(MovementState.AIR);
        else if (!isGrounded() && isHooked()) ChangeState(MovementState.HOOKED);
        else if (isOnWall() && isHooked()) ChangeState(MovementState.WALLWALK);
        else if (isOnWall() && !isHooked()) ChangeState(MovementState.WALLJUMP);
        else Debug.Log("Unaccounted movement state found. Grounded: " + isGrounded() + " On a wall: " + isOnWall() + ", Hooked: " + isHooked());
    }

    void Ground()
    {
        DoNormalMovement(isGrounded());
    }

    void Air()
    {
        DoNormalMovement(isGrounded());
    }

    void Hooked()
    {
        Vector2 pivotPoint = hookshotControl.HookPoint();
        if (horizontal > 0 && pivotPoint.x >= transform.position.x || horizontal < 0 && pivotPoint.x <= transform.position.x)
        {
            Vector2 lateralForce = Vector3.Cross((Vector3)pivotPoint - transform.position, Vector3.forward).normalized;
            lateralForce *= horizontal * force / (player.velocity.magnitude + 1f);
            player.AddForce(lateralForce);
        }
    }

    void WallJump()
    {

    }

    void WallWalk()
    {
        Rope().MoveAlongWall(); //RopeControl
    }

    void Disabled(){ /* The player is unable to move */ }

    void DoNormalMovement(bool onTheGround)
    {
        Vector2 lateralForce = new Vector2(horizontal * moveForce, 0);

        if (Mathf.Abs(player.velocity.x) < speed)
            player.AddForce(lateralForce);

        if (player.velocity.x > 0 && horizontal < 0
         || player.velocity.x < 0 && horizontal > 0)
        {
            player.velocity = new Vector2(0, player.velocity.y);

            if (!onTheGround)
            {
                StartCoroutine(DisableMovement(AIR_STOP_TIME));
            }
        }
    }

    bool isGrounded()
    {
        return jump.isGrounded();
    }

    bool isHooked()
    {
        return hookshotControl.IsHooked();
    }

    bool isOnWall()
    {
        return wallSensorLeft.IsWallCollide() || wallSensorRight.IsWallCollide();
    }

    RopeControl Rope()
    {
        return hookshotControl.Rope().GetComponent<RopeControl>();
    }

    void Orient()
    {
        if (!isHooked() && Mathf.Abs(horizontal) > 0)
        {
            Quaternion rot = horizontal == 1 ?
                Quaternion.Euler(0, 0, -5.73f) : 
                Quaternion.Euler(0, 180, -5.73f);
            characterSprite.transform.rotation = rot;
        }
    }

    IEnumerator DisableMovement(float t)
    {
        canMove = false;

        yield return new WaitForSeconds(t);

        canMove = true;
    }
}
