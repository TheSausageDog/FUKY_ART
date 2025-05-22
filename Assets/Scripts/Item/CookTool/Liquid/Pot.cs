using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public WaterFlow waterFlow;

    public float pourSlope = 15;

    public void Update()
    {
        if (!waterFlow.isFlowing & Vector3.Angle(transform.forward, Vector3.down) < pourSlope)
        {
            waterFlow.isFlowing = true;
        }
        else if (waterFlow.isFlowing && Vector3.Angle(transform.forward, Vector3.down) > pourSlope)
        {
            waterFlow.isFlowing = false;
        }
    }
}
