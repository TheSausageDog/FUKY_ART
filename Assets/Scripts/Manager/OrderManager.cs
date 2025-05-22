using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//needs to be redesign
public class OrderManager : SingletonMono<OrderManager>
{
    public GameObject foodTray;

    // public GameObject cup;

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
        // cup.GetComponent<AttachedPickableItem>().ResetAttach();
        plate.GetComponent<AttachedPickableItem>().ResetAttach();
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
        // cup.tag = "canInteract";
        plate.tag = "canInteract";
    }

    public void ThrowMenu(OrderItem orderItem)
    {
        orderItem.gameObject.SetActive(false);
    }

    public void SubmitOrder()
    {
        if (submitArea.IsContain(foodTray))
        {
            if (submitArea.IsContain(plate))
            {
                TasteReport tasteReport = TasteManager.Instance.GetTaste(plate.GetComponent<TasteCollector>());
                Debug.Log(tasteReport);
                List<Food> foods = plate.GetComponent<TasteCollector>().checkArea.foods;
                foreach (Food food in foods)
                {
                    Destroy(food.gameObject);
                }
            }
            else
            {
                Debug.Log("没有菜盘子");
            }

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