# 4. Employee and department API

These controllers call stored procedures directly. They do not use EF Core or a shared repository. Each action creates its own `SqlConnection` and `SqlCommand` from the configured connection string.

## Models and write requests

`Employee` contains the database fields plus `DepartmentName` from the join. `CreateEmployeeDto` sends a full record; `UpdateEmployeeDto` is partial.

**File:** `Models/Employee.cs` — **Use:** Represents an employee returned by the API.

<<< ../samples/EmployeeManagement.Api/Models/Employee.cs{csharp}

**File:** `DTO/CreateEmployee.cs` — **Use:** Receives data for a new employee.

<<< ../samples/EmployeeManagement.Api/DTO/CreateEmployee.cs{csharp}

**File:** `DTO/UpdateEmployeeDto.cs` — **Use:** Receives changed employee fields.

<<< ../samples/EmployeeManagement.Api/DTO/UpdateEmployeeDto.cs{csharp}

Send `DateOnly` values as `"1998-05-20"` and `DateTime` values as `"2026-09-25T09:00:00"`.

## Departments

`GET /api/Departments` runs `sp_GetDepartments` and reads its rows for the Angular dropdown:

**File:** `Models/Department.cs` — **Use:** Represents a department option.

<<< ../samples/EmployeeManagement.Api/Models/Department.cs{csharp}

**File:** `Controllers/DepartmentsController.cs` — **Use:** Reads departments for the dropdown.

<<< ../samples/EmployeeManagement.Api/Controllers/DepartmentsController.cs{csharp}

## Employee CRUD

The routes are `GET /api/Employee`, `GET /api/Employee/{id}`, `POST /api/Employee`, `PATCH /api/Employee/{id}`, and `DELETE /api/Employee/{id}`. The full controller shows parameter binding, `DBNull.Value` handling, result mapping, and affected-row checks:

**File:** `Controllers/EmployeeController.cs` — **Use:** Runs employee CRUD stored procedures.

<<< ../samples/EmployeeManagement.Api/Controllers/EmployeeController.cs{csharp}

`AddWithValue` lets SQL Server infer types and sizes. For production code, define `SqlDbType`, length, precision, and scale explicitly and validate the request on the server. `DepartmentName` comes from an `INNER JOIN`, so an employee without a valid department will not appear in read results.

**Next:** [Set up Angular](angular-setup.md).
