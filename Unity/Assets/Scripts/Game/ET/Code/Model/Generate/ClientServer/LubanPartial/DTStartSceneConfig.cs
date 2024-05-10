using System.Collections.Generic;

namespace ET
{
    public partial class DTStartSceneConfig
    {
        public MultiMap<int, DRStartSceneConfig> Gates = new();
        
        public MultiMap<int, DRStartSceneConfig> ProcessScenes = new();
        
        public Dictionary<long, Dictionary<string, DRStartSceneConfig>> ClientScenesByName = new();

        public DRStartSceneConfig LocationConfig;

        public List<DRStartSceneConfig> Realms = new();
        
        public List<DRStartSceneConfig> Routers = new();
        
        public List<DRStartSceneConfig> Maps = new();

        public DRStartSceneConfig Match;
        
        public DRStartSceneConfig Benchmark;
        
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
            this.Gates.Clear();
            this.ProcessScenes.Clear();
            this.ClientScenesByName.Clear();
            this.LocationConfig = null;
            this.Realms.Clear();
            this.Routers.Clear();
            this.Maps.Clear();
            this.Match = null;
            this.Benchmark = null;
            foreach (var startSceneConfig in this.DataList)
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
                    case SceneType.Router:
                        this.Routers.Add(startSceneConfig);
                        break;
                    case SceneType.Map:
                        this.Maps.Add(startSceneConfig);
                        break;
                    case SceneType.Match:
                        this.Match = startSceneConfig;
                        break;
                    case SceneType.BenchmarkServer:
                        this.Benchmark = startSceneConfig;
                        break;
                }
            }
        }
    }
}