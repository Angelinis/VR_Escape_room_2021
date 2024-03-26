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
    public XROrigin xrOriginScene1;
    // public ActionBasedController xrController;
    public GameObject xrOriginScene2;
    public GameObject trainingScene1;
    public GameObject trainingScene2;
    public GameObject trainingScene3;

    private bool congratulationsPlayed = false;
    private bool descriptionPlayed = false;


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
        if (trainingScene1.activeSelf)
        {
            if (RequireRotationXROrigin() == 0 && !congratulationsPlayed)
            {
                StartCoroutine(DelayedAction(trainingScene2, trainingScene1));

                congratulationsPlayed = true;
            }
        }

        if (trainingScene2.activeSelf)
        {
            if (!descriptionPlayed)
            {
                audioManager.PlayDescription(1);
                bool gameOver = xrOriginScene2.GetComponent<GameOverCollision>().gameOverCollision;
                descriptionPlayed = true;
            }

        }

    }

    public void ChangeTrainingScene(int audioClip)
    {

        audioManager.PlayDescription(audioClip);
    }

    public int RequireRotationXROrigin()
    {
        var xrOriginRotationY = xrOriginScene1.transform.rotation.eulerAngles.y;
        int finalTurnValue = Mathf.RoundToInt(xrOriginRotationY);
        return finalTurnValue;
    }

    IEnumerator DelayedAction(GameObject sceneToActivate, GameObject sceneToDeactivate)
    {

        yield return new WaitForSeconds(2);

        audioManager.PlayDescription(3);

        yield return new WaitForSeconds(1);

        sceneToDeactivate.SetActive(false);
        congratulationsPlayed = false;

        sceneToActivate.SetActive(true);
    }


}