using Unity.Mathematics;
using UnityEngine;

public class FukyPickUpAndInteract : PickUpAndInteract
{   
    public Transform Player;
    public Transform PlayerCamera;
    //public bool DeltaMethord = false;
    [Tooltip("Z轴偏移")]
    [Range(-10F, 10F)]
    public float Z_Offset;
    protected override void MoveHandTarget()
    {

        Vector3 NewPos = data.handTarget.localPosition + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue; // 使用了delta的方式，更方便控制位移量
        NewPos.x = Mathf.Clamp(NewPos.x, data.xMinMax.x, data.xMinMax.y);// 限制 handTarget 的本地位置
        NewPos.y = Mathf.Clamp(NewPos.y, data.yMinMax.x, data.yMinMax.y);
        NewPos.z = Mathf.Clamp(NewPos.z, data.zMinMax.x, data.zMinMax.y);
        data.handTarget.localPosition = NewPos;

        // 将世界空间的旋转转换到相机空间
        Quaternion cameraSpaceRotation = Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;
        data.holdPos.rotation = cameraSpaceRotation;



    }
}
