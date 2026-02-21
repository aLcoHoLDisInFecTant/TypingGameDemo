# TypeRogue 代码文档

本文档旨在为开发人员提供 TypeRogue 项目的代码结构概览、核心模块说明以及开发规范。

## 1. 代码结构概览

项目代码主要存放于 `Assets/Scripts` 目录下，按照功能模块进行划分：

*   **Bootstrap**: 游戏启动与初始化逻辑
*   **Combat**: 战斗核心逻辑（武器、伤害等）
*   **Typing**: 打字输入与指令解析核心逻辑
*   **UI**: 界面视图与交互逻辑

## 2. 核心模块说明

### 2.1 启动模块 (Bootstrap)

*   **[TypeRogueBootstrap.cs](Assets/Scripts/Bootstrap/TypeRogueBootstrap.cs)**
    *   **描述**: 游戏的核心协调器。
    *   **职责**:
        *   负责在 `Awake` 和 `Start` 阶段初始化各个子系统。
        *   手动绑定场景中的引用（如 UI 组件、输入路由）。
        *   协调输入、战斗和 UI 之间的事件传递。

### 2.2 打字系统 (Typing)

*   **[TypingInputRouter.cs](Assets/Scripts/Typing/TypingInputRouter.cs)**
    *   **描述**: 输入路由控制器。
    *   **职责**:
        *   直接对接 Unity Input System。
        *   维护字符输入缓冲区。
        *   处理特殊按键（退格、回车/空格）。
*   **[TypingCommandInterpreter.cs](Assets/Scripts/Typing/TypingCommandInterpreter.cs)**
    *   **描述**: 拼写命令解释器。
    *   **职责**:
        *   将用户输入的字符串解析为具体的游戏指令。
        *   维护指令词典（如 "pistol"）。
        *   分发解析结果（成功触发武器或未知指令）。
*   **[TypingResolveResult.cs](Assets/Scripts/Typing/TypingResolveResult.cs)**
    *   **描述**: 输入解析结果的数据结构。
    *   **职责**:
        *   定义解析结果类型（WeaponPistol, UnknownWord 等）。
        *   存储原始输入词。

### 2.3 战斗系统 (Combat)

*   **[PistolWeaponController.cs](Assets/Scripts/Combat/PistolWeaponController.cs)**
    *   **描述**: 手枪武器控制器。
    *   **职责**:
        *   管理武器的冷却时间。
        *   处理具体的发射逻辑（实例化子弹）。
*   **[PlayerController.cs](Assets/Scripts/Combat/PlayerController.cs)**
    *   **描述**: 玩家控制器。
    *   **职责**:
        *   管理玩家生命值。
        *   提供枪口（Muzzle）位置引用。
*   **[Projectile.cs](Assets/Scripts/Combat/Projectile.cs)**
    *   **描述**: 通用子弹控制器。
    *   **职责**:
        *   处理直线飞行逻辑。
        *   处理与敌人的碰撞检测。
*   **[Enemy.cs](Assets/Scripts/Combat/Enemy.cs)**
    *   **描述**: 敌人控制器。
    *   **职责**:
        *   控制敌人向下移动。
        *   处理受击与死亡逻辑。
*   **[EnemySpawner.cs](Assets/Scripts/Combat/EnemySpawner.cs)**
    *   **描述**: 敌人生成器。
    *   **职责**:
        *   按时间间隔生成敌人波次。
        *   控制生成位置的随机范围。

### 2.4 UI 系统 (UI)

*   **[BattleHudView.cs](Assets/Scripts/UI/BattleHudView.cs)**
    *   **描述**: 战斗主界面视图。
    *   **职责**:
        *   显示波次、玩家 HP（使用 TextMeshPro）。
*   **[TypingInputBarView.cs](Assets/Scripts/UI/TypingInputBarView.cs)**
    *   **描述**: 打字输入栏视图。
    *   **职责**:
        *   实时显示玩家输入的字符（使用 TextMeshPro）。
        *   显示光标闪烁动画。
*   **[WeaponStatusPanelView.cs](Assets/Scripts/UI/WeaponStatusPanelView.cs)**
    *   **描述**: 武器列表容器视图。
    *   **职责**:
        *   管理武器列表的布局（VerticalLayoutGroup）。
        *   动态创建和移除武器项（包含 `HideFlags` 修复逻辑以避免 Unity 断言错误）。
*   **[WeaponStatusItemView.cs](Assets/Scripts/UI/WeaponStatusItemView.cs)**
    *   **描述**: 单个武器的状态视图（Prefab）。
    *   **职责**:
        *   显示武器名称（TextMeshPro）。
        *   显示冷却遮罩（Image Fill）。

