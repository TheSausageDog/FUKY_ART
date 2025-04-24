using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialMoveToTable : TutorialStep
{
    protected TargetTrigger targetTrigger;

    protected bool stepOn = false;

    protected Transform tableObject;

    // Start is called before the first frame update
    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        targetTrigger = levelController.areaTrigger.Find("TableFrontArea").GetComponent<TargetTrigger>();
        targetTrigger.trigged += StepOn;
        tableObject = _levelController.envItemManager.table.transform;
        tableObject.AddComponent<HighLightedItem>().isHighlighted = true;
    }

    public override bool TutorialUpdate()
    {
        return !stepOn;
    }

    public void StepOn()
    {
        stepOn = true;
    }

    public override void TutorialEnd()
    {
        targetTrigger.trigged -= StepOn;
        base.TutorialEnd();
        tableObject.GetComponent<HighLightedItem>().isHighlighted = false;
    }
}
