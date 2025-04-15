using UnityEngine;

public class FridgeItem : DragItem
{
    protected override void MoveItem(Vector3 targetPosition)
    {
        // if(dragObject != null){
        //     transform.LookAt(dragObject.position, Vector3.up);
        //     Vector3 rotation = transform.eulerAngles;
        //     rotation.x = 0;
        //     rotation.z = 0;
        //     transform.eulerAngles = rotation;
        // }
    }
}