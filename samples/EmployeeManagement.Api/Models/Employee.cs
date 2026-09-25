namespace EmployeeManagement.Api.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string? Gender { get; set; }

        public bool IsActive { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
    }
}
