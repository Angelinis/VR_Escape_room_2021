using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool sightedPerson;

    public static AudioManager instance;


    public AudioSource[] gameSources;

    public AudioSource[] gameSFXSources;

    public AudioSource musicSource;


    public AudioSource puzzleDescriptionSource;


    public AudioSource accessibleDescriptionSource;

    public AudioSource uiAudioSource;

    // Add your sound effects clips here
    public AudioClip[] sfxClips;
    public AudioSource sfxSource;

    public AudioClip[] descriptionClips;
    public AudioSource descriptionSource;


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

    
    void Start()
    {
        if(sightedPerson){
            AccessibleDescriptionVolume(0);
            GameVolume(0);
        } else {
            MusicVolume(0);
            StopMusic();
        }
    }

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

    public void PlayPuzzleDescription(AudioClip audioClip)
    {
        if (audioClip)
        {
            puzzleDescriptionSource.Stop();
            puzzleDescriptionSource.clip = audioClip;
            puzzleDescriptionSource.Play();
        }
        else
        {
            Debug.LogWarning("Clip not found");
        }
    }

    public void PlayAccessibleDescription(AudioClip audioClip)
    {
        if (audioClip)
        {
            accessibleDescriptionSource.Stop();
            accessibleDescriptionSource.clip = audioClip;
            accessibleDescriptionSource.Play();
        }
        else
        {
            Debug.LogWarning("Clip not found");
        }
    }

    public void PlayDescription(int descriptionIndex)
    {
        if (descriptionIndex >= 0 && descriptionIndex < descriptionClips.Length)
        {
            descriptionSource.clip = descriptionClips[descriptionIndex];
            descriptionSource.Play();
        }
        else
        {
            Debug.LogWarning("Description index out of range");
        }
    }

    public void StopDescription()
    {
        descriptionSource.Stop();
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void AccessibleDescriptionVolume(float volume)
    {
        accessibleDescriptionSource.volume = volume;
    }

    public void UIVolume(float volume)
    {
        uiAudioSource.volume = volume;
    }

    public void DescriptionVolume(float volume)
    {
        descriptionSource.volume = volume;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void GameVolume(float volume)
    {
        for (int i = 0; i < gameSources.Length; i += 1)
        {
            gameSources[i].volume = volume;
        }

    }

    public void GameSFXVolume(float volume)
    {
        for (int i = 0; i < gameSFXSources.Length; i += 1)
        {
            gameSFXSources[i].volume = volume;
        }

    }
}

