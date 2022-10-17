// using System;
// using System.Threading.Tasks;
// using GameFramework.DataTable;
// using GameFramework.Event;
// using UnityGameFramework.Runtime;
//
// namespace UGF
// {
//     public partial class AwaitableExtensions
//     {
//         
//         /// <summary>
//         /// 打开界面（可等待）
//         /// </summary>
//         public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, int uiFormId,
//             object userData = null)
//         {
// #if UNITY_EDITOR
//             TipsSubscribeEvent();
// #endif
//             int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
//             if (serialId == null)
//             {
//                 return Task.FromResult((UIForm)null);
//             }
//
//             var tcs = new TaskCompletionSource<UIForm>();
//             s_UIFormTcs.Add(serialId.Value, tcs);
//             return tcs.Task;
//         }
//         /// <summary>
//         /// 打开界面（可等待）
//         /// </summary>
//         public static Task<UIForm> OpenUIFormAsync(this UIComponent uiComponent, UIFormId uiFormId,
//             object userData = null)
//         {
// #if UNITY_EDITOR
//             TipsSubscribeEvent();
// #endif
//             int? serialId = uiComponent.OpenUIForm(uiFormId, userData);
//             if (serialId == null)
//             {
//                 return Task.FromResult((UIForm)null);
//             }
//
//             var tcs = new TaskCompletionSource<UIForm>();
//             s_UIFormTcs.Add(serialId.Value, tcs);
//             return tcs.Task;
//         }
//         
//         /// <summary>
//         /// 显示实体（可等待）
//         /// </summary>
//         public static Task<Entity> ShowEntityAsync(this EntityComponent entityComponent, Type logicType,
//             int priority,
//             EntityData data)
//         {
// #if UNITY_EDITOR
//             TipsSubscribeEvent();
// #endif
//             var tcs = new TaskCompletionSource<Entity>();
//             s_EntityTcs.Add(data.Id, tcs);
//             entityComponent.ShowEntity(logicType, priority, data);
//             return tcs.Task;
//         }
//         
//          /// <summary>
//         /// 加载数据表（可等待）
//         /// </summary>
//         public static async Task<IDataTable<T>> LoadDataTableAsync<T>(this DataTableComponent dataTableComponent,
//             string dataTableName, bool formBytes, object userData = null) where T : IDataRow
//         {
// #if UNITY_EDITOR
//             TipsSubscribeEvent();
// #endif
//             IDataTable<T> dataTable = dataTableComponent.GetDataTable<T>();
//             if (dataTable != null)
//             {
//                 return await Task.FromResult(dataTable);
//             }
//
//             var loadTcs = new TaskCompletionSource<bool>();
//             var dataTableAssetName = AssetUtility.GetDataTableAsset(dataTableName, formBytes);
//             s_DataTableTcs.Add(dataTableAssetName, loadTcs);
//             dataTableComponent.LoadDataTable(dataTableName, dataTableAssetName, userData);
//             bool isLoaded = await loadTcs.Task;
//             dataTable = isLoaded ? dataTableComponent.GetDataTable<T>() : null;
//             return await Task.FromResult(dataTable);
//         }
//
//
//         private static void OnLoadDataTableSuccess(object sender, GameEventArgs e)
//         {
//             var ne = (LoadDataTableSuccessEventArgs)e;
//             s_DataTableTcs.TryGetValue(ne.DataTableAssetName, out TaskCompletionSource<bool> tcs);
//             if (tcs != null)
//             {
//                 Log.Info("Load data table '{0}' OK.", ne.DataTableAssetName);
//                 tcs.SetResult(true);
//                 s_DataTableTcs.Remove(ne.DataTableAssetName);
//             }
//         }
//
//         private static void OnLoadDataTableFailure(object sender, GameEventArgs e)
//         {
//             var ne = (LoadDataTableFailureEventArgs)e;
//             s_DataTableTcs.TryGetValue(ne.DataTableAssetName, out TaskCompletionSource<bool> tcs);
//             if (tcs != null)
//             {
//                 Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", ne.DataTableAssetName,
//                     ne.DataTableAssetName, ne.ErrorMessage);
//                 tcs.SetResult(false);
//                 s_DataTableTcs.Remove(ne.DataTableAssetName);
//             }
//         }
//     }
// }