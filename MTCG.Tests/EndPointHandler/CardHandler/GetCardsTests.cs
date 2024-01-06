using System.Text.Json;
using NUnit.Framework;
using SWEN.DbInitializer;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.EndpointHandlers;
using SWEN.MTCG.Integration;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Tests.EndPointHandler.Cards;

[TestFixture]
public class GetCardTests
{
    private CardHandler _cardHandler;
    private TransactionHandler _transactionHandler;

    [SetUp]
    public void SetUp()
    {
        // initialize handler
        _cardHandler = new("Host=localhost;Username=postgres;Password=postgres;Database=testdb");
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
    public void NoCards()
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
        HttpResponse response = _cardHandler.GetCards(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("204 No Content"));
    }

    [Test]
    public void Successful()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.POST,
            ["transactions", "packages"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        _transactionHandler.PostPackage(request);

        request = new(
            HttpMethods.GET,
            ["cards"],
            new()
            {
                {"Authorization", "alice-token"}
            }
        );

        // act
        HttpResponse response = _cardHandler.GetCards(request);

        List<Card> cards = JsonSerializer.Deserialize<List<Card>>(response.Body) ?? [];

        // assert
        Assert.That(cards, Has.Count.EqualTo(4));       
    }

    [Test]
    public void InvalidAuth()
    {
        // arrange
        HttpRequest request = new(
            HttpMethods.POST,
            ["transactions", "packages"],
            new()
            {
                {"Authorization", "invalid-token"}
            }
        );       

        // act
        HttpResponse response = _cardHandler.GetCards(request);

        // assert
        Assert.That(response.Status, Is.EqualTo("401 Unauthorized"));
    }
}