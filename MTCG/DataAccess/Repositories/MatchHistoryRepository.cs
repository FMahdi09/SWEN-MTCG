using System.Data;
using Npgsql;
using SWEN.MTCG.Businesslogic.Battle;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class MatchHistoryRepository(IDbConnection connection) : BaseRepository(connection)
{
    // CREATE
    public void InsertHistory(string description, BattleResult result, User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "INSERT INTO matchhistory (description, result, userid) " +
                              "VALUES (@description, @result, @userid)";

        // add parameters
        command.AddParameterWithValue("description", DbType.String, description);
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);
        command.AddParameterWithValue("result", DbType.String, result.ToString());

        // execute command
        ExecuteNonQuery(command);
    }

    // READ
    public List<MatchHistory> GetHistory(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT description, result " +
                              "FROM matchhistory " +
                              "WHERE userid = @userid";

        // add parameters
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);

        // execute query
        using IDataReader reader = ExecuteQuery(command);

        List<MatchHistory> history = [];

        while(reader.Read())
        {
            if(!Enum.TryParse((string)reader["result"], out BattleResult result))
                continue;

            MatchHistory entry = new(
                (string)reader["description"],
                result
            );

            history.Add(entry);
        }

        return history;
    }
}