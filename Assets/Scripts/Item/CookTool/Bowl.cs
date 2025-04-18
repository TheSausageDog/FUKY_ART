using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// 碗类，检测碗中的食物和味道成分，并进行评分。
/// 继承自 BasePickableItem。
/// </summary>
[RequireComponent(typeof(LiquidContainer))]
public class Bowl : AttachedPickableItem
{
    public override bool isInteractable { get { return true; } }

    public FoodRecorder checkArea;

    public LiquidContainer liquidContainer;
    // public string testRecipeName; // 测试配方名称
    // public Text text; // 显示评分信息的文本

    // private float pepperAmount = 0; // 胡椒量
    // private Collider[] results = new Collider[128]; // 检测到的碰撞体数组
    // private HashSet<GameObject> pepperSet = new HashSet<GameObject>(); // 胡椒对象集合

    public override void Awake()
    {
        base.Awake();
        liquidContainer = GetComponent<LiquidContainer>();
        liquidContainer.onLiquidChanged += AddLiquid;
    }

    // public override void Update()
    // {
    //     base.Update();

    // }

    protected void AddLiquid(float delta_volume, WaterFlow source)
    {
        if (source && source.TryGetComponent<Sauce>(out Sauce sauce))
        {
            foreach (var food in checkArea.mainIngredient)
            {
                foreach (var taste in sauce.tastes)
                {
                    if (food.tastes.ContainsKey(taste.Key))
                    {
                        food.tastes[taste.Key] += delta_volume * taste.Value / checkArea.mainIngredient.Count;
                    }
                    else
                    {
                        food.tastes[taste.Key] = delta_volume * taste.Value / checkArea.mainIngredient.Count;
                    }

                }
            }
        }
    }

    public override void OnInteract()
    {
        var tasteList = new Dictionary<TasteType, float>();
        var foodVolumeDic = new Dictionary<FoodType, float>();
        var foodList = new List<Food>();
        string info = "";
        // $"菜品: {testRecipeName}\n";

        foreach (var food in checkArea.foods)
        {
            foodList.Add(food);
            // if (food.Tastes == null) continue;

            foreach (var taste in food.tastes)
            {
                if (tasteList.ContainsKey(taste.Key))
                {
                    tasteList[taste.Key] += taste.Value;
                }
                else
                {
                    tasteList[taste.Key] = taste.Value;
                }
                // int index = tasteList.FindIndex(t => t.tasteType == taste.tasteType);
                // if (index != -1)
                // {
                //     var temp = tasteList[index];
                //     temp.tasteValue += taste.tasteValue * pepperAmount;
                //     tasteList[index] = temp;
                // }
                // else
                // {
                //     tasteList.Add(taste);
                // }
            }

            // if (foodVolumeDic.ContainsKey(food.foodType))
            // {
            //     foodVolumeDic[food.foodType] += food.volume;
            // }
            // else
            // {
            //     foodVolumeDic.Add(food.foodType, food.volume);
            // }
        }

        // if (pepperAmount > 0)
        // {
        //     foreach (var taste in FoodManager.Instance.GetStandardValue(FoodType.Pepper))
        //     {
        //         int index = tasteList.FindIndex(t => t.tasteType == taste.tasteType);
        //         if (index != -1)
        //         {
        //             var temp = tasteList[index];
        //             temp.tasteValue += taste.tasteValue * pepperAmount;
        //             tasteList[index] = temp;
        //         }
        //         else
        //         {
        //             tasteList.Add(taste);
        //         }
        //     }
        // }

        // foreach (var kvp in foodVolumeDic)
        // {
        //     info += $"包含食物类型：{kvp.Key}\n";
        //     info += $"体积为：{Mathf.Max(0.01f, kvp.Value):F}\n";
        // }

        foreach (var taste in tasteList)
        {
            info += $"当前食物味道：{taste.Key}\n";
            info += $"值为：{taste.Value:F}\n";
        }

        // var rating = RecipeManager.Instance.GetRecipe(testRecipeName).EvaluateRecipe(foodVolumeDic, tasteList, foodList);
        // info += $"评分：{rating}\n";

        // text.text = info;
        Debug.Log(info);
    }

    // public override void Interact(InteractionType type, params object[] args)
    // {
    //     base.Interact(type, args);
    //     if (type == InteractionType.Interact)
    //     {
    //         text.enabled = !text.enabled;
    //     }
    // }



    // private void OnDrawGizmos()
    // {
    //     // Gizmos.color = Color.red;
    //     // Gizmos.DrawWireSphere(checkCenter.position, checkRadius);
    // }
}
