using UnityEngine;

public class FukyPickUpAndInteract : PickUpAndInteract
{
    protected override void MoveHandTarget()
    {
        //why minus?
        Vector3 newLocalPosition = transform.InverseTransformPoint(data.handTarget.position) - FUKYMouse.Instance.deltaTranslate;

        newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, data.xMinMax.x, data.xMinMax.y);
        newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, data.yMinMax.x, data.yMinMax.y);
        newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, data.zMinMax.x, data.zMinMax.y);

        // data.handTarget.position = transform.TransformPoint(newLocalPosition);

        // Vector3 euler_rotation = FUKYMouse.Instance.deltaRotation.eulerAngles;
        // data.handTarget.rotation = data.handTarget.rotation * transform.rotation * FUKYMouse.Instance.deltaRotation * Quaternion.Inverse(transform.rotation);

        // data.handTarget.rotation *= Quaternion.AngleAxis(euler_rotation.y, transform.up) * Quaternion.AngleAxis(euler_rotation.z, transform.right) * Quaternion.AngleAxis(euler_rotation.x, transform.forward);

        data.handTarget.rotation = FUKYMouse.Instance.rawRotation;
    }
}
