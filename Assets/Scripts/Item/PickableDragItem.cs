using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableDragItem : BasePickableItem
{
    public override InteractionType interactionType{ get{ return InteractionType.Drag; } }


    public Transform dragObject;

    void Update()
    {
        if(dragObject != null){
            transform.LookAt(dragObject.position, Vector3.up);
            Vector3 rotation = transform.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            transform.eulerAngles = rotation;
        }
    }

    public override void OnPickup(Transform holdPos)
    {
        dragObject = holdPos;
        isPicking = true;
    }

    public override void OnThrow()
    {
        dragObject = null;
        isPicking = false;
    }
}
