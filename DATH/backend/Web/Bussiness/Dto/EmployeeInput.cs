using Entities.Enum.User;
using System.ComponentModel.DataAnnotations;

namespace Bussiness.Dto
{
    public class EmployeeInput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? IdNumber { get; set; }
        public string? Phone { get; set; }
    }
}
