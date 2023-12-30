using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using SWEN.HttpSender;
using SWEN.HttpServer;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.Models.SerializationObjects;

HttpSender client = new(IPAddress.Loopback, 12345);
string body;
HttpRequest request;
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

static void PrintResult(string endPoint, string message, HttpRequest request, bool clearScreen, HttpSender client)
{
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine($"# { endPoint }");
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine($"# { message }");
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine();
    System.Console.WriteLine(request);
    System.Console.WriteLine("############################################################");
    System.Console.WriteLine();
    System.Console.WriteLine(client.SendRequest(request));
    System.Console.ReadKey();

    if(clearScreen)
        System.Console.Clear();
};

if(clearScreen)
    System.Console.Clear();

// POST /users

// success alice
body = JsonSerializer.Serialize(AliceCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    new()
    {
        {"Content-Length", body.Length.ToString()}
    }
);

PrintResult("POST /users", "Successful: Alice", request, clearScreen, client);

// success bob
body = JsonSerializer.Serialize(BobCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    new()
    {
        {"Content-Length", body.Length.ToString()}
    }
);

PrintResult("POST /users", "Successful: Bob", request, clearScreen, client);

// failed duplicate
body = JsonSerializer.Serialize(AliceCredentials);

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    new()
    {
        {"Content-Length", body.Length.ToString()}
    }
);

PrintResult("POST /users", "Failed: Duplicate", request, clearScreen, client);

// failed invalid body
body = "invalid body";

request = new(
    HttpMethods.POST,
    ["users"],
    body,
    new()
    {
        {"Content-Length", body.Length.ToString()}
    }
);

PrintResult("POST /users", "Failed: Invalid Body", request, clearScreen, client);