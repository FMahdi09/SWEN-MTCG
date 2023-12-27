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
public class UserHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;

    [EndPoint(HttpMethods.POST, "/users")]
    public HttpResponse PostUser(HttpRequest request)
    {
        try
        {
            // deserialize body
            UserCredentials? userCredentials = JsonSerializer.Deserialize<UserCredentials>(request.Body);

            // check if body is valid
            if(userCredentials is null      ||
                userCredentials.Username is null ||
                userCredentials.Password is null)
                return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid Body")));

            // create unit of work
            using UnitOfWork unit = new(_connectionString, withTransaction: false);

            // create new user
            User user = new(username: userCredentials.Username, password: userCredentials.Password);

            // add new user
            if(!unit.UserRepository.CreateUser(user))
                return new HttpResponse("409 Conflict", JsonSerializer.Serialize(new Error("Username already exists")));

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