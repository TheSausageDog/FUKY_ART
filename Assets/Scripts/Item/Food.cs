using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Food : BasePickableItem
{
    [NonSerialized] public AllTaste Tastes;

    public FoodType foodType;
    [NonSerialized]public float volume;

    public Color foodColor;
    [Tooltip("小于这个值就不发射粒子")]
    public float minSplashSize = 0.1f;
    [Tooltip("大于这个值发射三个，小于这个值发射一个")]
    public float maxSplashSize = 0.3f;

    private Plane cutPlane;
    [NonSerialized] public Vector3 knifePos;
    [NonSerialized] public Vector3 knifeDir;
    [NonSerialized] public BzKnife Knife;
    [NonSerialized] public float timer;
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

    public async void CalculateTaste()
    {
        var list = await FoodManager.Instance.GetStandardValueAsync(foodType);

        volume = await VolumeCalculator.CalculateVolumesAsync(gameObject);

        var tastes = new List<Taste>();

        foreach (var taste in list)
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
            Vector3 dis = Knife.transform.position - knifePos;
            var project = Vector3.Project(dis, knifeDir);
            float dot = Vector3.Dot(project, knifeDir);

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

    public bool cutted;

    async void HandleSlice(Plane plane)
    {
        var tempSlicer = GetComponent<IBzMeshSlicer>();
        var results = await tempSlicer.SliceAsync(plane);
        gameObject.layer = LayerMask.NameToLayer("Default");
        cutted = true;
        if (results != null && results.resultObjects != null)
        {
            foreach (var resultObject in results.resultObjects)
            {
                resultObject.gameObject.layer = LayerMask.NameToLayer("Default");
                if (resultObject.gameObject.TryGetComponent(out Food food))
                {
                    food.cutted = true;
                }
            }
        }

        var count = volume < maxSplashSize ? 1 : 3;
        count = volume < minSplashSize ? UnityEngine.Random.Range(0, 2) : 1;

        SFXManager.Instance.PlaySfx(SFXName.Food, transform.position,foodColor,count);
        
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

public enum FoodType
{
    Vegetable,
    Lemon,
    Meat,
    Mushroom,
    Cucumber,
    Pepper
}