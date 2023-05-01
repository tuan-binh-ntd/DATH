using Entities.Enum.User;
using Entities;

namespace Bussiness.Dto
{
    public class EmployeeForViewDto : EntityDto<long>
    {
        public string? ShopName { get; set; }
        public int? ShopId { get; set; }
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
        public string? Username { get; set; }
    }
}
