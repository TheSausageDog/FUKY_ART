using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI文字透明度闪烁效果控制器
/// 使UI文字元素的透明度在最小值和最大值之间平滑变化，产生闪烁提示效果
/// </summary>
[DisallowMultipleComponent]
public class UITextFader : MonoBehaviour
{
    [Header("闪烁设置")]
    [Tooltip("闪烁速度 (数值越大越快)")]
    [Range(0.1f, 5f)]
    public float fadeSpeed = 1f;

    [Tooltip("最小透明度")]
    [Range(0f, 1f)]
    public float minAlpha = 0.2f;

    [Tooltip("最大透明度")]
    [Range(0f, 1f)]
    public float maxAlpha = 1f;

    [Header("高级设置")]
    [Tooltip("是否使用正弦曲线进行平滑过渡 (否则使用线性过渡)")]
    public bool useSmoothSine = true;

    [Tooltip("是否在启动时开始闪烁")]
    public bool fadeOnStart = true;

    // 私有变量
    private float currentTime = 0f;
    private bool isFading = false;
    
    // 组件引用
    private CanvasGroup canvasGroup;
    private Text textComponent;
    private TextMeshProUGUI tmpComponent;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Awake()
    {
        // 尝试获取CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        
        // 如果没有CanvasGroup组件，尝试获取Text或TextMeshProUGUI组件
        if (canvasGroup == null)
        {
            textComponent = GetComponent<Text>();
            tmpComponent = GetComponent<TextMeshProUGUI>();
            
            // 如果既没有Text也没有TextMeshProUGUI组件，添加警告
            if (textComponent == null && tmpComponent == null)
            {
                Debug.LogWarning("UITextFader: 未找到CanvasGroup、Text或TextMeshProUGUI组件。请添加其中一个组件或将此脚本附加到包含这些组件的对象上。");
            }
        }
    }

    /// <summary>
    /// 启动时初始化
    /// </summary>
    private void Start()
    {
        // 如果设置为启动时开始闪烁，则启动闪烁效果
        if (fadeOnStart)
        {
            StartFading();
        }
    }

    /// <summary>
    /// 每帧更新透明度
    /// </summary>
    private void Update()
    {
        // 如果未启用闪烁，则跳过
        if (!isFading)
            return;

        // 更新时间
        currentTime += Time.deltaTime * fadeSpeed;
        
        // 计算当前透明度
        float alpha;
        if (useSmoothSine)
        {
            // 使用正弦曲线实现平滑的透明度变化 (0-1-0)
            alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(currentTime * Mathf.PI) + 1f) * 0.5f);
        }
        else
        {
            // 使用锯齿波实现线性的透明度变化 (0-1-0)
            float t = Mathf.PingPong(currentTime, 1f);
            alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        }
        
        // 应用透明度
        ApplyAlpha(alpha);
    }

    /// <summary>
    /// 应用透明度到UI元素
    /// </summary>
    /// <param name="alpha">要应用的透明度值 (0-1)</param>
    private void ApplyAlpha(float alpha)
    {
        // 根据可用的组件类型应用透明度
        if (canvasGroup != null)
        {
            // 如果有CanvasGroup组件，直接设置其alpha值
            canvasGroup.alpha = alpha;
        }
        else if (textComponent != null)
        {
            // 如果有Text组件，设置其颜色的alpha通道
            Color color = textComponent.color;
            color.a = alpha;
            textComponent.color = color;
        }
        else if (tmpComponent != null)
        {
            // 如果有TextMeshProUGUI组件，设置其颜色的alpha通道
            Color color = tmpComponent.color;
            color.a = alpha;
            tmpComponent.color = color;
        }
    }

    /// <summary>
    /// 开始透明度闪烁效果
    /// </summary>
    public void StartFading()
    {
        isFading = true;
    }

    /// <summary>
    /// 停止透明度闪烁效果
    /// </summary>
    /// <param name="resetToMax">是否重置透明度到最大值</param>
    public void StopFading(bool resetToMax = true)
    {
        isFading = false;
        
        // 如果需要重置到最大透明度
        if (resetToMax)
        {
            ApplyAlpha(maxAlpha);
        }
    }

    /// <summary>
    /// 设置闪烁速度
    /// </summary>
    /// <param name="speed">新的闪烁速度</param>
    public void SetFadeSpeed(float speed)
    {
        fadeSpeed = Mathf.Clamp(speed, 0.1f, 5f);
    }

    /// <summary>
    /// 设置透明度范围
    /// </summary>
    /// <param name="min">最小透明度</param>
    /// <param name="max">最大透明度</param>
    public void SetAlphaRange(float min, float max)
    {
        minAlpha = Mathf.Clamp01(min);
        maxAlpha = Mathf.Clamp01(max);
        
        // 确保最小值不大于最大值
        if (minAlpha > maxAlpha)
        {
            minAlpha = maxAlpha;
        }
    }
}
