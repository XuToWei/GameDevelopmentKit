using System.Collections.Generic;

namespace ET.Client
{
    public class EntityComponent : Entity, IAwake, IDestroy
    {
        [StaticField]
        public static EntityComponent Instance;

        //所有的UIForm实体
        public readonly HashSet<UGFEntity> Entities = new HashSet<UGFEntity>();
    }
}
