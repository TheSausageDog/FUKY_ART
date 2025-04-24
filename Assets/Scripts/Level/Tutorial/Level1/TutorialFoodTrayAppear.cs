using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialFoodTrayAppear : TutorialStep
{
    protected CheckItem menuPickable;

    protected bool watched = false;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        OrderManager.Instance.NewOrder();
        menuPickable = levelController.envItemManager.menu.GetComponent<CheckItem>();
        menuPickable.AddComponent<HighLightedItem>().isHighlighted = true;
    }

    public override bool TutorialUpdate()
    {
        if (!watched)
        {
            if (menuPickable.isHolding)
            {
                watched = true;
            }
        }

        return !watched || menuPickable.isHolding;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        menuPickable.gameObject.SetActive(false);
        if (menuPickable.TryGetComponent<HighLightedItem>(out var highLighted))
        {
            highLighted.isHighlighted = false;
        }
    }
}
