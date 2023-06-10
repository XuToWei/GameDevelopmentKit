public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// CommandLine.dll
	// ET.dll
	// Game.dll
	// LubanLib.dll
	// MongoDB.Bson.dll
	// MongoDB.Driver.Core.dll
	// MongoDB.Driver.dll
	// System.Core.dll
	// System.dll
	// UniTask.dll
	// UnityEngine.CoreModule.dll
	// UnityGameFramework.Extension.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<byte>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>
	// Cysharp.Threading.Tasks.UniTask<byte>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTask<long>
	// Cysharp.Threading.Tasks.UniTask<int>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask<uint>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<int>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<byte>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<uint>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<long>
	// ET.AEvent<ET.EventType.ChangeRotation>
	// ET.AEvent<ET.EventType.EntryEvent1>
	// ET.AEvent<ET.EventType.AppStartInitFinish>
	// ET.AEvent<ET.EventType.NumbericChange>
	// ET.AEvent<ET.Server.EventType.UnitEnterSightRange>
	// ET.AEvent<ET.Server.EventType.UnitLeaveSightRange>
	// ET.AEvent<ET.Server.NetInnerComponentOnRead>
	// ET.AEvent<ET.Server.NetServerComponentOnRead>
	// ET.AEvent<ET.EventType.AfterUnitCreate>
	// ET.AEvent<ET.Client.NetClientComponentOnRead>
	// ET.AEvent<ET.EventType.EntryEvent3>
	// ET.AEvent<ET.EventType.ChangePosition>
	// ET.AEvent<ET.EventType.AfterCreateCurrentScene>
	// ET.AEvent<ET.EventType.SceneChangeStart>
	// ET.AEvent<ET.EventType.SceneChangeFinish>
	// ET.AEvent<ET.EventType.LoginFinish>
	// ET.AEvent<ET.EventType.AfterCreateClientScene>
	// ET.AEvent<ET.EventType.EntryEvent2>
	// ET.AInvokeHandler<ET.EventType.OnApplicationPause>
	// ET.AInvokeHandler<ET.EventType.OnApplicationFocus>
	// ET.AInvokeHandler<ET.EventType.OnShutdown>
	// ET.AInvokeHandler<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.LoadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.LoadOne,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.ReloadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.NavmeshComponent.RecastFileLoader,object>
	// ET.ATimer<object>
	// ET.AwakeSystem<object>
	// ET.AwakeSystem<object,object>
	// ET.AwakeSystem<object,long>
	// ET.AwakeSystem<object,ET.Server.MailboxType>
	// ET.AwakeSystem<object,System.Net.Sockets.AddressFamily>
	// ET.AwakeSystem<object,int>
	// ET.AwakeSystem<object,object,object>
	// ET.AwakeSystem<object,long,object>
	// ET.AwakeSystem<object,int,Unity.Mathematics.float3>
	// ET.AwakeSystem<object,object,int>
	// ET.AwakeSystem<object,object,object,int>
	// ET.DestroySystem<object>
	// ET.HashSetComponent<object>
	// ET.IAwake<ET.Server.MailboxType>
	// ET.IAwake<System.Net.Sockets.AddressFamily>
	// ET.IAwake<long>
	// ET.IAwake<object>
	// ET.IAwake<int>
	// ET.IAwake<int,Unity.Mathematics.float3>
	// ET.IAwake<long,object>
	// ET.IAwake<object,object>
	// ET.IAwake<object,int>
	// ET.IAwake<object,object,int>
	// ET.LateUpdateSystem<object>
	// ET.ListComponent<Unity.Mathematics.float3>
	// ET.ListComponent<Cysharp.Threading.Tasks.UniTask>
	// ET.ListComponent<object>
	// ET.ListComponent<long>
	// ET.LoadSystem<object>
	// ET.MultiMap<int,object>
	// ET.Singleton<object>
	// ET.UpdateSystem<object>
	// MongoDB.Driver.IMongoCollection<object>
	// System.Action<object>
	// System.Action<long,object>
	// System.Action<long,int>
	// System.Action<long,long,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.Dictionary<long,long>
	// System.Collections.Generic.Dictionary<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<uint,object>
	// System.Collections.Generic.Dictionary<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<uint,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.HashSet<ushort>
	// System.Collections.Generic.HashSet<long>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet.Enumerator<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<uint,object>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<Unity.Mathematics.float3>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.SortedDictionary<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,ET.Server.ActorMessageSender>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Func<object>
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
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<uint>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<uint>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_UnitStop>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_UnitStop>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>,object>(System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,uint>>&,object&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<object>(object&)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<byte>(Cysharp.Threading.Tasks.UniTask<byte>)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<object>(Cysharp.Threading.Tasks.UniTask<object>)
		// object ET.Entity.AddChild<object,long,object>(long,object,bool)
		// object ET.Entity.AddChild<object>(bool)
		// object ET.Entity.AddChild<object,object,object,int>(object,object,int,bool)
		// object ET.Entity.AddChild<object,object>(object,bool)
		// object ET.Entity.AddChildWithId<object>(long,bool)
		// object ET.Entity.AddChildWithId<object,int>(long,int,bool)
		// object ET.Entity.AddComponent<object,System.Net.Sockets.AddressFamily>(System.Net.Sockets.AddressFamily,bool)
		// object ET.Entity.AddComponent<object,long>(long,bool)
		// object ET.Entity.AddComponent<object,ET.Server.MailboxType>(ET.Server.MailboxType,bool)
		// object ET.Entity.AddComponent<object,object>(object,bool)
		// object ET.Entity.AddComponent<object,object,int>(object,int,bool)
		// object ET.Entity.AddComponent<object,int,Unity.Mathematics.float3>(int,Unity.Mathematics.float3,bool)
		// object ET.Entity.AddComponent<object,int>(int,bool)
		// object ET.Entity.AddComponent<object>(bool)
		// object ET.Entity.AddComponent<object,object,object>(object,object,bool)
		// object ET.Entity.GetChild<object>(long)
		// object ET.Entity.GetComponent<object>()
		// object ET.Entity.GetParent<object>()
		// System.Void ET.Entity.RemoveComponent<object>()
		// ET.SceneType ET.EnumHelper.FromString<ET.SceneType>(string)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.Invoke<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>(int,ET.Server.RobotInvokeArgs)
		// object ET.EventSystem.Invoke<ET.NavmeshComponent.RecastFileLoader,object>(ET.NavmeshComponent.RecastFileLoader)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterUnitCreate>(ET.Scene,ET.EventType.AfterUnitCreate)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterCreateClientScene>(ET.Scene,ET.EventType.AfterCreateClientScene)
		// System.Void ET.EventSystem.Publish<ET.EventType.SceneChangeFinish>(ET.Scene,ET.EventType.SceneChangeFinish)
		// System.Void ET.EventSystem.Publish<ET.EventType.SceneChangeStart>(ET.Scene,ET.EventType.SceneChangeStart)
		// System.Void ET.EventSystem.Publish<ET.Client.NetClientComponentOnRead>(ET.Scene,ET.Client.NetClientComponentOnRead)
		// System.Void ET.EventSystem.Publish<ET.EventType.AfterCreateCurrentScene>(ET.Scene,ET.EventType.AfterCreateCurrentScene)
		// System.Void ET.EventSystem.Publish<ET.EventType.OnApplicationPause>(ET.Scene,ET.EventType.OnApplicationPause)
		// System.Void ET.EventSystem.Publish<ET.EventType.OnApplicationFocus>(ET.Scene,ET.EventType.OnApplicationFocus)
		// System.Void ET.EventSystem.Publish<ET.EventType.OnShutdown>(ET.Scene,ET.EventType.OnShutdown)
		// System.Void ET.EventSystem.Publish<ET.Server.EventType.UnitLeaveSightRange>(ET.Scene,ET.Server.EventType.UnitLeaveSightRange)
		// System.Void ET.EventSystem.Publish<ET.Server.NetServerComponentOnRead>(ET.Scene,ET.Server.NetServerComponentOnRead)
		// System.Void ET.EventSystem.Publish<ET.EventType.MoveStart>(ET.Scene,ET.EventType.MoveStart)
		// System.Void ET.EventSystem.Publish<ET.EventType.MoveStop>(ET.Scene,ET.EventType.MoveStop)
		// System.Void ET.EventSystem.Publish<ET.EventType.EnterMapFinish>(ET.Scene,ET.EventType.EnterMapFinish)
		// System.Void ET.EventSystem.Publish<ET.EventType.NumbericChange>(ET.Scene,ET.EventType.NumbericChange)
		// System.Void ET.EventSystem.Publish<ET.EventType.ChangePosition>(ET.Scene,ET.EventType.ChangePosition)
		// System.Void ET.EventSystem.Publish<ET.EventType.ChangeRotation>(ET.Scene,ET.EventType.ChangeRotation)
		// System.Void ET.EventSystem.Publish<ET.Server.NetInnerComponentOnRead>(ET.Scene,ET.Server.NetInnerComponentOnRead)
		// System.Void ET.EventSystem.Publish<ET.Server.EventType.UnitEnterSightRange>(ET.Scene,ET.Server.EventType.UnitEnterSightRange)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent3>(ET.Scene,ET.EventType.EntryEvent3)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<ET.EventType.LoginFinish>(ET.Scene,ET.EventType.LoginFinish)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent1>(ET.Scene,ET.EventType.EntryEvent1)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<ET.EventType.EntryEvent2>(ET.Scene,ET.EventType.EntryEvent2)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<ET.EventType.AppStartInitFinish>(ET.Scene,ET.EventType.AppStartInitFinish)
		// object ET.Game.AddSingleton<object>()
		// object ET.JsonHelper.FromJson<object>(string)
		// object ET.MongoHelper.Deserialize<object>(byte[])
		// System.Void ET.ObjectHelper.Swap<object>(object&,object&)
		// System.Void ET.RandomGenerator.BreakRank<object>(System.Collections.Generic.List<object>)
		// string ET.StringHelper.ArrayToString<float>(float[])
		// Cysharp.Threading.Tasks.UniTask<UnityGameFramework.Runtime.Entity> Game.EntityExtension.ShowEntityAsync<object>(UnityGameFramework.Runtime.EntityComponent,int,object,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// byte[] MongoDB.Bson.BsonExtensionMethods.ToBson<object>(object,MongoDB.Bson.Serialization.IBsonSerializer<object>,MongoDB.Bson.IO.BsonBinaryWriterSettings,System.Action<MongoDB.Bson.Serialization.BsonSerializationContext.Builder>,MongoDB.Bson.Serialization.BsonSerializationArgs)
		// System.Threading.Tasks.Task<object> MongoDB.Driver.IAsyncCursorExtensions.FirstOrDefaultAsync<object>(MongoDB.Driver.IAsyncCursor<object>,System.Threading.CancellationToken)
		// System.Threading.Tasks.Task<MongoDB.Driver.IAsyncCursor<object>> MongoDB.Driver.IMongoCollectionExtensions.FindAsync<object>(MongoDB.Driver.IMongoCollection<object>,System.Linq.Expressions.Expression<System.Func<object,bool>>,MongoDB.Driver.FindOptions<object,object>,System.Threading.CancellationToken)
		// System.Threading.Tasks.Task<MongoDB.Driver.ReplaceOneResult> MongoDB.Driver.IMongoCollectionExtensions.ReplaceOneAsync<object>(MongoDB.Driver.IMongoCollection<object>,System.Linq.Expressions.Expression<System.Func<object,bool>>,object,MongoDB.Driver.ReplaceOptions,System.Threading.CancellationToken)
		// MongoDB.Driver.IMongoCollection<object> MongoDB.Driver.IMongoDatabase.GetCollection<object>(string,MongoDB.Driver.MongoCollectionSettings)
		// ET.Client.Wait_UnitStop System.Activator.CreateInstance<ET.Client.Wait_UnitStop>()
		// object System.Activator.CreateInstance<object>()
		// ET.Client.Wait_CreateMyUnit System.Activator.CreateInstance<ET.Client.Wait_CreateMyUnit>()
		// ET.Client.Wait_SceneChangeFinish System.Activator.CreateInstance<ET.Client.Wait_SceneChangeFinish>()
		// ET.RpcInfo[] System.Linq.Enumerable.ToArray<ET.RpcInfo>(System.Collections.Generic.IEnumerable<ET.RpcInfo>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Linq.Expressions.Expression<object> System.Linq.Expressions.Expression.Lambda<object>(System.Linq.Expressions.Expression,System.Linq.Expressions.ParameterExpression[])
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,object>(System.Runtime.CompilerServices.TaskAwaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,object>(System.Runtime.CompilerServices.TaskAwaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,object>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<object>(object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,object>(Cysharp.Threading.Tasks.UniTask.Awaiter&,object&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<object>(object&)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.TaskFactory.StartNew<object>(System.Func<object>,System.Threading.CancellationToken)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// Cysharp.Threading.Tasks.UniTask<object> UnityGameFramework.Extension.Awaitable.LoadAssetAsync<object>(UnityGameFramework.Runtime.ResourceComponent,string,int,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
	}
}