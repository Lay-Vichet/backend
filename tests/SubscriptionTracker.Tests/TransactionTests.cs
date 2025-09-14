using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Application.Services;
using Xunit;

namespace SubscriptionTracker.Tests
{
    public class TransactionTests
    {
        class TestDbConnection : DbConnection
        {
            public override string ConnectionString { get; set; }
            public override string Database => "TestDb";
            public override string DataSource => "Test";
            public override string ServerVersion => "1.0";
            public override ConnectionState State => ConnectionState.Open;
            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => null!;
            public override void Close() { }
            public override void ChangeDatabase(string databaseName) { }
            public override void Open() { }
            protected override DbCommand CreateDbCommand() => null!;
        }

        class TestDbTransaction : IDbTransaction
        {
            public IDbConnection Connection => null!;
            public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
            public void Commit() { }
            public void Dispose() { }
            public void Rollback() { }
        }
        // A minimal fake unit-of-work that exposes the fake repositories and records commit/rollback/dispose
        class FakeUnitOfWork : IUnitOfWork
        {
            public FakeUnitOfWork(FakePaymentRepo payRepo, FakeSubscriptionRepo subRepo)
            {
                SubscriptionPayments = payRepo;
                Subscriptions = subRepo;
            }

            public ISubscriptionRepository Subscriptions { get; }
            public ICategoryRepository Categories => throw new NotImplementedException();
            public IHouseholdRepository Households => throw new NotImplementedException();
            public IHouseholdMemberRepository HouseholdMembers => throw new NotImplementedException();
            public IPaymentMethodRepository PaymentMethods => throw new NotImplementedException();
            public ISharedSubscriptionRepository SharedSubscriptions => throw new NotImplementedException();
            public ISubscriptionCategoryRepository SubscriptionCategories => throw new NotImplementedException();
            public ISubscriptionPaymentRepository SubscriptionPayments { get; }
            public ISubscriptionRatingRepository SubscriptionRatings => throw new NotImplementedException();
            public ISubscriptionUsageRepository SubscriptionUsages => throw new NotImplementedException();
            public IUserRepository Users => throw new NotImplementedException();

            public bool Committed { get; private set; }
            public bool RolledBack { get; private set; }
            public bool Disposed { get; private set; }

            public Task CommitAsync()
            {
                Committed = true;
                return Task.CompletedTask;
            }

            public Task RollbackAsync()
            {
                RolledBack = true;
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync()
            {
                Disposed = true;
                return ValueTask.CompletedTask;
            }
        }

        // Simple factory wrapper that returns a non-disposing adapter around the provided IUnitOfWork instance.
        // This preserves the test's expectation that the underlying FakeUnitOfWork isn't disposed by callers.
        class FakeUnitOfWorkFactory : IUnitOfWorkFactory
        {
            private readonly IUnitOfWork _uow;
            public FakeUnitOfWorkFactory(IUnitOfWork uow) => _uow = uow;
            public IUnitOfWork Create() => new NonDisposingUnitOfWork(_uow);

            private class NonDisposingUnitOfWork : IUnitOfWork
            {
                private readonly IUnitOfWork _inner;
                public NonDisposingUnitOfWork(IUnitOfWork inner) => _inner = inner;

                public ISubscriptionRepository Subscriptions => _inner.Subscriptions;
                public ICategoryRepository Categories => _inner.Categories;
                public IHouseholdRepository Households => _inner.Households;
                public IHouseholdMemberRepository HouseholdMembers => _inner.HouseholdMembers;
                public IPaymentMethodRepository PaymentMethods => _inner.PaymentMethods;
                public ISharedSubscriptionRepository SharedSubscriptions => _inner.SharedSubscriptions;
                public ISubscriptionCategoryRepository SubscriptionCategories => _inner.SubscriptionCategories;
                public ISubscriptionPaymentRepository SubscriptionPayments => _inner.SubscriptionPayments;
                public ISubscriptionRatingRepository SubscriptionRatings => _inner.SubscriptionRatings;
                public ISubscriptionUsageRepository SubscriptionUsages => _inner.SubscriptionUsages;
                public IUserRepository Users => _inner.Users;

