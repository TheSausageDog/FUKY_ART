using UnityEngine;

public class SaucePot : WaterFlow
{
    public override void Update()
    {
        if (isUp && Vector3.Angle(transform.right, Vector3.down) < angleTreshold)
        {
            isUp = false;
            StartDrop();
        }
        else if (!isUp && Vector3.Angle(transform.right, Vector3.down) > angleTreshold)
        {
            isUp = true;
            EndDrop();
        }
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(transform.position, transform.position + transform.right);
    // }
}