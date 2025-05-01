using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : ActivatableItem
{
    public override void Active()
    {
        Debug.Log("点火");
    }

    public override void Deactive()
    {
        Debug.Log("关火");
    }
}
