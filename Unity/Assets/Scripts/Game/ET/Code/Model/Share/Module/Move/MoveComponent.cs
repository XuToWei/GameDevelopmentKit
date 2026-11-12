using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TrueSync;

namespace ET
{
    [ComponentOf(typeof(Unit))]
    public class MoveComponent: Entity, IAwake, IDestroy
    {
        public TSVector PreTarget
        {
            get
            {
                return this.Targets[this.N - 1];
            }
        }

        public TSVector NextTarget
        {
            get
            {
                return this.Targets[this.N];
            }
        }

        // 开启移动协程的时间
        public long BeginTime;

        // 每个点的开始时间
        public long StartTime { get; set; }

        // 开启移动协程的Unit的位置
        public TSVector StartPos;

        public TSVector RealPos
        {
            get
            {
                return this.Targets[0];
            }
        }

        private long needTime;

        public long NeedTime
        {
            get
            {
                return this.needTime;
            }
            set
            {
                this.needTime = value;
            }
        }

        public long MoveTimer;

        public FP Speed; // m/s

        public AutoResetUniTaskCompletionSource<bool> tcs;

        public List<TSVector> Targets = new List<TSVector>();

        public TSVector FinalTarget
        {
            get
            {
                return this.Targets[this.Targets.Count - 1];
            }
        }

        public int N;

        public int TurnTime;

        public bool IsTurnHorizontal;

        public TSQuaternion From;

        public TSQuaternion To;
    }
}
