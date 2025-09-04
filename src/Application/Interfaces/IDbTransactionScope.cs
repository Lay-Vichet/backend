using System.Data;

namespace SubscriptionTracker.Application.Interfaces;

public interface IDbTransactionScope : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    void Commit();
    void Rollback();
}
