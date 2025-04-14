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

    public string taskName = null;

    public string taskStepName = null;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        if (!string.IsNullOrEmpty(dialogText))
        {
            levelController.uiDialogText.text = dialogText;
            levelController.uiDialog.gameObject.SetActive(true);
        }

        if (!string.IsNullOrEmpty(taskName) && !string.IsNullOrEmpty(taskStepName))
        {
            TaskManager.Instance.showUI = true;
            TaskManager.Instance.AddTaskStep(taskName, taskStepName);
        }
    }

    public override void TutorialEnd()
    {
        if (dialogText != null && dialogText.Length != 0)
        {
            levelController.uiDialog.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(taskName) && !string.IsNullOrEmpty(taskStepName))
        {
            TaskManager.Instance.showUI = true;
            TaskManager.Instance.RemoveTaskStep(taskName, taskStepName);
        }
        base.TutorialEnd();
    }
}