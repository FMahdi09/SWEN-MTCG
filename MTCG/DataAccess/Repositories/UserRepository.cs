using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class UserRepository(IDbConnection connection) : BaseRepository(connection)
{
    // CREATE
    public bool CreateUser(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "INSERT INTO users (username, password, bio, image) " + 
                              "SELECT @username, @password, @bio, @image " + 
                              "WHERE NOT EXISTS (" +
                              "SELECT  id FROM users WHERE username = @username " +
                              ") " +
                              "RETURNING id";

        // add parameters
        command.AddParameterWithValue("username", DbType.String, user.Username);
        command.AddParameterWithValue("password", DbType.String, user.Password);
        command.AddParameterWithValue("bio", DbType.String, user.Bio);
        command.AddParameterWithValue("image", DbType.String, user.Image);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }

    // READ
    public User? GetUser(string username, string password)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT * FROM users " + 
                              "WHERE username = @username " +
                              "AND password = @password";

        // add parameters
        command.AddParameterWithValue("username", DbType.String, username);
        command.AddParameterWithValue("password", DbType.String, password);

        // execute query
        using IDataReader reader = ExecuteQuery(command);

        if(reader.Read())
        {
            return new User(
                (string)reader["username"],
                (string)reader["password"],
                (int)reader["id"],
                (string)reader["bio"],
                (string)reader["image"],
                (int)reader["currency"]
            );
        }
        return null;
    }

    public User? GetUser(string token)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT * FROM users " + 
                               "WHERE id = (SELECT userid FROM tokens WHERE guid = @token)"; 
        // add parameters
        command.AddParameterWithValue("token", DbType.String, token);

        // execute query
        using IDataReader reader = ExecuteQuery(command);

        if(reader.Read())
        {
            return new User(
                (string)reader["username"],
                (string)reader["password"],
                (int)reader["id"],
                (string)reader["bio"],
                (string)reader["image"],
                (int)reader["currency"]
            );
        }
        return null;
    }

    // UPDATE
    public bool UpdateUser(User user)
    {
        // create Command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE users " +
                              "SET bio = @bio, image = @image, username = @username " +
                              "WHERE id = @id " +
                              "AND " +
                              "NOT EXISTS (" +
                              "SELECT id FROM users WHERE username = @username AND id != @id " +
                              ") " +
                              "RETURNING id";

        // add parameters
        command.AddParameterWithValue("bio", DbType.String, user.Bio);
        command.AddParameterWithValue("image", DbType.String, user.Image);
        command.AddParameterWithValue("username", DbType.String, user.Username);
        command.AddParameterWithValue("id", DbType.Int32, user.Id);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }

    public bool PayCurrency(User user, int amount)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "WITH rows AS ( " +
                              "UPDATE users " +
                              "SET currency = currency - @amount " +
                              "WHERE id = @id AND currency >= @amount " +
                              "RETURNING 1 " +
                              ") " +
                              "SELECT count(*) as count FROM rows";

        // add parameters
        command.AddParameterWithValue("amount", DbType.Int32, amount);
        command.AddParameterWithValue("id", DbType.Int32, user.Id);

        // execute command
        using IDataReader reader = ExecuteQuery(command);
        reader.Read();        

        return (Int64)reader["count"] > 0;
    }   
}