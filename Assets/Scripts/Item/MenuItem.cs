using UnityEngine;

public class OrderItem : CheckItem
{
    public override void OnPickup(Transform holdPos)
    {
        base.OnPickup(holdPos);
        OrderManager.Instance.PickMenu(this);
    }

    public override void OnThrow()
    {
        base.OnThrow();
        OrderManager.Instance.ThrowMenu(this);
    }
}