using UnityEngine;
using System.Collections;

public class AnimateFrog : MonoBehaviour
{
    private LateralMovement player;
    private Animator anim;
    private Vector2 relativeVel;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<LateralMovement>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > 0.1f)
        {
            anim.SetBool("Grounded", player.isGrounded());
            anim.SetBool("Moving", relativeVel.magnitude > 0.01f);
            anim.SetFloat("Horizontal", Input.GetAxisRaw("Horizontal"));
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