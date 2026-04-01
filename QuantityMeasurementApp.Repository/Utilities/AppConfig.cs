using System;
using System.IO;
using System.Text.Json;

namespace QuantityMeasurementApp.Repository.Utilities
{
    public class AppConfig
    {
        private static AppConfig? _instance;

        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppConfig();
                return _instance;
            }
        }

        public string Provider          { get; }
        public string ConnectionString  { get; }
        public int    PoolSize          { get; }
        public int    ConnectionTimeout { get; }
        public string RepositoryType    { get; }

        private AppConfig()
        {
            string basePath   = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = Path.Combine(basePath, "appsettings.json");

            if (!File.Exists(configPath))
                throw new FileNotFoundException(
                    "appsettings.json not found at: " + configPath);

            string      json = File.ReadAllText(configPath);
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            JsonElement db   = root.GetProperty("Database");
            Provider         = db.GetProperty("Provider").GetString()         ?? "sqlserver";
            ConnectionString = db.GetProperty("ConnectionString").GetString() ?? DefaultConnectionString();
            PoolSize         = db.GetProperty("PoolSize").GetInt32();
            ConnectionTimeout= db.GetProperty("ConnectionTimeout").GetInt32();

            JsonElement repo = root.GetProperty("Repository");
            RepositoryType   = repo.GetProperty("Type").GetString() ?? "database";

            Console.WriteLine("[AppConfig] Provider: "   + Provider);
            Console.WriteLine("[AppConfig] Repository: " + RepositoryType);
            Console.WriteLine("[AppConfig] Pool Size: "  + PoolSize);
        }

        private static string DefaultConnectionString()
        {
            return "Server=(localdb)\\MSSQLLocalDB;" +
                   "Database=QuantityMeasurementDB;"  +
                   "Trusted_Connection=True;"          +
                   "TrustServerCertificate=True;";
        }
    }
}