using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using EmployeeManagement.Api.DTO;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly string _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using SqlConnection con = new SqlConnection(_connectionString);

            using SqlCommand cmd = new SqlCommand("sp_GetEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Employee employee = new Employee
                {
                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),

                    FirstName = reader["FirstName"].ToString()!,

                    LastName = reader["LastName"].ToString()!,

                    Email = reader["Email"].ToString()!,

                    Phone = reader["Phone"] == DBNull.Value
                ? null
                : reader["Phone"].ToString(),

                    Salary = reader["Salary"] == DBNull.Value
                ? null
                : Convert.ToDecimal(reader["Salary"]),

                    DateOfBirth = reader["DateOfBirth"] == DBNull.Value
    ? null
    : DateOnly.FromDateTime(Convert.ToDateTime(reader["DateOfBirth"])),

                    JoiningDate = reader["JoiningDate"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["JoiningDate"]),

                    Gender = reader["Gender"] == DBNull.Value
                ? null
                : reader["Gender"].ToString(),

                    IsActive = Convert.ToBoolean(reader["IsActive"]),

                    DepartmentId =
                Convert.ToInt32(reader["DepartmentId"]),

                    DepartmentName =
                reader["DepartmentName"].ToString()
                };

                employees.Add(employee);
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            Employee? employee = null;

            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetEmployeeById", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", id);

            await con.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                employee = new Employee
                {
                    EmployeeId =
                        Convert.ToInt32(reader["EmployeeId"]),

                    FirstName =
                        reader["FirstName"].ToString()!,

                    LastName =
                        reader["LastName"].ToString()!,

                    Email =
                        reader["Email"].ToString()!,

                    Phone = reader["Phone"] == DBNull.Value
                        ? null
                        : reader["Phone"].ToString(),

                    Salary = reader["Salary"] == DBNull.Value
                        ? null
                        : Convert.ToDecimal(reader["Salary"]),

                    DateOfBirth = reader["DateOfBirth"] == DBNull.Value
    ? null
    : DateOnly.FromDateTime(
        Convert.ToDateTime(reader["DateOfBirth"])),

                    JoiningDate = reader["JoiningDate"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["JoiningDate"]),

                    Gender = reader["Gender"] == DBNull.Value
                        ? null
                        : reader["Gender"].ToString(),

                    IsActive =
                        Convert.ToBoolean(reader["IsActive"]),

                    DepartmentId =
                        Convert.ToInt32(reader["DepartmentId"]),

                    DepartmentName =
                        reader["DepartmentName"].ToString()
                };
            }

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto employee)
        {
            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_InsertEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@LastName", employee.LastName);
            cmd.Parameters.AddWithValue("@Email", employee.Email);

            cmd.Parameters.AddWithValue(
                "@Phone",
                (object?)employee.Phone ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Salary",
                (object?)employee.Salary ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@DateOfBirth",
                (object?)employee.DateOfBirth ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@JoiningDate",
                (object?)employee.JoiningDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Gender",
                (object?)employee.Gender ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);

            cmd.Parameters.AddWithValue(
                "@DepartmentId",
                employee.DepartmentId);

            await con.OpenAsync();

            int rowsAffected =
                await cmd.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                return Ok("Employee created successfully.");
            }

            return BadRequest("Employee could not be created.");
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateEmployee(
     int id,
     UpdateEmployeeDto employee)
        {
            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_UpdateEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", id);

            cmd.Parameters.AddWithValue(
                "@FirstName",
                (object?)employee.FirstName ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@LastName",
                (object?)employee.LastName ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Email",
                (object?)employee.Email ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Phone",
                (object?)employee.Phone ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Salary",
                (object?)employee.Salary ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@DateOfBirth",
                employee.DateOfBirth.HasValue
                    ? employee.DateOfBirth.Value
                        .ToDateTime(TimeOnly.MinValue)
                    : DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@JoiningDate",
                (object?)employee.JoiningDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@Gender",
                (object?)employee.Gender ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@IsActive",
                (object?)employee.IsActive ?? DBNull.Value);

            cmd.Parameters.AddWithValue(
                "@DepartmentId",
                (object?)employee.DepartmentId ?? DBNull.Value);

            await con.OpenAsync();

            int rowsAffected =
                await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                return NotFound("Employee not found.");

            return Ok("Employee updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_DeleteEmployee", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", id);

            await con.OpenAsync();

            int rowsAffected =
                await cmd.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return NotFound("Employee not found.");
            }

            return Ok("Employee deleted successfully.");
        }

    }


}
