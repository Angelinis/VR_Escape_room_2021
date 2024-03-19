using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingManager : MonoBehaviour
{
    private AudioManager audioManager;
    // public ActionBasedController xrController;

    void Start()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayDescription(0);
    }

    void Update()
    {
        // bool aButtonPressed = xrController.activateAction.action.triggered;

        // if (aButtonPressed)
        // {
        //  Destroy(audioManager.gameObject);
        // SceneManager.LoadScene("MainScene");
        // }
    }
}