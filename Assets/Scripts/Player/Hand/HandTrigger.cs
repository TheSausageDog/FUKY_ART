using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandTrigger : MonoBehaviour
{
     public PickUpAndInteract pick;

     private Camera mainCamera;

     private void Awake()
     {
         mainCamera = Camera.main;
     }

    //  private void OnTriggerEnter(Collider other)
    // {
    //     pick.OnHandTriggerEnter(other.gameObject);
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     pick.OnHandTriggerExit(other.gameObject);
    // }

    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        var dir = transform.position - mainCamera.transform.position;
        int count=Physics.RaycastNonAlloc(mainCamera.transform.position, dir, hits,pick.pickUpRange);
        
        Array.Sort(hits, 0, count, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));
        
        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("canInteract"))
            {
                pick.OnHandTriggerEnter(hits[i].collider.gameObject);
//                Debug.Log(hits[i].collider.gameObject.name);
                return;
            }
        }
        if(pick.currentHandObj!=null)pick.OnHandTriggerExit(pick.currentHandObj._transform.gameObject);
    }
}
