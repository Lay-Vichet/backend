using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Infrastructure.Repositories;

namespace SubscriptionTracker.Infrastructure.Dapper;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbTransactionScope _scope;
    private readonly IDbConnectionFactory _factory;

    public ISubscriptionRepository Subscriptions { get; }
    public ICategoryRepository Categories { get; }
    public IHouseholdRepository Households { get; }
    public IHouseholdMemberRepository HouseholdMembers { get; }
    public IPaymentMethodRepository PaymentMethods { get; }
    public ISharedSubscriptionRepository SharedSubscriptions { get; }
    public ISubscriptionCategoryRepository SubscriptionCategories { get; }
    public ISubscriptionPaymentRepository SubscriptionPayments { get; }
    public ISubscriptionRatingRepository SubscriptionRatings { get; }
    public ISubscriptionUsageRepository SubscriptionUsages { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(IDbTransactionScopeFactory txFactory, IDbConnectionFactory connectionFactory)
    {
        _factory = connectionFactory;
        _scope = txFactory.BeginTransaction();

        // create repositories bound to this scope
        Subscriptions = new DapperSubscriptionRepository(connectionFactory, _scope);
        Categories = new DapperCategoryRepository(connectionFactory, _scope);
        Households = new DapperHouseholdRepository(connectionFactory, _scope);
        HouseholdMembers = new DapperHouseholdMemberRepository(connectionFactory, _scope);
        PaymentMethods = new DapperPaymentMethodRepository(connectionFactory, _scope);
        SharedSubscriptions = new DapperSharedSubscriptionRepository(connectionFactory, _scope);
        SubscriptionCategories = new DapperSubscriptionCategoryRepository(connectionFactory, _scope);
        SubscriptionPayments = new DapperSubscriptionPaymentRepository(connectionFactory, _scope);
        SubscriptionRatings = new DapperSubscriptionRatingRepository(connectionFactory, _scope);
        SubscriptionUsages = new DapperSubscriptionUsageRepository(connectionFactory, _scope);
        Users = new DapperUserRepository(connectionFactory, _scope);
    }

    public Task CommitAsync()
    {
        _scope.Commit();
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        _scope.Rollback();
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return _scope.DisposeAsync();
    }
}
