using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionConfig : MonoBehaviour
{
    public Vector3 handTargetOffset;

    public float rotationSensitivity = 1f; // 旋转灵敏度

    public float pickUpRange = 5f; // 拾取范围

    public GameObject uiCursor;

    [Header("手部相关")]
    public Transform holdPos; // 物品持有位置
    public Transform handTarget;
    public float CameraFieldOfViewOrgin = 0;
    public float CameraFieldOfViewOffset = 0;
    public float maxHandMovingSpeed = 5;

    [Header("按下Alt后手移动的灵敏度")]
    public float mouseSensitivity = 0.1f; // 鼠标灵敏度
    public float scrollSensitivity = 1f; // 滚轮灵敏度

    [Header("手可以移动的范围")]
    // 手部移动限制
    public Vector2 xMinMax;
    public Vector2 yMinMax;
    public Vector2 zMinMax;
}
