using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandTrigger : MonoBehaviour
{
     public PickUpScript pick;

     private Camera mainCamera;

     private void Awake()
     {
         mainCamera = Camera.main;
     }

     private void OnTriggerEnter(Collider other)
    {
        pick.OnHandTriggerEnter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        pick.OnHandTriggerExit(other.gameObject);
    }

    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        var dir = transform.position - mainCamera.transform.position;
        int count=Physics.RaycastNonAlloc(mainCamera.transform.position, dir, hits,10f);
        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("canPickUp"))
            {
                pick.OnHandTriggerEnter(hits[i].collider.gameObject);
                return;
            }
        }
        pick.OnHandTriggerExit(pick.gameObject);
    }
}
