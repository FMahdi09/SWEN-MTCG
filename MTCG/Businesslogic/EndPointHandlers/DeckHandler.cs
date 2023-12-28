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
public class DeckHandler(string connectionString)
{
    const int DeckSize = 4;
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.GET, "/deck")]
    public HttpResponse GetDeck(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // get deck from user
            List<Card> deck = unit.DeckRepository.GetDeck(user);

            if(deck.Count == 0)
                return new HttpResponse("204 No Content");

            return new HttpResponse("200 OK", JsonSerializer.Serialize(deck));
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

    [EndPoint(HttpMethods.PUT, "/deck")]
    public HttpResponse PutDeck(HttpRequest request)
    {
        try
        {
            // deserialize body
            List<string> cardGuids = JsonSerializer.Deserialize<List<string>>(request.Body)
                ?? throw new JsonException();

            // remove all duplicates
            cardGuids = cardGuids.Distinct().ToList();

            // check if correct number of cards have been provided
            if(cardGuids.Count != DeckSize)
                return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid number of cards")));

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: true);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // clear deck
            unit.DeckRepository.ClearDeck(user);

            // set deck
            if(!unit.DeckRepository.SetDeck(user, cardGuids))
                return new HttpResponse("403 Forbidden", JsonSerializer.Serialize(new Error("Invalid cards provided")));

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
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect to database")));
        }
    }
}