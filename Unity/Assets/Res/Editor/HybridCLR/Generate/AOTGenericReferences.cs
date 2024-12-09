using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"CommandLine.dll",
		"ET.Core.dll",
		"Game.ET.Loader.dll",
		"Game.dll",
		"LubanLib.dll",
		"MemoryPack.dll",
		"MongoDB.Bson.dll",
		"System.Core.dll",
		"System.Runtime.CompilerServices.Unsafe.dll",
		"System.dll",
		"UniTask.Extension.dll",
		"UniTask.dll",
		"UnityEngine.CoreModule.dll",
		"UnityGameFramework.Extension.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// <>f__AnonymousType16<int,object>
	// <>f__AnonymousType17<int,object>
	// <>f__AnonymousType1<object,object>
	// CSharpx.Just<object>
	// CSharpx.Maybe<object>
	// CSharpx.Nothing<object>
	// CommandLine.Core.InstanceBuilder.<>c__0<object>
	// CommandLine.Core.InstanceBuilder.<>c__DisplayClass0_0<object>
	// CommandLine.Core.InstanceBuilder.<>c__DisplayClass0_1<object>
	// CommandLine.Core.ReflectionExtensions.<>c__0<object>
	// CommandLine.Core.ReflectionExtensions.<>c__DisplayClass0_0<object>
	// CommandLine.NotParsed<object>
	// CommandLine.Parsed<object>
	// CommandLine.Parser.<>c__DisplayClass17_0<object>
	// CommandLine.ParserResult<object>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource.<>c<byte>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource.<>c<object>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<byte>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSource<object>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus.<>c<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus.<>c<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus.<>c<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus.<>c<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus.<>c<object>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.ADynamicEvent.<Handle>d__5<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.A2NetClient_MessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.A2NetClient_RequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AI_Attack.<Execute>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AI_XunLuo.<Execute>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ClientSenderComponentSystem.<Call>d__6,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4,long>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.EnterMapHelper.<EnterMapAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.EnterMapHelper.<Match>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.EntryEvent3_InitClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.FiberInit_NetClient.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.FiberInit_Robot.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.G2C_ReconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.HttpClientHelper.<Get>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSSceneInitFinish_Finish.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.LoginHelper.<Login>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_CreateUnitsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_PathfindingResultHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.M2C_StopHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.Main2NetClient_LoginHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.MoveHelper.<MoveToAsync>d__0,int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.MoveHelper.<MoveToAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.OneFrameInputsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.PingComponentSystem.<PingAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.Room2C_EnterMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterAddressComponentSystem.<Init>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterHelper.<Connect>d__2,uint>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterHelper.<CreateRouterSession>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.RouterHelper.<GetRouterAddress>d__1,System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.SceneChangeStart_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.ConsoleComponentSystem.<Start>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTAIConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTDemo.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTOneConfig.<LoadAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTStartMachineConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTStartProcessConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTStartSceneConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTStartZoneConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DTUnitConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.DynamicEventSystem.<PublishAsync>d__13<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.EntryEvent1_InitDynamicEvent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.EntryEvent1_InitShare.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.FiberInit_Main.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.LubanLoadAllAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.LubanLoadOneAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.LubanReloadAllAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MessageHandler.<Handle>d__1<object,object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MessageHandler.<Handle>d__1<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MessageSessionHandler.<HandleAsync>d__2<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MessageSessionHandler.<HandleAsync>d__2<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.MoveComponentSystem.<MoveToAsync>d__5,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.ReloadConfigConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.ReloadDllConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.A2NetInner_MessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.A2NetInner_RequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ARobotCase.<Handle>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.BenchmarkClientComponentSystem.<Start>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_BenchmarkHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_EnterMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_LoginGateHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_MatchHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2G_PingHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2M_PathfindingResultHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2M_StopHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2M_TransferMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2R_LoginHandler.<CloseSession>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2R_LoginHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.C2Room_CheckHashHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ChangePosition_NotifyAOI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.CreateRobotConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.EntryEvent2_InitServer.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_BenchmarkClient.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_BenchmarkServer.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Gate.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Location.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Map.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Match.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_NetInner.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Realm.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_RoomRoot.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_Router.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FiberInit_RouterManager.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.FrameMessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.G2Match_MatchHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.G2Room_ReconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.GateMapFactory.<Create>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.HttpComponentSystem.<Accept>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.HttpComponentSystem.<Handle>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.HttpGetRouterHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationOneTypeSystem.<Add>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationOneTypeSystem.<Get>d__5,ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationOneTypeSystem.<Lock>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationOneTypeSystem.<Remove>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<Add>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<AddLocation>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<Get>d__5,ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<Lock>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<Remove>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.LocationProxyComponentSystem.<UnLock>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.Match2Map_GetRoomHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MatchComponentSystem.<Match>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationHandler.<Handle>d__1<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationSenderComponentSystem.<Call>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationSenderComponentSystem.<Call>d__8,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MessageSenderSystem.<Call>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ObjectAddRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ObjectGetRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ObjectLockRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ObjectRemoveRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ObjectUnLockRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.ProcessOuterSenderSystem.<Call>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.RobotCaseComponentSystem.<New>d__1,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.RobotConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.RobotManagerComponentSystem.<NewRobot>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.RoomManager2Room_InitHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.TransferHelper.<Transfer>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.SessionSystem.<Call>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask.<>c<ET.Tables.<LoadAsync>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.ADynamicEvent.<Handle>d__5<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.A2NetClient_MessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.A2NetClient_RequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AI_Attack.<Execute>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AI_XunLuo.<Execute>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ClientSenderComponentSystem.<Call>d__6,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4,long>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.EnterMapHelper.<EnterMapAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.EnterMapHelper.<Match>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.EntryEvent3_InitClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.FiberInit_NetClient.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.FiberInit_Robot.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.G2C_ReconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.HttpClientHelper.<Get>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSSceneInitFinish_Finish.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.LoginHelper.<Login>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_CreateUnitsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_PathfindingResultHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.M2C_StopHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.Main2NetClient_LoginHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.MoveHelper.<MoveToAsync>d__0,int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.MoveHelper.<MoveToAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.OneFrameInputsHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.PingComponentSystem.<PingAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.Room2C_EnterMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterAddressComponentSystem.<Init>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterHelper.<Connect>d__2,uint>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterHelper.<CreateRouterSession>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.RouterHelper.<GetRouterAddress>d__1,System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.SceneChangeStart_AddComponent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.ConsoleComponentSystem.<Start>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTAIConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTDemo.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTOneConfig.<LoadAsync>d__5>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTStartMachineConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTStartProcessConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTStartSceneConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTStartZoneConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DTUnitConfig.<LoadAsync>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.DynamicEventSystem.<PublishAsync>d__13<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.EntryEvent1_InitDynamicEvent.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.EntryEvent1_InitShare.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.FiberInit_Main.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.LubanLoadAllAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.LubanLoadOneAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.LubanReloadAllAsyncHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MessageHandler.<Handle>d__1<object,object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MessageHandler.<Handle>d__1<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MessageSessionHandler.<HandleAsync>d__2<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MessageSessionHandler.<HandleAsync>d__2<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.MoveComponentSystem.<MoveToAsync>d__5,byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.ReloadConfigConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.ReloadDllConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.A2NetInner_MessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.A2NetInner_RequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ARobotCase.<Handle>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.BenchmarkClientComponentSystem.<Start>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_BenchmarkHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_EnterMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_LoginGateHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_MatchHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2G_PingHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2M_PathfindingResultHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2M_StopHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2M_TransferMapHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2R_LoginHandler.<CloseSession>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2R_LoginHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.C2Room_CheckHashHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ChangePosition_NotifyAOI.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.CreateRobotConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.EntryEvent2_InitServer.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_BenchmarkClient.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_BenchmarkServer.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Gate.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Location.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Map.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Match.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_NetInner.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Realm.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_RoomRoot.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_Router.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FiberInit_RouterManager.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.FrameMessageHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.G2Match_MatchHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.G2Room_ReconnectHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.GateMapFactory.<Create>d__0,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.HttpComponentSystem.<Accept>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.HttpComponentSystem.<Handle>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.HttpGetRouterHandler.<Handle>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationOneTypeSystem.<Add>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationOneTypeSystem.<Get>d__5,ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationOneTypeSystem.<Lock>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationOneTypeSystem.<Remove>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<Add>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<AddLocation>d__6>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<Get>d__5,ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<Lock>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<Remove>d__4>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.LocationProxyComponentSystem.<UnLock>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.Match2Map_GetRoomHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MatchComponentSystem.<Match>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationHandler.<Handle>d__1<object,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationSenderComponentSystem.<Call>d__10,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationSenderComponentSystem.<Call>d__8,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MessageSenderSystem.<Call>d__2,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ObjectAddRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ObjectGetRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ObjectLockRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ObjectRemoveRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ObjectUnLockRequestHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.ProcessOuterSenderSystem.<Call>d__13,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.RobotCaseComponentSystem.<New>d__1,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.RobotConsoleHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.RobotManagerComponentSystem.<NewRobot>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.RoomManager2Room_InitHandler.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.TransferHelper.<Transfer>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.SessionSystem.<Call>d__4,object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask<ET.Tables.<LoadAsync>d__36>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Client.OperaComponentSystem.<Test1>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Client.OperaComponentSystem.<Test2>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Client.TestComponentSystem.<RunTestCancel>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Entry.<StartAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid.<>c<ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Client.OperaComponentSystem.<Test1>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Client.OperaComponentSystem.<Test2>d__3>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Client.TestComponentSystem.<RunTestCancel>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Entry.<StartAsync>d__2>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1>
	// Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoid<ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<ET.ActorId>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<byte>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<int>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<long>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<object>
	// Cysharp.Threading.Tasks.CompilerServices.IStateMachineRunnerPromise<uint>
	// Cysharp.Threading.Tasks.ITaskPoolNode<object>
	// Cysharp.Threading.Tasks.IUniTaskSource<ET.ActorId>
	// Cysharp.Threading.Tasks.IUniTaskSource<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.IUniTaskSource<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.IUniTaskSource<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.IUniTaskSource<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,ET.ActorId>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,long>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<byte,uint>>
	// Cysharp.Threading.Tasks.IUniTaskSource<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.IUniTaskSource<byte>
	// Cysharp.Threading.Tasks.IUniTaskSource<int>
	// Cysharp.Threading.Tasks.IUniTaskSource<long>
	// Cysharp.Threading.Tasks.IUniTaskSource<object>
	// Cysharp.Threading.Tasks.IUniTaskSource<uint>
	// Cysharp.Threading.Tasks.Internal.StatePool<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>>
	// Cysharp.Threading.Tasks.Internal.StatePool<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>
	// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>>
	// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.ActorId>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,long>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,uint>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<byte>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<int>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<long>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<uint>
	// Cysharp.Threading.Tasks.UniTask.CanceledResultSource<object>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<ET.ActorId>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,ET.ActorId>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,long>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<byte,uint>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<byte>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<int>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<long>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<object>
	// Cysharp.Threading.Tasks.UniTask.IsCanceledSource<uint>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<ET.ActorId>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,ET.ActorId>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,long>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<byte,uint>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<byte>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<int>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<long>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<object>
	// Cysharp.Threading.Tasks.UniTask.MemoizeSource<uint>
	// Cysharp.Threading.Tasks.UniTask<ET.ActorId>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTask<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,ET.ActorId>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,byte>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,int>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,long>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,object>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<byte,uint>>
	// Cysharp.Threading.Tasks.UniTask<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTask<byte>
	// Cysharp.Threading.Tasks.UniTask<int>
	// Cysharp.Threading.Tasks.UniTask<long>
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTask<uint>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<Cysharp.Threading.Tasks.AsyncUnit>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<ET.ActorId>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<System.ValueTuple<uint,object>>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<byte>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<int>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<long>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<object>
	// Cysharp.Threading.Tasks.UniTaskCompletionSourceCore<uint>
	// Cysharp.Threading.Tasks.UniTaskExtension.<>c__DisplayClass1_0<ET.Client.WaitType.Wait_Room2C_Start>
	// Cysharp.Threading.Tasks.UniTaskExtension.<>c__DisplayClass1_0<ET.Client.Wait_CreateMyUnit>
	// Cysharp.Threading.Tasks.UniTaskExtension.<>c__DisplayClass1_0<ET.Client.Wait_SceneChangeFinish>
	// Cysharp.Threading.Tasks.UniTaskExtension.<>c__DisplayClass1_0<ET.Client.Wait_UnitStop>
	// Cysharp.Threading.Tasks.UniTaskExtension.<>c__DisplayClass1_0<object>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__19<byte>
	// Cysharp.Threading.Tasks.UniTaskExtensions.<>c__19<object>
	// ET.AEvent<object,ET.ChangePosition>
	// ET.AEvent<object,ET.ChangeRotation>
	// ET.AEvent<object,ET.Client.AfterCreateClientScene>
	// ET.AEvent<object,ET.Client.AfterCreateCurrentScene>
	// ET.AEvent<object,ET.Client.AfterUnitCreate>
	// ET.AEvent<object,ET.Client.AppStartInitFinish>
	// ET.AEvent<object,ET.Client.EnterMapFinish>
	// ET.AEvent<object,ET.Client.LSSceneChangeStart>
	// ET.AEvent<object,ET.Client.LSSceneInitFinish>
	// ET.AEvent<object,ET.Client.LoginFinish>
	// ET.AEvent<object,ET.Client.SceneChangeFinish>
	// ET.AEvent<object,ET.Client.SceneChangeStart>
	// ET.AEvent<object,ET.EntryEvent1>
	// ET.AEvent<object,ET.EntryEvent2>
	// ET.AEvent<object,ET.EntryEvent3>
	// ET.AEvent<object,ET.MoveStart>
	// ET.AEvent<object,ET.MoveStop>
	// ET.AEvent<object,ET.NumbericChange>
	// ET.AEvent<object,ET.Server.UnitEnterSightRange>
	// ET.AEvent<object,ET.Server.UnitLeaveSightRange>
	// ET.AInvokeHandler<ET.ConfigComponent.LoadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.ReloadAll,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.ConfigComponent.ReloadOne,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.FiberInit,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.MailBoxInvoker>
	// ET.AInvokeHandler<ET.NetComponentOnRead>
	// ET.AInvokeHandler<ET.OnApplicationFocus>
	// ET.AInvokeHandler<ET.OnApplicationPause>
	// ET.AInvokeHandler<ET.OnCodeReload>
	// ET.AInvokeHandler<ET.OnShutdown>
	// ET.AInvokeHandler<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>
	// ET.AInvokeHandler<ET.TimerCallback>
	// ET.AwakeSystem<object,ET.ActorId,object>
	// ET.AwakeSystem<object,int,Unity.Mathematics.float3>
	// ET.AwakeSystem<object,int,int>
	// ET.AwakeSystem<object,int>
	// ET.AwakeSystem<object,object,int>
	// ET.AwakeSystem<object,object,object>
	// ET.AwakeSystem<object,object>
	// ET.AwakeSystem<object>
	// ET.DestroySystem<object>
	// ET.DoubleMap<object,long>
	// ET.EntityRef<object>
	// ET.HashSetComponent<object>
	// ET.IAwake<ET.ActorId,object>
	// ET.IAwake<int,Unity.Mathematics.float3>
	// ET.IAwake<int,int>
	// ET.IAwake<int>
	// ET.IAwake<object,int>
	// ET.IAwake<object,object,object>
	// ET.IAwake<object,object>
	// ET.IAwake<object>
	// ET.IAwakeSystem<ET.ActorId,object>
	// ET.IAwakeSystem<int,Unity.Mathematics.float3>
	// ET.IAwakeSystem<int,int>
	// ET.IAwakeSystem<int>
	// ET.IAwakeSystem<object,int>
	// ET.IAwakeSystem<object,long>
	// ET.IAwakeSystem<object,object,object>
	// ET.IAwakeSystem<object,object>
	// ET.IAwakeSystem<object>
	// ET.LateUpdateSystem<object>
	// ET.ListComponent<Cysharp.Threading.Tasks.UniTask>
	// ET.ListComponent<ET.EntityRef<object>>
	// ET.ListComponent<Unity.Mathematics.float3>
	// ET.ListComponent<int>
	// ET.ListComponent<long>
	// ET.MultiMap<int,object>
	// ET.Singleton<object>
	// ET.StructBsonSerialize<ET.LSInput>
	// ET.StructBsonSerialize<TrueSync.FP>
	// ET.StructBsonSerialize<TrueSync.TSQuaternion>
	// ET.StructBsonSerialize<TrueSync.TSVector2>
	// ET.StructBsonSerialize<TrueSync.TSVector4>
	// ET.StructBsonSerialize<TrueSync.TSVector>
	// ET.StructBsonSerialize<Unity.Mathematics.float2>
	// ET.StructBsonSerialize<Unity.Mathematics.float3>
	// ET.StructBsonSerialize<Unity.Mathematics.float4>
	// ET.StructBsonSerialize<Unity.Mathematics.quaternion>
	// ET.StructBsonSerialize<object>
	// ET.UnOrderMultiMap<object,object>
	// ET.UpdateSystem<object>
	// MemoryPack.Formatters.ArrayFormatter<ET.LSInput>
	// MemoryPack.Formatters.ArrayFormatter<byte>
	// MemoryPack.Formatters.ArrayFormatter<object>
	// MemoryPack.Formatters.DictionaryFormatter<int,long>
	// MemoryPack.Formatters.DictionaryFormatter<long,ET.LSInput>
	// MemoryPack.Formatters.ListFormatter<Unity.Mathematics.float3>
	// MemoryPack.Formatters.ListFormatter<long>
	// MemoryPack.Formatters.ListFormatter<object>
	// MemoryPack.IMemoryPackFormatter<Unity.Mathematics.float3>
	// MemoryPack.IMemoryPackFormatter<byte>
	// MemoryPack.IMemoryPackFormatter<long>
	// MemoryPack.IMemoryPackFormatter<object>
	// MemoryPack.IMemoryPackable<ET.LSInput>
	// MemoryPack.IMemoryPackable<object>
	// MemoryPack.MemoryPackFormatter<ET.LSInput>
	// MemoryPack.MemoryPackFormatter<System.UIntPtr>
	// MemoryPack.MemoryPackFormatter<object>
	// MongoDB.Bson.Serialization.IBsonSerializer<object>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<ET.LSInput>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<TrueSync.FP>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<TrueSync.TSQuaternion>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<TrueSync.TSVector2>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<TrueSync.TSVector4>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<TrueSync.TSVector>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<Unity.Mathematics.float2>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<Unity.Mathematics.float3>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<Unity.Mathematics.float4>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<Unity.Mathematics.quaternion>
	// MongoDB.Bson.Serialization.Serializers.SerializerBase<object>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<ET.LSInput>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<TrueSync.FP>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<TrueSync.TSQuaternion>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<TrueSync.TSVector2>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<TrueSync.TSVector4>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<TrueSync.TSVector>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<Unity.Mathematics.float2>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<Unity.Mathematics.float3>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<Unity.Mathematics.float4>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<Unity.Mathematics.quaternion>
	// MongoDB.Bson.Serialization.Serializers.StructSerializerBase<object>
	// System.Action<Cysharp.Threading.Tasks.UniTask>
	// System.Action<DotRecast.Detour.StraightPathItem>
	// System.Action<ET.EntityRef<object>>
	// System.Action<ET.MessageSessionDispatcherInfo>
	// System.Action<ET.NumericWatcherInfo>
	// System.Action<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Action<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Action<Unity.Mathematics.float3>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int>
	// System.Action<long,int>
	// System.Action<long,object>
	// System.Action<long>
	// System.Action<object,long>
	// System.Action<object,object>
	// System.Action<object>
	// System.ArraySegment.Enumerator<byte>
	// System.ArraySegment<byte>
	// System.ByReference<byte>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ArraySortHelper<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ArraySortHelper<ET.EntityRef<object>>
	// System.Collections.Generic.ArraySortHelper<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.ArraySortHelper<ET.NumericWatcherInfo>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ArraySortHelper<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ArraySortHelper<Unity.Mathematics.float3>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<long>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.Comparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.Comparer<ET.ActorId>
	// System.Collections.Generic.Comparer<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Collections.Generic.Comparer<ET.Client.Wait_CreateMyUnit>
	// System.Collections.Generic.Comparer<ET.Client.Wait_SceneChangeFinish>
	// System.Collections.Generic.Comparer<ET.Client.Wait_UnitStop>
	// System.Collections.Generic.Comparer<ET.EntityRef<object>>
	// System.Collections.Generic.Comparer<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.Comparer<ET.NumericWatcherInfo>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.Comparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,ET.ActorId>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,long>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.Comparer<System.ValueTuple<byte,uint>>
	// System.Collections.Generic.Comparer<System.ValueTuple<uint,object>>
	// System.Collections.Generic.Comparer<Unity.Mathematics.float3>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<long>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<uint>
	// System.Collections.Generic.Comparer<ushort>
	// System.Collections.Generic.ComparisonComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ComparisonComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ComparisonComparer<ET.ActorId>
	// System.Collections.Generic.ComparisonComparer<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Collections.Generic.ComparisonComparer<ET.Client.Wait_CreateMyUnit>
	// System.Collections.Generic.ComparisonComparer<ET.Client.Wait_SceneChangeFinish>
	// System.Collections.Generic.ComparisonComparer<ET.Client.Wait_UnitStop>
	// System.Collections.Generic.ComparisonComparer<ET.EntityRef<object>>
	// System.Collections.Generic.ComparisonComparer<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.ComparisonComparer<ET.NumericWatcherInfo>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ComparisonComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,ET.ActorId>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,long>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<byte,uint>>
	// System.Collections.Generic.ComparisonComparer<System.ValueTuple<uint,object>>
	// System.Collections.Generic.ComparisonComparer<Unity.Mathematics.float3>
	// System.Collections.Generic.ComparisonComparer<byte>
	// System.Collections.Generic.ComparisonComparer<int>
	// System.Collections.Generic.ComparisonComparer<long>
	// System.Collections.Generic.ComparisonComparer<object>
	// System.Collections.Generic.ComparisonComparer<uint>
	// System.Collections.Generic.ComparisonComparer<ushort>
	// System.Collections.Generic.Dictionary.Enumerator<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary.Enumerator<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.Enumerator<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary.Enumerator<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<long,ET.ActorId>
	// System.Collections.Generic.Dictionary.Enumerator<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.Enumerator<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,ET.ActorId>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.KeyCollection<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary.KeyCollection<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary.KeyCollection<int,long>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<long,ET.ActorId>
	// System.Collections.Generic.Dictionary.KeyCollection<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.KeyCollection<long,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.KeyCollection<object,long>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,ET.ActorId>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,long>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<ushort,object>
	// System.Collections.Generic.Dictionary.ValueCollection<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary.ValueCollection<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary.ValueCollection<int,long>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<long,ET.ActorId>
	// System.Collections.Generic.Dictionary.ValueCollection<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<long,ET.LSInput>
	// System.Collections.Generic.Dictionary.ValueCollection<long,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary.ValueCollection<object,long>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<ushort,object>
	// System.Collections.Generic.Dictionary<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.Dictionary<int,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary<int,ET.MessageSenderStruct>
	// System.Collections.Generic.Dictionary<int,ET.RpcInfo>
	// System.Collections.Generic.Dictionary<int,long>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<long,ET.ActorId>
	// System.Collections.Generic.Dictionary<long,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary<long,ET.LSInput>
	// System.Collections.Generic.Dictionary<long,object>
	// System.Collections.Generic.Dictionary<object,ET.EntityRef<object>>
	// System.Collections.Generic.Dictionary<object,long>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<ushort,object>
	// System.Collections.Generic.EqualityComparer<ET.ActorId>
	// System.Collections.Generic.EqualityComparer<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Collections.Generic.EqualityComparer<ET.Client.Wait_CreateMyUnit>
	// System.Collections.Generic.EqualityComparer<ET.Client.Wait_SceneChangeFinish>
	// System.Collections.Generic.EqualityComparer<ET.Client.Wait_UnitStop>
	// System.Collections.Generic.EqualityComparer<ET.EntityRef<object>>
	// System.Collections.Generic.EqualityComparer<ET.LSInput>
	// System.Collections.Generic.EqualityComparer<ET.MessageSenderStruct>
	// System.Collections.Generic.EqualityComparer<ET.RpcInfo>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.EqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,ET.ActorId>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,long>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<byte,uint>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.EqualityComparer<System.ValueTuple<uint,object>>
	// System.Collections.Generic.EqualityComparer<byte>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<long>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.EqualityComparer<uint>
	// System.Collections.Generic.EqualityComparer<ushort>
	// System.Collections.Generic.HashSet.Enumerator<ET.EntityRef<object>>
	// System.Collections.Generic.HashSet.Enumerator<int>
	// System.Collections.Generic.HashSet.Enumerator<long>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet.Enumerator<ushort>
	// System.Collections.Generic.HashSet<ET.EntityRef<object>>
	// System.Collections.Generic.HashSet<int>
	// System.Collections.Generic.HashSet<long>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSet<ushort>
	// System.Collections.Generic.HashSetEqualityComparer<ET.EntityRef<object>>
	// System.Collections.Generic.HashSetEqualityComparer<int>
	// System.Collections.Generic.HashSetEqualityComparer<long>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.HashSetEqualityComparer<ushort>
	// System.Collections.Generic.ICollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ICollection<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ICollection<ET.EntityRef<object>>
	// System.Collections.Generic.ICollection<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.ICollection<ET.NumericWatcherInfo>
	// System.Collections.Generic.ICollection<ET.RpcInfo>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,ET.EntityRef<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,ET.MessageSenderStruct>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,ET.RpcInfo>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,ET.ActorId>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,ET.EntityRef<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,ET.LSInput>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,ET.EntityRef<object>>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.ICollection<Unity.Mathematics.float3>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<long>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<ushort>
	// System.Collections.Generic.IComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IComparer<ET.EntityRef<object>>
	// System.Collections.Generic.IComparer<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.IComparer<ET.NumericWatcherInfo>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IComparer<Unity.Mathematics.float3>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<long>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerable<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IEnumerable<ET.EntityRef<object>>
	// System.Collections.Generic.IEnumerable<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.IEnumerable<ET.NumericWatcherInfo>
	// System.Collections.Generic.IEnumerable<ET.RpcInfo>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,ET.MessageSenderStruct>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,ET.RpcInfo>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,ET.ActorId>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,ET.LSInput>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerable<Unity.Mathematics.float3>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<long>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<ushort>
	// System.Collections.Generic.IEnumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IEnumerator<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IEnumerator<ET.EntityRef<object>>
	// System.Collections.Generic.IEnumerator<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.IEnumerator<ET.NumericWatcherInfo>
	// System.Collections.Generic.IEnumerator<ET.RpcInfo>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,ET.MessageSenderStruct>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,ET.RpcInfo>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,ET.ActorId>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,ET.LSInput>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,ET.EntityRef<object>>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,long>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<ushort,object>>
	// System.Collections.Generic.IEnumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<long>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<ushort>
	// System.Collections.Generic.IEqualityComparer<ET.EntityRef<object>>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IEqualityComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<long>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IEqualityComparer<ushort>
	// System.Collections.Generic.IList<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.IList<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.IList<ET.EntityRef<object>>
	// System.Collections.Generic.IList<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.IList<ET.NumericWatcherInfo>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.IList<Unity.Mathematics.float3>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<long>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<System.ValueTuple<object,int>,object>
	// System.Collections.Generic.KeyValuePair<int,ET.EntityRef<object>>
	// System.Collections.Generic.KeyValuePair<int,ET.MessageSenderStruct>
	// System.Collections.Generic.KeyValuePair<int,ET.RpcInfo>
	// System.Collections.Generic.KeyValuePair<int,long>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<long,ET.ActorId>
	// System.Collections.Generic.KeyValuePair<long,ET.EntityRef<object>>
	// System.Collections.Generic.KeyValuePair<long,ET.LSInput>
	// System.Collections.Generic.KeyValuePair<long,object>
	// System.Collections.Generic.KeyValuePair<object,ET.EntityRef<object>>
	// System.Collections.Generic.KeyValuePair<object,long>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.KeyValuePair<ushort,object>
	// System.Collections.Generic.List.Enumerator<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List.Enumerator<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.List.Enumerator<ET.EntityRef<object>>
	// System.Collections.Generic.List.Enumerator<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.List.Enumerator<ET.NumericWatcherInfo>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List.Enumerator<Unity.Mathematics.float3>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<long>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.List<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.List<ET.EntityRef<object>>
	// System.Collections.Generic.List<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.List<ET.NumericWatcherInfo>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.List<Unity.Mathematics.float3>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<long>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.Generic.ObjectComparer<DotRecast.Detour.StraightPathItem>
	// System.Collections.Generic.ObjectComparer<ET.ActorId>
	// System.Collections.Generic.ObjectComparer<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Collections.Generic.ObjectComparer<ET.Client.Wait_CreateMyUnit>
	// System.Collections.Generic.ObjectComparer<ET.Client.Wait_SceneChangeFinish>
	// System.Collections.Generic.ObjectComparer<ET.Client.Wait_UnitStop>
	// System.Collections.Generic.ObjectComparer<ET.EntityRef<object>>
	// System.Collections.Generic.ObjectComparer<ET.MessageSessionDispatcherInfo>
	// System.Collections.Generic.ObjectComparer<ET.NumericWatcherInfo>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,ET.ActorId>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,long>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<byte,uint>>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<uint,object>>
	// System.Collections.Generic.ObjectComparer<Unity.Mathematics.float3>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<long>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<uint>
	// System.Collections.Generic.ObjectComparer<ushort>
	// System.Collections.Generic.ObjectEqualityComparer<ET.ActorId>
	// System.Collections.Generic.ObjectEqualityComparer<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Collections.Generic.ObjectEqualityComparer<ET.Client.Wait_CreateMyUnit>
	// System.Collections.Generic.ObjectEqualityComparer<ET.Client.Wait_SceneChangeFinish>
	// System.Collections.Generic.ObjectEqualityComparer<ET.Client.Wait_UnitStop>
	// System.Collections.Generic.ObjectEqualityComparer<ET.EntityRef<object>>
	// System.Collections.Generic.ObjectEqualityComparer<ET.LSInput>
	// System.Collections.Generic.ObjectEqualityComparer<ET.MessageSenderStruct>
	// System.Collections.Generic.ObjectEqualityComparer<ET.RpcInfo>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,ET.ActorId>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,byte>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,long>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,object>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<byte,uint>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.ObjectEqualityComparer<System.ValueTuple<uint,object>>
	// System.Collections.Generic.ObjectEqualityComparer<byte>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<long>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<uint>
	// System.Collections.Generic.ObjectEqualityComparer<ushort>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue.Enumerator<uint>
	// System.Collections.Generic.Queue<object>
	// System.Collections.Generic.Queue<uint>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_0<long,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<int,object>
	// System.Collections.Generic.SortedDictionary.<>c__DisplayClass34_1<long,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<int,object>
	// System.Collections.Generic.SortedDictionary.KeyCollection<long,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<int,object>
	// System.Collections.Generic.SortedDictionary.KeyValuePairComparer<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass5_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.<>c__DisplayClass6_0<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection.Enumerator<long,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<int,object>
	// System.Collections.Generic.SortedDictionary.ValueCollection<long,object>
	// System.Collections.Generic.SortedDictionary<int,object>
	// System.Collections.Generic.SortedDictionary<long,object>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass52_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass53_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<>c__DisplayClass85_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.<Reverse>d__94<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Enumerator<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.Node<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet.<>c__DisplayClass9_0<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet.TreeSubSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.SortedSetEqualityComparer<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.Stack.Enumerator<object>
	// System.Collections.Generic.Stack<object>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeSet<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.TreeWalkPredicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Cysharp.Threading.Tasks.UniTask>
	// System.Collections.ObjectModel.ReadOnlyCollection<DotRecast.Detour.StraightPathItem>
	// System.Collections.ObjectModel.ReadOnlyCollection<ET.EntityRef<object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<ET.MessageSessionDispatcherInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<ET.NumericWatcherInfo>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Collections.ObjectModel.ReadOnlyCollection<Unity.Mathematics.float3>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<long>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<Cysharp.Threading.Tasks.UniTask>
	// System.Comparison<DotRecast.Detour.StraightPathItem>
	// System.Comparison<ET.ActorId>
	// System.Comparison<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Comparison<ET.Client.Wait_CreateMyUnit>
	// System.Comparison<ET.Client.Wait_SceneChangeFinish>
	// System.Comparison<ET.Client.Wait_UnitStop>
	// System.Comparison<ET.EntityRef<object>>
	// System.Comparison<ET.MessageSessionDispatcherInfo>
	// System.Comparison<ET.NumericWatcherInfo>
	// System.Comparison<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Comparison<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Comparison<System.ValueTuple<byte,ET.ActorId>>
	// System.Comparison<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Comparison<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Comparison<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Comparison<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Comparison<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Comparison<System.ValueTuple<byte,byte>>
	// System.Comparison<System.ValueTuple<byte,int>>
	// System.Comparison<System.ValueTuple<byte,long>>
	// System.Comparison<System.ValueTuple<byte,object>>
	// System.Comparison<System.ValueTuple<byte,uint>>
	// System.Comparison<System.ValueTuple<uint,object>>
	// System.Comparison<Unity.Mathematics.float3>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<long>
	// System.Comparison<object>
	// System.Comparison<uint>
	// System.Comparison<ushort>
	// System.Func<Cysharp.Threading.Tasks.UniTask<object>>
	// System.Func<Cysharp.Threading.Tasks.UniTask>
	// System.Func<ET.ActorId>
	// System.Func<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Func<ET.Client.Wait_CreateMyUnit>
	// System.Func<ET.Client.Wait_SceneChangeFinish>
	// System.Func<ET.Client.Wait_UnitStop>
	// System.Func<System.ValueTuple<byte,ET.ActorId>>
	// System.Func<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Func<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Func<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Func<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Func<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Func<System.ValueTuple<byte,byte>>
	// System.Func<System.ValueTuple<byte,int>>
	// System.Func<System.ValueTuple<byte,long>>
	// System.Func<System.ValueTuple<byte,object>>
	// System.Func<System.ValueTuple<byte,uint>>
	// System.Func<System.ValueTuple<uint,object>>
	// System.Func<byte>
	// System.Func<int>
	// System.Func<long>
	// System.Func<object,Cysharp.Threading.Tasks.UniTask<object>>
	// System.Func<object,ET.ActorId>
	// System.Func<object,ET.Client.WaitType.Wait_Room2C_Start>
	// System.Func<object,ET.Client.Wait_CreateMyUnit>
	// System.Func<object,ET.Client.Wait_SceneChangeFinish>
	// System.Func<object,ET.Client.Wait_UnitStop>
	// System.Func<object,System.ValueTuple<byte,ET.ActorId>>
	// System.Func<object,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Func<object,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Func<object,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Func<object,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Func<object,System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Func<object,System.ValueTuple<byte,byte>>
	// System.Func<object,System.ValueTuple<byte,int>>
	// System.Func<object,System.ValueTuple<byte,long>>
	// System.Func<object,System.ValueTuple<byte,object>>
	// System.Func<object,System.ValueTuple<byte,uint>>
	// System.Func<object,System.ValueTuple<uint,object>>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,long>
	// System.Func<object,object,ET.ActorId>
	// System.Func<object,object,ET.Client.WaitType.Wait_Room2C_Start>
	// System.Func<object,object,ET.Client.Wait_CreateMyUnit>
	// System.Func<object,object,ET.Client.Wait_SceneChangeFinish>
	// System.Func<object,object,ET.Client.Wait_UnitStop>
	// System.Func<object,object,System.ValueTuple<byte,ET.ActorId>>
	// System.Func<object,object,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Func<object,object,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Func<object,object,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Func<object,object,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Func<object,object,System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Func<object,object,System.ValueTuple<byte,byte>>
	// System.Func<object,object,System.ValueTuple<byte,int>>
	// System.Func<object,object,System.ValueTuple<byte,long>>
	// System.Func<object,object,System.ValueTuple<byte,object>>
	// System.Func<object,object,System.ValueTuple<byte,uint>>
	// System.Func<object,object,System.ValueTuple<uint,object>>
	// System.Func<object,object,byte,object>
	// System.Func<object,object,byte>
	// System.Func<object,object,int>
	// System.Func<object,object,long>
	// System.Func<object,object,object>
	// System.Func<object,object,uint>
	// System.Func<object,object>
	// System.Func<object,uint>
	// System.Func<object>
	// System.Func<uint>
	// System.Linq.Buffer<ET.EntityRef<object>>
	// System.Linq.Buffer<ET.RpcInfo>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<OfTypeIterator>d__97<object>
	// System.Linq.Enumerable.<SelectManyIterator>d__17<object,object>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Linq.GroupedEnumerable<object,object,object>
	// System.Linq.Lookup.<GetEnumerator>d__12<object,object>
	// System.Linq.Lookup.Grouping.<GetEnumerator>d__7<object,object>
	// System.Linq.Lookup.Grouping<object,object>
	// System.Linq.Lookup<object,object>
	// System.Predicate<Cysharp.Threading.Tasks.UniTask>
	// System.Predicate<DotRecast.Detour.StraightPathItem>
	// System.Predicate<ET.EntityRef<object>>
	// System.Predicate<ET.MessageSessionDispatcherInfo>
	// System.Predicate<ET.NumericWatcherInfo>
	// System.Predicate<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Predicate<System.Collections.Generic.KeyValuePair<long,object>>
	// System.Predicate<Unity.Mathematics.float3>
	// System.Predicate<int>
	// System.Predicate<long>
	// System.Predicate<object>
	// System.Predicate<ushort>
	// System.ReadOnlySpan.Enumerator<byte>
	// System.ReadOnlySpan<byte>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<ET.ActorId>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<byte>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<int>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<long>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<uint>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<ET.ActorId>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<long>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<uint>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<ET.ActorId>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<long>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<object>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<uint>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<ET.ActorId>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<long>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable.ConfiguredValueTaskAwaiter<uint>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<ET.ActorId>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<byte>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<int>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<long>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<object>
	// System.Runtime.CompilerServices.ConfiguredValueTaskAwaitable<uint>
	// System.Runtime.CompilerServices.TaskAwaiter<ET.ActorId>
	// System.Runtime.CompilerServices.TaskAwaiter<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.TaskAwaiter<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.TaskAwaiter<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.TaskAwaiter<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.TaskAwaiter<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.TaskAwaiter<byte>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Runtime.CompilerServices.TaskAwaiter<long>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Runtime.CompilerServices.TaskAwaiter<uint>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<ET.ActorId>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<ET.Client.Wait_CreateMyUnit>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<ET.Client.Wait_SceneChangeFinish>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<ET.Client.Wait_UnitStop>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,ET.ActorId>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,byte>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,int>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,long>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,object>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<byte,uint>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<System.ValueTuple<uint,object>>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<int>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<long>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<object>
	// System.Runtime.CompilerServices.ValueTaskAwaiter<uint>
	// System.Span.Enumerator<byte>
	// System.Span<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<ET.ActorId>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<long>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<object>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<uint>
	// System.Threading.Tasks.Sources.IValueTaskSource<ET.ActorId>
	// System.Threading.Tasks.Sources.IValueTaskSource<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.Sources.IValueTaskSource<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.Sources.IValueTaskSource<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.Sources.IValueTaskSource<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.Sources.IValueTaskSource<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.Sources.IValueTaskSource<byte>
	// System.Threading.Tasks.Sources.IValueTaskSource<int>
	// System.Threading.Tasks.Sources.IValueTaskSource<long>
	// System.Threading.Tasks.Sources.IValueTaskSource<object>
	// System.Threading.Tasks.Sources.IValueTaskSource<uint>
	// System.Threading.Tasks.Task<ET.ActorId>
	// System.Threading.Tasks.Task<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.Task<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.Task<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.Task<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.Task<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.Task<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.Task<byte>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.Task<long>
	// System.Threading.Tasks.Task<object>
	// System.Threading.Tasks.Task<uint>
	// System.Threading.Tasks.TaskFactory.<>c<ET.ActorId>
	// System.Threading.Tasks.TaskFactory.<>c<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.TaskFactory.<>c<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.TaskFactory.<>c<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.TaskFactory.<>c<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.TaskFactory.<>c<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.TaskFactory.<>c<byte>
	// System.Threading.Tasks.TaskFactory.<>c<int>
	// System.Threading.Tasks.TaskFactory.<>c<long>
	// System.Threading.Tasks.TaskFactory.<>c<object>
	// System.Threading.Tasks.TaskFactory.<>c<uint>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<ET.ActorId>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<byte>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<long>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass32_0<uint>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<ET.ActorId>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<byte>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<long>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<object>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<uint>
	// System.Threading.Tasks.TaskFactory<ET.ActorId>
	// System.Threading.Tasks.TaskFactory<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.TaskFactory<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.TaskFactory<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.TaskFactory<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.TaskFactory<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.TaskFactory<byte>
	// System.Threading.Tasks.TaskFactory<int>
	// System.Threading.Tasks.TaskFactory<long>
	// System.Threading.Tasks.TaskFactory<object>
	// System.Threading.Tasks.TaskFactory<uint>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<ET.ActorId>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<byte>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<int>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<long>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c<uint>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<ET.ActorId>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<byte>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<int>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<long>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<object>
	// System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask<uint>
	// System.Threading.Tasks.ValueTask<ET.ActorId>
	// System.Threading.Tasks.ValueTask<ET.Client.WaitType.Wait_Room2C_Start>
	// System.Threading.Tasks.ValueTask<ET.Client.Wait_CreateMyUnit>
	// System.Threading.Tasks.ValueTask<ET.Client.Wait_SceneChangeFinish>
	// System.Threading.Tasks.ValueTask<ET.Client.Wait_UnitStop>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,ET.ActorId>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,byte>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,int>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,long>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,object>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<byte,uint>>
	// System.Threading.Tasks.ValueTask<System.ValueTuple<uint,object>>
	// System.Threading.Tasks.ValueTask<byte>
	// System.Threading.Tasks.ValueTask<int>
	// System.Threading.Tasks.ValueTask<long>
	// System.Threading.Tasks.ValueTask<object>
	// System.Threading.Tasks.ValueTask<uint>
	// System.Tuple<object,object,object>
	// System.Tuple<object,object>
	// System.ValueTuple<ET.ActorId,object>
	// System.ValueTuple<byte,ET.ActorId>
	// System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>
	// System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>
	// System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>
	// System.ValueTuple<byte,ET.Client.Wait_UnitStop>
	// System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>
	// System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>
	// System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>
	// System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>
	// System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.ActorId>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.WaitType.Wait_Room2C_Start>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_CreateMyUnit>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_SceneChangeFinish>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,ET.Client.Wait_UnitStop>>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,byte>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,int>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,long>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,object>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<byte,uint>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,System.ValueTuple<uint,object>>>
	// System.ValueTuple<byte,System.ValueTuple<byte,byte>>
	// System.ValueTuple<byte,System.ValueTuple<byte,int>>
	// System.ValueTuple<byte,System.ValueTuple<byte,long>>
	// System.ValueTuple<byte,System.ValueTuple<byte,object>>
	// System.ValueTuple<byte,System.ValueTuple<byte,uint>>
	// System.ValueTuple<byte,System.ValueTuple<uint,object>>
	// System.ValueTuple<byte,byte>
	// System.ValueTuple<byte,int>
	// System.ValueTuple<byte,long>
	// System.ValueTuple<byte,object>
	// System.ValueTuple<byte,uint>
	// System.ValueTuple<object,int>
	// System.ValueTuple<uint,object>
	// System.ValueTuple<uint,uint>
	// System.ValueTuple<ushort,object>
	// UnityGameFramework.Extension.Awaitable.<>c__DisplayClass25_0<object>
	// UnityGameFramework.Extension.Awaitable.MoveNextFunc<object>
	// UnityGameFramework.Extension.Awaitable.UniTaskConfiguredSource.<>c<object>
	// UnityGameFramework.Extension.Awaitable.UniTaskConfiguredSource<object>
	// }}

	public void RefMethods()
	{
		// System.Collections.Generic.IEnumerable<object> CSharpx.EnumerableExtensions.Memoize<object>(System.Collections.Generic.IEnumerable<object>)
		// CSharpx.Just<object> CSharpx.Maybe.Just<object>(object)
		// CSharpx.Maybe<object> CSharpx.Maybe.Nothing<object>()
		// object CSharpx.MaybeExtensions.MapValueOrDefault<object,object>(CSharpx.Maybe<object>,System.Func<object,object>,object)
		// CommandLine.ParserResult<object> CommandLine.Core.InstanceBuilder.Build<object>(CSharpx.Maybe<System.Func<object>>,System.Func<System.Collections.Generic.IEnumerable<string>,System.Collections.Generic.IEnumerable<CommandLine.Core.OptionSpecification>,RailwaySharp.ErrorHandling.Result<System.Collections.Generic.IEnumerable<CommandLine.Core.Token>,CommandLine.Error>>,System.Collections.Generic.IEnumerable<string>,System.StringComparer,bool,System.Globalization.CultureInfo,bool,bool,System.Collections.Generic.IEnumerable<CommandLine.ErrorType>)
		// System.Collections.Generic.IEnumerable<object> CommandLine.Core.ReflectionExtensions.GetSpecifications<object>(System.Type,System.Func<System.Reflection.PropertyInfo,object>)
		// RailwaySharp.ErrorHandling.Result<System.Collections.Generic.IEnumerable<CommandLine.Core.Token>,CommandLine.Error> CommandLine.Parser.<ParseArguments>b__11_0<object>(System.Collections.Generic.IEnumerable<string>,System.Collections.Generic.IEnumerable<CommandLine.Core.OptionSpecification>)
		// CommandLine.ParserResult<object> CommandLine.Parser.DisplayHelp<object>(CommandLine.ParserResult<object>,System.IO.TextWriter,int)
		// CommandLine.ParserResult<object> CommandLine.Parser.MakeParserResult<object>(CommandLine.ParserResult<object>,CommandLine.ParserSettings)
		// CommandLine.ParserResult<object> CommandLine.Parser.ParseArguments<object>(System.Collections.Generic.IEnumerable<string>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithNotParsed<object>(CommandLine.ParserResult<object>,System.Action<System.Collections.Generic.IEnumerable<CommandLine.Error>>)
		// CommandLine.ParserResult<object> CommandLine.ParserResultExtensions.WithParsed<object>(CommandLine.ParserResult<object>,System.Action<object>)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ADynamicEvent.<Handle>d__5<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ADynamicEvent.<Handle>d__5<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.A2NetClient_MessageHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.A2NetClient_MessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AI_Attack.<Execute>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.EntryEvent3_InitClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.FiberInit_NetClient.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.FiberInit_NetClient.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.FiberInit_Robot.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.FiberInit_Robot.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.G2C_ReconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.G2C_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LSSceneInitFinish_Finish.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LSSceneInitFinish_Finish.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.LoginHelper.<Login>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.M2C_StopHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Main2NetClient_LoginHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Main2NetClient_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.OneFrameInputsHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.OneFrameInputsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.PingComponentSystem.<PingAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_CheckHashFailHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.Room2C_EnterMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.Room2C_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterAddressComponentSystem.<Init>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ConsoleComponentSystem.<Start>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ConsoleComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.DynamicEventSystem.<PublishAsync>d__13<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.DynamicEventSystem.<PublishAsync>d__13<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.EntryEvent1_InitDynamicEvent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.EntryEvent1_InitDynamicEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.EntryEvent1_InitShare.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.FiberInit_Main.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.FiberInit_Main.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.LubanLoadAllAsyncHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.LubanLoadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.LubanLoadOneAsyncHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.LubanLoadOneAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.LubanReloadAllAsyncHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.LubanReloadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MessageHandler.<Handle>d__1<object,object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MessageHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MessageHandler.<Handle>d__1<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MessageHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MessageSessionHandler.<HandleAsync>d__2<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MessageSessionHandler.<HandleAsync>d__2<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.MessageSessionHandler.<HandleAsync>d__2<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.MessageSessionHandler.<HandleAsync>d__2<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ReloadConfigConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ReloadDllConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.A2NetInner_MessageHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.A2NetInner_MessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ARobotCase.<Handle>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.BenchmarkClientComponentSystem.<Start>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.BenchmarkClientComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_BenchmarkHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_BenchmarkHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_LoginGateHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_LoginGateHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2G_PingHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2G_PingHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_PathfindingResultHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_StopHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_TestRobotCaseHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2M_TransferMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2M_TransferMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2R_LoginHandler.<CloseSession>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2R_LoginHandler.<CloseSession>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.C2Room_CheckHashHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.C2Room_CheckHashHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ChangePosition_NotifyAOI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ChangePosition_NotifyAOI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.CreateRobotConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.CreateRobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_BenchmarkClient.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_BenchmarkClient.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_BenchmarkServer.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_BenchmarkServer.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Gate.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Gate.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Location.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Location.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Map.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Map.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Match.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Match.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_NetInner.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_NetInner.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Realm.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Realm.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_RoomRoot.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_RoomRoot.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_Router.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_Router.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FiberInit_RouterManager.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FiberInit_RouterManager.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.FrameMessageHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.FrameMessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2M_SessionDisconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2Match_MatchHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2Match_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.G2Room_ReconnectHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.G2Room_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.HttpComponentSystem.<Handle>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.HttpComponentSystem.<Handle>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.HttpGetRouterHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.HttpGetRouterHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationProxyComponentSystem.<AddLocation>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationProxyComponentSystem.<AddLocation>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.MessageLocationHandler.<Handle>d__1<object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.MessageLocationHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectAddRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectAddRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectLockRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectRemoveRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectRemoveRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ObjectUnLockRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ObjectUnLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.R2G_GetLoginKeyHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotConsoleHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RoomManager2Room_InitHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RoomManager2Room_InitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.TransferHelper.<Transfer>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.TransferHelper.<TransferAtFrameFinish>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Tables.<LoadAsync>d__36>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Tables.<LoadAsync>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>,ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>&,ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>,ET.Server.ObjectGetRequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>&,ET.Server.ObjectGetRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.WaitType.Wait_Room2C_Start>,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.WaitType.Wait_Room2C_Start>&,ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_CreateMyUnit>&,ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>,ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.Client.Wait_SceneChangeFinish>&,ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Client.MoveHelper.<MoveToAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Server.MoveHelper.<FindPathMoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Client.AI_XunLuo.<Execute>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Server.EntryEvent2_InitServer.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Server.EntryEvent2_InitServer.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Server.RobotManagerComponentSystem.<NewRobot>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Server.RobotManagerComponentSystem.<NewRobot>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<long>,ET.Client.LoginHelper.<Login>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<long>&,ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.A2NetClient_RequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.A2NetClient_RequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.EnterMapHelper.<Match>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.EnterMapHelper.<Match>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.Main2NetClient_LoginHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.Main2NetClient_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.PingComponentSystem.<PingAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTAIConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTAIConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTDemo.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTDemo.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTOneConfig.<LoadAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTOneConfig.<LoadAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTStartMachineConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTStartMachineConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTStartProcessConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTStartProcessConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTStartSceneConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTStartSceneConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTStartZoneConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTStartZoneConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.DTUnitConfig.<LoadAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.DTUnitConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanLoadOneAsyncHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanLoadOneAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanReloadAllAsyncHandler.<Handle>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanReloadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.A2NetInner_RequestHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.A2NetInner_RequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ARobotCase.<Handle>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_EnterMapHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2G_MatchHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2G_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.C2R_LoginHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.C2R_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Add>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Lock>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Lock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Remove>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Remove>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Add>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Lock>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Lock>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Remove>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Remove>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<UnLock>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<UnLock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MatchComponentSystem.<Match>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MatchComponentSystem.<Match>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.TransferHelper.<Transfer>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.ConsoleComponentSystem.<Start>d__1>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.ConsoleComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Server.HttpComponentSystem.<Accept>d__2>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Server.HttpComponentSystem.<Accept>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.ActorId>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationOneTypeSystem.<Get>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationOneTypeSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.ActorId>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.LocationProxyComponentSystem.<Get>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.LocationProxyComponentSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<uint>,ET.Client.RouterHelper.<GetRouterAddress>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<uint>&,ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.MoveComponentSystem.<MoveToAsync>d__5>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>,ET.Client.MoveHelper.<MoveToAsync>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<byte,ET.Client.Wait_UnitStop>>&,ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.GateMapFactory.<Create>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.GateMapFactory.<Create>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Server.RobotCaseComponentSystem.<New>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Server.RobotCaseComponentSystem.<New>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>,ET.Server.MessageLocationSenderComponentSystem.<Call>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>&,ET.Server.MessageLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<ET.ActorId>&,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>,ET.Client.RouterHelper.<CreateRouterSession>d__0>(Cysharp.Threading.Tasks.UniTask.Awaiter<System.ValueTuple<uint,object>>&,ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.ClientSenderComponentSystem.<Call>d__6>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.ClientSenderComponentSystem.<Call>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageLocationSenderComponentSystem.<Call>d__10>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageLocationSenderComponentSystem.<Call>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageLocationSenderComponentSystem.<Call>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.MessageSenderSystem.<Call>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.MessageSenderSystem.<Call>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.ProcessOuterSenderSystem.<Call>d__13>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.ProcessOuterSenderSystem.<Call>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.SessionSystem.<Call>d__4>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.SessionSystem.<Call>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,ET.Client.HttpClientHelper.<Get>d__0>(System.Runtime.CompilerServices.TaskAwaiter<object>&,ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.RouterHelper.<Connect>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ADynamicEvent.<Handle>d__5<object,object>>(ET.ADynamicEvent.<Handle>d__5<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.A2NetClient_MessageHandler.<Run>d__0>(ET.Client.A2NetClient_MessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.A2NetClient_RequestHandler.<Run>d__0>(ET.Client.A2NetClient_RequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AI_Attack.<Execute>d__1>(ET.Client.AI_Attack.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AI_XunLuo.<Execute>d__1>(ET.Client.AI_XunLuo.<Execute>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateClientScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0>(ET.Client.AfterCreateClientScene_LSAddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0>(ET.Client.AfterCreateCurrentScene_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0>(ET.Client.AfterUnitCreate_CreateUnitView.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0>(ET.Client.AppStartInitFinish_CreateLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0>(ET.Client.AppStartInitFinish_CreateUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0>(ET.Client.ChangePosition_SyncGameObjectPos.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0>(ET.Client.ChangeRotation_SyncGameObjectRotation.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3>(ET.Client.ClientSenderComponentSystem.<DisposeAsync>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2>(ET.Client.ClientSenderComponentSystem.<RemoveFiberAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EnterMapHelper.<EnterMapAsync>d__0>(ET.Client.EnterMapHelper.<EnterMapAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EnterMapHelper.<Match>d__1>(ET.Client.EnterMapHelper.<Match>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.EntryEvent3_InitClient.<Run>d__0>(ET.Client.EntryEvent3_InitClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.FiberInit_NetClient.<Handle>d__0>(ET.Client.FiberInit_NetClient.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.FiberInit_Robot.<Handle>d__0>(ET.Client.FiberInit_Robot.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.G2C_ReconnectHandler.<Run>d__0>(ET.Client.G2C_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0>(ET.Client.LSSceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2>(ET.Client.LSSceneChangeHelper.<SceneChangeToReconnect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1>(ET.Client.LSSceneChangeHelper.<SceneChangeToReplay>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0>(ET.Client.LSSceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSSceneInitFinish_Finish.<Run>d__0>(ET.Client.LSSceneInitFinish_Finish.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2>(ET.Client.LSUnitViewComponentSystem.<InitAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0>(ET.Client.LoginFinish_CreateLobbyUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0>(ET.Client.LoginFinish_CreateUILSLobby.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0>(ET.Client.LoginFinish_RemoveLoginUI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0>(ET.Client.LoginFinish_RemoveUILSLogin.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.LoginHelper.<Login>d__0>(ET.Client.LoginHelper.<Login>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_CreateMyUnitHandler.<Run>d__0>(ET.Client.M2C_CreateMyUnitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_CreateUnitsHandler.<Run>d__0>(ET.Client.M2C_CreateUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_PathfindingResultHandler.<Run>d__0>(ET.Client.M2C_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_RemoveUnitsHandler.<Run>d__0>(ET.Client.M2C_RemoveUnitsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_StartSceneChangeHandler.<Run>d__0>(ET.Client.M2C_StartSceneChangeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.M2C_StopHandler.<Run>d__0>(ET.Client.M2C_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Main2NetClient_LoginHandler.<Run>d__0>(ET.Client.Main2NetClient_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(ET.Client.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.MoveHelper.<MoveToAsync>d__1>(ET.Client.MoveHelper.<MoveToAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0>(ET.Client.NetClient2Main_SessionDisposeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.OneFrameInputsHandler.<Run>d__0>(ET.Client.OneFrameInputsHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.PingComponentSystem.<PingAsync>d__2>(ET.Client.PingComponentSystem.<PingAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0>(ET.Client.Room2C_AdjustUpdateTimeHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_CheckHashFailHandler.<Run>d__0>(ET.Client.Room2C_CheckHashFailHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.Room2C_EnterMapHandler.<Run>d__0>(ET.Client.Room2C_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2>(ET.Client.RouterAddressComponentSystem.<GetAllRouter>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<Init>d__1>(ET.Client.RouterAddressComponentSystem.<Init>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3>(ET.Client.RouterAddressComponentSystem.<WaitTenMinGetAllRouter>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1>(ET.Client.RouterCheckComponentSystem.<CheckAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0>(ET.Client.SceneChangeFinishEvent_CreateUIHelp.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeHelper.<SceneChangeTo>d__0>(ET.Client.SceneChangeHelper.<SceneChangeTo>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.SceneChangeStart_AddComponent.<Run>d__0>(ET.Client.SceneChangeStart_AddComponent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3>(ET.Client.UGFUILSLobbyComponentSystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3>(ET.Client.UGFUILobbyComponentSystem.<EnterMap>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ConsoleComponentSystem.<Start>d__1>(ET.ConsoleComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTAIConfig.<LoadAsync>d__4>(ET.DTAIConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTDemo.<LoadAsync>d__4>(ET.DTDemo.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTOneConfig.<LoadAsync>d__5>(ET.DTOneConfig.<LoadAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTStartMachineConfig.<LoadAsync>d__4>(ET.DTStartMachineConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTStartProcessConfig.<LoadAsync>d__4>(ET.DTStartProcessConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTStartSceneConfig.<LoadAsync>d__4>(ET.DTStartSceneConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTStartZoneConfig.<LoadAsync>d__4>(ET.DTStartZoneConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DTUnitConfig.<LoadAsync>d__4>(ET.DTUnitConfig.<LoadAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.DynamicEventSystem.<PublishAsync>d__13<object>>(ET.DynamicEventSystem.<PublishAsync>d__13<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EntryEvent1_InitDynamicEvent.<Run>d__0>(ET.EntryEvent1_InitDynamicEvent.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EntryEvent1_InitShare.<Run>d__0>(ET.EntryEvent1_InitShare.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.Client.AppStartInitFinish>>(ET.EventSystem.<PublishAsync>d__4<object,ET.Client.AppStartInitFinish>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.Client.LSSceneChangeStart>>(ET.EventSystem.<PublishAsync>d__4<object,ET.Client.LSSceneChangeStart>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.Client.LoginFinish>>(ET.EventSystem.<PublishAsync>d__4<object,ET.Client.LoginFinish>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent1>>(ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent1>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent2>>(ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent2>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent3>>(ET.EventSystem.<PublishAsync>d__4<object,ET.EntryEvent3>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.FiberInit_Main.<Handle>d__0>(ET.FiberInit_Main.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanLoadAllAsyncHandler.<Handle>d__0>(ET.LubanLoadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanLoadOneAsyncHandler.<Handle>d__0>(ET.LubanLoadOneAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.LubanReloadAllAsyncHandler.<Handle>d__0>(ET.LubanReloadAllAsyncHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1>(ET.MailBoxType_OrderedMessageHandler.<HandleInner>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1>(ET.MailBoxType_UnOrderedMessageHandler.<HandleAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MessageHandler.<Handle>d__1<object,object,object>>(ET.MessageHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MessageHandler.<Handle>d__1<object,object>>(ET.MessageHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MessageSessionHandler.<HandleAsync>d__2<object,object>>(ET.MessageSessionHandler.<HandleAsync>d__2<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.MessageSessionHandler.<HandleAsync>d__2<object>>(ET.MessageSessionHandler.<HandleAsync>d__2<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.NumericChangeEvent_NotifyWatcher.<Run>d__0>(ET.NumericChangeEvent_NotifyWatcher.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ReloadConfigConsoleHandler.<Run>d__0>(ET.ReloadConfigConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.ReloadDllConsoleHandler.<Run>d__0>(ET.ReloadDllConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.A2NetInner_MessageHandler.<Run>d__0>(ET.Server.A2NetInner_MessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.A2NetInner_RequestHandler.<Run>d__0>(ET.Server.A2NetInner_RequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ARobotCase.<Handle>d__1>(ET.Server.ARobotCase.<Handle>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d>(ET.Server.BenchmarkClientComponentSystem.<<Start>g__Call|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.BenchmarkClientComponentSystem.<Start>d__1>(ET.Server.BenchmarkClientComponentSystem.<Start>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_BenchmarkHandler.<Run>d__0>(ET.Server.C2G_BenchmarkHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_EnterMapHandler.<Run>d__0>(ET.Server.C2G_EnterMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1>(ET.Server.C2G_LoginGateHandler.<CheckRoom>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_LoginGateHandler.<Run>d__0>(ET.Server.C2G_LoginGateHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_MatchHandler.<Run>d__0>(ET.Server.C2G_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2G_PingHandler.<Run>d__0>(ET.Server.C2G_PingHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_PathfindingResultHandler.<Run>d__0>(ET.Server.C2M_PathfindingResultHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_StopHandler.<Run>d__0>(ET.Server.C2M_StopHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_TestRobotCaseHandler.<Run>d__0>(ET.Server.C2M_TestRobotCaseHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2M_TransferMapHandler.<Run>d__0>(ET.Server.C2M_TransferMapHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2R_LoginHandler.<CloseSession>d__1>(ET.Server.C2R_LoginHandler.<CloseSession>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2R_LoginHandler.<Run>d__0>(ET.Server.C2R_LoginHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0>(ET.Server.C2Room_ChangeSceneFinishHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.C2Room_CheckHashHandler.<Run>d__0>(ET.Server.C2Room_CheckHashHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ChangePosition_NotifyAOI.<Run>d__0>(ET.Server.ChangePosition_NotifyAOI.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.CreateRobotConsoleHandler.<Run>d__0>(ET.Server.CreateRobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.EntryEvent2_InitServer.<Run>d__0>(ET.Server.EntryEvent2_InitServer.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_BenchmarkClient.<Handle>d__0>(ET.Server.FiberInit_BenchmarkClient.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_BenchmarkServer.<Handle>d__0>(ET.Server.FiberInit_BenchmarkServer.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Gate.<Handle>d__0>(ET.Server.FiberInit_Gate.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Location.<Handle>d__0>(ET.Server.FiberInit_Location.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Map.<Handle>d__0>(ET.Server.FiberInit_Map.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Match.<Handle>d__0>(ET.Server.FiberInit_Match.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_NetInner.<Handle>d__0>(ET.Server.FiberInit_NetInner.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Realm.<Handle>d__0>(ET.Server.FiberInit_Realm.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_RoomRoot.<Handle>d__0>(ET.Server.FiberInit_RoomRoot.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_Router.<Handle>d__0>(ET.Server.FiberInit_Router.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FiberInit_RouterManager.<Handle>d__0>(ET.Server.FiberInit_RouterManager.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.FrameMessageHandler.<Run>d__0>(ET.Server.FrameMessageHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2M_SessionDisconnectHandler.<Run>d__0>(ET.Server.G2M_SessionDisconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2Match_MatchHandler.<Run>d__0>(ET.Server.G2Match_MatchHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.G2Room_ReconnectHandler.<Run>d__0>(ET.Server.G2Room_ReconnectHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3>(ET.Server.GateSessionKeyComponentSystem.<TimeoutRemoveKey>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.HttpComponentSystem.<Accept>d__2>(ET.Server.HttpComponentSystem.<Accept>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.HttpComponentSystem.<Handle>d__3>(ET.Server.HttpComponentSystem.<Handle>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.HttpGetRouterHandler.<Handle>d__0>(ET.Server.HttpGetRouterHandler.<Handle>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d>(ET.Server.LocationOneTypeSystem.<>c__DisplayClass3_0.<<Lock>g__TimeWaitAsync|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Add>d__1>(ET.Server.LocationOneTypeSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Lock>d__3>(ET.Server.LocationOneTypeSystem.<Lock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationOneTypeSystem.<Remove>d__2>(ET.Server.LocationOneTypeSystem.<Remove>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Add>d__1>(ET.Server.LocationProxyComponentSystem.<Add>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<AddLocation>d__6>(ET.Server.LocationProxyComponentSystem.<AddLocation>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Lock>d__2>(ET.Server.LocationProxyComponentSystem.<Lock>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<Remove>d__4>(ET.Server.LocationProxyComponentSystem.<Remove>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7>(ET.Server.LocationProxyComponentSystem.<RemoveLocation>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.LocationProxyComponentSystem.<UnLock>d__3>(ET.Server.LocationProxyComponentSystem.<UnLock>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0>(ET.Server.M2M_UnitTransferRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0>(ET.Server.Match2G_NotifyMatchSuccessHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.Match2Map_GetRoomHandler.<Run>d__0>(ET.Server.Match2Map_GetRoomHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MatchComponentSystem.<Match>d__0>(ET.Server.MatchComponentSystem.<Match>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>>(ET.Server.MessageLocationHandler.<Handle>d__1<object,object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MessageLocationHandler.<Handle>d__1<object,object>>(ET.Server.MessageLocationHandler.<Handle>d__1<object,object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7>(ET.Server.MessageLocationSenderComponentSystem.<SendInner>d__7&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.MoveHelper.<FindPathMoveToAsync>d__0>(ET.Server.MoveHelper.<FindPathMoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectAddRequestHandler.<Run>d__0>(ET.Server.ObjectAddRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectGetRequestHandler.<Run>d__0>(ET.Server.ObjectGetRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectLockRequestHandler.<Run>d__0>(ET.Server.ObjectLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectRemoveRequestHandler.<Run>d__0>(ET.Server.ObjectRemoveRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ObjectUnLockRequestHandler.<Run>d__0>(ET.Server.ObjectUnLockRequestHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d>(ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass13_0.<<Call>g__Timeout|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d>(ET.Server.ProcessOuterSenderSystem.<>c__DisplayClass3_0.<<OnRead>g__CallInner|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.R2G_GetLoginKeyHandler.<Run>d__0>(ET.Server.R2G_GetLoginKeyHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotConsoleHandler.<Run>d__0>(ET.Server.RobotConsoleHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d>(ET.Server.RobotManagerComponentSystem.<<Destroy>g__Remove|1_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RobotManagerComponentSystem.<NewRobot>d__2>(ET.Server.RobotManagerComponentSystem.<NewRobot>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.RoomManager2Room_InitHandler.<Run>d__0>(ET.Server.RoomManager2Room_InitHandler.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.TransferHelper.<Transfer>d__1>(ET.Server.TransferHelper.<Transfer>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.TransferHelper.<TransferAtFrameFinish>d__0>(ET.Server.TransferHelper.<TransferAtFrameFinish>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0>(ET.Server.UnitEnterSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0>(ET.Server.UnitLeaveSightRange_NotifyClient.<Run>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<ET.Tables.<LoadAsync>d__36>(ET.Tables.<LoadAsync>d__36&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.ActorId>.Start<ET.Server.LocationOneTypeSystem.<Get>d__5>(ET.Server.LocationOneTypeSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<ET.ActorId>.Start<ET.Server.LocationProxyComponentSystem.<Get>d__5>(ET.Server.LocationProxyComponentSystem.<Get>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<System.ValueTuple<uint,object>>.Start<ET.Client.RouterHelper.<GetRouterAddress>d__1>(ET.Client.RouterHelper.<GetRouterAddress>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<byte>.Start<ET.MoveComponentSystem.<MoveToAsync>d__5>(ET.MoveComponentSystem.<MoveToAsync>d__5&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<int>.Start<ET.Client.MoveHelper.<MoveToAsync>d__0>(ET.Client.MoveHelper.<MoveToAsync>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<long>.Start<ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4>(ET.Client.ClientSenderComponentSystem.<LoginAsync>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.ClientSenderComponentSystem.<Call>d__6>(ET.Client.ClientSenderComponentSystem.<Call>d__6&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.HttpClientHelper.<Get>d__0>(ET.Client.HttpClientHelper.<Get>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.RouterHelper.<CreateRouterSession>d__0>(ET.Client.RouterHelper.<CreateRouterSession>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>>(ET.Client.UGFEntityComponentSystem.<ShowEntityAsync>d__2<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2>(ET.Client.UGFUIComponentSystem.<OpenUIFormAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d>(ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadByteBuf|0_0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d>(ET.LubanLoadAllAsyncHandler.<<Handle>g__LoadJson|0_1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.GateMapFactory.<Create>d__0>(ET.Server.GateMapFactory.<Create>d__0&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.MessageLocationSenderComponentSystem.<Call>d__10>(ET.Server.MessageLocationSenderComponentSystem.<Call>d__10&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.MessageLocationSenderComponentSystem.<Call>d__8>(ET.Server.MessageLocationSenderComponentSystem.<Call>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11>(ET.Server.MessageLocationSenderComponentSystem.<CallInner>d__11&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.MessageSenderSystem.<Call>d__2>(ET.Server.MessageSenderSystem.<Call>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.ProcessOuterSenderSystem.<Call>d__13>(ET.Server.ProcessOuterSenderSystem.<Call>d__13&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.Server.RobotCaseComponentSystem.<New>d__1>(ET.Server.RobotCaseComponentSystem.<New>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<object>.Start<ET.SessionSystem.<Call>d__4>(ET.SessionSystem.<Call>d__4&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder<uint>.Start<ET.Client.RouterHelper.<Connect>d__2>(ET.Client.RouterHelper.<Connect>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.OperaComponentSystem.<Test1>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.OperaComponentSystem.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.OperaComponentSystem.<Test2>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.OperaComponentSystem.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Client.TestComponentSystem.<RunTestCancel>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Client.TestComponentSystem.<RunTestCancel>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.Entry.<StartAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.Entry.<StartAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter&,ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>,ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>&,ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<int>,ET.Entry.<StartAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<int>&,ET.Entry.<StartAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.OperaComponentSystem.<Test1>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.OperaComponentSystem.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Client.OperaComponentSystem.<Test2>d__3>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Client.OperaComponentSystem.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.OperaComponentSystem.<Test1>d__2>(ET.Client.OperaComponentSystem.<Test1>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.OperaComponentSystem.<Test2>d__3>(ET.Client.OperaComponentSystem.<Test2>d__3&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d>(ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run1|0>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d>(ET.Client.TestComponentSystem.<>c__DisplayClass2_0.<<RunTestCancel>g__Run2|1>d&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Client.TestComponentSystem.<RunTestCancel>d__2>(ET.Client.TestComponentSystem.<RunTestCancel>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Entry.<StartAsync>d__2>(ET.Entry.<StartAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>>(ET.ObjectWaitSystem.<>c__DisplayClass5_0.<<Wait>g__WaitTimeout|0>d<object>&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1>(ET.Server.NetComponentOnReadInvoker_Gate.<HandleAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d>(ET.SessionSystem.<>c__DisplayClass4_0.<<Call>g__Timeout|0>d&)
		// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>> Cysharp.Threading.Tasks.Internal.StateTuple.Create<Cysharp.Threading.Tasks.UniTask.Awaiter<byte>>(Cysharp.Threading.Tasks.UniTask.Awaiter<byte>)
		// Cysharp.Threading.Tasks.Internal.StateTuple<Cysharp.Threading.Tasks.UniTask.Awaiter<object>> Cysharp.Threading.Tasks.Internal.StateTuple.Create<Cysharp.Threading.Tasks.UniTask.Awaiter<object>>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>)
		// Cysharp.Threading.Tasks.UniTask<object> Cysharp.Threading.Tasks.UniTask.FromCanceled<object>(System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtension.AttachCancellation<ET.Client.WaitType.Wait_Room2C_Start>(Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.WaitType.Wait_Room2C_Start>,System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtension.AttachCancellation<ET.Client.Wait_CreateMyUnit>(Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_CreateMyUnit>,System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtension.AttachCancellation<ET.Client.Wait_SceneChangeFinish>(Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_SceneChangeFinish>,System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtension.AttachCancellation<ET.Client.Wait_UnitStop>(Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<ET.Client.Wait_UnitStop>,System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtension.AttachCancellation<object>(Cysharp.Threading.Tasks.AutoResetUniTaskCompletionSourcePlus<object>,System.Threading.CancellationToken)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<byte>(Cysharp.Threading.Tasks.UniTask<byte>)
		// System.Void Cysharp.Threading.Tasks.UniTaskExtensions.Forget<object>(Cysharp.Threading.Tasks.UniTask<object>)
		// object ET.Entity.AddChild<object,ET.ActorId,object>(ET.ActorId,object,bool)
		// object ET.Entity.AddChild<object,object,long>(object,long,bool)
		// object ET.Entity.AddChild<object,object>(object,bool)
		// object ET.Entity.AddChild<object>(bool)
		// object ET.Entity.AddChildWithId<object,int>(long,int,bool)
		// object ET.Entity.AddChildWithId<object,object,object,object>(long,object,object,object,bool)
		// object ET.Entity.AddChildWithId<object,object,object>(long,object,object,bool)
		// object ET.Entity.AddChildWithId<object,object>(long,object,bool)
		// object ET.Entity.AddChildWithId<object>(long,bool)
		// object ET.Entity.AddComponent<object,int,Unity.Mathematics.float3>(int,Unity.Mathematics.float3,bool)
		// object ET.Entity.AddComponent<object,int,int>(int,int,bool)
		// object ET.Entity.AddComponent<object,int>(int,bool)
		// object ET.Entity.AddComponent<object,object,int>(object,int,bool)
		// object ET.Entity.AddComponent<object,object,object>(object,object,bool)
		// object ET.Entity.AddComponent<object,object>(object,bool)
		// object ET.Entity.AddComponent<object>(bool)
		// object ET.Entity.AddComponentWithId<object,int,Unity.Mathematics.float3>(long,int,Unity.Mathematics.float3,bool)
		// object ET.Entity.AddComponentWithId<object,int,int>(long,int,int,bool)
		// object ET.Entity.AddComponentWithId<object,int>(long,int,bool)
		// object ET.Entity.AddComponentWithId<object,object,int>(long,object,int,bool)
		// object ET.Entity.AddComponentWithId<object,object,object,object>(long,object,object,object,bool)
		// object ET.Entity.AddComponentWithId<object,object,object>(long,object,object,bool)
		// object ET.Entity.AddComponentWithId<object,object>(long,object,bool)
		// object ET.Entity.AddComponentWithId<object>(long,bool)
		// object ET.Entity.GetChild<object>(long)
		// object ET.Entity.GetComponent<object>()
		// object ET.Entity.GetParent<object>()
		// System.Void ET.Entity.RemoveComponent<object>()
		// System.Void ET.EntitySystemSingleton.Awake<ET.ActorId,object>(ET.Entity,ET.ActorId,object)
		// System.Void ET.EntitySystemSingleton.Awake<int,Unity.Mathematics.float3>(ET.Entity,int,Unity.Mathematics.float3)
		// System.Void ET.EntitySystemSingleton.Awake<int,int>(ET.Entity,int,int)
		// System.Void ET.EntitySystemSingleton.Awake<int>(ET.Entity,int)
		// System.Void ET.EntitySystemSingleton.Awake<object,int>(ET.Entity,object,int)
		// System.Void ET.EntitySystemSingleton.Awake<object,long>(ET.Entity,object,long)
		// System.Void ET.EntitySystemSingleton.Awake<object,object,object>(ET.Entity,object,object,object)
		// System.Void ET.EntitySystemSingleton.Awake<object,object>(ET.Entity,object,object)
		// System.Void ET.EntitySystemSingleton.Awake<object>(ET.Entity,object)
		// long ET.EnumHelper.FromString<long>(string)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.Invoke<ET.Server.RobotInvokeArgs,Cysharp.Threading.Tasks.UniTask>(long,ET.Server.RobotInvokeArgs)
		// System.Void ET.EventSystem.Invoke<ET.NetComponentOnRead>(long,ET.NetComponentOnRead)
		// System.Void ET.EventSystem.Publish<object,ET.ChangePosition>(object,ET.ChangePosition)
		// System.Void ET.EventSystem.Publish<object,ET.ChangeRotation>(object,ET.ChangeRotation)
		// System.Void ET.EventSystem.Publish<object,ET.Client.AfterCreateCurrentScene>(object,ET.Client.AfterCreateCurrentScene)
		// System.Void ET.EventSystem.Publish<object,ET.Client.AfterUnitCreate>(object,ET.Client.AfterUnitCreate)
		// System.Void ET.EventSystem.Publish<object,ET.Client.EnterMapFinish>(object,ET.Client.EnterMapFinish)
		// System.Void ET.EventSystem.Publish<object,ET.Client.LSSceneInitFinish>(object,ET.Client.LSSceneInitFinish)
		// System.Void ET.EventSystem.Publish<object,ET.Client.SceneChangeFinish>(object,ET.Client.SceneChangeFinish)
		// System.Void ET.EventSystem.Publish<object,ET.Client.SceneChangeStart>(object,ET.Client.SceneChangeStart)
		// System.Void ET.EventSystem.Publish<object,ET.MoveStart>(object,ET.MoveStart)
		// System.Void ET.EventSystem.Publish<object,ET.MoveStop>(object,ET.MoveStop)
		// System.Void ET.EventSystem.Publish<object,ET.NumbericChange>(object,ET.NumbericChange)
		// System.Void ET.EventSystem.Publish<object,ET.Server.UnitEnterSightRange>(object,ET.Server.UnitEnterSightRange)
		// System.Void ET.EventSystem.Publish<object,ET.Server.UnitLeaveSightRange>(object,ET.Server.UnitLeaveSightRange)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.Client.AppStartInitFinish>(object,ET.Client.AppStartInitFinish)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.Client.LSSceneChangeStart>(object,ET.Client.LSSceneChangeStart)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.Client.LoginFinish>(object,ET.Client.LoginFinish)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EntryEvent1>(object,ET.EntryEvent1)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EntryEvent2>(object,ET.EntryEvent2)
		// Cysharp.Threading.Tasks.UniTask ET.EventSystem.PublishAsync<object,ET.EntryEvent3>(object,ET.EntryEvent3)
		// object ET.MongoHelper.Deserialize<object>(byte[])
		// object ET.MongoHelper.FromJson<object>(string)
		// System.Void ET.ObjectHelper.Swap<object>(object&,object&)
		// System.Void ET.RandomGenerator.BreakRank<object>(System.Collections.Generic.List<object>)
		// object ET.RandomGenerator.RandomArray<object>(System.Collections.Generic.List<object>)
		// ET.UGFUIWidget ET.UGFUIForm.AddUIWidget<object>(UnityEngine.Transform,object)
		// ET.UGFUIWidget ET.UGFUIFormSystem.AddUIWidget<object>(ET.UGFUIForm,UnityEngine.Transform,object)
		// object ET.World.AddSingleton<object>()
		// Cysharp.Threading.Tasks.UniTask<UnityGameFramework.Runtime.Entity> Game.EntityExtension.ShowEntityAsync<object>(UnityGameFramework.Runtime.EntityComponent,int,object,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// string Luban.StringUtil.CollectionToString<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Collections.Generic.List<object> MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&)
		// System.Void MemoryPack.Formatters.ListFormatter.DeserializePackable<object>(MemoryPack.MemoryPackReader&,System.Collections.Generic.List<object>&)
		// System.Void MemoryPack.Formatters.ListFormatter.SerializePackable<object>(MemoryPack.MemoryPackWriter&,System.Collections.Generic.List<object>&)
		// byte[] MemoryPack.Internal.MemoryMarshalEx.AllocateUninitializedArray<byte>(int,bool)
		// byte& MemoryPack.Internal.MemoryMarshalEx.GetArrayDataReference<byte>(byte[])
		// MemoryPack.MemoryPackFormatter<byte> MemoryPack.MemoryPackFormatterProvider.GetFormatter<byte>()
		// MemoryPack.MemoryPackFormatter<long> MemoryPack.MemoryPackFormatterProvider.GetFormatter<long>()
		// MemoryPack.MemoryPackFormatter<object> MemoryPack.MemoryPackFormatterProvider.GetFormatter<object>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<ET.LSInput>()
		// bool MemoryPack.MemoryPackFormatterProvider.IsRegistered<object>()
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<ET.LSInput>(MemoryPack.MemoryPackFormatter<ET.LSInput>)
		// System.Void MemoryPack.MemoryPackFormatterProvider.Register<object>(MemoryPack.MemoryPackFormatter<object>)
		// System.Void MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<byte>(byte[]&)
		// byte[] MemoryPack.MemoryPackReader.DangerousReadUnmanagedArray<byte>()
		// MemoryPack.IMemoryPackFormatter<byte> MemoryPack.MemoryPackReader.GetFormatter<byte>()
		// MemoryPack.IMemoryPackFormatter<long> MemoryPack.MemoryPackReader.GetFormatter<long>()
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackReader.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadPackable<object>(object&)
		// object MemoryPack.MemoryPackReader.ReadPackable<object>()
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<ET.ActorId>(ET.ActorId&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<ET.LSInput>(ET.LSInput&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<TrueSync.TSQuaternion>(TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<TrueSync.TSVector>(TrueSync.TSVector&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.float3>(Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.quaternion,int>(Unity.Mathematics.quaternion&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<Unity.Mathematics.quaternion>(Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,ET.ActorId>(byte&,int&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,Unity.Mathematics.float3>(byte&,int&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,ET.ActorId,ET.ActorId>(byte&,int&,int&,long&,ET.ActorId&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,ET.ActorId,int>(byte&,int&,int&,long&,ET.ActorId&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long,ET.ActorId>(byte&,int&,int&,long&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int,long>(byte&,int&,int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,int>(byte&,int&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,ET.LSInput>(byte&,int&,long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,Unity.Mathematics.float3,Unity.Mathematics.quaternion>(byte&,int&,long&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long,long>(byte&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int,long>(byte&,int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,int>(byte&,int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,TrueSync.TSVector,TrueSync.TSQuaternion>(byte&,long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,Unity.Mathematics.float3>(byte&,long&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,int,int,Unity.Mathematics.float3,Unity.Mathematics.float3>(byte&,long&,int&,int&,Unity.Mathematics.float3&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long,int,long>(byte&,long&,int&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,long>(byte&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte,uint>(byte&,uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<byte>(byte&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int,ET.ActorId>(int&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long,ET.LSInput>(long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long,TrueSync.TSVector,TrueSync.TSQuaternion>(long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long,long>(long&,long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<long>(long&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanaged<uint>(uint&)
		// System.Void MemoryPack.MemoryPackReader.ReadUnmanagedArray<byte>(byte[]&)
		// byte[] MemoryPack.MemoryPackReader.ReadUnmanagedArray<byte>()
		// System.Void MemoryPack.MemoryPackReader.ReadValue<object>(object&)
		// byte MemoryPack.MemoryPackReader.ReadValue<byte>()
		// long MemoryPack.MemoryPackReader.ReadValue<long>()
		// object MemoryPack.MemoryPackReader.ReadValue<object>()
		// System.Void MemoryPack.MemoryPackWriter.DangerousWriteUnmanagedArray<byte>(byte[])
		// MemoryPack.IMemoryPackFormatter<byte> MemoryPack.MemoryPackWriter.GetFormatter<byte>()
		// MemoryPack.IMemoryPackFormatter<long> MemoryPack.MemoryPackWriter.GetFormatter<long>()
		// MemoryPack.IMemoryPackFormatter<object> MemoryPack.MemoryPackWriter.GetFormatter<object>()
		// System.Void MemoryPack.MemoryPackWriter.WritePackable<object>(object&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<ET.ActorId>(ET.ActorId&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<ET.LSInput>(ET.LSInput&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<Unity.Mathematics.quaternion,int>(Unity.Mathematics.quaternion&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<int,ET.ActorId>(int&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<int>(int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long,ET.LSInput>(long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long,TrueSync.TSVector,TrueSync.TSQuaternion>(long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long,long>(long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanaged<long>(long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedArray<byte>(byte[])
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,ET.ActorId>(byte,byte&,int&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,Unity.Mathematics.float3>(byte,byte&,int&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,ET.ActorId,ET.ActorId>(byte,byte&,int&,int&,long&,ET.ActorId&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,ET.ActorId,int>(byte,byte&,int&,int&,long&,ET.ActorId&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long,ET.ActorId>(byte,byte&,int&,int&,long&,ET.ActorId&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int,long>(byte,byte&,int&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,int>(byte,byte&,int&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,ET.LSInput>(byte,byte&,int&,long&,ET.LSInput&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,Unity.Mathematics.float3,Unity.Mathematics.quaternion>(byte,byte&,int&,long&,Unity.Mathematics.float3&,Unity.Mathematics.quaternion&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long,long>(byte,byte&,int&,long&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int,long>(byte,byte&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,int>(byte,byte&,int&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,TrueSync.TSVector,TrueSync.TSQuaternion>(byte,byte&,long&,TrueSync.TSVector&,TrueSync.TSQuaternion&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,Unity.Mathematics.float3>(byte,byte&,long&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,int,int,Unity.Mathematics.float3,Unity.Mathematics.float3>(byte,byte&,long&,int&,int&,Unity.Mathematics.float3&,Unity.Mathematics.float3&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long,int,long>(byte,byte&,long&,int&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,long>(byte,byte&,long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte,uint>(byte,byte&,uint&)
		// System.Void MemoryPack.MemoryPackWriter.WriteUnmanagedWithObjectHeader<byte>(byte,byte&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<byte>(byte&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<long>(long&)
		// System.Void MemoryPack.MemoryPackWriter.WriteValue<object>(object&)
		// object MongoDB.Bson.Serialization.BsonSerializer.Deserialize<object>(MongoDB.Bson.IO.IBsonReader,System.Action<MongoDB.Bson.Serialization.BsonDeserializationContext.Builder>)
		// object MongoDB.Bson.Serialization.BsonSerializer.Deserialize<object>(string,System.Action<MongoDB.Bson.Serialization.BsonDeserializationContext.Builder>)
		// MongoDB.Bson.Serialization.IBsonSerializer<object> MongoDB.Bson.Serialization.BsonSerializer.LookupSerializer<object>()
		// object MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize<object>(MongoDB.Bson.Serialization.IBsonSerializer<object>,MongoDB.Bson.Serialization.BsonDeserializationContext)
		// object System.Activator.CreateInstance<object>()
		// byte[] System.Array.Empty<byte>()
		// object[] System.Array.Empty<object>()
		// int System.HashCode.Combine<TrueSync.TSVector2,int>(TrueSync.TSVector2,int)
		// int System.HashCode.Combine<object>(object)
		// bool System.Linq.Enumerable.Any<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Empty<object>()
		// System.Collections.Generic.IEnumerable<System.Linq.IGrouping<object,object>> System.Linq.Enumerable.GroupBy<object,object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfType<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfTypeIterator<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectMany<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectManyIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.Collections.Generic.IEnumerable<object>>)
		// ET.EntityRef<object>[] System.Linq.Enumerable.ToArray<ET.EntityRef<object>>(System.Collections.Generic.IEnumerable<ET.EntityRef<object>>)
		// ET.RpcInfo[] System.Linq.Enumerable.ToArray<ET.RpcInfo>(System.Collections.Generic.IEnumerable<ET.RpcInfo>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// System.Span<byte> System.MemoryExtensions.AsSpan<byte>(byte[])
		// byte& System.Runtime.CompilerServices.Unsafe.Add<byte>(byte&,int)
		// byte& System.Runtime.CompilerServices.Unsafe.As<byte,byte>(byte&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// byte& System.Runtime.CompilerServices.Unsafe.AsRef<byte>(byte&)
		// long& System.Runtime.CompilerServices.Unsafe.AsRef<long>(long&)
		// object& System.Runtime.CompilerServices.Unsafe.AsRef<object>(object&)
		// ET.ActorId System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ET.ActorId>(byte&)
		// ET.LSInput System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ET.LSInput>(byte&)
		// TrueSync.TSQuaternion System.Runtime.CompilerServices.Unsafe.ReadUnaligned<TrueSync.TSQuaternion>(byte&)
		// TrueSync.TSVector System.Runtime.CompilerServices.Unsafe.ReadUnaligned<TrueSync.TSVector>(byte&)
		// Unity.Mathematics.float3 System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Unity.Mathematics.float3>(byte&)
		// Unity.Mathematics.quaternion System.Runtime.CompilerServices.Unsafe.ReadUnaligned<Unity.Mathematics.quaternion>(byte&)
		// byte System.Runtime.CompilerServices.Unsafe.ReadUnaligned<byte>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.ReadUnaligned<int>(byte&)
		// long System.Runtime.CompilerServices.Unsafe.ReadUnaligned<long>(byte&)
		// uint System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>(byte&)
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ET.ActorId>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<ET.LSInput>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<TrueSync.TSQuaternion>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<TrueSync.TSVector>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<Unity.Mathematics.float3>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<Unity.Mathematics.quaternion>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<byte>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<int>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<long>()
		// int System.Runtime.CompilerServices.Unsafe.SizeOf<uint>()
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<ET.ActorId>(byte&,ET.ActorId)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<ET.LSInput>(byte&,ET.LSInput)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<TrueSync.TSQuaternion>(byte&,TrueSync.TSQuaternion)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<TrueSync.TSVector>(byte&,TrueSync.TSVector)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Unity.Mathematics.float3>(byte&,Unity.Mathematics.float3)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<Unity.Mathematics.quaternion>(byte&,Unity.Mathematics.quaternion)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<byte>(byte&,byte)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<int>(byte&,int)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<long>(byte&,long)
		// System.Void System.Runtime.CompilerServices.Unsafe.WriteUnaligned<uint>(byte&,uint)
		// byte& System.Runtime.InteropServices.MemoryMarshal.GetReference<byte>(System.Span<byte>)
		// System.Threading.Tasks.Task<object> System.Threading.Tasks.TaskFactory.StartNew<object>(System.Func<object>,System.Threading.CancellationToken)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// Cysharp.Threading.Tasks.UniTask<object> UnityGameFramework.Extension.Awaitable.LoadAssetAsync<object>(UnityGameFramework.Runtime.ResourceComponent,string,int,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// Cysharp.Threading.Tasks.UniTask<object> UnityGameFramework.Extension.Awaitable.NewUniTask<object>(UnityGameFramework.Extension.Awaitable.MoveNextFunc<object>,System.Threading.CancellationToken,System.Action)
		// string string.Join<int>(string,System.Collections.Generic.IEnumerable<int>)
		// string string.JoinCore<int>(System.Char*,int,System.Collections.Generic.IEnumerable<int>)
	}
}