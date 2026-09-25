# 3. Register, log in, and return a JWT

The API stores users in its own `Users` table. It calls stored procedures through ADO.NET and uses `PasswordHasher<User>` to hash and verify passwords. This is **not** EF Core Identity persistence.

## User and request models

**File:** `Models/User.cs` — **Use:** Represents a row in the Users table.

<<< ../samples/EmployeeManagement.Api/Models/User.cs{csharp}

**File:** `Models/RegisterUserDto.cs` — **Use:** Receives registration data.

<<< ../samples/EmployeeManagement.Api/Models/RegisterUserDto.cs{csharp}

**File:** `Models/LoginDto.cs` — **Use:** Receives login credentials.

<<< ../samples/EmployeeManagement.Api/Models/LoginDto.cs{csharp}

Login returns a token, email, role, and expiration:

**File:** `Models/AuthResponseDto.cs` — **Use:** Returns the JWT and login details.

<<< ../samples/EmployeeManagement.Api/Models/AuthResponseDto.cs{csharp}

## Complete authentication controller

`POST /api/Auth/register` checks for an existing email, hashes the password, and calls `sp_RegisterUser`. `POST /api/Auth/login` reads the user through `sp_GetUserByEmail`, checks the password hash and active flag, then signs a JWT with user ID, email, and role claims.

**File:** `Controllers/AuthController.cs` — **Use:** Registers users and creates JWTs.

<<< ../samples/EmployeeManagement.Api/Controllers/AuthController.cs{csharp}

Registration uses `ExecuteNonQueryAsync`; login uses `ExecuteReaderAsync`. Registration currently returns HTTP 200 with a message; login returns JSON.

::: warning Important limitation
The registration DTO accepts `role` from the client, so a caller can request an elevated role. The API also does not apply `[Authorize]` to the employee/department controllers. Do not describe those endpoints as server-protected.
:::

**Next:** [Employee endpoints](api-employees.md).
