using System.Data;

namespace SubscriptionTracker.Application.Interfaces;

public interface IDbTransactionScopeFactory
{
    IDbTransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
}
