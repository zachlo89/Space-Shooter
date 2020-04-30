using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Asteroid : MonoBehaviour
{
    // public GameObject asteroidPrefab;
    [SerializeField] private float _rotSpeed = 3.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    // ** Audio...
    [SerializeField] private SFXManager _sfxManager;



    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        _sfxManager = GameObject.Find("SFXManager").GetComponent<SFXManager>();
    }

    void Update()
    {
        transform.Rotate(0, 0, _rotSpeed * Time.deltaTime); // pass in vector3 euler
        // transform.Rotate(-Vector3.forward * _rotSpeed * Time.deltaTime);
    }

    // chk for laser collision of type trigger
    // instantiate explosion at the pos of the asteroid (us/my position)
    // destory explosion after 1 sec
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("Hit: " + other.transform.name);
        if (other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning(); // no enemies or powerups spawning until asteroid is destoyed.
            _sfxManager.PlaySFX("explosion_sound");
            Destroy(this.gameObject, 0.25f);
        }
    }
}
