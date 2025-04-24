using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialServing : TutorialStep
{
    protected TargetTrigger targetTrigger;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        targetTrigger = levelController.areaTrigger.Find("ServingArea").GetComponent<TargetTrigger>();
    }

    public override bool TutorialUpdate()
    {
        if (targetTrigger.isInside)
        {//&& 按铃
            return false;
        }
        return true;
    }
}