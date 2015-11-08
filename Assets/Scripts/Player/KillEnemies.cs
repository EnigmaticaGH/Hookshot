using UnityEngine;
using System.Collections;

public class KillEnemies : MonoBehaviour
{
    public float lethalVelocity;
    private HookshotControl hook;
    private Rigidbody2D player;
    private GameObject lastSpawn;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        hook = GetComponent<LateralMovement>().getHookScript();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Enemy") && c.relativeVelocity.magnitude >= lethalVelocity && Flying())
        {
            Destroy(c.gameObject);
        }
        else if (c.collider.CompareTag("Enemy") || c.collider.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Win Condition"))
        {
            c.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (c.CompareTag("Respawn"))
        {
            lastSpawn = c.gameObject;
        }
        if ((c.CompareTag("Boundary") || (c.CompareTag("Water"))) && !hook.IsHooked())
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = lastSpawn.transform.position;
        hook.CancelHook();
        player.velocity = Vector2.zero;
    }

    private bool Flying()
    {
        return hook.IsHooked() || hook.IsFlying();
    }
}
