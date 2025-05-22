using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class TutorialCutGarnish : TutorialStep
{
    protected ContainRecorder containRecorder;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
    }

    public override bool TutorialUpdate()
    {
        // bool hasMushroom = false, hasPepper = false;
        bool hasSlicer = false;
        foreach (var inside in containRecorder.inside)
        {
            if (inside != null && inside.TryGetComponent<Food>(out Food food) && inside.TryGetComponent<BzSliceableObject>(out BzSliceableObject sliceObj))
            {
                FoodType foodType = food.foodType;
                if (sliceObj.cutted)
                {
                    hasSlicer = true;
                    // if (foodType == FoodType.Mushroom) { hasMushroom = true; }
                    // if (foodType == FoodType.Pepper) { hasPepper = true; }
                }
            }
        }
        return !hasSlicer;
    }
}
