using Npgsql;
using SWEN.MTCG.DataAccess.Repositories;

namespace SWEN.MTCG.DataAccess.UnitOfWork;

internal class UnitOfWork : IDisposable
{
    private readonly NpgsqlConnection _connection;
    private readonly NpgsqlTransaction? _transaction;

    private bool _disposed = false;
    private bool _committed = false;
    
    // repositories

    private UserRepository? _userRepository;

    public UserRepository UserRepository
    {
        get
        {
            _userRepository ??= new(_connection);
            return _userRepository;
        }
    }

    private TokenRepository? _tokenRepository;

    public TokenRepository TokenRepository
    {
        get
        {
            _tokenRepository ??= new(_connection);
            return _tokenRepository;
        }
    }

    private CardRepository? _cardRepository;

    public CardRepository CardRepository
    {
        get
        {
            _cardRepository ??= new(_connection);
            return _cardRepository;
        }
    }

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