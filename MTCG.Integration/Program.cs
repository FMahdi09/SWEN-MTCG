using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SWEN.HttpSender;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.Integration;
using SWEN.MTCG.Models.DataModels;
using SWEN.MTCG.Models.SerializationObjects;

HttpSender client = new(IPAddress.Loopback, 12345);
string body, aliceAuth, bobAuth;
HttpRequest request;
HttpResponse response;
bool clearScreen = true;

// data
UserCredentials AliceCredentials = new()
{
    Username = "Alice",
    Password = "password"
};

UserCredentials BobCredentials = new() 
{
    Username = "Bob",
    Password = "password"
};

UserCredentials wrongCredentials = new()
{
    Username = "Alice",
    Password = "wrong password"
};

UserData AliceData = new()
{
    Username = "Alice",
    Bio = "Mein Name ist Alice",
    Image = ":^)"
};

UserData BobData = new()
{
    Username = "Bob",
    Bio = "Ich bin der Bob",
    Image = ":P"
};

// helper functions

static HttpResponse PrintResult(string endPoint, string message, HttpRequest request, bool clearScreen, HttpSender client)
{
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine($"# { endPoint }");
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine($"# { message }");
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine();

    if(request.Body != String.Empty)
        request.Body = PrettyJson(request.Body);

    System.Console.WriteLine(request);
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine();
    HttpResponse response = client.SendRequest(request);

    if(response.Body != String.Empty)
        response.Body = PrettyJson(response.Body);

    System.Console.WriteLine(response);
    System.Console.ReadKey();

    if(clearScreen)
    {
        System.Console.Clear();
        System.Console.WriteLine("\x1b[3J");
    }
    return response;
};

static string PrettyJson(string notPrettyJson)
{
    var options = new JsonSerializerOptions(){
        WriteIndented = true
    };

    var jsonElement = JsonSerializer.Deserialize<JsonElement>(notPrettyJson);

    return JsonSerializer.Serialize(jsonElement, options);
}

// integration script starts here

if(clearScreen)
    System.Console.Clear();

// POST /users

// success alice
body = JsonSerializer.Serialize(AliceCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    []
);

PrintResult("POST /users", "Successful: Alice", request, clearScreen, client);

// success bob
body = JsonSerializer.Serialize(BobCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    []
);

PrintResult("POST /users", "Successful: Bob", request, clearScreen, client);

// failed duplicate
body = JsonSerializer.Serialize(AliceCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    []
);

PrintResult("POST /users", "Failed: Duplicate", request, clearScreen, client);

// POST /session

// success alice
body = JsonSerializer.Serialize(AliceCredentials);

request = new(
    HttpMethods.POST,
    ["sessions"],
    body,
    []
);

response = PrintResult("POST /sessions", "Successful: Alice", request, clearScreen, client);

Auth? alice = JsonSerializer.Deserialize<Auth>(response.Body);

aliceAuth = alice?.Token ?? "token failed";

// success bob
body = JsonSerializer.Serialize(BobCredentials);

request = new(
    HttpMethods.POST,
    ["sessions"],
    body,
    []
);

response = PrintResult("POST /sessions", "Successful: Bob", request, clearScreen, client);

Auth? bob = JsonSerializer.Deserialize<Auth>(response.Body);

bobAuth = bob?.Token ?? "token failed";

// failed wrong credentials
body = JsonSerializer.Serialize(wrongCredentials);

request = new(
    HttpMethods.POST,
    ["sessions"],
    body,
    []
);

PrintResult("POST /session", "Failed: wrong Credentials", request, clearScreen, client);

// GET /users/:username

