using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreTracker : MonoBehaviour {

    private static int score;
    void Awake()
    {
        InfiniteRunGenerator.Score += UpdateScore;
    }
    void OnDestroy()
    {
        InfiniteRunGenerator.Score -= UpdateScore;
    }
    void UpdateScore(int score)
    {
        GetComponent<Text>().text = "Distance: " + score + "\nBest: ";
    }
}
