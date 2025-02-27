using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandTrigger : MonoBehaviour
{
     public PickUpScript pick;

    private void OnTriggerEnter(Collider other)
    {
        pick.OnHandTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        pick.OnHandTriggerExit(other);
    }
}
