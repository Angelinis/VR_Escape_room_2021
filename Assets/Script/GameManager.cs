using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject[] inspectedGameObjects;
    
    public GameObject completedActionObject;
    public GameObject completedActionLight;
    public AudioClip completedActionClip;
    public AudioSource completedActionAudioSource;


    private string[] collectedGameObjects;

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

    public bool AreAllObjectsInactive()
    {
        foreach (GameObject obj in inspectedGameObjects)
        {
            if(!obj.activeSelf)
            {
                int targetIndex = System.Array.IndexOf(collectedGameObjects, obj.name);
              if (targetIndex == -1)
                {
                    System.Array.Resize(ref collectedGameObjects, collectedGameObjects.Length + 1);
                    collectedGameObjects[collectedGameObjects.Length - 1] = obj.name;
                }
            }

            else
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
        
        // if (IsEnergySolutionActive() && !hasCompletedAction)
        // {
        //     completedActionLight.SetActive(true);
        //     completedActionAudioSource.clip = completedActionClip;
        //     completedActionAudioSource.volume = 1;
        //     StartCoroutine(playSoundAfterSomeSeconds());
        //     hasCompletedAction = true; 
        // }
    }
}
