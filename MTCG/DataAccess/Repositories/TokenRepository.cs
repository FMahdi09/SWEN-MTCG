using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;

namespace SWEN.MTCG.DataAccess.Repositories;


public class TokenRepository(IDbConnection connection) : BaseRepository(connection)
{
    // CREATE
    public void AddToken(int userId, string token)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "INSERT INTO tokens (guid, userId) " +
                              "VALUES (@guid, @userId)";

        // add parameters
        command.AddParameterWithValue("guid", DbType.String, token);
        command.AddParameterWithValue("userId", DbType.Int32, userId);

        // execute command
        ExecuteNonQuery(command);
    }
}                             