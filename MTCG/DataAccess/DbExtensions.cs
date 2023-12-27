using System.Data;

namespace SWEN.MTCG.DataAccess;

public static class DbExtensions
{
    public static void AddParameterWithValue(this IDbCommand command, string parameterName, DbType type, object value)
    {
        var parameter = command.CreateParameter();
        parameter.DbType = type;
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }
}