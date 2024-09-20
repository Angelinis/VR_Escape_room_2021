using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

[System.Serializable]
public class UnityAndGeminiURL
{
    public string key;
}

public class UnityAndGemini : MonoBehaviour
{
    private string gasURL;
    public bool usePrompt = false;
    public string prompt;
    public TextAsset jsonURL;

    void Start()
    {
        if(jsonURL != null)
        {
            UnityAndGeminiURL geminiURL = JsonUtility.FromJson<UnityAndGeminiURL>(jsonURL.text);
            gasURL = geminiURL.key;            
        }

        if(usePrompt){
            StartCoroutine(SendDataToGAS(prompt));
        } 
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

}