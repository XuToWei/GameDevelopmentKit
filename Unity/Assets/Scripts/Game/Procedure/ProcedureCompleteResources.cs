using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Extension;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game
{
    public class ProcedureCompleteResources : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            SaveLauncherResourcePathAsync().Forget();

            ChangeState<ProcedurePreload>(procedureOwner);
        }
        
        private async UniTaskVoid SaveLauncherResourcePathAsync()
        {
            if (GameEntry.Base.EditorResourceMode)
                return;
            string readPath = GameEntry.Resource.ReadWritePath;
            if (GameEntry.Resource.ResourceMode == ResourceMode.Package)
            {
                readPath = GameEntry.Resource.ReadOnlyPath;
            }
            else if (GameEntry.Resource.ResourceMode == ResourceMode.UpdatableWhilePlaying)
            {
                await GameEntry.Resource.UpdateResourcesAsync(GameEntryLoader.LauncherResourceName);
                readPath = GameEntry.Resource.ReadWritePath;
            }
            IResourceGroup resourceGroup = GameEntry.Resource.GetResourceGroup(GameEntryLoader.LauncherResourceGroupName);
            using UGFList<string> resourceNames = UGFList<string>.Create();
            resourceGroup.GetResourceNames(resourceNames);
            if (resourceNames.Count == 0)
            {
                throw new GameFrameworkException("Game Framework Resource Names is invalid.");
            }
            GameEntryLoader.SaveResourcePath(Utility.Path.GetRegularPath(Utility.Text.Format("{0}/{1}", readPath, resourceNames[0])));
        }
    }
}