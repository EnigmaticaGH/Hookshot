using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTracker : MonoBehaviour {

    private static int score;
    private static int best = 0;
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
    void UpdateScore(int score)
    {
        if (score > best) best = score;
        scoreText.text = "Distance: " + score;
    }
    void UpdateBest()
    {
        bestText.text = "Best: " + best;
    }
}
