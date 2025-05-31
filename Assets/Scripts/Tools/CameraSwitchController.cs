using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// 摄像机切换控制器
/// 实现两个摄像机之间的平滑过渡
/// </summary>
public class CameraSwitchController : MonoBehaviour
{
    [Header("摄像机引用")]
    [Tooltip("等待摄像机 (初始激活的摄像机)")]
    public Camera waitCamera;

    [Tooltip("准备摄像机 (目标摄像机，初始禁用)")]
    public Camera readyCamera;

    [Header("过渡设置")]
    [Tooltip("过渡持续时间(秒)")]
    [Range(0.5f, 5.0f)]
    public float transitionDuration = 2.0f;

    [Tooltip("过渡动画曲线")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("是否在过渡期间禁用摄像机旋转")]
    public bool disableRotationDuringTransition = true;

    [Header("事件设置")]
    [Tooltip("是否自动响应菜单过渡控制器的事件")]
    public bool autoLinkWithMenuController = true;
    
    [Header("调试设置")]
    [Tooltip("是否启用详细调试日志")]
    public bool enableDetailedLogs = true;
    
    // 定义摄像机切换完成事件
    public static event Action OnCameraSwitchCompleted;

    // 私有变量
    private bool _isTransitioning = false;
    private Vector3 _waitCameraInitialPosition;
    private Quaternion _waitCameraInitialRotation;
    private CameraRotator _cameraRotator;
    private MenuTransitionController _menuController;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Start()
    {
        // 检查必要的引用
        if (waitCamera == null || readyCamera == null)
        {
            Debug.LogError("CameraSwitchController: 缺少必要的摄像机引用，请在Inspector中设置。");
            enabled = false;
            return;
        }

        // 确保Ready_Camera初始禁用
        if (readyCamera.gameObject.activeSelf)
        {
            readyCamera.gameObject.SetActive(false);
        }

        // 获取摄像机旋转控制器组件
        _cameraRotator = waitCamera.GetComponent<CameraRotator>();

        // 保存初始位置和旋转
        _waitCameraInitialPosition = waitCamera.transform.position;
        _waitCameraInitialRotation = waitCamera.transform.rotation;

        // 如果设置了自动链接，尝试查找并链接菜单控制器
        if (autoLinkWithMenuController)
        {
            _menuController = FindObjectOfType<MenuTransitionController>();
            if (_menuController != null)
            {
                // 使用反射获取私有字段
                var field = typeof(MenuTransitionController).GetField("_transitionTriggered", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (field != null)
                {
                    // 添加Update事件监听
                    StartCoroutine(CheckMenuTransitionStatus());
                }
                else
                {
                    Debug.LogWarning("CameraSwitchController: 无法访问MenuTransitionController的_transitionTriggered字段，自动链接失败。");
                }
            }
            else
            {
                Debug.LogWarning("CameraSwitchController: 场景中未找到MenuTransitionController，自动链接失败。");
            }
        }
    }

    /// <summary>
    /// 检查菜单过渡状态并触发摄像机过渡
    /// </summary>
    private IEnumerator CheckMenuTransitionStatus()
    {
        bool previousTransitionState = false;
        
        while (true)
        {
            // 使用反射获取私有字段值
            var field = typeof(MenuTransitionController).GetField("_transitionTriggered", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                bool currentTransitionState = (bool)field.GetValue(_menuController);
                
                // 检测状态变化
                if (currentTransitionState && !previousTransitionState)
                {
                    // 菜单过渡刚被触发，开始摄像机过渡
                    StartCameraTransition();
                }
                
                previousTransitionState = currentTransitionState;
            }
            
            yield return null;
        }
    }

    /// <summary>
    /// 开始摄像机过渡
    /// </summary>
    public void StartCameraTransition()
    {
        // 如果已经在过渡中，则忽略
        if (_isTransitioning)
            return;

        // 开始过渡协程
        StartCoroutine(TransitionCamera());
    }

    /// <summary>
    /// 摄像机过渡协程
    /// </summary>
    private IEnumerator TransitionCamera()
    {
        _isTransitioning = true;

        // 如果设置了禁用旋转，则禁用摄像机旋转控制器
        bool wasRotating = false;
        if (_cameraRotator != null && disableRotationDuringTransition)
        {
            wasRotating = _cameraRotator.enableRotation;
            _cameraRotator.enableRotation = false;
        }

        // 获取目标位置和旋转
        Vector3 targetPosition = readyCamera.transform.position;
        Quaternion targetRotation = readyCamera.transform.rotation;

        // 获取初始位置和旋转
        Vector3 startPosition = waitCamera.transform.position;
        Quaternion startRotation = waitCamera.transform.rotation;

        // 过渡计时器
        float timer = 0;

        // 过渡循环
        while (timer < transitionDuration)
        {
            // 更新计时器
            timer += Time.deltaTime;

            // 计算过渡进度（0-1）
            float progress = timer / transitionDuration;

            // 应用动画曲线
            float curvedProgress = transitionCurve.Evaluate(progress);

            // 计算当前位置和旋转
            Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, curvedProgress);
            Quaternion currentRotation = Quaternion.Slerp(startRotation, targetRotation, curvedProgress);

            // 应用位置和旋转
            waitCamera.transform.position = currentPosition;
            waitCamera.transform.rotation = currentRotation;

            // 等待下一帧
            yield return null;
        }

        // 确保最终位置和旋转精确匹配
        waitCamera.transform.position = targetPosition;
        waitCamera.transform.rotation = targetRotation;

        // 切换摄像机
        SwitchToReadyCamera();

        // 如果之前启用了旋转，则恢复摄像机旋转控制器
        if (_cameraRotator != null && disableRotationDuringTransition && wasRotating)
        {
            _cameraRotator.enableRotation = true;
        }

        _isTransitioning = false;
    }

