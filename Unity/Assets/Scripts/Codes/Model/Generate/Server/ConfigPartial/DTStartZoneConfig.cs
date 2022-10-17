using System.Net;
using ET;

namespace cfg.ET
{
    public partial class DRStartProcessConfig
    {
        private IPEndPoint innerIPPort;

        public long SceneId;

        public IPEndPoint InnerIPPort
        {
            get
            {
                if (this.innerIPPort == null)
                {
                    this.innerIPPort = NetworkHelper.ToIPEndPoint($"{this.InnerIP}:{this.InnerPort}");
                }

                return this.innerIPPort;
            }
        }

        public string InnerIP => this.StartMachineConfig.InnerIP;

        public string OuterIP => this.StartMachineConfig.OuterIP;

        public DRStartMachineConfig StartMachineConfig => DataTables.Instance.DTStartMachineConfig.Get(Options.Instance.StartConfig, this.MachineId);

        partial void PostInit()
        {
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct((int)this.Id, 0);
            this.SceneId = instanceIdStruct.ToLong();
            Log.Info($"StartProcess info: {this.MachineId} {this.Id} {this.SceneId}");
        }
    }
}