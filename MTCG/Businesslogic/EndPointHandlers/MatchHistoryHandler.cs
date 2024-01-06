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
public class MatchHistoryHandler(string connectionstring)
{
    private readonly string _connectionString = connectionstring;

    [EndPoint(HttpMethods.GET, "/history")]
    public HttpResponse GetHistory(HttpRequest request)
    {
        try
        {
            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // check user from auth token and check permission
            if(unit.UserRepository.GetUser(request.GetToken()) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Access token is missing or invalid")));

            // get history
            List<MatchHistory> history = unit.MatchHistoryRepository.GetHistory(user);

            if(history.Count == 0)
                return new HttpResponse("204 No Content");

            return new HttpResponse("200 OK", JsonSerializer.Serialize(history));
        }
        catch(NpgsqlException)
        {
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect to database")));
        }
    }
}