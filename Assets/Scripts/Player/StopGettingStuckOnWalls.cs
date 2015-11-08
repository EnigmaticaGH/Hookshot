using UnityEngine;
using System.Collections;

public class StopGettingStuckOnWalls : MonoBehaviour
{
    private JumpControl jump;
    private HookshotControl hook;
    private PolygonCollider2D playerCollider;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D noFriction;
    private PhysicsMaterial2D temp;
    // Use this for initialization
    void Start()
    {
        hook = GetComponentInParent<LateralMovement>().getHookScript();
        jump = GetComponentInParent<JumpControl>();
    }

    // Update is called once per frame
    void Update()
    {
        playerCollider = GetComponent<PolygonCollider2D>();
        temp = playerCollider.sharedMaterial;
        playerCollider.sharedMaterial = IsGrounded() ? friction : noFriction;

        if (playerCollider.sharedMaterial != temp)
        {
            //Force an update to the collider's physics material
            playerCollider.enabled = false;
            playerCollider.enabled = true;
        }
    }

    private bool IsGrounded()
    {
        return !hook.IsHooked() && jump.isGrounded();
    }
}
