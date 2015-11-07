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
    private float vertical;

    private GameObject hand;
    private HookshotControl hookshotControl;
    private GameObject characterSprite;
    private JumpControl jump;
    private Rigidbody2D player;
    private Vector2 contactNormal;
    private CeilingSensor ceilingSensor;
    private WallSensor wallSensorRight;
    private WallSensor wallSensorLeft;

    private const float AIR_STOP_TIME = 0.08f;
    private const float SPRITE_OFFSET_ANGLE = -5.73f;
    private bool canMove;

    void Start()
    {
        regularSpeed = speed;
        horizontal = 0;
        canMove = true;
        MapStateFunctions();
        player = GetComponent<Rigidbody2D>();
        jump = GetComponent<JumpControl>();
    }

    void Awake()
    {
        FindPlayerParts();
    }

    private void FindPlayerParts()
    {
        characterSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        hand = characterSprite.GetComponentInChildren<AimAtMouse>().gameObject;
        hookshotControl = hand.GetComponentInChildren<HookshotControl>();
        wallSensorRight = GameObject.Find("WallSensorR").GetComponent<WallSensor>();
        wallSensorLeft = GameObject.Find("WallSensorL").GetComponent<WallSensor>();
        ceilingSensor = GameObject.Find("CeilingSensor").GetComponent<CeilingSensor>();
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        UpdateState();
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
        else if ((isOnWall() || isGrounded()) && isHooked()) ChangeState(MovementState.WALLWALK);
        else if (isOnWall() && !isHooked()) ChangeState(MovementState.WALLJUMP);
        else Debug.Log("Unaccounted movement state found. Grounded: " + isGrounded() + ", On a wall: " + isOnWall() + ", Hooked: " + isHooked());
        stateProcesses[(int)state]();
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
        OrientVertical();
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

        OrientHorizontal();
    }

    public bool isGrounded()
    {
        return jump.isGrounded();
    }

    public bool touchingCeiling()
    {
        return ceilingSensor.onCeiling();
    }

    public bool isHooked()
    {
        return hookshotControl.IsHooked();
    }

    public bool isOnWall()
    {
        return wallSensorLeft.IsWallCollide() || wallSensorRight.IsWallCollide();
    }

    public RopeControl Rope()
    {
        return hookshotControl.Rope().GetComponent<RopeControl>();
    }

    public HookshotControl getHookScript()
    {
        return hookshotControl;
    }

    public JumpControl getJumpScript()
    {
        return jump;
    }

    public WallSensor[] getWallSensors()
    {
        return new WallSensor[2]
        {
            wallSensorRight,
            wallSensorLeft
        };
    }

    public CeilingSensor getCeilingSensor()
    {
        return ceilingSensor;
    }

    public Rigidbody2D getRigidBody()
    {
        return player;
    }

    public GameObject getSprite()
    {
        return characterSprite;
    }

    void OrientHorizontal()
    {
        if (horizontal != 0)
        {
            Quaternion rot = horizontal > 0 ?
                Quaternion.Euler(0, 0, SPRITE_OFFSET_ANGLE) : 
                Quaternion.Euler(0, 180, SPRITE_OFFSET_ANGLE);
            characterSprite.transform.rotation = rot;
        }
    }

    void OrientVertical()
    {
        if (vertical != 0)
        {
            Quaternion rot = vertical > 0 ?
                Quaternion.Euler(0, 0, SPRITE_OFFSET_ANGLE + 90) :
                Quaternion.Euler(0, 180, SPRITE_OFFSET_ANGLE - 90);
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
