using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveToTable : TutorialStep
{
    protected TargetTrigger targetTrigger;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
        targetTrigger = levelController.areaTrigger.Find("TableFrontArea").GetComponent<TargetTrigger>();
        targetTrigger.trigged += StepOn;
    }

    
    public void StepOn()
    {
        EndStep();
    }

    public override void EndStep()
    {
        targetTrigger.trigged -= StepOn;
        base.EndStep();
    }
}
