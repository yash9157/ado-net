namespace StoreApi.Contracts;

public sealed record ProductResponse(
    int ProductId,
    string Name,
    decimal Price,
    DateTime CreatedUtc);
