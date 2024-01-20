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
            foreach (var key in this.KeyList)
            {
                var value = this.Get(key);
                SortedDictionary<int, DRAIConfig> aiNodeConfig;
                if (!this.AIConfigs.TryGetValue(value.AIConfigId, out aiNodeConfig))
                {
                    aiNodeConfig = new SortedDictionary<int, DRAIConfig>();
                    this.AIConfigs.Add(value.AIConfigId, aiNodeConfig);
                }

                aiNodeConfig.Add(key, value);
            }
        }
    }
}