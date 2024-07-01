using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObjectsOnSeen : MonoBehaviour
{
    private Renderer[] renderers;

    private Metadata metadata;

    public GameObject cameraTarget;

    void Awake ()
    {
        GameObject visibleObject = GameObject.FindGameObjectWithTag ("VisibleObject");
        renderers = visibleObject.GetComponentsInChildren<Renderer> ();
    }

    void Update() 
    {
        OutputVisibleRenderers(renderers);
    }

    void OutputVisibleRenderers (Renderer[] renderers)
    {
        foreach (var renderer in renderers)
        {
            // output only the visible renderers' name
            if (IsVisible(renderer)) 
            {

                Vector3 sourcePosition = renderer.gameObject.transform.position;

                metadata = renderer.gameObject.GetComponent<Metadata>();

                if(metadata != null)
                {                                
                    Vector3 cameraPosition = cameraTarget.transform.position;
                    float distance = Vector3.Distance(sourcePosition, cameraPosition);
                    float formattedDistance = Mathf.Round(distance * 10f) / 10f; 
                    Debug.Log (metadata.objectName + " is detected at " + formattedDistance  + " meters");
                }

                // Debug.Log (renderer.gameObject.name + " is detected in position" + sourcePosition);

            }  
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
