using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckItem : PickableItem
{
    public override bool isInteractable { get { return false; } }

    public float objectSize = 1;

    void Update()
    {
        if (isHolding && !PlayerInputController.IsMoveHandHeld())
        {
            transform.position = Camera.main.transform.position + objectSize * Camera.main.transform.forward;
            transform.LookAt(transform.position + Camera.main.transform.up, -Camera.main.transform.forward);
        }
    }
}
