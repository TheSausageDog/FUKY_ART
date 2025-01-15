using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouching : MonoBehaviour
{
    public Transform standinglocalPosition; // 站立时的相机位置
    public Transform crouchinglocalPosition; // 下蹲时的相机位置
    public Camera playerCamera; // 玩家相机
    public float transitionSpeed = 5f; // 相机位置切换的平滑速度

    private bool isCrouching = false; // 当前是否处于下蹲状态
    private Vector3 targetlocalPosition; // 相机目标位置
    private PlayerInputController inputController; // 输入控制器

    private void Start()
    {
        // 获取输入控制器
        inputController = GetComponent<PlayerInputController>();

        // 初始设置相机位置为站立位置
        if (playerCamera != null && standinglocalPosition != null)
        {
            playerCamera.transform.localPosition = standinglocalPosition.localPosition;
        }
    }

    private void Update()
    {
        HandleCrouching();
    }

    private void HandleCrouching()
    {
        if (inputController == null)
        {
            Debug.LogError("PlayerInputController is missing!");
            return;
        }

        // 检测是否按下下蹲键
        if (inputController.IsCrouching())
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        // 根据下蹲状态设置目标位置
        if (isCrouching)
        {
            targetlocalPosition = crouchinglocalPosition.localPosition;
        }
        else
        {
            targetlocalPosition = standinglocalPosition.localPosition;
        }

        // 平滑移动相机到目标位置
        if (playerCamera != null)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetlocalPosition, transitionSpeed * Time.deltaTime);
        }
    }
}