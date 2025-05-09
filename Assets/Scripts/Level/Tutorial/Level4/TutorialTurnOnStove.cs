using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialTurnOnStove : TutorialStep
{
    protected Stove stove;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        OrderManager.Instance.NewOrder();
        Utils.SetLayerRecursive(levelController.envItemManager.stoveSwitch.transform, "OutlineSecond");
        stove = levelController.envItemManager.stove.GetComponent<Stove>();
    }

    public override bool TutorialUpdate()
    {
        return !stove.isFireOn;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        Utils.SetLayerRecursive(levelController.envItemManager.stoveSwitch.transform, "Default");
    }
}
