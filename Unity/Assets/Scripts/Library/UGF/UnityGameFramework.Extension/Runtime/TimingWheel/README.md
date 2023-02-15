# TimingWheel

时间轮计时器。根据开源库[linys2333/TimingWheel](https://github.com/linys2333/TimingWheel) 修改而来。去除了多线程的部分 并提供ETTask 和 回调注册两种方式。

## 使用方式

计时基于毫秒, 返回状态 bool  `true` 执行成功  `false` 取消执行

1. ETTask   
   ```csharp
   await GameEntry.TimingWheel.AddTaskAsync(TimeSpan.FromMilliseconds(1000));
   ```
2. CallBack 回调方式
```csharp
	var task = GameEntry.TimingWheel.AddTask(TimeSpan.FromMilliseconds(1000), result => { Debug.Log(result); });
```
3. 循环调用计时器 
	可以自行设置  调用类型(帧,毫秒) 调用频率  
	回调参数为 开始时间和任务。
```csharp
	var loopTask = UGFExtensions.GameEntry.TimingWheel.AddLoopTask((startTime,task) =>  
	{  
	    Log.Info($" 每帧延迟 当前时间毫秒:{DateTimeHelper.GetTimestamp(false)} 当前时间秒:{DateTimeHelper.GetTimestamp(true)} 当前帧：{Time.frameCount}");  
	},LoopType.Millisecond,1500);

	//停止循环计时器 再需要停止的地方调用
	loopTask.Stop();
```

## 拓展使用方式

计时器本身只提供了  `添加任务`  `取消任务` `循环任务` API  但是可以基于这些自行实现 暂停 恢复等功能

* 暂停 恢复定时器功能
	```csharp
	private ITimeTask m_Task;
	//创建定时器
	m_Task = GameEntry.TimingWheel.AddTask(TimeSpan.FromMilliseconds(1000), result => { Debug.Log(result); });

	//暂停定时器 就是 记录定时器距离完成剩余时间  取消当前计时器。
	int time = m_Task.TimeoutMs - DateTimeHelper.GetTimestamp()//剩余时间= 结束时间-当前时间
	m_Task.Cancel();

	//恢复 在恢复时候重新创建定时器即可
	m_Task = GameEntry.TimingWheel.AddTask(TimeSpan.FromMilliseconds(1000), result => { Debug.Log(result); });
	```


# 引用
[linys2333/TimingWheel: c#版分层时间轮算法，参考kafka TimingWheel实现 (github.com)](https://github.com/linys2333/TimingWheel)