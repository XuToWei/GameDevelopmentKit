using System.Net;

namespace ET
{
    public partial class DRStartSceneConfig
    {
        public long InstanceId;
        
        public SceneType Type;

        public DRStartProcessConfig StartProcessConfig
        {
            get
            {
                return Tables.Instance.DTStartProcessConfig.Get(this.StartConfig, this.Process);
            }
        }
        
        public DRStartZoneConfig StartZoneConfig
        {
            get
            {
                return Tables.Instance.DTStartZoneConfig.Get(this.StartConfig, this.Zone);
            }
        }

        // 内网地址外网端口，通过防火墙映射端口过来
        private IPEndPoint innerIPOutPort;

        public IPEndPoint InnerIPOutPort
        {
            get
            {
                if (innerIPOutPort == null)
                {
                    this.innerIPOutPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.InnerIP}:{this.OuterPort}");
                }

                return this.innerIPOutPort;
            }
        }

        private IPEndPoint outerIPPort;

        // 外网地址外网端口
        public IPEndPoint OuterIPPort
        {
            get
            {
                if (this.outerIPPort == null)
                {
                    this.outerIPPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.OuterIP}:{this.OuterPort}");
                }

                return this.outerIPPort;
            }
        }

        partial void PostInit()
        {
            this.Type = EnumHelper.FromString<SceneType>(this.SceneType);
            InstanceIdStruct instanceIdStruct = new InstanceIdStruct(this.Process, (uint) this.Id);
            this.InstanceId = instanceIdStruct.ToLong();
        }
    }
}
