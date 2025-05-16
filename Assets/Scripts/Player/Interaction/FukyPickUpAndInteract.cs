using UnityEngine;

public class FukyPickUpAndInteract : PickUpAndInteract
{
    protected override void MoveHandTarget()
    {
        // Vector3 newLocalPosition = transform.InverseTransformPoint(data.handTarget.position) + screen_move;

        // // 限制 handTarget 的本地位置
        // newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, data.xMinMax.x, data.xMinMax.y);
        // newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, data.yMinMax.x, data.yMinMax.y);
        // newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, data.zMinMax.x, data.zMinMax.y);

        // Vector3 clampedWorldPosition = transform.TransformPoint(newLocalPosition);

        data.handTarget.localPosition += FUKYMouse.Instance.deltaTranslate;
        data.handTarget.localRotation *= FUKYMouse.Instance.deltaRotation;
    }
}
