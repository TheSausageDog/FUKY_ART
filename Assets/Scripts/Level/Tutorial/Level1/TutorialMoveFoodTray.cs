using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialMoveFoodTray : TutorialStep
{
    protected GameObject foodTray;

    protected BasePickableItem foodTrayPickable;
    protected Collider foodTrayCollider;
    protected ContainRecorder containRecorder;
    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foodTray = levelController.envItemManager.tray;
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
        foodTrayPickable = foodTray.GetComponent<BasePickableItem>();
        foodTrayCollider = foodTray.GetComponent<Collider>();
    }

    public override bool TutorialUpdate()
    {
        return !containRecorder.IsContain(foodTrayCollider) || foodTrayPickable.isPicking;
    }
}
