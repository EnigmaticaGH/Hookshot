using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HookshotControl : MonoBehaviour {
    enum HookshotState {
        READY,
        EXTENDING,
        RETRACTING,
        HOOKED,
        FLYING
    }
    private delegate void StateFunction();
    private StateFunction[] stateProcesses;

    void MapStateFunctions()
    {
        stateProcesses = new StateFunction[] {
            this.Ready,
            this.Extend,
            this.Retract,
            this.Hooked,
            this.Flying
        };
    }

    public GameObject hookFab;
    public GameObject ropeFab;

    public float retractTime;

    private List<Collider2D> playerColliders;
    private GameObject hand;
    private RopeControl rope;
    public AimAtMouse mouseAimer;
    private GameObject hook;

    private HookshotState state;
    private Vector2 retractPoint;
    private float stateSwitchTime;

    public JumpControl player;

    void Start()
    {
        MapStateFunctions();
        hand = transform.parent.gameObject;
        FindPlayerColliders();
        ChangeState(HookshotState.READY);
    }

    private void FindPlayerColliders()
    {
        playerColliders = new List<Collider2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach(Collider2D shittyCollider in player.GetComponents<Collider2D>())
            playerColliders.Add(shittyCollider);
    }

    void ChangeState(HookshotState newState)
    {
        state = newState;
        stateSwitchTime = Time.time;
        if (state == HookshotState.READY) {
            mouseAimer.ToggleAiming(true);
        } else {
            mouseAimer.ToggleAiming(false);
        }
    }

	void Update () 
    {
        stateProcesses[(int)state]();
	}

    void Ready()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireHookAndRope();
            ChangeState(HookshotState.EXTENDING);
        }
    }

    void Extend() { /* The hook object is traveling through the world. */ }

    void Hooked()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump"))
        {
            DestroyHookAndRope();
            ChangeState(HookshotState.FLYING);
        }

        RotatGunToFaceHook();
    }

    void Flying()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireHookAndRope();
            ChangeState(HookshotState.EXTENDING);
        }

        if (player.isGrounded())
        {
            ChangeState(HookshotState.READY);
        }
    }

    void Retract()
    {
        float t = (Time.time - stateSwitchTime) / retractTime;
        Vector2 targetPosition = transform.position;
        hook.transform.position = Vector3.Lerp(retractPoint, targetPosition, t);
        if (t >= 1.0f)
        {
            DestroyHookAndRope();
            ChangeState(HookshotState.READY);
        }
    }

    void FireHookAndRope()
    {
        // Shoot out a hook instance
        hook = (GameObject)Instantiate(hookFab, transform.position, transform.rotation);
        hook.GetComponent<Hook>().hookGun = this;

        IgnoreHookPlayerCollisions();

        // And spawn a rope to go with it
        GameObject ropeObj = Instantiate(ropeFab);
        rope = ropeObj.GetComponent<RopeControl>();
        rope.hookshot = this;
        rope.hook = hook;
    }

    void IgnoreHookPlayerCollisions()
    {
        Collider2D hookCollider = hook.GetComponent<PolygonCollider2D>();
        foreach(Collider2D playerCollider in playerColliders)
            Physics2D.IgnoreCollision(hookCollider, playerCollider, true);
    }

    void DestroyHookAndRope()
    {
        Destroy(hook);
        if (rope != null) {
            rope.DetachRope();
            Destroy(rope.gameObject);
        }
    }

    public void HookOn()
    {
        ChangeState(HookshotState.HOOKED);
        rope.AttachRope();
    }

    public void RetractRope()
    {
        retractPoint = hook.transform.position;
        hook.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        ChangeState(HookshotState.RETRACTING);
    }

    public bool IsHooked()
    {
        return state == HookshotState.HOOKED; 
    }

    public bool IsFlying()
    {
        return state == HookshotState.FLYING;
    }

    public Vector2 HookPoint()
    {
        return hook.transform.position;
    }

    public void CancelHook()
    {
        if(state != HookshotState.READY)
            DestroyHookAndRope();
        ChangeState(HookshotState.READY);
    }

    void RotatGunToFaceHook()
    {
        hand.transform.rotation = Quaternion.FromToRotation(
            Vector2.right, 
            hook.transform.position - hand.transform.position
        );
    }
}
