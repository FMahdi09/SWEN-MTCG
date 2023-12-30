using System.Net;
using System.Net.Sockets;
using SWEN.HttpServer;

namespace SWEN.HttpSender;

public class HttpSender(IPAddress iPAddress, int port)
{
    private readonly IPAddress _iPAddress = iPAddress;
    private readonly int _port = port;

    public HttpResponse SendRequest(HttpRequest request)
    {
        TcpClient client = new();
        client.Connect(_iPAddress, _port);

        using StreamReader reader = new(client.GetStream());
        using StreamWriter writer = new(client.GetStream()) {AutoFlush = true};

        // send message
        writer.Write(request);

        // recieve response
        HttpResponse response = HttpParser.ParseResponse(reader);

        // return response
        return response;
    }
}
