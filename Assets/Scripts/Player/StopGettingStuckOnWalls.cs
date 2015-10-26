using UnityEngine;
using System.Collections;

public class StopGettingStuckOnWalls : MonoBehaviour
{
    private JumpControl jump;
    public HookshotControl hook;
    private BoxCollider2D box;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D noFriction;
    // Use this for initialization
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        jump = GetComponentInParent<JumpControl>();
    }

    // Update is called once per frame
    void Update()
    {
        box.sharedMaterial = IsGrounded() ? friction : noFriction;
    }

    private bool IsGrounded()
    {
        return !hook.IsHooked() && jump.isGrounded();
    }
}
