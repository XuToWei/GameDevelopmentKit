using UnityEngine;
using System.Collections.Generic;

public class UnityObjectType : MonoBehaviour
{
    public List<UnityEngine.Object> objs; // Assign to this in the Editor

    void Start()
    {
        if(!ES3.KeyExists("this"))
            ES3.Save("this", this);
        else
            ES3.LoadInto("this", this);
        
        foreach(var obj in objs)
            Debug.Log(obj);
    }
}
