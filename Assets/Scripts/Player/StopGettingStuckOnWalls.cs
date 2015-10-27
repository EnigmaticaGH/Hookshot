using UnityEngine;
using System.Collections;

public class StopGettingStuckOnWalls : MonoBehaviour
{
    private JumpControl jump;
    public HookshotControl hook;
    private BoxCollider2D box;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D temp;
    // Use this for initialization
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        jump = GetComponentInParent<JumpControl>();
    }

    // Update is called once per frame
    void Update()
    {
        temp = box.sharedMaterial;
        box.sharedMaterial = IsGrounded() ? friction : noFriction;

        if (box.sharedMaterial != temp)
        {
            //Force an update to the collider's physics material
            box.enabled = false;
            box.enabled = true;
        }
    }

    private bool IsGrounded()
    {
        return !hook.IsHooked() && jump.isGrounded();
    }
}
