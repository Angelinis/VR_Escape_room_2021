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
    public GameObject colliderVRScene2;
    public GameObject objectVRScene3;
    public GameObject trainingScene1;
    public GameObject trainingScene2;
    public GameObject trainingScene2Environment;
    public GameObject trainingScene3;
    public GameObject trainingScene3Part2;
    public GameObject trainingScene4;

    public UserData userData;

    private bool congratulationsPlayed = false;
    private bool descriptionPlayed = false;
    private bool justOnce = true;

    public bool finalScene = false;

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

                trainingScene2Environment.SetActive(true);

                congratulationsPlayed = true;
            }
        }

        if (trainingScene2.activeSelf && trainingScene2Environment.activeSelf)
        {
            if (!descriptionPlayed)
            {
                audioManager.PlayDescription(1);
                descriptionPlayed = true;
                congratulationsPlayed = false;

            }

            if (colliderVRScene2.GetComponent<GameOverCollision>().gameOverCollision && !congratulationsPlayed && justOnce)
            {
                justOnce = false;
                StartCoroutine(DelayedAction(trainingScene3, trainingScene2Environment));
            }

        }

        if (trainingScene3.activeSelf)
        {
            if (!descriptionPlayed)
            {
                audioManager.PlayDescription(2);
                descriptionPlayed = true;
                congratulationsPlayed = false;
                justOnce = true;
            }

            if (!objectVRScene3.activeSelf && !congratulationsPlayed && justOnce)
            {
                //Adding two more training steps
                // StartCoroutine(DelayedFinalAction());
                audioManager.PlayDescription(5);
                trainingScene3Part2.SetActive(true);
                justOnce = false;
            }



        }

        
        if (trainingScene4.activeSelf)
        {
            if (!descriptionPlayed)
            {
                if(userData.internetConnection)
                {
                    audioManager.PlayDescription(6);
                    descriptionPlayed = true;
                    congratulationsPlayed = false;
                    justOnce = true;
                } else {
                    StartCoroutine(DelayedFinalActionAlternative());
                    descriptionPlayed = true;
                    justOnce = true;
                }

            }

            // if (!objectVRScene3.activeSelf && !congratulationsPlayed && justOnce)
            // {
            //     //Adding two more training steps
            //     // StartCoroutine(DelayedFinalAction());
            //     audioManager.PlayDescription(5);
            //     trainingScene3Part2.SetActive(true);
            //     justOnce = false;
            // }



        }

    }

    public int RequireRotationXROrigin()
    {
        var xrOriginRotationY = xrOriginScene1.transform.rotation.eulerAngles.y;
        int finalTurnValue = Mathf.RoundToInt(xrOriginRotationY);
        return finalTurnValue;
    }

    IEnumerator DelayedAction(GameObject sceneToActivate, GameObject sceneToDeactivate)
    {
        if(finalScene)
        {
            yield return new WaitForSeconds(5);
        } else {
            yield return new WaitForSeconds(2);
        }

        audioManager.PlayDescription(3);

        yield return new WaitForSeconds(1);

        sceneToDeactivate.SetActive(false);
        
        descriptionPlayed = false;

        sceneToActivate.SetActive(true);
    }

    public void CallFinalScene(){
        StartCoroutine(DelayedAction(trainingScene4, trainingScene3));
    }

    public IEnumerator DelayedFinalAction()
    {

        yield return new WaitForSeconds(15);

        audioManager.PlayDescription(4);

        yield return new WaitForSeconds(8);
        
        Destroy(audioManager.gameObject);

        SceneManager.LoadScene("MainScene");
    }

    public IEnumerator DelayedFinalActionAlternative()
    {

        yield return new WaitForSeconds(3);

        audioManager.PlayDescription(4);

        yield return new WaitForSeconds(8);
        
        Destroy(audioManager.gameObject);

        SceneManager.LoadScene("MainScene");
    }


}