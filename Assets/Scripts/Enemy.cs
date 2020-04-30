using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    // [SerializeField] private Vector3 _startPos = new Vector3(0, 8, 0);
    [SerializeField] private float _speed;
    [SerializeField] private float _enemyLxBounds = -9.2f;
    [SerializeField] private float _enemyRxBounds = 9.2f;
    [SerializeField] private float _enemyYPosTop = 8.0f;
    [SerializeField] private float _enemyYPosBot = -8.0f;
    private int _pointsAddedEnemyHit = 10;
    private Player _player;
    private Animator _anim;

    // ** Laser fire...
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _laserOffset = 1.1f;
    private int _defaultZero = 0;
    [SerializeField] private float _fireRate = 3.0f;
    [SerializeField] private float _canFire = -1.0f; // to know if we can fire; use this val to compare against how long the game has been running which is Time.time.  As soon as start game Time.time becomes 0;


    // ** Audio...
    // [SerializeField] public AudioClip _explosionSoundClip;
    private SFXManager _sfxManager;


    void Start()
    {
        // transform.position = _startPos;

        // store in cache once so as to make just one get component call
        _player = GameObject.Find("Player").GetComponent<Player>();
        // ** null chk for player
        if (_player == null)
        {
            Debug.LogError("The Player is NULL");
        }

        // ** assign component to anim
        // _anim = GameObject.Find("Anim").GetComponent<Animator(); // don't nn this cuz anim is attached to same GO enemy script is on
        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }

        // grab sfx manager
        _sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManager>();
    }

    void Update()
    {
        CalculateMovement();
        // enemy laser fire every 3-7 sec
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3.0f, 7.0f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            // auto pause unity as soon as enemy spawn an obj (laser)
            // Debug.Break();
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            
            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    void CalculateMovement() // makes it easy to debug
    {
        // move down 4m per sec
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // chk for bottom of screen and respwan at top with a new random x pos
        float _randxPos = Random.Range(_enemyLxBounds, _enemyRxBounds);

        if (transform.position.y < _enemyYPosBot)
        {
            // scrn top y 9; bot y -9
            transform.position = new Vector3(_randxPos, _enemyYPosTop, 0);
        }
    }


    // ** both enemy and laser hv rigidbody; use gravity false
        /**
         * if other is player
         * damage the player
         * destroy us
         * 
         * if other is laser
         * destroy laser
         * destroy us
        **/
    void OnTriggerEnter2D(Collider2D other) // other param is "obj collided w/ Enemy (.this)"
    {
        // Debug.Log("Hit: " + other.transform.name); // finds who collided w/ us player and laser

       // player collision
       if (other.tag == "Player")
       {
           // damage player ea time hit by enemy
           // null chk if other obj that collided w/ us has player component? does the component exist in the 1st plc
           // ** access other obj holds ref to player cuz tagged as player; transform to access root of obj and get the player type or class component.  Get player gameobject.
           // ** here able to access player methods and call it.

        //    other.transform.GetComponent<Player>().Damage();
           Player player = other.transform.GetComponent<Player>();

           if (player != null) // null chk component by storing it in a ref var; so don't get error null ref not set to instc of obj
           {
               player.Damage();
           }

           _anim.SetTrigger("OnEnemyDeath"); // ** trigger anim; type name of trigger which is in Animator params
           _speed = 0;
           _sfxManager.PlaySFX("explosion_sound");
           Destroy(this.gameObject, 1.5f); // destroy enemy if collide with player
       }


        // laser collision; if we hit laser
       if (other.tag == "Laser")
       {
           Destroy(other.gameObject);
           // Access player data
           // Add 10 to score
           // chk if player is alive
           if (_player != null)
           {
               _player.AddScore(_pointsAddedEnemyHit);
           }

           _anim.SetTrigger("OnEnemyDeath"); // ** trigger anim
           _speed = 0;
           _sfxManager.PlaySFX("explosion_sound");
           Destroy(GetComponent<Collider2D>());
           Destroy(this.gameObject, 1.5f);
       }
    }
}

