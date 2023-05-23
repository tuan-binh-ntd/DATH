using Entities;

namespace Bussiness.Interface.Core
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
