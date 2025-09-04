using System.Threading.Tasks;

namespace SubscriptionTracker.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    ISubscriptionRepository Subscriptions { get; }
    ICategoryRepository Categories { get; }
    IHouseholdRepository Households { get; }
    IHouseholdMemberRepository HouseholdMembers { get; }
    IPaymentMethodRepository PaymentMethods { get; }
    ISharedSubscriptionRepository SharedSubscriptions { get; }
    ISubscriptionCategoryRepository SubscriptionCategories { get; }
    ISubscriptionPaymentRepository SubscriptionPayments { get; }
    ISubscriptionRatingRepository SubscriptionRatings { get; }
    ISubscriptionUsageRepository SubscriptionUsages { get; }
    IUserRepository Users { get; }

    Task CommitAsync();
    Task RollbackAsync();
}
