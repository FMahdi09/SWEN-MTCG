using Npgsql;

namespace SWEN.DbInitializer;

public class DBInitializer(DbConfig config)
{
    private readonly DbConfig _config = config;

    public void InitDB()
    {
        DropDatabase();

        CreateDatabase();

        FillDatabase();
    }

    private void CreateDatabase()
    {
        try
        {
            NpgsqlConnection connection = new(_config.ConnectionString);
            connection.Open();

            string script = File.ReadAllText(_config.CreateScript); 

            NpgsqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
        catch(NpgsqlException ex)
        {
            Console.Error.WriteLine($"Failed to connect to database: { ex.Message }");
        }
    }

    private void DropDatabase()
    {
        try
        {
            NpgsqlConnection connection = new(_config.ConnectionString);
            connection.Open();

            string script = File.ReadAllText(_config.DropScript); 

            NpgsqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
        catch(NpgsqlException ex)
        {
            Console.Error.WriteLine($"Failed to connect to database: { ex.Message }");
        }
    }

    private void FillDatabase()
    {
        try
        {
            NpgsqlConnection connection = new(_config.ConnectionString);
            connection.Open();

            string script = File.ReadAllText(_config.FillScript); 

            NpgsqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
        catch(NpgsqlException ex)
        {
            Console.Error.WriteLine($"Failed to connect to database: { ex.Message }");
        }
    }
}