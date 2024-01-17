using System.Net;

namespace ET
{
    public partial class DRStartProcessConfig
    {
        public string InnerIP => this.StartMachineConfig.InnerIP;

        public string OuterIP => this.StartMachineConfig.OuterIP;
        
        // 内网地址外网端口，通过防火墙映射端口过来
        private IPEndPoint ipEndPoint;

        public IPEndPoint IPEndPoint
        {
            get
            {
                if (ipEndPoint == null)
                {
                    this.ipEndPoint = NetworkHelper.ToIPEndPoint(this.InnerIP, this.Port);
                }

                return this.ipEndPoint;
            }
        }

        public DRStartMachineConfig StartMachineConfig => Tables.Instance.DTStartMachineConfig.Get(Options.Instance.StartConfig, this.MachineId);
    }
}
