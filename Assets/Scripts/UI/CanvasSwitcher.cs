using System.Collections;
using UnityEngine;

/// <summary>
/// Canvas切换器
/// 专门用于处理Begin_Canvas到Start_Canvas的切换
/// </summary>
public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvas引用")]
    [Tooltip("Begin_Canvas对象")]
    public GameObject beginCanvas;
    
    [Tooltip("Start_Canvas对象")]
    public GameObject startCanvas;
    
    [Header("摄像机引用")]
    [Tooltip("准备摄像机 (用于检测其激活状态)")]
    public Camera readyCamera;
    
    [Header("设置")]
    [Tooltip("开始监听延迟(秒)")]
    public float startDelay = 0.5f;
    
    [Tooltip("检测间隔(秒)")]
    public float checkInterval = 0.1f;
    
    [Tooltip("是否启用调试日志")]
    public bool enableDebugLogs = true;
    
    // 引用UI控制器组件
    private StartCanvasController _startCanvasController;
    private UIAnimationController _uiAnimationController;
    
    private void Start()
    {
        // 检查必要的引用
        if (beginCanvas == null || startCanvas == null || readyCamera == null)
        {
            Debug.LogError("CanvasSwitcher: 缺少必要的引用，请在Inspector中设置。");
            enabled = false;
            return;
        }
        
        // 获取UI动画控制器组件
        _uiAnimationController = startCanvas.GetComponent<UIAnimationController>();
        if (_uiAnimationController == null)
        {
            Debug.Log("CanvasSwitcher: 尝试查找UIAnimationController失败，将尝试使用StartCanvasController");
            
            // 获取StartCanvasController组件
            _startCanvasController = startCanvas.GetComponent<StartCanvasController>();
            if (_startCanvasController == null)
            {
                Debug.LogWarning("CanvasSwitcher: Start_Canvas对象上没有找到UI动画控制器组件。UI动画将无法播放。");
            }
        }
        else
        {
            Debug.Log("CanvasSwitcher: 已找到UIAnimationController组件");
        }
        
        // 确保Start_Canvas初始禁用
        if (startCanvas.activeSelf)
        {
            if (enableDebugLogs)
            {
                Debug.Log("CanvasSwitcher: 确保Start_Canvas初始禁用");
            }
            startCanvas.SetActive(false);
        }
        
        // 开始监听Ready_Camera激活状态
        StartCoroutine(MonitorReadyCamera());
        
        if (enableDebugLogs)
        {
            Debug.Log("CanvasSwitcher: 初始化完成，开始监听Ready_Camera激活状态");
        }
    }
    
    /// <summary>
    /// 监听Ready_Camera激活状态
    /// </summary>
    private IEnumerator MonitorReadyCamera()
    {
        // 等待一段时间再开始监听，确保所有组件都已初始化
        yield return new WaitForSeconds(startDelay);
        
        if (enableDebugLogs)
        {
            Debug.Log("CanvasSwitcher: 开始监听Ready_Camera激活状态");
        }
        
        // 循环检测Ready_Camera激活状态
        while (true)
        {
            // 如果Ready_Camera已激活
            if (readyCamera.gameObject.activeSelf && readyCamera.enabled)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("CanvasSwitcher: 检测到Ready_Camera已激活");
                }
                
                // 执行Canvas切换
                SwitchCanvas();
                
                // 不再继续监听
                yield break;
            }
            
            // 等待一段时间再检测
            yield return new WaitForSeconds(checkInterval);
        }
    }
    
    /// <summary>
    /// 执行Canvas切换
    /// </summary>
    private void SwitchCanvas()
    {
        if (enableDebugLogs)
        {
            Debug.Log("CanvasSwitcher: 执行Canvas切换");
        }
        
        // 禁用Begin_Canvas
        if (beginCanvas.activeSelf)
        {
            beginCanvas.SetActive(false);
            
            if (enableDebugLogs)
            {
                Debug.Log("CanvasSwitcher: Begin_Canvas已禁用");
            }
        }
        
        // 启用Start_Canvas
        if (!startCanvas.activeSelf)
        {
            startCanvas.SetActive(true);
            
            if (enableDebugLogs)
            {
                Debug.Log("CanvasSwitcher: Start_Canvas已启用");
            }
        }
        
        // 优先使用UIAnimationController
        if (_uiAnimationController != null)
        {
            _uiAnimationController.StartUIAnimation();
            
            if (enableDebugLogs)
            {
                Debug.Log("CanvasSwitcher: 已调用UIAnimationController.StartUIAnimation()");
            }
        }
        // 如果没有UIAnimationController，尝试使用StartCanvasController
        else if (_startCanvasController != null)
        {
            _startCanvasController.TriggerUIAnimation();
            
            if (enableDebugLogs)
            {
                Debug.Log("CanvasSwitcher: 已调用StartCanvasController.TriggerUIAnimation()");
            }
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("CanvasSwitcher: 未找到任何UI动画控制器，无法播放UI动画");
            }
        }
    }
    
    /// <summary>
    /// 手动触发Canvas切换（用于外部调用）
    /// </summary>
    public void TriggerCanvasSwitch()
    {
        SwitchCanvas();
    }
}
