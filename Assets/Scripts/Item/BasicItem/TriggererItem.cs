using System.Collections.Generic;

public class TriggererItem : InteractItemBase
{
    public List<InteractItemBase> activatableItems;

    public override void OnInteract()
    {
        foreach (var item in activatableItems)
        {
            item.OnInteract();
        }
    }
}