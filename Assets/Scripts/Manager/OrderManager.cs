using System;
using System.Collections.Generic;
using Obi;
using UnityEngine;
using UnityEngine.Assertions;

//needs to be redesign
public class OrderManager : SingletonMono<OrderManager>
{
    public GameObject foodTray;

    // public GameObject cup;

    protected GameObject plate;
    public GameObject menu;

    public ContainRecorder submitArea;

    // public Transform trayTarget;
    // public Transform trayStart;


    protected Animator animator;
    // protected bool isTrayOnSite;
    protected Order order = null;
    public OrderItem orderItem;

    protected bool appear = true;

    void Start()
    {
        animator = foodTray.GetComponent<Animator>();
    }

    public void NewOrder(GameObject _plate, Recipe recipe = null)
    {
        foodTray.SetActive(true);
        plate = _plate;
        plate.SetActive(true);
        // cup.GetComponent<AttachedPickableItem>().ResetAttach();
        plate.GetComponent<AttachedPickableItem>().ResetAttach();
        plate.transform.parent = foodTray.transform;
        animator.enabled = true;
        appear = true;
        animator.Play("FoodTrayAnimation");
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
                if (stateInfo.normalizedTime >= 1.0f)
                {
                    if (appear && stateInfo.IsName("FoodTrayAnimation"))
                    {
                        animator.enabled = false;
                        foodTray.AddComponent<Rigidbody>();
                        plate.AddComponent<Rigidbody>();
                        // foodTray.AddComponent<NormalPickableItem>();
                        menu.tag = "canInteract";
                    }
                    else if (!appear && stateInfo.IsName("Submit"))
                    {
                        animator.enabled = false;
                        List<Food> foods = plate.GetComponent<TasteCollector>().checkArea.foods;
                        foreach (Food food in foods)
                        {
                            Destroy(food.gameObject);
                        }
                        plate.SetActive(false);
                        foodTray.SetActive(false);
                        orderItem = null;
                    }
                }
                // else
                // {
                //     foodTray.GetComponent<Rigidbody>().velocity = (trayTarget.position - foodTray.transform.position) * 2;
                // }
            }


            if (orderItem != null && PlayerInputController.Instance.IsRecipePressed())
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
                    if (food.TryGetComponent<Rigidbody>(out var rigidbody)) { Destroy(rigidbody); }
                    food.transform.parent = foodTray.transform;
                }
            }
            else
            {
                Debug.Log("没有菜盘子");
            }

            { if (foodTray.TryGetComponent<Rigidbody>(out var rigidbody)) { Destroy(rigidbody); } }
            { if (plate.TryGetComponent<ObiRigidbody>(out var rigidbody)) { Destroy(rigidbody); } }
            { if (plate.TryGetComponent<Rigidbody>(out var rigidbody)) { Destroy(rigidbody); } }
            appear = false;
            animator.enabled = true;
            animator.Play("Submit");
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