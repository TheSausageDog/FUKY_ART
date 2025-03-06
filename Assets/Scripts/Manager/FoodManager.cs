using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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

    public async UniTask<List<Taste>> GetStandardValue(FoodType foodType)
    {
        var obj=foodValueStandard.Find(s =>  s.FoodType == foodType );

        await UniTask.WaitUntil(()=>obj.inited);
        
        return obj.GetAllTastes();
    }
    
}

public class AllTaste
{
    public List<Taste> Tastes;

    public AllTaste(List<Taste> tastes)
    {
        Tastes = tastes;
    }

    public float GetTasteValue(TasteType tasteType)
    {
        var target=Tastes.Find(t => t.tasteType == tasteType);
        if (target != default)
        {
            return -1;
        }
        return target.tasteValue;
    }
    
    
}

[Serializable]
public struct Taste : IEquatable<Taste>
{
    public TasteType tasteType;
    public float tasteValue;


    public static bool operator ==(Taste taste1,Taste taste2)
    {
        return taste1.Equals(taste2);
    }
    public static bool operator !=(Taste taste1,Taste taste2)
    {
        return !taste1.Equals(taste2);
    }

    
    public bool Equals(Taste other)
    {
        return tasteType == other.tasteType && tasteValue.Equals(other.tasteValue);
    }

    public override bool Equals(object obj)
    {
        return obj is Taste other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)tasteType, tasteValue);
    }
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