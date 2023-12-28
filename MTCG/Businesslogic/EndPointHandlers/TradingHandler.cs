using System.Text.Json;
using Npgsql;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.Attributes;
using SWEN.MTCG.DataAccess.UnitOfWork;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.BusinessLogic.EndpointHandlers;

[EndPointHandler]
public class TradingHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.POST, "/tradings")]
    public HttpResponse PostTradingDeal(HttpRequest request)
    {
        try
        {
            // deserialize body
            TradeOffer? offer = JsonSerializer.Deserialize<TradeOffer>(request.Body);

            // check if body is valid
            if(offer == null ||
               offer.CardType == null ||
               offer.CardGuid == null ||
               offer.MinDamage == null)
               return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: true);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // get card from provided guid
            if(unit.CardRepository.GetCardFromGuid(offer.CardGuid) is not Card card)
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid card provided")));

            // unassign card from user
            if(!unit.CardRepository.RemoveCardOwnership(user, card))
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid card provided")));

            // create trading deal
            TradingDeal deal = new(
                tradeId: Guid.NewGuid().ToString(),
                userId: user.Id,
                card: card,
                minDamage: offer.MinDamage.GetValueOrDefault(),
                cardType: offer.CardType
            );

            if(!unit.TradingRepository.CreateTradingDeal(user, deal))
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid trading details provided")));

            // commit work
            unit.Commit();

            return new HttpResponse("201 Created");
        }
        catch(JsonException)
        {
            return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));
        }
        catch(NpgsqlException)
        {
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect ot database")));
        }
    }
}