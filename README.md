# ADO.NET + stored procedures + Identity/JWT

A small working ASP.NET Core API and a step-by-step documentation site.

## Run the API

Requires .NET 10 and SQL Server LocalDB on Windows:

```powershell
sqllocaldb create AdoNetInterview
sqllocaldb start AdoNetInterview
dotnet run --project samples/StoreApi/StoreApi.csproj --urls http://localhost:5000
```

The API applies EF Core migrations on startup in Development. They create the database, Identity tables, Products table, and stored procedures.

## Read the guide

```powershell
npm install
npm run docs:dev
```

Open the URL printed by VitePress and follow the guide. All code needed to build the API is shown directly in the documentation; the [sample project](samples/StoreApi) is only a local way to verify it.

## What the API shows

- Register and login with EF Core Identity; login returns a JWT.
- Protect product routes with `[Authorize]`.
- Use ADO.NET to call stored procedures for product search and insert with `@ProductId OUTPUT`.
- Use EF Core for Identity and migrations, not product requests.
- Create or update procedures through an EF Core migration.

The configured JWT key is a local Development example. Use a private `Jwt__Key` of at least 32 bytes for other environments.
