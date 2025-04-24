using UnityEngine;
using UnityEngine.Assertions;

//needs to be redesign
public class OrderManager : SingletonMono<OrderManager>
{
    public GameObject foodTray;

    public GameObject cup;

    public GameObject plate;


    protected Animator animator;

    protected Order order = null;
    public OrderItem orderItem;


    public void NewOrder(Recipe recipe = null)
    {
        animator = foodTray.GetComponent<Animator>();
        foodTray.SetActive(true);
        order = new Order(recipe);
    }

    void Update()
    {
        if (order != null)
        {
            if (animator.enabled)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("FoodTrayAnimation"))
                {
                    animator.enabled = false;
                }
            }

            if (orderItem != null && PlayerInputController.IsRecipePressed())
            {
                if (orderItem.isHolding)
                {
                    if (orderItem != PlayerBlackBoard.heldItem)
                    {
                        Debug.LogError("some thing wrong in holding order");
                    }
                    PickUpAndInteract.Instance.DropObject();
                }
                else
                {
                    orderItem.gameObject.SetActive(true);
                    PickUpAndInteract.Instance.PickObject(orderItem);
                }
            }
        }
    }

    public void PickMenu(OrderItem orderItem)
    {
        if (this.orderItem != orderItem)
        {
            this.orderItem = orderItem;
        }
        //weird to write here
        foodTray.tag = "canInteract";
        cup.tag = "canInteract";
        plate.tag = "canInteract";
    }

    public void ThrowMenu(OrderItem orderItem)
    {
        orderItem.gameObject.SetActive(false);
    }

    public class Order
    {
        public Recipe recipe;

        public Order(Recipe recipe)
        {
            this.recipe = recipe;
        }
    }
}