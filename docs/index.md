# ADO.NET + stored procedures in a .NET API

This website is the complete interview example. You do not need a separate source repository to read it: the commands, C# classes, stored procedure migration, and HTTP requests are all on these pages.

You will build one small Store API. **EF Core Identity** manages users and passwords. Login returns a **JWT**. Protected product endpoints use **ADO.NET** to call two SQL Server stored procedures. **EF Core migrations** create the tables and procedures.

## Build it in order

1. [Create the API and configure SQL Server](setup.md)
2. [Add the Product model and EF Core Identity](ef-core-identity.md)
3. [Add registration, login, and JWT](authentication.md)
4. [Prepare the stored procedure migration](stored-procedures.md)
5. [Call the procedures using ADO.NET](ado-net-methods.md)
6. [Add product endpoints and test the flow](web-api-example.md)

Start with [.NET 10 and SQL Server LocalDB](setup.md). Copy each displayed code block into the project as you go. The [final page](web-api-example.md#try-the-api) generates migrations, starts the API, and tests the whole flow.

## Say it in an interview

“Identity stores users in SQL Server and checks password hashes. Login returns a JWT. Protected product endpoints call a reusable ADO.NET repository, which uses typed `SqlParameter` values to execute stored procedures. EF Core migrations create the database objects; product requests do not use EF Core.”

| Request | What happens |
|---|---|
| `POST /api/auth/register` | Identity creates a user |
| `POST /api/auth/login` | Identity checks the password and returns a JWT |
| `POST /api/products` | ADO.NET calls `dbo.Products_Create` and reads its output ID |
| `GET /api/products?minimumPrice=1000` | ADO.NET calls `dbo.Products_SearchByMinimumPrice` and reads rows |

Vercel hosts this documentation website. The .NET API and SQL Server run on your machine while you practice.
