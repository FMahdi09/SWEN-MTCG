using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

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

    public List<UserStats> GetScoreboard(int limit)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT username, score, wins, losses " +
                              "FROM users " +
                              "ORDER BY  score DESC " +
                              "LIMIT @limit";

        // add parameters
        command.AddParameterWithValue("limit", DbType.Int32, limit);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        List<UserStats> scoreboard = [];

        while(reader.Read())
        {
            UserStats stats = new(
                name: (string)reader["username"],
                score: (int)reader["score"],
                wins: (int)reader["wins"],
                losses: (int)reader["losses"] 
            );
            scoreboard.Add(stats);
        }

        return scoreboard;
    }

    // UPDATE
    public void AddWin(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE users " + 
                              "SET score = score + 5, wins = wins + 1 " + 
                              "WHERE id = @userid";

        // add parameter
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);

        // execute command
        ExecuteNonQuery(command);
    }
    
    public void AddLoss(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE users " + 
                              "SET score = score - 3, losses = losses + 1 " + 
                              "WHERE id = @userid";

        // add parameter
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);

        // execute command
        ExecuteNonQuery(command);
    } 
}