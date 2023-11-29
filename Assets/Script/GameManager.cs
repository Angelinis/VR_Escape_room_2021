using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] inspectedGameObjects;


    public GameObject completedActionObject;
    public AudioClip completedActionClip;
    public AudioSource completedActionAudioSource;

    private bool hasCompletedAction = false;

    private void Awake()
    {
        // Ensure there is only one instance of GameManager
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

    // Call this method to check if all inspectedGameObjects are inactive
    public bool AreAllObjectsInactive()
    {
        foreach (GameObject obj in inspectedGameObjects)
        {
            if (obj.activeSelf)
            {
                return false;
            }
        }
        return true;
    }


    IEnumerator playSoundAfterSomeSeconds()
    {
        yield return new WaitForSeconds(3);
        completedActionAudioSource.Play();

        yield return new WaitForSeconds(completedActionAudioSource.clip.length);
        completedActionObject.SetActive(false);
    }

    void Update()
    {
        
        if (AreAllObjectsInactive() && !hasCompletedAction)
        {
            completedActionAudioSource.clip = completedActionClip;
            completedActionAudioSource.volume = 1;
            StartCoroutine(playSoundAfterSomeSeconds());
            hasCompletedAction = true; 
        }
    }
}
