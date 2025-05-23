using System.Collections.Generic;
using UnityEngine;

public class TasteManager : SingletonMono<TasteManager>
{
    public List<Dictionary<TasteType, float>> foretasteRecords = new List<Dictionary<TasteType, float>>();

    public Vector3 spicyValue = new Vector3(0, 5, 10);

    public Dictionary<TasteType, float> Foretaste(List<Food> foods)
    {
        TasteReport report = GetTaste(foods);
        foretasteRecords.Add(report.allTast);
        return report.allTast;
    }

    protected Vector3 GetTaggingValue(TasteType tasteType)
    {
        switch (tasteType)
        {
            case TasteType.Spicy:
                return spicyValue;
        }
        return Vector3.zero;
    }

    protected TasteTag GetTasteTag(TasteType tasteType, float tasteValue)
    {
        Vector3 taggingValue = GetTaggingValue(tasteType);
        if (tasteValue <= taggingValue.x)
        {
            return TasteTag.None;
        }
        else if (tasteValue <= taggingValue.y)
        {
            return TasteTag.Little;
        }
        else if (tasteValue <= taggingValue.z)
        {
            return TasteTag.Normal;
        }
        else
        {
            return TasteTag.Very;
        }
    }

    public TasteReport GetTaste(TasteCollector foods)
    {
        return GetTaste(foods.checkArea.foods);
    }

    public TasteReport GetTaste(List<Food> foods)
    {
        TasteReport tasteReport = new TasteReport();
        tasteReport.allTast = new Dictionary<TasteType, float>();
        tasteReport.doneness = 0;
        // var foodVolumeDic = new Dictionary<FoodType, float>();
        tasteReport.donenessTag = DonenessTag.Raw;

        foreach (var food in foods)
        {
            foreach (var taste in food.tastes)
            {
                if (tasteReport.allTast.ContainsKey(taste.Key))
                {
                    tasteReport.allTast[taste.Key] += taste.Value;
                }
                else
                {
                    tasteReport.allTast[taste.Key] = taste.Value;
                }
            }
            tasteReport.doneness += food.GetDoneness();
            DonenessTag tag = food.GetDonenessTag();
            if (tag == DonenessTag.Bruned)
            {
                tasteReport.donenessTag = tag;
            }
        }

        tasteReport.doneness /= foods.Count;
        if (tasteReport.donenessTag != DonenessTag.Bruned)
        {
            tasteReport.donenessTag = GetDonenessTag(tasteReport.doneness);
        }

        tasteReport.tasteTags = new Dictionary<TasteType, TasteTag>();
        foreach (var taste in tasteReport.allTast.Keys)
        {

            tasteReport.tasteTags[taste] = GetTasteTag(taste, tasteReport.allTast[taste]);
        }



        return tasteReport;
    }


    ///
    ///  raw       cooked    burned
    /// 0--2.5----7.5--10------15
    ///
    public static DonenessTag GetDonenessTag(float doneness)
    {
        if (doneness < 2.5)
        {
            return DonenessTag.Raw;
        }
        else if (doneness < 7.5)
        {
            return DonenessTag.HalfDone;
        }
        else if (doneness < 10)
        {
            return DonenessTag.Cooked;
        }
        else if (doneness < 15)
        {
            return DonenessTag.HalfBruned;
        }
        else
        {
            return DonenessTag.Bruned;
        }
    }

    public static string DonenessToString(DonenessTag doneness)
    {
        switch (doneness)
        {
            case DonenessTag.Raw:
                return "生";
            case DonenessTag.HalfDone:
                return "半生";
            case DonenessTag.Cooked:
                return "熟";
            case DonenessTag.HalfBruned:
                return "老";
            case DonenessTag.Bruned:
                return "烤焦";
        }
        return "";
    }

    public static string TasteToString(TasteType tasteType)
    {
        switch (tasteType)
        {
            case TasteType.Spicy:
                return "辣";
        }
        return "";
    }

    public static string TasteTagToString(TasteTag tasteType)
    {
        switch (tasteType)
        {
            case TasteTag.None:
                return "不";
            case TasteTag.Little:
                return "微";
            case TasteTag.Normal:
                return "";
            case TasteTag.Very:
                return "很";
        }
        return "";
    }
}


public struct TasteReport
{
    public Dictionary<TasteType, float> allTast;
    public Dictionary<TasteType, TasteTag> tasteTags;
    public float doneness;
    public DonenessTag donenessTag;

    public override string ToString()
    {
        string info = "";
        // $"菜品: {testRecipeName}\n";

        // foreach (var kvp in foodVolumeDic)
        // {
        //     info += $"包含食物类型：{kvp.Key}\n";
        //     info += $"体积为：{Mathf.Max(0.01f, kvp.Value):F}\n";
        // }
        foreach (var taste in allTast)
        {
            info += $"当前食物味道：{TasteManager.TasteToString(taste.Key)}\n";
            info += $"值为：{taste.Value:F}\n";
            info += $"评价为：{TasteManager.TasteTagToString(tasteTags[taste.Key]) + TasteManager.TasteToString(taste.Key)}\n";
        }

        info += $"\n\n煮熟程度为：{TasteManager.DonenessToString(donenessTag)}\n";

        return info;
        // var rating = RecipeManager.Instance.GetRecipe(testRecipeName).EvaluateRecipe(foodVolumeDic, tasteList, foodList);
        // info += $"评分：{rating}\n";

        // text.text = info;
    }
}

public enum DonenessTag
{
    Raw,
    HalfDone,
    Cooked,
    HalfBruned,
    Bruned
}

public enum TasteTag
{
    None,
    Little,
    Normal,
    Very
}