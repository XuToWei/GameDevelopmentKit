# [ET动态事件]

可以灵活的增删Entity响应的事件，不依赖委托实现，可以Reload

1.定义事件：
```csharp
[DynamicEvent(SceneType.Client)]
public class Test_DynamicEvent:ADynamicEvent<TestEntity,DynamicEventType.Test>
{
    protected override async ETTask Run(Scene scene, TestEntity self,DynamicEventType.Test arg)
    {
        //to do something
    }
}
```

2.注册实体：
```csharp
testEntity.AddComponent<DynamicEventComponent>();
```
或
```csharp
DynamicEventWatcherComponent.Instance.Register(testEntity);
```

3.反注册实体：
```csharp
testEntity.RemoveComponent<DynamicEventComponent>();
```
或
```csharp
DynamicEventWatcherComponent.Instance.UnRegister(testEntity);
```

4.通知事件：
```csharp
DynamicEventWatcherComponent.Instance.Publish<DynamicEventType.Test>(scene, new DynamicEventType.Test())
```
或
```csharp
await DynamicEventWatcherComponent.Instance.PublishAsync<DynamicEventType.Test>(scene, new DynamicEventType.Test())
```

