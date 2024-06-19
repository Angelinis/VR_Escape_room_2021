using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        audioManager = AudioManager.instance;
    }


    public void StartBtn(){
        
        Destroy(audioManager.gameObject);
        SceneManager.LoadScene("MainScene");
    }
}
