﻿using UnityEngine;
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
    private AnimateFrog frogAnim;

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
        ChangeState(MovementState.GROUND);
    }

    void Awake()
    {
        FindPlayerParts();
    }

    private void FindPlayerParts()
    {
        characterSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        frogAnim = characterSprite.GetComponent<AnimateFrog>();
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
        stateProcesses[(int)state]();
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
        state = canMove ? newState : MovementState.DISABLED;
    }

    void Ground()
    {
        if (!isGrounded()) ChangeState(MovementState.AIR);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(isGrounded());
    }

    void Air()
    {
        if (isGrounded()) ChangeState(MovementState.GROUND);
        if (isOnWall()) ChangeState(MovementState.WALLJUMP);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(isGrounded());
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
        }
    }

    void WallJump()
    {
        if (!isOnWall()) ChangeState(MovementState.AIR);
        if (isHooked()) ChangeState(MovementState.HOOKED);
        DoNormalMovement(true);
        if (Keybinds().GetButton("Jump") && Time.timeScale > 0f)
        {
            jump.WallJump();
            StartCoroutine(frogAnim.PlayWallJump());
            ChangeState(MovementState.DISABLED);
        }
        
    }

    void WallWalk()
    {
        if (!isOnWall()) ChangeState(MovementState.HOOKED);
        if (!isHooked()) ChangeState(MovementState.AIR);
        if (HookPoint() != null)
        {
            Rope().MoveAlongWall(); //RopeControl
        }
        //OrientVertical();
    }

    void Disabled(){ /* The player is unable to move */
        if (canMove) ChangeState(MovementState.AIR);
    }

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
                ChangeState(MovementState.DISABLED);
            }
        }
    }

    Transform HookPoint()
    {
        return hookshotControl.HookPoint();
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

    public bool IsNotTouchingAGodDamnThing()
    {
        return !isOnWall() && !isGrounded() && !isHooked();
    }

    public RopeControl Rope()
    {
        return hookshotControl.Rope().GetComponent<RopeControl>() != null ?
            hookshotControl.Rope().GetComponent<RopeControl>() : null;
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

    public IEnumerator DisableMovement(float t)
    {
        canMove = false;

        yield return new WaitForSeconds(t);

        canMove = true;
    }
}
