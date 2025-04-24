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

    // private float pepperAmount = 0; // 胡椒量
    // private Collider[] results = new Collider[128]; // 检测到的碰撞体数组
    // private HashSet<GameObject> pepperSet = new HashSet<GameObject>(); // 胡椒对象集合

    public override void Awake()
    {
        base.Awake();
        liquidContainer = GetComponent<LiquidContainer>();
        liquidContainer.onLiquidChanged += AddLiquid;
    }

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
        string info = "";
        // $"菜品: {testRecipeName}\n";
        var tasteList = ForetasteManager.Instance.Foretaste(checkArea.foods);

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
        // Debug.Log(info);
    }
}
