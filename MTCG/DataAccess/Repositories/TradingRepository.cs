using System.Data;
using Npgsql;
using SWEN.MTCG.DataAccess.Base;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.Enums;

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
        command.AddParameterWithValue("cardtype", DbType.String, deal.CardType.ToString());

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }

    // READ
    public List<TradingDeal> GetTradingDeals(User? user = null)
    {
        // create command
        using NpgsqlCommand command = new();

        if(user is not null)
        {
            command.CommandText = "SELECT td.guid as dealguid, td.userid as dealuser, td.mindamage, tdeal.name as dealcardtype, cc.guid as cardguid, c.name as cardname, c.damage as carddamage, c.id as cardid, e.name as elementname, tcard.name as cardcardtype " +
                                  "FROM tradingdeals td " +
                                  "JOIN createdcards cc ON td.cardid = cc.id " +
                                  "JOIN cards c ON cc.cardid = c.id " +
                                  "JOIN elements e ON c.elementid = e.id " +
                                  "JOIN cardtypes tcard ON c.cardtypeid = tcard.id " +
                                  "JOIN cardtypes tdeal ON td.cardtypeid = tdeal.id " +
                                  "WHERE td.userid = @userid";

            command.AddParameterWithValue("userid", DbType.Int32, user.Id);
        }
        else
        {
            command.CommandText = "SELECT td.guid as dealguid, td.userid as dealuser, td.mindamage, tdeal.name as dealcardtype, cc.guid as cardguid, c.name as cardname, c.damage as carddamage, c.id as cardid, e.name as elementname, tcard.name as cardcardtype " +
                                  "FROM tradingdeals td " +
                                  "JOIN createdcards cc ON td.cardid = cc.id " +
                                  "JOIN cards c ON cc.cardid = c.id " +
                                  "JOIN elements e ON c.elementid = e.id " +
                                  "JOIN cardtypes tcard ON c.cardtypeid = tcard.id " +
                                  "JOIN cardtypes tdeal ON td.cardtypeid = tdeal.id ";
        }

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        List<TradingDeal> deals = [];

        while(reader.Read())
        {
            // create card

            // if type or element can not be parsed skip card
            if(!Enum.TryParse((string)reader["cardcardtype"], out Cardtype type) ||
               !Enum.TryParse((string)reader["dealcardtype"], out Cardtype dealType) ||
               !Enum.TryParse((string)reader["elementname"], out CardElement element))
                continue;

            Card card = new(
                id: (int)reader["cardid"],
                guid: (string)reader["cardguid"],
                name: (string)reader["cardname"],
                damage: (int)reader["carddamage"],
                element: element,
                type: type
            );

            TradingDeal deal = new(
                tradeId: (string)reader["dealguid"],
                userId: (int)reader["dealuser"],
                card: card,
                minDamage: (int)reader["mindamage"],
                cardType: dealType
            );

            deals.Add(deal);
        }

        return deals;
    }

    public TradingDeal? GetTradingDeal(string guid)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "SELECT td.guid as dealguid, td.userid as dealuser, td.mindamage, tdeal.name as dealcardtype, cc.guid as cardguid, c.name as cardname, c.damage as carddamage, c.id as cardid, e.name as elementname, tcard.name as cardcardtype " +
                                "FROM tradingdeals td " +
                                "JOIN createdcards cc ON td.cardid = cc.id " +
                                "JOIN cards c ON cc.cardid = c.id " +
                                "JOIN elements e ON c.elementid = e.id " +
                                "JOIN cardtypes tcard ON c.cardtypeid = tcard.id " +
                                "JOIN cardtypes tdeal ON td.cardtypeid = tdeal.id " +
                                "WHERE td.guid = @guid";

        // add parameters
        command.AddParameterWithValue("guid", DbType.String, guid);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        if(reader.Read())
        {
            // create card

            // if type or element can not be parsed skip card
            if(!Enum.TryParse((string)reader["cardcardtype"], out Cardtype type) ||
               !Enum.TryParse((string)reader["dealcardtype"], out Cardtype dealType) ||
               !Enum.TryParse((string)reader["elementname"], out CardElement element))
                return null;

            Card card = new(
                id: (int)reader["cardid"],
                guid: (string)reader["cardguid"],
                name: (string)reader["cardname"],
                damage: (int)reader["carddamage"],
                element: element,
                type: type
            );

            return new TradingDeal(
                tradeId: (string)reader["dealguid"],
                userId: (int)reader["dealuser"],
                card: card,
                minDamage: (int)reader["mindamage"],
                cardType: dealType
            );
        }

        return null;
    }

    // DELETE
    public bool DeleteTradingDeal(TradingDeal deal)
    {
        // create command
        using NpgsqlCommand command = new();
        command.CommandText = "DELETE FROM tradingdeals " +
                              "WHERE guid = @guid " +
                              "RETURNING 1";

        // add parameters
        command.AddParameterWithValue("guid", DbType.String, deal.TradeId);

        // execute command
        using IDataReader reader = ExecuteQuery(command);

        return reader.Read();
    }
}