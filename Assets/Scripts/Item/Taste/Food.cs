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

    protected delegate void SetMaterial(Material material);
    protected void SetMaterials(SetMaterial f)
    {
        foreach (var material in materials)
        {
            f(material);
            material.SetColor("_MainColor", Color.white);
        }
    }

    ///
    ///  raw       cooked    burned
    /// 0--2.5----7.5--10------15
    ///
    public virtual void Heat(float _heat)
    {
        heated += _heat;
        if (heated < 2.5)
        {
            SetMaterials((Material material) => { material.SetColor("_MainColor", Color.white); });
        }
        else if (heated < 7.5)
        {
            float alpha = (heated - 2.5f) / 5;
            Color color = Color.Lerp(Color.white, new Color(138f / 255, 51 / 255, 36 / 255), alpha);
            SetMaterials((Material material) => { material.SetColor("_MainColor", color); });
        }
        else if (heated < 10)
        {
            SetMaterials((Material material) => { material.SetColor("_MainColor", new Color(138f / 255, 51 / 255, 36 / 255)); });
        }
        else if (heated < 15)
        {
            float alpha = (heated - 10f) / 5;
            Color color = Color.Lerp(new Color(138f / 255, 51 / 255, 36 / 255), Color.white * 0.1f, alpha);
            SetMaterials((Material material) => { material.SetColor("_MainColor", color); });

        }
        else
        {
            foreach (var material in materials)
            {
                material.SetColor("_MainColor", Color.white * 0.1f);
            }
        }
    }

    public virtual void DryHeat(float _heat)
    {
        if (heated < 11)
        {
            heated = 11;
        }
        Heat(_heat);
    }
}
