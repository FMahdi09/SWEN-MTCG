using System.Text.Json;
using NUnit.Framework;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Tests.EndPointHandler.Users;

[TestFixture]
public class GetUserDataTests
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
            fillScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG.Tests\Scripts\fillDatabaseGetUserData.sql",
            createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
            dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
        );

        DBInitializer initializer = new(config);
        initializer.InitDB();
    }

    [Test]
    public void Successful()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["users", "Alice"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        // act
        HttpResponse response = _userHandler.GetUser(request);

        UserData? data = JsonSerializer.Deserialize<UserData>(response.Body);

        // assert
        Assert.That(data, Is.Not.EqualTo(null));

        Assert.Multiple(() =>
        {
            Assert.That(data.Username, Is.EqualTo("Alice"));
            Assert.That(data.Bio, Is.EqualTo("alice-bio"));
            Assert.That(data.Image, Is.EqualTo("alice-image"));
        });
    }

    [Test]
    public void InvalidUser()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["users", "Charlie"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        // act
        HttpResponse response = _userHandler.GetUser(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("401 Unauthorized"));
    }
}