using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform sourceObject; // Assign the source object in the Unity Editor
    public bool followRotation = false;

    void Update()
    {
        // Check if the source object is assigned
        if (sourceObject != null)
        {
            // Copy the X and Z positions from the source object to this object
            Vector3 newPosition = new Vector3(sourceObject.position.x, transform.position.y, sourceObject.position.z);
            transform.position = newPosition;

            if(followRotation)
            {
                transform.rotation = sourceObject.rotation;
            }
        }
        else
        {
            Debug.LogError("Source object not assigned! Please assign a source object in the Unity Editor.");
        }
    }
}