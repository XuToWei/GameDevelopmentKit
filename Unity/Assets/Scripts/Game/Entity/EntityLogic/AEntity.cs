using GameFramework;
using UnityGameFramework.Runtime;

namespace Game
{
    public abstract class AEntity: EntityLogic
    {
        public int Id => Entity.Id;

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
#if UNITY_EDITOR
            Name = Utility.Text.Format("[Entity {0}]", Id);
#endif
        }
    }
}