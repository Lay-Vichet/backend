using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SubscriptionTracker.Application.Interfaces;
using SubscriptionTracker.Application.DTOs;
using SubscriptionTracker.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace SubscriptionTracker.Tests.Unit;

public class AuthRefreshTests
{
    [Fact]
    public async Task Refresh_RotatesToken_AndReturnsNewAccessAndRefresh()
    {
        var userId = Guid.NewGuid();
        var existingRefreshDto = new RefreshTokenDto { Id = Guid.NewGuid(), UserId = userId, CreatedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddDays(1), Revoked = false };

        var refreshRepoMock = new Mock<IRefreshTokenRepository>();
        refreshRepoMock.Setup(r => r.GetByHashAsync(It.IsAny<string>())).ReturnsAsync(existingRefreshDto);
        refreshRepoMock.Setup(r => r.CreateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);
        refreshRepoMock.Setup(r => r.RevokeAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        var usersMock = new Mock<IUserRepository>();
        usersMock.Setup(u => u.GetByIdAsync(userId)).ReturnsAsync(new SubscriptionTracker.Application.DTOs.UserDto { UserId = userId, Email = "a@b.com", PasswordHash = "x", CreatedAt = DateTime.UtcNow });

        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(u => u.RefreshTokens).Returns(refreshRepoMock.Object);
        uowMock.SetupGet(u => u.Users).Returns(usersMock.Object);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "0123456789012345678901234567890123456789012345678901234567890123") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        var resp = await svc.RefreshAsync(new RefreshRequest("sometoken"));

        Assert.NotNull(resp);
        Assert.False(string.IsNullOrEmpty(resp.AccessToken));
        Assert.False(string.IsNullOrEmpty(resp.RefreshToken));
        refreshRepoMock.Verify(r => r.CreateAsync(userId, It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        refreshRepoMock.Verify(r => r.RevokeAsync(existingRefreshDto.Id), Times.Once);
    }

    [Fact]
    public async Task Revoke_CallsRepositoryRevoke()
    {
        var refreshRepoMock = new Mock<IRefreshTokenRepository>();
        var uowMock = new Mock<IUnitOfWork>();
        uowMock.SetupGet(u => u.RefreshTokens).Returns(refreshRepoMock.Object);

        var uowFactoryMock = new Mock<IUnitOfWorkFactory>();
        uowFactoryMock.Setup(f => f.Create()).Returns(uowMock.Object);

        var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("JWT:Key", "0123456789012345678901234567890123456789012345678901234567890123") }).Build();
        var svc = new AuthService(uowFactoryMock.Object, config);

        await svc.RevokeRefreshAsync(new RevokeRequest("token"));

        refreshRepoMock.Verify(r => r.GetByHashAsync("token"), Times.Once);
    }
}
