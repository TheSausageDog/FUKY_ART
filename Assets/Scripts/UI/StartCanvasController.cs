using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Start_Canvas控制器
/// 处理从Begin_Canvas到Start_Canvas的过渡动画
/// </summary>
public class StartCanvasController : MonoBehaviour
{
    [Header("Canvas引用")]
    [Tooltip("Start_Canvas对象")]
    public Canvas startCanvas;

    [Tooltip("Begin_Canvas对象")]
    public GameObject beginCanvas;

    [Header("UI元素引用")]
    [Tooltip("标题图案")]
    public RectTransform titleLogo;

    [Tooltip("底部装饰")]
    public RectTransform bottomDecoration;

    [Tooltip("按钮组父对象")]
    public RectTransform buttonsGroup;

    [Header("动画设置")]
    [Tooltip("动画持续时间(秒)")]
    [Range(0.5f, 3.0f)]
    public float animationDuration = 1.5f;

    [Tooltip("元素进入时间间隔(秒)")]
    [Range(0.1f, 1.0f)]
    public float elementDelay = 0.2f;

    [Tooltip("动画曲线")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("屏幕外偏移距离(相对于屏幕尺寸)")]
    [Range(0.5f, 2.0f)]
    public float offscreenOffset = 1.2f;

    [Header("按钮交互设置")]
    [Tooltip("是否在动画完成前禁用按钮交互")]
    public bool disableButtonsUntilAnimationComplete = true;

    [Header("事件设置")]
    [Tooltip("是否自动监听摄像机切换事件")]
    public bool autoLinkWithCameraSwitch = true;

    [Tooltip("摄像机切换控制器")]
    public CameraSwitchController cameraSwitchController;

    // 私有变量
    private Vector2 _titleOriginalPosition;
    private Vector2 _bottomOriginalPosition;
    private Vector2[] _buttonOriginalPositions;
    private bool _animationStarted = false;
    private bool _animationCompleted = false;
    private float _canvasWidth;
    private float _canvasHeight;
    private Button[] _buttons;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Awake()
    {
        // 检查必要的引用
        if (startCanvas == null || titleLogo == null || bottomDecoration == null || buttonsGroup == null)
        {
            Debug.LogError("StartCanvasController: 缺少必要的UI引用，请在Inspector中设置。");
            enabled = false;
            return;
        }

        // 获取Canvas尺寸
        RectTransform canvasRect = startCanvas.GetComponent<RectTransform>();
        if (canvasRect != null)
        {
            _canvasWidth = canvasRect.rect.width;
            _canvasHeight = canvasRect.rect.height;
        }
        else
        {
            _canvasWidth = Screen.width;
            _canvasHeight = Screen.height;
        }

        // 保存原始位置
        _titleOriginalPosition = titleLogo.anchoredPosition;
        _bottomOriginalPosition = bottomDecoration.anchoredPosition;

        // 保存所有按钮的原始位置
        RectTransform[] buttons = buttonsGroup.GetComponentsInChildren<RectTransform>(true);
        _buttonOriginalPositions = new Vector2[buttons.Length - 1]; // 减1是因为第一个是父对象
        
        int buttonIndex = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            // 跳过父对象
            if (buttons[i] == buttonsGroup)
                continue;
                
            _buttonOriginalPositions[buttonIndex] = buttons[i].anchoredPosition;
            buttonIndex++;
        }
        
