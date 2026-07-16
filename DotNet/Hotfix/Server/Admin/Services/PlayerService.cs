
using LiteDB;

namespace ET
{
    public class PlayerService
    {
        private readonly ILiteCollection<PlayerInfo> _players;
        private readonly AdminActorService _actorService;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(AdminDatabase adminDb, AdminActorService actorService, ILogger<PlayerService> logger)
        {
            _players = adminDb.Database.GetCollection<PlayerInfo>("players");
            _players.EnsureIndex(x => x.Name);
            _players.EnsureIndex(x => x.IsOnline);
            _players.EnsureIndex(x => x.IsBanned);
            _actorService = actorService;
            _logger = logger;
        }

        public List<PlayerInfo> GetPlayers(string search = null, string statusFilter = null,
            int page = 0, int pageSize = 25, string sortBy = null, bool ascending = true)
        {
            var query = _players.FindAll().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Id.ToString().Contains(search));
            }

            query = statusFilter switch
            {
                "online" => query.Where(p => p.IsOnline && !p.IsBanned),
                "offline" => query.Where(p => !p.IsOnline && !p.IsBanned),
                "banned" => query.Where(p => p.IsBanned),
                _ => query,
            };

            query = sortBy switch
            {
                "id" => ascending ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id),
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "level" => ascending ? query.OrderBy(p => p.Level) : query.OrderByDescending(p => p.Level),
                "lastLogin" => ascending ? query.OrderBy(p => p.LastLoginTime) : query.OrderByDescending(p => p.LastLoginTime),
                _ => query.OrderByDescending(p => p.IsOnline).ThenByDescending(p => p.LastLoginTime),
            };

            return query.Skip(page * pageSize).Take(pageSize).ToList();
        }

        public int GetPlayerCount(string search = null, string statusFilter = null)
        {
            var query = _players.FindAll().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Id.ToString().Contains(search));
            }

            query = statusFilter switch
            {
                "online" => query.Where(p => p.IsOnline && !p.IsBanned),
                "offline" => query.Where(p => !p.IsOnline && !p.IsBanned),
                "banned" => query.Where(p => p.IsBanned),
                _ => query,
            };

            return query.Count();
        }

        public PlayerInfo GetPlayerById(long id)
        {
            return _players.FindById(id);
        }

        public async Task<bool> BanPlayerAsync(long playerId, string reason, DateTime expireTime)
        {
            var player = _players.FindById(playerId);
            if (player == null) return false;

            player.IsBanned = true;
            player.BanReason = reason;
            player.BanExpireTime = expireTime;

            bool disconnected = true;
            if (player.IsOnline)
            {
                disconnected = player.CurrentServerId != 0 &&
                        await _actorService.KickPlayerAsync(player.CurrentServerId, playerId, "banned");
                if (disconnected)
                {
                    player.IsOnline = false;
                    player.CurrentServerId = 0;
                }
                else
                {
                    _logger.LogWarning(
                        "Player {PlayerId} was banned, but the active session could not be disconnected",
                        playerId);
                }
            }

            return _players.Update(player) && disconnected;
        }

        public bool UnbanPlayer(long playerId)
        {
            var player = _players.FindById(playerId);
            if (player == null) return false;

            player.IsBanned = false;
            player.BanReason = string.Empty;
            player.BanExpireTime = default;
            return _players.Update(player);
        }

        public async Task<bool> KickPlayerAsync(long playerId)
        {
            var player = _players.FindById(playerId);
            if (player == null || !player.IsOnline) return false;

            if (player.CurrentServerId == 0 ||
                !await _actorService.KickPlayerAsync(player.CurrentServerId, playerId, "kicked"))
            {
                _logger.LogWarning("Failed to disconnect player {PlayerId}; online state was preserved", playerId);
                return false;
            }

            player.IsOnline = false;
            player.CurrentServerId = 0;
            return _players.Update(player);
        }
    }
}
