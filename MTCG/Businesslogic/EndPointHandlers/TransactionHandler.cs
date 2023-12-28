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
public class TransactionHandler(string connectionString)
{
    const int PackagePrice = 5;
    const int CardsPerPackage = 4;
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.POST, "/transactions/packages")]
    public HttpResponse PostPackage(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: true);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));

            // make user pay
            if(!unit.UserRepository.PayCurrency(user, PackagePrice))
                return new HttpResponse("409 Conflict", JsonSerializer.Serialize(new Error("Not enough money")));

            // generate cards
            List<Card> cards = unit.CardRepository.GenerateCards(CardsPerPackage);

            // assign cards to user
            unit.CardRepository.AssignCards(user, cards);           

            // commit work
            unit.Commit();

            return new HttpResponse("200 OK", JsonSerializer.Serialize(cards));
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