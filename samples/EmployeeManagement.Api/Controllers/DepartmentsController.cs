using EmployeeManagement.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly string _connectionString;

        public DepartmentsController(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")!;
        }


        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            List<Department> departments =
                new List<Department>();

            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand("sp_GetDepartments", con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            await con.OpenAsync();

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Department department =
                    new Department
                    {
                        DepartmentId =
                            Convert.ToInt32(
                                reader["DepartmentId"]),

                        DepartmentName =
                            reader["DepartmentName"]
                                .ToString()!
                    };

                departments.Add(department);
            }

            return Ok(departments);
        }
    }
}