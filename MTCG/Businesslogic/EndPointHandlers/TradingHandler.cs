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

    [EndPoint(HttpMethods.GET, "/tradings")]
    public HttpResponse GetAllTradingDeals(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize("Access token is missing or invalid"));

            // get trading deals
            List<TradingDeal> tradingDeals = unit.TradingRepository.GetTradingDeals();

            if(tradingDeals.Count == 0)
                return new HttpResponse("204 No Content");

            return new HttpResponse("200 OK", JsonSerializer.Serialize(tradingDeals));
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

    [EndPoint(HttpMethods.GET, "/tradings/:username")]
    public HttpResponse GetUserTradingDeals(HttpRequest request)
    {
        try
        {
            // get username from resource
            string username = request.Resource[1];

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user ||
               user.Username != username)
               return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));

            // get trading deals
            List<TradingDeal> deals = unit.TradingRepository.GetTradingDeals(user);

            if(deals.Count == 0)
                return new HttpResponse("204 No Content");

            return new HttpResponse("200 OK", JsonSerializer.Serialize(deals));
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

    [EndPoint(HttpMethods.DELETE, "/tradings/:guid")]
    public HttpResponse DeleteTradingDeal(HttpRequest request)
    {
        try
        {
            // get guid from resource
            string guid = request.Resource[1];

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: true);

            // get user from token and check permissions
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));

            // get deal from guid
            if(unit.TradingRepository.GetTradingDeal(guid) is not TradingDeal deal)
                return new HttpResponse("404 Not Found");

            // check if deal belongs to user
            if(deal.UserId != user.Id)
                return new HttpResponse("403 Forbidden");

            // delete deal
            if(!unit.TradingRepository.DeleteTradingDeal(deal))
                return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to delete deal")));

            // assign card back to user
            unit.CardRepository.ChangeCardOwnership(user.Id, deal.Card);

            // commit work
            unit.Commit();

            return new HttpResponse("200 OK");
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

    [EndPoint(HttpMethods.POST, "/tradings/:guid")]
    public HttpResponse AcceptTradingDeal(HttpRequest request)
    {
        try
        {
            // get guid from resource
            string dealGuid = request.Resource[1];

            // deserialize body
            string cardGuid = JsonSerializer.Deserialize<string>(request.Body)
                ?? throw new JsonException();

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: true);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // get deal from guid
            if(unit.TradingRepository.GetTradingDeal(dealGuid) is not TradingDeal deal)
                return new HttpResponse("404 Not Found", JsonSerializer.Serialize(new Error("Deal not found")));

            // get card from guid
            if(unit.CardRepository.GetCardFromGuid(cardGuid) is not Card card)
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid card provided")));

            // check trade deal requirements
            if(card.Type != deal.CardType ||
               card.Damage < deal.MinDamage)
               return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Card does not match requirements")));

            // unassign card from user (also checks for ownership and deck)
            if(!unit.CardRepository.RemoveCardOwnership(user, card))
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid card provided")));

            // delete trading deal
            if(!unit.TradingRepository.DeleteTradingDeal(deal))
                return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to accept deal")));

            // perform trade:

            // reassign cards
            unit.CardRepository.ChangeCardOwnership(user.Id, deal.Card);
            unit.CardRepository.ChangeCardOwnership(deal.UserId, card);

            // commit work
            unit.Commit();

            return new HttpResponse("200 OK");
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