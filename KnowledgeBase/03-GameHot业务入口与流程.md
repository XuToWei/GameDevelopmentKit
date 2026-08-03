# 模块 03：GameHot 业务入口与流程

## 概述
GameHot（纯 GF）模式：客户端以 UnityGameFramework 为底座，业务代码全部位于 `Game.Hot.Code` 程序集（可热更），使用 MonoBehaviour + GF 组件驱动。

## 启动链路
```
原生侧 Init.cs (MonoBehaviour, 挂 GameEntry 场景)
  └─ Start → StartAsync()
       ├─ EnableHotfix && CodeBytes 模式：Assembly.Load 热更 DLL (Game.Hot.Code.dll.bytes)
       └─ GameEntry.Resource.LoadAssetAsync → 实例化 HotEntry.prefab 到 CodeRunner.transform
            └─ HotEntry.Start():
                 ├─ InitComponents()：从 HotComponentEntry 取 Procedure/Tables/HPBar 组件
                 ├─ HotComponentEntry.Initialize()：遍历所有 HotComponent 调 OnInitialize
                 └─ Procedure.StartProcedure<ProcedureLaunch>()
            └─ HotEntry.Update()：驱动 HotComponentEntry.Update
            └─ HotEntry.OnDestroy()：逆序 Shutdown
```

## 核心类
| 类 | 职责 | 位置 |
| --- | --- | --- |
| `Init` | 原生加载入口，MonoBehaviour | `Game/Hot/Loader/Init.cs` |
| `HotEntry` | 热更业务入口，管理组件生命周期 | `Game/Hot/Code/Base/HotEntry.cs` |
| `HotComponentEntry` | HotComponent 注册表（按 Priority 排序链表） | `Game/Hot/Loader/Base/HotComponentEntry.cs` |
| `HotComponent` | MonoBehaviour 基类，Awake 自注册 | `Game/Hot/Code/` |
| `ProcedureComponent` | OnInitialize 反射扫描程序集，自动注册全部 `ProcedureBase` 为 FSM 状态 | `Game/Hot/Code/` |

## 流程链（Procedure）
| 流程 | 职责 |
| --- | --- |
| `ProcedureLaunch` | 注册 ProtoBuf 工厂 |
| `ProcedurePreload` | 加载配置/字体/HPBar |
| `ProcedureGame` | 游戏主流程 |
| `ProcedureChangeScene` | 读 DTScene 表加载场景，隐藏实体、停声音 |
| `ProcedureMenu` / `ProcedureMain` | 进菜单 / 进主玩法 |

## Procedure 定义方式
```csharp
public class ProcedureMain : ProcedureBase // FsmState<ProcedureComponent>
{
    protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        // 进入逻辑
    }
    protected override void OnUpdate(IFsm<ProcedureComponent> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        // 切换流程：ChangeState<ProcedureMenu>(procedureOwner);
    }
    // OnLeave...
}
```
- 无需手动注册：`ProcedureComponent.OnInitialize` 反射自动注册。
- 跨流程传参：`procedureOwner.SetData<VarInt32>(键, 值)`。

## Hot/Code 目录组织
| 目录 | 内容 |
| --- | --- |
| `Base/` | HotEntry、HotComponent 基类 |
| `Definition/` | `Constant/`（Layer 等）、`DataStruct/`（ImpactData 等）、`Enum/`（CampType、RelationType） |
| `Procedure/` | 流程类 |
| `UI/` | UIForm 类 |
| `Entity/` | EntityData + EntityLogic + EntityExtension |
| `Scene/` `HPBar/` `Game/` | 场景、血条、游戏模式（GameBase/GameMode/SurvivalGame） |
| `Generate/` | 自动生成：Luban 表、Message、UGFUIFormId/UGFSceneId 等常量 |
| `Tables/` `Utility/` | 配置表访问、工具 |

## 典型开发方式（对比 ET）
| 维度 | GameHot | ET |
| --- | --- | --- |
| 驱动 | MonoBehaviour + GF 组件 | Entity/Component + System |
| UI | `GameEntry.UI.OpenUIForm(UIFormId.TestForm, userData)` | `AddUIFormComponentAsync<T>(id)` |
| 实体 | `EntityExtension.ShowXXX(data)`（按 DREntity 表定位资源） | Unit/Entity 模型 + Handler |
| 场景 | ProcedureChangeScene 统一加载 | Scene Entity + 消息驱动 |
| 网络 | Protobuf + PacketHandler | Fiber / 消息驱动 |

## 官方文档
- `Book/Project结构.md`、`Book/UI开发.md`（GameHot 章节）
