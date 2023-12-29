using System.Diagnostics;
using System.Text.Json;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Tests.EndPointHandler.Session;

[TestFixture]
public class PostSessionTests
{
    private SessionHandler _sessionHandler;

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _sessionHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");

        // initialize database
        DbConfig config = new(
            connectionString: "Host=localhost;Username=postgres;Password=postgres;Database=testdb",
            fillScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG.Tests\Scripts\fillDatabasePostSession.sql",
            createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
            dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
        );

        DBInitializer initializer = new(config);
        initializer.InitDB();
   }

    [Test]
    public void InvalidBody()
    {
        // arrange 

        // body
        string body = "not valid";

        // headers
        Dictionary<string, string> headers = new()
        {
            {"Content-Length", body.Length.ToString()}
        };

        // resource
        string[] resource = ["session"];

        HttpRequest request = new(HttpMethods.GET, resource, body, headers);

        // act
        HttpResponse response = _sessionHandler.PostSession(request);

        // assert
        Debug.Assert(response.Status == "400 Bad Request", "POST /session failed with invalid body");
    }

    [Test]
    public void InvalidCredentials()
    {
        // arrange 

        // body
        UserCredentials credentials = new()
        {
            Username = "Alice",
            Password = "wrong password"
        };

        string body = JsonSerializer.Serialize(credentials);

        // headers
        Dictionary<string, string> headers = new()
        {
            {"Content-Length", body.Length.ToString()}
        };

        // resource
        string[] resource = ["session"];

        HttpRequest request = new(HttpMethods.GET, resource, body, headers);

        // act
        HttpResponse response = _sessionHandler.PostSession(request);

        // assert
        Debug.Assert(response.Status == "401 Unauthorized", "POST /session failed with invalid credentials");
    }

    [Test]
    public void Successfull()
    {
        // arrange 

        // body
        UserCredentials credentials = new()
        {
            Username = "Alice",
            Password = "password"
        };

        string body = JsonSerializer.Serialize(credentials);

        // headers
        Dictionary<string, string> headers = new()
        {
            {"Content-Length", body.Length.ToString()}
        };

        // resource
        string[] resource = ["session"];

        HttpRequest request = new(HttpMethods.GET, resource, body, headers);

        // act
        HttpResponse response = _sessionHandler.PostSession(request);

        // assert
        Debug.Assert(response.Status == "200 OK", "POST /session failed with valid credentials");
    }
}