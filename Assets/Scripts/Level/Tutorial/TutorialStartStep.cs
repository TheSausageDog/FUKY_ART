using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStartStep : TutorialStepBase
{
    public List<GameObject> stayObject = new List<GameObject>();

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        foreach (var item in stayObject)
        {
            SceneManager.MoveGameObjectToScene(item, levelController.mainScene);
        }
    }
}
