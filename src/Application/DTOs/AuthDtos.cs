using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SubscriptionTracker.Application.DTOs
{
    public sealed class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }

    public sealed class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
    public sealed class AuthResponse
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
