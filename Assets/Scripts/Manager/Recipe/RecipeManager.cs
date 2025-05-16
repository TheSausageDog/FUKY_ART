using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : SingletonMono<RecipeManager>
{
    private List<Recipe> recipes = new List<Recipe>();

    protected void Awake()
    {
        recipes = Resources.LoadAll<Recipe>("Data/Recipe").ToList();
    }

    public Recipe GetRecipe(string name)
    {
        return recipes.Find(r => r.name == name);
    }

    /// <summary>
    /// 评估配方是否符合要求，返回评级结果。
    /// </summary>
    /// Dictionary<FoodType, float> foodValues, 
    public RecipeRating EvaluateRecipe(Recipe recipe, Dictionary<TasteType, float> tasteTotals)
    {
        // return RecipeRating.差;

        //     Dictionary<FoodType, float> foodCount = foodValues;

        //     // 检查食材要求
        //     foreach (var requirement in ingredientRequirements)
        //     {
        //         if (!foodCount.TryGetValue(requirement.foodType, out float count) ||
        //             count < requirement.generalRange.x || count > requirement.generalRange.y)
        //         {
        //             return RecipeRating.差;
        //         }
        //     }

        //     // 检查是否需要切割
        //     foreach (var food in foods)
        //     {
        //         if (!food.cutted && ingredientRequirements.Find(req => food.foodType == req.foodType).needToCut)
        //         {
        //             return RecipeRating.差;
        //         }
        //     }

        //     // 检查味道要求
        foreach (var requirement in recipe.tasteRequirements)
        {
            if (!tasteTotals.TryGetValue(requirement.tasteType, out float value) ||
                value < requirement.generalRange.x || value > requirement.generalRange.y)
            {
                return RecipeRating.差;
            }
            else if (value < requirement.goodRange.x || value > requirement.goodRange.y)
            {
                return RecipeRating.一般;
            }
        }

        //     // 进一步评估优秀与一般
        //     bool isExcellent = true;

        //     foreach (var requirement in ingredientRequirements)
        //     {
        //         foodCount.TryGetValue(requirement.foodType, out float count);
        //         if (count < requirement.goodRange.x || count > requirement.goodRange.y)
        //         {
        //             isExcellent = false;
        //         }
        //     }

        //     foreach (var food in foods)
        //     {
        //         var singleGoodRange = ingredientRequirements.Find(req => food.foodType == req.foodType).singleGoodRange;
        //         if (food.volume < singleGoodRange.x || food.volume > singleGoodRange.y)
        //         {
        //             isExcellent = false;
        //         }
        //     }

        return RecipeRating.优秀;
    }
}
