using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTakeGarnish : TutorialStep
{
    protected ContainRecorder containRecorder;

    public override void Start()
    {
        base.Start();
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
    }

    void Update()
    {
        bool hasMushroom = false, hasPepper = false;
        foreach(var inside in containRecorder.inside){
            if(inside != null && inside.TryGetComponent<Food>(out Food food)){
                FoodType foodType = food.foodType;
                if(foodType == FoodType.Mushroom){hasMushroom = true;}
                if(foodType == FoodType.Pepper){hasPepper = true;}
            }
        }
        if(hasMushroom && hasPepper){
            EndStep();
        }
    }
}
