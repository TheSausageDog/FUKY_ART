using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Start_Canvas UI动画控制器
/// 在Begin_Canvas被禁用后，自动启用Start_Canvas并控制UI元素的进场动画
/// </summary>
public class StartCanvasUIAnimation : MonoBehaviour
{
    [Header("Canvas引用")]
    [Tooltip("Begin_Canvas对象引用")]
    public GameObject beginCanvas;

    [Header("UI元素引用")]
    [Tooltip("标题团案")]
    public RectTransform titleLogo;

    [Tooltip("底部装饰")]
    public RectTransform bottomDecoration;

    [Tooltip("开始游戏按钮")]
    public RectTransform startGameButton;

    [Tooltip("设置按钮")]
    public RectTransform settingsButton;

    [Tooltip("制作组按钮")]
    public RectTransform creditsButton;

    [Tooltip("退出游戏按钮")]
    public RectTransform exitButton;

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

    [Tooltip("是否在启用Canvas时自动播放动画")]
    public bool playOnEnable = true;

    [Header("按钮交互设置")]
    [Tooltip("是否在动画完成前禁用按钮交互")]
    public bool disableButtonsUntilAnimationComplete = true;

    [Header("监听设置")]
    [Tooltip("是否监听Begin_Canvas禁用事件")]
    public bool monitorBeginCanvas = true;

    [Tooltip("检测间隔(秒)")]
    [Range(0.05f, 0.5f)]
    public float checkInterval = 0.1f;

    [Header("调试设置")]
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = true;

    // 私有变量
    private Vector2 _titleOriginalPosition;
    private Vector2 _bottomOriginalPosition;
    private Vector2 _startGameOriginalPosition;
    private Vector2 _settingsOriginalPosition;
    private Vector2 _creditsOriginalPosition;
    private Vector2 _exitOriginalPosition;
    private float _canvasWidth;
    private float _canvasHeight;
    private bool _animationStarted = false;
    private bool _animationCompleted = false;
    private Dictionary<RectTransform, Button> _buttons = new Dictionary<RectTransform, Button>();
    private bool _beginCanvasWasActive = false;
    private bool _isMonitoring = false;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        // 检查必要的引用
        if (titleLogo == null || bottomDecoration == null || 
            startGameButton == null || settingsButton == null || 
            creditsButton == null || exitButton == null)
        {
            Debug.LogError("StartCanvasUIAnimation: 缺少必要的UI元素引用，请在Inspector中设置。");
            enabled = false;
            return;
        }

