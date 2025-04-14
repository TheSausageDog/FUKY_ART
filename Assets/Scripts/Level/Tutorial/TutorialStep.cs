using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStepBase : MonoBehaviour
{
    [NonSerialized]
    public LevelController levelController;

    public virtual void TutorialStart(LevelController _levelController)
    { levelController = _levelController; }

    public virtual bool TutorialUpdate()
    { return false; }

    public virtual void TutorialEnd() { }
}


public class TutorialStep : TutorialStepBase
{
    public string dialogText = null;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        if (dialogText != null && dialogText.Length != 0)
        {
            levelController.uiDialogText.text = dialogText;
            levelController.uiDialog.gameObject.SetActive(true);
        }
    }

    public override void TutorialEnd()
    {
        if (dialogText != null && dialogText.Length != 0)
        {
            levelController.uiDialog.gameObject.SetActive(false);
        }
        base.TutorialEnd();
    }
}