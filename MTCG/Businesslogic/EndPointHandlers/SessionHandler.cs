using SWEN.MTCG.BusinessLogic.Attributes;

namespace SWEN.MTCG.BusinessLogic.EndpointHandlers;

[EndPointHandler]
public class SessionHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;
}