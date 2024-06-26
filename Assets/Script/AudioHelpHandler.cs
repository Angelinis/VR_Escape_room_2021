using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelpHandler : MonoBehaviour
{
    public AudioClip puzzle_1, puzzle_2, puzzle_3, puzzle_4, puzzle_5;
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
        if(values[0] == "ajuda"){
            Debug.Log("Playing respective clip, according to the puzzle");
        }
    }
}
