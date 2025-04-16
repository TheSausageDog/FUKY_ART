using UnityEngine;

public class FridgeItem : DragItem
{
    public float speed = 1f;

    protected override void MoveItem(Vector3 targetPosition)
    {
        Vector3 forward = targetPosition - transform.position;
        forward.y = 0;
        if (forward.x > 0) { forward.x = 0; }
        Quaternion target_rot = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rot, speed * Time.deltaTime);
    }

    public override void OnPickup(Transform _holdPos)
    {
        base.OnPickup(_holdPos);
    }
}