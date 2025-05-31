# 摄像机切换控制器 (CameraSwitchController)

这个脚本用于实现两个摄像机之间的平滑过渡，特别适用于主菜单场景中点击任意键后的摄像机切换效果。

## 功能描述

当玩家点击任意键后（与UI动画同步）：
1. Wait_Camera（初始激活的摄像机）会平滑地平移和旋转到Ready_Camera的位置和旋转方向
2. 当两个摄像机完全重合后，Wait_Camera被禁用，Ready_Camera被启用
3. 整个过程平滑无缝，给玩家带来流畅的视觉体验
4. 可选择直接禁用Begin_Canvas并启用Start_Canvas，确保UI切换的可靠性

## 使用方法

1. 将 `CameraSwitchController.cs` 脚本添加到场景中的"摄像机切换控制"空物体上
2. 在 Inspector 面板中设置必要的摄像机引用：
   - **Wait Camera**: 初始激活的摄像机（通常是带有旋转效果的摄像机）
   - **Ready Camera**: 目标摄像机（通常是初始禁用的摄像机）
3. 设置Canvas引用：
   - **Begin Canvas**: Begin_Canvas对象
   - **Start Canvas**: Start_Canvas对象
4. 调整过渡参数以满足需求
5. 确保场景中已添加 `MenuTransitionController.cs` 脚本（用于自动触发摄像机过渡）
6. 运行场景，按下任意键触发UI动画和摄像机过渡

## 参数说明

### 摄像机引用

- **Wait Camera**: 初始激活的摄像机，将从此摄像机过渡
- **Ready Camera**: 目标摄像机，将过渡到此摄像机的位置和旋转

### 过渡设置

- **Transition Duration**: 过渡持续时间（秒），范围从0.5到5秒
- **Transition Curve**: 过渡动画曲线，控制过渡的加速和减速效果
- **Disable Rotation During Transition**: 是否在过渡期间禁用摄像机旋转（如果Wait_Camera上有CameraRotator组件）

### 事件设置

- **Auto Link With Menu Controller**: 是否自动响应菜单过渡控制器的事件（当玩家点击任意键时自动触发摄像机过渡）

### 直接操作设置（新增）

- **Directly Disable Begin Canvas**: 是否直接禁用Begin_Canvas，不依赖于事件系统
- **Begin Canvas**: Begin_Canvas对象引用
- **Directly Enable Start Canvas**: 是否直接启用Start_Canvas，不依赖于事件系统
- **Start Canvas**: Start_Canvas对象引用
- **Directly Call Start Canvas Controller**: 是否直接调用StartCanvasController的TriggerUIAnimation方法

## 与其他脚本的协同

### 与 CameraRotator 的协同

- 脚本会自动检测Wait_Camera上是否有CameraRotator组件
- 如果设置了"Disable Rotation During Transition"，过渡期间会暂时禁用旋转
- 过渡完成后，如果之前启用了旋转，会恢复旋转状态

### 与 MenuTransitionController 的协同

- 脚本使用反射机制监听MenuTransitionController的过渡状态
- 当检测到菜单过渡被触发时，自动开始摄像机过渡
- 这确保了UI动画和摄像机过渡的同步

### 与 StartCanvasController 的协同（新增）

- 如果启用了"Directly Call Start Canvas Controller"，会在摄像机切换完成后直接调用StartCanvasController的TriggerUIAnimation方法
- 这提供了一种更直接、更可靠的方式来触发UI动画，不依赖于事件系统

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `StartCameraTransition()`: 开始摄像机过渡
- `TriggerCameraTransition()`: 手动触发摄像机过渡（与StartCameraTransition功能相同，提供更直观的命名）
- `ResetCameras()`: 重置摄像机到初始状态

## 示例代码

```csharp
// 获取摄像机切换控制器组件
CameraSwitchController cameraSwitch = GetComponent<CameraSwitchController>();

// 手动触发摄像机过渡
cameraSwitch.TriggerCameraTransition();

// 重置摄像机到初始状态
cameraSwitch.ResetCameras();
```

## 注意事项

- 确保Wait_Camera和Ready_Camera都已正确设置，否则脚本将在控制台输出错误并禁用自身
- 确保Ready_Camera在初始状态下是禁用的，脚本会在Start中检查并自动禁用
- 如果使用自动链接功能，确保场景中已添加MenuTransitionController脚本
- 过渡完成后，Wait_Camera会被禁用，Ready_Camera会被启用
- 脚本使用多种机制确保摄像机正确切换：
  - 启用/禁用GameObject
  - 启用/禁用Camera组件
  - 调整Camera深度值
  - 使用延迟确认机制确保切换生效

## 摄像机切换机制（已增强）

脚本使用以下步骤确保摄像机正确切换：

1. 首先启用Ready_Camera的GameObject和Camera组件
2. 将Ready_Camera的深度设置为比Wait_Camera高10，确保它成为主摄像机
3. 禁用Wait_Camera的Camera组件和GameObject
4. 如果启用了直接操作设置，立即执行Canvas切换和UI动画触发
5. 延迟两帧，检查切换是否生效
6. 如果切换未生效，执行以下操作：
   - 确保Ready_Camera仍然是启用状态
   - 确保Wait_Camera仍然是禁用状态
   - 禁用场景中所有其他摄像机
   - 将Ready_Camera的深度设置为999，确保它成为主摄像机
   - 再次执行Canvas切换和UI动画触发
7. 触发摄像机切换完成事件

这种多重保障机制确保了摄像机切换和UI切换的可靠性，解决了之前可能存在的问题。
