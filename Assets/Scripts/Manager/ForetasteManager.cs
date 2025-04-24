using System.Collections.Generic;

public class ForetasteManager : SingletonMono<ForetasteManager>
{
    public List<Dictionary<TasteType, float>> foretasteRecords = new List<Dictionary<TasteType, float>>();

    public Dictionary<TasteType, float> Foretaste(List<Food> foods)
    {
        var tasteList = new Dictionary<TasteType, float>();
        // var foodVolumeDic = new Dictionary<FoodType, float>();


        foreach (var food in foods)
        {
            foreach (var taste in food.tastes)
            {
                if (tasteList.ContainsKey(taste.Key))
                {
                    tasteList[taste.Key] += taste.Value;
                }
                else
                {
                    tasteList[taste.Key] = taste.Value;
                }
            }

            // if (foodVolumeDic.ContainsKey(food.foodType))
            // {
            //     foodVolumeDic[food.foodType] += food.volume;
            // }
            // else
            // {
            //     foodVolumeDic.Add(food.foodType, food.volume);
            // }
        }

        // if (pepperAmount > 0)
        // {
        //     foreach (var taste in FoodManager.Instance.GetStandardValue(FoodType.Pepper))
        //     {
        //         int index = tasteList.FindIndex(t => t.tasteType == taste.tasteType);
        //         if (index != -1)
        //         {
        //             var temp = tasteList[index];
        //             temp.tasteValue += taste.tasteValue * pepperAmount;
        //             tasteList[index] = temp;
        //         }
        //         else
        //         {
        //             tasteList.Add(taste);
        //         }
        //     }
        // }
        foretasteRecords.Add(tasteList);
        return tasteList;
    }
}