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
    [NonSerialized]public AllTaste Tastes;
    
    public FoodType foodType;
    public float volume;
    [HideInInspector]public float knifeDepth => VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;

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
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        var list = FoodManager.Instance.GetStandardValue(foodType);
        
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

    private Plane cutPlane;
    [NonSerialized]public Vector3 knifePos;
    [NonSerialized]public Vector3 knifeDir;
    [NonSerialized]public BzKnife Knife;
    [NonSerialized]public float timer;
    private void Update()
    {
        if (knifePos != Vector3.zero)
        {
            Vector3 dis = Knife.transform.position - knifePos;
            var project = Vector3.Project(dis, knifeDir);
//            Debug.Log(project);
            if (project.magnitude > knifeDepth)
            {
                HandleSlice(cutPlane);
            }
            
            timer += Time.deltaTime;
            if (timer > 1 && Knife!=null)
            {
                Knife.OnFoodExit(this);
            }
        }
        

    }

    public bool cutted;
    async void HandleSlice(Plane plane)
    {
        var tempSlicer = GetComponent<IBzMeshSlicer>();
        var results =await tempSlicer.SliceAsync(plane);
        gameObject.layer = LayerMask.NameToLayer("Default");
        cutted = true;
        if (results != null && results.resultObjects!=null)
        {
            foreach (var resultObject in results.resultObjects)
            {
                resultObject.gameObject.layer = LayerMask.NameToLayer("Default");
                if(resultObject.gameObject.TryGetComponent(out Food food))
                {
                    food.cutted = true;
                }
            }
            
        }
        knifePos = Vector3.zero;
        Knife.OnFoodExit(this);
    }
    
    public void OnKnifeEnter(Plane plane,Vector3 knifePos,Vector3 knifeDir)
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
    Cucumber
}