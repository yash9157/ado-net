using StoreApi.Contracts;

namespace StoreApi.Repositories;

public interface IProductStoredProcedureRepository
{
    Task<IReadOnlyList<ProductResponse>> SearchByMinimumPriceAsync(
        decimal minimumPrice,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);
}
