
using UnityEngine;

public class TutorialDialogStep : TutorialStepBase
{
    public string dialogText = null;

    public float duration = 2;

    protected float start_time;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        if (!string.IsNullOrEmpty(dialogText))
        {
            levelController.uiDialogText.text = dialogText;
            levelController.uiDialog.gameObject.SetActive(true);
        }
        start_time = Time.time;
    }

    public override bool TutorialUpdate()
    {
        return Time.time - start_time < duration;
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