                public Task CommitAsync() => _inner.CommitAsync();
                public Task RollbackAsync() => _inner.RollbackAsync();
                public ValueTask DisposeAsync() => ValueTask.CompletedTask; // intentionally no-op
            }
        }

        class FakePaymentRepo : ISubscriptionPaymentRepository
        {
            public bool Added { get; private set; }
            public Task AddAsync(SubscriptionPaymentDto dto, IDbTransactionScope? scope = null)
            {
                Added = true;
                return Task.CompletedTask;
            }
            public Task DeleteAsync(Guid id, IDbTransactionScope? scope = null) => Task.CompletedTask;
            public Task<IEnumerable<SubscriptionPaymentDto>> GetAllAsync() => Task.FromResult<IEnumerable<SubscriptionPaymentDto>>(Array.Empty<SubscriptionPaymentDto>());
            public Task<SubscriptionPaymentDto?> GetByIdAsync(Guid id) => Task.FromResult<SubscriptionPaymentDto?>(null);
            public Task UpdateAsync(SubscriptionPaymentDto dto, IDbTransactionScope? scope = null) => Task.CompletedTask;
        }

        class FakeSubscriptionRepo : ISubscriptionRepository
        {
            public bool Updated { get; private set; }
            public bool ThrowOnUpdate { get; set; }
            public Task AddAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null) => Task.CompletedTask;
            public Task DeleteAsync(Guid id, Guid userId, IDbTransactionScope? scope = null) => Task.CompletedTask;
            public Task<IEnumerable<SubscriptionDto>> GetAllAsync(Guid userId) => Task.FromResult<IEnumerable<SubscriptionDto>>(Array.Empty<SubscriptionDto>());
            public Task<SubscriptionDto?> GetByIdAsync(Guid id, Guid userId) => Task.FromResult<SubscriptionDto?>(null);
            public Task UpdateAsync(SubscriptionDto subscription, IDbTransactionScope? scope = null)
            {
                if (ThrowOnUpdate) throw new InvalidOperationException("update failed");
                Updated = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task AddPaymentAndUpdate_Commits_OnSuccess()
        {
            var payRepo = new FakePaymentRepo();
            var subRepo = new FakeSubscriptionRepo();
            var uow = new FakeUnitOfWork(payRepo, subRepo);

            var svc = new SubscriptionPaymentService(new FakeUnitOfWorkFactory(uow));

            var payment = new SubscriptionPaymentDto { PaymentId = Guid.NewGuid(), SubscriptionId = Guid.NewGuid(), Amount = 10m, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
            var sub = new SubscriptionDto { Id = payment.SubscriptionId, Name = "s", Cost = 1m, Currency = "USD", BillingCycle = "Monthly", StartDate = DateTime.UtcNow };

            await svc.AddPaymentAndUpdateSubscriptionAsync(payment, sub);

            Assert.True(payRepo.Added);
            Assert.True(subRepo.Updated);
            Assert.True(uow.Committed);
            Assert.False(uow.RolledBack);
        }

        [Fact]
        public async Task AddPaymentAndUpdate_DoesNotCommit_OnFailure()
        {
            var payRepo = new FakePaymentRepo();
            var subRepo = new FakeSubscriptionRepo { ThrowOnUpdate = true };
            var uow = new FakeUnitOfWork(payRepo, subRepo);

            var svc = new SubscriptionPaymentService(new FakeUnitOfWorkFactory(uow));

            var payment = new SubscriptionPaymentDto { PaymentId = Guid.NewGuid(), SubscriptionId = Guid.NewGuid(), Amount = 10m, PaymentDate = DateTime.UtcNow, CreatedAt = DateTime.UtcNow };
            var sub = new SubscriptionDto { Id = payment.SubscriptionId, Name = "s", Cost = 1m, Currency = "USD", BillingCycle = "Monthly", StartDate = DateTime.UtcNow };

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.AddPaymentAndUpdateSubscriptionAsync(payment, sub));

            Assert.True(payRepo.Added);
            Assert.False(uow.Committed);
            Assert.True(uow.RolledBack);
            Assert.False(uow.Disposed);
        }
    }
}
