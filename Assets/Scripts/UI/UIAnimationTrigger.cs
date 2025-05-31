using UnityEngine;

/// <summary>
/// UI动画触发器
/// 自动触发UI动画控制器（兼容UIAnimationController和SimpleTweenAnimation）
/// 解决UI动画可能没有被正确触发的问题
/// </summary>
public class UIAnimationTrigger : MonoBehaviour
{
    [Tooltip("触发延迟(秒)")]
    [Range(0.1f, 5.0f)]
    public float triggerDelay = 1.0f;
    
    [Tooltip("是否在Start时自动触发")]
    public bool triggerOnStart = true;
    
    [Tooltip("是否在Canvas激活时自动触发")]
    public bool triggerOnEnable = true;
    
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = true;
    
    // 支持两种动画控制器
    private UIAnimationController _uiAnimator;
    private SimpleTweenAnimation _tweenAnimator;
    private bool _hasTriggered = false;
    
    private void Awake()
    {
        // 首先尝试获取SimpleTweenAnimation组件（优先使用新控制器）
        _tweenAnimator = GetComponent<SimpleTweenAnimation>();
        if (_tweenAnimator == null)
        {
            // 尝试在父对象上查找
            _tweenAnimator = GetComponentInParent<SimpleTweenAnimation>();
        }
        
        // 如果没有找到SimpleTweenAnimation，则尝试获取UIAnimationController组件
        if (_tweenAnimator == null)
        {
            _uiAnimator = GetComponent<UIAnimationController>();
            if (_uiAnimator == null)
            {
                // 尝试在父对象上查找
                _uiAnimator = GetComponentInParent<UIAnimationController>();
            }
        }
        
        // 检查是否找到任一动画控制器
        if (_tweenAnimator == null && _uiAnimator == null)
        {
            Debug.LogWarning("UIAnimationTrigger: 未找到SimpleTweenAnimation或UIAnimationController组件。");
            enabled = false;
            return;
        }
        
        if (enableDebugLogs)
        {
            if (_tweenAnimator != null)
            {
                Debug.Log("UIAnimationTrigger: 已找到SimpleTweenAnimation组件。");
            }
            else
            {
                Debug.Log("UIAnimationTrigger: 已找到UIAnimationController组件。");
            }
        }
    }
    
    private void Start()
    {
        if (triggerOnStart && !_hasTriggered)
        {
            // 延迟触发，确保UI元素已准备就绪
            Invoke("TriggerAnimation", triggerDelay);
        }
    }
    
    private void OnEnable()
    {
        if (triggerOnEnable && !_hasTriggered)
        {
            // 延迟触发，确保UI元素已准备就绪
            Invoke("TriggerAnimation", triggerDelay);
        }
    }
    
    /// <summary>
    /// 触发UI动画
    /// </summary>
    public void TriggerAnimation()
    {
        if (!_hasTriggered)
        {
            _hasTriggered = true;
            
            if (enableDebugLogs)
            {
                Debug.Log("UIAnimationTrigger: 触发UI动画");
            }
            
            // 优先使用SimpleTweenAnimation
            if (_tweenAnimator != null)
            {
                _tweenAnimator.PlayAnimation();
                
                if (enableDebugLogs)
                {
                    Debug.Log("UIAnimationTrigger: 已调用SimpleTweenAnimation.PlayAnimation()");
                }
            }
            // 如果没有SimpleTweenAnimation，则使用UIAnimationController
            else if (_uiAnimator != null)
            {
                _uiAnimator.StartUIAnimation();
                
                if (enableDebugLogs)
                {
                    Debug.Log("UIAnimationTrigger: 已调用UIAnimationController.StartUIAnimation()");
                }
            }
        }
    }
    
    /// <summary>
    /// 重置触发器状态
    /// </summary>
    public void ResetTrigger()
    {
        _hasTriggered = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationTrigger: 重置触发器状态");
        }
    }
}
