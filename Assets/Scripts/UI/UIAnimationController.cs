using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI动画控制器
/// 专门处理UI元素的动画效果，不依赖于Canvas切换逻辑
/// </summary>
public class UIAnimationController : MonoBehaviour
{
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

    [Header("调试设置")]
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = true;
    
    // 是否已经准备好执行动画
    private bool _isReady = false;

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
        if (titleLogo == null || bottomDecoration == null || buttonsGroup == null)
        {
            Debug.LogError("UIAnimationController: 缺少必要的UI引用，请在Inspector中设置。");
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
        
        // 标记为已准备好
        _isReady = true;
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationController: 初始化完成，UI动画控制器已准备就绪");
        }
    }

    /// <summary>
    /// 初始化时设置UI元素位置
    /// </summary>
    private void Start()
    {
        // 如果已经准备好，将UI元素移动到屏幕外
        if (_isReady)
        {
            MoveUIElementsOffscreen();
            
            if (enableDebugLogs)
            {
                Debug.Log("UIAnimationController: Start中自动将UI元素移动到屏幕外");
            }
        }
        else
        {
            // 如果还没准备好，延迟一帧执行
            StartCoroutine(DelayedMoveUIElementsOffscreen());
        }
        
        // 如果设置了禁用按钮交互，则禁用所有按钮
        if (disableButtonsUntilAnimationComplete && _buttons != null)
        {
            foreach (Button button in _buttons)
            {
                button.interactable = false;
            }
        }
    }
    
    /// <summary>
    /// 延迟一帧将UI元素移动到屏幕外
    /// </summary>
    private IEnumerator DelayedMoveUIElementsOffscreen()
    {
        yield return null;
        
        if (_isReady)
        {
            MoveUIElementsOffscreen();
            
            if (enableDebugLogs)
            {
                Debug.Log("UIAnimationController: 延迟一帧后将UI元素移动到屏幕外");
            }
        }
        else
        {
            Debug.LogWarning("UIAnimationController: 组件仍未初始化完成，无法移动UI元素");
        }
    }

    /// <summary>
    /// 将UI元素移动到屏幕外
    /// </summary>
    public void MoveUIElementsOffscreen()
    {
        if (!_isReady)
        {
            Debug.LogWarning("UIAnimationController: 组件尚未初始化完成，无法移动UI元素");
            return;
        }
        
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
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationController: 已将UI元素移动到屏幕外");
        }
    }

    /// <summary>
    /// 开始UI动画
    /// </summary>
    public void StartUIAnimation()
    {
        if (!_isReady)
        {
            Debug.LogWarning("UIAnimationController: 组件尚未初始化完成，无法开始UI动画");
            return;
        }
        
        // 如果已经开始动画，则忽略
        if (_animationStarted)
            return;
            
        _animationStarted = true;
        _animationCompleted = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationController: 开始UI动画");
        }
        
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
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationController: 所有UI动画已完成，按钮已启用交互。");
        }
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
        if (!_isReady)
        {
            Debug.LogWarning("UIAnimationController: 组件尚未初始化完成，无法重置UI元素");
            return;
        }
        
        // 停止所有协程
        StopAllCoroutines();
        
        // 重置动画状态
        _animationStarted = false;
        _animationCompleted = false;
        
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
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationController: 已重置UI元素到初始状态");
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
