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

    public override void Start()
    {
        base.Start();
        foodTray = levelController.envItemManager.tray;
        menuPickable = levelController.envItemManager.menu.GetComponent<BasePickableItem>();
        animator = foodTray.GetComponent<Animator>();
        foodTray.SetActive(true);
    }

    void Update()
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
            if (watched && !menuPickable.isPicking)
            {
                menuPickable.gameObject.SetActive(false);
                EndStep();
            }
        }
    }
}
