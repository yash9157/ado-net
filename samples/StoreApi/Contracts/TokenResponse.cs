namespace StoreApi.Contracts;

public sealed record TokenResponse(string AccessToken, DateTime ExpiresUtc);
