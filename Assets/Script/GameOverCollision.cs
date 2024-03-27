using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCollision : MonoBehaviour
{
    public bool gameOverCollision = false;
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "GameOverCollider")
        {
            gameOverCollision = true;
        }
    

    }

}
