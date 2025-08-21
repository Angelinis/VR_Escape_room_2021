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
public class ObjectRenderData
{
    public string objName;
    public Vector3 objPosition;

    public ObjectRenderData(string objectDataName, Vector3 objectDataPosition)
    {
        objName = objectDataName;
        objPosition = objectDataPosition;
    }
}

public class CheckRenderObjects : MonoBehaviour
{
    private Renderer[] renderers;

    private Metadata metadata;

    public GameObject cameraTarget;

    private bool active;

    public string prePrompt;

    public ObjectRenderData[] objects = new ObjectRenderData[15]; 

    // private string alternativePrompt;

    private int screenshotCount = 0;

    private string prompt;

    public InputActionProperty activateButton;

    public bool checkLabels = true;

    public Camera assignedCamera;

    void Start()
    {
        // assignedCamera.gameObject.SetActive(false);
        
    }

    void Awake ()
    {
        if(checkLabels)
        {
            GameObject visibleObject = GameObject.FindGameObjectWithTag ("VisibleObject");
            renderers = visibleObject.GetComponentsInChildren<Renderer> ();
        }
        active = false;
        // prePrompt = "You are a guide for a blind person in a Virtual Scene. Please provide an accessible " +
        // "description from the point of user's point of view (User POV) and the list of contents. " +
        // "The accessible description needs to be short. Keep it below 1200 characters. Omit any of (0.00, 0.00, 0.00) in your response.";
        
        // alternativePrompt = "Descreva o cenário virtual, destacando os elementos e características principais. Explore a arquitetura," +
        // "e elementos que compõem o ambiente." + 
        // "Crie uma descrição acessível para uma pessoa cega em menos de 600 caracteres, capturando a essência do local." +
        // " Exclua qualquer informação sobre os controles.";

        // alternativePrompt = "Based on the image, can you provide make a list of the elements you recognize?";

    }

    void Update() 
    {

        
        if (UnityEngine.Input.GetKeyDown(KeyCode.C))
        {
            if(!active)
            {

                //Code to send text and an image to the Gemini API
                StartCoroutine(CaptureScreenshot());

                active = true;

                if(checkLabels)
                {
                    prompt = OutputVisibleRenderers(renderers);
                }
                
    

            }
            
        }
    }

    
    private IEnumerator CaptureScreenshot()
{
    // assignedCamera.gameObject.SetActive(true);
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
        // assignedCamera.gameObject.SetActive(false);
    }
    catch (Exception ex)
    {
        Debug.LogError("Failed to save screenshot file: " + ex.Message);
        // assignedCamera.gameObject.SetActive(false);
    }


    // Check if file exists before attempting to read
   
}


         
    private void ErrorReceived(BadRequestData badRequestData)
    {
        Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
        active = false;
    }


    private string OutputVisibleRenderers (Renderer[] renderers)
    {
        ResetObjectsArray();

        Vector3 cameraPosition = cameraTarget.transform.position;

        // Quaternion cameraRotation = cameraTarget.transform.rotation;

        // ObjectCamera objectCamera = new  ObjectCamera("Camera", cameraPosition,cameraRotation);

        ObjectRenderData objectCamera = new  ObjectRenderData("User POV", cameraPosition);

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
                    
                    ObjectRenderData newObj = new  ObjectRenderData(metadata.objectName, sourcePosition);



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
