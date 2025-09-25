# Lab2D / 实验室 2D

## Overview / 项目概述
Lab2D is a Unity-based 2D sandbox showcasing three stand-alone gameplay and tooling experiments that can be explored independently.
Lab2D 是一个基于 Unity 的 2D 沙盒项目，展示了三个可以独立研究和复用的玩法与工具实验。

The repository is organized so you can open each scene directly and inspect how the systems are assembled without wading through unrelated assets.
本仓库的结构便于直接打开每个场景，专注于了解对应系统的实现，而不会被无关资源干扰。

## Feature Highlights / 特色功能
### Signed Distance Field (SDF) Navigation / 有符号距离场（SDF）导航
- Precomputes a grid-based Euclidean distance transform from polygon colliders to guide smooth player movement around obstacles.
- 通过对多边形碰撞体执行基于网格的欧氏距离变换，预先生成距离场，以引导玩家在障碍物附近平滑移动。
- Samples SDF gradients every physics frame so the character can slide along surfaces while preserving a safety radius.
- 在每个物理帧采样距离场梯度，使角色在保持安全距离的同时沿表面滑行。
- Uses the Unity Input System to support keyboard, gamepad, joystick, and XR controllers with minimal configuration.
- 使用 Unity 输入系统，几乎无需配置即可支持键盘、手柄、摇杆和 XR 控制器。

### Particle Life Simulation / 粒子生命模拟
- Spawns thousands of particles that carry position, velocity, and color data using Unity Entities (ECS).
- 利用 Unity Entities（ECS）生成数千个粒子，并为每个粒子附加位置、速度与颜色数据。
- Applies color-dependent attraction and repulsion rules inside an update system to create emergent behavior.
- 在更新系统中根据颜色定义的吸引与排斥规则推动粒子产生涌现行为。
- Clamps velocity and writes results back through the EntityManager to keep the simulation stable at high particle counts.
- 通过限制速度并使用 EntityManager 写回结果，保证在大量粒子情况下的模拟稳定性。

### Modular UI System / 模块化 UI 系统
- Centralizes HUD lifecycle management inside a singleton UI manager with layered stacks.
- 使用单例 UI 管理器和分层堆栈集中管理 HUD 的生命周期。
- Resolves UI prefabs via a resource manager and instantiates them with shared base classes for consistent behavior.
- 通过资源管理器加载 UI 预制体，并借助统一的基类实现一致的行为。
- Provides helper APIs to open, refresh, and close widgets so gameplay code stays decoupled from UI details.
- 提供便捷的 API 来打开、刷新与关闭控件，使游戏逻辑与 UI 细节保持解耦。

## Project Structure / 项目结构
```
Assets/
  GlobalScripts/         Shared MonoBehaviour utilities such as SingletonMonoBehaviour
  Particle Life/         DOTS-based particle simulation scene and scripts
  SDF-Based Movement/    Player movement demo, input actions, and distance field pipeline
  UI System/             Runtime UI framework scripts
Packages/                Package manifest referencing Entities, Input System, and 2D feature set
ProjectSettings/         Unity project settings (Unity 6000.0.40f1)
```
```
Assets/
  GlobalScripts/         共享的 MonoBehaviour 工具类（例如 SingletonMonoBehaviour）
  Particle Life/         基于 DOTS 的粒子模拟场景与脚本
  SDF-Based Movement/    玩家移动示例、输入配置与距离场流程
  UI System/             运行时 UI 框架脚本
Packages/                引用 Entities、Input System 与 2D 功能集的包清单
ProjectSettings/         Unity 项目设置（Unity 6000.0.40f1）
```

## Getting Started / 快速上手
### Prerequisites / 环境要求
- Unity Editor **6000.0.40f1** (or any compatible 6.0 release).
- Unity 编辑器 **6000.0.40f1**（或兼容的 6.0 版本）。
- Required packages listed in `Packages/manifest.json` download automatically when the project opens.
- 打开项目时会自动下载 `Packages/manifest.json` 中列出的依赖包。

### Open the Project / 打开项目
1. Clone the repository: `git clone <repo-url>`
2. Launch Unity Hub, click **Add**, and select the `Lab2D` folder.
3. Open the project with Unity 6000.0.40f1.
1. 克隆仓库：`git clone <repo-url>`
2. 启动 Unity Hub，点击 **Add** 并选择 `Lab2D` 文件夹。
3. 使用 Unity 6000.0.40f1 打开该项目。

### Run the Demos / 运行演示
- **SDF-Based Movement** – Open `Assets/SDF-Based Movement/Scene.unity` and press Play. Move with WASD, arrow keys, or a gamepad stick to see obstacle-aware sliding.
- **SDF-Based Movement** – 打开 `Assets/SDF-Based Movement/Scene.unity` 并点击 Play。使用 WASD、方向键或手柄摇杆即可体验避障滑动效果。
- **Particle Life** – Open `Assets/Particle Life/Scene.unity` and press Play to observe the color interaction rules in motion.
- **Particle Life** – 打开 `Assets/Particle Life/Scene.unity` 并点击 Play，观察颜色交互规则驱动的粒子运动。
- **UI System** – Integrate scripts from `Assets/UI System/` into your own scene to experiment with layered HUD management.
- **UI System** – 将 `Assets/UI System/` 中的脚本集成到你的场景，体验分层 HUD 管理流程。

## Contributing / 参与贡献
1. Create a feature branch for your changes.
2. Ensure your work follows the existing code style and add tests when applicable.
3. Open a pull request summarizing the improvements.
1. 为你的改动创建特性分支。
2. 确保实现符合现有代码风格，并在适用时补充测试。
3. 提交拉取请求并概述你的改进。

## License / 许可证
This project currently has no explicit license. Please add one before distributing the code or assets.
该项目目前未提供明确的许可证。在分发代码或资源之前，请务必添加合适的许可协议。
