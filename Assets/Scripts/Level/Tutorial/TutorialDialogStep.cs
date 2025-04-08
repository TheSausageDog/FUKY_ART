
using UnityEngine;

public class TutorialDialogStep : TutorialStep{
    public float duration = 2;
    
    protected float start_time;

    public override void Start()
    {
        base.Start();
        start_time = Time.time;
    }

    void Update()
    {
        if(Time.time - start_time > duration){ EndStep(); }
    }
}