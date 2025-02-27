using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : SingletonMono<FoodManager>
{
    public List<FoodStandard> foodValueStandard = new List<FoodStandard>();


    public float GetStandardValue(FoodType foodType)
    {
        var obj=foodValueStandard.Find(s =>  s.foodType == foodType );

        return VolumeCalculator.CalculateVolumes(obj.standard);
    }

    public void Start()
    {
        Debug.Log(GetStandardValue(FoodType.Meat));
    }
}
[Serializable]
public struct FoodStandard
{
    public FoodType foodType;
    public GameObject standard;
}