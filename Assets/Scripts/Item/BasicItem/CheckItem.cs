using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItem : PickableItem
{
    public Transform dragPoint;
    public float objectSize = 1;

    void Update()
    {
        if (!PlayerInputController.IsMoveHandHeld()){
            transform.position = Camera.main.transform.position + objectSize * Camera.main.transform.forward;
            transform.LookAt(transform.position + Camera.main.transform.up, -Camera.main.transform.forward);
        }
    }
}
