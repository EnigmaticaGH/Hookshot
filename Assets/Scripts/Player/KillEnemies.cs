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

    public delegate void RespawnAction();
    public static event RespawnAction OnRespawn;

    void Start()
    {
        InfiniteRunGenerator.Respawn += Respawn;
        playerSprite = GameObject.Find("FrogSprite");
        player = GetComponent<Rigidbody2D>();
        hook = GetComponent<LateralMovement>().getHookScript();
    }

    void OnDestroy()
    {
        InfiniteRunGenerator.Respawn -= Respawn;
    }

    private void Respawn()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        playerSprite.SetActive(false);
        hook.CancelHook();
        player.velocity = Vector2.zero;
        player.isKinematic = true;
        //ParticleSystem Death = Instantiate(deathParticle, transform.position, Quaternion.identity) as ParticleSystem;
        yield return new WaitForSeconds(respawnDelay);
        player.isKinematic = false;
        transform.position = Vector2.up;
        playerSprite.SetActive(true);
        //Destroy(Death.gameObject);
        if(OnRespawn != null)
            OnRespawn();
    }

    private bool Flying()
    {
        return hook.IsHooked() || hook.IsFlying();
    }
}
