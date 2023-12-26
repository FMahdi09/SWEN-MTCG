using System.Net;
using System.Net.Sockets;
using SWEN.HttpServer.Base;

namespace SWEN.HttpServer;

/// <summary>
/// manages a TcpListener and starts a thread for each connected client
/// </summary>
/// <param name="iPAddress"> IP address to listen on </param>
/// <param name="port"> port to listen on </param>
/// <param name="logic"> logic to use when generating responses (must implement the ILogic interface) </param>
public class HttpServer(IPAddress iPAddress, int port, ILogic logic)
{
    private readonly TcpListener _tcpListener = new(iPAddress, port);
    private readonly ILogic _logic = logic;

    /// <summary>
    /// starts listening and accepting new clients
    /// </summary>
    public void Start()
    {
        while(true)
        {
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();

            Thread clientThread = new(() => HandleClient(tcpClient));
            clientThread.Start();
        }
    }

    private void HandleClient(TcpClient client)
    {
        using StreamReader reader = new(client.GetStream());
        using StreamWriter writer = new(client.GetStream());

        try
        {
            // get HTTP request
            HttpRequest request = HttpParser.ParseRequest(reader);

            // get HTTP response
            HttpResponse response = _logic.GetResponse(request);

            // send HTTP response
            writer.Write(response);
        }
        catch(HttpRequestException)
        {
            writer.Write(new HttpResponse("400 Bad Request"));
        }
        catch(EndOfStreamException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
