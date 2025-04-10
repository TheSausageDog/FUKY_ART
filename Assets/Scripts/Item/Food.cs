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
    [NonSerialized] public Vector3 in_knifePos; // 刀的位置
    [NonSerialized] public Vector3 in_edgeDir; // 刀的方向
    [NonSerialized] public BzKnife knife; // 刀的引用
    [NonSerialized] public float timer; // 计时器

    public bool cutted; // 是否已切割
    [HideInInspector] public float cuttingDepth => VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;

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
        if (other.TryGetComponent<BzKnife>(out var _knife))
        {
            if (knife == null)
            {
                Vector3 move_dir = Vector3.Normalize(transform.position - _knife.transform.position);
                if (Vector3.Dot(move_dir, _knife.EdgeDirection) > Mathf.Epsilon)
                {
                    // Debug.Log(Vector3.Dot(move_dir, _knife.EdgeDirection));
                    OnKnifeEnter(_knife);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BzKnife>(out var _knife))
        {
            OnKnifeExit(_knife);
        }
    }

    private void Update()
    {
        if (knife != null)
        {
            if (CheckCutted())
            {
                HandleSlice(cutPlane);
            }

            // timer += Time.deltaTime;
            // if (timer > 1 && knife != null)
            // {
            //     // Knife.OnFoodExit(this);
            // }
        }
    }

    protected bool CheckCutted()
    {
        Vector3 knife_move = knife.transform.position - in_knifePos;
        Vector3 knife_in_dist = Vector3.Project(knife_move, in_edgeDir);
        // float dot = Vector3.Dot(knife_in_dist, in_edgeDir);
        // Debug.Log(knife_in_dist.magnitude + " " + dot);
        return knife_in_dist.magnitude > cuttingDepth;
    }

    /// <summary>
    /// 切割处理逻辑
    /// </summary>
    async void HandleSlice(Plane plane)
    {
        var slicer = GetComponent<IBzMeshSlicer>();
        var sliceResults = await slicer.SliceAsync(plane);
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
        OnKnifeExit(knife);
    }

    public void OnKnifeEnter(BzKnife _knife)
    {
        _knife.OnEnterFood(this, ref cutPlane);
        in_knifePos = _knife.transform.position;
        in_edgeDir = _knife.EdgeDirection;
        // cutPlane = plane;
        timer = 0;
        // what if have multiable knife
        knife = _knife;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public void OnKnifeExit(BzKnife _knife)
    {
        _knife.OnExitFood(this);

        in_knifePos = Vector3.zero;
        in_edgeDir = Vector3.zero;
        knife = null;

        // what for?
        gameObject.layer = LayerMask.NameToLayer("Default");
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
