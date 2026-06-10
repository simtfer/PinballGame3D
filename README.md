# 3D 弹球游戏 - 抖音小游戏

一个使用 Unity 引擎开发的酷炫 3D 弹球游戏，专为抖音小游戏平台优化。

## 项目特点

### 核心玩法
- 经典弹球物理模拟，使用 Unity 物理引擎
- 左右翻转板（Flippers）控制
- 多种弹球元素：碰撞器（Bumpers）、目标板（Targets）、弹弓（Slingshots）、旋转器（Spinners）、坡道（Ramps）
- 连击系统（Combo System）- 连续命中获得分数倍率
- 多球模式（Multiball）- 激活后同时控制多个球

### 视觉效果
- 霓虹风格（Neon Glow）着色器
- 动态球尾迹效果
- 碰撞火花粒子效果
- 动态灯光系统 - 随游戏节奏变化
- 浮动分数文字特效
- 相机动态跟随和 FOV 变化

### 抖音平台集成
- 分享功能 - 分享分数到抖音
- 排行榜集成
- 激励视频广告（看广告复活）
- 录屏分享功能
- 震动反馈

### 性能优化
- 动态质量调节系统
- 设备性能自动检测
- URP（通用渲染管线）支持
- IL2CPP 编译优化

## 项目结构

```
PinballGame3D/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # 核心系统
│   │   │   ├── GameManager.cs         # 游戏管理器
│   │   │   ├── BallController.cs      # 球控制器
│   │   │   ├── PlungerController.cs   # 发射器
│   │   │   ├── CameraController.cs    # 相机控制
│   │   │   ├── PinballTableBuilder.cs # 弹球台构建
│   │   │   ├── TouchInputManager.cs   # 触摸输入
│   │   │   ├── PerformanceManager.cs  # 性能管理
│   │   │   └── GameSceneSetup.cs      # 场景初始化
│   │   ├── Pinball/        # 弹球元素
│   │   │   ├── FlipperController.cs   # 翻转板
│   │   │   ├── BumperController.cs    # 碰撞器
│   │   │   ├── TargetController.cs    # 目标板
│   │   │   ├── SlingshotController.cs # 弹弓
│   │   │   ├── SpinnerController.cs   # 旋转器
│   │   │   ├── WallController.cs      # 墙壁
│   │   │   ├── KickbackController.cs  # 反弹器
│   │   │   ├── RampController.cs      # 坡道
│   │   │   ├── ScoreRolloverController.cs # 翻滚计分
│   │   │   └── MultiballController.cs # 多球模式
│   │   ├── UI/             # 用户界面
│   │   │   ├── GameHUD.cs            # 游戏HUD
│   │   │   ├── MainMenuUI.cs         # 主菜单
│   │   │   ├── GameOverUI.cs         # 游戏结束
│   │   │   ├── PauseMenuUI.cs        # 暂停菜单
│   │   │   ├── FloatingScoreText.cs  # 浮动分数
│   │   │   └── ScorePopupSpawner.cs  # 分数弹窗
│   │   ├── Effects/        # 特效系统
│   │   │   ├── NeonGlowEffect.cs     # 霓虹发光
│   │   │   └── TableLightingController.cs # 台面灯光
│   │   ├── Audio/          # 音频系统
│   │   │   └── AudioManager.cs       # 音频管理器
│   │   └── Platform/       # 平台集成
│   │       └── DouyinPlatformBridge.cs # 抖音SDK桥接
│   ├── Shaders/            # 着色器
│   │   ├── NeonGlow.shader           # 霓虹发光着色器
│   │   └── BallTrail.shader          # 球尾迹着色器
│   ├── Editor/             # 编辑器工具
│   │   ├── PinballBuildConfig.cs     # 构建配置
│   │   └── PinballTableBuilderEditor.cs # 弹球台编辑器
│   ├── Scenes/             # 场景
│   │   ├── MainMenu.unity            # 主菜单场景
│   │   └── GameScene.unity           # 游戏场景
│   ├── Resources/          # 资源
│   │   ├── URPAsset.asset            # URP配置
│   │   └── BallPhysics.physicMaterial # 球物理材质
│   └── Prefabs/            # 预制体（需创建）
├── Packages/
│   └── manifest.json       # 包管理配置
└── ProjectSettings/
    └── ProjectSettings.asset # 项目设置
```

## 操作方式

### 触屏操作
- **左侧触摸** - 左翻转板
- **右侧触摸** - 右翻转板
- **长按底部** - 蓄力发射球

### 键盘操作（测试用）
- **Left Shift / A** - 左翻转板
- **Right Shift / D** - 右翻转板
- **Space** - 蓄力发射球

## 开发指南

### 环境要求
- Unity 2021.3 LTS 或更高版本
- Universal Render Pipeline (URP)
- TextMeshPro

### 快速开始

1. 使用 Unity Hub 打开项目
2. 等待包管理器导入所有依赖
3. 打开 `GameScene.unity` 场景
4. 选中 `GameSetup` 对象，在 Inspector 中点击 "Build Table" 按钮
5. 点击 "Create Materials" 创建默认材质
6. 点击 "Create Prefabs" 创建默认预制体
7. 运行游戏进行测试

### 构建发布

在 Unity 编辑器中，使用菜单栏 `Pinball` 菜单：

- **Pinball > Build Android APK** - 构建 Android 版本
- **Pinball > Build iOS** - 构建 iOS 版本
- **Pinball > Build WebGL** - 构建 WebGL 版本

### 抖音小游戏集成

1. 从抖音开放平台下载 SDK
2. 将 SDK 导入到项目中
3. 在 `DouyinPlatformBridge.cs` 中配置 App ID
4. 按照抖音小游戏文档完成发布流程

## 计分规则

| 元素 | 基础分数 | 说明 |
|------|---------|------|
| 碰撞器 (Bumper) | 100 | 碰撞得分 |
| 目标板 (Target) | 250 | 击中得分 |
| 弹弓 (Slingshot) | 50 | 弹射得分 |
| 坡道 (Ramp) | 500 | 通过得分 |
| 旋转器 (Spinner) | 75 | 旋转得分 |
| 翻滚 (Rollover) | 100 | 通过得分 |
| 目标全灭 | 1000 | 额外奖励 |
| 翻滚全亮 | 2000 | 额外奖励 |

连击倍率最高 x5.0，2秒内无命中则重置连击。

## 技术特色

- **物理引擎**: 使用 Unity Rigidbody 进行精确物理模拟
- **碰撞检测**: ContinuousDynamic 模式防止高速穿模
- **材质系统**: 自定义 URP Shader 实现霓虹发光效果
- **性能优化**: 动态质量调节，自适应设备性能
- **音效系统**: 交叉淡入淡出，音效池化管理
