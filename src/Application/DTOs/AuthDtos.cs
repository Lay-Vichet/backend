using System;
using System.ComponentModel.DataAnnotations;

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
        public string Token { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
    }
}
