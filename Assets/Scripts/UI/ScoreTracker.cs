using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static int best = 0;
    private static int maxDistance;
    private static float startTime;
    public Text scoreText;
    public Text bestText;

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
        maxDistance = 0;
    }
    void OnDestroy()
    {
        KillEnemies.OnRespawn -= UpdateBest;
        KillEnemies.OnRespawn -= ResetScore;
    }

    void Update()
    {
        int score = 0;
        if (player.position.x > maxDistance) {
            score = Mathf.FloorToInt(player.position.x);
            maxDistance = Mathf.FloorToInt(player.position.x);
        } else
            score = maxDistance;

        score *= Mathf.FloorToInt(distanceScoreFactor);
        score -= Mathf.FloorToInt(timePenaltyFactor * (Time.time - startTime));
        if (score < 0)
            score = 0;
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
        startTime = Time.time;
    }
}
