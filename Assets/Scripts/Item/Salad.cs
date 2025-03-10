using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Salad : MonoBehaviour
{
    public float angleTreshold = 30f;
    bool isUp =true;
    private Coroutine generationCoroutine = null;
    
    public GameObject item;
    public Transform generatePos;
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
        StartGeneration();
    }

    private void StartGeneration()
    {
        generationCoroutine = StartCoroutine(Generate());
    }
    IEnumerator Generate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(item, generatePos.position, Quaternion.identity);
        }  
    }
    private void EndDrop()
    {
        
        Debug.Log("end");
        StopGeneration();
    }

    private void StopGeneration()
    {
        if(generationCoroutine != null)
        {
            StopCoroutine(generationCoroutine);
            generationCoroutine = null;
        }
    }
}