// success alice
request = new(
    HttpMethods.GET,
    ["users", "Alice"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /users/:username", "Successful: Alice", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["users", "Bob"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /users/:username", "Successful: Bob", request, clearScreen, client);

// failed alice tries to access bob
request = new(
    HttpMethods.GET,
    ["users", "Bob"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /users/:username", "Failed: Alice tries to access Bobs data", request, clearScreen, client);

// PUT /users/:username

// success alice
body = JsonSerializer.Serialize(AliceData);

request = new(
    HttpMethods.PUT,
    ["users", "Alice"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("PUT /users/:username", "Successful: Alice", request, clearScreen, client);

// success bob
body = JsonSerializer.Serialize(BobData);

request = new(
    HttpMethods.PUT,
    ["users", "Bob"],
    body,
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("PUT /users/:username", "Successful: Bob", request, clearScreen, client);

// GET /users/:username after PUT

// success alice
request = new(
    HttpMethods.GET,
    ["users", "Alice"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /users/:username", "Successful: Alice new data", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["users", "Bob"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /users/:username", "Successful: Bob new data", request, clearScreen, client);

// GET /cards empty 

// success alice
request = new(
    HttpMethods.GET,
    ["cards"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /cards", "Successful: Alice no cards", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["cards"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /cards", "Successful: Bob no cards", request, clearScreen, client);

// POST /transactions/packages

// bob buys 2 packs
request = new(
    HttpMethods.POST,
    ["transactions", "packages"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("POST /transactions/packages", "Successful: Bob buys package 1", request, clearScreen, client);
PrintResult("POST /transactions/packages", "Successful: Bob buys package 2", request, clearScreen, client);

// alice buys 4 packages
request = new(
    HttpMethods.POST,
    ["transactions", "packages"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("POST /transactions/packages", "Successful: Alice buys package 1", request, clearScreen, client);
PrintResult("POST /transactions/packages", "Successful: Alice buys package 2", request, clearScreen, client);
PrintResult("POST /transactions/packages", "Successful: Alice buys package 3", request, clearScreen, client);
PrintResult("POST /transactions/packages", "Successful: Alice buys package 4", request, clearScreen, client);

// failed: alice tries to buy package without money
PrintResult("POST /transactions/packages", "Failed: Alice is out of money", request, clearScreen, client);

// GET /cards with content

// success alice
request = new(
    HttpMethods.GET,
    ["cards"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

response = PrintResult("GET /cards", "Successful: Alice", request, clearScreen, client);

List<CardCode> aliceCards = JsonSerializer.Deserialize<List<CardCode>>(response.Body) ?? []; 

// success bob
request = new(
    HttpMethods.GET,
    ["cards"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

response = PrintResult("GET /cards", "Successful: Bob", request, clearScreen, client);

List<CardCode> bobCards = JsonSerializer.Deserialize<List<CardCode>>(response.Body) ?? []; 

// GET /deck empty

// success alice
request = new(
    HttpMethods.GET,
    ["deck"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /deck", "Successful: Alice empty deck", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["deck"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /deck", "Successful: Bob empty deck", request, clearScreen, client);

// PUT /deck

// failure not enough cards

List<string> cards = [];

for(int i = 0; i < 3; ++i)
{
    cards.Add(aliceCards[i].Guid ?? "");
}

body = JsonSerializer.Serialize(cards);

request = new(
    HttpMethods.PUT,
    ["deck"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("PUT /deck", "Failure: invalid number of cards provided", request, clearScreen, client);

// invalid card tokens provided

cards = [
    "invalid token 1",
    "invalid token 2",
    "invalid token 3",
    "invalid token 4"
];

body = JsonSerializer.Serialize(cards);

request = new(
    HttpMethods.PUT,
    ["deck"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("PUT /deck", "Failure: invalid card tokens provided", request, clearScreen, client);

// failure alice tries to access bob cards

cards.Clear();

for(int i = 0; i < 4; ++i)
{
    cards.Add(bobCards[i].Guid ?? "");
}

body = JsonSerializer.Serialize(cards);

request = new(
    HttpMethods.PUT,
    ["deck"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("PUT /deck", "Failure: Alice tries to access Bob cards", request, clearScreen, client);

// success alice

cards.Clear();

for(int i = 0; i < 4; ++i)
{
    cards.Add(aliceCards[i].Guid ?? "");
}

body = JsonSerializer.Serialize(cards);

request = new(
    HttpMethods.PUT,
    ["deck"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("PUT /deck", "Successful: Alice", request, clearScreen, client);

// success bob

cards.Clear();

for(int i = 0; i < 4; ++i)
{
    cards.Add(bobCards[i].Guid ?? "");
}

body = JsonSerializer.Serialize(cards);

request = new(
    HttpMethods.PUT,
    ["deck"],
    body,
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("PUT /deck", "Successful: Bob", request, clearScreen, client);

// GET /deck after successful PUT

// success alice
request = new(
    HttpMethods.GET,
    ["deck"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /deck", "Successful: Alice deck", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["deck"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /deck", "Successful: Bob deck", request, clearScreen, client);

// GET /tradings

request = new(
    HttpMethods.GET,
    ["tradings"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /tradings", "Successful: No tradingdeals available", request, clearScreen, client);

// POST /tradings

// success alice

TradeOffer offer = new()
{
    CardType = "monster",
    CardGuid = aliceCards[5].Guid,
    MinDamage = 20
};

body = JsonSerializer.Serialize(offer);

request = new(
    HttpMethods.POST,
    ["tradings"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("POST /tradings", "Successful: Alice", request, clearScreen, client);

// success bob

offer = new()
{
    CardType = "spell",
    CardGuid = bobCards[5].Guid,
    MinDamage = 40
};

body = JsonSerializer.Serialize(offer);

request = new(
    HttpMethods.POST,
    ["tradings"],
    body,
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("POST /tradings", "Successful: Bob", request, clearScreen, client);

// GET /tradings

request = new(
    HttpMethods.GET,
    ["tradings"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /tradings", "Successful: No tradingdeals available", request, clearScreen, client);

// GET /tradings/:username

request = new(
    HttpMethods.GET,
    ["tradings", "Alice"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

response = PrintResult("GET /tradings", "Successful: Alice", request, clearScreen, client);

List<TradeCode> aliceTrades = JsonSerializer.Deserialize<List<TradeCode>>(response.Body) ?? [];

request = new(
    HttpMethods.GET,
    ["tradings", "Bob"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

response = PrintResult("GET /tradings", "Successful: Bob", request, clearScreen, client);

List<TradeCode> bobTrades = JsonSerializer.Deserialize<List<TradeCode>>(response.Body) ?? [];

// DEL /tradings/:guid

// success alice

request = new(
    HttpMethods.DELETE,
    ["tradings", aliceTrades[0].TradeId ?? "unable to retrieve token from last request"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("DELETE /tradings/:guid", "Successful: Alice", request, clearScreen, client);

// GET /tradings/:username

request = new(
    HttpMethods.GET,
    ["tradings", "Alice"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /tradings", "Successful: Alice", request, clearScreen, client);

// POST /tradings/:guid

body = JsonSerializer.Serialize("unlucky no spell drawn");

foreach(var card in aliceCards[4..15])
{
    if(card.Type == "spell")
    {
        body = JsonSerializer.Serialize(card.Guid ?? "unable to retrieve card guid");
        break;
    }
}

request = new(
    HttpMethods.POST,
    ["tradings", bobTrades[0].TradeId ?? "unable to retrieve trade id for bobs request"],
    body,
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("POST /tradings/:guid", "Successful: Alice accepts bobs trade", request, clearScreen, client);

// GET /tradings

request = new(
    HttpMethods.GET,
    ["tradings"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /tradings", "Successful: No tradingdeals available", request, clearScreen, client);

// GET /stats

request = new(
    HttpMethods.GET,
    ["stats"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /stats", "Successful: Alice", request, clearScreen, client);

request = new(
    HttpMethods.GET,
    ["stats"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /stats", "Successful: Bob", request, clearScreen, client);

// GET /scoreboard

request = new(
    HttpMethods.GET,
    ["scoreboard"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /scoreboard", "Successful: Scoreboard", request, clearScreen, client);

// POST /battles

HttpRequest AliceRequest = new(
    HttpMethods.POST,
    ["battles"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

Task parallelBattle = new(() => client.SendRequest(AliceRequest));
parallelBattle.Start();

request = new(
    HttpMethods.POST,
    ["battles"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

response = client.SendRequest(request);

System.Console.WriteLine("############################################################");
System.Console.WriteLine("# POST /battles");
System.Console.WriteLine("############################################################");
System.Console.WriteLine($"# Successful: Alice vs Bob");
System.Console.WriteLine("############################################################");
System.Console.WriteLine();
System.Console.WriteLine(request);
System.Console.WriteLine("############################################################");
System.Console.WriteLine(JsonSerializer.Deserialize<string>(response.Body));
System.Console.ReadKey();
if(clearScreen)
{
    System.Console.Clear();
    System.Console.WriteLine("\x1b[3J");
}

// GET /scoreboard

request = new(
    HttpMethods.GET,
    ["scoreboard"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /scoreboard", "Successful: Scoreboard", request, clearScreen, client);

// GET /matchhistory

// success alice
request = new(
    HttpMethods.GET,
    ["history"],
    new()
    {
        {"Authorization", aliceAuth}
    }
);

PrintResult("GET /history", "Successful: Alice", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["history"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /history", "Successful: Bob", request, clearScreen, client);

