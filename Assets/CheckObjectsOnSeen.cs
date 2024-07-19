using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using UnityEditor;
using UnityEngine.InputSystem;

using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using GoogleTextToSpeech.Scripts.Data;
using GoogleTextToSpeech.Scripts;
using System;
using UnityEngine.Networking;


[System.Serializable]
public class ObjectData
{
    public string objName;
    public Vector3 objPosition;


    // Constructor to initialize with values
    public ObjectData(string objectDataName, Vector3 objectDataPosition)
    {
        objName = objectDataName;
        objPosition = objectDataPosition;
    }
}

// [System.Serializable]
// public class ObjectCamera
// {
//     public string objName;
//     public Vector3 objPosition;
//     public Quaternion objRotation;

//     // Constructor to initialize with values
//     public ObjectData(string objectDataName, Vector3 objectDataPosition, Quaternion objectDataRotation)
//     {
//         objName = objectDataName;
//         objPosition = objectDataPosition;
//         objRotation = objectDataRotation;
//     }
// }

public class CheckObjectsOnSeen : MonoBehaviour
{
    private Renderer[] renderers;

    private Metadata metadata;

    public GameObject cameraTarget;

    private bool active;

    public string prePrompt;

    public ObjectData[] objects = new ObjectData[15]; 

    private GeminiManager artificialInteligence;

    private string alternativePrompt;

    private int screenshotCount = 0;

    private string screenshotFileName = "";
    
    private string screenShotPath = "";

    private string prompt;

    private AudioManager audioManager;

    public InputActionProperty activateButton;

    public AudioSource audioSource;



    [SerializeField] private VoiceScriptableObject voice;
    [SerializeField] private TextToSpeech textToSpeech;
    private Action<AudioClip> _audioClipReceived;
    private Action<BadRequestData> _errorReceived;

    private bool isWaitingForAudioResponse;

    void Start()
    {
        audioManager = AudioManager.instance;
        isWaitingForAudioResponse = false;
    }

    void Awake ()
    {
        GameObject visibleObject = GameObject.FindGameObjectWithTag ("VisibleObject");
        renderers = visibleObject.GetComponentsInChildren<Renderer> ();
        active = false;
        artificialInteligence = GetComponent<GeminiManager>();
        // prePrompt = "You are a guide for a blind person in a Virtual Scene. Please provide an accessible " +
        // "description from the point of user's point of view (User POV) and the list of contents. " +
        // "The accessible description needs to be short. Keep it below 1200 characters. Omit any of (0.00, 0.00, 0.00) in your response.";
        
        alternativePrompt = "Descreva o cenário, destacando os elementos e características principais. Explore a arquitetura," +
        "e elementos que compõem o ambiente." + 
        "Crie uma accesivel para uma pessoa cega em menos de 700 caracteres, capturando a essência do local sem considerar os controles visíveis.";

        // alternativePrompt = "Based on the image, can you provide make a list of the elements you recognize?";

    }

    void Update() 
    {

        
          if (UnityEngine.Input.GetKeyDown(KeyCode.C))
        //  if (activateButton.action.WasPressedThisFrame())
        {
            if(!active)
            {

                //Code to send text and an image to the Gemini API
                StartCoroutine(CaptureScreenshot());

                active = true;


                
                prompt = OutputVisibleRenderers(renderers);
 
                //Code to send text to the Gemini API
                // StartCoroutine(artificialInteligence.SendDataToGAS(prePrompt + " " + prompt));

            }
            
        }
    }

    


    //  private IEnumerator CaptureScreenshot()
    //  {
    //     //File.ReadAllBytes only works with Unity Editor - Needs to be updated for working inside the Meta Quest

    //       screenshotCount ++;
    //       screenshotFileName = "/Screenshot_" + screenshotCount + ".png";
    //       screenShotPath = Application.streamingAssetsPath +  screenshotFileName;
    //       ScreenCapture.CaptureScreenshot(screenShotPath);
    //       yield return null;
    //       AssetDatabase.Refresh();

    //       yield return new WaitForSeconds(0.1f);

    //       byte[] screenshotBytes = File.ReadAllBytes(screenShotPath);
          
    //       StartCoroutine(artificialInteligence.SendMultimodalDataToGAS(alternativePrompt, screenshotBytes));
    //  }

        private IEnumerator CaptureScreenshot()
     {
        //File.ReadAllBytes only works with Unity Editor - Needs to be updated for working inside the Meta Quest

          screenshotCount ++;
          screenshotFileName = "/Screenshot_" + screenshotCount + ".png";
          screenShotPath = Application.persistentDataPath +  screenshotFileName;
          ScreenCapture.CaptureScreenshot(screenShotPath);

          yield return new WaitForSeconds(0.1f);

          byte[] screenshotBytes = File.ReadAllBytes(screenShotPath);
          
          StartCoroutine(artificialInteligence.SendMultimodalDataToGAS(alternativePrompt, screenshotBytes, (response) => {
            if (response != null)
            {
 
                Debug.Log("Response received: " + response);

                if (!isWaitingForAudioResponse) 
                {
                    isWaitingForAudioResponse = true;
                    _errorReceived += ErrorReceived;
                    _audioClipReceived += AudioClipReceived;
                    textToSpeech.GetSpeechAudioFromGoogle(response, voice, _audioClipReceived, _errorReceived);
                }
            }
            else
            {
                Debug.Log("Error occurred during request.");
            }}));

        
    }

         
    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
    }

    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        isWaitingForAudioResponse = false;
    }

    private string OutputVisibleRenderers (Renderer[] renderers)
    {
        ResetObjectsArray();

        Vector3 cameraPosition = cameraTarget.transform.position;

        // Quaternion cameraRotation = cameraTarget.transform.rotation;

        // ObjectCamera objectCamera = new  ObjectCamera("Camera", cameraPosition,cameraRotation);

        ObjectData objectCamera = new  ObjectData("User POV", cameraPosition);

        objects[0] = objectCamera;

        foreach (var renderer in renderers)
        {
            // output only the visible renderers' name
            if (IsVisible(renderer)) 
            {

                Vector3 sourcePosition = renderer.gameObject.transform.position;

                metadata = renderer.gameObject.GetComponent<Metadata>();
                
                // Renderer only works for mesh components. It won't get Metadata if it is an empty parent without a mesh
                // Debug.Log(renderer.gameObject.name);

                if(metadata != null)
                {                                
                    // Vector3 cameraPosition = cameraTarget.transform.position;
                    // float distance = Vector3.Distance(sourcePosition, cameraPosition);
                    // float formattedDistance = Mathf.Round(distance * 10f) / 10f; 
                    // Debug.Log (metadata.objectName + " is detected at " + formattedDistance  + " meters");
                    
                    ObjectData newObj = new  ObjectData(metadata.objectName, sourcePosition);



                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].objName == "")
                        {
                            objects[i] = newObj;
                            break; // Exit the loop once added
                        } 
                    }
                }

                // Debug.Log (renderer.gameObject.name + " is detected in position" + sourcePosition);

            }  
        }

        active = false;

        string result = "List contents: ";
        foreach (var item in objects)
        {
            result += item.objName.ToString() + " " + item.objPosition.ToString() + "---";
        }
        Debug.Log(result);

        return result;

    }

    private void ResetObjectsArray()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].objName = "";
            objects[i].objPosition = Vector3.zero;
        }

    }

    private bool IsVisible(Renderer renderer) 
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes , renderer.bounds))
            return true;
        else
            return false;
    }
}
