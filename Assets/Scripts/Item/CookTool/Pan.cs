using System.Collections.Generic;
using UnityEngine;

public class Pan : MonoBehaviour
{
    protected bool heated = false;

    protected HashSet<Food> foods = new HashSet<Food>();

    void Update()
    {
        if (heated)
        {
            float heat = Time.deltaTime;
            foreach (var food in foods)
            {
                food.Heat(heat);
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