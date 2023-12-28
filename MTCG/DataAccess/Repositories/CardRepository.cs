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
}