using LiteDB;

namespace ET
{
    public class HotReloadService
    {
        private readonly ProcessManagerService _processManager;
        private readonly AgentActorService _agentActorService;
        private readonly ILogger<HotReloadService> _logger;
        private readonly ILiteCollection<HotReloadRecord> _records;

        public HotReloadService(ProcessManagerService processManager, AdminDatabase adminDb,
            AgentActorService agentActorService, ILogger<HotReloadService> logger)
        {
            _processManager = processManager;
            _agentActorService = agentActorService;
            _logger = logger;
            _records = adminDb.Database.GetCollection<HotReloadRecord>("hotreload_records");
        }

        public async Task<HotReloadResult> ExecuteHotReloadAsync(string target, string type, string description, string operatorName)
        {
            var record = new HotReloadRecord
            {
                Target = target,
                Type = type,
                Description = description,
                Operator = operatorName,
                StartTime = DateTime.UtcNow,
            };

            var logs = new List<string>();
            try
            {
                logs.Add("开始执行热更新...");
                logs.Add($"目标: {target}, 类型: {type}");

                var servers = _processManager.GetAllServers()
                    .Where(s => s.Status == ServerStatus.Running)
                    .ToList();

                // Filter by machine if target is "Machine-{MachineId}"
                if (target.StartsWith("Machine-") && int.TryParse(target.AsSpan("Machine-".Length), out var machineId))
                {
                    servers = servers.Where(s =>
                    {
                        var pc = Tables.Instance.DTStartProcessConfig.Get(Options.Instance.StartConfig, s.ProcessId);
                        return pc != null && pc.MachineId == machineId;
                    }).ToList();
                }
                else
                {
                    var targetSceneType = target switch
                    {
                        "Gate 服务器" => SceneType.Gate,
                        "Map 服务器" => SceneType.Map,
                        "Login 服务器" => SceneType.Realm,
                        _ => SceneType.None,
                    };

                    if (targetSceneType != SceneType.None)
                    {
                        servers = servers.Where(s => Tables.Instance.DTStartSceneConfig
                                .GetByProcess(s.ProcessId)
                                .Any(scene => scene.Type == targetSceneType)).ToList();
                    }
                }

                if (!servers.Any())
                {
                    logs.Add("警告: 没有运行中的服务器");
                    record.Success = false;
                    record.Duration = "0s";
                    record.EndTime = DateTime.UtcNow;
                    _records.Insert(record);
                    return new HotReloadResult { Success = false, Logs = logs };
                }

                var reloadType = type switch
                {
                    "代码热更" => "code",
                    "配置热更" => "config",
                    "全量热更" => "all",
                    _ => "all",
                };

                // Group processes by Agent (same MachineId)
                var mappedServers = servers
                    .Select(s => new
                    {
                        Server = s,
                        Agent = Tables.Instance.DTStartSceneConfig.GetAgentForProcess(s.ProcessId)
                    })
                    .ToList();
                var serversWithoutAgent = mappedServers
                    .Where(x => x.Agent == null)
                    .Select(x => x.Server.ProcessId)
                    .ToList();
                var agentGroups = mappedServers
                    .Where(x => x.Agent != null)
                    .GroupBy(x => x.Agent!.Process)
                    .ToList();

                logs.Add($"找到 {servers.Count} 个服务器，分布在 {agentGroups.Count} 个 Agent");

                // Parallel hot reload across Agents
                var tasks = agentGroups.Select(async group =>
                {
                    int agentProcessId = group.Key;
                    List<int> processIds = group.Select(x => x.Server.ProcessId).ToList();
                    bool success = await _agentActorService.HotReloadAsync(agentProcessId, reloadType, processIds);
                    return (agentProcessId, processIds, success);
                });

                var results = await Task.WhenAll(tasks);

                var allSuccess = serversWithoutAgent.Count == 0;
                if (serversWithoutAgent.Count > 0)
                {
                    logs.Add($"未找到 Agent 的进程: {string.Join(",", serversWithoutAgent)}");
                }
                foreach (var (agentProcessId, processIds, success) in results)
                {
                    var pids = string.Join(",", processIds);
                    if (success)
                        logs.Add($"Agent-{agentProcessId} (进程 {pids}): 热更成功");
                    else
                    {
                        logs.Add($"Agent-{agentProcessId} (进程 {pids}): 热更失败");
                        allSuccess = false;
                    }
                }

                logs.Add(allSuccess ? "所有服务器热更新完成" : "部分服务器热更新失败");

                record.Success = allSuccess;
                record.EndTime = DateTime.UtcNow;
                record.Duration = $"{(record.EndTime - record.StartTime).TotalSeconds:F1}s";
                _records.Insert(record);

                return new HotReloadResult { Success = allSuccess, Logs = logs };
            }
            catch (Exception ex)
            {
                logs.Add($"热更新失败: {ex.Message}");
                record.Success = false;
                record.EndTime = DateTime.UtcNow;
                record.Duration = $"{(record.EndTime - record.StartTime).TotalSeconds:F1}s";
                _records.Insert(record);
                return new HotReloadResult { Success = false, Logs = logs };
            }
        }

        public List<HotReloadRecord> GetHistory(int limit = 20)
        {
            return _records.FindAll()
                .OrderByDescending(r => r.StartTime)
                .Take(limit)
                .ToList();
        }

        public class HotReloadRecord
        {
            public int Id { get; set; }
            public string Target { get; set; } = "";
            public string Type { get; set; } = "";
            public string Description { get; set; } = "";
            public string Operator { get; set; } = "";
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool Success { get; set; }
            public string Duration { get; set; } = "";
        }

        public class HotReloadResult
        {
            public bool Success { get; set; }
            public List<string> Logs { get; set; } = new();
        }
    }
}
