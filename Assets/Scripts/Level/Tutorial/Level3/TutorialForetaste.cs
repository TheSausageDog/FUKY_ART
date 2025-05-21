using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialForetaste : TutorialStep
{

    public override bool TutorialUpdate()
    {
        return TasteManager.Instance.foretasteRecords.Count == 0;
    }
}
