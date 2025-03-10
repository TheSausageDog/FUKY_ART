using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pepper : MonoBehaviour
{
    public float angleTreshold = 30f;
    bool isUp =true;
    public CubeParticleGenerator particleGenerator;
    
    private void Update()
    {
        if(isUp && Vector3.Angle(transform.up,-Vector3.up) < angleTreshold)
        {
            isUp = false;
            StartDrop();
        }
        else if(!isUp && Vector3.Angle(transform.up,-Vector3.up) > angleTreshold)
        {
            isUp = true;
            EndDrop();
        }
    }

    private void StartDrop()
    {
        Debug.Log("start");
        particleGenerator.StartGeneration();
    }
    private void EndDrop()
    {
        
        Debug.Log("end");
        
        particleGenerator.StopGeneration();
    }
}
