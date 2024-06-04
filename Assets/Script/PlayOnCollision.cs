using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCollision : MonoBehaviour
{
    // // Start is called before the first frame update
    // public float volume;
    // public int sfxIndex;
    // private AudioManager audioManager;
    // CharacterController controller;
    // // Start is called before the first frame update
    
    
    // void Start()
    // {
    //     audioManager = AudioManager.instance;
    //     controller = GetComponent<CharacterController>();
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //       if (((controller.collisionFlags & CollisionFlags.Sides) != 0) & collision.gameObject.tag == "SurfaceCollider")
    //     {
    //        audioManager.PlaySFX(sfxIndex);
    //     }

    // }
    CharacterController controller;

    public float volume;
    public int sfxIndex;
    private AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager = AudioManager.instance;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (((controller.collisionFlags & CollisionFlags.Sides) != 0))
        {
           audioManager.PlaySFX(sfxIndex);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SurfaceCollider")
        {
            Debug.Log("I am colliding!");
            // audioManager.PlaySFX(sfxIndex);
            //audioSource.PlayOneShot(clip, volume);
            //Debug.Log("I am colliding!");
        }
    }
}
