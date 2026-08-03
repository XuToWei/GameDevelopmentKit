# 模块 06：Entity 实体模块

## 概述
实体模块封装 UGF Entity 系统，提供数据（EntityData）与逻辑（EntityLogic）分离的开发模式：实体资源路径通过 `DREntity` 配置表定位，逻辑层通过 `EntityExtension` 扩展方法统一显示/隐藏/等待。

## 目录结构（`Game/Entity/`）
```
Entity/
├── EntityData/
│   ├── AEntityData.cs      # 实体数据基类（IReference 池化）
│   └── ItemEntityData.cs   # 示例：物品实体数据
├── EntityLogic/
│   ├── AEntity.cs          # 实体逻辑基类（EntityLogic）
│   ├── AExEntity.cs        # 扩展实体（含容器等高级能力）
│   └── ItemEntity.cs       # 示例：物品实体逻辑
├── EntityExtension.cs          # 扩展方法（同步）
└── EntityExtension.Awaitable.cs # 扩展方法（UniTask 异步）
```

## 核心类
### AEntityData（数据层）
- 继承 `IReference`，走 GF ReferencePool。
- 包含实体 Id、资源名（通常由 DREntity 表解析）、位置/朝向等数据。
```csharp
public class ItemEntityData : AEntityData
{
    public int ItemId { get; private set; }
    public static ItemEntityData Create(int itemId, Vector3 position, ...) { ... }
    public override void Clear() { ... }
}
```

### AEntity（逻辑层）
- 继承 `EntityLogic`。
- `OnInit`：缓存父 Transform；`OnShow`：编辑器下命名 `[Entity {Id} {name}]`；`OnHide`：恢复父 Transform、还原名字。
- 子类重写 `OnShow/OnUpdate/OnHide` 实现具体逻辑。
```csharp
public class ItemEntity : AEntity
{
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        ItemEntityData data = userData as ItemEntityData; // 取数据
        // 初始化表现
    }
    protected override void OnHide(bool isShutdown, object userData) { base.OnHide(isShutdown, userData); }
}
```

## 实体扩展 API（EntityExtension）
```csharp
// 同步显示
GameEntry.Entity.ShowItem(new ItemEntityData(itemId, position));

// 异步等待（UniTask）
await GameEntry.Entity.ShowEntityAsync<ItemEntity>(entityData);
```

## 设计要点
- **数据/逻辑分离**：EntityData 可池化、可跨场景传递；EntityLogic 只管表现。
- 资源定位统一走 `AssetUtility.GetEntityAsset(assetName)`（`Assets/Res/Entity/{name}.prefab`），配置在 `DREntity` 表中。
- 隐藏实体时若父节点被改变，`AEntity.OnHide` 会自动还原，避免场景树污染。
- 配合 `EntityContainer` 使用可实现窗体级实体生命周期管理（见模块 05）。

## 官方文档
- `Book/Entity开发.md`（GameHot Entity 与 ETEntity 的完整流程）
