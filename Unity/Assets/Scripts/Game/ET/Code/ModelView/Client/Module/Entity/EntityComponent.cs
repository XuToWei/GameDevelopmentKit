using System.Collections.Generic;

namespace ET.Client
{
    [ComponentOf]
    public class EntityComponent : Entity, IAwake, IDestroy
    {
        //所有显示的Entity实体
        public readonly HashSet<UGFEntity> AllShowEntities = new HashSet<UGFEntity>();
    }
}