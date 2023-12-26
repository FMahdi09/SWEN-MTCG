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
    public HttpMethods Method { get; }

    /// <summary>
    /// requested resource as a string array
    /// </summary>
    public string[] Resource { get; }

    /// <summary>
    /// provided body as string. string.Empty in case of no body
    /// </summary>
    public string Body { get; }

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

}