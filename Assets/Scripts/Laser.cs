using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Laser : MonoBehaviour
{
    // speed for laser 8
    [SerializeField] private float _speed = 7.0f;
    [SerializeField] private float _laserOutBounds = 8.0f;
    private bool _isEnemyLaser = false; // is this enemy or player laser


    void Update()
    {
        if (_isEnemyLaser == false) // player laser
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
    }


    // we move up if it's a player laser and down when enemy
    void MoveUp()
    {
        // translate laser up via sec
        // whatever num speed is number of meters/sec u move.
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y > _laserOutBounds)
        {
            // chk if this obj has a parent
            if (transform.parent != null)
            {
                // destroy the parent; and children destroyed as well
                Destroy(transform.parent.gameObject); // GO of parent
            }
            // otherwise it's normal laser and we destory this GO...
            Destroy(this.gameObject); // obj this script is attached to
            // Destroy(this.gameObject, 5.0f); // destorys GO in a given time
        }
    }

    // we move up if it's a player laser and down when enemy
    void MoveDown() 
    {
        // translate laser up via sec
        // whatever num speed is number of meters/sec u move.
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -_laserOutBounds)
        {
            // chk if this obj has a parent
            if (transform.parent != null)
            {
                // destroy the parent; and children destroyed as well
                Destroy(transform.parent.gameObject); // GO of parent
            }
            // otherwise it's normal laser and we destory this GO...
            Destroy(this.gameObject); // obj this script is attached to
            // Destroy(this.gameObject, 5.0f); // destorys GO in a given time
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    // chk if we collided with player; for enemy laser
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true) // means enemy laser hit us
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }
    }
}
