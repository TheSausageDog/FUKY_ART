using System.Collections;
using UnityEngine;

/// <summary>
/// 主菜单过渡控制器
/// 处理点击任意键后主菜单UI元素的动画过渡效果
/// </summary>
public class MenuTransitionController : MonoBehaviour
{
    [Header("UI引用")]
    [Tooltip("主菜单标志UI")]
    public RectTransform menuLogo;

    [Tooltip("点击任意键开始文字UI")]
    public GameObject startPrompt;

    [Tooltip("主菜单下划装饰UI")]
    public RectTransform menuDecoration;

    [Header("动画设置")]
    [Tooltip("动画持续时间(秒)")]
    [Range(0.5f, 3.0f)]
    public float animationDuration = 1.0f;

    [Tooltip("动画曲线")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("标志向上移动距离(相对于屏幕高度)")]
    [Range(1.0f, 3.0f)]
    public float logoMoveDistance = 1.5f;

    [Tooltip("装饰向下移动距离(相对于屏幕高度)")]
    [Range(1.0f, 3.0f)]
    public float decorationMoveDistance = 1.5f;

    // 私有变量
    private bool _transitionTriggered = false;
    private Vector2 _logoInitialPosition;
    private Vector2 _decorationInitialPosition;
    private float _screenHeight;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Start()
    {
        // 检查必要的引用
        if (menuLogo == null || startPrompt == null || menuDecoration == null)
        {
            Debug.LogError("MenuTransitionController: 缺少必要的UI引用，请在Inspector中设置。");
            enabled = false;
            return;
        }

        // 保存初始位置
        _logoInitialPosition = menuLogo.anchoredPosition;
        _decorationInitialPosition = menuDecoration.anchoredPosition;
        
        // 获取屏幕高度
        _screenHeight = Screen.height;
    }

    /// <summary>
    /// 每帧检测按键输入
    /// </summary>
    private void Update()
    {
        // 如果已经触发过渡，则不再检测按键
        if (_transitionTriggered)
            return;

        // 检测任意按键输入
        if (Input.anyKeyDown)
        {
            // 触发过渡动画
            StartTransition();
        }
    }

    /// <summary>
    /// 开始过渡动画
    /// </summary>
    public void StartTransition()
    {
        // 标记已触发过渡
        _transitionTriggered = true;
        
        // 立即禁用开始提示文字
        if (startPrompt != null)
        {
            startPrompt.SetActive(false);
        }
        
        // 开始标志和装饰的动画协程
        StartCoroutine(AnimateLogoUp());
        StartCoroutine(AnimateDecorationDown());
    }

    /// <summary>
    /// 标志向上飞出动画
    /// </summary>
    private IEnumerator AnimateLogoUp()
    {
        // 计算目标位置（向上移动屏幕高度的logoMoveDistance倍）
        Vector2 targetPosition = _logoInitialPosition + new Vector2(0, _screenHeight * logoMoveDistance);
        
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
            Vector2 currentPosition = Vector2.Lerp(_logoInitialPosition, targetPosition, curvedProgress);
            
            // 应用位置
            menuLogo.anchoredPosition = currentPosition;
            
            // 等待下一帧
            yield return null;
        }
        
        // 确保最终位置精确
        menuLogo.anchoredPosition = targetPosition;
        
        // 动画完成后禁用对象
        menuLogo.gameObject.SetActive(false);
    }

    /// <summary>
    /// 装饰向下飞出动画
    /// </summary>
    private IEnumerator AnimateDecorationDown()
    {
        // 计算目标位置（向下移动屏幕高度的decorationMoveDistance倍）
        Vector2 targetPosition = _decorationInitialPosition - new Vector2(0, _screenHeight * decorationMoveDistance);
        
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
            Vector2 currentPosition = Vector2.Lerp(_decorationInitialPosition, targetPosition, curvedProgress);
            
            // 应用位置
            menuDecoration.anchoredPosition = currentPosition;
            
            // 等待下一帧
            yield return null;
        }
        
        // 确保最终位置精确
        menuDecoration.anchoredPosition = targetPosition;
        
        // 动画完成后禁用对象
        menuDecoration.gameObject.SetActive(false);
    }

    /// <summary>
    /// 重置所有UI元素到初始状态
    /// </summary>
    public void ResetMenu()
    {
        // 重置标记
        _transitionTriggered = false;
        
        // 重置位置
        if (menuLogo != null)
        {
            menuLogo.anchoredPosition = _logoInitialPosition;
            menuLogo.gameObject.SetActive(true);
        }
        
        if (menuDecoration != null)
        {
            menuDecoration.anchoredPosition = _decorationInitialPosition;
            menuDecoration.gameObject.SetActive(true);
        }
        
        if (startPrompt != null)
        {
            startPrompt.SetActive(true);
        }
    }
}
