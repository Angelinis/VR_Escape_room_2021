using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    public InputActionProperty activateButtonRight, activateButtonLeft;
    public UserData userData;

    public bool isBlind;

    void Start()
    {
        if(isBlind)
        {
        audioManager = AudioManager.instance;
        audioManager.PlayDescription(0);
        }
    }

    void Update()
    {

        if (activateButtonRight.action.WasPressedThisFrame())
        {
         Destroy(audioManager.gameObject);
        SceneManager.LoadScene("TrainingScene");
        }

        if(activateButtonLeft.action.WasPressedThisFrame())
        {
            userData.SetInternetConnection();
            if(userData.internetConnection)
            {
                audioManager.PlaySFX(2);

            } else {
                audioManager.PlaySFX(1);
            }

            if(isBlind)
            {
            audioManager.PlayDescription(0);
            }
        }



    }

}