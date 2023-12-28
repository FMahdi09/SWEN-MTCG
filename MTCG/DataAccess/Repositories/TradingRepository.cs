using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.DataAccess.Repositories;

public class TradingRepository(IDbConnection connection) : BaseRepository(connection)
{
    // CREATE
    public bool CreateTradingDeal(User user, TradingDeal deal)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "INSERT INTO tradingdeals (guid, userid, cardid, mindamage, cardtypeid) " +
                              "SELECT " +
                              "@guid, " +
                              "@userid, " + 
                              "(SELECT id from createdcards WHERE guid = @cardguid), " +
                              "@minDamage, " +
                              "(SELECT id FROM cardtypes WHERE name = @cardtype) " +
                              "WHERE EXISTS (" +
                              "SELECT id FROM createdcards WHERE deck = FALSE AND guid = @cardguid" +
                              ") " +
                              "AND EXISTS (" +
                              "SELECT id FROM cardtypes WHERE name = @cardtype " +
                              ") " +
                              "RETURNING id";

        // add parameters
        command.AddParameterWithValue("guid", DbType.String, deal.TradeId);
        command.AddParameterWithValue("userid", DbType.Int32, user.Id);
        command.AddParameterWithValue("cardguid", DbType.String, deal.Card.Guid);
        command.AddParameterWithValue("minDamage", DbType.Int32, deal.MinDamage);
        command.AddParameterWithValue("cardtype", DbType.String, deal.CardType);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }
}