
using UnityEngine;

public class TutorialDialogStep : TutorialStep
{
    public float duration = 2;

    protected float start_time;

    public override void TutorialStart(LevelController _levelController)
    {
        base.TutorialStart(_levelController);
        start_time = Time.time;
    }

    public override bool TutorialUpdate()
    {
        return Time.time - start_time < duration;
    }
}