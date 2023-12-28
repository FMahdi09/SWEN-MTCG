using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.DAtaModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class ScoreRepository(IDbConnection connection) : BaseRepository(connection)
{
    // READ
    public UserStats? GetUserStats(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT username, score, wins, losses " +
                              "FROM users " +
                              "WHERE id = @id";

        // add parameters
        command.AddParameterWithValue("id", DbType.Int32, user.Id);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        if(reader.Read())
        {
            return new UserStats(
                name: (string)reader["username"],
                score: (int)reader["score"],
                wins: (int)reader["wins"],
                losses: (int)reader["losses"] 
            );
        }
        return null;
    }
}