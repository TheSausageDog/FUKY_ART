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
    protected HighLightedItem tableHighLight;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foodTray = levelController.envItemManager.tray;
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
        foodTrayPickable = foodTray.GetComponent<BasePickableItem>();
        foodTrayCollider = foodTray.GetComponent<Collider>();


        if(!_levelController.envItemManager.table.TryGetComponent<HighLightedItem>(out tableHighLight)){
            tableHighLight = _levelController.envItemManager.table.AddComponent<HighLightedItem>();
        }
        tableHighLight.isHighlighted = true;
        foodTrayPickable.AddComponent<HighLightedItem>().isHighlighted = true;
    }

    public override bool TutorialUpdate()
    {
        return !containRecorder.IsContain(foodTrayCollider) || foodTrayPickable.isPicking;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        tableHighLight.isHighlighted = false;
        foodTrayPickable.GetComponent<HighLightedItem>().isHighlighted = false;
    }
}
