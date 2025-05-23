using UnityEngine;

public class KeyBoardPickUpAndInteract : PickUpAndInteract
{
    protected override void MoveHandTarget()
    {
        if (PlayerInputController.IsRotateHeld())
        {
            Vector2 mouseInput = PlayerInputController.GetMouseInput() * data.rotationSensitivity;
            float scrollInput = PlayerInputController.GetScrollInput() * 10;

            data.holdPos.transform.Rotate(Vector3.up, mouseInput.x, Space.Self);
            data.holdPos.transform.Rotate(Vector3.right, mouseInput.y, Space.Self);
            data.holdPos.transform.Rotate(Vector3.forward, scrollInput, Space.Self);
        }
        else
        {
            // 如果没有持有物体，则允许鼠标移动 handTarget
            // if (!PlayerBlackBoard.isHeldObj || PlayerBlackBoard.holdingKnife)
            // {
            // 获取鼠标输入
            Vector2 mouseInput = PlayerInputController.GetMouseInput();
            float scrollInput = PlayerInputController.GetScrollInput();
            // 计算基于相机本地坐标系的移动量
            Vector3 screen_move = new Vector3(mouseInput.x * data.mouseSensitivity, scrollInput * data.scrollSensitivity, mouseInput.y * data.mouseSensitivity);
            // Vector3 world_move = new Vector3(0, scrollInput * scrollSensitivity, 0);

            // bool needLock = false;

            if (PlayerBlackBoard.moveLock != Vector3.zero)
            {
                // needLock = true;

                // var length = moveDelta.y;

                // moveDelta = PlayerBlackBoard.moveLock * length;
                screen_move = Vector3.Project(screen_move, PlayerBlackBoard.moveLock);
            }

            // if (!needLock)
            // {
            // 更新 handTarget 的本地位置
            Vector3 newLocalPosition = transform.InverseTransformPoint(data.handTarget.position) + screen_move;

            // 限制 handTarget 的本地位置
            newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, data.xMinMax.x, data.xMinMax.y);
            newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, data.yMinMax.x, data.yMinMax.y);
            newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, data.zMinMax.x, data.zMinMax.y);

            Vector3 clampedWorldPosition = transform.TransformPoint(newLocalPosition);
            data.handTarget.position = clampedWorldPosition;
        }
    }
}
