using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShotOnCollision : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "SurfaceCollider")
        {
            audioSource.PlayOneShot(clip, volume);
            Debug.Log("I am colliding!");
        }
    }

}
