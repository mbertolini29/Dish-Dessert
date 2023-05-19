using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Level")]
    [SerializeField] int numLevel = 0;

    [Header("Amount Dessert")]
    public int numCupcakes;
    public int numDonut;

    [Header("Score")]
    [SerializeField] int score = 0;
    [SerializeField] int highScore = 0;

    [Header("Game Over")]
    [SerializeField] bool gameOver = false;

    public int NumLevel
    {
        get => numLevel;
        set
        {
            numLevel = value;

            //llamar a tutorial, para cambiar el nombre y objetivo?
            TutorialManager.instance.ChangeTextLevel(numLevel);
        }
    }

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UIManager.instance.UpdateUIScore(score);
    }

    public void CheckLevels()
    {
        //primero objetivo 
        //llegar a completar 3 cupcakes y 3 donas.

        //como almaceno eso? 

        if(numCupcakes >= 3 && numDonut >= 3)
        {
            //etapa 2 completa.

            NumLevel++;

            //pasar a un nueva etapa. Etapa 3 (Cinnamon roll is added) (Clear 5 different orders)

        }

    }


}
