using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLocationHandler : MonoBehaviour
{
    public AudioClip lab_256, hallway_1, hallway_2, stairs, lab_151, secretary, training;
    private AudioManager audioManager;

    public Transform sourceObject; 

    public bool isTraining = false;
    
    //Lab 256 
    float minX_Lab256 = 2.7f;
    float maxX_Lab256 = 10.0f;
    float minY_Lab256 = 1.2f;
    float minZ_Lab256 = -9.7f;
    float maxZ_Lab256 = -3.9f;

    //Hallway 2 Part 1
    float minX_Hallway_2 = -1.7f;
    float maxX_Hallway_2 = 2.6f;
    float minY_Hallway_2 = 1.2f;
    float minZ_Hallway_2 = -22.9f;
    float maxZ_Hallway_2 = 5.3f;

    
    //Hallway 2 Part 2
    float minX_Hallway_2_1 = -0.2f;
    float maxX_Hallway_2_1 = 11.2f;
    float minY_Hallway_2_1 = 1.2f;
    float minZ_Hallway_2_1 = -26.0f;
    float maxZ_Hallway_2_1 = -23.0f;

    //Stairs
    float minX_Stairs = 2.8f;
    float maxX_Stairs = 5.7f;
    float minY_Stairs = -1.60f;
    float maxY_Stairs = 1.0f;
    float minZ_Stairs = -22.9f;
    float maxZ_Stairs = -18.0f;

    //Hallway 1 Part 1
    float minX_Hallway_1 = -0.18f;
    float maxX_Hallway_1 = 2.5f;
    float maxY_Hallway_1 = -1.61f;
    float minZ_Hallway_1 = -22.9f;
    float maxZ_Hallway_1 = 5.3f;

    
    //Hallway 1 Part 2
    float minX_Hallway_1_1 = -1.7f;
    float maxX_Hallway_1_1 = -0.18f;
    float maxY_Hallway_1_1 = -1.61f;
    float minZ_Hallway_1_1 = -21.3f;
    float maxZ_Hallway_1_1 = -18.2f;

    //Hallway 1 Part 3
    float minX_Hallway_1_2 = -0.2f;
    float maxX_Hallway_1_2 = 11.2f;
    float maxY_Hallway_1_2 = -1.61f;
    float minZ_Hallway_1_2 = -26.0f;
    float maxZ_Hallway_1_2 = -23.0f;

    //Secretary
    float minX_Secretary = 11.2f;
    float maxX_Secretary = 16.2f;
    float maxY_Secretary = -1.61f;
    float minZ_Secretary = -28.0f;
    float maxZ_Secretary = -20.2f;

    //Lab 151
    float minX_Lab151 = -7.5f;
    float maxX_Lab151 = -0.2f;
    float maxY_Lab151 = -1.61f;
    float minZ_Lab151 = -15.6f;
    float maxZ_Lab151 = -10.2f;

    void Start()
    {
        audioManager = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        // if (sourceObject != null)
        // {
        //     // Copy the X and Z positions from the source object to this object
        //     Vector3 sourcePosition = new Vector3(sourceObject.position.x, sourceObject.position.y, sourceObject.position.z);
        //     Debug.Log(sourcePosition);
        // }
        // else
        // {
        //     Debug.LogError("Source object not assigned! Please assign a source object in the Unity Editor.");
        // }
    }

    public void OnActivate(string[] values)
    {
        if(values[0] == "Onde"){
            Debug.Log("Playing respective clip, according to location");
            
            if(isTraining)
            {
                audioManager.PlayAccessibleDescription(training);
                return;
            }


            if (sourceObject != null)
            {
                // Copy the X and Z positions from the source object to this object
                Vector3 sourcePosition = new Vector3(sourceObject.position.x, sourceObject.position.y, sourceObject.position.z);
                if (sourcePosition.x >= minX_Lab256 && sourcePosition.x <= maxX_Lab256 &&
                     sourcePosition.y >= minY_Lab256 &&
                    sourcePosition.z >= minZ_Lab256 && sourcePosition.z <= maxZ_Lab256)
                {
                    audioManager.PlayAccessibleDescription(lab_256);
                }

                if (sourcePosition.x >= minX_Hallway_2 && sourcePosition.x <= maxX_Hallway_2 &&
                     sourcePosition.y >= minY_Hallway_2 &&
                    sourcePosition.z >= minZ_Hallway_2 && sourcePosition.z <= maxZ_Hallway_2)
                {
                    audioManager.PlayAccessibleDescription(hallway_2);
                }

                if (sourcePosition.x >= minX_Hallway_2_1 && sourcePosition.x <= maxX_Hallway_2_1 &&
                     sourcePosition.y >= minY_Hallway_2_1 &&
                    sourcePosition.z >= minZ_Hallway_2_1 && sourcePosition.z <= maxZ_Hallway_2_1)
                {
                    audioManager.PlayAccessibleDescription(hallway_2);
                }

                if (sourcePosition.x >= minX_Stairs && sourcePosition.x <= maxX_Stairs &&
                     sourcePosition.y >= minY_Stairs && sourcePosition.y <= maxY_Stairs &&
                    sourcePosition.z >= minZ_Stairs && sourcePosition.z <= maxZ_Stairs)
                {
                    audioManager.PlayAccessibleDescription(stairs);
                }


                if (sourcePosition.x >= minX_Hallway_1 && sourcePosition.x <= maxX_Hallway_1 &&
                     sourcePosition.y <= maxY_Hallway_1 &&
                    sourcePosition.z >= minZ_Hallway_1 && sourcePosition.z <= maxZ_Hallway_1)
                {
                    audioManager.PlayAccessibleDescription(hallway_1);
                }


                if (sourcePosition.x >= minX_Hallway_1_1 && sourcePosition.x <= maxX_Hallway_1_1 &&
                     sourcePosition.y <= maxY_Hallway_1_1 &&
                    sourcePosition.z >= minZ_Hallway_1_1 && sourcePosition.z <= maxZ_Hallway_1_1)
                {
                    audioManager.PlayAccessibleDescription(hallway_1);
                }

                if (sourcePosition.x >= minX_Hallway_1_2 && sourcePosition.x <= maxX_Hallway_1_2 &&
                     sourcePosition.y <= maxY_Hallway_1_2 &&
                    sourcePosition.z >= minZ_Hallway_1_2 && sourcePosition.z <= maxZ_Hallway_1_2)
                {
                    audioManager.PlayAccessibleDescription(hallway_1);
                }

                if (sourcePosition.x >= minX_Secretary && sourcePosition.x <= maxX_Secretary &&
                     sourcePosition.y <= maxY_Secretary &&
                    sourcePosition.z >= minZ_Secretary && sourcePosition.z <= maxZ_Secretary)
                {
                    audioManager.PlayAccessibleDescription(secretary);
                }

                if (sourcePosition.x >= minX_Lab151 && sourcePosition.x <= maxX_Lab151 &&
                     sourcePosition.y <= maxY_Lab151 &&
                    sourcePosition.z >= minZ_Lab151 && sourcePosition.z <= maxZ_Lab151)
                {
                    audioManager.PlayAccessibleDescription(lab_151);
                }

            }
        }
    }
}