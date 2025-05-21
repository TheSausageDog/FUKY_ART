using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Bowl : InteractItemBase
{
    public FoodRecorder checkArea;

    public override void OnInteract()
    {
        string info = "";
        var tasteList = TasteManager.Instance.Foretaste(checkArea.foods);

        foreach (var taste in tasteList)
        {
            info += $"当前食物味道：{taste.Key}\n";
            info += $"值为：{taste.Value:F}\n";
        }
    }
}
