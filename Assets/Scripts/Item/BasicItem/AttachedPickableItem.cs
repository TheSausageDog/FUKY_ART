using UnityEngine;

public class AttachedPickableItem : NormalPickableItem
{
    public AttachPoint attachPoint;

    public bool keepRigbody;

    public override void OnPickup(Transform _holdPos)
    {
        if (itemRigidbody == null)
            itemRigidbody = gameObject.AddComponent<Rigidbody>();
        base.OnPickup(_holdPos);
    }

    public override void OnThrow()
    {
        base.OnThrow();
        if (attachPoint != null && attachPoint.IsContain(itemCollider))
        {
            ResetAttach();
        }
    }

    public void ResetAttach()
    {
        transform.position = attachPoint.transform.position;
        transform.rotation = attachPoint.transform.rotation;
        if (!keepRigbody)
        {
            Destroy(itemRigidbody);
            itemRigidbody = null;
        }
    }
}