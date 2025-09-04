using System.Data;
using SubscriptionTracker.Application.Interfaces;

namespace SubscriptionTracker.Infrastructure.Dapper;

public sealed class DbTransactionScope : IDbTransactionScope
{
    public IDbConnection Connection { get; }
    public IDbTransaction Transaction { get; }

    internal DbTransactionScope(IDbConnectionFactory factory, IsolationLevel isolationLevel)
    {
        Connection = factory.CreateConnection();
        if (Connection.State != ConnectionState.Open) Connection.Open();
        Transaction = Connection.BeginTransaction(isolationLevel);
    }

    public void Commit() => Transaction.Commit();
    public void Rollback() => Transaction.Rollback();

    public void Dispose()
    {
        try
        {
            Transaction?.Dispose();
        }
        finally
        {
            Connection?.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        // No true async dispose on IDbConnection/IDbTransaction in ADO.NET
        // Dispose synchronously to ensure resources are released.
        Dispose();
        return ValueTask.CompletedTask;
    }
}

