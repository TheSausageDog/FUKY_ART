
using UnityEngine;

public class TutorialDelayStep : TutorialStepBase
{
    public float duration = 2;

    protected float start_time;

    public virtual void Start()
    {
        start_time = Time.time;
    }

    public virtual void Update()
    {
        if (Time.time - start_time > duration) { EndStep(); }
    }
}