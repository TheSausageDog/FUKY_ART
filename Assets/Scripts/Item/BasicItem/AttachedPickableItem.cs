using UnityEngine;

public  class AttachedPickableItem : NormalPickableItem
{

    Transform attachPoint;

    Collider attachActiveArea;

    public override void OnThrow()
    {    
        base.OnThrow();
        

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