using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOpenFridge : TutorialStep
{
    PickableDragItem fridge;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        fridge = levelController.envItemManager.fridge.transform.GetChild(1).GetComponent<PickableDragItem>();
    }

    public override bool TutorialUpdate()
    {
        return !fridge.isPicking;
    }
}
