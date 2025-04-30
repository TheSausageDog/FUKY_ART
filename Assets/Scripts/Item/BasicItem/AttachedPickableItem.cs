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
            transform.position = attachPoint.transform.position;
            transform.rotation = attachPoint.transform.rotation;
            if (!keepRigbody)
            {
                Destroy(itemRigidbody);
                itemRigidbody = null;
            }
        }
        // if (attachPoint != null)
        // {
        //     float dist = Vector3.Distance(attachPoint.position, transform.position);
        //     if (dist < 0.5)
        //     {
        //         // Destroy(gameObject.GetComponent<Rigidbody>());
        //         transform.parent = attachPoint;
        //     }
        // }
    }
}