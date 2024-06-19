using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesSolved : MonoBehaviour
{
    // Start is called before the first frame update
    // Electricity Puzzle
    private AudioManager audioManager;
    private GameManager gameManager;
    private PuzzleInformation puzzleInformation;

    private Animator animator;

    public int puzzleNumber = 0;

    public GameObject objectToInactive1;
    public GameObject objectToInactive2;
    // public GameObject objectToInactive3;
    public GameObject objectToActive1;
    public GameObject objectToActive2;
    // public GameObject objectToActive3;


    void Start()
    {
        audioManager = AudioManager.instance;
        gameManager = GameManager.instance;
        puzzleInformation = GetComponent<PuzzleInformation>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SolvedPuzzleAction(){

        puzzleInformation.collectedObjectInScene.GetComponent<WasUsed>().wasItUsed = true;

        gameManager.RemoveObjectByName(puzzleInformation.collectedObjectInScene.name);

        if(objectToActive1){
            objectToActive1.SetActive(true);
        } 
        if(objectToActive2){
            objectToActive2.SetActive(true);
        } 
        if(objectToInactive1){
            objectToInactive1.SetActive(false);
        } 
        if(objectToInactive2){
            objectToInactive2.SetActive(false);
        }     

        if((puzzleNumber == 1 || puzzleNumber ==4))
        {
            audioManager.PlaySFX(2);
        }

        if((puzzleNumber == 2))
        {
            audioManager.PlaySFX(3);
        }

        if((puzzleNumber == 3))
        {
            GameObject originalGameObject = GameObject.Find("Dog_Puzzle");
            GameObject child = originalGameObject.transform.Find("Dog").gameObject;

            animator = child.GetComponent<Animator>();

            animator.SetBool("PuzzleBlocked", false);
            animator.SetBool("PuzzleSolved",true);
            audioManager.PlaySFX(4);
        }

        if((puzzleNumber == 5))
        {
            audioManager.PlaySFX(5);
        }
    }

}
