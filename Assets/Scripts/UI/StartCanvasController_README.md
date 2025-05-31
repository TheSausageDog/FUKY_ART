# Start_Canvas控制器 (StartCanvasController)

这个脚本用于处理从Begin_Canvas到Start_Canvas的过渡动画，实现UI元素从屏幕外平滑进入的效果。

## 功能描述

当摄像机从Wait_Camera切换到Ready_Camera完成后：
1. Begin_Canvas自动禁用
2. Start_Canvas自动启用
3. 标题图案从屏幕上方平滑进入到达原始位置
4. 底部装饰从屏幕下方平滑进入到达原始位置
5. 四个按钮从屏幕左侧依次平滑进入到达原始位置
6. 在动画完成前，按钮无法交互

## 使用方法

1. 将 `StartCanvasController.cs` 脚本添加到场景中的 `Start_Canvas` 对象上
2. 在 Inspector 面板中设置必要的UI引用：
   - **Start Canvas**: Start_Canvas对象
   - **Title Logo**: 标题图案UI（需要是RectTransform类型）
   - **Bottom Decoration**: 底部装饰UI（需要是RectTransform类型）
   - **Buttons Group**: 包含四个按钮的父对象（需要是RectTransform类型）
3. 调整动画参数以满足需求
4. 确保场景中已添加 `CameraSwitchController.cs` 脚本（用于自动触发UI动画）
5. 运行场景，当摄像机切换完成后，Start_Canvas的UI元素将自动开始进场动画

## 参数说明

### Canvas引用

- **Start Canvas**: Start_Canvas对象，用于获取Canvas尺寸和确保Canvas激活
- **Begin Canvas**: Begin_Canvas对象，在摄像机切换完成后自动禁用

### UI元素引用

- **Title Logo**: 标题图案UI，将从屏幕上方进入
- **Bottom Decoration**: 底部装饰UI，将从屏幕下方进入
- **Buttons Group**: 包含四个按钮的父对象，按钮将从屏幕左侧依次进入

### 动画设置

- **Animation Duration**: 动画持续时间（秒），范围从0.5到3秒
- **Element Delay**: 元素进入时间间隔（秒），控制各元素开始动画的时间差
- **Animation Curve**: 动画曲线，控制动画的加速和减速效果
- **Offscreen Offset**: 屏幕外偏移距离，控制UI元素初始位置距离屏幕边缘的远近

### 按钮交互设置

- **Disable Buttons Until Animation Complete**: 是否在动画完成前禁用按钮交互，动画完成后自动启用

### 事件设置

- **Auto Link With Camera Switch**: 是否自动监听摄像机切换事件
- **Camera Switch Controller**: 摄像机切换控制器引用，用于监听摄像机切换完成事件

### 调试设置

- **Enable Debug Logs**: 是否启用调试日志，帮助排查问题
- **Use Alternative Monitoring**: 是否使用备用监听方法，直接监听Ready_Camera的激活状态而不是使用反射获取_isTransitioning字段

## 与其他脚本的协同

### 与 CameraSwitchController 的协同

- 脚本使用两种方式监听摄像机切换完成事件：
  1. **事件订阅方式**：直接订阅CameraSwitchController的OnCameraSwitchCompleted事件（主要方式）
  2. **反射监听方式**：使用反射机制监听CameraSwitchController的过渡状态（备用方式）
- 当检测到摄像机过渡完成时，自动禁用Begin_Canvas，启用Start_Canvas，并开始UI动画
- 这确保了摄像机切换和UI动画的顺序正确

## 工作流程

1. 在Awake中，脚本保存所有UI元素的原始位置
2. 在Start中，脚本将UI元素移动到屏幕外，并开始监听摄像机切换事件
3. 当摄像机切换完成后，脚本按照以下顺序播放UI动画：
   - 首先，标题图案从上方进入
   - 然后，底部装饰从下方进入
   - 最后，四个按钮从左侧依次进入
4. 每个UI元素都会平滑地移动到其原始位置

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `StartUIAnimation()`: 开始UI动画
- `TriggerUIAnimation()`: 手动触发UI动画（与StartUIAnimation功能相同，提供更直观的命名）
- `ResetUI()`: 重置UI元素到初始状态（屏幕外）
- `IsAnimationCompleted()`: 检查动画是否已完成，返回布尔值

## 示例代码

```csharp
// 获取Start_Canvas控制器组件
StartCanvasController startCanvasController = GetComponent<StartCanvasController>();

// 手动触发UI动画
startCanvasController.TriggerUIAnimation();

// 检查动画是否完成
if (startCanvasController.IsAnimationCompleted())
{
    // 动画已完成，可以执行其他操作
    Debug.Log("UI动画已完成，按钮可以交互");
}

// 重置UI元素到初始状态
startCanvasController.ResetUI();
```

## 注意事项

- 确保所有UI引用都已正确设置，否则脚本将在控制台输出错误并禁用自身
- 脚本会自动保存UI元素的原始位置，不需要手动记录
- 如果使用自动链接功能，确保场景中已添加CameraSwitchController脚本
- 按钮会按照层级顺序依次进入，确保按钮在层级面板中的顺序符合预期
- 如果需要调整UI元素的初始位置（屏幕外位置），可以修改offscreenOffset参数
