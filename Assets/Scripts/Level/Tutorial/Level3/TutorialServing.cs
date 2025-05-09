using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialServing : TutorialStep
{
    public override bool TutorialUpdate()
    {
        return OrderManager.Instance.orderItem != null;
    }
}