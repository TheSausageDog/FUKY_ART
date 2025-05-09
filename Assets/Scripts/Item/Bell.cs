using UnityEngine;

public class Bell : InteractItemBase
{
    protected Animator animator;

    protected int bellHitCountDown;

    void Awake()
    {
        animator = GetComponent<Animator>();
        bellHitCountDown = Random.Range(1, 5);
    }

    public override void OnInteract()
    {
        animator.SetTrigger("Ring");
        if (--bellHitCountDown == 0)
        {
            OrderManager.Instance.SubmitOrder();
            bellHitCountDown = Random.Range(1, 5);
        }
    }
}