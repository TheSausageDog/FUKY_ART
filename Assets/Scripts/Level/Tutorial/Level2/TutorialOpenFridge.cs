using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOpenFridge : TutorialStep
{
    FridgeItem fridge;

    protected HighLightedItem fridgeHightlighted;


    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        fridge = levelController.envItemManager.fridge.transform.GetChild(1).GetComponent<FridgeItem>();
        fridgeHightlighted = levelController.envItemManager.fridge.AddComponent<HighLightedItem>();
        fridgeHightlighted.isHighlighted = true;
    }

    public override bool TutorialUpdate()
    {
        return !fridge.isPicking;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        fridgeHightlighted.isHighlighted = false;
    }
}
