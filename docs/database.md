# 2. Create the database and stored procedures

This project uses SQL scripts, **not EF Core migrations**. Create the database, run the schema script, then add at least one department for the employee form.

## Create the database first

Start SQL Server on `localhost,1433`. In SQL Server Management Studio, connect to it and run:

```sql
CREATE DATABASE EmployeeManagementDB;
GO
```

Then execute the following script against that server. It starts with `USE [EmployeeManagementDB]`, so the database must already exist. The script is complete but intended for a **fresh** database; running its `CREATE TABLE` statements twice will fail.

**File:** `samples/script.sql` — **Use:** Creates tables and stored procedures.

<<< ../samples/script.sql{sql}

It creates `Departments`, `Employees`, `Users`, and eight procedures:

| Procedure | Purpose |
|---|---|
| `sp_GetDepartments` | Fill the department dropdown |
| `sp_GetEmployee`, `sp_GetEmployeeById` | List and edit employees |
| `sp_InsertEmployee`, `sp_UpdateEmployee`, `sp_DeleteEmployee` | Change employees |
| `sp_GetUserByEmail`, `sp_RegisterUser` | Register and log in |

## Add a department

The script creates no department records. Insert one before trying to add an employee:

```sql
INSERT INTO dbo.Departments (DepartmentName) VALUES (N'Engineering');
SELECT DepartmentId, DepartmentName FROM dbo.Departments;
```

Check that procedures exist:

```sql
SELECT name FROM sys.procedures WHERE name LIKE 'sp_%' ORDER BY name;
```

The controllers create `SqlCommand` objects with `CommandType.StoredProcedure`. Read actions use `ExecuteReaderAsync`; write actions use `ExecuteNonQueryAsync`. The update procedure uses `COALESCE`, so `null` means “keep the existing value”—it cannot clear a column to SQL `NULL`. The write procedures also omit `SET NOCOUNT ON`, because the controllers inspect affected-row counts.

**Next:** [Register and log in](api-auth.md).
