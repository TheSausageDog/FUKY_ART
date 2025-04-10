using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPickCan : TutorialStep
{
    protected BasePickableItem canPickable;

    public override void Start()
    {
        base.Start();
        canPickable = levelController.envItemManager.can.GetComponent<BasePickableItem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (canPickable.isPicking)
        {
            EndStep();
        }
    }
}
