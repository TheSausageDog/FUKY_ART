using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveKnob : InteractItemBase
{
    protected Animator animator;

    protected bool isOn = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnInteract()
    {
        isOn = !isOn;
        animator.SetBool("open", isOn);
    }
}
