using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Infrastructure.Dapper;

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbTransactionScopeFactory _txFactory;
    private readonly IDbConnectionFactory _connectionFactory;

    public UnitOfWorkFactory(IDbTransactionScopeFactory txFactory, IDbConnectionFactory connectionFactory)
    {
        _txFactory = txFactory;
        _connectionFactory = connectionFactory;
    }

    public IUnitOfWork Create()
    {
        // UnitOfWork constructor will create a transaction scope bound to the connection factory
        return new UnitOfWork(_txFactory, _connectionFactory);
    }
}
