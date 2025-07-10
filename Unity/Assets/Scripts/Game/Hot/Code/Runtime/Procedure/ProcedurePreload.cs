using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using UnityEngine;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game.Hot
{
    public class ProcedurePreload : ProcedureBase
    {
        protected override void OnEnter(IFsm<ProcedureComponent> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            PreloadAsync(procedureOwner).Forget();
        }

        private async UniTaskVoid PreloadAsync(IFsm<ProcedureComponent> procedureOwner)
        {
            await HotEntry.Tables.LoadAllAsync();
            Log.Info("Game.Hot.Code Load Config!");
            
            await HotEntry.HPBar.PreloadAsync();
            await LoadFontAsync("Default");
            
            ChangeState<ProcedureGame>(procedureOwner);
        }

        private async UniTask LoadFontAsync(string fontName)
        {
            Font font = await GameEntry.Resource.LoadAssetAsync<Font>(AssetUtility.GetFontAsset(fontName), Constant.AssetPriority.FontAsset);
            if (font == null)
            {
                Log.Error("Can not load font '{0}'.", fontName);
            }
            StarForceUIForm.SetMainFont(font);
        }
    }
}
