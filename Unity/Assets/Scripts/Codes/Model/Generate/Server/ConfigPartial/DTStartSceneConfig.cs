using System.Collections.Generic;
using System.Net;
using ET;

namespace cfg.ET
{
    public partial class DTStartSceneConfig
    {
        public MultiMap<int, DRStartSceneConfig> Gates = new MultiMap<int, DRStartSceneConfig>();
        
        public MultiMap<int, DRStartSceneConfig> ProcessScenes = new MultiMap<int, DRStartSceneConfig>();
        
        public Dictionary<long, Dictionary<string, DRStartSceneConfig>> ClientScenesByName = new Dictionary<long, Dictionary<string, DRStartSceneConfig>>();

        public DRStartSceneConfig LocationConfig;

        public List<DRStartSceneConfig> Realms = new List<DRStartSceneConfig>();
        
        public List<DRStartSceneConfig> Routers = new List<DRStartSceneConfig>();
        
        public List<DRStartSceneConfig> Robots = new List<DRStartSceneConfig>();

        public DRStartSceneConfig BenchmarkServer;
        
        public List<DRStartSceneConfig> GetByProcess(int process)
        {
            return this.ProcessScenes[process];
        }
        
        public DRStartSceneConfig GetBySceneName(int zone, string name)
        {
            return this.ClientScenesByName[zone][name];
        }

        partial void PostInit()
        {
            foreach (DRStartSceneConfig startSceneConfig in this.DataList)
            {
                if (!string.Equals(startSceneConfig.StartConfig, Options.Instance.StartConfig))
                {
                    continue;
                }
                
                this.ProcessScenes.Add(startSceneConfig.Process, startSceneConfig);
                
                if (!this.ClientScenesByName.ContainsKey(startSceneConfig.Zone))
                {
                    this.ClientScenesByName.Add(startSceneConfig.Zone, new Dictionary<string, DRStartSceneConfig>());
                }
                this.ClientScenesByName[startSceneConfig.Zone].Add(startSceneConfig.Name, startSceneConfig);
                
                switch (startSceneConfig.Type)
                {
                    case SceneType.Realm:
                        this.Realms.Add(startSceneConfig);
                        break;
                    case SceneType.Gate:
                        this.Gates.Add(startSceneConfig.Zone, startSceneConfig);
                        break;
                    case SceneType.Location:
                        this.LocationConfig = startSceneConfig;
                        break;
                    case SceneType.Robot:
                        this.Robots.Add(startSceneConfig);
                        break;
                    case SceneType.Router:
                        this.Routers.Add(startSceneConfig);
                        break;
                    case SceneType.BenchmarkServer:
                        this.BenchmarkServer = startSceneConfig;
                        break;
                }
            }
        }
    }
    
    public partial class DRStartSceneConfig
    {
        public long InstanceId;
        
        public SceneType Type;

        public DRStartProcessConfig StartProcessConfig
        {
            get
            {
                return DataTables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, this.Process);
            }
        }
        
        public DRStartZoneConfig StartZoneConfig
        {
            get
            {
                return DataTables.Instance.DTStartZoneConfig.Get(Options.Instance.StartConfig, this.Zone);
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