# Canvas切换器 (CanvasSwitcher)

这个脚本专门用于处理Begin_Canvas到Start_Canvas的切换，采用直接监听Ready_Camera激活状态的方式，确保Canvas切换的可靠性。

## 功能描述

脚本通过以下步骤实现Canvas切换：
1. 在游戏启动时，确保Start_Canvas初始禁用
2. 持续监听Ready_Camera的激活状态
3. 当检测到Ready_Camera被激活时（摄像机切换完成）：
   - 自动禁用Begin_Canvas
   - 自动启用Start_Canvas
   - 自动调用StartCanvasController的TriggerUIAnimation方法开始UI动画

## 使用方法

1. 在场景中创建一个空游戏对象，命名为"CanvasManager"
2. 将`CanvasSwitcher.cs`脚本添加到该对象上
3. 在Inspector面板中设置必要的引用：
   - **Begin Canvas**: Begin_Canvas对象
   - **Start Canvas**: Start_Canvas对象 
   - **Ready Camera**: Ready_Camera对象（用于检测其激活状态）
4. 调整检测设置：
   - **Start Delay**: 开始监听前的延迟时间
   - **Check Interval**: 检测间隔时间
   - **Enable Debug Logs**: 是否启用调试日志

## 参数说明

### Canvas引用
- **Begin Canvas**: Begin_Canvas对象，将在检测到Ready_Camera激活时禁用
- **Start Canvas**: Start_Canvas对象，将在Begin_Canvas禁用后启用

### 摄像机引用
- **Ready Camera**: Ready_Camera对象，脚本将监听其激活状态

### 设置
- **Start Delay**: 开始监听前的延迟时间（秒），默认为0.5秒
- **Check Interval**: 检测间隔时间（秒），默认为0.1秒
- **Enable Debug Logs**: 是否启用调试日志，默认为true

## 工作流程

1. 在Start方法中：
   - 检查必要的引用
   - 获取StartCanvasController组件
   - 确保Start_Canvas初始禁用
   - 开始监听Ready_Camera激活状态

2. 在MonitorReadyCamera协程中：
   - 等待startDelay秒，确保所有组件都已初始化
   - 循环检测Ready_Camera激活状态
   - 当检测到Ready_Camera已激活时，执行Canvas切换并停止监听

3. 在SwitchCanvas方法中：
   - 禁用Begin_Canvas
   - 启用Start_Canvas
   - 调用StartCanvasController的TriggerUIAnimation方法开始UI动画

## 与其他脚本的协同

### 与 StartCanvasController 的协同
- 脚本会自动获取Start_Canvas上的StartCanvasController组件
- 在Canvas切换完成后，调用StartCanvasController的TriggerUIAnimation方法开始UI动画
- 这确保了Canvas切换和UI动画的顺序正确

### 与 CameraSwitchController 的协同
- 脚本直接监听Ready_Camera的激活状态，不依赖于CameraSwitchController的事件
- 这种方法更可靠，因为它直接检查Ready_Camera的实际状态

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `TriggerCanvasSwitch()`: 手动触发Canvas切换

## 示例代码

```csharp
// 获取Canvas切换器组件
CanvasSwitcher canvasSwitcher = GetComponent<CanvasSwitcher>();

// 手动触发Canvas切换
canvasSwitcher.TriggerCanvasSwitch();
```

## 注意事项

- 确保所有引用都已正确设置，否则脚本将在控制台输出错误并禁用自身
- 如果遇到问题，可以启用调试日志，查看控制台输出帮助排查问题
- 这个脚本采用循环检测的方式，不依赖于事件或反射，因此更加可靠
- 为了避免性能问题，检测间隔默认设置为0.1秒，可以根据需要调整
