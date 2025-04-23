using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配方定义，包含食材和味道要求。
/// </summary>
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe/RecipeDefinition")]
public class Recipe : ScriptableObject
{
    [Header("食材要求")]
    [Tooltip("所需食材及其允许的数量范围。")]
    public List<IngredientRequirement> ingredientRequirements;

    [Header("味道要求")]
    [Tooltip("所需味道及其允许的强度范围。")]
    public List<TasteRequirement> tasteRequirements;
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

    [Tooltip("该食材的每个最佳数量范围（最小值，最大值），用于获得优秀评级。")]
    public Vector2 singleGoodRange;

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
