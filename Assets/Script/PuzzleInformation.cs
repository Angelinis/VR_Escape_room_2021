using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleInformation : MonoBehaviour
{
    // public GameObject gameManager;

    public GameObject puzzleObject; 
    private AudioInformation audioInformationPuzzleObject;

    public GameObject necessaryObject;

    public GameObject collectedObjectInScene;

    private AudioManager audioManager;

    private PuzzlesSolved puzzlesSolved;

    private GameManager gameManager;
    
    // Start is called before the first frame update

    void Awake(){
        
        if (puzzleObject != null) {
        audioInformationPuzzleObject = puzzleObject.GetComponent<AudioInformation>();
        }
              
    }

    void Start(){
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
        puzzlesSolved = GetComponent<PuzzlesSolved>();
    }

    public void CheckObject()
    {
        if (necessaryObject == gameManager.selectedGameObjectView){
            puzzlesSolved.SolvedPuzzleAction();
            // audioManager.PlayAccessibleDescription(audioInformationPuzzleObject.accessibleDescription[1]);
            audioManager.PlayPuzzleDescription(audioInformationPuzzleObject.accessibleDescription[1]);
            
            
        } else {
            // audioManager.PlayAccessibleDescription(audioInformationPuzzleObject.accessibleDescription[0]);
            audioManager.PlayPuzzleDescription(audioInformationPuzzleObject.accessibleDescription[0]);

        }

    }
}
