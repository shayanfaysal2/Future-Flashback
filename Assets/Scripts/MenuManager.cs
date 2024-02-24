using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    //public Slider volumeSlider;

    public Image soundButton;
    public Sprite[] soundSprites;

    public Slider difficultySlider;

    // Start is called before the first frame update
    void Start()
    {
        int sound = PlayerPrefs.GetInt("sound", 1);
        soundButton.sprite = soundSprites[sound];
        AudioListener.volume = sound;

        int dif = PlayerPrefs.GetInt("difficulty", 0);
        difficultySlider.value = dif;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ToggleSound()
    {
        if (AudioListener.volume == 1)
        {
            AudioListener.volume = 0;
            soundButton.sprite = soundSprites[0];
            PlayerPrefs.SetInt("sound", 0);
        }
        else
        {
            AudioListener.volume = 1;
            soundButton.sprite = soundSprites[1];
            PlayerPrefs.SetInt("sound", 1);
        }
    }

    public void SetVolume(float x)
    {
        AudioListener.volume = x;
        PlayerPrefs.SetFloat("volume", x);
    }

    public void SetDifficulty(float x)
    {
        int d = (int)x;
        PlayerPrefs.SetInt("difficulty", d);
    }
}