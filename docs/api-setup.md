# 1. Set up the ASP.NET Core API

The API targets .NET 8. It uses `Microsoft.Data.SqlClient` for ADO.NET, JWT bearer authentication, and Swashbuckle for Swagger.

## Create the project

```powershell
dotnet new webapi --use-controllers -f net8.0 -n EmployeeManagement.Api
cd EmployeeManagement.Api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package Microsoft.Data.SqlClient --version 7.1.0
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

Remove template WeatherForecast files if present. Add the `Models`, `DTO`, and `Controllers` classes shown on later pages. The project file is:

<<< ../samples/EmployeeManagement.Api/EmployeeManagement.Api.csproj{xml}

## Configure SQL Server and JWT

The code reads `ConnectionStrings:DefaultConnection` and the `Jwt` settings. The supplied API includes `appsettings.json`. Its values are public in this repository; replace them before connecting to any real database or deploying the API. A safe configuration template is:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=EmployeeManagementDB;User Id=sa;Password=YOUR_LOCAL_SQL_PASSWORD;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_PRIVATE_RANDOM_KEY_AT_LEAST_32_BYTES",
    "Issuer": "EmployeeManagementApi",
    "Audience": "EmployeeManagementUI",
    "ExpiryMinutes": 60
  },
  "AllowedHosts": "*"
}
```

For a real deployment, use .NET user secrets or environment variables for credentials. Do not rely on the publicly committed settings as secrets.

## Register middleware and Swagger

This is the complete `Program.cs`. It configures JWT validation, permits the Angular origin `http://localhost:4200`, enables Swagger in Development, and maps controllers. Authentication runs before authorization:

<<< ../samples/EmployeeManagement.Api/Program.cs{csharp}

The API's HTTPS launch profile listens at `https://localhost:7190`, matching the Angular environment file. The HTTP profile listens at `http://localhost:5024`.

**Next:** [Create the database](database.md).
