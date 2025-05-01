using System.Collections.Generic;

public class SwitchItem : BaseItem
{
    public override bool isHoldable { get { return false; } }

    public override bool isInteractable { get { return true; } }

    public List<ActivatableItem> activatableItems;

    protected bool isActive = false;

    public override void OnInteract()
    {
        isActive = !isActive;
        if (isActive)
        {
            foreach (var item in activatableItems)
            {
                item.Active();
            }
        }
        else
        {
            foreach (var item in activatableItems)
            {
                item.Deactive();
            }
        }
    }
}