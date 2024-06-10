using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasUsed : MonoBehaviour
{

    public bool wasItUsed;

    public bool wasItHidden;

    void Start()
    {
        wasItUsed = false;
    }

    public void StopHidden(){
        wasItHidden = false;
    }

}
