public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// CommandLine.dll
	// ET.dll
	// Game.dll
	// LubanLib.dll
	// MemoryPack.dll
	// MongoDB.Driver.Core.dll
	// MongoDB.Driver.dll
	// System.Core.dll
	// System.Runtime.CompilerServices.Unsafe.dll
	// System.dll
	// UniTask.dll
	// UnityEngine.CoreModule.dll
	// UnityGameFramework.Extension.dll
	// UnityGameFramework.Runtime.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<object>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.RobotCase_SecondCaseWait>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.RobotCase_SecondCaseWait>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTask<byte>
	// Cysharp.Threading.Tasks.UniTask<ET.RobotCase_SecondCaseWait>
	// Cysharp.Threading.Tasks.UniTask<long>
	// Cysharp.Threading.Tasks.UniTask<int>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask<uint>
	// Cysharp.Threading.Tasks.UniTask<ET.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.RobotCase_SecondCaseWait>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<long>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<int>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<byte>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<uint>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>
	// ET.AEvent<object,ET.EventType.ChangePosition>
	// ET.AEvent<object,ET.Server.EventType.UnitLeaveSightRange>
	// ET.AEvent<object,ET.EventType.EntryEvent2>
	// ET.AEvent<object,ET.Server.NetInnerComponentOnRead>
	// ET.AEvent<object,ET.Server.NetServerComponentOnRead>
	// ET.AEvent<object,ET.EventType.NumbericChange>
	// ET.AEvent<object,ET.Client.NetClientComponentOnRead>
	// ET.AEvent<object,ET.Server.EventType.UnitEnterSightRange>
	// ET.AEvent<object,ET.EventType.EntryEvent3>
	// ET.AEvent<object,ET.EventType.EntryEvent1>
	// ET.AEvent<object,ET.EventType.AfterCreateClientScene>
	// ET.AEvent<object,ET.EventType.LSSceneChangeStart>
	// ET.AEvent<object,ET.EventType.AfterCreateCurrentScene>
	// ET.AEvent<object,ET.EventType.SceneChangeStart>
	// ET.AEvent<object,ET.EventType.SceneChangeFinish>
	// ET.AEvent<object,ET.EventType.ChangeRotation>
	// ET.AEvent<object,ET.EventType.LoginFinish>
	// ET.AEvent<object,ET.EventType.AppStartInitFinish>
	// ET.AEvent<object,ET.EventType.AfterUnitCreate>
	// ET.AEvent<object,ET.EventType.LSSceneInitFinish>
	// ET.AInvokeHandler<ET.EventType.OnApplicationPause>
	// ET.AInvokeHandler<ET.EventType.OnShutdown>
	// ET.AInvokeHandler<ET.EventType.OnApplicationFocus>
	// ET.AInvokeHandler<ET.ConfigComponent.LoadOne,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.NavmeshComponent.RecastFileLoader,object>
	// ET.AInvokeHandler<ET.ConfigComponent.LoadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.ReloadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.ATimer<object>
	// ET.AwakeSystem<object>
	// ET.AwakeSystem<object,ET.Server.MailboxType>
	// ET.AwakeSystem<object,System.Net.Sockets.AddressFamily>
	// ET.AwakeSystem<object,int>
	// ET.AwakeSystem<object,object>
	// ET.AwakeSystem<object,object,object>
	// ET.AwakeSystem<object,object,int>
	// ET.AwakeSystem<object,long,object>
	// ET.AwakeSystem<object,int,Unity.Mathematics.float3>
	// ET.AwakeSystem<object,object,object,int>
	// ET.DestroySystem<object>
	// ET.EntityRef<object>
	// ET.HashSetComponent<object>
	// ET.IAwake<int>
	// ET.IAwake<object>
	// ET.IAwake<ET.Server.MailboxType>
	// ET.IAwake<System.Net.Sockets.AddressFamily>
	// ET.IAwake<long,object>
	// ET.IAwake<int,Unity.Mathematics.float3>
	// ET.IAwake<object,object>
	// ET.IAwake<object,int>
	// ET.IAwake<object,object,int>
	// ET.LateUpdateSystem<object>
	// ET.ListComponent<long>
	// ET.ListComponent<Cysharp.Threading.Tasks.UniTask>
	// ET.ListComponent<Unity.Mathematics.float3>
	// ET.ListComponent<object>
	// ET.LoadSystem<object>
	// ET.MultiMap<int,object>
	// ET.Singleton<object>
	// ET.UnOrderMultiMap<object,object>
	// ET.UpdateSystem<object>
	// MemoryPack.Formatters.ArrayFormatter<byte>
	// MemoryPack.Formatters.ArrayFormatter<ET.LSInput>
	// MemoryPack.Formatters.ArrayFormatter<object>
	// MemoryPack.Formatters.DictionaryFormatter<int,long>
	// MemoryPack.Formatters.DictionaryFormatter<long,ET.LSInput>
	// MemoryPack.Formatters.ListFormatter<object>
	// MemoryPack.Formatters.ListFormatter<Unity.Mathematics.float3>
	// MemoryPack.Formatters.ListFormatter<long>
	// MemoryPack.IMemoryPackable<object>
	// MemoryPack.IMemoryPackable<ET.LSInput>
	// MemoryPack.MemoryPackFormatter<object>
	// MemoryPack.MemoryPackFormatter<ET.LSInput>
	// MongoDB.Driver.IMongoCollection<object>
	// System.Action<object>
	// System.Action<long,int>
	// System.Action<long,object>
	// System.Action<long,long,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary<long,ET.LSInput>
	// System.Collections.Generic.Dictionary<long,long>
	// System.Collections.Generic.Dictionary<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary.Enumerator<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.HashSet<ushort>
	// System.Collections.Generic.HashSet<long>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet.Enumerator<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,ET.LSInput>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<Unity.Mathematics.float3>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedDictionary<object,object>
	// System.Collections.Generic.SortedDictionary<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<object,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<object,object>
	// System.Func<object>
	// System.Func<Cysharp.Threading.Tasks.UniTask>
	// System.Func<object,object>
	// System.Func<object,object,object>
	// System.Nullable<System.Threading.CancellationTokenRegistration>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.Task<System.ValueTuple<uint,uint>>
	// System.ValueTuple<object,int>
	// System.ValueTuple<uint,object>
	// System.ValueTuple<uint,uint>
	// }}

	public void RefMethods()
	{
		// string Bright.Common.StringUtil.CollectionToString<int>(System.Collections.Generic.IEnumerable<int>)
		// string Bright.Common.StringUtil.CollectionToString<object,object>(System.Collections.Generic.IDictionary<object,object>)
		// CommandLine.ParserResult<object> CommandLine.Parser.ParseArguments<object>(System.Collections.Generic.IEnumerable<string>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithNotParsed<object>(CommandLine.ParserResult<object>,System.Action<System.Collections.Generic.IEnumerable<CommandLine.Error>>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithParsed<object>(CommandLine.ParserResult<object>,System.Action<object>)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2Room_ReconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2Room_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2Room_CheckHashHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2Room_CheckHashHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MatchComponentSystem.<Match>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MatchComponentSystem.<Match>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ET.LubanLoadAllAsyncHandler.<Handle>d__0>(System.Runtime.CompilerServices.TaskAwaiter&,ET.LubanLoadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2Match_MatchHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2Match_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ET.LubanLoadOneAsyncHandler.<Handle>d__0>(System.Runtime.CompilerServices.TaskAwaiter&,ET.LubanLoadOneAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FrameMessageHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FrameMessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Entry.<StartAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Entry.<StartAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_MatchHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.NetServerComponentOnReadEvent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.NetServerComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.NetInnerComponentOnReadEvent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.NetInnerComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ET.LubanReloadAllAsyncHandler.<Handle>d__0>(System.Runtime.CompilerServices.TaskAwaiter&,ET.LubanReloadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorMessageDispatcherComponentHelper.<Handle>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorMessageDispatcherComponentHelper.<Handle>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Entry.<Test2>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Entry.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,ET.Server.ObjectGetRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,ET.Server.ObjectGetRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectAddRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectAddRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__9>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationProxyComponentSystem.<AddLocation>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationProxyComponentSystem.<AddLocation>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Remove>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Remove>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<UnLock>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<UnLock>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Lock>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Lock>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Add>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Add>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Lock>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Lock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Remove>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Remove>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Add>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Entry.<Test1>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Entry.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Entry.<Test1>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Entry.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Entry.<Test2>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Entry.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.HttpGetRouterHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.HttpGetRouterHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCaseSystem.<NewRobot>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCaseSystem.<NewRobot>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseSystem.<NewZoneRobot>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseSystem.<NewZoneRobot>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ChangePosition_NotifyAOI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ChangePosition_NotifyAOI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.R2G_GetLoginKeyHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_PingHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_PingHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_LoginGateHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_LoginGateHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_EnterMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_BenchmarkHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_BenchmarkHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.BenchmarkClientComponentSystem.<Start>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.BenchmarkClientComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.BenchmarkClientComponentSystem.<Start>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.BenchmarkClientComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.EntryEvent2_InitServer.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.EntryEvent2_InitServer.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ReloadDllConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ReloadConfigConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.EntryEvent1_InitShare.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_TestRobotCaseHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2M_SessionDisconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_PathfindingResultHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_StopHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectLockRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseSystem.<NewRobot>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseSystem.<NewRobot>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.CreateRobotConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.CreateRobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.CreateRobotConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.CreateRobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.RobotCase_SecondCaseWait>,ET.Server.RobotCase_SecondCase.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.RobotCase_SecondCaseWait>&,ET.Server.RobotCase_SecondCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCase_SecondCase.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCase_SecondCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.M2C_TestRobotCase2Handler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.M2C_TestRobotCase2Handler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCase_FirstCase.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCase_FirstCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCaseSystem.<NewZoneRobot>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCaseSystem.<NewZoneRobot>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCase_FirstCase.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCase_FirstCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.TransferHelper.<Transfer>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.TransferHelper.<Transfer>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.TransferHelper.<TransferAtFrameFinish>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_TransferMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_TransferMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Server.MoveHelper.<FindPathMoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2R_LoginHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2R_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectUnLockRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectUnLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectRemoveRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectRemoveRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Server.DBComponentSystem.<Query>d__6>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Server.DBComponentSystem.<Query>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.OneFrameInputsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.OneFrameInputsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_CheckHashFailHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_EnterMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorHandleHelper.<>c__DisplayClass0_0.<<Reply>g__HandleMessageInNextFrame|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorHandleHelper.<>c__DisplayClass0_0.<<Reply>g__HandleMessageInNextFrame|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorMessageSenderComponentSystem.<>c__DisplayClass5_0.<<Send>g__HandleMessageInNextFrame|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorMessageSenderComponentSystem.<>c__DisplayClass5_0.<<Send>g__HandleMessageInNextFrame|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.DBComponentSystem.<Query>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.DBComponentSystem.<Query>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.UGFUIUILSLobbySystem.<EnterMap>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.UGFUIUILSLobbySystem.<EnterMap>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LSSceneInitFinish_Finish.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LSSceneInitFinish_Finish.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.UGFUILobbySystem.<EnterMap>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.UGFUILobbySystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.EntryEvent3_InitClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.EntryEvent3_InitClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneInitFinish_Finish.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneInitFinish_Finish.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.WaitType.Wait_Room2C_Start>,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.WaitType.Wait_Room2C_Start>&,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.DBComponentSystem.<Save>d__12>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.DBComponentSystem.<Save>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Server.DBComponentSystem.<Save>d__12>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Server.DBComponentSystem.<Save>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ARobotCase.<Handle>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ARobotCase.<Handle>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>,ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>&,ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.EnterMapHelper.<Match>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.EnterMapHelper.<Match>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginHelper.<Login>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LoginHelper.<Login>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_StopHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Client.MoveHelper.<MoveToAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.PingComponentSystem.<PingAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.G2C_ReconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.G2C_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.PingComponentSystem.<PingAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.NetClientComponentOnReadEvent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.NetClientComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>&,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterAddressComponentSystem.<Init>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>&,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectRemoveRequestHandler.<Run>d__0>(ET.Server.ObjectRemoveRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectAddRequestHandler.<Run>d__0>(ET.Server.ObjectAddRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectUnLockRequestHandler.<Run>d__0>(ET.Server.ObjectUnLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectLockRequestHandler.<Run>d__0>(ET.Server.ObjectLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>(ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectGetRequestHandler.<Run>d__0>(ET.Server.ObjectGetRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.NetClientComponentOnReadEvent.<Run>d__0>(ET.Client.NetClientComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.DBComponentSystem.<Save>d__12>(ET.Server.DBComponentSystem.<Save>d__12&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>(ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.G2C_ReconnectHandler.<Run>d__0>(ET.Client.G2C_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.DBComponentSystem.<Query>d__6>(ET.Server.DBComponentSystem.<Query>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<Init>d__1>(ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.PingComponentSystem.<PingAsync>d__2>(ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.MoveHelper.<MoveToAsync>d__1>(ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_StopHandler.<Run>d__0>(ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginHelper.<Login>d__0>(ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EnterMapHelper.<Match>d__1>(ET.Client.EnterMapHelper.<Match>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ARobotCase.<Handle>d__1>(ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.OneFrameInputsHandler.<Run>d__0>(ET.Client.OneFrameInputsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageSenderComponentSystem.<>c__DisplayClass5_0.<<Send>g__HandleMessageInNextFrame|0>d>(ET.Server.ActorMessageSenderComponentSystem.<>c__DisplayClass5_0.<<Send>g__HandleMessageInNextFrame|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>(ET.Client.Room2C_CheckHashFailHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageHandler.<Handle>d__1<object,object>>(ET.Server.ActorMessageHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageLocationHandler.<Handle>d__1<object,object>>(ET.Server.ActorMessageLocationHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageLocationHandler.<Handle>d__1<object,object,object>>(ET.Server.ActorMessageLocationHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageHandler.<Handle>d__1<object,object,object>>(ET.Server.ActorMessageHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MessageHandler.<HandleAsync>d__2<object,object>>(ET.Server.MessageHandler.<HandleAsync>d__2<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>(ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>(ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.UGFUIUILSLobbySystem.<EnterMap>d__2>(ET.Client.UGFUIUILSLobbySystem.<EnterMap>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>(ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>(ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneInitFinish_Finish.<Run>d__0>(ET.Client.LSSceneInitFinish_Finish.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>(ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>(ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>(ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.UGFUILobbySystem.<EnterMap>d__3>(ET.Client.UGFUILobbySystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>(ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EntryEvent3_InitClient.<Run>d__0>(ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>(ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__9>(ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__9&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorHandleHelper.<>c__DisplayClass0_0.<<Reply>g__HandleMessageInNextFrame|0>d>(ET.Server.ActorHandleHelper.<>c__DisplayClass0_0.<<Reply>g__HandleMessageInNextFrame|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_EnterMapHandler.<Run>d__0>(ET.Client.Room2C_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<AddLocation>d__8>(ET.Server.LocationProxyComponentSystem.<AddLocation>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Remove>d__6>(ET.Server.LocationProxyComponentSystem.<Remove>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCaseSystem.<NewZoneRobot>d__3>(ET.Server.RobotCaseSystem.<NewZoneRobot>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_EnterMapHandler.<Run>d__0>(ET.Server.C2G_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCaseSystem.<NewZoneRobot>d__4>(ET.Server.RobotCaseSystem.<NewZoneRobot>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_BenchmarkHandler.<Run>d__0>(ET.Server.C2G_BenchmarkHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>(ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.BenchmarkClientComponentSystem.<Start>d__1>(ET.Server.BenchmarkClientComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.EntryEvent2_InitServer.<Run>d__0>(ET.Server.EntryEvent2_InitServer.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ReloadDllConsoleHandler.<Run>d__0>(ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ReloadConfigConsoleHandler.<Run>d__0>(ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotConsoleHandler.<Run>d__0>(ET.Server.RobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.HttpGetRouterHandler.<Handle>d__0>(ET.Server.HttpGetRouterHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.NetInnerComponentOnReadEvent.<Run>d__0>(ET.Server.NetInnerComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.NetServerComponentOnReadEvent.<Run>d__0>(ET.Server.NetServerComponentOnReadEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_MatchHandler.<Run>d__0>(ET.Server.C2G_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_PathfindingResultHandler.<Run>d__0>(ET.Server.C2M_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>(ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_LoginGateHandler.<Run>d__0>(ET.Server.C2G_LoginGateHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_PingHandler.<Run>d__0>(ET.Server.C2G_PingHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCaseSystem.<NewRobot>d__2>(ET.Server.RobotCaseSystem.<NewRobot>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_StopHandler.<Run>d__0>(ET.Server.C2M_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>(ET.Server.MoveHelper.<FindPathMoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_TransferMapHandler.<Run>d__0>(ET.Server.C2M_TransferMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>(ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>(ET.Server.TransferHelper.<TransferAtFrameFinish>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.TransferHelper.<Transfer>d__1>(ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>(ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>(ET.Server.C2M_TestRobotCaseHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FrameMessageHandler.<Run>d__0>(ET.Server.FrameMessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ChangePosition_NotifyAOI.<Run>d__0>(ET.Server.ChangePosition_NotifyAOI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2R_LoginHandler.<Run>d__0>(ET.Server.C2R_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCase_FirstCase.<Run>d__0>(ET.Server.RobotCase_FirstCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>(ET.Server.R2G_GetLoginKeyHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.M2C_TestRobotCase2Handler.<Run>d__0>(ET.Server.M2C_TestRobotCase2Handler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCase_SecondCase.<Run>d__0>(ET.Server.RobotCase_SecondCase.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.CreateRobotConsoleHandler.<Run>d__0>(ET.Server.CreateRobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>(ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotCaseSystem.<NewRobot>d__1>(ET.Server.RobotCaseSystem.<NewRobot>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>(ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7>(ET.Server.ActorLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8>(ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorMessage>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EntryEvent1_InitShare.<Run>d__0>(ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanReloadAllAsyncHandler.<Handle>d__0>(ET.LubanReloadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanLoadOneAsyncHandler.<Handle>d__0>(ET.LubanLoadOneAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanLoadAllAsyncHandler.<Handle>d__0>(ET.LubanLoadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Entry.<StartAsync>d__4>(ET.Entry.<StartAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7>(ET.Server.ActorMessageDispatcherComponentHelper.<HandleIActorRequest>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Entry.<Test2>d__3>(ET.Entry.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Add>d__1>(ET.Server.LocationOneTypeSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Remove>d__2>(ET.Server.LocationOneTypeSystem.<Remove>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Lock>d__3>(ET.Server.LocationOneTypeSystem.<Lock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Add>d__3>(ET.Server.LocationProxyComponentSystem.<Add>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Lock>d__4>(ET.Server.LocationProxyComponentSystem.<Lock>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<UnLock>d__5>(ET.Server.LocationProxyComponentSystem.<UnLock>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Entry.<Test1>d__2>(ET.Entry.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ActorMessageDispatcherComponentHelper.<Handle>d__6>(ET.Server.ActorMessageDispatcherComponentHelper.<Handle>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>(ET.Server.G2M_SessionDisconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2Match_MatchHandler.<Run>d__0>(ET.Server.G2Match_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MatchComponentSystem.<Match>d__0>(ET.Server.MatchComponentSystem.<Match>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2Room_CheckHashHandler.<Run>d__0>(ET.Server.C2Room_CheckHashHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2Room_ReconnectHandler.<Run>d__0>(ET.Server.G2Room_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCaseSystem.<NewRobot>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCaseSystem.<NewRobot>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.SessionSystem.<Call>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.SessionSystem.<Call>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotSceneFactory.<Create>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotSceneFactory.<Create>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.UIComponentSystem.<OpenUIFormAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.UIComponentSystem.<OpenUIFormAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.SceneFactory.<CreateClientScene>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.SceneFactory.<CreateClientScene>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterHelper.<Connect>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<uint>,ET.Client.RouterHelper.<GetRouterAddress>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<uint>&,ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotManagerComponentSystem.<NewRobot>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotManagerComponentSystem.<NewRobot>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RoomManagerComponentSystem.<CreateServerRoom>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RoomManagerComponentSystem.<CreateServerRoom>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,ET.Client.RouterHelper.<CreateRouterSession>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Client.HttpClientHelper.<Get>d__0>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>,ET.Client.MoveHelper.<MoveToAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>&,ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorMessageSenderComponentSystem.<Call>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorMessageSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotManagerComponentSystem.<NewRobot>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotManagerComponentSystem.<NewRobot>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseComponentSystem.<New>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseComponentSystem.<New>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseSystem.<NewRobot>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseSystem.<NewRobot>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCaseSystem.<NewRobot>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCaseSystem.<NewRobot>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Get>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Get>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Get>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseSystem.<NewRobot>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseSystem.<NewRobot>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorLocationSenderComponentSystem.<Call>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorLocationSenderComponentSystem.<Call>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,ET.Server.ActorLocationSenderComponentSystem.<Call>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,ET.Server.ActorLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.RobotCaseSystem.<NewRobot>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.RobotCaseSystem.<NewRobot>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorLocationSenderComponentSystem.<Call>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorMessageSenderComponentSystem.<Call>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorMessageSenderComponentSystem.<Call>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.SessionSystem.<Call>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.SessionSystem.<Call>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.SceneFactory.<CreateServerScene>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.SceneFactory.<CreateServerScene>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.MoveComponentSystem.<MoveToAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotCaseComponentSystem.<New>d__3>(ET.Server.RobotCaseComponentSystem.<New>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_CreateMyUnit>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_CreateMyUnit>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.Start<ET.Client.MoveHelper.<MoveToAsync>d__0>(ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.UIComponentSystem.<OpenUIFormAsync>d__2>(ET.Client.UIComponentSystem.<OpenUIFormAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RoomManagerComponentSystem.<CreateServerRoom>d__0>(ET.Server.RoomManagerComponentSystem.<CreateServerRoom>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_UnitStop>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_UnitStop>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_UnitStop>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.SceneFactory.<CreateClientScene>d__0>(ET.Client.SceneFactory.<CreateClientScene>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_SceneChangeFinish>>(ET.ObjectWaitSystem.<Wait>d__4<ET.Client.Wait_SceneChangeFinish>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.RobotCase_SecondCaseWait>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.RobotCase_SecondCaseWait>>(ET.ObjectWaitSystem.<Wait>d__4<ET.RobotCase_SecondCaseWait>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.Start<ET.Client.RouterHelper.<Connect>d__2>(ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ActorLocationSenderComponentSystem.<Call>d__10>(ET.Server.ActorLocationSenderComponentSystem.<Call>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotSceneFactory.<Create>d__0>(ET.Server.RobotSceneFactory.<Create>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.SessionSystem.<Call>d__4>(ET.SessionSystem.<Call>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.Start<ET.Server.LocationOneTypeSystem.<Get>d__5>(ET.Server.LocationOneTypeSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.Start<ET.Client.RouterHelper.<GetRouterAddress>d__1>(ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.RouterHelper.<CreateRouterSession>d__0>(ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ActorLocationSenderComponentSystem.<Call>d__8>(ET.Server.ActorLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.Start<ET.Server.LocationProxyComponentSystem.<Get>d__7>(ET.Server.LocationProxyComponentSystem.<Get>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.HttpClientHelper.<Get>d__0>(ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ActorMessageSenderComponentSystem.<Call>d__7>(ET.Server.ActorMessageSenderComponentSystem.<Call>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.SessionSystem.<Call>d__3>(ET.SessionSystem.<Call>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotManagerComponentSystem.<NewRobot>d__0>(ET.Server.RobotManagerComponentSystem.<NewRobot>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ActorMessageSenderComponentSystem.<Call>d__8>(ET.Server.ActorMessageSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotCaseSystem.<NewRobot>d__7>(ET.Server.RobotCaseSystem.<NewRobot>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.WaitType.Wait_Room2C_Start>.Start<ET.ObjectWaitSystem.<Wait>d__4<ET.WaitType.Wait_Room2C_Start>>(ET.ObjectWaitSystem.<Wait>d__4<ET.WaitType.Wait_Room2C_Start>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11>(ET.Server.ActorLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.SceneFactory.<CreateServerScene>d__0>(ET.Server.SceneFactory.<CreateServerScene>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotCaseSystem.<NewRobot>d__5>(ET.Server.RobotCaseSystem.<NewRobot>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<ET.MoveComponentSystem.<MoveToAsync>d__5>(ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotCaseSystem.<NewRobot>d__6>(ET.Server.RobotCaseSystem.<NewRobot>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AI_Attack.<Execute>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.ConsoleComponentSystem.<Start>d__3>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ConsoleComponentSystem.<Start>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.HttpComponentSystem.<Handle>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.HttpComponentSystem.<Handle>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Server.HttpComponentSystem.<Accept>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Server.HttpComponentSystem.<Accept>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Client.AI_XunLuo.<Execute>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Server.HttpComponentSystem.<Accept>d__4>(ET.Server.HttpComponentSystem.<Accept>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.AI_XunLuo.<Execute>d__1>(ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.AI_Attack.<Execute>d__1>(ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Server.HttpComponentSystem.<Handle>d__5>(ET.Server.HttpComponentSystem.<Handle>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.ConsoleComponentSystem.<Start>d__3>(ET.ConsoleComponentSystem.<Start>d__3&)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<byte>(Cysharp.Threading.Tasks.UniTask<byte>)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<object>(Cysharp.Threading.Tasks.UniTask<object>)
		// object ET.Entity.AddChild<object>(bool)
		// object ET.Entity.AddChild<object,object>(object,bool)
		// object ET.Entity.AddChild<object,int>(int,bool)
		// object ET.Entity.AddChild<object,object,object,int>(object,object,int,bool)
		// object ET.Entity.AddChild<object,long,object>(long,object,bool)
		// object ET.Entity.AddChildWithId<object>(long,bool)
		// object ET.Entity.AddChildWithId<object,object>(long,object,bool)
		// object ET.Entity.AddChildWithId<object,int>(long,int,bool)
		// object ET.Entity.AddComponent<object,int>(int,bool)
		// object ET.Entity.AddComponent<object,object>(object,bool)
		// object ET.Entity.AddComponent<object,ET.Server.MailboxType>(ET.Server.MailboxType,bool)
		// object ET.Entity.AddComponent<object,object,object>(object,object,bool)
		// object ET.Entity.AddComponent<object,System.Net.Sockets.AddressFamily>(System.Net.Sockets.AddressFamily,bool)
		// object ET.Entity.AddComponent<object,object,int>(object,int,bool)
		// object ET.Entity.AddComponent<object,int,Unity.Mathematics.float3>(int,Unity.Mathematics.float3,bool)
		// object ET.Entity.AddComponent<object>(bool)
		// object ET.Entity.AddComponentWithId<object>(long,bool)
		// object ET.Entity.GetChild<object>(long)
		// object ET.Entity.GetComponent<object>()
		// object ET.Entity.GetParent<object>()
		// System.Void ET.Entity.RemoveComponent<object>()
		// ET.SceneType ET.EnumHelper.FromString<ET.SceneType>(string)
		// object ET.EventSystem.Invoke<ET.NavmeshComponent.RecastFileLoader,object>(ET.NavmeshComponent.RecastFileLoader)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.Invoke<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>(int,ET.Server.RobotInvokeArgs)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnApplicationPause>(object,ET.EventType.OnApplicationPause)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterCreateCurrentScene>(object,ET.EventType.AfterCreateCurrentScene)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnShutdown>(object,ET.EventType.OnShutdown)
		// System.Void ET.EventSystem.Publish<object,ET.Server.NetServerComponentOnRead>(object,ET.Server.NetServerComponentOnRead)
		// System.Void ET.EventSystem.Publish<object,ET.Server.NetInnerComponentOnRead>(object,ET.Server.NetInnerComponentOnRead)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.MoveStop>(object,ET.EventType.MoveStop)
		// System.Void ET.EventSystem.Publish<object,ET.Server.EventType.UnitLeaveSightRange>(object,ET.Server.EventType.UnitLeaveSightRange)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.SceneChangeStart>(object,ET.EventType.SceneChangeStart)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterUnitCreate>(object,ET.EventType.AfterUnitCreate)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.EnterMapFinish>(object,ET.EventType.EnterMapFinish)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.OnApplicationFocus>(object,ET.EventType.OnApplicationFocus)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.LSSceneInitFinish>(object,ET.EventType.LSSceneInitFinish)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.NumbericChange>(object,ET.EventType.NumbericChange)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.ChangePosition>(object,ET.EventType.ChangePosition)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.ChangeRotation>(object,ET.EventType.ChangeRotation)
		// System.Void ET.EventSystem.Publish<object,ET.Server.EventType.UnitEnterSightRange>(object,ET.Server.EventType.UnitEnterSightRange)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.AfterCreateClientScene>(object,ET.EventType.AfterCreateClientScene)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.MoveStart>(object,ET.EventType.MoveStart)
		// System.Void ET.EventSystem.Publish<object,ET.EventType.SceneChangeFinish>(object,ET.EventType.SceneChangeFinish)
		// System.Void ET.EventSystem.Publish<object,ET.Client.NetClientComponentOnRead>(object,ET.Client.NetClientComponentOnRead)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent3>(object,ET.EventType.EntryEvent3)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent2>(object,ET.EventType.EntryEvent2)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.AppStartInitFinish>(object,ET.EventType.AppStartInitFinish)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.EntryEvent1>(object,ET.EventType.EntryEvent1)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.LoginFinish>(object,ET.EventType.LoginFinish)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EventType.LSSceneChangeStart>(object,ET.EventType.LSSceneChangeStart)
		// object ET.Game.AddSingleton<object>()
		// object ET.JsonHelper.FromJson<object>(string)
		// object ET.MongoHelper.Deserialize<object>(byte[])
		// System.Void ET.MongoHelper.RegisterStruct<ET.LSInput>()
		// System.Void ET.ObjectHelper.Swap<object>(object&,object&)
		// System.Void ET.RandomGenerator.BreakRank<object>(System.Collections.Generic.List<object>)
		// object ET.RandomGenerator.RandomArray<object>(System.Collections.Generic.List<object>)
		// string ET.StringHelper.ArrayToString<float>(float[])
		// Cysharp.Threading.Tasks.UniTask<UnityGameFramework.Runtime.Entity> Game.EntityExtension.ShowEntityAsync<object>(UnityGameFramework.Runtime.EntityComponent,int,object,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// System.Void MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&,System.Collections.Generic.List<object>&)
		// System.Collections.Generic.List<object> MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&)
		// System.Void MemoryPack.Formatters.ListFormatter.SerializePackable<object>(MemoryPack.MemoryPackWriter&,System.Collections.Generic.List<object>&)
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<ET.LSInput>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<object>()
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<object>(MemoryPack.MemoryPackFormatter<object>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<ET.LSInput>(MemoryPack.MemoryPackFormatter<ET.LSInput>)
		// System.Void MemoryPack.MemoryPackReader.ReadPackable<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadPackable<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<ET.LSInput>(ET.LSInput&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte>(byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long>(long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int>(byte&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int>(byte&,int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long>(byte&,int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,ET.LSInput>(byte&,int&,long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,long>(byte&,int&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,long,int>(byte&,int&,int&,long&,long&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long,long>(long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<TrueSync.TSVector>(TrueSync.TSVector&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,TrueSync.TSVector,TrueSync.TSQuaternion>(byte&,long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long>(byte&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,long>(byte&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,long,long>(byte&,int&,int&,long&,long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long>(byte&,int&,int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,long>(int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<TrueSync.TSQuaternion>(TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,Unity.Mathematics.float3>(byte&,int&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,int,int,Unity.Mathematics.float3,Unity.Mathematics.float3>(byte&,long&,int&,int&,Unity.Mathematics.float3&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,Unity.Mathematics.float3>(byte&,long&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,uint>(byte&,uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<uint>(uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,Unity.Mathematics.float3,Unity.Mathematics.quaternion>(byte&,int&,long&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.quaternion,int>(Unity.Mathematics.quaternion&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.float3>(Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,int>(int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.quaternion>(Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,int,long>(byte&,long&,int&,long&)
		// byte[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<byte>()
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanagedArray<byte>(byte[]&)
		// object MemoryPack.MemoryPackReader.ReadValue<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadValue<object>(object&)
		// System.Void MemoryPack.MemoryPackWriter.WritePackable<object>(object&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long>(long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long,long>(long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<ET.LSInput>(ET.LSInput&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<int,long>(int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<Unity.Mathematics.quaternion,int>(Unity.Mathematics.quaternion&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedArray<byte>(byte[])
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,long,long>(byte,byte&,int&,int&,long&,long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,long,int>(byte,byte&,int&,int&,long&,long&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,long>(byte,byte&,int&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long>(byte,byte&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int>(byte,byte&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int>(byte,byte&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long>(byte,byte&,int&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,long>(byte,byte&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,int,long>(byte,byte&,long&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<int,int>(byte,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,Unity.Mathematics.float3>(byte,byte&,long&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,Unity.Mathematics.float3,Unity.Mathematics.quaternion>(byte,byte&,int&,long&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,ET.LSInput>(byte,byte&,int&,long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,int,int,Unity.Mathematics.float3,Unity.Mathematics.float3>(byte,byte&,long&,int&,int&,Unity.Mathematics.float3&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,TrueSync.TSVector,TrueSync.TSQuaternion>(byte,byte&,long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long>(byte,byte&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte>(byte,byte&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,uint>(byte,byte&,uint&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,Unity.Mathematics.float3>(byte,byte&,int&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<object>(object&)
		// System.Threading.Tasks.Task<object> MongoDB.Driver.IAsyncCursorExtensions.FirstOrDefaultAsync<object>(MongoDB.Driver.IAsyncCursor<object>,System.Threading.CancellationToken)
		// System.Threading.Tasks.Task<MongoDB.Driver.IAsyncCursor<object>> MongoDB.Driver.IMongoCollectionExtensions.FindAsync<object>(MongoDB.Driver.IMongoCollection<object>,System.Linq.Expressions.Expression<System.Func<object,bool>>,MongoDB.Driver.FindOptions<object,object>,System.Threading.CancellationToken)
		// System.Threading.Tasks.Task<MongoDB.Driver.ReplaceOneResult> MongoDB.Driver.IMongoCollectionExtensions.ReplaceOneAsync<object>(MongoDB.Driver.IMongoCollection<object>,System.Linq.Expressions.Expression<System.Func<object,bool>>,object,MongoDB.Driver.ReplaceOptions,System.Threading.CancellationToken)
		// MongoDB.Driver.IMongoCollection<object> MongoDB.Driver.IMongoDatabase.GetCollection<object>(string,MongoDB.Driver.MongoCollectionSettings)
		// ET.Client.Wait_UnitStop System.Activator.CreateInstance<ET.Client.Wait_UnitStop>()
		// ET.Client.Wait_SceneChangeFinish System.Activator.CreateInstance<ET.Client.Wait_SceneChangeFinish>()
		// ET.Client.Wait_CreateMyUnit System.Activator.CreateInstance<ET.Client.Wait_CreateMyUnit>()
		// ET.RobotCase_SecondCaseWait System.Activator.CreateInstance<ET.RobotCase_SecondCaseWait>()
		// ET.WaitType.Wait_Room2C_Start System.Activator.CreateInstance<ET.WaitType.Wait_Room2C_Start>()
		// int System.HashCode.Combine<TrueSync.TSVector2,int>(TrueSync.TSVector2,int)
		// int System.HashCode.Combine<object>(object)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// ET.RpcInfo[] System.Linq.Enumerable.ToArray<ET.RpcInfo>(System.Collections.Generic.IEnumerable<ET.RpcInfo>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,System.Linq.Expressions.ParameterExpression[])
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,ET.Tables.<LoadAsync>d__36>(System.Runtime.CompilerServices.TaskAwaiter&,ET.Tables.<LoadAsync>d__36&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTStartZoneConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTStartZoneConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTStartSceneConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTStartSceneConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTStartProcessConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTStartProcessConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTOneConfig.<LoadAsync>d__3>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTOneConfig.<LoadAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTDemo.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTDemo.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTAIConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTAIConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTStartMachineConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTStartMachineConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.DTUnitConfig.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.DTUnitConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTAIConfig.<LoadAsync>d__4>(ET.DTAIConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTDemo.<LoadAsync>d__4>(ET.DTDemo.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTOneConfig.<LoadAsync>d__3>(ET.DTOneConfig.<LoadAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTStartMachineConfig.<LoadAsync>d__4>(ET.DTStartMachineConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTStartZoneConfig.<LoadAsync>d__4>(ET.DTStartZoneConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTUnitConfig.<LoadAsync>d__4>(ET.DTUnitConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.Tables.<LoadAsync>d__36>(ET.Tables.<LoadAsync>d__36&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTStartSceneConfig.<LoadAsync>d__4>(ET.DTStartSceneConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<ET.DTStartProcessConfig.<LoadAsync>d__4>(ET.DTStartProcessConfig.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d>(ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d>(ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d&)
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.TaskFactory.StartNew<object>(System.Func<object>,System.Threading.CancellationToken)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// Cysharp.Threading.Tasks.UniTask<object> UnityGameFramework.Extension.Awaitable.LoadAssetAsync<object>(UnityGameFramework.Runtime.ResourceComponent,string,int,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// object UnityGameFramework.Runtime.DataNodeComponent.GetData<object>(string)
		// System.Void UnityGameFramework.Runtime.DataNodeComponent.SetData<object>(string,object)
	}
}