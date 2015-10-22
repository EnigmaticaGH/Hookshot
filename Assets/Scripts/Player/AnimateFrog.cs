using UnityEngine;
using System.Collections;

public class AnimateFrog : MonoBehaviour
{
    public JumpControl jump;
    private Rigidbody2D player;
    private LateralMovement movement;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<Rigidbody2D>();
        movement = GetComponentInParent<LateralMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > 0.1f)
        {
            anim.SetBool("Grounded", jump.isGrounded());
            anim.SetBool("Moving", player.velocity.magnitude > 0.01f);
        }
    }
}
