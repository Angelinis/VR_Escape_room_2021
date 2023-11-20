using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] accessibleSoundNameSource;
    public AudioSource[] accessibleObjectSoundSource;

    //Add your sound effects clips here
    public AudioClip[] sfxClips;
    public AudioSource sfxSource;

    private void Awake()
    {
        // Ensure there is only one instance of AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public void PlayMusic(AudioClip musicClip)
    // {
    //     musicSource.clip = musicClip;
    //     musicSource.Play();
    // }

    public void PlaySFX(int sfxIndex)
    {
        if (sfxIndex >= 0 && sfxIndex < sfxClips.Length)
        {
            sfxSource.clip = sfxClips[sfxIndex];
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("SFX index out of range");
        }
    }

    // public void SetMusicVolume(float volume)
    // {
    //     musicSource.volume = volume;
    // }

    // public void SetSFXVolume(float volume)
    // {
    //     sfxSource.volume = volume;
    // }
}
