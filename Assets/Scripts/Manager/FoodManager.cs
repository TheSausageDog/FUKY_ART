using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodManager : SingletonMono<FoodManager>
{
    private List<Standard> foodValueStandard = new List<Standard>();

    protected override void Awake()
    {
        base.Awake();
        
        GetAllStandard();
    }

    private void GetAllStandard()
    {
        foodValueStandard=FindObjectsOfType<Standard>().ToList();
    }

    public List<Taste> GetStandardValue(FoodType foodType)
    {
        var obj=foodValueStandard.Find(s =>  s.FoodType == foodType );

        return obj.GetAllTastes();
    }
    
}

[Serializable]
public struct Taste
{
    public TasteType tasteType;
    public float tasteValue;
}
public enum TasteType
{
    Sour,
    Sweet,
    Bitter,
    Spicy,
    Salty,
    Fresh
}