using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBreakCan : TutorialStep
{
    protected CanContainer can;

    protected HighLightedItem knifeHightlighted;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        can = levelController.envItemManager.can.GetComponent<CanContainer>();
        knifeHightlighted = levelController.envItemManager.knife.AddComponent<HighLightedItem>();
        knifeHightlighted.isHighlighted = true;
    }
    // Update is called once per frame
    public override bool TutorialUpdate()
    {
        return !can.opened;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        knifeHightlighted.isHighlighted = false;
    }
}
