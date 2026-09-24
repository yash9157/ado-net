# 6. Add endpoints and try the API

This page puts the pieces together: EF Core Identity handles users and login; EF Core migrations create the database objects; ADO.NET calls the product stored procedures. Both product routes require a JWT.

## Product requests and responses

The create request validates a name and nonnegative price:

```csharp
using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record CreateProductRequest(
    [Required, StringLength(100)] string Name,
    [Range(0, 99999999.99)] decimal Price);
```

The search route returns this response model:

```csharp
namespace StoreApi.Contracts;

public sealed record ProductResponse(
    int ProductId,
    string Name,
    decimal Price,
    DateTime CreatedUtc);
```

## Complete product controller

The controller calls the [ADO.NET repository](ado-net-methods.md) for both product actions. `[Authorize]` requires the JWT before either action runs.

```csharp
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
```

## Generate migrations and start the API {#try-the-api}

Make sure every class from steps 1–6 is in place. From inside your `StoreApi` project, run:

```powershell
dotnet ef migrations add InitialStore --output-dir Data/Migrations
dotnet ef migrations add ProductProcedures --output-dir Data/Migrations
```

The first migration generates the Identity and Products tables. The second migration is empty until you paste in the `Up` and `Down` code from [step 4](stored-procedures.md). Then start the API:

```powershell
sqllocaldb start AdoNetInterview
dotnet run --urls http://localhost:5000
```

Keep the API terminal open and use another PowerShell window for the requests below. These requests go to your local .NET API, **not** to the Vercel documentation site.

## Register and get a JWT

```powershell
$body = @{ email = 'you@example.com'; password = 'Example123!' } | ConvertTo-Json

Invoke-RestMethod -Method Post `
    -Uri http://localhost:5000/api/auth/register `
    -ContentType 'application/json' -Body $body

$login = Invoke-RestMethod -Method Post `
    -Uri http://localhost:5000/api/auth/login `
    -ContentType 'application/json' -Body $body

$headers = @{ Authorization = "Bearer $($login.accessToken)" }
```

If you already registered this email, skip the register call and run only login. Check the current user:

```powershell
Invoke-RestMethod http://localhost:5000/api/auth/me -Headers $headers
```

Expected result includes `id` and `email`. Without the token, the endpoint returns HTTP 401.

## Insert through ADO.NET and a stored procedure

```powershell
$product = @{ name = 'Keyboard'; price = 1499.00 } | ConvertTo-Json

$created = Invoke-RestMethod -Method Post `
    -Uri http://localhost:5000/api/products `
    -Headers $headers -ContentType 'application/json' -Body $product

$created
```

Expected result: HTTP 201 with a `productId`, for example `{ "productId": 1 }`. The procedure writes the row and returns the ID through `@ProductId OUTPUT`. The ID may be different on your machine.

## Read through ADO.NET and a stored procedure

```powershell
Invoke-RestMethod 'http://localhost:5000/api/products?minimumPrice=1000' -Headers $headers
```

The controller calls `SearchByMinimumPriceAsync`, which opens `SqlConnection`, executes `dbo.Products_SearchByMinimumPrice` with `SqlCommand`, reads each row with `SqlDataReader`, and returns JSON. Omit `minimumPrice` to return all products (default `0`). Without a JWT, these product routes return HTTP 401.

## Verify the database result (optional)

```powershell
sqlcmd -S '(localdb)\AdoNetInterview' -E -d StoreInterviewDb `
    -Q 'SELECT ProductId, Name, Price FROM dbo.Products'
```

This is the complete flow: register → login → send JWT → call product controller → use ADO.NET → execute SQL Server procedure → return an HTTP response. EF Core still owns Identity and creates the schema and procedures through migrations.

## If something fails

| Result | Check |
|---|---|
| Connection error | Start LocalDB with `sqllocaldb start AdoNetInterview`; check the connection string. |
| Procedure not found | Confirm the API started in Development and the `ProductProcedures` migration was applied. |
| HTTP 401 | Log in again and send `Authorization: Bearer <token>`; tokens expire after 30 minutes. |
| HTTP 400 | Check that the name is not blank and prices are between `0` and `99999999.99`. |
| Register returns an error for the same email | The user already exists; run only the login request. |
