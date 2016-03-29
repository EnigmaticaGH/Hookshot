using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScoreTracker : MonoBehaviour {

    private static int best = 0;
    private static int score;
    private static int maxDistance;
    public Text scoreText;
    public Text bestText;
    public const int digitsInScore = 9;
    public Sprite[] numbers = new Sprite[10];
    public Image[] scoreSlots = new Image[digitsInScore];
    public Image[] highScoreSlots = new Image[digitsInScore];
    public Color scoreBasicColor;
    public Color scoreAltColor;
    private Rigidbody2D body;
    private LateralMovement movement;
    private bool aboveMax = false;

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
            //float scoreMultiplier = Mathf.Pow(Mathf.Clamp(movement.speed / movement.defaultMaxSpeed, 1, Mathf.Infinity), 2);
            //score += body.velocity.x * Time.deltaTime * (1 + Mathf.Pow(player.position.x / 100f, 0.5f)) * scoreMultiplier;
            maxDistance = Mathf.FloorToInt(player.position.x);
            score = maxDistance;
            toScoreUI(scoreToString(score), scoreSlots);
        }

        //scoreText.text = "Score: " + score.ToString("0");

        if (score > best)
        {
            changeColorOfScore(1);
            best = score;
        }
    }
    void UpdateBest()
    {
        toScoreUI(scoreToString(best), highScoreSlots);
        //bestText.text = "Best: " + best.ToString("0");
    }
    public void ResetScore()
    {
        changeColorOfScore(0);
        maxDistance = 0;
        score = 0;
        toScoreUI(scoreToString(0), scoreSlots);
    }

    void toScoreUI(string score, Image[] Slots)
    {
        int sl = Slots.Length;

        for (int b = 0; b < sl; b++)
        {
            //print(score[b]);
            Slots[b].sprite = numbers[charToInt(score[b])];
        }

    }

    int charToInt(char input)
    {
        if ((int)input >= 48 && (int)input <= 57)
        {
            //print(input - 48);
            return input - 48;
        }
        else
            return 0;
    }

    string scoreToString(int score)
    {
        int temp;
        if (score != 0)
            temp = Mathf.FloorToInt(Mathf.Log10(score));
        else
            temp = 0;

        int dif = digitsInScore - temp;
        string result = "";

        if (dif > 0)
        {
            for (int a = 0; a < dif; a++)
            {
                result += "0";
            }
        }

        result += score;
        print(result);
        return result;
    }

    void changeColorOfScore(int election)
    {
        int lenght = scoreSlots.Length;
        Color newColor = scoreAltColor;

        if (election == 0)
            newColor = scoreBasicColor;
        //else if (election == 1)
        //    newColor = scoreAltColor;

        for (int a = 0; a < lenght; a++)
        {
            scoreSlots[a].color = newColor;
        }
    }

}
