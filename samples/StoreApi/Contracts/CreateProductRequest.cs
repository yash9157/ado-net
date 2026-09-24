using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record CreateProductRequest(
    [Required, StringLength(100)] string Name,
    [Range(0, 99999999.99)] decimal Price);
