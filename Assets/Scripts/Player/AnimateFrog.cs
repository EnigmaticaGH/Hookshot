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
        if(Time.time > 0.1f && !anim.GetBool("WallJump"))
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