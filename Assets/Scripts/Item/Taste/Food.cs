using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 食物类，表示可切割的食物对象。
/// 继承自 BasePickableItem。
/// </summary>
public class Food : TasteObject
{
    public FoodType foodType; // 食物类型
    // public Color foodColor; // 食物颜色

    protected Material[] materials;

    protected float heated = 0f;

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
        Renderer[] renderers = GetComponents<Renderer>();
        Renderer[] renderers2 = GetComponentsInChildren<Renderer>();
        int length = renderers.Length + renderers2.Length;
        materials = new Material[length];
        int i = 0;
        while (i < renderers.Length)
        {
            materials[i] = renderers[i].material;
            ++i;
        }
        while (i < length)
        {
            materials[i] = renderers2[i - renderers.Length].material;
            ++i;
        }
    }

    public virtual void Heat(float _heat)
    {
        heated += _heat;
        if (heated < 5)
        {
            foreach (var material in materials)
            {
                material.SetColor("_MainColor", Color.white);
            }
        }
        else if (heated < 10)
        {
            foreach (var material in materials)
            {
                material.SetColor("_MainColor", new Color(138f / 255, 51 / 255, 36 / 255));
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.SetColor("_MainColor", Color.white * 0.1f);
            }
        }
    }

}
