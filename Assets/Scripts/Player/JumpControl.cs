using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(KeybindScript))]
public class JumpControl : MonoBehaviour
{
    public float jumpForce;
    public float wallJumpForce;

    private Rigidbody2D body;
    private KeybindScript keybinds;

    private bool jump;
    private bool doubleJump;
    public bool canDoubleJump;

    private bool grounded;

    void Start()
    {
        keybinds = GameObject.FindGameObjectWithTag("KeyBinds").GetComponent<KeybindScript>();
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (keybinds.GetButtonDown("Jump") && Time.timeScale > 0f)
        {
            if (grounded)
            {
                jump = true;
            }
            else if (doubleJump && canDoubleJump)
            {
                jump = true;
                doubleJump = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (jump)
        {
            body.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
    }

    public void SetGrounded(bool flag)
    {
        grounded = flag;
        doubleJump = flag || doubleJump;
    }

    public bool isGrounded()
    {
        return grounded;
    }
}
