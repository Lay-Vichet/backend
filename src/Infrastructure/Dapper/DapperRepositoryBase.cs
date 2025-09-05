using System.Data;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Infrastructure.Dapper;

public abstract class DapperRepositoryBase
{
    private readonly IDbConnectionFactory _db;
    private readonly IDbTransactionScope? _scope;

    protected DapperRepositoryBase(IDbConnectionFactory db)
    {
        _db = db;
    }

    // Constructor that binds this repository to an existing transaction scope (used by UnitOfWork)
    protected DapperRepositoryBase(IDbConnectionFactory db, IDbTransactionScope? scope)
    {
        _db = db;
        _scope = scope;
    }

    protected IDbConnection CreateConnection()
    {
        var c = _db.CreateConnection();
        if (c.State != ConnectionState.Open) c.Open();
        return c;
    }

    /// <summary>
    /// Begin a transaction scope which exposes Connection and Transaction and supports commit/rollback.
    /// Caller is responsible for disposing the returned scope.
    /// </summary>
    protected DbTransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return new DbTransactionScope(_db, isolationLevel);
    }

    // Expose a repository-level scope when created by UnitOfWork
    protected IDbTransactionScope? RepositoryTransactionScope => _scope;
}
