using UnityEngine;

public class FoodRecorder : ContainRecorder
{
    public bool isDirty { get; set; }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Food>(out var _))
        {
            inside.Add(other);
            isDirty = true;
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        inside.Remove(other);
        isDirty = true;
    }
}