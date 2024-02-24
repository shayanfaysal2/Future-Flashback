using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private PlayerController playerController;

    public AudioSource music;
    public AudioSource coinSound;
    public AudioSource gameOverSound;
    public AudioSource enemyGoSound;

    public GameObject redFlashPrefab;
    public GameObject gameOverPanel;
    public GameObject collectEffect;

    public Text scoreText;
    public Text coinsText;
    public Slider livesSlider;

    public int maxLives;
    private float score;
    private int coins;

    [HideInInspector]
    public int lives;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        score = 0;
        coins = 0;
        lives = maxLives;
        livesSlider.maxValue = maxLives;
        livesSlider.value = lives;
    }

    public void UpdateScore(float x)
    {
        score += x;
        scoreText.text = score.ToString("F1");
    }

    public void CollectCoin(Vector3 pos)
    {
        Destroy(Instantiate(collectEffect, pos, Quaternion.identity), 1);
        coinSound.Play();
        coins++;
        coinsText.text = coins.ToString();
    }

    public void LoseLife()
    {
        Handheld.Vibrate();
        Destroy(Instantiate(redFlashPrefab), 0.5f);
        if (lives > 1)
        {
            lives--;
            livesSlider.value = lives;
        }
        else
        {
            playerController.Die();
            Die(); 
        }
    }

    public void Die()
    {
        Handheld.Vibrate();
        Destroy(Instantiate(redFlashPrefab), 0.5f);

        music.Stop();
        gameOverSound.PlayDelayed(0.5f);
        enemyGoSound.Play();
        lives = 0;
        livesSlider.value = 0;
        gameOverPanel.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
