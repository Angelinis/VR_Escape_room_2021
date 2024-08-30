using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New User", menuName="User")]
public class UserData : ScriptableObject
{
    public bool internetConnection = true;
    public bool blind = true;

    public void SetUser(){
        blind = !blind;
    }

    public void SetInternetConnection(){
        internetConnection = !internetConnection;
    }
}


