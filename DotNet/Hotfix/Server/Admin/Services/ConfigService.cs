using LiteDB;

namespace ET
{
    public class ConfigService
    {
        private readonly ILiteCollection<ConfigEntry> _configs;
        private readonly ILiteCollection<Announcement> _announcements;

        public ConfigService(AdminDatabase adminDb)
        {
            _configs = adminDb.Database.GetCollection<ConfigEntry>("configs");
            _announcements = adminDb.Database.GetCollection<Announcement>("announcements");
            _configs.EnsureIndex(x => x.Key, true);
        }

        // Config entries
        public List<ConfigEntry> GetAllConfigs()
        {
            return _configs.FindAll().OrderBy(c => c.Key).ToList();
        }

        public ConfigEntry GetConfig(string key)
        {
            return _configs.FindOne(c => c.Key == key);
        }

        public void SetConfig(string key, string value, string type, string description, string modifiedBy)
        {
            var existing = _configs.FindOne(c => c.Key == key);
            if (existing != null)
            {
                existing.Value = value;
                existing.ModifiedAt = DateTime.UtcNow;
                existing.ModifiedBy = modifiedBy;
                _configs.Update(existing);
            }
            else
            {
                _configs.Insert(new ConfigEntry
                {
                    Key = key,
                    Value = value,
                    Type = type,
                    Description = description,
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedBy = modifiedBy,
                });
            }
        }

        // Announcements
        public List<Announcement> GetAllAnnouncements()
        {
            return _announcements.FindAll().OrderByDescending(a => a.PublishedAt).ToList();
        }

        public int CreateAnnouncement(Announcement announcement)
        {
            announcement.PublishedAt = DateTime.UtcNow;
            return _announcements.Insert(announcement);
        }

        public bool UpdateAnnouncement(Announcement announcement)
        {
            return _announcements.Update(announcement);
        }

        public bool DeleteAnnouncement(int id)
        {
            return _announcements.Delete(id);
        }

        // Maintenance
        public bool MaintenanceEnabled
        {
            get => GetConfig("MaintenanceEnabled")?.Value == "true";
            set => SetConfig("MaintenanceEnabled", value.ToString().ToLower(), "bool", "维护模式开关", "system");
        }

        public string MaintenanceMessage
        {
            get => GetConfig("MaintenanceMessage")?.Value ?? "服务器维护中，请稍后再试...";
            set => SetConfig("MaintenanceMessage", value, "string", "维护公告内容", "system");
        }

        public class ConfigEntry
        {
            public int Id { get; set; }
            public string Key { get; set; } = "";
            public string Value { get; set; } = "";
            public string Type { get; set; } = "string";
            public string Description { get; set; } = "";
            public DateTime ModifiedAt { get; set; }
            public string ModifiedBy { get; set; } = "";
        }

        public class Announcement
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Content { get; set; } = "";
            public string Type { get; set; } = "普通";
            public bool IsActive { get; set; }
            public DateTime PublishedAt { get; set; }
            public string PublishedBy { get; set; } = "";
        }
    }
}
