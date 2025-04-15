using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
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
        foreach (var item in rubbish)
        {
            item.transform.AddComponent<HighLightedItem>().isHighlighted = true;
        }
    }

    public override bool TutorialUpdate()
    {
        bool hasRubbish = false;
        foreach (var item in rubbish)
        {
            HighLightedItem highLight = item.transform.GetComponent<HighLightedItem>();
            if (containRecorder.IsContain(item))
            {
                highLight.isHighlighted = true;
                hasRubbish = true;
            }else{
                highLight.isHighlighted = false;
                
            }
        }

        return hasRubbish;
    }

    public override void TutorialEnd()
    {
        base.TutorialEnd();
        // foreach (var item in rubbish)
        // {
        //     Destroy(item.transform.GetComponent<HighLightedItem>());
        // }
    }
}
