using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHelpHandler : MonoBehaviour
{
    public AudioClip puzzle_1, puzzle_2, puzzle_3, puzzle_4, puzzle_5;
    public GameObject object_1, object_2, object_3, object_4, object_5;
    private AudioManager audioManager;
    private WasUsed puzzle1Completed;
    private WasUsed puzzle2Completed;
    private WasUsed puzzle3Completed;
    private WasUsed puzzle4Completed;
    private WasUsed puzzle5Completed;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        puzzle1Completed = object_1.GetComponent<WasUsed>();
        puzzle2Completed = object_2.GetComponent<WasUsed>();
        puzzle3Completed = object_3.GetComponent<WasUsed>();
        puzzle4Completed = object_4.GetComponent<WasUsed>();
        puzzle5Completed = object_5.GetComponent<WasUsed>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnActivate(string[] values)
    {
        if(values[0] == "ajuda"){
            Debug.Log("Playing respective clip, according to the puzzle");

            if(!puzzle1Completed.wasItUsed)
            {
                audioManager.PlayAccessibleDescription(puzzle_1);
            } else if (!puzzle2Completed.wasItUsed)
            {
                audioManager.PlayAccessibleDescription(puzzle_2);
            } else if(!puzzle3Completed.wasItUsed)
            {
                audioManager.PlayAccessibleDescription(puzzle_3);
            } else if(!puzzle4Completed.wasItUsed)
            {
                audioManager.PlayAccessibleDescription(puzzle_4);
            } else if (!puzzle5Completed.wasItUsed)
            {
                audioManager.PlayAccessibleDescription(puzzle_5);
            }
        }
    }
}
