using System.Text.Json;
using System.Text.Json.Serialization;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Tests.EndPointHandler.Deck;

[TestFixture]
public class GetStatsTests
{
    private ScoreHandler _scoreHandler;

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _scoreHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");

        // initialize database
        DbConfig config = new(
            connectionString: "Host=localhost;Username=postgres;Password=postgres;Database=testdb",
            fillScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG.Tests\Scripts\fillDatabasePostPackage.sql",
            createScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\createDatabase.sql",
            dropScript: @"C:\Users\fabia\Documents\Technikum\SWEN\Semesterprojekt\MTCG\DataAccess\Scripts\dropDatabase.sql"
        );

        DBInitializer initializer = new(config);
        initializer.InitDB();
    }

    [Test]
    public void InvalidAuth()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["stats"],
            new()
            {
                {"Authorization", "invalid token"}
            }
        );

        // act
        HttpResponse response = _scoreHandler.GetStats(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("401 Unauthorized"));
    }

    [Test]
    public void Successful()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["stats"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        // act
        HttpResponse response = _scoreHandler.GetStats(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("200 OK"));
    }
}