    [Header("直接操作设置")]
    [Tooltip("直接禁用Begin_Canvas")]
    public bool directlyDisableBeginCanvas = true;
    
    [Tooltip("Begin_Canvas对象引用")]
    public GameObject beginCanvas;
    
    [Tooltip("直接启用Start_Canvas")]
    public bool directlyEnableStartCanvas = true;
    
    [Tooltip("Start_Canvas对象引用")]
    public GameObject startCanvas;
    
    [Tooltip("直接调用StartCanvasController")]
    public bool directlyCallStartCanvasController = true;

    /// <summary>
    /// 切换到Ready_Camera
    /// </summary>
    private void SwitchToReadyCamera()
    {
        Debug.Log("CameraSwitchController: 开始执行摄像机切换");
        
        // 确保Ready_Camera的GameObject和Camera组件都是启用的
        readyCamera.gameObject.SetActive(true);
        readyCamera.enabled = true;
        
        // 确保Ready_Camera的深度高于Wait_Camera，使其成为主摄像机
        float originalDepth = readyCamera.depth;
        readyCamera.depth = waitCamera.depth + 10; // 增大深度差值，确保优先级
        
        // 禁用Wait_Camera的Camera组件和GameObject
        waitCamera.enabled = false;
        waitCamera.gameObject.SetActive(false);
        
        // 立即执行Canvas操作，不等待确认
        if (directlyDisableBeginCanvas && beginCanvas != null)
        {
            if (beginCanvas.activeSelf)
            {
                beginCanvas.SetActive(false);
                Debug.Log("CameraSwitchController: 已直接禁用Begin_Canvas");
            }
        }
        
        if (directlyEnableStartCanvas && startCanvas != null)
        {
            if (!startCanvas.activeSelf)
            {
                startCanvas.SetActive(true);
                Debug.Log("CameraSwitchController: 已直接启用Start_Canvas");
                
                // 首先尝试获取并调用SimpleTweenAnimation
                SimpleTweenAnimation tweenAnimation = startCanvas.GetComponent<SimpleTweenAnimation>();
                if (tweenAnimation != null)
                {
                    tweenAnimation.PlayAnimation();
                    Debug.Log("CameraSwitchController: 已直接调用SimpleTweenAnimation.PlayAnimation()");
                }
                // 如果没有SimpleTweenAnimation，尝试UIAnimationController
                else
                {
                    UIAnimationController uiAnimator = startCanvas.GetComponent<UIAnimationController>();
                    if (uiAnimator != null)
                    {
                        uiAnimator.StartUIAnimation();
                        Debug.Log("CameraSwitchController: 已直接调用UIAnimationController.StartUIAnimation()");
                    }
                    // 如果设置了直接调用StartCanvasController
                    else if (directlyCallStartCanvasController)
                    {
                        StartCanvasController startCanvasController = startCanvas.GetComponent<StartCanvasController>();
                        if (startCanvasController != null)
                        {
                            startCanvasController.TriggerUIAnimation();
                            Debug.Log("CameraSwitchController: 已直接调用StartCanvasController.TriggerUIAnimation()");
                        }
                    }
                }
            }
        }
        
        // 延迟一帧，确保摄像机切换已生效
        StartCoroutine(ConfirmCameraSwitch(originalDepth));
        
        Debug.Log("CameraSwitchController: 摄像机已切换到Ready_Camera");
    }
    
