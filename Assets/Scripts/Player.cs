﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos = new Vector3(0, 0, 0);
    [SerializeField] private float _speed;
    [SerializeField] private float _speedMultiplier = 2;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldPrefab;
    [SerializeField] private GameObject _leftEngineFire, _rightEngineFire;

    [SerializeField] private float _fireRate = 0.5f; // cool down delay
    [SerializeField] private int _lives = 3;

    [SerializeField] private bool _isTripleShotActive = false; // for testing serialize
    [SerializeField] private bool _isSpeedPowerUpActive = false;
    [SerializeField] private bool _isShieldPowerUpActive = false;
    
    private float _canfire = -1.0f; // cool down delay; this var determines if we can fire.
    private float _laserOffset = 1.1f;
    private float _leftBounds = -11.0f;
    private float _rightBounds = 11.0f;
    private int _defaultZero = 0;
    
    private SpawnManager _spawnManager; //  ** 1 create ref for when player dies
    private float _secToWait = 5.0f;


    // ** Score system...
    [SerializeField] private int _score = 0;
    
    
    private UIManager _uiManager; // create handle to obj we want; find it, cache it.

    // ** Audio...
    [SerializeField] private SFXManager _sfxManager;



    void Start()
    {
        transform.position = _startPos;

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }
        
        if (_sfxManager == null)
        {
            Debug.LogError("The SFX Manager is NULL");
        }
    }


    void Update()
    {
        CalculateMovement();

        // _laserOffset = new Vector3(0.0f, 1.0f, 0.0f);
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canfire) // cool down fire delay
        {
            FireLaser();
        }
    }


    void CalculateMovement()
    {
        float _horizontalInput = Input.GetAxis("Horizontal");
        float _verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(_horizontalInput, _verticalInput, _defaultZero);

        if (_isSpeedPowerUpActive == false)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.LeftShift) || _isSpeedPowerUpActive == true)
        {
            transform.Translate(direction * _speed * _speedMultiplier * Time.deltaTime);
        }

        // ** Restrain player; first chk inspector to see the x where player goes offscreen
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, _defaultZero), _defaultZero);


        if (transform.position.x > _rightBounds)
        {
            transform.position = new Vector3(_leftBounds, transform.position.y, _defaultZero);
        }
        else if (transform.position.x < _leftBounds)
        {
            transform.position = new Vector3(_rightBounds, transform.position.y, _defaultZero);
        }
    }


    void FireLaser()
    {
        _canfire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(_defaultZero, _laserOffset, _defaultZero), Quaternion.identity);
        }
    }

    public void Damage()
    {
        _lives--;

        if (_lives == 2)
        {
            _shieldPrefab.GetComponent<Renderer>().material.color = Color.yellow;
            _rightEngineFire.SetActive(true);
        }

        else if (_lives == 1)
        {
            _shieldPrefab.GetComponent<Renderer>().material.color = Color.red;
            _leftEngineFire.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.onPlayerDeath();
            _sfxManager.PlaySFX("explosion_sound");
            Destroy(this.gameObject);
        }
    }


    public void TripleShotActive() // ** TRIPLE SHOT POWERUP
    {
        _isTripleShotActive = true;
        _sfxManager.PlaySFX("power_up_sound");

        StartCoroutine(TripleShotCoolDownRoutine());
    }


    IEnumerator TripleShotCoolDownRoutine()
    {
        yield return new WaitForSeconds(_secToWait);
        _isTripleShotActive = false;
    }


    public void SpeedPowerUpActive() // ** SPEED POWERUP
    {
        _isSpeedPowerUpActive = true;
        _sfxManager.PlaySFX("power_up_sound");
        StartCoroutine(SpeedPowerUpCoolDownRoutine());
    }
    IEnumerator SpeedPowerUpCoolDownRoutine()
    {
        yield return new WaitForSeconds(_secToWait);
        _isSpeedPowerUpActive = false;
    }


    public void ShieldPowerUpActive() // ** SHIELD POWERUP; no need cooldown
    {
        _isShieldPowerUpActive = true;
        _sfxManager.PlaySFX("power_up_sound");
        _shieldPrefab.SetActive(true);
    }


    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
