using UnityEngine;
using System.Collections;

public class TriggerCheck : MonoBehaviour {
    public HookshotControl hookshot;
    public ParticleSystem death;
    public Transform playerSprite;

    private GameObject lastSpawn;
    private Rigidbody2D reggieBody;

    public float timeForRespawn = 0.1f;

    void Start()
    {
        reggieBody = gameObject.GetComponent<Rigidbody2D>();
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
        if (c.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        StartCoroutine(delayedRespawn());
    }

    IEnumerator delayedRespawn()
    {
        playerSprite.gameObject.SetActive(false);
        reggieBody.isKinematic = true;
        ParticleSystem deathAnimation = Instantiate(death, transform.position, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(timeForRespawn);
        transform.position = lastSpawn.transform.position;
        playerSprite.gameObject.SetActive(true);
        reggieBody.isKinematic = false;
        Destroy(deathAnimation);
        hookshot.CancelHook();
    }
}
