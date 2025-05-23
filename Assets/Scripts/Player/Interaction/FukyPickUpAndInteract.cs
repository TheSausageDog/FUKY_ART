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

        Vector3 NewPos = new Vector3(FUKYMouse.Instance.filteredTranslate.x, FUKYMouse.Instance.filteredTranslate.y, FUKYMouse.Instance.filteredTranslate.z - Z_Offset);
        data.handTarget.localPosition = NewPos;

        // 将世界空间的旋转转换到相机空间
        Quaternion cameraSpaceRotation = Quaternion.Inverse(PlayerCamera.rotation) * FUKYMouse.Instance.rawRotation;
        data.holdPos.rotation = cameraSpaceRotation;


        //data.handTarget.rotation = Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;
        //data.holdPos.rotation = FUKYMouse.Instance.rawRotation;
    }
}
