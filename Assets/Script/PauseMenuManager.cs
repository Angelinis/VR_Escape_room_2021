using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// using Unity.XR.CoreUtils;



public class PauseMenuManager : MonoBehaviour
{

    public GameObject menu;
    public InputActionProperty showButton;
    private GameObject rightHandRayInteractor;
    private XRRayInteractor rayInteractor;
    private AudioManager audioManager;
    public AudioClip[] menuClips;

    // public InputActionProperty showButton;


    // Start is called before the first frame update
    void Start()
    {
        rightHandRayInteractor = GameObject.Find("RightHand (Teleport Locomotion)/Ray Interactor");
        rayInteractor = rightHandRayInteractor.GetComponent<XRRayInteractor>();
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (showButton.action.WasPressedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);

            if (menu.activeSelf)
            {
                rayInteractor.maxRaycastDistance = 1;
                PauseGame();
                audioManager.PlayAccessibleDescription(menuClips[0]);


            }
            else
            {
                ResumeGame();
                audioManager.PlayAccessibleDescription(menuClips[1]);
                rayInteractor.maxRaycastDistance = 0.1f;
            }
        }


    }

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
