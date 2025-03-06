using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Standard : MonoBehaviour
{
    public FoodType FoodType;
    [SerializeField]private List<Taste> Tastes;
    private List<Taste> res;

    public async void Awake()
    {
        res = await InitAllTastes();
    }

    public List<Taste> GetAllTastes()
    {
        return res;
    }
    private async UniTask<List<Taste>> InitAllTastes()
    {
        var volume= await VolumeCalculator.CalculateVolumesAsync(gameObject);

        List<Taste> list = new();

        foreach (var taste in Tastes)
        {
            var newTaste = taste;
            newTaste.tasteValue /= volume;
            list.Add(newTaste);
        }
        
        return list;
        
    }
}
