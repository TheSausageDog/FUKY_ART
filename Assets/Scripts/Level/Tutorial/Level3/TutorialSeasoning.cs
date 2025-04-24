using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSeasoning : TutorialStep
{
    protected int foretasteIndex = 0;

    protected bool end = false;

    protected float endTime;

    public override bool TutorialUpdate()
    {
        if (end)
        {
            return Time.time - endTime < 2;
        }
        else
        {
            if (foretasteIndex != ForetasteManager.Instance.foretasteRecords.Count)
            {
                if (!ForetasteManager.Instance.foretasteRecords[foretasteIndex].TryGetValue(TasteType.Spicy, out float value))
                {
                    value = 0;
                }
                if (value < 2)
                {
                    dialogText = "似乎不够辣，得辣一些";
                }
                else if (value < 3)
                {
                    dialogText = "我想这应该就够了";
                    end = true;
                    endTime = Time.time;
                }
                else
                {
                    dialogText = "啊我操了好像多了";
                    end = true;
                    endTime = Time.time;
                }
                levelController.uiDialogText.text = dialogText;
                levelController.uiDialog.gameObject.SetActive(true);
                foretasteIndex++;
            }
        }
        return true;
    }
}
