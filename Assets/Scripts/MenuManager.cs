using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public AudioSource music;
    public AudioSource fxSound;
    //public AudioClip clickDish;
    public AudioClip clickButton;

    //[SerializeField] bool fxSoundOn = true;
    //[SerializeField] bool musicOn = true;

    public Button musicButton;
    public Button fxSoundButton;

    public void PlaySoundButton()
    {
        fxSound.PlayOneShot(clickButton);
    }

    public void Play()
    {
        PlaySoundButton();

        //SceneManager.LoadScene(changeScene);
        SceneManager.LoadScene("Game");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }


}
