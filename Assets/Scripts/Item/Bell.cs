using UnityEngine;

public class Bell : InteractItemBase
{
    protected Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnInteract()
    {
        animator.SetTrigger("Ring");
    }
}