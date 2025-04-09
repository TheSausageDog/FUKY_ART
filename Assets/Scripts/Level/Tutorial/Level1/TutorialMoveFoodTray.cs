using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TutorialMoveFoodTray : TutorialStep
{
    public GameObject foodTray;

    protected BasePickableItem foodTrayPickable;
    protected Collider foodTrayCollider;
    protected ContainRecorder containRecorder;

    public override void Start()
    {
        base.Start();
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
        foodTrayPickable = foodTray.GetComponent<BasePickableItem>();
        foodTrayCollider = foodTray.GetComponent<Collider>();
    }

    void Update()
    {
        if (containRecorder.IsContain(foodTrayCollider) && !foodTrayPickable.isPicking)
        {
            EndStep();
        }
    }

    public override void EndStep()
    {
        base.EndStep();
    }
}
