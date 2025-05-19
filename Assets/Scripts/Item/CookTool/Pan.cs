using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    public LiquidContainer oilNSauce;

    protected bool heated = false;

    protected bool hasOil = false;

    protected HashSet<Food> foods = new HashSet<Food>();

    void Update()
    {
        if (oilNSauce.volume != 0)
        {
            hasOil = true;
        }
        if (heated)
        {
            float heat = Time.deltaTime;
            if (hasOil)
            {
                foreach (var food in foods)
                {
                    food.Heat(heat);
                }
            }
            else
            {
                foreach (var food in foods)
                {
                    food.DryHeat(heat);
                }
            }
        }
    }

    public void Heat()
    {
        heated = true;

    }

    public void Unheat()
    {
        heated = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Food>(out var food))
        {
            foods.Add(food);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Food>(out var food) && foods.Contains(food))
        {
            foods.Remove(food);
        }
    }
}