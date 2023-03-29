using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] int score = 0;
    [SerializeField] int highScore = 0;

    [Header("Game Over")]
    [SerializeField] bool gameOver = false;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            UIManager.instance.UpdateUIScore(score);            
        }
    }

    public int HighScore
    {
        get => highScore;
        set
        {
            if(Score > HighScore)
            {
                highScore = Score;
                UIManager.instance.UpdateUIHighScore(highScore);
            }
        }
    }

    private void Start()
    {
        UIManager.instance.UpdateUIScore(score);
    }
}
