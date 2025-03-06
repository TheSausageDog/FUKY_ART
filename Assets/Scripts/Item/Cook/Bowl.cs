using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Bowl : MonoBehaviour
{
    public Transform checkCenter;
    public float checkRadius;
    public string testRecipeName;
    
    public Text text;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
     {
         CheckFood();
         text.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
     }

     Collider[] results = new Collider[128];
     
     private void CheckFood()
     {
         var size = Physics.OverlapSphereNonAlloc(checkCenter.position, checkRadius, results);
         var list = new List<Taste>();
         var foodVolumeDic = new Dictionary<FoodType,float>();
         string info = " ";
         info+="菜品: " + testRecipeName + "\n";
         for (int i = 0; i < size; i++)
         {
             if (results[i].CompareTag("canPickUp"))
             {
                 if (results[i].TryGetComponent<Food>(out Food food))
                 {
                     food.Tastes.Tastes.ForEach(taste =>
                     {
                         if (list.FindIndex(t => t.tasteType == taste.tasteType) != -1)
                         {
                             var index = list.FindIndex(t => t.tasteType == taste.tasteType);
                             var t = list[index];
                             t.tasteValue += taste.tasteValue;
                             list[index] = t;
                         }
                         else
                         {
                             list.Add(taste);
                         }
                     });
                     if (foodVolumeDic.ContainsKey(food.foodType))
                     {
                         foodVolumeDic[food.foodType] += food.volume;
                     }
                     else foodVolumeDic.Add(food.foodType, food.volume);
                     
                 }
             }
         }

         foreach (var kvp in foodVolumeDic)
         {
             info += $"包含食物类型：{kvp.Key}\n";
             info += $"体积为：{kvp.Value:F}\n";
         }

         foreach (var taste in list)
         {
             info += $"当前食物味道：{taste.tasteType}\n";
             info += $"值为：{taste.tasteValue:F}\n";
         }

         var rating=RecipeManager.Instance.GetRecipe(testRecipeName).EvaluateRecipe(foodVolumeDic, list);

         info += $"评分：{rating}\n";
         
         text.text = info;
     }

     private void OnDrawGizmos()
     {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkCenter.position, checkRadius);
     }
}
