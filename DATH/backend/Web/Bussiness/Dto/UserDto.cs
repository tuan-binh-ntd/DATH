using Entities;
using Entities.Enum.User;

namespace Bussiness.Dto
{
    public class UserDto : EntityDto<long>
    {
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string? Address { get; set; }
        public string? IdNumber { get; set; }
        public string? Phone { get; set; }
        public DateTime Birthday { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Token { get; set; }
        public UserType Type { get; set; }
    }
}
