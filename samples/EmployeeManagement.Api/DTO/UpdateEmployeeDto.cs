namespace EmployeeManagement.Api.DTO
{
    public class UpdateEmployeeDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public decimal? Salary { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public DateTime? JoiningDate { get; set; }

        public string? Gender { get; set; }

        public bool? IsActive { get; set; }

        public int? DepartmentId { get; set; }
    }
}
