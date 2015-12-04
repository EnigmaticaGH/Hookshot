using UnityEngine;
using System.Collections;

public class KillEnemies : MonoBehaviour
{
    public float lethalVelocity;
    public float respawnDelay = 0.5f;
    public ParticleSystem deathParticle;
    private Transform playerSprite;
    private HookshotControl hook;
    private Rigidbody2D player;
    private GameObject lastSpawn;

    public delegate void RespawnAction();
    public static event RespawnAction OnRespawn;

    void Start()
    {
        playerSprite = gameObject.transform.Find("FrogSprite");
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
        if ((c.CompareTag("Boundary")))
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
        playerSprite.gameObject.SetActive(false);
        player.velocity = Vector2.zero;
        player.isKinematic = true;
        ParticleSystem Death = Instantiate(deathParticle, transform.position, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(respawnDelay);
        transform.position = lastSpawn.transform.position;
        playerSprite.gameObject.SetActive(true);
        player.isKinematic = false;
        Destroy(Death.gameObject);
        OnRespawn();
    }

    private bool Flying()
    {
        return hook.IsHooked() || hook.IsFlying();
    }
}
