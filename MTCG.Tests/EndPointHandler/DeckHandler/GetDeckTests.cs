using System.Text.Json;
using System.Text.Json.Serialization;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Models.DataModels;

namespace SWEN.MTCG.Tests.EndPointHandler.Deck;

[TestFixture]
public class GetDeckTests
{
    private DeckHandler _deckHandler;
    private TransactionHandler _transactionHandler;

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _deckHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");
        _transactionHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");

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
            ["cards"],
            new()
            {
                {"Authorization", "invalid token"}
            }
        );

        // act
        HttpResponse response = _deckHandler.GetDeck(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("401 Unauthorized"));
    }

    [Test]
    public void EmptyDeck()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.GET,
            ["cards"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        // act
        HttpResponse response = _deckHandler.GetDeck(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("204 No Content"));
    }
}