using SWEN.HttpServer.Enums;

namespace SWEN.MTCG.BusinessLogic.Attributes;

[AttributeUsage(AttributeTargets.Method)]
internal class EndPointAttribute : Attribute
{
    public HttpMethods Method { get; set; }
    public string Resource { get; set; }  

    public EndPointAttribute(HttpMethods method, string resource)
         => (Method, Resource) = (method, resource);
}