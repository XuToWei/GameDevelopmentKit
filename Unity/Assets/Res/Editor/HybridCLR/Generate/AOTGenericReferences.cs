public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// GameFramework.dll
	// LubanLib.dll
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
	// Cysharp.Threading.Tasks.UniTask<object>
	// Cysharp.Threading.Tasks.UniTask.Awaiter<object>
	// GameFramework.Fsm.FsmState<object>
	// GameFramework.Fsm.IFsm<object>
	// GameFramework.GameFrameworkAction<object>
	// GameFramework.GameFrameworkLinkedList<object>
	// GameFramework.GameFrameworkLinkedList.Enumerator<object>
	// GameFramework.ObjectPool.IObjectPool<object>
	// GameFramework.Variable<byte>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.Dictionary<Game.Hot.GameMode,object>
	// System.Collections.Generic.Dictionary<Game.Hot.AIUtility.CampPair,Game.Hot.RelationType>
	// System.Collections.Generic.Dictionary<System.Collections.Generic.KeyValuePair<Game.Hot.CampType,Game.Hot.RelationType>,object>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.KeyValuePair<Game.Hot.CampType,Game.Hot.RelationType>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<Game.Hot.CampType>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List.Enumerator<object>
	// System.EventHandler<object>
	// System.Func<object>
	// System.Func<object,object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>
	// System.Runtime.CompilerServices.TaskAwaiter<object>
	// System.Threading.Tasks.Task<object>
	// UnityGameFramework.Extension.UGFList<object>
	// }}

	public void RefMethods()
	{
		// string Bright.Common.StringUtil.CollectionToString<int>(System.Collections.Generic.IEnumerable<int>)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Game.Hot.ProcedurePreload.<LoadFontAsync>d__2>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Game.Hot.ProcedurePreload.<LoadFontAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Game.Hot.HPBarComponent.<PreloadAsync>d__8>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Game.Hot.HPBarComponent.<PreloadAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Game.Hot.Tables.<LoadAllAsync>d__35>(System.Runtime.CompilerServices.TaskAwaiter&,Game.Hot.Tables.<LoadAllAsync>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Game.Hot.HPBarComponent.<PreloadAsync>d__8>(Game.Hot.HPBarComponent.<PreloadAsync>d__8&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Game.Hot.ProcedurePreload.<LoadFontAsync>d__2>(Game.Hot.ProcedurePreload.<LoadFontAsync>d__2&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskMethodBuilder.Start<Game.Hot.Tables.<LoadAllAsync>d__35>(Game.Hot.Tables.<LoadAllAsync>d__35&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter,Game.Hot.ProcedurePreload.<PreloadAsync>d__1>(Cysharp.Threading.Tasks.UniTask.Awaiter&,Game.Hot.ProcedurePreload.<PreloadAsync>d__1&)
		// System.Void Cysharp.Threading.Tasks.CompilerServices.AsyncUniTaskVoidMethodBuilder.Start<Game.Hot.ProcedurePreload.<PreloadAsync>d__1>(Game.Hot.ProcedurePreload.<PreloadAsync>d__1&)
		// System.Void GameFramework.Fsm.FsmState<object>.ChangeState<object>(GameFramework.Fsm.IFsm<object>)
		// object GameFramework.Fsm.IFsm<object>.GetData<object>(string)
		// System.Void GameFramework.Fsm.IFsm<object>.SetData<object>(string,object)
		// System.Void GameFramework.Fsm.IFsm<object>.Start<object>()
		// GameFramework.Fsm.IFsm<object> GameFramework.Fsm.IFsmManager.CreateFsm<object>(object,GameFramework.Fsm.FsmState<object>[])
		// bool GameFramework.Fsm.IFsmManager.DestroyFsm<object>(GameFramework.Fsm.IFsm<object>)
		// object GameFramework.GameFrameworkEntry.GetModule<object>()
		// object GameFramework.ReferencePool.Acquire<object>()
		// string GameFramework.Utility.Text.Format<object>(string,object)
		// string GameFramework.Utility.Text.Format<int>(string,int)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTArmor.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTArmor.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,Game.Hot.Tables.<LoadAsync>d__28>(System.Runtime.CompilerServices.TaskAwaiter&,Game.Hot.Tables.<LoadAsync>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTAsteroid.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTAsteroid.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTOneConfig.<LoadAsync>d__3>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTOneConfig.<LoadAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTThruster.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTThruster.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTWeapon.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTWeapon.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<object>,Game.Hot.DTAircraft.<LoadAsync>d__4>(System.Runtime.CompilerServices.TaskAwaiter<object>&,Game.Hot.DTAircraft.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTAircraft.<LoadAsync>d__4>(Game.Hot.DTAircraft.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTArmor.<LoadAsync>d__4>(Game.Hot.DTArmor.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTAsteroid.<LoadAsync>d__4>(Game.Hot.DTAsteroid.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTOneConfig.<LoadAsync>d__3>(Game.Hot.DTOneConfig.<LoadAsync>d__3&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTThruster.<LoadAsync>d__4>(Game.Hot.DTThruster.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.DTWeapon.<LoadAsync>d__4>(Game.Hot.DTWeapon.<LoadAsync>d__4&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<Game.Hot.Tables.<LoadAsync>d__28>(Game.Hot.Tables.<LoadAsync>d__28&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Game.Hot.Tables.<<LoadAllAsync>g__LoadJson|35_1>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Game.Hot.Tables.<<LoadAllAsync>g__LoadJson|35_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.AwaitUnsafeOnCompleted<Cysharp.Threading.Tasks.UniTask.Awaiter<object>,Game.Hot.Tables.<<LoadAllAsync>g__LoadByteBuf|35_0>d>(Cysharp.Threading.Tasks.UniTask.Awaiter<object>&,Game.Hot.Tables.<<LoadAllAsync>g__LoadByteBuf|35_0>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Game.Hot.Tables.<<LoadAllAsync>g__LoadJson|35_1>d>(Game.Hot.Tables.<<LoadAllAsync>g__LoadJson|35_1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Start<Game.Hot.Tables.<<LoadAllAsync>g__LoadByteBuf|35_0>d>(Game.Hot.Tables.<<LoadAllAsync>g__LoadByteBuf|35_0>d&)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityExtension.GetOrAddComponent<object>(UnityEngine.GameObject)
		// Cysharp.Threading.Tasks.UniTask<object> UnityGameFramework.Extension.Awaitable.LoadAssetAsync<object>(UnityGameFramework.Runtime.ResourceComponent,string,int,System.Threading.CancellationToken,System.Action<float>,System.Action<string>)
		// System.Void UnityGameFramework.Runtime.Log.Error<object,object>(string,object,object)
		// System.Void UnityGameFramework.Runtime.Log.Error<object>(string,object)
		// System.Void UnityGameFramework.Runtime.Log.Info<object,object,object,object>(string,object,object,object,object)
		// System.Void UnityGameFramework.Runtime.Log.Info<object>(string,object)
		// System.Void UnityGameFramework.Runtime.Log.Info<object,object>(string,object,object)
		// System.Void UnityGameFramework.Runtime.Log.Warning<object>(string,object)
		// System.Void UnityGameFramework.Runtime.Log.Warning<object,object>(string,object,object)
		// GameFramework.ObjectPool.IObjectPool<object> UnityGameFramework.Runtime.ObjectPoolComponent.CreateSingleSpawnObjectPool<object>(string,int)
	}
}