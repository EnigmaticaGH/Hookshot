using UnityEngine;
using System.Collections;

public class WaterParticleEffector : MonoBehaviour {
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        ApplyForce(collider);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        ApplyForce(collider);
    }

    void ApplyForce(Collider2D collider)
    {
        if (collider.CompareTag("Water") && collider.GetComponent<Rigidbody2D>() != null)
            return;

        Rigidbody2D otherBody = collider.GetComponent<Rigidbody2D>();
        Vector2 vel = otherBody.velocity;
        float mass = otherBody.mass;

        float yForce = vel.y * vel.y * mass * 0.5f; // Impulse = 1/2 * m * v^2
        body.AddForce(new Vector2(0.0f, -yForce));
    }
}
