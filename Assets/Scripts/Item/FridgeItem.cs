using UnityEngine;

public class FridgeItem : DragItem
{
    public float speed = 1f;

    protected Vector3 temp_target;

    protected override void MoveItem(Vector3 targetPosition)
    {
        temp_target = targetPosition;
        Vector3 forward = targetPosition - transform.position;
        forward.y = 0;
        if (forward.x > 0) { forward.x = 0; }
        Quaternion target_rot = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rot, speed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(temp_target, 0.5f);
    }
}