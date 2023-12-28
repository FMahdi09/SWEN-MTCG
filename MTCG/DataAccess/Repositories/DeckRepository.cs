using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class DeckRepository(IDbConnection connection) : BaseRepository(connection)
{
    // READ
    public List<Card> GetDeck(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT c.id, cc.guid, c.name as cardname, c.damage, e.name as elementname, t.name as typename " +
                              "FROM createdcards cc " +
                              "JOIN cards c ON cc.cardid = c.id " +
                              "JOIN elements e ON c.elementid = e.id " +
                              "JOIN cardtypes t ON c.cardtypeid = t.id " +
                              "WHERE cc.userid = @userid " +
                              "AND cc.deck = TRUE";
                              
        // add parameters
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        List<Card> toReturn = [];

        while(reader.Read())
        {
            Card card = new(
                (int)reader["id"],
                (string)reader["guid"],
                (string)reader["cardname"],
                (int)reader["damage"],
                (string)reader["elementname"],
                (string)reader["typename"]
            );
            toReturn.Add(card);
        }

        return toReturn;
    }

    // UPDATE
    public void ClearDeck(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE createdcards " +
                              "SET deck = FALSE " +
                              "WHERE userid = @userid";

        // add parameters
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);

        // execute command
        ExecuteNonQuery(command);
    }

    public bool SetDeck(User user, List<string> cardGuids)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "WITH rows AS ( " +
                              "UPDATE createdcards " +
                              "SET deck = true " +
                              "WHERE userid = @userid " +
                              "AND guid = @guid " +
                              "RETURNING 1 " +
                              ") " +
                              "SELECT count(*) as count FROM rows";

        foreach(string guid in cardGuids)
        {
            // add parameters
            command.AddParameterWithValue("userid", DbType.Int32, user.Id);
            command.AddParameterWithValue("guid", DbType.String, guid);

            // execute query
            using IDataReader reader = ExecuteQuery(command);
            command.Parameters.Clear();

            if(!reader.Read() || (Int64)reader["count"] != 1)
                return false;
        }

        return true;
    }
}