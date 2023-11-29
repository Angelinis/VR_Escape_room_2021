using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource[] accessibleSoundNameSource;
    public AudioSource[] accessibleObjectSoundSource;

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
}

