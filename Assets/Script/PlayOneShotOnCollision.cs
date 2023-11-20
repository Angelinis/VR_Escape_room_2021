using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotOnCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume;
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SurfaceCollider")
        {
            
            audioManager.PlaySFX(1);
            //audioSource.PlayOneShot(clip, volume);
            //Debug.Log("I am colliding!");
        }
    }

}
