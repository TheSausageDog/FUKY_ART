using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Bowl : BasePickableItem
{
    public Transform checkCenter;
    public float checkRadius;
    public string testRecipeName;
    
    public Text text;
    private Camera cam;

    private float pepperAmount = 0;
    
    public override void Awake()
    {
        base.Awake();
        text.enabled = false;
        cam = Camera.main;
    }

    private void FixedUpdate()
     {
         CheckFood();
         text.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
     }

    public override void Interact(InteractionType type, params object[] args)
    {
        base.Interact(type, args);
        if (type == InteractionType.Interact)
        {
            text.enabled=!text.enabled;
        }
    }


    Collider[] results = new Collider[128];
     
     HashSet<GameObject> pepperSet = new HashSet<GameObject>();
     private void CheckFood()
     {
         var size = Physics.OverlapSphereNonAlloc(checkCenter.position, checkRadius, results);

         if (size == 0)
         {
             pepperAmount = 0;
             return;
         }
         
         var list = new List<Taste>();
         var foodVolumeDic = new Dictionary<FoodType,float>();
         string info = " ";
         info+="菜品: " + testRecipeName + "\n";
         List<Food> foodList = new List<Food>();
         for (int i = 0; i < size; i++)
         {
             if (results[i].CompareTag("canInteract"))
             {
                 if (results[i].TryGetComponent<Food>(out Food food))
                 {
                     foodList.Add(food);
                     
                     if(food.Tastes==null)continue;
                     
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
             else if (results[i].CompareTag("PepperParticle"))
             {
                 if(pepperSet.Contains(results[i].gameObject))continue;
                 pepperSet.Add(results[i].gameObject);
                 pepperAmount += 0.1f;
             }
         }
         if (foodList.Count == 0)
         {
             pepperAmount = 0;
         }
         if (pepperAmount > 0)
         {
             FoodManager.Instance.GetStandardValue(FoodType.Pepper).ForEach(taste =>
             {
                 if (list.FindIndex(t => t.tasteType == taste.tasteType) != -1)
                 {
                     var index = list.FindIndex(t => t.tasteType == taste.tasteType);
                     var t = list[index];
                     t.tasteValue += taste.tasteValue * pepperAmount;
                     list[index] = t;
                 }
                 else
                 {
                     list.Add(taste);
                 }
             });

         }

         foreach (var kvp in foodVolumeDic)
         {
             info += $"包含食物类型：{kvp.Key}\n";
             info += $"体积为：{Mathf.Max(0.01f,kvp.Value):F}\n";
         }

         foreach (var taste in list)
         {
             info += $"当前食物味道：{taste.tasteType}\n";
             info += $"值为：{Mathf.Max(0.01f,taste.tasteValue):F}\n";
         }
//         Debug.Log(RecipeManager.Instance);
        // Debug.Log(testRecipeName);

         var rating=RecipeManager.Instance.GetRecipe(testRecipeName).EvaluateRecipe(foodVolumeDic, list,foodList);
         

         info += $"评分：{rating}\n";
         
         text.text = info;
         

     }

     private void OnDrawGizmos()
     {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(checkCenter.position, checkRadius);
     }
}
