# UI文字透明度闪烁效果控制器 (UITextFader)

这个脚本用于控制UI文字元素的透明度在最小值和最大值之间平滑变化，产生闪烁提示效果，特别适用于"点击任意键开始"等需要吸引玩家注意的提示文字。

## 使用方法

1. 将 `UITextFader.cs` 脚本添加到场景中的 `Begin_Canvas` 下包含"点击任意键开始"文字的UI元素上
2. 在 Inspector 面板中调整参数以满足需求
3. 运行场景，文字将按照设置进行透明度闪烁

## 支持的组件类型

脚本会自动检测并使用以下组件之一来控制透明度：
- **CanvasGroup**：如果对象上有CanvasGroup组件，将优先使用它控制整个对象的透明度
- **Text (UGUI)**：如果对象上有标准UGUI Text组件，将控制文本颜色的透明度通道
- **TextMeshProUGUI (TMP)**：如果对象上有TextMesh Pro文本组件，将控制文本颜色的透明度通道

## 参数说明

### 基本设置

- **闪烁速度 (Fade Speed)**：控制透明度变化的速度，数值越大闪烁越快，范围从 0.1 到 5
- **最小透明度 (Min Alpha)**：闪烁过程中的最低透明度，范围从 0 到 1
- **最大透明度 (Max Alpha)**：闪烁过程中的最高透明度，范围从 0 到 1

### 高级设置

- **使用平滑过渡 (Use Smooth Sine)**：勾选后使用正弦曲线实现平滑的透明度变化，否则使用线性变化
- **启动时开始闪烁 (Fade On Start)**：勾选后将在场景启动时自动开始闪烁效果

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `StartFading()`：开始透明度闪烁效果
- `StopFading(bool resetToMax = true)`：停止透明度闪烁效果，可选择是否重置为最大透明度
- `SetFadeSpeed(float speed)`：设置闪烁速度
- `SetAlphaRange(float min, float max)`：设置透明度范围

## 示例代码

```csharp
// 获取文本上的闪烁控制器组件
UITextFader fader = GetComponent<UITextFader>();

// 修改闪烁速度
fader.SetFadeSpeed(2.5f);

// 修改透明度范围
fader.SetAlphaRange(0.3f, 1.0f);

// 停止闪烁
fader.StopFading();

// 重新开始闪烁
fader.StartFading();
```

## 注意事项

- 该脚本使用 `[DisallowMultipleComponent]` 特性，确保每个对象上只能添加一个该组件
- 如果对象上没有CanvasGroup、Text或TextMeshProUGUI组件，脚本会在控制台输出警告信息
- 为获得最佳效果，建议将最小透明度设置在0.2-0.5之间，最大透明度设置为1
