using System;
using UnityEngine;
using UnityEngine.Assertions;

//needs to be redesign
public class OrderManager : SingletonMono<OrderManager>
{
    public GameObject foodTray;

    public GameObject cup;

    public GameObject plate;
    public GameObject menu;

    public ContainRecorder submitArea;

    // public Transform trayTarget;
    // public Transform trayStart;


    protected Animator animator;
    // protected bool isTrayOnSite;
    protected Order order = null;
    public OrderItem orderItem;

    void Start()
    {
        animator = foodTray.GetComponent<Animator>();
    }

    public void NewOrder(Recipe recipe = null)
    {
        foodTray.SetActive(true);
        // foodTray.transform.position = trayStart.position;
        // foodTray.GetComponent<Rigidbody>().velocity = (trayTarget.position - foodTray.transform.position) * 4;
        order = new Order(recipe);
        // isTrayOnSite = false;
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
                    // foodTray.AddComponent<Rigidbody>();
                    //what a mass
                    foodTray.AddComponent<NormalPickableItem>();
                    // cup.AddComponent<Rigidbody>().mass = 3;
                    // cup.GetComponent<AttachedPickableItem>().enabled = true;
                    // plate.AddComponent<Rigidbody>().mass = 3;
                    // plate.GetComponent<AttachedPickableItem>().enabled = true;
                    // foodTray.GetComponent<Rigidbody>().mass = 3;
                    menu.tag = "canInteract";
                }
                // else
                // {
                //     foodTray.GetComponent<Rigidbody>().velocity = (trayTarget.position - foodTray.transform.position) * 2;
                // }
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

    public void SubmitOrder()
    {
        if (submitArea.inside.Contains(foodTray.GetComponent<Collider>()))
        {
            Debug.Log("提交了给评分");
            orderItem = null;
            foodTray.SetActive(false);
            animator.Play("FoodTrayAnimation");
            animator.enabled = true;
        }
        else
        {
            Debug.Log("没有餐盘");
        }
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