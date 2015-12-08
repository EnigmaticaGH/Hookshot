using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static double currentScore = 0;
    private static double totalScore = 0;
    private static double best = 0;
    public Text scoreText;
    public Text bestText;
    void Awake()
    {
        InfiniteRunGenerator.Score += UpdateScore;
        InfiniteRunGenerator.Respawn += UpdateBest;
        bestText.text = "Best: " + best;
    }
    void OnDestroy()
    {
        InfiniteRunGenerator.Score -= UpdateScore;
        InfiniteRunGenerator.Respawn -= UpdateBest;
    }
    void UpdateScore(double score)
    {
        currentScore += score;
        if (currentScore > best) best = currentScore;
        if (currentScore > totalScore) totalScore = currentScore;
        scoreText.text = "Score: " + totalScore.ToString("0");
    }
    void UpdateBest()
    {
        bestText.text = "Best: " + best.ToString("0");
    }
    public static void ResetScore()
    {
        totalScore = 0;
        currentScore = 0;
    }
}
