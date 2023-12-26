using SWEN.HttpServer.Enums;

namespace SWEN.HttpServer;

/// <summary>
/// used for parsing HTTP requests from streams
/// </summary>
public static class HttpParser
{
    /// <summary>
    /// generates a HttpRequest object from a stream containing a HTTP request
    /// </summary>
    /// <param name="reader"> reader from which the request can be read </param>
    /// <returns> HttpRequest object </returns>
    /// <exception cref="EndOfStreamException"> if the end of the input stream is reached unexpectedly </exception>
    /// <exception cref="HttpRequestException"> if the HTTP request is invalid </exception>
    public static HttpRequest ParseRequest(StreamReader reader)
    {
        string? curLine;

        // interpret request line
        if((curLine = reader.ReadLine()) is null)
            throw new EndOfStreamException("Client closed connection");

        string[] requestLine = curLine.Split(' ');

        if(requestLine.Length != 3)
            throw new HttpRequestException("Invalid HTTP request line");

        // check HTTP version
        if(requestLine[2] is not "HTTP/1.1")
            throw new HttpRequestException("Invalid HTTP version");
    
        // get HTTP method
        HttpMethods method = requestLine[0] switch
        {
            "GET" => HttpMethods.GET,
            "POST" => HttpMethods.POST,
            "PUT" => HttpMethods.PUT,
            "DELETE" => HttpMethods.DELETE,
            _ => throw new HttpRequestException("Invalid HTTP method")
        };

        // get resource
        requestLine[1]  = requestLine[1].Replace("%20", " ");
        string[] resource = requestLine[1].Split('/', StringSplitOptions.RemoveEmptyEntries);

        // get headers
        Dictionary<string, string> headers = [];
        int contentLength = 0;

        while ((curLine = reader.ReadLine()) is not null)
        {
            if(curLine is "")
                break;

            // validate header
            string[] curHeader = curLine.Split(':');

            if(curHeader.Length is not 2)
                throw new HttpRequestException("Invalid HTTP header");

            // store header
            headers.Add(curHeader[1], curHeader[2]);

            // content length
            if(curHeader[0].Contains("Content-Length"))
            {
                if(!int.TryParse(curHeader[1].Trim(), out contentLength))
                    throw new HttpRequestException("Invalid content length");
            }
        }

        // get body
        if(contentLength > 0)
        {
            char[] buffer = new char[contentLength];
            reader.Read(buffer, 0, contentLength);
            string body = new(buffer);

            return new HttpRequest(method, resource, body, headers);
        }

        return new HttpRequest(method, resource, headers);
    }
}