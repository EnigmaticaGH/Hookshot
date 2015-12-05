using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StopGettingStuckOnWalls : MonoBehaviour
{
    private BoxCollider2D[] GroundColliders;
    private BoxCollider2D[] JumpColliders;
    private BoxCollider2D[] HookColliders;
    private List<BoxCollider2D> playerColliders;
    public PhysicsMaterial2D friction;
    public PhysicsMaterial2D noFriction;
    // Use this for initialization
    void Start()
    {
        LateralMovement.GroundState += Ground;
        LateralMovement.JumpState += Air;
        LateralMovement.HookState += Air;
        GroundColliders = GameObject.Find("GroundColliders").GetComponents<BoxCollider2D>();
        JumpColliders = GameObject.Find("JumpColliders").GetComponents<BoxCollider2D>();
        HookColliders = GameObject.Find("HookColliders").GetComponents<BoxCollider2D>();
        playerColliders = new List<BoxCollider2D>();
        playerColliders.AddRange(GroundColliders);
        playerColliders.AddRange(JumpColliders);
        playerColliders.AddRange(HookColliders);
        Ground();
    }

    void OnDestroy()
    {
        LateralMovement.GroundState -= Ground;
        LateralMovement.JumpState -= Air;
        LateralMovement.HookState -= Air;
    }

    void Ground()
    {
        foreach(BoxCollider2D c in playerColliders)
        {
            c.sharedMaterial = friction;
            //Force an update to the collider's physics material
            c.enabled = false;
            c.enabled = true;
        }
    }
    void Air()
    {
        foreach (BoxCollider2D c in playerColliders)
        {
            c.sharedMaterial = noFriction;
            //Force an update to the collider's physics material
            c.enabled = false;
            c.enabled = true;
        }
    }
}
