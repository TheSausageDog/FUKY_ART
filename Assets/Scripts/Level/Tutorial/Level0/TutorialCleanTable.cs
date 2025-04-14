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
    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        containRecorder = levelController.areaTrigger.Find("TableSurfaceArea").GetComponent<ContainRecorder>();
    }

    public override bool TutorialUpdate()
    {
        foreach (var item in rubbish)
        {
            if (containRecorder.IsContain(item))
            {
                return true;
            }
        }
        return false;
    }
}
