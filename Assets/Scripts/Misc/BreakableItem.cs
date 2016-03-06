using UnityEngine;
using System.Collections;

public class BreakableItem : MonoBehaviour {
    public float breakableForce;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            // Show Particles and Shit as well
            if(HasEnoughMomentum(other.collider.gameObject))
                GameObject.Destroy(this);
        }
    }

    bool HasEnoughMomentum(GameObject player)
    {
        Rigidbody2D playerBody = player.GetComponent<Rigidbody2D>();
        float xVelocity = playerBody.velocity.x;
        return xVelocity * playerBody.mass >= breakableForce;
    }
}
