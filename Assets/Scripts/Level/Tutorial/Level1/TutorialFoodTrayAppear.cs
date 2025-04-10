using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialFoodTrayAppear : TutorialStep
{
    public GameObject foodTray;

    public BasePickableItem menuPickable;

    protected Animator animator;

    protected bool watched = false;

    public override void Start()
    {
        base.Start();
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
        if (foodTray != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("FoodTrayAnimation"))
            {
                while (foodTray.transform.childCount != 0)
                {
                    foodTray.transform.GetChild(0).parent = null;
                }
                Destroy(foodTray);
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
