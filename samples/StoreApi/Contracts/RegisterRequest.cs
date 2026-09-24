using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);
