using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOpenFridge : TutorialStep
{
    PickableDragItem fridge;

    public override void Start()
    {
        base.Start();
        fridge = levelController.envItemManager.fridge.transform.GetChild(1).GetComponent<PickableDragItem>();
    }

    void Update()
    {
        if(fridge.isPicking){
            EndStep();
        }
    }
}
