using System.Text;

namespace SWEN.HttpServer;

/// <summary>
/// a HTTP response containing: status and body
/// </summary>
public class HttpResponse
{
    /// <summary>
    /// response statuscode + message
    /// </summary>
    public string Status { get; }
    
    /// <summary>
    /// resonse body. string.Empty if no body
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// instantiates a response with body
    /// </summary>
    /// <param name="status"> response statuscode + message </param>
    /// <param name="body"> response body </param>
    public HttpResponse(string status, string body)
        => (Status, Body) = (status, body);

    /// <summary>
    /// instantiates a response without body
    /// </summary>
    /// <param name="status"> response statuscode + message </param>
    public HttpResponse(string status)
        => (Status, Body) = (status, string.Empty);

    /// <summary>
    /// gets the valid request as a string 
    /// </summary>
    /// <returns> request as a valid HTTP string </returns>
    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine("HTTP/1.1 " + Status);

        if(Body != string.Empty)
        {
            builder.AppendLine("Content-Type: application/json");
            builder.AppendLine($"Content-Length: { Body.Length }");
        }

        builder.AppendLine();

        if(Body != string.Empty)
        {
            builder.AppendLine(Body);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}