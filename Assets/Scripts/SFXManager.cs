using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour

{
    // Audio Source
    [SerializeField] private AudioSource _audioSource;

    // Audio Clips
    [SerializeField] private AudioClip _laserSoundClip;
    [SerializeField] private AudioClip _explosionSoundClip;
    [SerializeField] private AudioClip _powerUpSoundClip;
    [SerializeField] private AudioClip _reloadClip;


    void Start()
    {
        if (_audioSource == null)
        {
            Debug.LogError("The Audio Source on the SFXManager is NULL");
        }
    }

    public void PlaySFX(string audioClipToPlay)
    {
        switch (audioClipToPlay)
        {
            case "laser_shot":
                _audioSource.clip = _laserSoundClip;
                break;

            case "explosion_sound":
                _audioSource.clip = _explosionSoundClip;
                break;

            case "power_up_sound":
                _audioSource.clip = _powerUpSoundClip;
                break;

            case "ReloadRemix":
                _audioSource.clip = _reloadClip;
                break;

            default:
                Debug.Log("No audio file found");
                break;
        }
        
        if (audioClipToPlay == "ReloadRemix")
        {
            _audioSource.Play();
            _audioSource.loop = true;
        }
        else
        {
            _audioSource.Play();
        }

    }

}