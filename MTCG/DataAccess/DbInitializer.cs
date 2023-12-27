using Npgsql;

namespace SWEN.MTCG.DataAccess;

public class DbInitializer(string configFile)
{
    private readonly string _configFile = configFile;

    public void CreateDatabase()
    {
        try
        {
            NpgsqlConnection connection = new("Host=localhost;Username=postgres;Password=postgres;Database=mydb");
            connection.Open();

            string script = File.ReadAllText(@"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql"); 

            NpgsqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
        catch(NpgsqlException ex)
        {
            Console.Error.WriteLine($"Failed to connect to database: { ex.Message }");
        }
    }

    public void DropDatabase()
    {
        try
        {
            NpgsqlConnection connection = new("Host=localhost;Username=postgres;Password=postgres;Database=mydb");
            connection.Open();

            string script = File.ReadAllText(@"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"); 

            NpgsqlCommand command = new(script, connection);
            command.ExecuteNonQuery();
        }
        catch(NpgsqlException ex)
        {
            Console.Error.WriteLine($"Failed to connect to database: { ex.Message }");
        }
    }
}