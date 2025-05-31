using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourableItem : PickupInteractableItem
{
    public float endSlope = 75;

    protected bool isPour = false;

    protected float slope = 0;

    public override void OnInteract()
    {
        isPour = !isPour;
    }

    public override void OnThrow()
    {
        base.OnThrow();
        isPour = false;
    }

    public override void Update()
    {
        base.Update();

        if (isPour)
        {
            if (slope != 1)
            {
                slope += Time.deltaTime;
                slope = Mathf.Min(slope, 1);
            }
        }
        else
        {
            if (slope != 0)
            {
                slope -= Time.deltaTime;
                slope = Mathf.Max(slope, 0);
            }
        }
        rotateOffset = new Vector3(Mathf.Lerp(0, endSlope, slope), 0, 0);
    }
}
