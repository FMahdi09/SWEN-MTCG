using System.Diagnostics;
using System.Text.Json;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Tests.EndpointHandler.Users;

[TestFixture]
public class PostUserTests
{
    private UserHandler _userHandler;

    private readonly UserCredentials aliceCredentials = new()
    {
        Username = "Alice",
        Password = "passworod"
    };
    
    private readonly UserCredentials bobCredentials = new()
    {
        Username = "Bob",
        Password = "password"
    };

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _userHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");

        // initialize database
        DbConfig config = new(
            connectionString: "Host=localhost;Username=postgres;Password=postgres;Database=testdb",
            fillScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\fillDatabase.sql",
            createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
            dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
        );

        DBInitializer initializer = new(config);
        initializer.InitDB();
    }

    [Test]
    public void Successfull()
    {
        // arrange
        string body = JsonSerializer.Serialize(aliceCredentials);

        HttpRequest request = new(
            HttpMethods.POST,
            ["users"],
            body,
            []
        );

        // act
        HttpResponse response = _userHandler.PostUser(request);

        // assert
        Debug.Assert(response.Status == "201 Created", "POST /users failed with correct credentials");
    }

    [Test]
    public void InvalidBody()
    {
        // arrange
        string body = "invalid body"; 

        HttpRequest request = new(
            HttpMethods.POST,
            ["users"],
            body,
            []
        );

        // act
        HttpResponse response = _userHandler.PostUser(request);

        // assert
        Debug.Assert(response.Status == "400 Bad Request", "POST /users failed with invalid body");
    }

    [Test]
    public void UsernameExists()
    {
        // arrange
        string body = JsonSerializer.Serialize(bobCredentials);

        HttpRequest request = new(
            HttpMethods.POST,
            ["users"],
            body,
            []
        );

        // act
        HttpResponse successfullResponse = _userHandler.PostUser(request);
        HttpResponse unsuccessfullResponse = _userHandler.PostUser(request);

        // assert
        Debug.Assert(successfullResponse.Status == "201 Created", "POST /users failed with correct credentials");
        Debug.Assert(unsuccessfullResponse.Status == "409 Conflict", "POST /users created user with duplicate credentials");
    }
}