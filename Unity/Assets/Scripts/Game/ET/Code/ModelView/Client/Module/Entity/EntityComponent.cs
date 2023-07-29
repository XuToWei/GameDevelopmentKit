using System.Collections.Generic;
using System.Linq;
using Game;

namespace ET.Client
{
    [ComponentOf]
    [EnableMethod]
    public class EntityComponent : Entity, IAwake, IDestroy
    {
        //所有显示的Entity实体
        public readonly HashSet<UGFEntity> AllShowEntities = new HashSet<UGFEntity>();

        public override void Dispose()
        {
            foreach (UGFEntity entity in this.AllShowEntities.ToArray())
            {
                GameEntry.Entity.HideEntity(entity.entity);
            }
            this.AllShowEntities.Clear();
            base.Dispose();
        }
    }
}