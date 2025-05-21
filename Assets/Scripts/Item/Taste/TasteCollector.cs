using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TasteCollector : MonoBehaviour
{
    public FoodRecorder checkArea;

    public LiquidContainer liquidContainer;

    // private float pepperAmount = 0; // 胡椒量
    // private Collider[] results = new Collider[128]; // 检测到的碰撞体数组
    // private HashSet<GameObject> pepperSet = new HashSet<GameObject>(); // 胡椒对象集合

    public void Awake()
    {
        liquidContainer.onLiquidChanged += AddLiquid;
    }

    protected void AddLiquid(Dictionary<WaterFlow, int> add, Dictionary<WaterFlow, int> minus)
    {
        foreach (WaterFlow flow in add.Keys)
        {
            float ave_adding = add[flow] / checkArea.mainIngredient.Count;
            if (flow.TryGetComponent<Sauce>(out Sauce sauce))
            {
                foreach (var taste in sauce.tastes)
                {
                    float ave_taste = ave_adding * taste.Value;
                    foreach (var food in checkArea.mainIngredient)
                    {
                        if (food.tastes.ContainsKey(taste.Key))
                        {
                            food.tastes[taste.Key] += ave_taste;
                        }
                        else
                        {
                            food.tastes[taste.Key] = ave_taste;
                        }
                    }
                }
            }
        }
    }
}
