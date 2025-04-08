using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndStep : TutorialStepBase
{
    public string nextLevel;

    // Start is called before the first frame update
    void Start()
    {
        levelController.LoadNewLevel(nextLevel);
    }

}
