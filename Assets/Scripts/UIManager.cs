using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Score")]
    [SerializeField] Text scoreText;

    [Header("Score Game Over")]
    [SerializeField] Text scoreTextGameOver;
    [SerializeField] Text highScoreText;

    [Header("Menu")]
    [SerializeField] bool paused = false;
    [SerializeField] GameObject pauseOn;
    [SerializeField] GameObject gameContinue;
    [SerializeField] GameObject gameRestart;
    [SerializeField] GameObject gameMenu;

    [Header("Gameover")]
    [SerializeField] GameObject gameOverScreen;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public AudioSource music;
    public AudioSource fxSound;
    public AudioClip clickDish;
    public AudioClip clickButton;

    [SerializeField] bool fxSoundOn = true;
    [SerializeField] bool musicOn = true;

    public Button musicButton;
    public Button fxSoundButton;

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundDish()
    {
        fxSound.PlayOneShot(clickDish);
    }

    public void PlaySoundButton()
    {
        fxSound.PlayOneShot(clickButton);
    }

    public void MusicVolume()
    {
        musicOn = !musicOn;
        //music.volume = musicOn ? 0 : 1;

        if(musicOn)
        {
            music.mute = false;
            //music.volume = 1f;
            //oscureser el boton.
            musicButton.GetComponent<Button>().image.color = new Color(255, 255, 255, 255);
        }
        else
        {
            music.mute = true;
            //oscureser el boton.
            musicButton.GetComponent<Button>().image.color = new Color(200, 250, 0, 255);
        }
    }

    public void FxSoundVolume()
    {
        fxSoundOn = !fxSoundOn;

        if (fxSoundOn)
        {
            fxSound.mute = false;
            //oscureser el boton.
            fxSoundButton.GetComponent<Button>().image.color = new Color(255, 255, 255, 255);
        }
        else
        {
            fxSound.mute = true;
            //oscureser el boton.
            fxSoundButton.GetComponent<Button>().image.color = new Color(200, 250, 0, 255);
        }
    }

    public void ButtonPause()
    {
        PlaySoundButton();
        //fxSound.Play();
        
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        pauseOn.SetActive(paused);
    }

    public void ButtonContinue()
    {
        PlaySoundButton();

        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        pauseOn.SetActive(paused);
    }

    public void ButtonRestart()
    {
        PlaySoundButton();

        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ButtonMenu()
    {
        PlaySoundButton();

        //SceneManager.LoadScene(changeScene);
        SceneManager.LoadScene("Game");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void GameoverScreen()
    {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        gameOverScreen.SetActive(true);
        
        //finalScoreText.text = " " + GameManager.Instance.Score;
    }

    public void UpdateUIScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdateUIHighScore(int highScore)
    {
        highScoreText.text = highScore.ToString();
    }
}
