using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;
using UnityEngine.UI;

public class ItemConsole : MonoListener
{
    public Text consoleText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            consoleText.enabled = !consoleText.enabled;
        }
    }

    [Listen(EventType.OnItemPicked)]
    private void OnPickUpItem(BasePickableItem item)
    {
        string info = "";
        info += $"当前物品的名字：{item.name}\n";
        
        if (item is Food food)
        {
            info += $"当前食物的种类：{food.foodType}\n";
            info += $"当前食物的体积：{Mathf.Max(0.01f,food.volume):F}\n";

            foreach (var taste in food.Tastes.Tastes)
            {
                info += $"当前食物味道：{taste.tasteType}\n";
                info += $"值为：{Mathf.Max(0.01f,taste.tasteValue):F}\n";
            }
        }
        
        consoleText.text = info;
    } 
    [Listen(EventType.OnItemDrop)]
    private void OnPickUpItem()
    {
        consoleText.text = " ";
    }
    
}
