using UnityEngine;
using System.Collections;

public class RelativeVelocity : MonoBehaviour
{
    private Vector2 relative = Vector2.zero;
    void OnCollisionStay2D(Collision2D c)
    {
        relative = c.relativeVelocity;
    }
    void OnCollisionExit2D(Collision2D c)
    {
        relative = Vector2.zero;
    }
    public Vector2 GetRelativeVelocity
    {
        get
        {
            return relative;
        }
    }
}
