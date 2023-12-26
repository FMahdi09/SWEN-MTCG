namespace SWEN.HttpServer.Base;

/// <summary>
/// any logic provided to the httpserver needs to implement this interface
/// </summary>
public interface ILogic
{
    /// <summary>
    /// generates a response for a request
    /// </summary>
    /// <param name="request"> provided request </param>
    /// <returns> generated response </returns>
    HttpResponse GetResponse(HttpRequest request);
}