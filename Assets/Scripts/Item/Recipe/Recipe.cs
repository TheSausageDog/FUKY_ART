using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe/RecipeDefinition")]
public class Recipe : ScriptableObject
{
    [Header("食材要求")]
    [Tooltip("所需食材及其允许的数量范围。")]
    public List<IngredientRequirement> ingredientRequirements;
    
    [Header("味道要求")]
    [Tooltip("所需味道及其允许的强度范围。")]
    public List<TasteRequirement> tasteRequirements;
    
    public RecipeRating EvaluateRecipe(Dictionary<FoodType,float> foodValues,List<Taste> allTaste,List<Food> foods)
    {
        Dictionary<FoodType, float> foodCount = foodValues;
        Dictionary<TasteType, float> tasteTotals = new Dictionary<TasteType, float>();
        

        foreach (var taste in allTaste)
        {
            if (tasteTotals.ContainsKey(taste.tasteType))
                tasteTotals[taste.tasteType] += taste.tasteValue;
            else
                tasteTotals[taste.tasteType] = taste.tasteValue;
        }

  
        
        // 检查食材要求
        foreach (var requirement in ingredientRequirements)
        {
            if (!foodCount.TryGetValue(requirement.foodType, out float count) || count > requirement.generalRange.y || count < requirement.generalRange.x)
            {
                return RecipeRating.差;
            }
        }
        
        foreach (var food in foods)
        {
            if (!food.cutted &&ingredientRequirements.Find(requirement => (food.foodType == requirement.foodType)).needToCut) return RecipeRating.差;
        }
        
        // 检查味道要求
        foreach (var requirement in tasteRequirements)
        {
            if (!tasteTotals.TryGetValue(requirement.tasteType, out float value) || value > requirement.generalRange.y || value < requirement.generalRange.x)
            {
                return RecipeRating.差;
            }
        }
        
        // 进一步评估一般和优秀
        bool isExcellent = true;
        foreach (var requirement in ingredientRequirements)
        {
            foodCount.TryGetValue(requirement.foodType, out float count);
            if (count < requirement.goodRange.x || count > requirement.goodRange.y)
            {
                isExcellent = false;
            }
        }
        
        foreach (var requirement in tasteRequirements)
        {
            tasteTotals.TryGetValue(requirement.tasteType, out float value);
            if (value < requirement.goodRange.x || value > requirement.goodRange.y)
            {
                isExcellent = false;
            }
        }
        
        return isExcellent ? RecipeRating.优秀 : RecipeRating.一般;
    }
}

[Serializable]
public struct IngredientRequirement
{
    [Tooltip("所需的食材类型。")]
    public FoodType foodType;
    
    [Tooltip("该食材的可接受数量范围（最小值，最大值）。")]
    public Vector2 generalRange;
    
    [Tooltip("该食材的最佳数量范围（最小值，最大值），用于获得优秀评级。")]
    public Vector2 goodRange;

    [Tooltip("是否需要被切")]
    public bool needToCut;
}

[Serializable]
public struct TasteRequirement
{
    [Tooltip("所需的味道类型。")]
    public TasteType tasteType;
    
    [Tooltip("该味道的可接受强度范围（最小值，最大值）。")]
    public Vector2 generalRange;
    
    [Tooltip("该味道的最佳强度范围（最小值，最大值），用于获得优秀评级。")]
    public Vector2 goodRange;
}

public enum RecipeRating
{
    [Tooltip("配方不符合最低标准。")]
    差,
    
    [Tooltip("配方符合一般标准，但不是最佳状态。")]
    一般,
    
    [Tooltip("配方平衡良好，达到最佳标准。")]
    优秀
}