namespace SubscriptionTracker.Application.DTOs
{
    public record RefreshRequest(string RefreshToken);

    public record RefreshResponse(string AccessToken, string RefreshToken, int ExpiresInSeconds);

    public record RevokeRequest(string RefreshToken);

    public class RefreshTokenDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
    }
}
