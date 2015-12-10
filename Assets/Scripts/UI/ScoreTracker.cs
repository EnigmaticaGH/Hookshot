using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static double best = 0;
    private static double maxDistance;
    private static double startTime;
    public Text scoreText;
    public Text bestText;

    public float distanceScoreFactor;
    public float timePenaltyFactor;

    [HideInInspector]
    public Transform player;
    void Awake()
    {
        InfiniteRunGenerator.Respawn += UpdateBest;
        bestText.text = "Best: " + best;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        maxDistance = 0;
    }
    void OnDestroy()
    {
        InfiniteRunGenerator.Respawn -= UpdateBest;
    }

    void Update()
    {
        double score = 0;
        if (player.position.x > maxDistance) {
            score = player.position.x;
            maxDistance = player.position.x;
        } else
            score = maxDistance;

        score *= distanceScoreFactor;
        score -= timePenaltyFactor * (Time.time - startTime);
        if (score < 0)
            score = 0;
        scoreText.text = "Score: " + score.ToString("0");

        if (score > best) best = score;
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
