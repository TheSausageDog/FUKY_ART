using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialFoodTrayAppear : TutorialStep
{
    protected GameObject foodTray;

    protected NormalPickableItem menuPickable;

    protected Animator animator;

    protected bool watched = false;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foodTray = levelController.envItemManager.tray;
        menuPickable = levelController.envItemManager.menu.GetComponent<NormalPickableItem>();
        animator = foodTray.GetComponent<Animator>();
        foodTray.SetActive(true);
    }

    public override bool TutorialUpdate()
    {
        if (!watched)
        {
            if (menuPickable.isPicking)
            {
                watched = true;
            }
        }
        if (animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("FoodTrayAnimation"))
            {
                animator.enabled = false;
                menuPickable.AddComponent<HighLightedItem>().isHighlighted = true;
            }
        }
        else
        {
            return !watched || menuPickable.isPicking;
        }
        return true;
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
