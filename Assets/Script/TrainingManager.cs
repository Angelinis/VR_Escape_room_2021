using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;


using Unity.XR.CoreUtils;
using UnityEngine.Assertions;

using UnityEngine.XR.Interaction.Toolkit;

public class TrainingManager : MonoBehaviour
{
    private AudioManager audioManager;
    // public ActionBasedController xrController;
    public GameObject trainingScene1;
    public GameObject trainingScene2;
    public GameObject trainingScene3;

    void Start()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayDescription(0);
        trainingScene2.SetActive(false);
        trainingScene3.SetActive(false);
        trainingScene1.SetActive(true);
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

     public void ChangeTrainingScene(int audioClip)
    {
 
        audioManager.PlayDescription(audioClip);
    }



}