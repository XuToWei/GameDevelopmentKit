using System.Collections.Generic;

namespace ET
{
    public partial class DTStartSceneConfig
    {
        public MultiMap<int, DRStartSceneConfig> Gates = new MultiMap<int, DRStartSceneConfig>();
        
        public MultiMap<int, DRStartSceneConfig> StartProcessScenes = new MultiMap<int, DRStartSceneConfig>();
        
        public Dictionary<long, Dictionary<string, DRStartSceneConfig>> ClientScenesByName = new Dictionary<long, Dictionary<string, DRStartSceneConfig>>();

        public DRStartSceneConfig LocationConfig;

        public List<DRStartSceneConfig> Realms = new List<DRStartSceneConfig>();
        
        public List<DRStartSceneConfig> Routers = new List<DRStartSceneConfig>();
        
        public List<DRStartSceneConfig> Robots = new List<DRStartSceneConfig>();
        
        public List<DRStartSceneConfig> Maps = new List<DRStartSceneConfig>();
        
        public DRStartSceneConfig Match;

        public DRStartSceneConfig BenchmarkServer;
        
        public List<DRStartSceneConfig> GetByProcess(int process)
        {
            return this.StartProcessScenes[process];
        }
        
        public DRStartSceneConfig GetBySceneName(int zone, string name)
        {
            return this.ClientScenesByName[zone][name];
        }

        partial void PostInit()
        {
            this.Realms.Clear();
            this.Gates.Clear();
            this.Robots.Clear();
            this.Routers.Clear();
            this.Maps.Clear();
            this.StartProcessScenes.Clear();
            this.ClientScenesByName.Clear();
            this.LocationConfig = default;
            this.BenchmarkServer = default;
            this.Match = default;
            
            foreach (DRStartSceneConfig startSceneConfig in this.DataList)
            {
                if (!string.Equals(startSceneConfig.StartConfig, Options.Instance.StartConfig))
                {
                    continue;
                }
                
                this.StartProcessScenes.Add(startSceneConfig.Process, startSceneConfig);
                
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
                    case SceneType.Map:
                        this.Maps.Add(startSceneConfig);
                        break;
                    case SceneType.Match:
                        this.Match = startSceneConfig;
                        break;
                }
            }
        }
    }
}