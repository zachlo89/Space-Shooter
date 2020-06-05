using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float _secToWaitEnemySpawn = 2.5f;
    [SerializeField] private float _secToWaitPowerUpSpawn;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerUps;
    private bool _stopEnemySpawning = false;
    private bool _stopPowerUpSpawning = false;



    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }


    // spawn GO every 5 secs
    // create coroutine of type IEnumerator allows us to yield events
    IEnumerator SpawnEnemyRoutine()
    {
        /**
         * while loop infinite - always inside coroutine; so we can use the yield event
         * allows computer to breath and do this forever.
         * Instantiate obj - enemy prefab
         * when want to run code again e.g. after 5 sec
         * */

        // yield return null; // waits one frame, then next line is called

        yield return new WaitForSeconds(3.0f); // wait to get into while loop; to give us time after destroy asteroid for enemies to attack

        while (_stopEnemySpawning == false)
        {
            // run below...like creating point to create platforms in endless runner
            Vector3 posToSpawnEnemy = new Vector3(Random.Range(-9.2f, 9.2f), 8, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawnEnemy, Quaternion.identity);

            // assign with same types of type transform; parent is of type Transform
            // can access object through newEnemy
            // get parent obj of gameobj instantiated and assign new parent
            newEnemy.transform.parent = _enemyContainer.transform;

            // wait 5 sec then back up two lines...repeat
            yield return new WaitForSeconds(_secToWaitEnemySpawn);
        }
        // ** WE NEVER GET HERE CUZ NEVER EXIT WHILE LOOP
    }

    IEnumerator SpawnPowerUpRoutine()
    {
        /**
         * every 3-7 sec spawn new powerup on rand X 8 on Y
         * */
        float secToWaitPowerUpSpawn = Random.Range(3, 8);
        
        yield return new WaitForSeconds(3.0f); // wait to get into while loop; to give us time after destroy asteroid for enemies to attack

        while (_stopPowerUpSpawning == false) // auto cleanup when player dies...auto stops everything
        {
            Vector3 posToSpawnPowerUp = new Vector3(Random.Range(-9.2f, 9.2f), 8, 0);

            int randomPowerUp = Random.Range(0, 4);
            Instantiate(_powerUps[randomPowerUp] , posToSpawnPowerUp ,Quaternion.identity);

            yield return new WaitForSeconds(secToWaitPowerUpSpawn);
        }
    }

    // triggers stop spawning
    public void onPlayerDeath()
    {
        _stopEnemySpawning  = true;
    }
}
