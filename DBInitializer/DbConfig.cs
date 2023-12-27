namespace SWEN.DbInitializer;

public class DbConfig(string connectionString, bool dropDb, string createScript, string dropScript)
{
    public string ConnectionString { get; set; } = connectionString;
    public bool DropDb { get; set; } = dropDb;
    public string CreateScript { get; set; } = createScript;
    public string DropScript { get; set; } = dropScript;
}