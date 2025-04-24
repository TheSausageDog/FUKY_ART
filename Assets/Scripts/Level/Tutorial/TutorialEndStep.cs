using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndStep : TutorialStepBase
{
    public string nextLevel;

    // Start is called before the first frame update
    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        levelController.LoadNewLevel(nextLevel);
    }
}
