using LiteDB;

namespace ET
{
    public class AdminDatabase : IDisposable
    {
        public LiteDatabase Database { get; }

        public AdminDatabase()
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "admin.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            Database = new LiteDatabase($"Filename={dbPath};Connection=shared");
        }

        public AdminDatabase(string dbPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            Database = new LiteDatabase($"Filename={dbPath};Connection=shared");
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
