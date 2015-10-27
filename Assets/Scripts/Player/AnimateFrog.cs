using UnityEngine;
using System.Collections;

public class AnimateFrog : MonoBehaviour
{
    public JumpControl jump;
    private Rigidbody2D player;
    private LateralMovement movement;
    private Animator anim;
    private Vector2 relativeVel;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<Rigidbody2D>();
        movement = GetComponentInParent<LateralMovement>();
        anim.speed = 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > 0.1f)
        {
            anim.SetBool("Grounded", jump.isGrounded());
            anim.SetBool("Moving", relativeVel.magnitude > 0.01f);
        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        relativeVel = c.relativeVelocity;
    }

    void OnCollisionExit2D(Collision2D c)
    {
        relativeVel = Vector2.zero;
    }
}
