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
            User user = new(userCredentials.Username, userCredentials.Password);

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
           return new HttpResponse("500 Internal Server Error", JsonSerializer.Serialize(new Error("Unable to connect to database")));
        }
    }

    [EndPoint(HttpMethods.GET, "/users/:username")]
    public HttpResponse GetUser(HttpRequest request)
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

            // send userdata to client
            UserData userData = new()
            {
                Username = user.Username,
                Bio = user.Bio,
                Image = user.Image
            };

            return new HttpResponse("200 OK", JsonSerializer.Serialize(userData));
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