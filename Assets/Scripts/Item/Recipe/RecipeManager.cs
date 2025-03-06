using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : SingletonMono<RecipeManager>
{
    private List<Recipe> recipes = new List<Recipe>();

    protected override void Awake()
    {
        base.Awake();
        
        recipes = Resources.LoadAll<Recipe>("Data/Recipe").ToList();
    }

    public Recipe GetRecipe(string name)
    {
        return recipes.Find(r => r.name == name);
    }
    
}
