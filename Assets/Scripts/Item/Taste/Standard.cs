using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Standard : MonoBehaviour
{
    public FoodType FoodType;
    [SerializeField]private List<Taste> Tastes;

    public List<Taste> GetAllTastes()
    {
        var volume=VolumeCalculator.CalculateVolumes(gameObject);

        List<Taste> list = new();

        foreach (var taste in Tastes)
        {
            var newTaste = taste;
            newTaste.tasteValue *= volume;
            list.Add(newTaste);
        }
        
        return list;
        
    }
}
