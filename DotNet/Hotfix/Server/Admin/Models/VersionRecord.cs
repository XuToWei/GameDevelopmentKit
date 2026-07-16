namespace ET
{
    public class VersionRecord
    {
        public int Id { get; set; }
        public string Platform { get; set; } = string.Empty; // Windows64/MacOS/iOS/Android
        public string Environment { get; set; } = "Production"; // Dev/Staging/Production
        public bool IsActive { get; set; }

        // VersionInfo fields (matching client VersionInfo)
        public bool ForceUpdateGame { get; set; }
        public string LatestGameVersion { get; set; } = string.Empty;
        public int InternalGameVersion { get; set; }
        public int InternalResourceVersion { get; set; }
        public string UpdatePrefixUri { get; set; } = string.Empty;
        public int VersionListLength { get; set; }
        public int VersionListHashCode { get; set; }
        public int VersionListCompressedLength { get; set; }
        public int VersionListCompressedHashCode { get; set; }

        // Metadata
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class VersionActivationHistory
    {
        public int Id { get; set; }
        public int VersionRecordId { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // Activated/Deactivated
        public DateTime Timestamp { get; set; }
        public string AdminUser { get; set; } = string.Empty;
        public int PreviousVersionId { get; set; }
    }
}
