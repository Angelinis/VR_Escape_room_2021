using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLocationHandler : MonoBehaviour
{
    public AudioClip lab_256, hallway_1, hallway_2, stairs, lab_151;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnActivate(string[] values)
    {
        if(values[0] == "Onde"){
            Debug.Log("Playing respective clip, according to location");
        }
    }
}