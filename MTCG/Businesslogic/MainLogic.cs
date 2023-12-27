using System.Reflection;
using System.Text.Json;
using Npgsql;
using SWEN.HttpServer;
using SWEN.HttpServer.Base;
using SWEN.HttpServer.Enums;
using SWEN.MTCG.BusinessLogic.Attributes;
using SWEN.MTCG.Models.SerializationObjects;

namespace SWEN.MTCG.Businesslogic;

public class MainLogic : ILogic
{
    // endpoint handlers
    private readonly Dictionary<Type, object> _handlers = [];

    // endpoint methods
    private readonly Dictionary<string[], MethodInfo> _getEndpoints = [];
    private readonly Dictionary<string[], MethodInfo> _postEndpoints = [];
    private readonly Dictionary<string[], MethodInfo> _putEndpoints = [];
    private readonly Dictionary<string[], MethodInfo> _delEndpoints = [];

    public MainLogic()
    {
        string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mydb";

        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach(Type type in assembly.GetTypes())
        {
            // instantiate handlers
            if(type.GetCustomAttribute(typeof(EndPointHandlerAttribute)) is not null &&
                Activator.CreateInstance(type, connectionString) is object handler)
            {
                _handlers.Add(type, handler);

                // map functions of each handler
                foreach(MethodInfo method in type.GetMethods())
                {
                    if(method.GetCustomAttribute(typeof(EndPointAttribute)) is EndPointAttribute attr)
                    {
                        switch(attr.Method)
                        {
                            case HttpMethods.GET:
                                _getEndpoints.Add(attr.Resource.Split('/', StringSplitOptions.RemoveEmptyEntries), method);
                                break;
                            case HttpMethods.POST:
                                _postEndpoints.Add(attr.Resource.Split('/', StringSplitOptions.RemoveEmptyEntries), method);
                                break;
                            case HttpMethods.PUT:
                                _putEndpoints.Add(attr.Resource.Split('/', StringSplitOptions.RemoveEmptyEntries), method);
                                break;
                            case HttpMethods.DELETE:
                                _delEndpoints.Add(attr.Resource.Split('/', StringSplitOptions.RemoveEmptyEntries), method);
                                break;
                        }
                    }
                }
            }
        }
    }

    public HttpResponse GetResponse(HttpRequest request)
    {
        if(request.Resource.Length < 1)
            return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid resource requested")));

        // get endpoint function
        MethodInfo? endpoint = null;

        switch(request.Method)
        {
            case HttpMethods.GET:
                endpoint = _getEndpoints.Where(x => x.Key.SequenceEqual(request.Resource))
                                        .Select(x => x.Value).FirstOrDefault();
                break;
            case HttpMethods.POST:
                endpoint = _postEndpoints.Where(x => x.Key.SequenceEqual(request.Resource))
                                        .Select(x => x.Value).FirstOrDefault();
                break;
            case HttpMethods.PUT:
                endpoint = _putEndpoints.Where(x => x.Key.SequenceEqual(request.Resource))
                                        .Select(x => x.Value).FirstOrDefault();
                break;
            case HttpMethods.DELETE:
                endpoint = _delEndpoints.Where(x => x.Key.SequenceEqual(request.Resource))
                                        .Select(x => x.Value).FirstOrDefault();
                break;
        }

        // call endpoint function with corresponding handler
        if(endpoint is not null && endpoint.DeclaringType is not null)
        {
                    object handler = _handlers[endpoint.DeclaringType];

                return (HttpResponse?)endpoint.Invoke(handler, [request]) ?? 
                    new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid resource requested")));
        }

        return new HttpResponse("400 Bad Request", JsonSerializer.Serialize(new Error("Invalid resource requested")));
    }
}