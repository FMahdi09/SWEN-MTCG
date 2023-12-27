using System.Data;

namespace SWEN.MTCG.DataAccess.Base;

public abstract class BaseRepository
{
    protected IDbConnection _connection;

    // constructor
    public BaseRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public IDataReader ExecuteQuery(IDbCommand command)
    {
        command.Connection = _connection;
        command.Prepare();

        return command.ExecuteReader();
    }

    public void ExecuteNonQuery(IDbCommand command) 
    {
        command.Connection = _connection;
        command.Prepare();

        command.ExecuteNonQuery();
    }
}