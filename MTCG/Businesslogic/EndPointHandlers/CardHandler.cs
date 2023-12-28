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
public class CardHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.GET, "/cards")]
    public HttpResponse GetCards(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
               return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));   
            
            // get cards from user
            List<Card> cards = unit.CardRepository.GetCards(user);

            if(cards.Count == 0)
                return new HttpResponse("204 No Content");

            return new HttpResponse("200 OK", JsonSerializer.Serialize(cards));
        }
        catch(JsonException)
        {
            return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));
        }
        catch(NpgsqlException)
        {
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect to database")));
        }
    }
}