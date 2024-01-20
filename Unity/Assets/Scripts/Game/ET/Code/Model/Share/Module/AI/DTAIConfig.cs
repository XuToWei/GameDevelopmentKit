using System.Collections.Generic;

namespace ET
{
    public partial class DTAIConfig
    {
        public Dictionary<int, SortedDictionary<int, DRAIConfig>> AIConfigs = new Dictionary<int, SortedDictionary<int, DRAIConfig>>();

        public SortedDictionary<int, DRAIConfig> GetAI(int aiConfigId)
        {
            return this.AIConfigs[aiConfigId];
        }

        partial void PostInit()
        {
            this.AIConfigs.Clear();
            foreach (var kv in this.DataMap)
            {
                SortedDictionary<int, DRAIConfig> aiNodeConfig;
                if (!this.AIConfigs.TryGetValue(kv.Value.AIConfigId, out aiNodeConfig))
                {
                    aiNodeConfig = new SortedDictionary<int, DRAIConfig>();
                    this.AIConfigs.Add(kv.Value.AIConfigId, aiNodeConfig);
                }

                aiNodeConfig.Add(kv.Key, kv.Value);
            }
        }
    }
}