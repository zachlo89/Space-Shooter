using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText; // create handle to text
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    private int _defaultScore = 0;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _liveSprites;
    private GameManager _gameManager;


    void Start()
    {
        _scoreText.text = "Score: " + _defaultScore;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_gameManager == null) // null chking when get component
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    // call into UI manager
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        // access display img sprite
        // give it a new one the curr lives index
        _livesImage.sprite = _liveSprites[currentLives];

        // if the curr lives val is zero then game over
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }


    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }


    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.2f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.2f);
        }
    }
}