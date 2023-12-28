using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class CardRepository(IDbConnection connection) : BaseRepository(connection)
{
    // CREATE
    public List<Card> GenerateCards(int amount)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT c.id, c.name as cardname, c.damage, e.name as elementname, t.name as typename " +
                              "FROM cards c " +
                              "JOIN elements e ON c.elementid = e.id " +
                              "JOIN cardtypes t ON c.cardtypeid = t.id " +
                              "ORDER BY random() limit @amount";

        // add parameters
        command.AddParameterWithValue("amount", DbType.Int32, amount);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        List<Card> toReturn = [];

        while (reader.Read())
        {
            // create card
            Card card = new(
                (int)reader["id"],
                guid: Guid.NewGuid().ToString(),
                (string)reader["cardname"],
                (int)reader["damage"],
                (string)reader["elementname"],
                (string)reader["typename"]
            );
            toReturn.Add(card);
        }
        return toReturn;
    }

    public void AssignCards(User user, List<Card> cards)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "INSERT INTO createdcards (guid, userid, cardid) " +
                              "VALUES (@guid, @userid, @cardid)";

        foreach(Card card in cards)
        {
            // add parameters
            command.AddParameterWithValue("guid", DbType.String, card.Guid);
            command.AddParameterWithValue("userid", DbType.Int32, user.Id);
            command.AddParameterWithValue("cardid", DbType.Int32, card.Id);

            // execute command
            ExecuteNonQuery(command);
            command.Parameters.Clear();
        }
    }

    // READ
    public List<Card> GetCards(User user)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT c.id, cc.guid, c.name as cardname, c.damage, e.name as elementname, t.name as typename " +
                              "FROM createdcards cc " +
                              "JOIN cards c ON cc.cardid = c.id " +
                              "JOIN elements e ON c.elementid = e.id " +
                              "JOIN cardtypes t ON c.cardtypeid = t.id " +
                              "WHERE cc.userid = @userid";

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

    public Card? GetCardFromGuid(string guid)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT c.id, cc.guid, c.name as cardname, c.damage, e.name as elementname, t.name as typename " +
                              "FROM createdcards cc " +
                              "JOIN cards c ON cc.cardid = c.id " +
                              "JOIN elements e ON c.elementid = e.id " +
                              "JOIN cardtypes t ON c.cardtypeid = t.id " +
                              "AND cc.guid = @guid";

        // add parameters
        command.AddParameterWithValue("guid", DbType.String, guid);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        if(reader.Read())
        {
            return new Card(
                (int)reader["id"],
                (string)reader["guid"],
                (string)reader["cardname"],
                (int)reader["damage"],
                (string)reader["elementname"],
                (string)reader["typename"]
            );
        }
        
        return null;
    }

    // UPDATE
    public bool RemoveCardOwnership(User user, Card card)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE createdcards " +
                              "SET userid = NULL " +
                              "WHERE userid = @userid " +
                              "AND guid = @guid " +
                              "AND deck = false " +
                              "RETURNING 1";

        // add parameters
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);
        command.AddParameterWithValue("guid", DbType.String, card.Guid);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }

    public void ChangeCardOwnership(User user, Card card)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "UPDATE createdcards " +
                              "SET userid = @userid " +
                              "WHERE guid = @guid";

        // add parameters
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);
        command.AddParameterWithValue("guid", DbType.String, card.Guid);

        // execute command
        ExecuteNonQuery(command);
    }
}