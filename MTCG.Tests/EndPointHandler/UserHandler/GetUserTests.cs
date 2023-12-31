using System.Diagnostics;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;

namespace SWEN.MTCG.Tests.EndpointHandler.Users;

[TestFixture]
public class GetUserTests
{
    private UserHandler _userHandler;

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _userHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");

        // initialize database
        DbConfig config = new(
            connectionString: "Host=localhost;Username=postgres;Password=postgres;Database=testdb",
            fillScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG.Tests\Scripts\fillDatabaseGetUser.sql",
            createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
            dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
        );

        DBInitializer initializer = new(config);
        initializer.InitDB();
    }

    [Test]
    public void InvalidToken()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["users", "Alice"],
            new()
            {
                {"Authorization", "invalid-token"}
            }
        );

        // act
        HttpResponse response = _userHandler.GetUser(request);

        // assert
        Debug.Assert(response.Status == "401 Unauthorized", "GET /users/:username failed with invalid auth token");
    }
    
    [Test]
    public void InvalidUsername()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["users", "Bob"],
            new()
            {
                {"Authorization", "test-token"}
            }
        );

        // act
        HttpResponse response = _userHandler.GetUser(request);


        // assert
        Debug.Assert(response.Status == "401 Unauthorized", "GET /users/:username failed with invalid username");
    }

    [Test]
    public void Successfull()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["users", "Alice"],
            new()
            {
                {"Authorization", "test-token"}
            }
        );

        // act
        HttpResponse response = _userHandler.GetUser(request);

        // assert
        Debug.Assert(response.Status == "200 OK", "GET /users/:username failed with invalid username");
    }
}