*   **[WeaponData.cs](Assets/Scripts/Data/WeaponData.cs)**
    *   **描述**: 武器属性配置（ScriptableObject）。
    *   **职责**:
        *   存储武器名称、图标、冷却时间、子弹预制体等。
        *   作为数据资产（Asset）存在，方便策划配置。

## 3. 场景层级配置 (Scene Hierarchy)

为了确保代码正常运行，请在 Unity 场景中按照以下结构搭建层级。
**注意：所有的 Text 组件均推荐使用 TextMeshPro - Text (UI)。**

### 3.1 核心层级结构

```text
TypeRogueScene
├── Main Camera
├── GameRoot (Empty GameObject)
│   ├── [Script] TypeRogueBootstrap (需拖拽引用 BattleHudView, TypingInputBarView, WeaponStatusPanelView)
│   ├── [Script] TypingInputRouter
│   ├── [Script] TypingCommandInterpreter
│   ├── [Script] PistolWeaponController (需拖拽 Projectile Prefab)
│   └── [Script] EnemySpawner (需拖拽 Enemy Prefab)
├── Player (GameObject / Sprite)
│   ├── [Script] PlayerController
│   └── Muzzle (Empty Child) -> 放置在枪口位置
├── UI_Canvas (Canvas)
│   ├── BattleHud (Panel)
│   │   ├── [Script] BattleHudView (需拖拽下方子组件引用)
│   │   ├── WaveText (TextMeshPro - Text)
│   │   ├── HPText (TextMeshPro - Text)
│   │   └── LastResultText (TextMeshPro - Text)
│   ├── WeaponStatusPanel (Panel)
│   │   ├── [Script] WeaponStatusPanelView (需拖拽下方引用)
│   │   └── WeaponListContainer (VerticalLayoutGroup) -> 武器Item生成的父节点
│   └── TypingInputBar (Panel)
│       ├── [Script] TypingInputBarView (需拖拽下方子组件引用)
│       └── BufferText (TextMeshPro - Text)
└── EventSystem
```

### 3.2 关键绑定说明

1.  **GameRoot**:
    *   挂载 `TypeRogueBootstrap`。
    *   同时挂载 `TypingInputRouter`, `TypingCommandInterpreter`, `PistolWeaponController`, `EnemySpawner`。
    *   **TypeRogueBootstrap Inspector 设置**:
        *   将 `UI_Canvas/BattleHud` 拖拽到 `BattleHudView` 字段。
        *   将 `UI_Canvas/TypingInputBar` 拖拽到 `TypingInputBarView` 字段。
        *   将 `UI_Canvas/WeaponStatusPanel` 拖拽到 `WeaponStatusPanelView` 字段。
        *   **创建并分配 WeaponData**:
            *   在 Project 窗口右键 -> Create -> TypeRogue -> WeaponData。
            *   配置好名称（如 "Pistol"）、冷却时间（如 0.5）、子弹预制体。
            *   将此 WeaponData 资产拖拽给 `Starting Weapon Data` 字段。
            *   同时，将 `PistolWeaponController` 中的 `Weapon Data` 字段也拖拽同一个资产（或者留空由代码注入）。
        *   `PlayerController` 和 `EnemySpawner` 字段可以留空（代码会自动查找），也可以手动拖拽。

2.  **Player**:
    *   挂载 `PlayerController`。
    *   创建一个子物体命名为 `Muzzle`，移动到枪口位置，并将其拖拽给 `PlayerController` 的 `MuzzlePoint` 字段。

3.  **UI_Canvas**:
    *   确保 `BattleHud`, `TypingInputBar` 和 `WeaponStatusPanel` 分别挂载了对应的 View 脚本。
    *   **WeaponStatusPanel**:
        *   需要制作一个 **WeaponItemPrefab**。
        *   Prefab 结构:
            *   Root (Image/Panel) -> 挂载 `WeaponStatusItemView`
            *   WeaponName (TMP)
            *   CooldownOverlay (Image, Type=Filled, Color=半透明黑/灰)
        *   将此 Prefab 拖拽给 `WeaponStatusPanelView` 的 `Item Prefab` 字段。
        *   将 `WeaponListContainer` 拖拽给 `WeaponStatusPanelView` 的 `Container` 字段。

### 3.3 核心 Prefab 制作指南

为了让游戏运行，你需要手动创建以下 Prefab：

1.  **Bullet (子弹)**:
    *   创建一个 Sprite (2D Object -> Sprites -> Square/Circle)。
    *   命名为 `BulletPrefab`。
    *   添加组件 **Box Collider 2D** (勾选 **Is Trigger**)。
    *   添加组件 **Rigidbody 2D** (Body Type 设置为 **Kinematic**，避免受重力下落)。
    *   添加脚本 **Projectile**。
    *   拖拽到 Project 窗口制作成 Prefab。

