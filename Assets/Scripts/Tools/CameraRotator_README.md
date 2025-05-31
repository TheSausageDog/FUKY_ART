# 摄像机旋转控制器 (CameraRotator)

这个脚本用于控制摄像机在场景运行时沿Y轴进行慢速循环旋转，特别适用于主菜单场景等需要环境缓慢旋转的场景。

## 使用方法

1. 将 `CameraRotator.cs` 脚本添加到场景中的 `Wait_Camera` 摄像机对象上
2. 在 Inspector 面板中调整参数以满足需求
3. 运行场景，摄像机将按照设置进行旋转

## 参数说明

### 基本设置

- **旋转速度 (Rotation Speed)**: 控制摄像机旋转的速度，单位为度/秒，范围从 0.1 到 20
- **逆时针旋转 (Counter Clockwise)**: 勾选后摄像机将逆时针旋转，默认为顺时针
- **启用旋转 (Enable Rotation)**: 控制是否启用旋转功能，可以在运行时切换

### 高级设置

- **限制旋转 (Limit Rotation)**: 勾选后将限制摄像机的旋转范围
- **最小角度 (Min Angle)**: 设置旋转的最小角度，范围从 -180 到 180 度
- **最大角度 (Max Angle)**: 设置旋转的最大角度，范围从 -180 到 180 度

## 公共方法

脚本提供了以下可以在其他脚本中调用的公共方法：

- `ResetRotation()`: 重置摄像机旋转到初始状态
- `ToggleDirection()`: 切换旋转方向（顺时针/逆时针）
- `SetRotationSpeed(float speed)`: 设置旋转速度

## 示例代码

```csharp
// 获取摄像机上的旋转控制器组件
CameraRotator rotator = Camera.main.GetComponent<CameraRotator>();

// 修改旋转速度
rotator.SetRotationSpeed(10f);

// 切换旋转方向
rotator.ToggleDirection();

// 禁用旋转
rotator.enableRotation = false;

// 重置旋转
rotator.ResetRotation();
```

## 注意事项

- 该脚本使用 `[DisallowMultipleComponent]` 特性，确保每个对象上只能添加一个该组件
- 如果启用了旋转限制，当达到限制角度时，摄像机会自动改变旋转方向
- 该脚本保留了摄像机的初始俯仰角和翻滚角，只改变Y轴（水平）旋转
