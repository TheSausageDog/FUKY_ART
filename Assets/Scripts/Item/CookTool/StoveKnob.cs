using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveKnob : ActivatableItem
{
    protected Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Active()
    {
        animator.SetBool("open", true);
    }

    public override void Deactive()
    {
        animator.SetBool("open", false);
    }
}
