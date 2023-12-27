using Npgsql;

namespace SWEN.MTCG.DataAccess.UnitOfWork;

internal class UnitOfWork : IDisposable
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction? _transaction;

    private bool _disposed = false;
    private bool _committed = false;
    
    // constructor
    public UnitOfWork(string connectionString, bool withTransaction)
    {            
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();

        if (withTransaction)
            _transaction = _connection.BeginTransaction();
    }

    // Commit
    public void Commit()
    {
        _transaction?.Commit();
        _committed = true;
    }

    // Disposing
    public void Dispose() 
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)  
    {
        if (!_disposed)
        {
            if(!_committed && _transaction != null)
            {
                _transaction.Rollback();
            }                
            
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }

            _disposed = true;
        }
    }
}