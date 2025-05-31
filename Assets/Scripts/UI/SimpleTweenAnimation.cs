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
    [System.Serializable]
    public class UIElementAnimation
    {
        public RectTransform targetElement;
        public Vector2 finalPosition;
        public AnimationDirection direction = AnimationDirection.FromTop;
        public float delay = 0f;
        public float duration = 0.5f;
        [HideInInspector] public Vector2 startPosition;
    }

    public enum AnimationDirection
    {
        FromTop,
        FromBottom,
        FromLeft,
        FromRight
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
    [Range(0.5f, 3.0f)]
    public float offscreenOffset = 1.5f;

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
    }

    /// <summary>
    /// 重置所有元素位置到屏幕外
    /// </summary>
    public void ResetElementPositions()
    {
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;

            // 根据方向计算屏幕外位置
            Vector2 offscreenPosition = CalculateOffscreenPosition(element.direction, element.finalPosition);
            element.startPosition = offscreenPosition;
            element.targetElement.anchoredPosition = offscreenPosition;

            if (enableDetailedLogs)
            {
                Debug.Log($"SimpleTweenAnimation: 重置元素 {element.targetElement.name} 位置至 {offscreenPosition}，最终位置将是 {element.finalPosition}");
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
    /// 计算屏幕外位置
    /// </summary>
    private Vector2 CalculateOffscreenPosition(AnimationDirection direction, Vector2 finalPosition)
    {
        switch (direction)
        {
            case AnimationDirection.FromTop:
                return new Vector2(finalPosition.x, finalPosition.y + _canvasHeight * offscreenOffset);
            case AnimationDirection.FromBottom:
                return new Vector2(finalPosition.x, finalPosition.y - _canvasHeight * offscreenOffset);
            case AnimationDirection.FromLeft:
                return new Vector2(finalPosition.x - _canvasWidth * offscreenOffset, finalPosition.y);
            case AnimationDirection.FromRight:
                return new Vector2(finalPosition.x + _canvasWidth * offscreenOffset, finalPosition.y);
            default:
                return finalPosition;
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        if (_isAnimating)
        {
            Debug.LogWarning("SimpleTweenAnimation: 动画已经在播放中，忽略此次播放请求。");
            return;
        }

        // 确保所有元素都有有效的起始位置
        foreach (var element in animatedElements)
        {
            if (element.targetElement == null) continue;

            if (Vector2.Distance(element.targetElement.anchoredPosition, element.finalPosition) < 1f)
            {
                // 如果当前位置接近最终位置，重新设置到屏幕外
                Vector2 offscreenPosition = CalculateOffscreenPosition(element.direction, element.finalPosition);
                element.startPosition = offscreenPosition;
                element.targetElement.anchoredPosition = offscreenPosition;

                if (enableDetailedLogs)
                {
                    Debug.Log($"SimpleTweenAnimation: 播放前检测到元素 {element.targetElement.name} 位置接近最终位置，已重置至 {offscreenPosition}");
                }
            }
            else
            {
                // 记录当前位置作为起始位置
                element.startPosition = element.targetElement.anchoredPosition;
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
}
