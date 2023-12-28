using System.Text.Json;
using Npgsql;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.Attributes;
using SWEN.MTCG.DataAccess.UnitOfWork;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.DAtaModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.BusinessLogic.EndpointHandlers;

[EndPointHandler]
public class ScoreHandler(string connectionString)
{
    const int DefaultScoreboardSize = 10;
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.GET, "/stats")]
    public HttpResponse GetStats(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permissions
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));

            // get stats from user
            if(unit.ScoreRepository.GetUserStats(user) is not UserStats stats)
                return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to retrieve stats")));

            return new HttpResponse("200 OK", JsonSerializer.Serialize(stats));
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

    [EndPoint(HttpMethods.GET, "/scoreboard")]
    public HttpResponse GetScoreboard(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token missing or invalid")));

            // get scoreboard
            List<UserStats> scoreboard = unit.ScoreRepository.GetScoreboard(DefaultScoreboardSize);

            return new HttpResponse("200 OK", JsonSerializer.Serialize(scoreboard));
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