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
public class SessionHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.POST, "/sessions")]
    public HttpResponse PostSession(HttpRequest request)
    {
        try
        {
            // deserialize body
            UserCredentials? userCredentials = JsonSerializer.Deserialize<UserCredentials>(request.Body);

            // check if body is valid
            if(userCredentials is null ||
                userCredentials.Username is null ||
                userCredentials.Password is null)
                return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // get user from credentials
            if(unit.UserRepository.GetUser(userCredentials.Username, userCredentials.Password) is not User user)
                return new HttpResponse("401 Unauthorized", JsonSerializer.Serialize(new Error("Invalid username/password provided")));

            // generate token and create session
            string token = Guid.NewGuid().ToString();

            unit.TokenRepository.AddToken(user.Id, token);

            // send token to client
            return new HttpResponse("200 OK", JsonSerializer.Serialize(new AuthToken(token)));
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