using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectData
{
    public string objName;
    public Vector3 objPosition;

    // Constructor to initialize with values
    public ObjectData(string objectDataName, Vector3 objectDataPosition)
    {
        objName = objectDataName;
        objPosition = objectDataPosition;
    }
}

// [System.Serializable]
// public class ObjectCamera
// {
//     public string objName;
//     public Vector3 objPosition;
//     public Quaternion objRotation;

//     // Constructor to initialize with values
//     public ObjectData(string objectDataName, Vector3 objectDataPosition, Quaternion objectDataRotation)
//     {
//         objName = objectDataName;
//         objPosition = objectDataPosition;
//         objRotation = objectDataRotation;
//     }
// }

public class CheckObjectsOnSeen : MonoBehaviour
{
    private Renderer[] renderers;

    private Metadata metadata;

    public GameObject cameraTarget;

    private bool active;

    public ObjectData[] objects = new ObjectData[15]; 

    void Awake ()
    {
        GameObject visibleObject = GameObject.FindGameObjectWithTag ("VisibleObject");
        renderers = visibleObject.GetComponentsInChildren<Renderer> ();
        active = false;
    }

    void Update() 
    {

        //This is for Desktop Testing:
         if (Input.GetKeyDown(KeyCode.C))
        {
            if(!active)
            {
                active = true;
                OutputVisibleRenderers(renderers);
            }
            
        }
    }

    void OutputVisibleRenderers (Renderer[] renderers)
    {
        ResetObjectsArray();

        Vector3 cameraPosition = cameraTarget.transform.position;

        // Quaternion cameraRotation = cameraTarget.transform.rotation;

        // ObjectCamera objectCamera = new  ObjectCamera("Camera", cameraPosition,cameraRotation);

        ObjectData objectCamera = new  ObjectData("Camera", cameraPosition);

        objects[0] = objectCamera;

        foreach (var renderer in renderers)
        {
            // output only the visible renderers' name
            if (IsVisible(renderer)) 
            {

                Vector3 sourcePosition = renderer.gameObject.transform.position;

                metadata = renderer.gameObject.GetComponent<Metadata>();
                
                // Renderer only works for mesh components. It won't get Metadata if it is an empty parent without a mesh
                // Debug.Log(renderer.gameObject.name);

                if(metadata != null)
                {                                
                    // Vector3 cameraPosition = cameraTarget.transform.position;
                    // float distance = Vector3.Distance(sourcePosition, cameraPosition);
                    // float formattedDistance = Mathf.Round(distance * 10f) / 10f; 
                    // Debug.Log (metadata.objectName + " is detected at " + formattedDistance  + " meters");
                    
                    ObjectData newObj = new  ObjectData(metadata.objectName, sourcePosition);



                    for (int i = 0; i < objects.Length; i++)
                    {
                        if (objects[i].objName == "")
                        {
                            objects[i] = newObj;
                            break; // Exit the loop once added
                        } 
                    }
                }

                // Debug.Log (renderer.gameObject.name + " is detected in position" + sourcePosition);

            }  
        }

        active = false;

        string result = "List contents: ";
        foreach (var item in objects)
        {
            result += item.objName.ToString() + " " + item.objPosition.ToString() + "---";
        }
        Debug.Log(result);


    }

    private void ResetObjectsArray()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].objName = "";
            objects[i].objPosition = Vector3.zero;
        }

    }

    private bool IsVisible(Renderer renderer) 
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes , renderer.bounds))
            return true;
        else
            return false;
    }
}
