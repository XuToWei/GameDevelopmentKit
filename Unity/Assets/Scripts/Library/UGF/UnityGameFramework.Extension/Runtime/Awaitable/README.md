# AwaitableExtension

对UnityGameFramework 的await扩展  使ugf 中各个组件 支持await/async 语法

# 使用教程

1. 注册各组件需要的事件

   UGF中各种加载 显示都是通过事件通知的。 修改成await/async 也是基于此 。所以需要调用

   [AwaitableExtension](./AwaitableExtension.cs)  中`SubscribeEvent` 函数来注册事件。 需要在框架的启动流程中调用。

   在启动流程中注册 的目的 也是为了防止调用框架重启 导致事件失效问题。

2. 通过UGF组件直接使用即可。  所有方法均使用扩展方法对UGF组件进行扩展  

   例如：打开一个界面

   ```csharp
   await GameEntry.UI.OpenUIFormAsync(UIFormId.Test);
   ```

   所有扩展 async 方法也支持嵌套 和 Task.WhenAll 等。

# 特殊说明

​	因为使用 web和download 组件时可能无论成功失败都需要返回 所以封装了 [WebResult](./WebResult.cs) 和	 [DownLoadResult](./DownLoadResult.cs) 用于查看请求是否成功 或返回数据。

​	例如:

1. WebRequest

   ```csharp
   WebResult result = await GameEntry.WebRequest.AddWebRequestAsync("url");
   Debug.Log(result.IsError);
   if (!result.IsError)
   {
       Debug.Log(result.Bytes);
   }
   ```

2. DownLoad

   ```csharp
   DownLoadResult result = await GameEntry.Download.AddDownloadAsync("", "");
   Debug.Log(result.IsError);
   if(result.IsError){
       DoSomething();
   }
   ```

   



