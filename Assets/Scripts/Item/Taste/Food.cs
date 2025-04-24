using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 食物类，表示可切割的食物对象。
/// 继承自 BasePickableItem。
/// </summary>
public class Food : TasteObject
{
    public FoodType foodType; // 食物类型
    // public Color foodColor; // 食物颜色

    protected void SetTasteByType()
    {
        tastes.Clear();
        switch (foodType)
        {
            case FoodType.Mushroom:
                tastes[TasteType.Fresh] = 1f;
                break;
            case FoodType.Pepper:
                tastes[TasteType.Spicy] = 1f;
                break;
            case FoodType.Sausage:
                tastes[TasteType.Salty] = 1f;
                break;
        }
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Bounds bounds = VolumeCalculator.CalculateWorldBounds(gameObject);
    //     Gizmos.DrawWireCube(bounds.center, bounds.size);
    // }

    public override void Start()
    {
        if (foodType != FoodType.None)
        {
            SetTasteByType();
        }
        else
        {
            base.Start();
        }
        // CalculateTaste();
    }

}
