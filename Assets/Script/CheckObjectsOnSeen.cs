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

    public ObjectData(string objectDataName, Vector3 objectDataPosition)
    {
        objName = objectDataName;
        objPosition = objectDataPosition;
    }
}

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

    private string prompt;

    private AudioManager audioManager;

    public InputActionProperty activateButton;

    public AudioSource audioSource;


    [SerializeField] private VoiceScriptableObject voice;
    [SerializeField] private TextToSpeech textToSpeech;
    private Action<AudioClip> _audioClipReceived;
    private Action<BadRequestData> _errorReceived;

    private bool isWaitingForAudioResponse;

    public bool isTraining = false;

    public bool isMainScene = false;

    public AudioClip loadingAudio;


    public TrainingManager trainingManager;

    public Camera assignedCamera;

    void Start()
    {
        audioManager = AudioManager.instance;
        isWaitingForAudioResponse = false;
        assignedCamera.gameObject.SetActive(false);
        
    }

    void Awake ()
    {
        if(!isTraining)
        {
            GameObject visibleObject = GameObject.FindGameObjectWithTag ("VisibleObject");
            renderers = visibleObject.GetComponentsInChildren<Renderer> ();
        }
        active = false;
        artificialInteligence = GetComponent<GeminiManager>();
        // prePrompt = "You are a guide for a blind person in a Virtual Scene. Please provide an accessible " +
        // "description from the point of user's point of view (User POV) and the list of contents. " +
        // "The accessible description needs to be short. Keep it below 1200 characters. Omit any of (0.00, 0.00, 0.00) in your response.";
        
        alternativePrompt = "Descreva o cenário virtual, destacando os elementos e características principais. Explore a arquitetura," +
        "e elementos que compõem o ambiente." + 
        "Crie uma descrição acessível para uma pessoa cega em menos de 600 caracteres, capturando a essência do local." +
        " Exclua qualquer informação sobre os controles.";

        // alternativePrompt = "Based on the image, can you provide make a list of the elements you recognize?";

        // string testPrompt_1 = "From now on, please act as an Orientation and Mobility Specialist. Focus on the environmental details and provide a comprehensive description from this perspective. Highlight key aspects relevant to navigation and accessibility to assist users in understanding the space effectively.";
        // string testPrompt_2 = "From now on, please act as a Sighted Guide. Concentrate on the environmental details pertinent to this role and provide observations that would be important for effective assistance in navigating the space.";
        // string testPrompt_3 = "This is a template for your response. Replace the placeholders (in all caps) with specific details. Use the following format: ``Hello! I'll be assisting you today. You are currently located at LOCATION. From your position, you can find a ELEMENT at your ORIENTATION o'clock, a ELEMENT at your ORIENTATION o'clock, and a ELEMENT at your ORIENTATION o'clock. Feel free to explore the PLACE, but be cautious of potential collisions with OBSTACLE, OBSTACLE, or OBSTACLE. If you need any additional information, just let me know!'' Adjust any details as needed!";
        // string testPrompt_4 = "When I ask you to describe this image, please create a DALL-E prompt that I can use to recreate this image. Choose the appropriate tools based on what needs to be visualized.";
        // string testPrompt_5 = "When analyzing the following scene, focus solely on elements that could be classified as OBSTACLES and IMPORTANT ELEMENTS for navigating by a visually impaired individual. Please disregard any UNIMPORTANT information.";
        // string testPrompt_6 = "When analyzing the upcoming data, concentrate exclusively on the USER POV. Describe the scene for a visually impaired individual by detailing the positions and descriptions of the ELEMENTS. Please ignore any UNIMPORTANT information.";
        // alternativePrompt = testPrompt_6 + ". Your output must have a maximum of 700 characters";

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

                if(!isTraining)
                {
                    // prompt = OutputVisibleRenderers(renderers);
                }
                
                
 
                //Code to send text to the Gemini API
                // StartCoroutine(artificialInteligence.SendDataToGAS(prePrompt + " " + prompt));

            }
            
        }
    }

    
    private IEnumerator CaptureScreenshot()
{
    assignedCamera.gameObject.SetActive(true);
    yield return new WaitForEndOfFrame();

    screenshotCount++;
    string screenshotFileName = "/Screenshot_" + screenshotCount + "_" + Screen.width + "X" + Screen.height + ".png";
    string screenShotPath = Application.persistentDataPath + screenshotFileName;

    RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
    assignedCamera.targetTexture = screenTexture;
    RenderTexture.active = screenTexture;
    assignedCamera.Render();

    Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
    renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    RenderTexture.active = null;

    byte[] imageBytes = renderedTexture.EncodeToPNG();

    try
    {
        System.IO.File.WriteAllBytes(screenShotPath, imageBytes);
        Debug.Log("Screenshot saved to: " + screenShotPath);
    }
    catch (Exception ex)
    {
        Debug.LogError("Failed to save screenshot file: " + ex.Message);
    }

    audioManager.PlayAccessibleDescription(loadingAudio);

    // Check if file exists before attempting to read
    if (File.Exists(screenShotPath))
    {
        try
        {

            byte[] screenshotBytes = File.ReadAllBytes(screenShotPath);

                        
                        
            assignedCamera.gameObject.SetActive(false);

            StartCoroutine(artificialInteligence.SendMultimodalDataToGAS(alternativePrompt, screenshotBytes, (response) => {
                if (response != null)
                {
                    Debug.Log("Response received: " + response);

                    if (!isWaitingForAudioResponse)
                    {
                        isWaitingForAudioResponse = true;
                        _errorReceived += ErrorReceived;
                        _audioClipReceived += AudioClipReceived;
                        //Not wanting to play the answer
                        textToSpeech.GetSpeechAudioFromGoogle(response, voice, _audioClipReceived, _errorReceived);
                    }
                }
                else
                {
                    Debug.Log("Error occurred during request.");
                    active = false;
                    audioManager.PlaySFX(2);
                }
            }));
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to read screenshot file: " + ex.Message);
            active = false;
            audioManager.PlaySFX(2);
        }
    }
    else
    {
        Debug.LogError("Screenshot file not found: " + screenShotPath);
        active = false;
        audioManager.PlaySFX(2);
    }
}


         
    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
        active = false;
        audioManager.PlaySFX(2);
    }

    private void AudioClipReceived(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        isWaitingForAudioResponse = false;
        

        if(isTraining && !isMainScene)
        {
            StartCoroutine(trainingManager.DelayedFinalAction());
        }

        if(isMainScene)
        {
            active = false;
        }
        
                
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
