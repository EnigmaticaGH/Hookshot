using UnityEngine;
using System.Collections;

public class AnimateFrog : MonoBehaviour
{
    private LateralMovement player;
    private Animator anim;
    private float horizontal;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<LateralMovement>();
        anim = GetComponent<Animator>();
        KillEnemies.OnRespawn += ResetAnim;
    }

    void OnDestroy()
    {
        KillEnemies.OnRespawn -= ResetAnim;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Time.time > 0.01f && !anim.GetBool("WallJump"))
        {
            anim.SetBool("Grounded", player.isGrounded());
            anim.SetBool("Moving", player.RelativeVelocity().magnitude > 0.01f);
            anim.SetFloat("Horizontal", horizontal);
        }
    }

    void ResetAnim()
    {
        anim.SetBool("Grounded", true);
        anim.SetBool("Moving", false);
        anim.SetFloat("Horizontal", 1);
    }

    public void PlayNextAnimation()
    {
        string dir = Input.GetAxis("Horizontal") > 0 ? "R" : "L";
        string animation = player.isGrounded() ? "Move" : "Jump";
        anim.SetBool("Hooked", false);
        anim.Play(animation + " Animation" + dir, 0, 0.1f);
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

        while (player.isHookedOrExtending() && !player.isGrounded())
            yield return null;

        anim.SetBool("Hooked", false);
    }
}