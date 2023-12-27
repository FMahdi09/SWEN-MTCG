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
}