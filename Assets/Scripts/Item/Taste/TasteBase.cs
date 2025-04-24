using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TasteObject : MonoBehaviour
{
    [Serializable]
    protected struct StartTaste
    {
        public TasteType taste;
        public float value;
    }

    [SerializeField]
    protected StartTaste[] startTastes;

    public Dictionary<TasteType, float> tastes = new Dictionary<TasteType, float>();

    public virtual void Start()
    {
        foreach (var item in startTastes)
        {
            tastes[item.taste] = item.value;
        }
        startTastes = null;
    }
}



public enum FoodType
{
    None,
    Vegetable,
    Lemon,
    Meat,
    Mushroom,
    Cucumber,
    Pepper,
    Sausage
}

public enum TasteType
{
    Sour,
    Sweet,
    Bitter,
    Salty,
    Fresh,
    Spicy,
    Pepper
}