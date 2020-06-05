using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    private float _powUpBot = -8.0f;
    /**
     * 0 - triple shot
     * 1 - speed
     * 2 - shields
     * 3 - reload
     * */
    [SerializeField] private int _powerUpID;

    void Update()
    {
        // move down at speed of 3
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // when we leave the screen at bottom destroy this obj
        if (transform.position.y < _powUpBot)
        {
            Destroy(this.gameObject);
        }
    }


    // which powerup getting...
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player")
        {
            // comm w/ player script 
            Player player = other.transform.GetComponent<Player>();

            // nn null chk; successfully grabbed component or we crashed program
            if (player != null) // we found player component
            {
                switch (_powerUpID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;

                    case 1:
                        player.SpeedPowerUpActive();
                        break;

                    case 2:
                        player.ShieldPowerUpActive();
                        break;
                    
                    // case 3:
                    //     player.ReloadAmmoActive();
                    //     break;

                    default:
                        Debug.Log("If none of above cases, see this");
                        break;
                }
            }
            
            Destroy(this.gameObject);
        }
    }
}
