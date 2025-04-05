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
public class Bowl : BasePickableItem
{
    public Transform checkCenter; // 检查中心点
    public float checkRadius; // 检查半径
    public string testRecipeName; // 测试配方名称
    public Text text; // 显示评分信息的文本

    public Material liquid_material;

    protected float _threshold_start = 0.7f;

    protected float _threshold_end = 0.25f;

    public float liquid_max = 5f;



    private Camera cam; // 主摄像机
    private float pepperAmount = 0; // 胡椒量
    private Collider[] results = new Collider[128]; // 检测到的碰撞体数组
    private HashSet<GameObject> pepperSet = new HashSet<GameObject>(); // 胡椒对象集合

    private Color liquid_color;

    private float liquid_volume;

    public override void Awake()
    {
        base.Awake();
        text.enabled = false;
        cam = Camera.main;
    }

    public override void Start()
    {
        base.Start();

        AddLiquid(Color.black, 0);
    }

    private void FixedUpdate()
    {
        CheckFood();
        text.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
    }

    public override void Interact(InteractionType type, params object[] args)
    {
        base.Interact(type, args);
        if (type == InteractionType.Interact)
        {
            text.enabled = !text.enabled;
        }
    }

    public void AddLiquid(Color _liquid_color, float _liquid_volume){
        
        if(liquid_volume == 0){
            liquid_color = _liquid_color;
            liquid_volume = _liquid_volume;
        }else{
            liquid_volume += _liquid_volume;
            float alpha = _liquid_volume / liquid_volume;
            liquid_color = (1- alpha) * liquid_color + alpha * _liquid_color;
        }

        liquid_color.a = 1;
        
        float threshold = _threshold_start - (_threshold_start - _threshold_end) * Mathf.Pow(Mathf.Min(liquid_volume / liquid_max, 1f), 1f/2);

        liquid_material.SetColor("_surface_color", liquid_color);
        liquid_material.SetFloat("_threshold", threshold);
    }

    /// <summary>
    /// 检查碗中的食物和味道成分
    /// </summary>
    private void CheckFood()
    {
        int size = Physics.OverlapSphereNonAlloc(checkCenter.position, checkRadius, results);
        if (size == 0)
        {
            pepperAmount = 0;
            return;
        }

        var tasteList = new List<Taste>();
        var foodVolumeDic = new Dictionary<FoodType, float>();
        var foodList = new List<Food>();
        string info = $"菜品: {testRecipeName}\n";

        for (int i = 0; i < size; i++)
        {
            if (results[i].CompareTag("canInteract") && results[i].TryGetComponent(out Food food))
            {
                foodList.Add(food);
                if (food.Tastes == null) continue;

                foreach (var taste in food.Tastes.Tastes)
                {
                    int index = tasteList.FindIndex(t => t.tasteType == taste.tasteType);
                    if (index != -1)
                    {
                        var temp = tasteList[index];
                        temp.tasteValue += taste.tasteValue * pepperAmount;
                        tasteList[index] = temp;
                    }
                    else
                    {
                        tasteList.Add(taste);
                    }
                }

                if (foodVolumeDic.ContainsKey(food.foodType))
                {
                    foodVolumeDic[food.foodType] += food.volume;
                }
                else
                {
                    foodVolumeDic.Add(food.foodType, food.volume);
                }
            }
            else if (results[i].CompareTag("PepperParticle"))
            {
                if (pepperSet.Add(results[i].gameObject))
                {
                    pepperAmount += 0.1f;
                }
            }
        }

        if (pepperAmount > 0)
        {
            foreach (var taste in FoodManager.Instance.GetStandardValue(FoodType.Pepper))
            {
                int index = tasteList.FindIndex(t => t.tasteType == taste.tasteType);
                if (index != -1)
                {
                    var temp = tasteList[index];
                    temp.tasteValue += taste.tasteValue * pepperAmount;
                    tasteList[index] = temp;
                }
                else
                {
                    tasteList.Add(taste);
                }
            }
        }

        foreach (var kvp in foodVolumeDic)
        {
            info += $"包含食物类型：{kvp.Key}\n";
            info += $"体积为：{Mathf.Max(0.01f, kvp.Value):F}\n";
        }

        foreach (var taste in tasteList)
        {
            info += $"当前食物味道：{taste.tasteType}\n";
            info += $"值为：{Mathf.Max(0.01f, taste.tasteValue):F}\n";
        }

        var rating = RecipeManager.Instance.GetRecipe(testRecipeName).EvaluateRecipe(foodVolumeDic, tasteList, foodList);
        info += $"评分：{rating}\n";

        text.text = info;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkCenter.position, checkRadius);
    }
}
