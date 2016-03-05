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
        GROUNDWALLHOOK,
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
            this.GroundWallHook,
            this.Disabled
        };
    }

    public float speed;
    public float speedInWater;
    public float force;
    public float moveForce;
    public float speedForParticles = 5;
    private float horizontal;

    private GameObject hand;
    private HookshotControl hookshotControl;
    private GameObject characterSprite;
    private JumpControl jump;
    private Rigidbody2D player;
    private Vector2 contactNormal;
    private CeilingSensor ceilingSensor;
    private WallSensor wallSensorRight;
    private WallSensor wallSensorLeft;
    private AnimateFrog frogAnim;
    private RelativeVelocity relative;
    private ParticleEffectManager PEM;

    private const float AIR_STOP_TIME = 0.05f;
    private const float SPRITE_OFFSET_ANGLE = -5.73f;
    private const float TIME_BETWEEN_JUMPS = 0.08f;
    private bool canMove;
    private bool canJump;

    public delegate void GroundStateDelegate();
    public delegate void JumpStateDelegate();
    public delegate void HookStateDelegate();
    public static event GroundStateDelegate GroundState;
    public static event JumpStateDelegate JumpState;
    public static event HookStateDelegate HookState;

    void Start()
    {
        horizontal = 0;
        canMove = true;
        canJump = true;
        MapStateFunctions();
        player = GetComponent<Rigidbody2D>();
        
        ChangeState(MovementState.GROUND);
        PEM = GameObject.FindGameObjectWithTag("Particles Manager").GetComponent<ParticleEffectManager>();
    }

    void Awake()
    {
        FindPlayerParts();
    }

    private void FindPlayerParts()
    {
        jump = GetComponent<JumpControl>();
        characterSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        frogAnim = characterSprite.GetComponent<AnimateFrog>();
        hand = characterSprite.GetComponentInChildren<AimAtMouse>().gameObject;
        hookshotControl = hand.GetComponentInChildren<HookshotControl>();
        wallSensorRight = GameObject.Find("WallSensorR").GetComponent<WallSensor>();
        wallSensorLeft = GameObject.Find("WallSensorL").GetComponent<WallSensor>();
        ceilingSensor = GameObject.Find("CeilingSensor").GetComponent<CeilingSensor>();
        relative = GameObject.Find("GroundColliders").GetComponent<RelativeVelocity>();
    }

    void FixedUpdate()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        stateProcesses[(int)state]();
        if (Mathf.Abs(player.velocity.x) >= speedForParticles)
            PEM.SendMessage("generateSpeedParticles", transform.position);
    }

    void ChangeState(MovementState newState)
    {
        state = canMove ? newState : MovementState.DISABLED;
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
    }

    void Ground()
    {
        if (!isGrounded()) ChangeState(MovementState.AIR);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(isGrounded());
        if(GroundState != null)
            GroundState();
    }

    void Air()
    {
        if (isGrounded()) ChangeState(MovementState.GROUND);
        if (isOnWall()) ChangeState(MovementState.WALLJUMP);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(isGrounded());
        if (JumpState != null)
            JumpState();
    }

    void Hooked()
    {
        if (isOnWall()) ChangeState(MovementState.WALLWALK);
        if (!isHooked()) ChangeState(MovementState.AIR);

        if (HookPoint() != null)
        {
            Vector2 pivotPoint = HookPoint().position;

            if (horizontal > 0 && pivotPoint.x >= transform.position.x || horizontal < 0 && pivotPoint.x <= transform.position.x)
            {
                Vector2 lateralForce = Vector3.Cross((Vector3)pivotPoint - transform.position, Vector3.forward).normalized;
                lateralForce *= horizontal * force / (player.velocity.magnitude + 1f);
                player.AddForce(lateralForce);
            }

            float yAngle = player.velocity.x > 0 ? 0 : 180;
            transform.rotation = Quaternion.Euler(0, yAngle, transform.rotation.eulerAngles.z);
        }
        if (HookState != null)
            HookState();
    }

    void WallJump()
    {
        if (!isOnWall()) ChangeState(MovementState.AIR);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        if (isGrounded()) ChangeState(MovementState.GROUND);
        DoNormalMovement(true);
        if (CanWallJump())
        {
            jump.WallJump();
            StartCoroutine(frogAnim.PlayWallJump());
            StartCoroutine(TimeBetweenJumps());
            ChangeState(MovementState.DISABLED);
        }
        if (JumpState != null)
            JumpState();
    }

    void WallWalk()
    {
        if (!isOnWall()) ChangeState(MovementState.HOOKED);
        if (!isHooked()) ChangeState(MovementState.AIR);
        if (isGrounded()) ChangeState(MovementState.GROUNDWALLHOOK);
        if (HookPoint() != null)
        {
            Rope().MoveAlongWall(); //RopeControl
        }
        if (HookState != null)
            HookState();
    }

    void GroundWallHook() //A very rare state in which the player is on the ground, touching a wall, and hooked at the same time
    {
        if (!isGrounded()) ChangeState(MovementState.WALLWALK);
        if (!isHooked()) ChangeState(MovementState.GROUND);
        if (!isOnWall()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(true);
        Rope().MoveAlongWall();
        if (GroundState != null)
            GroundState();
    }

    void Disabled(){ /* The player is unable to move */
        if (canMove) ChangeState(MovementState.AIR);
    }

    void DoNormalMovement(bool onTheGround)
    {
        if (onTheGround)
        {
            Vector2 lateralVelocity = new Vector2(speed * Input.GetAxis("Horizontal"), player.velocity.y);
            player.velocity = lateralVelocity;
        }
        else
        {
            Vector2 lateralForce = new Vector2(horizontal * moveForce, 0);

            if (Mathf.Abs(player.velocity.x) < speed)
                player.AddForce(lateralForce);
        }

        if (player.velocity.x > 0 && horizontal < 0
         || player.velocity.x < 0 && horizontal > 0)
        {
            player.velocity = new Vector2(0, player.velocity.y);

            if (!onTheGround)
            {
                StartCoroutine(DisableMovement(AIR_STOP_TIME));
                ChangeState(MovementState.DISABLED);
            }
        }
    }

    Transform HookPoint()
    {
        return hookshotControl.HookPoint();
    }

    public Vector2 RelativeVelocity()
    {
        return relative.GetRelativeVelocity;
    }

    private bool CanWallJump()
    {
        return jump.CanWallJump() && canJump && Keybinds().GetButton("Jump") && Time.timeScale > 0f;
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

    public bool isHookedOrExtending()
    {
        return hookshotControl.IsExtending() || isHooked();
    }

    public bool isOnWall()
    {
        return wallSensorLeft.IsWallCollide() || wallSensorRight.IsWallCollide();
    }

    public bool IsNotTouchingAGodDamnThing()
    {
        return !isOnWall() && !isGrounded() && !isHooked();
    }

    public RopeControl Rope()
    {
        return hookshotControl.Rope().GetComponent<RopeControl>() != null ?
            hookshotControl.Rope().GetComponent<RopeControl>() : null;
    }

    public GameObject Hook()
    {
        return hookshotControl.HookPoint().gameObject;
    }

    public HookshotControl getHookScript()
    {
        return hookshotControl;
    }

    public JumpControl getJumpScript()
    {
        return jump;
    }

    public KeybindScript Keybinds()
    {
        return jump.Keybinds();
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

    public IEnumerator DisableMovement(float disableTime)
    {
        ChangeState(MovementState.DISABLED);
        canMove = false;
        yield return new WaitForSeconds(disableTime);
        canMove = true;
    }

    public IEnumerator TimeBetweenJumps()
    {
        canJump = false;
        yield return new WaitForSeconds(TIME_BETWEEN_JUMPS);
        canJump = true;
    }

    public IEnumerator DebugFSMState()
    {
        while(true)
        {
            Debug.Log(state);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void disableMovement()
    {
        ChangeState(MovementState.DISABLED);
    }

    public void changeCanMove(bool value)
    {
        canMove = value;
    }
}
