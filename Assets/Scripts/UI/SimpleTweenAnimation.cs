using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// 简单补间动画控制器
/// 使用硬编码的方式控制UI元素动画，确保可靠性
/// </summary>
public class SimpleTweenAnimation : MonoBehaviour
{
    public enum AnimationDirection
    {
        FromTop,
        FromBottom,
        FromLeft,
        FromRight,
        Custom // 允许自定义方向
    }

    [System.Serializable]
    public class UIElementAnimation
    {
        public RectTransform targetElement;
        public AnimationDirection direction = AnimationDirection.FromRight;
        public Vector2 finalPosition;
        public float delay = 0f;
        public float duration = 0.5f;
        [HideInInspector] public Vector2 startPosition;
        // 自定义方向的起始位置偏移
        public Vector2 customOffset = Vector2.zero;
    }

    [Header("动画元素")]
    public List<UIElementAnimation> animatedElements = new List<UIElementAnimation>();

    [Header("动画设置")]
    [Tooltip("动画曲线")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("是否在启用时自动播放动画")]
    public bool playOnEnable = true;

    [Tooltip("是否在启用时重置元素位置")]
    public bool resetPositionOnEnable = true;
    
    [Tooltip("屏幕外偏移距离(相对于屏幕尺寸)")]
    [Range(0.5f, 2.0f)]
    public float offscreenOffset = 1.2f;
    
    [Tooltip("动画完成后冷却时间(秒)，防止短时间内重复触发")]
    [Range(0.5f, 10.0f)]
    public float animationCooldown = 3.0f;

    [Header("Begin_Canvas联动设置")]
    [Tooltip("Begin_Canvas对象引用")]
    public GameObject beginCanvas;
    
    [Tooltip("是否监听Begin_Canvas禁用事件")]
    public bool triggerOnBeginCanvasDisabled = true;
    
    [Tooltip("检查Begin_Canvas状态的间隔(秒)")]
    [Range(0.05f, 0.5f)]
    public float checkInterval = 0.1f;

    [Header("按钮交互设置")]
    [Tooltip("动画期间是否禁用按钮交互")]
    public bool disableButtonsUntilAnimationComplete = true;

    [Header("调试设置")]
    [Tooltip("是否启用详细日志")]
    public bool enableDetailedLogs = true;

    private Dictionary<RectTransform, Button> _buttons = new Dictionary<RectTransform, Button>();
    private bool _isAnimating = false;
    private float _canvasWidth;
    private float _canvasHeight;
    private float _lastAnimationTime = -999f;
    private bool _inCooldown = false;
    private bool _beginCanvasWasActive = false;
    private bool _isMonitoringBeginCanvas = false;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Awake()
    {
        // 检查并初始化动画元素
        if (animatedElements.Count == 0)
        {
            Debug.LogWarning("SimpleTweenAnimation: 未设置任何动画元素。");
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

        // 记录所有元素的最终位置
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null)
            {
                Debug.LogError("SimpleTweenAnimation: 存在未设置目标元素的动画元素。");
                continue;
            }

            element.finalPosition = element.targetElement.anchoredPosition;

            // 查找并记录按钮组件
            Button button = element.targetElement.GetComponent<Button>();
            if (button != null)
            {
                _buttons[element.targetElement] = button;
            }
        }

