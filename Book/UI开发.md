# UI 开发

GDK 的 UI 配置统一由 GF UIComponent 管理。业务层可选择 GameHot 的 MonoBehaviour 工作流，或 ETUI 的 Entity/System 工作流；两者共享 UI 表、资源路径、UIGroup 和 GF 生命周期。

## 选择实现方式

| 方式 | 业务代码 | 适用场景 |
| --- | --- | --- |
| GameHot UI | `Game/Hot/Code/UI/` | 熟悉 MonoBehaviour、界面逻辑较直接 |
| ETUI | `Game/ET/Code/ModelView` + `HotfixView` | 使用 ET Entity、System、事件和所有权模型 |

先通过 `Game/Define Symbol` 选择模块，再编辑对应模块的 Excel。模式切换会自动改变 ET 与 GameHot 的 `luban.conf.active`。

## 共同配置链路

```text
UI.xlsx
  → ExcelExporter
  → Assets/Res/Editor/Luban/dtuiform.json
  → UIFormId.cs 或 UGFUIFormId.cs
  → GF 根据 AssetName、UIGroupName 加载预制体
```

### UI 表字段

| 字段 | 含义 | 要点 |
| --- | --- | --- |
| `Id` | UI 类型编号 | 全表唯一，运行时打开接口使用 |
| `CSName` | 生成的常量名 | 必须是合法且唯一的 C# 标识符 |
| `Desc` | 描述 | 同时写入生成代码注释 |
| `AssetName` | 相对资源名 | 相对 `Assets/Res/UI/UIForm/`，不含扩展名 |
| `UIGroupName` | GF UI 分组 | 必须已在 GameEntry 的 UIComponent 中配置 |
| `AllowMultiInstance` | 是否允许多实例 | 弹窗类通常开启，主界面通常关闭 |
| `PauseCoveredUIForm` | 被覆盖时是否暂停 | 决定 Pause/Resume 生命周期 |

GameHot 表位于 `Design/Excel/GameHot/Datas/Game/UI.xlsx`，ET 表位于 `Design/Excel/ET/Datas/Game/UI.xlsx`。

## GameHot UI

### 1. 创建预制体

在 `Unity/Assets/Res/UI/UIForm/Hot/` 下创建预制体。例如：

```text
Unity/Assets/Res/UI/UIForm/Hot/TestForm.prefab
```

表中 `AssetName` 应填写 `Hot/TestForm`。

### 2. 添加配置并导表

在 GameHot 的 `UI.xlsx` 增加记录，例如：

| Id | CSName | AssetName | UIGroupName | AllowMultiInstance | PauseCoveredUIForm |
| --- | --- | --- | --- | --- | --- |
| 103 | `TestForm` | `Hot/TestForm` | `Default` | false | true |

执行 `Game/Tool/ExcelExporter`。常量生成到：

```text
Unity/Assets/Scripts/Game/Hot/Code/Generate/UGF/UIFormId.cs
```

### 3. 创建逻辑脚本

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace Game.Hot
{
    public sealed class TestForm : StarForceUIForm
    {
        [SerializeField]
        private Button m_TestButton;

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_TestButton.onClick.AddListener(OnTestButtonClick);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            m_TestButton.onClick.RemoveListener(OnTestButtonClick);
            base.OnClose(isShutdown, userData);
        }

        private void OnTestButtonClick()
        {
            Close();
        }
    }
}
```

把脚本挂到 `TestForm.prefab`。`StarForceUIForm` 已处理字体、本地化文本和淡入淡出；不需要这些约定时可直接继承 `AHotUIForm`。

### 4. 打开界面

```csharp
GameEntry.UI.OpenUIForm(UIFormId.TestForm, userData);
```

需要等待加载结果时，使用项目提供的 `OpenUIFormAsync` 扩展。界面内部调用 `Close()` 即可关闭当前实例。

## ETUI

ETUI 由三部分组成：ModelView 中的 ET 实体、HotfixView 中的 System、预制体上的 Mono 视图。`UGFUIForm<T>` 负责把三者与 GF UIForm 关联起来。

### 1. 使用代码生成器

打开 `ET/Code Creator`，选择 `UIFormCodeCreator`，输入 `Test`。工具生成：

```text
Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI/UITest/UIFormTest.cs
Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI/UITest/MonoUIFormTest.cs
Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI/UITest/UIFormTestSystem.cs
Assets/Res/UI/UIForm/UIFormTest.prefab
```

脚本重载后，工具会把 `MonoUIFormTest` 自动挂到新预制体。生成器不会写 Excel，UI 表记录仍需手工添加。

### 2. 添加 UI 表记录

例如：

| Id | CSName | AssetName | UIGroupName |
| --- | --- | --- | --- |
| 804 | `UITest` | `UIFormTest` | `Default` |

导表后使用 `UGFUIFormId.UITest`。`CSName` 是常量名，不要求与 ET 类名完全相同，但统一命名能减少查找成本。

### 3. 定义实体与视图

生成的 ModelView 代码结构如下：

```csharp
namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public class UIFormTest : UGFUIForm<MonoUIFormTest>,
        IAwake, IDestroy, IUGFUIFormOnOpen, IUGFUIFormOnClose
    {
    }
}
```

`[ComponentOf]` 应按实际所有者补齐。若界面作为 Scene 的 `UIComponent` 组件存在，使用 `ComponentOf(typeof(UIComponent))`；允许多实例时更适合创建为 Child。

Mono 视图只负责绑定 Unity 组件：

```csharp
using CodeBind;

