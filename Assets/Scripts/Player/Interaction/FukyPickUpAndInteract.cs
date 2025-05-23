using Unity.Mathematics;
using UnityEngine;

public class FukyPickUpAndInteract : PickUpAndInteract
{   public Transform Player;
    //public bool DeltaMethord = false;
    [Tooltip("Z轴单独的偏移")]
    [Range(-500F, 500F)]
    public float Z_Offset;
    protected override void MoveHandTarget()
    {
        //why minus?
        //if (DeltaMethord)
        //{
        //    //相对位移量的方式
        //    Vector3 newLocalPosition = transform.InverseTransformPoint(data.handTarget.position) - FUKYMouse.Instance.deltaTranslate;

        //    newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, data.xMinMax.x, data.xMinMax.y);
        //    newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, data.yMinMax.x, data.yMinMax.y);
        //    newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, data.zMinMax.x, data.zMinMax.y);

        //    //data.handTarget.position = transform.TransformPoint(newLocalPosition);

        //    Vector3 euler_rotation = FUKYMouse.Instance.deltaRotation.eulerAngles;
        //    //data.handTarget.rotation = data.handTarget.rotation * transform.rotation * FUKYMouse.Instance.deltaRotation * Quaternion.Inverse(transform.rotation);
        //    data.handTarget.rotation *= Quaternion.AngleAxis(euler_rotation.y, transform.up) * Quaternion.AngleAxis(euler_rotation.z, transform.right) * Quaternion.AngleAxis(euler_rotation.x, transform.forward);
        //    //data.handTarget.rotation = FUKYMouse.Instance.rawRotation;
        //}
        //else
        //{
        Vector3 NewPos = new Vector3(FUKYMouse.Instance.filteredTranslate.x, FUKYMouse.Instance.filteredTranslate.y, FUKYMouse.Instance.filteredTranslate.z - Z_Offset);
        //直接值的方式
        data.handTarget.localPosition = NewPos;
        data.handTarget.rotation = Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;

        //}

    }
}
