

namespace ET
{
    public class ServerMonitorNotifier
    {
        private readonly object _lock = new();

        private List<ServerInfo> _servers = new();
        private Dictionary<int, List<FiberInfo>> _fibersByProcess = new();
        private Dictionary<int, List<SceneInfo>> _scenesByProcess = new();

        public event Action OnServerStatusChanged;
        public event Action OnFibersChanged;
        public event Action OnScenesChanged;

        public List<ServerInfo> GetServers()
        {
            lock (_lock)
            {
                return new List<ServerInfo>(_servers);
            }
        }

        public List<FiberInfo> GetFibers(int processId)
        {
            lock (_lock)
            {
                return _fibersByProcess.TryGetValue(processId, out var fibers)
                    ? new List<FiberInfo>(fibers)
                    : new List<FiberInfo>();
            }
        }

        public List<FiberInfo> GetAllFibers()
        {
            lock (_lock)
            {
                return _fibersByProcess.Values.SelectMany(f => f).ToList();
            }
        }

        public List<SceneInfo> GetScenes(int processId)
        {
            lock (_lock)
            {
                return _scenesByProcess.TryGetValue(processId, out var scenes)
                    ? new List<SceneInfo>(scenes)
                    : new List<SceneInfo>();
            }
        }

        public List<SceneInfo> GetAllScenes()
        {
            lock (_lock)
            {
                return _scenesByProcess.Values.SelectMany(s => s).ToList();
            }
        }

        public void UpdateServers(List<ServerInfo> servers)
        {
            bool changed;
            lock (_lock)
            {
                changed = HasServerChanges(servers);
                if (changed)
                {
                    _servers = new List<ServerInfo>(servers);
                }
            }

            if (changed)
            {
                OnServerStatusChanged?.Invoke();
            }
        }

        public void UpdateFibers(int processId, List<FiberInfo> fibers)
        {
            bool changed;
            lock (_lock)
            {
                changed = HasFiberChanges(processId, fibers);
                if (changed)
                {
                    _fibersByProcess[processId] = new List<FiberInfo>(fibers);
                }
            }

            if (changed)
            {
                OnFibersChanged?.Invoke();
            }
        }

        public void UpdateScenes(int processId, List<SceneInfo> scenes)
        {
            bool changed;
            lock (_lock)
            {
                changed = HasSceneChanges(processId, scenes);
                if (changed)
                {
                    _scenesByProcess[processId] = new List<SceneInfo>(scenes);
                }
            }

            if (changed)
            {
                OnScenesChanged?.Invoke();
            }
        }

        public void RemoveProcessData(int processId)
        {
            bool fiberRemoved;
            bool sceneRemoved;
            lock (_lock)
            {
                fiberRemoved = _fibersByProcess.Remove(processId);
                sceneRemoved = _scenesByProcess.Remove(processId);
            }

            if (fiberRemoved) OnFibersChanged?.Invoke();
            if (sceneRemoved) OnScenesChanged?.Invoke();
        }

        private bool HasServerChanges(List<ServerInfo> newServers)
        {
            if (_servers.Count != newServers.Count) return true;

            for (int i = 0; i < _servers.Count; i++)
            {
                var old = _servers[i];
                var cur = newServers[i];
                if (old.ProcessId != cur.ProcessId
                    || old.Status != cur.Status
                    || old.PlayerCount != cur.PlayerCount
                    || Math.Abs(old.CpuUsage - cur.CpuUsage) > 0.1
                    || old.MemoryUsage != cur.MemoryUsage)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasFiberChanges(int processId, List<FiberInfo> newFibers)
        {
            if (!_fibersByProcess.TryGetValue(processId, out var oldFibers)) return true;
            if (oldFibers.Count != newFibers.Count) return true;

            for (int i = 0; i < oldFibers.Count; i++)
            {
                if (oldFibers[i].Id != newFibers[i].Id
                    || oldFibers[i].EntityCount != newFibers[i].EntityCount)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasSceneChanges(int processId, List<SceneInfo> newScenes)
        {
            if (!_scenesByProcess.TryGetValue(processId, out var oldScenes)) return true;
            if (oldScenes.Count != newScenes.Count) return true;

            for (int i = 0; i < oldScenes.Count; i++)
            {
                if (oldScenes[i].Id != newScenes[i].Id
                    || oldScenes[i].PlayerCount != newScenes[i].PlayerCount)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
