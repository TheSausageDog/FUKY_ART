# UI动画控制器 (UIAnimationController)

这个脚本专门用于处理UI元素的动画效果，不依赖于Canvas切换逻辑，解决了StartCanvasController中动画与Canvas切换逻辑耦合过高的问题。

## 功能描述

脚本实现以下UI动画效果：
1. 标题图案从屏幕上方平滑进入到达原始位置
2. 底部装饰从屏幕下方平滑进入到达原始位置
3. 按钮从屏幕左侧依次平滑进入到达原始位置
4. 在动画完成前，按钮无法交互

## 使用方法

1. 将 `UIAnimationController.cs` 脚本添加到 `Start_Canvas` 对象上
2. 在 Inspector 面板中设置必要的UI引用：
   - **Title Logo**: 标题图案UI（需要是RectTransform类型）
   - **Bottom Decoration**: 底部装饰UI（需要是RectTransform类型）
   - **Buttons Group**: 包含按钮的父对象（需要是RectTransform类型）
3. 调整动画参数以满足需求
4. 在CameraSwitchController的直接操作设置中启用"直接启用Start_Canvas"，确保Start_Canvas能够正确启用
5. 在摄像机切换完成后，通过以下代码触发UI动画：
   ```csharp
   UIAnimationController uiAnimator = startCanvas.GetComponent<UIAnimationController>();
   if (uiAnimator != null)
   {
       uiAnimator.StartUIAnimation();
   }
   ```

## 与CameraSwitchController的配合使用

1. 在CameraSwitchController脚本中配置：
   - 启用"直接禁用Begin_Canvas"选项
   - 启用"直接启用Start_Canvas"选项
   - 禁用"直接调用StartCanvasController"选项（因为StartCanvasController可能有问题）
   - 在SwitchToReadyCamera方法中添加以下代码：
   ```csharp
   // 如果启用了直接启用Start_Canvas
   if (directlyEnableStartCanvas && startCanvas != null)
   {
       startCanvas.SetActive(true);
       UIAnimationController uiAnimator = startCanvas.GetComponent<UIAnimationController>();
       if (uiAnimator != null)
       {
           uiAnimator.StartUIAnimation();
       }
   }
   ```

## 参数说明

### UI元素引用

- **Title Logo**: 标题图案UI，将从屏幕上方进入
- **Bottom Decoration**: 底部装饰UI，将从屏幕下方进入
- **Buttons Group**: 包含按钮的父对象，按钮将从屏幕左侧依次进入

### 动画设置

- **Animation Duration**: 动画持续时间（秒），范围从0.5到3秒
- **Element Delay**: 元素进入时间间隔（秒），控制各元素开始动画的时间差
- **Animation Curve**: 动画曲线，控制动画的加速和减速效果
- **Offscreen Offset**: 屏幕外偏移距离，控制UI元素初始位置距离屏幕边缘的远近

### 按钮交互设置

- **Disable Buttons Until Animation Complete**: 是否在动画完成前禁用按钮交互，动画完成后自动启用

### 调试设置

- **Enable Debug Logs**: 是否启用调试日志，帮助排查问题

## 优势

1. **解耦合**：将UI动画逻辑与Canvas切换逻辑完全分离，使得两者可以独立工作
2. **可靠性高**：不依赖于其他脚本的事件或状态，自主完成动画效果
3. **重用性好**：可以应用于任何需要类似入场动画的UI界面，不仅限于Start_Canvas
4. **易于调试**：提供详细的调试日志，方便排查问题

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `StartUIAnimation()`: 开始UI动画
- `MoveUIElementsOffscreen()`: 将UI元素移动到屏幕外
- `ResetUI()`: 重置UI元素到初始状态（屏幕外）
- `IsAnimationCompleted()`: 检查动画是否已完成，返回布尔值

## 工作流程

1. 在Awake中，脚本保存所有UI元素的原始位置
2. 在Start中，脚本将UI元素移动到屏幕外，并禁用按钮交互
3. 当调用StartUIAnimation方法时，脚本按照以下顺序播放UI动画：
   - 首先，标题图案从上方进入
   - 然后，底部装饰从下方进入
   - 最后，按钮从左侧依次进入
4. 每个UI元素都会平滑地移动到其原始位置
5. 动画完成后，按钮自动启用交互

## 注意事项

- 确保所有UI引用都已正确设置，否则脚本将在控制台输出错误并禁用自身
- 脚本会自动保存UI元素的原始位置，不需要手动记录
- 脚本会自动检测Canvas尺寸，用于计算屏幕外位置
- 按钮会按照层级顺序依次进入，确保按钮在层级面板中的顺序符合预期
- 在CameraSwitchController中启用"直接启用Start_Canvas"选项，确保Start_Canvas能够正确启用
