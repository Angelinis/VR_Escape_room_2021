using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;


[System.Serializable]
public class GeminiURL
{
    public string key;
}

public class GeminiManager : MonoBehaviour
{
    private string gasURL;
    // [SerializeField] private string prompt;
    public TextAsset jsonFile;

    private string alternativeGasURL;

    public TextAsset jsonAlternativeFile;


    // Start is called before the first frame update
    void Start()
    {
        GeminiURL geminiURL = JsonUtility.FromJson<GeminiURL>(jsonFile.text);

        GeminiURL geminiAlternativeURL = JsonUtility.FromJson<GeminiURL>(jsonAlternativeFile.text);
        
        alternativeGasURL = geminiAlternativeURL.key;

        gasURL = geminiURL.key;

    }


    public IEnumerator SendDataToGAS(string prompt)
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);
        UnityWebRequest www = UnityWebRequest.Post(gasURL,form);
     
        yield return www.SendWebRequest();  
        string response = "";

        if(www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        } else 
        {
            response = "There was an error!";
        }
        Debug.Log(response);
    }

    // public IEnumerator SendMultimodalDataToGAS(string prompt, byte[] imageData)
    // {
    //     // Texture2D texture = new Texture2D(1, 1);
    //     // texture.LoadImage(imageData);

    //     // // Calculate resized dimensions
    //     // int newWidth = 512;
    //     // int newHeight = (int)((float)texture.height * ((float)newWidth / (float)texture.width));

    //     // // Resize the texture
    //     // Texture2D resizedTexture = ScaleTexture(texture, newWidth, newHeight);

    //     // // Get resized image data
    //     // byte[] resizedImageData = resizedTexture.EncodeToJPG();

    //     WWWForm form = new WWWForm();
    //     form.AddField("parameter", prompt);  // Assuming 'parameter' is the key for text data in GAS doPost function
    //     form.AddBinaryData("imageData", imageData, "image.JPG", "image/jpeg");  // Add the image data as binary

        
    //     UnityWebRequest www = UnityWebRequest.Post(alternativeGasURL, form);
    //     // www.SetRequestHeader("Content-Type", UnityWebRequest.kHttpHeaderContentTypeForm); 

    //     yield return www.SendWebRequest();

    //     if (www.result == UnityWebRequest.Result.Success)
    //     {
    //         string response = www.downloadHandler.text;
    //         Debug.Log("Response: " + response);
    //     }
    //     else
    //     {
    //         Debug.Log("Error: " + www.error);
    //     }
    // }


    public IEnumerator SendMultimodalDataToGAS(string prompt, byte[] imageData)
    {
        WWWForm form = new WWWForm();
        form.AddField("parameter", prompt);  // Assuming 'parameter' is the key for text data in GAS doPost function
        
        string base64String = System.Convert.ToBase64String(imageData);

         form.AddField("imageData", base64String); 
        // form.AddBinaryData("imageData", imageData, "image.jpg", "image/jpeg");  // Add the image data as binary
        // form.AddField("imageData", System.Convert.ToBase64String(imageData)); 

        UnityWebRequest www = UnityWebRequest.Post(alternativeGasURL, form);

        

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string response = www.downloadHandler.text;
            Debug.Log("Response: " + response);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }
    // Update is called once per frame
    // private void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Space))
    //     {
    //         StartCoroutine(SendDataToGAS());
    //     }                
    // }
}
