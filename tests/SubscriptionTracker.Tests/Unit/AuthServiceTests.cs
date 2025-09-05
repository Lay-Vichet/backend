using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace SubscriptionTracker.Tests.Unit;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_Succeeds_ReturnsToken()
    {
        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((SubscriptionTracker.Application.DTOs.UserDto?)null);
        usersMock.Setup(x => x.AddAsync(It.IsAny<SubscriptionTracker.Application.DTOs.UserDto>(), null)).Returns(Task.CompletedTask);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(x => x.Users).Returns(usersMock.Object);
        uowMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "01234567890123456789012345678901"), new KeyValuePair<string, string?>("JWT:ExpiresMinutes", "60") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        var res = await svc.RegisterAsync(new RegisterRequest { Email = "a@b.com", Password = "Password1!" });
        Assert.False(string.IsNullOrEmpty(res.Token));
    }

    [Fact]
    public async Task Register_DuplicateEmail_Throws()
    {
        var existing = new SubscriptionTracker.Application.DTOs.UserDto { UserId = Guid.NewGuid(), Email = "a@b.com", PasswordHash = "x", CreatedAt = DateTime.UtcNow };
        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(existing);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(x => x.Users).Returns(usersMock.Object);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "01234567890123456789012345678901") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await svc.RegisterAsync(new RegisterRequest { Email = "a@b.com", Password = "Password1!" }));
    }

    [Fact]
    public async Task Login_Succeeds_ReturnsToken()
    {
        var password = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var existing = new SubscriptionTracker.Application.DTOs.UserDto { UserId = Guid.NewGuid(), Email = "a@b.com", PasswordHash = hash, CreatedAt = DateTime.UtcNow };

        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(existing);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(x => x.Users).Returns(usersMock.Object);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "01234567890123456789012345678901") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        var res = await svc.LoginAsync(new LoginRequest { Email = "a@b.com", Password = password });
        Assert.False(string.IsNullOrEmpty(res.Token));
    }

    [Fact]
    public async Task Login_WrongPassword_Throws()
    {
        var password = "Password1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var existing = new SubscriptionTracker.Application.DTOs.UserDto { UserId = Guid.NewGuid(), Email = "a@b.com", PasswordHash = hash, CreatedAt = DateTime.UtcNow };

        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(existing);

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(x => x.Users).Returns(usersMock.Object);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "01234567890123456789012345678901") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await svc.LoginAsync(new LoginRequest { Email = "a@b.com", Password = "wrong" }));
    }
}
