
using LiteDB;

namespace ET
{
    public class VersionService
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<VersionRecord> _versions;
        private readonly ILiteCollection<VersionActivationHistory> _history;

        public VersionService() : this(Path.Combine(AppContext.BaseDirectory, "Data", "versions.db"))
        {
        }

        public VersionService(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
            _database = new LiteDatabase($"Filename={dbPath};Connection=shared");
            _versions = _database.GetCollection<VersionRecord>("versions");
            _history = _database.GetCollection<VersionActivationHistory>("history");

            _versions.EnsureIndex(x => x.Platform);
            _versions.EnsureIndex(x => x.Environment);
            _versions.EnsureIndex(x => x.IsActive);
        }

        public List<VersionRecord> GetAllVersions()
        {
            return _versions.FindAll().OrderByDescending(v => v.CreatedAt).ToList();
        }

        public List<VersionRecord> GetVersionsByPlatform(string platform)
        {
            return _versions.Find(v => v.Platform == platform)
                .OrderByDescending(v => v.CreatedAt)
                .ToList();
        }

        public VersionRecord GetActiveVersion(string platform, string environment = "Production")
        {
            return _versions.FindOne(v => v.Platform == platform && v.Environment == environment && v.IsActive);
        }

        public VersionRecord GetVersionById(int id)
        {
            return _versions.FindById(id);
        }

        public int CreateVersion(VersionRecord version, string adminUser)
        {
            version.CreatedAt = DateTime.UtcNow;
            version.CreatedBy = adminUser;
            version.IsActive = false;
            return _versions.Insert(version);
        }

        public bool UpdateVersion(VersionRecord version)
        {
            return _versions.Update(version);
        }

        public bool DeleteVersion(int id)
        {
            var version = _versions.FindById(id);
            if (version == null) return false;

            if (version.IsActive)
            {
                return false; // Cannot delete active version
            }

            return _versions.Delete(id);
        }

        public bool ActivateVersion(int id, string adminUser)
        {
            var version = _versions.FindById(id);
            if (version == null) return false;

            // Deactivate current active version for same platform/environment
            var currentActive = GetActiveVersion(version.Platform, version.Environment);
            if (currentActive != null && currentActive.Id != id)
            {
                currentActive.IsActive = false;
                _versions.Update(currentActive);

                // Log deactivation
                _history.Insert(new VersionActivationHistory
                {
                    VersionRecordId = currentActive.Id,
                    Platform = currentActive.Platform,
                    Environment = currentActive.Environment,
                    Action = "Deactivated",
                    Timestamp = DateTime.UtcNow,
                    AdminUser = adminUser,
                    PreviousVersionId = 0
                });
            }

            // Activate new version
            version.IsActive = true;
            _versions.Update(version);

            // Log activation
            _history.Insert(new VersionActivationHistory
            {
                VersionRecordId = version.Id,
                Platform = version.Platform,
                Environment = version.Environment,
                Action = "Activated",
                Timestamp = DateTime.UtcNow,
                AdminUser = adminUser,
                PreviousVersionId = currentActive != null ? currentActive.Id : 0
            });

            return true;
        }

        public bool DeactivateVersion(int id, string adminUser)
        {
            var version = _versions.FindById(id);
            if (version == null || !version.IsActive) return false;

            version.IsActive = false;
            _versions.Update(version);

            _history.Insert(new VersionActivationHistory
            {
                VersionRecordId = version.Id,
                Platform = version.Platform,
                Environment = version.Environment,
                Action = "Deactivated",
                Timestamp = DateTime.UtcNow,
                AdminUser = adminUser
            });

            return true;
        }

        public List<VersionActivationHistory> GetActivationHistory(string platform = null, int limit = 50)
        {
            var query = _history.FindAll();

            if (!string.IsNullOrEmpty(platform))
            {
                query = query.Where(h => h.Platform == platform);
            }

            return query.OrderByDescending(h => h.Timestamp).Take(limit).ToList();
        }

        // Client API: Get version info for client
        public object GetClientVersionInfo(string platform, string environment = null)
        {
            if (environment == null)
            {
                environment = "Production";
            }

            var version = GetActiveVersion(platform, environment);

            if (version == null) return null;

            // Return format matching client VersionInfo class
            return new
            {
                version.ForceUpdateGame,
                version.LatestGameVersion,
                version.InternalGameVersion,
                version.InternalResourceVersion,
                version.UpdatePrefixUri,
                version.VersionListLength,
                version.VersionListHashCode,
                version.VersionListCompressedLength,
                version.VersionListCompressedHashCode
            };
        }
    }
}
