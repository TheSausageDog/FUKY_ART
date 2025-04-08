using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialCleanTable : TutorialStep
{
    public List<Collider> rubbish = new List<Collider>();

    protected ContainRecorder containRecorder;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
    }

    void Update()
    {
        foreach (var item in rubbish)
        {
            if(containRecorder.IsContain(item)){
                return;
            }
        }
        EndStep();
    }
}