        if (enableDetailedLogs)
        {
            Debug.Log($"SimpleTweenAnimation: 初始化完成，Canvas尺寸：{_canvasWidth}x{_canvasHeight}，动画元素数量：{animatedElements.Count}");
        }
    }

    /// <summary>
    /// 组件启用时
    /// </summary>
    private void OnEnable()
    {
        if (resetPositionOnEnable)
        {
            ResetElementPositions();
        }

        if (playOnEnable)
        {
            PlayAnimation();
        }
        
        // 开始监听Begin_Canvas状态
        if (triggerOnBeginCanvasDisabled && beginCanvas != null && !_isMonitoringBeginCanvas)
        {
            _beginCanvasWasActive = beginCanvas.activeSelf;
            StartCoroutine(MonitorBeginCanvasStatus());
            
            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 开始监听Begin_Canvas状态，当前状态：{(_beginCanvasWasActive ? "激活" : "禁用")}");
            }
        }
        else if (triggerOnBeginCanvasDisabled && beginCanvas == null)
        {
            // 如果未设置Begin_Canvas引用，尝试查找
            beginCanvas = GameObject.Find("Begin_Canvas");
            if (beginCanvas != null)
            {
                _beginCanvasWasActive = beginCanvas.activeSelf;
                StartCoroutine(MonitorBeginCanvasStatus());
                
                if (enableDetailedLogs)
                {
                    Debug.Log($"SimpleTweenAnimation: 已自动找到并开始监听Begin_Canvas状态，当前状态：{(_beginCanvasWasActive ? "激活" : "禁用")}");
                }
            }
            else
            {
                Debug.LogWarning("SimpleTweenAnimation: 未找到Begin_Canvas对象，无法监听状态变化。");
            }
        }
    }

    /// <summary>
    /// 组件禁用时
    /// </summary>
    private void OnDisable()
    {
        // 停止监听Begin_Canvas状态
        _isMonitoringBeginCanvas = false;
        StopCoroutine(MonitorBeginCanvasStatus());
    }
    
    /// <summary>
    /// 监听Begin_Canvas状态变化
    /// </summary>
    private IEnumerator MonitorBeginCanvasStatus()
    {
        _isMonitoringBeginCanvas = true;
        
        while (_isMonitoringBeginCanvas && beginCanvas != null)
        {
            // 检查Begin_Canvas状态是否从激活变为禁用
            bool currentActive = beginCanvas.activeSelf;
            
            if (_beginCanvasWasActive && !currentActive)
            {
                if (enableDetailedLogs)
                {
                    Debug.Log("SimpleTweenAnimation: 检测到Begin_Canvas已被禁用，触发UI动画");
                }
                
                // Begin_Canvas从激活变为禁用，触发动画
                PlayAnimation();
            }
            
            // 更新状态
            _beginCanvasWasActive = currentActive;
            
            // 等待下一次检查
            yield return new WaitForSeconds(checkInterval);
        }
        
        _isMonitoringBeginCanvas = false;
    }

    /// <summary>
    /// 将所有元素移到屏幕外作为起始位置
    /// </summary>
    public void ResetElementPositions()
    {
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;

            // 根据方向设置屏幕外起始位置
            Vector2 offscreenPosition = element.finalPosition;
            
            switch (element.direction)
            {
                case AnimationDirection.FromTop:
                    offscreenPosition.y = element.finalPosition.y + _canvasHeight * offscreenOffset;
                    break;
                case AnimationDirection.FromBottom:
                    offscreenPosition.y = element.finalPosition.y - _canvasHeight * offscreenOffset;
                    break;
                case AnimationDirection.FromLeft:
                    offscreenPosition.x = element.finalPosition.x - _canvasWidth * offscreenOffset;
                    break;
                case AnimationDirection.FromRight:
                    offscreenPosition.x = element.finalPosition.x + _canvasWidth * offscreenOffset;
                    break;
                case AnimationDirection.Custom:
                    offscreenPosition = element.finalPosition + element.customOffset;
                    break;
            }
            
            // 记录并设置屏幕外位置
            element.startPosition = offscreenPosition;
            element.targetElement.anchoredPosition = offscreenPosition;

            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 将元素 {element.targetElement.name} 设置到屏幕外位置 {offscreenPosition}，最终位置将是 {element.finalPosition}");
            }
        }

        // 如果设置了禁用按钮交互，则禁用所有按钮
        if (disableButtonsUntilAnimationComplete)
        {
            foreach (var button in _buttons.Values)
            {
                button.interactable = false;
            }
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        // 检查是否在冷却期
        if (_inCooldown)
        {
            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 动画处于冷却期中，忽略此次播放请求 (冷却时间: {animationCooldown}秒)");
            }
            return;
        }
        
        // 检查是否最近播放过动画
        float timeSinceLastAnimation = Time.time - _lastAnimationTime;
        if (_lastAnimationTime > 0 && timeSinceLastAnimation < animationCooldown)
        {
            _inCooldown = true;
            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 动画刚播放过 {timeSinceLastAnimation:F1}秒，进入冷却期 (冷却时间: {animationCooldown}秒)");
            }
            StartCoroutine(EndCooldown());
            return;
        }
        
        // 先检查元素是否都已经在正确位置，如果是，不需要再次播放动画
        bool allElementsInPlace = true;
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;
            
            if (Vector2.Distance(element.targetElement.anchoredPosition, element.finalPosition) > 0.1f)
            {
                allElementsInPlace = false;
                break;
            }
        }
        
        if (allElementsInPlace)
        {
            if (enableDetailedLogs)
            {
                Debug.Log("SimpleTweenAnimation: 所有元素已在最终位置，无需播放动画。");
            }
            return;
        }
        
        // 如果已经在播放动画，忽略此次请求
        if (_isAnimating)
        {
            if (enableDetailedLogs)
            {
                Debug.Log("SimpleTweenAnimation: 检测到重复播放请求，但当前已有动画在播放，忽略此次请求。");
            }
            return;
        }
        
        // 记录动画开始时间
        _lastAnimationTime = Time.time;

        // 记录所有元素的当前位置作为起始位置
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;
            
            // 记录当前位置作为起始位置（动画已经开始，这里只是记录当前位置，不改变位置）
            element.startPosition = element.targetElement.anchoredPosition;
            
            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 记录元素 {element.targetElement.name} 当前位置 {element.startPosition} 作为起始位置");
            }
        }

        // 开始动画
        _isAnimating = true;
        Debug.Log("SimpleTweenAnimation: 开始播放UI动画");

        // 为每个元素启动动画协程
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;
            StartCoroutine(AnimateElement(element));
        }
    }

    /// <summary>
    /// 为单个元素播放动画
    /// </summary>
    private IEnumerator AnimateElement(UIElementAnimation element)
    {
        // 等待延迟
        if (element.delay > 0)
        {
            yield return new WaitForSeconds(element.delay);
        }

        // 动画起始和目标位置
        Vector2 startPosition = element.startPosition;
        Vector2 endPosition = element.finalPosition;

        if (enableDetailedLogs)
        {
            Debug.Log($"SimpleTweenAnimation: 开始元素 {element.targetElement.name} 动画，从 {startPosition} 到 {endPosition}");
        }

        // 播放动画
        float elapsed = 0;
        while (elapsed < element.duration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / element.duration);
            float curvedTime = animationCurve.Evaluate(normalizedTime);

            Vector2 newPosition = Vector2.Lerp(startPosition, endPosition, curvedTime);
            element.targetElement.anchoredPosition = newPosition;

            if (enableDetailedLogs && Time.frameCount % 30 == 0) // 每30帧记录一次位置，避免日志过多
            {
                Debug.Log($"SimpleTweenAnimation: 元素 {element.targetElement.name} 动画进度 {normalizedTime:F2}，当前位置 {newPosition}");
            }

            yield return null;
        }

        // 确保最终位置精确
        element.targetElement.anchoredPosition = endPosition;

        if (enableDetailedLogs)
        {
            Debug.Log($"SimpleTweenAnimation: 元素 {element.targetElement.name} 动画完成，最终位置 {endPosition}");
        }

        // 检查是否所有动画都完成了
        bool allCompleted = true;
        foreach (var anim in animatedElements)
        {
            if (anim.targetElement == null) continue;
            if (Vector2.Distance(anim.targetElement.anchoredPosition, anim.finalPosition) > 0.1f)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            _isAnimating = false;

            // 启用所有按钮交互
            if (disableButtonsUntilAnimationComplete)
            {
                foreach (var button in _buttons.Values)
                {
                    button.interactable = true;
                }
            }

            Debug.Log("SimpleTweenAnimation: 所有UI动画播放完成");
        }
    }

    /// <summary>
    /// 立即完成动画
    /// </summary>
    public void CompleteAnimation()
    {
        StopAllCoroutines();
        _isAnimating = false;

        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;
            element.targetElement.anchoredPosition = element.finalPosition;
        }

        // 启用所有按钮交互
        if (disableButtonsUntilAnimationComplete)
        {
            foreach (var button in _buttons.Values)
            {
                button.interactable = true;
            }
        }

        Debug.Log("SimpleTweenAnimation: 动画已立即完成");
    }

    /// <summary>
    /// 动画是否正在播放
    /// </summary>
    public bool IsAnimating()
    {
        return _isAnimating;
    }
    
    /// <summary>
    /// 结束动画冷却期
    /// </summary>
    private IEnumerator EndCooldown()
    {
        // 等待冷却时间
        yield return new WaitForSeconds(animationCooldown);
        
        // 重置冷却状态
        _inCooldown = false;
        
        if (enableDetailedLogs)
        {
            Debug.Log("SimpleTweenAnimation: 动画冷却期结束，可以再次触发动画");
        }
    }
}