        // 获取Canvas尺寸
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
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
        }
        else
        {
            _canvasWidth = Screen.width;
            _canvasHeight = Screen.height;
        }

        // 保存所有UI元素的原始位置
        _titleOriginalPosition = titleLogo.anchoredPosition;
        _bottomOriginalPosition = bottomDecoration.anchoredPosition;
        _startGameOriginalPosition = startGameButton.anchoredPosition;
        _settingsOriginalPosition = settingsButton.anchoredPosition;
        _creditsOriginalPosition = creditsButton.anchoredPosition;
        _exitOriginalPosition = exitButton.anchoredPosition;

        // 获取并保存所有按钮组件
        CollectButtonComponents();

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 初始化完成，Canvas尺寸：{_canvasWidth}x{_canvasHeight}");
        }
    }

    /// <summary>
    /// 收集所有按钮组件
    /// </summary>
    private void CollectButtonComponents()
    {
        // 收集所有按钮引用
        Button button;

        // 开始游戏按钮
        button = startGameButton.GetComponent<Button>();
        if (button != null) _buttons[startGameButton] = button;

        // 设置按钮
        button = settingsButton.GetComponent<Button>();
        if (button != null) _buttons[settingsButton] = button;

        // 制作组按钮
        button = creditsButton.GetComponent<Button>();
        if (button != null) _buttons[creditsButton] = button;

        // 退出游戏按钮
        button = exitButton.GetComponent<Button>();
        if (button != null) _buttons[exitButton] = button;

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 已收集 {_buttons.Count} 个按钮组件");
        }
    }

    /// <summary>
    /// 组件启用时
    /// </summary>
    private void OnEnable()
    {
        // 重置动画状态
        _animationStarted = false;
        _animationCompleted = false;
        
        // 初始化时将UI元素移动到屏幕外
        ResetElementPositions();

        // 禁用按钮交互（如果设置）
        if (disableButtonsUntilAnimationComplete)
        {
            SetButtonsInteractable(false);
        }

        // 如果设置了自动播放，开始动画
        if (playOnEnable)
        {
            PlayAnimation();
        }

        // 开始监听Begin_Canvas状态
        if (monitorBeginCanvas && !_isMonitoring)
        {
            // 如果未设置Begin_Canvas引用，尝试查找
            if (beginCanvas == null)
            {
                beginCanvas = GameObject.Find("Begin_Canvas");
                if (beginCanvas == null)
                {
                    Debug.LogWarning("StartCanvasUIAnimation: 未找到Begin_Canvas对象，无法监听状态变化。");
                }
            }

            if (beginCanvas != null)
            {
                _beginCanvasWasActive = beginCanvas.activeSelf;
                StartCoroutine(MonitorBeginCanvasStatus());

                if (enableDebugLogs)
                {
                    Debug.Log($"StartCanvasUIAnimation: 开始监听Begin_Canvas状态，当前状态：{(_beginCanvasWasActive ? "激活" : "禁用")}");
                }
            }
        }
    }

    /// <summary>
    /// 组件禁用时
    /// </summary>
    private void OnDisable()
    {
        // 停止所有协程
        StopAllCoroutines();
        _isMonitoring = false;
    }

    /// <summary>
    /// 监听Begin_Canvas状态变化
    /// </summary>
    private IEnumerator MonitorBeginCanvasStatus()
    {
        _isMonitoring = true;

        while (_isMonitoring && beginCanvas != null)
        {
            // 检查Begin_Canvas状态是否从激活变为禁用
            bool currentActive = beginCanvas.activeSelf;

            if (_beginCanvasWasActive && !currentActive)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("StartCanvasUIAnimation: 检测到Begin_Canvas已被禁用，触发UI动画");
                }

                // 确保Start_Canvas已启用
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log("StartCanvasUIAnimation: Start_Canvas已启用");
                    }
                }

                // Begin_Canvas从激活变为禁用，触发动画
                TriggerUIAnimation();
            }

            // 更新状态
            _beginCanvasWasActive = currentActive;

            // 等待下一次检查
            yield return new WaitForSeconds(checkInterval);
        }

        _isMonitoring = false;
    }

    /// <summary>
    /// 将所有UI元素移动到屏幕外作为起始位置
    /// </summary>
    public void ResetElementPositions()
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

        // 将按钮移动到屏幕左侧
        startGameButton.anchoredPosition = new Vector2(
            _startGameOriginalPosition.x - _canvasWidth * offscreenOffset,
            _startGameOriginalPosition.y
        );

        settingsButton.anchoredPosition = new Vector2(
            _settingsOriginalPosition.x - _canvasWidth * offscreenOffset,
            _settingsOriginalPosition.y
        );

        creditsButton.anchoredPosition = new Vector2(
            _creditsOriginalPosition.x - _canvasWidth * offscreenOffset,
            _creditsOriginalPosition.y
        );

        exitButton.anchoredPosition = new Vector2(
            _exitOriginalPosition.x - _canvasWidth * offscreenOffset,
            _exitOriginalPosition.y
        );

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 已将所有UI元素重置到屏幕外起始位置");
        }
    }

    /// <summary>
    /// 设置所有按钮的交互状态
    /// </summary>
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in _buttons.Values)
        {
            button.interactable = interactable;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 已将所有按钮交互状态设置为 {interactable}");
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        // 如果已经在播放动画，忽略
        if (_animationStarted)
        {
            if (enableDebugLogs)
            {
                Debug.Log("StartCanvasUIAnimation: 动画已经在播放中，忽略此次请求");
            }
            return;
        }

        _animationStarted = true;
        _animationCompleted = false;

        // 开始各元素的动画
        StartCoroutine(AnimateTitleLogo());
        StartCoroutine(AnimateBottomDecoration());
        StartCoroutine(AnimateButtons());

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 开始播放UI动画");
        }
    }

    /// <summary>
    /// 标题动画
    /// </summary>
    private IEnumerator AnimateTitleLogo()
    {
        // 获取初始位置和目标位置
        Vector2 startPosition = titleLogo.anchoredPosition;
        Vector2 targetPosition = _titleOriginalPosition;

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 标题动画开始，从 {startPosition} 到 {targetPosition}");
        }

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

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 标题动画完成，最终位置 {titleLogo.anchoredPosition}");
        }
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

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 底部装饰动画开始，从 {startPosition} 到 {targetPosition}");
        }

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

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 底部装饰动画完成，最终位置 {bottomDecoration.anchoredPosition}");
        }
    }

    /// <summary>
    /// 按钮动画
    /// </summary>
    private IEnumerator AnimateButtons()
    {
        // 等待更长一点的时间再开始按钮动画
        yield return new WaitForSeconds(elementDelay * 2);

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 开始按钮组动画");
        }

        // 创建所有按钮的动画
        StartCoroutine(AnimateButton(startGameButton, _startGameOriginalPosition, 0));
        StartCoroutine(AnimateButton(settingsButton, _settingsOriginalPosition, 1));
        StartCoroutine(AnimateButton(creditsButton, _creditsOriginalPosition, 2));
        StartCoroutine(AnimateButton(exitButton, _exitOriginalPosition, 3));

        // 计算所有动画完成的总时间
        float totalAnimationTime = animationDuration + (4 * elementDelay * 0.5f);

        // 等待所有动画完成
        yield return new WaitForSeconds(totalAnimationTime);

        // 标记动画已完成
        _animationCompleted = true;

        // 如果设置了禁用按钮交互，则启用所有按钮
        if (disableButtonsUntilAnimationComplete)
        {
            SetButtonsInteractable(true);
        }

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 所有UI动画已完成，按钮已启用交互");
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

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 按钮 {button.name} 动画开始，从 {startPosition} 到 {targetPosition}");
        }

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

        if (enableDebugLogs)
        {
            Debug.Log($"StartCanvasUIAnimation: 按钮 {button.name} 动画完成，最终位置 {button.anchoredPosition}");
        }
    }

    /// <summary>
    /// 立即完成动画
    /// </summary>
    public void CompleteAnimation()
    {
        StopAllCoroutines();

        // 将所有元素立即移动到最终位置
        titleLogo.anchoredPosition = _titleOriginalPosition;
        bottomDecoration.anchoredPosition = _bottomOriginalPosition;
        startGameButton.anchoredPosition = _startGameOriginalPosition;
        settingsButton.anchoredPosition = _settingsOriginalPosition;
        creditsButton.anchoredPosition = _creditsOriginalPosition;
        exitButton.anchoredPosition = _exitOriginalPosition;

        // 标记动画已完成
        _animationStarted = true;
        _animationCompleted = true;

        // 如果设置了禁用按钮交互，则启用所有按钮
        if (disableButtonsUntilAnimationComplete)
        {
            SetButtonsInteractable(true);
        }

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 动画已立即完成");
        }
    }

    /// <summary>
    /// 手动触发UI动画（用于外部调用）
    /// </summary>
    public void TriggerUIAnimation()
    {
        // 停止所有正在运行的协程
        StopAllCoroutines();
        
        // 重置动画状态
        _animationStarted = false;
        _animationCompleted = false;
        
        // 重置UI元素位置
        ResetElementPositions();

        // 禁用按钮交互（如果设置）
        if (disableButtonsUntilAnimationComplete)
        {
            SetButtonsInteractable(false);
        }

        // 播放动画
        PlayAnimation();

        if (enableDebugLogs)
        {
            Debug.Log("StartCanvasUIAnimation: 已手动触发UI动画");
        }
    }

    /// <summary>
    /// 检查动画是否已完成
    /// </summary>
    public bool IsAnimationCompleted()
    {
        return _animationCompleted;
    }
}
