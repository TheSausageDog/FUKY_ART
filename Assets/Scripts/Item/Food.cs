using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using BzKovSoft.ObjectSlicer.Samples;
using UnityEngine;

public class Food : BasePickableItem
{
    public float FoodValue;
    
    public FoodType foodType;

    [HideInInspector]public float knifeDepth => VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, VolumeCalculator.CalculateWorldBounds(gameObject).size);
    }

    public override void Start()
    {
        base.Awake();

        FoodValue = VolumeCalculator.CalculateVolumes(gameObject) / FoodManager.Instance.GetStandardValue(foodType);

        //knifeDepth = VolumeCalculator.CalculateWorldBounds(gameObject).size.y * 0.5f;
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
    public Vector3 knifePos;
    public Vector3 knifeDir;
    public BzKnife Knife;
    public float timer;
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

    async void HandleSlice(Plane plane)
    {
        var tempSlicer = GetComponent<IBzMeshSlicer>();
        var results =await tempSlicer.SliceAsync(plane);
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (results != null && results.resultObjects!=null)
        {
            foreach (var resultObject in results.resultObjects)
            {
                resultObject.gameObject.layer = LayerMask.NameToLayer("Default");
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