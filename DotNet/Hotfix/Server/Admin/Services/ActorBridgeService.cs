using System.Collections.Concurrent;
using Cysharp.Threading.Tasks;

namespace ET
{
    /// <summary>
    /// Base class for bridging ASP.NET Core services with ET Actor messages.
    /// Uses ActorRpcBridge to send RPC requests without directly depending on ET fiber types.
    /// </summary>
    public abstract class ActorBridgeService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<int, bool> _processAliveCache = new();
        private readonly ConcurrentDictionary<int, long> _lastHeartbeatTime = new();

        // 心跳超时时间（毫秒），超过此时间没有收到心跳则认为离线
        protected virtual int HeartbeatTimeoutMs => 15000;

        protected ActorBridgeService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsProcessAlive(int processId)
        {
            // 先检查缓存状态
            if (!_processAliveCache.TryGetValue(processId, out var alive) || !alive)
            {
                return false;
            }

            // 再检查心跳超时
            if (_lastHeartbeatTime.TryGetValue(processId, out var lastTime))
            {
                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (now - lastTime > HeartbeatTimeoutMs)
                {
                    _processAliveCache[processId] = false;
                    return false;
                }
            }

            return true;
        }

        protected void SetProcessAlive(int processId, bool alive)
        {
            _processAliveCache[processId] = alive;
            if (alive)
            {
                _lastHeartbeatTime[processId] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }

        protected async Task<TResponse> CallAsync<TRequest, TResponse>(int processId, int fiberId, TRequest request, int timeoutSeconds = 10)
            where TRequest : MessageObject, IRequest
            where TResponse : MessageObject, IResponse
        {
            try
            {
                if (ActorRpcBridge.Instance == null)
                {
                    _logger.LogDebug("ActorRpcBridge not initialized, cannot send {Request}", typeof(TRequest).Name);
                    return null;
                }

                var task = ActorRpcBridge.Instance.CallAsync(processId, fiberId, request);
                var resp = await task.AsTask().WaitAsync(TimeSpan.FromSeconds(timeoutSeconds));

                if (resp is not TResponse response)
                {
                    _logger.LogWarning("Unexpected response type from process {ProcessId}: expected {Expected}, got {Actual}",
                        processId, typeof(TResponse).Name, resp?.GetType().Name);
                    _processAliveCache[processId] = false;
                    return null;
                }

                if (response.Error != 0)
                {
                    _logger.LogWarning("{Request} error from process {ProcessId}: {Error}",
                        typeof(TRequest).Name, processId, response.Message);
                    _processAliveCache[processId] = true;
                    return response;
                }

                _processAliveCache[processId] = true;
                return response;
            }
            catch (TimeoutException)
            {
                _logger.LogWarning("Timeout: {Request} to process {ProcessId}", typeof(TRequest).Name, processId);
                _processAliveCache[processId] = false;
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed: {Request} to process {ProcessId}", typeof(TRequest).Name, processId);
                _processAliveCache[processId] = false;
                return null;
            }
        }
    }
}
