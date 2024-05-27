using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.EventSystems;

using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    private AudioManager audioManager;

    public static GameManager instance;

    public GameObject[] inspectedGameObjects;
    
    // public GameObject completedActionObject;
    // public GameObject completedActionLight;
    // public AudioClip completedActionClip;
    // public AudioSource completedActionAudioSource;

    public InputActionProperty changeSelectedObjectButton;

    public GameObject selectedEmpty;

    public GameObject selectedGameObjectView;

    private string[] collectedGameObjects = new string[1];

    private int selectedGameObjectIndex = 0;

    // private bool hasCompletedAction = false;

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

    void Start()
    {
        audioManager = AudioManager.instance;
        
        collectedGameObjects[0] = "Empty_Collectable";
        selectedGameObjectView = selectedEmpty.transform.Find("Empty_Collectable").gameObject;
    }

    public void CheckObjects()
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
        }
    }



    // IEnumerator playSoundAfterSomeSeconds()
    // {
    //     yield return new WaitForSeconds(3);
    //     completedActionAudioSource.Play();

    //     yield return new WaitForSeconds(completedActionAudioSource.clip.length);
    //     completedActionObject.SetActive(false);
    // }

    void Update()
    {
        CheckObjects();

        if (changeSelectedObjectButton.action.WasPressedThisFrame())
        {
            selectedGameObjectIndex += 1;

            if((selectedGameObjectIndex) > (collectedGameObjects.Length - 1))
            {
                selectedGameObjectView.SetActive(false);
                selectedGameObjectView = selectedEmpty.transform.Find("Empty_Collectable").gameObject;
                selectedGameObjectView.SetActive(true);
                selectedGameObjectIndex = 0;
            } 

            if(selectedGameObjectIndex != 0) {
                selectedGameObjectView.SetActive(false);
                selectedGameObjectView = selectedEmpty.transform.Find(collectedGameObjects[selectedGameObjectIndex]).gameObject;
                selectedGameObjectView.SetActive(true);
            }
            audioManager.PlaySFX(1);
            audioManager.PlayAccessibleDescription(selectedGameObjectView.GetComponent<AudioInformation>().accessibleDescription[0]);
            
        }

    }
}
