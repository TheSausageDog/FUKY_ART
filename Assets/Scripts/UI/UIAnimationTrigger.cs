using UnityEngine;

/// <summary>
/// UI动画触发器
/// 自动触发UIAnimationController的StartUIAnimation方法
/// 解决UIAnimationController动画可能没有被正确触发的问题
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
    
    private UIAnimationController _uiAnimator;
    private bool _hasTriggered = false;
    
    private void Awake()
    {
        // 获取UIAnimationController组件
        _uiAnimator = GetComponent<UIAnimationController>();
        if (_uiAnimator == null)
        {
            // 尝试在父对象上查找
            _uiAnimator = GetComponentInParent<UIAnimationController>();
            
            if (_uiAnimator == null)
            {
                Debug.LogWarning("UIAnimationTrigger: 未找到UIAnimationController组件。");
                enabled = false;
                return;
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log("UIAnimationTrigger: 已找到UIAnimationController组件。");
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
        if (_uiAnimator != null && !_hasTriggered)
        {
            _hasTriggered = true;
            
            if (enableDebugLogs)
            {
                Debug.Log("UIAnimationTrigger: 触发UI动画");
            }
            
            _uiAnimator.StartUIAnimation();
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
