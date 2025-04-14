using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStepTest : TutorialStep
{
    public override void TutorialStart(LevelController _levelController)
    {
        dialogText = "Nothing here.施工中";
        base.TutorialStart(_levelController);
    }
}
