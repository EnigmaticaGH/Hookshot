using UnityEngine;
using System.Collections;

public class AnimateFrog : MonoBehaviour
{
    private LateralMovement player;
    private Animator anim;
    private Vector2 relativeVel;
    private float horizontal;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<LateralMovement>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > 0.1f && !anim.GetBool("WallJump"))
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            anim.SetBool("Grounded", player.isGrounded());
            anim.SetBool("Moving", relativeVel.magnitude > 0.01f);
            anim.SetFloat("Horizontal", horizontal);
        }
    }

    void OnCollisionStay2D(Collision2D c)
    {
        relativeVel = c.relativeVelocity;
    }

    public IEnumerator PlayWallJump()
    {
        string dir = player.getWallSensors()[0].IsWallCollide() ? "R" : "L";
        anim.Play("Wall Jump" + dir, 0, 0);
        anim.SetBool("WallJump", true);

        yield return new WaitForSeconds(0.25f);

        anim.SetBool("WallJump", false);
    }

    public IEnumerator PlayHooked()
    {
        anim.Play("Hooked", 0, 0);
        anim.SetBool("Hooked", true);

        while (player.isHookedOrExtending())
            yield return null;

        anim.SetBool("Hooked", false);
    }
}