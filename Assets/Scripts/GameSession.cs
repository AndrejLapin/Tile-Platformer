using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;

    [SerializeField] Text livesText;
    [SerializeField] Text scoreText;
    [SerializeField] Text healthText;

    Player player;

    private void Awake()
    {
        MakeSingleton();
    }

    

    // Start is called before the first frame update
    void Start()
    { 
        player = FindObjectOfType<Player>();
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
        UpdatePlayerHealth();
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
            UpdatePlayerHealth();
        }
        else
        {
            ResetGameSession();
        }
    }

    public void UpdatePlayerHealth()
    {
        healthText.text = player.GetCurrentHealth().ToString() + "/" + player.GetMaxHealth().ToString();
        //healthText.text = FindObjectOfType<Player>().GetCurrentHealth().ToString() + "/" + FindObjectOfType<Player>().GetMaxHealth().ToString();
    }

    private void MakeSingleton()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    private void TakeLife()
    {
        playerLives--;
        livesText.text = playerLives.ToString();
    }

}
