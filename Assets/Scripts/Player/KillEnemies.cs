using UnityEngine;
using System.Collections;
using System.IO;

public class KillEnemies : MonoBehaviour
{
    public float lethalVelocity;
    public float respawnDelay = 0.5f;
    public ParticleSystem deathParticle;
    private GameObject playerSprite;
    private HookshotControl hook;
    private Rigidbody2D player;
    private GameObject lastSpawn;
    private Vector2 deathBoxPos;

    public delegate void RespawnListener();
    public static event RespawnListener OnRespawn;
    void Start()
    {
        playerSprite = GameObject.Find("FrogSprite");
        player = GetComponent<Rigidbody2D>();
        hook = GetComponent<LateralMovement>().getHookScript();
        deathBoxPos = Vector2.down * 5;
    }

    void Update()
    {
        CheckForDeath();
    }

    void CheckForDeath()
    {
        if (transform.position.y < deathBoxPos.y)
        {
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        playerSprite.SetActive(false);
        hook.CancelHook();
        player.velocity = Vector2.zero;
        player.isKinematic = true;
        yield return new WaitForSeconds(respawnDelay);
        player.isKinematic = false;
        transform.position = Vector2.up;
        playerSprite.SetActive(true);
        if (OnRespawn != null)
            OnRespawn();
    }

    private bool Flying()
    {
        return hook.IsHooked() || hook.IsFlying();
    }
}
