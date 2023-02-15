# TimerExtension

为提供UGF提供一个定时器   定时器是 [ET](https://github.com/egametang/ET) 中的定时器 改造而来  增删了部分功能

# 文件说明

- CancellationToken ---- 取消令牌 
- MultiMap ----  一个有序的 存放多个值的Map   基于SortedDictionary
- TimerComponent ---- 定时器组件 
- TimerTimeUtility ---- 定时器时间工具  基于Utc时间

# 使用说明

Timer 延时时间以毫秒计时 

添加一个一秒后执行的定时器

```csharp 
AddOnceTimer(1000,()=>{Debug.Log("After a second")})
```

