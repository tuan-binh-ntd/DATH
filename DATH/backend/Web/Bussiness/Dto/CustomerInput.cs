using Entities.Enum.User;

namespace Bussiness.Dto
{
    public class CustomerInput
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string? IdNumber { get; set; }
        public string? Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public ICollection<string>? AddressList { get; set; }
    }
}
