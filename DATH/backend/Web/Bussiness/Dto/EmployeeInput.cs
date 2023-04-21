using Entities.Enum.User;

namespace Bussiness.Dto
{
    public class EmployeeInput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Code { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthday { get; set; }
        public EmployeeType Type { get; set; }
        public string? Email { get; set; }
        public DateTime JoinDate { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}
