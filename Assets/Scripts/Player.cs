using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos = new Vector3(0, 0, 0);
    [SerializeField] private float _speed;
    [SerializeField] private float _speedMultiplier = 2;

    [Header ("PREFABS")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleShotPrefab;
    [SerializeField] private GameObject _shieldPrefab;
    [SerializeField] private GameObject _leftEngineFire, _rightEngineFire;


    [Header ("LIFE")]
    [SerializeField] private int _lives = 3;


    // for testing serialize
    [Header ("POWERUP BOOLS")]
    [SerializeField] private bool _isTripleShotActive = false; 
    [SerializeField] private bool _isSpeedPowerUpActive = false;
    [SerializeField] private bool _isShieldPowerUpActive = false;
    [SerializeField] private bool _isReloadAmmoActive = false;
    [SerializeField] private bool _isHealthActive = false;
    


    // *** LASER ***********************************************************
    [Header ("LASER")]
    [SerializeField] private int _maxAmmoCount = 15;
    [SerializeField] private int _currentAmmoCount;
    private float _canFire = -1.0f; // cool down delay; this var determines if we can fire.
    [SerializeField] private float _fireRate = 0.5f; // cool down delay
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

    // ** CAM Shake
    [SerializeField] private CamShaker _camShake;


    // private Renderer cachedShieldColorComponent;




    void Start()
    {
        transform.position = _startPos;

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL");
        }

        _sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManager>();
        if (_sfxManager == null)
        {
            Debug.LogError("The SFX Manager is NULL");
        }

        // var cachedShieldColorComponent = _shieldPrefab.GetComponent<Renderer>();

        _currentAmmoCount = _maxAmmoCount;
    }


    void Update()
    {
        CalculateMovement();

        // _laserOffset = new Vector3(0.0f, 1.0f, 0.0f);
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire) // cool down fire delay
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



    void FireLaser() //***********************************************
    {
        _canFire = Time.time + _fireRate;

        _currentAmmoCount--;

        if (_currentAmmoCount <= 0)
        {
            _currentAmmoCount = 0;
            _sfxManager.PlaySFX("ReloadRemix");
            return;
        }

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

        StartCoroutine(_camShake.Shake(0.15f, 0.4f));

        if (_lives == 2)
        {
            _shieldPrefab.GetComponent<Renderer>().material.color = Color.yellow;
            // _colorToChange = _cachedShieldColorComponent.material.color.g;
            // cachedShieldColorComponent.material.SetColor("Color", Color.yellow);
            _rightEngineFire.SetActive(true);

            // if (_isHealthActive == true)
            // {
            //     _rightEngineFire.SetActive(false);
            // }
        }

        else if (_lives == 1)
        {
            _shieldPrefab.GetComponent<Renderer>().material.color = Color.red;
            _leftEngineFire.SetActive(true);

            if (_isHealthActive == true)
            {
                _leftEngineFire.SetActive(false);
            }
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.onPlayerDeath();
            _sfxManager.PlaySFX("explosion_sound");
            Destroy(this.gameObject);
        }
    
    }


    // *** HEALTH POWERUP ***  ******************************************
    public void HealthPowerUpActive()
    {
        _isHealthActive = true;
        _sfxManager.PlaySFX("power_up_sound");
        StartCoroutine(HealthCoolDownRoutine());
    }
    IEnumerator HealthCoolDownRoutine()
    {
        yield return new WaitForSeconds(0.5f); // 5
        AddLife();
        if (_isHealthActive == true)
        {
            _rightEngineFire.SetActive(false);
            _leftEngineFire.SetActive(false);
        }
        // _isHealthActive = false;
    }
    // ******************************************



    // *** RELOAD AMMO POWERUP ***
    public void ReloadAmmoActive() 
    {
        _isReloadAmmoActive = true;
        // Debug.Log("Reloading...");
        StartCoroutine(ReloadCoolDownRoutine());
    }
    IEnumerator ReloadCoolDownRoutine()
    {
        while (_currentAmmoCount == 0)
        {
            yield return new WaitForSeconds(0.5f);
            _currentAmmoCount = _maxAmmoCount;
        }
    }
    // ******************************************



    // *** TRIPLE SHOT POWERUP ***
    public void TripleShotActive() 
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
    // ******************************************



    // *** SPEED POWERUP ***
    public void SpeedPowerUpActive()
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
    // ******************************************
    


    // *** SHIELD POWERUP; no need cooldown ***
    public void ShieldPowerUpActive() // 
    {
        _isShieldPowerUpActive = true;
        _sfxManager.PlaySFX("power_up_sound");
        _shieldPrefab.SetActive(true);
    }
    // ******************************************


    public void AddLife()
    {
        _lives += 1;
        _uiManager.UpdateLives(_lives);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