2.  **Enemy (敌人)**:
    *   创建一个 Sprite (2D Object -> Sprites -> Square)。
    *   命名为 `EnemyPrefab`。
    *   颜色可以设为红色以示区分。
    *   添加组件 **Box Collider 2D** (不需要勾选 Is Trigger)。
    *   添加组件 **Rigidbody 2D** (Body Type 设置为 **Kinematic**)。
    *   添加脚本 **Enemy**。
    *   **碰撞设置**: 确保 Enemy 的 Collider 可以与 Player 的 Collider 发生碰撞（检查 Physics 2D Matrix）。建议 Enemy 或 Player 至少有一方的 Collider 勾选 **Is Trigger**。
    *   拖拽到 Project 窗口制作成 Prefab。

3.  **引用绑定**:
    *   **WeaponData**: 将 `BulletPrefab` 拖拽到 WeaponData 的 `Projectile Prefab` 字段。
    *   **EnemySpawner**: 将 `EnemyPrefab` 拖拽到场景中 GameRoot/EnemySpawner 组件的 `Enemy Prefab` 字段。

### 3.4 战场区域配置 (Battlefield & Spawn Area)

### 3.5 常见问题排查 (Troubleshooting)

**Q: 敌人穿过玩家但不扣血？**
1.  **缺少 Rigidbody 2D**: 确保 **Player** 和 **Enemy** 中至少有一个挂载了 `Rigidbody 2D` 组件。
    *   推荐两者都挂载，并将 **Body Type** 设置为 **Kinematic**。
2.  **Layer Collision Matrix**: 检查 Project Settings -> Physics 2D 中，Player 和 Enemy 的 Layer 交叉点是否勾选。
3.  **Is Trigger**: 确保 Player 或 Enemy 的 Collider 2D 中至少有一个勾选了 **Is Trigger**。
4.  **脚本**: 确保 Player 挂载了 `PlayerController` 脚本，且 Enemy 挂载了 `Enemy` 脚本。

### 3.6 战场区域配置 (Battlefield & Spawn Area)

为了控制敌人生成和物体销毁范围，请按以下步骤配置场景：

1.  **BattleField (战场范围)**:
    *   在场景中创建一个 Sprite (Square)，命名为 `BattleField`。
    *   调整其大小（Scale）以覆盖整个游戏可视区域。
    *   设置 Sprite 颜色（可以半透明作为背景，或者设为透明仅作逻辑用）。
    *   添加组件 **Box Collider 2D**，勾选 **Is Trigger**。
    *   添加脚本 **BattleFieldBoundary**。
    *   *(注意：确保 BattleField 的 Layer 不会与 Projectile/Enemy 发生物理碰撞，除非你只用 Trigger)*。

2.  **SpawnArea (敌人出生区)**:
    *   在场景中创建一个 Sprite (Square)，命名为 `SpawnArea`。
    *   将其放置在 BattleField 的上方（或者你希望敌人出现的位置）。
    *   调整大小以定义生成的矩形范围。
    *   可以将 Sprite Renderer 关闭（Disable）或者设为透明，使其在游戏中不可见。
    *   选中 **GameRoot**，将 `SpawnArea` 拖拽给 `EnemySpawner` 组件的 **Spawn Area** 字段。

## 4. 开发规范

### 4.1 脚本头部注释模板

所有新建的 C# 脚本 **必须** 包含统一的头部注释，以便于快速理解脚本职责。请复制以下模板并根据实际情况修改：

```csharp
using UnityEngine;

namespace TypeRogue
{
    /// <summary>
    /// [脚本名称]：[一句话描述脚本的主要职责]
    /// 功能：
    /// 1. [核心功能点1]
    /// 2. [核心功能点2]
    /// 3. [核心功能点3]
    /// </summary>
    public class NewScript : MonoBehaviour
    {
        // 代码实现...
    }
```

### 4.2 命名规范

*   **命名空间**: 所有脚本应包含在 `TypeRogue` 命名空间下。
*   **类名**: 使用帕斯卡命名法（PascalCase），如 `BattleHudView`。
*   **私有字段**: 使用 `SerializeField` 的私有字段建议采用驼峰命名法（camelCase），如 `weaponCooldownText`。
*   **公共属性/方法**: 使用帕斯卡命名法（PascalCase）。

### 4.3 架构原则

*   **依赖注入**: 尽量通过 `Initialize` 方法传入依赖，而不是在 `Start` 中使用 `FindObjectOfType`。
*   **事件驱动**: 模块间通信优先使用 C# `event` 或 `Action`，降低耦合度。
