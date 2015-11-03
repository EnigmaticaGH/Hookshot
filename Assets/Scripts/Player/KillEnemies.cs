using UnityEngine;
using System.Collections;

public class KillEnemies : MonoBehaviour
{
    public float lethalVelocity;
    public float respawnDelay = 0.4f;
    public Transform playersprite;
    public ParticleSystem DeathParticle;
    public HookshotControl hook;
    private Rigidbody2D player;
    private GameObject lastSpawn;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (c.collider.CompareTag("Enemy") && c.relativeVelocity.magnitude >= lethalVelocity && Flying())
        {
            Destroy(c.gameObject);
        }
        else if (c.collider.CompareTag("Enemy"))
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
        if (c.CompareTag("Boundary") || c.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        hook.CancelHook();
        playersprite.gameObject.SetActive(false);
        player.velocity = Vector2.zero;
        player.isKinematic = true;
        ParticleSystem Death = Instantiate(DeathParticle, transform.position, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(respawnDelay);
        transform.position = lastSpawn.transform.position;
        playersprite.gameObject.SetActive(true);
        player.isKinematic = false;
        Destroy(Death);
    }

    private bool Flying()
    {
        return hook.IsHooked() || hook.IsFlying();
    }
}
