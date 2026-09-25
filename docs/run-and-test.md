# 7. Run and test both projects

Run SQL Server, the API, and Angular separately. Vercel only hosts these documentation pages.

## 1. Prepare SQL Server

Start SQL Server on `localhost,1433`, create `EmployeeManagementDB`, run the [schema script](database.md), and insert a department. The API includes `appsettings.json`; make sure its SQL connection matches your local server. Its committed password and JWT key are public values, not production secrets.

## 2. Start the API

From the repository root:

```powershell
dotnet restore samples/EmployeeManagement.Api/EmployeeManagement.Api.csproj
dotnet run --project samples/EmployeeManagement.Api/EmployeeManagement.Api.csproj --launch-profile https
```

Open `https://localhost:7190/swagger`. If HTTPS is not trusted, run `dotnet dev-certs https --trust`. The Angular environment file uses this API URL.

## 3. Test API requests

In another PowerShell window, register and log in:

```powershell
$base = 'https://localhost:7190/api'
$account = @{ email = 'learner@example.com'; password = 'Example123!'; role = 'User' } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "$base/Auth/register" -ContentType 'application/json' -Body $account
$login = Invoke-RestMethod -Method Post -Uri "$base/Auth/login" -ContentType 'application/json' -Body $account
$headers = @{ Authorization = "Bearer $($login.token)" }
```

If the email is already registered, skip registration. Login returns `token`, `email`, `role`, and `expiration`.

List departments, then create an employee using an actual `departmentId`:

```powershell
$departments = Invoke-RestMethod "$base/Departments" -Headers $headers
$departments
$employee = @{
  firstName = 'Asha'; lastName = 'Patel'; email = 'asha@example.com'
  phone = '9876543210'; salary = 45000; dateOfBirth = '1998-05-20'
  joiningDate = '2026-09-25T09:00:00'; gender = 'Female'
  isActive = $true; departmentId = $departments[0].departmentId
} | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "$base/Employee" -Headers $headers -ContentType 'application/json' -Body $employee
$all = Invoke-RestMethod "$base/Employee" -Headers $headers
$all
```

Read, patch, and delete that employee:

```powershell
$id = ($all | Where-Object email -eq 'asha@example.com' | Select-Object -First 1).employeeId
Invoke-RestMethod "$base/Employee/$id" -Headers $headers
Invoke-RestMethod -Method Patch -Uri "$base/Employee/$id" -Headers $headers -ContentType 'application/json' -Body '{"salary":50000}'
Invoke-RestMethod -Method Delete -Uri "$base/Employee/$id" -Headers $headers
```

Write actions return text messages, not employee JSON. The current employee endpoints do **not** require the token despite these requests sending it. Registration also trusts a client-provided role; do not use that behavior in production.

## 4. Start Angular

In a third terminal from the repository root:

```powershell
cd samples/Employeemanagmnet
npm ci
npm start
```

Open `http://localhost:4200`. Visit Register, then Login, then use the list, Add, Edit, Delete, and Logout. If the department selector is empty, add a department in SQL. If API calls fail, check the API URL, HTTPS certificate, SQL connection, and CORS origin.

## Interview summary

`Angular → HTTP → ASP.NET controller → ADO.NET SqlCommand → SQL stored procedure`. `ExecuteReaderAsync` reads rows; `ExecuteNonQueryAsync` changes them. Passwords are hashed with `PasswordHasher<User>` and login returns a JWT. The current server authorization and secret handling need work before production.
