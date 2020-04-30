using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver = false;

// only press R key if game is over and game over screen is displaying
// hide the msg then appear when game over text appears
// wait for input R key to restart level; load scene with scene manager
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1); // curr game scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // SceneManager.LoadScene(0);
            Application.Quit();
        }
    }

    public void GameOver()
    {
        // Debug.Log("GameManager::GameOver() Called");
        _isGameOver = true;
    }
}
