using System.Collections.Generic;

namespace ET.Client
{
    public class EntityComponent : Entity, IAwake, IDestroy
    {
        [StaticField]
        public static EntityComponent Instance;

        //所有显示的UIForm实体
        public readonly HashSet<UGFEntity> AllShowEntities = new HashSet<UGFEntity>();
    }
}