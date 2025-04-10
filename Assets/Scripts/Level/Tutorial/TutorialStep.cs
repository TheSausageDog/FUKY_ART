using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialStepBase : MonoBehaviour
{
    [NonSerialized]
    public LevelController levelController;

    public virtual void EndStep()
    {
        levelController.NectStep();
    }
}


public class TutorialStep : TutorialStepBase
{
    public string dialogText = null;

    public virtual void Start()
    {
        if (dialogText != null && dialogText.Length != 0)
        {
            levelController.uiDialogText.text = dialogText;
            levelController.uiDialog.gameObject.SetActive(true);
        }
    }

    public override void EndStep()
    {
        if (dialogText != null && dialogText.Length != 0)
        {
            levelController.uiDialog.gameObject.SetActive(false);
        }
        base.EndStep();
    }
}