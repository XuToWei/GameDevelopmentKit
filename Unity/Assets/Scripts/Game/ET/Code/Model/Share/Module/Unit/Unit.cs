using System.Diagnostics;
using MongoDB.Bson.Serialization.Attributes;
using TrueSync;

namespace ET
{
    [ChildOf(typeof(UnitComponent))]
    [DebuggerDisplay("ViewName,nq")]
    public partial class Unit: Entity, IAwake<int>
    {
        public int ConfigId { get; set; } //配置表id

        [BsonElement]
        private TSVector position; //坐标

        [BsonIgnore]
        public TSVector Position
        {
            get
            {
                return this.position;
            }
            set
            {
                TSVector oldPos = this.position;
                this.position = value;
                EventSystem.Instance.Publish(this.Scene(), new ChangePosition() { Unit = this, OldPos = oldPos });
            }
        }

        [BsonIgnore]
        public TSVector Forward
        {
            get
            {
                return this.Rotation * TSVector.forward;
            }
            set
            {
                this.Rotation = TSQuaternion.LookRotation(value, TSVector.up);
            }
        }
        
        [BsonElement]
        private TSQuaternion rotation = TSQuaternion.identity;
        
        [BsonIgnore]
        public TSQuaternion Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
                EventSystem.Instance.Publish(this.Scene(), new ChangeRotation() { Unit = this });
            }
        }

        protected override string ViewName
        {
            get
            {
                return $"{this.GetType().FullName} ({this.Id})";
            }
        }
    }
}
