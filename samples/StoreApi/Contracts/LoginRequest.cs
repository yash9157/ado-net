using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
