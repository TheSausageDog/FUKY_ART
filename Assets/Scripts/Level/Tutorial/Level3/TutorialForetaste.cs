using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialForetaste : TutorialStep
{
    protected FoodRecorder foods;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foods = levelController.envItemManager.plate.GetComponent<TasteCollector>().checkArea;
    }

    public override bool TutorialUpdate()
    {
        return foods.foods.Count == 0;
    }
}
