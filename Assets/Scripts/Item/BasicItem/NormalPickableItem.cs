using UnityEngine;

//Just moving
public class NormalPickableItem : PickableItem
{
    public Rigidbody itemRigidbody {get; protected set;}

    public override void Awake()
    {
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public override void OnPickup(Transform holdPos)
    {
        SetPickedRigidbody(itemRigidbody);
        base.OnPickup(holdPos);
    }

    public override void OnThrow()
    {
        SetDropRigidbody(itemRigidbody);
        base.OnThrow();
    }
}