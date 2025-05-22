using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPickCan : TutorialStep
{
    protected HoldableItem canPickable;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        canPickable = levelController.envItemManager.can.GetComponent<HoldableItem>();
        canPickable.transform.gameObject.AddComponent<HighLightedItem>().isHighlighted = true;
    }
    // Update is called once per frame
    public override bool TutorialUpdate()
    {
        return !canPickable.isHolding;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        canPickable.GetComponent<HighLightedItem>().isHighlighted = false;
    }
}
