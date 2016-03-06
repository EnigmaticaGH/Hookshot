using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static float best = 0;
    private static float score;
    private static int maxDistance;
    public Text scoreText;
    public Text bestText;
    private Rigidbody2D body;
    private LateralMovement movement;

    public float distanceScoreFactor;
    public float timePenaltyFactor;

    [HideInInspector]
    public Transform player;
    void Awake()
    {
        KillEnemies.OnRespawn += UpdateBest;
        KillEnemies.OnRespawn += ResetScore;
        bestText.text = "Best: " + best;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        score = 0;
        maxDistance = 0;
        body = player.GetComponent<Rigidbody2D>();
        movement = player.GetComponent<LateralMovement>();
    }
    void OnDestroy()
    {
        KillEnemies.OnRespawn -= UpdateBest;
        KillEnemies.OnRespawn -= ResetScore;
    }

    void Update()
    {
        if (player.position.x > maxDistance && body.velocity.x > 0)
        {
            float scoreMultiplier = Mathf.Pow(Mathf.Clamp(movement.speed / movement.defaultMaxSpeed, 1, Mathf.Infinity), 2);
            score += body.velocity.x * Time.deltaTime * (1 + Mathf.Pow(player.position.x / 100f, 0.5f)) * scoreMultiplier;
            maxDistance = Mathf.FloorToInt(player.position.x);
        }

        scoreText.text = "Score: " + score.ToString("0");

        if (score > best) 
            best = score;
    }
    void UpdateBest()
    {
        bestText.text = "Best: " + best.ToString("0");
    }
    public static void ResetScore()
    {
        maxDistance = 0;
        score = 0;
    }
}
