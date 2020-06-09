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


    [Header ("Lives Arr")]
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _liveSprites;


    [Header ("Thrust/Speed")]
    [SerializeField] private float _minThrust = 0.0f;
    [SerializeField] private float _maxThrust = 1.0f;
    [SerializeField] private float _currentThrust;


    [Header ("Slider")]
    [SerializeField] private Slider _slider;

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

        // _currentThrust = _minThrust;
        // _slider.maxValue = _maxThrust;
        // _slider.minValue = _minThrust;
        _slider.value = _minThrust;
    }


    public void UseThrust(float thrustAmt)
    {
        _slider.value = thrustAmt;
    }

    // public void UseThrust(float thrustAmt)
    // {
    //     if (_currentThrust - thrustAmt <= 0)
    //     {
    //         _currentThrust -= thrustAmt;
    //         _slider.value = _currentThrust;

    //         StartCoroutine(DegenThrustRoutine());
    //     }
    //     else
    //     {
    //         // _slider.value = _currentThrust;
    //         Debug.Log("Out of thrust");
    //     }
    // }

    // IEnumerator DegenThrustRoutine()
    // {
    //     yield return new WaitForSeconds(1.0f);

    //     while (_currentThrust < _maxThrust)
    //     {
    //         _currentThrust += _maxThrust / 100;
    //         _slider.value = _currentThrust;
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }


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