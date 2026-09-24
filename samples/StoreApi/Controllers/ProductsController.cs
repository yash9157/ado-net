using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Contracts;
using StoreApi.Repositories;

namespace StoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController(IProductStoredProcedureRepository products)
    : ControllerBase
{
    // GET /api/products?minimumPrice=1000 calls dbo.Products_SearchByMinimumPrice.
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> Get(
        [FromQuery, Range(0, 99999999.99)] decimal minimumPrice = 0,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ProductResponse> results =
            await products.SearchByMinimumPriceAsync(minimumPrice, cancellationToken);

        return Ok(results);
    }

    // POST /api/products calls dbo.Products_Create and returns its output ID.
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        int productId = await products.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new { productId });
    }
}
