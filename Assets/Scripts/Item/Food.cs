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
public class Food : BasePickableItem
{
    [NonSerialized] public AllTaste Tastes; // 食物的味道属性
    public FoodType foodType; // 食物类型
    [NonSerialized] public float volume; // 食物体积
    public Color foodColor; // 食物颜色

    [Tooltip("小于这个值就不发射粒子")]
    public float minSplashSize = 0.1f;
    [Tooltip("大于这个值发射三个，小于这个值发射一个")]
    public float maxSplashSize = 0.3f;

    private Plane cutPlane; // 切割平面
    [NonSerialized] public Vector3 knifePos; // 刀的位置
    [NonSerialized] public Vector3 knifeDir; // 刀的方向
    [NonSerialized] public BzKnife Knife; // 刀的引用
    [NonSerialized] public float timer; // 计时器

    public bool cutted; // 是否已切割
    [HideInInspector] public float knifeDepth => VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, VolumeCalculator.CalculateWorldBounds(gameObject).size);
    }

    public override void Start()
    {
        base.Start();
        CalculateTaste();
    }

    /// <summary>
    /// 计算食物的味道属性。
    /// </summary>
    public async void CalculateTaste()
    {
        var standardValues = await FoodManager.Instance.GetStandardValueAsync(foodType);
        volume = await VolumeCalculator.CalculateVolumesAsync(gameObject);

        var tastes = new List<Taste>();

        foreach (var taste in standardValues)
        {
            var newTaste = taste;
            newTaste.tasteValue *= volume;
            tastes.Add(newTaste);
        }

        Tastes = new AllTaste(tastes);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BzKnife>(out var knife))
        {
            if (Knife == null) Knife = knife;
            knife.OnTouchFood(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BzKnife>(out var knife))
        {
            knifePos = Vector3.zero;
            knife.OnFoodExit(this);
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void Update()
    {
        if (knifePos != Vector3.zero)
        {
            Vector3 distance = Knife.transform.position - knifePos;
            Vector3 projection = Vector3.Project(distance, knifeDir);
            float dot = Vector3.Dot(projection, knifeDir);

            if (-dot > knifeDepth)
            {
                HandleSlice(cutPlane);
            }

            timer += Time.deltaTime;
            if (timer > 1 && Knife != null)
            {
                Knife.OnFoodExit(this);
            }
        }
    }

    /// <summary>
    /// 切割处理逻辑
    /// </summary>
    async void HandleSlice(Plane plane)
    {
        var slicer = GetComponent<IBzMeshSlicer>();
        var sliceResults = await slicer.SliceAsync(plane);
        gameObject.layer = LayerMask.NameToLayer("Default");
        cutted = true;

        if (sliceResults != null && sliceResults.resultObjects != null)
        {
            foreach (var resultObject in sliceResults.resultObjects)
            {
                resultObject.gameObject.layer = LayerMask.NameToLayer("Default");
                if (resultObject.gameObject.TryGetComponent(out Food slicedFood))
                {
                    slicedFood.cutted = true;
                }
            }
        }

        int splashCount = volume < maxSplashSize ? 1 : 3;
        splashCount = volume < minSplashSize ? UnityEngine.Random.Range(0, 2) : splashCount;

        SFXManager.Instance.PlaySfx(SFXName.Food, transform.position, foodColor, splashCount);
        knifePos = Vector3.zero;
        Knife.OnFoodExit(this);
    }

    public void OnKnifeEnter(Plane plane, Vector3 knifePos, Vector3 knifeDir)
    {
        this.knifePos = knifePos;
        this.knifeDir = knifeDir;
        cutPlane = plane;
        timer = 0;
    }
}

/// <summary>
/// 食物类型枚举
/// </summary>
public enum FoodType
{
    Vegetable,
    Lemon,
    Meat,
    Mushroom,
    Cucumber,
    Pepper
}
