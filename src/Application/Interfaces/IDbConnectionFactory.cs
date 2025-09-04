using System.Data;

namespace SubscriptionTracker.Application.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
