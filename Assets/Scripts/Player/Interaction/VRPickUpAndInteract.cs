using UnityEngine;
using UnityEngine.XR;
public class VRPickUpAndInteract : PickUpAndInteract
{
    public Transform VRRightHand;

    public Vector3 offset = Vector3.forward;

    protected override Ray GetPickUpRay => new Ray(VRRightHand.position, VRRightHand.forward);

    protected override void MoveHandTarget()
    {

        Vector3 newLocalPosition = transform.InverseTransformPoint(VRRightHand.position + VRRightHand.TransformDirection(offset));

        // 限制 handTarget 的本地位置
        // newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, data.xMinMax.x, data.xMinMax.y);
        // newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, data.yMinMax.x, data.yMinMax.y);
        // newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, data.zMinMax.x, data.zMinMax.y);

        Vector3 clampedWorldPosition = transform.TransformPoint(newLocalPosition);
        data.handTarget.position = clampedWorldPosition;
        // data.holdPos.position = clampedWorldPosition;
        data.holdPos.rotation = VRRightHand.rotation;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (VRRightHand != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(VRRightHand.position, 0.05f);
        }
    }

    // public override void PickObject(HoldableItem pickItem)
    // {
    //     uiCursor.SetActive(false);
    //     base.PickObject(pickItem);
    // }

    // public override void DropObject()
    // {
    //     uiCursor.SetActive(true);
    //     base.DropObject();
    // }
}

