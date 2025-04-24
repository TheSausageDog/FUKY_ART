using System.Collections.Generic;
using UnityEngine;

public class FoodRecorder : ContainRecorder
{
    public List<Food> foods = new List<Food>();

    public List<MainIngredient> mainIngredient = new List<MainIngredient>();

    public List<Spice> spices = new List<Spice>();

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.TryGetComponent<Food>(out var food))
        {
            foods.Add(food);
            if (food.GetType() == typeof(MainIngredient))
            {
                mainIngredient.Add((MainIngredient)food);
            }
            else if (food.GetType() == typeof(Spice))
            {
                spices.Add((Spice)food);
            }
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.TryGetComponent<Food>(out var food))
        {
            foods.Remove(food);
            if (food.GetType() == typeof(MainIngredient))
            {
                mainIngredient.Remove((MainIngredient)food);
            }
            else if (food.GetType() == typeof(Spice))
            {
                spices.Remove((Spice)food);
            }
        }
    }
}