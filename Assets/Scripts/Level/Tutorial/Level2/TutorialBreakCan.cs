using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBreakCan : TutorialStep
{
    protected Container can;

    public override void Start()
    {
        base.Start();
        can = levelController.envItemManager.can.GetComponent<Container>();
    }
    // Update is called once per frame
    void Update()
    {
        if (can.opened)
        {
            EndStep();
        }
    }
}
