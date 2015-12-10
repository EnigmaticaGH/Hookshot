using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static double currentScore = 0;
    private static double totalScore = 0;
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
        InfiniteRunGenerator.Score += UpdateScore;
        InfiniteRunGenerator.Respawn += UpdateBest;
        bestText.text = "Best: " + best;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        maxDistance = 0;
    }
    void OnDestroy()
    {
        InfiniteRunGenerator.Score -= UpdateScore;
        InfiniteRunGenerator.Respawn -= UpdateBest;
    }

    void Update()
    {
        Debug.Log("Update score");
        double score = 0;
        if (player.position.x > maxDistance) {
            score = player.position.x;
            maxDistance = player.position.x;
        } else
            score = maxDistance;

        score *= distanceScoreFactor;
        score -= timePenaltyFactor * (Time.time - startTime);
        scoreText.text = "Score: " + score.ToString("0");
    }
    void UpdateScore(double score)
    {
        /*
        currentScore += score;
        if (currentScore > best) best = currentScore;
        if (currentScore > totalScore) totalScore = currentScore;
        scoreText.text = "Score: " + totalScore.ToString("0");
        */
    }
    void UpdateBest()
    {
        bestText.text = "Best: " + best.ToString("0");
    }
    public static void ResetScore()
    {
        maxDistance = 0;
        totalScore = 0;
        currentScore = 0;
        startTime = Time.time;
    }
}
