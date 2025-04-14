using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveToTable : TutorialStep
{
    protected TargetTrigger targetTrigger;

    protected bool stepOn = false;

    // Start is called before the first frame update
    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        targetTrigger = levelController.areaTrigger.Find("TableFrontArea").GetComponent<TargetTrigger>();
        targetTrigger.trigged += StepOn;
    }


    public void StepOn()
    {
        stepOn = true;
    }

    public override void TutorialEnd()
    {
        targetTrigger.trigged -= StepOn;
        base.TutorialEnd();
    }
}
