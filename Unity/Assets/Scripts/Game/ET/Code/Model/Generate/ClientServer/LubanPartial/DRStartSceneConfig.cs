using System.Net;

namespace ET
{
    public partial class DRStartSceneConfig
    {
        public ActorId ActorId;
        
        public SceneType Type;

        public DRStartProcessConfig StartProcessConfig
        {
            get
            {
                return Tables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, this.Process);
            }
        }
        
        public DRStartZoneConfig StartZoneConfig
        {
            get
            {
                return Tables.Instance.DTStartZoneConfig.Get(Options.Instance.StartConfig, this.Zone);
            }
        }

        // 内网地址外网端口，通过防火墙映射端口过来
        private IPEndPoint innerIPPort;

        public IPEndPoint InnerIPPort
        {
            get
            {
                if (innerIPPort == null)
                {
                    this.innerIPPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.InnerIP}:{this.Port}");
                }

                return this.innerIPPort;
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
                    this.outerIPPort = NetworkHelper.ToIPEndPoint($"{this.StartProcessConfig.OuterIP}:{this.Port}");
                }

                return this.outerIPPort;
            }
        }

        partial void PostInit()
        {
            this.ActorId = new ActorId(this.Process, this.Id, 1);
            this.Type = EnumHelper.FromString<SceneType>(this.SceneType);
        }
    }
}
