using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SWEN.HttpSender;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.Integration;
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

PrintResult("GET /cards", "Successful: Alice", request, clearScreen, client);

// success bob
request = new(
    HttpMethods.GET,
    ["cards"],
    new()
    {
        {"Authorization", bobAuth}
    }
);

PrintResult("GET /cards", "Successful: Bob", request, clearScreen, client);

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