        // 获取所有按钮组件
        _buttons = buttonsGroup.GetComponentsInChildren<Button>(true);
    }

    /// <summary>
    /// 初始化时设置UI元素位置
    /// </summary>
    private void Start()
    {
        // 确保Start_Canvas初始禁用
        if (startCanvas.gameObject.activeSelf)
        {
            startCanvas.gameObject.SetActive(false);
        }
        
        // 初始化时将UI元素移动到屏幕外
        MoveUIElementsOffscreen();
        
        // 如果设置了禁用按钮交互，则禁用所有按钮
        if (disableButtonsUntilAnimationComplete && _buttons != null)
        {
            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
        }
        
        // 如果设置了自动链接，尝试查找并链接摄像机切换控制器
        if (autoLinkWithCameraSwitch && cameraSwitchController == null)
        {
            cameraSwitchController = FindObjectOfType<CameraSwitchController>();
            if (cameraSwitchController == null)
            {
                Debug.LogWarning("StartCanvasController: 场景中未找到CameraSwitchController，自动链接失败。");
            }
        }
        
        // 订阅摄像机切换完成事件
        CameraSwitchController.OnCameraSwitchCompleted += OnCameraSwitchCompleted;
        
        // 开始监听摄像机切换事件（备用方法）
        if (cameraSwitchController != null && useAlternativeMonitoring)
        {
            StartCoroutine(MonitorCameraSwitch());
        }
        
        // 如果没有设置Begin_Canvas引用，尝试查找
        if (beginCanvas == null)
        {
            beginCanvas = GameObject.Find("Begin_Canvas");
            if (beginCanvas == null)
            {
                Debug.LogWarning("StartCanvasController: 未找到Begin_Canvas对象，无法自动禁用。");
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasController: 初始化完成，Start_Canvas已禁用，等待摄像机切换事件");
        }
    }
    
    /// <summary>
    /// 组件销毁时取消订阅事件
    /// </summary>
    private void OnDestroy()
    {
        // 取消订阅摄像机切换完成事件
        CameraSwitchController.OnCameraSwitchCompleted -= OnCameraSwitchCompleted;
    }
    
    /// <summary>
    /// 摄像机切换完成事件处理
    /// </summary>
    private void OnCameraSwitchCompleted()
    {
        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasController: 收到摄像机切换完成事件");
        }
        
        // 禁用Begin_Canvas
        if (beginCanvas != null && beginCanvas.activeSelf)
        {
            beginCanvas.SetActive(false);
            
            if (enableDebugLogs)
            {
                Debug.Log("StartCanvasController: Begin_Canvas已禁用");
            }
        }
        
        // 强制启用Start_Canvas
        if (!startCanvas.gameObject.activeSelf)
        {
            startCanvas.gameObject.SetActive(true);
            
            if (enableDebugLogs)
            {
                Debug.Log("StartCanvasController: Start_Canvas已启用");
            }
            
            // 开始UI动画
            StartUIAnimation();
        }
    }

    /// <summary>
    /// 将UI元素移动到屏幕外
    /// </summary>
    private void MoveUIElementsOffscreen()
    {
        // 将标题移动到屏幕上方
        titleLogo.anchoredPosition = new Vector2(
            _titleOriginalPosition.x, 
            _titleOriginalPosition.y + _canvasHeight * offscreenOffset
        );
        
        // 将底部装饰移动到屏幕下方
        bottomDecoration.anchoredPosition = new Vector2(
            _bottomOriginalPosition.x, 
            _bottomOriginalPosition.y - _canvasHeight * offscreenOffset
        );
        
        // 将所有按钮移动到屏幕左侧
        RectTransform[] buttons = buttonsGroup.GetComponentsInChildren<RectTransform>(true);
        int buttonIndex = 0;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            // 跳过父对象
            if (buttons[i] == buttonsGroup)
                continue;
                
            buttons[i].anchoredPosition = new Vector2(
                _buttonOriginalPositions[buttonIndex].x - _canvasWidth * offscreenOffset,
                _buttonOriginalPositions[buttonIndex].y
            );
            
            buttonIndex++;
        }
    }

    [Header("调试设置")]
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = true;

    [Tooltip("是否使用备用监听方法")]
    public bool useAlternativeMonitoring = true;

    /// <summary>
    /// 监听摄像机切换事件
    /// </summary>
    private IEnumerator MonitorCameraSwitch()
    {
        bool previousTransitioning = true;
        
        // 等待一帧，确保所有组件都已初始化
        yield return null;
        
        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasController: 开始监听摄像机切换事件");
        }
        
        // 如果使用备用监听方法，直接监听Ready_Camera的激活状态
        if (useAlternativeMonitoring && cameraSwitchController != null)
        {
            Camera readyCamera = null;
            
            // 尝试获取Ready_Camera引用
            var readyCameraField = typeof(CameraSwitchController).GetField("readyCamera", 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.Instance);
                
            if (readyCameraField != null)
            {
                readyCamera = (Camera)readyCameraField.GetValue(cameraSwitchController);
                
                if (enableDebugLogs)
                {
                    Debug.Log("StartCanvasController: 成功获取Ready_Camera引用");
                }
            }
            
            if (readyCamera != null)
            {
                // 等待Ready_Camera被激活
                while (!readyCamera.gameObject.activeSelf)
                {
                    yield return null;
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log("StartCanvasController: 检测到Ready_Camera已激活，开始UI过渡");
                }
                
                // Ready_Camera已激活，禁用Begin_Canvas并启用Start_Canvas
                if (beginCanvas != null && beginCanvas.activeSelf)
                {
                    beginCanvas.SetActive(false);
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log("StartCanvasController: Begin_Canvas已禁用");
                    }
                }
                
                // 强制启用Start_Canvas
                startCanvas.gameObject.SetActive(true);
                
                if (enableDebugLogs)
                {
                    Debug.Log("StartCanvasController: Start_Canvas已启用");
                }
                
                // 开始UI动画
                StartUIAnimation();
                
                // 不再继续监听
                yield break;
            }
        }
        
        // 如果备用方法不可用或未启用，使用原始反射方法
        while (true)
        {
            // 使用反射获取私有字段值
            var field = typeof(CameraSwitchController).GetField("_isTransitioning", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                bool currentTransitioning = (bool)field.GetValue(cameraSwitchController);
                
                // 检测状态变化：从过渡中到过渡完成
                if (previousTransitioning && !currentTransitioning)
                {
                    if (enableDebugLogs)
                    {
                        Debug.Log("StartCanvasController: 检测到摄像机过渡完成，开始UI过渡");
                    }
                    
                    // 摄像机过渡刚完成，禁用Begin_Canvas并启用Start_Canvas
                    if (beginCanvas != null && beginCanvas.activeSelf)
                    {
                        beginCanvas.SetActive(false);
                        
                        if (enableDebugLogs)
                        {
                            Debug.Log("StartCanvasController: Begin_Canvas已禁用");
                        }
                    }
                    
                    // 强制启用Start_Canvas
                    startCanvas.gameObject.SetActive(true);
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log("StartCanvasController: Start_Canvas已启用");
                    }
                    
                    // 开始UI动画
                    StartUIAnimation();
                    
                    // 不再继续监听
                    yield break;
                }
                
                previousTransitioning = currentTransitioning;
            }
            
            yield return null;
        }
    }

    /// <summary>
    /// 开始UI动画
    /// </summary>
    public void StartUIAnimation()
    {
        // 如果已经开始动画，则忽略
        if (_animationStarted)
            return;
            
        _animationStarted = true;
        _animationCompleted = false;
        
        // 开始各元素的动画
        StartCoroutine(AnimateTitleLogo());
        StartCoroutine(AnimateBottomDecoration());
        StartCoroutine(AnimateButtons());
        
        // 启动动画完成检查
        StartCoroutine(CheckAnimationCompletion());
    }

    /// <summary>
    /// 检查所有动画是否完成
    /// </summary>
    private IEnumerator CheckAnimationCompletion()
    {
        // 等待所有动画完成的时间（标题 + 底部装饰 + 按钮组 + 最后一个按钮）
        float totalAnimationTime = animationDuration + elementDelay + elementDelay * 2 + 
                                  (_buttons != null ? _buttons.Length * elementDelay * 0.5f : 0) + 
                                  animationDuration;
        
        // 等待动画完成
        yield return new WaitForSeconds(totalAnimationTime);
        
        // 标记动画已完成
        _animationCompleted = true;
        
        // 如果设置了禁用按钮交互，则启用所有按钮
        if (disableButtonsUntilAnimationComplete && _buttons != null)
        {
            foreach (Button button in _buttons)
            {
                button.interactable = true;
            }
        }
        
        Debug.Log("StartCanvasController: 所有UI动画已完成，按钮已启用交互。");
    }

    /// <summary>
    /// 标题图案动画
    /// </summary>
    private IEnumerator AnimateTitleLogo()
    {
        // 获取初始位置和目标位置
        Vector2 startPosition = titleLogo.anchoredPosition;
        Vector2 targetPosition = _titleOriginalPosition;
        
        // 动画计时器
        float timer = 0;
        
        // 动画循环
        while (timer < animationDuration)
        {
            // 更新计时器
            timer += Time.deltaTime;
            
            // 计算动画进度（0-1）
            float progress = timer / animationDuration;
            
            // 应用动画曲线
            float curvedProgress = animationCurve.Evaluate(progress);
            
            // 计算当前位置
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, curvedProgress);
            
            // 应用位置
            titleLogo.anchoredPosition = currentPosition;
            
            // 等待下一帧
            yield return null;
        }
        
        // 确保最终位置精确
        titleLogo.anchoredPosition = targetPosition;
    }

    /// <summary>
    /// 底部装饰动画
    /// </summary>
    private IEnumerator AnimateBottomDecoration()
    {
        // 等待一小段时间再开始动画
        yield return new WaitForSeconds(elementDelay);
        
        // 获取初始位置和目标位置
        Vector2 startPosition = bottomDecoration.anchoredPosition;
        Vector2 targetPosition = _bottomOriginalPosition;
        
        // 动画计时器
        float timer = 0;
        
        // 动画循环
        while (timer < animationDuration)
        {
            // 更新计时器
            timer += Time.deltaTime;
            
            // 计算动画进度（0-1）
            float progress = timer / animationDuration;
            
            // 应用动画曲线
            float curvedProgress = animationCurve.Evaluate(progress);
            
            // 计算当前位置
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, curvedProgress);
            
            // 应用位置
            bottomDecoration.anchoredPosition = currentPosition;
            
            // 等待下一帧
            yield return null;
        }
        
        // 确保最终位置精确
        bottomDecoration.anchoredPosition = targetPosition;
    }

    /// <summary>
    /// 按钮组动画
    /// </summary>
    private IEnumerator AnimateButtons()
    {
        // 等待更长一点的时间再开始按钮动画
        yield return new WaitForSeconds(elementDelay * 2);
        
        // 获取所有按钮
        RectTransform[] buttons = buttonsGroup.GetComponentsInChildren<RectTransform>(true);
        
        // 为每个按钮创建动画
        int buttonIndex = 0;
        
        for (int i = 0; i < buttons.Length; i++)
        {
            // 跳过父对象
            if (buttons[i] == buttonsGroup)
                continue;
                
            // 开始这个按钮的动画
            StartCoroutine(AnimateButton(buttons[i], _buttonOriginalPositions[buttonIndex], buttonIndex));
            
            buttonIndex++;
        }
    }

    /// <summary>
    /// 单个按钮动画
    /// </summary>
    private IEnumerator AnimateButton(RectTransform button, Vector2 targetPosition, int index)
    {
        // 按钮之间的额外延迟
        yield return new WaitForSeconds(index * elementDelay * 0.5f);
        
        // 获取初始位置
        Vector2 startPosition = button.anchoredPosition;
        
        // 动画计时器
        float timer = 0;
        
        // 动画循环
        while (timer < animationDuration)
        {
            // 更新计时器
            timer += Time.deltaTime;
            
            // 计算动画进度（0-1）
            float progress = timer / animationDuration;
            
            // 应用动画曲线
            float curvedProgress = animationCurve.Evaluate(progress);
            
            // 计算当前位置
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, curvedProgress);
            
            // 应用位置
            button.anchoredPosition = currentPosition;
            
            // 等待下一帧
            yield return null;
        }
        
        // 确保最终位置精确
        button.anchoredPosition = targetPosition;
    }

    /// <summary>
    /// 重置UI元素到初始状态
    /// </summary>
    public void ResetUI()
    {
        // 停止所有协程
        StopAllCoroutines();
        
        // 重置动画状态
        _animationStarted = false;
        _animationCompleted = false;
        
        // 禁用Start_Canvas
        startCanvas.gameObject.SetActive(false);
        
        // 启用Begin_Canvas
        if (beginCanvas != null)
        {
            beginCanvas.SetActive(true);
        }
        
        // 将UI元素移回屏幕外
        MoveUIElementsOffscreen();
        
        // 如果设置了禁用按钮交互，则禁用所有按钮
        if (disableButtonsUntilAnimationComplete && _buttons != null)
        {
            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
        }
        
        // 如果设置了自动链接，重新开始监听
        if (autoLinkWithCameraSwitch && cameraSwitchController != null)
        {
            StartCoroutine(MonitorCameraSwitch());
        }
    }

    /// <summary>
    /// 手动触发UI动画（用于外部调用）
    /// </summary>
    public void TriggerUIAnimation()
    {
        // 禁用Begin_Canvas
        if (beginCanvas != null && beginCanvas.activeSelf)
        {
            beginCanvas.SetActive(false);
        }
        
        // 启用Start_Canvas
        startCanvas.gameObject.SetActive(true);
        
        // 开始UI动画
        StartUIAnimation();
    }
    
    /// <summary>
    /// 检查动画是否已完成
    /// </summary>
    public bool IsAnimationCompleted()
    {
        return _animationCompleted;
    }
}
