using SWEN.MTCG.BusinessLogic.Attributes;

namespace SWEN.MTCG.BusinessLogic.EndpointHandlers;

[EndPointHandler]
public class UserHandler(string connectionString)
{
    private readonly string _connectionString = connectionString;


}