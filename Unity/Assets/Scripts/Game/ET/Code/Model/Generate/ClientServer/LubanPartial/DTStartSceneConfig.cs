using System.Collections.Generic;
using System.Linq;

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

        public DRStartSceneConfig AdminConfig;

        public List<DRStartSceneConfig> Agents = new();

        public Dictionary<int, DRStartSceneConfig> AgentByMachineId = new();
        
        public List<DRStartSceneConfig> GetByProcess(int process)
        {
            return this.ProcessScenes[process];
        }
        
        public DRStartSceneConfig GetBySceneName(int zone, string name)
        {
            return this.ClientScenesByName[zone][name];
        }

        public DRStartSceneConfig GetAgentForProcess(int processId)
        {
            var processConfig = Tables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, processId);
            if (processConfig == null) return null;
            this.AgentByMachineId.TryGetValue(processConfig.MachineId, out var agent);
            return agent;
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
            this.AdminConfig = null;
            this.Agents.Clear();
            this.AgentByMachineId.Clear();
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
                    case SceneType.Admin:
                        this.AdminConfig = startSceneConfig;
                        break;
                    case SceneType.Agent:
                        this.Agents.Add(startSceneConfig);
                        this.AgentByMachineId[startSceneConfig.StartProcessConfig.MachineId] = startSceneConfig;
                        break;
                }
            }
        }
    }
}