    /// <summary>
    /// 确认摄像机切换已生效
    /// </summary>
    private IEnumerator ConfirmCameraSwitch(float originalDepth)
    {
        // 等待两帧
        yield return null;
        yield return null;
        
        // 确保Ready_Camera仍然是启用状态
        if (!readyCamera.gameObject.activeSelf || !readyCamera.enabled)
        {
            Debug.LogWarning("CameraSwitchController: Ready_Camera可能被意外禁用，重新启用");
            readyCamera.gameObject.SetActive(true);
            readyCamera.enabled = true;
        }
        
        // 确保Wait_Camera仍然是禁用状态
        if (waitCamera.gameObject.activeSelf || waitCamera.enabled)
        {
            Debug.LogWarning("CameraSwitchController: Wait_Camera可能被意外启用，重新禁用");
            waitCamera.enabled = false;
            waitCamera.gameObject.SetActive(false);
        }
        
        // 检查当前主摄像机是否是Ready_Camera - 注意：这可能是误报
        if (Camera.main != readyCamera && enableDetailedLogs)
        {
            Debug.Log("CameraSwitchController: 正在验证摄像机切换 - 当前主摄像机: " + (Camera.main != null ? Camera.main.name : "null") + ", 预期主摄像机: " + readyCamera.name);
            
            // 如果不是，尝试再次强制设置
            foreach (Camera cam in Camera.allCameras)
            {
                if (cam != readyCamera)
                {
                    cam.enabled = false;
                    Debug.Log($"CameraSwitchController: 禁用了摄像机 {cam.name}");
                }
            }
            
            readyCamera.enabled = true;
            readyCamera.depth = 999; // 设置非常高的深度值确保优先级
            Debug.Log("CameraSwitchController: 已设置Ready_Camera深度为999以确保其为主摄像机");
            
            // 再次直接操作Canvas
            if (directlyDisableBeginCanvas && beginCanvas != null)
            {
                beginCanvas.SetActive(false);
                Debug.Log("CameraSwitchController: 确认阶段再次禁用Begin_Canvas");
            }
            
            if (directlyEnableStartCanvas && startCanvas != null)
            {
                startCanvas.SetActive(true);
                Debug.Log("CameraSwitchController: 确认阶段再次启用Start_Canvas");
                
                // 首先尝试获取并调用SimpleTweenAnimation
                SimpleTweenAnimation tweenAnimation = startCanvas.GetComponent<SimpleTweenAnimation>();
                if (tweenAnimation != null)
                {
                    tweenAnimation.PlayAnimation();
                    Debug.Log("CameraSwitchController: 确认阶段再次调用SimpleTweenAnimation.PlayAnimation()");
                }
                // 如果没有SimpleTweenAnimation，尝试UIAnimationController
                else
                {
                    UIAnimationController uiAnimator = startCanvas.GetComponent<UIAnimationController>();
                    if (uiAnimator != null)
                    {
                        uiAnimator.StartUIAnimation();
                        Debug.Log("CameraSwitchController: 确认阶段再次调用UIAnimationController.StartUIAnimation()");
                    }
                    // 如果设置了直接调用StartCanvasController
                    else if (directlyCallStartCanvasController)
                    {
                        StartCanvasController startCanvasController = startCanvas.GetComponent<StartCanvasController>();
                        if (startCanvasController != null)
                        {
                            startCanvasController.TriggerUIAnimation();
                            Debug.Log("CameraSwitchController: 确认阶段再次调用StartCanvasController.TriggerUIAnimation()");
                        }
                    }
                }
            }
        }
        
        // 无论如何都触发摄像机切换完成事件
        if (OnCameraSwitchCompleted != null)
        {
            Debug.Log("CameraSwitchController: 触发摄像机切换完成事件");
            OnCameraSwitchCompleted.Invoke();
        }
        
        // 设置标志
        _isTransitioning = false;
        
        Debug.Log("CameraSwitchController: 摄像机切换确认完成");
    }

    /// <summary>
    /// 重置摄像机到初始状态
    /// </summary>
    public void ResetCameras()
    {
        // 停止所有协程
        StopAllCoroutines();

        // 重置过渡状态
        _isTransitioning = false;

        // 重置Wait_Camera位置和旋转
        waitCamera.transform.position = _waitCameraInitialPosition;
        waitCamera.transform.rotation = _waitCameraInitialRotation;

        // 启用Wait_Camera
        waitCamera.gameObject.SetActive(true);

        // 禁用Ready_Camera
        readyCamera.gameObject.SetActive(false);

        // 如果有摄像机旋转控制器，重置它
        if (_cameraRotator != null)
        {
            _cameraRotator.ResetRotation();
            _cameraRotator.enableRotation = true;
        }

        // 如果设置了自动链接，重新开始监听
        if (autoLinkWithMenuController && _menuController != null)
        {
            StartCoroutine(CheckMenuTransitionStatus());
        }
    }

    /// <summary>
    /// 手动触发摄像机过渡（用于外部调用）
    /// </summary>
    public void TriggerCameraTransition()
    {
        StartCameraTransition();
    }
}
