using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialFoodTrayAppear : TutorialStep
{
    protected GameObject foodTray;

    protected BasePickableItem menuPickable;

    protected Animator animator;

    protected bool watched = false;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foodTray = levelController.envItemManager.tray;
        menuPickable = levelController.envItemManager.menu.GetComponent<BasePickableItem>();
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
                Debug.Log("watched");
            }
        }
        if (animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("FoodTrayAnimation"))
            {
                animator.enabled = false;
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
    }
}
