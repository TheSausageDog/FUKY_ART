using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsManager : SingletonMono<TipsManager>
{
    public bool showUI
    {
        set
        {
            animator.SetBool("ShowTipsUI", value);
            animator.SetBool("Active", true);
        }
    }

    public Transform missionUI = null;

    [NonSerialized]
    protected Text uiTipsText;

    [NonSerialized]
    protected UGUISpriteAnimation uiSpriteAnimation;
    [NonSerialized]
    protected Image uiTipsImage;

    protected Animator animator;

    protected bool task_dirty = false;

    protected void Awake()
    {
        animator = missionUI.GetComponent<Animator>();
        uiTipsText = missionUI.GetChild(0).GetComponent<Text>();
        uiSpriteAnimation = missionUI.GetChild(1).GetComponent<UGUISpriteAnimation>();
        uiTipsImage = missionUI.GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (task_dirty)
    //     {
    //         // if (tasks.Count != 0)
    //         // {
    //         //     foreach (var task in tasks)
    //         //     {
    //         //         uiTaskText.text = task.Key;
    //         //         if (task.Value.steps.Count != 0)
    //         //         {
    //         //             foreach (var step in task.Value.steps)
    //         //             {
    //         //                 uiDetailsText.text = step.Key;
    //         //                 break;
    //         //             }
    //         //         }
    //         //         else
    //         //         {
    //         //             uiDetailsText.text = "";
    //         //         }
    //         //         break;
    //         //     }
    //         // }
    //         // else
    //         // {
    //         //     uiTaskText.text = "";
    //         //     uiDetailsText.text = "";
    //         // }
    //         task_dirty = false;
    //     }
    // }

    public void ShowTextTips(string tips)
    {
        uiTipsText.enabled = true;
        uiTipsText.text = tips;
        uiTipsImage.enabled = false;
    }

    public void ShowAnimatedTips(List<Sprite> spriteFrames)
    {
        if (spriteFrames != null && spriteFrames.Count != 0)
        {
            uiTipsImage.enabled = true;
            uiSpriteAnimation.SpriteFrames = spriteFrames;
            uiSpriteAnimation.Play();
            uiTipsText.enabled = false;
        }
        else
        {
            uiSpriteAnimation.Pause();
        }
    }
}