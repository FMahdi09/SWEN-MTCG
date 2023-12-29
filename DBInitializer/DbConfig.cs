namespace SWEN.DbInitializer;

public class DbConfig(string connectionString, string fillScript, string createScript, string dropScript)
{
    public string ConnectionString { get; set; } = connectionString;
    public string CreateScript { get; set; } = createScript;
    public string FillScript { get; set; } = fillScript;
    public string DropScript { get; set; } = dropScript;
}