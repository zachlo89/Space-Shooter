using System.Collections;
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
        // user input use the a,d or arrow keys to move L/R; becomes 1 or - 1 in translate code below
        // assign to input manager
        float _horizontalInput = Input.GetAxis("Horizontal");
        float _verticalInput = Input.GetAxis("Vertical");

        // transform.Translate(Vector3.right);
        // transform.Translate(new Vector3(1, 0, 0)); // same as above

        // to slow it down converts from frame based to real time; Time.deltaTime considered as 1 sec; time in sec it took complete the last fr to curr fr
        // moves 1m/sec
        // transform.Translate(Vector3.right * Time.deltaTime);

        // moves 5m/sec
        // new Vector3(1, 0, 0) * 1 * 3 * real time; distributive property => (5,0,0) after mult by 5
        // transform.Translate(Vector3.right * _horizontalInput * _speed * Time.deltaTime);
        // transform.Translate(Vector3.up * _verticalInput * _speed * Time.deltaTime);

        // combine into one line for optimization; same as above two lines
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
        // reassign canfire to how long game has been running + fire rate
        // means time.time IS NOT going to be T for at least whatever fire rate is equal to.
        _canfire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            // + new Vector3(_laserOffset, _defaultZero , _defaultZero)
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            // spawn obj, define pos, and rot; ** AT RUNTIME instantiate a prefab
            // offset laser instantiate
            Instantiate(_laserPrefab, transform.position + new Vector3(_defaultZero, _laserOffset, _defaultZero), Quaternion.identity);
        }
    }

    public void Damage()
    {
        // if shield is active
        // player invincible for one hit
        if (_isShieldPowerUpActive == true)
        {
            // de-activate shields
            // to allow next time get hit deduct life
            _isShieldPowerUpActive = false;

            // ** disable shield visualizer
            _shieldPrefab.SetActive(false);

            // do nothing...leave this method
            // return; keyword
            return;
        }

        _lives--;
        // if lives is 2 enable right engine
        if (_lives == 2)
        {
            _rightEngineFire.SetActive(true);
        }
        // else if lives is 1 enable left engine
        else if (_lives == 1)
        {
            _leftEngineFire.SetActive(true);
        }

        // call update method in UI manager
        _uiManager.UpdateLives(_lives);

        // chk if dead
        // destroy us if are
        if (_lives < 1)
        {
            // comm w/ spawn manager; stop spawning once player dies
            _spawnManager.onPlayerDeath();
            _sfxManager.PlaySFX("explosion_sound");
            Destroy(this.gameObject); // the player
        }
    }


    public void TripleShotActive() // ** TRIPLE SHOT POWERUP
    {
        // triple shot active becomes true
        _isTripleShotActive = true;
        _sfxManager.PlaySFX("power_up_sound");

        // start the power down coroutine for triple shot
        StartCoroutine(TripleShotCoolDownRoutine());
    }
    // Ienumerator TripleShotPowerDownRoutine
    // wait 5 sec
    // set the triple shot to false
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
        // get component
        _uiManager.UpdateScore(_score); // pass in curr val of score
    }
}
