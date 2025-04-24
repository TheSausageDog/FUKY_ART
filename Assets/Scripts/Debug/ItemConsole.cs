using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using UnityEngine;
using UnityEngine.UI;

public class ItemConsole : MonoListener
{
    public string consoleText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log(consoleText);
        }
    }

    [Listen(EventType.OnItemHeld)]
    private void OnPickUpItem()
    {
        BaseItem item = PlayerBlackBoard.heldItem;
        string info = "";
        info += $"当前物品的名字：{item.name}\n";

        if (item.TryGetComponent<Food>(out var food))
        {
            info += $"当前食物的种类：{food.foodType}\n";
            // info += $"当前食物的体积：{Mathf.Max(0.01f, food.volume):F}\n";

            foreach (var taste in food.tastes)
            {
                info += $"当前食物味道：{taste.Key}\n";
                info += $"值为：{Mathf.Max(0.01f, taste.Value):F}\n";
            }

            // info += food.cutted ? "当前食物被切过了" : "当前食物未被切过\n";
        }



        consoleText = info;
    }
    [Listen(EventType.OnItemDrop)]
    private void OnItemDrop()
    {
        // consoleText.text = " ";
    }

}
