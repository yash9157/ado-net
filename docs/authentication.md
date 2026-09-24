# 3. Register, log in and use JWT

Identity stores users and checks password hashes. After a successful login, this API signs a JWT. The client sends that token in the `Authorization` header.

## Request and response shapes

Registration accepts an email and password:

```csharp
using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);
```

Login accepts the same fields:

```csharp
using System.ComponentModel.DataAnnotations;

namespace StoreApi.Contracts;

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);
```

Login returns the token and its expiry:

```csharp
namespace StoreApi.Contracts;

public sealed record TokenResponse(string AccessToken, DateTime ExpiresUtc);
```

## Registration, login and current user

This complete controller creates users with `UserManager`, checks passwords, returns a 30 minute JWT, and reads the logged in user from the JWT claims:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Contracts;

namespace StoreApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<IdentityUser> users,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim()
        };

        IdentityResult result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(error => error.Description));

        return StatusCode(StatusCodes.Status201Created, new { user.Id, user.Email });
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(LoginRequest request)
    {
        IdentityUser? user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Unauthorized();

        if (await users.IsLockedOutAsync(user))
            return Unauthorized();

        if (!await users.CheckPasswordAsync(user, request.Password))
        {
            await users.AccessFailedAsync(user);
            return Unauthorized();
        }

        await users.ResetAccessFailedCountAsync(user);

        DateTime expiresUtc = DateTime.UtcNow.AddMinutes(30);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresUtc,
            signingCredentials: credentials);

        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresUtc));
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Id = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
            Email = User.FindFirstValue(JwtRegisteredClaimNames.Email)
        });
    }
}
```

## Register Identity and JWT in the API

The application registers the EF Core context, Identity user store, JWT validation and authorization. In Development it also applies pending migrations before accepting requests. This file also references the ADO.NET repository from step 5, so finish that step before building:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreApi.Data;
using StoreApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("StoreDb")
    ?? throw new InvalidOperationException(
        "Connection string 'StoreDb' is not configured.");
string jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 32 bytes.");
string issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
string audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentityCore<IdentityUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<StoreDbContext>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IProductStoredProcedureRepository,
    ProductStoredProcedureRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<StoreDbContext>()
        .Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

The JWT middleware validates the token signature, issuer, audience and expiry. The signing key in the Development settings is a demo value for local use.

The three auth routes are `POST /api/auth/register`, `POST /api/auth/login`, and `GET /api/auth/me`. The [final walkthrough](web-api-example.md#try-the-api) has the exact requests to run after the API starts.

**Next:** [Create the stored procedures](stored-procedures.md).
