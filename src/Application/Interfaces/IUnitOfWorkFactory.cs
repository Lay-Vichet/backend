using System.Threading.Tasks;

namespace SubscriptionTracker.Application.Interfaces;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
