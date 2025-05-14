using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : PickupInteractableItem
{
    public WaterFlow waterFlow;

    protected bool isPour = false;

    protected float slope = 0;

    public override void OnInteract()
    {
        isPour = !isPour;
        if (!isPour)
        {
            waterFlow.isFlowing = false;
        }
    }

    public override void OnThrow()
    {
        base.OnThrow();
        isPour = false;
        waterFlow.isFlowing = false;
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
                transform.eulerAngles = new Vector3(Mathf.Lerp(0, 75, slope), 0, 0);
            }
            else
            {
                waterFlow.isFlowing = true;
            }
        }
        else
        {
            if (slope != 0)
            {
                slope -= Time.deltaTime;
                slope = Mathf.Max(slope, 0);
                transform.eulerAngles = new Vector3(Mathf.Lerp(0, 75, slope), 0, 0);
            }
        }
    }
}
