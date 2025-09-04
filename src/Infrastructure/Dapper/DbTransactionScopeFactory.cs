using System.Data;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Infrastructure.Dapper;

public class DbTransactionScopeFactory : Application.Interfaces.IDbTransactionScopeFactory
{
    private readonly IDbConnectionFactory _factory;

    public DbTransactionScopeFactory(IDbConnectionFactory factory)
    {
        _factory = factory;
    }

    public IDbTransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        return new DbTransactionScope(_factory, isolationLevel);
    }
}
