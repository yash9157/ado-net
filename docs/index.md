# Employee Management: .NET API + Angular

This is a step-by-step guide to the two Employee Management projects. The ASP.NET Core 8 API uses ADO.NET and SQL Server stored procedures. The Angular 21 app calls that API for registration, login, and employee management. The important source code appears on these pages, so an interview reader does not need to open the project folders.

## Follow the complete path

1. [Set up the API](api-setup.md) — packages, SQL connection, JWT, CORS.
2. [Create the database](database.md) — tables, procedures, first department.
3. [Register and log in](api-auth.md) — password hashing and JWT.
4. [Build employee endpoints](api-employees.md) — ADO.NET CRUD calls.
5. [Set up Angular](angular-setup.md) — API URL, routing, HTTP client.
6. [Build Angular screens](angular-flow.md) — auth, list, add, edit, delete.
7. [Run and test everything](run-and-test.md) — three processes and real requests.

`Angular form → HTTP service → ASP.NET controller → SqlCommand → stored procedure → SQL Server`

This API does **not** use EF Core, EF Core Identity stores, or automatic migrations. It uses a SQL setup script and ASP.NET Core's `PasswordHasher<User>`.

::: warning Security boundary
The supplied sample has a hard-coded SQL password and JWT key, accepts a role from the registration request, and does not put `[Authorize]` on employee endpoints. It is an interview learning sample, not a production-ready secure system. The Angular route guard is only client-side navigation protection.
:::

Vercel hosts this documentation website only. The API, SQL Server, and Angular app must run separately.
