using System.Text;
using SWEN.HttpServer.Enums;

namespace SWEN.HttpServer;

/// <summary>
/// a HTTP request containing: method, resource, body and headers
/// </summary>
public class HttpRequest
{
    /// <summary>
    /// requested method
    /// </summary>
    public HttpMethods Method { get; set; }

    /// <summary>
    /// requested resource as a string array
    /// </summary>
    public string[] Resource { get; set; }

    /// <summary>
    /// provided body as string. string.Empty in case of no body
    /// </summary>
    public string Body { get; set;}

    /// <summary>
    /// dictionary containing all provided headers
    /// </summary>
    public Dictionary<string, string> Headers { get; }

    /// <summary>
    /// instantiates a HTTP request with body
    /// </summary>
    /// <param name="method"> requested method </param>
    /// <param name="resource"> requested resource </param>
    /// <param name="body"> provided body </param>
    /// <param name="headers"> provided headers </param>
    public HttpRequest(HttpMethods method, string[] resource, string body, Dictionary<string, string> headers)
        => (Method, Resource, Body, Headers) = (method, resource, body, headers);  

    /// <summary>
    /// instantiates a HTTP request without body
    /// </summary>
    /// <param name="method"> requested method </param>
    /// <param name="resource"> requested resource </param>
    /// <param name="headers"> provided headers </param>
    public HttpRequest(HttpMethods method, string[] resource, Dictionary<string, string> headers)
        => (Method, Resource, Body, Headers) = (method, resource, string.Empty, headers);  


    /// <summary>
    /// gets the token from the authorization header
    /// </summary>
    /// <returns> return the authorization header, if no authorization header has been provided returns string.Empty</returns>
    public string GetToken()
    {
        if(!Headers.TryGetValue("Authorization", out string? value))
            return string.Empty;

        return value;
    }
    
    /// <summary>
    /// gets the valid request as a string 
    /// </summary>
    /// <returns> request as a valid HTTP string </returns>
    public override string ToString()
    {
        StringBuilder builder = new();

        // request line
        switch(Method)
        {
            case HttpMethods.GET:
                builder.Append("GET ");
                break;
            case HttpMethods.POST:
                builder.Append("POST ");
                break;
            case HttpMethods.PUT:
                builder.Append("PUT ");
                break;
            case HttpMethods.DELETE:
                builder.Append("DELETE ");
                break;
        }

        foreach(string element in Resource)
        {
            builder.Append($"/{ element }");
        }

        builder.Append(" HTTP/1.1");
        builder.AppendLine();

        // headers

        if(Body != String.Empty)
        {
            builder.AppendLine("Content-Type: application/json");
            builder.AppendLine($"Content-Length: {Body.Length}");
        }

        foreach(var header in Headers)
        {
            builder.AppendLine($"{header.Key}: {header.Value}");
        }

        builder.AppendLine();

        // body

        if(Body != String.Empty)
        {
            builder.AppendLine(Body);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}