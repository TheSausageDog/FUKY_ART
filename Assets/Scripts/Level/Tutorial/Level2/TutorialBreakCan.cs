using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBreakCan : TutorialStep
{
    protected Container can;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        can = levelController.envItemManager.can.GetComponent<Container>();
    }
    // Update is called once per frame
    public override bool TutorialUpdate()
    {
        return !can.opened;
    }
}
