using System.Collections.Concurrent;

namespace ET
{
    public class LogService : IDisposable
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new();
        private const int MaxLogEntries = 5000;

        public event Action OnNewLog;

        public void AddLog(string level, string server, string message)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Server = server,
                Message = message,
            };

            _logs.Enqueue(entry);

            // Trim oldest entries
            while (_logs.Count > MaxLogEntries && _logs.TryDequeue(out _)) { }

            OnNewLog?.Invoke();
        }

        public void AddProcessOutput(int processId, string line, bool isError = false)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            var level = isError ? "ERROR" : DetectLevel(line);
            AddLog(level, $"Process-{processId}", line);
        }

        public List<LogEntry> GetLogs(string levelFilter = null, string serverFilter = null,
            string search = null, int limit = 500)
        {
            var query = _logs.AsEnumerable();

            if (!string.IsNullOrEmpty(levelFilter))
                query = query.Where(l => l.Level == levelFilter);

            if (!string.IsNullOrEmpty(serverFilter))
                query = query.Where(l => l.Server == serverFilter);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(l => l.Message.Contains(search, StringComparison.OrdinalIgnoreCase));

            return query.TakeLast(limit).ToList();
        }

        public Dictionary<string, int> GetLevelCounts()
        {
            return _logs.GroupBy(l => l.Level)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public List<string> GetServerNames()
        {
            return _logs.Select(l => l.Server).Distinct().OrderBy(s => s).ToList();
        }

        public int TotalCount => _logs.Count;

        public void Clear()
        {
            _logs.Clear();
        }

        private static string DetectLevel(string line)
        {
            if (line.Contains("[ERR]") || line.Contains("[Error]") || line.Contains("Exception"))
                return "ERROR";
            if (line.Contains("[WRN]") || line.Contains("[Warning]"))
                return "WARN";
            if (line.Contains("[DBG]") || line.Contains("[Debug]"))
                return "DEBUG";
            return "INFO";
        }

        public void Dispose()
        {
        }

        public class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Level { get; set; } = "";
            public string Server { get; set; } = "";
            public string Message { get; set; } = "";
        }
    }
}