namespace ET.Client
{
    [MonoCodeBind]
    public partial class MonoUIFormTest : AETMonoUGFUIForm
    {
    }
}
```

推荐在预制体上使用 CodeBind 生成按钮、文本和输入框字段，System 通过 `self.View` 访问，避免在 HotfixView 中保存 Unity 序列化字段。

### 4. 编写生命周期 System

```csharp
namespace ET.Client
{
    [EntitySystemOf(typeof(UIFormTest))]
    public static partial class UIFormTestSystem
    {
        [UGFUIFormSystem]
        private static void UGFUIFormOnOpen(this UIFormTest self)
        {
            // self.View.TestButton ...
        }

        [UGFUIFormSystem]
        private static void UGFUIFormOnClose(this UIFormTest self, bool isShutdown)
        {
        }
    }
}
```

只有实现了对应接口的实体才会接收回调。例如需要 `UGFUIFormOnUpdate`，类声明必须加入 `IUGFUIFormOnUpdate`。

### 5. 打开与关闭

单实例界面通常作为 `UIComponent` 的组件：

```csharp
UIFormTest form = await scene.GetComponent<UIComponent>()
    .AddUIFormComponentAsync<UIFormTest>(UGFUIFormId.UITest);
```

多实例界面使用 Child：

```csharp
UIFormTest form = await scene.GetComponent<UIComponent>()
    .AddUIFormChildAsync<UIFormTest>(UGFUIFormId.UITest);
```

移除或 Dispose 这个 ET 实体时，`UGFUIForm.Dispose()` 会取消未完成加载并关闭对应 GF UIForm。不要绕过所有者只关闭 GF 实例，否则 ET 实体仍可能存活。

## 生命周期对应关系

| GF/ETUI 回调 | 触发时机 | 常见用途 |
| --- | --- | --- |
| `IAwake` | ET 实体创建 | 初始化纯逻辑状态 |
| `OnOpen` | 每次打开 | 绑定事件、读取参数、刷新数据 |
| `OnUpdate` | 界面更新 | 少量持续刷新逻辑 |
| `OnPause` / `OnResume` | 被覆盖 / 恢复 | 暂停输入或动画 |
| `OnCover` / `OnReveal` | 遮挡状态变化 | 可见性相关处理 |
| `OnRefocus` | 多实例重新聚焦 | 更新焦点状态 |
| `OnDepthChanged` | UI 深度变化 | Canvas 或特效排序 |
| `OnClose` | 关闭 | 解订阅、停止任务 |
| `OnRecycle` | GF 实例回收 | 清理视图缓存 |
| `IDestroy` | ET 实体销毁 | 清理 ET 侧状态 |

## UIWidget

可复用的小型界面使用 ET UIWidget。`UIWidgetCodeCreator` 会生成 Widget 实体、System 与 Mono 视图；运行时可通过 `LoadChildUIWidgetAsync<T>` 从 UIEntity 表加载，或把现有 `AETMonoUGFUIWidget` 绑定为 Child/Component。

UIWidget 的资源配置使用 `UIEntity.xlsx`，生成常量为 `UGFUIEntityId` 或 `UIEntityId`，不要与 UIForm 表混用。

## 常见问题

### 打开后提示 UIGroup 不存在

检查 `UIGroupName` 是否已配置在 `Unity/Assets/Res/GameEntry.prefab` 的 UIComponent 中。编辑器预加载流程会扫描 UI 表并记录错误。

### 修改 Excel 后找不到常量

确认当前模块的 `luban.conf.active` 为 `true`，再执行 ExcelExporter。不要手改 `UIFormId.cs` 或 `UGFUIFormId.cs`。

### ETUI 的 `View` 为 null

确认预制体挂有与泛型参数一致的 `MonoUIForm*`，并继承 `AETMonoUGFUIForm`。代码生成后还需等待脚本重载完成。

### 允许多实例但第二次打开失败

同时检查表中的 `AllowMultiInstance` 与 ET 所有权。GF 必须允许多实例，ET 侧也应使用 Child，而不是同类型只能存在一个的 Component。

## 关键代码

| 作用 | 文件 |
| --- | --- |
| GameHot UI 基类 | `Game/Hot/Code/UI/StarForceUIForm.cs` |
| ETUI 桥接 | `Game/ET/Loader/UGF/UIForm/UGFUIForm.cs` |
| ET UI 所有者扩展 | `Game/ET/Code/ModelView/Client/Module/UI/UIComponentSystem.cs` |
| ETUI 示例 | `Game/ET/Code/ModelView/Client/Demo/UI/UILogin/` |
| UI 常量生成器 | `Share/Tool/ExcelExporter/Generate/GenerateUGFUIFormId.cs` |
